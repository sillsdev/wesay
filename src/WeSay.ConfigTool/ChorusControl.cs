using System;
using System.IO;
using Chorus.sync;

namespace WeSay.ConfigTool
{
	public partial class ChorusControl : ConfigurationControlBase
	{
		public ChorusControl()
			: base("set up synchronization with team members")
		{
			InitializeComponent();
			_syncPanel.ProjectFolderConfig = new ProjectFolderConfiguration(Project.BasilProject.Project.ProjectDirectoryPath);
			_historyPanel.ProjectFolderConfig = _syncPanel.ProjectFolderConfig;
		}

		private void syncPanel1_Load(object sender, EventArgs e)
		{
		}

	}
}
