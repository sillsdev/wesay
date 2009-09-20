using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WeSay.ConfigTool
{
	public partial class UserSpecificSettingIndicator : UserControl
	{
		public UserSpecificSettingIndicator()
		{
			InitializeComponent();
		}

		private void UserSpecificSettingIndicator_Load(object sender, EventArgs e)
		{
			toolTip1.SetToolTip(_imageButton, String.Format("This is a user-specific setting, stored in {0}.", Environment.UserName+".WeSayUserConfig"));
			this.BackColor = this.Parent.BackColor;
		}

		private void _imageButton_Click(object sender, EventArgs e)
		{
			MessageBox.Show(toolTip1.GetToolTip(_imageButton));
		}
	}
}
