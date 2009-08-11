using System;
using System.IO;
using NUnit.Framework;
using Palaso.Data;
using Palaso.Reporting;
using WeSay.Data;
using WeSay.Foundation;
using Palaso.TestUtilities;
using WeSay.LexicalModel;
using WeSay.LexicalTools.GatherByWordList;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class GatherWordListTaskTests: TaskBaseTests
	{
		private TemporaryFolder _tempFolder;
		private LexEntryRepository _lexEntryRepository;
		private string _wordListFilePath;
		private string _filePath;
		private readonly string[] _words = new string[] {"one", "two", "three"};
		private ViewTemplate _viewTemplate;
		private string _glossingLanguageWSId;
		private string _vernacularLanguageWSId;
		private WordListCatalog _catalog;

		[SetUp]
		public void Setup()
		{
			_tempFolder = new TemporaryFolder();
			_wordListFilePath = _tempFolder.GetTemporaryFile();
			_filePath = _tempFolder.GetTemporaryFile();
			WeSayWordsProject.InitializeForTests();

			_lexEntryRepository = new LexEntryRepository(_filePath); // InMemoryRecordListManager();
			_glossingLanguageWSId = BasilProject.Project.WritingSystems.TestWritingSystemAnalId;
			_vernacularLanguageWSId = BasilProject.Project.WritingSystems.TestWritingSystemVernId;

			File.WriteAllLines(_wordListFilePath, _words);
			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(),
										"LexEntry",
										new string[]
											{
													BasilProject.Project.WritingSystems.
															TestWritingSystemVernId
											}));

			_catalog = new WordListCatalog();
			_catalog.Add(_wordListFilePath, new WordListDescription("en","label","longLabel", "description"));
			_task = new GatherWordListTask( GatherWordListConfig.CreateForTests( _wordListFilePath,_glossingLanguageWSId, _catalog),
											_lexEntryRepository,
										   _viewTemplate, new TaskMemoryRepository());
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			_tempFolder.Delete();
		}

		[Test]
		public void Ctor_EmptyTemplate_DoesntCrash()
		{
			GatherWordListTask g = new GatherWordListTask(
					GatherWordListConfig.CreateForTests(_wordListFilePath,
							WritingSystem.IdForUnknownAnalysis, _catalog),
					_lexEntryRepository,
					new ViewTemplate(), new TaskMemoryRepository());

			Assert.IsNotNull(g);
		}

		[Test]
		[ExpectedException(typeof (ErrorReport.ProblemNotificationSentToUserException))]
		public void Activate_MissingWordListFile_GivesMessage()
		{
			GatherWordListTask g = new GatherWordListTask(
				   GatherWordListConfig.CreateForTests("NotThere.txt",
						   WritingSystem.IdForUnknownAnalysis, new WordListCatalog()),
				   _lexEntryRepository,
				   new ViewTemplate(), new TaskMemoryRepository());

			 g.Activate(); //should give a box to user, an exception in this text environment
		}


		[Test]
		[ExpectedException(typeof (ErrorReport.ProblemNotificationSentToUserException))]
		public void Activate_WritingSystemNotInCurrentList_GivesMessage()
		{
			GatherWordListTask g = new GatherWordListTask(
				GatherWordListConfig.CreateForTests(_wordListFilePath,
					"z7z", new WordListCatalog()),
				_lexEntryRepository,
				_viewTemplate, new TaskMemoryRepository());

			g.Activate(); //should give a box to user, an exception in this text environment
		}

		[Test]
		public void InitiallyWordIsCorrect()
		{
			Assert.AreEqual("one", Task.CurrentWordFromWordlist);
		}

		[Test]
		public void CanNavigateToSecondWord()
		{
			Task.NavigateNext();
			Assert.AreEqual("two", Task.CurrentWordFromWordlist);
		}

		[Test]
		public void CanNavigateBackToFirstWord()
		{
			Task.NavigateNext();
			Task.NavigatePrevious();
			Assert.AreEqual("one", Task.CurrentWordFromWordlist);
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
			for (int i = 0;i < _words.Length;i++)
			{
				Task.NavigateNext();
			}
		}

		[Test]
		[Ignore("Can't be tested on task, make sure it is correct on view.")]
		public void GoingToNextWordSavesCurrentGloss() {}

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
			Assert.IsFalse(Task.IsTaskComplete);
			AddEntryAndSense("one");
			AddEntryAndSense("two");
			AddEntryAndSense("three");
			Task.NavigateFirstToShow();
			Assert.IsTrue(Task.IsTaskComplete);
		}

		[Test]
		public void FirstWordAlreadyCollected()
		{
			//add a word with the first wordlist-word already in a sense
			AddEntryAndSense("one");
			Task.NavigateFirstToShow();
			Assert.AreEqual("two", Task.CurrentWordFromWordlist);
		}

		[Test]
		public void LastWordAlreadyCollected()
		{
			//add an entry with a sense using the last word in the list as a gloss
			AddEntryAndSense("three");

			Task.NavigateFirstToShow();
			Assert.AreEqual("one", Task.CurrentWordFromWordlist);
			Task.NavigateNext();
			Assert.IsTrue(Task.CanNavigateNext);
			Assert.AreEqual("two", Task.CurrentWordFromWordlist);
			Task.NavigateNext();
			Assert.IsTrue(Task.IsTaskComplete); //we don't get to see "three"
		}

		[Test]
		public void SkipMiddleWordAlreadyCollected()
		{
			AddEntryAndSense("two");
			Task.NavigateFirstToShow();

			Assert.AreEqual("one", Task.CurrentWordFromWordlist);
			Task.NavigateNext();
			Assert.AreEqual("three", Task.CurrentWordFromWordlist);
		}

		[Test]
		public void SkipFirstTwoWordsAlreadyCollected()
		{
			AddEntryAndSense("one");
			AddEntryAndSense("two");
			Task.NavigateFirstToShow();
			Assert.AreEqual("three", Task.CurrentWordFromWordlist);
		}

		[Test]
		public void AddWordNotInDB()
		{
			Task.NavigateFirstToShow();
			Assert.AreEqual(0, _lexEntryRepository.CountAllItems());
			MultiText word = new MultiText();
			word[VernWs.Id] = "uno";
			Task.WordCollected(word);
			Assert.AreEqual(1, _lexEntryRepository.CountAllItems());

			//this is an attempt to get a failure that I was able to get at one time in the
			//app itself, but which I haven't got to fail under tests.  I believe I've
			//fixed the bug, but alas this never really demonstrated it.
			Assert.AreEqual(1, Task.GetRecordsWithMatchingGloss().Count);
		}

		[Test]
		public void AddWord_LexEntryAlreadyExists_WordAppearsInCompletedBox()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm[VernWs.Id] = "uno";
			Assert.AreEqual(1, _lexEntryRepository.CountAllItems());
			MultiText word = new MultiText();
			word[VernWs.Id] = "uno";
			Assert.AreEqual(0, e.Senses.Count);
			_lexEntryRepository.SaveItem(e);

			Task.NavigateFirstToShow();
			Task.WordCollected(word);
			Task.NavigateNext();
			Task.WordCollected(word);

			ResultSet<LexEntry> matchingLexicalForms = Task.GetRecordsWithMatchingGloss();
			Assert.AreEqual(1, matchingLexicalForms.Count);
		}

		[Test]
		public void AddWordAlreadyInDBAddsNewSense()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm[VernWs.Id] = "uno";
			Assert.AreEqual(1, _lexEntryRepository.CountAllItems());
			MultiText word = new MultiText();
			word[VernWs.Id] = "uno";
			Assert.AreEqual(0, e.Senses.Count);
			_lexEntryRepository.SaveItem(e);

			Task.NavigateFirstToShow();
			Task.WordCollected(word);
			Assert.AreEqual(1, e.Senses.Count);
			Assert.AreEqual(1, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void AddWordAlreadyInDBAddsAdditionalSense()
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			e.LexicalForm[VernWs.Id] = "uno";
			Assert.AreEqual(1, _lexEntryRepository.CountAllItems());
			MultiText word = new MultiText();
			word[VernWs.Id] = "uno";
			Assert.AreEqual(0, e.Senses.Count);
			_lexEntryRepository.SaveItem(e);

			Task.NavigateFirstToShow();
			Task.WordCollected(word);
			Task.NavigateNext();
			Task.WordCollected(word);

			Assert.AreEqual(2, e.Senses.Count);
			Assert.AreEqual(1, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void AddWordASecondTime_DoesNothing()
		{
			RecordToken<LexEntry> token = PrepareEntryWithOneGloss();
			RecordToken<LexEntry> token2 = PrepareEntryWithOneGloss();
			LexEntry entry = token.RealObject;
			LexEntry entry2 = token2.RealObject;
			Assert.AreSame(entry, entry2);

			Assert.AreEqual(1, entry.Senses.Count);
			Assert.AreEqual(1, _lexEntryRepository.CountAllItems());
		}

		private void AddEntryAndSense(string gloss)
		{
			LexEntry e = _lexEntryRepository.CreateItem();
			LexSense sense = new LexSense();
			e.Senses.Add(sense);
			sense.Gloss[_glossingLanguageWSId] = gloss;
			_lexEntryRepository.SaveItem(e);
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
			RecordToken<LexEntry> token = PrepareEntryWithOneGloss();

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(token);
			Assert.AreEqual(0,
							_lexEntryRepository.GetEntriesWithMatchingLexicalForm("uno", VernWs).
									Count);
		}

		/// <summary>
		/// test support for spell fixing
		/// </summary>
		[Test]
		public void RemovingGlossFromEntryWithOtherSenses_OnlyRemovesGloss()
		{
			RecordToken<LexEntry> token = PrepareEntryWithOneGloss();
			//now tweak the entry
			LexEntry entry = token.RealObject;
			LexSense leaveAloneSense = new LexSense();
			entry.Senses.Add(leaveAloneSense);
			leaveAloneSense.Gloss.SetAlternative(_glossingLanguageWSId, "single");
			Assert.AreEqual(2, entry.Senses.Count);

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(token);
			Assert.AreEqual(1,
							_lexEntryRepository.GetEntriesWithMatchingLexicalForm("uno", VernWs).
									Count);
			Assert.AreEqual(1, entry.Senses.Count);
		}

		/// <summary>
		/// test support for spell fixing
		/// </summary>
		[Test]
		public void RemovingAssociationWith_OnlyRemovesGloss()
		{
			RecordToken<LexEntry> token = PrepareEntryWithOneGloss();
			//now tweak the entry
			LexEntry entry = token.RealObject;
			LexSense leaveAloneSense = new LexSense();
			entry.Senses.Add(leaveAloneSense);
			leaveAloneSense.Gloss.SetAlternative(_glossingLanguageWSId, "single");
			Assert.AreEqual(2, entry.Senses.Count);

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(token);
			Assert.AreEqual(1,
							_lexEntryRepository.GetEntriesWithMatchingLexicalForm("uno", VernWs).
									Count);
			Assert.AreEqual(1, entry.Senses.Count);
		}

		/// <summary>
		/// test support for spell fixing (ideally, this would move the sense, but this what we do for now)
		/// </summary>
		[Test]
		public void RemovingAssociationWhereSenseHasExample_DoesNothing()
		{
			RecordToken<LexEntry> token = PrepareEntryWithOneGloss();
			//now tweak the entry
			LexEntry entry = token.RealObject;
			LexSense sense = entry.Senses[0];
			LexExampleSentence ex = new LexExampleSentence();
			sense.ExampleSentences.Add(ex);
			ex.Sentence.SetAlternative(VernWs.Id, "blah blah");

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(token);
			Assert.AreEqual(1,
							_lexEntryRepository.GetEntriesWithMatchingLexicalForm("uno", VernWs).
									Count);
			Assert.AreEqual(1, entry.Senses.Count);
			Assert.AreEqual("one",
							sense.Gloss.GetExactAlternative(_glossingLanguageWSId),
							"should not remove the gloss");
		}

		private RecordToken<LexEntry> PrepareEntryWithOneGloss()
		{
			Task.NavigateAbsoluteFirst();
			MultiText word = new MultiText();

			word.SetAlternative(_vernacularLanguageWSId, "uno");
			Task.WordCollected(word);
			Assert.AreEqual(1,
							_lexEntryRepository.GetEntriesWithMatchingLexicalForm("uno", VernWs).
									Count);

			ResultSet<LexEntry> entries =
					_lexEntryRepository.GetEntriesWithMatchingLexicalForm("uno", VernWs);

			return entries[0];
		}

		private WritingSystem VernWs
		{
			get
			{
				WritingSystem vernWs;
				BasilProject.Project.WritingSystems.TryGetValue(_vernacularLanguageWSId, out vernWs);
				return vernWs;
			}
		}
	}
}