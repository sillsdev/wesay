using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace Reporting
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Summary description for ErrorReporter.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class ErrorReporter : Form, IDisposable
	{
		#region Member variables
		private Label label2;
		private Label label3;
		private TextBox m_details;
		private TextBox m_notification;
		private TextBox m_reproduce;
		private Label labelAttemptToContinue;
		protected static string s_emailAddress= null;
		protected static string s_emailSubject= "Automated Error Report";

		private bool m_isLethal;

		/// <summary>
		/// a list of name, string value pairs that will be included in the details of the error report.
		/// For example, xWorks would could the name of the database in here.
		/// </summary>
		protected static StringDictionary s_properties =
			new StringDictionary();

		protected static bool s_isOkToInteractWithUser = true;
		private Button btnClose;
		private static bool s_fIgnoreReport = false;
		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// this is protected so that we can have a Singleton
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected ErrorReporter(bool isLethal)
		{
			m_isLethal = isLethal;
		}

		#region IDisposable override

		/// <summary>
		/// Check to see if the object has been disposed.
		/// All public Properties and Methods should call this
		/// before doing anything else.
		/// </summary>
		public void CheckDisposed()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(String.Format("'{0}' in use after being disposed.", GetType().Name));
		}

		/// <summary>
		/// Executes in two distinct scenarios.
		///
		/// 1. If disposing is true, the method has been called directly
		/// or indirectly by a user's code via the Dispose method.
		/// Both managed and unmanaged resources can be disposed.
		///
		/// 2. If disposing is false, the method has been called by the
		/// runtime from inside the finalizer and you should not reference (access)
		/// other managed objects, as they already have been garbage collected.
		/// Only unmanaged resources can be disposed.
		/// </summary>
		/// <param name="disposing"></param>
		/// <remarks>
		/// If any exceptions are thrown, that is fine.
		/// If the method is being done in a finalizer, it will be ignored.
		/// If it is thrown by client code calling Dispose,
		/// it needs to be handled by fixing the bug.
		///
		/// If subclasses override this method, they should call the base implementation.
		/// </remarks>
		protected override void Dispose(bool disposing)
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if (disposing)
			{
				// Dispose managed resources here.
			}

			// Dispose unmanaged resources here, whether disposing is true or false.

			base.Dispose(disposing);
		}

		#endregion IDisposable override

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorReporter));
			this.label2 = new System.Windows.Forms.Label();
			this.m_reproduce = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.m_details = new System.Windows.Forms.TextBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.m_notification = new System.Windows.Forms.TextBox();
			this.labelAttemptToContinue = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// label2
			//
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			//
			// m_reproduce
			//
			this.m_reproduce.AcceptsReturn = true;
			this.m_reproduce.AcceptsTab = true;
			resources.ApplyResources(this.m_reproduce, "m_reproduce");
			this.m_reproduce.Name = "m_reproduce";
			//
			// label3
			//
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			//
			// m_details
			//
			resources.ApplyResources(this.m_details, "m_details");
			this.m_details.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.m_details.Name = "m_details";
			this.m_details.ReadOnly = true;
			//
			// btnClose
			//
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			//
			// m_notification
			//
			resources.ApplyResources(this.m_notification, "m_notification");
			this.m_notification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.m_notification.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.m_notification.ForeColor = System.Drawing.Color.Black;
			this.m_notification.Name = "m_notification";
			this.m_notification.ReadOnly = true;
			//
			// labelAttemptToContinue
			//
			resources.ApplyResources(this.labelAttemptToContinue, "labelAttemptToContinue");
			this.labelAttemptToContinue.ForeColor = System.Drawing.Color.Firebrick;
			this.labelAttemptToContinue.Name = "labelAttemptToContinue";
			//
			// ErrorReporter
			//
			this.AcceptButton = this.btnClose;
			resources.ApplyResources(this, "$this");
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.CancelButton = this.btnClose;
			this.ControlBox = false;
			this.Controls.Add(this.m_reproduce);
			this.Controls.Add(this.m_notification);
			this.Controls.Add(this.m_details);
			this.Controls.Add(this.labelAttemptToContinue);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnClose);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorReporter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Static methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// show a dialog or output to the error log, as appropriate.
		/// </summary>
		/// <param name="error">the exception you want to report</param>
		/// ------------------------------------------------------------------------------------
		public static void ReportException(Exception error)
		{
			ReportException(error, null);
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="error"></param>
		/// <param name="parent"></param>
		public static void ReportException(Exception error, Form parent)
		{
			ReportException(error, null, true);
		}


		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// show a dialog or output to the error log, as appropriate.
		/// </summary>
		/// <param name="error">the exception you want to report</param>
		/// <param name="parent">the parent form that this error belongs to (i.e. the form
		/// show modally on)</param>
		/// ------------------------------------------------------------------------------------
		public static void ReportException(Exception error, Form parent, bool isLethal)
		{
			// ignore message if we are showing from a previous error
			if (s_fIgnoreReport)
				return;

//			// If the error has a message and a help link, then show that error
//			if (error.HelpLink != null && error.HelpLink != string.Empty && error.HelpLink.IndexOf("::/") > 0 &&
//				error.Message != null && error.Message != string.Empty)
//			{
//				s_fIgnoreReport = true; // This is presumably a hopelessly fatal error, so we don't want to report any subsequent errors at all.
//				// Look for the end of the basic message which will be terminated by two new lines or
//				// two CRLF sequences.
//				int lengthOfBasicMessage = error.Message.IndexOf("\r\n");
//				if (lengthOfBasicMessage <= 0)
//					lengthOfBasicMessage = error.Message.IndexOf("\n\n");
//				if (lengthOfBasicMessage <= 0)
//					lengthOfBasicMessage = error.Message.Length;
//
//				int iSeparatorBetweenFileNameAndTopic = error.HelpLink.IndexOf("::/");
//				string sHelpFile = error.HelpLink.Substring(0, iSeparatorBetweenFileNameAndTopic);
//				string sHelpTopic = error.HelpLink.Substring(iSeparatorBetweenFileNameAndTopic + 3);
//
//				string caption = ReportingStrings.kstidFieldWorksErrorCaption;
//				string appExit = ReportingStrings.kstidFieldWorksErrorExitInfo;
//				FwMessageBox.StaticShow(parent, error.Message.Substring(0, lengthOfBasicMessage) + "\n" + appExit,
//					caption, FwMessageBoxButton.OK, FwMessageBoxIcon.Error, sHelpFile, sHelpTopic);
//				Clipboard.SetDataObject(error.Message, true);
//				Application.Exit();
//			}

			using (ErrorReporter e = new ErrorReporter(isLethal))
			{
				e.HandleError(error, parent);
			}
		}
		#endregion

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		///make this false during automated testing
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool OkToInteractWithUser
		{
			set {s_isOkToInteractWithUser = value;}
			get {return s_isOkToInteractWithUser;}
		}

		/// <summary>
		/// set this property if you want the dialog to offer to create an e-mail message.
		/// </summary>
		public static string EmailAddress
		{
			set {s_emailAddress = value;}
			get {return s_emailAddress;}
		}
		/// <summary>
		/// set this property if you want something other than the default e-mail subject
		/// </summary>
		public static string EmailSubject
		{
			set {s_emailSubject = value;}
			get {return s_emailSubject;}
		}



		#endregion

		#region Methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected void GatherData()
		{
			m_details.Text += "\r\nTo Reproduce: " + m_reproduce.Text + "\r\n";
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///	add a property that he would like included in any bug reports created by this application.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void AddProperty(string label, string contents)
		{
			//avoid an error if the user changes the value of something,
			//which happens in FieldWorks, for example, when you change the language project.
			if (s_properties.ContainsKey(label))
				s_properties.Remove(label);

			s_properties.Add(label, contents);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// <param name="error"></param>
		/// <param name="owner"></param>
		/// ------------------------------------------------------------------------------------
		public void HandleError(Exception error, Form owner)
		{
			CheckDisposed();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// These 2 lines can be deleted after the problems with mailto have been resolved.
//			this.radEmail.Enabled = false;
//			this.radSelf.Checked = true;
//
//			if(s_emailAddress == null)
//			{
//				this.radEmail.Enabled = false;
//				this.radSelf.Checked = true;
//			}
//			else
//			{
//				// Add the e-mail address to the dialog.
//				label1.Text += ": " + s_emailAddress;
//			}

			if(!m_isLethal)
			{
				btnClose.Text = ReportingStrings.ks_Ok;
				this.BackColor =  Color.FromArgb(255, 255, 192);//yellow
				m_notification.BackColor = this.BackColor;
			}

			Exception innerMostException = null;
			m_details.Text += GetHiearchicalExceptionInfo(error, ref innerMostException);

			//if the exception had inner exceptions, show the inner-most exception first, since that is usually the one
			//we want the developer to read.
			if(innerMostException != null)
				m_details.Text = "Inner most exception:\r\n" + GetExceptionText(innerMostException) +
					"\r\n\r\nFull, hierarchical exception contents:\r\n" + m_details.Text;

			m_details.Text += "\r\nError Reporting Properties:\r\n";
			foreach(string label in s_properties.Keys )
				m_details.Text += label + ": " + s_properties[label] + "\r\n";

			m_details.Text += Logger.LogText;
			Debug.WriteLine(m_details.Text);
			if (innerMostException != null)
				error = innerMostException;
			Logger.WriteEvent("Got exception " + error.GetType().Name);

			if (s_isOkToInteractWithUser)
			{
				s_fIgnoreReport = true;
				ShowDialog(owner);
				s_fIgnoreReport = false;
			}
			else	//the test environment already prohibits dialogs but will save the contents of assertions in some log.
				Debug.Fail(m_details.Text);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		protected string GetHiearchicalExceptionInfo(Exception error, ref Exception innerMostException)
		{
			string x = GetExceptionText(error);

			if (error.InnerException!= null)
			{
				innerMostException = error.InnerException;

				x += "**Inner Exception:\r\n";
				x += GetHiearchicalExceptionInfo(error.InnerException, ref innerMostException);
			}
			return x;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// <param name="error"></param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		private string GetExceptionText(Exception error)
		{
			StringBuilder txt = new StringBuilder();

			txt.Append("Msg: ");
			txt.Append(error.Message);

			try
			{
				if (error is COMException)
				{
					txt.Append("\r\nCOM message: ");
					txt.Append(new Win32Exception(((COMException)error).ErrorCode).Message);
				}
			}
			catch
			{
			}

			try
			{
				txt.Append("\r\nSource: ");
				txt.Append(error.Source);
			}
			catch
			{
			}

			try
			{
				if(error.TargetSite != null)
				{
					txt.Append("\r\nAssembly: ");
					txt.Append(error.TargetSite.DeclaringType.Assembly.FullName);
				}
			}
			catch
			{
			}

			try
			{
				txt.Append("\r\nStack: ");
				txt.Append(error.StackTrace);
			}
			catch
			{
			}
			txt.Append("\r\n");
			return txt.ToString();
		}
		#endregion

		#region Event Handlers
		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// ------------------------------------------------------------------------------------
		private void btnClose_Click(object sender, EventArgs e)
		{
			if (ModifierKeys.Equals(Keys.Shift))
			{
				return;
			}
			GatherData();

			Clipboard.SetDataObject(m_details.Text, true);

			try
			{
				Reporting.MAPI msg = new MAPI();
				msg.AddRecipientTo(s_emailAddress);
				if (msg.SendMailDirect(s_emailSubject, m_details.Text))
				{
					CloseUp();
				}
			}
			catch(Exception)
			{
				//swallow it and go to the mailto method
			}

			try
			{
				//EmailMessage msg = new EmailMessage();
				// This currently does not work. The main issue seems to be the length of the error report. mailto
				// apparently has some limit on the length of the message, and we are exceeding that.
				//make it safe, but does too much (like replacing spaces with +'s)
				//string s = System.Web.HttpUtility.UrlPathEncode( m_details.Text);
				//msg.Body = m_details.Text.Replace(Environment.NewLine, "%0A").Replace("\"", "%22").Replace("&", "%26");
				EmailMessage msg = new EmailMessage();
				msg.Body = "<Please paste the details of the crash here>";
				msg.Address = s_emailAddress;
				msg.Subject = s_emailSubject;
				msg.Send();
				CloseUp();
			}
			catch (Exception)
			{
				//swallow it and go to the clipboard method
			}

			if (s_emailAddress != null)
			{
				m_details.Text = String.Format(ReportingStrings.ksPleaseEMailThisToUs, s_emailAddress, m_details.Text);
			}
			Clipboard.SetDataObject(m_details.Text, true);

			CloseUp();
		}

		private  void CloseUp()
		{
			if(!m_isLethal || ModifierKeys.Equals(Keys.Shift))
			{
				Logger.WriteEvent("Continuing...");
				return;
			}
			Logger.WriteEvent("Exiting...");
			Application.Exit();
			//still didn't work? Sheesh.
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the attempt to continue label if the shift key is pressed
		/// </summary>
		/// <param name="e"></param>
		/// ------------------------------------------------------------------------------------
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ShiftKey && Visible)
				labelAttemptToContinue.Visible = true;
			base.OnKeyDown(e);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Hides the attempt to continue label if the shift key is pressed
		/// </summary>
		/// <param name="e"></param>
		/// ------------------------------------------------------------------------------------
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ShiftKey && Visible)
				labelAttemptToContinue.Visible = false;
			base.OnKeyUp(e);
		}
		#endregion

		public static void AddStandardProperties()
		{
			AddProperty("Version", GetVersionForErrorReporting());
			AddProperty("CommandLine", Environment.CommandLine);
			AddProperty("CurrentDirectory", Environment.CurrentDirectory);
			AddProperty("MachineName", Environment.MachineName);
			AddProperty("OSVersion", Environment.OSVersion.ToString());
			AddProperty("DotNetVersion", Environment.Version.ToString());
			AddProperty("WorkingSet", Environment.WorkingSet.ToString());
			AddProperty("UserDomainName", Environment.UserDomainName);
			AddProperty("UserName", Environment.UserName);
			AddProperty("Culture", CultureInfo.CurrentCulture.ToString());
		}

		private static string GetVersionForErrorReporting()
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			if (assembly != null)
			{
				string version = VersionNumberString;

				version += " (apparent build date: ";
				try
				{
					string path = assembly.CodeBase.Replace(@"file:///", "");
					version +=  File.GetLastWriteTimeUtc(path).Date.ToShortDateString() +")";
				}
				catch
				{
					version += "???";
				}

#if DEBUG
				version += "  (Debug version)";
#endif
				return version;
			}
			return "unknown";
		}

		public static string VersionNumberString
		{
			get
			{
				Assembly assembly = Assembly.GetEntryAssembly();
				if (assembly != null)
				{
					object[] attributes =
						assembly.GetCustomAttributes(typeof (AssemblyFileVersionAttribute), false);
					string version;
					if (attributes != null && attributes.Length > 0)
					{
						version = ((AssemblyFileVersionAttribute) attributes[0]).Version;
					}
					else
					{
						version = Application.ProductVersion;
					}
					return version;
				}
				return "unknown";
			}
		}

		public static string UserFriendlyVersionString
		{
			get
			{
				string v = VersionNumberString;
				string build = v.Substring(v.LastIndexOf('.') + 1);
				return "Version 1 Preview, build " + build;
			}
		}

		/// <summary>
		/// Put up a message box, unless OkToInteractWithUser is false, in which case throw an Appliciation Exception
		/// </summary>
		public static void ReportNonFatalMessage(string message, params object[] args)
		{
			if (Reporting.ErrorReporter.OkToInteractWithUser)
			{
				MessageBox.Show(
					String.Format(message,args),
					 Application.ProductName+" Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				throw new NonFatalMessageSentToUserException(String.Format(message,args));
			}
		}

		//for tests to catch
		public class NonFatalMessageSentToUserException : ApplicationException
		{
			public NonFatalMessageSentToUserException(string message) :base(message)
			{
			}
		}
	}
}
