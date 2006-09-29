using System;
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
		private int _currentIndex;

		public EntryDetailTask(IRecordListManager recordListManager, string unusedLabel)
			: this(recordListManager)
		{

		}

		public EntryDetailTask(IRecordListManager recordListManager)
		{
			InitializeComponent();
			_records = recordListManager.Get<LexEntry>();

			_entryDetailPanel.BackColor = SystemColors.Control;//we like it to stand out at design time, but not runtime
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			_currentIndex = _recordsListBox.SelectedIndex;
			_entryDetailPanel.DataSource = CurrentRecord;
		}


		public void Activate()
		{
			_recordsListBox.DataSource = _records;
			_entryDetailPanel.DataSource = CurrentRecord;
			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);

			_recordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns.StretchToFit();
		}

		public void Deactivate()
		{
			_recordsListBox.SelectedIndexChanged -= new EventHandler(OnRecordSelectionChanged);
		}

		public string Label
		{
			get
			{
				return "Words";
			}
		}

		public string Description
		{
			get
			{
				return "Edit all relevant fields for lexical entries.";
			}
		}

		public Control Control
		{
			get { return this; }
		}

		private LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0)
				{
					return null;
				}
				return _records[_currentIndex];
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{

		}
	}
}
