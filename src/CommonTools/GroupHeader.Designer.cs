namespace WeSay.CommonTools
{
	partial class GroupHeader
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
			this._title = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _title
			//
			this._title.AutoSize = true;
			this._title.Location = new System.Drawing.Point(-3, 4);
			this._title.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this._title.Name = "_title";
			this._title.Size = new System.Drawing.Size(60, 24);
			this._title.TabIndex = 0;
			this._title.Text = "label1";
			//
			// GroupHeader
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._title);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.Name = "GroupHeader";
			this.Size = new System.Drawing.Size(396, 33);
			this.Load += new System.EventHandler(this.GroupHeader_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _title;

	}
}
