using System;
using System.Drawing;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;

namespace Addin.Backup
{
	[Extension]
	public class BackupToDevice: IWeSayAddin //, IWeSayProjectAwareAddin
	{
		#region IWeSayAddin Members

		public Image ButtonImage
		{
			get { return Resources.backupToDeviceImage; }
		}

		public bool Available
		{
			get { return true; }
		}

		public string LocalizedName
		{
			get
			{
				return StringCatalog.Get("~Backup To Device",
										 "Long name for usb flash-drive backup action");
			}
		}

		public string Description
		{
			get
			{
				return StringCatalog.Get("~Saves a backup on an external device, like a USB key.",
										 "description of usb backup action");
			}
		}

		public string ID
		{
			get { return "ManualBackupToDevice"; }
			set { throw new NotImplementedException(); }
		}

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			if (Type.GetType("Mono.Runtime") != null)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
					StringCatalog.Get("~USB Backup is currently not suppoted with Mono.",
					"Error message for usb backup on mono/linux"));
				return;
			}
			BackupDialog d = new BackupDialog(projectInfo);
			d.ShowDialog(parentForm);
		}

		#endregion

		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.Share; }
		}

		public string LocalizedLabel
		{
			get
			{
				return StringCatalog.Get("~Backup",
										 "Short buton label for usb flash-drive backup action");
			}
		}

		public string LocalizedLongLabel
		{
			get { return LocalizedName; }
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconVariableWidth; }
		}

		public Image DashboardButtonImage
		{
			get { return Resources.greenUsbKey; }
		}

		#endregion
	}
}