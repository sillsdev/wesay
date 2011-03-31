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
			private Dictionary<string, string> _oldToNewRfcTagMap;
			private readonly string _pathToWsPrefsFile;
			private readonly TemporaryFolder _pretendProjectDirectory;
			private readonly XmlNamespaceManager _namespaceManager;

			public TestEnvironment()
			{
				_pretendProjectDirectory = new TemporaryFolder("PretendWeSayProject");
				_pathToWsPrefsFile = Path.Combine(_pretendProjectDirectory.Path, "WritingSystemPrefs.xml");
				PathToLdmlWsRepo = Path.Combine(_pretendProjectDirectory.Path, "WritingSystems");
				_namespaceManager = new XmlNamespaceManager(new NameTable());
				NamespaceManager.AddNamespace("palaso", "urn://palaso.org/ldmlExtensions/v1");
			}

			public string PathToWsPrefsFile
			{
				get { return _pathToWsPrefsFile; }
			}

			public string PathToLdmlWsRepo { get; private set; }

			public XmlNamespaceManager NamespaceManager
			{
				get { return _namespaceManager; }
			}

			public string GetFileForOriginalRfcTag(string oldRfcTag)
			{
				return Path.Combine(PathToLdmlWsRepo, _oldToNewRfcTagMap[oldRfcTag] + ".ldml");
			}

			public void WriteContentToWsPrefsFile(string content)
			{
				File.WriteAllText(_pathToWsPrefsFile, content);
			}

			public void ChangeRfcTags(Dictionary<string, string> oldToNewRfcTagMap)
			{
				_oldToNewRfcTagMap = oldToNewRfcTagMap;
			}

			public void Dispose()
			{
				if (Directory.Exists(PathToLdmlWsRepo))
				{
					foreach (var ldmlFile in Directory.GetFiles(PathToLdmlWsRepo))
					{
						File.Delete(ldmlFile);
					}
					Directory.Delete(PathToLdmlWsRepo);
				}
				if (File.Exists(_pathToWsPrefsFile))
				{
					File.Delete(_pathToWsPrefsFile);
				}
				if (Directory.Exists(PathToLdmlWsRepo))
				{
					Directory.Delete(_pretendProjectDirectory.Path);
				}
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsFontName_FontNameIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				string propertyInQuestion = "Arial";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", propertyInQuestion, 0, "en", false, true, "", false,
														  "", "")
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:defaultFontFamily[@value='{0}']", propertyInQuestion),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsFontSize_FontSizeIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				int propertyInQuestion = 12;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", propertyInQuestion, "en", false, true, "", false,
														  "", "")
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:defaultFontSize[@value='{0}']", propertyInQuestion),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsKeyboardName_KeyboardNameIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				string propertyInQuestion = "IPA Unicode 5.1(ver 1.2 US) MSK";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, "en", false, true, propertyInQuestion, false,
														  "", "")
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:defaultKeyboard[@value='{0}']", propertyInQuestion),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsAbbreviation_AbbreviationIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				string propertyInQuestion = "v";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  propertyInQuestion, "", "", 0, "en", false, true, "", false,
														  "", "")
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:abbreviation[@value='{0}']", propertyInQuestion),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsCustomSimpleSortRules_CustomSortRulesAreInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				string sortUsing = "CustomSimple";
				string sortRules =
@"N n
O o";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", sortRules, "", 0, "en", false, true, "", false, sortUsing, "")
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
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsCustomIcuSortRules_CustomSortRulesAreInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				string sortUsing = "CustomICU";
				string sortRules = "&amp; C &lt; č";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", sortRules, "", 0, "en", false, true, "", false, sortUsing, "")
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
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsSortRulesFromOtherLanguage_whatTodo()
		{
			using (var environment = new TestEnvironment())
			{
				string propertyInQuestion = "v";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, "en", false, true, "", false,
														  "", "")
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:abbreviation[@value='{0}']", propertyInQuestion),
					environment.NamespaceManager
					);
			}
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsIsUnicodeEncodedIsFalse_IsLegacyIsTrueInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				bool propertyInQuestion = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, "en", false, propertyInQuestion, "", false,
														  "", "")
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:isLegacyEncoded[@value='{0}']", !propertyInQuestion),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsIsUnicodeEncodedIsTrue_IsLegacyDoesNotAppearInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				bool propertyInQuestion = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, "en", false, propertyInQuestion, "", false,
														  "", "")
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
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsRightToLeftIsTrue_CharacterOrientationIsMarkedRightToLeftInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				bool propertyInQuestion = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, "en", false, true, "", propertyInQuestion,
														  "", "")
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
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsRightToLeftIsFalse_CharacterOrientationIsnotContainedInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				bool propertyInQuestion = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, "en", false, true, "", propertyInQuestion,
														  "", "")
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
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsSortUsing_whattodo()
		{
			using (var environment = new TestEnvironment())
			{
				bool propertyInQuestion = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, "en", false, true, "", propertyInQuestion,
														  "", "")
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
				string propertyInQuestion = "spell";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, "en", false, true, "", false,
														  "", propertyInQuestion)
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:spellCheckingId[@value='{0}']", propertyInQuestion),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecassary_WsPrefsFileContainsRfcTagThatChangesOnMigration_MigrationDelegateIsCalled()
		{
			using (var environment = new TestEnvironment())
			{
				string language = "en";
				bool isAudio = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  language, "", "", 0, language, isAudio, true, "", false,
														  "", "")
					);
				bool delegateCalledCorrectly = false;
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					delegate(Dictionary<string, string> oldToNewRfcTagsMap)
						{
							if(oldToNewRfcTagsMap["en"].Equals("en-Zxxx-x-audio"))
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
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsIsAudioIsTrue_ScriptContainsZxxxAndVariantContainsXDashAudio()
		{
			using (var environment = new TestEnvironment())
			{
				string language = "en";
				bool isAudio = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, language, isAudio, true, "", false,
														  "", "")
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
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsIsAudioIsTrueAndrfcTagAlreadyHasScript_ScriptContainsZxxxAndVariantContainsXDashAudioDashScript()
		{
			using (var environment = new TestEnvironment())
			{
				string language = "en-Latn";
				bool isAudio = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, language, isAudio, true, "", false,
														  "", "")
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
		public void MigrateIfNecassary_WsPrefsFileContainsWsContainsIsAudioIsFalse_audioIsRemovedFromRfcTag()
		{
			using (var environment = new TestEnvironment())
			{
				string propertyInQuestion = "en-audio";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, propertyInQuestion, false, true, "", false,
														  "", "")
					);
				var migrator = new WritingSystemMigrator(
					WritingSystemDefinition.LatestWritingSystemDefinitionVersion,
					environment.PathToWsPrefsFile,
					environment.ChangeRfcTags);
				migrator.Migrate();
				string pathToEnFile = environment.GetFileForOriginalRfcTag(propertyInQuestion);

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
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(
														  "", "", "", 0, "en", false, true, "", false,
														  "", "")
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
				string abbreviation,
				string customSortRules,
				string fontName,
				int fontSize,
				string id,
				bool isAudio,
				bool isUnicode,
				string keyboard,
				bool rightToleft,
				string sortUsing,
				string spellCheckingId
				)
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
