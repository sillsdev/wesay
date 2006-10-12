using System;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class EntryDetailControl : UserControl
	{
		private IRecordList<LexEntry> _records;
		private readonly FieldInventory _fieldInventory;
		public event EventHandler SelectedIndexChanged;

		public EntryDetailControl(IRecordList<LexEntry> records, FieldInventory fieldInventory)
		{
			if (records == null)
			{
				throw new ArgumentNullException("records");
			}
			if (fieldInventory == null)
			{
				throw new ArgumentNullException("fieldInventory");
			}
			_records = records;
			_fieldInventory = fieldInventory;
			InitializeComponent();
			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_entryDetailPanel.BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_entryDetailPanel.DataSource = CurrentRecord;

			_recordsListBox.DataSource = _records;
			_recordsListBox.Font = BasilProject.Project.WritingSystems.VernacularWritingSystemDefault.Font;
			_recordsListBox.AutoSize();
			_recordsListBox.Columns.StretchToFit();

			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			if (_entryDetailPanel.DataSource == CurrentRecord)
			{
				//we were getting 3 calls to this for each click on a new word
				return;
			}
			_entryDetailPanel.DataSource = CurrentRecord;
			_btnDeleteWord.Enabled = (CurrentRecord != null);
			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged.Invoke(this,null);
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
