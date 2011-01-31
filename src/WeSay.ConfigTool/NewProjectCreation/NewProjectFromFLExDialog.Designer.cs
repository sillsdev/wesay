namespace WeSay.ConfigTool.NewProjectCreation
{
	partial class NewProjectFromFLExDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		//private readonly System.ComponentModel.IContainer components = null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProjectFromFLExDialog));
			this._launchWebPage = new System.Windows.Forms.LinkLabel();
			this._browseForLiftPathButton = new System.Windows.Forms.Button();
			this.whereIsLiftLabel = new System.Windows.Forms.Label();
			this._liftPathTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			//
			// label1
			//
			this.label1.Location = new System.Drawing.Point(26, 99);
			//
			// btnOK
			//
			this.btnOK.Location = new System.Drawing.Point(195, 231);
			//
			// btnCancel
			//
			this.btnCancel.Location = new System.Drawing.Point(292, 231);
			//
			// _textProjectName
			//
			this._textProjectName.Location = new System.Drawing.Point(23, 118);
			this._textProjectName.TabIndex = 1;
			//
			// _pathLabel
			//
			this._pathLabel.Location = new System.Drawing.Point(26, 141);
			//
			// _launchWebPage
			//
			this._launchWebPage.LinkArea = new System.Windows.Forms.LinkArea(139, 154);
			this._launchWebPage.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
			this._launchWebPage.Location = new System.Drawing.Point(26, 165);
			this._launchWebPage.MaximumSize = new System.Drawing.Size(300, 0);
			this._launchWebPage.Name = "_launchWebPage";
			this._launchWebPage.Size = new System.Drawing.Size(300, 63);
			this._launchWebPage.TabIndex = 7;
			this._launchWebPage.TabStop = true;
			this._launchWebPage.Text = "Don\'t do this if you already have a corresponding WeSay project.  To learn how to add changes from FLEx to an existing WeSay project, read this web page.";
			this._launchWebPage.UseCompatibleTextRendering = true;
			this._launchWebPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._launchWebPage_LinkClicked);
			//
			// _browseForLiftPathButton
			//
			this._browseForLiftPathButton.Location = new System.Drawing.Point(353, 49);
			this._browseForLiftPathButton.Name = "_browseForLiftPathButton";
			this._browseForLiftPathButton.Size = new System.Drawing.Size(27, 23);
			this._browseForLiftPathButton.TabIndex = 0;
			this._browseForLiftPathButton.Text = "...";
			this._browseForLiftPathButton.UseVisualStyleBackColor = true;
			this._browseForLiftPathButton.Click += new System.EventHandler(this._browseForLiftPathButton_Click);
			//
			// whereIsLiftLabel
			//
			this.whereIsLiftLabel.AutoSize = true;
			this.whereIsLiftLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.whereIsLiftLabel.Location = new System.Drawing.Point(23, 21);
			this.whereIsLiftLabel.Name = "whereIsLiftLabel";
			this.whereIsLiftLabel.Size = new System.Drawing.Size(280, 15);
			this.whereIsLiftLabel.TabIndex = 9;
			this.whereIsLiftLabel.Text = "Where is the LIFT file exported from FLEx?";
			//
			// _liftPathTextBox
			//
			this._liftPathTextBox.BackColor = System.Drawing.SystemColors.ButtonFace;
			this._liftPathTextBox.Location = new System.Drawing.Point(27, 49);
			this._liftPathTextBox.Name = "_liftPathTextBox";
			this._liftPathTextBox.Size = new System.Drawing.Size(318, 20);
			this._liftPathTextBox.TabIndex = 0;
			this._liftPathTextBox.TextChanged += new System.EventHandler(this._liftPathTextBox_TextChanged);
			//
			// NewProjectFromFLExDialog
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(402, 270);
			this.Controls.Add(this._liftPathTextBox);
			this.Controls.Add(this.whereIsLiftLabel);
			this.Controls.Add(this._browseForLiftPathButton);
			this.Controls.Add(this._launchWebPage);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "NewProjectFromFLExDialog";
			this.Text = "Create Project From FLEx LIFT Export";
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.btnOK, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			this.Controls.SetChildIndex(this._textProjectName, 0);
			this.Controls.SetChildIndex(this._pathLabel, 0);
			this.Controls.SetChildIndex(this._launchWebPage, 0);
			this.Controls.SetChildIndex(this._browseForLiftPathButton, 0);
			this.Controls.SetChildIndex(this.whereIsLiftLabel, 0);
			this.Controls.SetChildIndex(this._liftPathTextBox, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel _launchWebPage;
		private System.Windows.Forms.Button _browseForLiftPathButton;
		private System.Windows.Forms.Label whereIsLiftLabel;
		private System.Windows.Forms.TextBox _liftPathTextBox;
	}
}