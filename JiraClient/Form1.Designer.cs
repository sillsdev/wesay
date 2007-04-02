namespace JiraClient
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
			this.components = new System.ComponentModel.Container();
			this._submitButton = new System.Windows.Forms.Button();
			this._log = new System.Windows.Forms.TextBox();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this._newButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// _submitButton
			//
			this._submitButton.Location = new System.Drawing.Point(456, 12);
			this._submitButton.Name = "_submitButton";
			this._submitButton.Size = new System.Drawing.Size(75, 23);
			this._submitButton.TabIndex = 0;
			this._submitButton.Text = "Submit";
			this._submitButton.UseVisualStyleBackColor = true;
			this._submitButton.Click += new System.EventHandler(this.button1_Click);
			//
			// _log
			//
			this._log.Location = new System.Drawing.Point(12, 517);
			this._log.Multiline = true;
			this._log.Name = "_log";
			this._log.Size = new System.Drawing.Size(528, 55);
			this._log.TabIndex = 1;
			//
			// propertyGrid1
			//
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.Location = new System.Drawing.Point(12, 41);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(528, 456);
			this.propertyGrid1.TabIndex = 2;
			//
			// _newButton
			//
			this._newButton.Location = new System.Drawing.Point(239, 12);
			this._newButton.Name = "_newButton";
			this._newButton.Size = new System.Drawing.Size(75, 23);
			this._newButton.TabIndex = 3;
			this._newButton.Text = "New";
			this._newButton.UseVisualStyleBackColor = true;
			//
			// Form1
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(552, 584);
			this.Controls.Add(this._newButton);
			this.Controls.Add(this.propertyGrid1);
			this.Controls.Add(this._log);
			this.Controls.Add(this._submitButton);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _submitButton;
		private System.Windows.Forms.TextBox _log;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.Button _newButton;
	}
}
