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
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this._whereLabel = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this._changeVMessage = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			//
			// btnOK
			//
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOK.Location = new System.Drawing.Point(299, 241);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(91, 29);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			//
			// linkLabel1
			//
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkLabel1.LinkColor = System.Drawing.Color.Gray;
			this.linkLabel1.Location = new System.Drawing.Point(14, 68);
			this.linkLabel1.MinimumSize = new System.Drawing.Size(360, 0);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(360, 15);
			this.linkLabel1.TabIndex = 6;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "(click here if you want it somewhere else)";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			//
			// _whereLabel
			//
			this._whereLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._whereLabel.BackColor = System.Drawing.SystemColors.Control;
			this._whereLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._whereLabel.Location = new System.Drawing.Point(17, 12);
			this._whereLabel.Multiline = true;
			this._whereLabel.Name = "_whereLabel";
			this._whereLabel.ReadOnly = true;
			this._whereLabel.Size = new System.Drawing.Size(373, 53);
			this._whereLabel.TabIndex = 7;
			this._whereLabel.TabStop = false;
			this._whereLabel.Text = "Your project has been created here:\r\n{0}";
			//
			// textBox2
			//
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox2.BackColor = System.Drawing.SystemColors.Control;
			this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox2.Location = new System.Drawing.Point(17, 104);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(373, 70);
			this.textBox2.TabIndex = 8;
			this.textBox2.TabStop = false;
			this.textBox2.Text = "The project is ready to use: just click on \"Open in WeSay\" button in the upper-ri" +
				"ght.  Later, you can come back here and make adjustments.";
			//
			// _changeVMessage
			//
			this._changeVMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._changeVMessage.BackColor = System.Drawing.SystemColors.Control;
			this._changeVMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._changeVMessage.Location = new System.Drawing.Point(17, 180);
			this._changeVMessage.Multiline = true;
			this._changeVMessage.Name = "_changeVMessage";
			this._changeVMessage.ReadOnly = true;
			this._changeVMessage.Size = new System.Drawing.Size(373, 55);
			this._changeVMessage.TabIndex = 9;
			this._changeVMessage.TabStop = false;
			this._changeVMessage.Text = "One important adjustment is to edit the \'v\' writing system, changing its ID to ma" +
				"tch the Ethnologue/ISO 693 code for this language.";
			//
			// NewProjectInformationDialog
			//
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(402, 282);
			this.Controls.Add(this._changeVMessage);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this._whereLabel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.linkLabel1);
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
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.TextBox _whereLabel;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox _changeVMessage;
	}
}