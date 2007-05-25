using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WeSay.AddinLib
{
	public partial class ActionSetupDialog : Form
	{
		public ActionSetupDialog(Object settings)
		{
			InitializeComponent();
			_propertyGrid.SelectedObject = settings;
		}


		private void _btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}