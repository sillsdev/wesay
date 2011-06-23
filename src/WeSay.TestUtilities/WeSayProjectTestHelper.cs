using System;
using System.IO;
using Palaso.WritingSystems;
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
			IWritingSystemRepository wsc = new LdmlInFolderWritingSystemRepository(pathToLdmlWsFolder);
			wsc.Set(WritingSystemDefinition.FromLanguage(WritingSystemsIdsForTests.VernacularIdForTest));
			wsc.Set(WritingSystemDefinition.FromLanguage(WritingSystemsIdsForTests.AnalysisIdForTest));
			wsc.Set(WritingSystemDefinition.FromLanguage(WritingSystemsIdsForTests.OtherIdForTest));


			wsc.Save();

			project.SetupProjectDirForTests(WeSayWordsProject.PathToPretendLiftFile);
			project.BackupMaker = null;//don't bother. Modern tests which might want to check backup won't be using this old approach anyways.
			return project;
		}
	}
}
