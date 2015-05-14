using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SIL.Migration;
using SIL.WritingSystems;

namespace WeSay.Project.ConfigMigration.UserConfig
{
	public class WeSayUserConfigMigrator
	{
		private readonly FileMigrator _migrator;

		public WeSayUserConfigMigrator(string filePath, LdmlInFolderWritingSystemRepository writingSystemRepository)
		{
			_migrator = new FileMigrator(3, filePath);
			_migrator.AddVersionStrategy(new XPathVersion(99, "/configuration/@version"));
			_migrator.AddMigrationStrategy(new XslFromResourceMigrator(1, 2, "UserConfig.Migrate1To2.xsl"));
			_migrator.AddMigrationStrategy(new WeSayUserConfigV2MigrationStrategy(writingSystemRepository));
		}

		public void MigrateIfNeeded()
		{
			if (File.Exists(_migrator.SourceFilePath))
			{
				if (_migrator.NeedsMigration())
				{
					_migrator.Migrate();
				}
			}
		}

	}
}
