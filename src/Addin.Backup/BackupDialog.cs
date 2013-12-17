using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Palaso.i18n;
using Palaso.Reporting;
using Palaso.UsbDrive;
using WeSay.AddinLib;

namespace Addin.Backup
{
	public partial class BackupDialog: Form
	{
		private readonly ProjectInfo _projectInfo;

		public BackupDialog(ProjectInfo projectInfo)
		{
			_projectInfo = projectInfo;
			InitializeComponent();
			_topLabel.Text = "~Looking for USB Flash Drives...";
			pictureBox1.Image = Resources.backupToDeviceImage;
			_cancelButton.Text = StringCatalog.Get(_cancelButton.Text);
			_cancelButton.Font = StringCatalog.ModifyFontForLocalization(_cancelButton.Font);
		}

		private void DoBackup(IUsbDriveInfo info)
		{
			_checkForUsbKeyTimer.Enabled = false;
			_noteLabel.Visible = false;
			_topLabel.Text = "~Backing Up...";
			Refresh();
			try
			{
				string dest = Path.Combine(info.RootDirectory.FullName,
										   _projectInfo.Name + "_wesay.zip");
				BackupMaker.BackupToExternal(_projectInfo.PathToTopLevelDirectory,
											 dest,
											 _projectInfo.FilesBelongingToProject);
				_topLabel.Text = "~Backup Complete";
				_noteLabel.Visible = true;
				_noteLabel.Text = String.Format("~Files backed up to {0}", dest);
			}
			catch (Exception e)
			{
				ErrorReport.NotifyUserOfProblem(
						"WeSay could to perform the backup.  Reason: {0}", e.Message);
				_topLabel.Text = "~Files were not backed up.";
				_topLabel.ForeColor = Color.Red;
			}

			_cancelButton.Text = "&OK";
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_checkForUsbKeyTimer.Enabled = false;
		}

		public List<IUsbDriveInfo> GetLogicalUsbDisks()
		{
			return UsbDriveInfo.GetDrives();
		}

		private void Dialog_Load(object sender, EventArgs e)
		{
			_checkForUsbKeyTimer.Enabled = true;
		}

		private void OnCheckForUsbKeyTimer_Tick(object sender, EventArgs e)
		{
			LookForTargetVolume();
		}

		private void LookForTargetVolume()
		{
			try
			{
				var usbDrives = GetLogicalUsbDisks();
				if ((usbDrives.Count > 0) && (usbDrives[0].IsReady))
				{
					DoBackup(usbDrives[0]);
				}
				else
				{
					_topLabel.Text = "~Please insert the USB Flash Drive to backup to.";
				}
			}
			catch (Exception error)
			{
				_checkForUsbKeyTimer.Enabled = false;
				ErrorReport.ReportNonFatalException(error);
				_topLabel.Text = "Unable to look for the device due to an error.";
			}
		}
	}
}