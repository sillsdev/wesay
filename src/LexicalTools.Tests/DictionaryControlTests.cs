using System.IO;
using System.Threading;
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
	public class DictionaryControlTests : NUnitFormTest
	{
		private DictionaryTask _task;
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
		}

		public override void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			base.Setup();
			WeSayWordsProject.InitializeForTests();
			this._vernacularWsId = BasilProject.Project.WritingSystems.TestWritingSystemVernId;

			this._filePath = Path.GetTempFileName();
			this._recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _filePath);
			Db4oLexModelHelper.Initialize(((Db4oRecordListManager)_recordListManager).DataSource.Data);

			this._records = this._recordListManager.GetListOfType<LexEntry>();
			AddEntry("Initial", "meaning", true);
			AddEntry("Secondary", "meaning", false);
			AddEntry("Tertiary", "meaning", true);

			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { this._vernacularWsId };
			ViewTemplate viewTemplate = new ViewTemplate();
			viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry",vernacularWritingSystemIds));
			viewTemplate.Add(new Field("EntryNote", "LexEntry", new string[]{"en"}, Field.MultiplicityType.ZeroOr1, "MultiText" ));
			viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), "LexSense",analysisWritingSystemIds));
			viewTemplate.Add(new Field("SenseNote", "LexSense", new string[]{"en"}, Field.MultiplicityType.ZeroOr1, "MultiText" ));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), "LexExampleSentence",vernacularWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), "LexExampleSentence",analysisWritingSystemIds));

			Field customField = new Field("SemanticDomains", "LexSense", analysisWritingSystemIds, Field.MultiplicityType.ZeroOr1, "OptionCollection");
			customField.DisplayName = "Sem Dom";
			customField.OptionsListFile = "SemanticDomains.xml";
			viewTemplate.Add(customField);

			Field customPOSField = new Field(LexSense.WellKnownProperties.PartOfSpeech, "LexSense", analysisWritingSystemIds, Field.MultiplicityType.ZeroOr1, "Option");
			customPOSField.DisplayName = "POS";
			customPOSField.OptionsListFile = "PartsOfSpeech.xml";
			viewTemplate.Add(customPOSField);

			Field customNotesField = new Field(LexSense.WellKnownProperties.Note, "LexSense", analysisWritingSystemIds);
			customNotesField.DisplayName = "s-note";
			viewTemplate.Add(customNotesField);

			Field exampleNotesField = new Field(LexSense.WellKnownProperties.Note, "LexExampleSentence", analysisWritingSystemIds);
			exampleNotesField.DisplayName = "ex-note";
			viewTemplate.Add(exampleNotesField);

			this._task = new DictionaryTask(_recordListManager, viewTemplate);
			this._detailTaskPage = new TabPage();
			ActivateTask();

			this._tabControl = new TabControl();

			this._tabControl.Dock = DockStyle.Fill;
			this._tabControl.TabPages.Add(this._detailTaskPage);
			this._tabControl.TabPages.Add("Dummy");
			this._window = new Form();
			this._window.Controls.Add(this._tabControl);
			_window.Width = 500;
			_window.Height = 500;
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

		private void AddEntry(string lexemeForm, string meaning, bool includeExample)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative(this._vernacularWsId, lexemeForm);

			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss[
				WeSayWordsProject.Project.ViewTemplate.GetField("SenseGloss").WritingSystemIds[0]] =
				meaning;

			if (includeExample)
			{
				LexExampleSentence ex = (LexExampleSentence) sense.ExampleSentences.AddNew();
				ex.Sentence.SetAlternative("x", "hello");
			}
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
			File.Delete(_filePath);
			base.TearDown();
		}

		[Test]
		public void Construct_EmptyViewTemplate_NoCrash()
		{
			DictionaryControl e = new DictionaryControl(_recordListManager, new ViewTemplate());
			Assert.IsNotNull(e);
		}

		[Test]
		public void ClickingAddWordFocusesFirstField()
		{
			ClickAddWord();
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
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
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
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

		static private void DeleteAllEntries()
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
			ButtonTester l = new ButtonTester("_btnDeleteWord");
			Assert.IsFalse(l.Properties.Enabled);
		}

		[Test]
		public void EmptyDictionary_EnterText_PressFindButton_NoCrash()
		{
			StartWithEmpty();
			TextBoxTester t = new TextBoxTester("_findText");
			t.Enter("blah");
			ButtonTester b = new ButtonTester("_btnFind");
			b.Click();
			ListBoxTester l = new ListBoxTester("_recordsListBox");

			Assert.AreEqual(-1,l.Properties.SelectedIndex);
		}


		[Test]
		public void IfNoWordsDeleteButtonDisabled()
		{
			ButtonTester l = new ButtonTester("_btnDeleteWord");
			Assert.IsTrue(l.Properties.Enabled);
			DeleteAllEntries();
			Assert.IsFalse(l.Properties.Enabled);
		}

		[Test]
		public void CustomTextFieldPreservedNoOtherEditing()
		{
			CustomTextFieldPreservedCore("*EntryNote");
		}

		[Test]
		public void CustomTextFieldPreserved()
		{
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			t.Enter("test");

			CustomTextFieldPreservedCore("*EntryNote");
		}

		private void CustomTextFieldPreservedCore(string fieldLabel)
		{
			MultiTextControl note= GetEditControl(fieldLabel);
			WeSayTextBox box = note.TextBoxes[0];
			box.Focus();
			box.Text = "a note";
			LexEntry entry = GetCurrentEntry();
			Assert.AreEqual(1, GetCurrentEntry().Properties.Count);

			CycleTheCurrentEntryOutAndBackIn(entry);

			note = GetEditControl(fieldLabel);
			Assert.AreEqual("a note", note.TextBoxes[0].Text);
		}

		private void CustomOptionRefPreservedCore(string fieldLabel)
		{
			SingleOptionControl combo = GetOptionControl(fieldLabel);

			combo.Value = "verb";

			LexEntry entry = GetCurrentEntry();
			Assert.AreEqual(1, GetCurrentEntry().Properties.Count);

			CycleTheCurrentEntryOutAndBackIn(entry);

			combo = GetOptionControl(fieldLabel);
			Assert.AreEqual("verb", combo.Value);
		}

		[Test]
		public void CustomOptionRefOnSensePreserved()
		{
			CustomOptionRefPreservedCore("POS");
		}

		[Test]
		public void CustomTextFieldOnSensePreserved()
		{
			CustomTextFieldPreservedCore("s-note");
		}

		private void CycleTheCurrentEntryOutAndBackIn(LexEntry entry)
		{
//cycle out this record
			EntryViewControl parentControl = ((DictionaryControl) _task.Control).Control_EntryDetailPanel;
			parentControl.DataSource = new LexEntry();
			//bring this record back in
			parentControl.DataSource = entry;
		}

		private LexEntry GetCurrentEntry()
		{
			EntryViewControl parentControl = ((DictionaryControl) _task.Control).Control_EntryDetailPanel;
			return parentControl.DataSource;
		}

		[Test]
		public void EmptyProperitesRemovedAfterEditting()
		{

			EntryViewControl parentControl = ((DictionaryControl)_task.Control).Control_EntryDetailPanel;
			LexEntry entry = parentControl.DataSource;
			Assert.Less(0, entry.Properties.Count, "the setup of this test should have some custom properties");
		   TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			t.Enter("test");
			Assert.Less(0, entry.Properties.Count, "the setup of this test should have some custom properties");

			//cycle out this record
			parentControl.DataSource = new LexEntry();

			Assert.AreEqual(0, entry.Properties.Count);
			Assert.AreEqual(0, ((LexSense)entry.Senses[0]).Properties.Count);
			Assert.AreEqual(0, ((LexSense)entry.Senses[0]).Properties.Count);

		}



		[Test]
		public void ClickingTheStarButton_AfterTyping_SetsAnnotation()
		{
			TypeInLexicalForm("one");
			ClickStarOfLexemeForm();
			Assert.IsTrue(this._records[0].LexicalForm.GetAnnotationOfAlternativeIsStarred(_vernacularWsId));
		}

		[Test]
		public void ClickingTheStarButton_WithEmptySenseBelow_RegressionTest()
		{
	  //      this._records[0].Senses.AddNew();
   //         this._records[0].NotifyPropertyChanged("senses");
		   // Application.DoEvents();
			ClickStarOfLexemeForm();
			Assert.IsTrue(this._records[0].LexicalForm.GetAnnotationOfAlternativeIsStarred(_vernacularWsId));
		}

		private void ClickStarOfLexemeForm()
		{
			ControlTester t = new ControlTester(GetNameOfLexicalFormAnnotationControl());
			t.Click();
			GetEditControl(Field.FieldNames.EntryLexicalForm.ToString());
		}

		private static string GetNameOfLexicalFormAnnotationControl()
		{
			return GetLexicalFormControlName()+"-annotationWidget";
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

			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
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
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			Assert.AreEqual(value, t.Properties.Text);
		}

		private static string GetLexicalFormControlName()
		{
			return Field.FieldNames.EntryLexicalForm + "_" + BasilProject.Project.WritingSystems.TestWritingSystemVernId;
		}

		private static void TypeInLexicalForm(string value)
		{
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			t.Properties.Text = value;
		}

		private static string GetMeaningControlName()
		{
			return Field.FieldNames.SenseGloss + "_" + BasilProject.Project.WritingSystems.TestWritingSystemAnalId;
		}

		private static void TypeInMeaning(string value)
		{
			TextBoxTester t = new TextBoxTester(GetMeaningControlName());
			t.Properties.Text = value;
		}

		private string LexemeFormOfSelectedEntry
		{
			get
			{
				return ((DictionaryControl)_detailTaskPage.Controls[0]).CurrentRecord.LexicalForm.GetBestAlternative(_vernacularWsId);
			}
		}

		private static void ClickAddWord()
		{
			ButtonTester l = new ButtonTester("_btnNewWord");
			l.Click();
		}

		static private void ClickDeleteWord()
		{
			ButtonTester l = new ButtonTester("_btnDeleteWord");
			l.Click();
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
		public void BaselineForRemovingSenseTests()
		{
			RemovingSenseTestCore();
			TypeInMeaning("samo");
			TextBoxTester tb2 = new TextBoxTester(GetMeaningControlName());
			Assert.AreEqual("samo", tb2.Properties.Text);
		}

		[Test]
		public void EditField_RemoveSenseContents_RemovesSense()
		{
			RemovingSenseTestCore();
			TypeInMeaning(string.Empty);
			Thread.Sleep(1000);
			Application.DoEvents();
			Assert.IsTrue(GetEditControl("Meaning").Name.Contains("ghost"), "Only ghost should remain");
		}

		[Test]  //regression test
		public void PastingBlankOverAMeaningOfEmptySenseDoesntCrash()
		{
			RemovingSenseTestCore();
			TextBoxTester tb = new TextBoxTester(GetMeaningControlName());
			tb.Properties.Paste(" ");
		}

		[Test]  //regression test
		public void PastingTextOverAMeaningOfEmptySenseDoesntJustChangesMeaning()
		{
			RemovingSenseTestCore();
			TextBoxTester tb = new TextBoxTester(GetMeaningControlName());
			tb.Properties.Paste("samo");
			TextBoxTester tb2 = new TextBoxTester(GetMeaningControlName());
			Assert.AreEqual("samo", tb2.Properties.Text);
		}


		private void RemovingSenseTestCore()
		{
//skip to second word (first has extra stuff in the sense)
			ListBoxTester t = new ListBoxTester("_recordsListBox");
			t.Properties.Focus();
			t.Properties.SelectedIndex = 1;
			Assert.AreEqual("Secondary", LexemeFormOfSelectedEntry);
			Assert.AreEqual(1, GetCurrentEntry().Senses.Count, "this test assumes an entry with 1 sense");
			Assert.AreEqual(0, ((LexSense)(GetCurrentEntry().Senses[0])).ExampleSentences.Count, "this test assumes a sense w/ no example");
			MultiTextControl editControl = GetEditControl("Meaning");
			editControl.TextBoxes[0].Focus();
		}

		/// <summary>
		/// Actually, I never was able to get this test to fail. Sigh.
		/// </summary>
		[Test]
		public void GhostField_Trigger_RegressionTest()
		{
			ClickAddWord();

			DetailList detailList = GetDetailList();
			int initialCount = detailList.Count;

			MultiTextControl editControl = GetEditControl("Meaning");
			Assert.IsTrue(GetEditControl("Meaning").Name.Contains("ghost"));
			 editControl.TextBoxes[0].Focus();
			 TextBoxTester t = new TextBoxTester(editControl.TextBoxes[0].Name);
		  //didn''t work  t.FireEvent("KeyPress", new KeyPressEventArgs('a'));
		   t.Properties.Text = "foo";
//            Application.DoEvents();
//            t.FireEvent("KeyDown", new KeyEventArgs(Keys.Tab));
//            Application.DoEvents();
//            t.FireEvent("KeyUp", new KeyEventArgs(Keys.Tab));
//            Application.DoEvents();
			TextBoxTester lxt = new TextBoxTester(GetLexicalFormControlName());
			lxt.Properties.Focus();
			//ghost really did fire
			Assert.IsTrue(detailList.Count > initialCount);

			//now do another one
			initialCount = detailList.Count;
			MultiTextControl editControl2 = (MultiTextControl) GetEditControl("Meaning",true);
			Assert.IsTrue(editControl2.Name.Contains("ghost"));
			editControl2.TextBoxes[0].Focus();
			Application.DoEvents();
			TextBoxTester t2 = new TextBoxTester(editControl2.TextBoxes[0].Name);
			t2.Properties.Text = "bar";
			Application.DoEvents();
			lxt.Properties.Focus();
			Application.DoEvents();
			Assert.IsTrue(detailList.Count > initialCount);
		}

		private DetailList GetDetailList()
		{
			return ((DictionaryControl)_task.Control).Control_EntryDetailPanel.ControlEntryDetail;
		}

		private SingleOptionControl GetOptionControl(string labelText)
		{
			return (SingleOptionControl )GetEditControl(labelText, false);
		}
		private MultiTextControl GetEditControl(string labelText)
		{
			return (MultiTextControl)GetEditControl(labelText, false);
		}

		private Control GetEditControl(string labelText, bool lookingForGhostVersion)
		{
			DetailList detailList = ((DictionaryControl)_task.Control).Control_EntryDetailPanel.ControlEntryDetail;
			Control foundControl = null;
			for (int i = 0; i < detailList.Count; i++)
			{
				Label label = detailList.GetLabelControlFromRow(i);
				if (label.Text == labelText)
				{
					foundControl = detailList.GetEditControlFromRow(i);
					if (lookingForGhostVersion)
					{
						if (!foundControl.Name.Contains("ghost"))
						{
							foundControl = null;
							continue;
						}
					}
					break;
				}
			}
			return foundControl;
		}

	}

}