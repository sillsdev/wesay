namespace WeSay.UI.Buttons
{
	partial class DeleteButton
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
			this._button = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			//
			// _button
			//
			this._button.AutoSize = true;
			this._button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._button.FlatAppearance.BorderSize = 0;
			this._button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this._button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._button.Image = global::WeSay.UI.Properties.Resources.DeleteIcon;
			this._button.Location = new System.Drawing.Point(0, 3);
			this._button.Name = "_button";
			this._button.Size = new System.Drawing.Size(22, 26);
			this._button.TabIndex = 0;
			this._button.UseVisualStyleBackColor = true;
			//
			// DeleteButton
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this._button);
			this.Name = "DeleteButton";
			this.Size = new System.Drawing.Size(25, 32);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _button;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
