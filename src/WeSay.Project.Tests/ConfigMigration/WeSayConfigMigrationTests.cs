using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using Palaso.TestUtilities;
using WeSay.LexicalModel.Foundation;
using WeSay.Project.ConfigMigration.WeSayConfig;

namespace WeSay.Project.Tests.ConfigMigration
{
	[TestFixture]
	public class WeSayConfigMigrationTests
	{
		private TemporaryFolder _projectFolder;
		private string _pathToInputConfig;
		private string _outputPath;
		private string _writingSystemPrefsFilepath;

		[SetUp]
		public void Setup()
		{
			_projectFolder = new TemporaryFolder();
			_pathToInputConfig = _projectFolder.GetPathForNewTempFile(false);
			_outputPath = _projectFolder.GetPathForNewTempFile(false);
			_writingSystemPrefsFilepath = Path.Combine(_projectFolder.Path, "WritingSystemPrefs.xml");
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(_pathToInputConfig);
			File.Delete(_outputPath);
			_projectFolder.Dispose();
		}

		private readonly string _queryToCheckConfigVersion = String.Format("configuration[@version='{0}']",
																		   WeSayWordsProject.CurrentWeSayConfigFileVersion);

		private ConfigurationMigrator _migrator = new ConfigurationMigrator();

		[Test]
		public void DoesMigrateV0File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><tasks><components><viewTemplate></viewTemplate></components><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks>");
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);

			Assert.IsTrue(didMigrate);
			XmlDocument outputDoc = new XmlDocument();
			outputDoc.Load(_outputPath);
			Assert.IsNotNull(outputDoc.SelectSingleNode(_queryToCheckConfigVersion));
		}

		[Test]
		public void DoesMigrateV1File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='1'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV2File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='2'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV3File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='3'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV4File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='4'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV5File()
		{
			File.WriteAllText(_pathToInputConfig,
							  @"<?xml version='1.0' encoding='utf-8'?>
			  <configuration version='5'>
					<components>
						<viewTemplate></viewTemplate>
					</components>
					<tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks>
			  </configuration>");
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		/* since literal meaning is unlikely to even be in use at this stage, we just deal with it in code
  [Test]
		public void V5File_ViewTemplateHasOldLiteralMeaningField_ConvertedToHyphenatedFormOnEntry()
		{
			File.WriteAllText(_pathToInputConfig,
							  @"<?xml version='1.0' encoding='utf-8'?>
									<configuration version='5'>
										<components>
											<viewTemplate>
												<field>
													<className>LexSense</className>
													<fieldName>LiteralMeaning</fieldName>
												</field>
											</viewTemplate>
										</components>
									</configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			_migrator.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
			AssertHasAtLeastOneMatch("//field[className/@text()='LexSense' && fieldName/@text()='literal-meaning']", _outputPath);
		}
		*/

		[Test]
		public void V5File_ViewTemplateHasSemanticDomainDdp4Field_ConvertedToHyphenatedForm()
		{
			File.WriteAllText(_pathToInputConfig,
							  @"<?xml version='1.0' encoding='utf-8'?>
									<configuration version='5'>
										<components>
											<viewTemplate>
												<field>
													<fieldName>SemanticDomainDdp4</fieldName>
												</field>
											</viewTemplate>
										</components>
									</configuration>");
			_migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			AssertHasAtLeastOneMatch("//field/fieldName[text()='semantic-domain-ddp4']", _outputPath);
		}

		[Test]
		public void V5File_MissingInfoTaskHasSemanticDomainDdp4Field_ConvertedToHyphenatedForm()
		{
			File.WriteAllText(_pathToInputConfig,
							  @"<?xml version='1.0' encoding='utf-8'?>
									<configuration version='5'>
										<components><tasks>
										   <task taskName='AddMissingInfo' visible='true'>
											  <field>SemanticDomainDdp4</field>
											  <showFields>SemanticDomainDdp4</showFields>
											  <readOnly>SemanticDomainDdp4</readOnly>
											  <writingSystemsToMatch />
											</task>
										</tasks></components></configuration>");
			_migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			AssertHasAtLeastOneMatch("//task/field[text()='semantic-domain-ddp4']", _outputPath);
			AssertHasAtLeastOneMatch("//task/showFields[text()='semantic-domain-ddp4']", _outputPath);
			AssertHasAtLeastOneMatch("//task/readOnly[text()='semantic-domain-ddp4']", _outputPath);
		}

		[Test]
		public void DoesMigrateV6File()
		{
			File.WriteAllText(_pathToInputConfig,
			@"<?xml version='1.0' encoding='utf-8'?>
			<configuration version='5'>
				<components>
					<viewTemplate></viewTemplate>
				</components>
				<tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks>
			</configuration>");
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void V6File_ViewTemplateHasWeSayDataObject_ConvertedToPalasoDataObject()
		{
			File.WriteAllText(_pathToInputConfig,
				@"<?xml version='1.0' encoding='utf-8'?>
				<configuration version='6'>
					<fields>
						<field>
							<className>WeSayDataObject</className>
							<dataType>MultiText</dataType>
						</field>
					</fields>
				</configuration>");
			_migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			AssertHasAtLeastOneMatch("//field/className[text()='PalasoDataObject']", _outputPath);
		}

		[Test]
		public void V7File_ConfigFileIsVersion7_ConvertedToLatest()
		{
			File.WriteAllText(_pathToInputConfig,
				@"<?xml version='1.0' encoding='utf-8'?>
				<configuration version='7'>
					<fields>
						<field>
							<className>WeSayDataObject</className>
							<dataType>MultiText</dataType>
						</field>
					</fields>
				</configuration>");
			_migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			AssertHasAtLeastOneMatch(String.Format("//configuration[@version='{0}']",WeSayWordsProject.CurrentWeSayConfigFileVersion), _outputPath);
		}

		[Test]
		public void DoesNotTouchCurrentFile()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='9'></configuration>");
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsFalse(didMigrate);
		}

		private static void AssertHasAtLeastOneMatch(string xpath, string filePath)
		{
			AssertThatXmlIn.File(filePath).
				HasAtLeastOneMatchForXpath(xpath);
//
//            XmlDocument doc = new XmlDocument();
//            try
//            {
//                doc.Load(filePath);
//            }
//            catch (Exception err)
//            {
//                Console.WriteLine(err.Message);
//                Console.WriteLine(File.ReadAllText(filePath));
//            }
//            XmlNode node = doc.SelectSingleNode(xpath);
//            if (node == null)
//            {
//                XmlWriterSettings settings = new XmlWriterSettings();
//                settings.Indent = true;
//                settings.ConformanceLevel = ConformanceLevel.Fragment;
//                XmlWriter writer = XmlWriter.Create(Console.Out, settings);
//                doc.WriteContentTo(writer);
//                writer.Flush();
//            }
//            Assert.IsNotNull(node);
		}

		[Test]
		public void NewProject_ContainsOnlyLegacyWeSayWritingsystemsFile_WritingSystemsAreLoadedFromThatFile()
		{
			WeSayWordsProject.InitializeForTests();
			CreateVersion7ProjectFiles();
			_migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			AssertHasAtLeastOneMatch(String.Format("//configuration[@version='{0}']",WeSayWordsProject.CurrentWeSayConfigFileVersion), _outputPath);
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(_projectFolder.Path);

			Assert.AreEqual(2, project.WritingSystems.Count);
			Assert.IsTrue(project.WritingSystems.ContainsKey("PretendAnalysis"));
			Assert.IsTrue(project.WritingSystems.ContainsKey("PretendVernacular"));
		}

		[Test]
		public void NewProject_ContainsOnlyLegacyWeSayWritingsystemsFile_WritingSystemsAreConvertedToLdml()
		{
			WeSayWordsProject.InitializeForTests();
			CreateVersion7ProjectFiles();
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(_projectFolder.Path);
			WritingSystemCollection wsCollection = new WritingSystemCollection();
			wsCollection.Load(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectFolder.Path));

			AssertWritingSystemCollectionsAreEqual(project.WritingSystems, wsCollection);
		}

		[Test]
		public void NewProject_ContainsOnlyLegacyWeSayWritingsystemsFile_LegacyFileIsDeleted()
		{
			CreateVersion7ProjectFiles();
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(_projectFolder.Path);

			Assert.IsFalse(File.Exists(BasilProject.GetPathToWritingSystemPrefs(_writingSystemPrefsFilepath)));
		}

		private void CreateVersion7ProjectFiles()
		{
			File.WriteAllText(_pathToInputConfig,
							  @"<?xml version='1.0' encoding='utf-8'?>
				<configuration version='7'>
					<fields>
						<field>
							<className>WeSayDataObject</className>
							<dataType>MultiText</dataType>
						</field>
					</fields>
				</configuration>");
			using (StreamWriter writer = File.CreateText(_writingSystemPrefsFilepath))
			{
				writer.Write(@"<?xml version='1.0' encoding='utf-8'?>
					<WritingSystemCollection>
					  <members>
						<WritingSystem>
						  <FontName>Courier New</FontName>
						  <FontSize>10</FontSize>
						  <Id>PretendAnalysis</Id>
						</WritingSystem>
						<WritingSystem>
						  <FontName>Courier New</FontName>
						  <FontSize>20</FontSize>
						  <Id>PretendVernacular</Id>
						</WritingSystem>
					  </members>
					</WritingSystemCollection>");
				writer.Close();
			}
		}

		private void AssertWritingSystemCollectionsAreEqual(WritingSystemCollection ws1, WritingSystemCollection ws2)
		{
			foreach (KeyValuePair<string, WritingSystem> idWspair in ws1)
			{
				Assert.IsTrue(ws2.ContainsKey(idWspair.Key));
				Assert.AreEqual(idWspair.Value.Id, ws2[idWspair.Key].Id);
				Assert.AreEqual(idWspair.Value.Abbreviation, ws2[idWspair.Key].Abbreviation);
				Assert.AreEqual(idWspair.Value.CustomSortRules, ws2[idWspair.Key].CustomSortRules);
				Assert.AreEqual(idWspair.Value.Font.ToString(), ws2[idWspair.Key].Font.ToString());
				Assert.AreEqual(idWspair.Value.FontName, ws2[idWspair.Key].FontName);
				Assert.AreEqual(idWspair.Value.FontSize, ws2[idWspair.Key].FontSize);
				Assert.AreEqual(idWspair.Value.IsAudio, ws2[idWspair.Key].IsAudio);
				Assert.AreEqual(idWspair.Value.IsUnicode, ws2[idWspair.Key].IsUnicode);
				Assert.AreEqual(idWspair.Value.KeyboardName, ws2[idWspair.Key].KeyboardName);
				Assert.AreEqual(idWspair.Value.RightToLeft, ws2[idWspair.Key].RightToLeft);
				Assert.AreEqual(idWspair.Value.SortUsing, ws2[idWspair.Key].SortUsing);
				Assert.AreEqual(idWspair.Value.SpellCheckingId, ws2[idWspair.Key].SpellCheckingId);
			}
		}
	}
}