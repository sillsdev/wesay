using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Palaso.IO;
using Palaso.Reporting;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration;

namespace WeSay.Project.ConfigMigration.WritingSystem
{
	class WritingSystemsInLiftFileHelper
	{
		private readonly string _liftFilePath;
		private readonly string _writingSystemFolderPath;

		public WritingSystemsInLiftFileHelper(string writingSystemFolderPath, string liftFilePath)
		{
			_writingSystemFolderPath = writingSystemFolderPath;
			_liftFilePath = liftFilePath;
		}

		public IEnumerable<string> WritingSystemsInUse
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

		public void ReplaceWritingSystemId(string oldId, string newId)
		{
			try
			{
				Palaso.IO.FileUtils.GrepFile(_liftFilePath,
				String.Format(@"lang\s*=\s*[""']{0}[""']", Regex.Escape(oldId)),
				String.Format(@"lang=""{0}""", newId));
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem("Another program has WeSay's dictionary file open, so we cannot make the writing system change.  Make sure WeSay isn't running.");
			}


		}

		public void CreateNonExistentWritingSystemsFoundInFile()
		{
			var writingSystemRepository =
				new LdmlInFolderWritingSystemRepository(_writingSystemFolderPath);
			WritingSystemOrphanFinder.FindOrphans(WritingSystemsInUse, ReplaceWritingSystemId, writingSystemRepository);
		}
	}
}
