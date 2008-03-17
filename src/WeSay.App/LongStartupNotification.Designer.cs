namespace WeSay.App
{
	partial class LongStartupNotification
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LongStartupNotification));
			this._message = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _message
			//
			this._message.AutoSize = true;
			this._message.Location = new System.Drawing.Point(13, 13);
			this._message.Name = "_message";
			this._message.Size = new System.Drawing.Size(35, 13);
			this._message.TabIndex = 0;
			this._message.Text = "label1";
			this._message.UseWaitCursor = true;
			//
			// LongStartupNotification
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(399, 147);
			this.Controls.Add(this._message);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LongStartupNotification";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "WeSay";
			this.TopMost = true;
			this.UseWaitCursor = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _message;
	}
}