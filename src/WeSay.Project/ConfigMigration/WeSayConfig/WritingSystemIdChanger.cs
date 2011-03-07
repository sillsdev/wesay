using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Palaso.Migration;

namespace WeSay.Project.ConfigMigration.WeSayConfig
{
	public class WritingSystemIdChanger
	{
		private string _pathToConfigFile;

		public WritingSystemIdChanger(string pathToConfigFile)
		{
			if(!File.Exists(pathToConfigFile)){throw new ArgumentOutOfRangeException("please provide a valid path to the config file.");}
			_pathToConfigFile = pathToConfigFile;
		}

		public void ChangeWritingSystemIdInConfigFile(Dictionary<string, string> oldidToNewIdmap)
		{
			string tempFile = Path.GetTempFileName();
			File.Copy(_pathToConfigFile, tempFile, true);
			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.Load(tempFile);
			XmlNode versionNode = xmlDoc.SelectSingleNode("configuration/@version");
			string versionNumber = versionNode.Value;
			switch(versionNumber)
			{
				case "7":
					XmlNodeList writingSystemIdNodes = xmlDoc.SelectNodes("//writingSystems/id");
					foreach (XmlNode writingsystemidNode in writingSystemIdNodes)
					{
						if (oldidToNewIdmap.ContainsKey(writingsystemidNode.InnerText))
						{
							writingsystemidNode.InnerText = oldidToNewIdmap[writingsystemidNode.InnerText];
						}
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(String.Format("Can not rename writing systems in config files of version {0}.", versionNumber));
			}

			xmlDoc.Save(tempFile);
			SafelyMoveTempFileTofinalDestination(tempFile, _pathToConfigFile);
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
