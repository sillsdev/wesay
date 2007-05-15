using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text;
using System.Windows.Forms;
using WeSay.AddinLib;
using WeSay.Project;

namespace Addin.Backup
{
	public partial class BackupDialog : Form
	{
		private ProjectInfo _projectInfo;

		public BackupDialog(ProjectInfo projectInfo)
		{
			_projectInfo = projectInfo;
		   // _pathToProjectDirectory = pathToProjectDirectory;
			//_project = _project ;
			InitializeComponent();
			_topLabel.Text = "Looking for USB Keys...";
			pictureBox1.Image = Resources.buttonImage;
		}


		private void DoBackup(DriveInfo info)
		{
			_checkForUsbKeyTimer.Enabled = false;
			_noteLabel.Visible = false;
			_topLabel.Text = "Backing Up...";
			this.Refresh();
			try
			{
				string dest= Path.Combine(info.RootDirectory.FullName, _projectInfo.Name + "_wesay.zip");
				WeSay.Foundation.BackupMaker.BackupToExternal(_projectInfo.PathToTopLevelDirectory, dest, _projectInfo.FilesBelongingToProject);
				_topLabel.Text = "Backup Complete";
				_noteLabel.Visible = true;
				_noteLabel.Text = String.Format("Files backed up to {0}", dest);
			}
			catch (Exception e)
			{
				Reporting.ErrorReporter.ReportNonFatalMessage("WeSay could to perform the backup.  Reason: {0}",
															  e.Message);
				 _topLabel.Text = "Files were not backed up.";
				 _topLabel.ForeColor = Color.Red;
		   }

			this._cancelButton.Text = "&OK";
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_checkForUsbKeyTimer.Enabled = false;
		}

		public List<DriveInfo> GetLogicalUsbDisks()
		{
			List<DriveInfo> driveInfos=new List<DriveInfo>();

			using (ManagementObjectSearcher driveSearcher = new ManagementObjectSearcher(
					"SELECT Caption, DeviceID FROM Win32_DiskDrive WHERE InterfaceType='USB'"))
					{
						// walk all USB WMI physical disks
						foreach (ManagementObject drive in driveSearcher.Get())
						{
							// browse all USB WMI physical disks

							using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
								"ASSOCIATORS OF {Win32_DiskDrive.DeviceID='"
								+ drive["DeviceID"]
								+ "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition"))
							{
								// walk all USB WMI physical disks
								foreach (ManagementObject partition in searcher.Get())
								{
									using (ManagementObjectSearcher partitionSearcher = new ManagementObjectSearcher(
										"ASSOCIATORS OF {Win32_DiskPartition.DeviceID='"
										+ partition["DeviceID"]
										+ "'} WHERE AssocClass = Win32_LogicalDiskToPartition"))
									{
										foreach (ManagementObject disk in partitionSearcher.Get())
										{
											foreach (DriveInfo driveInfo in System.IO.DriveInfo.GetDrives())
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
			List<DriveInfo> list = GetLogicalUsbDisks();
			if (list.Count > 0)
			{
				DoBackup(list[0]);
			}
			else
			{
				this._topLabel.Text = "Please insert the USB Key to backup to.";
			}
		}

		private void _noteLabel_Click(object sender, EventArgs e)
		{

		}

	}
}