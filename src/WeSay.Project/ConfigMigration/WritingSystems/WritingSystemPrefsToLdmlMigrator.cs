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

		public Dictionary<string, string> MigrateIfNeeded()
		{
			string pathToWritingSystemPrefsFile = BasilProject.GetPathToWritingSystemPrefs(_pathToProjectFolder);
			Dictionary<string, string> oldIdToNewIdMap = new Dictionary<string, string>();
			if (File.Exists(pathToWritingSystemPrefsFile))
			{
				string pathToLdmlWritingSystemsFolder = BasilProject.GetPathToLdmlWritingSystemsFolder(_pathToProjectFolder);
				WritingSystemCollection writingSystems = new WritingSystemCollection();
				writingSystems.LoadFromLegacyWeSayFile(pathToWritingSystemPrefsFile);
				oldIdToNewIdMap = MapOldIdsToNewIdsInProjectFiles(pathToWritingSystemPrefsFile);
				writingSystems.Write(pathToLdmlWritingSystemsFolder);
				File.Delete(pathToWritingSystemPrefsFile);
			}
			return oldIdToNewIdMap;
		}

		private Dictionary<string, string> MapOldIdsToNewIdsInProjectFiles(string pathToWritingSystemPrefsFile)
		{
			Dictionary<string, string> oldidToNewIdmap = GetMapOfOldIdsToNewIds(pathToWritingSystemPrefsFile);

			foreach (KeyValuePair<string, string> oldIdNewIdPair in oldidToNewIdmap)
			{
				if (!WeSayWordsProject.Project.ChangeWritingSystemIdsInLiftFile(oldIdNewIdPair.Key, oldIdNewIdPair.Value)) { throw new ApplicationException(String.Format("An error occured while migrating the writing system {0} to {1} in your lift file.", oldIdNewIdPair.Key, oldIdNewIdPair.Value)); }
			}
			return oldidToNewIdmap;
		}

		private Dictionary<string, string> GetMapOfOldIdsToNewIds(string pathToWritingSystemPrefsFile)
		{
			Dictionary<string, string> oldIdToNewIdMap = new Dictionary<string, string>();
			using(XmlReader reader = XmlReader.Create(pathToWritingSystemPrefsFile))
			{
				while(reader.Read())
				{
					if(reader.Name == "WritingSystem")
					{
						string oldId = "";
						bool isAudio = false;

						while(!(reader.Name == "WritingSystem" && reader.NodeType == XmlNodeType.EndElement))
						{
							reader.Read();
							if(reader.Name == "Id")
							{
								oldId = reader.ReadString();
							}
							else if(reader.Name == "IsAudio")
							{
								isAudio = Convert.ToBoolean(reader.ReadString());
							}
						}
						string newId = new WritingSystem() { ISO = oldId, IsAudio = isAudio }.Id;
						if (oldId != newId)
						{
							oldIdToNewIdMap.Add(oldId, newId);
						}
					}
				}
			}
			return oldIdToNewIdMap;
		}
	}
}