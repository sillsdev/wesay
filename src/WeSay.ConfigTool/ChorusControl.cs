using System.Diagnostics;
using System.Drawing;
using Palaso.Reporting;


namespace WeSay.ConfigTool
{
	public partial class ChorusControl : ConfigurationControlBase
	{
		public ChorusControl(ILogger logger)
			: base("set up synchronization with team members", logger)
		{
			this.Font = SystemFonts.MessageBoxFont;//use the default OS UI font
			InitializeComponent();

			var msg = Chorus.sync.RepositoryManager.GetEnvironmentReadinessMessage("en");
			if (string.IsNullOrEmpty(msg))
			{
				_chorusNotReady1.Visible = false;
				_chorusNotReady2.Visible = false;
				_chorusGetTortoiseLink.Visible = false;
				_chorusGetHgLink.Visible = false;
				_chorusReadinessMessage.Visible = false;
			}
			else
			{
				_chorusIsReady.Visible = false;
				_chorusReadinessMessage.Text = msg;
			}
//            _syncPanel.ProjectFolderConfig = new ProjectFolderConfiguration(Project.BasilProject.Project.ProjectDirectoryPath);
//            _historyPanel.ProjectFolderConfig = _syncPanel.ProjectFolderConfig;
		}



		private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(@"http://sourceforge.net/project/showfiles.php?group_id=199155");
		}

		private void _chorusGetHgLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(@"http://mercurial.selenic.com/wiki/BinaryPackages");
		}

	}
}
