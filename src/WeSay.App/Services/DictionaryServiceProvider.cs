using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Palaso.Services.Dictionary;
using Palaso.UI.WindowsForms.i8n;
using WeSay.App.Services;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.App.Services
{
	public class DictionaryServiceProvider : MarshalByRefObject, IDictionaryServiceBase, IDisposable
	{
		private readonly WeSayWordsProject _project;
		readonly List<int> _registeredClientProcessIds;
		private readonly WeSayApp _app;
		private const int _maxNumberOfEntriesToReturn = 20;
		public event EventHandler LastClientDeregistered;
		private readonly HtmlArticleMaker _articleMaker;
		private SynchronizationContext _uiSynchronizationContext;
		private readonly LexEntryRepository _lexEntryRepository;
		public DictionaryServiceProvider(LexEntryRepository lexEntryRepository, WeSayApp app, WeSayWordsProject project)
		{
			this._app = app;
			this._project = project;
			this._registeredClientProcessIds = new List<int>();
			this._lexEntryRepository = lexEntryRepository;
			this._articleMaker = new HtmlArticleMaker(this._project.LocateFile("WritingSystemPrefs.xml"),
												 this._project.LocateFile("PartsOfSpeech.xml"));
		}

		public SynchronizationContext UiSynchronizationContext
		{
			get { return this._uiSynchronizationContext; }
			set { this._uiSynchronizationContext = value; }
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
				if (!this._project.WritingSystems.ContainsKey(writingSystemId))
				{
					return r;
				}
				WritingSystem ws = this._project.WritingSystems[writingSystemId];

				IList<RecordToken> matches;
				switch (method)
				{
					case FindMethods.Exact:
						matches = this._lexEntryRepository.GetEntriesWithMatchingLexicalForm(form, ws);
						break;

					default:
					case FindMethods.DefaultApproximate:
						matches = this._lexEntryRepository.GetEntriesWithSimilarLexicalForm(form, ws,
																					   ApproximateMatcherOptions.
																							   IncludePrefixedAndNextClosestForms);
						break;
				}
				r.ids = new string[matches.Count];
				r.forms = new string[matches.Count];
				int i = 0;
				foreach (RecordToken token in matches)
				{
					LexEntry entry = this._lexEntryRepository.GetItem(token);
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

		private string GetHtmlForEntriesCore(IEnumerable<string> entryIds)
		{
			try
			{
				Palaso.Reporting.Logger.WriteMinorEvent("GetHtmlForEntries()");
				StringBuilder builder = new StringBuilder();
				LiftExporter exporter = new LiftExporter(builder, true, this._lexEntryRepository);
				exporter.SetUpForPresentationLiftExport(this._project.DefaultPrintingTemplate);

				foreach (string entryId in entryIds)
				{
					LexEntry entry = this._lexEntryRepository.GetLexEntryWithMatchingId(entryId);
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
				return this._articleMaker.GetHtmlFragment(builder.ToString());
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
									 Debug.Assert(!this._registeredClientProcessIds.Contains(clientProcessId),
												  "Warning: clientProcessId already registered once.");
									 if (!this._registeredClientProcessIds.Contains(clientProcessId))
									 {
										 Palaso.Reporting.Logger.WriteMinorEvent("Registering Service Client {0}",
																				 clientProcessId);
										 this._registeredClientProcessIds.Add(clientProcessId);
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
									 Debug.Assert(this._registeredClientProcessIds.Contains(clientProcessId),
												  "Warning: clientProcessId was not in list of clients.");
									 if (this._registeredClientProcessIds.Contains(clientProcessId))
									 {
										 Palaso.Reporting.Logger.WriteMinorEvent(
												 "Deregistering Service Client {0}", clientProcessId);
										 this._registeredClientProcessIds.Remove(clientProcessId);
									 }
									 if (this._registeredClientProcessIds.Count == 0 &&
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

			LexEntry e = this._lexEntryRepository.CreateItem();
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
			this._lexEntryRepository.SaveItem(e);
			return e.Id;
		}

		private bool CheckWritingSystemAndContentsPair(string writingSystemId, string form)
		{
			if (!string.IsNullOrEmpty(form))
			{
				if (!this._project.WritingSystems.ContainsKey(writingSystemId))
				{
					Palaso.Reporting.Logger.WriteEvent("Dictionary Services given unknown writing system id: '{0}'", writingSystemId);
					return false;
				}
			}
			return true;
		}

		public string GetCurrentUrl()
		{
			return this._app.CurrentUrl;
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
									 this._app.GoToUrl(url);
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
			return this._app.IsInServerMode;
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