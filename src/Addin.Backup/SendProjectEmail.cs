using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Mono.Addins;
using Reporting;
using WeSay.AddinLib;
using WeSay.Foundation;

namespace Addin.Backup
{
	[Extension]
	public class SendProjectEmail :IWeSayAddin, IWeSayAddinHasSettings
	{
		private SendProjectEmailSettings _settings;

		public SendProjectEmail()
		{
			_settings = new SendProjectEmailSettings();
		}

		public string Name
		{
			get
			{
				return string.Format("Email My Work to {0}", _settings.RecipientName);
			}
		}

		public  string ShortDescription
		{
			get
			{
				return string.Format("Send a zipped email containing all your WeSay work.");
			}
		}

		#region IWeSayAddin Members

		public string ID
		{
			get
			{
				return "SendProjectEmail";
			}
		}

		#endregion

		public Image ButtonImage
		{
			get
			{
				return Resources.emailProjectImage;
			}
		}

		public bool Available
		{
			get
			{
				return !string.IsNullOrEmpty(_settings.Email);
			}
		}

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			string dest = Path.Combine(Path.GetTempPath(), projectInfo.Name + "_wesay.zip");
			BackupMaker.BackupToExternal(projectInfo.PathToTopLevelDirectory,
				dest, projectInfo.FilesBelongingToProject);

			MAPI msg = new MAPI();
			msg.AddAttachment(dest);
			msg.AddRecipientTo(_settings.Email);
			msg.SendMailPopup("WeSay Project Data","The latest WeSay project data is attached.");
		}

		#region IWeSayAddinHasSettings Members

		public bool DoShowSettingsDialog(Form parentForm)
		{
			SendProjectEmailSettingsDialog dlg = new SendProjectEmailSettingsDialog(_settings);
			return dlg.ShowDialog(parentForm) == DialogResult.OK;
		}

		public object Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = (SendProjectEmailSettings)value;
			}
		}

		#endregion
	}

	[Serializable]
	public class SendProjectEmailSettings
	{
		private string _recipientName="someone";
		private string _email="";

		public string RecipientName
		{
			get
			{
				return _recipientName;
			}
			set
			{
				_recipientName = value;
			}
		}

		public string Email
		{
			get
			{
				return _email;
			}
			set
			{
				_email = value;
			}
		}
	}
}
