using System;

namespace WeSay.CommonTools
{
	partial class CurrentTaskIndicatorControl
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
			this.label1 = new System.Windows.Forms.Label();
			this._indicatorPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Current task:";
			//
			// _indicatorPanel
			//
			this._indicatorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));

			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				SetAutoSizeToGrowAndShrink();
			}
			this._indicatorPanel.BackColor = System.Drawing.Color.Transparent;
			this._indicatorPanel.Location = new System.Drawing.Point(70, 35);
			this._indicatorPanel.Name = "_indicatorPanel";
			this._indicatorPanel.Size = new System.Drawing.Size(485, 100);
			this._indicatorPanel.TabIndex = 1;

			//
			// CurrentTaskIndicatorControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(253)))), ((int)(((byte)(219)))));
			this.Controls.Add(this._indicatorPanel);
			this.Controls.Add(this.label1);
			this.Name = "CurrentTaskIndicatorControl";
			this.Size = new System.Drawing.Size(563, 138);
			this.SizeChanged += new System.EventHandler(this.CurrentTaskIndicatorControl_SizeChanged);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private void SetAutoSizeToGrowAndShrink()
		{
			this._indicatorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel _indicatorPanel;
	}
}
