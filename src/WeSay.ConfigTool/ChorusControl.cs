using Chorus.sync;
using SIL.Reporting;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;


namespace WeSay.ConfigTool
{
	public partial class ChorusControl : ConfigurationControlBase
	{
		private readonly ProjectFolderConfiguration _projectFolderConfiguration;

		public ChorusControl(ILogger logger, Chorus.sync.ProjectFolderConfiguration projectFolderConfiguration)
			: base("set up synchronization with team members", logger, "chorus")
		{
			_projectFolderConfiguration = projectFolderConfiguration;
			this.Font = SystemFonts.MessageBoxFont;//use the default OS UI font
			InitializeComponent();

			readinessPanel1.ProjectFolderPath = projectFolderConfiguration.FolderPath;
		}



		private void _launchChorus_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			var path = Path.Combine(DirectoryOfTheApplicationExecutable, "Chorus.exe");
			var projectFolderPath = '"' + _projectFolderConfiguration.FolderPath + '"';
			try
			{
#if __MonoCS__
				Process.Start("mono", path + " " + projectFolderPath);
#else
				Process.Start(path, projectFolderPath);
#endif
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
