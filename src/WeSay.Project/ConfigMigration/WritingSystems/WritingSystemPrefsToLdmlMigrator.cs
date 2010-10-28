using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Palaso.Migration;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project.ConfigMigration.WritingSystems
{
	public class WritingSystemPrefsToLdmlMigrator
	{
		private string _pathToProjectFolder;

		public WritingSystemPrefsToLdmlMigrator(string pathToProjectFolder)
		{
			_pathToProjectFolder = pathToProjectFolder;
		}

		public void MigrateIfNeeded()
		{
			string pathToWritingSystemPrefsFile = BasilProject.GetPathToWritingSystemPrefs(_pathToProjectFolder);
			if (File.Exists(pathToWritingSystemPrefsFile))
			{
				string pathToLdmlWritingSystemsFolder = BasilProject.GetPathToLdmlWritingSystemsFolder(_pathToProjectFolder);
				WritingSystemCollection writingSystems = new WritingSystemCollection();
				writingSystems.LoadFromLegacyWeSayFile(pathToWritingSystemPrefsFile);
				writingSystems.Write(pathToLdmlWritingSystemsFolder);
				File.Delete(pathToWritingSystemPrefsFile);
			}
		}
	}
}