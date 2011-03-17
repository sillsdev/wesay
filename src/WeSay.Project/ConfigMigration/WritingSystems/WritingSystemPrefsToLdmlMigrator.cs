using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Palaso.IO;
using Palaso.Migration;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project.ConfigMigration.WritingSystems
{
	public class WritingSystemPrefsToLdmlMigrator
	{
		private string _pathToProjectFolder;
		private Dictionary<string, string> _oldIdToNewIdMap;

		public WritingSystemPrefsToLdmlMigrator(string pathToProjectFolder)
		{
			_pathToProjectFolder = pathToProjectFolder;
		}

		public void MigrateIfNeeded()
		{
			if(!WeSayWordsProject.ProjectExists()){return;}  //this is the case for many tests
			string pathToWritingSystemPrefsFile = BasilProject.GetPathToWritingSystemPrefs(_pathToProjectFolder);
			if (File.Exists(pathToWritingSystemPrefsFile))
			{
				string pathToLdmlWritingSystemsFolder = BasilProject.GetPathToLdmlWritingSystemsFolder(_pathToProjectFolder);
				WritingSystemCollection writingSystems = new WritingSystemCollection();
				writingSystems.LoadFromLegacyWeSayFile(pathToWritingSystemPrefsFile);
				MapOldIdsToNewIdsInProjectFiles(writingSystems);
				writingSystems.Write(pathToLdmlWritingSystemsFolder);
				File.Delete(pathToWritingSystemPrefsFile);
			}
		}

		private void MapOldIdsToNewIdsInProjectFiles(WritingSystemCollection writingSystems)
		{
			foreach (WritingSystem ws in writingSystems.Values)
			{
					if (!WeSayWordsProject.Project.ChangeWritingSystemIdsInLiftFile(ws.Rfc5646TagOnLoad.CompleteTag,ws.Id))
					{
						throw new ApplicationException(
							String.Format(
								"An error occured while migrating the writing system {0} to {1} in your lift file.",
								ws.Rfc5646TagOnLoad.CompleteTag, ws.Id));
					}
					WeSayWordsProject.Project.ConfigFile.ReplaceWritingSystemId(ws.Rfc5646TagOnLoad.CompleteTag, ws.Id);
			}
		}
	}
}