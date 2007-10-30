using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Palaso.Reporting;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class GatherWordListTaskTests : TaskBaseTests
	{
		Db4oRecordListManager _recordListManager;
		private string _wordListFilePath;
		private string _dbFilePath;
		private string[] _words=new string[] {"one","two","three"};
		ViewTemplate _viewTemplate;
		private string _glossingLanguageWSId ;
		private string _vernacularLanguageWSId ;

		[SetUp]
		public void Setup()
		{
			_wordListFilePath = Path.GetTempFileName();
			_dbFilePath = Path.GetTempFileName();
			//Db4oLexModelHelper.InitializeForNonDbTests();
			WeSayWordsProject.InitializeForTests();

			this._recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _dbFilePath);// InMemoryRecordListManager();
			Db4oLexModelHelper.Initialize(((Db4oRecordListManager)_recordListManager).DataSource.Data);
			Lexicon.Init(_recordListManager);
			_glossingLanguageWSId = BasilProject.Project.WritingSystems.TestWritingSystemAnalId;
			_vernacularLanguageWSId = BasilProject.Project.WritingSystems.TestWritingSystemVernId;

			File.WriteAllLines(_wordListFilePath, _words);
			_viewTemplate = new ViewTemplate();
			this._viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry", new string[] { BasilProject.Project.WritingSystems.TestWritingSystemVernId }));

			this._task = new GatherWordListTask(_recordListManager,
												"label",
												"description",
												_wordListFilePath,
												_glossingLanguageWSId,
												_viewTemplate);
		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
			File.Delete(_wordListFilePath);
			File.Delete(_dbFilePath);
		}

		[Test]
		public void EmptyTemplate()
		{
			GatherWordListTask g = new GatherWordListTask(_recordListManager,
														  "label",
														  "description",
														  _wordListFilePath,
														  WritingSystem.IdForUnknownAnalysis,
														  new ViewTemplate());

			Assert.IsNotNull(g);
		}

		[Test, ExpectedException(typeof(ErrorReport.NonFatalMessageSentToUserException))]
		public void MissingWordListFileGivesMessage()
		{
			GatherWordListTask g = new GatherWordListTask(_recordListManager,
														  "label",
														  "description",
														  "NotThere.txt",
														  WritingSystem.IdForUnknownAnalysis,
														  new ViewTemplate());
			g.Activate();//should give a box to user, an exception in this text environment
		}

		[Test, ExpectedException(typeof(ErrorReport.NonFatalMessageSentToUserException))]
		public void WritingSystemNotInCurrentListGivesMessage()
		{
			GatherWordListTask g = new GatherWordListTask(_recordListManager,
														  "label",
														  "description",
														  _wordListFilePath,
														  "z7z",
														  new ViewTemplate());
			g.Activate();//should give a box to user, an exception in this text environment
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
		   Task.NavigateFirstToShow();
			Assert.AreEqual("two", Task.CurrentWord);
		}

		[Test]
		public void LastWordAlreadyCollected()
		{
			//add an entry with a sense using the last word in the list as a gloss
			AddEntryAndSense("three");

			Task.NavigateFirstToShow();
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
			Task.NavigateFirstToShow();

			Assert.AreEqual("one", Task.CurrentWord);
			Task.NavigateNext();
			Assert.AreEqual("three", Task.CurrentWord);
		}

		[Test]
		public void SkipFirstTwoWordsAlreadyCollected()
		{
			AddEntryAndSense("one");
			AddEntryAndSense("two");
			Task.NavigateFirstToShow();
			Assert.AreEqual("three", Task.CurrentWord);
		}

		[Test]
		public void AddWordNotInDB()
		{
			Task.NavigateFirstToShow();
			Assert.AreEqual(0, _recordListManager.GetListOfType<LexEntry>().Count);
			MultiText word = new MultiText();
			word[VernWs.Id] = "uno";
			Task.WordCollected(word);
			Assert.AreEqual(1, _recordListManager.GetListOfType<LexEntry>().Count);

		   //this is an attempt to get a failure that I was able to get at one time in the
			//app itself, but which I haven't got to fail under tests.  I believe I've
			//fixed the bug, but alas this never really demonstrated it.
			Assert.AreEqual(1, Task.GetMatchingRecords(Task.CurrentWordAsMultiText).Count);
		}

		[Test]
		public void AddWordAlreadyInDBAddsNewSense()
		{
			LexEntry e = (LexEntry)EntriesList.AddNew();
			e.LexicalForm[VernWs.Id] = "uno";
			Assert.AreEqual(1, _recordListManager.GetListOfType<LexEntry>().Count);
			MultiText word = new MultiText();
			word[VernWs.Id] = "uno";
			Assert.AreEqual(0, e.Senses.Count);

			Task.NavigateFirstToShow();
			Task.WordCollected(word);
			Assert.AreEqual(1, e.Senses.Count);
			Assert.AreEqual(1, _recordListManager.GetListOfType<LexEntry>().Count);
	   }


	   [Test]
	   public void AddWordASecondTime_DoesNothing()
	   {
		   LexEntry entry =PrepareEntryWithOneGloss();
		   LexEntry entry2 =PrepareEntryWithOneGloss();
		   Assert.AreSame(entry, entry2);
		   Assert.AreEqual(1, entry.Senses.Count);
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

		/// <summary>
		/// test support for spell fixing
		/// </summary>
		[Test]
		public void RemovingGlossFromEmptyEntry_RemovesEntry()
		{
			LexEntry entry = PrepareEntryWithOneGloss();

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(entry);
			Assert.AreEqual(0, Lexicon.GetEntriesHavingLexicalForm("uno", VernWs).Count);
		}


		/// <summary>
		/// test support for spell fixing
		/// </summary>
		[Test]
		public void RemovingGlossFromEntryWithOtherSenses_OnlyRemovesGloss()
		{
			LexEntry entry = PrepareEntryWithOneGloss();
			//now tweak the entry
			LexSense leaveAloneSense = (LexSense) entry.Senses.AddNew();
			leaveAloneSense.Gloss.SetAlternative(_glossingLanguageWSId, "single");
			Assert.AreEqual(2,entry.Senses.Count);

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(entry);
			Assert.AreEqual(1, Lexicon.GetEntriesHavingLexicalForm("uno", VernWs).Count);
			Assert.AreEqual(1,entry.Senses.Count);
		}


		/// <summary>
		/// test support for spell fixing
		/// </summary>
		[Test]
		public void RemovingAssociationWith_OnlyRemovesGloss()
		{
			LexEntry entry = PrepareEntryWithOneGloss();
			//now tweak the entry
			LexSense leaveAloneSense = (LexSense)entry.Senses.AddNew();
			leaveAloneSense.Gloss.SetAlternative(_glossingLanguageWSId, "single");
			Assert.AreEqual(2, entry.Senses.Count);

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(entry);
			Assert.AreEqual(1, Lexicon.GetEntriesHavingLexicalForm("uno", VernWs).Count);
			Assert.AreEqual(1, entry.Senses.Count);
		}


		/// <summary>
		/// test support for spell fixing (ideally, this would move the sense, but this what we do for now)
		/// </summary>
		[Test]
		public void RemovingAssociationWhereSenseHasExample_DoesNothing()
		{
			LexEntry entry = PrepareEntryWithOneGloss();
			//now tweak the entry
			LexSense sense = (LexSense) entry.Senses[0];
			LexExampleSentence ex= (LexExampleSentence) sense.ExampleSentences.AddNew();
			ex.Sentence.SetAlternative(VernWs.Id, "blah blah");

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(entry);
			Assert.AreEqual(1, Lexicon.GetEntriesHavingLexicalForm("uno", VernWs).Count);
			Assert.AreEqual(1, entry.Senses.Count);
			Assert.AreEqual("one", sense.Gloss.GetExactAlternative(_glossingLanguageWSId), "should not remove the gloss");
		}

		private LexEntry PrepareEntryWithOneGloss()
		{
			Task.NavigateAbsoluteFirst();
			MultiText word = new MultiText();

			word.SetAlternative(_vernacularLanguageWSId,"uno");
			Task.WordCollected(word);
			Assert.AreEqual(1, Lexicon.GetEntriesHavingLexicalForm("uno", VernWs).Count);

			IList<LexEntry> entries = Lexicon.GetEntriesHavingLexicalForm("uno", VernWs);

			return (LexEntry)entries[0];
		}

		private WritingSystem VernWs
		{
			get
			{
				WritingSystem vernWs;
				BasilProject.Project.WritingSystems.TryGetValue(_vernacularLanguageWSId,out vernWs);
				return vernWs;
			}
		}
	}

}