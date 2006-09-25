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
		public void NoSetupDefaultWritingSystems()
		{
			BasilProject project = new BasilProject();
			project.Load(_projectDirectory,true);
			Directory.CreateDirectory(Directory.GetParent(project.PathToWritingSystemPrefs).FullName);
			WritingSystemTests.WriteSampleWritingSystemFile(project.PathToWritingSystemPrefs);
			project.InitWritingSystems();
			WritingSystem ws = BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault;
			Assert.IsNotNull(ws);
			ws = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault;
			Assert.IsNotNull(ws);
		}


		[Test]
		public void LocalizedStringsDuringTests()
		{
			BasilProject project = new BasilProject();
			project.Load(_projectDirectory,true);
			WriteSampleStringCatalogFile(project);
			project.InitStringCatalog();
			Assert.AreEqual("deng", StringCatalog.Get("red"));
		}

		[Test]
		public void LocalizedStringsFromPretendSample()
		{
			BasilProject project = new BasilProject();
			project.Load(_projectDirectory,true);
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
				Assert.IsTrue(File.Exists(project.PathToWritingSystemPrefs));
			}
			finally
			{
				Directory.Delete(path, true);
			}
		}
	}
}
