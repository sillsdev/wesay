using System;
using System.Windows.Forms;

namespace WeSay.AddinLib
{
	public partial class ActionSetupDialog: Form
	{
		public ActionSetupDialog(Object settings)
		{
			InitializeComponent();
			_propertyGrid.SelectedObject = settings;
		}

		private void _btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}