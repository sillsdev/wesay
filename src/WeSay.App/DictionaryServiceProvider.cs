using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using Palaso.DictionaryService.Client;
using Palaso.Text;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.App
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class DictionaryServiceProvider:Palaso.DictionaryService.Client.IDictionaryService, IDisposable
	{
		private readonly WeSayWordsProject _project;
		List<int> _registeredClientProcessIds;
		private WeSayApp _app;
		public event EventHandler LastClientDeregistered;

		public DictionaryServiceProvider(WeSayApp app, WeSayWordsProject project)
		{
			_app = app;
			_project = project;
			_registeredClientProcessIds = new List<int>();
		}

		#region IDictionaryService Members


		public string[] GetIdsOfMatchingEntries(string writingSystemId, string form, FindMethods method)
		{
			try
			{
				Palaso.Reporting.Logger.WriteMinorEvent("GetIdsOfMatchingEntries({0},{1},{2})", writingSystemId, form,
														method.ToString());
				if (!_project.WritingSystems.ContainsKey(writingSystemId))
				{
					return new string[0];
				}
				WritingSystem ws = _project.WritingSystems[writingSystemId];
				List<LexEntry> matches = Lexicon.GetEntriesHavingLexicalForm(form, ws);
				string[] ids = new string[matches.Count];
				int i = 0;
				foreach (LexEntry entry in matches)
				{
					ids[i] = entry.Id;
					i++;
				}
				return ids;
			}
			catch (Exception e)
			{
				Palaso.Reporting.Logger.WriteEvent("Error from dictionary services, RegisterClient: " + e.Message);
				Debug.Fail(e.Message);
			}
			return new string[0];
		}

		public string GetHmtlForEntry(string entryId)
		{
			/**************
			 *
			 *
			 * todo: this is just a proof of concept
			 *
			 *
			 *****************/
			try
			{
				Palaso.Reporting.Logger.WriteMinorEvent("GetHmtlForEntry{0}", entryId);
				LexEntry e = Lexicon.FindFirstLexEntryMatchingId(entryId);
				if (e == null)
				{
					return "Not Found";
				}
				StringBuilder b = new StringBuilder();
				b.AppendFormat("{0}: ", e.LexicalForm.GetFirstAlternative());
				foreach (LexSense sense in e.Senses)
				{
					foreach (LanguageForm form in sense.Gloss)
					{
						b.AppendFormat("{0} ", form.Form);
					}

					foreach (LexExampleSentence o in sense.ExampleSentences)
					{
						foreach (LanguageForm form in o.Sentence.Forms)
						{
							b.AppendFormat("{0} ", form.Form);
						}
					}
				}
				b.Append(" (WeSay)");
				return b.ToString();
			}
			catch (Exception e)
			{
				Palaso.Reporting.Logger.WriteEvent("Error from dictionary services, RegisterClient: " + e.Message);
				Debug.Fail(e.Message);
			}
			return StringCatalog.Get("~Program Error", "This is what we call it when something goes wrong but it's the computer's fault, not the user's.");
		}

		public void RegisterClient(int clientProcessId)
		{
			try
			{
				Palaso.Reporting.Logger.WriteMinorEvent("dictionary services registering client {0}", clientProcessId);
				Debug.Assert(!_registeredClientProcessIds.Contains(clientProcessId),
							 "Warning: clientProcessId already registered once.");
				if (!_registeredClientProcessIds.Contains(clientProcessId))
				{
					Palaso.Reporting.Logger.WriteMinorEvent("Registering Service Client {0}", clientProcessId);
					_registeredClientProcessIds.Add(clientProcessId);
				}
			}
			catch (Exception e)
			{
				Palaso.Reporting.Logger.WriteEvent("Error from dictionary services, RegisterClient: " + e.Message);
				Debug.Fail(e.Message);
			}
		}

		public void DeregisterClient(int clientProcessId)
		{
			try
			{
				Palaso.Reporting.Logger.WriteMinorEvent("dictionary services deregistering client {0}", clientProcessId);
				Debug.Assert(_registeredClientProcessIds.Contains(clientProcessId),
							 "Warning: clientProcessId was not in list of clients.");
				if (_registeredClientProcessIds.Contains(clientProcessId))
				{
					Palaso.Reporting.Logger.WriteMinorEvent("Deregistering Service Client {0}", clientProcessId);
					_registeredClientProcessIds.Remove(clientProcessId);
				}
				if (_registeredClientProcessIds.Count == 0 && LastClientDeregistered != null)
				{
					LastClientDeregistered.Invoke(this, null);
				}
			}
			catch (Exception e)
			{
				Palaso.Reporting.Logger.WriteEvent("Error from dictionary services, DeregisterClient: " + e.Message);
				Debug.Fail(e.Message);
			}
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
				sense.Gloss.SetAlternative(definitionWritingSystemId, definition);
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

		public void ShowUIWithUrl(string url)
		{
			try
			{
				Palaso.Reporting.Logger.WriteMinorEvent("dictionary services ShowUIWithUrl({0})", url);
				_app.GoToUrl(url);
			}
			catch (Exception e)
			{
				Palaso.Reporting.Logger.WriteEvent("Error from dictionary services, ShowUIWithUrl{0}: {1}",url, e.Message);
				//url navigation errors are handled/reported at the point of failure
			}
		}


		public bool IsInServerMode()
		{
			return _app.IsInServerMode;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{

		}

		#endregion
	}
}
