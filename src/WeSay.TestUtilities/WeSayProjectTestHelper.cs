using System;
using System.Collections.Generic;
using System.IO;
using SIL.Lexicon;
using SIL.WritingSystems;
using SIL.WritingSystems.Migration;
using SIL.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;
using WeSay.Project;

namespace WeSay.TestUtilities
{
	public class WeSayProjectTestHelper
	{
		/// <summary>
		/// See comment on BasilProject.InitializeForTests()
		/// </summary>
		public static WeSayWordsProject InitializeForTests()
		{
			WeSayWordsProject project = new WeSayWordsProject();

			try
			{
				File.Delete(WeSayWordsProject.PathToPretendLiftFile);
			}
			catch (Exception) { }
			DirectoryInfo projectDirectory = Directory.CreateDirectory(Path.GetDirectoryName(WeSayWordsProject.PathToPretendLiftFile));
			string pathToLdmlWsFolder = BasilProject.GetPathToLdmlWritingSystemsFolder(projectDirectory.FullName);

			if (File.Exists(WeSayWordsProject.PathToPretendWritingSystemPrefs))
			{
				File.Delete(WeSayWordsProject.PathToPretendWritingSystemPrefs);
			}

			if (Directory.Exists(pathToLdmlWsFolder))
			{
				Directory.Delete(pathToLdmlWsFolder, true);
			}

			Palaso.Lift.Utilities.CreateEmptyLiftFile(WeSayWordsProject.PathToPretendLiftFile, "InitializeForTests()", true);

			//setup writing systems
			Directory.CreateDirectory(pathToLdmlWsFolder);

			var userMemorySettingsStore = new MemorySettingsStore();
			var projectMemorySettingsStore = new MemorySettingsStore();

			var userSettingsDataMapper = new UserLexiconSettingsWritingSystemDataMapper(userMemorySettingsStore);
			var projectSettingsDataMapper = new ProjectLexiconSettingsWritingSystemDataMapper(projectMemorySettingsStore);
			var customDataMapper = new ICustomDataMapper<WritingSystemDefinition>[]
			{
				userSettingsDataMapper,
				projectSettingsDataMapper
			};

			IWritingSystemRepository wsc = LdmlInFolderWritingSystemRepository.Initialize(
				pathToLdmlWsFolder,
				customDataMapper,
				null,
				OnMigrationHandler,
				OnWritingSystemLoadProblem
			);

			wsc.Set(new WritingSystemDefinition(WritingSystemsIdsForTests.VernacularIdForTest));
			wsc.Set(new WritingSystemDefinition(WritingSystemsIdsForTests.AnalysisIdForTest));
			wsc.Set(new WritingSystemDefinition(WritingSystemsIdsForTests.OtherIdForTest));

			wsc.Save();

			project.SetupProjectDirForTests(WeSayWordsProject.PathToPretendLiftFile);
			project.BackupMaker = null;//don't bother. Modern tests which might want to check backup won't be using this old approach anyways.
			return project;
		}

		private static void OnWritingSystemLoadProblem(IEnumerable<WritingSystemRepositoryProblem> problems)
		{
			throw new ApplicationException("Unexpected WritingSystem load problem in test.");
		}

		private static void OnMigrationHandler(int version, IEnumerable<LdmlMigrationInfo> migrationinfo)
		{
			throw new ApplicationException("Unexpected WritingSystem migration in test.");
		}
	}
}
