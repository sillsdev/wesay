using System;
using System.IO;
using System.Text;
using System.Xml;
using Chorus.VcsDrivers.Mercurial;
using NUnit.Framework;
using SIL.TestUtilities;
using SIL.Progress;
using SIL.Xml;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class ChorusBackupMakerTests
	{
		class BackupScenario : IDisposable
		{
			private readonly ProjectDirectorySetupForTesting _projDir;
			private readonly TemporaryFolder _backupDir;
			private readonly ChorusBackupMaker _backupMaker;

			public BackupScenario(string testName)
			{
				SIL.Reporting.ErrorReport.IsOkToInteractWithUser = false;
				_projDir = new ProjectDirectorySetupForTesting("");

				_backupMaker = new ChorusBackupMaker(new CheckinDescriptionBuilder());
				_backupDir = new TemporaryFolder(testName);

				_backupMaker.PathToParentOfRepositories = _backupDir.Path;

			}

			private string PathToBackupProjectDir
			{
				get { return Path.Combine(_backupDir.Path, _projDir.ProjectDirectoryName);}
			}

			private ChorusBackupMaker BackupMaker
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
				BackupMaker.BackupNow(SourceProjectDir, "en", _projDir.PathToLiftFile);
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
				var  r = new HgRepository(PathToBackupProjectDir, new NullProgress());
				Assert.IsTrue(r.GetFileExistsInRepo(s));
			}
			public void AssertFileDoesNotExistInRepo(string s)
			{
				var r = new HgRepository(PathToBackupProjectDir, new NullProgress());
				Assert.IsFalse(r.GetFileExistsInRepo(s));
			}
			public void MakeTestLiftFile(string filename)
			{
				StringBuilder builder = new StringBuilder();
				int numberOfTestLexEntries = 50;
				for (int i = 0; i < numberOfTestLexEntries; i++)
				{
					builder.AppendFormat(@"
				<entry id='{0}'>
					<lexical-unit>
					  <form lang='qaa-x-qaa'>
						<text>{0}</text>
					  </form>
					</lexical-unit>
					<sense>
						<grammatical-info value='n'/>
						<definition><form lang='en'><text>blah blah {0} blah blah</text></form></definition>
						<example lang='qaa-x-qaa'><text>and example of lah blah {0} blah blah</text></example>
					</sense>
				</entry>", i);
				}

				string liftContents =
						  string.Format(
								  "<?xml version='1.0' encoding='utf-8'?><lift version='{0}'>{1}</lift>",
								  .12,
								  builder.ToString());

				File.WriteAllText(Path.Combine(SourceProjectDir, filename), liftContents);
			}
		}

		[Test]
		[Category("Known Mono Issue")]
		public void BackupNow_FirstTime_CreatesValidRepositoryAndWorkingTree()
		{
			using (var scenario = new BackupScenario("BackupNow_NewFolder_CreatesNewRepository"))
			{
				scenario.BackupNow();
				scenario.AssertDirExistsInWorkingDirectory(".hg");
				scenario.AssertFileExistsInRepo("Test.lift");
				scenario.AssertFileExistsInRepo("Test.WeSayConfig");
				scenario.AssertFileExistsInRepo(Path.Combine("WritingSystems", "en.ldml"));
				scenario.AssertFileExistsInWorkingDirectory("Test.lift");
				scenario.AssertFileExistsInWorkingDirectory("Test.WeSayConfig");
				scenario.AssertFileExistsInWorkingDirectory(Path.Combine("WritingSystems", "en.ldml"));
			}
		}

		[Test]
		[Ignore("FLAKY - test sometimes fails first time run in all tests in VS. System.AccessViolationException : Attempted to read or write protected memory. This is often an indication that other memory is corrupt.")]
		public void BackupNow_ExistingRepository_AddsInvalidNewFileToBackupDir()
		{
			// Test causes a crash in WrapShellCall.exe - is there an updated version?
			using (var scenario = new BackupScenario("BackupNow_ExistingRepository_AddsInvalidNewFileToBackupDir"))
			{
				scenario.BackupNow();
				File.Create(Path.Combine(scenario.SourceProjectDir, "blah.lift")).Close();
				scenario.BackupNow();
				scenario.AssertFileDoesNotExistInWorkingDirectory("blah.lift");
				scenario.AssertFileDoesNotExistInRepo("blah.lift");
			}
		}

		[Test]
		[Ignore("FLAKY - test sometimes fails first time run in all tests in VS. System.AccessViolationException : Attempted to read or write protected memory. This is often an indication that other memory is corrupt.")]
		public void BackupNow_ExistingRepository_AddsNewFileToBackupDir()
		{
			// Test causes a crash in WrapShellCall.exe - is there an updated version?
			using (var scenario = new BackupScenario("BackupNow_ExistingRepository_AddsNewFileToBackupDir"))
			{
				scenario.BackupNow();
				scenario.MakeTestLiftFile("blah.lift");
				scenario.BackupNow();
				scenario.AssertFileExistsInWorkingDirectory("blah.lift");
				scenario.AssertFileExistsInRepo("blah.lift");
			}
		}

		[Test]
		public void BackupNow_RemoveFile_RemovedFromBackupDir()
		{
			// Test causes a crash in WrapShellCall.exe - is there an updated version?
			using (var scenario = new BackupScenario("BackupNow_RemoveFile_RemovedFromBackupDir"))
			{
				scenario.MakeTestLiftFile("blah.lift");
				scenario.BackupNow();
				File.Delete(Path.Combine(scenario.SourceProjectDir, "blah.lift"));
				scenario.BackupNow();
				scenario.AssertFileDoesNotExistInWorkingDirectory("blah.lift");
			}
		}

		[Test]
		public void CanSerializeAndDeserializeSettings()
		{
			var b = new ChorusBackupMaker(new CheckinDescriptionBuilder());
			b.PathToParentOfRepositories = @"z:\";
			var builder = new StringBuilder();
			//var dom = new XmlDocument();
			//dom.LoadXml("<foobar><blah></blah></foobar>");

			using (var writer = XmlWriter.Create(builder, CanonicalXmlSettings.CreateXmlWriterSettings()))
			{
				b.Save(writer);
				var dom = new XmlDocument();
				dom.Load(new StringReader(builder.ToString()));
				var loadedGuy = ChorusBackupMaker.CreateFromDom(dom, new CheckinDescriptionBuilder());
				Assert.AreEqual(@"z:\", loadedGuy.PathToParentOfRepositories);

			}
		}

	}

}
