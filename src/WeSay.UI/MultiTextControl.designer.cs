namespace WeSay.UI
{
	partial class MultiTextControl
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
			this.SuspendLayout();
			//
			// _vbox
			//
			this._vbox.AutoScroll = true;
			this._vbox.Location = new System.Drawing.Point(0, 0);
			this._vbox.Name = "_vbox";
			this._vbox.Size = new System.Drawing.Size(150, 69);
			this._vbox.TabIndex = 0;
			//
			// MultiTextControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._vbox);
			this.Name = "MultiTextControl";
			this.Size = new System.Drawing.Size(150, 69);
			this.Enter += new System.EventHandler(this.MultiTextControl_Enter);
			this.ResumeLayout(false);

		}

		#endregion

		private VBox _vbox;

	}
}
