using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Palaso.Migration;

namespace WeSay.Project.ConfigMigration.UserConfig
{
	public class WeSayUserConfigMigrator
	{
		private Migrator _migrator;

		public WeSayUserConfigMigrator(string filePath)
		{
			_migrator = new Migrator(2, filePath);
			_migrator.AddVersionStrategy(new XPathVersion(99, "/configuration/@version"));
			_migrator.AddMigrationStrategy(new XslFromResourceMigrator(1, 2, "UserConfig.Migrate1To2.xsl"));
		}

		public void MigrateIfNeeded()
		{
			if (_migrator.NeedsMigration())
			{
				_migrator.Migrate();
			}
		}

	}
}
