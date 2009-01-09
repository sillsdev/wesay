namespace WeSay.ConfigTool
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
			this._basicControl = new WritingSystemBasic();
			this._fontsPage = new System.Windows.Forms.TabPage();
			this._fontControl = new FontControl();
			this._sortingPage = new System.Windows.Forms.TabPage();
			this._sortControl = new WritingSystemSortControl();
			this.toolStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this._tabControl.SuspendLayout();
			this._basicPage.SuspendLayout();
			this._fontsPage.SuspendLayout();
			this._sortingPage.SuspendLayout();
			this.SuspendLayout();
			//
			// toolStrip1
			//
			this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.BackColor = System.Drawing.SystemColors.Window;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this._btnAddWritingSystem,
			this._btnRemove});
			this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.toolStrip1.Location = new System.Drawing.Point(3, 400);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(210, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			//
			// _btnAddWritingSystem
			//
			this._btnAddWritingSystem.Image = ((System.Drawing.Image)(resources.GetObject("_btnAddWritingSystem.Image")));
			this._btnAddWritingSystem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAddWritingSystem.Name = "_btnAddWritingSystem";
			this._btnAddWritingSystem.Size = new System.Drawing.Size(49, 22);
			this._btnAddWritingSystem.Text = "New";
			this._btnAddWritingSystem.Click += new System.EventHandler(this._btnAddWritingSystem_Click);
			//
			// _btnRemove
			//
			this._btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("_btnRemove.Image")));
			this._btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnRemove.Name = "_btnRemove";
			this._btnRemove.Size = new System.Drawing.Size(58, 22);
			this._btnRemove.Text = "Delete";
			this._btnRemove.Click += new System.EventHandler(this._btnRemove_Click);
			//
			// splitContainer1
			//
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(0, -2);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._wsListBox);
			this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this._tabControl);
			this.splitContainer1.Size = new System.Drawing.Size(768, 429);
			this.splitContainer1.SplitterDistance = 215;
			this.splitContainer1.TabIndex = 0;
			//
			// _wsListBox
			//
			this._wsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._wsListBox.FormattingEnabled = true;
			this._wsListBox.Location = new System.Drawing.Point(2, 11);
			this._wsListBox.Name = "_wsListBox";
			this._wsListBox.Size = new System.Drawing.Size(210, 394);
			this._wsListBox.TabIndex = 2;
			this._wsListBox.SelectedIndexChanged += new System.EventHandler(this._wsListBox_SelectedIndexChanged);
			//
			// _tabControl
			//
			this._tabControl.Controls.Add(this._basicPage);
			this._tabControl.Controls.Add(this._fontsPage);
			this._tabControl.Controls.Add(this._sortingPage);
			this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tabControl.Location = new System.Drawing.Point(0, 0);
			this._tabControl.Name = "_tabControl";
			this._tabControl.SelectedIndex = 0;
			this._tabControl.Size = new System.Drawing.Size(549, 429);
			this._tabControl.TabIndex = 0;
			//
			// _basicPage
			//
			this._basicPage.Controls.Add(this._basicControl);
			this._basicPage.Location = new System.Drawing.Point(4, 22);
			this._basicPage.Name = "_basicPage";
			this._basicPage.Size = new System.Drawing.Size(541, 403);
			this._basicPage.TabIndex = 2;
			this._basicPage.Text = "Basic";
			this._basicPage.UseVisualStyleBackColor = true;
			//
			// _basicControl
			//
			this._basicControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._basicControl.Location = new System.Drawing.Point(0, 0);
			this._basicControl.Margin = new System.Windows.Forms.Padding(4);
			this._basicControl.Name = "_basicControl";
			this._basicControl.Size = new System.Drawing.Size(541, 403);
			this._basicControl.TabIndex = 0;
			this._basicControl.WritingSystemCollection = null;
			this._basicControl.WritingSystemIdChanged += new System.EventHandler(this.OnWritingSystemIdChanged);
			this._basicControl.IsAudioChanged += new System.EventHandler(this.OnIsAudioChanged);
			//
			// _fontsPage
			//
			this._fontsPage.Controls.Add(this._fontControl);
			this._fontsPage.Location = new System.Drawing.Point(4, 22);
			this._fontsPage.Name = "_fontsPage";
			this._fontsPage.Padding = new System.Windows.Forms.Padding(3);
			this._fontsPage.Size = new System.Drawing.Size(541, 403);
			this._fontsPage.TabIndex = 0;
			this._fontsPage.Text = "Font";
			this._fontsPage.UseVisualStyleBackColor = true;
			//
			// _fontControl
			//
			this._fontControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fontControl.Location = new System.Drawing.Point(3, 3);
			this._fontControl.Margin = new System.Windows.Forms.Padding(4);
			this._fontControl.Name = "_fontControl";
			this._fontControl.Size = new System.Drawing.Size(535, 397);
			this._fontControl.TabIndex = 0;
			//
			// _sortingPage
			//
			this._sortingPage.Controls.Add(this._sortControl);
			this._sortingPage.Location = new System.Drawing.Point(4, 22);
			this._sortingPage.Name = "_sortingPage";
			this._sortingPage.Padding = new System.Windows.Forms.Padding(3);
			this._sortingPage.Size = new System.Drawing.Size(541, 403);
			this._sortingPage.TabIndex = 1;
			this._sortingPage.Text = "Sorting";
			this._sortingPage.UseVisualStyleBackColor = true;
			//
			// _sortControl
			//
			this._sortControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._sortControl.Location = new System.Drawing.Point(3, 3);
			this._sortControl.Margin = new System.Windows.Forms.Padding(4);
			this._sortControl.Name = "_sortControl";
			this._sortControl.Size = new System.Drawing.Size(535, 397);
			this._sortControl.TabIndex = 0;
			//
			// WritingSystemSetup
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.splitContainer1);
			this.Name = "WritingSystemSetup";
			this.Size = new System.Drawing.Size(780, 441);
			this.Load += new System.EventHandler(this.WritingSystemSetup_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this._tabControl.ResumeLayout(false);
			this._basicPage.ResumeLayout(false);
			this._fontsPage.ResumeLayout(false);
			this._sortingPage.ResumeLayout(false);
			this.ResumeLayout(false);

		}


		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton _btnAddWritingSystem;
		private System.Windows.Forms.ToolStripButton _btnRemove;
		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage _basicPage;
		private WritingSystemBasic _basicControl;
		private WritingSystemSortControl _sortControl;
		private System.Windows.Forms.TabPage _fontsPage;
		private FontControl _fontControl;
		private System.Windows.Forms.TabPage _sortingPage;
		private System.Windows.Forms.ListBox _wsListBox;
	}
}
