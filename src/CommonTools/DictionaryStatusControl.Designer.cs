namespace WeSay.CommonTools
{
	partial class DictionaryStatusControl
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
			this._dictionarySizeLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _dictionarySizeLabel
			//
			this._dictionarySizeLabel.AutoSize = true;
			this._dictionarySizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._dictionarySizeLabel.Location = new System.Drawing.Point(2, 0);
			this._dictionarySizeLabel.Name = "_dictionarySizeLabel";
			this._dictionarySizeLabel.Size = new System.Drawing.Size(253, 24);
			this._dictionarySizeLabel.TabIndex = 5;
			this._dictionarySizeLabel.Text = "Your dictionary has {0} words";
			//
			// DictionaryStatusControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._dictionarySizeLabel);
			this.Name = "DictionaryStatusControl";
			this.Size = new System.Drawing.Size(538, 41);
			this.ResumeLayout(false);
			this.PerformLayout();
			this.TabStop = false;

		}

		#endregion

		private System.Windows.Forms.Label _dictionarySizeLabel;
	}
}
