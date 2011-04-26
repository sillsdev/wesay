using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Palaso.IO;
using Palaso.Reporting;
using Palaso.WritingSystems.Migration;

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
			//WritingSystemPrefsMigrator oldMigrator = new WritingSystemPrefsMigrator()



			//LdmlInFolderWritingSystemRepositoryMigrator _ldmlMigrator;

		}

		public void OnWritingSystemTagChange(string newTag, string oldTag)
		{
			RenameWritingSystemTagInFile(LiftFilePath, "WeSay Dictionary File", (filePath) =>
				//todo: expand the regular expression here to account for all reasonable patterns
				FileUtils.GrepFile(
					filePath,
					string.Format(@"lang\s*=\s*[""']{0}[""']", Regex.Escape(oldTag)),
					string.Format(@"lang=""{0}""", newTag)
				)
			);

			//DefaultViewTemplate.OnWritingSystemIDChange(oldTag, ws.Id);
		}

		protected string LiftFilePath
		{
			get { throw new NotImplementedException(); }
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
