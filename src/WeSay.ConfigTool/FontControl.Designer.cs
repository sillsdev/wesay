namespace WeSay.ConfigTool
{
	partial class FontControl
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
			this._fontDialog = new System.Windows.Forms.FontDialog();
			this._fontInfoDisplay = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._sampleTextBox = new WeSay.UI.WeSayTextBox();
			this.SuspendLayout();
			//
			// _fontDialog
			//
			this._fontDialog.ShowEffects = false;
			//
			// _fontInfoDisplay
			//
			this._fontInfoDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._fontInfoDisplay.BackColor = System.Drawing.SystemColors.Window;
			this._fontInfoDisplay.Location = new System.Drawing.Point(13, 16);
			this._fontInfoDisplay.Name = "_fontInfoDisplay";
			this._fontInfoDisplay.ReadOnly = true;
			this._fontInfoDisplay.Size = new System.Drawing.Size(209, 20);
			this._fontInfoDisplay.TabIndex = 2;
			//
			// button1
			//
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(228, 16);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(27, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "...";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this._btnFont_Click);
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 59);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Text Sample Area:";
			//
			// _sampleTextBox
			//
			this._sampleTextBox.AcceptsReturn = true;
			this._sampleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._sampleTextBox.BackColor = System.Drawing.Color.White;
			this._sampleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._sampleTextBox.Location = new System.Drawing.Point(13, 75);
			this._sampleTextBox.Multiline = true;
			this._sampleTextBox.MultiParagraph = false;
			this._sampleTextBox.Name = "_sampleTextBox";
			this._sampleTextBox.Size = new System.Drawing.Size(242, 20);
			this._sampleTextBox.TabIndex = 6;
			//
			// FontControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._sampleTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this._fontInfoDisplay);
			this.Name = "FontControl";
			this.Size = new System.Drawing.Size(258, 150);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FontDialog _fontDialog;
		private System.Windows.Forms.TextBox _fontInfoDisplay;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private WeSay.UI.WeSayTextBox _sampleTextBox;
	}
}
