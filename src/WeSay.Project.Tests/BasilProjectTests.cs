using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Palaso.i18n;
using Palaso.TestUtilities;
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
			TestUtilities.DeleteFolderThatMayBeInUse(_projectDirectory);
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

		[Test]
		public void NewProject_ContainsOnlyLegacyWeSayWritingsystemsFile_WritingSystemsAreLoadedFromThatFile()
		{
			InitializeSampleProject();
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(_projectDirectory);

			Assert.AreEqual(2, project.WritingSystems.Count);
			Assert.IsTrue(project.WritingSystems.Contains("PretendAnalysis"));
			Assert.IsTrue(project.WritingSystems.Contains("PretendVernacular"));
		}

		[Test]
		public void NewProject_ContainsOnlyLegacyWeSayWritingsystemsFile_WritingSystemsAreConvertedToLdml()
		{
			InitializeSampleProject();
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(_projectDirectory);
			WritingSystemCollection wsCollection = new WritingSystemCollection();
			wsCollection.Load(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectDirectory));

			AssertWritingSystemCollectionsAreEqual(project.WritingSystems, wsCollection);
		}

		[Test]
		public void NewProject_ContainsOnlyLegacyWeSayWritingsystemsFile_LegacyFileIsDeleted()
		{
			InitializeSampleProject();
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(_projectDirectory);

			Assert.IsFalse(File.Exists(BasilProject.GetPathToWritingSystemPrefs(_projectDirectory)));
		}

		private static void AssertWritingSystemCollectionsAreEqual(WritingSystemCollection lhs, WritingSystemCollection rhs)
		{
			foreach (var lhsWritingSystem in lhs)
			{
				Assert.IsTrue(rhs.Contains(lhsWritingSystem.Id));
				var rhsWritingSystem = rhs.Get(lhsWritingSystem.Id);
				Assert.AreEqual(lhsWritingSystem.Id, rhsWritingSystem.Id);
				Assert.AreEqual(lhsWritingSystem.Abbreviation, rhsWritingSystem.Abbreviation);
				Assert.AreEqual(lhsWritingSystem.CustomSortRules, rhsWritingSystem.CustomSortRules);
				Assert.AreEqual(WritingSystemInfo.CreateFont(lhsWritingSystem).ToString(), WritingSystemInfo.CreateFont(rhsWritingSystem).ToString());
				Assert.AreEqual(lhsWritingSystem.DefaultFontName, rhsWritingSystem.DefaultFontName);
				Assert.AreEqual(lhsWritingSystem.DefaultFontSize, rhsWritingSystem.DefaultFontSize);
				Assert.AreEqual(lhsWritingSystem.IsVoice, rhsWritingSystem.IsVoice);
				Assert.AreEqual(lhsWritingSystem.IsUnicodeEncoded, rhsWritingSystem.IsUnicodeEncoded);
				Assert.AreEqual(lhsWritingSystem.Keyboard, rhsWritingSystem.Keyboard);
				Assert.AreEqual(lhsWritingSystem.RightToLeftScript, rhsWritingSystem.RightToLeftScript);
				Assert.AreEqual(lhsWritingSystem.SortUsing, rhsWritingSystem.SortUsing);
				Assert.AreEqual(lhsWritingSystem.SpellCheckingId, rhsWritingSystem.SpellCheckingId);
			}
		}

		[Test]
		public void NewProject_ContainsNoWritingsystemFiles_DefaultsAreLoaded()
		{
			InitializeSampleProject();
			File.Delete(BasilProject.GetPathToWritingSystemPrefs(_projectDirectory));
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(_projectDirectory);

			Assert.AreEqual(7, project.WritingSystems.Count);
			Assert.IsTrue(project.WritingSystems.Contains("en"));
			Assert.IsTrue(project.WritingSystems.Contains("tpi"));
		}

		[Test]
		//WS-33900
		public void NewProject_ContainsNoWritingsystemFiles_DefaultsAreLoadedButWeDontWriteToTheFilesInTheCommonDirectory()
		{
			InitializeSampleProject();
			File.Delete(BasilProject.GetPathToWritingSystemPrefs(_projectDirectory));
			BasilProject project = new BasilProject();
			project.LoadFromProjectDirectoryPath(_projectDirectory);
			string pathToWritingSystemsInApplicationCommonDirectory = BasilProject.GetPathToLdmlWritingSystemsFolder(BasilProject.ApplicationCommonDirectory);
			string englishLdmlContent = File.ReadAllText(Path.Combine(pathToWritingSystemsInApplicationCommonDirectory, "en.ldml"));

			WritingSystem ws = project.WritingSystems["en"];
			if (ws.Abbreviation == "writeme!"){throw new ApplicationException("This test seems to have failed at some point and the en.ldml file in the application common directory neesds to be reverted before the next test run.");}
			ws.Abbreviation = "writeme!";
			project.Save();
			Assert.AreEqual(englishLdmlContent, File.ReadAllText(Path.Combine(pathToWritingSystemsInApplicationCommonDirectory, "en.ldml")));
		}

		[Test]
		public void NewProject_ContainsLdmlWritingSystemFiles_LdmlFilesAreLoaded()
		{
			InitializeSampleProject();
			BasilProject project = new BasilProject();
			project.WritingSystems.LoadFromLegacyWeSayFile(BasilProject.GetPathToWritingSystemPrefs(_projectDirectory));
			File.Delete(BasilProject.GetPathToWritingSystemPrefs(_projectDirectory));
			project.LoadFromProjectDirectoryPath(_projectDirectory);

			Assert.AreEqual(2, project.WritingSystems.Count);
			Assert.IsTrue(project.WritingSystems.Contains("PretendAnalysis"));
			Assert.IsTrue(project.WritingSystems.Contains("PretendVernacular"));
		}

		[Test]
		public void NewProject_ContainsLdmlAndLegacyWritingSystemFiles_OnlyLdmlFilesAreLoaded()
		{
			InitializeSampleProject();
			BasilProject project = new BasilProject();
			WritingSystemCollection wsCollection = new WritingSystemCollection();
			WritingSystem ws = new WritingSystem(){ISO = "ldmlWs"};
			wsCollection.Add(ws.Id, ws);
			wsCollection.Write(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectDirectory));
			project.LoadFromProjectDirectoryPath(_projectDirectory);

			Assert.AreEqual(1, project.WritingSystems.Count);
			Assert.IsTrue(project.WritingSystems.Contains("ldmlWs"));
		}
	}
}
