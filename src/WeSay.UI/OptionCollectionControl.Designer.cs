namespace WeSay.UI
{
	partial class OptionCollectionControl
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
			this._textBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			//
			// _textBox
			//
			this._textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._textBox.Location = new System.Drawing.Point(0, 0);
			this._textBox.Name = "_textBox";
			this._textBox.ReadOnly = true;
			this._textBox.Size = new System.Drawing.Size(262, 13);
			this._textBox.TabIndex = 0;
			this._textBox.TabStop = false;
			//
			// OptionCollectionControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._textBox);
			this.Name = "OptionCollectionControl";
			this.Size = new System.Drawing.Size(262, 26);
			this.Load += new System.EventHandler(this.OptionCollectionControl_Load);
			this.BackColorChanged += new System.EventHandler(this.OptionCollectionControl_BackColorChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _textBox;
	}
}
