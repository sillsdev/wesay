namespace Addin.Backup
{
	partial class BackupDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._topLabel = new System.Windows.Forms.Label();
			this._noteLabel = new System.Windows.Forms.Label();
			this._cancelButton = new System.Windows.Forms.Button();
			this._checkForUsbKeyTimer = new System.Windows.Forms.Timer(this.components);
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.localizationHelper1 = new WeSay.UI.LocalizationHelper(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).BeginInit();
			this.SuspendLayout();
			//
			// _topLabel
			//
			this._topLabel.AutoSize = true;
			this._topLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._topLabel.Location = new System.Drawing.Point(178, 12);
			this._topLabel.Name = "_topLabel";
			this._topLabel.Size = new System.Drawing.Size(144, 20);
			this._topLabel.TabIndex = 1;
			this._topLabel.Text = "status goes here";
			//
			// _noteLabel
			//
			this._noteLabel.AutoSize = true;
			this._noteLabel.Location = new System.Drawing.Point(179, 51);
			this._noteLabel.Name = "_noteLabel";
			this._noteLabel.Size = new System.Drawing.Size(139, 13);
			this._noteLabel.TabIndex = 2;
			this._noteLabel.Text = "Secondary status goes here";
			this._noteLabel.Visible = false;
			//
			// _cancelButton
			//
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(433, 88);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 3;
			this._cancelButton.Text = "~&Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			//
			// _checkForUsbKeyTimer
			//
			this._checkForUsbKeyTimer.Interval = 1000;
			this._checkForUsbKeyTimer.Tick += new System.EventHandler(this.OnCheckForUsbKeyTimer_Tick);
			//
			// pictureBox1
			//
			this.pictureBox1.Location = new System.Drawing.Point(13, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(99, 80);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			//
			// localizationHelper1
			//
			this.localizationHelper1.Parent = this;
			//
			// BackupDialog
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(526, 123);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._noteLabel);
			this.Controls.Add(this._topLabel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BackupDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Backup To USB Key";
			this.Load += new System.EventHandler(this.Dialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _topLabel;
		private System.Windows.Forms.Label _noteLabel;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Timer _checkForUsbKeyTimer;
		private System.Windows.Forms.PictureBox pictureBox1;
		private WeSay.UI.LocalizationHelper localizationHelper1;
	}
}