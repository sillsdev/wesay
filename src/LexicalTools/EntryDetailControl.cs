using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.TreeViewIList;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class EntryDetailControl : UserControl,ITask
	{

		private IBindingList _records;
		private System.Windows.Forms.BindingSource _bindingSource; //could get by with CurrencyManager


		public EntryDetailControl(IBindingList records)
		{
			_records = records;
			_bindingSource = new System.Windows.Forms.BindingSource(_records, null);
			_bindingSource.PositionChanged += new EventHandler(OnCurrentRecordChanged);

			InitializeComponent();
			_detailPanel.BackColor = SystemColors.Control;//we like it to stand out at design time, but not runtime

			_recordsListBox.DataSource = records;

			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			_bindingSource.Position = _recordsListBox.SelectedIndex;
		}

		void OnCurrentRecordChanged(object sender, EventArgs e)
		{
			BuildDetailArea();
		}

		public void Activate()
		{
			BuildDetailArea();
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

		public void Add(Control c)
		{
			c.Dock = DockStyle.Top;
			_detailPanel.Controls.Add(c);
		}


		private void BuildDetailArea()
		{
			this.SuspendLayout();
			this._detailPanel.SuspendLayout();
			_detailPanel.Controls.Clear();
			LexEntry record = _bindingSource.Current as LexEntry;
			if (record == null)
			{
				return;
			}

			LexEntryLayouter layout = new LexEntryLayouter(this._detailPanel);
			layout.AddWidgets(record);

			this._detailPanel.ResumeLayout(true);
			this.ResumeLayout(false);
		}


		private void button2_Click(object sender, EventArgs e)
		{

		}
	}
}
