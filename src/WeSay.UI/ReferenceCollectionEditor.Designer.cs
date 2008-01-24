namespace WeSay.UI
{
	partial class ReferenceCollectionEditor<KV, ValueT, KEY_CONTAINER>
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
			this._flowPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.SuspendLayout();
			_flowPanel.SuspendLayout();
			//
			// _flowPanel
			//
			this._flowPanel.AutoSize = true;
			this._flowPanel.BackColor = System.Drawing.Color.Gainsboro;
			this._flowPanel.Dock = System.Windows.Forms.DockStyle.None;
			this._flowPanel.Location = new System.Drawing.Point(0, 0);
			this._flowPanel.Name = "_flowPanel";
			this._flowPanel.Size = new System.Drawing.Size(309, 37);
			this._flowPanel.TabIndex = 0;
			this._flowPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
			//
			// ReferenceCollectionEditor
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.NavajoWhite;
			this.Controls.Add(this._flowPanel);
			this.Name = "ReferenceCollectionEditor";
			this.Size = new System.Drawing.Size(309, 37);
			this.Load += new System.EventHandler(this.OnLoad);
			this.AutoSize = false;

			this._flowPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		   // this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel _flowPanel;

	}
}
