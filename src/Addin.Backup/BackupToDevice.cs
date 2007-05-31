using System;
using System.Drawing;
using System.Windows.Forms;
using Mono.Addins;
using WeSay.AddinLib;

namespace Addin.Backup
{

	[Extension]
	public class BackupToDevice : IWeSayAddin//, IWeSayProjectAwareAddin
	{

		#region IWeSayAddin Members

		public Image ButtonImage
		{
			get
			{
				return Resources.backupToDeviceImage;
			}
		}

		public bool Available
		{
			get
			{
				return true;
			}
		}

		public string Name
		{
			get
			{
				return "Backup To Device";
			}
		}

		public string ShortDescription
		{
			get
			{
				return "Saves a backup on an external device, like a USB key.";
			}
		}



		public string ID
		{
			get
			{
				return "ManualBackupToDevice";
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			BackupDialog d = new BackupDialog(projectInfo);
			d.ShowDialog(parentForm);
		}
		#endregion

	}
}
