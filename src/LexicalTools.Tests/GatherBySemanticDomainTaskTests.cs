using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Reporting;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.LexicalTools.GatherBySemanticDomains;
using WeSay.Project;
using Palaso.Lift.Options;

using NUnit.Framework;

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
		private static string _vernacularWritingSystemId = "br";

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			WeSayProjectTestHelper.InitializeForTests();
		}

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.Project.RemoveCache();
			_tempFolder = new TemporaryFolder();
			_filePath = _tempFolder.GetTemporaryFile();
			_semanticDomainFilePath = _tempFolder.GetTemporaryFile();
			CreateSemanticDomainFile();

			_lexEntryRepository = new LexEntryRepository(_filePath);
			_viewTemplate = MakeViewTemplate("en");
			var config = GatherBySemanticDomainConfig.CreateForTests(_semanticDomainFilePath);
			_task = new GatherBySemanticDomainTask(config,
												   _lexEntryRepository,
												   _viewTemplate,
												   new TaskMemoryRepository(),
												   new StringLogger());
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
			semanticDomainField.OptionsListFile = "Ddp4.xml";
			semanticDomainField.DataTypeName = "OptionRefCollection";

			ViewTemplate v = new ViewTemplate();
			Field lexicalFormField = new Field(Field.FieldNames.EntryLexicalForm.ToString(),
											   "LexEntry",
											   new string[] {_vernacularWritingSystemId});
			lexicalFormField.DataTypeName = "MultiText";

			v.Add(lexicalFormField);
			v.Add(semanticDomainField);

			v.Add(new Field(LexSense.WellKnownProperties.Definition,"LexSense", new string[]{"en"}));

			if(!v.WritingSystems.Contains("en"))
			{
				v.WritingSystems.Set(WritingSystemDefinition.FromLanguage("en"));
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
			_tempFolder.Dispose();
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
		public void ConstructWithTemplate_NonExistantSemanticDomainFilePath_Throws()
		{
			Assert.Throws<ApplicationException>(() =>
				new GatherBySemanticDomainTask(GatherBySemanticDomainConfig.CreateForTests(Path.GetRandomFileName()),
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
		}

		[Test]
		public void SemanticDomainWritingSystem()
		{
			Assert.AreEqual("en", Task.SemanticDomainWritingSystemId);
		}

		#region Current

		[Test]
		public void CurrentDomainIndex_Initial_0()
		{
			Assert.AreEqual(0, Task.CurrentDomainIndex);
		}

		[Test]
		public void CurrentDomainIndex_Leave_ReturnToPreviousPosition()
		{
			Task.CurrentDomainIndex = 2;
			Task.Deactivate();
			//task gets automatically activated in Task accessor
			Assert.AreEqual(2, Task.CurrentDomainIndex);
		}

		[Test]
		public void CurrentDomainIndex_Set()
		{
			Task.CurrentDomainIndex = 2;
			Assert.AreEqual(2, Task.CurrentDomainIndex);
		}

		[Test]
		public void CurrentDomainIndex_SetNegative_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.CurrentDomainIndex = -1);
		}

		[Test]
		public void CurrentDomainIndex_SetGreaterThanCountOfDomains_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.CurrentDomainIndex = Task.DomainKeys.Count);
		}

		[Test]
		public void Domains()
		{
			Assert.AreEqual(8, Task.DomainKeys.Count);
		}

		[Test]
		public void CurrentQuestionIndex_Initial_0()
		{
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
		}

		[Test]
		public void CurrentQuestionIndex_SetNegative_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.CurrentQuestionIndex = -1);
		}

		[Test]
		public void CurrentQuestionIndex_SetGreaterThanCountOfQuestions_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.CurrentQuestionIndex = Task.Questions.Count);
		}

		[Test]
		public void CurrentDomain()
		{
			Assert.IsNotNull(Task.CurrentDomainKey);
		}

		[Test]
		public void CurrentQuestion()
		{
			Assert.IsNotNull(Task.CurrentQuestion);
		}

		[Test]
		public void CurrentQuestionIndex_Leave_ReturnToPreviousPosition()
		{
			Task.CurrentQuestionIndex = 2;
			Task.Deactivate();
			//task gets automatically activated in Task accessor
			Assert.AreEqual(2, Task.CurrentQuestionIndex);
		}

		[Test]
		public void CurrentQuestionIndex_SetCurrentDomainIndex_0()
		{
			Task.CurrentQuestionIndex = 2;
			Task.CurrentDomainIndex = 2;
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
		}

		[Test]
		public void Questions_DifferentDomain_DifferentQuestions()
		{
			List<string> questions = Task.Questions;
			Task.CurrentDomainIndex = 2;
			Assert.AreNotEqual(questions, Task.Questions);
		}

		#endregion

		#region WordList

		[Test]
		public void CurrentWords()
		{
			Assert.IsNotNull(Task.CurrentWords);
		}

		[Test]
		public void CurrentWords_NoWords_Empty()
		{
			Assert.IsEmpty(Task.CurrentWords);
		}

		[Test]
		public void CurrentWords_HasWords_NotEmpty()
		{
			Task.AddWord("peixe");
			Assert.IsNotEmpty(Task.CurrentWords);
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
			List<string> words = Task.CurrentWords;
			Assert.Contains("peixe", words);
		}

		[Test]
		public void CurrentWords_ChangeQuestion_SameWordList()
		{
			List<string> wordList = Task.CurrentWords;
			Task.CurrentQuestionIndex = 2;
			Assert.AreSame(wordList, Task.CurrentWords);
		}

		[Test]
		public void CurrentWords_ChangeDomain_NewWordList()
		{
			List<string> wordList = Task.CurrentWords;
			Task.CurrentDomainIndex = 2;
			Assert.AreNotSame(wordList, Task.CurrentWords);
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
		}

		#endregion

		[Test]
		public void AddWord_null_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => Task.AddWord(null));
		}


		[Test]
		public void AddWord_NewWord_AddedToDatabase()
		{
			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord("vernacular", String.Empty);
			Assert.AreEqual(originalCount + 1, _lexEntryRepository.CountAllItems());
		}

		/// <summary>
		/// regression test of WS-15019
		/// </summary>
		[Test]
		public void AddWord_WordConsistsOfOnlySegmentSeparatorCharacter_AddedToDatabase()
		{
			int originalCount = _lexEntryRepository.CountAllItems();Task.AddWord('\u001F'.ToString(), String.Empty);
			Assert.AreEqual(originalCount + 1, _lexEntryRepository.CountAllItems());
		}

	   [Test]
		public void RemainingCount_Initially_RemainingCountEqualsReferenceCount()
		{
			Assert.AreEqual(Task.GetReferenceCount(), Task.GetRemainingCount());
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
		}

		[Test]
		public void RemainingCount_RemoveWordCausingEmptyDomain_RemainingCountIncreases()
		{
			Task.CurrentDomainIndex = 3;
			Task.AddWord("boo");
			int originalCount = Task.GetRemainingCount();
			Task.PrepareToMoveWordToEditArea("boo");
			Assert.AreEqual(originalCount + 1, Task.GetRemainingCount());
		}

		[Test]
		public void AddWord_NewWord_AddedToCurrentWords()
		{
			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord("vernacular", String.Empty);
			Assert.Contains("vernacular", Task.CurrentWords);
			Assert.AreEqual(originalCount + 1, _lexEntryRepository.CountAllItems());
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
		}

		[Test]
		public void PrepareToMoveWordToEditArea_null_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => Task.PrepareToMoveWordToEditArea(null));
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
		}

		[Test]
		public void PrepareToMoveWordToEditArea_WordExistsWithDefAndSemanticDomain_Disassociated()
		{
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);
			AddEntryToRecordList("raposa", "fox", Task.DomainKeys[0]);
			AddEntryToRecordList("cachorro", "dog", Task.DomainKeys[0]);

			Task.PrepareToMoveWordToEditArea("raposa");
			Assert.IsFalse(Task.CurrentWords.Contains("raposa"));
			Assert.AreEqual(3, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void SetMeaningMultiTextForWordRecentlyMovedToEditArea_WordHad1Meaning_SetProperly()
		{
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);

			Task.PrepareToMoveWordToEditArea("peixe");
			var meaning = Task.GetMeaningForWordRecentlyMovedToEditArea();

			Assert.AreEqual("fish", meaning["en"]);
		}


		[Test]
		public void SetMeaningMultiTextForWordRecentlyMovedToEditArea_WordHad0Meaning_SetProperly()
		{
			AddEntryToRecordList("peixe", null, Task.DomainKeys[0]);

			Task.PrepareToMoveWordToEditArea("peixe");
			var meaning = Task.GetMeaningForWordRecentlyMovedToEditArea();

			Assert.IsFalse(meaning.ContainsAlternative("en"));
		}

		[Test]
		public void PrepareToMoveWordToEditArea_WordExistsWithTwoDefsHavingSemanticDomain_DisassociatesBothSenses()
		{
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);
			LexEntry e = AddEntryToRecordList("raposa", "fox", Task.DomainKeys[0]);
			AddSenseToEntry(e, "special", Task.DomainKeys[0]);
			AddEntryToRecordList("cachorro", "dog", Task.DomainKeys[0]);

			Task.PrepareToMoveWordToEditArea("raposa");
			Assert.IsFalse(Task.CurrentWords.Contains("raposa"));
			Assert.AreEqual(2, Task.CurrentWords.Count);
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
		}

		[Test]
		public void PrepareToMoveWordToEditArea_HasCustomFieldInExample_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative(_vernacularWritingSystemId, "peixe");
			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			o.Add(Task.DomainKeys[0]);

			LexExampleSentence example = new LexExampleSentence();
			s.ExampleSentences.Add(example);
			OptionRef optionRef = example.GetOrCreateProperty<OptionRef>("custom");
			optionRef.Value = "hello";
			_lexEntryRepository.SaveItem(e);

			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.PrepareToMoveWordToEditArea("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
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
		}

		#region GotoLastDomainWithAnswers

		[Test]
		public void GotoLastDomainWithAnswers_NoDomainsHaveAnswers_GoesToFirstDomain()
		{
			Task.GotoLastDomainWithAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(0, Task.CurrentDomainIndex);
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
		}

		[Test]
		public void GotoLastDomainWithAnswers_AllDomainsHaveAnswers()
		{
			FillAllDomainsWithWords();
			Task.GotoLastDomainWithAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(Task.DomainKeys.Count - 1, Task.CurrentDomainIndex);
		}

		private void FillAllDomainsWithWords()
		{
			for (int i = 0;i < Task.DomainKeys.Count;i++)
			{
				Task.CurrentDomainIndex = i;
				Task.AddWord(i.ToString());
			}
		}

		#endregion

		#region GotoNextDomainQuestion

		[Test]
		public void GotoNextDomainQuestion_HasNextQuestion_GoesToNextQuestion()
		{
			Task.CurrentDomainIndex = 0;
			Task.GotoNextDomainQuestion();
			Assert.AreEqual(1, Task.CurrentQuestionIndex);
		}

		[Test]
		public void GotoNextDomainQuestion_HasNoMoreQuestions_GoesToNextDomainFirstQuestion()
		{
			Task.CurrentDomainIndex = 0;
			Task.CurrentQuestionIndex = Task.Questions.Count - 1;
			Task.GotoNextDomainQuestion();
			Assert.AreEqual(1, Task.CurrentDomainIndex);
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
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
		}


		[Test]
		public void AddWordEmptyDef_ExistingWordDefAlreadyExists_SemDomAdded()
		{
			var entries1 = Task.AddWord("one", "1");
			Task.CurrentDomainIndex++;
			var entries2 = Task.AddWord("one", string.Empty);
			Assert.AreEqual(entries1[0], entries2[0], "Should not create new word");
			Assert.AreEqual(1, entries1[0].Senses.Count, "Should not create new senses");
			AssertNumberOfDomainsInSense(2, entries1[0].Senses[0]);
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
		}

		private void AssertNumberOfDomainsInSense(int expectedDomains, LexSense sense)
		{
			Assert.AreEqual(expectedDomains, sense.GetOrCreateProperty<OptionRefCollection>(LexSense.WellKnownProperties.SemanticDomainDdp4).Count);
		}


		[Test]
		public void AddWordWithDef_ExistingWordHasDifferentDef_NewSenseCreated()
		{
		   var entries1 =Task.AddWord("one", "1");
			Task.CurrentDomainIndex++;
			var entries2 = Task.AddWord("one", "2");
			Assert.AreEqual(entries1[0], entries2[0], "Should not create new word");
			Assert.AreEqual(2, entries1[0].Senses.Count, "Should  create new sense");
			AssertNumberOfDomainsInSense(1, entries1[0].Senses[1]);
			AssertNumberOfDomainsInSense(1, entries1[0].Senses[0]);
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
		}

		[Test]
		public void HasNextDomainQuestion_HasMoreDomains_True()
		{
			Task.CurrentDomainIndex = 0;
			Assert.IsTrue(Task.HasNextDomainQuestion);
		}

		[Test]
		public void HasNextDomainQuestion_HasNoMoreDomainsHasMoreQuestions_True()
		{
			Task.CurrentDomainIndex = Task.DomainKeys.Count - 1;
			Assert.IsTrue(Task.HasNextDomainQuestion);
		}

		[Test]
		public void HasNextDomainQuestion_HasNoMoreDomainsHasNoMoreQuestions_False()
		{
			Task.CurrentDomainIndex = Task.DomainKeys.Count - 1;
			Task.CurrentQuestionIndex = Task.Questions.Count - 1;
			Assert.IsFalse(Task.HasNextDomainQuestion);
		}

		[Test]
		public void CurrentQuestionIndex_DomainHasNoQuestionsInInput_0()
		{
			Task.CurrentDomainIndex = 3;
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
		}

		[Test]
		public void Questions_DomainHasNoQuestionsInInput_CountIs1()
		{
			Task.CurrentDomainIndex = 3;
			Assert.AreEqual(1, Task.Questions.Count);
		}

		[Test]
		public void GotoNextDomainQuestion_NextDomainHasNoQuestions_QuestionIndexIs0()
		{
			Task.CurrentDomainIndex = 3;
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(1, Task.Questions.Count);
		}

		#endregion

		#region GotoPreviousDomainQuestion

		[Test]
		public void GotoPreviousDomainQuestion_HasPriorQuestion_GoesToPriorQuestion()
		{
			Task.CurrentQuestionIndex = 1;
			Task.GotoPreviousDomainQuestion();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
		}

		[Test]
		public void GotoPreviousDomainQuestion_IsFirstQuestion_GoesToPriorDomainLastQuestion()
		{
			Task.CurrentDomainIndex = 1;
			Task.CurrentQuestionIndex = 0;
			Task.GotoPreviousDomainQuestion();
			Assert.AreEqual(Task.Questions.Count - 1, Task.CurrentQuestionIndex);
			Assert.AreEqual(0, Task.CurrentDomainIndex);
		}

		[Test]
		public void GotoPreviousDomainQuestion_IsFirstQuestionOfFirstDomain_Disabled()
		{
			Task.GotoPreviousDomainQuestion();
			Assert.AreEqual(0, Task.CurrentDomainIndex);
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
		}

		[Test]
		public void HasPreviousDomainQuestion_HasPriorDomains_True()
		{
			Task.CurrentDomainIndex = 1;
			Assert.IsTrue(Task.HasNextDomainQuestion);
		}

		[Test]
		public void HasPreviousDomainQuestion_HasNoPriorDomainsHasPriorQuestions_True()
		{
			Task.CurrentDomainIndex = 0;
			Task.CurrentQuestionIndex = 1;
			Assert.IsTrue(Task.HasPreviousDomainQuestion);
		}

		[Test]
		public void HasPreviousDomainQuestion_HasNoPriorDomainsHasNoPriorQuestions_False()
		{
			Task.CurrentDomainIndex = 0;
			Assert.IsFalse(Task.HasPreviousDomainQuestion);
		}

		#endregion

		[Test]
		public void WordsInDomain_NegativeIndex_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.WordsInDomain(-1));
		}

		[Test]
		public void WordsInDomain_IndexBeyondDomainCount_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Task.WordsInDomain(Task.DomainKeys.Count));
		}

		[Test]
		public void WordsInDomain_NoWords_Zero()
		{
			Assert.AreEqual(0, Task.WordsInDomain(0));
		}

		[Test]
		public void WordsInDomain_ThreeWords_Three()
		{
			AddEntryToRecordList("peixe", "fish", Task.DomainKeys[0]);
			AddEntryToRecordList("raposa", "fox", Task.DomainKeys[0]);
			AddEntryToRecordList("cachorro", "dog", Task.DomainKeys[0]);

			Assert.AreEqual(3, Task.WordsInDomain(0));
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
			Assert.AreEqual("(1) What words refer to causing air to move?", Task.Questions[0]);
			Assert.AreEqual("(2) What words refer to letting air blow through something?",
							Task.Questions[1]);
			Assert.AreEqual(
					"(3) What words refer to putting air into something (such as a tire or balloon)?",
					Task.Questions[2]);
			Assert.AreEqual("(4) What words refer to keeping air out of something?",
							Task.Questions[3]);
			Assert.AreEqual("(5) What words refer to how much air is in something?",
							Task.Questions[4]);
			Assert.AreEqual("(6) What words refer to using the wind to winnow grain?",
							Task.Questions[5]);
			Assert.AreEqual("(7) What tools and machines are used to create or use the wind?",
							Task.Questions[6]);
		}

		[Test]
		public void ParseEmptySemanticDomainFile()
		{
			string emptySemanticDomainFilePath = Path.GetTempFileName();
			using (StreamWriter streamWriter = File.CreateText(emptySemanticDomainFilePath))
			{
				streamWriter.Write("");
			}

			GatherBySemanticDomainTask task = new GatherBySemanticDomainTask(  GatherBySemanticDomainConfig.CreateForTests(emptySemanticDomainFilePath),
																			 _lexEntryRepository,
																			 _viewTemplate,
																			 new TaskMemoryRepository(),
																			 new StringLogger());
			task.Activate();
			Assert.AreEqual(1, task.DomainKeys.Count);
			Assert.AreEqual(string.Empty, task.CurrentDomainKey);
			Assert.AreEqual(1, task.Questions.Count);
			Assert.AreEqual(string.Empty, task.CurrentQuestion);

			File.Delete(emptySemanticDomainFilePath);
		}

		[Test]
		public void ParseFrenchSemanticDomainFile_Localized()
		{
			string frenchSemanticDomainFilePath = Path.GetTempFileName();
			using (StreamWriter streamWriter = File.CreateText(frenchSemanticDomainFilePath))
			{
				streamWriter.Write(
						@"<?xml version='1.0' encoding='utf-8'?>
<semantic-domain-questions semantic-domain-type='DDP4' lang='fr'>
<semantic-domain guid='I63403699-07C1-43F3-A47C-069D6E4316E5' id='1 Universe, creation'>
<question>Quels sont les mots qui font r�f�rence � tout ce qu'on peut voir?</question>
</semantic-domain>
<semantic-domain guid='I999581C4-1611-4ACB-AE1B-5E6C1DFE6F0C' id='1.1 Sky'>
<question>Quels sont les mots qui signifient le ciel?</question>
<question>Quels sont les mots qui signifient l'endroit ou le pays au-del� du ciel?</question>
<question>Quels sont les mots qui d�crivent l'aspect du ciel?</question>
<question>Quels sont les mots qui d�crivent l'endroit o� le ciel touche la terre?</question>
<question>Quel terme g�n�ral fait r�f�rence aux objets dans le ciel?</question>
</semantic-domain></semantic-domain-type>");
			}

			ViewTemplate template = MakeViewTemplate("fr");
			GatherBySemanticDomainTask task = new GatherBySemanticDomainTask( GatherBySemanticDomainConfig.CreateForTests(frenchSemanticDomainFilePath),
																			 _lexEntryRepository,
																			 template,
																			 new TaskMemoryRepository(),
																			 new StringLogger());
			task.Activate();
			Assert.AreEqual("1 L'univers physique", task.DomainNames[0]);
			Assert.AreEqual(" 1.1 Ciel", task.DomainNames[1]);
			Assert.AreEqual("fr", task.SemanticDomainWritingSystemId);

			File.Delete(frenchSemanticDomainFilePath);
		}

		private void CreateSemanticDomainFile()
		{
			using (StreamWriter streamWriter = File.CreateText(_semanticDomainFilePath))
			{
				streamWriter.Write(
						@"<?xml version='1.0' encoding='utf-8'?>
<semantic-domain-questions semantic-domain-type='DDP4' lang='en'>
<semantic-domain guid='I63403699-07C1-43F3-A47C-069D6E4316E5' id='1 Universe, creation'>
<question>(1) What words refer to the whole of everything we can see?</question>
<question>(2) What words refer to everything there is?</question>
<question>(3) What words refer to everything that we know to exist?</question>
</semantic-domain>
<semantic-domain guid='I999581C4-1611-4ACB-AE1B-5E6C1DFE6F0C' id='1.1 Sky'>
<question>(1) What words are used to refer to the sky?</question>
<question>(2) What words refer to the air around the earth?</question>
<question>(3) What words are used to refer to the place or area beyond the sky?</question>
<question>(4) What words describe something in the sky or something that happens in the sky?</question>
<question>(5) What words describe the appearance of the sky?</question>
<question>(6) What words refer to the edge of the sky where it meets the ground?</question>
<question>(7) What words refer to something in the sky?</question>
<question>(8) What words refer to the lights that appear in the northern (or southern) sky?</question>
<question>(9) What words refer to something being in the sky?</question>
</semantic-domain>
<semantic-domain guid='IDC1A2C6F-1B32-4631-8823-36DACC8CB7BB' id='1.1.1 Sun'>
<question>(1) What words refer to the sun?</question>
<question>(2) What words refer to how the sun moves?</question>
<question>(3) What words refer to the time when the sun rises?</question>
<question>(4) What words refer to the time when the sun is at its highest point?</question>
<question>(5) What words refer to the time when the sun sets?</question>
<question>(6) What words refer to when the sun is shining?</question>
<question>(7) What words refer to the sun shining through the clouds?</question>
<question>(8) What words describe where the sun is shining?</question>
<question>(9) What words describe when or where the sun doesn't shine?</question>
<question>(10) What words refer to the light of the sun?</question>
<question>(11) What words describe the brightness of the sun?</question>
<question>(12) What refer to the sun heating things?</question>
<question>(13) What else does the sun do?</question>
<question>(14) What words describe the damage done by sunlight?</question>
<question>(15) What do people use to protect themselves from the sun?</question>
<question>(16) What words are used of telling time by the sun?</question>
<question>(17) What words refer to using the power of the sun?</question>
</semantic-domain>
<semantic-domain guid='I1BD42665-0610-4442-8D8D-7C666FEE3A6D' id='1.1.1.1 Moon'>
</semantic-domain>
<semantic-domain guid='IB044E890-CE30-455C-AEDE-7E9D5569396E' id='1.1.1.2 Star'>
<question>(1) What words are used to refer to the stars?</question>
<question>(2) What words describe the sky when the stars are shining?</question>
<question>(3) What words are used for where the stars are shining?</question>
<question>(4) What words are used for when or where the stars don't shine?</question>
<question>(5) What words refer to the light of the stars?</question>
<question>(6) What words describe the brightness of the stars?</question>
<question>(7) What words describe the appearance of the stars?</question>
<question>(8) What words refer to the study of the stars?</question>
<question>(9) What is a group of stars called?</question>
</semantic-domain>
<semantic-domain guid='IA0D073DF-D413-4DFD-9BA1-C3C68F126D90' id='1.1.1.3 Planet'>
<question>(1) What words refer to a planet?</question>
<question>(2) What are the names of the planets?</question>
<question>(3) What words refer to how the planets move?</question>
<question>(4) What words refer to the sun and planets?</question>
<question>(5) What words refer to comets?</question>
<question>(6) What do comets do?</question>
<question>(7) What words refer to meteors?</question>
<question>(8) What do meteors do?</question>
<question>(9) What words are used for when a meteor hits the earth?</question>
<question>(10) What is a small planet called?</question>
</semantic-domain>
<semantic-domain guid='IE836B01B-6C1A-4D41-B90A-EA5F349F88D4' id='1.1.2 Air'>
<question>(1) What words refer to the air we breathe?</question>
<question>(2) What words refer to how much water is in the air?</question>
<question>(3) What words describe good air (such as when the air in a room is clean or doesn't smell bad)?</question>
<question>(4) What words describe bad air (such as when the air in a room is dirty, hot, smells bad, or too many people are breathing)?</question>
</semantic-domain>
<semantic-domain guid='I18595DF7-1C69-40DB-A7C1-74D490115C0C' id='1.1.2.1 Blow air'>
<question>(1) What words refer to causing air to move?</question>
<question>(2) What words refer to letting air blow through something?</question>
<question>(3) What words refer to putting air into something (such as a tire or balloon)?</question>
<question>(4) What words refer to keeping air out of something?</question>
<question>(5) What words refer to how much air is in something?</question>
<question>(6) What words refer to using the wind to winnow grain?</question>
<question>(7) What tools and machines are used to create or use the wind?</question>
</semantic-domain></semantic-domain-questions>");
			}
		}
	}
}