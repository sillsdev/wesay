using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Reporting;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.LexicalTools.GatherBySemanticDomains;
using WeSay.Project;
using WeSay.Project.LocalizedList;
using Palaso.Lift.Options;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using WeSay.TestUtilities;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class GatherBySemanticDomainsTaskTests: TaskBaseTests
	{
		private LexEntryRepository _lexEntryRepository;
		private TemporaryFolder _tempFolder;
		private string _semanticDomainFilePath;
		private string _filePath;
		private ViewTemplate _viewTemplate;
		private static string _vernacularWritingSystemId = WritingSystemsIdsForTests.VernacularIdForTest;
		private GatherBySemanticDomainConfig _config;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Palaso.UI.WindowsForms.Keyboarding.KeyboardController.Initialize();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			Palaso.UI.WindowsForms.Keyboarding.KeyboardController.Shutdown();
		}

		[SetUp]
		public void Setup()
		{
			WeSayProjectTestHelper.InitializeForTests();

			WeSayWordsProject.Project.RemoveCache();
			_tempFolder = new TemporaryFolder();
			_filePath = _tempFolder.GetTemporaryFile();
			_semanticDomainFilePath = _tempFolder.GetTemporaryFile();

			_lexEntryRepository = new LexEntryRepository(_filePath);
			_viewTemplate = MakeViewTemplate("en");
			_config = GatherBySemanticDomainConfig.CreateForTests(_semanticDomainFilePath);
			_task = new GatherBySemanticDomainTask(_config,
												   _lexEntryRepository,
												   _viewTemplate,
												   new TaskMemoryRepository(),
												   new StringLogger());
			if (!File.Exists(Path.Combine(BasilProject.GetPretendProjectDirectory(), "SemDom.xml")))
			{
				File.Copy(Path.Combine(WeSayWordsProject.Project.ApplicationTestDirectory, "SemDom.xml"),
					Path.Combine(BasilProject.GetPretendProjectDirectory(), "SemDom.xml"));
			}
			if (!File.Exists(Path.Combine(BasilProject.GetPretendProjectDirectory(), "LocalizedLists-fr.xml")))
			{
				File.Copy(Path.Combine(WeSayWordsProject.Project.ApplicationTestDirectory, "LocalizedLists-fr.xml"),
					Path.Combine(BasilProject.GetPretendProjectDirectory(), "LocalizedLists-fr.xml"));
			}
		}

		private static LexSense AddNewSenseToEntry(LexEntry e)
		{
			var s = new LexSense();
			e.Senses.Add(s);
			return s;
		}

		private static ViewTemplate MakeViewTemplate(string nameAndQuestionWritingSystem)
		{
			Field semanticDomainField = new Field(LexSense.WellKnownProperties.SemanticDomainDdp4,
												  "LexSense",
												  new string[] {nameAndQuestionWritingSystem});
			semanticDomainField.OptionsListFile = "SemDom.xml";
			semanticDomainField.DataTypeName = "OptionRefCollection";

			ViewTemplate v = new ViewTemplate();
			Field lexicalFormField = new Field(Field.FieldNames.EntryLexicalForm.ToString(),
											   "LexEntry",
											   new string[] {_vernacularWritingSystemId});
			lexicalFormField.DataTypeName = "MultiText";

			v.Add(lexicalFormField);
			v.Add(semanticDomainField);

			Field defnField = new Field(LexSense.WellKnownProperties.Definition, "LexSense", new string[] { "en" });
			defnField.IsMeaningField = true;

			v.Add(defnField);

			if(!v.WritingSystems.Contains("en"))
			{
				v.WritingSystems.Set(WritingSystemDefinition.Parse("en"));
			}
			return v;
		}

		[TearDown]
		public void TearDown()
		{
			if (_task.IsActive)
			{
				_task.Deactivate(); //needed for disposal of controls
			}
			_lexEntryRepository.Dispose();
			_tempFolder.Delete();
		}

		private GatherBySemanticDomainTask Task
		{
			get
			{
				if (!_task.IsActive)
				{
					_task.Activate();
				}
				return ((GatherBySemanticDomainTask) _task);
			}
		}

		private LexEntry AddEntryToRecordList(string lexicalForm, string gloss, string optionalDomainToAdd)
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, lexicalForm);
			AddSenseToEntry(e, gloss, optionalDomainToAdd);
			_lexEntryRepository.SaveItem(e);
			return e;
		}

		private void AddSenseToEntry(LexEntry e, string gloss, string domainToAdd)
		{
			LexSense s = AddNewSenseToEntry(e);
			if(!string.IsNullOrEmpty(gloss))
				s.Definition.SetAlternative("en", gloss);
			if (!string.IsNullOrEmpty(domainToAdd))
			{
				OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
						LexSense.WellKnownProperties.SemanticDomainDdp4);

				o.Add(domainToAdd);
			}
			_lexEntryRepository.SaveItem(e);
		}

		[Test]
		public void TaskMemory_ValueOK_ReturnsToSameSpot()
		{
			var memory = new TaskMemoryRepository();
			var task = new GatherBySemanticDomainTask(GatherBySemanticDomainConfig.CreateForTests(_semanticDomainFilePath),
															_lexEntryRepository,
															_viewTemplate,
															memory,
															new StringLogger());

			task.Activate();
			task.CurrentDomainIndex = 5;
			task.CurrentQuestionIndex = 2;
			task.Deactivate();

			var task2 = new GatherBySemanticDomainTask(GatherBySemanticDomainConfig.CreateForTests(_semanticDomainFilePath),
															_lexEntryRepository,
															_viewTemplate,
															memory,
															new StringLogger());
			task2.Activate();
			Assert.AreEqual(5,task2.CurrentDomainIndex);
			Assert.AreEqual(2, task2.CurrentQuestionIndex);
			task2.Deactivate();
		}

		[Test]
		public void TaskMemory_DomainValueOutOfRange_RevertsToDefault()
		{

			var config = GatherBySemanticDomainConfig.CreateForTests(_semanticDomainFilePath);
			var memory = new TaskMemoryRepository();
			memory.FindOrCreateSettingsByTaskId(config.TaskName).Set(GatherBySemanticDomainTask.DomainIndexTaskMemoryKey, "200000");

			var task = new GatherBySemanticDomainTask(config,
															 _lexEntryRepository,
															 _viewTemplate,
															 memory,
															 new StringLogger());

			task.Activate();
			Assert.AreEqual(0, task.CurrentDomainIndex);
			Assert.AreEqual(0, task.CurrentQuestionIndex);
			task.Deactivate();
		}

		[Test]
		public void TaskMemory_QuestionValueOutOfRange_RevertsToDefault()
		{

			var config = GatherBySemanticDomainConfig.CreateForTests(_semanticDomainFilePath);
			var memory = new TaskMemoryRepository();
			memory.FindOrCreateSettingsByTaskId(config.TaskName).Set(GatherBySemanticDomainTask.DomainIndexTaskMemoryKey, "1");
			memory.FindOrCreateSettingsByTaskId(config.TaskName).Set(GatherBySemanticDomainTask.QuestionIndexTaskMemoryKey, "200000");

			var task = new GatherBySemanticDomainTask(config,
															 _lexEntryRepository,
															 _viewTemplate,
															 memory,
															 new StringLogger());

			task.Activate();
			Assert.AreEqual(1, task.CurrentDomainIndex);
			Assert.AreEqual(0, task.CurrentQuestionIndex);
			task.Deactivate();
		}

		[Test]
		public void ConstructWithTemplate()
		{
			Assert.IsNotNull(new GatherBySemanticDomainTask( GatherBySemanticDomainConfig.CreateForTests(_semanticDomainFilePath),
															_lexEntryRepository,
															_viewTemplate,
															new TaskMemoryRepository(),
															new StringLogger()));
		}

		[Test]
		public void ConstructWithTemplate_NullTemplate_Throws()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new GatherBySemanticDomainTask(GatherBySemanticDomainConfig.CreateForTests(_semanticDomainFilePath),
											   _lexEntryRepository,
											   null, new TaskMemoryRepository(), new StringLogger()));
		}


		[Test]
		public void WordWritingSystem()
		{
			Assert.AreEqual(_vernacularWritingSystemId, Task.WordWritingSystemId);
			Task.Deactivate();
		}

		[Test]
		public void SemanticDomainWritingSystem()
		{
			Assert.AreEqual("en", Task.SemanticDomainWritingSystemId);
			Task.Deactivate();
		}

		#region Current

		[Test]
		public void CurrentDomainIndex_Initial_0()
		{
			Assert.AreEqual(0, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		[Test]
		public void CurrentDomainIndex_Leave_ReturnToPreviousPosition()
		{
			Task.CurrentDomainIndex = 2;
			//task gets automatically activated in Task accessor
			Assert.AreEqual(2, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		[Test]
		public void CurrentDomainIndex_Set()
		{
			Task.CurrentDomainIndex = 2;
			Assert.AreEqual(2, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		[Test]
		public void CurrentDomainIndex_SetNegative_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.CurrentDomainIndex = -1);
			Task.Deactivate();
		}

		[Test]
		public void CurrentDomainIndex_SetGreaterThanCountOfDomains_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.CurrentDomainIndex = Task.DomainKeys.Count);
			Task.Deactivate();
		}

		[Test]
		public void Domains()
		{
			Assert.AreEqual(106, Task.DomainKeys.Count);
			Task.Deactivate();
		}

		[Test]
		public void CurrentQuestionIndex_Initial_0()
		{
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Task.Deactivate();
		}

		[Test]
		public void CurrentQuestionIndex_SetNegative_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.CurrentQuestionIndex = -1);
			Task.Deactivate();
		}

		[Test]
		public void CurrentQuestionIndex_SetGreaterThanCountOfQuestions_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.CurrentQuestionIndex = Task.Questions.Count);
			Task.Deactivate();
		}

		[Test]
		public void CurrentDomain()
		{
			Assert.IsNotNull(Task.CurrentDomainKey);
			Task.Deactivate();
		}

		[Test]
		public void CurrentQuestion()
		{
			Assert.IsNotNull(Task.CurrentQuestion);
			Task.Deactivate();
		}

		[Test]
		public void CurrentQuestionIndex_Leave_ReturnToPreviousPosition()
		{
			Task.CurrentQuestionIndex = 2;
			//task gets automatically activated in Task accessor
			Assert.AreEqual(2, Task.CurrentQuestionIndex);
			Task.Deactivate();
		}

		[Test]
		public void CurrentQuestionIndex_SetCurrentDomainIndex_0()
		{
			Task.CurrentQuestionIndex = 2;
			Task.CurrentDomainIndex = 2;
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Task.Deactivate();
		}

		[Test]
		public void Questions_DifferentDomain_DifferentQuestions()
		{
			List<string> questions = Task.Questions;
			Task.CurrentDomainIndex = 2;
			Assert.AreNotEqual(questions, Task.Questions);
			Task.Deactivate();
		}

		#endregion

		#region WordList

		[Test]
		public void CurrentWords()
		{
			Assert.IsNotNull(Task.CurrentWords);
			Task.Deactivate();
		}

		[Test]
		public void CurrentWords_NoWords_Empty()
		{
			Assert.IsEmpty(Task.CurrentWords);
			Task.Deactivate();
		}

		[Test]
		public void CurrentWords_HasWords_NotEmpty()
		{
			Task.AddWord("peixe");
			Assert.IsNotEmpty(Task.CurrentWords);
			Task.Deactivate();
		}

		[Test]
		public void CurrentWords_HasWords_ContainsWord()
		{
			Task.CurrentDomainIndex = 1;
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			LexSense s = AddNewSenseToEntry(e);
			s.Definition.SetAlternative("en", "fish");
			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);

			Task.CurrentDomainIndex = 0;
			var words = Task.CurrentWords;
			Assert.IsTrue(Task.CurrentWords.Any(w => w.Vernacular.Form == "peixe"));
			Task.Deactivate();
		}

		[Test]
		public void CurrentWords_ChangeQuestion_SameWordList()
		{
			var wordList = Task.CurrentWords;
			Task.CurrentQuestionIndex = 2;
			Assert.AreSame(wordList, Task.CurrentWords);
			Task.Deactivate();
		}

		[Test]
		public void CurrentWords_ChangeDomain_NewWordList()
		{
			var wordList = Task.CurrentWords;
			Task.CurrentDomainIndex = 2;
			Assert.AreNotSame(wordList, Task.CurrentWords);
			Task.Deactivate();
		}

		[Test]
		public void CurrentWords_WordExistsWithTwoDefsHavingSemanticDomain_ShowsUpOnce()
		{
			Task.CurrentDomainIndex = 1;
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);
			LexEntry e = AddEntryToRecordList("raposa", "fox", Task.DomainKeys[0]);
			AddSenseToEntry(e, "special", Task.DomainKeys[0]);
			AddEntryToRecordList("cachorro", "dog", Task.DomainKeys[0]);
			Task.CurrentDomainIndex = 0;
			Assert.AreEqual(3, Task.CurrentWords.Count);
			Task.Deactivate();
		}

		#endregion

		[Test]
		public void AddWord_null_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => Task.AddWord(null));
			Task.Deactivate();
		}


		[Test]
		public void AddWord_NewWord_AddedToDatabase()
		{
			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord("vernacular", String.Empty);
			Assert.AreEqual(originalCount + 1, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		/// <summary>
		/// regression test of WS-15019
		/// </summary>
		/// <remarks>
		/// Using the sorter causes XElement.Parse() to be called, which chokes on '\u001F'.
		/// </remarks>
		[Test]
		[ExpectedException("Palaso.Reporting.ErrorReport+ProblemNotificationSentToUserException", ExpectedMessage="character", MatchType=MessageMatch.Contains)]
		public void AddWord_WordConsistsOfOnlySegmentSeparatorCharacter_AddedToDatabase()
		{
			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord('\u001F'.ToString(), String.Empty);
			Assert.AreEqual(originalCount + 1, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void RemainingCount_Initially_RemainingCountEqualsReferenceCount()
		{
			Assert.AreEqual(Task.GetReferenceCount(), Task.GetRemainingCount());
			Task.Deactivate();
		}

		[Test]
		public void RemainingCount_NotYetActivated_NotComputedConstant()
		{
			Assert.AreEqual(TaskBase.CountNotComputed, _task.GetRemainingCount());
		}

		[Test]
		public void ReferenceCount_NotYetActivated_NotComputedConstant()
		{
			Assert.AreEqual(TaskBase.CountNotComputed, _task.GetReferenceCount());
		}

		[Test]
		public void RemainingCount_AddWordToPreviouslyEmptyDomain_RemainingCountDecreases()
		{
			int originalCount = Task.GetRemainingCount();
			Task.CurrentDomainIndex = 3;
			Task.AddWord("vernacular", String.Empty);
			Assert.AreEqual(originalCount - 1, Task.GetRemainingCount());
			Task.Deactivate();
		}

		[Test]
		public void RemainingCount_RemoveWordCausingEmptyDomain_RemainingCountIncreases()
		{
			Task.CurrentDomainIndex = 3;
			Task.AddWord("boo");
			int originalCount = Task.GetRemainingCount();
			Task.PrepareToMoveWordToEditArea("boo");
			Assert.AreEqual(originalCount + 1, Task.GetRemainingCount());
			Task.Deactivate();
		}

		[Test]
		public void AddWord_NewWord_AddedToCurrentWords()
		{
			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord("vernacular", String.Empty);
			Assert.IsTrue(Task.CurrentWords.Any(w=>w.Vernacular.Form=="vernacular"));
			Assert.AreEqual(originalCount + 1, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void AddWord_WordExistsWithDefAndSemanticDomain_NotAdded()
		{
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);
			AddEntryToRecordList("raposa", "fox", Task.DomainKeys[0]);
			AddEntryToRecordList("cachorro", "dog", Task.DomainKeys[0]);

			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord("raposa");
			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_null_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => Task.PrepareToMoveWordToEditArea((GatherBySemanticDomainTask.WordDisplay) null));
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_HasOnlyLexemeForm_DeletesWord()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			LexSense s = new LexSense();
			e.Senses.Add(s);
			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount - 1, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_WordExistsWithDefAndSemanticDomain_Disassociated()
		{
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);
			AddEntryToRecordList("raposa", "fox", Task.DomainKeys[0]);
			AddEntryToRecordList("cachorro", "dog", Task.DomainKeys[0]);

			Task.PrepareToMoveWordToEditArea("raposa");
			Assert.IsFalse(Task.CurrentWords.Any(w=>w.Vernacular.Form=="raposa"));
			Assert.AreEqual(2, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void SetMeaningMultiTextForWordRecentlyMovedToEditArea_WordHad1Meaning_SetProperly()
		{
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);

			Task.PrepareToMoveWordToEditArea("peixe");
			var meaning = Task.GetMeaningForWordRecentlyMovedToEditArea();

			Assert.AreEqual("fish", meaning["en"]);
			Task.Deactivate();
		}


		[Test]
		public void SetMeaningMultiTextForWordRecentlyMovedToEditArea_WordHad0Meaning_SetProperly()
		{
			AddEntryToRecordList("peixe", null, Task.DomainKeys[0]);

			Task.PrepareToMoveWordToEditArea("peixe");
			var meaning = Task.GetMeaningForWordRecentlyMovedToEditArea();

			Assert.IsFalse(meaning.ContainsAlternative("en"));
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_WordExistsWithTwoDefsHavingSemanticDomain_DisassociatesBothSenses()
		{
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);
			LexEntry e = AddEntryToRecordList("raposa", "fox", Task.DomainKeys[0]);
			AddSenseToEntry(e, "special", Task.DomainKeys[0]);
			AddEntryToRecordList("cachorro", "dog", Task.DomainKeys[0]);

			Task.PrepareToMoveWordToEditArea("raposa");
			Assert.IsFalse(Task.CurrentWords.Any(w=>w.Vernacular.Form==("raposa")));
			Assert.AreEqual(2, Task.CurrentWords.Count);
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_HasAnotherSense_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			LexSense s = AddNewSenseToEntry(e);
			s.Definition.SetAlternative("en", "fish");
			s = new LexSense();
			e.Senses.Add(s);
			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_HasAnotherSense_RemovesEmptySense()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			LexSense s = AddNewSenseToEntry(e);
			s.Definition.SetAlternative("en", "fish");
			s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Assert.AreEqual(1, e.Senses.Count);
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_HasTwoLexicalForms_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			e.LexicalForm.SetAlternative("v", "peshi");
			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_HasCustomFieldInEntry_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			MultiText mt = e.GetOrCreateProperty<MultiText>("custom");
			mt["en"] = "hello";

			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_HasCustomFieldInSense_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);

			MultiText mt = e.GetOrCreateProperty<MultiText>("custom");
			mt["en"] = "hello";
			_lexEntryRepository.SaveItem(e);

			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_DoesNotHaveSemanticDomainFieldInSense_DoNothing()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			AddNewSenseToEntry(e);

			MultiText mt = e.GetOrCreateProperty<MultiText>("custom");
			mt["en"] = "hello";
			_lexEntryRepository.SaveItem(e);

			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void SimluateEditing_NoGloss_HasCustomFieldInExample_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			LexSense s = AddNewSenseToEntry(e);

			var o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);

			var example = new LexExampleSentence();
			s.ExampleSentences.Add(example);
			var optionRef = example.GetOrCreateProperty<OptionRef>("custom");
			optionRef.Value = "hello";
			_lexEntryRepository.SaveItem(e);

			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount-1, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		//Regression (WS-34245) Corrections to words with meanings making new words in Gather by SemDom. The problem is that words with glosses
		[Test]
		public void PrepareToMoveWordToEditArea_HasMeaning_RemovesWord()
		{
			var originalCount = AddEntry(MakeEntryWithMeaning());
			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount-1, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		//Regression (WS-34245) Corrections to words with meanings making new words in Gather by SemDom. The problem is that words with glosses
		[Test]
		public void SimulateEditingSpelling_Simple_EntryCountUnchanged()
		{
			var originalCount = AddEntry(MakeEntryWithMeaning());
			Task.PrepareToMoveWordToEditArea("peixe");
			Task.AddWord("peixe2", "the meaning");
			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void SimulateEditingSpelling_HasExample_ExamplePreserved()
		{
			var e = MakeEntryWithMeaning();
			var lexExampleSentence = new LexExampleSentence();
			lexExampleSentence.Sentence.SetAlternative(_vernacularWritingSystemId, "an example");
			e.Senses[0].ExampleSentences.Add(lexExampleSentence);
			var originalCount = AddEntry(e);
			Task.PrepareToMoveWordToEditArea("peixe");
			var modifiedEntries = Task.AddWord("peixe2", "the meaning");
			Assert.AreEqual("an example", modifiedEntries.First().Senses[0].ExampleSentences[0].Sentence[_vernacularWritingSystemId]);
			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		[Test]
		public void SimulateEditingSpelling_HasPartOfSpeech_POSPreserved()
		{
			var e = MakeEntryWithMeaning();
			e.Senses[0].GetOrCreateProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech).Key = "noun";
			AddEntry(e);
			Task.PrepareToMoveWordToEditArea("peixe");
			var modifiedEntries = Task.AddWord("peixe2", "the meaning");
			Assert.AreEqual(1, modifiedEntries.Count());
			Assert.AreEqual(1, modifiedEntries.First().Senses.Count());
			Assert.AreEqual("noun", modifiedEntries.First().Senses[0].GetProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech).Key);
			Task.Deactivate();
		}

		[Test]
		public void SimulateEditingHeadwordSpelling_MeaningFieldIsEnabled_ChangeCausesMatchWIthAnExistingEntry_EnteredMeaningIsIdenticalToExistingSense_SenseMerged()
		{
			_config.ShowMeaningField = true;
			//ok, so we have this word out there with definition in 2 languages, and an example
			var preexistingEntry = MakeEntryWithMeaning("peixe2");
			preexistingEntry.Senses[0].Definition.SetAlternative("FR", "oui");
			var example1 = new LexExampleSentence();
			example1.Sentence.SetAlternative(_vernacularWritingSystemId, "example1");
			preexistingEntry.Senses[0].ExampleSentences.Add(example1);

			var entryWithMistake = MakeEntryWithMeaning("peixe1");
			var lexExampleSentence = new LexExampleSentence();
			lexExampleSentence.Sentence.SetAlternative(_vernacularWritingSystemId, "example2");
			entryWithMistake.Senses[0].ExampleSentences.Add(lexExampleSentence);
			var originalCount = AddEntry(entryWithMistake);

			Task.PrepareToMoveWordToEditArea(new GatherBySemanticDomainTask.WordDisplay
												 {
													 Vernacular = entryWithMistake.LexicalForm.GetBestAlternative(new[] { _vernacularWritingSystemId }),
													 Meaning = entryWithMistake.Senses[0].Definition.GetBestAlternative(new[] { Task.DefinitionWritingSystem.Id })
												 });
			Assert.AreEqual(originalCount-1, _lexEntryRepository.CountAllItems(),"expected it to be removed from the lexicon");
			var modifiedEntries = Task.AddWord("peixe2", "the meaning");
			Assert.AreEqual(1, modifiedEntries.Count);
			Assert.AreEqual(preexistingEntry, modifiedEntries[0], "expected the pre-existing 'peixe2' to be the recipient of the move, now that the spelling was changed to match it.");
			Assert.AreEqual(1, preexistingEntry.Senses.Count, "expected the incoming sense to merge with the existing one");
			Assert.AreEqual("oui", modifiedEntries.First().Senses[0].Definition.GetExactAlternative("FR"));
			Assert.AreEqual(2, preexistingEntry.Senses[0].ExampleSentences.Count, "expected to end up with 2 example sentences");
			Assert.AreEqual("example1", modifiedEntries.First().Senses[0].ExampleSentences[0].Sentence[_vernacularWritingSystemId]);
			Assert.AreEqual("example2", modifiedEntries.First().Senses[0].ExampleSentences[1].Sentence[_vernacularWritingSystemId]);
			Task.Deactivate();
		}

		[Test]
		public void SimulateEditingHeadwordSpelling_MeaningFieldIsEnabled_ChangeCausesMatchWIthAnExistingEntry_EnteredMeaningIsNotIdenticalToExistingSense_SenseIsAdded()
		{
			_config.ShowMeaningField = true;
			//ok, so we have this word out there with definition in 2 languages, and an example
			var preexistingEntry = MakeEntryWithMeaning("peixe2");
			preexistingEntry.Senses[0].Definition.SetAlternative("FR", "oui");
			var example1 = new LexExampleSentence();
			example1.Sentence.SetAlternative(_vernacularWritingSystemId, "example1");
			preexistingEntry.Senses[0].ExampleSentences.Add(example1);

			var entryWithMistake = MakeEntryWithMeaning("peixe1");
			var lexExampleSentence = new LexExampleSentence();
			lexExampleSentence.Sentence.SetAlternative(_vernacularWritingSystemId, "example2");
			entryWithMistake.Senses[0].ExampleSentences.Add(lexExampleSentence);
			var originalCount = AddEntry(entryWithMistake);

			Task.PrepareToMoveWordToEditArea(new GatherBySemanticDomainTask.WordDisplay
												{
													Vernacular = entryWithMistake.LexicalForm.GetBestAlternative(new[] { _vernacularWritingSystemId }),
													Meaning = entryWithMistake.Senses[0].Definition.GetBestAlternative(new[] { Task.DefinitionWritingSystem.Id })
												});
			Assert.AreEqual(originalCount - 1, _lexEntryRepository.CountAllItems(), "expected it to be removed from the lexicon");
			var modifiedEntries = Task.AddWord("peixe2", "the meaning2");
			Assert.AreEqual(1, modifiedEntries.Count);
			Assert.AreEqual(preexistingEntry, modifiedEntries[0], "expected the pre-existing 'peixe2' to be the recipient of the move, now that the spelling was changed to match it.");
			Assert.AreEqual(2, preexistingEntry.Senses.Count, "expected the incoming sense to be added to the existing one");
			Assert.AreEqual("oui", modifiedEntries.First().Senses[0].Definition.GetExactAlternative("FR"));
			Assert.AreEqual(1, preexistingEntry.Senses[0].ExampleSentences.Count, "expected to end up with 1 example sentences");
			Assert.AreEqual("example1", modifiedEntries.First().Senses[0].ExampleSentences[0].Sentence[_vernacularWritingSystemId]);
			Assert.AreEqual(1, preexistingEntry.Senses[1].ExampleSentences.Count, "expected to end up with 1 example sentences");
			Assert.AreEqual("example2", modifiedEntries.First().Senses[1].ExampleSentences[0].Sentence[_vernacularWritingSystemId]);
			Task.Deactivate();
		}

		[Test]
		public void SimulateEditingHeadwordSpelling_MeaningFieldIsNotEnabled_ChangeCausesMatchWIthAnExistingEntry_ExistingMeaningIsIdenticalToExistingSense_SenseMerged()
		{
			//ok, so we have this word out there with definition in 2 languages, and an example
			var preexistingEntry = MakeEntryWithMeaning("peixe2");
			preexistingEntry.Senses[0].Definition.SetAlternative("FR", "oui");
			var example1 = new LexExampleSentence();
			example1.Sentence.SetAlternative(_vernacularWritingSystemId, "example1");
			preexistingEntry.Senses[0].ExampleSentences.Add(example1);

			var entryWithMistake = MakeEntryWithMeaning("peixe1");

			var lexExampleSentence = new LexExampleSentence();
			lexExampleSentence.Sentence.SetAlternative(_vernacularWritingSystemId, "example2");
			entryWithMistake.Senses[0].ExampleSentences.Add(lexExampleSentence);
			var originalCount = AddEntry(entryWithMistake);
			Task.PrepareToMoveWordToEditArea("peixe1");
			Assert.AreEqual(originalCount - 1, _lexEntryRepository.CountAllItems(), "expected it to be removed from the lexicon");
			var modifiedEntries = Task.AddWord("peixe2", "");
			Assert.AreEqual(1, modifiedEntries.Count);
			Assert.AreEqual(preexistingEntry, modifiedEntries[0], "expected the pre-existing 'peixe2' to be the recipient of the move, now that the spelling was changed to match it.");
			Assert.AreEqual(1, preexistingEntry.Senses.Count, "expected the incoming sense to merge with the existing one");
			Assert.AreEqual("oui", modifiedEntries.First().Senses[0].Definition.GetExactAlternative("FR"));
			Assert.AreEqual(2, preexistingEntry.Senses[0].ExampleSentences.Count, "expected to end up with 2 example sentences");
			Assert.AreEqual("example1", modifiedEntries.First().Senses[0].ExampleSentences[0].Sentence[_vernacularWritingSystemId]);
			Assert.AreEqual("example2", modifiedEntries.First().Senses[0].ExampleSentences[1].Sentence[_vernacularWritingSystemId]);
			Task.Deactivate();
		}

		[Test]
		public void SimulateEditingHeadwordSpelling_MeaningFieldIsNotEnabled_ChangeCausesMatchWIthAnExistingEntry_ExistingMeaningIsNotIdenticalToExistingSense_SenseIsAdded()
		{
			//ok, so we have this word out there with definition in 2 languages, and an example
			var preexistingEntry = MakeEntryWithMeaning("peixe2");
			preexistingEntry.Senses[0].Definition.SetAlternative("FR", "oui");
			var example1 = new LexExampleSentence();
			example1.Sentence.SetAlternative(_vernacularWritingSystemId, "example1");
			preexistingEntry.Senses[0].ExampleSentences.Add(example1);

			var entryWithMistake = MakeEntryWithMeaning("peixe1");
			//change the "en" definition to anything but "the meaning"
			entryWithMistake.Senses[0].Definition["en"] = "not \"the meaning\"";

			var lexExampleSentence = new LexExampleSentence();
			lexExampleSentence.Sentence.SetAlternative(_vernacularWritingSystemId, "example2");
			entryWithMistake.Senses[0].ExampleSentences.Add(lexExampleSentence);
			var originalCount = AddEntry(entryWithMistake);
			Task.PrepareToMoveWordToEditArea("peixe1");
			Assert.AreEqual(originalCount - 1, _lexEntryRepository.CountAllItems(), "expected it to be removed from the lexicon");
			var modifiedEntries = Task.AddWord("peixe2", "");
			Assert.AreEqual(1, modifiedEntries.Count);
			Assert.AreEqual(preexistingEntry, modifiedEntries[0], "expected the pre-existing 'peixe2' to be the recipient of the move, now that the spelling was changed to match it.");
			Assert.AreEqual(2, preexistingEntry.Senses.Count, "expected the incoming sense to be added to the existing one");
			Assert.AreEqual("oui", modifiedEntries.First().Senses[0].Definition.GetExactAlternative("FR"));
			Assert.AreEqual(1, preexistingEntry.Senses[0].ExampleSentences.Count, "expected to end up with 1 example sentences");
			Assert.AreEqual("example1", modifiedEntries.First().Senses[0].ExampleSentences[0].Sentence[_vernacularWritingSystemId]);
			Assert.AreEqual(1, preexistingEntry.Senses[1].ExampleSentences.Count, "expected to end up with 1 example sentences");
			Assert.AreEqual("example2", modifiedEntries.First().Senses[1].ExampleSentences[0].Sentence[_vernacularWritingSystemId]);
			Task.Deactivate();
		}

		[Test]
		public void SimulateEditingHeadwordSpelling_MeaningFieldIsNotEnabled_EntryHasMultipleSensesSomeMatchingSemDomOthersNot_AllSensesMatchingSemanticDomainAreMovedToNewEntry_SensesNotMatchingStayOnOldEntry()
		{
			_config.ShowMeaningField = false;
			var entry = MakeEntryWithMeaning("peixe2");
			entry.Senses.Add(GetSenseWithDefAndSemDom(Task.DefinitionWritingSystem.Id, "de_form", Task.DomainKeys[0]));
			entry.Senses.Add(GetSenseWithDefAndSemDom("fr", "oui", Task.DomainKeys[1]));

			Assert.AreEqual(1, _lexEntryRepository.CountAllItems());
			Task.PrepareToMoveWordToEditArea("peixe2");
			Assert.AreEqual(1, _lexEntryRepository.CountAllItems(), "should stay in repo because of non matching sem dom");

			Task.AddWord("peixe1", "");
			Assert.AreEqual(2, _lexEntryRepository.CountAllItems(), "should have created a new entry.");
			Assert.AreEqual(1, entry.Senses.Count, "expected to end up with 1 sense as the two matching the current domain should have been moved");
			Assert.That(entry.Senses.All(s => !s.GetProperty<OptionRefCollection>(LexSense.WellKnownProperties.SemanticDomainDdp4).Contains(Task.DomainKeys[0])), Is.True);
			var newEntry = _lexEntryRepository.GetEntriesWithMatchingLexicalForm("peixe1",WeSayWordsProject.Project.WritingSystems.Get(_vernacularWritingSystemId))[0].RealObject;
			Assert.AreEqual(2, newEntry.Senses.Count, "expected to end up with 2 sense as the two matching the current domain should have been moved");
			Assert.That(newEntry.LexicalForm[_vernacularWritingSystemId], Is.EqualTo("peixe1"));
			Assert.That(newEntry.Senses.All(s => s.GetProperty<OptionRefCollection>(LexSense.WellKnownProperties.SemanticDomainDdp4).Contains(Task.DomainKeys[0])), Is.True);
			Task.Deactivate();
		}

		private LexSense GetSenseWithDefAndSemDom(string defWs, string defForm, string semDom)
		{
			var newSense = new LexSense();
			newSense.Definition.SetAlternative(defWs, defForm);
			var semDoms = newSense.GetOrCreateProperty<OptionRefCollection>(LexSense.WellKnownProperties.SemanticDomainDdp4);
			semDoms.Add(semDom.Trim());
			return newSense;
		}

		[Test]
		public void SimulateEditingHeadwordSpelling_MeaningFieldIsNotEnabled_EntryHasMultipleSensesAllMatchingSemDom_AllSensesMatchingSemanticDomainAreMovedToNewEntry_OldEntryIsDeleted()
		{
			_config.ShowMeaningField = false;
			var entry = MakeEntryWithMeaning("peixe2");
			entry.Senses.Add(GetSenseWithDefAndSemDom(Task.DefinitionWritingSystem.Id, "de_word", Task.DomainKeys[0]));
			var sensesToMove =
				entry.Senses.Where(
					s => s.GetProperty<OptionRefCollection>(LexSense.WellKnownProperties.SemanticDomainDdp4).Contains(Task.DomainKeys[0]));

			Task.PrepareToMoveWordToEditArea("peixe2");
			Assert.AreEqual(0, _lexEntryRepository.CountAllItems(), "expected it to be removed from the lexicon");

			Task.AddWord("peixe1", "");
			Assert.AreEqual(1, _lexEntryRepository.CountAllItems(), "expected new entry 'peixe1' in lexicon");
			var newEntry = _lexEntryRepository.GetEntriesWithMatchingLexicalForm("peixe1", WeSayWordsProject.Project.WritingSystems.Get(_vernacularWritingSystemId))[0].RealObject;
			Assert.That(sensesToMove.All(movedSense => newEntry.Senses.Contains(movedSense)));
			Assert.That(newEntry.Senses.All(s=>s.GetProperty<OptionRefCollection>(LexSense.WellKnownProperties.SemanticDomainDdp4).Contains(Task.DomainKeys[0])));
			Task.Deactivate();
		}

		[Test]
		public void SimulateEditingHeadwordSpelling_MeaningFieldIsEnabled_EntryHasMultipleSensesSomeMatchingSemDomOthersNot_SensesMatchingMovedMeaningIsMovedToNewEntry_SensesNotMatchingStayOnOldEntry()
		{
			_config.ShowMeaningField = true;
			var entry = MakeEntryWithMeaning("peixe2");
			var firstSense = entry.Senses[0];
			entry.Senses.Add(GetSenseWithDefAndSemDom(Task.DefinitionWritingSystem.Id, "de_word", Task.DomainKeys[0]));
			var secondSense = entry.Senses[1];

			Task.PrepareToMoveWordToEditArea(new GatherBySemanticDomainTask.WordDisplay
												 {
													 Vernacular = entry.LexicalForm.GetBestAlternative(new []{_vernacularWritingSystemId}),
													 Meaning = secondSense.Definition.GetBestAlternative(new[]{Task.DefinitionWritingSystem.Id})
												 });
			Task.AddWord("peixe1", "de_word");

			Assert.AreEqual(2, _lexEntryRepository.CountAllItems(), "expected new entry 'peixe1' in lexicon");
			var newEntry = _lexEntryRepository.GetEntriesWithMatchingLexicalForm("peixe1", WeSayWordsProject.Project.WritingSystems.Get(_vernacularWritingSystemId))[0].RealObject;
			Assert.AreEqual(1, entry.Senses.Count, "expected the sense with the non matching meaning to still be around");
			Assert.That(entry.Senses[0], Is.EqualTo(firstSense));
			Assert.AreEqual(1, newEntry.Senses.Count, "expected the sense with the matching meaning to be added to the existing one");
			Assert.That(newEntry.Senses[0], Is.EqualTo(secondSense));
			Task.Deactivate();
		}

		[Test]
		public void SimulateEditingHeadwordSpelling_MeaningFieldIsEnabled_EntryHasOneSenseMatchingSemDom_SenseIsMovedToNewEntry_OldEntryIsDeleted()
		{
			_config.ShowMeaningField = true;
			var entry = MakeEntryWithMeaning("peixe2");
			var firstSense = entry.Senses[0];

			Task.PrepareToMoveWordToEditArea(new GatherBySemanticDomainTask.WordDisplay
			{
				Vernacular = entry.LexicalForm.GetBestAlternative(new[] { _vernacularWritingSystemId }),
				Meaning = firstSense.Definition.GetBestAlternative(new[] { Task.DefinitionWritingSystem.Id })
			});
			Task.AddWord("peixe1", "de_word");

			Assert.AreEqual(1, _lexEntryRepository.CountAllItems(), "expected new entry 'peixe1' in lexicon");
			var newEntry = _lexEntryRepository.GetEntriesWithMatchingLexicalForm("peixe1", WeSayWordsProject.Project.WritingSystems.Get(_vernacularWritingSystemId))[0].RealObject;
			Assert.AreEqual(1, newEntry.Senses.Count, "expected the sense with the non matching meaning to still be around");
			Assert.That(newEntry.Senses[0], Is.EqualTo(firstSense));
			Task.Deactivate();
		}

		[Test]
		public void CurrentWords_MeaningFieldIsEnabled_EntryHasMultipleSenses_NotFirstSenseThatHasMatchingSemanticDomain_DefinitionOfFirstSenseWithmatchingSemanticDomainIsShown()
		{
			_config.ShowMeaningField = true;
			var entry = MakeEntryWithMeaning("peixe2");
			entry.Senses.Add(GetSenseWithDefAndSemDom(Task.DefinitionWritingSystem.Id, "form", Task.DomainKeys[1]));
			_lexEntryRepository.SaveItem(entry);

			Task.CurrentDomainIndex = 1;
			Assert.That(Task.CurrentWords[0].Meaning.Form, Is.EqualTo("form"));
			Task.Deactivate();
		}

		[Test]
		public void CurrentWords_MeaningFieldIsEnabled_EntryHasMultipleSenses_OneSenseMatchesSemDomOtherHasNoSemDomAtAll_DefinitionOfFirstSenseWithMatchingSemanticDomainIsShown()
		{
			_config.ShowMeaningField = true;
			var entry = MakeEntryWithMeaning("peixe2");
			entry.Senses.Add(new LexSense());   //This sense needs to be before the sense we are looking for. i.e. order is relevant
			entry.Senses.Add(GetSenseWithDefAndSemDom(Task.DefinitionWritingSystem.Id, "form", Task.DomainKeys[1]));
			_lexEntryRepository.SaveItem(entry);

			Task.CurrentDomainIndex = 1;
			Assert.That(Task.CurrentWords[0].Meaning.Form, Is.EqualTo("form"));
			Task.Deactivate();
		}

		[Test]
		public void PrepareToMoveWordToEditArea_MeaningFieldIsEnabled_EntryHasMultipleSenses_OneSenseMatchesSemDomOtherHasNoSemDomAtAll_CorrectSenseIsRemovedForEdit()
		{
			_config.ShowMeaningField = true;
			var entry = MakeEntryWithMeaning("peixe2");
			entry.Senses.Add(new LexSense());   //This sense needs to be before the sense we are looking for. i.e. order is relevant
			var senseWeWantToEdit = GetSenseWithDefAndSemDom(Task.DefinitionWritingSystem.Id, "form", Task.DomainKeys[1]);
			entry.Senses.Add(senseWeWantToEdit);
			_lexEntryRepository.SaveItem(entry);

			Task.PrepareToMoveWordToEditArea(new GatherBySemanticDomainTask.WordDisplay
			{
				Vernacular = entry.LexicalForm.GetBestAlternative(new[] { _vernacularWritingSystemId }),
				Meaning = senseWeWantToEdit.Definition.GetBestAlternative(new[] { Task.DefinitionWritingSystem.Id })
			});
			Assert.That(!entry.Senses.Contains(senseWeWantToEdit), Is.False);
			Task.Deactivate();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="e"></param>
		/// <returns>the count of entries</returns>
		private int AddEntry(LexEntry e)
		{
			_lexEntryRepository.SaveItem(e);
			Task.CurrentDomainIndex = 0;
			return _lexEntryRepository.CountAllItems();
		}

		private LexEntry MakeEntryWithMeaning()
		{
			return MakeEntryWithMeaning("peixe");
		}

		private LexEntry MakeEntryWithMeaning(string headword)
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, headword);
			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
				s.GetOrCreateProperty<OptionRefCollection>(
					LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);
			s.Definition.SetAlternative("en", "the meaning");
			return e;
		}

		[Test]
		public void PrepareToMoveWordToEditArea_WordNotInDatabase_DoNothing()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("v", "peshi");
			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peshi");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Task.Deactivate();
		}

		#region Navigation

		/// <summary>
		/// not clear what the best behavior here would be, this just documents
		/// what I did
		/// </summary>
		[Test]
		public void GotoNextDomainLackingAnswers_AllDomainsFull_GoesToLast()
		{
			FillAllDomainsWithWords();
			Task.CurrentDomainIndex = 0;
			Task.GotoNextDomainLackingAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(Task.DomainKeys.Count-1, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		[Test]
		public void GotoNextDomainLackingAnswers_AllDomainsExceptCurrentOneAreFull_StaysPut()
		{
			FillAllDomainsWithWords();
			Task.CurrentDomainIndex = 3;
			Task.PrepareToMoveWordToEditArea("3");
			Task.GotoNextDomainLackingAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(3, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		[Test]
		public void GotoNextDomainLackingAnswers_HasToWrapToFindEmpty_Wraps()
		{
			Task.CurrentDomainIndex = 0;
			Task.AddWord("first");
			Task.CurrentDomainIndex = 1;
			Task.AddWord("second");
			Task.CurrentDomainIndex = 3;
			Task.AddWord("fourth");
			Task.CurrentDomainIndex = Task.DomainKeys.Count - 1;
			Task.AddWord("last");
			Task.GotoNextDomainLackingAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(2, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		[Test]
		public void GotoNextDomainLackingAnswers_SomeDomainsFull_SkipsToEmptyOne()
		{
			Task.CurrentDomainIndex = 0;
			Task.AddWord("first");
			Task.CurrentDomainIndex = 1;
			Task.AddWord("second");
			Task.CurrentDomainIndex = 3;
			Task.AddWord("fourth");
			Task.CurrentDomainIndex = 0;
			Task.GotoNextDomainLackingAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(2, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		#region GotoLastDomainWithAnswers

		[Test]
		public void GotoLastDomainWithAnswers_NoDomainsHaveAnswers_GoesToFirstDomain()
		{
			Task.GotoLastDomainWithAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(0, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		[Test]
		public void GotoLastDomainWithAnswers_GoesToDomainBeforeFirstDomainWithNoAnswers()
		{
			Task.CurrentDomainIndex = 0;
			Task.AddWord("first");
			Task.CurrentDomainIndex = 1;
			Task.AddWord("second");
			Task.CurrentDomainIndex = 3;
			Task.AddWord("fourth");
			Task.GotoLastDomainWithAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(1, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		[Test]
		public void GotoLastDomainWithAnswers_AllDomainsHaveAnswers()
		{
			FillAllDomainsWithWords();
			Task.GotoLastDomainWithAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(Task.DomainKeys.Count - 1, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		private void FillAllDomainsWithWords()
		{
			for (int i = 0;i < Task.DomainKeys.Count;i++)
			{
				Task.CurrentDomainIndex = i;
				Task.AddWord(i.ToString());
			}
			Task.Deactivate();
		}

		#endregion

		#region GotoNextDomainQuestion

		[Test]
		public void GotoNextDomainQuestion_HasNextQuestion_GoesToNextQuestion()
		{
			Task.CurrentDomainIndex = 1;
			Task.GotoNextDomainQuestion();
			Assert.AreEqual(1, Task.CurrentQuestionIndex);
			Task.Deactivate();
		}

		[Test]
		public void GotoNextDomainQuestion_HasNoMoreQuestions_GoesToNextDomainFirstQuestion()
		{
			Task.CurrentDomainIndex = 0;
			Task.CurrentQuestionIndex = Task.Questions.Count - 1;
			Task.GotoNextDomainQuestion();
			Assert.AreEqual(1, Task.CurrentDomainIndex);
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Task.Deactivate();
		}

 /* I JH think we don't want this behavior anymore
  * [Test]
		public void GotoNextDomainQuestion_HasNoMoreDomainsNoMoreQuestions_DoesNothing()
		{
			Task.CurrentDomainIndex = Task.DomainKeys.Count - 1;
			Task.CurrentQuestionIndex = Task.Questions.Count - 1;
			Task.GotoNextDomainQuestion();
			Assert.AreEqual(Task.DomainKeys.Count - 1, Task.CurrentDomainIndex);
			Assert.AreEqual(Task.Questions.Count - 1, Task.CurrentQuestionIndex);
		}
*/


		[Test]
		public void AddWordWithDef_ExistingWordWithSameDomainButDiffDef_NewSense()
		{
			Task.AddWord("one", "1");
			var entries =Task.AddWord("one", "2");
			Assert.AreEqual(1, entries.Count);
			Assert.AreEqual(2, entries[0].Senses.Count, "Should create new senses");
			Task.Deactivate();
		}


		[Test]
		public void AddWordEmptyDef_ExistingWordDefAlreadyExists_SemDomAdded()
		{
			_config.ShowMeaningField = true;
			var entries1 = Task.AddWord("one", "1");
			Task.CurrentDomainIndex++;
			var entries2 = Task.AddWord("one", string.Empty);
			Assert.AreEqual(entries1[0], entries2[0], "Should not create new word");
			Assert.AreEqual(1, entries1[0].Senses.Count, "Should not create new senses");
			AssertNumberOfDomainsInSense(2, entries1[0].Senses[0]);
			Task.Deactivate();
		}

		[Test]
		public void AddWordWithDef_ExistingWordDefAlreadyExists_SemDomAdded()
		{
			var entries1 =Task.AddWord("one", "1");
			Task.CurrentDomainIndex++;
			var entries2 = Task.AddWord("one", "1");
			Assert.AreEqual(entries1[0], entries2[0], "Should not create new word");
			Assert.AreEqual(1, entries1[0].Senses.Count, "Should not create new senses");
			AssertNumberOfDomainsInSense(2, entries1[0].Senses[0]);
			Task.Deactivate();
		}

		[Test]
		public void AddWordWithDef_MatchesTwoWordsNeitherHasMatchingDef_SenseAddedToFirstOnly()
		{
			var e1  = AddEntryToRecordList("one", "1", null);
			var e2  = AddEntryToRecordList("one", "2", null);
			AssertNumberOfDomainsInSense(0, e2.Senses[0]);
			var entries = Task.AddWord("one", "3");
			Assert.AreEqual(e1, entries[0], "Should choose first match");
			Assert.AreEqual(1, e2.Senses.Count, "Should not touch second match");
			Assert.AreEqual(2, entries[0].Senses.Count, "Should add sense");
			Assert.AreEqual(1, entries.Count, "Should only touch one entry");
			AssertNumberOfDomainsInSense(0, e2.Senses[0]);
			Task.Deactivate();
		}

		private void AssertNumberOfDomainsInSense(int expectedDomains, LexSense sense)
		{
			Assert.AreEqual(expectedDomains, sense.GetOrCreateProperty<OptionRefCollection>(LexSense.WellKnownProperties.SemanticDomainDdp4).Count);
		}


		[Test]
		public void AddWordWithDef_ExistingWordHasDifferentDef_NewSenseCreated()
		{
			var entries1 = Task.AddWord("one", "1");
			Task.CurrentDomainIndex++;
			var entries2 = Task.AddWord("one", "2");
			Assert.AreEqual(entries1[0], entries2[0], "Should not create new word");
			Assert.AreEqual(2, entries1[0].Senses.Count, "Should  create new sense");
			AssertNumberOfDomainsInSense(1, entries1[0].Senses[1]);
			AssertNumberOfDomainsInSense(1, entries1[0].Senses[0]);
			Task.Deactivate();
		}


		/// <summary>
		/// these fixing scenarios are REALLY HARD.  Hard to read people's minds.
		/// </summary>
		[Test, Ignore("We don't know how to pull this off yet")]
		public void FixWordSpelling_HadDefInThisSession_SpellingFixed()
		{
			var originalWordCount =_lexEntryRepository.CountAllItems();
			var entries1 = Task.AddWord("onee", "1");
			Task.PrepareToMoveWordToEditArea("one");//this is what the control currently does when you click on a word in the list so you can edit it
			var entries2 = Task.AddWord("one", "1");

			Assert.AreEqual(1, _lexEntryRepository.CountAllItems()-originalWordCount, "Should only create one new word");
			Assert.AreEqual(1, entries2[0].Senses.Count, "Should have just one sense");
			AssertNumberOfDomainsInSense(1, entries1[0].Senses[0]);
			Assert.AreEqual("1", entries2[0].LexicalForm[_vernacularWritingSystemId], "Should have just one sense");
			Assert.AreEqual("1", entries2[0].Senses[0].Definition["en"], "Should have just one sense");
			Task.Deactivate();
		}

		/// <summary>
		/// these fixing scenarios are REALLY HARD.  Hard to read people's minds.
		/// </summary>
		[Test, Ignore("We don't know how to pull this off yet")]
		public void FixDefinition_HadDefInThisSession_SpellingFixed()
		{
			var originalWordCount = _lexEntryRepository.CountAllItems();
			var entries1 = Task.AddWord("one", "01");
			Task.PrepareToMoveWordToEditArea("one");//this is what the control currently does when you click on a word in the list so you can edit it
			var entries2 = Task.AddWord("one", "1");

			Assert.AreEqual(1, _lexEntryRepository.CountAllItems() - originalWordCount, "Should only create one new word");
			Assert.AreEqual(1, entries2[0].Senses.Count, "Should have just one sense");
			AssertNumberOfDomainsInSense(1, entries1[0].Senses[0]);
			Assert.AreEqual("1", entries2[0].LexicalForm[_vernacularWritingSystemId], "Should have just one sense");
			Assert.AreEqual("1", entries2[0].Senses[0].Definition["en"], "Should have just one sense");
			Task.Deactivate();
		}

		[Test]
		public void HasNextDomainQuestion_HasMoreDomains_True()
		{
			Task.CurrentDomainIndex = 0;
			Assert.IsTrue(Task.HasNextDomainQuestion);
			Task.Deactivate();
		}

		[Test]
		public void HasNextDomainQuestion_HasNoMoreDomainsHasMoreQuestions_True()
		{
			Task.CurrentDomainIndex = Task.DomainKeys.Count - 1;
			Assert.IsTrue(Task.HasNextDomainQuestion);
			Task.Deactivate();
		}

		[Test]
		public void HasNextDomainQuestion_HasNoMoreDomainsHasNoMoreQuestions_False()
		{
			Task.CurrentDomainIndex = Task.DomainKeys.Count - 1;
			Task.CurrentQuestionIndex = Task.Questions.Count - 1;
			Assert.IsFalse(Task.HasNextDomainQuestion);
			Task.Deactivate();
		}

		[Test]
		public void CurrentQuestionIndex_DomainHasNoQuestionsInInput_0()
		{
			Task.CurrentDomainIndex = 3;
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Task.Deactivate();
		}

		[Test]
		public void Questions_DomainHasNoQuestionsInInput_CountIs1()
		{
			Task.CurrentDomainIndex = 3;
			Assert.AreEqual(1, Task.Questions.Count);
			Task.Deactivate();
		}

		[Test]
		public void GotoNextDomainQuestion_NextDomainHasNoQuestions_QuestionIndexIs0()
		{
			Task.CurrentDomainIndex = 3;
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(1, Task.Questions.Count);
			Task.Deactivate();
		}

		#endregion

		#region GotoPreviousDomainQuestion

		[Test]
		public void GotoPreviousDomainQuestion_HasPriorQuestion_GoesToPriorQuestion()
		{
			Task.CurrentQuestionIndex = 1;
			Task.GotoPreviousDomainQuestion();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Task.Deactivate();
		}

		[Test]
		public void GotoPreviousDomainQuestion_IsFirstQuestion_GoesToPriorDomainLastQuestion()
		{
			Task.CurrentDomainIndex = 1;
			Task.CurrentQuestionIndex = 0;
			Task.GotoPreviousDomainQuestion();
			Assert.AreEqual(Task.Questions.Count - 1, Task.CurrentQuestionIndex);
			Assert.AreEqual(0, Task.CurrentDomainIndex);
			Task.Deactivate();
		}

		[Test]
		public void GotoPreviousDomainQuestion_IsFirstQuestionOfFirstDomain_Disabled()
		{
			Task.GotoPreviousDomainQuestion();
			Assert.AreEqual(0, Task.CurrentDomainIndex);
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Task.Deactivate();
		}

		[Test]
		public void HasPreviousDomainQuestion_HasPriorDomains_True()
		{
			Task.CurrentDomainIndex = 1;
			Assert.IsTrue(Task.HasNextDomainQuestion);
			Task.Deactivate();
		}

		[Test]
		public void HasPreviousDomainQuestion_HasNoPriorDomainsHasPriorQuestions_True()
		{
			Task.CurrentDomainIndex = 0;
			Task.CurrentQuestionIndex = 1;
			Assert.IsTrue(Task.HasPreviousDomainQuestion);
			Task.Deactivate();
		}

		[Test]
		public void HasPreviousDomainQuestion_HasNoPriorDomainsHasNoPriorQuestions_False()
		{
			Task.CurrentDomainIndex = 0;
			Assert.IsFalse(Task.HasPreviousDomainQuestion);
			Task.Deactivate();
		}

		#endregion

		[Test]
		public void WordsInDomain_NegativeIndex_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.WordsInDomain(-1));
			Task.Deactivate();
		}

		[Test]
		public void WordsInDomain_IndexBeyondDomainCount_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.WordsInDomain(Task.DomainKeys.Count));
			Task.Deactivate();
		}

		[Test]
		public void WordsInDomain_NoWords_Zero()
		{
			Assert.AreEqual(0, Task.WordsInDomain(0));
			Task.Deactivate();
		}

		[Test]
		public void WordsInDomain_ThreeWords_Three()
		{
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);
			AddEntryToRecordList("raposa", "fox", Task.DomainKeys[0]);
			AddEntryToRecordList("cachorro", "dog", Task.DomainKeys[0]);

			Assert.AreEqual(3, Task.WordsInDomain(0));
			Task.Deactivate();
		}

		#endregion


		[Test]
		public void ParseSemanticDomainFile()
		{
			Assert.AreEqual("1 Universe, creation", Task.DomainKeys[0]);
			Assert.AreEqual("1.1 Sky", Task.DomainKeys[1]);
			Assert.AreEqual("1.1.1 Sun", Task.DomainKeys[2]);
			Assert.AreEqual("1.1.1.1 Moon", Task.DomainKeys[3]);
			Assert.AreEqual("1.1.1.2 Star", Task.DomainKeys[4]);
			Assert.AreEqual("1.1.1.3 Planet", Task.DomainKeys[5]);
			Assert.AreEqual("1.1.2 Air", Task.DomainKeys[6]);
			Assert.AreEqual("1.1.2.1 Blow air", Task.DomainKeys[7]);

			Task.CurrentDomainIndex = 7;
			Assert.AreEqual("(1) What words refer to causing air to move? (blow, fan, exhaust, expel, explode)", Task.Questions[0]);
			Assert.AreEqual("(2) What words refer to letting air blow through something? (air out, ventilate)",
							Task.Questions[1]);
			Assert.AreEqual(
					"(3) What words refer to putting air into something (such as a tire or balloon)? (blow up, inflate, pump up, pneumatic)",
					Task.Questions[2]);
			Assert.AreEqual("(4) What words refer to keeping air out of something? (seal, airtight)",
							Task.Questions[3]);
			Assert.AreEqual("(5) What words refer to how much air is in something? (air pressure, vacuum)",
							Task.Questions[4]);
			Assert.AreEqual("(6) What words refer to using the wind to winnow grain? (winnow)",
							Task.Questions[5]);
			Assert.AreEqual("(7) What tools and machines are used to create or use the wind? (fan, air pump, bellows, ventilator, wind tunnel, propeller, air pipe, airshaft, vent, chimney, exhaust, funnel, windmill, sail, valve)",
							Task.Questions[6]);
			Task.Deactivate();
		}

		[Test]
		public void ParseFrenchSemanticDomainFile_Localized()
		{
			WeSayWordsProject.Project.WritingSystems.Set(WritingSystemDefinition.Parse("fr"));
			WeSayWordsProject.Project.DefaultViewTemplate.GetField(LexSense.WellKnownProperties.SemanticDomainDdp4).
										WritingSystemIds[0] ="fr";
			string frenchSemanticDomainFilePath = Path.GetTempFileName();
			ViewTemplate template = MakeViewTemplate("fr");
			GatherBySemanticDomainTask task = new GatherBySemanticDomainTask( GatherBySemanticDomainConfig.CreateForTests(frenchSemanticDomainFilePath),
																			 _lexEntryRepository,
																			 template,
																			 new TaskMemoryRepository(),
																			 new StringLogger());

			task.Activate();
			Assert.AreEqual("1 L’univers, la création", task.DomainNames[0]);
			Assert.AreEqual(" 1.1 Ciel", task.DomainNames[1]);
			Assert.AreEqual("fr", task.SemanticDomainWritingSystemId);
			task.Deactivate();

		}
	}
}
