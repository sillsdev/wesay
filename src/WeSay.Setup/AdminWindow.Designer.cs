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
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectInWeSayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.newProjectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutWeSayConfigurationToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._versionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
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
			this.fileToolStripMenuItem,
			this.helpToolStripMenuItem1,
			this._versionToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip1.Size = new System.Drawing.Size(538, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			//
			// fileToolStripMenuItem
			//
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.openProjectInWeSayToolStripMenuItem,
			this.toolStripMenuItem1,
			this.newProjectToolStripMenuItem1,
			this.openProjectToolStripMenuItem1,
			this.toolStripMenuItem3,
			this.exitToolStripMenuItem1});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			//
			// openProjectInWeSayToolStripMenuItem
			//
			this.openProjectInWeSayToolStripMenuItem.Image = global::WeSay.Setup.Properties.Resources.WeSayMenuSized;
			this.openProjectInWeSayToolStripMenuItem.Name = "openProjectInWeSayToolStripMenuItem";
			this.openProjectInWeSayToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.openProjectInWeSayToolStripMenuItem.Text = "Open this Project in &WeSay";
			this.openProjectInWeSayToolStripMenuItem.Click += new System.EventHandler(this.OnOpenThisProjectInWeSay);
			//
			// toolStripMenuItem1
			//
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(202, 6);
			//
			// newProjectToolStripMenuItem1
			//
			this.newProjectToolStripMenuItem1.Name = "newProjectToolStripMenuItem1";
			this.newProjectToolStripMenuItem1.Size = new System.Drawing.Size(205, 22);
			this.newProjectToolStripMenuItem1.Text = "&New Project";
			this.newProjectToolStripMenuItem1.Click += new System.EventHandler(this.OnCreateProject);
			//
			// openProjectToolStripMenuItem1
			//
			this.openProjectToolStripMenuItem1.Name = "openProjectToolStripMenuItem1";
			this.openProjectToolStripMenuItem1.Size = new System.Drawing.Size(205, 22);
			this.openProjectToolStripMenuItem1.Text = "&Open Project...";
			this.openProjectToolStripMenuItem1.Click += new System.EventHandler(this.OnChooseProject);
			//
			// toolStripMenuItem3
			//
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(202, 6);
			//
			// exitToolStripMenuItem1
			//
			this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
			this.exitToolStripMenuItem1.Size = new System.Drawing.Size(205, 22);
			this.exitToolStripMenuItem1.Text = "E&xit";
			this.exitToolStripMenuItem1.Click += new System.EventHandler(this.OnExit_Click);
			//
			// helpToolStripMenuItem1
			//
			this.helpToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.aboutWeSayConfigurationToolToolStripMenuItem});
			this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
			this.helpToolStripMenuItem1.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem1.Text = "&Help";
			//
			// aboutWeSayConfigurationToolToolStripMenuItem
			//
			this.aboutWeSayConfigurationToolToolStripMenuItem.Name = "aboutWeSayConfigurationToolToolStripMenuItem";
			this.aboutWeSayConfigurationToolToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
			this.aboutWeSayConfigurationToolToolStripMenuItem.Text = "&About WeSay Configuration Tool";
			this.aboutWeSayConfigurationToolToolStripMenuItem.Click += new System.EventHandler(this.OnAboutToolStripMenuItem_Click);
			//
			// _versionToolStripMenuItem
			//
			this._versionToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this._versionToolStripMenuItem.Enabled = false;
			this._versionToolStripMenuItem.Name = "_versionToolStripMenuItem";
			this._versionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this._versionToolStripMenuItem.Text = "Version";
			//
			// projectToolStripMenuItem
			//
			this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
			this.projectToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.projectToolStripMenuItem.Text = "&Project";
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
			this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.OnChooseProject);
			//
			// toolStripMenuItem2
			//
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(202, 6);
			//
			// exitToolStripMenuItem
			//
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnExit_Click);
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
			this.Icon = global::WeSay.Setup.Properties.Resources.WeSaySetupApplicationIcon;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "AdminWindow";
			this.Text = "WeSay Configuration Tool";
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
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem _versionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openProjectInWeSayToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem aboutWeSayConfigurationToolToolStripMenuItem;
	}
}
