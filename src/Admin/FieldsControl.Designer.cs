namespace WeSay.Admin
{
	partial class FieldsControl
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
			this._fieldsListBox = new System.Windows.Forms.CheckedListBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._writingSystemListBox = new System.Windows.Forms.CheckedListBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._descriptionBox = new System.Windows.Forms.TextBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			//
			// splitContainer1
			//
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(7, 57);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._fieldsListBox);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
			this.splitContainer1.Size = new System.Drawing.Size(507, 267);
			this.splitContainer1.SplitterDistance = 169;
			this.splitContainer1.TabIndex = 0;
			//
			// _fieldsListBox
			//
			this._fieldsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldsListBox.FormattingEnabled = true;
			this._fieldsListBox.Items.AddRange(new object[] {
			"Word",
			"Meaning (Gloss)",
			"Definition",
			"Example Sentence",
			"Translation of Example Sentence",
			"Semantic Domains",
			"Grammatical Category"});
			this._fieldsListBox.Location = new System.Drawing.Point(0, 0);
			this._fieldsListBox.Name = "_fieldsListBox";
			this._fieldsListBox.Size = new System.Drawing.Size(169, 259);
			this._fieldsListBox.TabIndex = 0;
			//
			// tableLayoutPanel1
			//
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(334, 267);
			this.tableLayoutPanel1.TabIndex = 0;
			//
			// groupBox1
			//
			this.groupBox1.Controls.Add(this._writingSystemListBox);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 136);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(7);
			this.groupBox1.Size = new System.Drawing.Size(328, 128);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Show these Writing Systems in this field";
			//
			// _writingSystemListBox
			//
			this._writingSystemListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._writingSystemListBox.FormattingEnabled = true;
			this._writingSystemListBox.Items.AddRange(new object[] {
			"Foo(IPA)",
			"Foo(Thai)",
			"Thai",
			"English"});
			this._writingSystemListBox.Location = new System.Drawing.Point(7, 20);
			this._writingSystemListBox.Name = "_writingSystemListBox";
			this._writingSystemListBox.Size = new System.Drawing.Size(314, 94);
			this._writingSystemListBox.TabIndex = 0;
			//
			// groupBox2
			//
			this.groupBox2.Controls.Add(this._descriptionBox);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(3, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(328, 127);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "About This Field";
			//
			// _descriptionBox
			//
			this._descriptionBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._descriptionBox.Location = new System.Drawing.Point(7, 20);
			this._descriptionBox.Multiline = true;
			this._descriptionBox.Name = "_descriptionBox";
			this._descriptionBox.Size = new System.Drawing.Size(314, 101);
			this._descriptionBox.TabIndex = 0;
			//
			// richTextBox1
			//
			this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.richTextBox1.Location = new System.Drawing.Point(7, 7);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(507, 45);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "Check each field you want WeSay to display. ";
			//
			// FieldsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "FieldsControl";
			this.Padding = new System.Windows.Forms.Padding(7);
			this.Size = new System.Drawing.Size(521, 331);
			this.Load += new System.EventHandler(this.FieldsControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.CheckedListBox _fieldsListBox;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckedListBox _writingSystemListBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox _descriptionBox;
	}
}
