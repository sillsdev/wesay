using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Palaso.Reporting;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration;

namespace WeSay.Project.ConfigMigration.WritingSystem
{
	class WritingSystemsFromLiftCreator
	{
		private readonly string _pathToProjectFolder;

		public WritingSystemsFromLiftCreator(string pathToProjectFolder)
		{
			_pathToProjectFolder = pathToProjectFolder;
		}

		public void CreateNonExistentWritingSystemsFoundInLift(string pathToLiftFile)
		{
			if (File.Exists(pathToLiftFile))
			{
				string pathToLdmlWritingSystemsFolder = BasilProject.GetPathToLdmlWritingSystemsFolder(_pathToProjectFolder);
				IWritingSystemRepository writingSystems = new LdmlInFolderWritingSystemRepository(pathToLdmlWritingSystemsFolder);
				var idsToChange = new Dictionary<string, string>();
				var idsInFile = new List<string>();
				var seenIds = new List<string>();
				using (var reader = XmlReader.Create(pathToLiftFile))
				{
					while (reader.Read())
					{
						if (reader.MoveToAttribute("lang"))
						{
							idsInFile.Add(reader.Value);
						}
					}
				}

				foreach (string idInFile in idsInFile)
				{
					var tagCleaner = new Rfc5646TagCleaner(idInFile);
					tagCleaner.Clean();
					string newId = tagCleaner.GetCompleteTag();

					if (newId != idInFile)
					{
						if (idsInFile.Contains(newId)) // check for writing system id collision (rare)
						{
							newId = MakeUniqueTag(newId, idsInFile);
						}
						idsToChange[idInFile] = newId;
					}
				}

				foreach (var pair in idsToChange)
				{
					string fromId = pair.Key;
					string toId = pair.Value;

					// Update WritingSystems
					if (!writingSystems.Contains(toId))
					{
						writingSystems.Set(WritingSystemDefinition.Parse(toId));
					}

					// Update the LIFT file
					Palaso.IO.FileUtils.GrepFile(pathToLiftFile,
												 String.Format(@"lang\s*=\s*[""']{0}[""']", fromId),
												 String.Format(@"lang=""{0}""", toId));
				}
				writingSystems.Save();
			}
		}

		private static string MakeUniqueTag(string rfcTag, IEnumerable<string> uniqueRfcTags)
		{
			int duplicateNumber = 0;
			string newRfcTag;
			do
			{
				duplicateNumber++;
				newRfcTag = rfcTag + String.Format("-dupl{0}", duplicateNumber);
			} while (uniqueRfcTags.Any(s => s.Equals(newRfcTag, StringComparison.OrdinalIgnoreCase)));
			return newRfcTag;
		}
	}
}
