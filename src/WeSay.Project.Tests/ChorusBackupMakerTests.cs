using System;
using System.IO;
using System.Text;
using System.Xml;
using Chorus.sync;
using NUnit.Framework;
using Palaso.TestUtilities;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class ChorusBackupMakerTests
	{
		class BackupScenario : IDisposable
		{
			private ProjectDirectorySetupForTesting _projDir;
			private TemporaryFolder _backupDir;
			private ChorusBackupMaker _backupMaker;
			public BackupScenario(string testName)
			{
				_projDir = new ProjectDirectorySetupForTesting("");

				_backupMaker = new ChorusBackupMaker(new CheckinDescriptionBuilder());
				_backupDir = new TemporaryFolder(testName);

				_backupMaker.PathToParentOfRepositories = _backupDir.FolderPath;

			}
			public string PathToBackupProjectDir
			{
				get { return Path.Combine(_backupDir.FolderPath, _projDir.ProjectDirectoryName);}
			}

			public ChorusBackupMaker BackupMaker
			{
				get { return _backupMaker; }
			}

			public string SourceProjectDir
			{
				get { return _projDir.PathToDirectory; }
			}

			public void Dispose()
			{
				_projDir.Dispose();
				_backupDir.Dispose();
			}

			public void BackupNow()
			{
				BackupMaker.BackupNow(SourceProjectDir, "en");
			}

			public void AssertDirExistsInWorkingDirectory(string s)
			{
				string  expectedDir = Path.Combine(PathToBackupProjectDir, s);
				Assert.IsTrue(Directory.Exists(expectedDir));
			}

			public void AssertFileExistsInWorkingDirectory(string s)
			{
				string  path = Path.Combine(PathToBackupProjectDir, s);
				Assert.IsTrue(File.Exists(path));
			}

			public void AssertFileDoesNotExistInWorkingDirectory(string s)
			{
				string path = Path.Combine(PathToBackupProjectDir, s);
				Assert.IsFalse(File.Exists(path));
			}

			public void AssertFileExistsInRepo(string s)
			{
				Chorus.sync.RepositoryManager r = new RepositoryManager(PathToBackupProjectDir, new ProjectFolderConfiguration(SourceProjectDir));
				Assert.IsTrue(r.GetFileExistsInRepo(Path.Combine(PathToBackupProjectDir ,"test.lift")));
			}
		}

		[Test]
		[Category("Known Mono Issue")]
		public void BackupNow_FirstTime_CreatesValidRepositoryAndWorkingTree()
		{
			using (BackupScenario scenario = new BackupScenario("BackupNow_NewFolder_CreatesNewRepository"))
			{
				scenario.BackupNow();
				scenario.AssertDirExistsInWorkingDirectory(".hg");
				scenario.AssertFileExistsInRepo("test.lift");
				scenario.AssertFileExistsInRepo("test.WeSayConfig");
				scenario.AssertFileExistsInRepo("WritingSystemPrefs.xml");
				scenario.AssertFileExistsInWorkingDirectory("test.lift");
				scenario.AssertFileExistsInWorkingDirectory("test.WeSayConfig");
				scenario.AssertFileExistsInWorkingDirectory("WritingSystemPrefs.xml");
			}
		}

		[Test]
		public void BackupNow_ExistingRepository_AddsNewFileToBackupDir()
		{
			// Test causes a crash in WrapShellCall.exe - is there an updated version?
			using (BackupScenario scenario = new BackupScenario("BackupNow_ExistingRepository_AddsNewFileToBackupDir"))
			{
				scenario.BackupNow();
				File.Create(Path.Combine(scenario.SourceProjectDir, "blah.foo")).Close();
				scenario.BackupNow();
				scenario.AssertFileExistsInWorkingDirectory("blah.foo");
				scenario.AssertFileExistsInRepo("blah.foo");
			}
		}

		[Test]
		public void BackupNow_RemoveFile_RemovedFromBackupDir()
		{
			// Test causes a crash in WrapShellCall.exe - is there an updated version?
			using (BackupScenario scenario = new BackupScenario("BackupNow_RemoveFile_RemovedFromBackupDir"))
			{
				File.Create(Path.Combine(scenario.SourceProjectDir, "blah.foo")).Close();
				scenario.BackupNow();
				File.Delete(Path.Combine(scenario.SourceProjectDir, "blah.foo"));
				scenario.BackupNow();
				scenario.AssertFileDoesNotExistInWorkingDirectory("blah.foo");
			}
		}

		[Test]
		public void CanSerializeAndDeserializeSettings()
		{
			ChorusBackupMaker b = new ChorusBackupMaker(new CheckinDescriptionBuilder());
			b.PathToParentOfRepositories = @"z:\";
			StringBuilder builder = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(builder))
			{
				b.Save(writer);
				using (XmlReader reader = XmlReader.Create(new StringReader(builder.ToString())))
				{
					ChorusBackupMaker loadedGuy = ChorusBackupMaker.LoadFromReader(reader, new CheckinDescriptionBuilder());
					Assert.AreEqual(@"z:\", loadedGuy.PathToParentOfRepositories);
				}

			}
		}

	}

}
