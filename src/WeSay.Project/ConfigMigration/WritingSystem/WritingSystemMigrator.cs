using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Palaso.IO;
using Palaso.Reporting;
using Palaso.WritingSystems.Migration;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;

namespace WeSay.Project.ConfigMigration.WritingSystem
{
	public class WritingSystemsMigrator
	{
		private delegate void DelegateThatTouchesFile(string filePath);
		List<string> _pathsToConfigFiles = new List<string>();
		List<string> _pathsToLiftFiles = new List<string>();

		public WritingSystemsMigrator(string projectPath)
		{
			ProjectPath = projectPath;
		}

		protected string ProjectPath { get; private set; }

		public string WritingSystemsPath
		{
			get { return Path.Combine(ProjectPath, "WritingSystems"); }
		}

		public string WritingSystemsOldPrefsFilePath
		{
			get { return Path.Combine(ProjectPath, "WritingSystemPrefs.xml"); }
		}

		public void MigrateIfNecessary()
		{
			_pathsToConfigFiles.AddRange(Directory.GetFiles(ProjectPath, "*.WeSayConfig"));
			_pathsToLiftFiles.AddRange(Directory.GetFiles(ProjectPath, "*.lift"));
			var oldMigrator = new WritingSystemPrefsMigrator(WritingSystemsOldPrefsFilePath, OnWritingSystemTagChange);
			oldMigrator.MigrateIfNecassary();
			var ldmlMigrator = new LdmlInFolderWritingSystemRepositoryMigrator(WritingSystemsPath, OnWritingSystemTagChange);
			ldmlMigrator.Migrate();
		}

		public void OnWritingSystemTagChange(IEnumerable<LdmlVersion0MigrationStrategy.MigrationInfo> newToOldTagMap)
		{
			//Only change rfcTags in files where they have actually changed
			foreach (var oldAndNewRfcTag in newToOldTagMap.Where(m => !m.RfcTagBeforeMigration.Equals(m.RfcTagAfterMigration)))
			{
				//foreach (var liftFilePath in _pathsToLiftFiles)
				//{
				//    RenameWritingSystemTagInFile(liftFilePath, "WeSay Dictionary File", (pathToFileToChange) =>
				//                                                                        //todo: expand the regular expression here to account for all reasonable patterns
				//                                                                        FileUtils.GrepFile(
				//                                                                            pathToFileToChange,
				//                                                                            string.Format(
				//                                                                                @"lang\s*=\s*[""']{0}[""']",
				//                                                                                Regex.Escape(
				//                                                                                    oldAndNewRfcTag.
				//                                                                                        RfcTagBeforeMigration)),
				//                                                                            string.Format(
				//                                                                                @"lang=""{0}""",
				//                                                                                oldAndNewRfcTag.RfcTagAfterMigration)
				//                                                                            )
				//    );
				//}
				foreach (var configFilepath in _pathsToConfigFiles)
				{
					string tempFile = Path.GetTempFileName();
					File.Copy(configFilepath, tempFile, true);
					int versionOfConfigFileInWhichWeCanRenameRfcTags = 8;
					XmlDocument configFileXmlDocument = new XmlDocument();

					configFileXmlDocument.Load(configFilepath);

					XmlNode versionNode = configFileXmlDocument.SelectSingleNode("configuration/@version");
					if ((versionNode == null) || (Convert.ToInt32(versionNode.Value) != versionOfConfigFileInWhichWeCanRenameRfcTags))
					{
						throw new ApplicationException(
							"Some writing system Rfc tags were changed during writingsystem migration and WeSay needs to update your '.WeSayConfig' file. However, that file is not version {0} and so WeSay cannot make the necassary changes.");
					}

					ReplaceWritingSystemIdsAtXPath(configFileXmlDocument, "//writingSystems/id", oldAndNewRfcTag);
					ReplaceWritingSystemIdsAtXPath(configFileXmlDocument, "//writingSystemsToMatch", oldAndNewRfcTag);
					ReplaceWritingSystemIdsAtXPath(configFileXmlDocument, "//writingSystemsWhichAreRequired", oldAndNewRfcTag);

					configFileXmlDocument.Save(tempFile);
					SafelyMoveTempFileTofinalDestination(tempFile, configFilepath);
					}
			}
		}

		private void ReplaceWritingSystemIdsAtXPath(XmlDocument configFileXmlDocument, string xPath, LdmlVersion0MigrationStrategy.MigrationInfo oldAndNewRfcTag)
		{
			XmlNodeList writingSystemIdNodes;
			writingSystemIdNodes = configFileXmlDocument.SelectNodes(xPath);
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

					if (trimmedWritingSystemId.Equals(oldAndNewRfcTag.RfcTagBeforeMigration))
					{
						newIds.Append(oldAndNewRfcTag.RfcTagAfterMigration);
						continue;
					}
					newIds.Append(trimmedWritingSystemId);
				}
				writingsystemidNode.InnerText = newIds.ToString();
			}
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
	}
}
