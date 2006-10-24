namespace WeSay.Admin
{
	partial class TaskListControl
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._description = new System.Windows.Forms.TextBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			//
			// splitContainer1
			//
			this.splitContainer1.Location = new System.Drawing.Point(0, 54);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._taskList);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Size = new System.Drawing.Size(443, 207);
			this.splitContainer1.SplitterDistance = 147;
			this.splitContainer1.TabIndex = 0;
			//
			// _taskList
			//
			this._taskList.Dock = System.Windows.Forms.DockStyle.Fill;
			this._taskList.FormattingEnabled = true;
			this._taskList.Location = new System.Drawing.Point(0, 0);
			this._taskList.Name = "_taskList";
			this._taskList.Size = new System.Drawing.Size(147, 199);
			this._taskList.TabIndex = 0;
			this._taskList.SelectedIndexChanged += new System.EventHandler(this._taskList_SelectedIndexChanged);
			this._taskList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._taskList_ItemCheck);
			//
			// groupBox1
			//
			this.groupBox1.Controls.Add(this._description);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(292, 207);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "About this Task";
			//
			// _description
			//
			this._description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._description.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._description.Location = new System.Drawing.Point(11, 20);
			this._description.Multiline = true;
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(262, 179);
			this._description.TabIndex = 0;
			//
			// richTextBox1
			//
			this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Location = new System.Drawing.Point(0, 10);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(443, 23);
			this.richTextBox1.TabIndex = 2;
			this.richTextBox1.Text = "Check each task you want to make available inside WeSay.";
			//
			// TaskListControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "TaskListControl";
			this.Size = new System.Drawing.Size(443, 261);
			this.Load += new System.EventHandler(this.TaskList_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.CheckedListBox _taskList;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.TextBox _description;
	}
}
