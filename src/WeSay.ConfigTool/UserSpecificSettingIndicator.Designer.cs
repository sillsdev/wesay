namespace WeSay.ConfigTool
{
	partial class UserSpecificSettingIndicator
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
			this.components = new System.ComponentModel.Container();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._imageButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// _imageButton
			//
			this._imageButton.FlatAppearance.BorderSize = 0;
			this._imageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._imageButton.Image = global::WeSay.ConfigTool.Properties.Resources.userSpecific;
			this._imageButton.Location = new System.Drawing.Point(3, 3);
			this._imageButton.Name = "_imageButton";
			this._imageButton.Size = new System.Drawing.Size(32, 33);
			this._imageButton.TabIndex = 0;
			this._imageButton.UseVisualStyleBackColor = true;
			this._imageButton.Click += new System.EventHandler(this._imageButton_Click);
			//
			// UserSpecificSettingIndicator
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._imageButton);
			this.Name = "UserSpecificSettingIndicator";
			this.Size = new System.Drawing.Size(37, 40);
			this.Load += new System.EventHandler(this.UserSpecificSettingIndicator_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _imageButton;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
