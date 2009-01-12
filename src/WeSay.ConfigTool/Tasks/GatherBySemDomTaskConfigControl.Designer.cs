namespace WeSay.ConfigTool.Tasks
{
	partial class GatherBySemDomTaskConfigControl
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
			this._showMeaningField = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			//
			// _showMeaningField
			//
			this._showMeaningField.AutoSize = true;
			this._showMeaningField.Location = new System.Drawing.Point(17, 239);
			this._showMeaningField.Name = "_showMeaningField";
			this._showMeaningField.Size = new System.Drawing.Size(308, 17);
			this._showMeaningField.TabIndex = 23;
			this._showMeaningField.Text = "Allow a short definition to be collected along with each word";
			this._showMeaningField.UseVisualStyleBackColor = true;
			this._showMeaningField.CheckedChanged += new System.EventHandler(this.OnShowMeaning_CheckedChanged);
			//
			// GatherBySemDomTaskConfigControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._showMeaningField);
			this.Name = "GatherBySemDomTaskConfigControl";
			this.Size = new System.Drawing.Size(488, 289);
			this.Controls.SetChildIndex(this._showMeaningField, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _showMeaningField;
	}
}
