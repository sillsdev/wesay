using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class LexFieldTask : UserControl, ITask
	{

		private IBindingList _records;
		private string _label;
		private int _currentIndex;

		public LexFieldTask(IBindingList records, string label, Predicate<string> filter)
		{
			if (records == null)
			{
				throw new ArgumentNullException("records");
			}
			if (label == null)
			{
				throw new ArgumentNullException("label");
			}
			_records = records;
			_label = label;

			InitializeComponent();
			_lexFieldDetailPanel.BackColor = SystemColors.Control;//we like it to stand out at design time, but not runtime
			_lexFieldDetailPanel.Control_EntryDetail.BackColor = SystemColors.Control;
			_lexFieldDetailPanel.ShowField = filter;
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			_currentIndex = _recordsListBox.SelectedIndex;
			_lexFieldDetailPanel.DataSource = CurrentRecord;
		}


		public void Activate()
		{
			_recordsListBox.DataSource = _records;
			_lexFieldDetailPanel.DataSource = CurrentRecord;

			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);

			_recordsListBox.Refresh();
			_lexFieldDetailPanel.Refresh();
		}


		public void Deactivate()
		{
			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
		}

		public string Label
		{
			get
			{
				return _label;
			}
		}

		public Control Control
		{
			get
			{
				return this;
			}
		}

		public IBindingList DataSource
		{
			get
			{
				return _records;
			}
		}

		public LexFieldControl Control_Details
		{
			get
			{
				return _lexFieldDetailPanel;
			}
		}
		/// <summary>
		/// Gets current record as selected in record list
		/// </summary>
		/// <value>null if record list is empty</value>
		private LexEntry CurrentRecord
		{
			get
			{
				if (_records.Count == 0)
				{
					return null;
				}
				return _records[_currentIndex] as LexEntry;
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{

		}
	}
}
