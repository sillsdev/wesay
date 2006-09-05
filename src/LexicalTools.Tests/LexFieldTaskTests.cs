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

		Predicate<string> _fieldFilter;
		string _label;
		string _lexicalForm;

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();

			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._records = new Db4oBindingList<LexEntry>(this._dataSource);

			LexEntry entry = new LexEntry();
			_lexicalForm = "vernacular";
			entry.LexicalForm.SetAlternative(BasilProject.Project.VernacularWritingSystemDefault.Id, _lexicalForm);
			_records.Add(entry);

			_fieldFilter = delegate(string s)
						{
							return s == "LexicalForm";
						};
			_label = "My label";
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
			LexFieldTask task = new LexFieldTask(_records, _label, _fieldFilter);
			Assert.IsNotNull(task);
		}

		[Test]
		public void Create_RecordsIsEmpty()
		{
			_records.Clear();
			LexFieldTask task = new LexFieldTask(_records, _label, _fieldFilter);
			Assert.IsNotNull(task);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_RecordsIsNull_ThrowsArgumentNullException()
		{
			LexFieldTask task = new LexFieldTask(null, _label, _fieldFilter);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_LabelIsNull_ThrowsArgumentNullException()
		{
			LexFieldTask task = new LexFieldTask(_records, null, _fieldFilter);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_FilterIsNull_ThrowsArgumentNullException()
		{
			LexFieldTask task = new LexFieldTask(_records, _label, null);
		}

		[Test]
		public void Label_InitializedFromCreate()
		{
			LexFieldTask task = new LexFieldTask(_records, _label, _fieldFilter);
			Assert.AreEqual(_label, task.Label);
		}

		[Test]
		public void Activate_Refreshes()
		{
			LexFieldTask task = new LexFieldTask(_records, _label, _fieldFilter);
			task.Activate();
			Assert.IsTrue(task.Control_Details.Control_FormattedView.Contains(_lexicalForm));
			Assert.AreEqual(1, task.DataSource.Count);
			task.Deactivate();
			_records.Clear();
			task.Activate();
			Assert.AreEqual(string.Empty, task.Control_Details.Control_FormattedView);
			Assert.AreEqual(0, task.DataSource.Count);
			task.Deactivate();
		}

		[Test]
		public void Activate_Refilters()
		{
			LexFieldTask task = new LexFieldTask(_records, _label, _fieldFilter);

			Predicate<LexEntry> entriesWithoutGlosses = delegate(LexEntry entry)
						{
							if (entry.Senses.Count == 0)
							{
								return true;
							}
							foreach (LexSense sense in entry.Senses)
							{
								if (sense.Gloss[BasilProject.Project.AnalysisWritingSystemDefault.Id] == string.Empty)
								{
									return true;
								}
							}
							return false;
						};

			_records.ApplyFilter(entriesWithoutGlosses);

			task.Activate();
			Assert.AreEqual(1, task.DataSource.Count);
			task.Deactivate();
			LexSense newSense = (LexSense)_records[0].Senses.AddNew();
			newSense.Gloss[BasilProject.Project.AnalysisWritingSystemDefault.Id] = "a gloss";
			task.Activate();
			Assert.AreEqual(0, task.DataSource.Count);
			task.Deactivate();
		}

	}
}
