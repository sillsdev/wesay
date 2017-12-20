using System.IO;
using NUnit.Framework;
using SIL.Reporting;
using SIL.WritingSystems;
using SIL.TestUtilities;
using WeSay.Project;

namespace WeSay.ConfigTool.Tests
{
	[TestFixture]
	public class MigrationTests
	{
		private class EnvironmentForTests : TemporaryFolder
		{
			public EnvironmentForTests(string path) :
				base(path)
			{
				ErrorReport.IsOkToInteractWithUser = false;
				WeSayWordsProject.PreventBackupForTests = true;
				Sldr.Initialize(true);
			}

			public void CreateDefaultProject()
			{
				new WeSayWordsProject();
				WeSayWordsProject.CreateEmptyProjectFiles(Path, WellKnownSubtags.UnlistedLanguage);
				WeSayWordsProject.Project.LoadFromProjectDirectoryPath(Path);
				WeSayWordsProject.Project.Save();
			}

			public string GetUserConfigFilePath()
			{
				var files = Directory.GetFiles(Path, "*.WeSayUserConfig");
				return files[0];
			}

			public void WriteUserConfig(string s)
			{
				File.WriteAllText(GetUserConfigFilePath(), s);
			}

			public new void Dispose()
			{
				base.Dispose();
				Sldr.Cleanup();
			}

		}

		[Test]
		public void MigrateUserConfig_WithV1_DoesMigrate()
		{
			string v1 =
				@"<?xml version='1.0' encoding='utf-8'?>
<configuration version='1'>
  <uiOptions>
	<language>wesay-th</language>
	<labelFontName>Segoe UI</labelFontName>
	<labelFontSizeInPoints>9</labelFontSizeInPoints>
  </uiOptions>
</configuration>
".Replace("'", "\"");

			using (var e = new EnvironmentForTests("ConfigToolMigrationTests"))
			{
				e.CreateDefaultProject();
				e.WriteUserConfig(v1);
				new WeSayWordsProject();
				WeSayWordsProject.Project.LoadFromProjectDirectoryPath(e.Path);
				AssertThatXmlIn.File(e.GetUserConfigFilePath()).HasAtLeastOneMatchForXpath("configuration[@version='3']");

			}
		}
	}
}
