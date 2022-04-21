using NUnit.Framework;
using SIL.Data;
using SIL.DictionaryServices.Model;
using SIL.Lift;
using SIL.Lift.Options;
using SIL.Reporting;
using SIL.TestUtilities;
using SIL.WritingSystems;
using System.Collections.Generic;
using System.IO;
using WeSay.LexicalModel;
using WeSay.LexicalTools.GatherByWordList;
using WeSay.Project;
using WeSay.TestUtilities;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class GatherWordListTaskTests : TaskBaseTests
	{
		private TemporaryFolder _tempFolder;
		private LexEntryRepository _lexEntryRepository;
		private string _simpleWordListFilePath;
		private readonly string[] _words = { "one", "two", "three" };
		private ViewTemplate _viewTemplate;
		private string _glossingLanguageWSId;
		private string _vernacularLanguageWSId;
		private WordListCatalog _catalog;

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			Sldr.Initialize(true);
		}

		[OneTimeTearDown]
		public void FixtureTearDown()
		{
			Sldr.Cleanup();
		}

		[SetUp]
		public void Setup()
		{
			WeSayProjectTestHelper.InitializeForTests();
			_glossingLanguageWSId = WritingSystemsIdsForTests.AnalysisIdForTest;
			_vernacularLanguageWSId = WritingSystemsIdsForTests.VernacularIdForTest;
			BasilProject.Project.WritingSystems.Set(new WritingSystemDefinition("fr"));

			_tempFolder = new TemporaryFolder(GetType().Name);
			_simpleWordListFilePath = _tempFolder.GetPathForNewTempFile(false);
			//            _liftWordListFile = new TempLiftFile("wordlist.lift",_tempFolder, LiftXml, LiftIO.Validation.Validator.LiftVersion);

			_lexEntryRepository = new LexEntryRepository(_tempFolder.GetPathForNewTempFile(false)); // InMemoryRecordListManager();
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

			_viewTemplate.Add(
						new Field(
							LexSense.WellKnownProperties.Gloss.ToString(),
							"LexSense",
							new string[]
											{
												WritingSystemsIdsForTests.AnalysisIdForTest,
												"fr"
											}
						 )
					 );
			_catalog = new WordListCatalog();
			_catalog.Add(_simpleWordListFilePath, new WordListDescription("en", "label", "longLabel", "description"));
			_task = new GatherWordListTask(GatherWordListConfig.CreateForTests(_simpleWordListFilePath, _glossingLanguageWSId, _catalog),
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
						<gloss lang='es'>
							<text>manzana</text>
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
			if (_lexEntryRepository != null)
			{
				_lexEntryRepository.Dispose();
			}
			if (_tempFolder != null)
			{
				_tempFolder.Dispose();
			}
			WeSayProjectTestHelper.CleanupForTests();
		}

		[Test]
		public void Ctor_EmptyTemplate_DoesntCrash()
		{
			GatherWordListTask g = new GatherWordListTask(
					GatherWordListConfig.CreateForTests(_simpleWordListFilePath,
							WritingSystemsIdsForTests.AnalysisIdForTest, _catalog),
					_lexEntryRepository,
					_viewTemplate, new TaskMemoryRepository());
			g.Activate();
			Assert.IsNotNull(g);
			g.Deactivate();
		}

		[Test]
		public void Activate_MissingWordListFile_GivesMessage()
		{
			GatherWordListTask g = new GatherWordListTask(
				   GatherWordListConfig.CreateForTests("NotThere.txt",
						   WritingSystemsIdsForTests.AnalysisIdForTest, new WordListCatalog()),
				   _lexEntryRepository,
					_viewTemplate, new TaskMemoryRepository());

			// the code doesn't show the errror box in release builds, but
			// the builder publishing configuration does run tests in release builds
			using (new SIL.Reporting.ErrorReport.NonFatalErrorReportExpected())
			{
				g.Activate();
			}
			g.Deactivate();
		}


		[Test]
		public void CurrentLexemeFormFromWordList_AtStart_IsCorrect()
		{
			Assert.AreEqual("one", Task.CurrentPromptingForm);
			Task.Deactivate();
		}

		[Test]
		public void NavigateNext_HasAnotherWord_DoesMove()
		{
			Task.NavigateNext();
			Assert.AreEqual("two", Task.CurrentPromptingForm);
			Task.Deactivate();
		}

		[Test]
		public void NavigatePrevious_OnSecond_TakesToFirst()
		{
			Task.NavigateNext();
			Task.NavigatePrevious();
			Assert.AreEqual("one", Task.CurrentPromptingForm);
			Task.Deactivate();
		}

		[Test]
		public void CanNavigateNext_OnFirst_True()
		{
			Assert.IsTrue(Task.CanNavigateNext);
			Task.Deactivate();
		}

		[Test]
		public void CanNavigatePrevious_OnFirst_False()
		{
			Assert.IsFalse(Task.CanNavigatePrevious);
			Task.Deactivate();
		}

		[Test]
		public void CanNavigateNext_AtEnd_False()
		{
			Assert.IsTrue(Task.CanNavigateNext);
			NextToEnd();
			Assert.IsFalse(Task.CanNavigateNext);
			Task.Deactivate();
		}

		private void NextToEnd()
		{
			for (int i = 0; i < _words.Length; i++)
			{
				Task.NavigateNext();
			}
		}

		[Test]
		[Ignore("Can't be tested on task, make sure it is correct on view.")]
		public void GoingToNextWordSavesCurrentGloss() { }

		[Test]
		public void IsTaskComplete_AtEnd_True()
		{
			Assert.IsFalse(Task.IsTaskComplete);
			NextToEnd();
			Assert.IsTrue(Task.IsTaskComplete);
			Task.NavigatePrevious();
			Assert.IsFalse(Task.IsTaskComplete);
			Task.Deactivate();
		}

		[Test]
		public void CanBackupFromEnd()
		{
			Assert.IsFalse(Task.IsTaskComplete);
			NextToEnd();
			Task.NavigatePrevious();
			Assert.IsFalse(Task.IsTaskComplete);
			Assert.IsTrue(Task.CanNavigateNext);
			Task.Deactivate();
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
			Task.Deactivate();
		}

		[Test]
		public void FirstWordAlreadyCollected()
		{
			//add a word with the first wordlist-word already in a sense
			AddEntryAndSense("one");
			Task.NavigateFirstToShow();
			Assert.AreEqual("two", Task.CurrentPromptingForm);
			Task.Deactivate();
		}

		[Test]
		public void LastWordAlreadyCollected()
		{
			//add an entry with a sense using the last word in the list as a gloss
			AddEntryAndSense("three");

			Task.NavigateFirstToShow();
			Assert.AreEqual("one", Task.CurrentPromptingForm);
			Task.NavigateNext();
			Assert.IsTrue(Task.CanNavigateNext);
			Assert.AreEqual("two", Task.CurrentPromptingForm);
			Task.NavigateNext();
			Assert.IsTrue(Task.IsTaskComplete); //we don't get to see "three"
			Task.Deactivate();
		}

		[Test]
		public void SkipMiddleWordAlreadyCollected()
		{
			AddEntryAndSense("two");
			Task.NavigateFirstToShow();

			Assert.AreEqual("one", Task.CurrentPromptingForm);
			Task.NavigateNext();
			Assert.AreEqual("three", Task.CurrentPromptingForm);
			Task.Deactivate();
		}

		[Test]
		public void SkipFirstTwoWordsAlreadyCollected()
		{
			AddEntryAndSense("one");
			AddEntryAndSense("two");
			Task.NavigateFirstToShow();
			Assert.AreEqual("three", Task.CurrentPromptingForm);
			Task.Deactivate();
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
			Assert.AreEqual(1, Task.GetRecordsWithMatchingMeaning().Count);
			Task.Deactivate();
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

			ResultSet<LexEntry> matchingLexicalForms = Task.GetRecordsWithMatchingMeaning();
			Assert.AreEqual(1, matchingLexicalForms.Count);
			Task.Deactivate();
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
			Task.Deactivate();
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
			Task.Deactivate();
		}

		[Test]
		public void AddWordASecondTime_DoesNothing()
		{
			RecordToken<LexEntry> token = PrepareEntryWithOneMeaning();
			RecordToken<LexEntry> token2 = PrepareEntryWithOneMeaning();
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
			sense.Definition[_glossingLanguageWSId] = gloss;
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
				return ((GatherWordListTask)_task);
			}
		}

		/// <summary>
		/// test support for spell fixing
		/// </summary>
		[Test]
		public void RemovingGlossFromEmptyEntry_RemovesEntry()
		{
			RecordToken<LexEntry> token = PrepareEntryWithOneMeaning();

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(token);
			Assert.AreEqual(0,
							_lexEntryRepository.GetEntriesWithMatchingLexicalForm("uno", VernWs).
									Count);
			Task.Deactivate();
		}

		/// <summary>
		/// test support for spell fixing
		/// </summary>
		[Test]
		public void RemovingGlossFromEntryWithOtherSenses_OnlyRemovesGloss()
		{
			RecordToken<LexEntry> token = PrepareEntryWithOneMeaning();
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
			Task.Deactivate();
		}

		/// <summary>
		/// test support for spell fixing
		/// </summary>
		[Test]
		public void RemovingAssociationWith_OnlyRemovesGloss()
		{
			RecordToken<LexEntry> token = PrepareEntryWithOneMeaning();
			//now tweak the entry
			LexEntry entry = token.RealObject;
			LexSense leaveAloneSense = new LexSense();
			entry.Senses.Add(leaveAloneSense);
			leaveAloneSense.Definition.SetAlternative(_glossingLanguageWSId, "single");
			Assert.AreEqual(2, entry.Senses.Count);

			//now simulate removing it, as when the user wants to correct spelling
			Task.TryToRemoveAssociationWithListWordFromEntry(token);
			Assert.AreEqual(1,
							_lexEntryRepository.GetEntriesWithMatchingLexicalForm("uno", VernWs).
									Count);
			Assert.AreEqual(1, entry.Senses.Count);
			Task.Deactivate();
		}

		/// <summary>
		/// test support for spell fixing (ideally, this would move the sense, but this what we do for now)
		/// </summary>
		[Test]
		public void TryToRemoveAssociationWithListWordFromEntry_SenseHasExample_DoesNothing()
		{
			RecordToken<LexEntry> token = PrepareEntryWithOneMeaning();
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
							sense.Definition.GetExactAlternative(_glossingLanguageWSId),
							"should not remove the gloss");
			Task.Deactivate();
		}

		private RecordToken<LexEntry> PrepareEntryWithOneMeaning()
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
			Task.Deactivate();
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
			Assert.AreEqual("apple", task.CurrentPromptingForm);
			task.NavigateNext();
			Assert.AreEqual("cloud", task.CurrentPromptingForm);
			task.Deactivate();
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

			var task = CreateAndActivateLiftTask(new List<string>(new string[] { "en" }), entries);
			task.NavigateFirstToShow();
			Assert.AreEqual("apple", task.CurrentPromptingForm);
			task.Deactivate();
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
			using (new ErrorReport.NonFatalErrorReportExpected())
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

			var task = CreateAndActivateLiftTask(new List<string>(new string[] { "th", "fr" }), entries);
			task.NavigateFirstToShow();
			task.Deactivate();
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
			task.Deactivate();
		}

		[Test]
		public void CurrentPromptingForm_HasDefButNotGloss_GivesCorrectWritingSystemAlternative()
		{
			const string entries = @"
<entry
		id='one'>
		<lexical-unit>
			<form
				lang='en'>
				<text>skin (of man)</text>
			</form>
		</lexical-unit>
		<sense
			id='skin'>
			<gloss
				lang='en'>
				<text>skin (of man)</text>
			</gloss>
			<definition>
				<form
					lang='en'>
					<text>skin (of man)</text>
				</form>
				<form
					lang='id'>
					<text>kulit (manusia)</text>
				</form>
			</definition>
			</sense>
	</entry>";

			var task = CreateAndActivateLiftTask(new List<string>(new string[] { "id", "en" }), entries);
			task.NavigateFirstToShow();
			Assert.AreEqual("kulit (manusia)", task.CurrentPromptingForm);
			task.Deactivate();
		}

		[Test]
		public void CurrentPromptingForm_FieldSpecifiesSecondWritingSystemInGloss_GivesCorrectWritingSystemAlternative()
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

			var task = CreateAndActivateLiftTask(new List<string>(new string[] { "fr", "en" }), entries);
			task.NavigateFirstToShow();
			Assert.AreEqual("corps", task.CurrentPromptingForm);
			task.Deactivate();
		}

		[Test]
		public void CurrentPromptingForm_SenseMissing_GivesCorrectWritingSystemAlternative()
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
			Assert.AreEqual("body", task.CurrentPromptingForm);
			task.Deactivate();
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

			var task = CreateAndActivateLiftTask(new List<string>(new string[] { "en" }),
								entries);
			task.NavigateFirstToShow();
			Assert.AreEqual("apple", task.CurrentPromptingForm);
			task.NavigateNext();//skips "Skip me!"
			Assert.IsFalse(task.IsTaskComplete);
			Assert.AreEqual("orange", task.CurrentPromptingForm);
			task.Deactivate();
		}

		[Test]
		public void CanNavigateNext_NoFurtherMatchesHaveRequiredLanguages_False()
		{
			var wsWhichIsValidButIsntInTheWordList = WritingSystemsIdsForTests.VernacularIdForTest;

			var entries = @"
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
			Assert.AreEqual("apple", task.CurrentPromptingForm);
			Assert.IsTrue(task.CanNavigateNext); //notice, even though there will be none, thid is defined to say true until we try... it doesn't look ahead
			task.NavigateNext();
			Assert.IsTrue(task.IsTaskComplete);
			Assert.IsFalse(task.CanNavigateNext);
			task.Deactivate();
		}


		[Test]
		public void WordCollected_LiftWithSemanticDomain_CopiedOver()
		{
			LexSense firstSense = AddWordAndGetFirstSense();
			OptionRefCollection domains =
					firstSense.GetProperty<OptionRefCollection>(
							LexSense.WellKnownProperties.SemanticDomainDdp4);
			Assert.AreEqual(1, domains.Count);
			Assert.AreEqual("fruit", domains.KeyAtIndex(0));
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

		/// <summary>
		/// for example, the SIL-CAWL list has lots of glossing langs... people don't want all those copied in (and then showing up in the list of languages from the project!)
		/// </summary>
		[Test]
		public void WordCollected_LiftWithExtraLangs_OnlyThoseFormsFromLangsInThisProjectAreCopiedOver()
		{
			LexSense firstSense = AddWordAndGetFirstSense();
			Assert.IsFalse(firstSense.Definition.ContainsAlternative("es"), "should not have received spanish definition because it wasn't in the viewtemplate");
			Assert.IsFalse(firstSense.Gloss.ContainsAlternative("es"), "should not have received spanish gloss because it wasn't in the viewtemplate");
		}

		[Test]
		public void TryToRemoveAssociationWithListWordFromEntry_SenseInDictIsIdenticalToSenseInWordList_AssociationIsRemoved()
		{
			Task.NavigateAbsoluteFirst();
			Task.WordCollected(GetMultiText("test"));
			var resultSet = Task.GetRecordsWithMatchingMeaning();
			Assert.That(Task.GetRecordsWithMatchingMeaning().Count, Is.EqualTo(1));
			Task.TryToRemoveAssociationWithListWordFromEntry(resultSet[0]);
			Assert.That(Task.GetRecordsWithMatchingMeaning().Count, Is.EqualTo(0));
			Task.Deactivate();
		}

		[Test]
		public void TryToRemoveAssociationWithListWordFromEntry_SenseInDictHasOneMorePropertyThanSenseInWordList_AssociationIsNotRemoved()
		{
			Task.NavigateAbsoluteFirst();
			Task.WordCollected(GetMultiText("test"));
			var token = Task.GetRecordsWithMatchingMeaning()[0];
			var senseToModify = token.RealObject.Senses[(int)token["SenseNumber"]];
			senseToModify.GetOrCreateProperty<MultiText>("ExtraProperty");
			Assert.That(Task.GetRecordsWithMatchingMeaning().Count, Is.EqualTo(1));
			Task.TryToRemoveAssociationWithListWordFromEntry(token);
			Assert.That(Task.GetRecordsWithMatchingMeaning().Count, Is.EqualTo(1));
			Task.Deactivate();
		}

		[Test]
		//Here I consider information that was removed to have been intentionally removed so we are NOT removing the association
		public void TryToRemoveAssociationWithListWordFromEntry_SenseInDictIsHasOneLessPropertyThanSenseInWordList_AssociationIsNotRemoved()
		{
			Task.NavigateAbsoluteFirst();
			Task.WordCollected(GetMultiText("test"));
			Task.CurrentTemplateSense.GetOrCreateProperty<MultiText>("ExtraProperty");
			var token = Task.GetRecordsWithMatchingMeaning()[0];
			Assert.That(Task.GetRecordsWithMatchingMeaning().Count, Is.EqualTo(1));
			Task.TryToRemoveAssociationWithListWordFromEntry(token);
			Assert.That(Task.GetRecordsWithMatchingMeaning().Count, Is.EqualTo(1));
			Task.Deactivate();
		}

		private LexSense AddWordAndGetFirstSense()
		{
			var task = CreateAndActivateLiftTask(
				new List<string>(new[] { WritingSystemsIdsForTests.AnalysisIdForTest }),
				LiftXml
			);
			task.NavigateFirstToShow();
			task.WordCollected(GetMultiText("apun"));
			var entries = task.GetRecordsWithMatchingMeaning();
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
			var file = new TempLiftFile("wordlist.lift", _tempFolder, entriesXml, SIL.Lift.Validation.Validator.LiftVersion);

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
			vt.Add(
					new Field(
						LexSense.WellKnownProperties.Gloss.ToString(),
						"LexSense",
						new string[]
										{
											WritingSystemsIdsForTests.AnalysisIdForTest,
											"fr"
										}
					 )
				);

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
