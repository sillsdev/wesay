using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
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
			this.label2.Text = String.Format(this.label2.Text, records.Count);
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
	}
}
