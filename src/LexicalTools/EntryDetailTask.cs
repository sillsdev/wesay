using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class EntryDetailTask : UserControl,ITask
	{

		private IBindingList _records;
		private int _currentIndex;

		public EntryDetailTask(BasilProject project, IBindingList records)
		{
			_records = records;

			InitializeComponent();
			_entryDetailPanel.BackColor = SystemColors.Control;//we like it to stand out at design time, but not runtime

			_recordsListBox.DataSource = records;
			_entryDetailPanel.DataSource = CurrentRecord;
			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			_currentIndex = _recordsListBox.SelectedIndex;
			_entryDetailPanel.DataSource = CurrentRecord;
		}


		public void Activate()
		{
		}


		public void Deactivate()
		{

		}

		public string Label
		{
			get
			{
				return "Words";
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
				return _records[_currentIndex] as LexEntry;
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{

		}
	}
}
