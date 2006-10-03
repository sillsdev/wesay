using System.IO;
using NUnit.Framework;
using WeSay.Language;
using WeSay.Language.Tests;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class BasilProjectTests
	{
		private string _projectDirectory;

		[SetUp]
		public void Setup()
		{
			DirectoryInfo dirProject = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
			_projectDirectory = dirProject.FullName;

		}

		private void InitializeSampleProject()
		{
			WriteSampleStringCatalogFile();
			WriteSampleWritingSystemFile();
	}

		private string GetCommonDirectory()
		{
			return Path.Combine(_projectDirectory, "common");
		}

		private void WriteSampleStringCatalogFile()
		{
			Directory.CreateDirectory(_projectDirectory);
			Directory.CreateDirectory(GetCommonDirectory());
			string pathToStringCatalogInProjectDir = Path.Combine(GetCommonDirectory(), "th.po");
			StreamWriter writer = File.CreateText(pathToStringCatalogInProjectDir);
			writer.Write(TestResources.poStrings);
			writer.Close();
		}

		private void WriteSampleWritingSystemFile()
		{
			Directory.CreateDirectory(_projectDirectory);
			Directory.CreateDirectory(GetCommonDirectory());
			string pathToWritingSystemPrefs = Path.Combine(GetCommonDirectory(), "writingSystemPrefs.xml");
			WritingSystemTests.WriteSampleWritingSystemFile(pathToWritingSystemPrefs);
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(_projectDirectory, true);
		}

		[Test]
		public void NoSetupDefaultWritingSystems()
		{
			InitializeSampleProject();

			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(_projectDirectory);
			WritingSystem ws = BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault;
			Assert.IsNotNull(ws);
			ws = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault;
			Assert.IsNotNull(ws);
		}

		[Test]
		public void LocalizedStringsDuringTests()
		{
			InitializeSampleProject();

			BasilProject project = new BasilProject();
			project.StringCatalogSelector = "th";
			project.LoadFromProjectDirectoryPath(_projectDirectory);
			Assert.AreEqual("deng", StringCatalog.Get("red"));
		}

		[Test]
		public void LocalizedStringsFromPretendSample()
		{
			InitializeSampleProject();
			BasilProject project = new BasilProject();
			project.StringCatalogSelector = "en";
			project.LoadFromProjectDirectoryPath(_projectDirectory);

			Assert.AreEqual("red", StringCatalog.Get("red"));
		}

		[Test]
		public void MakeProjectFiles()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				Directory.CreateDirectory(path);
				BasilProject project = new BasilProject();
				project.Create(path);
				Assert.IsTrue(Directory.Exists(path));
				Assert.IsTrue(Directory.Exists(project.ApplicationCommonDirectory));
				Assert.IsTrue(File.Exists(project.PathToWritingSystemPrefs));
			}
			finally
			{
				Directory.Delete(path, true);
			}
		}
	}
}
