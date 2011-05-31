using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Palaso.IO;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;
using WeSay.Project.ConfigMigration.WritingSystem;

namespace WeSay.Project.Tests.ConfigMigration.WritingSystem
{
	[TestFixture]
	public class WritingsystemsFromConfigFileCreatorTests
	{
		private class TestEnvironment : IDisposable {
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
		public void CreateNonExistentWritingSystemsFoundInLift_FieldXmlContainsNonConformantRfcTag_CreatesConformingWritingSystem()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("bogusws1", "audio")))
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInConfigFile(environment.WritingSystemsPath);

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
		public void CreateNonExistentWritingSystemsFoundInLift_TasksXmlContainsNonConformantRfcTag_CreatesConformingWritingSystem()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingMissingInfoTaskWithWritingSystems("bogusws1", "audio", "de", "Zxxx")))
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInConfigFile(environment.WritingSystemsPath);

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
		public void CreateNonExistentWritingSystemsFoundInLift_AddinsXmlContainsNonConformantRfcTag_CreatesConformingWritingSystem()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingSfmExporterAddinWithWritingSystems("de", "bogusws1", "audio", "Zxxx")))
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInConfigFile(environment.WritingSystemsPath);


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
		public void CreateNonExistentWritingSystemsFoundInLift_FieldContainsNonConformantRfcTag_UpdatesRfcTagInFieldXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("bogusws1", "audio")))
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInConfigFile(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='x-bogusws1']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_TasksXmlContainsNonConformantRfcTag_UpdatesRfcTagInTasksXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingMissingInfoTaskWithWritingSystems("bogusws1", "audio", "de", "Zxxx")))
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInConfigFile(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/tasks/task/writingSystemsToMatch[text()='x-bogusws1, qaa-Zxxx-x-audio']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/tasks/task/writingSystemsWhichAreRequired[text()='de, qaa-Zxxx']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_AddinsXmlContainsNonConformantRfcTag_UpdatesRfcTagInAddinsXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingSfmExporterAddinWithWritingSystems("en", "bogusws1", "audio", "Zxxx")))
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInConfigFile(environment.WritingSystemsPath);
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
		public void CreateNonExistentWritingSystemsFoundInLift_FieldsXmlContainsNonConformantRfcTagWithDuplicates_UpdatesRfcTagInFieldsXmlOfConfigFile()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("qaa-audio", "audio")))
			{
				environment.Creator.CreateNonExistentWritingSystemsFoundInConfigFile(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio-dupl1']");


				string writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");

				writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "qaa-Zxxx-x-audio-dupl1" + ".ldml");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio-dupl1']");
			}
		}

		[Test]
		public void CreateNonExistentWritingSystemsFoundInLift_FieldsXmlContainsRfcTagThatAfterBeingMadeConformMatchesExistingLdml_LdmlFilesAreLeftUnchanged()
		{
			using (var environment = new TestEnvironment(ConfigFileContentCreator.GetConfigFileContainingFieldWithWritingSystems("en", "audio")))
			{
				string writingSystemFilePath = Path.Combine(environment.WritingSystemsPath, "en" + ".ldml");

				var enWs = WritingSystemDefinition.FromLanguage("en");
				enWs.Abbreviation = "Dont change me!";
				new LdmlAdaptor().Write(writingSystemFilePath, enWs, null);

				environment.Creator.CreateNonExistentWritingSystemsFoundInConfigFile(environment.WritingSystemsPath);
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='en']");
				AssertThatXmlIn.File(environment.ConfigFilePath).HasAtLeastOneMatchForXpath(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id[text()='qaa-Zxxx-x-audio']");

				AssertThatXmlIn.File(writingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='en']");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(writingSystemFilePath).HasNoMatchForXpath("/ldml/identity/variant");
				AssertThatXmlIn.File(writingSystemFilePath).
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

			private static  string WrapContentInConfigurationTags(string contentToWrap)
			{
				return
				"<?xml version='1.0' encoding='utf-8'?><configuration version='8'>" + Environment.NewLine +
					contentToWrap + Environment.NewLine +
				"</configuration>";
			}
		}
	}
}
