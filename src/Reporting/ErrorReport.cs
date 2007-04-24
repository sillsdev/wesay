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
using SIL.Utils;

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
		private Label label1;
		private TextBox m_details;
		private RadioButton radSelf;
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
		private RadioButton radEmail;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ErrorReporter));
			this.label2 = new System.Windows.Forms.Label();
			this.m_reproduce = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.radEmail = new System.Windows.Forms.RadioButton();
			this.m_details = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.radSelf = new System.Windows.Forms.RadioButton();
			this.btnClose = new System.Windows.Forms.Button();
			this.m_notification = new System.Windows.Forms.TextBox();
			this.labelAttemptToContinue = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// label2
			//
			this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
			this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
			this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
			this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
			this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
			this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
			this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
			this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
			this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
			this.label2.Name = "label2";
			this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
			this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
			this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
			this.label2.Text = resources.GetString("label2.Text");
			this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
			this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
			//
			// m_reproduce
			//
			this.m_reproduce.AcceptsReturn = true;
			this.m_reproduce.AcceptsTab = true;
			this.m_reproduce.AccessibleDescription = resources.GetString("m_reproduce.AccessibleDescription");
			this.m_reproduce.AccessibleName = resources.GetString("m_reproduce.AccessibleName");
			this.m_reproduce.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_reproduce.Anchor")));
			this.m_reproduce.AutoSize = ((bool)(resources.GetObject("m_reproduce.AutoSize")));
			this.m_reproduce.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_reproduce.BackgroundImage")));
			this.m_reproduce.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_reproduce.Dock")));
			this.m_reproduce.Enabled = ((bool)(resources.GetObject("m_reproduce.Enabled")));
			this.m_reproduce.Font = ((System.Drawing.Font)(resources.GetObject("m_reproduce.Font")));
			this.m_reproduce.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_reproduce.ImeMode")));
			this.m_reproduce.Location = ((System.Drawing.Point)(resources.GetObject("m_reproduce.Location")));
			this.m_reproduce.MaxLength = ((int)(resources.GetObject("m_reproduce.MaxLength")));
			this.m_reproduce.Multiline = ((bool)(resources.GetObject("m_reproduce.Multiline")));
			this.m_reproduce.Name = "m_reproduce";
			this.m_reproduce.PasswordChar = ((char)(resources.GetObject("m_reproduce.PasswordChar")));
			this.m_reproduce.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_reproduce.RightToLeft")));
			this.m_reproduce.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("m_reproduce.ScrollBars")));
			this.m_reproduce.Size = ((System.Drawing.Size)(resources.GetObject("m_reproduce.Size")));
			this.m_reproduce.TabIndex = ((int)(resources.GetObject("m_reproduce.TabIndex")));
			this.m_reproduce.Text = resources.GetString("m_reproduce.Text");
			this.m_reproduce.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("m_reproduce.TextAlign")));
			this.m_reproduce.Visible = ((bool)(resources.GetObject("m_reproduce.Visible")));
			this.m_reproduce.WordWrap = ((bool)(resources.GetObject("m_reproduce.WordWrap")));
			//
			// label3
			//
			this.label3.AccessibleDescription = resources.GetString("label3.AccessibleDescription");
			this.label3.AccessibleName = resources.GetString("label3.AccessibleName");
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label3.Anchor")));
			this.label3.AutoSize = ((bool)(resources.GetObject("label3.AutoSize")));
			this.label3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label3.Dock")));
			this.label3.Enabled = ((bool)(resources.GetObject("label3.Enabled")));
			this.label3.Font = ((System.Drawing.Font)(resources.GetObject("label3.Font")));
			this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
			this.label3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.ImageAlign")));
			this.label3.ImageIndex = ((int)(resources.GetObject("label3.ImageIndex")));
			this.label3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label3.ImeMode")));
			this.label3.Location = ((System.Drawing.Point)(resources.GetObject("label3.Location")));
			this.label3.Name = "label3";
			this.label3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label3.RightToLeft")));
			this.label3.Size = ((System.Drawing.Size)(resources.GetObject("label3.Size")));
			this.label3.TabIndex = ((int)(resources.GetObject("label3.TabIndex")));
			this.label3.Text = resources.GetString("label3.Text");
			this.label3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.TextAlign")));
			this.label3.Visible = ((bool)(resources.GetObject("label3.Visible")));
			//
			// radEmail
			//
			this.radEmail.AccessibleDescription = resources.GetString("radEmail.AccessibleDescription");
			this.radEmail.AccessibleName = resources.GetString("radEmail.AccessibleName");
			this.radEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("radEmail.Anchor")));
			this.radEmail.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("radEmail.Appearance")));
			this.radEmail.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("radEmail.BackgroundImage")));
			this.radEmail.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radEmail.CheckAlign")));
			this.radEmail.Checked = true;
			this.radEmail.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("radEmail.Dock")));
			this.radEmail.Enabled = ((bool)(resources.GetObject("radEmail.Enabled")));
			this.radEmail.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("radEmail.FlatStyle")));
			this.radEmail.Font = ((System.Drawing.Font)(resources.GetObject("radEmail.Font")));
			this.radEmail.Image = ((System.Drawing.Image)(resources.GetObject("radEmail.Image")));
			this.radEmail.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radEmail.ImageAlign")));
			this.radEmail.ImageIndex = ((int)(resources.GetObject("radEmail.ImageIndex")));
			this.radEmail.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radEmail.ImeMode")));
			this.radEmail.Location = ((System.Drawing.Point)(resources.GetObject("radEmail.Location")));
			this.radEmail.Name = "radEmail";
			this.radEmail.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radEmail.RightToLeft")));
			this.radEmail.Size = ((System.Drawing.Size)(resources.GetObject("radEmail.Size")));
			this.radEmail.TabIndex = ((int)(resources.GetObject("radEmail.TabIndex")));
			this.radEmail.TabStop = true;
			this.radEmail.Text = resources.GetString("radEmail.Text");
			this.radEmail.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radEmail.TextAlign")));
			this.radEmail.Visible = ((bool)(resources.GetObject("radEmail.Visible")));
			//
			// m_details
			//
			this.m_details.AccessibleDescription = resources.GetString("m_details.AccessibleDescription");
			this.m_details.AccessibleName = resources.GetString("m_details.AccessibleName");
			this.m_details.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_details.Anchor")));
			this.m_details.AutoSize = ((bool)(resources.GetObject("m_details.AutoSize")));
			this.m_details.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.m_details.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_details.BackgroundImage")));
			this.m_details.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_details.Dock")));
			this.m_details.Enabled = ((bool)(resources.GetObject("m_details.Enabled")));
			this.m_details.Font = ((System.Drawing.Font)(resources.GetObject("m_details.Font")));
			this.m_details.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_details.ImeMode")));
			this.m_details.Location = ((System.Drawing.Point)(resources.GetObject("m_details.Location")));
			this.m_details.MaxLength = ((int)(resources.GetObject("m_details.MaxLength")));
			this.m_details.Multiline = ((bool)(resources.GetObject("m_details.Multiline")));
			this.m_details.Name = "m_details";
			this.m_details.PasswordChar = ((char)(resources.GetObject("m_details.PasswordChar")));
			this.m_details.ReadOnly = true;
			this.m_details.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_details.RightToLeft")));
			this.m_details.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("m_details.ScrollBars")));
			this.m_details.Size = ((System.Drawing.Size)(resources.GetObject("m_details.Size")));
			this.m_details.TabIndex = ((int)(resources.GetObject("m_details.TabIndex")));
			this.m_details.Text = resources.GetString("m_details.Text");
			this.m_details.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("m_details.TextAlign")));
			this.m_details.Visible = ((bool)(resources.GetObject("m_details.Visible")));
			this.m_details.WordWrap = ((bool)(resources.GetObject("m_details.WordWrap")));
			//
			// label1
			//
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			//
			// radSelf
			//
			this.radSelf.AccessibleDescription = resources.GetString("radSelf.AccessibleDescription");
			this.radSelf.AccessibleName = resources.GetString("radSelf.AccessibleName");
			this.radSelf.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("radSelf.Anchor")));
			this.radSelf.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("radSelf.Appearance")));
			this.radSelf.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("radSelf.BackgroundImage")));
			this.radSelf.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radSelf.CheckAlign")));
			this.radSelf.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("radSelf.Dock")));
			this.radSelf.Enabled = ((bool)(resources.GetObject("radSelf.Enabled")));
			this.radSelf.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("radSelf.FlatStyle")));
			this.radSelf.Font = ((System.Drawing.Font)(resources.GetObject("radSelf.Font")));
			this.radSelf.Image = ((System.Drawing.Image)(resources.GetObject("radSelf.Image")));
			this.radSelf.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radSelf.ImageAlign")));
			this.radSelf.ImageIndex = ((int)(resources.GetObject("radSelf.ImageIndex")));
			this.radSelf.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radSelf.ImeMode")));
			this.radSelf.Location = ((System.Drawing.Point)(resources.GetObject("radSelf.Location")));
			this.radSelf.Name = "radSelf";
			this.radSelf.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radSelf.RightToLeft")));
			this.radSelf.Size = ((System.Drawing.Size)(resources.GetObject("radSelf.Size")));
			this.radSelf.TabIndex = ((int)(resources.GetObject("radSelf.TabIndex")));
			this.radSelf.Text = resources.GetString("radSelf.Text");
			this.radSelf.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("radSelf.TextAlign")));
			this.radSelf.Visible = ((bool)(resources.GetObject("radSelf.Visible")));
			//
			// btnClose
			//
			this.btnClose.AccessibleDescription = resources.GetString("btnClose.AccessibleDescription");
			this.btnClose.AccessibleName = resources.GetString("btnClose.AccessibleName");
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnClose.Anchor")));
			this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnClose.Dock")));
			this.btnClose.Enabled = ((bool)(resources.GetObject("btnClose.Enabled")));
			this.btnClose.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnClose.FlatStyle")));
			this.btnClose.Font = ((System.Drawing.Font)(resources.GetObject("btnClose.Font")));
			this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
			this.btnClose.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnClose.ImageAlign")));
			this.btnClose.ImageIndex = ((int)(resources.GetObject("btnClose.ImageIndex")));
			this.btnClose.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnClose.ImeMode")));
			this.btnClose.Location = ((System.Drawing.Point)(resources.GetObject("btnClose.Location")));
			this.btnClose.Name = "btnClose";
			this.btnClose.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnClose.RightToLeft")));
			this.btnClose.Size = ((System.Drawing.Size)(resources.GetObject("btnClose.Size")));
			this.btnClose.TabIndex = ((int)(resources.GetObject("btnClose.TabIndex")));
			this.btnClose.Text = resources.GetString("btnClose.Text");
			this.btnClose.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnClose.TextAlign")));
			this.btnClose.Visible = ((bool)(resources.GetObject("btnClose.Visible")));
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			//
			// m_notification
			//
			this.m_notification.AccessibleDescription = resources.GetString("m_notification.AccessibleDescription");
			this.m_notification.AccessibleName = resources.GetString("m_notification.AccessibleName");
			this.m_notification.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("m_notification.Anchor")));
			this.m_notification.AutoSize = ((bool)(resources.GetObject("m_notification.AutoSize")));
			this.m_notification.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.m_notification.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_notification.BackgroundImage")));
			this.m_notification.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.m_notification.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("m_notification.Dock")));
			this.m_notification.Enabled = ((bool)(resources.GetObject("m_notification.Enabled")));
			this.m_notification.Font = ((System.Drawing.Font)(resources.GetObject("m_notification.Font")));
			this.m_notification.ForeColor = System.Drawing.Color.Black;
			this.m_notification.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("m_notification.ImeMode")));
			this.m_notification.Location = ((System.Drawing.Point)(resources.GetObject("m_notification.Location")));
			this.m_notification.MaxLength = ((int)(resources.GetObject("m_notification.MaxLength")));
			this.m_notification.Multiline = ((bool)(resources.GetObject("m_notification.Multiline")));
			this.m_notification.Name = "m_notification";
			this.m_notification.PasswordChar = ((char)(resources.GetObject("m_notification.PasswordChar")));
			this.m_notification.ReadOnly = true;
			this.m_notification.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("m_notification.RightToLeft")));
			this.m_notification.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("m_notification.ScrollBars")));
			this.m_notification.Size = ((System.Drawing.Size)(resources.GetObject("m_notification.Size")));
			this.m_notification.TabIndex = ((int)(resources.GetObject("m_notification.TabIndex")));
			this.m_notification.Text = resources.GetString("m_notification.Text");
			this.m_notification.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("m_notification.TextAlign")));
			this.m_notification.Visible = ((bool)(resources.GetObject("m_notification.Visible")));
			this.m_notification.WordWrap = ((bool)(resources.GetObject("m_notification.WordWrap")));
			//
			// labelAttemptToContinue
			//
			this.labelAttemptToContinue.AccessibleDescription = resources.GetString("labelAttemptToContinue.AccessibleDescription");
			this.labelAttemptToContinue.AccessibleName = resources.GetString("labelAttemptToContinue.AccessibleName");
			this.labelAttemptToContinue.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("labelAttemptToContinue.Anchor")));
			this.labelAttemptToContinue.AutoSize = ((bool)(resources.GetObject("labelAttemptToContinue.AutoSize")));
			this.labelAttemptToContinue.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("labelAttemptToContinue.Dock")));
			this.labelAttemptToContinue.Enabled = ((bool)(resources.GetObject("labelAttemptToContinue.Enabled")));
			this.labelAttemptToContinue.Font = ((System.Drawing.Font)(resources.GetObject("labelAttemptToContinue.Font")));
			this.labelAttemptToContinue.ForeColor = System.Drawing.Color.Firebrick;
			this.labelAttemptToContinue.Image = ((System.Drawing.Image)(resources.GetObject("labelAttemptToContinue.Image")));
			this.labelAttemptToContinue.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelAttemptToContinue.ImageAlign")));
			this.labelAttemptToContinue.ImageIndex = ((int)(resources.GetObject("labelAttemptToContinue.ImageIndex")));
			this.labelAttemptToContinue.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("labelAttemptToContinue.ImeMode")));
			this.labelAttemptToContinue.Location = ((System.Drawing.Point)(resources.GetObject("labelAttemptToContinue.Location")));
			this.labelAttemptToContinue.Name = "labelAttemptToContinue";
			this.labelAttemptToContinue.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("labelAttemptToContinue.RightToLeft")));
			this.labelAttemptToContinue.Size = ((System.Drawing.Size)(resources.GetObject("labelAttemptToContinue.Size")));
			this.labelAttemptToContinue.TabIndex = ((int)(resources.GetObject("labelAttemptToContinue.TabIndex")));
			this.labelAttemptToContinue.Text = resources.GetString("labelAttemptToContinue.Text");
			this.labelAttemptToContinue.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelAttemptToContinue.TextAlign")));
			this.labelAttemptToContinue.Visible = ((bool)(resources.GetObject("labelAttemptToContinue.Visible")));
			//
			// ErrorReporter
			//
			this.AcceptButton = this.btnClose;
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.CancelButton = this.btnClose;
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.ControlBox = false;
			this.Controls.Add(this.m_reproduce);
			this.Controls.Add(this.m_notification);
			this.Controls.Add(this.m_details);
			this.Controls.Add(this.labelAttemptToContinue);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.radEmail);
			this.Controls.Add(this.radSelf);
			this.Controls.Add(this.btnClose);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.KeyPreview = true;
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ErrorReporter";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.ResumeLayout(false);

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
			this.radEmail.Enabled = false;
			this.radSelf.Checked = true;

			if(s_emailAddress == null)
			{
				this.radEmail.Enabled = false;
				this.radSelf.Checked = true;
			}
			else
			{
				// Add the e-mail address to the dialog.
				label1.Text += ": " + s_emailAddress;
			}

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
		/// <param name="error"></param>
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
			GatherData();

			if(radEmail.Checked)
			{
				try
				{
					// WARNING! This currently does not work. The main issue seems to be the length of the error report. mailto
					// apparently has some limit on the length of the message, and we are exceeding that.
					//make it safe, but does too much (like replacing spaces with +'s)
					//string s = System.Web.HttpUtility.UrlPathEncode( m_details.Text);

					EmailMessage msg = new EmailMessage();
					msg.Body = m_details.Text.Replace(Environment.NewLine, "%0A").Replace("\"", "%22").Replace("&", "%26");
					msg.Address = s_emailAddress;
					msg.Subject = s_emailSubject;
					msg.Send();
					CloseUp();
				}
				catch(Exception)
				{
					//swallow it and go to the clipboard method
				}
			}

			if(s_emailAddress != null)
			{
				m_details.Text =String.Format (ReportingStrings.ksPleaseEMailThisToUs,s_emailAddress, m_details.Text);
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
				string version = versionNumberString;

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

		private static string versionNumberString
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
				string v = versionNumberString;
				string build = v.Substring(v.LastIndexOf('.') + 1);
				return "Version 1 Preview, build " + build;
			}
		}

		/// <summary>
		/// Put up a message box, unless OkToInteractWithUser is false, in which case throw an Appliciation Exception
		/// </summary>
		/// <param name="message"></param>
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
				throw new ApplicationException(message);
			}
		}
	}
}
