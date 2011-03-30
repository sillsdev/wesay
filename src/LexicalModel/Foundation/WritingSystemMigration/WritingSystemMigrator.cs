using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Palaso.Migration;
using Palaso.WritingSystems.Migration;

namespace WeSay.LexicalModel.Foundation.WritingSystemMigration
{
	public class WritingSystemMigrator:Migrator
	{
		private Dictionary<string, string> _oldToNewRfcTagMap = new Dictionary<string, string>();
		private ConsumerLevelRfcTagChanger _rfcTagChanger;
		private string _pathToSourceFile;

		public WritingSystemMigrator(int versionToMigrateTo, string sourceFilePath, ConsumerLevelRfcTagChanger rfcTagChanger) : base(versionToMigrateTo, sourceFilePath)
		{
			_rfcTagChanger = rfcTagChanger;
			_pathToSourceFile = sourceFilePath;
		}

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
				var strategy = new WesayWsPrefsToPalasoWsLdmlMigrationStrategy(UpdateOldToNewRfcTagMap);
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
			_rfcTagChanger(_oldToNewRfcTagMap);
		}

		void UpdateOldToNewRfcTagMap(Dictionary<string, string> oldToNewRfcTagMapFromSingleStrategy)
		{
			foreach (var oldandNewRfcTagPair in oldToNewRfcTagMapFromSingleStrategy)
			{
				_oldToNewRfcTagMap[oldandNewRfcTagPair.Key] = oldandNewRfcTagPair.Value;
			}
		}
	}
}
