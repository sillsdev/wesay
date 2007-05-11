namespace WeSay.AddinLib
{
	partial class ActionItemControl
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
			this._description = new System.Windows.Forms.TextBox();
			this._launchButton = new System.Windows.Forms.Button();
			this._actionName = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _description
			//
			this._description.BackColor = System.Drawing.SystemColors.Window;
			this._description.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._description.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._description.ForeColor = System.Drawing.SystemColors.WindowText;
			this._description.Location = new System.Drawing.Point(84, 27);
			this._description.Multiline = true;
			this._description.Name = "_description";
			this._description.ReadOnly = true;
			this._description.Size = new System.Drawing.Size(302, 42);
			this._description.TabIndex = 1;
			this._description.Text = "blah blah blah";
			//
			// _launchButton
			//
			this._launchButton.Image = global::WeSay.AddinLib.Properties.Resources.construction;
			this._launchButton.Location = new System.Drawing.Point(3, 4);
			this._launchButton.Name = "_launchButton";
			this._launchButton.Size = new System.Drawing.Size(75, 65);
			this._launchButton.TabIndex = 2;
			this._launchButton.UseVisualStyleBackColor = true;
			this._launchButton.Click += new System.EventHandler(this._launchButton_Click);
			//
			// _actionName
			//
			this._actionName.AutoSize = true;
			this._actionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._actionName.Location = new System.Drawing.Point(80, 4);
			this._actionName.Name = "_actionName";
			this._actionName.Size = new System.Drawing.Size(111, 20);
			this._actionName.TabIndex = 3;
			this._actionName.Text = "Action Name";
			//
			// ActionItemControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._actionName);
			this.Controls.Add(this._launchButton);
			this.Controls.Add(this._description);
			this.Name = "ActionItemControl";
			this.Size = new System.Drawing.Size(402, 77);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _description;
		private System.Windows.Forms.Button _launchButton;
		private System.Windows.Forms.Label _actionName;
	}
}
