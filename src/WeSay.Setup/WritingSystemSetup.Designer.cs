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
			this._sortingPage = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.toolStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this._tabControl.SuspendLayout();
			this._basicPage.SuspendLayout();
			this._fontsPage.SuspendLayout();
			this._sortingPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
			this.toolStrip1.Location = new System.Drawing.Point(3, 266);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(167, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			//
			// _btnAddWritingSystem
			//
			this._btnAddWritingSystem.Image = ((System.Drawing.Image)(resources.GetObject("_btnAddWritingSystem.Image")));
			this._btnAddWritingSystem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAddWritingSystem.Name = "_btnAddWritingSystem";
			this._btnAddWritingSystem.Size = new System.Drawing.Size(48, 22);
			this._btnAddWritingSystem.Text = "New";
			this._btnAddWritingSystem.Click += new System.EventHandler(this._btnAddWritingSystem_Click);
			//
			// _btnRemove
			//
			this._btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("_btnRemove.Image")));
			this._btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnRemove.Name = "_btnRemove";
			this._btnRemove.Size = new System.Drawing.Size(66, 22);
			this._btnRemove.Text = "Remove";
			this._btnRemove.Click += new System.EventHandler(this._btnRemove_Click);
			//
			// splitContainer1
			//
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(0, 25);
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
			this.splitContainer1.Size = new System.Drawing.Size(613, 295);
			this.splitContainer1.SplitterDistance = 172;
			this.splitContainer1.TabIndex = 0;
			//
			// _wsListBox
			//
			this._wsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._wsListBox.FormattingEnabled = true;
			this._wsListBox.Location = new System.Drawing.Point(0, 0);
			this._wsListBox.Name = "_wsListBox";
			this._wsListBox.Size = new System.Drawing.Size(171, 251);
			this._wsListBox.TabIndex = 0;
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
			this._tabControl.Size = new System.Drawing.Size(437, 295);
			this._tabControl.TabIndex = 0;
			//
			// _basicPage
			//
			this._basicPage.Controls.Add(this._basicControl);
			this._basicPage.Location = new System.Drawing.Point(4, 22);
			this._basicPage.Name = "_basicPage";
			this._basicPage.Size = new System.Drawing.Size(429, 269);
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
			this._basicControl.Size = new System.Drawing.Size(429, 269);
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
			this._fontsPage.Size = new System.Drawing.Size(429, 221);
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
			this._fontControl.Size = new System.Drawing.Size(423, 215);
			this._fontControl.TabIndex = 0;
			//
			// _sortingPage
			//
			this._sortingPage.Controls.Add(this.label2);
			this._sortingPage.Controls.Add(this.pictureBox2);
			this._sortingPage.Location = new System.Drawing.Point(4, 22);
			this._sortingPage.Name = "_sortingPage";
			this._sortingPage.Padding = new System.Windows.Forms.Padding(3);
			this._sortingPage.Size = new System.Drawing.Size(429, 221);
			this._sortingPage.TabIndex = 1;
			this._sortingPage.Text = "Sorting";
			this._sortingPage.UseVisualStyleBackColor = true;
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(38, 15);
			this.label2.MaximumSize = new System.Drawing.Size(266, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(250, 52);
			this.label2.TabIndex = 11;
			this.label2.Text = "Here, you\'ll have control over sort order.  Currently, WeSay just uses unicode or" +
				"der, unless the writing system id matches a locale that Windows supports. E.g. F" +
				"R (French), ES (Spanish), TH (Thai)";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			//
			// pictureBox2
			//
			this.pictureBox2.Image = global::WeSay.Setup.Properties.Resources.construction;
			this.pictureBox2.Location = new System.Drawing.Point(6, 15);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(25, 21);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox2.TabIndex = 8;
			this.pictureBox2.TabStop = false;
			//
			// textBox1
			//
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.BackColor = System.Drawing.SystemColors.Window;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.ForeColor = System.Drawing.Color.Gray;
			this.textBox1.Location = new System.Drawing.Point(40, 338);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(569, 41);
			this.textBox1.TabIndex = 7;
			this.textBox1.Text = "Eventually, you\'ll have more control over writing systems.  You\'ll be able to spe" +
				"cify the abbreviation used for display, separately from the code, which will be " +
				"an international standard. ";
			//
			// pictureBox1
			//
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.pictureBox1.Image = global::WeSay.Setup.Properties.Resources.construction;
			this.pictureBox1.Location = new System.Drawing.Point(0, 338);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(26, 24);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 6;
			this.pictureBox1.TabStop = false;
			//
			// WritingSystemSetup
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "WritingSystemSetup";
			this.Size = new System.Drawing.Size(784, 382);
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
			this._sortingPage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox _wsListBox;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton _btnAddWritingSystem;
		private System.Windows.Forms.ToolStripButton _btnRemove;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage _basicPage;
		private WritingSystemBasic _basicControl;
		private System.Windows.Forms.TabPage _fontsPage;
		private FontControl _fontControl;
		private System.Windows.Forms.TabPage _sortingPage;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Label label2;
	}
}
