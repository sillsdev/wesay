using System;

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
			this._vbox = new WeSay.UI.VBox();
			this._projectNameLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _vbox
			//
			this._vbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				SetAutoSizeToGrowAndShrink();
			}
			this._vbox.Location = new System.Drawing.Point(20, 62);
			this._vbox.Name = "_vbox";
			this._vbox.Size = new System.Drawing.Size(438, 258);
			this._vbox.TabIndex = 3;
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
			//
			// DashboardControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._vbox);
			this.Controls.Add(this._projectNameLabel);
			this.Name = "DashboardControl";
			this.Size = new System.Drawing.Size(499, 342);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void SetAutoSizeToGrowAndShrink()
		{
			this._vbox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		}

		#endregion

		private WeSay.UI.VBox _vbox;
		private System.Windows.Forms.Label _projectNameLabel;

	}
}
