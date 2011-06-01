using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Palaso.i18n;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class BasilProjectTests
	{
		private string _projectDirectory;

		[SetUp]
		public void Setup()
		{
			DirectoryInfo dirProject =
					Directory.CreateDirectory(Path.Combine(Path.GetTempPath(),
														   Path.GetRandomFileName()));
			_projectDirectory = dirProject.FullName;
		}

		private void InitializeSampleProject()
		{
			WriteSampleStringCatalogFile();
			WriteSampleWritingSystemFile();
		}

		private string GetCommonDirectory()
		{
			return _projectDirectory;
			//return Path.Combine(_projectDirectory, "common");
		}

		private void WriteSampleStringCatalogFile()
		{
			Directory.CreateDirectory(_projectDirectory);
			Directory.CreateDirectory(GetCommonDirectory());
			string pathToStringCatalogInProjectDir = Path.Combine(GetCommonDirectory(), "wesay.th.po");
			using (StreamWriter writer = File.CreateText(pathToStringCatalogInProjectDir))
			{
				writer.Write(TestResources.poStrings);
				writer.Close();
			}
		}

		private void WriteSampleWritingSystemFile()
		{
			Directory.CreateDirectory(_projectDirectory);
			Directory.CreateDirectory(GetCommonDirectory());
			string pathToWritingSystemPrefs = Path.Combine(GetCommonDirectory(),
														   "WritingSystemPrefs.xml");
			CreateSampleWritingSystemFile(pathToWritingSystemPrefs);
		}

		private void CreateSampleWritingSystemFile(string path)
		{
			using (StreamWriter writer = File.CreateText(path))
			{
				writer.Write(@"<?xml version='1.0' encoding='utf-8'?>
					<WritingSystemCollection>
					  <members>
						<WritingSystem>
						  <FontName>Courier New</FontName>
						  <FontSize>10</FontSize>
						  <Id>PretendAnalysis</Id>
						</WritingSystem>
						<WritingSystem>
						  <FontName>Courier New</FontName>
						  <FontSize>20</FontSize>
						  <Id>PretendVernacular</Id>
						</WritingSystem>
					  </members>
					</WritingSystemCollection>");
				writer.Close();
			}
		}

		[TearDown]
		public void TearDown()
		{
			if (BasilProject.IsInitialized)
			{
				BasilProject.Project.Dispose();
			}
			Palaso.TestUtilities.TestUtilities.DeleteFolderThatMayBeInUse(_projectDirectory);
		}

		//  not relevant anymore
		//        [Test]
		//        public void NoSetupDefaultWritingSystems()
		//        {
		//            InitializeSampleProject();
		//
		//            BasilProject project = new BasilProject();
		//            project.LoadFromProjectDirectoryPath(_projectDirectory);
		//            WritingSystem ws = BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault;
		//            Assert.IsNotNull(ws);
		//            ws = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault;
		//            Assert.IsNotNull(ws);
		//        }

		[Test]
		public void LocalizedStringsDuringTests()
		{
			InitializeSampleProject();

			BasilProject project = new BasilProject();
			project.UiOptions.Language = "th";
			project.LoadFromProjectDirectoryPath(_projectDirectory);
			Assert.AreEqual("deng", StringCatalog.Get("red"));
		}

		[Test]
		public void LocalizedStringsFromPretendSample()
		{
			InitializeSampleProject();
			BasilProject project = new BasilProject();
			project.UiOptions.Language = "en";
			project.LoadFromProjectDirectoryPath(_projectDirectory);

			Assert.AreEqual("red", StringCatalog.Get("red"));
		}

	}
}
