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

			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative(this._vernacularWsId, "Initial");
			this._records = this._recordListManager.Get<LexEntry>();
			this._records.Add(entry);

			this._control = new EntryDetailTask(_recordListManager);
			this._control.Dock = DockStyle.Fill;
			Form f = new Form();
			f.Controls.Add(this._control);
			f.Show();
			_control.Activate();
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
	}
}
