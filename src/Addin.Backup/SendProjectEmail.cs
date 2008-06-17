using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;

namespace Addin.Backup
{
	[Extension]
	public class SendProjectEmail: IWeSayAddin, IWeSayAddinHasSettings
	{
		private SendProjectEmailSettings _settings;
		private string subject;

		public SendProjectEmail()
		{
			_settings = new SendProjectEmailSettings();
		}

		public string LocalizedName
		{
			get
			{
				return
						string.Format(StringCatalog.Get("~Email My Work to {0}"),
									  _settings.RecipientName);
			}
		}

		public string ShortDescription
		{
			get
			{
				return
						string.Format(
								StringCatalog.Get(
										"~Send a zipped email containing all your WeSay work."));
			}
		}

		#region IWeSayAddin Members

		public string ID
		{
			get { return "SendProjectEmail"; }
		}

		#endregion

		public Image ButtonImage
		{
			get { return Resources.emailProjectImage; }
		}

		public bool Available
		{
			get { return !string.IsNullOrEmpty(_settings.Email); }
		}

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			string dest = Path.Combine(Path.GetTempPath(), projectInfo.Name + "_wesay.zip");
			BackupMaker.BackupToExternal(projectInfo.PathToTopLevelDirectory,
										 dest,
										 projectInfo.FilesBelongingToProject);

			MAPI msg = new MAPI();
			msg.AddAttachment(dest);
			msg.AddRecipientTo(_settings.Email);
			subject =
					String.Format(
							StringCatalog.Get("{0} WeSay Project Data",
											  "The subject line of the email send by the 'Send Email' Action. The {0} will be replaced by the name of the project, as in 'Greek WeSay Project Data'"),
							projectInfo.Name);
			string body = StringCatalog.Get("The latest WeSay project data is attached.");

			//I tried hard to get this to run in a thread so it wouldn't block wesay,
			//but when called in a thread we always just get the generic '2' back.

			//            EmailMessage emailWorker =
			//                new EmailMessage(
			//                    subject,
			//                    body,
			//                    msg);

			///emailWorker.SendMail();
			///

			msg.SendMailPopup(subject, body);
		}

		/// <summary>
		/// all this is just so that wesay doesn't hang waiting for this to happen.
		/// </summary>
		//        class EmailMessage: System.ComponentModel.BackgroundWorker
		//        {
		//            private string subject;
		//            private string contents;
		//            private MAPI msg;
		//
		//            public EmailMessage(string subject, string contents, MAPI msg)
		//            {
		//                this.subject = subject;
		//                this.msg = msg;
		//                this.contents = contents;
		//            }
		//
		////            protected override void  OnDoWork(System.ComponentModel.DoWorkEventArgs e)
		////            {
		////                base.OnDoWork(e);
		////                 msg.SendMailPopup(subject,contents);
		////            }
		//            protected void  Start()
		//            {
		//                 msg.SendMailPopup(subject,contents);
		//            }
		//            public void SendMail()
		//            {
		//                System.Threading.Thread t = new Thread(Start);
		//                t.IsBackground = false;
		//                t.Start();
		//
		//            }
		//        }

		#region IWeSayAddinHasSettings Members

		public bool DoShowSettingsDialog(Form parentForm, ProjectInfo projectInfo)
		{
			SendProjectEmailSettingsDialog dlg = new SendProjectEmailSettingsDialog(_settings);
			return dlg.ShowDialog(parentForm) == DialogResult.OK;
		}

		public object Settings
		{
			get { return _settings; }
			set { _settings = (SendProjectEmailSettings) value; }
		}

		#endregion

		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.Share; }
		}

		public string LocalizedLabel
		{
			get { return LocalizedName; }
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconVariableWidth; }
		}

		public Image DashboardButtonImage
		{
			get { return Resources.greenEmail; }
		}

		#endregion
	}

	[Serializable]
	public class SendProjectEmailSettings
	{
		private string _recipientName = "someone";
		private string _email = "";

		public string RecipientName
		{
			get { return _recipientName; }
			set { _recipientName = value; }
		}

		public string Email
		{
			get { return _email; }
			set { _email = value; }
		}
	}
}