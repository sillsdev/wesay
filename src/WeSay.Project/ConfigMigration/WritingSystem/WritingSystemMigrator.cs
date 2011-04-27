using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Palaso.IO;
using Palaso.Reporting;
using Palaso.WritingSystems.Migration;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;

namespace WeSay.Project.ConfigMigration.WritingSystem
{
	public class WritingSystemsMigrator
	{
		private delegate void DelegateThatTouchesFile(string filePath);

		public WritingSystemsMigrator(string projectPath)
		{
			ProjectPath = projectPath;
		}

		protected string ProjectPath { get; private set; }

		public string WritingSystemsPath
		{
			get { return Path.Combine(ProjectPath, "WritingSystems"); }
		}

		public string WritingSystemsOldPrefsFilePath
		{
			get { return Path.Combine(ProjectPath, "WritingSystemPrefs.xml"); }
		}

		public void MigrateIfNeeded()
		{
			var oldMigrator = new WritingSystemPrefsMigrator(WritingSystemsOldPrefsFilePath, OnWritingSystemTagChange);
			oldMigrator.MigrateIfNecassary();
			var ldmlMigrator = new LdmlInFolderWritingSystemRepositoryMigrator(WritingSystemsPath, OnWritingSystemTagChange);
			ldmlMigrator.Migrate();
		}

		public void OnWritingSystemTagChange(IEnumerable<LdmlVersion0MigrationStrategy.MigrationInfo> newToOldTagMap)
		{
			foreach (var map in newToOldTagMap)
			{
				RenameWritingSystemTagInFile(LiftFilePath, "WeSay Dictionary File", (filePath) =>
																					//todo: expand the regular expression here to account for all reasonable patterns
																					FileUtils.GrepFile(
																						filePath,
																						string.Format(
																							@"lang\s*=\s*[""']{0}[""']",
																							Regex.Escape(map.RfcTagBeforeMigration)),
																						string.Format(@"lang=""{0}""",
																									  map.RfcTagAfterMigration)
																						)
					);
			}
		}

		protected string LiftFilePath
		{
			//this code lifted from WeSayWordsProject.GetPathToLiftFileGivenProjectDirectory()
			get
			{
				//first, we assume it's based on the name of the directory
				var path = Path.Combine(ProjectPath,
											   Path.GetFileName(ProjectPath) + ".lift");

				//if that doesn't give us one, then we find one which has a matching wesayconfig file
				if (!File.Exists(path))
				{
					foreach (var liftPath in Directory.GetFiles(ProjectPath, "*.lift"))
					{
						if (File.Exists(liftPath.Replace(".lift", ".WeSayConfig")))
						{
							return liftPath;
						}
					}
#if mono    //try this too(probably not needed...)
				//anyhow remember case is sensitive, and a simpe "tolower"
				//doens't cut it because the exists will fail if it's the wrong case (WS-14982)
				foreach (var liftPath in Directory.GetFiles(ProjectDirectoryPath, "*.Lift"))
				{
					if (File.Exists(liftPath.Replace(".Lift", ".WeSayConfig")))
					{
						return liftPath;
					}
				}
#endif
				}
				return path;
			}
		}

		private bool RenameWritingSystemTagInFile(string filePath, string uiFileDescription, DelegateThatTouchesFile doSomething)
		{
			if (!File.Exists(filePath))
				return false;
			try
			{
				doSomething(filePath);
				return true;
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem("Another program has {0} open, so we cannot make the writing system change.  Make sure no other instances of WeSay are running.\n\n\t'{1}'", uiFileDescription, filePath);
				return false;
			}
		}




	}
}
