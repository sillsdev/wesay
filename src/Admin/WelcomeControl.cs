using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WeSay.Admin
{
	public partial class WelcomeControl : UserControl
	{
		public event EventHandler NewProjectClicked;
		public event EventHandler OpenProjectClicked;

		public WelcomeControl()
		{
			InitializeComponent();
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
	}
}
