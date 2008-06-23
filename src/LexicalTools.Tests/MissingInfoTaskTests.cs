using System;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4oSpecific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingInfoTaskTests: TaskBaseTests
	{
		private class NoEntriesFilter: IFilter<LexEntry>
		{
			public Predicate<LexEntry> FilteringPredicate
			{
				get { return ReturnFalse; }
			}

			private static bool ReturnFalse(LexEntry e)
			{
				return false;
			}

			public string Key
			{
				get { return "NoEntries"; }
			}
		}

		private LexEntryRepository _lexEntryRepository;
		private string _filePath;

		private IFilter<LexEntry> _filter;
		private string _fieldsToShow;
		private string _label;
		private string _description;

		private string _lexicalForm;
		private ViewTemplate _viewTemplate;
		private readonly string _vernacularWritingSystemId = "PretendVernacular";

		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			Db4oLexModelHelper.InitializeForNonDbTests();
			WeSayWordsProject.InitializeForTests();
			RtfRenderer.HeadWordWritingSystemId = _vernacularWritingSystemId;

			Field field =
					new Field(LexSense.WellKnownProperties.Definition,
							  "LexSense",
							  new string[] {"analysis"});
			_filter = new MissingItemFilter(field);

			LexEntry entry = _lexEntryRepository.CreateItem();
			_lexicalForm = "vernacular";
			entry.LexicalForm.SetAlternative(_vernacularWritingSystemId, _lexicalForm);
			_lexEntryRepository.SaveItem(entry);

			_fieldsToShow = "LexicalForm";
			_label = "My label";
			_description = "My description";

			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(
					new Field(LexEntry.WellKnownProperties.LexicalUnit,
							  "LexEntry",
							  new string[] {_vernacularWritingSystemId}));
			_viewTemplate.Add(
					new Field(LexSense.WellKnownProperties.Definition,
							  "LexSense",
							  new string[] {"en"}));
			_viewTemplate.Add(
					new Field(Field.FieldNames.ExampleSentence.ToString(),
							  "LexExampleSentence",
							  new string[] {"th"}));

			_task =
					new MissingInfoTask(_lexEntryRepository,
										_filter,
										_label,
										_description,
										_viewTemplate,
										_fieldsToShow);
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void Create()
		{
			Assert.IsNotNull(_task);
		}

		[Test]
		public void Create_RecordsIsEmpty()
		{
			IFilter<LexEntry> filter = new NoEntriesFilter();
			MissingInfoTask task =
					new MissingInfoTask(_lexEntryRepository,
										filter,
										_label,
										_description,
										_viewTemplate,
										_fieldsToShow);
			Assert.IsNotNull(task);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_RecordsIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(null, _filter, _label, _description, _viewTemplate, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_FilterIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(_lexEntryRepository,
								null,
								_label,
								_description,
								_viewTemplate,
								_fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_LabelIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(_lexEntryRepository,
								_filter,
								null,
								_description,
								_viewTemplate,
								_fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_DescriptionIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(_lexEntryRepository,
								_filter,
								_label,
								null,
								_viewTemplate,
								_fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_FieldFilterIsNull_ThrowsArgumentNullException()
		{
			new MissingInfoTask(_lexEntryRepository,
								_filter,
								_label,
								_description,
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

			MissingInfoTask task =
					new MissingInfoTask(_lexEntryRepository,
										_filter,
										_label,
										_description,
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

			MissingInfoTask task =
					new MissingInfoTask(_lexEntryRepository,
										_filter,
										_label,
										_description,
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

			MissingInfoTask task =
					new MissingInfoTask(_lexEntryRepository,
										_filter,
										_label,
										_description,
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

			MissingInfoTask task =
					new MissingInfoTask(_lexEntryRepository,
										_filter,
										_label,
										_description,
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

			MissingInfoTask task =
					new MissingInfoTask(_lexEntryRepository,
										_filter,
										_label,
										_description,
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

			MissingInfoTask task =
					new MissingInfoTask(_lexEntryRepository,
										_filter,
										_label,
										_description,
										viewTemplate,
										"PrefixDummy Dummy");
			Assert.AreEqual(true, task.ViewTemplate.Contains("Dummy"));
		}
	}
}