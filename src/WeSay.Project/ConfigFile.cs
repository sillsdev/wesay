using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using WeSay.Project.ConfigMigration.WeSayConfig;

namespace WeSay.Project
{
	public class ConfigFile
	{
		private string _path;

		public ConfigFile(string pathToconfigFile)
		{
			_path = pathToconfigFile;
			if (Version > LatestVersion)
			{
				throw new ApplicationException("The config file is too new for this version of wesay. Please download a newer version of wesay from www.wesay.org");
			}
			MigrateIfNecassary();
		}

		public int LatestVersion
		{
			get { return 9; }
		}

		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		public int Version
		{
			get
			{
				XmlDocument configFile = new XmlDocument();
				configFile.Load(_path);
				XmlNode versionNode = configFile.SelectSingleNode("configuration/@version");
				if(versionNode == null)
				{
					return 0;
				}
				return Convert.ToInt32(versionNode.Value);
			}
		}

		private void MigrateIfNecassary()
		{
			var m = new ConfigurationMigrator();
			Console.WriteLine("{0}", _path);
			m.MigrateConfigurationXmlIfNeeded(_path, _path);
		}

		public void ReplaceWritingSystemId(string oldId, string newId)
		{
			string pathToConfigFile = _path;
			string tempFile = System.IO.Path.GetTempFileName();
			File.Copy(pathToConfigFile, tempFile, true);
			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.Load(tempFile);
			switch (Version)
			{
				case 7:
					ReplaceAsInVersion7(xmlDoc, oldId, newId);
					break;
				case 8:
					ReplaceAsInVersion7(xmlDoc, oldId, newId);
					break;
				case 9:
					ReplaceAsInVersion7(xmlDoc, oldId, newId);
					break;
				default:
					throw new ApplicationException(
						String.Format("Can not rename writing systems in config files of version {0}.", Version));
			}

			xmlDoc.Save(tempFile);
			SafelyMoveTempFileTofinalDestination(tempFile, pathToConfigFile);
		}

		private void SafelyMoveTempFileTofinalDestination(string tempPath, string targetPath)
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

		private void ReplaceAsInVersion7(XmlDocument xmlDoc, string oldId, string newId)
		{
			XmlNodeList writingSystemIdNodes = xmlDoc.SelectNodes("//writingSystems/id");
			foreach (XmlNode writingsystemidNode in writingSystemIdNodes)
			{
				if (writingsystemidNode.InnerText == oldId)
				{
					writingsystemidNode.InnerText = newId;
				}
			}
		}
	}
}