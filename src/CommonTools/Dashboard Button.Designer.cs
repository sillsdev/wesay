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
			this._label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _label
			//
			this._label.AutoSize = true;
			this._label.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._label.ForeColor = System.Drawing.SystemColors.ControlText;
			this._label.Location = new System.Drawing.Point(8, 11);
			this._label.Name = "_label";
			this._label.Size = new System.Drawing.Size(50, 18);
			this._label.TabIndex = 0;
			this._label.Text = "_label";
			//
			// DashboardButton
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._label);
			this.DoubleBuffered = true;
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.Name = "DashboardButton";
			this.Size = new System.Drawing.Size(166, 50);
			this.Click += new System.EventHandler(this.DashboardButton_Click);
			this.FontChanged += new System.EventHandler(this.DashboardButton_FontChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected Label _label;

	}
}
