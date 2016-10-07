namespace WeSay.ConfigTool.Tasks
{
	partial class WritingSystemFilterControl
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
			this._menuStrip = new System.Windows.Forms.MenuStrip();
			this._writingSystemList = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this._menuStrip.SuspendLayout();
			this.SuspendLayout();
			//
			// _menuStrip
			//
			this._menuStrip.BackColor = System.Drawing.SystemColors.Window;
			this._menuStrip.Dock = System.Windows.Forms.DockStyle.Fill;
			this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this._writingSystemList});
			this._menuStrip.Location = new System.Drawing.Point(0, 0);
			this._menuStrip.MinimumSize = new System.Drawing.Size(100, 0);
			this._menuStrip.Name = "_menuStrip";
			this._menuStrip.Size = new System.Drawing.Size(150, 26);
			this._menuStrip.TabIndex = 30;
			this._menuStrip.Text = "menuStrip2";
			//
			// _writingSystemList
			//
			this._writingSystemList.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripMenuItem5,
			this.toolStripMenuItem2,
			this.toolStripMenuItem3,
			this.toolStripMenuItem4});
			this._writingSystemList.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this._writingSystemList.Name = "_writingSystemList";
			this._writingSystemList.Size = new System.Drawing.Size(40, 22);
			this._writingSystemList.Text = "Any";
			//
			// toolStripMenuItem5
			//
			this.toolStripMenuItem5.CheckOnClick = true;
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem5.Text = "Any";
			//
			// toolStripMenuItem2
			//
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem2.Text = "English";
			//
			// toolStripMenuItem3
			//
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem3.Text = "Voice";
			//
			// toolStripMenuItem4
			//
			this.toolStripMenuItem4.Checked = true;
			this.toolStripMenuItem4.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(152, 22);
			this.toolStripMenuItem4.Text = "Tok Pisin";
			//
			// WritingSystemFilterControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._menuStrip);
			this.Name = "WritingSystemFilterControl";
			this.Size = new System.Drawing.Size(150, 26);
			this._menuStrip.ResumeLayout(false);
			this._menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip _menuStrip;
		private System.Windows.Forms.ToolStripMenuItem _writingSystemList;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
	}
}
