namespace ImagePicker
{
	partial class ImageDisplayWidget
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
			this._pictureBox = new System.Windows.Forms.PictureBox();
			this._chooseImageLink = new System.Windows.Forms.LinkLabel();
			this._problemLabel = new System.Windows.Forms.Label();
			this._removeImageLink = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
			this.SuspendLayout();
			//
			// _pictureBox
			//
			this._pictureBox.Location = new System.Drawing.Point(6, 3);
			this._pictureBox.Name = "_pictureBox";
			this._pictureBox.Size = new System.Drawing.Size(97, 82);
			this._pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this._pictureBox.TabIndex = 0;
			this._pictureBox.TabStop = false;
			//
			// _chooseImageLink
			//
			this._chooseImageLink.AutoSize = true;
			this._chooseImageLink.Location = new System.Drawing.Point(166, 0);
			this._chooseImageLink.Name = "_chooseImageLink";
			this._chooseImageLink.Size = new System.Drawing.Size(84, 13);
			this._chooseImageLink.TabIndex = 1;
			this._chooseImageLink.TabStop = true;
			this._chooseImageLink.Text = "Choose Image...";
			this._chooseImageLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._chooseImageLink_LinkClicked);
			//
			// _problemLabel
			//
			this._problemLabel.AutoSize = true;
			this._problemLabel.ForeColor = System.Drawing.Color.Red;
			this._problemLabel.Location = new System.Drawing.Point(3, 0);
			this._problemLabel.Name = "_problemLabel";
			this._problemLabel.Size = new System.Drawing.Size(107, 13);
			this._problemLabel.TabIndex = 2;
			this._problemLabel.Text = "What is the problem?";
			//
			// _removeImageLink
			//
			this._removeImageLink.AutoSize = true;
			this._removeImageLink.Location = new System.Drawing.Point(176, 0);
			this._removeImageLink.Name = "_removeImageLink";
			this._removeImageLink.Size = new System.Drawing.Size(88, 13);
			this._removeImageLink.TabIndex = 1;
			this._removeImageLink.TabStop = true;
			this._removeImageLink.Text = "Remove Image...";
			this._removeImageLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._removeImageLink_LinkClicked);
			//
			// ImageDisplayWidget
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._problemLabel);
			this.Controls.Add(this._removeImageLink);
			this.Controls.Add(this._chooseImageLink);
			this.Controls.Add(this._pictureBox);
			this.Name = "ImageDisplayWidget";
			this.Size = new System.Drawing.Size(292, 103);
			this.Load += new System.EventHandler(this.ImageDisplayWidget_Load);
			((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox _pictureBox;
		private System.Windows.Forms.LinkLabel _chooseImageLink;
		private System.Windows.Forms.Label _problemLabel;
		private System.Windows.Forms.LinkLabel _removeImageLink;
	}
}
