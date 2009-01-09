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
			this.SuspendLayout();
			//
			// _showExampleField
			//
			this._showExampleField.AutoSize = true;
			this._showExampleField.Location = new System.Drawing.Point(17, 239);
			this._showExampleField.Name = "_showExampleField";
			this._showExampleField.Size = new System.Drawing.Size(319, 17);
			this._showExampleField.TabIndex = 23;
			this._showExampleField.Text = "Also show an editable translation field underneath the example";
			this._showExampleField.UseVisualStyleBackColor = true;
			this._showExampleField.CheckedChanged += new System.EventHandler(this.OnShowExampleField_CheckedChanged);
			//
			// MissingInfoTaskConfigControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._showExampleField);
			this.Name = "MissingInfoTaskConfigControl";
			this.Size = new System.Drawing.Size(488, 289);
			this.Controls.SetChildIndex(this._showExampleField, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _showExampleField;
	}
}
