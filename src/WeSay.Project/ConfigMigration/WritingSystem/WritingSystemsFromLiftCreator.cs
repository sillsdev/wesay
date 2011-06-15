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
				var writingSystems = new LdmlInFolderWritingSystemRepository(pathToLdmlWritingSystemsFolder);
				var uniqueIdsInFile = new List<string>();
				using (var reader = XmlReader.Create(pathToLiftFile))
				{
					while (reader.Read())
					{
						if (reader.MoveToAttribute("lang"))
						{
							if (!uniqueIdsInFile.Contains(reader.Value))
							{
								uniqueIdsInFile.Add(reader.Value);
							}
						}
					}
				}

				foreach (string idInFile in uniqueIdsInFile)
				{
					string newId = idInFile;
					if(!(idInFile.StartsWith("x", StringComparison.OrdinalIgnoreCase) && writingSystems.Contains(idInFile)))
					{
						var tagCleaner = new Rfc5646TagCleaner(idInFile);
						tagCleaner.Clean();
						newId = tagCleaner.GetCompleteTag();
					}

					if (newId != idInFile)
					{
						if (uniqueIdsInFile.Contains(newId)) // check for writing system id collision (rare)
						{
							newId = MakeUniqueTag(newId, uniqueIdsInFile);
						}
						Palaso.IO.FileUtils.GrepFile(pathToLiftFile,
												 String.Format(@"lang\s*=\s*[""']{0}[""']", idInFile),
												 String.Format(@"lang=""{0}""", newId));
					}
					if(!writingSystems.Contains(newId))
					{
						writingSystems.Set(WritingSystemDefinition.Parse(newId));
					}
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
