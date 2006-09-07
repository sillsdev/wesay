using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class DashboardControl : UserControl, ITask
	{
		private IBindingList _records;
		private WeSay.UI.IProject _project;

		public DashboardControl(WeSay.UI.IProject project, IBindingList records)
		{
			_records = records;
			_project = project;
			InitializeComponent();
			this._dictionarySizeLabel.Text = String.Format(StringCatalog.Get(this._dictionarySizeLabel.Text), records.Count);
			this._projectNameLabel.Text = _project.Name;
		}

		public void Activate()
		{

		}

		public void Deactivate()
		{

		}

		public string Label
		{
			get { return "Dashboard";
				 }
		}

		public Control Control
		{
			get { return this; }
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}
	}
}
