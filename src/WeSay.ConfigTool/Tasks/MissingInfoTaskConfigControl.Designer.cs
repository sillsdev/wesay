namespace WeSay.ConfigTool.Tasks
{
	partial class MissingInfoTaskConfigControl
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
			this._showExampleField = new System.Windows.Forms.CheckBox();
			this._requiredToBeFilledIn = new WeSay.ConfigTool.Tasks.WritingSystemFilterControl();
			this._matchWhenEmpty = new WeSay.ConfigTool.Tasks.WritingSystemFilterControl();
			this._matchWhenEmptyLabel = new System.Windows.Forms.TextBox();
			this._requiredToBeFilledInLabel = new System.Windows.Forms.TextBox();
			this._checkboxLabel = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			//
			// _showExampleField
			//
			this._showExampleField.AutoSize = true;
			this._showExampleField.Location = new System.Drawing.Point(17, 269);
			this._showExampleField.Name = "_showExampleField";
			this._showExampleField.Size = new System.Drawing.Size(15, 14);
			this._showExampleField.TabIndex = 23;
			this._showExampleField.UseVisualStyleBackColor = true;
			this._showExampleField.CheckedChanged += new System.EventHandler(this.OnShowExampleField_CheckedChanged);
			//
			// _requiredToBeFilledIn
			//
			this._requiredToBeFilledIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._requiredToBeFilledIn.BackColor = System.Drawing.SystemColors.Control;
			this._requiredToBeFilledIn.Location = new System.Drawing.Point(234, 223);
			this._requiredToBeFilledIn.MinimumSize = new System.Drawing.Size(100, 0);
			this._requiredToBeFilledIn.Name = "_requiredToBeFilledIn";
			this._requiredToBeFilledIn.Size = new System.Drawing.Size(121, 26);
			this._requiredToBeFilledIn.TabIndex = 29;
			//
			// _matchWhenEmpty
			//
			this._matchWhenEmpty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._matchWhenEmpty.BackColor = System.Drawing.SystemColors.Control;
			this._matchWhenEmpty.Location = new System.Drawing.Point(234, 175);
			this._matchWhenEmpty.MinimumSize = new System.Drawing.Size(100, 0);
			this._matchWhenEmpty.Name = "_matchWhenEmpty";
			this._matchWhenEmpty.Size = new System.Drawing.Size(121, 28);
			this._matchWhenEmpty.TabIndex = 30;
			//
			// _matchWhenEmptyLabel
			//
			this._matchWhenEmptyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._matchWhenEmptyLabel.BackColor = System.Drawing.SystemColors.Control;
			this._matchWhenEmptyLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._matchWhenEmptyLabel.Location = new System.Drawing.Point(17, 175);
			this._matchWhenEmptyLabel.Multiline = true;
			this._matchWhenEmptyLabel.Name = "_matchWhenEmptyLabel";
			this._matchWhenEmptyLabel.Size = new System.Drawing.Size(211, 42);
			this._matchWhenEmptyLabel.TabIndex = 31;
			this._matchWhenEmptyLabel.Text = "Select items where any of the following writing systems are empty:";
			//
			// _requiredToBeFilledInLabel
			//
			this._requiredToBeFilledInLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._requiredToBeFilledInLabel.BackColor = System.Drawing.SystemColors.Control;
			this._requiredToBeFilledInLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._requiredToBeFilledInLabel.Location = new System.Drawing.Point(17, 223);
			this._requiredToBeFilledInLabel.Multiline = true;
			this._requiredToBeFilledInLabel.Name = "_requiredToBeFilledInLabel";
			this._requiredToBeFilledInLabel.Size = new System.Drawing.Size(211, 40);
			this._requiredToBeFilledInLabel.TabIndex = 32;
			this._requiredToBeFilledInLabel.Text = "And where at least one of the following writing systems is already filled in:";
			//
			// _checkboxLabel
			//
			this._checkboxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._checkboxLabel.BackColor = System.Drawing.SystemColors.Control;
			this._checkboxLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._checkboxLabel.Location = new System.Drawing.Point(38, 269);
			this._checkboxLabel.Multiline = true;
			this._checkboxLabel.Name = "_checkboxLabel";
			this._checkboxLabel.Size = new System.Drawing.Size(317, 40);
			this._checkboxLabel.TabIndex = 33;
			this._checkboxLabel.Text = "Also show an editable translation field underneath the example";
			//
			// MissingInfoTaskConfigControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._checkboxLabel);
			this.Controls.Add(this._requiredToBeFilledInLabel);
			this.Controls.Add(this._matchWhenEmptyLabel);
			this.Controls.Add(this._matchWhenEmpty);
			this.Controls.Add(this._requiredToBeFilledIn);
			this.Controls.Add(this._showExampleField);
			this.Name = "MissingInfoTaskConfigControl";
			this.Size = new System.Drawing.Size(369, 332);
			this.Load += new System.EventHandler(this.MissingInfoTaskConfigControl_Load);
			this.Controls.SetChildIndex(this._setupLabel, 0);
			this.Controls.SetChildIndex(this._showExampleField, 0);
			this.Controls.SetChildIndex(this._requiredToBeFilledIn, 0);
			this.Controls.SetChildIndex(this._matchWhenEmpty, 0);
			this.Controls.SetChildIndex(this._matchWhenEmptyLabel, 0);
			this.Controls.SetChildIndex(this._requiredToBeFilledInLabel, 0);
			this.Controls.SetChildIndex(this._checkboxLabel, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _showExampleField;
		private WritingSystemFilterControl _requiredToBeFilledIn;
		private WritingSystemFilterControl _matchWhenEmpty;
		private System.Windows.Forms.TextBox _matchWhenEmptyLabel;
		private System.Windows.Forms.TextBox _requiredToBeFilledInLabel;
		private System.Windows.Forms.TextBox _checkboxLabel;
	}
}
