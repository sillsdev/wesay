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
		private FieldInventory _fieldInventory;

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			BasilProject.InitializeForTests();

			this._recordListManager = new InMemoryRecordListManager();
			Field field = new Field(Field.FieldNames.SenseGloss.ToString() , new string[]{BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Id});
			this._filter = new MissingGlossFilter(field);
			this._recordListManager.Register<LexEntry>(_filter);

			LexEntry entry = new LexEntry();
			this._lexicalForm = "vernacular";
			entry.LexicalForm.SetAlternative(BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Id, _lexicalForm);
			IRecordList<LexEntry> masterRecordList = this._recordListManager.Get<LexEntry>();
			masterRecordList.Add(entry);

			this._fieldsToShow = "LexicalForm";
			this._label = "My label";
			this._description = "My description";

			this._fieldInventory = new FieldInventory();
			this._fieldInventory.Add(new Field(Field.FieldNames.SenseGloss.ToString(), new string[] { "en" }));
			this._fieldInventory.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), new string[] { "th" }));

			this._task = new LexFieldTask(_recordListManager, _filter, _label, _description, _fieldInventory, _fieldsToShow);

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
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, _fieldInventory, _fieldsToShow);
			Assert.IsNotNull(task);
		}

		private void ClearMasterRecordList() {
			this._recordListManager.Get<LexEntry>().Clear();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_RecordsIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(null, _filter, _label, _description, _fieldInventory, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_FilterIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, null, _label, _description, _fieldInventory, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_LabelIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, _filter, null, _description, _fieldInventory, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_DescriptionIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, _filter, _label, null, _fieldInventory, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_FieldFilterIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, _filter, _label, _description, _fieldInventory, null);
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
			FieldInventory fieldInventory = new FieldInventory();
			string[] writingSystemIds = new string[] {"en"};
			fieldInventory.Add(new Field("Single", writingSystemIds));
			fieldInventory.Add(new Field("SingleField", writingSystemIds));
			fieldInventory.Add(new Field("Field", writingSystemIds));

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, fieldInventory, "Single");
			Assert.AreEqual(true, task.FieldInventory.Contains("Single"));
			Assert.AreEqual(false, task.FieldInventory.Contains("SingleField"));
			Assert.AreEqual(false, task.FieldInventory.Contains("Field"));
		}

		[Test]
		public void FieldsToShow_TwoFields_InitializedFromCreate()
		{
			FieldInventory fieldInventory = new FieldInventory();
			string[] writingSystemIds = new string[] { "en" };
			fieldInventory.Add(new Field("First", writingSystemIds));
			fieldInventory.Add(new Field("Second", writingSystemIds));
			fieldInventory.Add(new Field("FirstSecond", writingSystemIds));

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, fieldInventory, "First Second");
			Assert.AreEqual(true, task.FieldInventory.Contains("First"));
			Assert.AreEqual(true, task.FieldInventory.Contains("Second"));
			Assert.AreEqual(false, task.FieldInventory.Contains("FirstSecond"));
		}

		[Test]
		public void FieldsToShow_ThreeFields_InitializedFromCreate()
		{
			FieldInventory fieldInventory = new FieldInventory();
			string[] writingSystemIds = new string[] { "en" };
			fieldInventory.Add(new Field("First", writingSystemIds));
			fieldInventory.Add(new Field("Second", writingSystemIds));
			fieldInventory.Add(new Field("Third", writingSystemIds));
			fieldInventory.Add(new Field("FirstSecond", writingSystemIds));
			fieldInventory.Add(new Field("SecondThird", writingSystemIds));
			fieldInventory.Add(new Field("FirstSecondThird", writingSystemIds));

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, fieldInventory, "First Second Third");
			Assert.AreEqual(true, task.FieldInventory.Contains("First"));
			Assert.AreEqual(true, task.FieldInventory.Contains("Second"));
			Assert.AreEqual(true, task.FieldInventory.Contains("Third"));
			Assert.AreEqual(false, task.FieldInventory.Contains("FirstSecond"));
			Assert.AreEqual(false, task.FieldInventory.Contains("SecondThird"));
		}

		[Test]
		public void FieldsToShow_HidingField_InitializedFromCreate()
		{
			FieldInventory fieldInventory = new FieldInventory();
			string[] writingSystemIds = new string[] { "en" };
			fieldInventory.Add(new Field("Dummy", writingSystemIds));
			fieldInventory.Add(new Field("PrefixDummy", writingSystemIds));

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, fieldInventory, "PrefixDummy Dummy");
			Assert.AreEqual(true, task.FieldInventory.Contains("Dummy"));
			Assert.AreEqual(true, task.FieldInventory.Contains("PrefixDummy"));
		}

		[Test]
		public void FieldsToShow_PrefixedField_InitializedFromCreate()
		{
			FieldInventory fieldInventory = new FieldInventory();
			string[] writingSystemIds = new string[] { "en" };
			fieldInventory.Add(new Field("Dummy", writingSystemIds));
			fieldInventory.Add(new Field("PrefixDummy", writingSystemIds));

			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, fieldInventory, "Dummy");
			Assert.AreEqual(true, task.FieldInventory.Contains("Dummy"));
			Assert.AreEqual(false, task.FieldInventory.Contains("PrefixDummy"));
		}

	}
}
