using Resources=WeSay.ConfigTool.Properties.Resources;

namespace WeSay.ConfigTool
{
	partial class BackupPlanControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackupPlanControl));
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.betterLabel2 = new WeSay.UI.BetterLabel();
			this._browseButton = new System.Windows.Forms.Button();
			this._pathText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.betterLabel1 = new WeSay.UI.BetterLabel();
			this.userSpecificSettingIndicator1 = new WeSay.ConfigTool.UserSpecificSettingIndicator();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			//
			// betterLabel2
			//
			this.betterLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.betterLabel2.BackColor = System.Drawing.SystemColors.Window;
			this.betterLabel2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.betterLabel2.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.betterLabel2.Location = new System.Drawing.Point(19, 248);
			this.betterLabel2.Multiline = true;
			this.betterLabel2.Name = "betterLabel2";
			this.betterLabel2.ReadOnly = true;
			this.betterLabel2.Size = new System.Drawing.Size(562, 143);
			this.betterLabel2.TabIndex = 10;
			this.betterLabel2.TabStop = false;
			this.betterLabel2.Text = resources.GetString("betterLabel2.Text");
			//
			// _browseButton
			//
			this._browseButton.Location = new System.Drawing.Point(306, 3);
			this._browseButton.Name = "_browseButton";
			this._browseButton.Size = new System.Drawing.Size(29, 23);
			this._browseButton.TabIndex = 2;
			this._browseButton.Text = "...";
			this._browseButton.UseVisualStyleBackColor = true;
			this._browseButton.Click += new System.EventHandler(this._browseButton_Click);
			//
			// _pathText
			//
			this._pathText.Location = new System.Drawing.Point(129, 3);
			this._pathText.Name = "_pathText";
			this._pathText.Size = new System.Drawing.Size(171, 20);
			this._pathText.TabIndex = 1;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Backup Media Location";
			//
			// flowLayoutPanel1
			//
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.Controls.Add(this.label1);
			this.flowLayoutPanel1.Controls.Add(this._pathText);
			this.flowLayoutPanel1.Controls.Add(this._browseButton);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(62, 192);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(519, 31);
			this.flowLayoutPanel1.TabIndex = 11;
			//
			// betterLabel1
			//
			this.betterLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.betterLabel1.BackColor = System.Drawing.SystemColors.Window;
			this.betterLabel1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.betterLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.betterLabel1.Location = new System.Drawing.Point(19, 0);
			this.betterLabel1.Multiline = true;
			this.betterLabel1.Name = "betterLabel1";
			this.betterLabel1.ReadOnly = true;
			this.betterLabel1.Size = new System.Drawing.Size(562, 168);
			this.betterLabel1.TabIndex = 9;
			this.betterLabel1.TabStop = false;
			this.betterLabel1.Text = resources.GetString("betterLabel1.Text");
			//
			// userSpecificSettingIndicator1
			//
			this.userSpecificSettingIndicator1.BackColor = System.Drawing.SystemColors.Window;
			this.userSpecificSettingIndicator1.Location = new System.Drawing.Point(19, 186);
			this.userSpecificSettingIndicator1.Name = "userSpecificSettingIndicator1";
			this.userSpecificSettingIndicator1.Size = new System.Drawing.Size(40, 47);
			this.userSpecificSettingIndicator1.TabIndex = 3;
			//
			// BackupPlanControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.betterLabel2);
			this.Controls.Add(this.betterLabel1);
			this.Controls.Add(this.userSpecificSettingIndicator1);
			this.Name = "BackupPlanControl";
			this.Size = new System.Drawing.Size(614, 385);
			this.Load += new System.EventHandler(this.BackupPlanControl_Load);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private WeSay.UI.BetterLabel betterLabel2;
		private System.Windows.Forms.Button _browseButton;
		private System.Windows.Forms.TextBox _pathText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private WeSay.UI.BetterLabel betterLabel1;
		private UserSpecificSettingIndicator userSpecificSettingIndicator1;

	}
}
