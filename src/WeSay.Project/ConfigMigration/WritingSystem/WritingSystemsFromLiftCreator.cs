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
		private readonly string _liftFilePath;
		private readonly string _writingSystemFolderPath;

		public WritingSystemsFromLiftCreator(string writingSystemFolderPath, string liftFilePath)
		{
			_writingSystemFolderPath = writingSystemFolderPath;
			_liftFilePath = liftFilePath;
		}

		private IEnumerable<string> WritingSystemsInUse
		{
			get
			{
				var uniqueIds = new List<string>();
				using (var reader = XmlReader.Create(_liftFilePath))
				{
					while (reader.Read())
					{
						if (reader.MoveToAttribute("lang"))
						{
							if (!uniqueIds.Contains(reader.Value))
							{
								uniqueIds.Add(reader.Value);
							}
						}
					}
				}
				return uniqueIds;
			}
		}

		private static string CleanWritingSystemIdIfNecessary(string writingSystemId)
		{
			var rfcTagCleaner = new Rfc5646TagCleaner(writingSystemId);
			rfcTagCleaner.Clean();
			string newId = rfcTagCleaner.GetCompleteTag();
			return newId;
		}

		private void ReplaceWritingSystemId(string oldId, string newId)
		{
			Palaso.IO.FileUtils.GrepFile(_liftFilePath,
			 String.Format(@"lang\s*=\s*[""']{0}[""']", oldId),
			 String.Format(@"lang=""{0}""", newId));
		}

		public void CreateNonExistentWritingSystemsFoundInLift()
		{
			/* Note: This method is identical/copied from ConfigFile.CreateWritingSystemsForIdsInFileWhereNecassary
			 * If an improvement in the algorithm is made here (or over there!) make sure to update the old one.
			 * Maybe somebody should pull this method out into a class or something. */

			var writingSystemRepo = new LdmlInFolderWritingSystemRepository(_writingSystemFolderPath);
			foreach (var wsId in WritingSystemsInUse)
			{
				// Check if it's in the repo
				if (writingSystemRepo.Contains(wsId))
				{
					continue;
				}
				// It's an orphan
				// Clean it
				var conformantWritingSystem = WritingSystemDefinition.Parse(CleanWritingSystemIdIfNecessary(wsId));
				// If it changed, then change
				if (conformantWritingSystem.RFC5646 != wsId)
				{
					conformantWritingSystem = WritingSystemDefinition.CreateCopyWithUniqueId(conformantWritingSystem, WritingSystemsInUse);
					ReplaceWritingSystemId(wsId, conformantWritingSystem.RFC5646);
				}
				// Check if it's in the repo
				if (writingSystemRepo.Contains(conformantWritingSystem.RFC5646))
				{
					continue;
				}
				// It's not in the repo so set it
				writingSystemRepo.Set(conformantWritingSystem);
			}
			writingSystemRepo.Save();
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
