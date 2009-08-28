using Resources=WeSay.ConfigTool.Properties.Resources;

namespace WeSay.ConfigTool
{
	partial class ChorusControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChorusControl));
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this._chorusIsReady = new System.Windows.Forms.Label();
			this._launchChorus = new System.Windows.Forms.LinkLabel();
			this.readinessPanel1 = new Chorus.UI.Misc.ReadinessPanel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// pictureBox1
			//
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(15, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(100, 117);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.label1.Location = new System.Drawing.Point(173, 3);
			this.label1.MaximumSize = new System.Drawing.Size(330, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(329, 221);
			this.label1.TabIndex = 1;
			this.label1.Text = resources.GetString("label1.Text");
			//
			// _chorusIsReady
			//
			this._chorusIsReady.AutoSize = true;
			this._chorusIsReady.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this._chorusIsReady.Location = new System.Drawing.Point(175, 233);
			this._chorusIsReady.MaximumSize = new System.Drawing.Size(330, 0);
			this._chorusIsReady.Name = "_chorusIsReady";
			this._chorusIsReady.Size = new System.Drawing.Size(327, 85);
			this._chorusIsReady.TabIndex = 4;
			this._chorusIsReady.Text = resources.GetString("_chorusIsReady.Text");
			//
			// _launchChorus
			//
			this._launchChorus.AutoSize = true;
			this._launchChorus.Location = new System.Drawing.Point(175, 331);
			this._launchChorus.Name = "_launchChorus";
			this._launchChorus.Size = new System.Drawing.Size(136, 13);
			this._launchChorus.TabIndex = 6;
			this._launchChorus.TabStop = true;
			this._launchChorus.Text = "Run the Chorus Application";
			this._launchChorus.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._launchChorus_LinkClicked);
			//
			// readinessPanel1
			//
			this.readinessPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.readinessPanel1.Location = new System.Drawing.Point(24, 258);
			this.readinessPanel1.Name = "readinessPanel1";
			this.readinessPanel1.Size = new System.Drawing.Size(478, 101);
			this.readinessPanel1.TabIndex = 7;
			//
			// ChorusControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._launchChorus);
			this.Controls.Add(this._chorusIsReady);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.readinessPanel1);
			this.Name = "ChorusControl";
			this.Size = new System.Drawing.Size(614, 385);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label _chorusIsReady;
		private System.Windows.Forms.LinkLabel _launchChorus;
		private Chorus.UI.Misc.ReadinessPanel readinessPanel1;


	}
}
