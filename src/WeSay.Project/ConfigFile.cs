using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration;
using WeSay.Project.ConfigMigration.WeSayConfig;

namespace WeSay.Project
{
	public class ConfigurationFileTooNewException: ApplicationException
	{
		public ConfigurationFileTooNewException(int currentCodeVersion, int fileVersion)
			: base(string.Format("This configuration file is version {0}, but this version of WeSay can only handle version {1}. Please download a newer version of wesay from wesay.org", fileVersion,currentCodeVersion))
		{

		}

	}

	public class ConfigFile
	{
		private readonly Dictionary<string, string> _oldToNewIdMap = new Dictionary<string, string>();
		public const int LatestVersion = 8;
		private readonly XmlDocument _xmlDocument = new XmlDocument();

		private string _configFilePath;

		public ConfigFile(string configFilePath)
		{
			_configFilePath = configFilePath;
			_xmlDocument.Load(configFilePath);
			if (Version > LatestVersion)
			{
				throw new ConfigurationFileTooNewException(LatestVersion,Version);
			}
		}

		public string ConfigFilePath
		{
			get { return _configFilePath; }
			set { _configFilePath = value; }
		}

		public int Version
		{
			get
			{
				XmlNode versionNode = _xmlDocument.SelectSingleNode("configuration/@version");
				if(versionNode == null)
				{
					return 0;
				}
				return Convert.ToInt32(versionNode.Value);
			}
		}

		public bool MigrateIfNecassary()
		{
			var m = new ConfigurationMigrator();
			bool migrationWasSuccessful = m.MigrateConfigurationXmlIfNeeded(_configFilePath, _configFilePath);
			_xmlDocument.Load(_configFilePath);
			return migrationWasSuccessful;
		}

		public void ReplaceWritingSystemId(string oldId, string newId)
		{
			string pathToConfigFile = _configFilePath;
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

		public void CreateNonExistentWritingSystemsFoundInConfigFile(string pathToWritingSystemsFolder)
		{
				XmlNode versionNode = _xmlDocument.SelectSingleNode("configuration/@version");
				if (versionNode == null)
				{
					throw new ApplicationException("Unable to determine the config file version.");
				}
				int versionOf_xmlDocument = Convert.ToInt32(versionNode.Value);
				//versions 8 and 9 are structurally identical
				if ((versionOf_xmlDocument != 8) && (versionOf_xmlDocument != 9))
				{
					throw new ConfigurationFileTooNewException(8, versionOf_xmlDocument);
				}

				//this section fixes all the writingsystems in the fields section
				var fieldWritingSystemNodes =
					_xmlDocument.SelectNodes("/configuration/components/viewTemplate/fields/field/writingSystems/id");
				if (fieldWritingSystemNodes != null)
				{
					foreach (XmlNode node in fieldWritingSystemNodes)
					{
						node.InnerXml = GetNewId(node.InnerXml);
					}
				}

				//this section fixes all the writing systems in the tasks section
				var tasksWritingSystemNodes = _xmlDocument.SelectNodes("/configuration/tasks/task/writingSystemsToMatch").Cast<XmlNode>().Concat(_xmlDocument.SelectNodes("/configuration/tasks/task/writingSystemsWhichAreRequired").Cast<XmlNode>());
				if (tasksWritingSystemNodes != null)
				{
					foreach (XmlNode node in tasksWritingSystemNodes)
					{
						//The innerxml is a comma separated list so we need to split and trim
						node.InnerXml = String.Join(", ", node.InnerXml.Split(',').Select(str => str.Trim()).Select(str => GetNewId(str)).ToArray());
					}
				}

				//this section  fixes the writing systems in the sfm export task
				var sfmExportWritingSystemNodes =
					_xmlDocument.SelectNodes("/configuration/addins/addin/SfmTransformSettings/EnglishLanguageWritingSystemId").Cast<XmlNode>().Concat(
					_xmlDocument.SelectNodes("/configuration/addins/addin/SfmTransformSettings/NationalLanguageWritingSystemId").Cast<XmlNode>()).Concat(
					_xmlDocument.SelectNodes("/configuration/addins/addin/SfmTransformSettings/RegionalLanguageWritingSystemId").Cast<XmlNode>()).Concat(
					_xmlDocument.SelectNodes("/configuration/addins/addin/SfmTransformSettings/VernacularLanguageWritingSystemId").Cast<XmlNode>());
				if (sfmExportWritingSystemNodes != null)
				{
					foreach (XmlNode node in sfmExportWritingSystemNodes)
					{
						//The innerxml is a comma separated list so we need to split and trim
						node.InnerXml = GetNewId(node.InnerXml);
					}
				}

				_xmlDocument.Save(_configFilePath);

				var writingSystemRepo = new LdmlInFolderWritingSystemRepository(pathToWritingSystemsFolder);
				foreach (var newId in _oldToNewIdMap.Values)
				{
					if (!writingSystemRepo.Contains(newId))
					{
						writingSystemRepo.Set(WritingSystemDefinition.Parse(newId));
					}
				}
				writingSystemRepo.Save();
		}

		private string GetNewId(string currentWritingSystemId)
		{
			if (!_oldToNewIdMap.Keys.Contains(currentWritingSystemId))
			{
				var rfcTagCleaner = new Rfc5646TagCleaner(currentWritingSystemId);
				rfcTagCleaner.Clean();
				string newId = rfcTagCleaner.GetCompleteTag();
				if (_oldToNewIdMap.Values.Any(str => str.Equals(newId, StringComparison.OrdinalIgnoreCase)))
				{
					newId = MakeUniqueTag(newId, _oldToNewIdMap.Values);
				}
				_oldToNewIdMap[currentWritingSystemId] = newId;
			}
			return _oldToNewIdMap[currentWritingSystemId];
		}

		private static string MakeUniqueTag(string rfcTag, IEnumerable<string> uniqueRfcTags)
		{
			int duplicateNumber = 0;
			string newRfcTag;
			do
			{
				duplicateNumber++;
				newRfcTag = rfcTag + String.Format("-dupl{0}", duplicateNumber);
			} while (uniqueRfcTags.Any(s => s.Equals(newRfcTag, StringComparison.OrdinalIgnoreCase)));
			return newRfcTag;
		}
	}
}