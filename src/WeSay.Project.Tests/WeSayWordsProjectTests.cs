using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using Palaso.Reporting;
using TestUtilities;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class WeSayWordsProjectTests
	{

		[SetUp]
		public void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;

		}

		[TearDown]
		public void TearDown()
		{
		}


		[Test]
		public void DefaultConfigFile_DoesntNeedMigrating()
		{
			WeSayWordsProject p = new WeSayWordsProject();
			XPathDocument defaultConfig = new XPathDocument(p.PathToDefaultConfig);
			using (TempFile f = new TempFile())
			{
				bool migrated = WeSayWordsProject.MigrateConfigurationXmlIfNeeded(defaultConfig, f.Path);
				Assert.IsFalse(migrated, "The default config file should never need migrating");
			}
		}

		[Test]
		[ExpectedException(typeof (ErrorReport.NonFatalMessageSentToUserException))]
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
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting(""))
			{
			   // WeSayWordsProject p = CreateAndLoad();
				Field f = new Field();
				f.OptionsListFile = "PartsOfSpeech.xml";
				OptionsList list = p.CreateLoadedProject().GetOptionsList(f, false);
				Assert.IsTrue(list.Options.Count > 2);
			}
		}

		[Test]
		public void CorrectFieldToOptionListNameDictionary()
		{
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting(""))
			{
				Field f = new Field();
				f.OptionsListFile = "PartsOfSpeech.xml";
				WeSayWordsProject project = p.CreateLoadedProject();
				Dictionary<string, string> dict = project.GetFieldToOptionListNameDictionary();
				Assert.AreEqual("PartsOfSpeech", dict[LexSense.WellKnownProperties.PartOfSpeech]);
			}
		}
//
//        private static WeSayWordsProject CreateAndLoad(TemporaryFolder projectFolder)
//        {
//            WeSayWordsProject p = new WeSayWordsProject();
//            p.LoadFromProjectDirectoryPath(projectDir);
//            return p;
//        }

		private static string MakeDir(string existingParent, string newChild)
		{
			string dir = Path.Combine(existingParent, newChild);
			Directory.CreateDirectory(dir);
			return dir;
		}

		private static void TryLoading(string lexiconPath, string experimentDir)
		{
			try
			{
				WeSayWordsProject p = new WeSayWordsProject();
				lexiconPath = p.UpdateFileStructure(lexiconPath);

				p.LoadFromLiftLexiconPath(lexiconPath);
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

		[Test]
		public void MakeFieldNameChange_CannotBeBrokenWithWeirdNames_IfSafe()
		{
			TryFieldNameChangeAfterMakingSafe("color)", "color(");
			TryFieldNameChangeAfterMakingSafe("(color", ")color");
			TryFieldNameChangeAfterMakingSafe("*color", "color*");
			TryFieldNameChangeAfterMakingSafe("[color", "]color");
			TryFieldNameChangeAfterMakingSafe("color[", "color]");
			TryFieldNameChangeAfterMakingSafe("{color", "}color");
			TryFieldNameChangeAfterMakingSafe("color{", "color}");
			TryFieldNameChangeAfterMakingSafe("?color{", "color");
		}

		[Test]
		public void MigrateAndSaveProduceSameVersion()
		{

			using (ProjectDirectorySetupForTesting projectDir = new ProjectDirectorySetupForTesting(""))
			{
				string configPath = Path.Combine(projectDir.PathToDirectory, "TestProj.WeSayConfig");
				File.WriteAllText(configPath,
								  "<?xml version='1.0' encoding='utf-8'?><tasks><components><viewTemplate></viewTemplate></components><task id='Dashboard' class='WeSay.CommonTools.DashboardControl' assembly='CommonTools' default='true'></task></tasks>");
				XPathDocument doc = new XPathDocument(configPath);
				string outputPath = Path.Combine(projectDir.PathToDirectory, Path.GetTempFileName());
				WeSayWordsProject.MigrateConfigurationXmlIfNeeded(doc, outputPath);
				XmlDocument docFile = new XmlDocument();
				docFile.Load(outputPath);
				XmlNode node = docFile.SelectSingleNode("configuration");
				string migrateVersion = node.Attributes["version"].Value;

				WeSayWordsProject p = projectDir.CreateLoadedProject();
				p.Save();
				docFile.Load(p.PathToConfigFile);
				node = docFile.SelectSingleNode("configuration");
				string saveVersion = node.Attributes["version"].Value;

				Assert.AreEqual(saveVersion, migrateVersion);
			}
		}

		private static void TryFieldNameChangeAfterMakingSafe(string oldName, string newName)
		{
			using (
					ProjectDirectorySetupForTesting dir =
							new ProjectDirectorySetupForTesting(string.Empty))
			{
				WeSayWordsProject p = dir.CreateLoadedProject();
				p.ViewTemplates.Add(new ViewTemplate());
				oldName = Field.MakeFieldNameSafe(oldName);
				newName = Field.MakeFieldNameSafe(newName);
				Field f = new Field(oldName, "LexEntry", new string[] {"en"});
				p.ViewTemplates[0].Add(f);
				p.Save();
				f.FieldName = newName;
				p.MakeFieldNameChange(f, oldName);
			}
		}
	}
}