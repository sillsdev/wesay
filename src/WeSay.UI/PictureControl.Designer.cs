namespace WeSay.UI
{
	partial class PictureControl
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
			this._pictureBox = new System.Windows.Forms.PictureBox();
			this._problemLabel = new System.Windows.Forms.Label();
			this._removeImageLink = new System.Windows.Forms.LinkLabel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._imageToolboxLink = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
			this.SuspendLayout();
			//
			// _pictureBox
			//
			this._pictureBox.Location = new System.Drawing.Point(0, 0);
			this._pictureBox.Name = "_pictureBox";
			this._pictureBox.Size = new System.Drawing.Size(174, 143);
			this._pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this._pictureBox.TabIndex = 0;
			this._pictureBox.TabStop = false;
			this._pictureBox.Click += new System.EventHandler(this._pictureBox_Click);
			//
			// _problemLabel
			//
			this._problemLabel.AutoSize = true;
			this._problemLabel.ForeColor = System.Drawing.Color.Red;
			this._problemLabel.Location = new System.Drawing.Point(48, 28);
			this._problemLabel.Name = "_problemLabel";
			this._problemLabel.Size = new System.Drawing.Size(107, 13);
			this._problemLabel.TabIndex = 2;
			this._problemLabel.Text = "What is the problem?";
			//
			// _removeImageLink
			//
			this._removeImageLink.AutoSize = true;
			this._removeImageLink.Location = new System.Drawing.Point(180, 28);
			this._removeImageLink.Name = "_removeImageLink";
			this._removeImageLink.Size = new System.Drawing.Size(92, 13);
			this._removeImageLink.TabIndex = 1;
			this._removeImageLink.TabStop = true;
			this._removeImageLink.Text = "Remove Picture...";
			this._removeImageLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._removeImageLink_LinkClicked);
			this._removeImageLink.MouseEnter += new System.EventHandler(this._removeImageLink_MouseEnter);
			this._removeImageLink.MouseLeave += new System.EventHandler(this._removeImageLink_MouseLeave);
			//
			// _imageToolboxLink
			//
			this._imageToolboxLink.AutoSize = true;
			this._imageToolboxLink.Location = new System.Drawing.Point(180, 0);
			this._imageToolboxLink.Name = "_imageToolboxLink";
			this._imageToolboxLink.Size = new System.Drawing.Size(69, 13);
			this._imageToolboxLink.TabIndex = 1;
			this._imageToolboxLink.TabStop = true;
			this._imageToolboxLink.Text = "Get Picture...";
			this._imageToolboxLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnSearchGalleryLink_LinkClicked);
			this._imageToolboxLink.MouseEnter += new System.EventHandler(this._chooseImageLink_MouseEnter);
			this._imageToolboxLink.MouseLeave += new System.EventHandler(this._chooseImageLink_MouseLeave);
			//
			// PictureControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._problemLabel);
			this.Controls.Add(this._removeImageLink);
			this.Controls.Add(this._imageToolboxLink);
			this.Controls.Add(this._pictureBox);
			this.Name = "PictureControl";
			this.Size = new System.Drawing.Size(393, 153);
			this.Load += new System.EventHandler(this.ImageDisplayWidget_Load);
			this.MouseLeave += new System.EventHandler(this.ImageDisplayWidget_MouseLeave);
			this.MouseHover += new System.EventHandler(this.ImageDisplayWidget_MouseHover);
			((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox _pictureBox;
		private System.Windows.Forms.Label _problemLabel;
		private System.Windows.Forms.LinkLabel _removeImageLink;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.LinkLabel _imageToolboxLink;
	}
}