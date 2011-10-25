using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using NUnit.Framework;
using Palaso.DictionaryServices.Model;
using Palaso.IO;
using Palaso.Lift;
using Palaso.Lift.Options;
using Palaso.Reporting;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;
using WeSay.LexicalModel;
using WeSay.Project.ConfigMigration.WeSayConfig;
using WeSay.Project.ConfigMigration.WritingSystem;
using WeSay.Project.Tests.ConfigMigration.WritingSystem;
using WeSay.TestUtilities;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class WeSayWordsProjectTests
	{

		[SetUp]
		public void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;
			WeSayWordsProject.PreventBackupForTests = true;
		}

		[TearDown]
		public void TearDown()
		{
		}


		[Test]
		public void UpdateFileStructure_LiftByItself_DoesNothing()
		{
			using (var f = new TemporaryFolder("OpeningLiftFile_MissingConfigFile_GivesMessage"))
			{
				using(var lift = new TempLiftFile(f, "", "0.13"))
				{
					using(var p = new WeSayWordsProject())
					{
						Assert.AreEqual(lift.Path,p.UpdateFileStructure(lift.Path));
					}
				}
			}
		}

		/// <summary>
		/// check  (WS-1004) Exception: Access to the path is denied
		/// </summary>
		[Test]
		public void Save_LiftFileLocked_NotifiesUser()
		{
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting("<entry id='foo1'><lexical-unit><form lang='qaa-x-qaa'><text>fooOne</text></form></lexical-unit></entry>"))
			{
				WeSayWordsProject project = p.CreateLoadedProject();
				using (File.OpenWrite(p.PathToLiftFile))
				{
					WritingSystemDefinition ws = project.WritingSystems.Get("qaa-x-qaa");
					ws.Language = "aac";
					project.MakeWritingSystemIdChange("aac", "qaa-x-qaa");
					using (new Palaso.Reporting.ErrorReport.NonFatalErrorReportExpected())
					{
						project.Save();
					}
				}
			}
		}

		[Test]
		public void Save_MakeWritingSystemIdChangeOnWritingSystemFoundInLiftWasPreviosulyCalled_Changed()
		{
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting("<entry id='foo1'><lexical-unit><form lang='qaa-x-qaa'><text>fooOne</text></form></lexical-unit></entry>"))
			{
				WeSayWordsProject project = p.CreateLoadedProject();
				XmlDocument doc = new XmlDocument();
				doc.Load(p.PathToLiftFile);
				Assert.AreNotEqual(0, doc.SelectNodes("//form[@lang='qaa-x-qaa']").Count);
				project.MakeWritingSystemIdChange("qaa-x-qaa", "aac");
				project.Save();
				doc.Load(p.PathToLiftFile);
				Assert.AreNotEqual(0, doc.SelectNodes("//form[@lang='aac']").Count);
			}
		}

		[Test]
		public void Save_MakeWritingSystemIdChangeOnWritingSystemFoundInLiftWasPreviosulyCalledMultipleTimes_ChangedCorrectly()
		{
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting("<entry id='foo1'><lexical-unit><form lang='qaa-x-qaa'><text>fooOne</text></form></lexical-unit></entry>"))
			{
				WeSayWordsProject project = p.CreateLoadedProject();
				XmlDocument doc = new XmlDocument();
				doc.Load(p.PathToLiftFile);
				Assert.AreNotEqual(0, doc.SelectNodes("//form[@lang='qaa-x-qaa']").Count);
				project.MakeWritingSystemIdChange("qaa-x-qaa", "en");
				project.MakeWritingSystemIdChange("en", "de");
				project.Save();
				doc.Load(p.PathToLiftFile);
				Assert.AreNotEqual(0, doc.SelectNodes("//form[@lang='de']").Count);
			}
		}

		[Test]
		public void Save_ContentAndWritingSystemContainedInOptionListIsChanged_FileContentAndWritingSystemIsChanged()
		{
			using (var p = new ProjectDirectorySetupForTesting(""))
			{
				//create an option list file containing en and de writing systems
				const string optionListName = "options.xml";
				var optionListPath = Path.Combine(p.PathToDirectory, optionListName);
				File.WriteAllText(optionListPath, OptionListFileContent.GetOptionListWithWritingSystems(
					WritingSystemsIdsForTests.AnalysisIdForTest, WritingSystemsIdsForTests.OtherIdForTest));
				//create the project
				WeSayWordsProject project = p.CreateLoadedProject();
				//Add an option to the list
				//Got to create a field to pass to the project
				Field fieldThatUsesOptionsList = new Field{OptionsListFile = optionListName};
				var optionList = project.GetOptionsList(fieldThatUsesOptionsList, false);
				var multitextToAdd = new MultiText();
				multitextToAdd.SetAlternative(WritingSystemsIdsForTests.OtherIdForTest, "yodel");
				optionList.Options.Add(new Option("entry", multitextToAdd));
				project.MarkOptionListAsUpdated(optionList);
				//Simulate a writing system change made by the UI
				var wsToChange = project.WritingSystems.Get("qaa-x-qaa");
				wsToChange.Language = "de";
				project.WritingSystems.Set(wsToChange);
				project.MakeWritingSystemIdChange("qaa-x-qaa", "de");
				project.Save();
				AssertThatXmlIn.File(optionListPath).HasNoMatchForXpath("/optionsList/option/name/form[@lang='qaa-x-qaa']");
				AssertThatXmlIn.File(optionListPath).HasAtLeastOneMatchForXpath("/optionsList/option/name/form[@lang='de'][text()='one']");
				AssertThatXmlIn.File(optionListPath).HasAtLeastOneMatchForXpath("/optionsList/option/abbreviation/form[@lang='de'][text()='one']");
				AssertThatXmlIn.File(optionListPath).HasAtLeastOneMatchForXpath("/optionsList/option/name/form[@lang='de'][text()='yodel']");
				AssertThatXmlIn.File(optionListPath).HasAtLeastOneMatchForXpath("/optionsList/option/abbreviation/form[@lang='de'][text()='yodel']");
			}
		}

		[Test]
		public void Save_OptionListWasNeverLoadedButWritingSystemContainedThereinWasChanged_FileIsUpdated()
		{
			using (var p = new ProjectDirectorySetupForTesting(""))
			{
				//create an option list file containing en and de writing systems
				const string optionListName = "options.xml";
				var optionListPath = Path.Combine(p.PathToDirectory, optionListName);
				File.WriteAllText(optionListPath, OptionListFileContent.GetOptionListWithWritingSystems(
					WritingSystemsIdsForTests.AnalysisIdForTest, WritingSystemsIdsForTests.OtherIdForTest));
				//create the project
				WeSayWordsProject project = p.CreateLoadedProject();
				//Simulate a writing system change made by the UI
				var wsToChange = project.WritingSystems.Get("qaa-x-qaa");
				wsToChange.Language = "de";
				project.WritingSystems.Set(wsToChange);
				project.MakeWritingSystemIdChange("qaa-x-qaa", "de");
				project.Save();
				AssertThatXmlIn.File(optionListPath).HasNoMatchForXpath("/optionsList/option/name/form[@lang='qaa-x-qaa']");
				AssertThatXmlIn.File(optionListPath).HasAtLeastOneMatchForXpath("/optionsList/option/name/form[@lang='de'][text()='one']");
				AssertThatXmlIn.File(optionListPath).HasAtLeastOneMatchForXpath("/optionsList/option/abbreviation/form[@lang='de'][text()='one']");
			}
		}

		[Test]
		public void MakeWritingSystemIdChange_OptionListIsAlreadyLoaded_OptionListIsChanged()
		{
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting("<entry id='foo1'><lexical-unit><form lang='qaa-x-qaa'><text>fooOne</text></form></lexical-unit></entry>"))
			{
				WeSayWordsProject project = p.CreateLoadedProject();
				var optionList = project.GetOptionsList("POS");
				Assert.That(optionList.GetOptionFromKey("Verb").Abbreviation["en"], Is.EqualTo("verb"));
				Assert.That(optionList.GetOptionFromKey("Verb").Name["en"], Is.EqualTo("verb"));
				Assert.That(optionList.GetOptionFromKey("Verb").Description["en"], Is.EqualTo(""));
				project.MakeWritingSystemIdChange("en", "de");
				Assert.That(optionList.GetOptionFromKey("Verb").Abbreviation["de"], Is.EqualTo("verb"));
				Assert.That(optionList.GetOptionFromKey("Verb").Name["de"], Is.EqualTo("verb"));
				Assert.That(optionList.GetOptionFromKey("Verb").Description["de"], Is.EqualTo(""));
			}
		}

		[Test]
		public void MakeWritingSystemIdChange_LoadList_OptionListIsChanged()
		{
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting("<entry id='foo1'><lexical-unit><form lang='qaa-x-qaa'><text>fooOne</text></form></lexical-unit></entry>"))
			{
				WeSayWordsProject project = p.CreateLoadedProject();
				project.MakeWritingSystemIdChange("en", "de");
				var optionList = project.GetOptionsList("POS");
				Assert.That(optionList.GetOptionFromKey("Verb").Abbreviation["de"], Is.EqualTo("verb"));
				Assert.That(optionList.GetOptionFromKey("Verb").Name["de"], Is.EqualTo("verb"));
				Assert.That(optionList.GetOptionFromKey("Verb").Description["de"], Is.EqualTo(""));
			}
		}

		[Test]
		//the change should be happening when the project saves so that all files are nice and in sync
		public void MakeWritingSystemIdChange_OptionListIsNotChangedOnDisk()
		{
			using (var p = new ProjectDirectorySetupForTesting(""))
			{
				//create an option list file containing en and de writing systems
				const string optionListName = "options.xml";
				var optionListPath = Path.Combine(p.PathToDirectory, optionListName);
				File.WriteAllText(optionListPath, OptionListFileContent.GetOptionListWithWritingSystems(
					WritingSystemsIdsForTests.AnalysisIdForTest, WritingSystemsIdsForTests.OtherIdForTest));
				//create the project
				WeSayWordsProject project = p.CreateLoadedProject();
				//Add an option to the list
				//Got to create a field to pass to the project
				Field fieldThatUsesOptionsList = new Field { OptionsListFile = optionListName };
				var optionList = project.GetOptionsList(fieldThatUsesOptionsList, false);
				var multitextToAdd = new MultiText();
				multitextToAdd.SetAlternative(WritingSystemsIdsForTests.OtherIdForTest, "yodel");
				optionList.Options.Add(new Option("entry", multitextToAdd));
				project.MarkOptionListAsUpdated(optionList);
				//Simulate a writing system change made by the UI
				var wsToChange = project.WritingSystems.Get("qaa-x-qaa");
				wsToChange.Language = "de";
				project.WritingSystems.Set(wsToChange);
				project.MakeWritingSystemIdChange("qaa-x-qaa", "de");
				AssertThatXmlIn.File(optionListPath).HasNoMatchForXpath("/optionsList/option/name/form[@lang='de']");
				AssertThatXmlIn.File(optionListPath).HasAtLeastOneMatchForXpath("/optionsList/option/name/form[@lang='qaa-x-qaa'][text()='one']");
				AssertThatXmlIn.File(optionListPath).HasAtLeastOneMatchForXpath("/optionsList/option/abbreviation/form[@lang='qaa-x-qaa'][text()='one']");
				AssertThatXmlIn.File(optionListPath).HasNoMatchForXpath("/optionsList/option/name/form[@lang='de'][text()='yodel']");
				AssertThatXmlIn.File(optionListPath).HasNoMatchForXpath("/optionsList/option/abbreviation/form[@lang='de'][text()='yodel']");

			}
		}

		[Test]
		public void MakeWritingSystemIdChange_DefaultViewTemplateContainsFieldsWithWritingSystem_FieldsAreUpdated()
		{
			using (var p = new ProjectDirectorySetupForTesting(""))
			{
				WeSayWordsProject project = p.CreateLoadedProject();
				var fieldsUsingQaa = new List<Field>(project.ViewTemplates.SelectMany(template => template.Fields).Where(field => field.WritingSystemIds.Contains("qaa-x-qaa")));
				//Simulate a writing system change made by the UI
				var wsToChange = project.WritingSystems.Get("qaa-x-qaa");
				wsToChange.Language = "de";
				project.WritingSystems.Set(wsToChange);
				project.MakeWritingSystemIdChange("qaa-x-qaa", "de");
				foreach (var field in fieldsUsingQaa)
				{
					Assert.That(!field.WritingSystemIds.Contains("qaa-x-qaa"));
					Assert.That(field.WritingSystemIds.Contains("de"));
				}
			}
		}


		/// <summary>
		/// related to ws-944: Crash opening lift file from FLEx which was sitting in My Documents without a configuration file
		/// </summary>
		[Test, Ignore("Cannot easily run on vista or linux")]
		public void UpdateFileStructure_LiftByItselfAtRoot_DoesNothing()
		{
			string path = @"C:\unittest.lift"; //this is at the root ON PURPOSE
			File.CreateText(path).Close();
			using (TempFile.TrackExisting(path))
			{
					using (WeSayWordsProject p = new WeSayWordsProject())
					{
						Assert.AreEqual(path, p.UpdateFileStructure(path));
					}
				}
		}

		[Test]
		public void DefaultConfigFile_DoesntNeedMigrating()
		{
			using (var p = new ProjectDirectorySetupForTesting(""))
			{
				using (var proj = p.CreateLoadedProject())
				{
					bool migrated = proj.MigrateConfigurationXmlIfNeeded();
					Assert.IsFalse(migrated, "The default config file should never need migrating");
				}
			}
		}

		[Test]
		public void LoadPartsOfSpeechList()
		{
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting(""))
			{
				Field f = new Field();
				f.OptionsListFile = "PartsOfSpeech.xml";
				using (var proj = p.CreateLoadedProject())
				{
					OptionsList list = proj.GetOptionsList(f, false);
					Assert.IsTrue(list.Options.Count > 2);
				}
			}
		}

		[Test]
		public void CorrectFieldToOptionListNameDictionary()
		{
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting(""))
			{
				Field f = new Field();
				f.OptionsListFile = "PartsOfSpeech.xml";
				using (WeSayWordsProject project = p.CreateLoadedProject())
				{
					Dictionary<string, string> dict = project.GetFieldToOptionListNameDictionary();
					Assert.AreEqual("PartsOfSpeech", dict[LexSense.WellKnownProperties.PartOfSpeech]);
				}
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

			using (var x = new ProjectDirectorySetupForTesting(""))
			{
				WeSayWordsProject p = x.CreateLoadedProject();

				OptionsList list = p.GetOptionsList("POS");
				Assert.IsNotNull(list);
				Assert.IsNotNull(list.Options);
				Assert.Greater(list.Options.Count, 2);
			}
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
								  "<?xml version='1.0' encoding='utf-8'?><tasks><components><viewTemplate></viewTemplate></components><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks>");
				string outputPath = Path.Combine(projectDir.PathToDirectory, Path.GetTempFileName());
				new ConfigurationMigrator().MigrateConfigurationXmlIfNeeded(configPath, outputPath);
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


				using (File.OpenWrite(dir.PathToConfigFile))
				{
				}
				p.Save();
				f.FieldName = newName;
				p.MakeFieldNameChange(f, oldName);
			}
		}

		[Test]
		public void WeSayConfigFileIsToNew_Throws()
		{
			using (ProjectDirectorySetupForTesting projectDir = new ProjectDirectorySetupForTesting(""))
			{
				string configFilePath = Path.Combine(projectDir.PathToDirectory, "TestProj.WeSayConfig");
				const int version = ConfigFile.LatestVersion + 1;
				File.WriteAllText(
					configFilePath,
					String.Format(
						"<?xml version='1.0' encoding='utf-8'?><configuration version=\"{0}\"><tasks><components><viewTemplate></viewTemplate></components><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>"
						, version
					)
				);
				Assert.Throws<ConfigurationFileTooNewException>(() => new ConfigFile(configFilePath));
			}
		}

		[Test]
		public void WeSayConfigFileIsToCurrent_DoesNotThrow()
		{

			using (var projectDir = new ProjectDirectorySetupForTesting(""))
			{
				string configFilePath = Path.Combine(projectDir.PathToDirectory, "TestProj.WeSayConfig");
				const int version = ConfigFile.LatestVersion;
				File.WriteAllText(configFilePath,
								  String.Format("<?xml version='1.0' encoding='utf-8'?><configuration version=\"{0}\"><tasks><components><viewTemplate></viewTemplate></components><task id='Dashboard' class='WeSay.LexicalTools.Dashboard.DashboardControl' assembly='CommonTools' default='true'></task></tasks></configuration>", version));
				var configFile = new ConfigFile(configFilePath);
			}
		}

		/// <summary>
		/// check  (WS-1030) When WeSay is open and you try to change a field, get green box, should get friendly message.
		/// </summary>
		[Test]
		public void MakeFieldNameChange_FileLocked_NotifiesUser()
		{
			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting("<entry id='foo1'><lexical-unit><form lang='qaa-x-qaa'><text>fooOne</text></form></lexical-unit></entry>"))
			{
				WeSayWordsProject project = p.CreateLoadedProject();
				using (File.OpenWrite(p.PathToLiftFile))
				{
					using (new Palaso.Reporting.ErrorReport.NonFatalErrorReportExpected())
					{
						Field f = new Field("old", "LexEntry", new string[] {"en"});
						project.ViewTemplates[0].Add(f);
						project.Save();
						f.FieldName = "new";
						Assert.IsFalse(project.MakeFieldNameChange(f, "old"));
					}
				}
			}
		}

		[Ignore("just for manual use"), Test]
		public void MakeTestLiftFile()
		{
			string pathToFolder=@"C:\wesay\lifttest";
			string projectName= "lifttest";

			if (!Directory.Exists(pathToFolder))
				Directory.CreateDirectory(pathToFolder);

			StringBuilder builder = new StringBuilder();
			int numberOfTestLexEntries =50;
			for (int i = 0; i < numberOfTestLexEntries; i++)
			{
				builder.AppendFormat(@"
				<entry id='{0}'>
					<lexical-unit>
					  <form lang='qaa-x-qaa'>
						<text>{0}</text>
					  </form>
					</lexical-unit>
					<sense>
						<grammatical-info value='n'/>
						<definition><form lang='en'><text>blah blah {0} blah blah</text></form></definition>
						<example lang='qaa-x-qaa'><text>and example of lah blah {0} blah blah</text></example>
					</sense>
				</entry>", i);
			}

			string liftContents =
					  string.Format(
							  "<?xml version='1.0' encoding='utf-8'?><lift version='{0}'>{1}</lift>",
							  .12,
							  builder.ToString());

			File.WriteAllText(Path.Combine(pathToFolder, projectName + ".lift"), liftContents);


		}

		/// <summary>
		/// check issue related to (WS-1035)
		/// </summary>
		[Test]
		public void PathProvidedAsSimpleFileName_GetsConverted()
		{
			using (ProjectDirectorySetupForTesting dir = new ProjectDirectorySetupForTesting("<entry id='foo1'><lexical-unit><form lang='qaa-x-qaa'><text>fooOne</text></form></lexical-unit></entry>"))
			{
				string oldWorkingDir= System.Environment.CurrentDirectory;
				try
				{
					using (WeSayWordsProject project = new WeSayWordsProject())
					{
						System.Environment.CurrentDirectory = dir.PathToDirectory;
						project.LoadFromLiftLexiconPath(Path.GetFileName(dir.PathToLiftFile));

						Assert.AreEqual(dir.PathToLiftFile, project.PathToLiftFile);
					}
				}
				finally
				{
					System.Environment.CurrentDirectory = oldWorkingDir;
				}

			}
		}

		/// <summary>
		/// regression from WS-1395
		/// </summary>
		[Test]
		public void GetLexEntryRepository_LiftIsBad_NotfiesUser()
		{
			//here the lang attribute is missing (as it was from a user's lexique pro output)
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = true;
			using (var projectDir =
				new WeSay.Project.Tests.ProjectDirectorySetupForTesting(
					@"
				<entry id='x'>
					<lexical-unit>
					  <form>
						<text>test</text>
					  </form>
					</lexical-unit>
				</entry>"))
			{

				var project = projectDir.CreateLoadedProject();
				var gotException = false;
				//we actually get a dialog box *and* and exception (the later, rather than, say, null, because AutoFac doesn't let use return null)
				using (new Palaso.Reporting.ErrorReport.NonFatalErrorReportExpected())
				{
					try
					{
						// This will leak an application exception due to the Dispose not being called.
						// due to the constructor throw.
						project.GetLexEntryRepository();
					}
					catch(LiftFormatException)
					{
						 gotException = true;
					}
				}
				Assert.IsTrue(gotException);
			}
		}

		[Test]
		public void LoadProject_EmptyLanguageInUserConfig_ReadsDefaultEn()
		{
			string config = @"<?xml version='1.0' encoding='utf-8'?>
<configuration version='2'>
  <backupPlan />
  <uiOptions>
	<language></language>
	<labelFontName>Angsana New</labelFontName>
	<labelFontSizeInPoints>18</labelFontSizeInPoints>
  </uiOptions>
</configuration>".Replace("'", "\"");

			using (var projectDir = new ProjectDirectorySetupForTesting(""))
			{
				File.WriteAllText(projectDir.PathToUserConfigFile, config);
				var project = projectDir.CreateLoadedProject();
				Assert.That(project.UiOptions.Language, Is.EqualTo("en"));
			}
		}



		[Test]
		public void LoadFromProjectDirectoryPath_NoPreviousWeSaySpecificFiles_CreatesWeSayConfigAndPartsOfSpeech()
		{
			using (var projectDir = new ProjectDirectorySetupForTesting(""))
			{
				File.Delete(projectDir.PathToConfigFile);
				File.Delete(projectDir.PathToUserConfigFile);
				var project = new WeSayWordsProject();
				project.LoadFromProjectDirectoryPath(projectDir.PathToDirectory);
				Assert.IsTrue(File.Exists(projectDir.PathToConfigFile));
			}
		}

		[Test]
		public void ProjectCreation_WritingSystemLdml_IsLatestVersion()
		{
			using (var project = new ProjectDirectorySetupForTesting("").CreateLoadedProject())
			{
				var namespaceManager = new XmlNamespaceManager(new NameTable());
				namespaceManager.AddNamespace("palaso", "urn://palaso.org/ldmlExtensions/v1");
				var wsFolderPath = WeSayWordsProject.GetPathToLdmlWritingSystemsFolder(project.ProjectDirectoryPath);
				var pathToEnglish = Path.Combine(wsFolderPath, "en.ldml");
				AssertThatXmlIn.File(pathToEnglish).HasAtLeastOneMatchForXpath(String.Format("/ldml/special/palaso:version[@value = {0}]", WritingSystemDefinition.LatestWritingSystemDefinitionVersion), namespaceManager);
				var pathToQaa = Path.Combine(wsFolderPath, "qaa-x-qaa.ldml");
				AssertThatXmlIn.File(pathToQaa).HasAtLeastOneMatchForXpath(String.Format("/ldml/special/palaso:version[@value = {0}]", WritingSystemDefinition.LatestWritingSystemDefinitionVersion), namespaceManager);
			}
		}

		[Test]
		//This test was formerly part of the LdmlInFolderWritingSystemCollectionTests TA 4/19/2011
		[Category("For review")]
		public void ProjectCreation_WritingSystemCollection_HasUnknownVernacular()
		{
			using (var project = new ProjectDirectorySetupForTesting("").CreateLoadedProject())
			{
				Assert.IsNotNull(project.WritingSystems.Get(WritingSystemsIdsForTests.OtherIdForTest));
			}
		}

		[Test]
		//This test was formerly part of the LdmlInFolderWritingSystemCollectionTests TA 4/19/2011
		[Category("For review")]
		public void WritingSystemCollection_HasUnknownAnalysis()
		{
			using (var project = new ProjectDirectorySetupForTesting("").CreateLoadedProject())
			{
				Assert.IsNotNull(project.WritingSystems.Get(WritingSystemsIdsForTests.AnalysisIdForTest));
			}
		}

		[Test]
		//WS-33900
		public void NewProject_ContainsNoWritingsystemFiles_DefaultsAreLoadedButWeDontWriteToTheFilesInTheCommonDirectory()
		{
			using (var environment = new ProjectDirectorySetupForTesting(""))
			{
				var project = new WeSayWordsProject();
				project.LoadFromProjectDirectoryPath(environment.PathToDirectory);
				string pathToWritingSystemsInApplicationCommonDirectory =
					BasilProject.GetPathToLdmlWritingSystemsFolder(BasilProject.ApplicationCommonDirectory);
				string englishLdmlContent =
					File.ReadAllText(Path.Combine(pathToWritingSystemsInApplicationCommonDirectory, "en.ldml"));

				WritingSystemDefinition ws = project.WritingSystems.Get("en");
				if (ws.Abbreviation == "writeme!")
				{
					throw new ApplicationException(
						"This test seems to have failed at some point and the en.ldml file in the application common directory neesds to be reverted before the next test run.");
				}
				ws.Abbreviation = "writeme!";
				project.Save();
				Assert.AreEqual(
					englishLdmlContent,
					File.ReadAllText(Path.Combine(pathToWritingSystemsInApplicationCommonDirectory, "en.ldml")));
			}
		}

		[Test]
		public void OpenProject_ConfigFileContainsWritingSystemIdForWhichThereIsNoLdml_LdmlIsCreated()
		{
			using (var projectDirectory = new TemporaryFolder())
			{
				//setting up a minimal WeSay project with a config file that contains an id for a nonexistent writing system
				var project = new WeSayWordsProject();
				string liftFilePath = Path.Combine(projectDirectory.Path, "test.lift");
				File.WriteAllText(liftFilePath, "<?xml version='1.0' encoding='utf-8'?><lift version='0.12'></lift>");

				//Grab a valid configfile and insert german into it
				string configFilePath = Path.Combine(projectDirectory.Path, "test.WeSayConfig");
				File.Copy(Path.Combine(project.ApplicationTestDirectory, "PRETEND.WeSayConfig"), configFilePath);
				var configFile = new XmlDocument();
				configFile.Load(configFilePath);
				var node = configFile.SelectSingleNode(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id");
				node.InnerXml = "de";
				configFile.Save(configFilePath);

				project.LoadFromProjectDirectoryPath(projectDirectory.Path);

				Assert.That(project.WritingSystems.Contains("de"));
			}
		}

		[Test]
		public void OpenProject_ConfigFileContainsWritingSystemIdForWhichThereIsNoLdml_ProjectHasWritingSystem()
		{
			using (var projectDirectory = new TemporaryFolder())
			{
				//setting up a minimal WeSay project with a config file that contains an id for a nonexistent writing system
				var project = new WeSayWordsProject();
				string liftFilePath = Path.Combine(projectDirectory.Path, "test.lift");
				File.WriteAllText(liftFilePath, "<?xml version='1.0' encoding='utf-8'?><lift version='0.12'></lift>");

				//Grab a valid configfile and insert german into it
				string configFilePath = Path.Combine(projectDirectory.Path, "test.WeSayConfig");
				File.Copy(Path.Combine(project.ApplicationTestDirectory, "PRETEND.WeSayConfig"), configFilePath);
				var configFile = new XmlDocument();
				configFile.Load(configFilePath);
				var node = configFile.SelectSingleNode(
					"/configuration/components/viewTemplate/fields/field/writingSystems/id");
				node.InnerXml = "de";
				configFile.Save(configFilePath);

				project.LoadFromProjectDirectoryPath(projectDirectory.Path);

				Assert.That(File.Exists(Path.Combine(WeSayWordsProject.GetPathToLdmlWritingSystemsFolder(projectDirectory.Path), "de.ldml")), Is.True);
			}
		}

		[Test]
		public void LoadFromLiftLexiconPath_WritingsystemsAreInOldWsPrefsFormat_WritingSystemsAreMigrated()
		{
			var language = "english";
			using(var projectDirectory = new TemporaryFolder())
			{
				//setting up a minimal WeSay project with an old writingsystemprefs.xml file
				var project = new WeSayWordsProject();

				var wsPrefsFile = projectDirectory.GetNewTempFile(true);
				string liftFilePath = Path.Combine(projectDirectory.Path, "test.lift");
				wsPrefsFile.MoveTo(Path.Combine(projectDirectory.Path, "WritingSystemPrefs.xml"));
				File.Copy(Path.Combine(project.ApplicationTestDirectory, "PRETEND.WeSayConfig"), Path.Combine(projectDirectory.Path, "test.WeSayConfig"));
				File.Copy(WeSayWordsProject.PathToPretendLiftFile, liftFilePath);
				string wsToMigrateXml = WritingSystemPrefsFileContent.GetSingleWritingSystemXml(language, language, "", "", "", 12,
																					false, language, "", false, true);
				string english = WritingSystemPrefsFileContent.GetSingleWritingSystemXmlForLanguage("en");
				File.WriteAllText(wsPrefsFile.Path, WritingSystemPrefsFileContent.WrapWritingSystemXmlWithCollectionXml(wsToMigrateXml+english));

				project.LoadFromLiftLexiconPath(liftFilePath);

				string newLdmlWritingSystemFilePath =
					Path.Combine(WeSayWordsProject.GetPathToLdmlWritingSystemsFolder(projectDirectory.Path), "qaa-Zxxx-x-english-audio.ldml");
				AssertThatXmlIn.File(newLdmlWritingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
				AssertThatXmlIn.File(newLdmlWritingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(newLdmlWritingSystemFilePath).HasNoMatchForXpath("/ldml/identity/territory");
				AssertThatXmlIn.File(newLdmlWritingSystemFilePath).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-english-audio']");
			}
		}

		[Test]
		public void LoadFromLiftLexiconPath_LiftConfigFileAndOptionListContainVariousNonConformantAndOrOrphanedWritingSystems_WritingSystemsAreMigrated()
		{
			using (var projectDirectory = new TemporaryFolder("OrphanWritingSystemsTest"))
			{
				//Create Config, Lift and OptionList files as well as a writing system folder.
				//These files contain various orphans
				string configFilePath = Path.Combine(projectDirectory.Path, "test.WeSayConfig");
				File.WriteAllText(configFilePath, ConfigFileContentForTests.GetCompleteV8ConfigFile("en","config","de"));
				string liftFilePath = Path.Combine(projectDirectory.Path, "test.lift");
				File.WriteAllText(liftFilePath, LiftContentForTests.WrapEntriesInLiftElements("0.13",
					LiftContentForTests.GetSingleEntryWithWritingSystems("option", "de")
					));
				string writingSystemFolderPath = Path.Combine(projectDirectory.Path, "WritingSystems");
				Directory.CreateDirectory(writingSystemFolderPath);
				//Now populate the writing system repo with an "en" writing system and a "qaa-x-changedWs" writing system as well as
				//a changelog that  indicates that "x-changedWs" got changed to "qaa-x-changedWs"
				var wsRepo = LdmlInFolderWritingSystemRepository.Initialize(
					writingSystemFolderPath,
					OnWritingSystemMigrationHandler,
					OnWritingSystemLoadProblem,
					WritingSystemCompatibility.Flex7V0Compatible
				);
				var ws = new WritingSystemDefinition("en");
				var ws1 = new WritingSystemDefinition("x-changeme");
				wsRepo.Set(ws);
				wsRepo.Set(ws1);
				wsRepo.Save();
				ws1.SetAllComponents("fr", "Latn", "US", "x-CHANGED");
				wsRepo.Set(ws1);
				wsRepo.Save();

				//Now open the project
				var project = new WeSayWordsProject();
				project.LoadFromLiftLexiconPath(liftFilePath);

				Assert.That(project.WritingSystems.Contains("en"));
				Assert.That(project.WritingSystems.Contains("qaa-x-config"));
				Assert.That(project.WritingSystems.Contains("de"));
				Assert.That(project.WritingSystems.Contains("qaa-x-option"));
				Assert.That(project.WritingSystems.Contains("fr-Latn-US-x-CHANGED"));

				Assert.That(File.Exists(Path.Combine(writingSystemFolderPath, "en.ldml")));
				Assert.That(File.Exists(Path.Combine(writingSystemFolderPath, "qaa-x-config.ldml")));
				Assert.That(File.Exists(Path.Combine(writingSystemFolderPath, "de.ldml")));
				Assert.That(File.Exists(Path.Combine(writingSystemFolderPath, "qaa-x-option.ldml")));
				Assert.That(File.Exists(Path.Combine(writingSystemFolderPath, "fr-Latn-US-x-CHANGED.ldml")));

				var configFile = new ConfigFile(configFilePath);
				Assert.That(configFile.WritingSystemsInUse.Count(), Is.EqualTo(3));
				Assert.That(configFile.WritingSystemsInUse.All(wsId => wsId.Equals("en") || wsId.Equals("qaa-x-config") || wsId.Equals("de")));
				var liftFileHelper = new WritingSystemsInLiftFileHelper(wsRepo, liftFilePath);
				Assert.That(liftFileHelper.WritingSystemsInUse.Count(), Is.EqualTo(2));
				Assert.That(liftFileHelper.WritingSystemsInUse.All(wsId => wsId.Equals("qaa-x-option") || wsId.Equals("de")));
			}
		}

		private void OnWritingSystemLoadProblem(IEnumerable<WritingSystemRepositoryProblem> problems)
		{
			throw new NotImplementedException();
		}

		private void OnWritingSystemMigrationHandler(IEnumerable<LdmlVersion0MigrationStrategy.MigrationInfo> migrationinfo)
		{
			throw new NotImplementedException();
		}

		[Test]
		public void NewProject_WritingsystemFilesAreLatestVersion()
		{
			var namespaceManager = new XmlNamespaceManager(new NameTable());
			namespaceManager.AddNamespace("palaso", "urn://palaso.org/ldmlExtensions/v1");
			using (var projectFolder = new TemporaryFolder("MigrationTest"))
			{
				WeSayWordsProject.CreateEmptyProjectFiles(projectFolder.Path);
				foreach (var ldmlFilePath in Directory.GetFiles(WeSayWordsProject.GetPathToLdmlWritingSystemsFolder(projectFolder.Path)))
				{
					AssertThatXmlIn.File(ldmlFilePath).HasAtLeastOneMatchForXpath(String.Format("/ldml/special/palaso:version[@value='{0}']/@value", WritingSystemDefinition.LatestWritingSystemDefinitionVersion),
																	   namespaceManager);
				}
			}
		}

		[Test]
		public void NewProject_ContainsOnlyEnglishAndUnknownWritingSystems()
		{
			var namespaceManager = new XmlNamespaceManager(new NameTable());
			namespaceManager.AddNamespace("palaso", "urn://palaso.org/ldmlExtensions/v1");
			using (var projectFolder = new TemporaryFolder("MigrationTest"))
			{
				WeSayWordsProject.CreateEmptyProjectFiles(projectFolder.Path);
				string pathToLdmlWritingSystemsFolder =
					WeSayWordsProject.GetPathToLdmlWritingSystemsFolder(projectFolder.Path);
				Assert.IsTrue(File.Exists(Path.Combine(pathToLdmlWritingSystemsFolder, "en.ldml")));
				Assert.IsTrue(File.Exists(Path.Combine(pathToLdmlWritingSystemsFolder, "qaa-x-qaa.ldml")));
				Assert.AreEqual(2, Directory.GetFiles(pathToLdmlWritingSystemsFolder).Length);
			}
		}

		[Test]
		public void IsWritingSystemInUse_LiftFileContainsWritingSystem_ReturnsTrue()
		{
			using (var project = new ProjectDirectorySetupForTesting("").CreateLoadedProject())
			{
				File.WriteAllText(project.PathToLiftFile, @"<entry id='foo1'><lexical-unit><form lang='de'><text>fooOne</text></form></lexical-unit></entry>");
				Assert.That(project.IsWritingSystemUsedInLiftFile("de"), Is.True);
			}
		}

		[Test]
		public void IsWritingSystemInUse_WritingSystemIsNotUsed_ReturnsFalse()
		{
			using (var project = new ProjectDirectorySetupForTesting("").CreateLoadedProject())
			{
				Assert.That(project.IsWritingSystemUsedInLiftFile("de"), Is.False);
			}
		}

		[Test]
		public void IsWritingSystemInUseInLift_LiftFileContainsWritingSystem_ReturnsTrue()
		{
			using (var project = new ProjectDirectorySetupForTesting("").CreateLoadedProject())
			{
				File.WriteAllText(project.PathToLiftFile, @"<entry id='foo1'><lexical-unit><form lang='de'><text>fooOne</text></form></lexical-unit></entry>");
				Assert.That(project.IsWritingSystemUsedInLiftFile("de"), Is.True);
			}
		}

		[Test]
		public void IsWritingSystemInUseInLift_WritingSystemIsNotUsed_ReturnsFalse()
		{
			using (var project = new ProjectDirectorySetupForTesting("").CreateLoadedProject())
			{
				Assert.That(project.IsWritingSystemUsedInLiftFile("de"), Is.False);
			}
		}
	}
}