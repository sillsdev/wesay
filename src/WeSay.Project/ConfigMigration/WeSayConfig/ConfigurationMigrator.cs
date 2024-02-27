using SIL.Reporting;
using SIL.Xml;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace WeSay.Project.ConfigMigration.WeSayConfig
{
	public class ConfigurationMigrator
	{
		public bool MigrateConfigurationXmlIfNeeded(string pathToConfigFile, string targetPath)
		{
			XPathDocument configurationDoc = GetConfigurationFileAsXPathDocument(pathToConfigFile);

			Logger.WriteEvent("Checking if migration of configuration is needed.");

			bool didMigrate = false;

			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration") == null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig0To1.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (
					configurationDoc.CreateNavigator().SelectSingleNode(
							"configuration[@version='1']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig1To2.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='2']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig2To3.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='3']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig3To4.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='4']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig4To5.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='5']") != null)
			{
				var m = new MigrateConfig5To6();
				m.Migrate(configurationDoc, targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='6']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig6To7.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='7']") != null)
			{
				MigrateInCode(configurationDoc, targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='8']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig8To9.xsl", targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			if (configurationDoc.CreateNavigator().SelectSingleNode("configuration[@version='9']") != null)
			{
				MigrateUsingXSLT(configurationDoc, "MigrateConfig9To10.xsl", targetPath);
				//configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			return didMigrate;

		}

		private static void MigrateInCode(XPathDocument configurationDoc, string targetPath)
		{
			XPathNavigator navigator = configurationDoc.CreateNavigator();

			string tempFilePath = Path.GetTempFileName();
			var settings = CanonicalXmlSettings.CreateXmlWriterSettings();
			var writer = XmlWriter.Create(tempFilePath, settings);

			using (writer)
			{
				writer.WriteStartDocument();
				navigator.MoveToFirstChild();

				if (navigator.Name != "configuration") { throw new ApplicationException("The configuration file does not have the expected format. 'configuration' was not the first node found."); }

				writer.WriteStartElement("configuration");
				writer.WriteAttributeString("version", "8");
				CopyChildren(writer, navigator);
			}
			SafelyMoveTempFileTofinalDestination(tempFilePath, targetPath);
		}

		private static void CopyChildren(XmlWriter writer, XPathNavigator navigator)
		{
			navigator.MoveToFirstChild();
			do
			{
				writer.WriteNode(navigator, true);
			} while (navigator.MoveToNext());
		}

		private static XPathDocument GetConfigurationFileAsXPathDocument(string pathToConfigFile)
		{
			XPathDocument configurationDoc = null;
			if (File.Exists(pathToConfigFile))
			{
				try
				{
					configurationDoc = new XPathDocument(pathToConfigFile);
					//projectDoc.Load(Project.PathToConfigFile);
				}
				catch (Exception e)
				{
					ErrorReport.NotifyUserOfProblem("There was a problem reading the wesay config xml: " + e.Message);
					configurationDoc = null;
				}
			}
			return configurationDoc;
		}

		public static void MigrateUsingXSLT(IXPathNavigable configurationDoc,
											 string xsltName,
											 string targetPath)
		{
			Logger.WriteEvent("Migrating Configuration File {0}", xsltName);
			var resourceName = "ConfigMigration.WeSayConfig." + xsltName;
			using (
					Stream stream =
							Assembly.GetExecutingAssembly().GetManifestResourceStream(
									typeof(WeSayWordsProject), resourceName))
			{
				if (stream == null)
				{
					throw new ApplicationException("Could not find the resource " + resourceName);
				}
				var transform = new XslCompiledTransform();
				using (var reader = XmlReader.Create(stream))
				{
					transform.Load(reader);
					string tempPath = Path.GetTempFileName();
					using (var writer = XmlWriter.Create(tempPath, CanonicalXmlSettings.CreateXmlWriterSettings()))
					{
						transform.Transform(configurationDoc, writer);
						writer.Close();
					}
					SafelyMoveTempFileTofinalDestination(tempPath, targetPath);
				}
			}
		}

		private static void SafelyMoveTempFileTofinalDestination(string tempPath, string targetPath)
		{
			string s = targetPath + ".tmp";
			if (File.Exists(s))
			{
				File.Delete(s);
			}
			if (File.Exists(targetPath)) //review: JDH added this because of a failing test, and from my reading, the target shouldn't need to pre-exist
			{
				File.Move(targetPath, s);
			}
			File.Move(tempPath, targetPath);
			File.Delete(s);
		}
	}
}
