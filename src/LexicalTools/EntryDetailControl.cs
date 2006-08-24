using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.LexicalModel;
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
			listBox1.DataSource = records;
		}

		void OnCurrentRecordChanged(object sender, EventArgs e)
		{
		}

		public void Activate()
		{
			AddDetailArea();
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


		private void AddDetailArea()
		{
			this._detailPanel.SuspendLayout();
			LexEntry record = _bindingSource.Current as LexEntry;
			if (record == null)
			{
				return;
			}

			LexEntryLayouter layout = new LexEntryLayouter(this._detailPanel);
			layout.AddWidgets(record);

			this._detailPanel.ResumeLayout(true);
			//this._detailPanel.Refresh();
		}
	}
}
