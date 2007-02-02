using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class GatherWordListTaskTests : TaskBaseTests
	{
		IRecordListManager _recordListManager;
		private string _wordListFilePath;
		private string _dbFilePath;
		private string[] _words=new string[] {"one","two","three"};

		[SetUp]
		public void Setup()
		{
			_wordListFilePath = Path.GetTempFileName();
			_dbFilePath = Path.GetTempFileName();
			//Db4oLexModelHelper.InitializeForNonDbTests();
			WeSayWordsProject.InitializeForTests();

			Db4oModelConfiguration.Configure();
			this._recordListManager = new Db4oRecordListManager(_dbFilePath);// InMemoryRecordListManager();
			Db4oLexModelHelper.Initialize(((Db4oRecordListManager)_recordListManager).DataSource.Data);

			File.WriteAllLines(_wordListFilePath, _words);
			this._task = new GatherWordListTask(_recordListManager, "label", "description", _wordListFilePath);
		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
			File.Delete(_wordListFilePath);
			File.Delete(_dbFilePath);
		}

		[Test]
		public void InitiallyWordIsCorrect()
		{
			Assert.AreEqual("one",Task.CurrentWord);
		}

		[Test]
		public void CanNavigateToSecondWord()
		{
			Task.NavigateNext();
			Assert.AreEqual("two", Task.CurrentWord);
		}

		[Test]
		public void CanNavigateBackToFirstWord()
		{
			Task.NavigateNext();
			 Task.NavigatePrevious();
		   Assert.AreEqual("one", Task.CurrentWord);
		}

		[Test]
		public void InitiallyCanNavigateNext()
		{
			Assert.IsTrue(Task.CanNavigateNext);
		}
		[Test]
		public void InitiallyCannotNavigatePrevious()
		{
			Assert.IsFalse(Task.CanNavigatePrevious);
		}
		[Test]
		public void NavigateNextEnabledFalseAtEnd()
		{
			Assert.IsTrue(Task.CanNavigateNext);
			NextToEnd();
			Assert.IsFalse(Task.CanNavigateNext);
	   }

		private void NextToEnd()
		{
			for (int i = 0; i < _words.Length; i++)
			{
				Task.NavigateNext();
			}

		}

		[Test,Ignore("Can't be tested on task, make sure it is correct on view.")]
		public void GoingToNextWordSavesCurrentGloss()
		{
		}

		[Test]
		public void IsTaskCompleteTrueAtEnd()
		{
			Assert.IsFalse(Task.IsTaskComplete);
			NextToEnd();
			Assert.IsTrue(Task.IsTaskComplete);
			Task.NavigatePrevious();
			Assert.IsFalse(Task.IsTaskComplete);
		}
		[Test]
		public void CanBackupFromEnd()
		{
			Assert.IsFalse(Task.IsTaskComplete);
			NextToEnd();
			Task.NavigatePrevious();
			Assert.IsFalse(Task.IsTaskComplete);
			Assert.IsTrue(Task.CanNavigateNext);
		}

		[Test]
		public void NoWorkToDo()
		{
			AddEntryAndSense("one");
			AddEntryAndSense("two");
			AddEntryAndSense("three");
			Assert.IsTrue(Task.IsTaskComplete);
		}

		[Test]
		public void FirstWordAlreadyCollected()
		{
			 //add a word with the first wordlist-word already in a sense
			AddEntryAndSense("one");
		   Task.NavigateFirst();
			Assert.AreEqual("two", Task.CurrentWord);
		}

		[Test]
		public void LastWordAlreadyCollected()
		{
			//add an entry with a sense using the last word in the list as a gloss
			AddEntryAndSense("three");

			Task.NavigateFirst();
			Assert.AreEqual("one", Task.CurrentWord);
			Task.NavigateNext();
			Assert.IsTrue(Task.CanNavigateNext);
			Assert.AreEqual("two", Task.CurrentWord);
			Task.NavigateNext();
			Assert.IsTrue(Task.IsTaskComplete);//we don't get to see "three"
		}

		[Test]
		public void SkipMiddleWordAlreadyCollected()
		{
			AddEntryAndSense("two");
			Task.NavigateFirst();

			Assert.AreEqual("one", Task.CurrentWord);
			Task.NavigateNext();
			Assert.AreEqual("three", Task.CurrentWord);
		}

		[Test]
		public void SkipFirstTwoWordsAlreadyCollected()
		{
			AddEntryAndSense("one");
			AddEntryAndSense("two");
			Task.NavigateFirst();
			Assert.AreEqual("three", Task.CurrentWord);
		}

		[Test]
		public void AddWordNotInDB()
		{
			Task.NavigateFirst();
			Assert.AreEqual(0, _recordListManager.GetListOfType<LexEntry>().Count);
			MultiText word = new MultiText();
			word["en"] = "uno";
			Task.WordCollected(word, false);
			Assert.AreEqual(1, _recordListManager.GetListOfType<LexEntry>().Count);
		}

		[Test]
		public void AddWordAlreadyInDBAddsNewSense()
		{
			LexEntry e = (LexEntry)EntriesList.AddNew();
			e.LexicalForm["en"] = "uno";
			Assert.AreEqual(1, _recordListManager.GetListOfType<LexEntry>().Count);
			MultiText word = new MultiText();
			word["en"] = "uno";
			Assert.AreEqual(0, e.Senses.Count);

			Task.NavigateFirst();
			Task.WordCollected(word, false);
			Assert.AreEqual(1, e.Senses.Count);
			Assert.AreEqual(1, _recordListManager.GetListOfType<LexEntry>().Count);
	   }

		private void AddEntryAndSense(string gloss)
		{
			LexEntry e = (LexEntry)EntriesList.AddNew();
			((LexSense) e.Senses.AddNew()).Gloss["en"] = gloss;
		}

		private IRecordList<LexEntry> EntriesList
		{
			get
			{
				IRecordList<LexEntry> list = _recordListManager.GetListOfType<LexEntry>();
				return list;
			}
		}

		private GatherWordListTask Task
		{
			get
			{
				if (!_task.IsActive)
				{
					_task.Activate();
				}
				return ((GatherWordListTask) _task);
			}
		}
	}

}