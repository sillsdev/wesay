using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using Palaso.DictionaryService.Client;
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
			Palaso.Reporting.Logger.WriteMinorEvent("GetIdsOfMatchingEntries({0},{1},{2})", writingSystemId,form, method.ToString());
			if (!_project.WritingSystems.ContainsKey(writingSystemId))
			{
				return new string[0];
			}
			WritingSystem ws = _project.WritingSystems[writingSystemId];
			List<LexEntry> matches = Lexicon.GetEntriesHavingLexicalForm(form,ws);
			string[] ids = new string[matches.Count];
			int i = 0;
			foreach (LexEntry entry in matches)
			{
				ids[i] = entry.Id;
				i++;
			}
			return ids;
		}

		public string GetHmtlForEntry(string entryId)
		{
			Palaso.Reporting.Logger.WriteMinorEvent("GetHmtlForEntry{0}", entryId);
			LexEntry e = Lexicon.FindFirstLexEntryMatchingId(entryId);
			if(e==null)
			{
				return "Not Found";
			}
			StringBuilder b = new StringBuilder();
			b.AppendFormat("{0}:",e.LexicalForm.GetFirstAlternative());
			foreach (LexSense sense in e.Senses)
			{
				if (sense.Gloss != null)
				{
					b.AppendFormat("{0}", sense.Gloss.GetFirstAlternative());
				}
			}
			b.Append(" (WeSay)");
			return b.ToString();
		}

		public void RegisterClient(int clientProcessId)
		{
			Debug.Assert(!_registeredClientProcessIds.Contains(clientProcessId), "Warning: clientProcessId already registered once.");
			if(!_registeredClientProcessIds.Contains(clientProcessId))
			{
				Palaso.Reporting.Logger.WriteMinorEvent("Registering Service Client {0}", clientProcessId);
				_registeredClientProcessIds.Add(clientProcessId);
			}
		}

		public void DeregisterClient(int clientProcessId)
		{
			Debug.Assert(_registeredClientProcessIds.Contains(clientProcessId), "Warning: clientProcessId was not in list of clients.");
			if(_registeredClientProcessIds.Contains(clientProcessId))
			{
				Palaso.Reporting.Logger.WriteMinorEvent("Deregistering Service Client {0}", clientProcessId);
				_registeredClientProcessIds.Remove(clientProcessId);
			}
			if (_registeredClientProcessIds.Count == 0 && LastClientDeregistered != null)
			{
				LastClientDeregistered.Invoke(this, null);
			}
		}

		public void JumpToEntry(string entryId)
		{

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
