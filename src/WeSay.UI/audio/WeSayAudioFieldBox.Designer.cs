namespace WeSay.UI.audio
{
	partial class WeSayAudioFieldBox
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
			this._fileName = new System.Windows.Forms.TextBox();
			this._shortSoundFieldControl1 = new SIL.Media.ShortSoundFieldControl();
			this.SuspendLayout();
			//
			// _fileName
			//
			this._fileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
																		  | System.Windows.Forms.AnchorStyles.Right)));
			this._fileName.Location = new System.Drawing.Point(240, 3);
			this._fileName.Name = "_fileName";
			this._fileName.Size = new System.Drawing.Size(15, 20);
			this._fileName.TabIndex = 1;
			this._fileName.Visible = false;
			this._fileName.TextChanged += new System.EventHandler(this._fileName_TextChanged);
			//
			// shortSoundFieldControl1
			//
			this._shortSoundFieldControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
																						| System.Windows.Forms.AnchorStyles.Right)));
			this._shortSoundFieldControl1.Location = new System.Drawing.Point(0, 2);
			this._shortSoundFieldControl1.Name = "_shortSoundFieldControl1";
			this._shortSoundFieldControl1.Path = null;
			this._shortSoundFieldControl1.Size = new System.Drawing.Size(234, 19);
			this._shortSoundFieldControl1.TabIndex = 2;
			//
			// WeSayAudioFieldBox
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._shortSoundFieldControl1);
			this.Controls.Add(this._fileName);
			this.Name = "WeSayAudioFieldBox";
			this.Size = new System.Drawing.Size(255, 23);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _fileName;
		private SIL.Media.ShortSoundFieldControl _shortSoundFieldControl1;

	}
}