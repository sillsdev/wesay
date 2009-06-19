using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Misc;
using Palaso.Reporting;
using Palaso.Text;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalTools.Properties;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;

namespace WeSay.LexicalTools.DictionaryBrowseAndEdit
{
	public partial class DictionaryControl: UserControl
	{
		private readonly ViewTemplate _viewTemplate;
		private readonly ILogger _logger;
		private readonly ContextMenu _cmWritingSystems;
		private WritingSystem _listWritingSystem;
		private readonly LexEntryRepository _lexEntryRepository;
		private ResultSet<LexEntry> _records;
		private bool _keepRecordCurrent;
		private readonly ResultSetToListOfStringsAdapter _findTextAdapter;

		public DictionaryControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public DictionaryControl(LexEntryRepository lexEntryRepository,
			ViewTemplate viewTemplate, IUserInterfaceMemory memory, ILogger logger)
		{
			if (lexEntryRepository == null)
			{
				throw new ArgumentNullException("lexEntryRepository");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			_viewTemplate = viewTemplate;
			_logger = logger;
			_lexEntryRepository = lexEntryRepository;
			_cmWritingSystems = new ContextMenu();

			SetupPickerControlWritingSystems();

			InitializeComponent();
			InitializeDisplaySettings();

			_writingSystemChooser.Image = Resources.Expand.GetThumbnailImage(6,
																			 6,
																			 ReturnFalse,
																			 IntPtr.Zero);
			_btnFind.Image = Resources.Find.GetThumbnailImage(18, 18, ReturnFalse, IntPtr.Zero);
			_btnDeleteWord.Image = Resources.DeleteWord;
			_btnNewWord.Image = Resources.NewWord.GetThumbnailImage(18, 18, ReturnFalse, IntPtr.Zero);

			Control_EntryDetailPanel.ViewTemplate = _viewTemplate;
			Control_EntryDetailPanel.LexEntryRepository = _lexEntryRepository;

			_findTextAdapter = new ResultSetToListOfStringsAdapter("Form", _records);
			_findText.Items = _findTextAdapter;

			SetListWritingSystem(
					_viewTemplate.GetDefaultWritingSystemForField(
							Field.FieldNames.EntryLexicalForm.ToString()));

			_findText.KeyDown += _findText_KeyDown;
			_recordsListBox.SelectedIndexChanged += OnRecordSelectionChanged;

			_splitter.SetMemory(memory);
			_entryViewControl.SetMemory(memory.CreateNewSection("entryView"));

			UpdateDisplay();
		}

		public EntryViewControl Control_EntryDetailPanel
		{
			get { return _entryViewControl; }
		}

		public LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0 || CurrentIndex == -1)
				{
					return null;
				}
				try
				{
					RepositoryId id = _records[CurrentIndex].Id;
					return _lexEntryRepository.GetItem(id);
				}
				catch (Exception e)
				{
					WeSayWordsProject.Project.HandleProbableCacheProblem(e);
					return null;
				}
			}
		}

		protected int CurrentIndex
		{
			get { return _recordsListBox.SelectedIndex; }
		}



		public event EventHandler SelectedIndexChanged = delegate { };

		private void InitializeDisplaySettings()
		{
			BackColor = DisplaySettings.Default.BackgroundColor;
			_findWritingSystemId.ForeColor = DisplaySettings.Default.WritingSystemLabelColor;
		}

		private void SetupPickerControlWritingSystems()
		{
			RegisterFieldWithPicker(Field.FieldNames.EntryLexicalForm.ToString());
			RegisterFieldWithPicker(LexSense.WellKnownProperties.Gloss); //Reversal
		}

		private void RegisterFieldWithPicker(string fieldName)
		{
			Field field = _viewTemplate.GetField(fieldName);
			if (field != null)
			{
				if (field.WritingSystemIds.Count > 0)
				{
					IList<WritingSystem> writingSystems =
							BasilProject.Project.WritingSystemsFromIds(field.WritingSystemIds);
					foreach (WritingSystem writingSystem in writingSystems)
					{
						if (!WritingSystemExistsInPicker(writingSystem))
						{
							AddWritingSystemToPicker(writingSystem, field);
						}
					}
				}
				else
				{
					ErrorReport.NotifyUserOfProblem(
							"There are no writing systems enabled for the Field '{0}'",
							field.FieldName);
				}
			}
		}

		private void AddWritingSystemToPicker(WritingSystem writingSystem, Field field)
		{
			MenuItem item =
					new MenuItem(writingSystem.Abbreviation + "\t" + StringCatalog.Get(field.DisplayName),
								 OnCmWritingSystemClicked);
			item.RadioCheck = true;
			item.Tag = writingSystem;
			_cmWritingSystems.MenuItems.Add(item);
		}

		private bool WritingSystemExistsInPicker(WritingSystem writingSystem)
		{
			foreach (MenuItem item in _cmWritingSystems.MenuItems)
			{
				if (writingSystem.Id == ((WritingSystem) item.Tag).Id)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsWritingSystemUsedInLexicalForm(string writingSystemId)
		{
			return _viewTemplate.IsWritingSystemUsedInField(writingSystemId,
															Field.FieldNames.EntryLexicalForm.
																	ToString());
		}

		public void SetListWritingSystem(WritingSystem writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException();
			}
			if (_listWritingSystem == writingSystem)
			{
				return;
			}
			_listWritingSystem = writingSystem;

			LoadRecords();
			_recordsListBox.RetrieveVirtualItem += OnRetrieveVirtualItemEvent;

			//WHy was this here (I'm (JH) scared to remove it)?
			// it is costing us an extra second, as we set the record
			// to the first one, then later set it to the one we actually want.
			//  SetRecordToBeEdited(CurrentRecord);


			_recordsListBox.WritingSystem = _listWritingSystem;

			int originalHeight = _findText.Height;
			_findText.ItemFilterer = FindClosestAndNextClosestAndPrefixedForms;
			_findText.Items = _findTextAdapter;
			_findText.WritingSystem = _listWritingSystem;

			_findWritingSystemId.Text = _listWritingSystem.Abbreviation;
			int width = _findWritingSystemId.Width;
			_findWritingSystemId.AutoSize = false;
			_findWritingSystemId.Size = new Size(Math.Min(width, 25), _findText.Height);
			int heightDifference = _findText.Height - originalHeight;

			_recordsListBox.SetBounds(_recordsListBox.Bounds.X,
									  _recordsListBox.Bounds.Y + heightDifference,
									  _recordsListBox.Bounds.Width,
									  _recordsListBox.Bounds.Height - heightDifference);

			_btnFind.Height = _findText.Height;
			_writingSystemChooser.Height = _findText.Height;
			_btnFind.Image = Resources.Find.GetThumbnailImage(_btnFind.Width - 2,
															  _btnFind.Width - 2,
															  ReturnFalse,
															  IntPtr.Zero);

			_btnFind.Left = _writingSystemChooser.Left - _btnFind.Width;
			_findText.Width = _btnFind.Left - _findText.Left;
		}

		private void SetRecordToBeEdited(LexEntry record)
		{
			if (record == Control_EntryDetailPanel.DataSource)
				return;

			SaveAndCleanUpPreviousEntry();
			Control_EntryDetailPanel.DataSource = record;
			if (record != null)
			{
				record.PropertyChanged += OnEntryChanged;
			}
		}

		private void SaveAndCleanUpPreviousEntry() {
			LexEntry previousEntry = Control_EntryDetailPanel.DataSource;
			if (previousEntry != null)
			{
				previousEntry.PropertyChanged -= OnEntryChanged;
				if (!previousEntry.IsBeingDeleted)
				{
					_lexEntryRepository.SaveItem(previousEntry);
				}
			}
		}

		private void OnEntryChanged(object sender, PropertyChangedEventArgs e)
		{


			_lexEntryRepository.NotifyThatLexEntryHasBeenUpdated((LexEntry)sender);

			Debug.Assert(CurrentIndex != -1);
			RecordToken<LexEntry> recordToken = _records[CurrentIndex];
			_keepRecordCurrent = true;
			LoadRecords();
			int index = _records.FindFirstIndex(recordToken);
			//may not have been successful in which case we should
			//just try to go to the first with the same id
			if (index < 0)
			{
				index = _records.FindFirstIndex(recordToken.Id);
			}
			Debug.Assert(index != -1);
			_recordsListBox.SelectedIndex = index;
			_keepRecordCurrent = false;
		}

		private void LoadRecords()
		{
			if (IsWritingSystemUsedInLexicalForm(_listWritingSystem.Id))
			{
				_records = _lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(_listWritingSystem);
				_findTextAdapter.Items = _records;
			}
			else
			{
				_records = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(_listWritingSystem);
				_findTextAdapter.Items = _records;
			}
			_recordsListBox.DataSource = (BindingList<RecordToken<LexEntry>>) _records;
		}

		private void OnRetrieveVirtualItemEvent(object sender, RetrieveVirtualItemEventArgs e)
		{
			RecordToken<LexEntry> recordToken = _records[e.ItemIndex];
			string displayString = (string) recordToken["Form"];
			e.Item = new ListViewItem(displayString);

			if ((string) recordToken["WritingSystem"] != _listWritingSystem.Id)
			{
				displayString = (string) recordToken["Form"];
				e.Item.Font = new Font(e.Item.Font, FontStyle.Italic);
				//!!! TODO: Get the correct font from the respective writingsystem and maybe put the writingsystem id behind the form!! --TA 8.9.08
			}

			bool writingSystemUsedInLexicalForm =
					IsWritingSystemUsedInLexicalForm(_listWritingSystem.Id);
			if (string.IsNullOrEmpty(displayString))
			{
				displayString = "(";
				if (writingSystemUsedInLexicalForm)
				{
					displayString += StringCatalog.Get("~Empty",
													   "This is what shows for a word in a list when the user hasn't yet typed anything in for the word.  Like if you click the 'New Word' button repeatedly.");
				}
				else
				{
					displayString += StringCatalog.Get("~No Gloss",
													   "This is what shows if the user is listing words by the glossing language, but the word doesn't have a gloss.");
				}
				displayString += ")";
			}
			e.Item.Text = displayString;
		}

		private static IEnumerable FindClosestAndNextClosestAndPrefixedForms(string text,
																			 IEnumerable items,
																			 IDisplayStringAdaptor
																					 adaptor)
		{
			return ApproximateMatcher.FindClosestForms(items,
													   text,
													   ApproximateMatcherOptions.IncludePrefixedAndNextClosestForms);
		}

		private void OnCmWritingSystemClicked(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem) sender;
			if (_listWritingSystem != item.Tag)
			{
				SetListWritingSystem((WritingSystem) item.Tag);
			}
		}

		private void OnWritingSystemChooser_Click(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("WritingSystemChooser_Click");

			foreach (MenuItem menuItem in _cmWritingSystems.MenuItems)
			{
				menuItem.Checked = (_listWritingSystem == menuItem.Tag);
			}
			_cmWritingSystems.Show(_writingSystemChooser,
								   new Point(_writingSystemChooser.Width,
											 _writingSystemChooser.Height));
		}

		private void OnFindWritingSystemId_MouseClick(object sender, MouseEventArgs e)
		{
			Logger.WriteMinorEvent("FindWritingSystemId_MouseClick");
			_findText.Focus();
		}

		// primarily for testing

		private void _findText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None && e.KeyData == Keys.Enter)
			{
				e.Handled = true;
				e.SuppressKeyPress = true; // otherwise it beeps!
				SelectItemWithDisplayString(_findText.Text);
			}
		}

		private void _findText_AutoCompleteChoiceSelected(object sender, EventArgs e)
		{
			SelectItemWithDisplayString(_findText.Text);
		}

		private void OnFind_Click(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("FindButton_Click");

			SelectItemWithDisplayString(_findText.Text);
		}


		public void GoToEntry(Guid entryGuid)
		{
			LexEntry entry = _lexEntryRepository.GetLexEntryWithMatchingGuid(entryGuid);
			if (entry == null)
			{
				throw new NavigationException("Could not find the entry with guid " + entryGuid);
			}
			_recordsListBox.SelectedIndex = _records.FindFirstIndex(entry);
		}

		public void GotoFirstEntry()
		{
			if(_recordsListBox.Items.Count>0)
				_recordsListBox.SelectedIndex = 0;

		}

		public void GoToEntry(string entryId)
		{
			LexEntry entry = _lexEntryRepository.GetLexEntryWithMatchingId(entryId);
			if (entry == null)
			{
				throw new NavigationException("Could not find the entry with id " + entryId);
			}
			_recordsListBox.SelectedIndex = _records.FindFirstIndex(entry);
		}

		private void SelectItemWithDisplayString(string text)
		{
			Logger.WriteMinorEvent("SelectItemWithDisplayString");
			_recordsListBox.SelectedIndex =
					_records.FindFirstIndex(
							delegate(RecordToken<LexEntry> token) { return (string) token["Form"] == text; });
		}

		private void OnRecordSelectionChanged(object sender, EventArgs e)
		{

			if (_keepRecordCurrent)
			{
				return;
			}

			if (Control_EntryDetailPanel.DataSource == CurrentRecord)
			{
				//we were getting 3 calls to this for each click on a new word
				// update display to catch changes from null to current value
				UpdateDisplay();
				return;
			}

			if (CurrentRecord != null)
			{
				Logger.WriteEvent("RecordSelectionChanged to " +
								  CurrentRecord.LexicalForm.GetFirstAlternative());
			}
			else
			{
				Logger.WriteEvent("RecordSelectionChanged Skipping because record is null");
			}

			SetRecordToBeEdited(CurrentRecord);


			//nb: SelectedIndexChanged,  which calls this, is fired twice
			//once for the deselection, again with the selection
			if (CurrentIndex == -1)
			{
				return;
			}

			SelectedIndexChanged.Invoke(this, null);
			UpdateDisplay();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//Catching ctrl-n here is a problem from the localisation perspective
			//What if the language does not lend itself to "n" as a representation of the word "new"
			//We could solve this by using an accelerator
			if (keyData == (Keys.Control | Keys.N))
			{
				bool FocusWasOnFindTextBox = _findText.Focused;
				_btnNewWord.Focus(); //this is necassary to cause TextBinding to update it's multitext
				AddNewWord(FocusWasOnFindTextBox);
				return true;
			}

			if (keyData == (Keys.Control | Keys.F))
			{
				if (!_findText.Focused)
				{
					_findText.Focus();
					_findText.Mode = WeSayAutoCompleteTextBox.EntryMode.List;
				}
				else
				{
					SelectItemWithDisplayString(_findText.Text);
				}

				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void OnNewWord_Click(object sender, EventArgs e)
		{
			AddNewWord(false);
		}

		internal void AddNewWord(bool FocusWasOnFindTextBox)
		{
			Logger.WriteMinorEvent("NewWord_Click");

			// only create a new word when there is not an empty word already
			int emptyWordIndex = GetEmptyWordIndex();
			int selectIndex;
			if (emptyWordIndex == -1)
			{
				LexEntry entry = this._lexEntryRepository.CreateItem();
				//bool NoPriorSelection = _recordsListBox.SelectedIndex == -1;
				//_recordListBoxActive = true; // allow onRecordSelectionChanged
				if (FocusWasOnFindTextBox && !string.IsNullOrEmpty(_findText.Text) &&
					IsWritingSystemUsedInLexicalForm(_listWritingSystem.Id))
				{
					entry.LexicalForm[_listWritingSystem.Id] = _findText.Text.Trim();
					_lexEntryRepository.SaveItem(entry);
				}
				//review: Revert (remove) below for WS-950
				// _lexEntryRepository.SaveItem(entry);
				LoadRecords();
				selectIndex = this._records.FindFirstIndex(entry);
			}
			else
			{
				selectIndex = emptyWordIndex;
			}

			if (!_btnNewWord.Focused)
			{
				// if we use a hot key, it may not have received the focus
				// but we assume it has the focus when we do our selection change event
				_btnNewWord.Focus();
			}
			Debug.Assert(selectIndex != -1);
			_recordsListBox.SelectedIndex = selectIndex;
			OnRecordSelectionChanged(_recordsListBox, new EventArgs());
			_entryViewControl.Focus();

			_logger.WriteConciseHistoricalEvent("Added Word");

		}

		private int GetEmptyWordIndex()
		{
			// empty forms will always sort to the top
			for (int i = 0;i < _records.Count;++i)
			{
				if (!string.IsNullOrEmpty((string) _records[i]["Form"]))
				{
					break;
				}

				if (_records[i].RealObject.IsEmpty)
				{
					return i;
				}
			}
			return -1; // there is no empty word
		}

		private void OnDeleteWord_Click(object sender, EventArgs e)
		{
			Logger.WriteEvent("DeleteWord_Clicked");

			using (var dlg = new ConfirmDelete())
			{
				dlg.ShowDialog(this);
				if (dlg.DialogResult != DialogResult.OK)
				{
					Logger.WriteEvent("DeleteWord_Cancelled");
					return;
				}
			}
			DeleteWord();
		}

		internal void DeleteWord()
		{
			Debug.Assert(CurrentIndex >= 0);
			if (CurrentIndex == -1)
			{
				return;
			}
			if (!_btnDeleteWord.Focused)
			{
				// if we use a hot key, it may not have received the focus
				// but we assume it has the focus when we do our selection change event
				_btnDeleteWord.Focus();
			}
			//review: This save isn't necessary, but the possibility of deleting unsave records currently doesn't work.
			//_lexEntryRepository.SaveItem(CurrentRecord);

			_logger.WriteConciseHistoricalEvent("Deleted '{0}'",CurrentRecord.GetSimpleFormForLogging());
			CurrentRecord.IsBeingDeleted = true;
			RecordToken<LexEntry> recordToken = _records[CurrentIndex];
			_lexEntryRepository.DeleteItem(recordToken.Id);
			int index = _recordsListBox.SelectedIndex;
			LoadRecords();
			if (index >= _records.Count)
			{
				index = _records.Count - 1;
			}
			_recordsListBox.SelectedIndex = index;
			OnRecordSelectionChanged(this, null);
			_entryViewControl.Focus();
		}



		private void OnShowAllFields_Click(object sender, EventArgs e)
		{
			_entryViewControl.ToggleShowNormallyHiddenFields();
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			_btnDeleteWord.Enabled = (CurrentRecord != null);
			if (_entryViewControl.ShowNormallyHiddenFields)
			{
				_showAllFieldsToggleButton.Text = StringCatalog.Get("~Hide &Uncommon Fields");
			}
			else
			{
				_showAllFieldsToggleButton.Text = StringCatalog.Get("~Show &Uncommon Fields");
			}
		}


	}
}
