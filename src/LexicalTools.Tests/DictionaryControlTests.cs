using System;
using System.Collections.Generic;
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
		private LexEntry _secondEntry;
		private Field _definitionField;

		[TestFixtureSetUp]
		public void SetupFixture()
		{
			WeSayWordsProject.InitializeForTests();
		}

		public override void Setup()
		{
			base.Setup();
			this._vernacularWsId = BasilProject.Project.WritingSystems.TestWritingSystemVernId;

			this._filePath = Path.GetTempFileName();
			this._recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _filePath);
			Db4oLexModelHelper.Initialize(((Db4oRecordListManager)_recordListManager).DataSource.Data);
			Lexicon.Init((Db4oRecordListManager)_recordListManager);

			this._records = this._recordListManager.GetListOfType<LexEntry>();

			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { this._vernacularWsId };
			ViewTemplate viewTemplate = new ViewTemplate();
			viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry",vernacularWritingSystemIds));
			viewTemplate.Add(new Field("MyEntryCustom", "LexEntry", new string[]{"en"}, Field.MultiplicityType.ZeroOr1, "MultiText" ));

			Field shy1 = new Field("MyShyEntryCustom", "LexEntry", new string[] { "en" }, Field.MultiplicityType.ZeroOr1, "MultiText");
			shy1.Visibility = WeSay.Foundation.CommonEnumerations.VisibilitySetting.NormallyHidden;
			shy1.DisplayName = "MyShyEntryCustom";
			viewTemplate.Add(shy1);

#if GlossMeaning
			viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), "LexSense", analysisWritingSystemIds));
#else
			_definitionField = new Field(LexSense.WellKnownProperties.Definition, "LexSense", analysisWritingSystemIds);
			viewTemplate.Add(_definitionField);
#endif
			viewTemplate.Add(new Field("MySenseCustom", "LexSense", new string[] { "en" }, Field.MultiplicityType.ZeroOr1, "MultiText"));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), "LexExampleSentence",vernacularWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), "LexExampleSentence",analysisWritingSystemIds));

			AddEntry("Initial", analysisWritingSystemIds[0], "meaning", true);
			_secondEntry = AddEntry("Secondary", analysisWritingSystemIds[0], "meaning", false);
			AddEntry("Tertiary", analysisWritingSystemIds[0], "meaning", true);


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
			this._tabControl.TabPages.Add(new TabPage("Dummy"));
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

		private LexEntry AddEntry(string lexemeForm, string meaningWritingSystemId, string meaning, bool includeExample)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative(this._vernacularWsId, lexemeForm);

			LexSense sense = (LexSense) entry.Senses.AddNew();
#if GlossMeaning
			sense.Gloss[
				WeSayWordsProject.Project.DefaultViewTemplate.GetField("SenseGloss").WritingSystemIds[0]] =
				meaning;
#else
			sense.Definition.SetAlternative(meaningWritingSystemId,meaning);
#endif
			if (includeExample)
			{
				LexExampleSentence ex = (LexExampleSentence) sense.ExampleSentences.AddNew();
				ex.Sentence.SetAlternative("x", "hello");
			}
			this._records.Add(entry);
			return entry;
		}

		public override void TearDown()
		{
			_window.Close();
			_window.Dispose();
			_window = null;
			_task.Deactivate();
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
			using (DictionaryControl e = new DictionaryControl(_recordListManager, new ViewTemplate()))
			{
				Assert.IsNotNull(e);
			}
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
		public void ClickingDeleteWordDecreasesRecordsByOne()
		{
			int before = _records.Count;
			ClickDeleteWord();
			Assert.AreEqual(before - 1, _records.Count);
		}

		/// <summary>
		/// regression test for WS-536, WS-545
		/// </summary>
		[Test]
		public void DeleteWordWhenEvenHasCleanup_Regression()
		{
			ClickAddWord();
			int before = _records.Count;

			EntryViewControl parentControl = ((DictionaryControl)_task.Control).Control_EntryDetailPanel;
			LexEntry entry = parentControl.DataSource;
			const string form = "xx";
			entry.LexicalForm.SetAlternative(_vernacularWsId, form);
			GoToLexicalEntryUseFind("Initial"); //go away
			GoToLexicalEntryUseFind(form);//come back

			KeyValuePair<string, object>  item2 = new KeyValuePair<string, object>("test", new LexRelation("b","bbb", entry));
			entry.Properties.Add(item2);

			GetEditControl("*EntryLexicalForm").FocusOnFirstWsAlternative();
			ClickDeleteWord();
			Assert.AreEqual(before - 1, _records.Count);
		   // GoToLexicalEntryUseFind(form); should fail to find it

			AssertExistenceOfEntryInList(form, false);
		}

		private static void GoToLexicalEntryUseFind(string lexemeForm)
		{
			TextBoxTester t = new TextBoxTester("_findText");
			t.Enter(lexemeForm);
			t.FireEvent("KeyDown", new KeyEventArgs(Keys.Enter));
			ListViewTester l = new ListViewTester("_recordsListBox");
			string label = GetSelectedLabel((WeSayListBox)l.Properties);
			Assert.AreEqual(lexemeForm, label);
		}

		private static string GetSelectedLabel(WeSayListBox box)
		{
			return box.Items[box.SelectedIndex].Text;
		}

		private static void AssertExistenceOfEntryInList(string form, bool shouldExist)
		{
			ListViewTester l = new ListViewTester("_recordsListBox");
			bool found = false;

			for (int i=0; i < l.Properties.Items.Count; ++i)
			{
				if (l.Properties.Items[i].ToString() == form)
				{
					found = true;
					break;
				}
			}
			Assert.AreEqual(shouldExist, found);
		}

		[Test]
		public void ClickingDeleteWordFocusesFirstField()
		{
			ClickDeleteWord();
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			Assert.IsTrue(t.Properties.Focused);
		}

		[Test]
		public void ClickingDeleteWordRefreshesDetailView()
		{
			ClickDeleteWord();
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			Assert.AreEqual(t.Text, LexemeFormOfSelectedEntry);
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
			ListViewTester t = new ListViewTester("_recordsListBox");
			((WeSayListBox)t.Properties).SelectedIndex = 2;
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
			ListViewTester t = new ListViewTester("_recordsListBox");
			Assert.AreEqual(0, ((WeSayListBox)t.Properties).SelectedIndex);
			string label = GetSelectedLabel((WeSayListBox)t.Properties);
			Assert.AreEqual("*", label);
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
			ListViewTester l = new ListViewTester("_recordsListBox");

			Assert.AreEqual(-1,((WeSayListBox)l.Properties).SelectedIndex);
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
			CustomTextFieldPreservedCore("*MyEntryCustom");
		}

		[Test]
		public void CustomTextFieldPreserved()
		{
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName());
			t.Enter("test");

			CustomTextFieldPreservedCore("*MyEntryCustom");
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
#if GlossMeaning
			Assert.AreEqual(0, ((LexSense)entry.Senses[0]).Properties.Count);
#else
			Assert.AreEqual(1, ((LexSense)entry.Senses[0]).Properties.Count);
			Assert.AreEqual(LexSense.WellKnownProperties.Definition, ((LexSense)entry.Senses[0]).Properties[0].Key);
#endif
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
#if GlossMeaning
			return Field.FieldNames.SenseGloss + "_" + BasilProject.Project.WritingSystems.TestWritingSystemAnalId;
#else
			return LexSense.WellKnownProperties.Definition + "_" + BasilProject.Project.WritingSystems.TestWritingSystemAnalId;
#endif
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
			ListViewTester l = new ListViewTester("_recordsListBox");

			string label = GetSelectedLabel((WeSayListBox)l.Properties);
			Assert.AreEqual("Secondary", label);
			RichTextBoxTester r = new RichTextBoxTester("_lexicalEntryPreview");
			Assert.IsTrue(r.Text.Contains("Secondary"));
		}

		[Test]
		public void FindText_Enter_Finds()
		{
			TextBoxTester t = new TextBoxTester("_findText");
			t.Enter("Secondary");
			t.FireEvent("KeyDown", new KeyEventArgs(Keys.Enter));
			ListViewTester l = new ListViewTester("_recordsListBox");

			string label = GetSelectedLabel((WeSayListBox)l.Properties);
			Assert.AreEqual("Secondary", label);
		}

		[Test]
		public void BaselineForRemovingSenseTests()
		{
			PutCursorInMeaningFieldOfSecondEntry();

			TypeInMeaning("samo");

			TextBoxTester tb2 = new TextBoxTester(GetMeaningControlName());
			Assert.AreEqual("samo", tb2.Properties.Text);
		}

		[Test]
		public void EditField_RemoveSenseContents_RemovesSense()
		{
			PutCursorInMeaningFieldOfSecondEntry();
			TypeInMeaning(string.Empty);
			Thread.Sleep(1000);
			Application.DoEvents();
			Assert.IsTrue(GetEditControl("Meaning").Name.Contains("ghost"), "Only ghost should remain");
		}

		private void DoUi()
		{
			while (true)

				Application.DoEvents();
		}


		[Test]  //regression test
		public void PastingBlankOverAMeaningOfEmptySenseDoesntCrash()
		{
			PutCursorInMeaningFieldOfSecondEntry();
			TextBoxTester tb = new TextBoxTester(GetMeaningControlName());
			Clipboard.SetText(" ");
			tb.Properties.Paste();
		}

		[Test]  //regression test
		public void PastingTextOverAMeaningOfEmptySenseDoesntJustChangesMeaning()
		{
			PutCursorInMeaningFieldOfSecondEntry();
			TextBoxTester tb = new TextBoxTester(GetMeaningControlName());
			Clipboard.SetText("samo");
			tb.Properties.Paste();
			TextBoxTester tb2 = new TextBoxTester(GetMeaningControlName());
			Assert.AreEqual("samo", tb2.Properties.Text);
		}


		private void PutCursorInMeaningFieldOfSecondEntry()
		{
//skip to second word (first has extra stuff in the sense)
			ListViewTester t = new ListViewTester("_recordsListBox");
			t.Properties.Focus();
			((WeSayListBox)t.Properties).SelectedIndex = 1;
			Assert.AreEqual("Secondary", LexemeFormOfSelectedEntry);
			Assert.AreEqual(1, GetCurrentEntry().Senses.Count, "this test assumes an entry with 1 sense");
			Assert.AreEqual(0, ((LexSense)(GetCurrentEntry().Senses[0])).ExampleSentences.Count, "this test assumes a sense w/ no example");
			MultiTextControl editControl = GetEditControl("Meaning 1");
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

		[Test]
		public void ClickingShowAllOnce_ShowsCustomShyGuyOnEntry()
		{
			ClickAddWord();
			Assert.IsNull(GetEditControl("MyShyEntryCustom"));
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton");
			btn.Click();
			Assert.IsNotNull(GetEditControl("MyShyEntryCustom"));
		}

		[Test]
		public void ClickingShowAllTwice_HidesCustomShyGuy()
		{
			ClickAddWord();
			Assert.IsNull(GetEditControl("MyShyEntryCustom"));
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton");
			btn.Click();
			Assert.IsNotNull(GetEditControl("MyShyEntryCustom"));
			btn.Click();
			Assert.IsNull(GetEditControl("MyShyEntryCustom"));
		}

		[Test]
		public void AddingNewWord_HidesCustomShyGuy()
		{
			ClickAddWord();
			Assert.IsNull(GetEditControl("MyShyEntryCustom"));
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton");
			Assert.IsTrue(btn.Text.Contains("Show"));
			btn.Click();
			Assert.IsTrue(btn.Text.Contains("Hide"));
			Assert.IsNotNull(GetEditControl("MyShyEntryCustom"));
			ClickAddWord();
			Assert.IsNull(GetEditControl("MyShyEntryCustom"));
			Assert.IsTrue(btn.Text.Contains("Show"));
		}

		[Test]
		public void AddingNewWord_ClearsShowHiddenState()
		{
			ClickAddWord();
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton");
			Assert.IsTrue(btn.Text.Contains("Show"));
			btn.Click();
			Assert.IsTrue(btn.Text.Contains("Hide"));
			ClickAddWord();
			Assert.IsTrue(btn.Text.Contains("Show"));
		}
		[Test]
		public void DeletingWord_ClearsShowHiddenState()
		{
			ClickAddWord();
			ClickAddWord();
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton");
			Assert.IsTrue(btn.Text.Contains("Show"));
			btn.Click();
			Assert.IsTrue(btn.Text.Contains("Hide"));
			ButtonTester delbtn = new ButtonTester("_btnDeleteWord");
			delbtn.Click();
			Assert.IsTrue(btn.Text.Contains("Show"));
		}

		[Test]
		public void GotoEntry_EntryInList_GoesToIt()
		{
			DictionaryControl control = (DictionaryControl) _task.Control;
			string idOfInitial = control.CurrentRecord.Id;
			GoToLexicalEntryUseFind("Secondary"); //go away from that one
			control.GoToEntry(idOfInitial);
			Assert.AreEqual(idOfInitial,control.CurrentRecord.Id);
		}


		[Test, ExpectedException(typeof(NavigationException))]
		public void GotoEntry_EntryNotInList_Throws()
		{
			DictionaryControl control = (DictionaryControl)_task.Control;
			control.GoToEntry("bogus");
		}
//
//        [Test]
//        public void GotoEntry_LackingFormInCurrentListWritingSystem()
//        {
//            LexEntry e = AddEntry("", "findme!", false);
//            e.LexicalForm.
//            DictionaryControl control = (DictionaryControl)_task.Control;
//            string idOfInitial = control.CurrentRecord.Id;
//            GoToLexicalEntryUseFind("Secondary"); //go away from that one
//            control.GoToEntry(idOfInitial);
//            Assert.AreEqual(idOfInitial, control.CurrentRecord.Id);
//        }

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