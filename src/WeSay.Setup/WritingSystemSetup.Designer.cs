namespace WeSay.Setup
{
	partial class WritingSystemSetup
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WritingSystemSetup));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this._btnAddWritingSystem = new System.Windows.Forms.ToolStripButton();
			this._btnRemove = new System.Windows.Forms.ToolStripButton();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._wsListBox = new System.Windows.Forms.ListBox();
			this._tabControl = new System.Windows.Forms.TabControl();
			this._basicPage = new System.Windows.Forms.TabPage();
			this._basicControl = new WeSay.Setup.WritingSystemBasic();
			this._fontsPage = new System.Windows.Forms.TabPage();
			this._fontControl = new WeSay.Setup.FontControl();
			this._typingPage = new System.Windows.Forms.TabPage();
			this._sortingPage = new System.Windows.Forms.TabPage();
			this.toolStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this._tabControl.SuspendLayout();
			this._basicPage.SuspendLayout();
			this._fontsPage.SuspendLayout();
			this.SuspendLayout();
			//
			// toolStrip1
			//
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this._btnAddWritingSystem,
			this._btnRemove});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(329, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			//
			// _btnAddWritingSystem
			//
			this._btnAddWritingSystem.Image = ((System.Drawing.Image)(resources.GetObject("_btnAddWritingSystem.Image")));
			this._btnAddWritingSystem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAddWritingSystem.Name = "_btnAddWritingSystem";
			this._btnAddWritingSystem.Size = new System.Drawing.Size(123, 22);
			this._btnAddWritingSystem.Text = "New Writing System";
			this._btnAddWritingSystem.Click += new System.EventHandler(this._btnAddWritingSystem_Click);
			//
			// _btnRemove
			//
			this._btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("_btnRemove.Image")));
			this._btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnRemove.Name = "_btnRemove";
			this._btnRemove.Size = new System.Drawing.Size(141, 22);
			this._btnRemove.Text = "Remove Writing System";
			this._btnRemove.Click += new System.EventHandler(this._btnRemove_Click);
			//
			// splitContainer1
			//
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 25);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._wsListBox);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this._tabControl);
			this.splitContainer1.Size = new System.Drawing.Size(329, 125);
			this.splitContainer1.SplitterDistance = 109;
			this.splitContainer1.TabIndex = 0;
			//
			// _wsListBox
			//
			this._wsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._wsListBox.FormattingEnabled = true;
			this._wsListBox.Location = new System.Drawing.Point(0, 0);
			this._wsListBox.Name = "_wsListBox";
			this._wsListBox.Size = new System.Drawing.Size(109, 121);
			this._wsListBox.TabIndex = 0;
			this._wsListBox.SelectedIndexChanged += new System.EventHandler(this._wsListBox_SelectedIndexChanged);
			//
			// _tabControl
			//
			this._tabControl.Controls.Add(this._basicPage);
			this._tabControl.Controls.Add(this._fontsPage);
			this._tabControl.Controls.Add(this._typingPage);
			this._tabControl.Controls.Add(this._sortingPage);
			this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tabControl.Location = new System.Drawing.Point(0, 0);
			this._tabControl.Name = "_tabControl";
			this._tabControl.SelectedIndex = 0;
			this._tabControl.Size = new System.Drawing.Size(216, 125);
			this._tabControl.TabIndex = 0;
			//
			// _basicPage
			//
			this._basicPage.Controls.Add(this._basicControl);
			this._basicPage.Location = new System.Drawing.Point(4, 22);
			this._basicPage.Name = "_basicPage";
			this._basicPage.Size = new System.Drawing.Size(208, 99);
			this._basicPage.TabIndex = 2;
			this._basicPage.Text = "Basic";
			this._basicPage.UseVisualStyleBackColor = true;
			//
			// _basicControl
			//
			this._basicControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._basicControl.Location = new System.Drawing.Point(0, 0);
			this._basicControl.Name = "_basicControl";
			this._basicControl.Size = new System.Drawing.Size(208, 99);
			this._basicControl.TabIndex = 0;
			this._basicControl.WritingSystemCollection = null;
			this._basicControl.WritingSystemIdChanged += new System.EventHandler(this._basicControl_DisplayPropertiesChanged);
			//
			// _fontsPage
			//
			this._fontsPage.Controls.Add(this._fontControl);
			this._fontsPage.Location = new System.Drawing.Point(4, 22);
			this._fontsPage.Name = "_fontsPage";
			this._fontsPage.Padding = new System.Windows.Forms.Padding(3);
			this._fontsPage.Size = new System.Drawing.Size(208, 99);
			this._fontsPage.TabIndex = 0;
			this._fontsPage.Text = "Font";
			this._fontsPage.UseVisualStyleBackColor = true;
			//
			// _fontControl
			//
			this._fontControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fontControl.Location = new System.Drawing.Point(3, 3);
			this._fontControl.Name = "_fontControl";
			this._fontControl.Size = new System.Drawing.Size(202, 93);
			this._fontControl.TabIndex = 0;
			//
			// _typingPage
			//
			this._typingPage.Location = new System.Drawing.Point(4, 22);
			this._typingPage.Name = "_typingPage";
			this._typingPage.Size = new System.Drawing.Size(208, 99);
			this._typingPage.TabIndex = 3;
			this._typingPage.Text = "Typing";
			this._typingPage.UseVisualStyleBackColor = true;
			//
			// _sortingPage
			//
			this._sortingPage.Location = new System.Drawing.Point(4, 22);
			this._sortingPage.Name = "_sortingPage";
			this._sortingPage.Padding = new System.Windows.Forms.Padding(3);
			this._sortingPage.Size = new System.Drawing.Size(208, 99);
			this._sortingPage.TabIndex = 1;
			this._sortingPage.Text = "Sorting";
			this._sortingPage.UseVisualStyleBackColor = true;
			//
			// WritingSystemSetup
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "WritingSystemSetup";
			this.Size = new System.Drawing.Size(329, 150);
			this.Load += new System.EventHandler(this.WritingSystemSetup_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this._tabControl.ResumeLayout(false);
			this._basicPage.ResumeLayout(false);
			this._fontsPage.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox _wsListBox;
		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage _basicPage;
		private System.Windows.Forms.TabPage _fontsPage;
		private System.Windows.Forms.TabPage _sortingPage;
		private System.Windows.Forms.TabPage _typingPage;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton _btnAddWritingSystem;
		private System.Windows.Forms.ToolStripButton _btnRemove;
		private WritingSystemBasic _basicControl;
		private FontControl _fontControl;
	}
}
