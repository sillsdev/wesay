using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;
using Palaso.TestUtilities;
using WeSay.Project.ConfigMigration.WritingSystem;

namespace WeSay.Project.Tests.ConfigMigration.WritingSystem
{
	[TestFixture]
	public class WritingSystemsMigratorTests
	{
		private class TestEnvironment : IDisposable
		{
			private readonly TemporaryFolder _folder;
			private readonly XmlNamespaceManager _namespaceManager;
			private TempFile _configFile;

			public TestEnvironment()
			{
				_folder = new TemporaryFolder("WritingSystemsMigratorTests");

				_namespaceManager = new XmlNamespaceManager(new NameTable());
				_namespaceManager.AddNamespace("palaso", "urn://palaso.org/ldmlExtensions/v1");
				var pathToConfigFile = Path.Combine(_folder.Path, "test.WeSayConfig");
				_configFile = new TempFile(configFileContent);
				_configFile.MoveTo(pathToConfigFile);
			}

			//This config file was created by opening WeSay 0.9.69 Config tool and turning on every option that I could find that might insert a writingsystem into the config file. Then I removed any redundancies for brevity sake. This probably means that the config file here would not load in WeSay but it contains the relevant xml for writingsystems
			private string configFileContent =
			#region LongFileContent
 @"<?xml version='1.0' encoding='utf-8'?>
<configuration
	version='8'>
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
						<id>bogusws1</id>
						<id>bogusws2</id>
					</writingSystems>
				</field>
			</fields>
			<id>Default View Template</id>
		</viewTemplate>
	</components>
	<tasks>
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
			<writingSystemsToMatch>bogusws1, bogusws2</writingSystemsToMatch>
			<writingSystemsWhichAreRequired>bogusws1, bogusws2</writingSystemsWhichAreRequired>
		</task>
	</tasks>
</configuration>".Replace("\"", "'");
			#endregion

			public string ProjectPath
			{
				get { return _folder.Path; }
			}

			public void Dispose()
			{
				_configFile.Dispose();
				_folder.Dispose();
			}

			public string WritingSystemsPath
			{
				get { return Path.Combine(ProjectPath, "WritingSystems"); }
			}

			public string WritingSystemsOldPrefsFilePath
			{
				get { return Path.Combine(ProjectPath, "WritingSystemPrefs.xml"); }
			}

			public XmlNamespaceManager NamespaceManager
			{
				get { return _namespaceManager; }
			}

			public string PathToConfigFile
			{
				get { return _configFile.Path; }
			}

			public string WritingSystemFilePath(string tag)
			{
				return Path.Combine(WritingSystemsPath, String.Format("{0}.ldml", tag));
			}

			public void WriteToPrefsFile(string content)
			{
				File.WriteAllText(WritingSystemsOldPrefsFilePath, content);
			}
		}

		[Test]
		public void MigrateIfNeeded_HasPrefsFile_LdmlLastestVersion()
		{
			using (var e = new TestEnvironment())
			{
				e.WriteToPrefsFile(WritingSystemPrefsFileContent.SingleWritingSystemForLanguage("qaa-x-test"));
				var migrator = new WritingSystemsMigrator(e.ProjectPath);
				migrator.MigrateIfNecessary();

				AssertThatXmlIn.File(e.WritingSystemFilePath("qaa-x-test")).HasAtLeastOneMatchForXpath(
					"/ldml/special/palaso:version[@value='1']",
					e.NamespaceManager);
			}
		}

		[Test]
		public void MigrateIfNeeded_ConfigFileContainsWritingSystemIdThatIsMigrated_WritingSystemIdIsChanged()
		{
			using (var e = new TestEnvironment())
			{
				e.WriteToPrefsFile(WritingSystemPrefsFileContent.TwoWritingSystems("bogusws1", "bogusws2"));
				var migrator = new WritingSystemsMigrator(e.ProjectPath);
				migrator.MigrateIfNecessary();
				AssertThatXmlIn.File(e.PathToConfigFile).HasAtLeastOneMatchForXpath(
						"/configuration/components/viewTemplate/fields/field/writingSystems/id[1][text()='x-bogusws1']"
					);
				AssertThatXmlIn.File(e.PathToConfigFile).HasAtLeastOneMatchForXpath(
						"/configuration/components/viewTemplate/fields/field/writingSystems/id[2][text()='x-bogusws2']"
					);
				AssertThatXmlIn.File(e.PathToConfigFile).HasAtLeastOneMatchForXpath(
						"/configuration/tasks/task/writingSystemsToMatch[text()='x-bogusws1, x-bogusws2']"
					);
				AssertThatXmlIn.File(e.PathToConfigFile).HasAtLeastOneMatchForXpath(
						"/configuration/tasks/task/writingSystemsWhichAreRequired[text()='x-bogusws1, x-bogusws2']"
					);
			}
		}

		[Test]
		public void MigrateIfNeeded_ConfigFileIsVersionOtherThanWhatWeKnowTheWritingSystemMigratorCanChange_Throws()
		{
			using (var e = new TestEnvironment())
			{

			}
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNeeded_LiftFileContainsWritingSystemIdThatIsMigrated_WritingSystemIdIsChanged()
		{
			using (var e = new TestEnvironment())
			{

			}
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNeeded_LiftFileIsVersionOtherThanWhatWeKnowTheWritingSystemMigratorCanChange_Throws()
		{
			using (var e = new TestEnvironment())
			{

			}
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNeeded_NoWritingSystemsExist_DoesNotThrow()
		{
			using (var e = new TestEnvironment())
			{
				var migrator = new WritingSystemsMigrator(e.ProjectPath);
				migrator.MigrateIfNecessary();
			}
		}

		[Test]
		public void MigrateIfNeeded_WritingSystemPrefsConatinsAudioWritingSystem_IsMigratedCorrectly()
		{
			using (var e = new TestEnvironment())
			{
			}
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_RfcTagIsChangedInConfigFile_BackupOfOriginalFileExists()
		{
			using (var environment = new TestEnvironment())
			{

			}
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_RfcTagIsChangedInLiftFile_BackupOfOriginalFileExists()
		{
			using (var environment = new TestEnvironment())
			{

			}
			throw new NotImplementedException();
		}
	}
}
