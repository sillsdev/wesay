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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryStatusControl));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this._dictionarySizeLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// pictureBox1
			//
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(36, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(311, 78);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			//
			// _dictionarySizeLabel
			//
			this._dictionarySizeLabel.AutoSize = true;
			this._dictionarySizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._dictionarySizeLabel.Location = new System.Drawing.Point(32, 99);
			this._dictionarySizeLabel.Name = "_dictionarySizeLabel";
			this._dictionarySizeLabel.Size = new System.Drawing.Size(178, 20);
			this._dictionarySizeLabel.TabIndex = 5;
			this._dictionarySizeLabel.Text = "Dictionary has {0} words";
			//
			// DictionaryStatusControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._dictionarySizeLabel);
			this.Controls.Add(this.pictureBox1);
			this.Name = "DictionaryStatusControl";
			this.Size = new System.Drawing.Size(538, 129);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label _dictionarySizeLabel;
	}
}
