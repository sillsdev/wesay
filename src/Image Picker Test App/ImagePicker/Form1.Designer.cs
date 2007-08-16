namespace ImagePicker
{
	partial class Form1
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
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.imageDisplayWidget1 = new ImagePicker.ImageDisplayWidget();
			this.imageDisplayWidget2 = new ImagePicker.ImageDisplayWidget();
			this.imageDisplayWidget3 = new ImagePicker.ImageDisplayWidget();
			this.SuspendLayout();
			//
			// openFileDialog1
			//
			this.openFileDialog1.FileName = "openFileDialog1";
			//
			// imageDisplayWidget1
			//
			this.imageDisplayWidget1.BackColor = System.Drawing.SystemColors.Control;
			this.imageDisplayWidget1.Location = new System.Drawing.Point(68, 31);
			this.imageDisplayWidget1.Name = "imageDisplayWidget1";
			this.imageDisplayWidget1.Size = new System.Drawing.Size(292, 103);
			this.imageDisplayWidget1.TabIndex = 0;
			//
			// imageDisplayWidget2
			//
			this.imageDisplayWidget2.BackColor = System.Drawing.SystemColors.Control;
			this.imageDisplayWidget2.Location = new System.Drawing.Point(68, 180);
			this.imageDisplayWidget2.Name = "imageDisplayWidget2";
			this.imageDisplayWidget2.Size = new System.Drawing.Size(292, 103);
			this.imageDisplayWidget2.TabIndex = 0;
			//
			// imageDisplayWidget3
			//
			this.imageDisplayWidget3.BackColor = System.Drawing.SystemColors.Control;
			this.imageDisplayWidget3.Location = new System.Drawing.Point(68, 335);
			this.imageDisplayWidget3.Name = "imageDisplayWidget3";
			this.imageDisplayWidget3.Size = new System.Drawing.Size(292, 103);
			this.imageDisplayWidget3.TabIndex = 0;
			//
			// Form1
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(469, 474);
			this.Controls.Add(this.imageDisplayWidget3);
			this.Controls.Add(this.imageDisplayWidget2);
			this.Controls.Add(this.imageDisplayWidget1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private ImageDisplayWidget imageDisplayWidget1;
		private ImageDisplayWidget imageDisplayWidget2;
		private ImageDisplayWidget imageDisplayWidget3;
	}
}
