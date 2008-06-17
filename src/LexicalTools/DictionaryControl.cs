using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalTools.Properties;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;

namespace WeSay.LexicalTools
{
	public partial class DictionaryControl: UserControl
	{
		private readonly ViewTemplate _viewTemplate;
		private readonly ContextMenu _cmWritingSystems;
		private WritingSystem _listWritingSystem;
		private readonly LexEntryRepository _lexEntryRepository;
		private ResultSet<LexEntry> _records;
		private bool _keepRecordCurrent;

		public DictionaryControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public DictionaryControl(LexEntryRepository recordManager,
								 ViewTemplate viewTemplate)
		{
			if (recordManager == null)
			{
				throw new ArgumentNullException("recordManager");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			_viewTemplate = viewTemplate;
			this._lexEntryRepository = recordManager;
			_cmWritingSystems = new ContextMenu();

			SetupPickerControlWritingSystems();

			InitializeComponent();
			InitializeDisplaySettings();

			_writingSystemChooser.Image =
					Resources.Expand.GetThumbnailImage(6,
													   6,
													   ReturnFalse,
													   IntPtr.Zero);
			_btnFind.Image =
					Resources.Find.GetThumbnailImage(18,
													 18,
													 ReturnFalse,
													 IntPtr.Zero);
			_btnDeleteWord.Image =
					Resources.DeleteWord.GetThumbnailImage(18,
														   18,
														   ReturnFalse,
														   IntPtr.Zero);
			_btnNewWord.Image =
					Resources.NewWord.GetThumbnailImage(18,
														18,
														ReturnFalse,
														IntPtr.Zero);

			Control_EntryDetailPanel.ViewTemplate = _viewTemplate;
			Control_EntryDetailPanel.LexEntryRepository = this._lexEntryRepository;

			SetListWritingSystem(_viewTemplate.GetDefaultWritingSystemForField(Field.FieldNames.EntryLexicalForm.ToString()));

			_findText.KeyDown += _findText_KeyDown;
			_recordsListBox.SelectedIndexChanged +=OnRecordSelectionChanged;

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
					RepositoryId id = this._records[CurrentIndex].Id;
					return this._lexEntryRepository.GetItem(id);
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
			this._findWritingSystemId.ForeColor = DisplaySettings.Default.WritingSystemLabelColor;

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
				if (field.WritingSystems.Count > 0)
				{
					foreach (WritingSystem writingSystem in field.WritingSystems)
					{
						AddWritingSystemToPicker(writingSystem, field);
					}
				}
				else
				{
					ErrorReport.ReportNonFatalMessage(
							"There are no writing systems enabled for the Field '{0}'",
							field.FieldName);
				}
			}
		}
		private void AddWritingSystemToPicker(WritingSystem writingSystem, Field field)
		{
			MenuItem item =
				new MenuItem(
					writingSystem.Id + "\t" +
					StringCatalog.Get(field.DisplayName),
					OnCmWritingSystemClicked);
			item.RadioCheck = true;
			item.Tag = writingSystem;
			_cmWritingSystems.MenuItems.Add(item);
		}

		private bool IsWritingSystemUsedInLexicalForm(WritingSystem writingSystem)
		{
			return _viewTemplate.IsWritingSystemUsedInField(writingSystem, Field.FieldNames.EntryLexicalForm.ToString());
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

			SetRecordToBeEdited(CurrentRecord);
			_recordsListBox.WritingSystem = _listWritingSystem;

			int originalHeight = _findText.Height;
			_findText.ItemFilterer = FindClosestAndNextClosestAndPrefixedForms;
			_findText.Items = _records;
			_findText.WritingSystem = _listWritingSystem;

			_findWritingSystemId.Text = _listWritingSystem.Id;
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
			_btnFind.Image =
					Resources.Find.GetThumbnailImage(_btnFind.Width - 2,
													 _btnFind.Width - 2,
													 ReturnFalse,
													 IntPtr.Zero);

			_btnFind.Left = _writingSystemChooser.Left - _btnFind.Width;
			_findText.Width = _btnFind.Left - _findText.Left;
		}

		private void SetRecordToBeEdited(LexEntry record) {
			if (Control_EntryDetailPanel.DataSource != null)
			{
				Control_EntryDetailPanel.DataSource.PropertyChanged -= OnEntryChanged;
			}
			Control_EntryDetailPanel.DataSource = record;
			if (record != null)
			{
				record.PropertyChanged += OnEntryChanged;
			}
		}

		private void OnEntryChanged(object sender, PropertyChangedEventArgs e)
		{
			Debug.Assert(CurrentIndex != -1);
			RecordToken<LexEntry> recordToken = _records[CurrentIndex];
			if(!recordToken.IsFresh)
			{
				recordToken.Refresh();
				_keepRecordCurrent = true;
				LoadRecords();
				int index = this._records.FindFirstIndex(recordToken);
				//may not have been successful with the refresh of the recordToken
				// in which case we should just try to go to the first with
				// the same id
				if(index < 0)
				{
				   index = this._records.FindFirstIndex(recordToken.Id);
				}
				Debug.Assert(index != -1);
				_recordsListBox.SelectedIndex = index;
				_keepRecordCurrent = false;
			}
		}

		private void LoadRecords() {

			if(IsWritingSystemUsedInLexicalForm(this._listWritingSystem))
			{
				this._records = this._lexEntryRepository.GetAllEntriesSortedByLexicalForm(_listWritingSystem);
		}
			else
			{
				this._records = this._lexEntryRepository.GetAllEntriesSortedByGloss(_listWritingSystem);

			}
			_recordsListBox.BeginUpdate();
			_recordsListBox.DataSource = (BindingList<RecordToken<LexEntry>>)_records;
			_recordsListBox.EndUpdate();
		}

		private void OnRetrieveVirtualItemEvent(object sender, RetrieveVirtualItemEventArgs e)
		{
			string displayString = _records[e.ItemIndex].DisplayString;
			e.Item = new ListViewItem(displayString);
		}

		private static IEnumerable FindClosestAndNextClosestAndPrefixedForms(string text, IEnumerable items, IDisplayStringAdaptor adaptor)
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

		private void OnFindWritingSystemId_MouseClick(object sender,
													  MouseEventArgs e)
		{
			Logger.WriteMinorEvent("FindWritingSystemId_MouseClick");
			_findText.Focus();
		}

		// primarily for testing

		private void _findText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None &&
				e.KeyData == Keys.Enter)
			{
				e.Handled = true;
				e.SuppressKeyPress = true; // otherwise it beeps!
				SelectItemWithDisplayString(_findText.Text);
			}
		}

		private void _findText_AutoCompleteChoiceSelected(object sender,
														  EventArgs e)
		{
			SelectItemWithDisplayString(_findText.Text);
		}

		private void OnFind_Click(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("FindButton_Click");

			SelectItemWithDisplayString(_findText.Text);
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
			_recordsListBox.SelectedIndex = _records.FindFirstIndexWithDisplayString(text);
		}

		private void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			if (_keepRecordCurrent) return;

			if (Control_EntryDetailPanel.DataSource == CurrentRecord)
			{
				//we were getting 3 calls to this for each click on a new word
				return;
			}

			if (CurrentRecord != null)
			{
				Logger.WriteEvent("RecordSelectionChanged to " +
								  CurrentRecord.LexicalForm.GetFirstAlternative());
			}
			else
			{
				Logger.WriteEvent(
						"RecordSelectionChanged Skipping because record is null");
			}

			SetRecordToBeEdited(CurrentRecord);

			SelectedIndexChanged.Invoke(this, null);
			UpdateDisplay();
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(keyData == (Keys.Control | Keys.N))
			{
				AddNewWord();
				return true;
			}

			if(keyData == (Keys.Control | Keys.F))
			{
				if(!_findText.Focused)
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
			AddNewWord();
		}

		private void AddNewWord()
		{
			Logger.WriteEvent("NewWord_Click");

			LexEntry entry = this._lexEntryRepository.CreateItem();
			//bool NoPriorSelection = _recordsListBox.SelectedIndex == -1;
			//_recordListBoxActive = true; // allow onRecordSelectionChanged
			if (_findText.Focused
				&& !string.IsNullOrEmpty(_findText.Text)
				&& IsWritingSystemUsedInLexicalForm(_listWritingSystem))
				//todo only create new when not found (doesn't already exist)
			{
				entry.LexicalForm[_listWritingSystem.Id] = _findText.Text.Trim();
			}

			if (!_btnNewWord.Focused)
			{
				// if we use a hot key, it may not have received the focus
				// but we assume it has the focus when we do our selection change event
				_btnNewWord.Focus();
			}
			LoadRecords();
			int index = _records.FindFirstIndex(entry);
			Debug.Assert(index != -1);
			_recordsListBox.SelectedIndex = index;
			OnRecordSelectionChanged(_recordsListBox, new EventArgs());
			_entryViewControl.Focus();
		}

		private void OnDeleteWord_Click(object sender, EventArgs e)
		{
			Logger.WriteEvent("DeleteWord_Clicked");

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
		   CurrentRecord.IsBeingDeleted = true;
		   RecordToken<LexEntry> recordToken = _records[CurrentIndex];
			this._lexEntryRepository.DeleteItem(recordToken.Id);
			int index = _recordsListBox.SelectedIndex;
			LoadRecords();
			if(index >= _records.Count)
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
				_showAllFieldsToggleButton.Text = StringCatalog.Get("~Hide Uncommon Fields");
			}
			else
			{
				_showAllFieldsToggleButton.Text = StringCatalog.Get("~Show Uncommon Fields");
			}
		}
	}
}
