using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Extensions.Forms;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class EntryDetailControlTests : NUnit.Extensions.Forms.NUnitFormTest
	{
		private EntryDetailTask _task;
		IRecordListManager _recordListManager;
		string _filePath;
		private IRecordList<LexEntry> _records;
		private string _vernacularWsId;
		private TabControl _tabControl;
		private Form _window;
		private TabPage _detailTaskPage;

		[TestFixtureSetUp]
		public void SetupFixture()
		{
			WeSayWordsProject.InitializeForTests();
			File.Copy(Path.Combine(WeSayWordsProject.Project.ApplicationTestDirectory, "tasks.xml"), WeSayWordsProject.Project.PathToProjectTaskInventory, true);
		}

		public override void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			base.Setup();
			//            Db4oLexModelHelper.InitializeForNonDbTests();
			WeSayWordsProject.InitializeForTests();
			this._vernacularWsId = BasilProject.Project.WritingSystems.TestWritingSystemVernId;

			this._filePath = System.IO.Path.GetTempFileName();
			this._recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _filePath);
			Db4oLexModelHelper.Initialize(((Db4oRecordListManager)_recordListManager).DataSource.Data);

			this._records = this._recordListManager.GetListOfType<LexEntry>();
			AddEntry("Initial", "meaning");
			AddEntry("Secondary", string.Empty);
			AddEntry("Tertiary", string.Empty);

			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { this._vernacularWsId };
			ViewTemplate viewTemplate = new ViewTemplate();
			viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), vernacularWritingSystemIds));

			viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), analysisWritingSystemIds));

			Field customField;

			viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), vernacularWritingSystemIds));

			viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), analysisWritingSystemIds));

			customField = new Field("SemanticDomains", analysisWritingSystemIds, Field.MultiplicityType.ZeroOr1, "OptionCollection");
			customField.ClassName = "LexSense";
			customField.DisplayName = "Sem Dom";
			customField.OptionsListFile = "SemanticDomains.xml";
			viewTemplate.Add(customField);

			this._task = new EntryDetailTask(_recordListManager, viewTemplate);
			this._detailTaskPage = new TabPage();
			ActivateTask();

			this._tabControl = new TabControl();

			this._tabControl.Dock = DockStyle.Fill;
			this._tabControl.TabPages.Add(this._detailTaskPage);
			this._tabControl.TabPages.Add("Dummy");
			this._window = new Form();
			this._window.Controls.Add(this._tabControl);
			this._window.Show();
		}


		/// <summary>
		/// DOESN'T WORK (this version of nunit forms seems broken in this respect)
		/// </summary>
		public override bool UseHidden
		{
			get
			{
				return true;
			}
		}

		private void AddEntry(string lexemeForm, string meaning)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative(this._vernacularWsId, lexemeForm);

			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss[WeSay.Project.WeSayWordsProject.Project.ViewTemplate.GetField("SenseGloss").WritingSystemIds[0]] = meaning;

			this._records.Add(entry);
		}

		public override void TearDown()
		{
			_window.Close();
			_window.Dispose();
			_window = null;
			this._recordListManager.Dispose();
			_recordListManager = null;
			_records.Dispose();
			_records = null;
			System.IO.File.Delete(_filePath);
			base.TearDown();
		}

		[Test]
		public void Construct_EmptyViewTemplate_NoCrash()
		{
			EntryDetailControl e = new EntryDetailControl(_recordListManager, new ViewTemplate());
			Assert.IsNotNull(e);
		}

		[Test]
		public void ClickingAddWordFocusesFirstField()
		{
			ClickAddWord();
			NUnit.Extensions.Forms.TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			Assert.IsTrue(t.Properties.Focused);
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
			LexicalFormMustMatch("Initial");
			ClickAddWord();
			LexicalFormMustMatch("");
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
			Assert.AreEqual(before - 1, _records.Count);
		}

		[Test]
		public void ClickingDeleteWordFocusesFirstField()
		{
			ClickDeleteWord();
			NUnit.Extensions.Forms.TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			Assert.IsTrue(t.Properties.Focused);
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
			ListBoxTester t = new ListBoxTester("_recordsListBox");
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
			DeleteAllEntries();
			ClickAddWord();
		}

		private void DeleteAllEntries()
		{
			ClickDeleteWord();
			ClickDeleteWord();
			ClickDeleteWord();
		}

		private void StartWithEmpty()
		{
			DeleteAllEntries();
			this._task.Deactivate();
			ActivateTask();
		}
		[Test]
		public void EmptyDictionary_AddWords_NewWordSelectedInListBox()
		{
			DeleteAllEntries();
			ClickAddWord();
			ListBoxTester t = new ListBoxTester("_recordsListBox");
			Assert.AreEqual(0, t.Properties.SelectedIndex);
			Assert.AreEqual("*", t.Properties.SelectedItem);
		}

		[Test]
		public void EmptyDictionary_AddWords_FieldsExist()
		{
			DeleteAllEntries();
			ClickAddWord();
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			Assert.IsTrue(t.Properties.Visible);
		}

		[Test]
		public void EmptyDictionary_AddWords_CanTypeInFirstField()
		{
			DeleteAllEntries();
			ClickAddWord();
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			t.Enter("test");
			Assert.AreEqual("test", t.Text);
		}

		[Test]
		public void EmptyDictionary_DeleteButtonDisabled()
		{
			StartWithEmpty();
			NUnit.Extensions.Forms.LinkLabelTester l = new LinkLabelTester("_btnDeleteWord");
			Assert.IsFalse(l.Properties.Enabled);
		}

		[Test]
		public void IfNoWordsDeleteButtonDisabled()
		{
			NUnit.Extensions.Forms.LinkLabelTester l = new LinkLabelTester("_btnDeleteWord");
			Assert.IsTrue(l.Properties.Enabled);
			DeleteAllEntries();
			Assert.IsFalse(l.Properties.Enabled);
		}

		[Test]
		public void SwitchingToAnotherTaskDoesNotLooseBindings()
		{
			LexicalFormMustMatch("Initial");
			TypeInLexicalForm("one");
			this._task.Deactivate();
			_tabControl.SelectedIndex = 1;
			_tabControl.SelectedIndex = 0;
			ActivateTask();

			NUnit.Extensions.Forms.TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			t.Properties.Visible = true;

			LexicalFormMustMatch("one");

			TypeInLexicalForm("plus"); // need something that still sorts higher than Secondary and Tertiary
			this._task.Deactivate();
			_tabControl.SelectedIndex = 1;
			_tabControl.SelectedIndex = 0;
			ActivateTask();
			t = new TextBoxTester(GetLexicalFormControlName());
			t.Properties.Visible = true;
			LexicalFormMustMatch("plus");
		}

		private void ActivateTask()
		{
			this._task.Activate();
			this._task.Control.Dock = DockStyle.Fill;
			this._detailTaskPage.Controls.Clear();
			this._detailTaskPage.Controls.Add(this._task.Control);
		}

		private static void LexicalFormMustMatch(string value)
		{
			NUnit.Extensions.Forms.TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			Assert.AreEqual(value, t.Properties.Text);
		}

		private static string GetLexicalFormControlName()
		{
			return Field.FieldNames.EntryLexicalForm.ToString() + "_" + BasilProject.Project.WritingSystems.TestWritingSystemVernId;
		}

		private static void TypeInLexicalForm(string value)
		{
			NUnit.Extensions.Forms.TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			t.Properties.Text = value;
		}

		private static string GetMeaningControlName()
		{
			return Field.FieldNames.SenseGloss.ToString() + "_" + BasilProject.Project.WritingSystems.TestWritingSystemAnalId;
		}

		private static void TypeInMeaning(string value)
		{
			NUnit.Extensions.Forms.TextBoxTester t = new TextBoxTester(GetMeaningControlName());
			t.Properties.Text = value;
		}

		private string LexemeFormOfSelectedEntry
		{
			get
			{
				BindingListGridTester t = new BindingListGridTester("_recordsListBox");
				return ((EntryDetailControl)_detailTaskPage.Controls[0]).CurrentRecord.LexicalForm.GetAlternative(_vernacularWsId);
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

		[Test]
		public void FindTextChanged_ButtonSaysFind()
		{
			TextBoxTester t = new TextBoxTester("_findText");
			t.Enter("changed");
			ButtonTester b = new ButtonTester("_btnFind");
			Assert.AreEqual("Find", b.Text);
		}


		[Test]
		public void EnterText_PressFindButton_Finds()
		{
			TextBoxTester t = new TextBoxTester("_findText");
			t.Enter("Secondary");
			ButtonTester b = new ButtonTester("_btnFind");
			b.Click();
			ListBoxTester l = new ListBoxTester("_recordsListBox");

			Assert.AreEqual("Secondary", l.Properties.SelectedItem);
			RichTextBoxTester r = new RichTextBoxTester("_lexicalEntryPreview");
			Assert.IsTrue(r.Text.Contains("Secondary"));
		}

		[Test]
		public void FindText_Enter_Finds()
		{
			TextBoxTester t = new TextBoxTester("_findText");
			t.Enter("Secondary");
			t.FireEvent("KeyDown", new KeyEventArgs(Keys.Enter));
			ListBoxTester l = new ListBoxTester("_recordsListBox");

			Assert.AreEqual("Secondary", l.Properties.SelectedItem);
		}

		[Test]
		public void EditField_RemoveSenseContents_RemovesSense()
		{
			DetailList detailList = ((EntryDetailControl)_task.Control).Control_EntryDetailPanel.ControlEntryDetail;

			MultiTextControl editControl = GetEditControl(detailList, "Meaning");
			editControl.TextBoxes[0].Focus();
			TypeInMeaning(string.Empty);

			Assert.IsTrue(GetEditControl(detailList, "Meaning").Name.Contains("ghost"), "Only ghost should remain");
		}


		private static MultiTextControl GetEditControl(DetailList detailList, string labelText)
		{
			MultiTextControl editControl = null;
			for (int i = 0; i < detailList.Count; i++)
			{
				Control referenceControl = detailList.GetControlOfRow(i);
				Label label = detailList.GetLabelControlFromReferenceControl(referenceControl);
				if (label.Text == labelText)
				{
					editControl = (MultiTextControl)detailList.GetEditControlFromReferenceControl(referenceControl);
					break;
				}
			}
			return editControl;
		}

	}

}