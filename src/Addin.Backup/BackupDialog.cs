using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Management;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;

namespace Addin.Backup
{
	public partial class BackupDialog: Form
	{
		private readonly ProjectInfo _projectInfo;

		public BackupDialog(ProjectInfo projectInfo)
		{
			_projectInfo = projectInfo;
			InitializeComponent();
			_topLabel.Text = "~Looking for USB Keys...";
			pictureBox1.Image = Resources.backupToDeviceImage;
			_cancelButton.Text = StringCatalog.Get(_cancelButton.Text);
			_cancelButton.Font = StringCatalog.ModifyFontForLocalization(_cancelButton.Font);
		}

		private void DoBackup(DriveInfo info)
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
				ErrorReport.ReportNonFatalMessage(
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

		public List<DriveInfo> GetLogicalUsbDisks()
		{
			List<DriveInfo> driveInfos = new List<DriveInfo>();
			using (
					ManagementObjectSearcher driveSearcher =
							new ManagementObjectSearcher(
									"SELECT Caption, DeviceID FROM Win32_DiskDrive WHERE InterfaceType='USB'")
					)
			{
				// walk all USB WMI physical disks
				foreach (ManagementObject drive in driveSearcher.Get())
				{
					// browse all USB WMI physical disks

					using (
							ManagementObjectSearcher searcher =
									new ManagementObjectSearcher(
											"ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" +
											drive["DeviceID"] +
											"'} WHERE AssocClass = Win32_DiskDriveToDiskPartition"))
					{
						// walk all USB WMI physical disks
						foreach (ManagementObject partition in searcher.Get())
						{
							using (
									ManagementObjectSearcher partitionSearcher =
											new ManagementObjectSearcher(
													"ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" +
													partition["DeviceID"] +
													"'} WHERE AssocClass = Win32_LogicalDiskToPartition")
									)
							{
								foreach (ManagementObject disk in partitionSearcher.Get())
								{
									foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
									{
										string s = driveInfo.Name.Replace("\\", "");
										if (s == disk["NAME"].ToString())
										{
											driveInfos.Add(driveInfo);
										}
									}
								}
							}
						}
					}
				}
			}
			return driveInfos;
		}

		private void Dialog_Load(object sender, EventArgs e)
		{
			_checkForUsbKeyTimer.Enabled = true;
			//   LookForTargetVolume();
		}

		private void OnCheckForUsbKeyTimer_Tick(object sender, EventArgs e)
		{
			LookForTargetVolume();
		}

		private void LookForTargetVolume()
		{
			try
			{
				List<DriveInfo> list = GetLogicalUsbDisks();
				if (list.Count > 0)
				{
					DoBackup(list[0]);
				}
				else
				{
					_topLabel.Text = "~Please insert the USB Key to backup to.";
				}
			}
			catch (Exception error)
			{
				_checkForUsbKeyTimer.Enabled = false;
				ErrorNotificationDialog.ReportException(error, this, false);
				_topLabel.Text = "Unable to look for the device due to an error.";
			}
		}
	}
}