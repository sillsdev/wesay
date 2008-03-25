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
			this._flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._flow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(242)))));
			this._flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._flow.Location = new System.Drawing.Point(20, 11);
			this._flow.Name = "_flow";
			this._flow.Size = new System.Drawing.Size(516, 492);
			this._flow.TabIndex = 0;
			this._flow.WrapContents = false;
			//
			// Dash
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this._flow);
			this.Name = "Dash";
			this.Size = new System.Drawing.Size(563, 519);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel _flow;
	}
}
