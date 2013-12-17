using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;

namespace Addin.Backup
{
	public partial class SynchronizeDialog: Form
	{
		private readonly ProjectInfo _projectInfo;
		private readonly SynchronizeSettings _settings;

		public SynchronizeDialog(ProjectInfo projectInfo, SynchronizeSettings settings)
		{
			_projectInfo = projectInfo;
			_settings = settings;
			InitializeComponent();
			_topLabel.Text = "~Synchronizing...";
			pictureBox1.Image = Resources.backupToDeviceImage;
			_cancelButton.Text = StringCatalog.Get(_cancelButton.Text);
			_cancelButton.Font = StringCatalog.ModifyFontForLocalization(_cancelButton.Font);
		}

		private void Dialog_Load(object sender, EventArgs e)
		{
			bool hadProblems = false;
			Process p = new Process();
			p.StartInfo.FileName = _settings.GetRuntimeProcessPath(_projectInfo);
			p.StartInfo.Arguments = _settings.GetRuntimeArguments(_projectInfo);
			p.StartInfo.WorkingDirectory = _projectInfo.PathToTopLevelDirectory;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;

			p.StartInfo.UseShellExecute = false;
			try
			{
				p.Start();

				_outputBox.ForeColor = Color.Red;
				string error = p.StandardError.ReadToEnd();
				if (error.Length > 0)
				{
					hadProblems = true;
				}
				_outputBox.Text += error;
				_outputBox.ForeColor = Color.Black;
				_outputBox.Text += p.StandardOutput.ReadToEnd();

				p.WaitForExit(); //NB: don't do this before the read!!!! can give a deadlock
			}
			catch (Exception err)
			{
				_outputBox.ForeColor = Color.Red;
				_outputBox.Text = err.Message;
				hadProblems = true;
			}
			if (hadProblems)
			{
				_topLabel.Text = "~WeSay Had Problems Synchronizing";
			}
			else
			{
				_topLabel.Text = "~Finished";
			}
			_cancelButton.Text = "~OK";
		}
	}
}