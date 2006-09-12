namespace Admin
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this._tasksPage = new System.Windows.Forms.TabPage();
			this.taskList1 = new Admin.TaskList();
			this._writingSystemPage = new System.Windows.Forms.TabPage();
			this.writingSystemSetup1 = new Admin.WritingSystemSetup();
			this._transferPage = new System.Windows.Forms.TabPage();
			this._maintenancePage = new System.Windows.Forms.TabPage();
			this.menuStrip1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this._tasksPage.SuspendLayout();
			this._writingSystemPage.SuspendLayout();
			this.SuspendLayout();
			//
			// menuStrip1
			//
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.projectToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(448, 24);
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
			//
			// openProjectToolStripMenuItem
			//
			this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
			this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.openProjectToolStripMenuItem.Text = "&Open Project...";
			//
			// tabControl1
			//
			this.tabControl1.Controls.Add(this._tasksPage);
			this.tabControl1.Controls.Add(this._writingSystemPage);
			this.tabControl1.Controls.Add(this._maintenancePage);
			this.tabControl1.Controls.Add(this._transferPage);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 24);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(448, 242);
			this.tabControl1.TabIndex = 1;
			//
			// _tasksPage
			//
			this._tasksPage.Controls.Add(this.taskList1);
			this._tasksPage.Location = new System.Drawing.Point(4, 22);
			this._tasksPage.Name = "_tasksPage";
			this._tasksPage.Padding = new System.Windows.Forms.Padding(3);
			this._tasksPage.Size = new System.Drawing.Size(440, 216);
			this._tasksPage.TabIndex = 0;
			this._tasksPage.Text = "Tasks";
			this._tasksPage.UseVisualStyleBackColor = true;
			//
			// taskList1
			//
			this.taskList1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.taskList1.Location = new System.Drawing.Point(3, 3);
			this.taskList1.Name = "taskList1";
			this.taskList1.Size = new System.Drawing.Size(434, 210);
			this.taskList1.TabIndex = 0;
			//
			// _writingSystemPage
			//
			this._writingSystemPage.Controls.Add(this.writingSystemSetup1);
			this._writingSystemPage.Location = new System.Drawing.Point(4, 22);
			this._writingSystemPage.Name = "_writingSystemPage";
			this._writingSystemPage.Padding = new System.Windows.Forms.Padding(3);
			this._writingSystemPage.Size = new System.Drawing.Size(440, 216);
			this._writingSystemPage.TabIndex = 1;
			this._writingSystemPage.Text = "Writing Systems";
			this._writingSystemPage.UseVisualStyleBackColor = true;
			//
			// writingSystemSetup1
			//
			this.writingSystemSetup1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.writingSystemSetup1.Location = new System.Drawing.Point(3, 3);
			this.writingSystemSetup1.Name = "writingSystemSetup1";
			this.writingSystemSetup1.Size = new System.Drawing.Size(434, 210);
			this.writingSystemSetup1.TabIndex = 0;
			//
			// _transferPage
			//
			this._transferPage.Location = new System.Drawing.Point(4, 22);
			this._transferPage.Name = "_transferPage";
			this._transferPage.Size = new System.Drawing.Size(440, 216);
			this._transferPage.TabIndex = 2;
			this._transferPage.Text = "Transfer";
			this._transferPage.UseVisualStyleBackColor = true;
			//
			// _maintenancePage
			//
			this._maintenancePage.Location = new System.Drawing.Point(4, 22);
			this._maintenancePage.Name = "_maintenancePage";
			this._maintenancePage.Size = new System.Drawing.Size(440, 216);
			this._maintenancePage.TabIndex = 3;
			this._maintenancePage.Text = "Maintenance";
			this._maintenancePage.UseVisualStyleBackColor = true;
			//
			// AdminWindow
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(448, 266);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "AdminWindow";
			this.Text = "WeSay Admin";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this._tasksPage.ResumeLayout(false);
			this._writingSystemPage.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage _tasksPage;
		private System.Windows.Forms.TabPage _writingSystemPage;
		private TaskList taskList1;
		private WritingSystemSetup writingSystemSetup1;
		private System.Windows.Forms.TabPage _maintenancePage;
		private System.Windows.Forms.TabPage _transferPage;
	}
}
