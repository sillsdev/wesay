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

		[SetUp]
		public void Setup()
		{
			_pathToInputConfig = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(_pathToInputConfig);
		}

		[Test]
		public void DoesMigrateV0File()
		{
			File.WriteAllText(_pathToInputConfig, "<?xml version='1.0' encoding='utf-8'?><tasks><components><viewTemplate></viewTemplate></components><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			string outputPath = Path.GetTempFileName();
			bool didMigrate = WeSay.Project.WeSayWordsProject.MigrateConfigurationXmlIfNeeded(doc, outputPath);

			Assert.IsTrue(didMigrate);
			XmlDocument outputDoc = new XmlDocument();
			outputDoc.Load(outputPath);
			Assert.IsNotNull(outputDoc.SelectSingleNode("configuration[@version='1']"));
			File.Delete(outputPath);
		}

		[Test]
		public void DoesNotTouchV1File()
		{
			File.WriteAllText(_pathToInputConfig, "<?xml version='1.0' encoding='utf-8'?><configuration version='1'><components><viewTemplate></viewTemplate></components><tasks><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>");
			XPathDocument doc = new XPathDocument(_pathToInputConfig);
			string outputPath = Path.GetTempFileName();
			bool didMigrate = WeSay.Project.WeSayWordsProject.MigrateConfigurationXmlIfNeeded(doc, outputPath);
			File.Delete(outputPath);
			Assert.IsFalse(didMigrate);
		}
	}

}