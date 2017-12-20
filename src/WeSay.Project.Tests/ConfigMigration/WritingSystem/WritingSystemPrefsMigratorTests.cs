using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using SIL.TestUtilities;
using SIL.Lexicon;
using SIL.WritingSystems;
using SIL.WritingSystems.Migration;
using SIL.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;
using WeSay.Project.ConfigMigration.WritingSystem;

namespace WeSay.Project.Tests.ConfigMigration.WritingSystem
{
	[TestFixture]
	public class WritingSystemPrefsMigratorTests
	{
		private class TestEnvironment : IDisposable
		{
			private IEnumerable<LdmlMigrationInfo> _tagMigrationInfo = new List<LdmlMigrationInfo>();

			private readonly string _wsPrefsFilePath;
			private readonly string _writingSystemsPath = "";
			private readonly TemporaryFolder _testFolder;
			private readonly XmlNamespaceManager _namespaceManager;
			private IWritingSystemRepository _writingSystems;

			public TestEnvironment()
			{
				Sldr.Initialize(true);
				_testFolder = new TemporaryFolder("WritingSystemsMigratorTests");
				_wsPrefsFilePath = Path.Combine(_testFolder.Path, "WritingSystemPrefs.xml");
				_writingSystemsPath = Path.Combine(_testFolder.Path, "WritingSystems");
				Directory.CreateDirectory(WritingSystemsPath);
				_namespaceManager = new XmlNamespaceManager(new NameTable());
				NamespaceManager.AddNamespace("palaso", "urn://palaso.org/ldmlExtensions/v1");
			}

			public string WsPrefsFilePath
			{
				get { return _wsPrefsFilePath; }
			}

			public XmlNamespaceManager NamespaceManager
			{
				get { return _namespaceManager; }
			}

			public IWritingSystemRepository WritingSystems
			{
				get
				{
					return _writingSystems ?? (_writingSystems = LdmlInFolderWritingSystemRepository.Initialize(
						WritingSystemsPath));
				}
			}

			private static void OnWritingSystemLoadProblem(IEnumerable<WritingSystemRepositoryProblem> problems)
			{
				throw new ApplicationException("Unexpected input system load problem during test.");
			}

			private static void OnWritingSystemMigration(int version, IEnumerable<LdmlMigrationInfo> migrationinfo)
			{
				throw new ApplicationException("Unexpected input system migration during test.");
			}

			private string WritingSystemsPath
			{
				get { return _writingSystemsPath; }
			}

			public string GetFileForOriginalRfcTag(string oldRfcTag)
			{
				var migrationinfoForOldRfcTag =
					_tagMigrationInfo.FirstOrDefault(info => info.LanguageTagBeforeMigration == oldRfcTag);
				if( migrationinfoForOldRfcTag != null)
				{
					return Path.Combine(WritingSystemsPath, migrationinfoForOldRfcTag.LanguageTagAfterMigration + ".ldml");
				}
				return Path.Combine(WritingSystemsPath, oldRfcTag + ".ldml");
			}

			public void WriteContentToWsPrefsFile(string content)
			{
				File.WriteAllText(_wsPrefsFilePath, content);
			}

			public void ChangeRfcTags(int version, IEnumerable<LdmlMigrationInfo> migrationInfo)
			{
				_tagMigrationInfo = migrationInfo;
			}

			public void Dispose()
			{
				Sldr.Cleanup();
				if (Directory.Exists(WritingSystemsPath))
				{
					foreach (var ldmlFile in Directory.GetFiles(WritingSystemsPath))
					{
						File.Delete(ldmlFile);
					}
					Directory.Delete(WritingSystemsPath);
				}
				if (File.Exists(_wsPrefsFilePath))
				{
					File.Delete(_wsPrefsFilePath);
				}
				if (Directory.Exists(WritingSystemsPath))
				{
					Directory.Delete(_testFolder.Path);
				}
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingId_IdIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string id = "en-US-fonipa-x-etic";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(id, "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag(id);
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/identity/language[@type='{0}']", id),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingFontName_FontNameIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string fontName = "Arial";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", fontName, 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:defaultFontFamily[@value='{0}']", fontName),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingFontSize_FontSizeIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const int fontSize = 12;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", fontSize, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:defaultFontSize[@value='{0}']", fontSize),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingKeyboardName_KeyboardNameIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string keyboardName = "IPA Unicode 5.1(ver 1.2 US) MSK";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, "", keyboardName, true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:defaultKeyboard[@value='{0}']", keyboardName),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingAbbreviation_AbbreviationIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string abbreviation = "Eng";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", abbreviation,
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:abbreviation[@value='{0}']", abbreviation),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingCustomSimpleSortRules_CustomSortRulesAreInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string sortUsing = "CustomSimple";
				const string sortRules =
@"N n
O o";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "", sortUsing, sortRules, "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath(
					String.Format("/ldml/collations/collation/special/palaso:sortRulesType[@value='{0}']", sortUsing), environment.NamespaceManager
					);
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/collations/collation/rules/p[text()='N'] "); //Only checking one character. Hopefully this means the rest are there too.
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingCustomIcuSortRules_CustomSortRulesAreInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string sortUsing = "CustomICU";
				const string sortRules = "&amp; C &lt; č";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "", sortUsing, sortRules, "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath(
					String.Format("/ldml/collations/collation/special/palaso:sortRulesType[@value='{0}']", sortUsing), environment.NamespaceManager
					);
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/collations/collation/rules/p"); //Only checking one character. Hopefully this means the rest are there too.
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingSortRulesFromOtherLanguage_OtherLanguageIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string otherLanguage = "de";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  otherLanguage, "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/collations/collation/base/alias[@source='{0}']", otherLanguage),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsWithNoSortInfoSpecified_LdmlContainsNoSortInfo()
		{
			using (var environment = new TestEnvironment())
			{
				const string id = "en";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(id, "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/collations/collation");
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingIsUnicodeEncodedIsFalse_IsLegacyIsTrueInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const bool isUnicodeEncoded = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, "", "", isUnicodeEncoded, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:isLegacyEncoded[@value='{0}']", !isUnicodeEncoded),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingIsUnicodeEncodedIsTrue_IsLegacyDoesNotAppearInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const bool isUnicodeEncoded = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, "", "", isUnicodeEncoded, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasNoMatchForXpath(
					"/ldml/special/palaso:isLegacyEncoded",
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingRightToLeftIsTrue_CharacterOrientationIsMarkedRightToLeftInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const bool rightToleft = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, rightToleft, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath("/ldml/layout/orientation[@characters='right-to-left']");
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingRightToLeftIsFalse_CharacterOrientationIsnotContainedInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const bool rightToleft = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, rightToleft, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasNoMatchForXpath("/ldml/layout/orientation/@characters");
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsSpellCheckingId_SpellCheckingIdIsInLdml()
		{
			using (var environment = new TestEnvironment())
			{
				const string spellCheckingId = "spell";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, spellCheckingId, "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");
				AssertThatXmlIn.File(pathToEnFile).
					HasAtLeastOneMatchForXpath(String.Format(
					"/ldml/special/palaso:spellCheckingId[@value='{0}']", spellCheckingId),
					environment.NamespaceManager
					);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsRfcTagThatChangesOnMigration_MigrationDelegateIsCalled()
		{
			using (var environment = new TestEnvironment())
			{
				const string language = "en";
				const bool isAudio = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(language, language,
														  "", "", "", 0, false, "", "", true, isAudio)
					);
				bool delegateCalledCorrectly = false;
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					(version, oldToNewRfcTagsMap) =>
						{
							if(oldToNewRfcTagsMap.First(info => info.LanguageTagBeforeMigration == "en").
								LanguageTagAfterMigration =="en-Zxxx-x-audio")
							{
								delegateCalledCorrectly = true;
							}
						}
					);
				migrator.MigrateIfNecassary();

				Assert.IsTrue(delegateCalledCorrectly);
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingIsAudioIsTrue_ScriptContainsZxxxAndVariantContainsXDashAudio()
		{
			using (var environment = new TestEnvironment())
			{
				const string language = "en";
				const bool isAudio = false;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(language, "",
														  "", "", "", 0, false, "", "", true, isAudio)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");

				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type = 'en']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/script[@type = 'Zxxx']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/region[@type = '']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/variant[@type = 'x-audio']");
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingIsAudioIsTrueAndrfcTagAlreadyHasScript_ScriptContainsZxxxAndVariantContainsXDashAudioDashScript()
		{
			using (var environment = new TestEnvironment())
			{
				const string language = "en-Latn";
				const bool isAudio = true;
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(language, "",
														  "", "", "", 0, false, "", "", true, isAudio)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag(language);

				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type = 'en-Latn']");
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type = 'Zxxx']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/region");
				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type = 'x-audio']");
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingIsAudioIsFalse_audioIsRemovedFromRfcTag()
		{
			using (var environment = new TestEnvironment())
			{
				const string id = "en-audio";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(id, "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag(id);

				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type = 'en']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/region");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/variant");
			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsWsContainingIsAudioIsFalse_AudioIsRemovedFromRfcTag()
		{
			using (var environment = new TestEnvironment())
			{
				const string id = "en-Audio";
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem(id, "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag(id);

				AssertThatXmlIn.File(pathToEnFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type = 'en']");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/region");
				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/variant");
			}
		}

		[Test]
		public void MigrateIfNecessary_DateModified_IsSetToRecentTime()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();
				string pathToEnFile = environment.GetFileForOriginalRfcTag("en");

				AssertThatXmlIn.File(pathToEnFile).HasNoMatchForXpath("/ldml/identity/generation[@date = '0001-01-01T00:00:00']");
			}
		}

		[Test]
		public void MigrateIfNecessary_SuccessfulMigration_LegacyFileIsDeleted()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystem("en", "",
														  "", "", "", 0, false, "", "", true, false)
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();

				Assert.That(File.Exists(environment.WsPrefsFilePath), Is.False);

			}
		}

		[Test]
		public void MigrateIfNecessary_WsPrefsFileContainsMultipleWs_MultipleLdmlFilesAreCreated()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.TwoWritingSystems("bogus1", "bogus2")
					);
				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();

				Assert.That(File.Exists(environment.GetFileForOriginalRfcTag("bogus1")), Is.True);
				Assert.That(File.Exists(environment.GetFileForOriginalRfcTag("bogus2")), Is.True);
			}
		}

		[Test]
		public void MigrateIfNecessary_LdmlForWritingSystemInWsPrefsFileAlreadyExists_LdmlIsUntouched()
		{
			using (var environment = new TestEnvironment())
			{
				// RightToLeft defaults to false
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystemForLanguage("en"));

				var ws = new WritingSystemDefinition("en")
				{
					RightToLeftScript = true
				};
				var wsRepo = environment.WritingSystems;
				wsRepo.Set(ws);
				wsRepo.Save();

				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();

				AssertThatXmlIn.File(environment.GetFileForOriginalRfcTag("en")).HasAtLeastOneMatchForXpath(
					"/ldml/layout/orientation/characterOrder['right-to-left']",
					environment.NamespaceManager);
			}
		}

		[Test]
		public void MigrateIfNecessary_LdmlForWritingSystemInWsPrefsFileAlreadyExists_LdmlIsCreatedForOtherWritingSystemsInWsPrefsFile()
		{
			using (var environment = new TestEnvironment())
			{
				environment.WriteContentToWsPrefsFile(WritingSystemPrefsFileContent.TwoWritingSystems("en", "de")
					);
				var ws = new WritingSystemDefinition("en");
				ws.Abbreviation = "untouched";
				var wsRepo = environment.WritingSystems;
				wsRepo.Set(ws);
				wsRepo.Save();

				var migrator = new WritingSystemPrefsMigrator(
					environment.WsPrefsFilePath,
					environment.ChangeRfcTags);
				migrator.MigrateIfNecassary();

				Assert.That(File.Exists(environment.GetFileForOriginalRfcTag("de")), Is.True);
			}
		}
	}
}
