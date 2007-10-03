namespace WeSay.UI.AutoCompleteTextBox
{
	partial class AutoCompleteWithCreationBox<KV, ValueT>
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;


		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._textBox = new WeSay.UI.AutoCompleteTextBox.WeSayAutoCompleteTextBox();
			this._addNewButton = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			//
			// _textBox
			//
			this._textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._textBox.Location = new System.Drawing.Point(1, 1);
			this._textBox.Multiline = true;
			this._textBox.MultiParagraph = false;
			this._textBox.Name = "_textBox";
			this._textBox.PopupBorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._textBox.PopupOffset = new System.Drawing.Point(0, 0);
			this._textBox.PopupSelectionBackColor = System.Drawing.SystemColors.Highlight;
			this._textBox.PopupSelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this._textBox.PopupWidth = 110;
			this._textBox.Size = new System.Drawing.Size(110, 20);
			this._textBox.TabIndex = 0;
			this._textBox.TooltipToDisplayStringAdaptor = null;
			this._textBox.TextChanged += new System.EventHandler(this.box_TextChanged);
			//
			// _addNewButton
			//
			this._addNewButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._addNewButton.Location = new System.Drawing.Point(110, -1);
			this._addNewButton.Name = "_addNewButton";
			this._addNewButton.Size = new System.Drawing.Size(37, 23);
			this._addNewButton.TabIndex = 1;
			this._addNewButton.Text = "+";
			this._addNewButton.UseVisualStyleBackColor = true;
			this._addNewButton.Click += new System.EventHandler(this.OnAddNewButton_Click);
			//
			// AutoCompleteWithCreationBox
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._addNewButton);
			this.Controls.Add(this._textBox);
			this.Name = "AutoCompleteWithCreationBox";
			this.Size = new System.Drawing.Size(150, 25);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private WeSayAutoCompleteTextBox _textBox;
		private System.Windows.Forms.Button _addNewButton;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}