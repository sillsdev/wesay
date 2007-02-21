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
			this._fontProperties = new System.Windows.Forms.PropertyGrid();
			this.btnUseForVernacular = new System.Windows.Forms.LinkLabel();
			this.btnUseForAnalysis = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			//
			// _fontProperties
			//
			this._fontProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._fontProperties.HelpVisible = false;
			this._fontProperties.Location = new System.Drawing.Point(3, 3);
			this._fontProperties.Name = "_fontProperties";
			this._fontProperties.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this._fontProperties.Size = new System.Drawing.Size(130, 90);
			this._fontProperties.TabIndex = 2;
			this._fontProperties.ToolbarVisible = false;
			this._fontProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this._fontProperties_PropertyValueChanged);
			//
			// btnUseForVernacular
			//
			this.btnUseForVernacular.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnUseForVernacular.AutoSize = true;
			this.btnUseForVernacular.Location = new System.Drawing.Point(3, 114);
			this.btnUseForVernacular.Name = "btnUseForVernacular";
			this.btnUseForVernacular.Size = new System.Drawing.Size(140, 13);
			this.btnUseForVernacular.TabIndex = 4;
			this.btnUseForVernacular.TabStop = true;
			this.btnUseForVernacular.Text = "Use this for vernacular fields";
			this.btnUseForVernacular.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnUseForVernacular_LinkClicked);
			//
			// btnUseForAnalysis
			//
			this.btnUseForAnalysis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnUseForAnalysis.AutoSize = true;
			this.btnUseForAnalysis.Location = new System.Drawing.Point(3, 137);
			this.btnUseForAnalysis.Name = "btnUseForAnalysis";
			this.btnUseForAnalysis.Size = new System.Drawing.Size(195, 13);
			this.btnUseForAnalysis.TabIndex = 4;
			this.btnUseForAnalysis.TabStop = true;
			this.btnUseForAnalysis.Text = "Use this for analysis fields (e.g. glossing)";
			this.btnUseForAnalysis.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnUseForAnalysis_LinkClicked);
			//
			// WritingSystemBasic
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnUseForAnalysis);
			this.Controls.Add(this.btnUseForVernacular);
			this.Controls.Add(this._fontProperties);
			this.Name = "WritingSystemBasic";
			this.Size = new System.Drawing.Size(211, 150);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid _fontProperties;
		private System.Windows.Forms.LinkLabel btnUseForVernacular;
		private System.Windows.Forms.LinkLabel btnUseForAnalysis;
	}
}
