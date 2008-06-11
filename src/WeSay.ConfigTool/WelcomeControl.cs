using System;
using System.Diagnostics;
using System.Drawing;
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

			if (Settings.Default.MruConfigFilePaths.Paths.Length != 0)
		   {
			   CreateRecentProjectsList();
		   }
		   else
		   {
			   this.firstCellPanel.Visible = false;
			   this.tableLayoutPanel1.ColumnStyles[0].Width = 0;
			   this.openDifferentProject.Text = "Open Existing Project";
		   }
		}

		private void CreateRecentProjectsList()
		{
			bool haveProcessedTopMostProject = false;
			foreach (string path in Settings.Default.MruConfigFilePaths.Paths)
			{
				LinkLabel recentProjectLabel = new LinkLabel();
				recentProjectLabel.Text = Path.GetFileNameWithoutExtension(path);
				recentProjectLabel.AutoSize = true;
				recentProjectLabel.LinkColor = Color.Black;
				recentProjectLabel.LinkBehavior = LinkBehavior.HoverUnderline;
				if (!haveProcessedTopMostProject)
				{
					recentProjectLabel.Font = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
					haveProcessedTopMostProject = true;
				}
				else
				{
					recentProjectLabel.Font = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				}
				recentProjectLabel.Tag = path;
				recentProjectLabel.LinkClicked += openRecentProject_LinkClicked;
				flowLayoutPanel2.Controls.Add(recentProjectLabel);
			}
		}

		private void openRecentProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (OpenPreviousProjectClicked != null)
			{
				OpenPreviousProjectClicked.Invoke(((LinkLabel)sender).Tag, null);
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
			if (flowLayoutPanel2.Controls.Count != 0)
			{
				flowLayoutPanel2.Controls[0].Focus();
			}
			else
			{
				openDifferentProject.Focus();
			}
		}

		private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}
