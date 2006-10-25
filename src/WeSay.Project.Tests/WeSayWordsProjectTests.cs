using System;
using System.IO;
using NUnit.Framework;
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
				p.Create(path);
				Assert.IsTrue(Directory.Exists(path));
				Assert.IsTrue(Directory.Exists(p.PathToWeSaySpecificFilesDirectoryInProject));
			}
			finally
			{
				Directory.Delete(path, true);
			}
		}

		[Test, ExpectedException(typeof(ApplicationException))]
		public void WeSayDirNotInValidBasilDir()
		{
			string experimentDir = MakeDir(Path.GetTempPath(), Path.GetRandomFileName());
			string weSayDir = MakeDir(experimentDir, "WeSay");
			string wordsPath = Path.Combine(weSayDir, "AAA.words");
			File.Create(wordsPath).Close();
			TryLoading(wordsPath, experimentDir);
		}

		[Test, ExpectedException(typeof(ApplicationException))]
		public void WordsFileNotInValidWeSayDir()
		{
			string experimentDir = MakeDir(Path.GetTempPath(), Path.GetRandomFileName());
			string wordsPath = Path.Combine(experimentDir, "test.words");
			File.Create(wordsPath).Close();
			TryLoading(wordsPath, experimentDir);
		}


		[Test]
		public void PreferWordsFileMatchingProjectName()
		{
			WeSayWordsProject p = CreateAndLoad(true);
			Assert.IsTrue(p.PathToLexicalModelDB.Contains("TestProj.words"));
		}

		/// <summary>
		/// This may be a bad idea, but this test at least nails current behavior
		/// </summary>
		[Test]
		public void UseSomethingIfPreferedNameMissing()
		{
			WeSayWordsProject p = CreateAndLoad(false);
			Assert.IsTrue(p.PathToLexicalModelDB.Contains("AAA.words"));
		}

		private static WeSayWordsProject CreateAndLoad(bool doMakeFileMatchingProjectName)
		{
			string experimentDir = MakeDir(Path.GetTempPath(), Path.GetRandomFileName());
			string projectDir = MakeDir(experimentDir, "TestProj");
			string weSayDir = MakeDir(projectDir, "WeSay");
			MakeDummyWordsFile("AAA.words", weSayDir);
			if (doMakeFileMatchingProjectName)
			{
				MakeDummyWordsFile("TestProj.words", weSayDir);
			}
			MakeDummyWordsFile("ZZZ.words", weSayDir);
			WeSayWordsProject p = new WeSayWordsProject();
			p.LoadFromProjectDirectoryPath(projectDir);
			return p;
		}

		private static void MakeDummyWordsFile(string fileName, string weSayDir)
		{
			File.Create(Path.Combine(weSayDir, fileName)).Close();
		}

		private static string MakeDir(string existingParent, string newChild)
		{
			string dir = Path.Combine(existingParent,newChild);
			Directory.CreateDirectory(dir);
			return dir;
		}

		private static void TryLoading(string lexiconPath, string experimentDir)
		{
			try
			{
				WeSayWordsProject p = new WeSayWordsProject();
				p.LoadFromLexiconPath(lexiconPath);
			}
			finally
			{
				Directory.Delete(experimentDir, true);
			}
		}
	}
}