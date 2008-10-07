using System;
using System.IO;
using NUnit.Framework;
using TestUtilities;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingInfoTaskTests: TaskBaseTests
	{
		private LexEntryRepository _lexEntryRepository;
		private TemporaryFolder _tempFolder;
		private string _filePath;

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

		[SetUp]
		public void Setup()
		{
			_tempFolder = new TemporaryFolder();
			_filePath = _tempFolder.GetTemporaryFile();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			WeSayWordsProject.InitializeForTests();
			RtfRenderer.HeadWordWritingSystemId = _vernacularWritingSystemId;

			this._missingFieldName = LexSense.WellKnownProperties.Definition;

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
										new string[] {_vernacularWritingSystemId}));
			_viewTemplate.Add(new Field(LexSense.WellKnownProperties.Definition,
										"LexSense",
										new string[] {"en"}));
			_viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(),
										"LexExampleSentence",
										new string[] {"th"}));
			_task = new MissingInfoTask(_lexEntryRepository,
										_missingFieldName,
										_label,
										_longLabel,
										_description,
										_remainingCountText,
										_referenceCountText,
										_viewTemplate,
										_fieldsToShow);
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			_tempFolder.Delete();
		}

		[Test]
		public void Create()
		{
			Assert.IsNotNull(_task);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_RecordsIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(null,
								this._missingFieldName,
								_label,
								_longLabel,
								_description,
								_remainingCountText,
								_referenceCountText,
								_viewTemplate,
								_fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_FilterIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(_lexEntryRepository,
								null,
								_label,
								_longLabel,
								_description,
								_remainingCountText,
								_referenceCountText,
								_viewTemplate,
								_fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_LabelIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(_lexEntryRepository,
								this._missingFieldName,
								_label,
								null,
								_description,
								_remainingCountText,
								_referenceCountText,
								_viewTemplate,
								_fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_DescriptionIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(_lexEntryRepository,
								this._missingFieldName,
								_label,
								_longLabel,
								null,
								_remainingCountText,
								_referenceCountText,
								_viewTemplate,
								_fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_ReferenceCountTextIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(_lexEntryRepository,
								this._missingFieldName,
								_label,
								_longLabel,
								_description,
								_remainingCountText,
								null,
								_viewTemplate,
								_fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_FieldFilterIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(_lexEntryRepository,
								this._missingFieldName,
								_label,
								_longLabel,
								_description,
								_remainingCountText,
								_referenceCountText,
								_viewTemplate,
								null);
		}

		[Test]
		public void Label_InitializedFromCreate()
		{
			Assert.AreEqual(_label, _task.Label);
		}

		[Test]
		public void Description_InitializedFromCreate()
		{
			Assert.AreEqual(_description, _task.Description);
		}

		[Test]
		public void Activate_Refreshes()
		{
			MissingInfoTask task = (MissingInfoTask) _task;
			task.Activate();
			try
			{
				Assert.IsTrue(
						((MissingInfoControl) task.Control).EntryViewControl.ControlFormattedView.
								Text.Contains(_lexicalForm));

				Assert.AreEqual(1, _lexEntryRepository.CountAllItems());
			}
			finally
			{
				task.Deactivate();
			}
			_lexEntryRepository.CreateItem();
			task.Activate();
			try
			{
				Assert.IsTrue(
						((MissingInfoControl) task.Control).EntryViewControl.DataSource.LexicalForm.
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
			string[] writingSystemIds = new string[] {"en"};
			viewTemplate.Add(new Field("Single", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("SingleField", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("Field", "LexSense", writingSystemIds));

			MissingInfoTask task = new MissingInfoTask(_lexEntryRepository,
													   "Single",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "Single");
			Assert.AreEqual(true, task.ViewTemplate.Contains("Single"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("SingleField"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("Field"));
		}

		[Test]
		public void FieldsToShow_TwoFields_InitializedFromCreate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] {"en"};
			viewTemplate.Add(new Field("First", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("Second", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("FirstSecond", "LexSense", writingSystemIds));

			MissingInfoTask task = new MissingInfoTask(_lexEntryRepository,
													   "Second",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "First Second");
			Assert.AreEqual(true, task.ViewTemplate.Contains("First"));
			Assert.AreEqual(true, task.ViewTemplate.Contains("Second"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("FirstSecond"));
		}

		[Test]
		public void FieldsToShow_ThreeFields_InitializedFromCreate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] {"en"};
			viewTemplate.Add(new Field("First", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("Second", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("Third", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("FirstSecond", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("SecondThird", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("FirstSecondThird", "LexSense", writingSystemIds));

			MissingInfoTask task = new MissingInfoTask(_lexEntryRepository,
													   "Third",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "First Second Third");
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
			string[] writingSystemIds = new string[] {"en"};
			viewTemplate.Add(new Field("Dummy", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("PrefixDummy", "LexSense", writingSystemIds));

			MissingInfoTask task = new MissingInfoTask(_lexEntryRepository,
													   "Dummy",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "PrefixDummy Dummy");
			Assert.AreEqual(true, task.ViewTemplate.Contains("Dummy"));
			Assert.AreEqual(true, task.ViewTemplate.Contains("PrefixDummy"));
		}

		[Test]
		public void FieldsToShow_PrefixedField_InitializedFromCreate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] {"en"};
			viewTemplate.Add(new Field("Dummy", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("PrefixDummy", "LexSense", writingSystemIds));

			MissingInfoTask task = new MissingInfoTask(_lexEntryRepository,
													   "Dummy",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "Dummy");
			Assert.AreEqual(true, task.ViewTemplate.Contains("Dummy"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("PrefixDummy"));
		}

		[Test] //Greg's WS-375
		public void FieldsToShow_RequiredFields_ShownEvenIfDisabledInDefaultTemplate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] {"en"};
			Field field = new Field("Dummy", "LexSense", writingSystemIds);
			field.Enabled = false;
			viewTemplate.Add(field);
			viewTemplate.Add(new Field("PrefixDummy", "LexSense", writingSystemIds));

			MissingInfoTask task = new MissingInfoTask(_lexEntryRepository,
													   "Dummy",
													   _label,
													   _longLabel,
													   _description,
													   _remainingCountText,
													   _referenceCountText,
													   viewTemplate,
													   "PrefixDummy Dummy");
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
					MissingInfoTask task = new MissingInfoTask(repo,
							   this._missingFieldName,
								_label,
								_longLabel,
								_description,
								_remainingCountText,
								_referenceCountText,
								_viewTemplate,
								_fieldsToShow);

					Assert.AreEqual(0, task.ExactCount);
					task.Activate();
					task.Deactivate();
				}
			}
		}
	}
}