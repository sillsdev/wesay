using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Tests.TestHelpers;
using WeSay.LexicalModel;
using WeSay.LexicalTools.DictionaryBrowseAndEdit;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class DictionaryControlTests: NUnitFormTest
	{
		private TemporaryFolder _tempFolder;
		private DictionaryTask _task;
		private LexEntryRepository _lexEntryRepository;
		private string _filePath;
		private WritingSystem _vernacularWritingSystem;
		private TabControl _tabControl;
		private Form _window;
		private TabPage _detailTaskPage;
		private Field _definitionField;
		private Guid _firstEntryGuid;
		private Guid _secondEntryGuid;

		[TestFixtureSetUp]
		public void SetupFixture()
		{
			WeSayWordsProject.InitializeForTests();
		}

		public override void Setup()
		{
			base.Setup();
			_tempFolder = new TemporaryFolder();
			_vernacularWritingSystem =
					new WritingSystem(BasilProject.Project.WritingSystems.TestWritingSystemVernId,
									  SystemFonts.DefaultFont);
			RtfRenderer.HeadWordWritingSystemId = _vernacularWritingSystem.Id;

			_filePath = _tempFolder.GetTemporaryFile();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			string[] analysisWritingSystemIds = new string[]
													{
															BasilProject.Project.WritingSystems.
																	TestWritingSystemAnalId
													};
			string[] vernacularWritingSystemIds = new string[] {_vernacularWritingSystem.Id};
			ViewTemplate viewTemplate = new ViewTemplate();
			viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(),
									   "LexEntry",
									   vernacularWritingSystemIds));
			viewTemplate.Add(new Field("focusOnMe",
									   "LexEntry",
									   analysisWritingSystemIds,
									   Field.MultiplicityType.ZeroOr1,
									   "MultiText"));
			viewTemplate.Add(new Field("MyEntryCustom",
									   "LexEntry",
									   analysisWritingSystemIds,
									   Field.MultiplicityType.ZeroOr1,
									   "MultiText"));

			Field readOnlySemanticDomain =
					new Field(LexSense.WellKnownProperties.SemanticDomainsDdp4,
							  "LexSense",
							  analysisWritingSystemIds);
			readOnlySemanticDomain.Visibility = CommonEnumerations.VisibilitySetting.ReadOnly;
			viewTemplate.Add(readOnlySemanticDomain);

			Field shy1 = new Field("MyShyEntryCustom",
								   "LexEntry",
								   analysisWritingSystemIds,
								   Field.MultiplicityType.ZeroOr1,
								   "MultiText");
			shy1.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			shy1.DisplayName = "MyShyEntryCustom";
			viewTemplate.Add(shy1);

#if GlossMeaning
			viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), "LexSense", analysisWritingSystemIds));
#else
			_definitionField = new Field(LexSense.WellKnownProperties.Definition,
										 "LexSense",
										 analysisWritingSystemIds);
			viewTemplate.Add(_definitionField);
#endif
			viewTemplate.Add(new Field("MySenseCustom",
									   "LexSense",
									   analysisWritingSystemIds,
									   Field.MultiplicityType.ZeroOr1,
									   "MultiText"));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(),
									   "LexExampleSentence",
									   vernacularWritingSystemIds));
			viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(),
									   "LexExampleSentence",
									   analysisWritingSystemIds));

			_firstEntryGuid = AddEntry("Initial", analysisWritingSystemIds[0], "meaning", true);
			_secondEntryGuid = AddEntry("Secondary", analysisWritingSystemIds[0], "secondarymeaning", false);
			AddEntry("Tertiary", analysisWritingSystemIds[0], "meaning", true);

			Field customField = new Field("SemanticDomains",
										  "LexSense",
										  analysisWritingSystemIds,
										  Field.MultiplicityType.ZeroOr1,
										  "OptionCollection");
			customField.DisplayName = "Sem Dom";
			customField.OptionsListFile = "SemanticDomains.xml";
			viewTemplate.Add(customField);

			Field customPOSField = new Field(LexSense.WellKnownProperties.PartOfSpeech,
											 "LexSense",
											 analysisWritingSystemIds,
											 Field.MultiplicityType.ZeroOr1,
											 "Option");
			customPOSField.DisplayName = "POS";
			customPOSField.OptionsListFile = "PartsOfSpeech.xml";
			viewTemplate.Add(customPOSField);

			Field customNotesField = new Field(WeSayDataObject.WellKnownProperties.Note,
											   "LexSense",
											   analysisWritingSystemIds);
			customNotesField.DisplayName = "s-note";
			viewTemplate.Add(customNotesField);

			Field exampleNotesField = new Field(WeSayDataObject.WellKnownProperties.Note,
												"LexExampleSentence",
												analysisWritingSystemIds);
			exampleNotesField.DisplayName = "ex-note";
			viewTemplate.Add(exampleNotesField);

			_task = new DictionaryTask(new DictionaryBrowseAndEditConfiguration(), _lexEntryRepository, viewTemplate);//, new UserSettingsForTask());
			_detailTaskPage = new TabPage();
			ActivateTask();

			_tabControl = new TabControl();

			_tabControl.Dock = DockStyle.Fill;
			_tabControl.TabPages.Add(_detailTaskPage);
			_tabControl.TabPages.Add(new TabPage("Dummy"));
			_window = new Form();
			_window.Controls.Add(_tabControl);
			_window.Width = 700;
			_window.Height = 500;
			_window.Show();
		}

		/// <summary>
		/// DOESN'T WORK (this version of nunit forms seems broken in this respect)
		/// </summary>
		public override bool UseHidden
		{
			get { return true; }
		}

		private Guid AddEntry(string lexemeForm,
							  string meaningWritingSystemId,
							  string meaning,
							  bool includeExample)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(_vernacularWritingSystem.Id, lexemeForm);

			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
#if GlossMeaning
			sense.Gloss[
				WeSayWordsProject.Project.DefaultViewTemplate.GetField("SenseGloss").WritingSystemIds[0]] =
				meaning;
#else
			sense.Definition.SetAlternative(meaningWritingSystemId, meaning);
#endif
			if (includeExample)
			{
				LexExampleSentence ex = new LexExampleSentence();
				sense.ExampleSentences.Add(ex);
				ex.Sentence.SetAlternative("x", "hello");
			}
			_lexEntryRepository.SaveItem(entry);
			return entry.Guid;
		}

		//        [Test]
		//        public void TabKeySkipsReadOnlyField()
		//        {
		//            while (true)
		//            {
		//                Application.DoEvents();
		//            }
		//        }

		public override void TearDown()
		{
			_window.Close();
			_window.Dispose();
			_window = null;
			_task.Deactivate();
			_lexEntryRepository.Dispose();
			_lexEntryRepository = null;
			_tempFolder.Delete();
			base.TearDown();
		}

		[Test]
		public void Construct_EmptyViewTemplate_NoCrash()
		{
			using (
					DictionaryControl e = new DictionaryControl(_lexEntryRepository,
																new ViewTemplate()))
			{
				Assert.IsNotNull(e);
			}
		}

		[Test]
		public void ClickingAddWordFocusesFirstField()
		{
			ClickAddWord();
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
			Assert.IsTrue(t.Properties.Focused);
		}

		[Test]
		public void ClickingAddWordIncreasesRecordsByOne()
		{
			int before = _lexEntryRepository.CountAllItems();
			ClickAddWord();
			Assert.AreEqual(1 + before, _lexEntryRepository.CountAllItems());
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
			int before = _lexEntryRepository.CountAllItems();
			ClickDeleteWord();
			Assert.AreEqual(before - 1, _lexEntryRepository.CountAllItems());
		}

		private void ConfirmDeleteWhenDialogShows()
		{
			ExpectModal("ConfirmDelete", delegate { var x  = new ButtonTester("deleteBtn"); x.Click(); });
		}

		/// <summary>
		/// regression test for WS-536, WS-545
		/// </summary>
		[Test]
		public void DeleteWordWhenEvenHasCleanup_Regression()
		{
			ConfirmDeleteWhenDialogShows();
			ClickAddWord();
			int before = _lexEntryRepository.CountAllItems();

			EntryViewControl parentControl =
					((DictionaryControl) _task.Control).Control_EntryDetailPanel;
			LexEntry entry = parentControl.DataSource;
			const string form = "xx";
			entry.LexicalForm.SetAlternative(_vernacularWritingSystem.Id, form);
			GoToLexicalEntryUseFind("Initial"); //go away
			GoToLexicalEntryUseFind(form); //come back

			KeyValuePair<string, object> item2 = new KeyValuePair<string, object>("test",
																				  new LexRelation(
																						  "b",
																						  "bbb",
																						  entry));
			entry.Properties.Add(item2);

			GetEditControl("*EntryLexicalForm").FocusOnFirstWsAlternative();
			ClickDeleteWord();
			Assert.AreEqual(before - 1, _lexEntryRepository.CountAllItems());
			// GoToLexicalEntryUseFind(form); should fail to find it

			AssertExistenceOfEntryInList(form, false);
		}

		private void GoToLexicalEntryUseFind(string lexemeForm)
		{
			TextBoxTester t = new TextBoxTester("_findText", _window);
			t.Enter(lexemeForm);
			t.FireEvent("KeyDown", new KeyEventArgs(Keys.Enter));
			ListViewTester l = new ListViewTester("_recordsListBox", _window);
			string label = GetSelectedLabel((WeSayListView) l.Properties);
			Assert.AreEqual(lexemeForm, label);
		}

		private static string GetSelectedLabel(WeSayListView box)
		{
			return box.Items[box.SelectedIndex].Text;
		}

		private void AssertExistenceOfEntryInList(string form, bool shouldExist)
		{
			ListViewTester l = new ListViewTester("_recordsListBox", _window);
			bool found = false;

			for (int i = 0;i < l.Properties.Items.Count;++i)
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
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
			Assert.IsTrue(t.Properties.Focused);
		}

		[Test]
		public void ClickingDeleteWordRefreshesDetailView()
		{
			ClickDeleteWord();
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
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
			ListViewTester t = new ListViewTester("_recordsListBox", _window);
			((WeSayListView) t.Properties).SelectedIndex = 2;
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
			_task.Deactivate();
			ActivateTask();
		}

		[Test]
		public void EmptyDictionary_AddWords_NewWordSelectedInListBox()
		{
			DeleteAllEntries();
			ClickAddWord();
			ListViewTester t = new ListViewTester("_recordsListBox", _window);
			Assert.AreEqual(0, ((WeSayListView) t.Properties).SelectedIndex);
			string label = GetSelectedLabel((WeSayListView) t.Properties);
			Assert.AreEqual("(Empty)", label);
		}

		[Test]
		public void EmptyDictionary_AddWords_FieldsExist()
		{
			DeleteAllEntries();
			ClickAddWord();
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
			Assert.IsTrue(t.Properties.Visible);
		}

		[Test]
		public void EmptyDictionary_AddWordsTwice_OneWordExists()
		{
			ListViewTester l = new ListViewTester("_recordsListBox", _window);
			StartWithEmpty();
			ClickAddWord();
			Assert.AreEqual(1, l.Properties.Items.Count);
			ClickAddWord();
			Assert.AreEqual(1, l.Properties.Items.Count);
		}

		[Test]
		public void EmptyDictionary_AddWords_CanTypeInFirstField()
		{
			DeleteAllEntries();
			ClickAddWord();
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
			t.Enter("test");
			Assert.AreEqual("test", t.Text);
		}

		[Test]
		public void NewWord_DictionaryContainsWordEmpty_ContainsBlankAndEmpty()
		{
			//review: I (jh) find this test confusing.  if "empty" is a special word, can
			//someone add a comment explaining it?  Else, why not just use, like, "hello"?

			Application.DoEvents();
			StartWithEmpty();
			ClickAddWord();
			Application.DoEvents();
			var form = "hello";// "(Empty)";
			TypeInLexicalForm(form);
			Application.DoEvents();
			ClickAddWord();
			Application.DoEvents();
			Assert.AreEqual(2, _lexEntryRepository.CountAllItems());
			LexicalFormMustMatch(string.Empty);
			ListViewTester l = new ListViewTester("_recordsListBox", _window);
			// select other entry
			l.Select((l.Properties.SelectedIndices[0] + 1) % 2);
			LexicalFormMustMatch(form);
		}

		[Test]
		public void NewWord_NonEmptyEntryWithNoLexicalFormInCurrentWritingSystem_EntryStillAdded()
		{
			StartWithEmpty();
			ClickAddWord();
			RepositoryId[] repositoryId = _lexEntryRepository.GetAllItems();
			LexEntry entry = _lexEntryRepository.GetItem(repositoryId[0]);
			entry.LexicalForm[_vernacularWritingSystem.Id + "X"] = "something";
			ClickAddWord();
			Assert.AreEqual(2, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void NewWord_NonEmptyEntryWithNoLexicalForm_EntryStillAdded()
		{
			StartWithEmpty();
			ClickAddWord();
			RepositoryId[] repositoryId = _lexEntryRepository.GetAllItems();
			LexEntry entry = _lexEntryRepository.GetItem(repositoryId[0]);
			entry.Senses.Add(new LexSense(entry));
			(entry.Senses[0]).Definition.Add("blah");
			ClickAddWord();
			Assert.AreEqual(2, _lexEntryRepository.CountAllItems());
		}

		[Test]
		public void EmptyDictionary_DeleteButtonDisabled()
		{
			StartWithEmpty();
			ButtonTester l = new ButtonTester("_btnDeleteWord", _window);
			Assert.IsFalse(l.Properties.Enabled);
		}

		[Test]
		public void EmptyDictionary_EnterText_PressFindButton_NoCrash()
		{
			StartWithEmpty();
			TextBoxTester t = new TextBoxTester("_findText", _window);
			t.Enter("blah");
			ButtonTester b = new ButtonTester("_btnFind", _window);
			b.Click();
			ListViewTester l = new ListViewTester("_recordsListBox", _window);

			Assert.AreEqual(-1, ((WeSayListView) l.Properties).SelectedIndex);
		}

		[Test]
		public void IfNoWordsDeleteButtonDisabled()
		{
			ButtonTester l = new ButtonTester("_btnDeleteWord", _window);
			Assert.IsTrue(l.Properties.Enabled);
			DeleteAllEntries();
			Assert.IsFalse(l.Properties.Enabled);
		}

		[Test]
		public void CustomTextFieldPreservedNoOtherEditing()
		{
			CustomTextFieldPreservedCore("*MyEntryCustom");
			EnsureHasOneEntryProperty();
		}

		private void EnsureHasOneEntryProperty()
		{
			LexEntry entry = GetCurrentEntry();
			entry.CleanUpAfterEditting();
			Assert.AreEqual(1, entry.Properties.Count);
		}

		[Test]
		public void CustomTextFieldPreserved()
		{
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
			t.Enter("test");

			CustomTextFieldPreservedCore("*MyEntryCustom");
			EnsureHasOneEntryProperty();
		}

		private void CustomTextFieldPreservedCore(string fieldLabel)
		{
			MultiTextControl note = GetEditControl(fieldLabel);
			WeSayTextBox box = note.TextBoxes[0];
			box.Focus();
			box.Text = "a note";

			Application.DoEvents();
			LexEntry entry = GetCurrentEntry();
			CycleTheCurrentEntryOutAndBackIn(entry);
			Application.DoEvents();
			note = GetEditControl(fieldLabel);
			Assert.AreEqual("a note", note.TextBoxes[0].Text);
		}

		private void ShiftFocus()
		{
			GetEditControl("*focusOnMe").TextBoxes[0].Focus();
		}

		private void CustomOptionRefPreservedCore(string fieldLabel)
		{
			SingleOptionControl combo = GetOptionControl(fieldLabel);

			combo.Value = "verb";

			LexEntry entry = GetCurrentEntry();
			CycleTheCurrentEntryOutAndBackIn(entry);

			combo = GetOptionControl(fieldLabel);
			Assert.AreEqual("verb", combo.Value);
		}

		[Test]
		public void CustomOptionRefOnSensePreserved()
		{
			CustomOptionRefPreservedCore("POS");
			EnsureHasTwoSenseProperties();
		}

		[Test]
		public void CustomTextFieldOnSensePreserved()
		{
			CustomTextFieldPreservedCore("s-note");
			EnsureHasTwoSenseProperties();
		}

		private void EnsureHasTwoSenseProperties()
		{
			LexEntry entry = GetCurrentEntry();
			entry.CleanUpAfterEditting();
			//one for definition, one for custom field
			Assert.AreEqual(2, entry.Senses[0].Properties.Count);
		}

		private void CycleTheCurrentEntryOutAndBackIn(LexEntry entry)
		{
			ShiftFocus();//this should not be needed, and *is not needed* in the real, running app, or if you step through. I tried for over an hour to get the test code to accurately replicate the run-time situation, but I give up!

			Application.DoEvents();
			((DictionaryControl) _task.Control).GoToEntry(_firstEntryGuid);

		   ((DictionaryControl) _task.Control).GoToEntry(_secondEntryGuid);
			Application.DoEvents();

			 Thread.Sleep(1000);
		   Application.DoEvents();
			((DictionaryControl) _task.Control).GoToEntry(entry.Id);
			Application.DoEvents();
			Thread.Sleep(1000);
			Application.DoEvents();
			return;
		}

		private LexEntry GetCurrentEntry()
		{
			EntryViewControl parentControl =
					((DictionaryControl) _task.Control).Control_EntryDetailPanel;
			return parentControl.DataSource;
		}

		[Test]
		public void EmptyProperitesRemovedAfterEditting()
		{
			EntryViewControl parentControl =
					((DictionaryControl) _task.Control).Control_EntryDetailPanel;
			LexEntry entry = parentControl.DataSource;
			Assert.Less(0,
						entry.Properties.Count,
						"the setup of this test should have some custom properties");
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
			t.Enter("test");
			Assert.Less(0,
						entry.Properties.Count,
						"the setup of this test should have some custom properties");

			//cycle out this record
			parentControl.DataSource =_lexEntryRepository.CreateItem();

			Assert.AreEqual(0, entry.Properties.Count);
#if GlossMeaning
			Assert.AreEqual(0, entry.Senses[0].Properties.Count);
#else
			Assert.AreEqual(1, entry.Senses[0].Properties.Count);
			Assert.AreEqual(LexSense.WellKnownProperties.Definition,
							entry.Senses[0].Properties[0].Key);
#endif
		}

		[Test]
		public void ClickingTheStarButton_AfterTyping_SetsAnnotation()
		{
			TypeInLexicalForm("one");
			ClickStarOfLexemeForm();
			ResultSet<LexEntry> list =
					_lexEntryRepository.GetAllEntriesSortedByHeadword(_vernacularWritingSystem);
			LexEntry entry = list[0].RealObject;
			Assert.IsTrue(
					entry.LexicalForm.GetAnnotationOfAlternativeIsStarred(
							_vernacularWritingSystem.Id));
		}

		[Test]
		public void ClickingTheStarButton_WithEmptySenseBelow_RegressionTest()
		{
			//      this._records[0].Senses.AddNew();
			//         this._records[0].NotifyPropertyChanged("senses");
			// Application.DoEvents();
			ClickStarOfLexemeForm();
			ResultSet<LexEntry> list =
					_lexEntryRepository.GetAllEntriesSortedByHeadword(_vernacularWritingSystem);
			LexEntry entry = list[0].RealObject;

			Assert.IsTrue(
					entry.LexicalForm.GetAnnotationOfAlternativeIsStarred(
							_vernacularWritingSystem.Id));
		}

		private void ClickStarOfLexemeForm()
		{
			ControlTester t = new ControlTester(GetNameOfLexicalFormAnnotationControl(), _window);
			t.Click();
			GetEditControl(Field.FieldNames.EntryLexicalForm.ToString());
		}

		private static string GetNameOfLexicalFormAnnotationControl()
		{
			return GetLexicalFormControlName() + "-annotationWidget";
		}

		[Test]
		public void SwitchingToAnotherTaskDoesNotLooseBindings()
		{
			LexicalFormMustMatch("Initial");
			TypeInLexicalForm("one");
			_task.Deactivate();
			_tabControl.SelectedIndex = 1;
			_tabControl.SelectedIndex = 0;
			ActivateTask();

			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
			t.Properties.Visible = true;

			LexicalFormMustMatch("one");

			TypeInLexicalForm("plus");
			// need something that still sorts higher than Secondary and Tertiary
			_task.Deactivate();
			_tabControl.SelectedIndex = 1;
			_tabControl.SelectedIndex = 0;
			ActivateTask();
			t = new TextBoxTester(GetLexicalFormControlName(), _window);
			t.Properties.Visible = true;
			LexicalFormMustMatch("plus");
		}

		private void ActivateTask()
		{
			_task.Activate();
			_task.Control.Dock = DockStyle.Fill;
			_detailTaskPage.Controls.Clear();
			_detailTaskPage.Controls.Add(_task.Control);
		}

		private void LexicalFormMustMatch(string value)
		{
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
			Assert.AreEqual(value, t.Properties.Text);
		}

		private static string GetLexicalFormControlName()
		{
			return Field.FieldNames.EntryLexicalForm + "_" +
				   BasilProject.Project.WritingSystems.TestWritingSystemVernId;
		}

		private void TypeInLexicalForm(string value)
		{
			TextBoxTester t = new TextBoxTester(GetLexicalFormControlName(), _window);
			t.Properties.Text = value;
			//change the focus
			ShiftFocus();
		}

		private static string GetMeaningControlName()
		{
#if GlossMeaning
			return Field.FieldNames.SenseGloss + "_" + BasilProject.Project.WritingSystems.TestWritingSystemAnalId;
#else
			return LexSense.WellKnownProperties.Definition + "_" +
				   BasilProject.Project.WritingSystems.TestWritingSystemAnalId;
#endif
		}

		private void TypeInMeaning(string value)
		{
			TextBoxTester t = new TextBoxTester(GetMeaningControlName(), _window);
			t.Properties.Text = value;
		}

		private string LexemeFormOfSelectedEntry
		{
			get
			{
				return
						((DictionaryControl) _detailTaskPage.Controls[0]).CurrentRecord.LexicalForm.
								GetBestAlternative(_vernacularWritingSystem.Id);
			}
		}

		private void ClickAddWord()
		{
			ButtonTester l = new ButtonTester("_btnNewWord", _window);
			l.Click();
		}

		private void ClickDeleteWord()
		{
			ConfirmDeleteWhenDialogShows();
			ButtonTester l = new ButtonTester("_btnDeleteWord", _window);
			l.Click();
		}

		private void ClickFindButton()
		{
			ButtonTester b = new ButtonTester("_btnFind", this._window);
			b.Click();
		}

		[Test]
		public void FindText_EnterTextThenPressFindButton_Finds()
		{
			TextBoxTester t = new TextBoxTester("_findText", _window);
			t.Enter("Secondary");
			ClickFindButton();
			ListViewTester l = new ListViewTester("_recordsListBox", _window);

			string label = GetSelectedLabel((WeSayListView) l.Properties);
			Assert.AreEqual("Secondary", label);
			RichTextBoxTester r = new RichTextBoxTester("_lexicalEntryPreview", _window);
			Assert.IsTrue(r.Text.Contains("secondarymeaning"));
		}

		[Test]
		public void FindText_EnterTextOneCharacterAtATime_DoesNotThrow()
		{
			TextBoxTester t = new TextBoxTester("_findText", _window);
			//This is a beter test but gives a cryptic error message
			//KeyboardController keyboardController = new KeyboardController(t);
			//t.Properties.Focus();
			//keyboardController.Press("Test");
			//keyboardController.Press("e");
			//keyboardController.Press("s");
			//keyboardController.Press("t");
			//keyboardController.Dispose();
			t.Enter("Test");
			t.FireEvent("TextChanged", new EventArgs());
			Assert.AreEqual("Test", t.Text);
		}

		[Test]
		public void FindText_Enter_Finds()
		{
			TextBoxTester t = new TextBoxTester("_findText", _window);
			t.Enter("Secondary");
			t.FireEvent("KeyDown", new KeyEventArgs(Keys.Enter));
			ListViewTester l = new ListViewTester("_recordsListBox", _window);

			string label = GetSelectedLabel((WeSayListView) l.Properties);
			Assert.AreEqual("Secondary", label);
		}

		[Test]
		public void FindText_EnterWordNotInDictionaryThenPressCtrlN_AddsWordInFindText()
		{
			TextBoxTester t = new TextBoxTester("_findText", _window);
			t.Enter("NewWord");
			PressCtrlN(t);

			VerifySelectedWordIs("NewWord");
		}

		private void VerifySelectedWordIs(string word)
		{
			ListViewTester l = new ListViewTester("_recordsListBox", this._window);

			string label = GetSelectedLabel((WeSayListView) l.Properties);
			Assert.AreEqual(word, label);
		}

		private static void PressCtrlN(ControlTester t)
		{
			using (KeyboardController kc = new KeyboardController(t))
			{
				kc.Press("^n"); // Ctrl+N
			}
		}

		[Test]
		public void FindText_EnterWordInDictionaryThenPressCtrlN_AddsWordInFindTextSoTwoEntries()
		{
			TextBoxTester t = new TextBoxTester("_findText", _window);
			t.Enter("Secondary");
			PressCtrlN(t);
			VerifySelectedWordIs("Secondary");
			Assert.AreEqual(2,
							_lexEntryRepository.GetEntriesWithMatchingLexicalForm("Secondary",
																				  _vernacularWritingSystem)
									.Count);
		}

		[Test]
		public void NewWord_FindTextNotInDictionary_CreatesNewEmptyWord()
		{
			TextBoxTester t = new TextBoxTester("_findText", _window);
			t.Enter("NewWord");

			ClickAddWord();
			VerifyNewEmptyWordCreated();
		}

		private void VerifyNewEmptyWordCreated()
		{
			LexEntry entry = GetCurrentEntry();
			Assert.AreEqual(0, entry.LexicalForm.Count);
		}

		[Test]
		public void NewWord_FindTextInDictionary_CreatesNewEmptyWord()
		{
			TextBoxTester t = new TextBoxTester("_findText", _window);
			t.Enter("Secondary");
			ClickAddWord();
			VerifyNewEmptyWordCreated();
		}

		[Test]
		public void BaselineForRemovingSenseTests()
		{
			PutCursorInMeaningFieldOfSecondEntry();

			TypeInMeaning("samo");

			TextBoxTester tb2 = new TextBoxTester(GetMeaningControlName(), _window);
			Assert.AreEqual("samo", tb2.Properties.Text);
		}

		[Test]
		public void EditField_RemoveSenseContents_RemovesSense()
		{
			PutCursorInMeaningFieldOfSecondEntry();
			TypeInMeaning(string.Empty);
			ShiftFocus();
			Thread.Sleep(1000);
			Application.DoEvents();
			Assert.IsTrue(GetEditControl("Meaning").Name.Contains("ghost"),
						  "Only ghost should remain");
		}

		//private void DoUi()
		//{
		//    while (true)
		//    {
		//        Application.DoEvents();
		//    }
		//}

		[Test] //regression test
		public void PastingBlankOverAMeaningOfEmptySenseDoesntCrash()
		{
			PutCursorInMeaningFieldOfSecondEntry();
			TextBoxTester tb = new TextBoxTester(GetMeaningControlName(), _window);
			Clipboard.SetText(" ");
			tb.Properties.Paste();
		}

		[Test] //regression test
		public void PastingTextOverAMeaningOfEmptySenseDoesntJustChangesMeaning()
		{
			PutCursorInMeaningFieldOfSecondEntry();
			TextBoxTester tb = new TextBoxTester(GetMeaningControlName(), _window);
			Clipboard.SetText("samo");
			tb.Properties.Paste();
			TextBoxTester tb2 = new TextBoxTester(GetMeaningControlName(), _window);
			Assert.AreEqual("samo", tb2.Properties.Text);
		}

		private void PutCursorInMeaningFieldOfSecondEntry()
		{
			//skip to second word (first has extra stuff in the sense)
			ListViewTester t = new ListViewTester("_recordsListBox", _window);
			t.Properties.Focus();
			((WeSayListView) t.Properties).SelectedIndex = 1;
			Assert.AreEqual("Secondary", LexemeFormOfSelectedEntry);
			Assert.AreEqual(1,
							GetCurrentEntry().Senses.Count,
							"this test assumes an entry with 1 sense");
			Assert.AreEqual(0,
							GetCurrentEntry().Senses[0].ExampleSentences.Count,
							"this test assumes a sense w/ no example");
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

			FillInTheGhostMeaning();

			//ghost really did fire
			Assert.IsTrue(detailList.Count > initialCount);

			//now do another one
			initialCount = detailList.Count;
			MultiTextControl editControl2 = (MultiTextControl) GetEditControl("Meaning 2", true);
			Assert.IsTrue(editControl2.Name.Contains("ghost"));
			editControl2.TextBoxes[0].Focus();
			Application.DoEvents();
			TextBoxTester t2 = new TextBoxTester(editControl2.TextBoxes[0].Name, _window);
			t2.Properties.Text = "bar";
			Application.DoEvents();
			TextBoxTester lxt = new TextBoxTester(GetLexicalFormControlName(), _window);
			lxt.Properties.Focus();
			Application.DoEvents();
			Assert.IsTrue(detailList.Count > initialCount);
		}

		private void FillInTheGhostMeaning()
		{
			MultiTextControl editControl = (MultiTextControl) GetEditControl("Meaning", true);
			Assert.IsTrue(GetEditControl("Meaning").Name.Contains("ghost"));
			editControl.TextBoxes[0].Focus();
			TextBoxTester t = new TextBoxTester(editControl.TextBoxes[0].Name, _window);
			//didn''t work  t.FireEvent("KeyPress", new KeyPressEventArgs('a'));
			t.Properties.Text = "foo";
			//move focus away
			Application.DoEvents();
			TextBoxTester lxt = new TextBoxTester(GetLexicalFormControlName(), _window);
			lxt.Properties.Focus();
			Application.DoEvents();
		}

		[Test]
		public void NewWord_GhostMeaningLabelWithNoNumber()
		{
			ClickAddWord();
			Assert.AreEqual("Meaning", GetLabelOfMeaningRow(0));
		}

		[Test]
		public void AfterAddingMeaning_RealMeaningLabelHasNumber()
		{
			ClickAddWord();
			FillInTheGhostMeaning();
			Assert.AreEqual("Meaning 1", GetLabelOfMeaningRow(0));
		}

		/// <summary>
		/// Regression for WS-620
		/// </summary>
		[Test]
		public void AfterAddingMeaning_GhostMeaningLabelHasNumber()
		{
			ClickAddWord();
			FillInTheGhostMeaning();
			Assert.AreEqual("Meaning 2", GetLabelOfMeaningRow(1));
		}

		[Test]
		public void ClickingShowAllOnce_ShowsCustomShyGuyOnEntry()
		{
			ClickAddWord();
			Assert.IsNull(GetEditControl("MyShyEntryCustom"));
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton", _window);
			btn.Click();
			Assert.IsNotNull(GetEditControl("MyShyEntryCustom"));
		}

		[Test]
		public void ClickingShowAllTwice_HidesCustomShyGuy()
		{
			ClickAddWord();
			Assert.IsNull(GetEditControl("MyShyEntryCustom"));
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton", _window);
			btn.Click();
			Assert.IsNotNull(GetEditControl("MyShyEntryCustom"));
			btn.Click();
			Assert.IsNull(GetEditControl("MyShyEntryCustom"));
		}

		[Test]
		public void AddingNewWord_HidesCustomShyGuy()
		{
			ClickAddWord();
			TypeInLexicalForm("foo");
			Assert.IsNull(GetEditControl("MyShyEntryCustom"));
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton", _window);
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
			TypeInLexicalForm("foo");
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton", _window);
			Assert.IsTrue(btn.Text.Contains("Show"));
			btn.Click();
			Assert.IsTrue(btn.Text.Contains("Hide"));
			ClickAddWord();
			Assert.IsTrue(btn.Text.Contains("Show"));
		}

		//Regression test WS-950. Also effects WS-962.
		//This does not so much test that an entry is id'd correctly, but simply tests that the
		//error causing method call  is not reintroduced.
		[Test]
		public void AddingNewWord_WordIsNotPrematurelyIdd()
		{
			StartWithEmpty();
			ClickAddWord();
			RepositoryId[] repositoryId = _lexEntryRepository.GetAllItems();
			LexEntry entry = _lexEntryRepository.GetItem(repositoryId[0]);
			Assert.AreEqual(null, entry.Id);
		}

		[Test]
		public void DeletingWord_ClearsShowHiddenState()
		{
			ClickAddWord();
			TypeInLexicalForm("foo");
			ClickAddWord();
			ButtonTester btn = new ButtonTester("_showAllFieldsToggleButton", _window);
			Assert.IsTrue(btn.Text.Contains("Show"));
			btn.Click();
			Assert.IsTrue(btn.Text.Contains("Hide"));
			ConfirmDeleteWhenDialogShows();
			ButtonTester delbtn = new ButtonTester("_btnDeleteWord", _window);
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
			Assert.AreEqual(idOfInitial, control.CurrentRecord.Id);
		}

		[Test]
		[ExpectedException(typeof (NavigationException))]
		public void GotoEntry_EntryNotInList_Throws()
		{
			DictionaryControl control = (DictionaryControl) _task.Control;
			control.GoToEntry("bogus");
		}

		[Test]
		public void ClickOnWhiteSpaceToRightOfEntry_EntryAlreadySelected_DeleteButtonStaysEnabled()
		{
			ListViewTester l = new ListViewTester("_recordsListBox", _window);
			ButtonTester b = new ButtonTester("_btnDeleteWord", _window);
			using (MouseController mc = new MouseController(l))
			{
				Rectangle r = l.Properties.GetItemRect(0);
				mc.Click(r.Right + 1, r.Top + 1);
				// move enough to not count as a double-click
				mc.Click(r.Right + SystemInformation.DoubleClickSize.Width + 2, r.Top + 1);
			}
			Assert.IsTrue(b.Properties.Enabled);
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
			return ((DictionaryControl) _task.Control).Control_EntryDetailPanel.ControlEntryDetail;
		}

		private SingleOptionControl GetOptionControl(string labelText)
		{
			return (SingleOptionControl) GetEditControl(labelText, false);
		}

		private MultiTextControl GetEditControl(string labelText)
		{
			return (MultiTextControl) GetEditControl(labelText, false);
		}

		private Control GetEditControl(string labelText, bool lookingForGhostVersion)
		{
			DetailList detailList =
					((DictionaryControl) _task.Control).Control_EntryDetailPanel.ControlEntryDetail;
			Control foundControl = null;
			for (int i = 0;i < detailList.Count;i++)
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

		private string GetLabelOfMeaningRow(int whichMeaningZeroBased)
		{
			DetailList detailList =
					((DictionaryControl) _task.Control).Control_EntryDetailPanel.ControlEntryDetail;
			int foundSoFar = -1;
			for (int i = 0;i < detailList.Count;i++)
			{
				Label label = detailList.GetLabelControlFromRow(i);
				if (label.Text.Contains("Meaning"))
				{
					++foundSoFar;
					if (foundSoFar == whichMeaningZeroBased)
					{
						return label.Text;
					}
				}
			}
			return "Not Found";
		}
	}
}