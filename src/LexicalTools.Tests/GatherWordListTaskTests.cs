using System;
using System.Collections.Generic;
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
		private string _filePath;
		private string[] _words=new string[] {"one","two","three"};

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			WeSayWordsProject.InitializeForTests();

			this._recordListManager = new InMemoryRecordListManager();

			_filePath = Path.GetTempFileName();
			File.WriteAllLines(_filePath, _words);
			this._task = new GatherWordListTask(_recordListManager, "label", "description", _filePath);
		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
			File.Delete(_filePath);
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
			Assert.IsTrue(Task.NavigateNextEnabled);
		}
		[Test]
		public void InitiallyCannotNavigatePrevious()
		{
			Assert.IsFalse(Task.NavigatePreviousEnabled);
		}
		[Test]
		public void NavigateNextEnabledFalseAtEnd()
		{
			Assert.IsTrue(Task.NavigateNextEnabled);
			NextToEnd();
			Assert.IsFalse(Task.NavigateNextEnabled);
	   }

		private void NextToEnd()
		{
			for (int i = 0; i < _words.Length; i++)
			{
				Task.NavigateNext();
			}

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
			Assert.IsTrue(Task.NavigateNextEnabled);
		}

		[Test,Ignore]
		public void NoWorkAtStart()
		{
		}

		[Test, Ignore]
		public void FirstWordAlreadyCollected()
		{
		}

		[Test, Ignore]
		public void LastWordAlreadyCollected()
		{
		}

		[Test, Ignore]
		public void SkipMiddleWordAlreadyCollected()
		{
		}

		[Test, ExpectedException(typeof(NotImplementedException))]
		public void AddWordTestInMemoryNotPossibleYet()
		{
			MultiText word=new MultiText();
			word["en"] = "uno";
			Task.WordCollected(word, false);
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