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
			this._flow = new System.Windows.Forms.FlowLayoutPanel();
			this.SuspendLayout();
			//
			// _flow
			//
			this._flow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._flow.BackColor = System.Drawing.SystemColors.Window;
			this._flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._flow.Location = new System.Drawing.Point(20, 20);
			this._flow.Name = "_flow";
			this._flow.Size = new System.Drawing.Size(534, 493);
			this._flow.TabIndex = 0;
			this._flow.WrapContents = false;
			//
			// Dash
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._flow);
			this.Name = "Dash";
			this.Size = new System.Drawing.Size(581, 534);
			this.Load += new System.EventHandler(this.Dash_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel _flow;
	}
}
