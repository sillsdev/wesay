using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;
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
			ReplaceWritingSystemIdsAtXPath("//writingSystems/id", oldId, newId);
			ReplaceWritingSystemIdsAtXPath("//writingSystemsToMatch", oldId, newId);
			ReplaceWritingSystemIdsAtXPath("//writingSystemsWhichAreRequired", oldId, newId);
			ReplaceWritingSystemIdsAtXPath("//EnglishLanguageWritingSystemId", oldId, newId);
			ReplaceWritingSystemIdsAtXPath("//RegionalLanguageWritingSystemId", oldId, newId);
			ReplaceWritingSystemIdsAtXPath("//NationalLanguageWritingSystemId", oldId, newId);
			ReplaceWritingSystemIdsAtXPath("//VernacularLanguageWritingSystemId", oldId, newId);
			_xmlDocument.Save(_configFilePath);
		}

		private void ReplaceWritingSystemIdsAtXPath(string xPath, string oldId, string newId)
		{
			XmlNodeList writingSystemIdNodes = _xmlDocument.SelectNodes(xPath);
			foreach (XmlNode writingsystemidNode in writingSystemIdNodes)
			{
				var newIds = new StringBuilder();
				foreach (var writingSystemId in writingsystemidNode.InnerText.Split(','))
				{
					var trimmedWritingSystemId = writingSystemId.Trim();
					//If we already have a writingSystemId append a comma
					if (newIds.Length != 0)
					{
						newIds.Append(", ");
					}

					if (trimmedWritingSystemId.Equals(oldId))
					{
						newIds.Append(newId);
						continue;
					}
					newIds.Append(trimmedWritingSystemId);
				}
				writingsystemidNode.InnerText = newIds.ToString();
			}
		}

		public IEnumerable<string> WritingSystemsInUse()
		{
				IEnumerable<string> fieldWritingsystems = _xmlDocument.SelectNodes("/configuration/components/viewTemplate/fields/field/writingSystems/id").Cast<XmlNode>().Select(node => node.InnerXml);
				IEnumerable<string> taskWritingSystems = _xmlDocument.SelectNodes("/configuration/tasks/task/writingSystemsToMatch").Cast<XmlNode>().Concat(
										 _xmlDocument.SelectNodes("/configuration/tasks/task/writingSystemsWhichAreRequired").Cast<XmlNode>()).
										 SelectMany(node=>node.InnerXml.Split(',').Select(str => str.Trim()));
				IEnumerable<string> sfmExportWritingSystems =
					_xmlDocument.SelectNodes("/configuration/addins/addin/SfmTransformSettings/EnglishLanguageWritingSystemId").Cast<XmlNode>().Concat(
					_xmlDocument.SelectNodes("/configuration/addins/addin/SfmTransformSettings/NationalLanguageWritingSystemId").Cast<XmlNode>()).Concat(
					_xmlDocument.SelectNodes("/configuration/addins/addin/SfmTransformSettings/RegionalLanguageWritingSystemId").Cast<XmlNode>()).Concat(
					_xmlDocument.SelectNodes("/configuration/addins/addin/SfmTransformSettings/VernacularLanguageWritingSystemId").Cast<XmlNode>()).
					Select(node => node.InnerXml); ;
				return fieldWritingsystems.Concat(taskWritingSystems).Concat(sfmExportWritingSystems).Distinct().Where(str => !String.IsNullOrEmpty(str));
		}

		public void CreateWritingSystemsForIdsInFileWhereNecassary(string pathToWritingSystemsFolder)
		{
			var writingSystemRepository =
				new LdmlInFolderWritingSystemRepository(pathToWritingSystemsFolder);
			OrphanFinder.FindOrphans(WritingSystemsInUse, ReplaceWritingSystemId, writingSystemRepository);
		}
	}
}