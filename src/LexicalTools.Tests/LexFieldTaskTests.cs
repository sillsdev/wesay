using System;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using NUnit.Framework;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LexFieldTaskTests : TaskBaseTests
	{

		IRecordListManager _recordListManager;

		private IFilter<LexEntry> _filter;
		private string _fieldsToShow;
		private string _label;
		private string _description;

		private string _lexicalForm;
		private ViewTemplate _viewTemplate;

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			BasilProject.InitializeForTests();

			this._recordListManager = new InMemoryRecordListManager();
			Field field = new Field(Field.FieldNames.SenseGloss.ToString(),"LexSense" , new string[]{BasilProject.Project.WritingSystems.TestGetWritingSystemVern.Id});
			this._filter = new MissingItemFilter(field);
			this._recordListManager.Register<LexEntry>(_filter);

			LexEntry entry = new LexEntry();
			this._lexicalForm = "vernacular";
			entry.LexicalForm.SetAlternative(BasilProject.Project.WritingSystems.TestGetWritingSystemVern.Id, _lexicalForm);
			IRecordList<LexEntry> masterRecordList = this._recordListManager.GetListOfType<LexEntry>();
			masterRecordList.Add(entry);

			this._fieldsToShow = "LexicalForm";
			this._label = "My label";
			this._description = "My description";

			this._viewTemplate = new ViewTemplate();
			this._viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), "LexSense",new string[] { "en" }));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), "LexExampleSentence",new string[] { "th" }));

			this._task = new LexFieldTask(_recordListManager, _filter, _label, _description, _viewTemplate, _fieldsToShow);

		}

		[TearDown]
		public void TearDown()
		{
			this._recordListManager.Dispose();
		}

		[Test]
		public void Create()
		{
			Assert.IsNotNull(_task);
		}

		[Test]
		public void Create_RecordsIsEmpty()
		{
			ClearMasterRecordList();
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, _viewTemplate, _fieldsToShow);
			Assert.IsNotNull(task);
		}

		private void ClearMasterRecordList() {
			this._recordListManager.GetListOfType<LexEntry>().Clear();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_RecordsIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(null, _filter, _label, _description, _viewTemplate, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_FilterIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, null, _label, _description, _viewTemplate, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_LabelIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, _filter, null, _description, _viewTemplate, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_DescriptionIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, _filter, _label, null, _viewTemplate, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_FieldFilterIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, _filter, _label, _description, _viewTemplate, null);
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
			LexFieldTask task = (LexFieldTask)_task;
			task.Activate();
			Assert.IsTrue(((LexFieldControl)task.Control).ControlDetails.ControlFormattedView.Text.Contains(_lexicalForm));
			Assert.AreEqual(1, task.DataSource.Count);
			task.Deactivate();
			ClearMasterRecordList();
			task.Activate();
			Assert.AreEqual(string.Empty, ((LexFieldControl)task.Control).ControlDetails.ControlFormattedView.Text);
			Assert.AreEqual(0, task.DataSource.Count);
			task.Deactivate();
		}

		[Test]
		public void FieldsToShow_SingleField_InitializedFromCreate()
		{
			ViewTemplate viewTemplate = new ViewTemplate();
			string[] writingSystemIds = new string[] {"en"};
			viewTemplate.Add(new Field("Single", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("SingleField", "LexSense", writingSystemIds));
			viewTemplate.Add(new Field("Field", "LexSense", writingSystemIds));

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, viewTemplate, "Single");
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

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, viewTemplate, "First Second");
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

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, viewTemplate, "First Second Third");
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

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, viewTemplate, "PrefixDummy Dummy");
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

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, viewTemplate, "Dummy");
			Assert.AreEqual(true, task.ViewTemplate.Contains("Dummy"));
			Assert.AreEqual(false, task.ViewTemplate.Contains("PrefixDummy"));
		}

	}
}
