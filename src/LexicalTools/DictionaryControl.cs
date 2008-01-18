using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalTools.Properties;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class DictionaryControl: UserControl
	{
		private readonly ViewTemplate _viewTemplate;
		private readonly ContextMenu _cmWritingSystems;
		private WritingSystem _listWritingSystem;
		private bool _recordListBoxActive;
		private bool _programmaticallyGoingToNewEntry;
		private readonly Db4oRecordListManager _recordManager;
		private IBindingList _records;

		public DictionaryControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public DictionaryControl(IRecordListManager recordManager,
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
			_recordManager = (Db4oRecordListManager) recordManager;
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
			Control_EntryDetailPanel.RecordListManager = this._recordManager;

			SetListWritingSystem(_viewTemplate.GetDefaultWritingSystemForField(Field.FieldNames.EntryLexicalForm.ToString()));

			_findText.KeyDown += _findText_KeyDown;
			_recordsListBox.SelectedIndexChanged +=OnRecordSelectionChanged;
			_recordsListBox.Enter += _recordsListBox_Enter;
			_recordsListBox.Leave += _recordsListBox_Leave;
			UpdateDisplay();

			_programmaticallyGoingToNewEntry = false;
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
					LexEntry record = ((CachedSortedDb4oList<string, LexEntry>)_records).
						GetValue(CurrentIndex);

					return record;
				}
				catch (Exception e)
				{
					WeSay.Project.WeSayWordsProject.Project.HandleProbableCacheProblem(e);
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
			RegisterFieldWithPicker(Field.FieldNames.SenseGloss.ToString());
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
					Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
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

			LexEntrySortHelper sortHelper =
					new LexEntrySortHelper(_recordManager.DataSource,
										   _listWritingSystem,
										   IsWritingSystemUsedInLexicalForm(
												   _listWritingSystem));
			CachedSortedDb4oList<string, LexEntry> cachedSortedDb4oList = this._recordManager.GetSortedList(sortHelper);
			_records = cachedSortedDb4oList;

			_recordsListBox.DataSource = _records;

			Control_EntryDetailPanel.DataSource = CurrentRecord;
			_recordsListBox.WritingSystem = _listWritingSystem;

			_findText.ItemFilterer = FindClosestAndNextClosestAndPrefixedForms;
			_findText.Items = _records;
			_findText.WritingSystem = _listWritingSystem;

			int originalHeight = _findText.Height;

			_findWritingSystemId.Text = _listWritingSystem.Id;
			int width = _findWritingSystemId.Width;
			_findWritingSystemId.AutoSize = false;
			_findWritingSystemId.Height = _findText.Height;
			_findWritingSystemId.Width = Math.Min(width, 25);

			int heightDifference = _findText.Height - originalHeight;
			_recordsListBox.Location = new Point(_recordsListBox.Location.X,
												 _recordsListBox.Location.Y +
												 heightDifference);
			_recordsListBox.Height -= heightDifference;
			_btnFind.Height = _findText.Height;
			_writingSystemChooser.Height = _findText.Height;
			_btnFind.Image =
					Resources.Find.GetThumbnailImage(_btnFind.Width - 2,
													 _btnFind.Width - 2,
													 ReturnFalse,
													 IntPtr.Zero);

			_btnFind.Left = _writingSystemChooser.Left - _btnFind.Width;
			_findText.Width = _btnFind.Left - _findText.Left;
			_findText.PopupWidth = _recordsListBox.Width;
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
				FindInList(_findText.Text);
			}
		}

		private void _findText_AutoCompleteChoiceSelected(object sender,
														  EventArgs e)
		{
			FindInList(_findText.Text);
		}

		private void OnFind_Click(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("FindButton_Click");

			FindInList(_findText.Text);
		}

		public void GoToEntry(string entryId)
		{
			LexEntry entry = Lexicon.FindFirstLexEntryMatchingId(entryId);
			if (entry == null)
			{
				throw new NavigationException("Could not find the entry with id " + entryId);
			}
		   int index =_records.IndexOf(entry);
			if(index <0)
			{
				throw new NavigationException("The requested entry was found in the database, but the ui could not display it.");
			}

			_programmaticallyGoingToNewEntry = true;
			_recordsListBox.SelectedIndex = index;
			_programmaticallyGoingToNewEntry = false;
		}

		private bool FindInList(string text)
		{
			Logger.WriteMinorEvent("FindInList");
			int index =
					((CachedSortedDb4oList<string, LexEntry>) _records).
							BinarySearch(text);
			if (index < 0)
			{
				index = ~index;
				if (index == _records.Count && index != 0)
				{
					index--;
				}
			}
			if (0 <= index && index < _recordsListBox.Items.Count)
			{
				_recordListBoxActive = true; // allow onRecordSelectionChanged
				_recordsListBox.SelectedIndex = index;
				_recordListBoxActive = false;
				return true;
			}
			else
			{
				return false;
			}
		}

		private void _recordsListBox_Enter(object sender, EventArgs e)
		{
			_recordListBoxActive = true;
		}

		private void _recordsListBox_Leave(object sender, EventArgs e)
		{
			_recordListBoxActive = false;
		}

		private void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			if (Control_EntryDetailPanel.DataSource == CurrentRecord)
			{
				//we were getting 3 calls to this for each click on a new word
				return;
			}
			if (!_recordListBoxActive && !_programmaticallyGoingToNewEntry)
			{
				// When we change the content of the displayed string,
				// Windows.Forms.ListBox removes the item (and sends
				// the new selection event) then adds it in to the right
				// place (and sends the new selection event again)
				// We don't want to know about this case
				// We only want to know about the case where the user
				// has selected a record in the list box itself (so has to enter
				// the list box first)
				return;
			}

			if (CurrentRecord != null)
			{
				Logger.WriteEvent("RecordSelectionChanged to " +
								  CurrentRecord.LexicalForm.GetFirstAlternative());
				Control_EntryDetailPanel.DataSource = CurrentRecord;
			}
			else
			{
				Logger.WriteEvent(
						"RecordSelectionChanged Skipping because record is null");
			}

			SelectedIndexChanged.Invoke(this, null);
			UpdateDisplay();
		}

		private void OnNewWord_Click(object sender, EventArgs e)
		{
			Logger.WriteEvent("NewWord_Click");

			if (!_btnNewWord.Focused)
			{
				// if we use a hot key, it may not have received the focus
				// but we assume it has the focus when we do our selection change event
				_btnNewWord.Focus();
			}
			LexEntry entry = new LexEntry();
			bool NoPriorSelection = _recordsListBox.SelectedIndex == -1;
			_recordListBoxActive = true; // allow onRecordSelectionChanged
			_records.Add(entry);
			_recordsListBox.SelectedIndex = _records.IndexOf(entry);
			if (NoPriorSelection)
			{
				// Windows.Forms.Listbox does not consider it a change of Selection
				// index if the index was -1 and a record is added.
				// (No event is sent so we must do it ourselves)
				OnRecordSelectionChanged(this, null);
			}
			_recordListBoxActive = false;
			UpdateDisplay();
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
			_recordListBoxActive = true; // allow onRecordSelectionChanged
		   CurrentRecord.IsBeingDeleted = true;
			_records.RemoveAt(CurrentIndex);
			OnRecordSelectionChanged(this, null);
			_recordListBoxActive = false;

			if (CurrentRecord == null)
			{
				Control_EntryDetailPanel.DataSource = CurrentRecord;
			}
			UpdateDisplay();
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
