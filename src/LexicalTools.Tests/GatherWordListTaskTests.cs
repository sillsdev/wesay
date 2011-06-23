using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Palaso.Data;
using Palaso.Lift;
using Palaso.Lift.Options;
using Palaso.Reporting;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.LexicalTools.GatherByWordList;
using WeSay.Project;
using Palaso.DictionaryServices.Model;
using WeSay.TestUtilities;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class GatherWordListTaskTests: TaskBaseTests
	{
		private TemporaryFolder _tempFolder;
		private LexEntryRepository _lexEntryRepository;
		private string _simpleWordListFilePath;
		private string _filePath;
		private readonly string[] _words = new string[] {"one", "two", "three"};
		private ViewTemplate _viewTemplate;
		private string _glossingLanguageWSId;
		private string _vernacularLanguageWSId;
		private WordListCatalog _catalog;
		private TempLiftFile _liftWordListFile;

		[SetUp]
		public void Setup()
		{
			WeSayProjectTestHelper.InitializeForTests();
			_glossingLanguageWSId = WritingSystemsIdsForTests.AnalysisIdForTest;
			_vernacularLanguageWSId = WritingSystemsIdsForTests.VernacularIdForTest;
			BasilProject.Project.WritingSystems.Set(WritingSystemDefinition.FromLanguage("fr"));

			_tempFolder = new TemporaryFolder();
			_simpleWordListFilePath = _tempFolder.GetTemporaryFile();
//            _liftWordListFile = new TempLiftFile("wordlist.lift",_tempFolder, LiftXml, LiftIO.Validation.Validator.LiftVersion);
			_filePath = _tempFolder.GetTemporaryFile();

			_lexEntryRepository = new LexEntryRepository(_filePath); // InMemoryRecordListManager();
			File.WriteAllLines(_simpleWordListFilePath, _words);
			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(),
										"LexEntry",
										new string[]
											{
													WritingSystemsIdsForTests.VernacularIdForTest
											}));

			_viewTemplate.Add(
				new Field(
					LexSense.WellKnownProperties.Definition.ToString(),
					"LexSense",
					new string[]
						{
							WritingSystemsIdsForTests.AnalysisIdForTest,
							"fr"
						}
				 )
			 );

			_catalog = new WordListCatalog();
			_catalog.Add(_simpleWordListFilePath, new WordListDescription("en","label","longLabel", "description"));
		   // _catalog.Add(_liftWordListFile.Path, new WordListDescription("en", "liftWordList", "liftWordListLong", "liftWordListDescription"));
			_task = new GatherWordListTask( GatherWordListConfig.CreateForTests( _simpleWordListFilePath,_glossingLanguageWSId, _catalog),
											_lexEntryRepository,
										   _viewTemplate, new TaskMemoryRepository());
		}

		protected string LiftXml
		{
			get
			{
				return @"
				<entry id='one'>
					<lexical-unit>
					  <form lang='glossWS'>
						<text>apple</text>
					  </form>
					  <form lang='fr'>
						<text>pom</text>
					  </form>
					</lexical-unit>
					<sense>
						<gloss lang='glossWS'>
							<text>apple</text>
					</gloss>
							<grammatical-info value='noun' />
							<trait name='semantic-domain-ddp4' value='fruit'/>
							<field type='custom1'><form lang='en'><text>EnglishCustomValue</text></form></field>
				  </sense>
				</entry>

				<entry id='two'>
					<lexical-unit>
					  <form lang='glossWS'>
						<text>cloud</text>
					  </form>
					</lexical-unit>
				</entry>
				<entry id='man'>
					<lexical-unit>
					  <form lang='fr'>
						<text>gar�on</text>
					  </form>
					</lexical-unit>
				</entry>".Replace("glossWS", _glossingLanguageWSId).Replace("bogusWS", _vernacularLanguageWSId);
			}
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
					GatherWordListConfig.CreateForTests(_simpleWordListFilePath,
							WritingSystemsIdsForTests.AnalysisIdForTest, _catalog),
					_lexEntryRepository,
					_viewTemplate, new TaskMemoryRepository());

			Assert.IsNotNull(g);
		}

		[Test]
		public void Activate_MissingWordListFile_GivesMessage()
		{
			GatherWordListTask g = new GatherWordListTask(
				   GatherWordListConfig.CreateForTests("NotThere.txt",
						   WritingSystemsIdsForTests.AnalysisIdForTest, new WordListCatalog()),
				   _lexEntryRepository,
					_viewTemplate, new TaskMemoryRepository());

			 Assert.Throws<ErrorReport.ProblemNotificationSentToUserException>(() => g.Activate()); //should give a box to user, an exception in this text environment
		}


		[Test]
		public void Activate_WritingSystemNotInCurrentList_GivesMessage()
		{
			GatherWordListTask g = new GatherWordListTask(
				GatherWordListConfig.CreateForTests(_simpleWordListFilePath,
					"z7z", new WordListCatalog()),
				_lexEntryRepository,
				_viewTemplate, new TaskMemoryRepository());

			Assert.Throws<ErrorReport.ProblemNotificationSentToUserException>(() => g.Activate()); //should give a box to user, an exception in this text environment
		}

		[Test]
		public void CurrentLexemeFormFromWordList_AtStart_IsCorrect()
		{
			Assert.AreEqual("one", Task.CurrentEllicitationForm);
		}

		[Test]
		public void NavigateNext_HasAnotherWord_DoesMove()
		{
			Task.NavigateNext();
			Assert.AreEqual("two", Task.CurrentEllicitationForm);
		}

		[Test]
		public void NavigatePrevious_OnSecond_TakesToFirst()
		{
			Task.NavigateNext();
			Task.NavigatePrevious();
			Assert.AreEqual("one", Task.CurrentEllicitationForm);
		}

		[Test]
		public void CanNavigateNext_OnFirst_True()
		{
			Assert.IsTrue(Task.CanNavigateNext);
		}

		[Test]
		public void CanNavigatePrevious_OnFirst_False()
		{
			Assert.IsFalse(Task.CanNavigatePrevious);
		}

		[Test]
		public void CanNavigateNext_AtEnd_False()
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
		public void IsTaskComplete_AtEnd_True()
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
			Assert.AreEqual("two", Task.CurrentEllicitationForm);
		}

		[Test]
		public void LastWordAlreadyCollected()
		{
			//add an entry with a sense using the last word in the list as a gloss
			AddEntryAndSense("three");

			Task.NavigateFirstToShow();
			Assert.AreEqual("one", Task.CurrentEllicitationForm);
			Task.NavigateNext();
			Assert.IsTrue(Task.CanNavigateNext);
			Assert.AreEqual("two", Task.CurrentEllicitationForm);
			Task.NavigateNext();
			Assert.IsTrue(Task.IsTaskComplete); //we don't get to see "three"
		}

		[Test]
		public void SkipMiddleWordAlreadyCollected()
		{
			AddEntryAndSense("two");
			Task.NavigateFirstToShow();

			Assert.AreEqual("one", Task.CurrentEllicitationForm);
			Task.NavigateNext();
			Assert.AreEqual("three", Task.CurrentEllicitationForm);
		}

		[Test]
		public void SkipFirstTwoWordsAlreadyCollected()
		{
			AddEntryAndSense("one");
			AddEntryAndSense("two");
			Task.NavigateFirstToShow();
			Assert.AreEqual("three", Task.CurrentEllicitationForm);
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
		public void WordCollected_LexEntryAlreadyExists_WordAppearsInCompletedBox()
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
		public void WordCollected_WordAlreadyInDBButNoSense_AddsNewSense()
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
		public void WordCollected_WordAlreadyInDBWithSense_AddsAnotherSense()
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

		private WritingSystemDefinition VernWs
		{
			get
			{
				return BasilProject.Project.WritingSystems.Get(_vernacularLanguageWSId);
			}
		}


		[Test]
		public void CurrentLexemeForm_UsingLift_ShowsFirstItem()
		{
			var task = CreateAndActivateLiftTask(
				new List<string>(new[] { WritingSystemsIdsForTests.AnalysisIdForTest }), LiftXml
			);
			task.NavigateFirstToShow();
			Assert.AreEqual("apple", task.CurrentEllicitationForm);
			task.NavigateNext();
			Assert.AreEqual("cloud", task.CurrentEllicitationForm);
		 }

		[Test]
		public void CurrentLexemeForm_FieldSpecifiesFirstWritingSystem_GivesCorrectWritingSystemAlternative()
		{
			const string entries = @"
				<entry id='one'>
					<lexical-unit>
					  <form lang='en'>
						<text>apple</text>
					  </form>
					  <form lang='fr'>
						<text>pom</text>
					  </form>
					</lexical-unit>
				</entry>";

			var task = CreateAndActivateLiftTask(new List<string>(new string[]{"en"}), entries);
			task.NavigateFirstToShow();
			Assert.AreEqual("apple", task.CurrentEllicitationForm);
		}

		[Test]
		public void Activate_NoWordsWithElligibleWritingSystems_GivesMessage()
		{
			const string entries = @"
				<entry id='one'>
					<lexical-unit>
					  <form lang='en'>
						<text>apple</text>
					  </form>
					  <form lang='fr'>
						<text>pom</text>
					  </form>
					</lexical-unit>
				</entry>";
			using (new Palaso.Reporting.ErrorReport.NonFatalErrorReportExpected())
			{
				CreateAndActivateLiftTask(new List<string>(new string[] { "th" }), entries);
			}
		}
		[Test]
		public void Activate_NoWordsWithFirstWSButHaveOthers_NoMessage()
		{
			const string entries = @"
				<entry id='one'>
					<lexical-unit>
					  <form lang='en'>
						<text>apple</text>
					  </form>
					  <form lang='fr'>
						<text>pom</text>
					  </form>
					</lexical-unit>
				</entry>";

			var task =CreateAndActivateLiftTask(new List<string>(new string[] { "th", "fr" }), entries);
			task.NavigateFirstToShow();
		}

		[Test]
		public void Activate_NoSenseWithFirstWSButHaveOthers_NoMessage()
		{
			const string entries = @"
				<entry id='one'>
					<lexical-unit>
					  <form lang='en'>
						<text>apple</text>
					  </form>
					</lexical-unit>
					<sense>
						<gloss lang='en'><text>body</text></gloss>
						<gloss lang='fr'><text>corps</text></gloss>
					</sense>
			   </entry>";

			var task = CreateAndActivateLiftTask(new List<string>(new string[] { "th", "fr" }), entries);
			task.NavigateFirstToShow();
		}

		[Test]
		public void CurrentEllicitationForm_FieldSpecifiesSecondWritingSystemInGloss_GivesCorrectWritingSystemAlternative()
		{
			const string entries = @"
				<entry id='one'>
					<lexical-unit>
					  <form lang='en'>
						<text>body</text>
					  </form>
					</lexical-unit>
					<sense>
						<gloss lang='en'><text>body</text></gloss>
						<gloss lang='fr'><text>corps</text></gloss>
					</sense>
				</entry>";

			var task = CreateAndActivateLiftTask(new List<string>(new string[]{"fr","en"}), entries);
			task.NavigateFirstToShow();
			Assert.AreEqual("corps", task.CurrentEllicitationForm);
		}

		[Test]
		public void CurrentEllicitationForm_SenseMissing_GivesCorrectWritingSystemAlternative()
		{
			const string entries = @"
				<entry id='one'>
					<lexical-unit>
					  <form lang='en'>
						<text>body</text>
					  </form>
					</lexical-unit>
				</entry>";

			var task = CreateAndActivateLiftTask(new List<string>(new string[] { "fr", "en" }), entries);
			task.NavigateFirstToShow();
			Assert.AreEqual("body", task.CurrentEllicitationForm);
		}
		[Test]
		public void NavigateNext_NextDoesntHaveAndMatchingLanguages_SkipsOver()
		{
			const string entries = @"
				<entry id='one'>
					<lexical-unit>
					  <form lang='en'>
						<text>apple</text>
					  </form>
					</lexical-unit>
				</entry>
				<entry id='2'>
					<lexical-unit>
					  <form lang='bogusWS'>
						<text>SKIP ME!</text>
					  </form>
					</lexical-unit>
				</entry>
				<entry id='3'>
					<lexical-unit>
					  <form lang='en'>
						<text>orange</text>
					  </form>
					</lexical-unit>
				</entry>";

			var task = CreateAndActivateLiftTask(new List<string>(new string[]{"en"}),
								entries);
			task.NavigateFirstToShow();
			Assert.AreEqual("apple", task.CurrentEllicitationForm);
			task.NavigateNext();//skips "Skip me!"
			Assert.IsFalse(task.IsTaskComplete);
			Assert.AreEqual("orange", task.CurrentEllicitationForm);
		}
		[Test]
		public void CanNavigateNext_NoFurtherMatchesHaveRequiredLanguages_False()
		{
			var wsWhichIsValidButIsntInTheWordList = WritingSystemsIdsForTests.VernacularIdForTest;

			var entries =@"
				<entry id='one'>
					<lexical-unit>
					  <form lang='glossWS'>
						<text>apple</text>
					  </form>
					</lexical-unit>
				</entry>
				<entry id='2'>
					<lexical-unit>
					  <form lang='bogusWS'>
						<text>2</text>
					  </form>
					</lexical-unit>
				</entry>
				<entry id='3'>
					<lexical-unit>
					  <form lang='bogusWS'>
						<text>3</text>
					  </form>
					</lexical-unit>
				</entry>".Replace("glossWS", _glossingLanguageWSId).Replace("bogusWS", _vernacularLanguageWSId);

			var task = CreateAndActivateLiftTask(new List<string>(new string[] { WritingSystemsIdsForTests.AnalysisIdForTest }),
												 entries);
			task.NavigateFirstToShow();
			Assert.AreEqual("apple", task.CurrentEllicitationForm);
			Assert.IsTrue(task.CanNavigateNext); //notice, even though there will be none, thid is defined to say true until we try... it doesn't look ahead
			task.NavigateNext();
			Assert.IsTrue(task.IsTaskComplete);
			Assert.IsFalse(task.CanNavigateNext);
		}


		[Test]
		public void WordCollected_LiftWithSemanticDomain_CopiedOver()
		{
			LexSense firstSense = AddWordAndGetFirstSense();
			OptionRefCollection domains =
					firstSense.GetProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			Assert.AreEqual(1, domains.Count);
			Assert.AreEqual("fruit",domains.KeyAtIndex(0));
		}

		[Test]
		public void WordCollected_LiftWithPartOfSpeech_CopiedOver()
		{
			LexSense firstSense = AddWordAndGetFirstSense();
			OptionRef pos =
					firstSense.GetProperty<OptionRef>(
							LexSense.WellKnownProperties.PartOfSpeech);
			Assert.AreEqual("noun", pos.Key);
		}

		[Test]
		public void WordCollected_LiftWithCustomField_CopiedOver()
		{
			LexSense firstSense = AddWordAndGetFirstSense();
			var custom = firstSense.GetProperty<MultiText>("custom1");
			Assert.IsNotNull(custom);
			Assert.AreEqual("EnglishCustomValue", custom.GetExactAlternative("en"));
		}

		private LexSense AddWordAndGetFirstSense()
		{
			var task = CreateAndActivateLiftTask(
				new List<string>(new[] { WritingSystemsIdsForTests.AnalysisIdForTest }),
				LiftXml
			);
			task.NavigateFirstToShow();
			task.WordCollected( GetMultiText("apun"));
			var entries = task.GetRecordsWithMatchingGloss();
			Assert.AreEqual(1, entries.Count);

			return entries[0].RealObject.Senses[0];
		}


		private MultiText GetMultiText(string text)
		{
			MultiText word = new MultiText();
			word[VernWs.Id] = text;
			return word;
		}

		private GatherWordListTask CreateAndActivateLiftTask(IEnumerable<string> definitionWritingSystems, string entriesXml)
		{
			var file = new TempLiftFile("wordlist.lift", _tempFolder, entriesXml, Palaso.Lift.Validation.Validator.LiftVersion);

			var vt = new ViewTemplate();

			vt.Add(new Field(
				Field.FieldNames.EntryLexicalForm.ToString(),
				"LexEntry",
				new[] { WritingSystemsIdsForTests.VernacularIdForTest }
			));
			vt.Add(new Field(
				LexSense.WellKnownProperties.Definition.ToString(),
				"LexSense", definitionWritingSystems
			));

			var t = new GatherWordListTask(
				GatherWordListConfig.CreateForTests(file.Path, "xx", _catalog),
				_lexEntryRepository,
				vt,
				new TaskMemoryRepository()
			);
			t.Activate();
			return t;
		}
	}
}