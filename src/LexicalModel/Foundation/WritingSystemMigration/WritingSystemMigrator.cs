using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Palaso.Migration;
using Palaso.WritingSystems.Migration;

namespace WeSay.LexicalModel.Foundation.WritingSystemMigration
{
	public class WritingSystemMigrator:MigratorBase
	{
		private WesayWsPrefsToPalasoWsLdmlMigrationStrategy.OnMigrationFn _rfcTagChanger;
		private string _pathToSourceFile;

		public WritingSystemMigrator(int versionToMigrateTo, string sourceFilePath, WesayWsPrefsToPalasoWsLdmlMigrationStrategy.OnMigrationFn rfcTagChanger)
			: base(versionToMigrateTo)
		{
			_rfcTagChanger = rfcTagChanger;
			_pathToSourceFile = sourceFilePath;
		}

		private string BackupFilePath { get; set; }
		private string SourceFilePath { get; set; }

		public void Migrate()
		{
			var filesToDelete = new List<string>();
			var directoriesToDelete = new List<string>();
			if (File.Exists(BackupFilePath))
			{
				File.Delete(BackupFilePath);
			}
			File.Copy(SourceFilePath, BackupFilePath);
			filesToDelete.Add(BackupFilePath);
			int currentVersion = new WritingSystemPrefsVersionGetter().GetFileVersion(_pathToSourceFile);
			if (currentVersion == 0)
			{
				string pathToWritingSystemRepoToCreate = Path.Combine(Path.GetDirectoryName(SourceFilePath), "WritingSystems");
				var strategy = new WesayWsPrefsToPalasoWsLdmlMigrationStrategy(_rfcTagChanger);
				string sourceFilePath = SourceFilePath;
				string destinationFilePath = String.Format("{0}.Migrate_{1}_{2}", SourceFilePath, strategy.FromVersion,
													strategy.ToVersion);
				strategy.Migrate(sourceFilePath, destinationFilePath);
				File.Delete(SourceFilePath);
				Directory.Move(destinationFilePath, pathToWritingSystemRepoToCreate);
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
}
