using System;
using System.Windows.Forms;

namespace Addin.Backup
{
	public partial class SendProjectEmailSettingsDialog : Form
	{
		private SendProjectEmailSettings _settings;

		internal SendProjectEmailSettingsDialog(SendProjectEmailSettings settings)
		{
			_settings = settings;
			InitializeComponent();
			_email.Text = settings.Email;
			_name.Text = settings.RecipientName;
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			_settings.Email = _email.Text.Trim();
			_settings.RecipientName = _name.Text.Trim();
			Close();
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}


	}
}