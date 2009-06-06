namespace WeSay.ConfigTool.NewProjectCreation
{
	partial class NewProjectInformationDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProjectInformationDialog));
			this.btnOK = new System.Windows.Forms.Button();
			this._whereLabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// btnOK
			//
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOK.Location = new System.Drawing.Point(296, 270);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(91, 29);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			//
			// _whereLabel
			//
			this._whereLabel.AutoSize = true;
			this._whereLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._whereLabel.Location = new System.Drawing.Point(13, 13);
			this._whereLabel.MaximumSize = new System.Drawing.Size(380, 0);
			this._whereLabel.Name = "_whereLabel";
			this._whereLabel.Size = new System.Drawing.Size(254, 42);
			this._whereLabel.TabIndex = 4;
			this._whereLabel.Text = "Your project has been created here:\r\n{0}";
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(13, 94);
			this.label3.MaximumSize = new System.Drawing.Size(380, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(352, 63);
			this.label3.TabIndex = 4;
			this.label3.Text = "The project is ready to use: just click on \"Open in WeSay\" button in the upper-ri" +
				"ght.  Later, you can come back here and make adjustments.";
			//
			// linkLabel1
			//
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkLabel1.LinkColor = System.Drawing.Color.Gray;
			this.linkLabel1.Location = new System.Drawing.Point(14, 55);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(224, 15);
			this.linkLabel1.TabIndex = 6;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "(click here if you want it somewhere else)";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(13, 175);
			this.label1.MaximumSize = new System.Drawing.Size(380, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(378, 63);
			this.label1.TabIndex = 4;
			this.label1.Text = "One important adjustment is to edit the \'v\' writing system, changing its ID to ma" +
				"tch the Ethnologue/ISO 693 code for this language.";
			//
			// NewProjectInformationDialog
			//
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(415, 311);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._whereLabel);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewProjectInformationDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "New Project Information";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label _whereLabel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label label1;
	}
}