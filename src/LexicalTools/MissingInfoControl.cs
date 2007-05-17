using System;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;
using WeSay.Data;

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

		public MissingInfoControl(IRecordList<LexEntry> records, ViewTemplate viewTemplate, Predicate<LexEntry> isNotComplete)
		{
			InitializeComponent();
			if (DesignMode)
			{
				return;
			}

			if (records == null)
			{
				throw new ArgumentNullException("records");
			}
			if(viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			if(isNotComplete == null)
			{
				throw new ArgumentNullException("isNotComplete");
			}

			_records = records;
			_completedRecords = new InMemoryBindingList<LexEntry>();
			_viewTemplate = viewTemplate;
			_isNotComplete = isNotComplete;
			InitializeDisplaySettings();
			_entryViewControl.KeyDown += new KeyEventHandler(OnKeyDown);
			_entryViewControl.ViewTemplate = _viewTemplate;

			_recordsListBox.DataSource = _records;
			_records.ListChanged += OnRecordsListChanged; // this needs to be after so it will get change event after the ListBox

			WritingSystem listWritingSystem = BasilProject.Project.WritingSystems.UnknownVernacularWritingSystem;

			Field field = _viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
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

			this._recordsListBox.BorderStyle = BorderStyle.None;
			this._recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
			this._recordsListBox.Enter += new EventHandler(_recordsListBox_Enter);
			this._recordsListBox.Leave += new EventHandler(_recordsListBox_Leave);
			this._recordsListBox.WritingSystem = listWritingSystem;

			this._completedRecordsListBox.DataSource = _completedRecords;
			this._completedRecordsListBox.BorderStyle = BorderStyle.None;
			this._completedRecordsListBox.SelectedIndexChanged += new EventHandler(OnCompletedRecordSelectionChanged);
			this._completedRecordsListBox.Enter += new EventHandler(_completedRecordsListBox_Enter);
			this._completedRecordsListBox.Leave += new EventHandler(_completedRecordsListBox_Leave);
			this._completedRecordsListBox.WritingSystem = listWritingSystem;

			this.labelNextHotKey.BringToFront();
			this._btnNextWord.BringToFront();
			this._btnPreviousWord.BringToFront();
			SetCurrentRecordFromRecordList();
		}

		private bool _recordsListBoxActive;
		private bool _completedRecordsListBoxActive;

		void _recordsListBox_Leave(object sender, EventArgs e)
		{
			_recordsListBoxActive = false;
		}

		void _recordsListBox_Enter(object sender, EventArgs e)
		{
			_recordsListBoxActive = true;
		}

		void _completedRecordsListBox_Leave(object sender, EventArgs e)
		{
			_completedRecordsListBoxActive = false;
		}

		void _completedRecordsListBox_Enter(object sender, EventArgs e)
		{
			_completedRecordsListBoxActive = true;
		}

		private void InitializeDisplaySettings() {
			BackColor = DisplaySettings.Default.BackgroundColor;
			_entryViewControl.BackColor = DisplaySettings.Default.BackgroundColor;//we like it to stand out at design time, but not runtime
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
				SelectedIndexChanged.Invoke(this,null);
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
				CurrentRecord = _nextRecord ?? _records[_records.Count-1];
				SelectCurrentRecordInRecordList();
				this._recordsListBox.Focus(); // change the focus so that the next focus event will for sure work
				this._entryViewControl.Focus();
				UpdatePreviousAndNextRecords();
			}
			else
			{
				CurrentRecord = null;
				_congratulationsControl.Show(StringCatalog.Get("Congratulations. You have completed this task."));
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
				this._recordsListBox.Focus(); // change the focus so that the next focus event will for sure work
				this._entryViewControl.Focus();
				UpdatePreviousAndNextRecords();
			}
		}

		private void UpdatePreviousAndNextRecords()
		{
			int currentIndex = RecordListCurrentIndex;
			_previousRecord = (currentIndex > 0) ? _records[currentIndex - 1]: null;
			_nextRecord = (currentIndex < _records.Count - 1) ? _records[currentIndex + 1] : null;
		}

		private void SetCurrentRecordFromRecordList()
		{
			ClearSelectionForCompletedRecordsListBox();

			if (this._records.Count == 0)
			{
				CurrentRecord = null;
				_congratulationsControl.Show(StringCatalog.Get("There is no work left to be done on this task."));
			}
			else
			{
				CurrentRecord = _records[RecordListCurrentIndex];
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
			if (this._completedRecords.Count == 0)
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
			get
			{
				return _currentRecord;
			}
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

		void OnRecordsListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
			if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemAdded)
			{
				SelectCurrentRecordInRecordList();
			}
			else if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted)
			{
				ClearSelectionForRecordsListBox();
			}
		}

		private void SelectCurrentRecordInRecordList()
		{
			int index = this._records.IndexOf(CurrentRecord);
			Debug.Assert(index != -1);
			this._recordsListBox.SelectedIndex = index;
			ClearSelectionForCompletedRecordsListBox();
		}

		void OnCurrentRecordPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			LexEntry entry = (LexEntry)sender;
			if (_isNotComplete(entry))
			{
				if (_completedRecords.Contains(entry))
				{
					this._completedRecords.Remove(entry);
					ClearSelectionForCompletedRecordsListBox();
				}
			}
			else
			{
			   if (!_completedRecords.Contains(entry))
				{
					this._completedRecords.Add(entry);
					int index = this._completedRecords.IndexOf(CurrentRecord);
					Debug.Assert(index != -1);
					this._completedRecordsListBox.SelectedIndex = index;
					ClearSelectionForRecordsListBox();
				}
			}
		}

		private void ClearSelectionForCompletedRecordsListBox() {
			int topIndex = this._completedRecordsListBox.TopIndex;
			this._completedRecordsListBox.ClearSelected();
			this._completedRecordsListBox.ClearSelected();
			this._completedRecordsListBox.TopIndex = topIndex;
		}

		private void ClearSelectionForRecordsListBox()
		{
			int topIndex = this._recordsListBox.TopIndex;
			this._recordsListBox.ClearSelected();
			this._recordsListBox.ClearSelected();
			this._recordsListBox.TopIndex = topIndex;
		}

		void OnBtnPreviousWordClick(object sender, EventArgs e)
		{
			SetCurrentRecordToPrevious();
		}
		void OnBtnNextWordClick(object sender, EventArgs e)
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
				case Keys.PageDown:
					SetCurrentRecordToNext();
					break;

				default:
					e.Handled = false;
					if (Environment.OSVersion.Platform != PlatformID.Unix)
					{
						SetSuppressKeyPress(e, false);
					}
					break;
			}
		}
		private static void SetSuppressKeyPress(KeyEventArgs e, bool suppress)
		{
			e.SuppressKeyPress = suppress;
		}

	}
}
