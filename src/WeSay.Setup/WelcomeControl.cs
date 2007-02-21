using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WeSay.Setup
{
	public partial class WelcomeControl : UserControl
	{
		public event EventHandler NewProjectClicked;
		public event EventHandler OpenProjectClicked;

		public WelcomeControl()
		{
			InitializeComponent();

		   string s = WeSay.Setup.Properties.Settings.Default.LastProjectPath;
		   if (s != null && s.Length > 0 && Directory.Exists(s))
		   {
			   string[] directories = s.Split('\\');
			   this.openRecentProject.Text = "Open " + directories[directories.Length-1];
		   }
		   else
		   {
			   this.openRecentProject.Visible = false;
			   this.openDifferentProject.Text = "Open Existing Project";
		   }

		}

		private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			string s = e.Url.AbsolutePath;
			e.Cancel = true;
			if(s.IndexOf("CreateProject")>-1)
			{
				if (NewProjectClicked != null)
				{
					NewProjectClicked.Invoke(this, null);
				}
			}
			else if(s.IndexOf("OpenProject")>-1)
			{
				if (OpenProjectClicked != null)
				{
					OpenProjectClicked.Invoke(this, null);
				}
			}
		}


		private void openRecentProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		   if (OpenProjectClicked != null)
			{
				OpenProjectClicked.Invoke(WeSay.Setup.Properties.Settings.Default.LastProjectPath, null);
			}

		}

		private void openDifferentProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (OpenProjectClicked != null)
			{
				OpenProjectClicked.Invoke(this, null);
			}
		}

		private void createNewProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			 if (NewProjectClicked != null)
			{
				NewProjectClicked.Invoke(this, null);
			}
		}
	}
}
