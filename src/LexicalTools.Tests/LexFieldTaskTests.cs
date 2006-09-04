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
		IBindingList _records;
		Predicate<string> _fieldFilter;
		string _label;
		string _lexicalForm;

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();
			_records = new InMemoryBindingList<LexEntry>();
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
			Assert.IsTrue(task.Control_Details.Control_DataView.Contains(_lexicalForm));
			Assert.AreEqual(1, task.DataSource.Count);
			task.Deactivate();
			_records.Clear();
			task.Activate();
			Assert.AreEqual(string.Empty, task.Control_Details.Control_DataView);
			Assert.AreEqual(0, task.DataSource.Count);
			task.Deactivate();
		}
	}
}
