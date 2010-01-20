namespace WeSay.LexicalTools.DictionaryBrowseAndEdit
{
	partial class SearchBoxControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchBoxControl));
			this._textToSearchForBox = new WeSay.UI.AutoCompleteTextBox.WeSayAutoCompleteTextBox();
			this._writingSystemChooser = new System.Windows.Forms.Button();
			this._selectedWritingSystemLabel = new System.Windows.Forms.Label();
			this._desperationDisplayTimer = new System.Windows.Forms.Timer(this.components);
			this._findButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// _textToSearchForBox
			//
			this._textToSearchForBox.BackColor = System.Drawing.Color.White;
			this._textToSearchForBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._textToSearchForBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._textToSearchForBox.IsSpellCheckingEnabled = false;
			this._textToSearchForBox.Location = new System.Drawing.Point(37, 4);
			this._textToSearchForBox.Multiline = true;
			this._textToSearchForBox.MultiParagraph = false;
			this._textToSearchForBox.Name = "_textToSearchForBox";
			this._textToSearchForBox.PopupBorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._textToSearchForBox.PopupOffset = new System.Drawing.Point(0, 0);
			this._textToSearchForBox.PopupSelectionBackColor = System.Drawing.SystemColors.Highlight;
			this._textToSearchForBox.PopupSelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this._textToSearchForBox.PopupWidth = 200;
			this._textToSearchForBox.SelectedItem = null;
			this._textToSearchForBox.Size = new System.Drawing.Size(68, 22);
			this._textToSearchForBox.TabIndex = 7;
			this._textToSearchForBox.WordWrap = false;
			//
			// _writingSystemChooser
			//
			this._writingSystemChooser.BackColor = this._textToSearchForBox.BackColor;
			this._writingSystemChooser.FlatAppearance.BorderSize = 0;
			this._writingSystemChooser.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this._writingSystemChooser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._writingSystemChooser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._writingSystemChooser.Location = new System.Drawing.Point(130, 4);
			this._writingSystemChooser.Name = "_writingSystemChooser";
			this._writingSystemChooser.Size = new System.Drawing.Size(15, 19);
			this._writingSystemChooser.TabIndex = 9;
			this._writingSystemChooser.UseVisualStyleBackColor = false;
			this._writingSystemChooser.Click += new System.EventHandler(this.OnWritingSystemChooser_Click);
			//
			// _selectedWritingSystemLabel
			//
			this._selectedWritingSystemLabel.BackColor = System.Drawing.Color.White;
			this._selectedWritingSystemLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._selectedWritingSystemLabel.ForeColor = System.Drawing.Color.LightGray;
			this._selectedWritingSystemLabel.Location = new System.Drawing.Point(6, 3);
			this._selectedWritingSystemLabel.Name = "_selectedWritingSystemLabel";
			this._selectedWritingSystemLabel.Size = new System.Drawing.Size(20, 19);
			this._selectedWritingSystemLabel.TabIndex = 10;
			this._selectedWritingSystemLabel.Text = "en";
			this._selectedWritingSystemLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// _desperationDisplayTimer
			//
			this._desperationDisplayTimer.Tick += new System.EventHandler(this._desperationDisplayTimer_Tick);
			//
			// _findButton
			//
			this._findButton.BackColor = this._textToSearchForBox.BackColor;
			this._findButton.FlatAppearance.BorderSize = 0;
			this._findButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this._findButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._findButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._findButton.Image = ((System.Drawing.Image)(resources.GetObject("_findButton.Image")));
			this._findButton.Location = new System.Drawing.Point(111, 1);
			this._findButton.Name = "_findButton";
			this._findButton.Size = new System.Drawing.Size(19, 27);
			this._findButton.TabIndex = 8;
			this._findButton.UseVisualStyleBackColor = false;
			//
			// SearchBoxControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._findButton);
			this.Controls.Add(this._textToSearchForBox);
			this.Controls.Add(this._writingSystemChooser);
			this.Controls.Add(this._selectedWritingSystemLabel);
			this.Name = "SearchBoxControl";
			this.Size = new System.Drawing.Size(148, 31);
			this.Load += new System.EventHandler(this.SearchBoxControl_Load);
			this.Resize += new System.EventHandler(this.SearchBoxControl_SizeChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _findButton;
		private WeSay.UI.AutoCompleteTextBox.WeSayAutoCompleteTextBox _textToSearchForBox;
		private System.Windows.Forms.Button _writingSystemChooser;
		private System.Windows.Forms.Label _selectedWritingSystemLabel;
		private System.Windows.Forms.Timer _desperationDisplayTimer;
	}
}
