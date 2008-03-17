using System;
using System.Windows.Forms;

namespace WeSay.CommonTools
{
	partial class DashboardButton
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
			this.SuspendLayout();
			//
			// DashboardButton
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.DoubleBuffered = true;
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.Name = "DashboardButton";
			this.Size = new System.Drawing.Size(166, 50);
			this.Click += new System.EventHandler(this.DashboardButton_Click);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DashboardButton_MouseDown);
			this.MouseLeave += new System.EventHandler(this.DashboardButton_MouseLeave);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DashboardButton_MouseUp);
			this.ResumeLayout(false);

		}

		#endregion


	}
}
