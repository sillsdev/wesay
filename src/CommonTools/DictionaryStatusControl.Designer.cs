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
			this._logoImage = new System.Windows.Forms.PictureBox();
			this._flow = new System.Windows.Forms.FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._logoImage)).BeginInit();
			this._flow.SuspendLayout();
			this.SuspendLayout();
			//
			// _dictionarySizeLabel
			//
			this._dictionarySizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._dictionarySizeLabel.AutoSize = true;
			this._dictionarySizeLabel.Font = new System.Drawing.Font("Arial", 16F);
			this._dictionarySizeLabel.Location = new System.Drawing.Point(67, 27);
			this._dictionarySizeLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
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
			// _logoImage
			//
			this._logoImage.Image = global::WeSay.CommonTools.Properties.Resources.blueWeSay;
			this._logoImage.Location = new System.Drawing.Point(3, 3);
			this._logoImage.Name = "_logoImage";
			this._logoImage.Size = new System.Drawing.Size(58, 50);
			this._logoImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this._logoImage.TabIndex = 6;
			this._logoImage.TabStop = false;
			this._logoImage.Visible = false;
			//
			// _flow
			//
			this._flow.Controls.Add(this._logoImage);
			this._flow.Controls.Add(this._dictionarySizeLabel);
			this._flow.Dock = System.Windows.Forms.DockStyle.Fill;
			this._flow.Location = new System.Drawing.Point(0, 0);
			this._flow.Name = "_flow";
			this._flow.Size = new System.Drawing.Size(538, 57);
			this._flow.TabIndex = 7;
			//
			// DictionaryStatusControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._flow);
			this.Name = "DictionaryStatusControl";
			this.Size = new System.Drawing.Size(538, 57);
			this.FontChanged += new System.EventHandler(this.DictionaryStatusControl_FontChanged);
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._logoImage)).EndInit();
			this._flow.ResumeLayout(false);
			this._flow.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label _dictionarySizeLabel;
		private WeSay.UI.LocalizationHelper localizationHelper1;
		private System.Windows.Forms.PictureBox _logoImage;
		private System.Windows.Forms.FlowLayoutPanel _flow;
	}
}
