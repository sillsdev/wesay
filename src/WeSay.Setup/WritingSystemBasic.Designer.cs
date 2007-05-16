namespace WeSay.Setup
{
	partial class WritingSystemBasic
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
			this._writingSystemProperties = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			//
			// _fontProperties
			//
			this._writingSystemProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._writingSystemProperties.HelpVisible = false;
			this._writingSystemProperties.Location = new System.Drawing.Point(3, 3);
			this._writingSystemProperties.Name = "_writingSystemProperties";
			this._writingSystemProperties.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this._writingSystemProperties.Size = new System.Drawing.Size(130, 144);
			this._writingSystemProperties.TabIndex = 2;
			this._writingSystemProperties.ToolbarVisible = false;
			this._writingSystemProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.OnPropertyValueChanged);
			//
			// WritingSystemBasic
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._writingSystemProperties);
			this.Name = "WritingSystemBasic";
			this.Size = new System.Drawing.Size(211, 150);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid _writingSystemProperties;
	}
}
