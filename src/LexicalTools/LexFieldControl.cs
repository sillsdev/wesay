using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ListBox;
using SourceGrid3;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;
using WeSay.Data;

namespace WeSay.LexicalTools
{
	public partial class LexFieldControl : UserControl
	{
		private IRecordList<LexEntry> _records;
		private InMemoryBindingList<LexEntry> _completedRecords;
		private LexEntry _currentRecord;
		private LexEntry _previousRecord;
		private LexEntry _nextRecord;

		private readonly ViewTemplate _viewTemplate;
		private readonly Predicate<LexEntry> _isNotComplete;
		public event EventHandler SelectedIndexChanged;

		public LexFieldControl(IRecordList<LexEntry> records, ViewTemplate viewTemplate, Predicate<LexEntry> isNotComplete)
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
			_records.ListChanged += OnRecordsListChanged;
			_completedRecords = new InMemoryBindingList<LexEntry>();
			_viewTemplate = viewTemplate;
			_isNotComplete = isNotComplete;
			BackColor = DisplaySettings.Default.BackgroundColor;
			_lexFieldDetailPanel.BackColor = DisplaySettings.Default.BackgroundColor;//we like it to stand out at design time, but not runtime
			_lexFieldDetailPanel.KeyDown += new KeyEventHandler(OnKeyDown);
			_lexFieldDetailPanel.ViewTemplate = _viewTemplate;

			_recordsListBox.DataSource = _records;
			_recordsListBox.Font = BasilProject.Project.WritingSystems.TestGetWritingSystemVern.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns.StretchToFit();
			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
			_recordsListBox.Selection.BorderMode = SelectionBorderMode.None;
			_recordsListBox.Columns[0].Width = _recordsListBox.Width - _recordsListBox.VScrollBar.Width;

			_completedRecordsListBox.DataSource = _completedRecords;
			_completedRecordsListBox.Font = BasilProject.Project.WritingSystems.TestGetWritingSystemVern.Font;
			_completedRecordsListBox.AutoSize();
			_completedRecordsListBox.Columns.StretchToFit();
			_completedRecordsListBox.SelectedIndexChanged += new EventHandler(OnCompletedRecordSelectionChanged);
			_completedRecordsListBox.BackColor = DisplaySettings.Default.BackgroundColor;
			_completedRecordsListBox.ForeColor = DisplaySettings.Default.BackgroundColor;
			_completedRecordsListBox.Selection.BorderMode = SelectionBorderMode.None;
			_completedRecordsListBox.Columns[0].Width = _completedRecordsListBox.Width - _completedRecordsListBox.VScrollBar.Width;

			this.labelNextHotKey.BringToFront();
			this._btnNextWord.BringToFront();
			this._btnPreviousWord.BringToFront();
			SetCurrentRecordFromRecordList();
		}

		private void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			SetCurrentRecordFromRecordList();
			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged.Invoke(this,null);
			}
		}

		public void SetCurrentRecordToNext()
		{
			if (_records.Count > 0)
			{
				CurrentRecord = _nextRecord ?? _records[_records.Count-1];
				SelectCurrentRecordInRecordList();
				this._lexFieldDetailPanel.Focus();
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
				CurrentRecord = _previousRecord ?? _records[0];
				SelectCurrentRecordInRecordList();
				this._lexFieldDetailPanel.Focus();
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
			SwitchPrimaryRecordListControl(_recordsListBox);

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

		private void SwitchPrimaryRecordListControl(BindingListGrid list)
		{
			BindingListGrid otherList = _recordsListBox;
			if(list == _recordsListBox)
			{
				otherList = _completedRecordsListBox;
			}

			list.Selection.BackColor = list.Selection.FocusBackColor;
			otherList.Selection.BackColor = Color.White;
		}

		private void OnCompletedRecordSelectionChanged(object sender, EventArgs e)
		{
			SetCurrentRecordFromCompletedRecordList();
			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged.Invoke(this, null);
			}
		}

		private void SetCurrentRecordFromCompletedRecordList()
		{
			SwitchPrimaryRecordListControl(_completedRecordsListBox);

			if (this._completedRecords.Count == 0)
			{
				CurrentRecord = null;
			}
			else
			{
				CurrentRecord = _completedRecords[CompletedRecordListCurrentIndex];
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

		public LexPreviewWithEntryControl ControlDetails
		{
			get { return _lexFieldDetailPanel; }
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
				if(_currentRecord != null)
				{
					_currentRecord.PropertyChanged -= OnCurrentRecordPropertyChanged;
				}
				_currentRecord = value;
				_lexFieldDetailPanel.DataSource = value;
				if (_currentRecord != null)
				{
					_currentRecord.PropertyChanged += OnCurrentRecordPropertyChanged;
					_congratulationsControl.Hide();
				}
			}
		}

		void OnRecordsListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
			if(e.ListChangedType == System.ComponentModel.ListChangedType.ItemAdded)
			{
				SelectCurrentRecordInRecordList();
			}
		}

		private void SelectCurrentRecordInRecordList() {
			int index = this._records.IndexOf(this._currentRecord);
			Debug.Assert(index != -1);
			this._recordsListBox.Selection.Clear();
			this._recordsListBox.Selection.SelectRow(index, true);
			this._recordsListBox.ShowCell(new Position(index, 0));
			SwitchPrimaryRecordListControl(_recordsListBox);
		}

		void OnCurrentRecordPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Debug.Assert(sender == CurrentRecord);
			LexEntry entry = (LexEntry)sender;
			if (_isNotComplete(entry))
			{
				if (_completedRecords.Contains(entry))
				{
					this._completedRecordsListBox.Selection.Clear();
					this._completedRecords.Remove(entry);
				   // SwitchPrimaryRecordListControl(_recordsListBox);
				}
			}
			else
			{
			   if (!_completedRecords.Contains(entry))
				{
					this._completedRecords.Add(entry);
					SelectCurrentRecordInCompletedRecordList();
					//SwitchPrimaryRecordListControl(_completedRecordsListBox);
				}
			}
		}

		private void SelectCurrentRecordInCompletedRecordList() {
			int index = this._completedRecords.IndexOf(CurrentRecord);
			Debug.Assert(index != -1);
			this._completedRecordsListBox.Selection.Clear();
			this._completedRecordsListBox.Selection.SelectRow(index, true);
			this._completedRecordsListBox.ShowCell(new Position(index, 0));
			SwitchPrimaryRecordListControl(_completedRecordsListBox);
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
