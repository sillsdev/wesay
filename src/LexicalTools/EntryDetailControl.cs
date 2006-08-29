using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class EntryDetailControl : UserControl,ITask
	{

		private IBindingList _records;

		public EntryDetailControl(IBindingList records)
		{
			_records = records;
			this.BindingContext[_records].PositionChanged += new EventHandler(OnCurrentRecordChanged);

			InitializeComponent();
			_detailPanel.BackColor = SystemColors.Control;//we like it to stand out at design time, but not runtime

			_recordsListBox.DataSource = records;

			_recordsListBox.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			this.BindingContext[_records].Position = _recordsListBox.SelectedIndex;
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
			Object record = CurrentRecord ;
			if (record == null)
			{
				return;
			}

			LexEntryLayouter layout = new LexEntryLayouter(this._detailPanel);
			layout.AddWidgets(record);

			this._detailPanel.ResumeLayout(true);
			this.ResumeLayout(false);
		}

		private Object CurrentRecord
		{
			get
			{
				return this.BindingContext[_records].Current as LexEntry;
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{

		}
	}
}
