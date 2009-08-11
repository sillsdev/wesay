namespace Addin.Backup
{
	partial class SynchronizeDialog
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
			this.components = new System.ComponentModel.Container();
			this._checkForUsbKeyTimer = new System.Windows.Forms.Timer(this.components);
			this.localizationHelper1 = new Palaso.UI.WindowsForms.i8n.LocalizationHelper(this.components);
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).BeginInit();
			this.SuspendLayout();
			//
			// _checkForUsbKeyTimer
			//
			this._checkForUsbKeyTimer.Interval = 1000;
			//
			// localizationHelper1
			//
			this.localizationHelper1.Parent = this;
			//
			// SynchronizeDialog
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(611, 552);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SynchronizeDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "~SynchronizeAction With Repository";
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer _checkForUsbKeyTimer;
		private Palaso.UI.WindowsForms.i8n.LocalizationHelper localizationHelper1;
	}
}