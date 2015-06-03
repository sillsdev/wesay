using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using SIL.TestUtilities;
using WeSay.Project.ConfigMigration.WeSayConfig;

namespace WeSay.Project.Tests.ConfigMigration.WeSayConfig
{
	[TestFixture]
	public class WeSayConfigMigrationTests
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

		private readonly string _queryToCheckConfigVersion = String.Format(
			"configuration[@version='{0}']",
			ConfigFile.LatestVersion
		);

		private readonly ConfigurationMigrator _migrator = new ConfigurationMigrator();

		[Test]
		public void DoesMigrateV0File()
		{
			File.WriteAllText(
				_pathToInputConfig,
				"<?xml version='1.0' encoding='utf-8'?><tasks><components><viewTemplate></viewTemplate></components><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks>"
			);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);

			Assert.IsTrue(didMigrate);
			var outputDoc = new XmlDocument();
			outputDoc.Load(_outputPath);
			Assert.IsNotNull(outputDoc.SelectSingleNode(_queryToCheckConfigVersion));
		}

		[Test]
		public void DoesMigrateV1File()
		{
			File.WriteAllText(
				_pathToInputConfig,
				"<?xml version='1.0' encoding='utf-8'?><configuration version='1'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>"
			);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV2File()
		{
			File.WriteAllText(
				_pathToInputConfig,
				"<?xml version='1.0' encoding='utf-8'?><configuration version='2'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>"
			);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV3File()
		{
			File.WriteAllText(
				_pathToInputConfig,
				"<?xml version='1.0' encoding='utf-8'?><configuration version='3'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>"
			);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV4File()
		{
			File.WriteAllText(
				_pathToInputConfig,
				"<?xml version='1.0' encoding='utf-8'?><configuration version='4'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>"
			);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsTrue(didMigrate);
			AssertHasAtLeastOneMatch(_queryToCheckConfigVersion, _outputPath);
		}

		[Test]
		public void DoesMigrateV5File()
		{
			File.WriteAllText(
				_pathToInputConfig,
				@"<?xml version='1.0' encoding='utf-8'?>
				<configuration version='5'>
					<components>
						<viewTemplate></viewTemplate>
					</components>
					<tasks><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks>
				</configuration>"
			);
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
			_migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			AssertHasAtLeastOneMatch("//field[className/@text()='LexSense' && fieldName/@text()='literal-meaning']", _outputPath);
		}
		*/

		[Test]
		public void V5File_ViewTemplateHasSemanticDomainDdp4Field_ConvertedToHyphenatedForm()
		{
			File.WriteAllText(
				_pathToInputConfig,
				@"<?xml version='1.0' encoding='utf-8'?>
					<configuration version='5'>
						<components>
							<viewTemplate>
								<field>
									<fieldName>SemanticDomainDdp4</fieldName>
								</field>
							</viewTemplate>
						</components>
					</configuration>"
			);
			_migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			AssertHasAtLeastOneMatch("//field/fieldName[text()='semantic-domain-ddp4']", _outputPath);
		}

		[Test]
		public void V5File_MissingInfoTaskHasSemanticDomainDdp4Field_ConvertedToHyphenatedForm()
		{
			File.WriteAllText(
				_pathToInputConfig,
				@"<?xml version='1.0' encoding='utf-8'?>
					<configuration version='5'>
						<components><tasks>
						   <task taskName='AddMissingInfo' visible='true'>
							  <field>SemanticDomainDdp4</field>
							  <showFields>SemanticDomainDdp4</showFields>
							  <readOnly>SemanticDomainDdp4</readOnly>
							  <writingSystemsToMatch />
							</task>
						</tasks></components></configuration>"
			);
			_migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig,_outputPath);
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
		public void V7File_ConfigFileIsVersion7_ConvertedToVersion8()
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
			AssertHasAtLeastOneMatch("//configuration[@version='8']", _outputPath);
		}

		[Test]
		public void DoesNotTouchCurrentFile()
		{
			File.WriteAllText(
				_pathToInputConfig,
				"<?xml version='1.0' encoding='utf-8'?><configuration version='8'></configuration>"
			);
			bool didMigrate = _migrator.MigrateConfigurationXmlIfNeeded(_pathToInputConfig, _outputPath);
			Assert.IsFalse(didMigrate);
		}

		private static void AssertHasAtLeastOneMatch(string xpath, string filePath)
		{
			AssertThatXmlIn.File(filePath).HasAtLeastOneMatchForXpath(xpath);
		}
	}
}