using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Palaso.Reporting;
using WeSay.Project.ConfigMigration.WritingSystems;

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
				string pathToMostRecentConfigFile = pathToConfigFile;
				if (didMigrate){pathToMostRecentConfigFile = targetPath;}
				MigrateInCodeFromVersion7To8(pathToMostRecentConfigFile, targetPath);
				configurationDoc = new XPathDocument(targetPath);
				didMigrate = true;
			}
			return didMigrate;

		}

		private XPathDocument GetConfigurationFileAsXPathDocument(string pathToConfigFile)
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

		private void MigrateInCodeFromVersion7To8(string pathToConfigFile, string targetPath)
		{
			string pathToProject = Path.GetDirectoryName(targetPath);
			var writingSystemPrefsToLdmlMigrator = new WritingSystemPrefsToLdmlMigrator(pathToProject);
			var oldidToNewIdMap = writingSystemPrefsToLdmlMigrator.MigrateIfNeeded();

			WritingSystemIdChanger idInConfigFileChanger = new WritingSystemIdChanger(pathToConfigFile);
			idInConfigFileChanger.ChangeWritingSystemIdInConfigFile(oldidToNewIdMap);

			string tempFilePath = Path.GetTempFileName();

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(pathToConfigFile);
			XmlNode versionNode = xmlDoc.SelectSingleNode("configuration/@version");
			string versionNumber = versionNode.Value;
			if(versionNumber!= "7"){throw new ApplicationException("Expected a config file version 7.");}
			versionNode.Value = "8";
			xmlDoc.Save(tempFilePath);

			SafelyMoveTempFileTofinalDestination(tempFilePath, targetPath);
		}

		private void CopyChildren(XmlWriter writer, XPathNavigator navigator)
		{
			navigator.MoveToFirstChild();
			do
			{
				writer.WriteNode(navigator, true);
			} while (navigator.MoveToNext());
		}

		public static void MigrateUsingXSLT(IXPathNavigable configurationDoc,
											 string xsltName,
											 string targetPath)
		{
			Logger.WriteEvent("Migrating Configuration File {0}", xsltName);
			var resourceName = "ConfigMigration.WeSayConfig."+ xsltName;
			using (
					Stream stream =
							Assembly.GetExecutingAssembly().GetManifestResourceStream(
									typeof(WeSayWordsProject), resourceName))
			{
				if(stream==null)
				{
					throw new ApplicationException("Could not find the resource "+resourceName);
				}
				XslCompiledTransform transform = new XslCompiledTransform();
				using (XmlReader reader = XmlReader.Create(stream))
				{
					transform.Load(reader);
					string tempPath = Path.GetTempFileName();
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Indent = true;
					using (XmlWriter writer = XmlWriter.Create(tempPath, settings))
					{
						transform.Transform(configurationDoc, writer);
						var tempfiles = transform.TemporaryFiles;
						if (tempfiles != null)
						// tempfiles will be null when debugging is not enabled
						{
							tempfiles.Delete();
						}
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
