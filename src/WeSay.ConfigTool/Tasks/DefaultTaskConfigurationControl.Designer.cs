namespace WeSay.ConfigTool.Tasks
{
	partial class DefaultTaskConfigurationControl
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
			this.label2 = new System.Windows.Forms.Label();
			this._description = new System.Windows.Forms.TextBox();
			this._setupLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(14, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 13);
			this.label2.TabIndex = 21;
			this.label2.Text = "About this task: ";
			//
			// _description
			//
			this._description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._description.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._description.Location = new System.Drawing.Point(17, 41);
			this._description.Multiline = true;
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(358, 94);
			this._description.TabIndex = 22;
			//
			// _setupLabel
			//
			this._setupLabel.AutoSize = true;
			this._setupLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._setupLabel.Location = new System.Drawing.Point(14, 150);
			this._setupLabel.Name = "_setupLabel";
			this._setupLabel.Size = new System.Drawing.Size(48, 13);
			this._setupLabel.TabIndex = 21;
			this._setupLabel.Text = "Setup: ";
			this._setupLabel.Visible = false;
			//
			// DefaultTaskConfigurationControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._description);
			this.Controls.Add(this._setupLabel);
			this.Controls.Add(this.label2);
			this.Name = "DefaultTaskConfigurationControl";
			this.Size = new System.Drawing.Size(410, 180);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _description;
		protected System.Windows.Forms.Label _setupLabel;
	}
}
