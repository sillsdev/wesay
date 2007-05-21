using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WeSay.Setup
{
	public partial class ProjectTabs : UserControl
	{
		public ProjectTabs()
		{
//            if (DesignMode)
//                WeSayWordsProject.InitializeForTests();
			InitializeComponent();
			this.Resize += new EventHandler(ProjectTabs_Resize);
		}

		void ProjectTabs_Resize(object sender, EventArgs e)
		{
			TryToFixScaling(_writingSystemSetupControl);
			TryToFixScaling(fieldsControl1);
			TryToFixScaling(_taskListControl);
			TryToFixScaling(_optionListPage);
			TryToFixScaling(_actionsPage);
		}

		//seems to help with some, not with others
		private void TryToFixScaling(Control c)
		{
			//this is part of dealing with .net not adjusting stuff well for different dpis
			c.Dock = DockStyle.None;
			c.Size = new Size(this.Width, this.Height-25);
		   // c.BackColor = System.Drawing.Color.Crimson;
		}
	}
}
