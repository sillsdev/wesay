namespace WeSay.CommonTools
{
	partial class Dash
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
			System.Windows.Forms.Label DeveloperNote;
			this._panel = new System.Windows.Forms.Panel();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			DeveloperNote = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// DeveloperNote
			//
			DeveloperNote.AutoSize = true;
			DeveloperNote.Enabled = false;
			DeveloperNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			DeveloperNote.Location = new System.Drawing.Point(-3, 0);
			DeveloperNote.Name = "DeveloperNote";
			DeveloperNote.Size = new System.Drawing.Size(547, 15);
			DeveloperNote.TabIndex = 1;
			DeveloperNote.Text = "NOTE: This control should be the same size as the initial tab page size so the in" +
				"itial layout is correct.";
			DeveloperNote.Visible = false;
			//
			// _panel
			//
			this._panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panel.BackColor = System.Drawing.Color.Transparent;
			this._panel.Location = new System.Drawing.Point(20, 11);
			this._panel.Name = "_panel";
			this._panel.Size = new System.Drawing.Size(603, 397);
			this._panel.TabIndex = 0;
			//
			// _toolTip
			//
			this._toolTip.AutoPopDelay = 15000;
			this._toolTip.InitialDelay = 500;
			this._toolTip.OwnerDraw = true;
			this._toolTip.ReshowDelay = 100;
			this._toolTip.Popup += new System.Windows.Forms.PopupEventHandler(this._toolTip_Popup);
			this._toolTip.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this._toolTip_Draw);
			//
			// Dash
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(242)))));
			this.Controls.Add(DeveloperNote);
			this.Controls.Add(this._panel);
			this.Name = "Dash";
			this.Size = new System.Drawing.Size(623, 408);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel _panel;
		private System.Windows.Forms.ToolTip _toolTip;
	}
}
