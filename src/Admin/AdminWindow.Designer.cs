namespace WeSay.Admin
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._chooseProjectLocationDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			//
			// menuStrip1
			//
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.projectToolStripMenuItem});
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
			this.newProjectToolStripMenuItem,
			this.openProjectToolStripMenuItem});
			this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
			this.projectToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.projectToolStripMenuItem.Text = "&Project";
			//
			// newProjectToolStripMenuItem
			//
			this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
			this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.newProjectToolStripMenuItem.Text = "&New Project...";
			this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.OnCreateProject);
			//
			// openProjectToolStripMenuItem
			//
			this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
			this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.openProjectToolStripMenuItem.Text = "&Open Project...";
			this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.OnOpenProject);
			//
			// AdminWindow
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(538, 312);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "AdminWindow";
			this.Text = "WeSay Admin";
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
	}
}
