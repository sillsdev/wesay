namespace WeSay.CommonTools
{
	partial class DashboardControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardControl));
			this._projectNameLabel = new System.Windows.Forms.Label();
			this._dictionarySizeLabel = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// _projectNameLabel
			//
			this._projectNameLabel.AutoSize = true;
			this._projectNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._projectNameLabel.Location = new System.Drawing.Point(14, 13);
			this._projectNameLabel.Name = "_projectNameLabel";
			this._projectNameLabel.Size = new System.Drawing.Size(194, 31);
			this._projectNameLabel.TabIndex = 0;
			this._projectNameLabel.Text = "Lahu Champu";
			this._projectNameLabel.Click += new System.EventHandler(this.label1_Click);
			//
			// _dictionarySizeLabel
			//
			this._dictionarySizeLabel.AutoSize = true;
			this._dictionarySizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._dictionarySizeLabel.Location = new System.Drawing.Point(16, 267);
			this._dictionarySizeLabel.Name = "_dictionarySizeLabel";
			this._dictionarySizeLabel.Size = new System.Drawing.Size(178, 20);
			this._dictionarySizeLabel.TabIndex = 1;
			this._dictionarySizeLabel.Text = "Dictionary has {0} words";
			//
			// pictureBox1
			//
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(45, 55);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(311, 177);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			//
			// DashboardControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this._dictionarySizeLabel);
			this.Controls.Add(this._projectNameLabel);
			this.Name = "DashboardControl";
			this.Size = new System.Drawing.Size(499, 342);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _projectNameLabel;
		private System.Windows.Forms.Label _dictionarySizeLabel;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}
