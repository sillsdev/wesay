using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Palaso.Services.Dictionary;
using Palaso.Text;
using Palaso.UI.WindowsForms.i8n;
using WeSay.App.Services;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.App
{

	public class DictionaryServiceProvider : MarshalByRefObject, Palaso.Services.Dictionary.IDictionaryServiceBase, IDisposable
	{
		private readonly WeSayWordsProject _project;
		List<int> _registeredClientProcessIds;
		private WeSayApp _app;
		private int _maxNumberOfEntriesToReturn = 20;
		public event EventHandler LastClientDeregistered;
		private WeSay.App.Services.HtmlArticleMaker _articleMaker;
		private SynchronizationContext _uiSynchronizationContext;

		public DictionaryServiceProvider(WeSayApp app, WeSayWordsProject project)
		{
			_app = app;
			_project = project;
			_registeredClientProcessIds = new List<int>();

			_articleMaker = new HtmlArticleMaker(_project.LocateFile("WritingSystemPrefs.xml"),
										_project.LocateFile("PartsOfSpeech.xml"));
		}

		public SynchronizationContext UiSynchronizationContext
		{
			get { return _uiSynchronizationContext; }
			set { _uiSynchronizationContext = value; }
		}

		#region IDictionaryService Members



		public FindResult GetMatchingEntries(string writingSystemId, string form, string findMethod)
		{
			FindMethods method;
			try
			{
				method = (FindMethods) Enum.Parse(typeof (FindMethods), findMethod);
			}
			catch(ArgumentException e)
			{
				throw new ArgumentException("'" + findMethod + "' is not a recognized find method.", e);
			}

			//in case something goes wrong
			FindResult r = new FindResult();
			try
			{

				Palaso.Reporting.Logger.WriteMinorEvent("GetIdsOfMatchingEntries({0},{1},{2})", writingSystemId, form,
														method.ToString());
				if (!_project.WritingSystems.ContainsKey(writingSystemId))
				{
					return r;
				}
				WritingSystem ws = _project.WritingSystems[writingSystemId];

				IList<LexEntry> matches;
				switch (method)
				{
					case FindMethods.Exact:
						matches = Lexicon.GetEntriesHavingLexicalForm(form, ws);
						break;

					default:
					case FindMethods.DefaultApproximate:
						matches = Lexicon.GetEntriesWithSimilarLexicalForms(form, ws,
																			ApproximateMatcherOptions.
																				IncludePrefixedAndNextClosestForms,
																				_maxNumberOfEntriesToReturn);
						break;
				}
				r.ids = new string[matches.Count];
				r.forms = new string[matches.Count];
				int i = 0;
				foreach (LexEntry entry in matches)
				{
					if(i == _maxNumberOfEntriesToReturn)
					{
						break;
					}
					r.forms[i] = entry.LexicalForm.GetBestAlternative(writingSystemId);
					r.ids[i] = entry.Id;
					i++;
				}
			}
			catch (Exception e)
			{
				Palaso.Reporting.Logger.WriteEvent("Error from dictionary services, RegisterClient: " + e.Message);
				Debug.Fail(e.Message);
			}
			return r;
		}

		public string GetHtmlForEntries(string[] entryIds)
		{
			if (UiSynchronizationContext != null)
			{
				string result = string.Empty;
				UiSynchronizationContext.Send(delegate { result = GetHtmlForEntriesCore(entryIds); }, null);
				return result;
			}
			else
			{
				return GetHtmlForEntriesCore(entryIds);
			}
		}

		private string GetHtmlForEntriesCore(string[] entryIds)
		{
			try
			{
				Palaso.Reporting.Logger.WriteMinorEvent("GetHtmlForEntries()");
				StringBuilder builder = new StringBuilder();
				LiftExporter exporter = new LiftExporter(builder, true);
				IHomographCalculator homographCalculator = new HomographCalculator(Lexicon.RecordListManager, _project.DefaultPrintingTemplate.HeadwordWritingSytem);
				Db4oLexEntryFinder finder = new Db4oLexEntryFinder(Lexicon.RecordListManager.DataSource);
				exporter.SetUpForPresentationLiftExport(_project.DefaultPrintingTemplate, homographCalculator, finder);

				foreach (string entryId in entryIds)
				{
					LexEntry entry = Lexicon.FindFirstLexEntryMatchingId(entryId);
					if (entry == null)
					{
						builder.AppendFormat("Error: entry '{0}' not found.", entryId);
					}
					else
					{
						exporter.Add(entry);
					}
				}
				exporter.End();
				return _articleMaker.GetHtmlFragment(builder.ToString());
			}
			catch (Exception e)
			{
				Palaso.Reporting.Logger.WriteEvent("Error from dictionary services, GetHtmlForEntries: " + e.Message);
				Debug.Fail(e.Message);
			}
			return StringCatalog.Get("~Program Error", "This is what we call it when something goes wrong but it's the computer's fault, not the user's.");
		}

		public void RegisterClient(int clientProcessId)
		{
			RunInSafeContext(delegate
			 {
				 try
				 {
					 Palaso.Reporting.Logger.WriteMinorEvent(
						 "dictionary services registering client {0}", clientProcessId);
					 Debug.Assert(!_registeredClientProcessIds.Contains(clientProcessId),
								  "Warning: clientProcessId already registered once.");
					 if (!_registeredClientProcessIds.Contains(clientProcessId))
					 {
						 Palaso.Reporting.Logger.WriteMinorEvent("Registering Service Client {0}",
																 clientProcessId);
						 _registeredClientProcessIds.Add(clientProcessId);
					 }
				 }
				 catch (Exception e)
				 {
					 Palaso.Reporting.Logger.WriteEvent(
						 "Error from dictionary services, RegisterClient: " + e.Message);
					 Debug.Fail(e.Message);
				 }
			 });
		}

		public void DeregisterClient(int clientProcessId)
		{
			RunInSafeContext(delegate
			 {
				 try
				 {
					 Palaso.Reporting.Logger.WriteMinorEvent(
						 "dictionary services deregistering client {0}", clientProcessId);
					 Debug.Assert(_registeredClientProcessIds.Contains(clientProcessId),
								  "Warning: clientProcessId was not in list of clients.");
					 if (_registeredClientProcessIds.Contains(clientProcessId))
					 {
						 Palaso.Reporting.Logger.WriteMinorEvent(
							 "Deregistering Service Client {0}", clientProcessId);
						 _registeredClientProcessIds.Remove(clientProcessId);
					 }
					 if (_registeredClientProcessIds.Count == 0 &&
						 LastClientDeregistered != null)
					 {
						 LastClientDeregistered.Invoke(this, null);
					 }
				 }
				 catch (Exception e)
				 {
					 Palaso.Reporting.Logger.WriteEvent(
						 "Error from dictionary services, DeregisterClient: " +
						 e.Message);
					 Debug.Fail(e.Message);
				 }
			 });
		}

		public void JumpToEntry(string entryId)
		{
			ShowUIWithUrl(entryId);
		}

		/// <summary>
		/// Add a new entry to the lexicon
		/// </summary>
		/// <returns>the id that was assigned to the new entry</returns>
		public string AddEntry(string lexemeFormWritingSystemId, string lexemeForm, string definitionWritingSystemId,
							   string definition, string exampleWritingSystemId, string example)
		{
			if (UiSynchronizationContext != null)
			{
				string result=string.Empty;
				UiSynchronizationContext.Send(
					delegate
						{
							result = AddEntryCore(lexemeFormWritingSystemId, lexemeForm, definitionWritingSystemId,
												definition, exampleWritingSystemId, example);
						},null);
				return result;
			}
			else
			{
				return AddEntryCore(lexemeFormWritingSystemId, lexemeForm, definitionWritingSystemId,
							 definition, exampleWritingSystemId, example);
			}
		}

		public string AddEntryCore(string lexemeFormWritingSystemId, string lexemeForm, string definitionWritingSystemId,
							   string definition, string exampleWritingSystemId, string example)
		{
			if (string.IsNullOrEmpty(lexemeForm ))
			{
				Palaso.Reporting.Logger.WriteEvent("Dictionary Services AddEntry() called with Empty lexemeform");
				return null;
			}
			if( !CheckWritingSystemAndContentsPair(lexemeFormWritingSystemId, lexemeForm))
				return null;
			 if( !CheckWritingSystemAndContentsPair(definitionWritingSystemId, definition))
				return null;
		   if( !CheckWritingSystemAndContentsPair(exampleWritingSystemId, example))
				return null;

			Palaso.Reporting.Logger.WriteEvent("dictionary services.AddEntry()");

			LexEntry e = Lexicon.AddNewEntry();
			e.LexicalForm.SetAlternative(lexemeFormWritingSystemId, lexemeForm);

			LexSense sense = null;
			if (!string.IsNullOrEmpty(definition))
			{
				sense = (LexSense) e.Senses.AddNew();
				sense.Definition.SetAlternative(definitionWritingSystemId, definition);
			}
			if (!string.IsNullOrEmpty(example))
			{
				if (sense == null)
				{
					sense = (LexSense) e.Senses.AddNew();
				}
				LexExampleSentence ex= (LexExampleSentence) sense.ExampleSentences.AddNew();
				ex.Sentence.SetAlternative(exampleWritingSystemId, example);
			}
			return e.Id;
		}

		private bool CheckWritingSystemAndContentsPair(string writingSystemId, string form)
		{
			if (!string.IsNullOrEmpty(form))
			{
				if (!_project.WritingSystems.ContainsKey(writingSystemId))
				{
					Palaso.Reporting.Logger.WriteEvent("Dictionary Services given unknown writing system id: '{0}'", writingSystemId);
					return false;
				}
			}
			return true;
		}

		public string GetCurrentUrl()
		{
			return _app.CurrentUrl;
		}
		private void RunInSafeContext(SendOrPostCallback core)
		{
			if (UiSynchronizationContext != null)
			{
				UiSynchronizationContext.Send(
					core, null);
			}
			else
			{
				core.Invoke(null);
			}
		}

		public void ShowUIWithUrl(string url)
		{
			RunInSafeContext(delegate
								 {
									 try
									 {
										 Palaso.Reporting.Logger.WriteMinorEvent("dictionary services ShowUIWithUrl({0})", url);
										 _app.GoToUrl(url);
									 }
									 catch (Exception e)
									 {
										 Palaso.Reporting.Logger.WriteEvent("Error from dictionary services, ShowUIWithUrl{0}: {1}", url, e.Message);
										 //url navigation errors are handled/reported at the point of failure
									 }
								 });
		}

		public bool IsInServerMode()
		{
			return _app.IsInServerMode;
		}

	   /*worked fine, but trimmed from the service until need is demonstrated
		* public string[] GetFormsFromIds(string writingSytemId, string[] ids)
		{
			if (string.IsNullOrEmpty(writingSytemId))
			{
				return null;
			}
			List<string> forms = new List<string>(ids.Length);
			foreach (string id in ids)
			{
				LexEntry entry = Lexicon.FindFirstLexEntryMatchingId(id);
				if (entry == null)
				{
					forms.Add(string.Empty);
				}
				else
				{
					//nb: we want to add this, even it is empty
					forms.Add(entry.LexicalForm.GetExactAlternative(writingSytemId));
				}
			}
			Debug.Assert(forms.Count == ids.Length, "These must be the same, as the receiver expects them to be aligned.");
			return forms.ToArray();
		}*/


		#endregion

		#region IDisposable Members

		public void Dispose()
		{

		}

		#endregion

		public bool Ping()
		{
			return true;
		}
	}
}
