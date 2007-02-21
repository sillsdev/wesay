using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Reporting
{
	/// <summary>
	/// Summary description for UsageEmailDialog.
	/// </summary>
	public class UsageEmailDialog : Form, IDisposable
	{
		private TabControl tabControl1;
		private TabPage tabPage1;
		private PictureBox pictureBox1;
		private RichTextBox richTextBox2;
		private Button btnSend;
		private LinkLabel btnNope;
		private RichTextBox m_topLineText;

		private EmailMessage _message = new EmailMessage();

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private UsageEmailDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

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
		///
		/// </summary>
		public string TopLineText
		{
			set
			{
				CheckDisposed();
				m_topLineText.Text = value;
			}
			get
			{
				CheckDisposed();
				return m_topLineText.Text;
			}
		}

		public EmailMessage EmailMessage
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}



		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsageEmailDialog));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.m_topLineText = new System.Windows.Forms.RichTextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.richTextBox2 = new System.Windows.Forms.RichTextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.btnNope = new System.Windows.Forms.LinkLabel();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// tabControl1
			//
			this.tabControl1.Controls.Add(this.tabPage1);
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			//
			// tabPage1
			//
			this.tabPage1.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage1.Controls.Add(this.m_topLineText);
			this.tabPage1.Controls.Add(this.pictureBox1);
			this.tabPage1.Controls.Add(this.richTextBox2);
			resources.ApplyResources(this.tabPage1, "tabPage1");
			this.tabPage1.Name = "tabPage1";
			//
			// m_topLineText
			//
			this.m_topLineText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this.m_topLineText, "m_topLineText");
			this.m_topLineText.Name = "m_topLineText";
			this.m_topLineText.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
			//
			// pictureBox1
			//
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			//
			// richTextBox2
			//
			this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this.richTextBox2, "richTextBox2");
			this.richTextBox2.Name = "richTextBox2";
			//
			// btnSend
			//
			resources.ApplyResources(this.btnSend, "btnSend");
			this.btnSend.Name = "btnSend";
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			//
			// btnNope
			//
			resources.ApplyResources(this.btnNope, "btnNope");
			this.btnNope.Name = "btnNope";
			this.btnNope.TabStop = true;
			this.btnNope.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnNope_LinkClicked);
			//
			// UsageEmailDialog
			//
			this.AcceptButton = this.btnSend;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnNope;
			this.ControlBox = false;
			this.Controls.Add(this.btnNope);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MinimizeBox = false;
			this.Name = "UsageEmailDialog";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			try
			{

				this.EmailMessage.Send();
			}
			catch(Exception)
			{
				//swallow it
			}
			this.DialogResult = DialogResult.OK;
			this.Close();
		}


		private void btnNope_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.DialogResult = DialogResult.No;
			this.Close();
		}


		/// <summary>
		/// call this each time the application is launched if you have launch count-based reporting
		/// </summary>
		public static void IncrementLaunchCount()
		{
			int launchCount = 1 + int.Parse(RegistryAccess.GetStringRegistryValue("launches","0"));
			RegistryAccess.SetStringRegistryValue("launches",launchCount.ToString());
		}

		/// <summary>
		/// used for testing purposes
		/// </summary>
		public static void ClearLaunchCount()
		{
			RegistryAccess.SetStringRegistryValue("launches","0");
		}


		/// <summary>
		/// if you call this every time the application starts, it will send reports on those intervals
		/// (e.g. {1, 10}) that are listed in the intervals parameter.  It will get version number and name out of the application.
		/// </summary>
		public static void DoTrivialUsageReport(string emailAddress, string topMessage, int[] intervals)
		{
			int launchCount = int.Parse(RegistryAccess.GetStringRegistryValue("launches","0"));

			foreach(int launch in intervals)
			{
				if (launch == launchCount)
				{
					SendReport(emailAddress, launchCount, topMessage);
					break;
				}
			}
		}

		private static void SendReport(string emailAddress, int launchCount, string topMessage)
		{
			// Set the Application label to the name of the app
			Assembly assembly = Assembly.GetEntryAssembly();
			string version = Application.ProductVersion;
			if (assembly != null)
			{
				object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
				version = (attributes != null && attributes.Length > 0) ?
						  ((AssemblyFileVersionAttribute)attributes[0]).Version : Application.ProductVersion;
			}

			using (UsageEmailDialog d = new UsageEmailDialog())
			{
				d.TopLineText = topMessage;
				d.EmailMessage.Address = emailAddress;
				d.EmailMessage.Subject = string.Format("{0} {1} Report {2} Launches", Application.ProductName, version, launchCount);
				d.EmailMessage.Body = string.Format("<report app='{0}' version='{1}'><stat type='launches' value='{2}'/></report>", Application.ProductName, version, launchCount);
				d.ShowDialog();
			}
		}

		/// <summary>
		/// A class for managing registry access.
		/// </summary>
		public class RegistryAccess
		{
			private const string SOFTWARE_KEY = "Software";
//			private static string s_company;
//			private static string s_application;
//
//			static Application App
//			{
//				set
//				{
//					s_company = Application.CompanyName;
//
//				}
//			}
			// Method for retrieving a Registry Value.
			static public string GetStringRegistryValue(string key, string defaultValue)
			{
				RegistryKey rkCompany;
				RegistryKey rkApplication;
				//RegistryKey rkFieldWorks;

				// The generic Company Name is SIL International, but in the registry we want this to use
				// SIL. If we want to keep a generic approach, we probably need another member variable
				// for ShortCompanyName, or something similar.
				rkCompany = Registry.CurrentUser.OpenSubKey(SOFTWARE_KEY, false).OpenSubKey(Application.CompanyName, false);
				//rkCompany = Registry.CurrentUser.OpenSubKey(SOFTWARE_KEY, false).OpenSubKey("SIL", false);
				if( rkCompany != null )
				{
//					rkFieldWorks = rkCompany.OpenSubKey("FieldWorks", false);
//					if( rkFieldWorks != null)
//					{
//						rkApplication = rkFieldWorks.OpenSubKey( Application.ProductName, false);
						rkApplication = rkCompany.OpenSubKey( Application.ProductName, false);
						if( rkApplication != null )
						{
							foreach(string sKey in rkApplication.GetValueNames())
							{
								if( sKey == key )
								{
									return (string)rkApplication.GetValue(sKey);
								}
							}
//						}
					}
				}
				return defaultValue;
			}

			// Method for storing a Registry Value.
			static public void SetStringRegistryValue(string key, string stringValue)
			{
				RegistryKey rkSoftware;
				RegistryKey rkCompany;
				RegistryKey rkApplication;

				rkSoftware = Registry.CurrentUser.OpenSubKey(SOFTWARE_KEY, true);
				// The generic Company Name is SIL International, but in the registry we want this to use
				// SIL. If we want to keep a generic approach, we probably need another member variable
				// for ShortCompanyName, or something similar.
				rkCompany = rkSoftware.CreateSubKey(Application.CompanyName);
				//rkCompany = rkSoftware.CreateSubKey("SIL");
				if( rkCompany != null )
				{
						rkApplication = rkCompany.CreateSubKey(Application.ProductName);
						if( rkApplication != null )
						{
							rkApplication.SetValue(key, stringValue);
						}
				}
			}
		}



	}
}
