using NUnit.Framework;
using SIL.DictionaryServices.Model;
using SIL.IO;
using SIL.TestUtilities;
using SIL.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalTools.AddMissingInfo;
using WeSay.Project;
using WeSay.TestUtilities;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingInfoTaskTests : TaskBaseTests
	{
		private LexEntryRepository _lexEntryRepository;
		private TemporaryFolder _tempFolder;

		private string _fieldsToShow;
		private string _label;
		private string _longLabel;
		private string _description;
		private string _remainingCountText;
		private string _referenceCountText;

		private string _lexicalForm;
		private ViewTemplate _viewTemplate;
		private readonly string _vernacularWritingSystemId = "PretendVernacular";
		private string _missingFieldName;

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			Sldr.Initialize(true);
			SIL.Windows.Forms.Keyboarding.KeyboardController.Initialize();
		}

		[OneTimeTearDown]
		public void FixtureTearDown()
		{
			Sldr.Cleanup();
		}

		[SetUp]
		public void Setup()
		{
			_tempFolder = new TemporaryFolder(GetType().Name);
			_lexEntryRepository = new LexEntryRepository(_tempFolder.GetPathForNewTempFile(false));

			WeSayProjectTestHelper.InitializeForTests();
			RtfRenderer.HeadWordWritingSystemId = _vernacularWritingSystemId;

			_missingFieldName = LexSense.WellKnownProperties.Definition;

			LexEntry entry = _lexEntryRepository.CreateItem();
			_lexicalForm = "vernacular";
			entry.LexicalForm.SetAlternative(_vernacularWritingSystemId, _lexicalForm);
			_lexEntryRepository.SaveItem(entry);
			_longLabel = "Long label";
			_remainingCountText = "Remaining count:";
			_referenceCountText = "Reference count:";

			_fieldsToShow = "LexicalForm";
			_label = "My label";
			_description = "My description";

			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(new Field(LexEntry.WellKnownProperties.LexicalUnit,
										"LexEntry",
										new string[] { _vernacularWritingSystemId }));
			_viewTemplate.Add(new Field(LexSense.WellKnownProperties.Definition,
										"LexSense",
										new string[] { "en" }));
			_viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(),
										"LexExampleSentence",
										new string[] { "th" }));
			_task = CreateMissingInfoTask(_lexEntryRepository,
										_missingFieldName,
										_label,
										_longLabel,
										_description,
										_remainingCountText,
										_referenceCountText,
										_viewTemplate,
										_fieldsToShow, string.Empty);
		}

		private MissingInfoTask CreateMissingInfoTask(LexEntryRepository repository, string missingInfoField, string label, string longLabel, string description, string remainingCountText, string referenceCountText, ViewTemplate template, string fieldsToShow, string writingSystemsToMatchCommaSeparated)
		{
			MissingInfoConfiguration config = MissingInfoConfiguration.CreateForTests(missingInfoField, label, longLabel, description, remainingCountText, referenceCountText, fieldsToShow, writingSystemsToMatchCommaSeparated);
			return new MissingInfoTask(config, repository, template, new TaskMemoryRepository());
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
		public void Create()
		{
			Assert.IsNotNull(_task);
		}



		//broke around changeset e9988de5d599, (20Mar2009)when I (JH) added the ability to specify which writing system to filter on
		[Test, Ignore("broken test which is based on some unwritten assumption...")]
		public void Activate_Refreshes()
		{
			MissingInfoTask task = (MissingInfoTask)_task;
			task.Activate();
			try
			{
				Assert.IsTrue(
						((MissingInfoControl)task.Control).EntryViewControl.RtfContentsOfPreviewForTests.Contains(_lexicalForm));

				Assert.AreEqual(1, _lexEntryRepository.CountAllItems());
			}
			finally
			{
				task.Deactivate();
			}
			_lexEntryRepository.CreateItem();  //REVIEW: So, connect the dots for me...  Why should creating an
											   // item here make the list switch to that item after the Activate()? (JH)
			task.Activate();
			try
			{
				Assert.IsTrue(
						((MissingInfoControl)task.Control).EntryViewControl.DataSource.LexicalForm.
								Empty);
				Assert.AreEqual(2, _lexEntryRepository.CountAllItems());
			}
			finally
			{
				task.Deactivate();
			}
		}

		[Test]
		public void FieldsToShow_SingleField_InitializedFromCreate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] { "en" };
			viewTemplate.Add(new Field("Single", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("SingleField", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("Field", "LexSense", writingSystemIds));

			MissingInfoTask task = CreateMissingInfoTask(_lexEntryRepository,
													   "Single",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "Single", string.Empty);
			Assert.AreEqual(true, task.ViewTemplate.Contains("Single"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("SingleField"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("Field"));
		}

		[Test]
		public void FieldsToShow_TwoFields_InitializedFromCreate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] { "en" };
			viewTemplate.Add(new Field("First", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("Second", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("FirstSecond", "LexSense", writingSystemIds));

			MissingInfoTask task = CreateMissingInfoTask(_lexEntryRepository,
													   "Second",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "First Second", string.Empty);
			Assert.AreEqual(true, task.ViewTemplate.Contains("First"));
			Assert.AreEqual(true, task.ViewTemplate.Contains("Second"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("FirstSecond"));
		}

		[Test]
		public void FieldsToShow_ThreeFields_InitializedFromCreate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] { "en" };
			viewTemplate.Add(new Field("First", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("Second", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("Third", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("FirstSecond", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("SecondThird", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("FirstSecondThird", "LexSense", writingSystemIds));

			MissingInfoTask task = CreateMissingInfoTask(_lexEntryRepository,
													   "Third",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "First Second Third", string.Empty);
			Assert.AreEqual(true, task.ViewTemplate.Contains("First"));
			Assert.AreEqual(true, task.ViewTemplate.Contains("Second"));
			Assert.AreEqual(true, task.ViewTemplate.Contains("Third"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("FirstSecond"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("SecondThird"));
		}

		[Test]
		public void FieldsToShow_HidingField_InitializedFromCreate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] { "en" };
			viewTemplate.Add(new Field("Dummy", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("PrefixDummy", "LexSense", writingSystemIds));

			MissingInfoTask task = CreateMissingInfoTask(_lexEntryRepository,
													   "Dummy",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "PrefixDummy Dummy", string.Empty);
			Assert.AreEqual(true, task.ViewTemplate.Contains("Dummy"));
			Assert.AreEqual(true, task.ViewTemplate.Contains("PrefixDummy"));
		}

		[Test]
		public void FieldsToShow_PrefixedField_InitializedFromCreate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] { "en" };
			viewTemplate.Add(new Field("Dummy", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("PrefixDummy", "LexSense", writingSystemIds));

			MissingInfoTask task = CreateMissingInfoTask(_lexEntryRepository,
													   "Dummy",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "Dummy", string.Empty);
			Assert.AreEqual(true, task.ViewTemplate.Contains("Dummy"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("PrefixDummy"));
		}

		[Test] //Greg's WS-375
		public void FieldsToShow_RequiredFields_ShownEvenIfDisabledInDefaultTemplate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] { "en" };
			Field field = new Field("Dummy", "LexSense", writingSystemIds);
			field.Enabled = false;
			viewTemplate.Add(field);
			viewTemplate.Add(new Field("PrefixDummy", "LexSense", writingSystemIds));

			MissingInfoTask task = CreateMissingInfoTask(_lexEntryRepository,
													   "Dummy",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "PrefixDummy Dummy", string.Empty);
			Assert.AreEqual(true, task.ViewTemplate.Contains("Dummy"));
		}

		/// <summary>
		/// regression test for ws-960
		/// </summary>
		[Test]
		public void Deactivate_NoCurrentEntry_DoesntCrash()
		{
			using (TempFile repoFile = new TempFile())
			{
				using (LexEntryRepository repo = new LexEntryRepository(repoFile.Path))
				{
					//notice, no entries
					MissingInfoTask task = CreateMissingInfoTask(repo,
							   this._missingFieldName,
								_label,
								_longLabel,
								_description,
								_remainingCountText,
								_referenceCountText,
								_viewTemplate,
								_fieldsToShow, string.Empty);

					Assert.AreEqual(0, task.ExactCount);
					task.Activate();
					task.Deactivate();
				}
			}
		}

		//  in progress      [Test]
		//        public void OneSenseFromGatherBySemDom_ShowsOnlyOneMeaning()
		//        {
		//            LexEntry entry = CreateEmptyEntryWithOneSense();
		//            LexSense sense = entry.Senses[0];
		//            var sds = sense.GetOrCreateProperty<OptionRefCollection>(LexSense.WellKnownProperties.SemanticDomainsDdp4);
		//            var list = WeSay.Project.WeSayWordsProject.Project.GetOptionsList(LexSense.WellKnownProperties.SemanticDomainsDdp4);
		//            sds.Add(list.Options[0].Key);
		//
		//
		//        }
	}
}
