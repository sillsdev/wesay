using System.IO;
using NUnit.Framework;
using WeSay.Language;

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

		private void WriteSampleWritingSystemFile()
		{
			BasilProject project = new BasilProject();
			project.Load(_projectDirectory);
			Directory.CreateDirectory(Directory.GetParent(project.PathToWritingSystemPrefs).FullName);
			StreamWriter writer = File.CreateText(project.PathToWritingSystemPrefs);
			writer.Write(TestResources.WritingSystemPrefs);
			writer.Close();
			project.InitWritingSystems();
		}

		private void WriteSampleStringCatalogFile(BasilProject project)
		{
			Directory.CreateDirectory(Directory.GetParent(project.PathToStringCatalogInProjectDir).FullName);
			StreamWriter writer = File.CreateText(project.PathToStringCatalogInProjectDir);
			writer.Write(TestResources.poStrings);
			writer.Close();
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete(_projectDirectory, true);
		}

		[Test]
		public void RightFont()
		{
			WriteSampleWritingSystemFile();
			WritingSystem ws = BasilProject.Project.AnalysisWritingSystemDefault;
			Assert.AreEqual("ANA", ws.Id);
			Assert.AreEqual("Wingdings", ws.Font.Name);
			Assert.AreEqual(20, ws.Font.Size);

		}

		[Test]
		public void NoSetupDefaultWritingSystems()
		{
			WriteSampleWritingSystemFile();
			WritingSystem ws = BasilProject.Project.AnalysisWritingSystemDefault;
			Assert.IsNotNull(ws);
			ws = BasilProject.Project.VernacularWritingSystemDefault;
			Assert.IsNotNull(ws);
		}


		[Test]
		public void LocalizedStringsDuringTests()
		{
			BasilProject project = new BasilProject();
			project.Load(_projectDirectory);
			WriteSampleStringCatalogFile(project);
			project.InitStringCatalog();
			Assert.AreEqual("deng", StringCatalog.Get("red"));
		}

		[Test]
		public void LocalizedStringsFromPretendSample()
		{
			BasilProject project = new BasilProject();
			project.Load(_projectDirectory);
			project.StringCatalogSelector = "en";
			project.InitStringCatalog();

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
				Assert.IsTrue(Directory.Exists(project.PathToWritingSystemPrefs));
			}
			finally
			{
				Directory.Delete(path, true);
			}
		}
	}
}
