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
			this.components = new System.ComponentModel.Container();
			this._dictionarySizeLabel = new System.Windows.Forms.Label();
			this.localizationHelper1 = new WeSay.UI.LocalizationHelper(this.components);
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// _dictionarySizeLabel
			//
			this._dictionarySizeLabel.AutoSize = true;
			this._dictionarySizeLabel.Font = new System.Drawing.Font("Arial", 16F);
			this._dictionarySizeLabel.Location = new System.Drawing.Point(3, 19);
			this._dictionarySizeLabel.Name = "_dictionarySizeLabel";
			this._dictionarySizeLabel.Size = new System.Drawing.Size(305, 25);
			this._dictionarySizeLabel.TabIndex = 5;
			this._dictionarySizeLabel.Text = "~Your dictionary has {0} words";
			this._dictionarySizeLabel.Click += new System.EventHandler(this._dictionarySizeLabel_Click);
			this._dictionarySizeLabel.FontChanged += new System.EventHandler(this._dictionarySizeLabel_FontChanged);
			//
			// localizationHelper1
			//
			this.localizationHelper1.Parent = this;
			//
			// pictureBox1
			//
			this.pictureBox1.Image = global::WeSay.CommonTools.Properties.Resources.blueWeSay;
			this.pictureBox1.Location = new System.Drawing.Point(381, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(58, 50);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 6;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Visible = false;
			//
			// DictionaryStatusControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this._dictionarySizeLabel);
			this.Name = "DictionaryStatusControl";
			this.Size = new System.Drawing.Size(538, 57);
			this.FontChanged += new System.EventHandler(this.DictionaryStatusControl_FontChanged);
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _dictionarySizeLabel;
		private WeSay.UI.LocalizationHelper localizationHelper1;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}
