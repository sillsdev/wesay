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
			this._matchWhenEmptyLabel = new System.Windows.Forms.Label();
			this._requiredToBeFilledIn = new WeSay.ConfigTool.Tasks.WritingSystemFilterControl();
			this._matchWhenEmpty = new WeSay.ConfigTool.Tasks.WritingSystemFilterControl();
			this._requiredToBeFilledInLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _showExampleField
			//
			this._showExampleField.AutoSize = true;
			this._showExampleField.Location = new System.Drawing.Point(17, 269);
			this._showExampleField.Name = "_showExampleField";
			this._showExampleField.Size = new System.Drawing.Size(319, 17);
			this._showExampleField.TabIndex = 23;
			this._showExampleField.Text = "Also show an editable translation field underneath the example";
			this._showExampleField.UseVisualStyleBackColor = true;
			this._showExampleField.CheckedChanged += new System.EventHandler(this.OnShowExampleField_CheckedChanged);
			//
			// _matchWhenEmptyLabel
			//
			this._matchWhenEmptyLabel.AutoSize = true;
			this._matchWhenEmptyLabel.Location = new System.Drawing.Point(14, 177);
			this._matchWhenEmptyLabel.MaximumSize = new System.Drawing.Size(200, 0);
			this._matchWhenEmptyLabel.Name = "_matchWhenEmptyLabel";
			this._matchWhenEmptyLabel.Size = new System.Drawing.Size(193, 26);
			this._matchWhenEmptyLabel.TabIndex = 28;
			this._matchWhenEmptyLabel.Text = "Select items where any of the following writing systems are empty:";
			//
			// _requiredToBeFilledIn
			//
			this._requiredToBeFilledIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._requiredToBeFilledIn.BackColor = System.Drawing.SystemColors.Control;
			this._requiredToBeFilledIn.Location = new System.Drawing.Point(205, 223);
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
			this._matchWhenEmpty.Location = new System.Drawing.Point(205, 175);
			this._matchWhenEmpty.MinimumSize = new System.Drawing.Size(100, 0);
			this._matchWhenEmpty.Name = "_matchWhenEmpty";
			this._matchWhenEmpty.Size = new System.Drawing.Size(121, 28);
			this._matchWhenEmpty.TabIndex = 30;
			//
			// _requiredToBeFilledInLabel
			//
			this._requiredToBeFilledInLabel.AutoSize = true;
			this._requiredToBeFilledInLabel.Location = new System.Drawing.Point(14, 223);
			this._requiredToBeFilledInLabel.MaximumSize = new System.Drawing.Size(200, 0);
			this._requiredToBeFilledInLabel.Name = "_requiredToBeFilledInLabel";
			this._requiredToBeFilledInLabel.Size = new System.Drawing.Size(193, 26);
			this._requiredToBeFilledInLabel.TabIndex = 28;
			this._requiredToBeFilledInLabel.Text = "And where at least one of the following writing systems is already filled in:\r\n";
			//
			// MissingInfoTaskConfigControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._matchWhenEmpty);
			this.Controls.Add(this._requiredToBeFilledIn);
			this.Controls.Add(this._requiredToBeFilledInLabel);
			this.Controls.Add(this._matchWhenEmptyLabel);
			this.Controls.Add(this._showExampleField);
			this.Name = "MissingInfoTaskConfigControl";
			this.Size = new System.Drawing.Size(369, 332);
			this.Load += new System.EventHandler(this.MissingInfoTaskConfigControl_Load);
			this.Controls.SetChildIndex(this._setupLabel, 0);
			this.Controls.SetChildIndex(this._showExampleField, 0);
			this.Controls.SetChildIndex(this._matchWhenEmptyLabel, 0);
			this.Controls.SetChildIndex(this._requiredToBeFilledInLabel, 0);
			this.Controls.SetChildIndex(this._requiredToBeFilledIn, 0);
			this.Controls.SetChildIndex(this._matchWhenEmpty, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _showExampleField;
		private System.Windows.Forms.Label _matchWhenEmptyLabel;
		private WritingSystemFilterControl _requiredToBeFilledIn;
		private WritingSystemFilterControl _matchWhenEmpty;
		private System.Windows.Forms.Label _requiredToBeFilledInLabel;
	}
}
