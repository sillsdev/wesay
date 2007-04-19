namespace WeSay.Setup
{
	partial class ProjectTabs
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._projectTabControl = new System.Windows.Forms.TabControl();
			this._tasksPage = new System.Windows.Forms.TabPage();
			this._taskListControl = new WeSay.Setup.TaskListControl();
			this._fieldsPage = new System.Windows.Forms.TabPage();
			this.fieldsControl1 = new WeSay.Setup.FieldsControl();
			this._writingSystemPage = new System.Windows.Forms.TabPage();
			this._writingSystemSetupControl = new WeSay.Setup.WritingSystemSetup();
			this._otherPage = new System.Windows.Forms.TabPage();
			this.otherControl1 = new WeSay.Setup.OtherControl();
			this._actionsPage = new System.Windows.Forms.TabPage();
			this.actionsControl1 = new WeSay.Setup.ActionsControl();
			this._projectTabControl.SuspendLayout();
			this._tasksPage.SuspendLayout();
			this._fieldsPage.SuspendLayout();
			this._writingSystemPage.SuspendLayout();
			this._otherPage.SuspendLayout();
			this._actionsPage.SuspendLayout();
			this.SuspendLayout();
			//
			// _projectTabControl
			//
			this._projectTabControl.Controls.Add(this._tasksPage);
			this._projectTabControl.Controls.Add(this._fieldsPage);
			this._projectTabControl.Controls.Add(this._writingSystemPage);
			this._projectTabControl.Controls.Add(this._otherPage);
			this._projectTabControl.Controls.Add(this._actionsPage);
			this._projectTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._projectTabControl.Location = new System.Drawing.Point(0, 0);
			this._projectTabControl.Name = "_projectTabControl";
			this._projectTabControl.SelectedIndex = 0;
			this._projectTabControl.Size = new System.Drawing.Size(712, 447);
			this._projectTabControl.TabIndex = 2;
			//
			// _tasksPage
			//
			this._tasksPage.Controls.Add(this._taskListControl);
			this._tasksPage.Location = new System.Drawing.Point(4, 22);
			this._tasksPage.Name = "_tasksPage";
			this._tasksPage.Padding = new System.Windows.Forms.Padding(3);
			this._tasksPage.Size = new System.Drawing.Size(704, 421);
			this._tasksPage.TabIndex = 0;
			this._tasksPage.Text = "Tasks";
			this._tasksPage.UseVisualStyleBackColor = true;
			//
			// _taskListControl
			//
			this._taskListControl.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this._taskListControl.CausesValidation = false;
			this._taskListControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._taskListControl.Location = new System.Drawing.Point(3, 3);
			this._taskListControl.Name = "_taskListControl";
			this._taskListControl.Size = new System.Drawing.Size(698, 415);
			this._taskListControl.TabIndex = 0;
			//
			// _fieldsPage
			//
			this._fieldsPage.Controls.Add(this.fieldsControl1);
			this._fieldsPage.Location = new System.Drawing.Point(4, 22);
			this._fieldsPage.Name = "_fieldsPage";
			this._fieldsPage.Size = new System.Drawing.Size(704, 421);
			this._fieldsPage.TabIndex = 4;
			this._fieldsPage.Text = "Fields";
			this._fieldsPage.UseVisualStyleBackColor = true;
			//
			// fieldsControl1
			//
			this.fieldsControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.fieldsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fieldsControl1.Location = new System.Drawing.Point(0, 0);
			this.fieldsControl1.Name = "fieldsControl1";
			this.fieldsControl1.Padding = new System.Windows.Forms.Padding(7);
			this.fieldsControl1.Size = new System.Drawing.Size(704, 421);
			this.fieldsControl1.TabIndex = 0;
			//
			// _writingSystemPage
			//
			this._writingSystemPage.Controls.Add(this._writingSystemSetupControl);
			this._writingSystemPage.Location = new System.Drawing.Point(4, 22);
			this._writingSystemPage.Name = "_writingSystemPage";
			this._writingSystemPage.Padding = new System.Windows.Forms.Padding(3);
			this._writingSystemPage.Size = new System.Drawing.Size(704, 421);
			this._writingSystemPage.TabIndex = 1;
			this._writingSystemPage.Text = "Writing Systems";
			this._writingSystemPage.UseVisualStyleBackColor = true;
			//
			// _writingSystemSetupControl
			//
			this._writingSystemSetupControl.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this._writingSystemSetupControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._writingSystemSetupControl.Location = new System.Drawing.Point(3, 3);
			this._writingSystemSetupControl.Name = "_writingSystemSetupControl";
			this._writingSystemSetupControl.Size = new System.Drawing.Size(698, 415);
			this._writingSystemSetupControl.TabIndex = 0;
			//
			// _otherPage
			//
			this._otherPage.Controls.Add(this.otherControl1);
			this._otherPage.Location = new System.Drawing.Point(4, 22);
			this._otherPage.Name = "_otherPage";
			this._otherPage.Size = new System.Drawing.Size(704, 421);
			this._otherPage.TabIndex = 3;
			this._otherPage.Text = "Other Settings";
			this._otherPage.UseVisualStyleBackColor = true;
			//
			// otherControl1
			//
			this.otherControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.otherControl1.Location = new System.Drawing.Point(0, 0);
			this.otherControl1.Name = "otherControl1";
			this.otherControl1.Size = new System.Drawing.Size(704, 421);
			this.otherControl1.TabIndex = 0;
			//
			// _actionsPage
			//
			this._actionsPage.Controls.Add(this.actionsControl1);
			this._actionsPage.Location = new System.Drawing.Point(4, 22);
			this._actionsPage.Name = "_actionsPage";
			this._actionsPage.Size = new System.Drawing.Size(704, 421);
			this._actionsPage.TabIndex = 2;
			this._actionsPage.Text = "Actions";
			this._actionsPage.UseVisualStyleBackColor = true;
			//
			// actionsControl1
			//
			this.actionsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.actionsControl1.Location = new System.Drawing.Point(0, 0);
			this.actionsControl1.Name = "actionsControl1";
			this.actionsControl1.Size = new System.Drawing.Size(704, 421);
			this.actionsControl1.TabIndex = 0;
			//
			// ProjectTabs
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._projectTabControl);
			this.Name = "ProjectTabs";
			this.Size = new System.Drawing.Size(712, 447);
			this._projectTabControl.ResumeLayout(false);
			this._tasksPage.ResumeLayout(false);
			this._fieldsPage.ResumeLayout(false);
			this._writingSystemPage.ResumeLayout(false);
			this._otherPage.ResumeLayout(false);
			this._actionsPage.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl _projectTabControl;
		private System.Windows.Forms.TabPage _tasksPage;
		private TaskListControl _taskListControl;
		private System.Windows.Forms.TabPage _fieldsPage;
		private FieldsControl fieldsControl1;
		private System.Windows.Forms.TabPage _writingSystemPage;
		private WritingSystemSetup _writingSystemSetupControl;
		private System.Windows.Forms.TabPage _otherPage;
		private OtherControl otherControl1;
		private System.Windows.Forms.TabPage _actionsPage;
		private ActionsControl actionsControl1;
	}
}
