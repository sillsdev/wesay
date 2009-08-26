using System;

using System.IO;
using System.Windows.Forms;
using Chorus.sync;
using Chorus.VcsDrivers.Mercurial;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class BackupPlanControl: ConfigurationControlBase
	{
		public BackupPlanControl(ILogger logger)
			: base("prepare for the worst", logger)
		{
			InitializeComponent();
		}

		private void BackupPlanControl_Load(object sender, EventArgs e)
		{
			string s = HgRepository.GetEnvironmentReadinessMessage("en");

			_pathText.Text = WeSayWordsProject.Project.BackupMaker.PathToParentOfRepositories;
			WeSayWordsProject.Project.EditorsSaveNow += new EventHandler(OnEditorsSaveNow);
		}


		private void OnEditorsSaveNow(object sender, EventArgs e)
		{
			WeSayWordsProject.Project.BackupMaker.PathToParentOfRepositories = _pathText.Text;
		}

		private void _browseButton_Click(object sender, EventArgs e)
		{
			try
			{
				FolderBrowserDialog dialog = new FolderBrowserDialog();
				dialog.Description = "Choose drive or folder for backups";
				dialog.RootFolder =Environment.SpecialFolder.MyComputer;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					if( _pathText.Text != dialog.SelectedPath)
					{
						_logger.WriteConciseHistoricalEvent(StringCatalog.Get("BackupPlan path changed", "Checkin Description in WeSay Config Tool used when you change the backup path"), dialog.SelectedPath);
					}
					_pathText.Text = dialog.SelectedPath;
				}
			}
			catch (Exception error)
			{
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem("Something went wrong choosing the folder. " +
																   error.Message);
			}
		}
	}
}