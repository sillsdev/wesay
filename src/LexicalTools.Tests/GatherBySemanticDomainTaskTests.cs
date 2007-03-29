using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class GatherBySemanticDomainsTaskTests : TaskBaseTests
	{
		Db4oRecordListManager _recordListManager;
		private string _semanticDomainFilePath;
		private string _dbFilePath;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			WeSayWordsProject.InitializeForTests();
			_semanticDomainFilePath = Path.Combine(WeSayWordsProject.Project.ApplicationTestDirectory, "SemanticDomains.xml");
		}

		[SetUp]
		public void Setup()
		{
			_dbFilePath = Path.GetTempFileName();

			this._recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _dbFilePath);// InMemoryRecordListManager();
			Db4oLexModelHelper.Initialize(_recordListManager.DataSource.Data);

			this._task = new GatherBySemanticDomainTask(_recordListManager,
												"label",
												"description",
												_semanticDomainFilePath,
												"en",
												"br",
												"SemanticDomain");
		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
			File.Delete(_dbFilePath);
		}

		private GatherBySemanticDomainTask Task
		{
			get
			{
				if (!_task.IsActive)
				{
					_task.Activate();
				}
				return ((GatherBySemanticDomainTask)_task);
			}
		}

		private LexEntry AddEntryToRecordList(IRecordList<LexEntry> recordList, string lexicalForm, string gloss)
		{
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", lexicalForm);
			AddSenseToEntry(e, gloss);
			return e;
		}

		private LexSense AddSenseToEntry(LexEntry e, string gloss)
		{
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Gloss.SetAlternative("en", gloss);
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);
			return s;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullRecordListManager_Throws()
		{
			this._task = new GatherBySemanticDomainTask(null,
												"label",
												"description",
												_semanticDomainFilePath,
												"en",
												"br",
												"SemanticDomain");
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullLabel_Throws()
		{
			this._task = new GatherBySemanticDomainTask(_recordListManager,
												null,
												"description",
												_semanticDomainFilePath,
												"en",
												"br",
												"SemanticDomain");
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullDescription_Throws()
		{
			this._task = new GatherBySemanticDomainTask(_recordListManager,
												"label",
												null,
												_semanticDomainFilePath,
												"en",
												"br",
												"SemanticDomain");
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullSemanticDomainFilePath_Throws()
		{
			this._task = new GatherBySemanticDomainTask(_recordListManager,
												"label",
												"description",
												null,
												"en",
												"br",
												"SemanticDomain");
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullSemanticDomainFileWritingSystem_Throws()
		{
			this._task = new GatherBySemanticDomainTask(_recordListManager,
												"label",
												"description",
												_semanticDomainFilePath,
												null,
												"br",
												"SemanticDomain");
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullLexicalFormWritingSystemId_Throws()
		{
			this._task = new GatherBySemanticDomainTask(_recordListManager,
												"label",
												"description",
												_semanticDomainFilePath,
												"en",
												null,
												"SemanticDomain");
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullSemanticDomainFieldName_Throws()
		{
			this._task = new GatherBySemanticDomainTask(_recordListManager,
												"label",
												"description",
												_semanticDomainFilePath,
												"en",
												"br",
												null);
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
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CurrentDomainIndex_SetNegative_Throws()
		{
			Task.CurrentDomainIndex = -1;
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CurrentDomainIndex_SetGreaterThanCountOfDomains_Throws()
		{
			Task.CurrentDomainIndex = Task.Domains.Count;
		}

		[Test]
		public void Domains()
		{
			Assert.AreEqual(8, Task.Domains.Count);
		}

		[Test]
		public void CurrentQuestionIndex_Initial_0()
		{
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CurrentQuestionIndex_SetNegative_Throws()
		{
			Task.CurrentQuestionIndex = -1;
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CurrentQuestionIndex_SetGreaterThanCountOfQuestions_Throws()
		{
			Task.CurrentQuestionIndex = Task.Questions.Count;
		}

		[Test]
		public void CurrentDomain()
		{
			Assert.IsNotNull(Task.CurrentDomain);
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
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Gloss.SetAlternative("en", "fish");
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);

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
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			AddEntryToRecordList(recordList, "peixe", "fish");
			LexEntry e = AddEntryToRecordList(recordList, "raposa", "fox");
			AddSenseToEntry(e, "special");
			AddEntryToRecordList(recordList, "cachorro", "dog");

			Assert.AreEqual(3, Task.CurrentWords.Count);
		}


		#endregion
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddWord_null_Throws()
		{
			Task.AddWord(null);
		}

		[Test]
		public void AddWord_EmptyWord_NotAddedToDatabase()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			int originalCount = recordList.Count;
			Task.AddWord(string.Empty);
			Assert.AreEqual(originalCount, recordList.Count);
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
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			int originalCount = recordList.Count;
			Task.AddWord("vernacular");
			Assert.AreEqual(originalCount+1, recordList.Count);
		}

		[Test]
		public void AddWord_NewWord_AddedToCurrentWords()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			int originalCount = recordList.Count;
			Task.AddWord("vernacular");
			Assert.Contains("vernacular", Task.CurrentWords);
			Assert.AreEqual(originalCount + 1, recordList.Count);
		}

		[Test]
		public void AddWord_WordExistsWithGlossAndSemanticDomain_NotAdded()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			AddEntryToRecordList(recordList, "peixe", "fish");
			AddEntryToRecordList(recordList, "raposa", "fox");
			AddEntryToRecordList(recordList, "cachorro", "dog");

			int originalCount = recordList.Count;
			Task.AddWord("raposa");
			Assert.AreEqual(originalCount, recordList.Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveWord_null_Throws()
		{
			Task.RemoveWord(null);
		}

		[Test]
		public void RemoveWord_HasOnlyLexemeForm_DeletesWord()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = (LexSense)e.Senses.AddNew();
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);
			int originalCount = recordList.Count;

			Task.CurrentDomainIndex = 0;
			Task.RemoveWord("peixe");

			Assert.AreEqual(originalCount - 1, recordList.Count);
		}

		[Test]
		public void RemoveWord_WordExistsWithGlossAndSemanticDomain_Disassociated()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			AddEntryToRecordList(recordList, "peixe", "fish");
			AddEntryToRecordList(recordList, "raposa", "fox");
			AddEntryToRecordList(recordList, "cachorro", "dog");

			Task.RemoveWord("raposa");
			Assert.IsFalse(Task.CurrentWords.Contains("raposa"));
			Assert.AreEqual(3, recordList.Count);
		}

		[Test]
		public void RemoveWord_WordExistsWithTwoGlossesHavingSemanticDomain_DisassociatesBothSenses()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			AddEntryToRecordList(recordList, "peixe", "fish");
			LexEntry e = AddEntryToRecordList(recordList, "raposa", "fox");
			AddSenseToEntry(e, "special");
			AddEntryToRecordList(recordList, "cachorro", "dog");

			Task.RemoveWord("raposa");
			Assert.IsFalse(Task.CurrentWords.Contains("raposa"));
			Assert.AreEqual(2, Task.CurrentWords.Count);
		}

		[Test]
		public void RemoveWord_HasAnotherSense_DisassociatesWordFromDomain()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Gloss.SetAlternative("en", "fish");
			s = (LexSense)e.Senses.AddNew();
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);
			int originalCount = recordList.Count;

			Task.CurrentDomainIndex = 0;
			Task.RemoveWord("peixe");

			Assert.AreEqual(originalCount, recordList.Count);
		}

		[Test]
		public void RemoveWord_HasAnotherSense_RemovesEmptySense()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Gloss.SetAlternative("en", "fish");
			s = (LexSense)e.Senses.AddNew();
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);
			int originalCount = recordList.Count;

			Task.CurrentDomainIndex = 0;
			Task.RemoveWord("peixe");

			Assert.AreEqual(originalCount, recordList.Count);
			Assert.AreEqual(1, e.Senses.Count);
		}

		[Test]
		public void RemoveWord_HasTwoLexicalForms_DisassociatesWordFromDomain()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", "peixe");
			e.LexicalForm.SetAlternative("v", "peshi");
			LexSense s = (LexSense)e.Senses.AddNew();
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);
			int originalCount = recordList.Count;

			Task.CurrentDomainIndex = 0;
			Task.RemoveWord("peixe");

			Assert.AreEqual(originalCount, recordList.Count);
		}

		[Test]
		public void RemoveWord_HasCustomFieldInEntry_DisassociatesWordFromDomain()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", "peixe");
			OptionRef optionRef = e.GetOrCreateProperty<OptionRef>("custom");
			optionRef.Value = "hello";

			LexSense s = (LexSense)e.Senses.AddNew();
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);
			int originalCount = recordList.Count;

			Task.CurrentDomainIndex = 0;
			Task.RemoveWord("peixe");

			Assert.AreEqual(originalCount, recordList.Count);
		}

		[Test]
		public void RemoveWord_HasCustomFieldInSense_DisassociatesWordFromDomain()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = (LexSense)e.Senses.AddNew();
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);

			OptionRef optionRef = s.GetOrCreateProperty<OptionRef>("custom");
			optionRef.Value = "hello";

			int originalCount = recordList.Count;

			Task.CurrentDomainIndex = 0;
			Task.RemoveWord("peixe");

			Assert.AreEqual(originalCount, recordList.Count);
		}

		[Test]
		public void RemoveWord_DoesNotHaveSemanticDomainFieldInSense_DoNothing()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = (LexSense)e.Senses.AddNew();
			OptionRef optionRef = s.GetOrCreateProperty<OptionRef>("custom");
			optionRef.Value = "hello";

			int originalCount = recordList.Count;

			Task.CurrentDomainIndex = 0;
			Task.RemoveWord("peixe");

			Assert.AreEqual(originalCount, recordList.Count);
		}

		[Test]
		public void RemoveWord_HasCustomFieldInExample_DisassociatesWordFromDomain()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("br", "peixe");
			LexSense s = (LexSense)e.Senses.AddNew();
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);

			LexExampleSentence example = (LexExampleSentence) s.ExampleSentences.AddNew();
			OptionRef optionRef = example.GetOrCreateProperty<OptionRef>("custom");
			optionRef.Value = "hello";

			int originalCount = recordList.Count;

			Task.CurrentDomainIndex = 0;
			Task.RemoveWord("peixe");

			Assert.AreEqual(originalCount, recordList.Count);
		}

		[Test]
		public void RemoveWord_WordNotInDatabase_DoNothing()
		{
			IRecordList<LexEntry> recordList = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e = (LexEntry)recordList.AddNew();
			e.LexicalForm.SetAlternative("v", "peshi");
			LexSense s = (LexSense)e.Senses.AddNew();
			OptionRefCollection o = s.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			o.Add(Task.Domains[0]);
			int originalCount = recordList.Count;

			Task.CurrentDomainIndex = 0;
			Task.RemoveWord("peshi");

			Assert.AreEqual(originalCount, recordList.Count);
		}


		#region Navigation
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
			Task.CurrentQuestionIndex = Task.Questions.Count-1;
			Task.GotoNextDomainQuestion();
			Assert.AreEqual(1, Task.CurrentDomainIndex);
			Assert.AreEqual(0, Task.CurrentQuestionIndex);
		}

		[Test]
		public void GotoNextDomainQuestion_HasNoMoreDomainsNoMoreQuestions_DoesNothing()
		{
			Task.CurrentDomainIndex = Task.Domains.Count - 1;
			Task.CurrentQuestionIndex = Task.Questions.Count - 1;
			Task.GotoNextDomainQuestion();
			Assert.AreEqual(Task.Domains.Count - 1, Task.CurrentDomainIndex);
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
			Task.CurrentDomainIndex = Task.Domains.Count - 1;
			Assert.IsTrue(Task.HasNextDomainQuestion);
		}

		[Test]
		public void HasNextDomainQuestion_HasNoMoreDomainsHasNoMoreQuestions_False()
		{
			Task.CurrentDomainIndex = Task.Domains.Count - 1;
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

		#endregion

	}

}