using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reporting
{
	public partial class RequestUserIdentifier : Form
	{
		public RequestUserIdentifier()
		{
			InitializeComponent();
			UpdateThings();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			this.Close();
		}

		public string EmailAddress
		{
			get
			{
				return _emailAddress.Text;
			}
		}

		private void _emailAddress_TextChanged(object sender, EventArgs e)
		{
			UpdateThings();
		}

		private void UpdateThings()
		{
			_okButton.Enabled = _emailAddress.Text.Trim().Length > 4;
		}

		private void label2_Click(object sender, EventArgs e)
		{

		}
	}
}