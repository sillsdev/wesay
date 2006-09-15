namespace WeSay.Admin
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this._tasksPage = new System.Windows.Forms.TabPage();
			this.taskList1 = new WeSay.Admin.TaskList();
			this._fieldsPage = new System.Windows.Forms.TabPage();
			this.fieldsControl1 = new WeSay.Admin.FieldsControl();
			this._writingSystemPage = new System.Windows.Forms.TabPage();
			this.writingSystemSetup1 = new WeSay.Admin.WritingSystemSetup();
			this._otherPage = new System.Windows.Forms.TabPage();
			this.otherControl1 = new WeSay.Admin.OtherControl();
			this._transferPage = new System.Windows.Forms.TabPage();
			this.tabControl1.SuspendLayout();
			this._tasksPage.SuspendLayout();
			this._fieldsPage.SuspendLayout();
			this._writingSystemPage.SuspendLayout();
			this._otherPage.SuspendLayout();
			this.SuspendLayout();
			//
			// tabControl1
			//
			this.tabControl1.Controls.Add(this._tasksPage);
			this.tabControl1.Controls.Add(this._fieldsPage);
			this.tabControl1.Controls.Add(this._writingSystemPage);
			this.tabControl1.Controls.Add(this._otherPage);
			this.tabControl1.Controls.Add(this._transferPage);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(712, 447);
			this.tabControl1.TabIndex = 2;
			//
			// _tasksPage
			//
			this._tasksPage.Controls.Add(this.taskList1);
			this._tasksPage.Location = new System.Drawing.Point(4, 22);
			this._tasksPage.Name = "_tasksPage";
			this._tasksPage.Padding = new System.Windows.Forms.Padding(3);
			this._tasksPage.Size = new System.Drawing.Size(704, 421);
			this._tasksPage.TabIndex = 0;
			this._tasksPage.Text = "Tasks";
			this._tasksPage.UseVisualStyleBackColor = true;
			//
			// taskList1
			//
			this.taskList1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.taskList1.CausesValidation = false;
			this.taskList1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.taskList1.Location = new System.Drawing.Point(3, 3);
			this.taskList1.Name = "taskList1";
			this.taskList1.Size = new System.Drawing.Size(698, 415);
			this.taskList1.TabIndex = 0;
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
			this._writingSystemPage.Controls.Add(this.writingSystemSetup1);
			this._writingSystemPage.Location = new System.Drawing.Point(4, 22);
			this._writingSystemPage.Name = "_writingSystemPage";
			this._writingSystemPage.Padding = new System.Windows.Forms.Padding(3);
			this._writingSystemPage.Size = new System.Drawing.Size(704, 421);
			this._writingSystemPage.TabIndex = 1;
			this._writingSystemPage.Text = "Writing Systems";
			this._writingSystemPage.UseVisualStyleBackColor = true;
			//
			// writingSystemSetup1
			//
			this.writingSystemSetup1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.writingSystemSetup1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.writingSystemSetup1.Location = new System.Drawing.Point(3, 3);
			this.writingSystemSetup1.Name = "writingSystemSetup1";
			this.writingSystemSetup1.Size = new System.Drawing.Size(698, 415);
			this.writingSystemSetup1.TabIndex = 0;
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
			// _transferPage
			//
			this._transferPage.Location = new System.Drawing.Point(4, 22);
			this._transferPage.Name = "_transferPage";
			this._transferPage.Size = new System.Drawing.Size(704, 421);
			this._transferPage.TabIndex = 2;
			this._transferPage.Text = "Transfer";
			this._transferPage.UseVisualStyleBackColor = true;
			//
			// ProjectTabs
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl1);
			this.Name = "ProjectTabs";
			this.Size = new System.Drawing.Size(712, 447);
			this.tabControl1.ResumeLayout(false);
			this._tasksPage.ResumeLayout(false);
			this._fieldsPage.ResumeLayout(false);
			this._writingSystemPage.ResumeLayout(false);
			this._otherPage.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage _tasksPage;
		private TaskList taskList1;
		private System.Windows.Forms.TabPage _fieldsPage;
		private FieldsControl fieldsControl1;
		private System.Windows.Forms.TabPage _writingSystemPage;
		private WritingSystemSetup writingSystemSetup1;
		private System.Windows.Forms.TabPage _otherPage;
		private OtherControl otherControl1;
		private System.Windows.Forms.TabPage _transferPage;
	}
}
