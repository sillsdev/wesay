using System;

using System.IO;
using Chorus.sync;

namespace WeSay.ConfigTool
{
	public partial class BackupPlanControl: ConfigurationControlBase
	{

		public BackupPlanControl(): base("set up backup plan")
		{
			InitializeComponent();
			_syncPanel.ProjectFolderConfig = new ProjectFolderConfiguration(Project.BasilProject.Project.ProjectDirectoryPath);
			_historyPanel.ProjectFolderConfig = _syncPanel.ProjectFolderConfig;
		}
	}
}