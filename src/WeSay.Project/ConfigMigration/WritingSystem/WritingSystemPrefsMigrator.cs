using System;
using System.Collections.Generic;
using System.IO;
using Palaso.Migration;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;

namespace WeSay.Project.ConfigMigration.WritingSystem
{
	public class WritingSystemPrefsMigrator
	{
		private readonly string _sourceFilePath;
		private readonly LdmlVersion0MigrationStrategy.OnMigrationFn _onWritingSystemTagChange;

		public WritingSystemPrefsMigrator(string sourceFilePath, LdmlVersion0MigrationStrategy.OnMigrationFn onWritingSystemTagChange)
		{
			_onWritingSystemTagChange = onWritingSystemTagChange;
			_sourceFilePath = sourceFilePath;
		}

		public string WritingSystemsFolder(string basePath)
		{
			return Path.Combine(basePath, "WritingSystems");
		}

		public void MigrateIfNecassary()
		{
			if(!File.Exists(_sourceFilePath))
			{
				return;
			}
			string backupFilePath = _sourceFilePath + ".bak";
			var filesToDelete = new List<string>();
			var directoriesToDelete = new List<string>();
			if (File.Exists(backupFilePath))
			{
				File.Delete(backupFilePath);
			}
			File.Copy(_sourceFilePath, backupFilePath);
			filesToDelete.Add(backupFilePath);

			string ldmlPath = WritingSystemsFolder(Path.GetDirectoryName(_sourceFilePath));

			//hack because I need this to work for the workshop tomorrow, but it needs more thought/testing
			if (!Directory.Exists(ldmlPath))
			{
				var strategy = new WritingSystemPrefsToLdmlMigrationStrategy(_onWritingSystemTagChange);
				string sourceFilePath = _sourceFilePath;
				string destinationFilePath = String.Format("{0}.Migrate_{1}_{2}", _sourceFilePath, strategy.FromVersion,
														   strategy.ToVersion);
				strategy.Migrate(sourceFilePath, destinationFilePath);

				Directory.Move(destinationFilePath, ldmlPath);
			}

			File.Delete(_sourceFilePath);

			foreach (var filePath in filesToDelete)
			{
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}
			foreach (var filePath in directoriesToDelete)
			{
				if (Directory.Exists(filePath))
				{
					Directory.Delete(filePath);
				}
			}
		}
	}
}
