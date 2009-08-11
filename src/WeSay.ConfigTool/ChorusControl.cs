using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Chorus.sync;
using Chorus.VcsDrivers.Mercurial;
using Palaso.Reporting;


namespace WeSay.ConfigTool
{
	public partial class ChorusControl : ConfigurationControlBase
	{
		private readonly ProjectFolderConfiguration _projectFolderConfiguration;

		public ChorusControl(ILogger logger, Chorus.sync.ProjectFolderConfiguration projectFolderConfiguration)
			: base("set up synchronization with team members", logger)
		{
			_projectFolderConfiguration = projectFolderConfiguration;
			this.Font = SystemFonts.MessageBoxFont;//use the default OS UI font
			InitializeComponent();

			var msg = HgRepository.GetEnvironmentReadinessMessage("en");
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

		private void _launchChorus_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			var path = Path.Combine(DirectoryOfTheApplicationExecutable, "Chorus.exe");
			try
			{
				Process.Start(path, _projectFolderConfiguration.FolderPath);
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem(error.Message);
			}
		}

		public static string DirectoryOfTheApplicationExecutable
		{
			get
			{
				string path;
				bool unitTesting = Assembly.GetEntryAssembly() == null;
				if (unitTesting)
				{
					path = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
					path = Uri.UnescapeDataString(path);
				}
				else
				{
					//was suspect in WS1156, where it seemed to start looking in the,
					//outlook express program folder after sending an email from wesay...
					//so maybe it doesn't always mean *this* executing assembly?
					//  path = Assembly.GetExecutingAssembly().Location;
					path = Application.ExecutablePath;
				}
				return Directory.GetParent(path).FullName;
			}
		}

	}
}
