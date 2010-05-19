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
			this._launchChorus = new System.Windows.Forms.LinkLabel();
			this.readinessPanel1 = new Chorus.UI.Misc.ReadinessPanel();
			this.betterLabel1 = new WeSay.UI.BetterLabel();
			this._chorusIsReady = new WeSay.UI.BetterLabel();
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
			// _launchChorus
			//
			this._launchChorus.AutoSize = true;
			this._launchChorus.Location = new System.Drawing.Point(142, 349);
			this._launchChorus.Name = "_launchChorus";
			this._launchChorus.Size = new System.Drawing.Size(180, 17);
			this._launchChorus.TabIndex = 6;
			this._launchChorus.TabStop = true;
			this._launchChorus.Text = "Run the Chorus Application";
			this._launchChorus.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._launchChorus_LinkClicked);
			//
			// readinessPanel1
			//
			this.readinessPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.readinessPanel1.BackColor = System.Drawing.SystemColors.Window;
			this.readinessPanel1.Location = new System.Drawing.Point(88, 176);
			this.readinessPanel1.Name = "readinessPanel1";
			this.readinessPanel1.ProjectFolderPath = null;
			this.readinessPanel1.Size = new System.Drawing.Size(507, 101);
			this.readinessPanel1.TabIndex = 7;
			//
			// betterLabel1
			//
			this.betterLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.betterLabel1.BackColor = System.Drawing.SystemColors.Window;
			this.betterLabel1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.betterLabel1.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.betterLabel1.Location = new System.Drawing.Point(145, 3);
			this.betterLabel1.Multiline = true;
			this.betterLabel1.Name = "betterLabel1";
			this.betterLabel1.ReadOnly = true;
			this.betterLabel1.Size = new System.Drawing.Size(450, 167);
			this.betterLabel1.TabIndex = 9;
			this.betterLabel1.TabStop = false;
			this.betterLabel1.Text = resources.GetString("betterLabel1.Text");
			//
			// _chorusIsReady
			//
			this._chorusIsReady.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._chorusIsReady.BackColor = System.Drawing.SystemColors.Window;
			this._chorusIsReady.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._chorusIsReady.Font = new System.Drawing.Font("Segoe UI", 10F);
			this._chorusIsReady.Location = new System.Drawing.Point(145, 283);
			this._chorusIsReady.Multiline = true;
			this._chorusIsReady.Name = "_chorusIsReady";
			this._chorusIsReady.ReadOnly = true;
			this._chorusIsReady.Size = new System.Drawing.Size(450, 63);
			this._chorusIsReady.TabIndex = 10;
			this._chorusIsReady.TabStop = false;
			this._chorusIsReady.Text = resources.GetString("_chorusIsReady.Text");
			//
			// ChorusControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._chorusIsReady);
			this.Controls.Add(this.betterLabel1);
			this.Controls.Add(this._launchChorus);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.readinessPanel1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ChorusControl";
			this.Size = new System.Drawing.Size(614, 385);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.LinkLabel _launchChorus;
		private Chorus.UI.Misc.ReadinessPanel readinessPanel1;
		private WeSay.UI.BetterLabel betterLabel1;
		private WeSay.UI.BetterLabel _chorusIsReady;


	}
}
