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
			this._showExampleFieldBox = new System.Windows.Forms.CheckBox();
			this._requiredToBeFilledIn = new WeSay.ConfigTool.Tasks.WritingSystemFilterControl();
			this._matchWhenEmpty = new WeSay.ConfigTool.Tasks.WritingSystemFilterControl();
			this._matchWhenEmptyLabel = new System.Windows.Forms.TextBox();
			this._requiredToBeFilledInLabel = new System.Windows.Forms.TextBox();
			this._showExampleLabel = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			//
			// _showExampleFieldBox
			//
			this._showExampleFieldBox.AutoSize = true;
			this._showExampleFieldBox.Location = new System.Drawing.Point(17, 269);
			this._showExampleFieldBox.Name = "_showExampleFieldBox";
			this._showExampleFieldBox.Size = new System.Drawing.Size(15, 14);
			this._showExampleFieldBox.TabIndex = 23;
			this._showExampleFieldBox.UseVisualStyleBackColor = true;
			this._showExampleFieldBox.CheckedChanged += new System.EventHandler(this.OnShowExampleField_CheckedChanged);
			//
			// _requiredToBeFilledIn
			//
			this._requiredToBeFilledIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._requiredToBeFilledIn.BackColor = System.Drawing.SystemColors.Control;
			this._requiredToBeFilledIn.Location = new System.Drawing.Point(229, 223);
			this._requiredToBeFilledIn.MinimumSize = new System.Drawing.Size(100, 0);
			this._requiredToBeFilledIn.Name = "_requiredToBeFilledIn";
			this._requiredToBeFilledIn.Size = new System.Drawing.Size(169, 26);
			this._requiredToBeFilledIn.TabIndex = 29;
			//
			// _matchWhenEmpty
			//
			this._matchWhenEmpty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._matchWhenEmpty.BackColor = System.Drawing.SystemColors.Control;
			this._matchWhenEmpty.Location = new System.Drawing.Point(229, 175);
			this._matchWhenEmpty.MinimumSize = new System.Drawing.Size(100, 0);
			this._matchWhenEmpty.Name = "_matchWhenEmpty";
			this._matchWhenEmpty.Size = new System.Drawing.Size(169, 28);
			this._matchWhenEmpty.TabIndex = 30;
			//
			// _matchWhenEmptyLabel
			//
			this._matchWhenEmptyLabel.BackColor = System.Drawing.SystemColors.Control;
			this._matchWhenEmptyLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._matchWhenEmptyLabel.Location = new System.Drawing.Point(17, 175);
			this._matchWhenEmptyLabel.Multiline = true;
			this._matchWhenEmptyLabel.Name = "_matchWhenEmptyLabel";
			this._matchWhenEmptyLabel.ReadOnly = true;
			this._matchWhenEmptyLabel.Size = new System.Drawing.Size(206, 42);
			this._matchWhenEmptyLabel.TabIndex = 31;
			this._matchWhenEmptyLabel.TabStop = false;
			this._matchWhenEmptyLabel.Text = "Add to only those words where the field is missing the following input systems:";
			//
			// _requiredToBeFilledInLabel
			//
			this._requiredToBeFilledInLabel.BackColor = System.Drawing.SystemColors.Control;
			this._requiredToBeFilledInLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._requiredToBeFilledInLabel.Location = new System.Drawing.Point(17, 223);
			this._requiredToBeFilledInLabel.Multiline = true;
			this._requiredToBeFilledInLabel.Name = "_requiredToBeFilledInLabel";
			this._requiredToBeFilledInLabel.ReadOnly = true;
			this._requiredToBeFilledInLabel.Size = new System.Drawing.Size(206, 40);
			this._requiredToBeFilledInLabel.TabIndex = 32;
			this._requiredToBeFilledInLabel.TabStop = false;
			this._requiredToBeFilledInLabel.Text = "And where the field already has at least one of the following input systems filled in:";
			//
			// _showExampleLabel
			//
			this._showExampleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._showExampleLabel.BackColor = System.Drawing.SystemColors.Control;
			this._showExampleLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._showExampleLabel.Location = new System.Drawing.Point(38, 269);
			this._showExampleLabel.Multiline = true;
			this._showExampleLabel.Name = "_showExampleLabel";
			this._showExampleLabel.ReadOnly = true;
			this._showExampleLabel.Size = new System.Drawing.Size(365, 40);
			this._showExampleLabel.TabIndex = 33;
			this._showExampleLabel.TabStop = false;
			this._showExampleLabel.Text = "Also show an editable translation field underneath the example";
			//
			// MissingInfoTaskConfigControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._matchWhenEmpty);
			this.Controls.Add(this._requiredToBeFilledIn);
			this.Controls.Add(this._showExampleLabel);
			this.Controls.Add(this._requiredToBeFilledInLabel);
			this.Controls.Add(this._matchWhenEmptyLabel);
			this.Controls.Add(this._showExampleFieldBox);
			this.Name = "MissingInfoTaskConfigControl";
			this.Size = new System.Drawing.Size(417, 332);
			this.Load += new System.EventHandler(this.MissingInfoTaskConfigControl_Load);
			this.BackColorChanged += new System.EventHandler(this.MissingInfoTaskConfigControl_BackColorChanged);
			this.Controls.SetChildIndex(this._setupLabel, 0);
			this.Controls.SetChildIndex(this._showExampleFieldBox, 0);
			this.Controls.SetChildIndex(this._matchWhenEmptyLabel, 0);
			this.Controls.SetChildIndex(this._requiredToBeFilledInLabel, 0);
			this.Controls.SetChildIndex(this._showExampleLabel, 0);
			this.Controls.SetChildIndex(this._requiredToBeFilledIn, 0);
			this.Controls.SetChildIndex(this._matchWhenEmpty, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _showExampleFieldBox;
		private WritingSystemFilterControl _requiredToBeFilledIn;
		private WritingSystemFilterControl _matchWhenEmpty;
		private System.Windows.Forms.TextBox _matchWhenEmptyLabel;
		private System.Windows.Forms.TextBox _requiredToBeFilledInLabel;
		private System.Windows.Forms.TextBox _showExampleLabel;
	}
}
