using System;
using System.IO;
using System.Xml;
using WeSay.Project.ConfigMigration.WeSayConfig;

namespace WeSay.Project
{
	public class ConfigFile
	{
		public const int LatestVersion = 8;

		private string _path;

		public ConfigFile(string pathToconfigFile)
		{
			_path = pathToconfigFile;
			if (Version > LatestVersion)
			{
				throw new ApplicationException("The config file is too new for this version of wesay. Please download a newer version of wesay from www.wesay.org");
			}
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
				var configFile = new XmlDocument();
				configFile.Load(_path);
				XmlNode versionNode = configFile.SelectSingleNode("configuration/@version");
				if(versionNode == null)
				{
					return 0;
				}
				return Convert.ToInt32(versionNode.Value);
			}
		}

		public void MigrateIfNecassary()
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
			var xmlDoc = new XmlDocument();

			xmlDoc.Load(tempFile);
			switch (Version)
			{
				case 7:
					ReplaceAsInVersion7(xmlDoc, oldId, newId);
					break;
				case 8:
					ReplaceAsInVersion7(xmlDoc, oldId, newId);
					break;
				default:
					throw new ApplicationException(
						String.Format("Can not rename writing systems in config files of version {0}.", Version));
			}

			xmlDoc.Save(tempFile);
			SafelyMoveTempFileTofinalDestination(tempFile, pathToConfigFile);
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

		private static void ReplaceAsInVersion7(XmlDocument xmlDoc, string oldId, string newId)
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