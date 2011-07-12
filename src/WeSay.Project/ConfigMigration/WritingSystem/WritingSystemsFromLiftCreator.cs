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

		private IEnumerable<string> WritingSystemsInUse()
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

		private void ReplaceWritingSystemId(string oldId, string newId)
		{
			Palaso.IO.FileUtils.GrepFile(_liftFilePath,
			 String.Format(@"lang\s*=\s*[""']{0}[""']", oldId),
			 String.Format(@"lang=""{0}""", newId));
		}

		public void CreateNonExistentWritingSystemsFoundInLift()
		{
			IWritingSystemRepository writingSystemRepository =
				new LdmlInFolderWritingSystemRepository(_writingSystemFolderPath);
			OrphanFinder.FindOrphans(WritingSystemsInUse, ReplaceWritingSystemId, writingSystemRepository);
		}
	}
}
