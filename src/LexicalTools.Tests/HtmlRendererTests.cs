
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Contexts;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Autofac;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;
using WeSay.Project;
using WeSay.UI;
using WeSay.LexicalModel;
using Palaso.DictionaryServices.Model;
using Palaso.TestUtilities;
using WeSay.TestUtilities;

namespace WeSay.LexicalTools.Tests
{


	[TestFixture]
	public class HtmlRendererTests
	{
		private IServiceLocator Context { get; set; }
		private LexEntryRepository _lexEntryRepository;
		private ViewTemplate _viewTemplate;
		private CurrentItemEventArgs _currentItem;

		private TemporaryFolder _tempFolder;
		private string _filePath;

		private LexEntry empty;
		private LexEntry apple;
		private LexEntry anotherApple;
		private LexEntry banana;
		private LexEntry car;
		private LexEntry bike;
		private LexEntry _entry;

		[SetUp]
		public void Setup()
		{
			InitializeForTests();
			var b = new ContainerBuilder();
			b.Register(c => new MediaNamingHelper(new string[] { "en" }));

			Context = new WeSay.Project.ServiceLocatorAdapter(b.Build());
			SetupTestData();
		}
		[TearDown]
		public void TearDown()
		{
			if (_lexEntryRepository != null)
			{
				_lexEntryRepository.Dispose();
			}
			_tempFolder.Delete();
		}

		[Test]
		public void LexEntry_Unassigned_Focused_Throws()
		{
			_lexEntryRepository = null;
			Assert.Throws<ArgumentNullException>(() => HtmlRenderer.ToHtml(_entry, _currentItem, _lexEntryRepository, System.Drawing.Color.LightSeaGreen));
		}
		[Test]
		public void NullEntryReturnsEmptyString()
		{
			String html = HtmlRenderer.ToHtml(null, _currentItem, _lexEntryRepository, System.Drawing.Color.LightSeaGreen);
			Assert.IsEmpty(html);
		}
		[Test]
		public void ConversionTest()
		{
			String html = HtmlRenderer.ToHtml(_entry, _currentItem, _lexEntryRepository, System.Drawing.Color.LightSeaGreen);
			Assert.IsTrue(html.Contains("contentEditable='false'"));
		}
		[Test]
		public void HomographEntryTest()
		{
			String html = HtmlRenderer.ToHtml(anotherApple, _currentItem, _lexEntryRepository, System.Drawing.Color.LightSeaGreen);
			Assert.IsTrue(html.Contains("<sub>2</sub>"));
		}

		private void SetupTestData()
		{
			_tempFolder = new TemporaryFolder();
			_filePath = _tempFolder.GetTemporaryFile();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			_currentItem = null;
			string lexicalForm = "test";
			string definition = "definition";
			string exampleSentence = "Test sentence";
			string exampleTranslation = "Translated sentence";
			_lexEntryRepository = (LexEntryRepository)
				WeSayWordsProject.Project.ServiceLocator.GetService(typeof(LexEntryRepository));
			_viewTemplate = (ViewTemplate)
						WeSayWordsProject.Project.ServiceLocator.GetService(typeof(ViewTemplate));

			List<String> headwordWritingSystemIds = new List<string>(_viewTemplate.GetHeadwordWritingSystemIds());
			string wsA = headwordWritingSystemIds[0] ;
			string wsB = _viewTemplate.GetDefaultWritingSystemForField(definition).Id;
			HtmlRenderer.HeadWordWritingSystemId = _viewTemplate.HeadwordWritingSystem.Id;

			_entry = _lexEntryRepository.CreateItem();
			_entry.LexicalForm[wsA] = lexicalForm;
			LexSense sense = new LexSense();
			sense.Definition[wsB] = definition;
			LexExampleSentence example = new LexExampleSentence();
			example.Sentence[wsA] = exampleSentence;
			example.Translation[wsB] = exampleTranslation;
			sense.ExampleSentences.Add(example);
			_entry.Senses.Add(sense);
			empty = CreateTestEntry("", "", "");
			apple = CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away.");
			anotherApple = CreateTestEntry("apple", "fruit", "An apple a day keeps the doctor away.");
			banana = CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas.");
			car = CreateTestEntry("car",
								  "small motorized vehicle",
								  "Watch out for cars when you cross the street.");
			bike = CreateTestEntry("bike", "vehicle with two wheels", "He rides his bike to school.");

		}
		private LexEntry CreateTestEntry(string lexicalForm, string meaning, string exampleSentence)
		{
			List<String> headwordWritingSystemIds = new List<string>(_viewTemplate.GetHeadwordWritingSystemIds());
			string wsA = headwordWritingSystemIds[0];
			string wsB = _viewTemplate.GetDefaultWritingSystemForField("definition").Id;
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm[wsA] = lexicalForm;
			LexSense sense = new LexSense();
#if GlossMeaning
			sense.Gloss[GetSomeValidWsIdForField("SenseGloss")] = meaning;
#else
			sense.Definition[wsB] = meaning;
#endif
			LexExampleSentence example = new LexExampleSentence();
			example.Sentence[wsA] = exampleSentence;
			sense.ExampleSentences.Add(example);
			entry.Senses.Add(sense);
			_lexEntryRepository.SaveItem(entry);
			return entry;
		}
		private static string GetSomeValidWsIdForField(string fieldName)
		{
			return
					WeSayWordsProject.Project.DefaultViewTemplate.GetField(fieldName).
							WritingSystemIds[0];
		}
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
			IWritingSystemRepository wsc = LdmlInFolderWritingSystemRepository.Initialize(
				pathToLdmlWsFolder,
				OnMigrationHandler,
				OnWritingSystemLoadProblem,
				WritingSystemCompatibility.Flex7V0Compatible
			);

			IWritingSystemDefinition _ws1 = WritingSystemDefinition.Parse(WritingSystemsIdsForTests.VernacularIdForTest);
			_ws1.DefaultFontName = "Arial";
			_ws1.DefaultFontSize = 30;
			wsc.Set(_ws1);
			IWritingSystemDefinition _ws2 = WritingSystemDefinition.Parse(WritingSystemsIdsForTests.AnalysisIdForTest);
			_ws2.DefaultFontName = new Font(FontFamily.GenericSansSerif, 12).Name;
			_ws2.DefaultFontSize = new Font(FontFamily.GenericSansSerif, 12).Size;
			wsc.Set(_ws2);
			IWritingSystemDefinition _ws3 = WritingSystemDefinition.Parse(WritingSystemsIdsForTests.OtherIdForTest);
			_ws3.DefaultFontName = "Arial";
			_ws3.DefaultFontSize = 15;
			wsc.Set(_ws3);


			wsc.Save();

			project.SetupProjectDirForTests(WeSayWordsProject.PathToPretendLiftFile);
			project.BackupMaker = null;//don't bother. Modern tests which might want to check backup won't be using this old approach anyways.
			return project;
		}

		private static void OnWritingSystemLoadProblem(IEnumerable<WritingSystemRepositoryProblem> problems)
		{
			throw new ApplicationException("Unexpected WritingSystem load problem in test.");
		}

		private static void OnMigrationHandler(IEnumerable<LdmlVersion0MigrationStrategy.MigrationInfo> migrationinfo)
		{
			throw new ApplicationException("Unexpected WritingSystem migration in test.");
		}
	}
}
