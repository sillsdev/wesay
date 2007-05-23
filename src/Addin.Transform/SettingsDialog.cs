using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Addin.Transform
{
	public partial class SettingsDialog : Form
	{
		private Transform.TransformLift.TransformSettings _settings;

		public SettingsDialog(TransformLift.TransformSettings settings)
		{
			_settings = settings;
			InitializeComponent();
			//the xml serialization process seems to convert the \r\n we need (on windows) to \n
			_pairsText.Text = settings.SfmTagConversions.Replace("\n", System.Environment.NewLine);
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult= DialogResult.Cancel;
			Close();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_settings.SfmTagConversions = _pairsText.Text;

			DialogResult= DialogResult.OK;
			Close();
		}
	}
}