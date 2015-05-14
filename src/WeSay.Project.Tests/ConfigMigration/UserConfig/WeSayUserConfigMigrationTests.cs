using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms.Layout;
using System.Xml.Linq;
using Palaso.TestUtilities;
using SIL.LexiconUtils;
using SIL.WritingSystems;
using SIL.WritingSystems.Tests;
using WeSay.Project.ConfigMigration.UserConfig;
using NUnit.Framework;

namespace WeSay.Project.Tests.ConfigMigration.UserConfig
{
	[TestFixture]
	public class WeSayUserConfigMigrationTests
	{

		[Test]
		public void Migrate_1To2_MigratesToVersion2WithChangedLanguage()
		{
			string v1 = @"<?xml version='1.0' encoding='utf-8'?>
<configuration version='1'>
  <backupPlan />
  <uiOptions>
	<language>wesay-th</language>
	<labelFontName>Angsana New</labelFontName>
	<labelFontSizeInPoints>18</labelFontSizeInPoints>
  </uiOptions>
</configuration>
".Replace("'", "\"");

			using (var folder = new TemporaryFolder("WeSayUserConfigMigrationTests"))
			{
				using (var file = folder.GetNewTempFile(false))
				{
					File.WriteAllText(file.Path, v1);
					var migrator = new WeSayUserConfigMigrator(file.Path, null);
					migrator.MigrateIfNeeded();
					AssertThatXmlIn.File(file.Path).HasAtLeastOneMatchForXpath("configuration[@version='3']");
					AssertThatXmlIn.File(file.Path).HasAtLeastOneMatchForXpath("configuration/uiOptions[language='th']");
				}
			}
		}

		[Test]
		public void Migrate_2To3_MigratesToVersion3WithoutKeyboards()
		{
			string v2 = @"<?xml version='1.0' encoding='utf-8'?>
<configuration version='2'>
	<backupPlan />
	<uiOptions>
		<language>th</language>
		<labelFontName>Angsana New</labelFontName>
		<labelFontSizeInPoints>18</labelFontSizeInPoints>
	</uiOptions>
	<keyboards>
		<keyboard
			ws='ja'
			layout='Thai Kedmanee'
			locale='th-TH' />
	</keyboards>
</configuration>
".Replace("'", "\"");

			using (var folder = new TemporaryFolder("WeSayUserConfigMigrationTests"))
			{
				var filename = Path.Combine(folder.Path, System.Environment.UserName + ".WeSayUserConfig");
				File.WriteAllText(filename, v2);
				var settingsStore = new MemorySettingsStore();
				var userSettingsDataMapper = new UserLexiconSettingsWritingSystemDataMapper(settingsStore);
				ICustomDataMapper<WritingSystemDefinition>[] customDataMapper =
				{
					userSettingsDataMapper
				};
				LdmlInFolderWritingSystemRepository wsRepo = LdmlInFolderWritingSystemRepository.Initialize(folder.Path,
					customDataMapper);

				var ws = new WritingSystemDefinition("ja");
				wsRepo.Set(ws);
				wsRepo.Save();

				var migrator = new WeSayUserConfigMigrator(filename, wsRepo);
				migrator.MigrateIfNeeded();
				AssertThatXmlIn.File(filename).HasAtLeastOneMatchForXpath("configuration[@version='3']");
				AssertThatXmlIn.File(filename).HasAtLeastOneMatchForXpath("configuration[not(keyboards)]");

				// Keyboard info is migrated to user config shared settings file
				//AssertThatXmlIn.File(userConfigSettingsFilename).HasAtLeastOneMatchForXpath("LexiconUserSettings/WritingSystems/WritingSystem[@id='ja']/LocalKeyboard[value()='th-TH_Thai Kedmanee']");
				Assert.That(settingsStore.SettingsElement, Is.EqualTo(XElement.Parse(
@"<LexiconUserSettings>
<WritingSystems>
	<WritingSystem id=""ja"">
	<LocalKeyboard>th-TH_Thai Kedmanee</LocalKeyboard>
	<KnownKeyboards>
	<KnownKeyboard id=""th-TH_Thai Kedmanee"" />
	</KnownKeyboards>
	</WritingSystem>
	</WritingSystems>
</LexiconUserSettings>")).Using((IEqualityComparer<XNode>)new XNodeEqualityComparer()));
			}
		}
	}
}
