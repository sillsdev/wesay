using WeSay.Setup.Properties;

namespace WeSay.Setup
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FieldsControl));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._fieldsListBox = new System.Windows.Forms.CheckedListBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this._writingSystemListBox = new System.Windows.Forms.CheckedListBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._descriptionBox = new System.Windows.Forms.TextBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label2 = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// splitContainer1
			//
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(7, 27);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._fieldsListBox);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
			this.splitContainer1.Size = new System.Drawing.Size(507, 279);
			this.splitContainer1.SplitterDistance = 169;
			this.splitContainer1.TabIndex = 0;
			//
			// _fieldsListBox
			//
			this._fieldsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._fieldsListBox.FormattingEnabled = true;
			this._fieldsListBox.Items.AddRange(new object[] {
			"Word",
			"Meaning (Gloss)",
			"Definition",
			"Example Sentence",
			"Translation of Example Sentence",
			"Semantic Domains",
			"Grammatical Category"});
			this._fieldsListBox.Location = new System.Drawing.Point(0, 10);
			this._fieldsListBox.Name = "_fieldsListBox";
			this._fieldsListBox.Size = new System.Drawing.Size(169, 259);
			this._fieldsListBox.TabIndex = 0;
			this._fieldsListBox.SelectedIndexChanged += new System.EventHandler(this._fieldsListBox_SelectedIndexChanged);
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
			this.tableLayoutPanel1.Size = new System.Drawing.Size(334, 279);
			this.tableLayoutPanel1.TabIndex = 0;
			//
			// groupBox1
			//
			this.groupBox1.Controls.Add(this.btnMoveDown);
			this.groupBox1.Controls.Add(this.btnMoveUp);
			this.groupBox1.Controls.Add(this._writingSystemListBox);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 142);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(7);
			this.groupBox1.Size = new System.Drawing.Size(328, 134);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Show these Writing Systems in this field";
			//
			// btnMoveDown
			//
			this.btnMoveDown.FlatAppearance.BorderSize = 0;
			this.btnMoveDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveDown.Image")));
			this.btnMoveDown.Location = new System.Drawing.Point(7, 46);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(18, 21);
			this.btnMoveDown.TabIndex = 2;
			this.toolTip1.SetToolTip(this.btnMoveDown, "List this writing system later");
			this.btnMoveDown.UseVisualStyleBackColor = true;
			this.btnMoveDown.Click += new System.EventHandler(this.OnBtnMoveDownClick);
			//
			// btnMoveUp
			//
			this.btnMoveUp.FlatAppearance.BorderSize = 0;
			this.btnMoveUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveUp.Image")));
			this.btnMoveUp.Location = new System.Drawing.Point(8, 21);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(17, 19);
			this.btnMoveUp.TabIndex = 1;
			this.toolTip1.SetToolTip(this.btnMoveUp, "List this writing system earlier");
			this.btnMoveUp.UseVisualStyleBackColor = true;
			this.btnMoveUp.Click += new System.EventHandler(this.OnBtnMoveUpClick);
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
			this._writingSystemListBox.Location = new System.Drawing.Point(31, 20);
			this._writingSystemListBox.Name = "_writingSystemListBox";
			this._writingSystemListBox.Size = new System.Drawing.Size(290, 109);
			this._writingSystemListBox.TabIndex = 0;
			this._writingSystemListBox.SelectedIndexChanged += new System.EventHandler(this._writingSystemListBox_SelectedIndexChanged);
			this._writingSystemListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._writingSystemListBox_ItemCheck);
			//
			// groupBox2
			//
			this.groupBox2.Controls.Add(this._descriptionBox);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(3, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(328, 133);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "About This Field";
			//
			// _descriptionBox
			//
			this._descriptionBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._descriptionBox.BackColor = System.Drawing.SystemColors.Window;
			this._descriptionBox.Location = new System.Drawing.Point(7, 20);
			this._descriptionBox.Multiline = true;
			this._descriptionBox.Name = "_descriptionBox";
			this._descriptionBox.ReadOnly = true;
			this._descriptionBox.Size = new System.Drawing.Size(314, 107);
			this._descriptionBox.TabIndex = 0;
			this._descriptionBox.Text = "hello";
			//
			// richTextBox1
			//
			this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Location = new System.Drawing.Point(7, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(507, 21);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "Check each field you want WeSay to display. ";
			//
			// pictureBox1
			//
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.pictureBox1.Image = global::WeSay.Setup.Properties.Resources.construction;
			this.pictureBox1.Location = new System.Drawing.Point(7, 316);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(29, 28);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 16;
			this.pictureBox1.TabStop = false;
			//
			// label2
			//
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.ForeColor = System.Drawing.Color.Gray;
			this.label2.Location = new System.Drawing.Point(40, 316);
			this.label2.MaximumSize = new System.Drawing.Size(250, 0);
			this.label2.MinimumSize = new System.Drawing.Size(350, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(350, 26);
			this.label2.TabIndex = 15;
			this.label2.Text = "You will be able to add custom fields here. For now, you can add them by editting" +
				" the WeSayConfig file directly.";
			//
			// FieldsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "FieldsControl";
			this.Padding = new System.Windows.Forms.Padding(7);
			this.Size = new System.Drawing.Size(521, 348);
			this.Load += new System.EventHandler(this.FieldsControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

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
		private System.Windows.Forms.Button btnMoveUp;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label2;
	}
}
