namespace Admin
{
	partial class TaskList
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._taskList = new System.Windows.Forms.CheckedListBox();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			//
			// splitContainer1
			//
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._taskList);
			this.splitContainer1.Size = new System.Drawing.Size(443, 261);
			this.splitContainer1.SplitterDistance = 147;
			this.splitContainer1.TabIndex = 0;
			//
			// _taskList
			//
			this._taskList.Dock = System.Windows.Forms.DockStyle.Fill;
			this._taskList.FormattingEnabled = true;
			this._taskList.Location = new System.Drawing.Point(0, 0);
			this._taskList.Name = "_taskList";
			this._taskList.Size = new System.Drawing.Size(147, 259);
			this._taskList.TabIndex = 0;
			//
			// TaskList
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "TaskList";
			this.Size = new System.Drawing.Size(443, 261);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.CheckedListBox _taskList;
	}
}
