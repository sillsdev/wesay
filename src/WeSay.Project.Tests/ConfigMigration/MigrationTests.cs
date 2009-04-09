using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using Palaso.Test;
using WeSay.Project.ConfigMigration;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class MigrationTests
	{
		private string _pathToInputConfig;
		private string _outputPath;

		[SetUp]
		public void Setup()
		{
			_pathToInputConfig = Path.GetTempFileName();
			_outputPath = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(_pathToInputConfig);
			File.Delete(_outputPath);
		}

		private readonly string _queryToCheckConfigVersion = String.Format("configuration[@version='{0}']",
											 WeSayWordsProject.CurrentWeSayConfigFileVersion);

		private ConfigurationMigrator _migrator = new ConfigurationMigrator();

		[Test]
		public void DoesMigrateV0File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><tasks><components><viewTemplate></viewTemplate></components><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);

			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(doc, _outputPath);

			Assert.IsTrue(didMigrate);
			XmlDocument outputDoc = new XmlDocument();
			outputDoc.Load(_outputPath);
			Assert.IsNotNull(outputDoc.SelectSingleNode(_queryToCheckConfigVersion));
		}

		[Test]
		public void DoesMigrateV1File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='1'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV2File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='2'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV3File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='3'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV4File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='4'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
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
					<tasks><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks>
			  </configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
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
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			_migrator.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
			AssertHasAtLeastOneMatch("//field/fieldName[text()='semantic-domain-ddp4']", _outputPath);
		}

		[Test]
		public void         V5File_MissingInfoTaskHasSemanticDomainDdp4Field_ConvertedToHyphenatedForm()
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
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			_migrator.MigrateConfigurationXmlIfNeeded(doc,_outputPath);
			AssertHasAtLeastOneMatch("//task/field[text()='semantic-domain-ddp4']", _outputPath);
			AssertHasAtLeastOneMatch("//task/showFields[text()='semantic-domain-ddp4']", _outputPath);
			AssertHasAtLeastOneMatch("//task/readOnly[text()='semantic-domain-ddp4']", _outputPath);
		}

		[Test]
		public void DoesNotTouchCurrentFile()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='6'></configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
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
	}
}