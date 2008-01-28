namespace WeSay.AddinLib
{
	partial class ActionSetupDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionSetupDialog));
			this._propertyGrid = new System.Windows.Forms.PropertyGrid();
			this._btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// _propertyGrid
			//
			this._propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._propertyGrid.Location = new System.Drawing.Point(12, 12);
			this._propertyGrid.Name = "_propertyGrid";
			this._propertyGrid.Size = new System.Drawing.Size(268, 209);
			this._propertyGrid.TabIndex = 0;
			this._propertyGrid.ToolbarVisible = false;
			//
			// _btnOK
			//
			this._btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnOK.Location = new System.Drawing.Point(205, 231);
			this._btnOK.Name = "_btnOK";
			this._btnOK.Size = new System.Drawing.Size(75, 23);
			this._btnOK.TabIndex = 1;
			this._btnOK.Text = "&OK";
			this._btnOK.UseVisualStyleBackColor = true;
			this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
			//
			// ActionSetupDialog
			//
			this.AcceptButton = this._btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this._btnOK);
			this.Controls.Add(this._propertyGrid);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ActionSetupDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Setup Action...";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid _propertyGrid;
		private System.Windows.Forms.Button _btnOK;
	}
}