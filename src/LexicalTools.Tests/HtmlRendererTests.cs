using Autofac;
using NUnit.Framework;
using SIL.DictionaryServices.Model;
using SIL.Reporting;
using SIL.WritingSystems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.TestUtilities;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{


	[TestFixture]
	public class HtmlRendererTests
	{
		private LexEntryRepository _lexEntryRepository;
		private ViewTemplate _viewTemplate;
		private CurrentItemEventArgs _currentItem;

		private LexEntry _anotherApple;
		private LexEntry _entry;

		[SetUp]
		public void Setup()
		{
			InitializeForTests();
			var b = new ContainerBuilder();
			b.Register(c => new MediaNamingHelper(new[] { "en" }));

			// ReSharper disable once ObjectCreationAsStatement
			new ServiceLocatorAdapter(b.Build());
			SetupTestData();
		}
		[TearDown]
		public void TearDown()
		{
			if (_lexEntryRepository != null)
			{
				_lexEntryRepository.Dispose();
			}
			Sldr.Cleanup();
		}

		[Test]
		[Platform(Exclude = "Linux", Reason = "does not close progess dialog, and hangs, on mono when run after DictionaryControlTests")]
		// Mar2017 see WeSay.ConfigTool.Tests.BackupPlanControlTests.SetValues_Reopen_HasSameValues
		public void LexEntry_Unassigned_Focused_Throws()
		{
			_lexEntryRepository = null;
			Assert.Throws<ArgumentNullException>(() => HtmlRenderer.ToHtml(_entry, _currentItem, _lexEntryRepository, System.Drawing.Color.LightSeaGreen));
		}
		[Test]
		[Platform(Exclude = "Linux", Reason = "does not close progess dialog, and hangs, mono when run after DictionaryControlTests")]
		// Mar2017 see WeSay.ConfigTool.Tests.BackupPlanControlTests.SetValues_Reopen_HasSameValues
		public void NullEntryReturnsEmptyString()
		{
			String html = HtmlRenderer.ToHtml(null, _currentItem, _lexEntryRepository, System.Drawing.Color.LightSeaGreen);
			Assert.IsEmpty(html);
		}
		[Test]
		[Platform(Exclude = "Linux", Reason = "does not close progess dialog, and hangs, on mono when run after DictionaryControlTests")]
		// Mar2017 see WeSay.ConfigTool.Tests.BackupPlanControlTests.SetValues_Reopen_HasSameValues
		public void ConversionTest()
		{
			String html = HtmlRenderer.ToHtml(_entry, _currentItem, _lexEntryRepository, System.Drawing.Color.LightSeaGreen);
			Assert.IsTrue(html.Contains("contentEditable='false'"));
		}
		[Test]
		[Platform(Exclude = "Linux", Reason = "does not close progess dialog, and hangs, on mono when run after DictionaryControlTests")]
		// Mar2017 see WeSay.ConfigTool.Tests.BackupPlanControlTests.SetValues_Reopen_HasSameValues
		public void HomographEntryTest()
		{
			String html = HtmlRenderer.ToHtml(_anotherApple, _currentItem, _lexEntryRepository, System.Drawing.Color.LightSeaGreen);
			Assert.IsTrue(html.Contains("<sub>2</sub>"));
		}

		private void SetupTestData()
		{
			ErrorReport.IsOkToInteractWithUser = false;

			_currentItem = null;
			string lexicalForm = "test";
			string definition = "definition";
			string exampleSentence = "Test sentence";
			string exampleTranslation = "Translated sentence";
			_lexEntryRepository = (LexEntryRepository)
				WeSayWordsProject.Project.ServiceLocator.GetService(typeof(LexEntryRepository));
			_viewTemplate = (ViewTemplate)
						WeSayWordsProject.Project.ServiceLocator.GetService(typeof(ViewTemplate));

			var headwordWritingSystemIds = new List<string>(_viewTemplate.GetHeadwordWritingSystemIds());
			string wsA = headwordWritingSystemIds[0];
			string wsB = _viewTemplate.GetDefaultWritingSystemForField(definition).LanguageTag;
			HtmlRenderer.HeadWordWritingSystemId = _viewTemplate.HeadwordWritingSystems[0].LanguageTag;

			_entry = _lexEntryRepository.CreateItem();
			_entry.LexicalForm[wsA] = lexicalForm;
			LexSense sense = new LexSense();
			sense.Definition[wsB] = definition;
			LexExampleSentence example = new LexExampleSentence();
			example.Sentence[wsA] = exampleSentence;
			example.Translation[wsB] = exampleTranslation;
			sense.ExampleSentences.Add(example);
			_entry.Senses.Add(sense);
			CreateTestEntry("", "", "");
			CreateTestEntry("apple", "red thing", "An apple a day keeps the doctor away.");
			_anotherApple = CreateTestEntry("apple", "fruit", "An apple a day keeps the doctor away.");
			CreateTestEntry("banana", "yellow food", "Monkeys like to eat bananas.");
			CreateTestEntry("car", "small motorized vehicle", "Watch out for cars when you cross the street.");
			CreateTestEntry("bike", "vehicle with two wheels", "He rides his bike to school.");

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
			Sldr.Initialize(true);
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

			SIL.Lift.Utilities.CreateEmptyLiftFile(WeSayWordsProject.PathToPretendLiftFile, "InitializeForTests()", true);

			//setup writing systems
			Directory.CreateDirectory(pathToLdmlWsFolder);

			IWritingSystemRepository wsc = LdmlInFolderWritingSystemRepository.Initialize(
				pathToLdmlWsFolder);

			WritingSystemDefinition _ws1 = new WritingSystemDefinition(WritingSystemsIdsForTests.VernacularIdForTest);
			_ws1.DefaultFont = new FontDefinition("Arial");
			_ws1.DefaultFontSize = 30;
			_ws1.DefaultCollation = new IcuRulesCollationDefinition("standard");
			wsc.Set(_ws1);
			WritingSystemDefinition _ws2 = new WritingSystemDefinition(WritingSystemsIdsForTests.AnalysisIdForTest);
			_ws2.DefaultFont = new FontDefinition(FontFamily.GenericSansSerif.Name);
			_ws2.DefaultFontSize = new Font(FontFamily.GenericSansSerif, 12).Size;
			_ws2.DefaultCollation = new IcuRulesCollationDefinition("standard");
			wsc.Set(_ws2);
			WritingSystemDefinition _ws3 = new WritingSystemDefinition(WritingSystemsIdsForTests.OtherIdForTest);
			_ws3.DefaultFont = new FontDefinition("Arial");
			_ws3.DefaultFontSize = 15;
			_ws3.DefaultCollation = new IcuRulesCollationDefinition("standard");
			wsc.Set(_ws3);


			wsc.Save();

			project.SetupProjectDirForTests(WeSayWordsProject.PathToPretendLiftFile, new NullProgressNotificationProvider());
			project.BackupMaker = null;//don't bother. Modern tests which might want to check backup won't be using this old approach anyways.
			return project;
		}
	}
}
