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
			this._panel = new System.Windows.Forms.TableLayoutPanel();
			this.SuspendLayout();
			//
			// _panel
			//
			this._panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._panel.AutoSize = true;
			this._panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panel.ColumnCount = 1;
			this._panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panel.Location = new System.Drawing.Point(0, 0);
			this._panel.Name = "_panel";
			this._panel.Padding = new System.Windows.Forms.Padding(10);
			this._panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panel.Size = new System.Drawing.Size(420, 20);
			this._panel.TabIndex = 0;
			//
			// DashboardControl
			//
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._panel);
			this.Name = "DashboardControl";
			this.Size = new System.Drawing.Size(421, 342);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.TableLayoutPanel _panel;
		private System.Windows.Forms.Label _projectNameLabel;
	}
}
