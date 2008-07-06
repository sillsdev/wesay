using System;
using System.IO;
using System.Windows.Forms;
using Chorus.sync;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class BackupPlanControl : ConfigurationControlBase
	{
		public BackupPlanControl()
			: base("prepare for the worst")
		{
			InitializeComponent();
		}

		private void BackupPlanControl_Load(object sender, EventArgs e)
		{
			string s = RepositoryManager.GetEnvironmentReadinessMessage("en");
			if(string.IsNullOrEmpty(s))
			{
				_environmentNotReadyLabel.Visible = false;
			}
			else
			{
				_environmentNotReadyLabel.Visible = true;
				_environmentNotReadyLabel.Text += s;
			}

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
					_pathText.Text = dialog.SelectedPath;
				}
			}
			catch (Exception error)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage("Something went wrong choosing the folder. " +
																   error.Message);
			}
		}
	}
}
