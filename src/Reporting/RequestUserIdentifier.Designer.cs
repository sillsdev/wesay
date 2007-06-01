namespace Reporting
{
	partial class RequestUserIdentifier
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequestUserIdentifier));
			this._emailAddress = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._okButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _emailAddress
			//
			this._emailAddress.Location = new System.Drawing.Point(26, 77);
			this._emailAddress.Name = "_emailAddress";
			this._emailAddress.Size = new System.Drawing.Size(228, 20);
			this._emailAddress.TabIndex = 0;
			this._emailAddress.TextChanged += new System.EventHandler(this._emailAddress_TextChanged);
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 117);
			this.label1.MaximumSize = new System.Drawing.Size(230, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(225, 143);
			this.label1.TabIndex = 1;
			this.label1.Text = resources.GetString("label1.Text");
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(23, 61);
			this.label2.MaximumSize = new System.Drawing.Size(200, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(73, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Email Address";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			//
			// _okButton
			//
			this._okButton.Location = new System.Drawing.Point(178, 291);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 1;
			this._okButton.Text = "&Register";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(23, 18);
			this.label3.MaximumSize = new System.Drawing.Size(200, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(126, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Welcome To WeSay!";
			//
			// RequestUserIdentifier
			//
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(291, 327);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._emailAddress);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RequestUserIdentifier";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Please Register";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _emailAddress;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Label label3;
	}
}