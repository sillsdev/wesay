using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class MissingInfoControl : UserControl
	{
		private IRecordList<LexEntry> _records;
		private InMemoryBindingList<LexEntry> _completedRecords;
		private LexEntry _currentRecord;
		private LexEntry _previousRecord;
		private LexEntry _nextRecord;

		private readonly ViewTemplate _viewTemplate;
		private readonly Predicate<LexEntry> _isNotComplete;
		public event EventHandler SelectedIndexChanged;

		public MissingInfoControl(IRecordList<LexEntry> records, ViewTemplate viewTemplate,
								  Predicate<LexEntry> isNotComplete, IRecordListManager recordListManager)
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
				if (recordListManager == null)
				{
					throw new ArgumentNullException("recordListManager");
				}
			}

			InitializeComponent();
			this.PreviewKeyDown += new PreviewKeyDownEventHandler(OnPreviewKeyDown);

			_btnNextWord.ReallySetSize(50, 50);
		   // _btnPreviousWord.ReallySetSize(30, 30);
			if (DesignMode)
			{
				return;
			}


			_records = records;
			_completedRecords = new InMemoryBindingList<LexEntry>();
			_viewTemplate = viewTemplate;
			_isNotComplete = isNotComplete;
			InitializeDisplaySettings();
			_entryViewControl.KeyDown += new KeyEventHandler(OnKeyDown);
			_entryViewControl.ViewTemplate = _viewTemplate;

			_entryViewControl.RecordListManager = recordListManager;

			_recordsListBox.DataSource = _records;
			_records.ListChanged += OnRecordsListChanged;
					// this needs to be after so it will get change event after the ListBox

			WritingSystem listWritingSystem = GetListWritingSystem();

			_recordsListBox.BorderStyle = BorderStyle.None;
			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
			_recordsListBox.Enter += new EventHandler(_recordsListBox_Enter);
			_recordsListBox.Leave += new EventHandler(_recordsListBox_Leave);
			_recordsListBox.WritingSystem = listWritingSystem;
			_completedRecordsListBox.DataSource = _completedRecords;
			_completedRecordsListBox.BorderStyle = BorderStyle.None;
			_completedRecordsListBox.SelectedIndexChanged += new EventHandler(OnCompletedRecordSelectionChanged);
			_completedRecordsListBox.Enter += new EventHandler(_completedRecordsListBox_Enter);
			_completedRecordsListBox.Leave += new EventHandler(_completedRecordsListBox_Leave);
			_completedRecordsListBox.WritingSystem = listWritingSystem;

			labelNextHotKey.BringToFront();
			_btnNextWord.BringToFront();
			_btnPreviousWord.BringToFront();
			SetCurrentRecordFromRecordList();
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
				this.SetCurrentRecordToPrevious();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		/// <summary>
		/// needed because the pos combo box blocks our access to enter, PageDown, etc.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e. KeyCode == Keys.PageDown)
			{
				e.IsInputKey = false;
				SetCurrentRecordToNext();
			}
			if (e.KeyCode == Keys.PageUp)
			{
				e.IsInputKey = false;
				this.SetCurrentRecordToPrevious();
			}
		}

		private WritingSystem GetListWritingSystem()
		{
			WritingSystem listWritingSystem = BasilProject.Project.WritingSystems.UnknownVernacularWritingSystem;

			// use the master view Template instead of the one for this task. (most likely the one for this
			// task doesn't have the EntryLexicalForm field specified but the Master (Default) one will
			Field field = WeSayWordsProject.Project.DefaultViewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());

			if (field != null)
			{
				if (field.WritingSystems.Count > 0)
				{
					listWritingSystem = field.WritingSystems[0];
				}
				else
				{
					MessageBox.Show(String.Format("There are no writing systems enabled for the Field '{0}'", field.FieldName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);//review
				}
			}
			return listWritingSystem;
		}

		private bool _recordsListBoxActive;
		private bool _completedRecordsListBoxActive;

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
			SetCurrentRecordFromRecordList();
			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged.Invoke(this, null);
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

			if (_records.Count > 0)
			{
				CurrentRecord = _nextRecord ?? _records[_records.Count - 1];
				SelectCurrentRecordInRecordList();
				_recordsListBox.Focus(); // change the focus so that the next focus event will for sure work
				_entryViewControl.Focus();
				UpdatePreviousAndNextRecords();
			}
			else
			{
				CurrentRecord = null;
				_congratulationsControl.Show(StringCatalog.Get("~Congratulations. You have completed this task."));
			}
		}

		public void SetCurrentRecordToPrevious()
		{
			if (_records.Count > 0)
			{
				if (!_btnPreviousWord.Focused)
				{
					// we need to make sure that any ghosts have lost their focus and triggered updates before
					// we do anything else
					_btnPreviousWord.Focus();
				}

				CurrentRecord = _previousRecord ?? _records[0];
				SelectCurrentRecordInRecordList();
				_recordsListBox.Focus(); // change the focus so that the next focus event will for sure work
				_entryViewControl.Focus();
				UpdatePreviousAndNextRecords();
			}
		}

		private void UpdatePreviousAndNextRecords()
		{
			int currentIndex = RecordListCurrentIndex;
			_previousRecord = (currentIndex > 0) ? _records[currentIndex - 1] : null;
			_nextRecord = (currentIndex < _records.Count - 1) ? _records[currentIndex + 1] : null;
		}

		private void SetCurrentRecordFromRecordList()
		{
			ClearSelectionForCompletedRecordsListBox();

			if (_records.Count == 0)
			{
				CurrentRecord = null;
				_congratulationsControl.Show(StringCatalog.Get("~There is no work left to be done on this task."));
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
					CurrentRecord = _records[RecordListCurrentIndex];
				}
				UpdatePreviousAndNextRecords();
			}
		}

		private void OnCompletedRecordSelectionChanged(object sender, EventArgs e)
		{
			if (!_completedRecordsListBoxActive)
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

			ClearSelectionForRecordsListBox();
			if (_completedRecords.Count == 0)
			{
				CurrentRecord = null;
			}
			else
			{
				CurrentRecord = _completedRecords[CompletedRecordListCurrentIndex];
			}

			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged.Invoke(this, null);
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
		public LexEntry CurrentRecord
		{
			get { return _currentRecord; }
			private set
			{
				if (_currentRecord != value)
				{
					if (_currentRecord != null)
					{
						_currentRecord.PropertyChanged -= OnCurrentRecordPropertyChanged;
					}
					_currentRecord = value;
					_entryViewControl.DataSource = value;
					if (_currentRecord != null)
					{
						_currentRecord.PropertyChanged += OnCurrentRecordPropertyChanged;
						_congratulationsControl.Hide();
					}
				}
			}
		}

		private void OnRecordsListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				SelectCurrentRecordInRecordList();
			}
			else if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				ClearSelectionForRecordsListBox();
			}
		}

		private void SelectCurrentRecordInRecordList()
		{
			int index = _records.IndexOf(CurrentRecord);
			Debug.Assert(index != -1);
			_recordsListBox.SelectedIndex = index;
			ClearSelectionForCompletedRecordsListBox();
		}

		private void OnCurrentRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			LexEntry entry = (LexEntry) sender;
			if (_isNotComplete(entry))
			{
				if (_completedRecords.Contains(entry))
				{
					_completedRecords.Remove(entry);
					ClearSelectionForCompletedRecordsListBox();
				}
			}
			else
			{
				if (!_completedRecords.Contains(entry))
				{
					_completedRecords.Add(entry);
					int index = _completedRecords.IndexOf(CurrentRecord);
					Debug.Assert(index != -1);
					_completedRecordsListBox.SelectedIndex = index;
					ClearSelectionForRecordsListBox();
				}
			}
		}

		private void ClearSelectionForCompletedRecordsListBox()
		{
			int topIndex = _completedRecordsListBox.TopIndex;
			_completedRecordsListBox.ClearSelected();
			_completedRecordsListBox.ClearSelected();
			_completedRecordsListBox.TopIndex = topIndex;
		}

		private void ClearSelectionForRecordsListBox()
		{
			int topIndex = _recordsListBox.TopIndex;
			_recordsListBox.ClearSelected();
			_recordsListBox.ClearSelected();
			_recordsListBox.TopIndex = topIndex;
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
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				SetSuppressKeyPress(e, true);
			}
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
					if (Type.GetType("Mono.Runtime") == null) // Work around not yet implemented in Mono
					{
						SetSuppressKeyPress(e, false);
					}
					break;
			}
		}

		private static void SetSuppressKeyPress(KeyEventArgs e, bool suppress)
		{
#if !MONO
			e.SuppressKeyPress = suppress;
#endif
		}
	}
}
