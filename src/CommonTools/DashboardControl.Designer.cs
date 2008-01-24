using System;
using System.Windows.Forms;

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
			this._vbox = new System.Windows.Forms.TableLayoutPanel();
			this.SuspendLayout();
			//
			// _vbox
			//
			this._vbox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
			this._vbox.Margin = new Padding(10, 10, 10, 10);
			this._vbox.Padding = new Padding(10, 10, 10, 10);
			this._vbox.Dock = DockStyle.Fill;
			this._vbox.DockPadding.All = 10;
			this._vbox.ColumnCount = 1;
			this._vbox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
			this._vbox.Location = new System.Drawing.Point(3, 62);
			this._vbox.Name = "_vbox";
			this._vbox.Size = new System.Drawing.Size(493, 277);
			this._vbox.TabIndex = 3;
			this._vbox.AutoScroll = true;
			this._vbox.VerticalScroll.Visible = true;
			//
			// DashboardControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			//this.VerticalScroll.Visible = true;

			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._vbox);
			this.Name = "DashboardControl";
			this.Size = new System.Drawing.Size(499, 342);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _vbox;
		private System.Windows.Forms.Label _projectNameLabel;

	}
}
