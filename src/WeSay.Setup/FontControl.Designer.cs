namespace WeSay.Setup
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
			this._btnFont = new System.Windows.Forms.Button();
			this._fontDialog = new System.Windows.Forms.FontDialog();
			this._fontProperties = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			//
			// _btnFont
			//
			this._btnFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._btnFont.AutoSize = true;
			this._btnFont.Location = new System.Drawing.Point(9, 124);
			this._btnFont.Name = "_btnFont";
			this._btnFont.Size = new System.Drawing.Size(99, 23);
			this._btnFont.TabIndex = 1;
			this._btnFont.Text = "&Change Font...";
			this._btnFont.UseVisualStyleBackColor = true;
			this._btnFont.Click += new System.EventHandler(this._btnFont_Click);
			//
			// _fontDialog
			//
			this._fontDialog.ShowColor = true;
			//
			// _fontProperties
			//
			this._fontProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._fontProperties.Enabled = false;
			this._fontProperties.HelpVisible = false;
			this._fontProperties.Location = new System.Drawing.Point(4, 3);
			this._fontProperties.Name = "_fontProperties";
			this._fontProperties.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this._fontProperties.Size = new System.Drawing.Size(130, 115);
			this._fontProperties.TabIndex = 3;
			this._fontProperties.ToolbarVisible = false;
			//
			// FontControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._fontProperties);
			this.Controls.Add(this._btnFont);
			this.Name = "FontControl";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _btnFont;
		private System.Windows.Forms.FontDialog _fontDialog;
		private System.Windows.Forms.PropertyGrid _fontProperties;
	}
}
