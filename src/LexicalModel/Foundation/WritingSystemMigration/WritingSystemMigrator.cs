using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palaso.Migration;
using Palaso.WritingSystems.Migration;

namespace WeSay.LexicalModel.Foundation.WritingSystemMigration
{
	public class WritingSystemMigrator
	{
		private Dictionary<string, string> _oldToNewRfcTagMap;

		public void MigrateIfNecassary(string pathToWritingSystemPrefsFile, string pathToWritingSystemRepo, ConsumerLevelRfcTagChanger rfcTagChanger)
		{
			var weSayWsPrefsToLdmlMigrator = new Migrator(1, pathToWritingSystemPrefsFile);
			weSayWsPrefsToLdmlMigrator.AddVersionStrategy(new WritingSystemPrefsVersionGetter());
			weSayWsPrefsToLdmlMigrator.AddMigrationStrategy(new WesayWsPrefsToPalasoWsLdmlMigrationStrategy(UpdateOldToNewRfcTagMap));
			weSayWsPrefsToLdmlMigrator.Migrate();
			var ldmlMigrator = new LdmlInFolderWritingSystemRepositoryMigrator(pathToWritingSystemRepo, UpdateOldToNewRfcTagMap);
			ldmlMigrator.Migrate();
			rfcTagChanger(_oldToNewRfcTagMap);
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
