namespace Admin
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._wsListBox = new System.Windows.Forms.ListBox();
			this._tabControl = new System.Windows.Forms.TabControl();
			this._renderingPage = new System.Windows.Forms.TabPage();
			this._sortingPage = new System.Windows.Forms.TabPage();
			this._basicPage = new System.Windows.Forms.TabPage();
			this._typingPage = new System.Windows.Forms.TabPage();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this._tabControl.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
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
			//
			// _tabControl
			//
			this._tabControl.Controls.Add(this._basicPage);
			this._tabControl.Controls.Add(this._renderingPage);
			this._tabControl.Controls.Add(this._typingPage);
			this._tabControl.Controls.Add(this._sortingPage);
			this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tabControl.Location = new System.Drawing.Point(0, 0);
			this._tabControl.Name = "_tabControl";
			this._tabControl.SelectedIndex = 0;
			this._tabControl.Size = new System.Drawing.Size(216, 125);
			this._tabControl.TabIndex = 0;
			//
			// _renderingPage
			//
			this._renderingPage.Location = new System.Drawing.Point(4, 22);
			this._renderingPage.Name = "_renderingPage";
			this._renderingPage.Padding = new System.Windows.Forms.Padding(3);
			this._renderingPage.Size = new System.Drawing.Size(208, 124);
			this._renderingPage.TabIndex = 0;
			this._renderingPage.Text = "Rendering";
			this._renderingPage.UseVisualStyleBackColor = true;
			//
			// _sortingPage
			//
			this._sortingPage.Location = new System.Drawing.Point(4, 22);
			this._sortingPage.Name = "_sortingPage";
			this._sortingPage.Padding = new System.Windows.Forms.Padding(3);
			this._sortingPage.Size = new System.Drawing.Size(208, 124);
			this._sortingPage.TabIndex = 1;
			this._sortingPage.Text = "Sorting";
			this._sortingPage.UseVisualStyleBackColor = true;
			//
			// _basicPage
			//
			this._basicPage.Location = new System.Drawing.Point(4, 22);
			this._basicPage.Name = "_basicPage";
			this._basicPage.Size = new System.Drawing.Size(208, 99);
			this._basicPage.TabIndex = 2;
			this._basicPage.Text = "Basic";
			this._basicPage.UseVisualStyleBackColor = true;
			//
			// _typingPage
			//
			this._typingPage.Location = new System.Drawing.Point(4, 22);
			this._typingPage.Name = "_typingPage";
			this._typingPage.Size = new System.Drawing.Size(208, 124);
			this._typingPage.TabIndex = 3;
			this._typingPage.Text = "Typing";
			this._typingPage.UseVisualStyleBackColor = true;
			//
			// toolStrip1
			//
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripButton1,
			this.toolStripButton2,
			this.toolStripButton3,
			this.toolStripButton4});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(329, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			//
			// toolStripButton1
			//
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton1.Text = "New Writing System";
			//
			// toolStripButton2
			//
			this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton2.Text = "toolStripButton2";
			//
			// toolStripButton3
			//
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton3.Text = "toolStripButton3";
			//
			// toolStripButton4
			//
			this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
			this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton4.Text = "toolStripButton4";
			//
			// WritingSystemSetup
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "WritingSystemSetup";
			this.Size = new System.Drawing.Size(329, 150);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this._tabControl.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox _wsListBox;
		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage _basicPage;
		private System.Windows.Forms.TabPage _renderingPage;
		private System.Windows.Forms.TabPage _sortingPage;
		private System.Windows.Forms.TabPage _typingPage;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripButton toolStripButton4;
	}
}
