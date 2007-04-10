using WeSay.Setup.Properties;

namespace WeSay.Setup
{
	partial class AdminWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminWindow));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openThisProjectInWeSayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.exportToLIFTXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importFromLIFTXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._chooseProjectLocationDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			//
			// menuStrip1
			//
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.projectToolStripMenuItem,
			this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip1.Size = new System.Drawing.Size(538, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			//
			// projectToolStripMenuItem
			//
			this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.openThisProjectInWeSayToolStripMenuItem,
			this.toolStripSeparator1,
			this.newProjectToolStripMenuItem,
			this.openProjectToolStripMenuItem,
			this.toolStripMenuItem2,
			this.exportToLIFTXmlToolStripMenuItem,
			this.importFromLIFTXMLToolStripMenuItem,
			this.toolStripMenuItem1,
			this.exitToolStripMenuItem});
			this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
			this.projectToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.projectToolStripMenuItem.Text = "&Project";
			//
			// openThisProjectInWeSayToolStripMenuItem
			//
			this.openThisProjectInWeSayToolStripMenuItem.Image = Resources.WeSayApplicationIcon.ToBitmap();
			this.openThisProjectInWeSayToolStripMenuItem.Name = "openThisProjectInWeSayToolStripMenuItem";
			this.openThisProjectInWeSayToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.openThisProjectInWeSayToolStripMenuItem.Text = "Open this Project in &WeSay";
			this.openThisProjectInWeSayToolStripMenuItem.Click += new System.EventHandler(this.OnOpenThisProjectInWeSay);
			//
			// toolStripSeparator1
			//
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(202, 6);
			//
			// newProjectToolStripMenuItem
			//
			this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
			this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.newProjectToolStripMenuItem.Text = "&New Project...";
			this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.OnCreateProject);
			//
			// openProjectToolStripMenuItem
			//
			this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
			this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.openProjectToolStripMenuItem.Text = "&Open Project...";
			this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.OnOpenProject);
			//
			// toolStripMenuItem2
			//
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(202, 6);
			//
			// exportToLIFTXmlToolStripMenuItem
			//
			this.exportToLIFTXmlToolStripMenuItem.Name = "exportToLIFTXmlToolStripMenuItem";
			this.exportToLIFTXmlToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.exportToLIFTXmlToolStripMenuItem.Text = "&Export To LIFT XML...";
			this.exportToLIFTXmlToolStripMenuItem.Click += new System.EventHandler(this.OnExportToLiftXmlToolStripMenuItem_Click);
			//
			// importFromLIFTXMLToolStripMenuItem
			//
			this.importFromLIFTXMLToolStripMenuItem.Name = "importFromLIFTXMLToolStripMenuItem";
			this.importFromLIFTXMLToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.importFromLIFTXMLToolStripMenuItem.Text = "&Import From LIFT XML...";
			this.importFromLIFTXMLToolStripMenuItem.Click += new System.EventHandler(this.OnImportFromLiftXml);
			//
			// toolStripMenuItem1
			//
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(202, 6);
			//
			// exitToolStripMenuItem
			//
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			//
			// helpToolStripMenuItem
			//
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			//
			// aboutToolStripMenuItem
			//
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.aboutToolStripMenuItem.Text = "&About WeSay...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.OnAboutToolStripMenuItem_Click);
			//
			// AdminWindow
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(538, 312);
			this.Controls.Add(this.menuStrip1);
			this.Icon = Resources.WeSaySetupApplicationIcon;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "AdminWindow";
			this.Text = "WeSay Setup";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AdminWindow_FormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdminWindow_FormClosing);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
		private System.Windows.Forms.FolderBrowserDialog _chooseProjectLocationDialog;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem exportToLIFTXmlToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importFromLIFTXMLToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openThisProjectInWeSayToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	}
}
