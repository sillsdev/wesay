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
			this._projectNameLabel = new WeSay.UI.LocalizableLabel();
			this.SuspendLayout();
			//
			// _vbox
			//
			this._vbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._vbox.AutoScroll = true;
			this._vbox.ColumnCount = 1;
			this._vbox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 493F));
			this._vbox.Location = new System.Drawing.Point(3, 62);
			this._vbox.Name = "_vbox";
			this._vbox.Size = new System.Drawing.Size(493, 277);
			this._vbox.TabIndex = 3;
			//
			// _projectNameLabel
			//
			this._projectNameLabel.AutoSize = true;
			this._projectNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold);
			this._projectNameLabel.Location = new System.Drawing.Point(3, 11);
			this._projectNameLabel.Name = "_projectNameLabel";
			this._projectNameLabel.Size = new System.Drawing.Size(190, 31);
			this._projectNameLabel.TabIndex = 4;
			this._projectNameLabel.Text = "Project Name";
			//
			// DashboardControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._projectNameLabel);
			this.Controls.Add(this._vbox);
			this.Name = "DashboardControl";
			this.Size = new System.Drawing.Size(499, 342);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private WeSay.UI.VBox _vbox;
		private WeSay.UI.LocalizableLabel _projectNameLabel;

	}
}
