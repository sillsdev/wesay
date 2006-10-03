using System;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.UI;
using NUnit.Framework;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LexFieldTaskTests
	{
		IRecordListManager _recordListManager;
		string _filePath;

		private IFilter<LexEntry> _filter;
		private string _fieldsToShow;
		private string _label;
		private string _description;

		private string _lexicalForm;

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();

			this._filePath = System.IO.Path.GetTempFileName();
			this._recordListManager = new Db4oRecordListManager(_filePath);

			LexEntry entry = new LexEntry();
			_lexicalForm = "vernacular";
			entry.LexicalForm.SetAlternative(BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Id, _lexicalForm);
			IRecordList<LexEntry> masterRecordList = this._recordListManager.Get<LexEntry>();
			masterRecordList.Add(entry);

			_fieldsToShow = "LexicalForm";
			_label = "My label";
			_description = "My description";
			_filter = new MissingGlossFilter(BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Id);
		}

		[TearDown]
		public void TearDown()
		{
			this._recordListManager.Dispose();
			System.IO.File.Delete(_filePath);
		}

		[Test]
		public void Create()
		{
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, _fieldsToShow);
			Assert.IsNotNull(task);
		}

		[Test]
		public void Create_RecordsIsEmpty()
		{
			ClearMasterRecordList();
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, _fieldsToShow);
			Assert.IsNotNull(task);
		}

		private void ClearMasterRecordList() {
			this._recordListManager.Get<LexEntry>().Clear();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_RecordsIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(null, _filter, _label, _description, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_FilterIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, null, _label, _description, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_LabelIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, _filter, null, _description, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_DescriptionIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, _filter, _label, null, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_FieldFilterIsNull_ThrowsArgumentNullException()
		{
			new LexFieldTask(_recordListManager, _filter, _label, _description, null);
		}

		[Test]
		public void Label_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, _fieldsToShow);
			Assert.AreEqual(_label, task.Label);
		}

		[Test]
		public void Description_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, _fieldsToShow);
			Assert.AreEqual(_description, task.Description);
		}

		[Test]
		public void Activate_Refreshes()
		{
			//JDH broke this Tuesday, but is at a loss to say why (it's fine in the real app)
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, _fieldsToShow);
			task.Activate();
			Assert.IsTrue(((LexFieldTool)task.Control).ControlDetails.ControlFormattedView.Text.Contains(_lexicalForm));
			Assert.AreEqual(1, task.DataSource.Count);
			task.Deactivate();
			ClearMasterRecordList();
			task.Activate();
			Assert.AreEqual(string.Empty, ((LexFieldTool)task.Control).ControlDetails.ControlFormattedView.Text);
			Assert.AreEqual(0, task.DataSource.Count);
			task.Deactivate();
		}

		[Test]
		public void FieldsToShow_SingleField_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, "Single");
			Assert.AreEqual(true, task.ShowField("Single"));
			Assert.AreEqual(false, task.ShowField("SingleField"));
			Assert.AreEqual(false, task.ShowField("Single Field"));
			Assert.AreEqual(false, task.ShowField("Field"));
			Assert.AreEqual(false, task.ShowField(" "));
			Assert.AreEqual(false, task.ShowField(String.Empty));
		}

		[Test]
		public void FieldsToShow_TwoFields_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, "First Second");
			Assert.AreEqual(true, task.ShowField("First"));
			Assert.AreEqual(true, task.ShowField("Second"));
			Assert.AreEqual(false, task.ShowField("FirstSecond"));
			Assert.AreEqual(false, task.ShowField(" "));
			Assert.AreEqual(false, task.ShowField(String.Empty));
		}

		[Test]
		public void FieldsToShow_ThreeFields_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, "First Second Third");
			Assert.AreEqual(true, task.ShowField("First"));
			Assert.AreEqual(true, task.ShowField("Second"));
			Assert.AreEqual(true, task.ShowField("Third"));
			Assert.AreEqual(false, task.ShowField("FirstSecond"));
			Assert.AreEqual(false, task.ShowField("SecondThird"));
			Assert.AreEqual(false, task.ShowField(" "));
			Assert.AreEqual(false, task.ShowField(String.Empty));
		}

		[Test]
		public void FieldsToShow_HidingField_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_recordListManager, _filter, _label, _description, "GhostGloss Gloss");
			Assert.AreEqual(true, task.ShowField("Gloss"));
			Assert.AreEqual(true, task.ShowField("GhostGloss"));
		}

	}
}
