namespace WeSay.Setup
{
	partial class InterfaceLanguageControl
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
			System.Windows.Forms.Button _chooseFontButton;
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._languageCombo = new System.Windows.Forms.ComboBox();
			this._fontInfoDisplay = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			_chooseFontButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// _languageCombo
			//
			this._languageCombo.FormattingEnabled = true;
			this._languageCombo.Location = new System.Drawing.Point(119, 3);
			this._languageCombo.Name = "_languageCombo";
			this._languageCombo.Size = new System.Drawing.Size(229, 21);
			this._languageCombo.TabIndex = 0;
			this.toolTip1.SetToolTip(this._languageCombo, "The language (e.g., national language) to use for labels, buttons, etc.");
			//
			// _chooseFontButton
			//
			_chooseFontButton.Location = new System.Drawing.Point(354, 37);
			_chooseFontButton.Name = "_chooseFontButton";
			_chooseFontButton.Size = new System.Drawing.Size(27, 23);
			_chooseFontButton.TabIndex = 2;
			_chooseFontButton.Text = "...";
			_chooseFontButton.UseVisualStyleBackColor = true;
			_chooseFontButton.Click += new System.EventHandler(this.OnChooseFont);
			//
			// _fontInfoDisplay
			//
			this._fontInfoDisplay.BackColor = System.Drawing.SystemColors.Window;
			this._fontInfoDisplay.Location = new System.Drawing.Point(119, 39);
			this._fontInfoDisplay.Name = "_fontInfoDisplay";
			this._fontInfoDisplay.ReadOnly = true;
			this._fontInfoDisplay.Size = new System.Drawing.Size(229, 20);
			this._fontInfoDisplay.TabIndex = 1;
			//
			// label4
			//
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 39);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(73, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Interface Font";
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Interface Language";
			//
			// InterfaceLanguageControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(_chooseFontButton);
			this.Controls.Add(this._fontInfoDisplay);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._languageCombo);
			this.Name = "InterfaceLanguageControl";
			this.Size = new System.Drawing.Size(444, 235);
			this.Load += new System.EventHandler(this.OnLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TextBox _fontInfoDisplay;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox _languageCombo;
	}
}
