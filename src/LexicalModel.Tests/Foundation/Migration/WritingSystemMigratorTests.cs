using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation.WritingSystemMigration;

namespace WeSay.LexicalModel.Tests.Foundation.Migration
{
	[TestFixture]
	public class WritingSystemMigratorTests
	{
		private class TestEnvironment : IDisposable
		{
			private IEnumerable<WesayWsPrefsToPalasoWsLdmlMigrationStrategy.MigrationInfo> _oldToNewRfcTagMap;
			private readonly string _pathToWsPrefsFile;
			private readonly TemporaryFolder _pretendProjectDirectory;
			private readonly XmlNamespaceManager _namespaceManager;
			private readonly string _pathToLdmlWsRepo = "";

			public TestEnvironment()
			{
				_pretendProjectDirectory = new TemporaryFolder("PretendWeSayProject");
				_pathToWsPrefsFile = Path.Combine(_pretendProjectDirectory.Path, "WritingSystemPrefs.xml");
				_pathToLdmlWsRepo = Path.Combine(_pretendProjectDirectory.Path, "WritingSystems");
				_namespaceManager = new XmlNamespaceManager(new NameTable());
				NamespaceManager.AddNamespace("palaso", "urn://palaso.org/ldmlExtensions/v1");
			}

			public string PathToWsPrefsFile
			{
				get { return _pathToWsPrefsFile; }
			}

			public XmlNamespaceManager NamespaceManager
			{
				get { return _namespaceManager; }
			}

			public string GetFileForOriginalRfcTag(string oldRfcTag)
			{
				return Path.Combine(_pathToLdmlWsRepo, _oldToNewRfcTagMap.First(info => info.RfcTagAfterMigration == oldRfcTag).RfcTagAfterMigration + ".ldml");
			}

			public void WriteContentToWsPrefsFile(string content)
			{
				File.WriteAllText(_pathToWsPrefsFile, content);
			}

			public void ChangeRfcTags(IEnumerable<WesayWsPrefsToPalasoWsLdmlMigrationStrategy.MigrationInfo> migrationInfo)
			{
				_oldToNewRfcTagMap = migrationInfo;
			}

			public void Dispose()
			{
				if (Directory.Exists(_pathToLdmlWsRepo))
				{
					foreach (var ldmlFile in Directory.GetFiles(_pathToLdmlWsRepo))
					{
						File.Delete(ldmlFile);
					}
					Directory.Delete(_pathToLdmlWsRepo);
				}
				if (File.Exists(_pathToWsPrefsFile))
				{
					File.Delete(_pathToWsPrefsFile);
				}
				if (Directory.Exists(_pathToLdmlWsRepo))
				{
					Directory.Delete(_pretendProjectDirectory.Path);
				}
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingId_IdIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string id = "en-US-fonipa-x-etic";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(id, "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag(id);
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/identity/language[@type='{0}']", id),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingFontName_FontNameIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string fontName = "Arial";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", fontName, 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:defaultFontFamily[@value='{0}']", fontName),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingFontSize_FontSizeIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const int fontSize = 12;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", fontSize, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:defaultFontSize[@value='{0}']", fontSize),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingKeyboardName_KeyboardNameIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string keyboardName = "IPA Unicode 5.1(ver 1.2 US) MSK";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, "", keyboardName, true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:defaultKeyboard[@value='{0}']", keyboardName),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingAbbreviation_AbbreviationIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string abbreviation = "Eng";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", abbreviation,
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:abbreviation[@value='{0}']", abbreviation),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingCustomSimpleSortRules_CustomSortRulesAreInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string sortUsing = "CustomSimple";
				const string sortRules =
@"N n
O o";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "", sortUsing, sortRules, "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath(
					String.Format("/ldml/collations/collation/special/palaso:sortRulesType[@value='{0}']", sortUsing), environment.NamespaceManager
					);
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/collations/collation/rules/p[text()='N'] "); //Only checking one character. Hopefully this means the rest are there too.
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingCustomIcuSortRules_CustomSortRulesAreInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string sortUsing = "CustomICU";
				const string sortRules = "&amp; C &lt; č";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "", sortUsing, sortRules, "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath(
					String.Format("/ldml/collations/collation/special/palaso:sortRulesType[@value='{0}']", sortUsing), environment.NamespaceManager
					);
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/collations/collation/rules/p"); //Only checking one character. Hopefully this means the rest are there too.
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingSortRulesFromOtherLanguage_OtherLanguageIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string otherLanguage = "de";
				const string sortUsing = "";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  sortUsing, otherLanguage, "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/collations/collation/base/alias[@source='{0}']", otherLanguage),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWswithNoSortRuleInfoSpecified_LdmlSpecifiesIdAsLanguageToSortOn()
		{
			using (var environment = new TestEnvironment())
			{
				const string id = "en";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(id, "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/collations/collation/base/alias[@source='{0}']", id),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingIsUnicodeEncodedIsFalse_IsLegacyIsTrueInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const bool isUnicodeEncoded = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, "", "", isUnicodeEncoded, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:isLegacyEncoded[@value='{0}']", !isUnicodeEncoded),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingIsUnicodeEncodedIsTrue_IsLegacyDoesNotAppearInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const bool isUnicodeEncoded = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, "", "", isUnicodeEncoded, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasNoMatchForXpath(
					"/ldml/special/palaso:isLegacyEncoded",
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingRightToLeftIsTrue_CharacterOrientationIsMarkedRightToLeftInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const bool rightToleft = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, rightToleft, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath("/ldml/layout/orientation[@characters='right-to-left']");
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingRightToLeftIsFalse_CharacterOrientationIsnotContainedInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const bool rightToleft = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, rightToleft, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasNoMatchForXpath("/ldml/layout/orientation/@characters");
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsSpellCheckingId_SpellCheckingIdIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string spellCheckingId = "spell";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, spellCheckingId, "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:spellCheckingId[@value='{0}']", spellCheckingId),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsRfcTagThatChangesOnMigration_MigrationDelegateIsCalled()
		{
			using (var environment = new TestEnvironment())
			{
				const string language = "en";
				const bool isAudio = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(language, language,
														  "", "", "", 0, false, "", "", true, isAudio)
					);
				bool delegateCalledCorrectly = false;
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					delegate(IEnumerable<WesayWsPrefsToPalasoWsLdmlMigrationStrategy.MigrationInfo> oldToNewRfcTagsMap)
						{
							if(oldToNewRfcTagsMap.
								First(info => info.RfcTagAfterMigration == "en").
								RfcTagAfterMigration =="en-Zxxx-x-audio")
							{
								delegateCalledCorrectly = true;
							}
						}
					);
				migrator.Migrate();

				Assert.IsTrue(delegateCalledCorrectly);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingIsAudioIsTrue_ScriptContainsZxxxAndVariantContainsXDashAudio()
		{
			using (var environment = new TestEnvironment())
			{
				const string language = "en";
				const bool isAudio = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(language, "",
														  "", "", "", 0, false, "", "", true, isAudio)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");

				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type = 'en']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/script[@type = 'Zxxx']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/region[@type = '']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/variant[@type = 'x-audio']");
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingIsAudioIsTrueAndrfcTagAlreadyHasScript_ScriptContainsZxxxAndVariantContainsXDashAudioDashScript()
		{
			using (var environment = new TestEnvironment())
			{
				const string language = "en-Latn";
				const bool isAudio = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(language, "",
														  "", "", "", 0, false, "", "", true, isAudio)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag(language);

				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type = 'en-Latn']");
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type = 'Zxxx']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/region");
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type = 'x-audio']");
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingIsAudioIsFalse_audioIsRemovedFromRfcTag()
		{
			using (var environment = new TestEnvironment())
			{
				const string id = "en-audio";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(id, "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag(id);

				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type = 'en']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/region");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/variant");
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainingIsAudioIsFalse_AudioIsRemovedFromRfcTag()
		{
			using (var environment = new TestEnvironment())
			{
				const string id = "en-Audio";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(id, "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag(id);

				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type = 'en']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/region");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/variant");
			}
		}

		[Test]
		public void MigrateIfNecassary_DateModified_IsSetToRecentTime()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");

				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/generation[@date = '0001-01-01T00:00:00']");
			}
		}

		public class WritingSystemPrefsFileContent
		{
			public static string SingleWritingSystem(
				string id,
				string abbreviation,
				string sortUsing,
				string customSortRules,
				string fontName,
				int fontSize,
				bool rightToleft,
				string spellCheckingId,
				string keyboard,
				bool isUnicode,
				bool isAudio)
			{
				string sortRulesXml = String.Empty;
				if(!String.IsNullOrEmpty(customSortRules))
				{
					sortRulesXml = String.Format("<CustomSortRules>{0}</CustomSortRules>", customSortRules);
				}
				return String.Format(
					@"<?xml version='1.0' encoding='utf-8'?>
<WritingSystemCollection>
  <members>
	<WritingSystem>
	  <Abbreviation>{0}</Abbreviation>
	  {1}
	  <FontName>{2}</FontName>
	  <FontSize>{3}</FontSize>
	  <Id>{4}</Id>
	  <IsAudio>{5}</IsAudio>
	  <IsUnicode>{6}</IsUnicode>
	  <WindowsKeyman>{7}</WindowsKeyman>
	  <RightToLeft>{8}</RightToLeft>
	  <SortUsing>{9}</SortUsing>
	  <SpellCheckingId>{10}</SpellCheckingId>
	</WritingSystem>
  </members>
</WritingSystemCollection>".Replace("'", "\""),
					abbreviation, sortRulesXml, fontName, fontSize, id, isAudio,
					isUnicode, keyboard, rightToleft, sortUsing, spellCheckingId
					);
			}
		}
	}
}
