using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ListBox;
using SourceGrid3;
using WeSay.LexicalModel;
using WeSay.UI;
using WeSay.Data;

namespace WeSay.LexicalTools
{
	public partial class LexFieldControl : UserControl
	{
		private IRecordList<LexEntry> _records;
		private InMemoryBindingList<LexEntry> _completedRecords;
		private LexEntry _currentRecord;

		private readonly FieldInventory _fieldInventory;
		private readonly Predicate<LexEntry> _isNotComplete;
		public event EventHandler SelectedIndexChanged;

		public LexFieldControl(IRecordList<LexEntry> records, FieldInventory fieldInventory, Predicate<LexEntry> isNotComplete)
		{
			if (records == null)
			{
				throw new ArgumentNullException("records");
			}
			if(fieldInventory == null)
			{
				throw new ArgumentNullException("fieldInventory");
			}
			_records = records;
			_records.ListChanged += OnRecordsListChanged;
			_completedRecords = new InMemoryBindingList<LexEntry>();
			_fieldInventory = fieldInventory;
			_isNotComplete = isNotComplete;

			InitializeComponent();
			_lexFieldDetailPanel.BackColor = DisplaySettings.Default.BackgroundColor;//we like it to stand out at design time, but not runtime

			_recordsListBox.DataSource = _records;
			_recordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns.StretchToFit();
			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
			_recordsListBox.Selection.BorderMode = SelectionBorderMode.None;

			_completedRecordsListBox.DataSource = _completedRecords;
			_completedRecordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_completedRecordsListBox.AutoSize();
			_completedRecordsListBox.Columns.StretchToFit();
			_completedRecordsListBox.SelectedIndexChanged += new EventHandler(OnCompletedRecordSelectionChanged);
			_completedRecordsListBox.BackColor = DisplaySettings.Default.BackgroundColor;
			_completedRecordsListBox.ForeColor = DisplaySettings.Default.BackgroundColor;
			_completedRecordsListBox.Selection.BorderMode = SelectionBorderMode.None;

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

		private void SetCurrentRecordFromRecordList()
		{
			SetCurrentRecordListControl(_recordsListBox);

			if (this._records.Count == 0)
			{
				CurrentRecord = null;
			}
			else
			{
				CurrentRecord = _records[RecordListCurrentIndex];
			}
		}

		private void SetCurrentRecordListControl(BindingListGrid list)
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
			SetCurrentRecordListControl(_completedRecordsListBox);

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
			get
			{
				return _completedRecordsListBox.SelectedIndex;
			}
		}

		public LexPreviewWithEntryControl ControlDetails
		{
			get
			{
				return _lexFieldDetailPanel;
			}
		}
		/// <summary>
		/// Sets current record as selected in record list or completed record list
		/// </summary>
		/// <value>null if record list is empty</value>
		private LexEntry CurrentRecord
		{
			set
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
				}
			}
		}

		void OnRecordsListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
			if(e.ListChangedType == System.ComponentModel.ListChangedType.ItemAdded)
			{
				int index = _records.IndexOf(_currentRecord);
				_recordsListBox.Selection.Clear();
				_recordsListBox.Selection.SelectRow(index, true);
				_recordsListBox.ShowCell(new Position(index, 0));
			}
		}

		void OnCurrentRecordPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			LexEntry entry = (LexEntry) sender;
			if (_isNotComplete(entry))
			{
				if (_completedRecords.Contains(entry))
				{
					_completedRecordsListBox.Selection.Clear();
					_completedRecords.Remove(entry);
					SetCurrentRecordListControl(_recordsListBox);
				}
			}
			else
			{
			   if (!_completedRecords.Contains(entry))
				{
					_completedRecords.Add(entry);
					int index = _completedRecords.IndexOf(entry);
					_completedRecordsListBox.Selection.Clear();
					_completedRecordsListBox.Selection.SelectRow(index, true);
					_completedRecordsListBox.ShowCell(new Position(index, 0));
					SetCurrentRecordListControl(_completedRecordsListBox);
				}
			}
		}
	}
}
