using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Palaso.Reporting;
using WeSay.Foundation.Options;
using WeSay.Project;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class WeSayWordsProjectTests
	{
		private string _projectDirectory;

		[SetUp]
		public void Setup()
		{
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = false;
			DirectoryInfo dirProject = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
			this._projectDirectory = dirProject.FullName;
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(this._projectDirectory, true);
		}


		[Test, Ignore()]
		public void MakeProjectFiles()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				Directory.CreateDirectory(Directory.GetParent(path).FullName);
				WeSayWordsProject p = new WeSayWordsProject();
				p.CreateEmptyProjectFiles(path);
				Assert.IsTrue(Directory.Exists(path));
				Assert.IsTrue(Directory.Exists(p.PathToWeSaySpecificFilesDirectoryInProject));
			}
			finally
			{
				Directory.Delete(path, true);
			}
		}

		[Test, ExpectedException(typeof(ErrorReport.NonFatalMessageSentToUserException))]
		public void WeSayDirNotInValidBasilDir()
		{
			string experimentDir = MakeDir(Path.GetTempPath(), Path.GetRandomFileName());
			string weSayDir = experimentDir; // MakeDir(experimentDir, "WeSay");
			string wordsPath = Path.Combine(weSayDir, "AAA.words");
			File.Create(wordsPath).Close();
			TryLoading(wordsPath, experimentDir);
		}

		[Test]
		public void LoadPartsOfSpeechList()
		{
			WeSayWordsProject p = CreateAndLoad(false);
			Field f = new Field();
			f.OptionsListFile = "PartsOfSpeech.xml";
			OptionsList list = p.GetOptionsList(f, false);
			Assert.IsTrue(list.Options.Count>2);
		}

		[Test]
		public void CorrectFieldToOptionListNameDictionary()
		{
			WeSayWordsProject p = CreateAndLoad(false);
			Field f = new Field();
			f.OptionsListFile = "PartsOfSpeech.xml";
			OptionsList list = p.GetOptionsList(f, false);
			Dictionary<string, string> dict = p.GetFieldToOptionListNameDictionary();
			Assert.AreEqual("PartsOfSpeech", dict[LexicalModel.LexSense.WellKnownProperties.PartOfSpeech]);
		}


		private static WeSayWordsProject CreateAndLoad(bool doMakeFileMatchingProjectName)
		{
			string experimentDir = MakeDir(Path.GetTempPath(), Path.GetRandomFileName());
			string projectDir = MakeDir(experimentDir, "TestProj");
		   // string weSayDir = MakeDir(projectDir, "WeSay");
//            MakeDummyWordsFile("AAA.words", weSayDir);
//            if (doMakeFileMatchingProjectName)
//            {
//                MakeDummyWordsFile("TestProj.words", weSayDir);
//            }
//            MakeDummyWordsFile("ZZZ.words", weSayDir);
			WeSayWordsProject p = new WeSayWordsProject();
			p.LoadFromProjectDirectoryPath(projectDir);
			return p;
		}


//        [Test]
//        public void LoadFromOldStructure(bool doMakeFileMatchingProjectName)
//        {
//            string experimentDir = MakeDir(Path.GetTempPath(), Path.GetRandomFileName());
//            string projectDir = MakeDir(experimentDir, "TestProj");
//        }


		//private static void MakeDummyWordsFile(string fileName, string weSayDir)
		//{
		//    File.Create(Path.Combine(weSayDir, fileName)).Close();
		//}

		private static string MakeDir(string existingParent, string newChild)
		{
			string dir = Path.Combine(existingParent,newChild);
			Directory.CreateDirectory(dir);
			return dir;
		}

		private static bool TryLoading(string lexiconPath, string experimentDir)
		{
			try
			{
				WeSayWordsProject p = new WeSayWordsProject();
				lexiconPath = p.UpdateFileStructure(lexiconPath);

				return p.LoadFromLiftLexiconPath(lexiconPath);
			}
			finally
			{
				Directory.Delete(experimentDir, true);
			}
		}

		[Test]
		public void GetOptionsListFromFieldName()
		{
			WeSayWordsProject p = new WeSayWordsProject();

			OptionsList list = p.GetOptionsList("POS");
			Assert.IsNotNull(list);
			Assert.IsNotNull(list.Options);
			Assert.Greater(list.Options.Count, 2);
		}
	}
}