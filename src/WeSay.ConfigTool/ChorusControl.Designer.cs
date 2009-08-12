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
			this._chorusNotReady1 = new System.Windows.Forms.Label();
			this._chorusGetTortoiseLink = new System.Windows.Forms.LinkLabel();
			this._chorusNotReady2 = new System.Windows.Forms.Label();
			this._chorusReadinessMessage = new System.Windows.Forms.Label();
			this._chorusIsReady = new System.Windows.Forms.Label();
			this._chorusGetHgLink = new System.Windows.Forms.LinkLabel();
			this._launchChorus = new System.Windows.Forms.LinkLabel();
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
			// _chorusNotReady1
			//
			this._chorusNotReady1.AutoSize = true;
			this._chorusNotReady1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this._chorusNotReady1.ForeColor = System.Drawing.Color.DarkRed;
			this._chorusNotReady1.Location = new System.Drawing.Point(15, 242);
			this._chorusNotReady1.MaximumSize = new System.Drawing.Size(500, 0);
			this._chorusNotReady1.Name = "_chorusNotReady1";
			this._chorusNotReady1.Size = new System.Drawing.Size(456, 17);
			this._chorusNotReady1.TabIndex = 1;
			this._chorusNotReady1.Text = "But in order for Chorus to work, you need a recent version of Mercurial.";
			//
			// _chorusGetTortoiseLink
			//
			this._chorusGetTortoiseLink.AutoSize = true;
			this._chorusGetTortoiseLink.Location = new System.Drawing.Point(16, 282);
			this._chorusGetTortoiseLink.Name = "_chorusGetTortoiseLink";
			this._chorusGetTortoiseLink.Size = new System.Drawing.Size(222, 13);
			this._chorusGetTortoiseLink.TabIndex = 2;
			this._chorusGetTortoiseLink.TabStop = true;
			this._chorusGetTortoiseLink.Text = "Get TortoiseHg GUI package at SourceForge";
			this._chorusGetTortoiseLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			//
			// _chorusNotReady2
			//
			this._chorusNotReady2.AutoSize = true;
			this._chorusNotReady2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this._chorusNotReady2.ForeColor = System.Drawing.Color.DimGray;
			this._chorusNotReady2.Location = new System.Drawing.Point(13, 331);
			this._chorusNotReady2.MaximumSize = new System.Drawing.Size(300, 0);
			this._chorusNotReady2.Name = "_chorusNotReady2";
			this._chorusNotReady2.Size = new System.Drawing.Size(264, 17);
			this._chorusNotReady2.TabIndex = 1;
			this._chorusNotReady2.Text = "When asked if it was ready, Chorus said:";
			//
			// _chorusReadinessMessage
			//
			this._chorusReadinessMessage.AutoSize = true;
			this._chorusReadinessMessage.ForeColor = System.Drawing.Color.DimGray;
			this._chorusReadinessMessage.Location = new System.Drawing.Point(16, 365);
			this._chorusReadinessMessage.MaximumSize = new System.Drawing.Size(500, 0);
			this._chorusReadinessMessage.Name = "_chorusReadinessMessage";
			this._chorusReadinessMessage.Size = new System.Drawing.Size(141, 13);
			this._chorusReadinessMessage.TabIndex = 3;
			this._chorusReadinessMessage.Text = "Chorus Message Goes here.";
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
			// _chorusGetHgLink
			//
			this._chorusGetHgLink.AutoSize = true;
			this._chorusGetHgLink.Location = new System.Drawing.Point(253, 282);
			this._chorusGetHgLink.Name = "_chorusGetHgLink";
			this._chorusGetHgLink.Size = new System.Drawing.Size(138, 13);
			this._chorusGetHgLink.TabIndex = 5;
			this._chorusGetHgLink.TabStop = true;
			this._chorusGetHgLink.Text = "Or, get Mercurial alone here";
			this._chorusGetHgLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._chorusGetHgLink_LinkClicked);
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
			// ChorusControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._launchChorus);
			this.Controls.Add(this._chorusGetHgLink);
			this.Controls.Add(this._chorusIsReady);
			this.Controls.Add(this._chorusReadinessMessage);
			this.Controls.Add(this._chorusGetTortoiseLink);
			this.Controls.Add(this._chorusNotReady2);
			this.Controls.Add(this._chorusNotReady1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
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
		private System.Windows.Forms.Label _chorusNotReady1;
		private System.Windows.Forms.LinkLabel _chorusGetTortoiseLink;
		private System.Windows.Forms.Label _chorusNotReady2;
		private System.Windows.Forms.Label _chorusReadinessMessage;
		private System.Windows.Forms.Label _chorusIsReady;
		private System.Windows.Forms.LinkLabel _chorusGetHgLink;
		private System.Windows.Forms.LinkLabel _launchChorus;


	}
}
