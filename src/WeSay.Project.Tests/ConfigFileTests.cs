using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Palaso.IO;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.TestUtilities;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class ConfigFileTests
	{
		[Test]
		public void ChangeWritingSystemId_IdExists_IsChanged()
		{
			WeSayProjectTestHelper.InitializeForTests();
			string pathToConfigFile = Path.GetTempFileName();
			File.WriteAllText(pathToConfigFile,
								  GetV7ConfigFileContent());
			var configFile = new ConfigFile(pathToConfigFile);
			configFile.ReplaceWritingSystemId("hi-u", "hi-Zxxx-x-audio");
			string newFileContent = File.ReadAllText(pathToConfigFile);
			Assert.IsFalse(newFileContent.Contains("hi-u"));
			Assert.IsTrue(newFileContent.Contains("hi-Zxxx-x-audio"));
		}

		[Test]
		public void ChangeWritingSystemId_DoesnotExist_NoChange()
		{
			WeSayProjectTestHelper.InitializeForTests();
			string pathToConfigFile = Path.GetTempFileName();
			File.WriteAllText(pathToConfigFile,
								  GetV7ConfigFileContent());
			var configFile = new ConfigFile(pathToConfigFile);
			configFile.ReplaceWritingSystemId("hi-up", "hi-Zxxx-x-audio");
			string newFileContent = File.ReadAllText(pathToConfigFile);
			Assert.IsFalse(newFileContent.Contains("hi-Zxxx-x-audio"));
		}

		[Test]
		public void MigrateIfNeeded_AfterMigrationTheNewConfigFileIsLoaded()
		{
			WeSayProjectTestHelper.InitializeForTests();
			string pathToConfigFile = Path.GetTempFileName();
			File.WriteAllText(pathToConfigFile,
								  GetV7ConfigFileContent());
			var configFile = new ConfigFile(pathToConfigFile);
			configFile.MigrateIfNecassary();
			Assert.That(configFile.Version, Is.EqualTo(8));
		}

		[Test]
		public void DefaultConfigFile_DoesntNeedMigrating()
		{
			var configFile = new XmlDocument();
			configFile.Load(WeSayWordsProject.PathToDefaultConfig);
			XmlNode versionNode = configFile.SelectSingleNode("configuration/@version");
			Assert.AreEqual(ConfigFile.LatestVersion, Convert.ToInt32(versionNode.Value));
		}

		private static string GetV7ConfigFileContent()
		{
			return
				@"<?xml version='1.0' encoding='utf-8'?>
<configuration version='7'>
  <components>
	<viewTemplate>
	  <fields>
		<field>
		  <className>LexEntry</className>
		  <dataType>MultiText</dataType>
		  <displayName>Word</displayName>
		  <enabled>True</enabled>
		  <fieldName>EntryLexicalForm</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>hi-u</id>
			<id>tap-Zxxx-x-audio</id>
			<id>lalaa</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexEntry</className>
		  <dataType>MultiText</dataType>
		  <displayName>Citation Form</displayName>
		  <enabled>False</enabled>
		  <fieldName>citation</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>hi-u</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>MultiText</dataType>
		  <displayName>Definition (Meaning)</displayName>
		  <enabled>True</enabled>
		  <fieldName>definition</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>MultiText</dataType>
		  <displayName>Gloss</displayName>
		  <enabled>False</enabled>
		  <fieldName>gloss</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexEntry</className>
		  <dataType>MultiText</dataType>
		  <displayName>Literal Meaning</displayName>
		  <enabled>False</enabled>
		  <fieldName>literal-meaning</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>PalasoDataObject</className>
		  <dataType>MultiText</dataType>
		  <displayName>Note</displayName>
		  <enabled>True</enabled>
		  <fieldName>note</fieldName>
		  <multiParagraph>True</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>Picture</dataType>
		  <displayName>Picture</displayName>
		  <enabled>True</enabled>
		  <fieldName>Picture</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>Option</dataType>
		  <displayName>PartOfSpeech</displayName>
		  <enabled>True</enabled>
		  <fieldName>POS</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <optionsListFile>PartsOfSpeech.xml</optionsListFile>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexExampleSentence</className>
		  <dataType>MultiText</dataType>
		  <displayName>Example Sentence</displayName>
		  <enabled>True</enabled>
		  <fieldName>ExampleSentence</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>hi-u</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexExampleSentence</className>
		  <dataType>MultiText</dataType>
		  <displayName>Example Translation</displayName>
		  <enabled>False</enabled>
		  <fieldName>ExampleTranslation</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>OptionCollection</dataType>
		  <displayName>Sem Dom</displayName>
		  <enabled>True</enabled>
		  <fieldName>semantic-domain-ddp4</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <optionsListFile>Ddp4.xml</optionsListFile>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexEntry</className>
		  <dataType>RelationToOneEntry</dataType>
		  <displayName>Base Form</displayName>
		  <enabled>False</enabled>
		  <fieldName>BaseForm</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>hi-u</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexEntry</className>
		  <dataType>RelationToOneEntry</dataType>
		  <displayName>Cross Reference</displayName>
		  <enabled>False</enabled>
		  <fieldName>confer</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOrMore</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>hi-u</id>
		  </writingSystems>
		</field>
	  </fields>
	  <id>Default View Template</id>
	</viewTemplate>
  </components>
  <tasks>
	<task taskName='Dashboard' visible='true' />
	<task taskName='Dictionary' visible='true' />
	<task taskName='AddMissingInfo' visible='false'>
	  <label>Meanings</label>
	  <longLabel>Add Meanings</longLabel>
	  <description>Add meanings (senses) to entries where they are missing.</description>
	  <field>definition</field>
	  <showFields>definition</showFields>
	  <readOnly>semantic-domain-ddp4</readOnly>
	  <writingSystemsToMatch />
	  <writingSystemsWhichAreRequired />
	</task>
	<task taskName='AddMissingInfo' visible='false'>
	  <label>Parts of Speech</label>
	  <longLabel>Add Parts of Speech</longLabel>
	  <description>Add parts of speech to senses where they are missing.</description>
	  <field>POS</field>
	  <showFields>POS</showFields>
	  <readOnly>definition, ExampleSentence</readOnly>
	  <writingSystemsToMatch />
	  <writingSystemsWhichAreRequired />
	</task>
	<task taskName='AddMissingInfo' visible='false'>
	  <label>Example Sentences</label>
	  <longLabel>Add Example Sentences</longLabel>
	  <description>Add example sentences to senses where they are missing.</description>
	  <field>ExampleSentence</field>
	  <showFields>ExampleSentence</showFields>
	  <readOnly>definition</readOnly>
	  <writingSystemsToMatch />
	  <writingSystemsWhichAreRequired />
	</task>
	<task taskName='AddMissingInfo' visible='false'>
	  <label>Base Forms</label>
	  <longLabel>Add Base Forms</longLabel>
	  <description>Identify the 'base form' word that this word is built from. In the printed dictionary, the derived or variant words can optionally be shown as subentries of their base forms.</description>
	  <field>BaseForm</field>
	  <showFields>BaseForm</showFields>
	  <readOnly />
	  <writingSystemsToMatch />
	  <writingSystemsWhichAreRequired />
	</task>
	<task taskName='AdvancedHistory' visible='false' />
	<task taskName='NotesBrowser' visible='false' />
	<task taskName='GatherWordList' visible='false'>
	  <wordListFileName>SILCAWL.lift</wordListFileName>
	  <wordListWritingSystemId>en</wordListWritingSystemId>
	</task>
	<task taskName='GatherWordsBySemanticDomains' visible='true'>
	  <semanticDomainsQuestionFileName>Ddp4Questions-en.xml</semanticDomainsQuestionFileName>
	  <showMeaningField>False</showMeaningField>
	</task>
  </tasks>
  <addins>
	<addin id='SendReceiveAction' showInWeSay='True' />
  </addins>
</configuration>"
					.Replace("'", "\"");
		}
	}

	[TestFixture]
	public class WritingsystemsFromConfigFileCreatorTests
	{
		private class TestEnvironment : IDisposable
		{
			private readonly TemporaryFolder _folder;
			private readonly TempFile _configFile;

			public TestEnvironment(string configFileContent)
			{
				_folder = new TemporaryFolder("WritingSystemsFromConfigFileCreator");
				var configFilePath = Path.Combine(_folder.Path, "test1.WeSayConfig");
				_configFile = new TempFile(configFileContent);
				_configFile.MoveTo(configFilePath);
				NamespaceManager = new XmlNamespaceManager(new NameTable());
				NamespaceManager.AddNamespace("palaso", "urn://palaso.org/ldmlExtensions/v1");
				Directory.CreateDirectory(Path.Combine(ProjectPath, "WritingSystems"));
				Creator = new ConfigFile(_configFile.Path);
			}

			public XmlNamespaceManager NamespaceManager { get; private set; }

			public string ProjectPath
			{
				get { return _folder.Path; }
			}

			public ConfigFile Creator { get; private set; }

			public void Dispose()
			{
				_configFile.Dispose();
				_folder.Dispose();
			}

			public string WritingSystemsPath
			{
				get { return Path.Combine(ProjectPath, "WritingSystems"); }
			}

			public string ConfigFilePath
			{
				get { return _configFile.Path; }
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_FieldXmlContainsNonConformantRfcTag_CreatesConformingWritingSystem()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("bogusws1", "audio")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);

				string writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "x-bogusws1" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogusws1']");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_TasksXmlContainsNonConformantRfcTag_CreatesConformingWritingSystem()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingMissingInfoTaskWithWritingSystems("bogusws1", "audio", "de", "Zxxx")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);

				string writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "x-bogusws1" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogusws1']");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "de" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='de']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/variant");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/variant");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_AddinsXmlContainsNonConformantRfcTag_CreatesConformingWritingSystem()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingSfmExporterAddinWithWritingSystems("de", "bogusws1", "audio", "Zxxx")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);


				string writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "x-bogusws1" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogusws1']");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "de" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='de']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/variant");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/variant");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_FieldContainsNonConformantRfcTag_UpdatesRfcTagInFieldXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("bogusws1", "audio")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='x-bogusws1']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_TasksXmlContainsNonConformantRfcTag_UpdatesRfcTagInTasksXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingMissingInfoTaskWithWritingSystems("bogusws1", "audio", "de", "Zxxx")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/tasks/task/writingSystemsToMatch[text()='x-bogusws1, qaa-Zxxx-x-audio']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/tasks/task/writingSystemsWhichAreRequired[text()='de, qaa-Zxxx']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_AddinsXmlContainsNonConformantRfcTag_UpdatesRfcTagInAddinsXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingSfmExporterAddinWithWritingSystems("en", "bogusws1", "audio", "Zxxx")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/addins/addin/SfmTransformSettings/EnglishLanguageWritingSystemId[text()='en']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/addins/addin/SfmTransformSettings/NationalLanguageWritingSystemId[text()='x-bogusws1']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/addins/addin/SfmTransformSettings/RegionalLanguageWritingSystemId[text()='qaa-Zxxx-x-audio']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/addins/addin/SfmTransformSettings/VernacularLanguageWritingSystemId[text()='qaa-Zxxx']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_WS34066RegressionTest_UpdatesRfcTagInFieldsXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("x-aaa", "aaa")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);

				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='x-aaa']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='aaa']");


				string writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "x-aaa" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-aaa']");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "aaa" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='aaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/variant");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_FieldsXmlContainsNonConformantRfcTagWithDuplicates_UpdatesRfcTagInFieldsXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("qaa-audio", "qaa-Zxxx-x-audio")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio-dupl0']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio']");


				string writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio-dupl0" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio-dupl0']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_FieldsXmlContainsNonConformantRfcTagWithDuplicatesAndAlreadyhasDuplicateMarker_UpdatesRfcTagInFieldsXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("qaa-audio-dupl1", "qaa-Zxxx-x-audio-dupl1")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio-dupl1']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio-dupl1-dupl0']");


				string writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio-dupl1-dupl0" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio-dupl1-dupl0']");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio-dupl1" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio-dupl1']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_XmlContainsSameWsTwice_SecondInstanceIsNotSeenAsDuplicateWritingSystem()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingSfmExporterAddinWithWritingSystems("qaa-audio", "qaa-audio", "en", "de")))
			{
				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/addins/addin/SfmTransformSettings/EnglishLanguageWritingSystemId[text()='qaa-Zxxx-x-audio']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/addins/addin/SfmTransformSettings/NationalLanguageWritingSystemId[text()='qaa-Zxxx-x-audio']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/addins/addin/SfmTransformSettings/RegionalLanguageWritingSystemId[text()='en']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/addins/addin/SfmTransformSettings/VernacularLanguageWritingSystemId[text()='de']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInConfig_FieldsXmlContainsRfcTagThatAfterBeingMadeConformMatchesExistingLdml_LdmlFilesAreLeftUnchanged()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("en", "audio")))
			{
				var wsRepo = new LdmlInFolderWritingSystemRepository(environment.WritingSystemsPath);

				var enWs = WritingSystemDefinition.FromLanguage("en");
				enWs.Abbreviation = "Dont change me!";
				wsRepo.Set(enWs);
				wsRepo.Save();

				environment.Creator.CreateWritingSystemsForIdsInFileWhereNecassary(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='en']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio']");

				var pathToLdml = Path.Combine(environment.WritingSystemsPath, "en.ldml");
				AssertThatXmlIn.File(pathToLdml).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='en']");
				AssertThatXmlIn.File(pathToLdml).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(pathToLdml).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(pathToLdml).HasNoMatchForXpath("/ldml/identity/variant");
				AssertThatXmlIn.File(pathToLdml).
					HasAtLeastOneMatchForXpath("/ldml/special/palaso:abbreviation[@value='Dont change me!']", environment.NamespaceManager);
			}
		}

		private class ConfigFileContentCreator
		{
			public static string GetConfigFileContainingFieldWithWritingSystems(string writingSystemLabel1, string writingSystemLabel2)
			{
				string fieldXml = String.Format(
   @"<components>
		<viewTemplate>
			<fields>
				<field>
					<className>LexEntry</className>
					<dataType>MultiText</dataType>
					<displayName>Word</displayName>
					<enabled>True</enabled>
					<fieldName>EntryLexicalForm</fieldName>
					<multiParagraph>False</multiParagraph>
					<spellCheckingEnabled>False</spellCheckingEnabled>
					<multiplicity>ZeroOr1</multiplicity>
					<visibility>Visible</visibility>
					<writingSystems>
						<id>{0}</id>
						<id>{1}</id>
					</writingSystems>
				</field>
			</fields>
			<id>Default View Template</id>
		</viewTemplate>
	</components>".Replace("'", "\""), writingSystemLabel1, writingSystemLabel2);
				return WrapContentInConfigurationTags(fieldXml);
			}

			public static string GetConfigFileContainingMissingInfoTaskWithWritingSystems(string writingSystemLabelToMatch1, string writingSystemLabelToMatch2, string requiredWritingSystemLabel1, string requiredWritingSystemLabel2)
			{
				string taskXml = String.Format(
   @"<tasks>
		<task
			taskName='Dashboard'
			visible='true' />
		<task
			taskName='Dictionary'
			visible='true' />
		<task
			taskName='AddMissingInfo'
			visible='true'>
			<label>Meanings</label>
			<longLabel>Add Meanings</longLabel>
			<description>Add meanings (senses) to entries where they are missing.</description>
			<field>definition</field>
			<showFields>definition</showFields>
			<readOnly>semantic-domain-ddp4</readOnly>
			<writingSystemsToMatch>{0}, {1}</writingSystemsToMatch>
			<writingSystemsWhichAreRequired>{2}, {3}</writingSystemsWhichAreRequired>
		</task>
	</tasks>".Replace("'", "\""), writingSystemLabelToMatch1, writingSystemLabelToMatch2, requiredWritingSystemLabel1, requiredWritingSystemLabel2);
				return WrapContentInConfigurationTags(taskXml);
			}

			public static string GetConfigFileContainingSfmExporterAddinWithWritingSystems(string englishWritingSystemLabel,
																					string nationalWritingSystemLabel,
																					string regionalWritingSystemLabel,
																					string vernacularWritingSystemLabel)
			{
				string addinXml = String.Format(
  @"<addins>
		<addin
			id='Export To SFM'
			showInWeSay='True'>
			<SfmTransformSettings>
				<SfmTagConversions />
				<EnglishLanguageWritingSystemId>{0}</EnglishLanguageWritingSystemId>
				<NationalLanguageWritingSystemId>{1}</NationalLanguageWritingSystemId>
				<RegionalLanguageWritingSystemId>{2}</RegionalLanguageWritingSystemId>
				<VernacularLanguageWritingSystemId>{3}</VernacularLanguageWritingSystemId>
			</SfmTransformSettings>
		</addin>
	</addins>".Replace("'", "\""), englishWritingSystemLabel, nationalWritingSystemLabel, regionalWritingSystemLabel, vernacularWritingSystemLabel);
				return WrapContentInConfigurationTags(addinXml);
			}

			private static string WrapContentInConfigurationTags(string contentToWrap)
			{
				return
				"<?xml version='1.0' encoding='utf-8'?><configuration version='8'>" + Environment.NewLine +
					contentToWrap + Environment.NewLine +
				"</configuration>";
			}
		}
	}
}
