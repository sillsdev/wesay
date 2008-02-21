using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;

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

		[Test]
		public void DoesMigrateV0File()
		{
			File.WriteAllText(_pathToInputConfig, "<?xml version='1.0' encoding='utf-8'?><tasks><components><viewTemplate></viewTemplate></components><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);

			bool didMigrate = WeSay.Project.WeSayWordsProject.MigrateConfigurationXmlIfNeeded(doc, _outputPath);

			Assert.IsTrue(didMigrate);
			XmlDocument outputDoc = new XmlDocument();
			outputDoc.Load(_outputPath);
			Assert.IsNotNull(outputDoc.SelectSingleNode("configuration[@version='2']"));
		}

		[Test]
		public void DoesMigrateV1File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='1'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			bool didMigrate = WeSay.Project.WeSayWordsProject.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
			 Assert.IsTrue(didMigrate);
			AssertXPathNotNull("configuration[@version='2']", _outputPath);
		}

		[Test]
		public void DoesNotTouchV2File()
		{
			File.WriteAllText(_pathToInputConfig,
							  "<?xml version='1.0' encoding='utf-8'?><configuration version='2'></configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			bool didMigrate = WeSay.Project.WeSayWordsProject.MigrateConfigurationXmlIfNeeded(doc, _outputPath);
			Assert.IsFalse(didMigrate);
		}

		private void AssertXPathNotNull(string xpath, string filePath)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(filePath);
			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
				Console.WriteLine(File.ReadAllText(filePath));
			}
			XmlNode node = doc.SelectSingleNode(xpath);
			if (node == null)
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.ConformanceLevel = ConformanceLevel.Fragment;
				XmlWriter writer = XmlTextWriter.Create(Console.Out, settings);
				doc.WriteContentTo(writer);
				writer.Flush();
			}
			Assert.IsNotNull(node);
		}
	}

}