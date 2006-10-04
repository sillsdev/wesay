using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class EntryDetailTask : UserControl,ITask
	{

		private IRecordList<LexEntry> _records;
		private bool _isActive;
//        private int _currentIndex;

		public EntryDetailTask(IRecordListManager recordListManager)
		{
			if (recordListManager == null)
			{
				throw new ArgumentNullException();
			}
			InitializeComponent();
			_records = recordListManager.Get<LexEntry>();


			_entryDetailPanel.BackColor = SystemColors.Control;//we like it to stand out at design time, but not runtime
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
//            _currentIndex = _recordsListBox.SelectedIndex;
			_entryDetailPanel.DataSource = CurrentRecord;
			_btnDeleteWord.Enabled = (CurrentRecord != null);
		}


		public void Activate()
		{
			if (IsActive)
			{
				throw new InvalidOperationException("Activate should not be called when object is active.");
			}

			_recordsListBox.DataSource = _records;
			_entryDetailPanel.DataSource = CurrentRecord;
			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);

			_recordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns.StretchToFit();
			_isActive = true;
		}

		public void Deactivate()
		{
			if (!IsActive)
			{
				throw new InvalidOperationException("Deactivate should only be called once after Activate.");
			}
			_recordsListBox.SelectedIndexChanged -= new EventHandler(OnRecordSelectionChanged);
			_isActive = false;
		}

		public bool IsActive
		{
			get { return this._isActive; }
		}

		public string Label
		{
			get
			{
				return StringCatalog.Get("Dictionary");
			}
		}

		public string Description
		{
			get
			{
				return String.Format(StringCatalog.Get("See all {0} {1} words."), _records.Count, BasilProject.Project.Name);
			}
		}

		public Control Control
		{
			get { return this; }
		}

		public bool IsPinned
		{
			get
			{
				return true;
			}
		}

		public string Status
		{
			get
			{
				return string.Empty;
			}
		}

		private LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0)
				{
					return null;
				}
				return _records[CurrentIndex];
			}
		}

		protected int CurrentIndex
		{
			get { return _recordsListBox.SelectedIndex; }
		}

		private void _btnNewWord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LexEntry entry = new LexEntry();
			_records.Add(entry);
			_recordsListBox.SelectedIndex = _records.IndexOf(entry);
		}

		private void _btnDeleteWord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Debug.Assert(CurrentIndex >= 0);
			_records.RemoveAt(CurrentIndex);
			//hack until we can get selection change events sorted out in BindingGridList
			OnRecordSelectionChanged(this, null);
		}
	}
}
