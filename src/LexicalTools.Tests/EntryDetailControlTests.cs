using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Extensions.Forms;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class EntryDetailControlTests : NUnit.Extensions.Forms.NUnitFormTest
	{
	   // private FormTester _mainWindowTester;
		private EntryDetailTask _control;
		IRecordListManager _recordListManager;
		string _filePath;
		private IRecordList<LexEntry> _records;
		private string _vernacularWsId;

		public override void Setup()
		{
			base.Setup();

			BasilProject.InitializeForTests();
			this._vernacularWsId = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Id;

			this._filePath = System.IO.Path.GetTempFileName();
			this._recordListManager = new Db4oRecordListManager(_filePath);

			this._records = this._recordListManager.Get<LexEntry>();
			AddEntry("Initial");
			AddEntry("Secondary");
			AddEntry("Tertiary");

			this._control = new EntryDetailTask(_recordListManager);
			this._control.Dock = DockStyle.Fill;
			Form f = new Form();
			f.Controls.Add(this._control);
			f.Show();
			_control.Activate();
		}

		private void AddEntry(string lexemeForm)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative(this._vernacularWsId, lexemeForm);
			this._records.Add(entry);
		}

		public override void TearDown()
		{
			base.TearDown();
			this._recordListManager.Dispose();
			System.IO.File.Delete(_filePath);
		}

		[Test]
		public void ClickingAddWordIncreasesRecordsByOne()
		{
			int before = _records.Count;
			ClickAddWord();
			Assert.AreEqual(1 + before, _records.Count);
		}

		[Test]
		public void ClickingAddWordShowsEmptyRecord()
		{
			NUnit.Extensions.Forms.TextBoxTester t = new TextBoxTester("LexicalForm");
			Assert.AreEqual("Initial", t.Properties.Text);

			ClickAddWord();
			Assert.AreEqual("", t.Properties.Text);
		}

		[Test]
		public void ClickingAddWordSelectsNewWordInList()
		{
			Assert.AreEqual("Initial", LexemeFormOfSelectedEntry);
			ClickAddWord();
			Assert.AreEqual("", LexemeFormOfSelectedEntry);
		}

		[Test]
		public void ClickingDeleteWordDereasesRecordsByOne()
		{
			int before = _records.Count;
			ClickDeleteWord();
			Assert.AreEqual(before-1, _records.Count);
		}



		[Test]
		public void DeletingFirstWordSelectsNextWordInList()
		{
			Assert.AreEqual("Initial", LexemeFormOfSelectedEntry);
			ClickDeleteWord();
			Assert.AreEqual("Secondary", LexemeFormOfSelectedEntry);
		}

		[Test]
		public void DeletingLastWordSelectsPreviousWordInList()
		{
			BindingListGridTester t = new BindingListGridTester("_recordsListBox");
			t.Properties.SelectedIndex = 2;
			Assert.AreEqual("Tertiary", LexemeFormOfSelectedEntry);
			ClickDeleteWord();
			Assert.AreEqual("Secondary", LexemeFormOfSelectedEntry);
		}

		[Test]
		public void AddWordsThenDeleteDoesNotCrash()
		{
			ClickAddWord();
			ClickDeleteWord();
		}

		[Test]
		public void DeletingAllWordsThenAddingDoesNotCrash()
		{
			ClickDeleteWord();
			ClickDeleteWord();
			ClickAddWord();
		}

		[Test]
		public void IfNoWordsDeleteButtonDisabled()
		{
			NUnit.Extensions.Forms.LinkLabelTester l = new LinkLabelTester("_btnDeleteWord");
			Assert.IsTrue(l.Properties.Enabled);
			ClickDeleteWord();
			ClickDeleteWord();
			ClickDeleteWord();
			Assert.IsFalse(l.Properties.Enabled);
		}

		private string LexemeFormOfSelectedEntry
		{
			get
			{
				BindingListGridTester t = new BindingListGridTester("_recordsListBox");
				return ((LexEntry)t.Properties.SelectedObject).LexicalForm.GetAlternative(_vernacularWsId);
			}
		}

		private static void ClickAddWord()
		{
			NUnit.Extensions.Forms.LinkLabelTester l = new LinkLabelTester("_btnNewWord");
			l.Click();
		}

		private void ClickDeleteWord()
		{
			NUnit.Extensions.Forms.LinkLabelTester l = new LinkLabelTester("_btnDeleteWord");
			l.Click();
		}
	}
}
