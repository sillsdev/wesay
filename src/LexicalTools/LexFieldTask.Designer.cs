using System;

namespace WeSay.LexicalTools
{
	partial class LexFieldTask
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
			this._recordsListBox = new System.Windows.Forms.ListBox();
			this._lexFieldDetailPanel = new LexFieldControl();
			this.SuspendLayout();
			//
			// _recordsListBox
			//
			this._recordsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this._recordsListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._recordsListBox.FormattingEnabled = true;
			this._recordsListBox.ItemHeight = 20;
			this._recordsListBox.Location = new System.Drawing.Point(0, 5);
			this._recordsListBox.Name = "_recordsListBox";
			this._recordsListBox.Size = new System.Drawing.Size(120, 124);
			this._recordsListBox.TabIndex = 2;
			//
			// _detailPanel
			//
			this._lexFieldDetailPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lexFieldDetailPanel.AutoScroll = true;
			this._lexFieldDetailPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this._lexFieldDetailPanel.Location = new System.Drawing.Point(126, 5);

			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				//     this._lexFieldDetailPanel.Margin = new System.Windows.Forms.Padding(5);
			}
			this._lexFieldDetailPanel.Name = "_lexFieldDetailPanel";
			this._lexFieldDetailPanel.Size = new System.Drawing.Size(367, 122);
			this._lexFieldDetailPanel.TabIndex = 4;
			//
			// LexFieldTask
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._lexFieldDetailPanel);
			this.Controls.Add(this._recordsListBox);
			this.Name = "LexFieldTask";
			this.Size = new System.Drawing.Size(493, 169);
			this.ResumeLayout(true);

		}

		#endregion

		private System.Windows.Forms.ListBox _recordsListBox;
		private LexFieldControl _lexFieldDetailPanel;
	}
}
