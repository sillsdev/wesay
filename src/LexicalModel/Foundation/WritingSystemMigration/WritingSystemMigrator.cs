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
		private readonly string _sourceFilePath;
		private WesayWsPrefsToPalasoWsLdmlMigrationStrategy.OnMigrationFn _rfcTagChanger;

		public WritingSystemMigrator(int versionToMigrateTo, string sourceFilePath, WesayWsPrefsToPalasoWsLdmlMigrationStrategy.OnMigrationFn rfcTagChanger)
			: base(versionToMigrateTo)
		{
			_rfcTagChanger = rfcTagChanger;
			_sourceFilePath = sourceFilePath;
		}

		public void Migrate()
		{
			string _backupFilePath = _sourceFilePath + ".bak";
			var filesToDelete = new List<string>();
			var directoriesToDelete = new List<string>();
			if (File.Exists(_backupFilePath))
			{
				File.Delete(_backupFilePath);
			}
			File.Copy(_sourceFilePath, _backupFilePath);
			filesToDelete.Add(_backupFilePath);
			int currentVersion = new WritingSystemPrefsVersionGetter().GetFileVersion(_sourceFilePath);
			if (currentVersion == 0)
			{
				string pathToWritingSystemRepoToCreate = Path.Combine(Path.GetDirectoryName(_sourceFilePath), "WritingSystems");
				var strategy = new WesayWsPrefsToPalasoWsLdmlMigrationStrategy(_rfcTagChanger);
				string sourceFilePath = _sourceFilePath;
				string destinationFilePath = String.Format("{0}.Migrate_{1}_{2}", _sourceFilePath, strategy.FromVersion,
													strategy.ToVersion);
				strategy.Migrate(sourceFilePath, destinationFilePath);
				File.Delete(_sourceFilePath);
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
