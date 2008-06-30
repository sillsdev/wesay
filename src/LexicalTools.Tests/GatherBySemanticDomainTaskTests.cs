using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class GatherBySemanticDomainsTaskTests: TaskBaseTests
	{
		private LexEntryRepository _lexEntryRepository;
		private string _semanticDomainFilePath;
		private string _dbFilePath;
		private ViewTemplate _viewTemplate;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			WeSayWordsProject.InitializeForTests();
		}

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.Project.RemoveCache();
			_dbFilePath = Path.GetTempFileName();
			_semanticDomainFilePath = Path.GetTempFileName();
			CreateSemanticDomainFile();

			_lexEntryRepository = new LexEntryRepository(_dbFilePath);
			_viewTemplate = MakeViewTemplate("en");
			_task =
					new GatherBySemanticDomainTask(_lexEntryRepository,
												   "label",
												   "description",
												   _semanticDomainFilePath,
												   _viewTemplate,
												   LexSense.WellKnownProperties.SemanticDomainsDdp4);
		}

		private static LexSense AddNewSenseToEntry(LexEntry e)
		{
			LexSense s = new LexSense();
			e.Senses.Add(s);
			return s;
		}

		private static ViewTemplate MakeViewTemplate(string nameAndQuestionWritingSystem)
		{
			Field semanticDomainField =
					new Field(LexSense.WellKnownProperties.SemanticDomainsDdp4,
							  "LexSense",
							  new string[] {nameAndQuestionWritingSystem});
			semanticDomainField.OptionsListFile = "Ddp4.xml";
			semanticDomainField.DataTypeName = "OptionRefCollection";

			ViewTemplate v = new ViewTemplate();
			Field lexicalFormField =
					new Field(Field.FieldNames.EntryLexicalForm.ToString(),
							  "LexEntry",
							  new string[] {"br"});
			lexicalFormField.DataTypeName = "MultiText";

			v.Add(lexicalFormField);
			v.Add(semanticDomainField);
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
			File.Delete(_dbFilePath);
			File.Delete(_semanticDomainFilePath);
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

		private LexEntry AddEntryToRecordList(string lexicalForm, string gloss)
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("br", lexicalForm);
			AddSenseToEntry(e, gloss);
			_lexEntryRepository.SaveItem(e);
			return e;
		}

		private void AddSenseToEntry(LexEntry e, string gloss)
		{
			LexSense s = AddNewSenseToEntry(e);
			s.Gloss.SetAlternative("en", gloss);
			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
		}

		[Test]
		public void ConstructWithTemplate()
		{
			Assert.IsNotNull(
					new GatherBySemanticDomainTask(_lexEntryRepository,
												   "label",
												   "description",
												   _semanticDomainFilePath,
												   _viewTemplate,
												   LexSense.WellKnownProperties.SemanticDomainsDdp4));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithTemplate_NullRecordListManager_Throws()
		{
			new GatherBySemanticDomainTask(null,
										   "label",
										   "description",
										   _semanticDomainFilePath,
										   _viewTemplate,
										   LexSense.WellKnownProperties.SemanticDomainsDdp4);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithTemplate_NullLabel_Throws()
		{
			new GatherBySemanticDomainTask(_lexEntryRepository,
										   null,
										   "description",
										   _semanticDomainFilePath,
										   _viewTemplate,
										   LexSense.WellKnownProperties.SemanticDomainsDdp4);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithTemplate_NullDescription_Throws()
		{
			new GatherBySemanticDomainTask(_lexEntryRepository,
										   "label",
										   null,
										   _semanticDomainFilePath,
										   _viewTemplate,
										   LexSense.WellKnownProperties.SemanticDomainsDdp4);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithTemplate_NullSemanticDomainFilePath_Throws()
		{
			new GatherBySemanticDomainTask(_lexEntryRepository,
										   "label",
										   "description",
										   null,
										   _viewTemplate,
										   LexSense.WellKnownProperties.SemanticDomainsDdp4);
		}

		[Test]
		[ExpectedException(typeof (ApplicationException))]
		public void ConstructWithTemplate_NonExistantSemanticDomainFilePath_Throws()
		{
			new GatherBySemanticDomainTask(_lexEntryRepository,
										   "label",
										   "description",
										   Path.GetRandomFileName(),
										   _viewTemplate,
										   LexSense.WellKnownProperties.SemanticDomainsDdp4);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithTemplate_NullTemplate_Throws()
		{
			new GatherBySemanticDomainTask(_lexEntryRepository,
										   "label",
										   "description",
										   _semanticDomainFilePath,
										   null,
										   LexSense.WellKnownProperties.SemanticDomainsDdp4);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructWithTemplate_NullFieldName_Throws()
		{
			new GatherBySemanticDomainTask(_lexEntryRepository,
										   "label",
										   "description",
										   _semanticDomainFilePath,
										   _viewTemplate,
										   null);
		}

		[Test]
		public void WordWritingSystem()
		{
			Assert.AreEqual("br", Task.WordWritingSystemId);
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
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void CurrentDomainIndex_SetNegative_Throws()
		{
			Task.CurrentDomainIndex = -1;
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void CurrentDomainIndex_SetGreaterThanCountOfDomains_Throws()
		{
			Task.CurrentDomainIndex = Task.DomainKeys.Count;
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
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void CurrentQuestionIndex_SetNegative_Throws()
		{
			Task.CurrentQuestionIndex = -1;
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void CurrentQuestionIndex_SetGreaterThanCountOfQuestions_Throws()
		{
			Task.CurrentQuestionIndex = Task.Questions.Count;
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
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = AddNewSenseToEntry(e);
			s.Gloss.SetAlternative("en", "fish");
			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
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
		public void CurrentWords_WordExistsWithTwoGlossesHavingSemanticDomain_ShowsUpOnce()
		{
			Task.CurrentDomainIndex = 1;
			AddEntryToRecordList("peixe", "fish");
			LexEntry e = AddEntryToRecordList("raposa", "fox");
			AddSenseToEntry(e, "special");
			AddEntryToRecordList("cachorro", "dog");
			Task.CurrentDomainIndex = 0;
			Assert.AreEqual(3, Task.CurrentWords.Count);
		}

		#endregion

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void AddWord_null_Throws()
		{
			Task.AddWord(null);
		}

		[Test]
		public void AddWord_EmptyWord_NotAddedToDatabase()
		{
			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord(string.Empty);
			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void AddWord_EmptyWord_NotInCurrentWords()
		{
			Task.AddWord(string.Empty);
			Assert.IsEmpty(Task.CurrentWords);
		}

		[Test]
		public void AddWord_NewWord_AddedToDatabase()
		{
			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord("vernacular");
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
			Task.AddWord("vernacular");
			Assert.AreEqual(originalCount - 1, Task.GetRemainingCount());
		}

		[Test]
		public void RemainingCount_RemoveWordCausingEmptyDomain_RemainingCountIncreases()
		{
			Task.CurrentDomainIndex = 3;
			Task.AddWord("boo");
			int originalCount = Task.GetRemainingCount();
			Task.DetachFromMatchingEntries("boo");
			Assert.AreEqual(originalCount + 1, Task.GetRemainingCount());
		}

		[Test]
		public void AddWord_NewWord_AddedToCurrentWords()
		{
			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord("vernacular");
			Assert.Contains("vernacular", Task.CurrentWords);
			Assert.AreEqual(originalCount + 1, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void AddWord_WordExistsWithGlossAndSemanticDomain_NotAdded()
		{
			AddEntryToRecordList("peixe", "fish");
			AddEntryToRecordList("raposa", "fox");
			AddEntryToRecordList("cachorro", "dog");

			int originalCount = _lexEntryRepository.CountAllItems();
			Task.AddWord("raposa");
			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void RemoveWord_null_Throws()
		{
			Task.DetachFromMatchingEntries(null);
		}

		[Test]
		public void RemoveWord_HasOnlyLexemeForm_DeletesWord()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = new LexSense();
			e.Senses.Add(s);
			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.DetachFromMatchingEntries("peixe");

			Assert.AreEqual(originalCount - 1, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void RemoveWord_WordExistsWithGlossAndSemanticDomain_Disassociated()
		{
			AddEntryToRecordList("peixe", "fish");
			AddEntryToRecordList("raposa", "fox");
			AddEntryToRecordList("cachorro", "dog");

			Task.DetachFromMatchingEntries("raposa");
			Assert.IsFalse(Task.CurrentWords.Contains("raposa"));
			Assert.AreEqual(3, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void RemoveWord_WordExistsWithTwoGlossesHavingSemanticDomain_DisassociatesBothSenses()
		{
			AddEntryToRecordList("peixe", "fish");
			LexEntry e = AddEntryToRecordList("raposa", "fox");
			AddSenseToEntry(e, "special");
			AddEntryToRecordList("cachorro", "dog");

			Task.DetachFromMatchingEntries("raposa");
			Assert.IsFalse(Task.CurrentWords.Contains("raposa"));
			Assert.AreEqual(2, Task.CurrentWords.Count);
		}

		[Test]
		public void RemoveWord_HasAnotherSense_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = AddNewSenseToEntry(e);
			s.Gloss.SetAlternative("en", "fish");
			s = new LexSense();
			e.Senses.Add(s);
			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.DetachFromMatchingEntries("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void RemoveWord_HasAnotherSense_RemovesEmptySense()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = AddNewSenseToEntry(e);
			s.Gloss.SetAlternative("en", "fish");
			s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.DetachFromMatchingEntries("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
			Assert.AreEqual(1, e.Senses.Count);
		}

		[Test]
		public void RemoveWord_HasTwoLexicalForms_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("br", "peixe");
			e.LexicalForm.SetAlternative("v", "peshi");
			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.DetachFromMatchingEntries("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void RemoveWord_HasCustomFieldInEntry_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("br", "peixe");
			MultiText mt = e.GetOrCreateProperty<MultiText>("custom");
			mt["en"] = "hello";

			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.DetachFromMatchingEntries("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void RemoveWord_HasCustomFieldInSense_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(Task.DomainKeys[0]);

			MultiText mt = e.GetOrCreateProperty<MultiText>("custom");
			mt["en"] = "hello";
			_lexEntryRepository.SaveItem(e);

			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.DetachFromMatchingEntries("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void RemoveWord_DoesNotHaveSemanticDomainFieldInSense_DoNothing()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("br", "peixe");
			AddNewSenseToEntry(e);

			MultiText mt = e.GetOrCreateProperty<MultiText>("custom");
			mt["en"] = "hello";
			_lexEntryRepository.SaveItem(e);

			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.DetachFromMatchingEntries("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void RemoveWord_HasCustomFieldInExample_DisassociatesWordFromDomain()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(Task.DomainKeys[0]);

			LexExampleSentence example = new LexExampleSentence();
			s.ExampleSentences.Add(example);
			OptionRef optionRef = example.GetOrCreateProperty<OptionRef>("custom");
			optionRef.Value = "hello";
			_lexEntryRepository.SaveItem(e);

			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.DetachFromMatchingEntries("peixe");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void RemoveWord_WordNotInDatabase_DoNothing()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm.SetAlternative("v", "peshi");
			LexSense s = AddNewSenseToEntry(e);

			OptionRefCollection o =
					s.GetOrCreateProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(Task.DomainKeys[0]);
			_lexEntryRepository.SaveItem(e);
			int originalCount = _lexEntryRepository.CountAllItems();

			Task.CurrentDomainIndex = 0;
			Task.DetachFromMatchingEntries("peshi");

			Assert.AreEqual(originalCount, _lexEntryRepository.CountAllItems());
		}

		#region Navigation

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
			for (int i = 0;i < Task.DomainKeys.Count;i++)
			{
				Task.CurrentDomainIndex = i;
				Task.AddWord(i.ToString());
			}
			Task.GotoLastDomainWithAnswers();
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
			Assert.AreEqual(Task.DomainKeys.Count - 1, Task.CurrentDomainIndex);
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

		[Test]
		public void GotoNextDomainQuestion_HasNoMoreDomainsNoMoreQuestions_DoesNothing()
		{
			Task.CurrentDomainIndex = Task.DomainKeys.Count - 1;
			Task.CurrentQuestionIndex = Task.Questions.Count - 1;
			Task.GotoNextDomainQuestion();
			Assert.AreEqual(Task.DomainKeys.Count - 1, Task.CurrentDomainIndex);
			Assert.AreEqual(Task.Questions.Count - 1, Task.CurrentQuestionIndex);
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
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void WordsInDomain_NegativeIndex_Throws()
		{
			Task.WordsInDomain(-1);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void WordsInDomain_IndexBeyondDomainCount_Throws()
		{
			Task.WordsInDomain(Task.DomainKeys.Count);
		}

		[Test]
		public void WordsInDomain_NoWords_Zero()
		{
			Assert.AreEqual(0, Task.WordsInDomain(0));
		}

		[Test]
		public void WordsInDomain_ThreeWords_Three()
		{
			AddEntryToRecordList("peixe", "fish");
			AddEntryToRecordList("raposa", "fox");
			AddEntryToRecordList("cachorro", "dog");

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

			GatherBySemanticDomainTask task =
					new GatherBySemanticDomainTask(_lexEntryRepository,
												   "label",
												   "description",
												   emptySemanticDomainFilePath,
												   _viewTemplate,
												   LexSense.WellKnownProperties.SemanticDomainsDdp4);
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
<question>Quels sont les mots qui font référence à tout ce qu'on peut voir?</question>
</semantic-domain>
<semantic-domain guid='I999581C4-1611-4ACB-AE1B-5E6C1DFE6F0C' id='1.1 Sky'>
<question>Quels sont les mots qui signifient le ciel?</question>
<question>Quels sont les mots qui signifient l'endroit ou le pays au-delà du ciel?</question>
<question>Quels sont les mots qui décrivent l'aspect du ciel?</question>
<question>Quels sont les mots qui décrivent l'endroit où le ciel touche la terre?</question>
<question>Quel terme général fait référence aux objets dans le ciel?</question>
</semantic-domain></semantic-domain-type>");
			}

			ViewTemplate template = MakeViewTemplate("fr");
			GatherBySemanticDomainTask task =
					new GatherBySemanticDomainTask(_lexEntryRepository,
												   "label",
												   "description",
												   frenchSemanticDomainFilePath,
												   template,
												   LexSense.WellKnownProperties.SemanticDomainsDdp4);
			task.Activate();
			Assert.AreEqual("L'univers physique", task.DomainNames[0]);
			Assert.AreEqual("Ciel", task.DomainNames[1]);
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