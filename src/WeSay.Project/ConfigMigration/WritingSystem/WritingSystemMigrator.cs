using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using SIL.IO;
using SIL.Reporting;
using SIL.WritingSystems.Migration;
using SIL.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;

namespace WeSay.Project.ConfigMigration.WritingSystem
{
	public class WritingSystemsMigrator
	{
		private delegate void DelegateThatTouchesFile(string filePath);

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
			var oldMigrator = new WritingSystemPrefsMigrator(WritingSystemsOldPrefsFilePath, OnWritingSystemTagChange);
			oldMigrator.MigrateIfNecassary();
			var ldmlMigrator = new LdmlInFolderWritingSystemRepositoryMigrator(WritingSystemsPath, OnWritingSystemTagChange);
			ldmlMigrator.Migrate();
		}

		public void OnWritingSystemTagChange(int version, IEnumerable<LdmlMigrationInfo> newToOldTagMap)
		{
			//Only change rfcTags in files where they have actually changed
			foreach (var oldAndNewRfcTag in newToOldTagMap.Where(m => !m.LanguageTagBeforeMigration.Equals(m.LanguageTagAfterMigration)))
			{
				//The replacement strategy in this section should be specially tailored to the latest version of the '.lift' file. I.e. the lift file should always be migrated before the writing systems. If we up the lift file version this section may need to be updated.
				foreach (var liftFilePath in Directory.GetFiles(ProjectPath, "*.lift"))
				{
					string tempFile = Path.GetTempFileName();
					File.Copy(liftFilePath, tempFile, true);

					var reader = XmlReader.Create(tempFile);
					reader.MoveToContent();
					while(reader.NodeType != XmlNodeType.Element && reader.Name != "lift")
					{
						reader.Read();
					}

					const string versionOfLiftFileInWhichWeCanRenameRfcTags = "0.13";
					string versionNode = reader.GetAttribute("version");
					reader.Close();
					if (versionNode != versionOfLiftFileInWhichWeCanRenameRfcTags)
					{
						continue;
					}

					RenameWritingSystemTagInFile(tempFile, "WeSay Dictionary File", (pathToFileToChange) =>
						//todo: expand the regular expression here to account for all reasonable patterns
																					FileUtils.GrepFile(
																						pathToFileToChange,
																						string.Format(
																							@"lang\s*=\s*[""']{0}[""']",
																							Regex.Escape(
																								oldAndNewRfcTag.LanguageTagBeforeMigration)),
																						string.Format(
																							@"lang=""{0}""",
																							oldAndNewRfcTag.LanguageTagAfterMigration)
																						)
					);
					SafelyMoveTempFileTofinalDestination(tempFile, liftFilePath);
				}

				//The replacement strategy in this section should be specially tailored to the latest version of the '.WeSayConfig' file. I.e. the config file should always be migrated before the writing systems. If we up the config file version this section may need to be updated.
				foreach (var configFilepath in Directory.GetFiles(ProjectPath, "*.WeSayConfig"))
				{
					string tempFile = Path.GetTempFileName();
					File.Copy(configFilepath, tempFile, true);

					var configFile = new ConfigFile(tempFile);
					foreach (var oldAndNewId in newToOldTagMap)
					{
						configFile.ReplaceWritingSystemId(oldAndNewId.LanguageTagBeforeMigration, oldAndNewId.LanguageTagAfterMigration);
					}
					SafelyMoveTempFileTofinalDestination(tempFile, configFilepath);
				}

				//Now let's replace writing systems in OptionLists
				var xmlDoc = new XmlDocument();
				foreach (var filePath in Directory.GetFiles(ProjectPath))
				{
					try
					{
						xmlDoc.Load(filePath);
						if (xmlDoc.SelectSingleNode("/optionsList") != null)
						{
							foreach (XmlNode node in xmlDoc.SelectNodes("//form"))
							{
								if (node.Attributes["lang"].Value == oldAndNewRfcTag.LanguageTagBeforeMigration)
								{
									node.Attributes["lang"].Value = oldAndNewRfcTag.LanguageTagAfterMigration;
								}
							}
						}
						xmlDoc.Save(filePath);
					}
					catch(Exception e)
					{
						//Do nothing. If the load failed then it's not an optionlist.
					}
				}
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



		private bool RenameWritingSystemTagInFile(string filePath, string uiFileDescription, DelegateThatTouchesFile doSomething)
		{
			if (!File.Exists(filePath))
				return false;
			try
			{
				doSomething(filePath);
				return true;
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem("Another program has {0} open, so we cannot make the input system change.  Make sure no other instances of WeSay are running.\n\n\t'{1}'", uiFileDescription, filePath);
				return false;
			}
		}
	}
}
