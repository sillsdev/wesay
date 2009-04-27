using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WeSay.ConfigTool.NewProjectCreation;

namespace WeSay.ConfigTool.NewProjectDialogs
{
	public partial class NewProjectFromFLExDialog: NewProject
	{
		public NewProjectFromFLExDialog()
		{
			InitializeComponent();
			btnOK.Enabled = false;
			_pathLabel.Text = "";
		}

		public string PathToLift
		{
			get { return _liftPathTextBox.Text; }
		}


		protected override bool EnableOK
		{
			get { return base.EnableOK && !string.IsNullOrEmpty(PathToLift) && File.Exists(PathToLift) ; }
		}

		private void _launchWebPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://wesay.org/index.php?title=ShareWithFLEx");
		}

		private void _browseForLiftPathButton_Click(object sender, EventArgs e)
		{
			using(var dlg = new OpenFileDialog())
			{
				dlg.AutoUpgradeEnabled = true;
				dlg.Title = "Locate LIFT exported from FLEx";
				dlg.AutoUpgradeEnabled = true;
				dlg.RestoreDirectory = true;
				dlg.DefaultExt = ".lift";
				dlg.Filter = "LIFT Lexicon File (*.lift)|*.lift";

				if (System.Windows.Forms.DialogResult.OK != dlg.ShowDialog())
					return;

				_liftPathTextBox.Text = dlg.FileName;
			}
		}

		private void _liftPathTextBox_TextChanged(object sender, EventArgs e)
		{
			if(_textProjectName.Text==string.Empty && _liftPathTextBox.Text.Contains(".lift"))
			{
				try
				{
					_textProjectName.Text = Path.GetFileNameWithoutExtension(_liftPathTextBox.Text);
				}
				catch(Exception)
				{
				}
			}
			btnOK.Enabled = EnableOK;
		}


	}
}