using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.UI;
using NUnit.Framework;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LexFieldTaskTests
	{
		Db4oDataSource _dataSource;
		Db4oBindingList<LexEntry> _records;
		string _FilePath;

		private IFilter _filter;
		private string _fieldsToShow;
		private string _label;
		private string _description;

		private string _lexicalForm;

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();

			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._records = new Db4oBindingList<LexEntry>(this._dataSource);

			LexEntry entry = new LexEntry();
			_lexicalForm = "vernacular";
			entry.LexicalForm.SetAlternative(BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Id, _lexicalForm);
			_records.Add(entry);

			_fieldsToShow = "LexicalForm";
			_label = "My label";
			_description = "My description";
			_filter = new MissingGlossFilter(BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Id);
		}

		[TearDown]
		public void TearDown()
		{
			this._records.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		[Test]
		public void Create()
		{
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, _fieldsToShow);
			Assert.IsNotNull(task);
		}

		[Test]
		public void Create_RecordsIsEmpty()
		{
			_records.Clear();
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, _fieldsToShow);
			Assert.IsNotNull(task);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_RecordsIsNull_ThrowsArgumentNullException()
		{
			LexFieldTask task = new LexFieldTask(null, _filter, _label, _description, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_FilterIsNull_ThrowsArgumentNullException()
		{
			LexFieldTask task = new LexFieldTask(_records, null, _label, _description, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_LabelIsNull_ThrowsArgumentNullException()
		{
			LexFieldTask task = new LexFieldTask(_records, _filter, null, _description, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_DescriptionIsNull_ThrowsArgumentNullException()
		{
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, null, _fieldsToShow);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_FieldFilterIsNull_ThrowsArgumentNullException()
		{
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, null);
		}

		[Test]
		public void Label_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, _fieldsToShow);
			Assert.AreEqual(_label, task.Label);
		}

		[Test]
		public void Description_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, _fieldsToShow);
			Assert.AreEqual(_description, task.Description);
		}

		[Test]
		public void Activate_Refreshes()
		{
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, _fieldsToShow);
			task.Activate();
			Assert.IsTrue(((LexFieldTool)task.Control).Control_Details.Control_FormattedView.Text.Contains(_lexicalForm));
			Assert.AreEqual(1, task.DataSource.Count);
			task.Deactivate();
			_records.Clear();
			task.Activate();
			Assert.AreEqual(string.Empty, ((LexFieldTool)task.Control).Control_Details.Control_FormattedView.Text);
			Assert.AreEqual(0, task.DataSource.Count);
			task.Deactivate();
		}

		[Test]
		public void FieldsToShow_SingleField_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, "Single");
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
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, "First Second");
			Assert.AreEqual(true, task.ShowField("First"));
			Assert.AreEqual(true, task.ShowField("Second"));
			Assert.AreEqual(false, task.ShowField("FirstSecond"));
			Assert.AreEqual(false, task.ShowField(" "));
			Assert.AreEqual(false, task.ShowField(String.Empty));
		}

		[Test]
		public void FieldsToShow_ThreeFields_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, "First Second Third");
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
			LexFieldTask task = new LexFieldTask(_records, _filter, _label, _description, "GhostGloss Gloss");
			Assert.AreEqual(true, task.ShowField("Gloss"));
			Assert.AreEqual(true, task.ShowField("GhostGloss"));
		}

	}
}
