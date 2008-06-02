using System;
using System.IO;
using System.Windows.Forms;
using Settings=WeSay.ConfigTool.Properties.Settings;

namespace WeSay.ConfigTool
{
	public partial class WelcomeControl : UserControl
	{
		public event EventHandler NewProjectClicked;
		public event EventHandler OpenPreviousProjectClicked;
		public event EventHandler ChooseProjectClicked;

		public WelcomeControl()
		{
			InitializeComponent();

		   string s = Settings.Default.LastConfigFilePath;
		   if (s != null && s.Length > 0 && File.Exists(s))
		   {
			   this.openRecentProject.Text = "Open " + Path.GetFileNameWithoutExtension(s);
		   }
		   else
		   {
			   this.openRecentProject.Visible = false;
			   this.openDifferentProject.Text = "Open Existing Project";
		   }
		}

		private void openRecentProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (OpenPreviousProjectClicked != null)
			{
				OpenPreviousProjectClicked.Invoke(Settings.Default.LastConfigFilePath, null);
			}

		}

		private void openDifferentProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (ChooseProjectClicked  != null)
			{
				ChooseProjectClicked.Invoke(this, null);
			}
		}

		private void createNewProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			 if (NewProjectClicked != null)
			{
				NewProjectClicked.Invoke(this, null);
			}
		}

		private void WelcomeControl_Load(object sender, EventArgs e)
		{
			if (openRecentProject.Visible)
			{
				openRecentProject.Focus();
			}
			else
			{
				openDifferentProject.Focus();
			}
		}
	}
}
