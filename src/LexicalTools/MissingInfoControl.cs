using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class MissingInfoControl: UserControl
	{
		private readonly BindingList<RecordToken<LexEntry>> _completedRecords;
		private readonly BindingList<RecordToken<LexEntry>> _todoRecords;
		private RecordToken<LexEntry> _currentRecord;
		private RecordToken<LexEntry> _previousRecord;
		private RecordToken<LexEntry> _nextRecord;

		private readonly ViewTemplate _viewTemplate;
		private readonly Predicate<LexEntry> _isNotComplete;
		public event EventHandler TimeToSaveRecord;

		public MissingInfoControl(ResultSet<LexEntry> records,
								  ViewTemplate viewTemplate,
								  Predicate<LexEntry> isNotComplete,
								  LexEntryRepository lexEntryRepository)
		{
			if (!DesignMode)
			{
				if (records == null)
				{
					throw new ArgumentNullException("records");
				}
				if (viewTemplate == null)
				{
					throw new ArgumentNullException("viewTemplate");
				}
				if (isNotComplete == null)
				{
					throw new ArgumentNullException("isNotComplete");
				}
				if (lexEntryRepository == null)
				{
					throw new ArgumentNullException("lexEntryRepository");
				}
			}

			InitializeComponent();
			PreviewKeyDown += OnPreviewKeyDown;

			_btnNextWord.ReallySetSize(50, 50);
			// _btnPreviousWord.ReallySetSize(30, 30);
			if (DesignMode)
			{
				return;
			}

			_completedRecords = new BindingList<RecordToken<LexEntry>>();
			_todoRecords = (BindingList<RecordToken<LexEntry>>) records;

			_viewTemplate = viewTemplate;
			_isNotComplete = isNotComplete;
			InitializeDisplaySettings();
			_entryViewControl.KeyDown += OnKeyDown;
			_entryViewControl.ViewTemplate = _viewTemplate;

			_entryViewControl.LexEntryRepository = lexEntryRepository;

			_recordsListBox.DataSource = _todoRecords;
			//            _records.ListChanged += OnRecordsListChanged;
			// this needs to be after so it will get change event after the ListBox

			WritingSystem listWritingSystem = GetListWritingSystem();

			_recordsListBox.BorderStyle = BorderStyle.None;
			_recordsListBox.SelectedIndexChanged += OnRecordSelectionChanged;
			_recordsListBox.MouseDown += _recordsListBox_MouseDown;
			_recordsListBox.Enter += _recordsListBox_Enter;
			_recordsListBox.Leave += _recordsListBox_Leave;
			_recordsListBox.RetrieveVirtualItem += OnRetrieveVirtualItemEvent;
			_recordsListBox.WritingSystem = listWritingSystem;

			_completedRecordsListBox.DataSource = _completedRecords;
			_completedRecordsListBox.BorderStyle = BorderStyle.None;
			_completedRecordsListBox.SelectedIndexChanged += OnCompletedRecordSelectionChanged;
			_completedRecordsListBox.MouseDown += _completedRecordsListBox_MouseDown;
			_completedRecordsListBox.Enter += _completedRecordsListBox_Enter;
			_completedRecordsListBox.Leave += _completedRecordsListBox_Leave;
			_completedRecordsListBox.WritingSystem = listWritingSystem;
			_completedRecordsListBox.RetrieveVirtualItem += OnRetrieveVirtualItemEvent;

			labelNextHotKey.BringToFront();
			_btnNextWord.BringToFront();
			_btnPreviousWord.BringToFront();
			SetCurrentRecordFromRecordList();
		}

		private void _completedRecordsListBox_MouseDown(object sender, MouseEventArgs e)
		{
			_completedRecordsListBox_Enter(sender, e);
		}

		private void _recordsListBox_MouseDown(object sender, MouseEventArgs e)
		{
			_recordsListBox_Enter(sender, e);
		}

		private void OnRetrieveVirtualItemEvent(object sender, RetrieveVirtualItemEventArgs e)
		{
			RecordToken<LexEntry> recordToken;
			if (sender == _recordsListBox)
			{
				recordToken = this._todoRecords[e.ItemIndex];
			}
			else
			{
				Debug.Assert(sender == _completedRecordsListBox);
				recordToken = this._completedRecords[e.ItemIndex];
			}
			string displayString = (string) recordToken["Form"];
			e.Item = new ListViewItem(displayString);
			if (!string.IsNullOrEmpty(displayString))
			{
				return;
			}

			displayString =
					recordToken.RealObject.LexicalForm.GetBestAlternative(
							_recordsListBox.WritingSystem.Id, string.Empty);
			e.Item.Font = new Font(e.Item.Font, FontStyle.Italic);

			if (string.IsNullOrEmpty(displayString))
			{
				displayString = "(" +
								StringCatalog.Get("~Empty",
												  "This is what shows for a word in a list when the user hasn't yet typed anything in for the word.  Like if you click the 'New Word' button repeatedly.") +
								")";
			}
			e.Item.Text = displayString;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Enter || keyData == Keys.PageDown)
			{
				SetCurrentRecordToNext();
				return true;
			}
			if (keyData == Keys.PageUp)
			{
				SetCurrentRecordToPrevious();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

		/// <summary>
		/// needed because the pos combo box blocks our access to enter, PageDown, etc.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.PageDown)
			{
				e.IsInputKey = false;
				SetCurrentRecordToNext();
			}
			if (e.KeyCode == Keys.PageUp)
			{
				e.IsInputKey = false;
				SetCurrentRecordToPrevious();
			}
		}

		private static WritingSystem GetListWritingSystem()
		{
			WritingSystemCollection writingSystems = BasilProject.Project.WritingSystems;
			WritingSystem listWritingSystem = writingSystems.UnknownVernacularWritingSystem;

			// use the master view Template instead of the one for this task. (most likely the one for this
			// task doesn't have the EntryLexicalForm field specified but the Master (Default) one will
			Field field =
					WeSayWordsProject.Project.DefaultViewTemplate.GetField(
							Field.FieldNames.EntryLexicalForm.ToString());

			if (field != null)
			{
				if (field.WritingSystemIds.Count > 0)
				{
					listWritingSystem = writingSystems[field.WritingSystemIds[0]];
				}
				else
				{
					throw new ConfigurationException("There are no writing systems enabled for the Field '{0}'",
												 field.FieldName);
				}
			}
			return listWritingSystem;
		}

		private bool _recordsListBoxActive;
		private bool _completedRecordsListBoxActive;
		private LexEntry _currentEntry;

		private void _recordsListBox_Leave(object sender, EventArgs e)
		{
			_recordsListBoxActive = false;
		}

		private void _recordsListBox_Enter(object sender, EventArgs e)
		{
			_recordsListBoxActive = true;
		}

		private void _completedRecordsListBox_Leave(object sender, EventArgs e)
		{
			_completedRecordsListBoxActive = false;
		}

		private void _completedRecordsListBox_Enter(object sender, EventArgs e)
		{
			_completedRecordsListBoxActive = true;
		}

		private void InitializeDisplaySettings()
		{
			BackColor = DisplaySettings.Default.BackgroundColor;
			_entryViewControl.BackColor = DisplaySettings.Default.BackgroundColor;
			//we like it to stand out at design time, but not runtime
		}

		private void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			if (!_recordsListBoxActive)
			{
				// We don't care about the case where a record moves from the
				// record list box to the completed records list box automatically
				// (although the selection will change)
				//
				// We only want to know about the case where the user
				// has selected a record in the list box itself (so has to enter
				// the list box first)
				return;
			}

			// only do something if an item is being selected (not deselected)
			if (_recordsListBox.SelectedIndex == -1)
			{
				return;
			}

			SetCurrentRecordFromRecordList();

		}

		private void SaveNow()
		{

					if (TimeToSaveRecord != null)
					{
						TimeToSaveRecord.Invoke(this, null);
					}

		}

		public void SetCurrentRecordToNext()
		{
			if (!_btnNextWord.Focused)
			{
				// we need to make sure that any ghosts have lost their focus and triggered updates before
				// we do anything else
				_btnNextWord.Focus();
			}

			if (_todoRecords.Count > 0)
			{
				CurrentRecord = _nextRecord ?? _todoRecords[_todoRecords.Count - 1];
				SelectCurrentRecordInTodoRecordList();
				_recordsListBox.Focus();
				// change the focus so that the next focus event will for sure work
				_entryViewControl.Focus();
				UpdatePreviousAndNextRecords();
			}
			else
			{
				CurrentRecord = null;
				_congratulationsControl.Show(
						StringCatalog.Get("~Congratulations. You have completed this task."));
			}
		}

		public void SetCurrentRecordToPrevious()
		{
			if (_todoRecords.Count > 0)
			{
				if (!_btnPreviousWord.Focused)
				{
					// we need to make sure that any ghosts have lost their focus and triggered updates before
					// we do anything else
					_btnPreviousWord.Focus();
				}

				CurrentRecord = _previousRecord ?? _todoRecords[0];
				SelectCurrentRecordInTodoRecordList();
				_recordsListBox.Focus();
				// change the focus so that the next focus event will for sure work
				_entryViewControl.Focus();
				UpdatePreviousAndNextRecords();
			}
		}

		private void UpdatePreviousAndNextRecords()
		{
			int currentIndex = RecordListCurrentIndex;
			_previousRecord = (currentIndex > 0) ? _todoRecords[currentIndex - 1] : null;
			_nextRecord = (currentIndex < _todoRecords.Count - 1)
								  ? _todoRecords[currentIndex + 1]
								  : null;
		}

		private void SetCurrentRecordFromRecordList()
		{
			ClearSelectionForCompletedRecordsListBox();

			if (_todoRecords.Count == 0)
			{
				CurrentRecord = null;
				_congratulationsControl.Show(
						StringCatalog.Get("~There is no work left to be done on this task."));
			}
			else
			{
				if (RecordListCurrentIndex == -1)
				{
					if (_recordsListBox.Items.Count > 0)
					{
						_recordsListBox.SelectedIndex = 0;
					}
				}
				if (RecordListCurrentIndex != -1)
				{
					CurrentRecord = _todoRecords[RecordListCurrentIndex];
				}
				UpdatePreviousAndNextRecords();
			}
		}

		private void OnCompletedRecordSelectionChanged(object sender, EventArgs e)
		{
			if (!_completedRecordsListBoxActive)
			{
				// We don't care about the case where a record moves from the
				// record list box to the completed records list box automatically
				// (although the selection will change)
				//
				// We only want to know about the case where the user
				// has selected a record in the list box itself (so has to enter
				// the list box first)
				return;
			}

			// only do something if an item is being selected (not deselected)
			if (_completedRecordsListBox.SelectedIndex == -1)
			{
				return;
			}

			ClearSelectionForRecordsListBox();
			if (_completedRecords.Count == 0)
			{
				CurrentRecord = null;
			}
			else
			{
				CurrentRecord = _completedRecords[CompletedRecordListCurrentIndex];
			}

			if (TimeToSaveRecord != null)
			{
				TimeToSaveRecord.Invoke(this, null);
			}
		}

		protected int RecordListCurrentIndex
		{
			get { return _recordsListBox.SelectedIndex; }
		}

		protected int CompletedRecordListCurrentIndex
		{
			get { return _completedRecordsListBox.SelectedIndex; }
		}

		public EntryViewControl EntryViewControl
		{
			get { return _entryViewControl; }
		}

		/// <summary>
		/// Sets current record as selected in record list or completed record list
		/// </summary>
		/// <value>null if record list is empty</value>
		public RecordToken<LexEntry> CurrentRecord
		{
			get { return _currentRecord; }
			private set
			{
				if (_currentRecord != value)
				{
					if (CurrentEntry != null)
					{
						CurrentEntry.PropertyChanged -= OnCurrentRecordPropertyChanged;
					}

					_currentRecord = value;
					if (_currentRecord == null)
					{
						_entryViewControl.DataSource = null;
					}
					else
					{
						CurrentEntry = _currentRecord.RealObject;
						CurrentEntry.PropertyChanged += OnCurrentRecordPropertyChanged;
						_entryViewControl.DataSource = CurrentEntry;
						_congratulationsControl.Hide();
					}
				}
			}
		}

		public LexEntry CurrentEntry
		{
			get { return _currentEntry; }
			private set { _currentEntry = value; }
		}

		//private void OnRecordsListChanged(object sender, ListChangedEventArgs e)
		//{
		//    switch(e.ListChangedType)
		//    {
		//        case ListChangedType.ItemAdded:
		//            SelectCurrentRecordInTodoRecordList();
		//            break;
		//        case ListChangedType.ItemDeleted:
		//            ClearSelectionForRecordsListBox();
		//            break;
		//        case ListChangedType.Reset:
		//            SelectCurrentRecordInTodoRecordList();
		//            break;
		//    }
		//}

		private void SelectCurrentRecordInTodoRecordList()
		{
			int index = _todoRecords.IndexOf(CurrentRecord);
			Debug.Assert(index != -1);
			_recordsListBox.SelectedIndex = index;
			ClearSelectionForCompletedRecordsListBox();
		}

		private void OnCurrentRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SaveNow();
			Debug.Assert(sender == CurrentEntry);
			if (_isNotComplete(CurrentEntry))
			{
				if (_completedRecords.Contains(_currentRecord))
				{

					_completedRecords.Remove(_currentRecord);
				}
				if (!_todoRecords.Contains(_currentRecord))
				{
					_todoRecords.Add(_currentRecord);
				}
				SelectCurrentRecordInTodoRecordList();
			}
			else
			{
				if (_todoRecords.Contains(_currentRecord))
				{
					_todoRecords.Remove(_currentRecord);
				}
				if (!_completedRecords.Contains(_currentRecord))
				{
					_completedRecords.Add(_currentRecord);
				}
				SelectCurrentRecordInCompletedRecordList();
			}
		}

		private void SelectCurrentRecordInCompletedRecordList()
		{
			int index = _completedRecords.IndexOf(_currentRecord);
			_completedRecordsListBox.SelectedIndex = index;
			ClearSelectionForRecordsListBox();
		}

		private void ClearSelectionForCompletedRecordsListBox()
		{
			_completedRecordsListBox.SelectedIndex = -1;
			_completedRecordsListBox.HideSelection = true;
			_recordsListBox.HideSelection = false;
		}

		private void ClearSelectionForRecordsListBox()
		{
			_recordsListBox.SelectedIndex = -1;
			_recordsListBox.HideSelection = true;
			_completedRecordsListBox.HideSelection = false;
		}

		private void OnBtnPreviousWordClick(object sender, EventArgs e)
		{
			SetCurrentRecordToPrevious();
		}

		private void OnBtnNextWordClick(object sender, EventArgs e)
		{
			SetCurrentRecordToNext();
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			e.SuppressKeyPress = true;
			switch (e.KeyCode)
			{
				case Keys.PageUp:
					SetCurrentRecordToPrevious();
					break;
				case Keys.Enter:
				case Keys.PageDown:
					SetCurrentRecordToNext();
					break;

				default:
					e.Handled = false;
					e.SuppressKeyPress = false;
					break;
			}
		}

		// hack to get around the fact that SplitContainer takes over the
		// tab order and doesn't allow you to specify that the controls in the
		// right pane should get the highest tab order.
		// this means the RTF view looks bad. Still haven't figured out how to make
		// cursor go to right position.
		private bool monoOnEnterFix;


		protected override void OnEnter(EventArgs e)
		{
			if (monoOnEnterFix)
			{
				return;
			}
			try
			{
				monoOnEnterFix = true;
				base.OnEnter(e);
				_entryViewControl.Select();
			}
			finally
			{
				monoOnEnterFix = false;
			}
		}
	}
}