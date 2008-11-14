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
			this.label1 = new System.Windows.Forms.Label();
			this._pathText = new System.Windows.Forms.TextBox();
			this._browseButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._environmentNotReadyLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 154);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Backup Media Location";
			//
			// _pathText
			//
			this._pathText.Location = new System.Drawing.Point(142, 151);
			this._pathText.Name = "_pathText";
			this._pathText.Size = new System.Drawing.Size(171, 20);
			this._pathText.TabIndex = 1;
			//
			// _browseButton
			//
			this._browseButton.Location = new System.Drawing.Point(319, 149);
			this._browseButton.Name = "_browseButton";
			this._browseButton.Size = new System.Drawing.Size(29, 23);
			this._browseButton.TabIndex = 2;
			this._browseButton.Text = "...";
			this._browseButton.UseVisualStyleBackColor = true;
			this._browseButton.Click += new System.EventHandler(this._browseButton_Click);
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 40);
			this.label2.MaximumSize = new System.Drawing.Size(500, 100);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(498, 78);
			this.label2.TabIndex = 3;
			this.label2.Text = resources.GetString("label2.Text");
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 11);
			this.label3.MaximumSize = new System.Drawing.Size(500, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(214, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "We recommend a two-part backup strategy:";
			//
			// label4
			//
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(16, 204);
			this.label4.MaximumSize = new System.Drawing.Size(500, 100);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(484, 65);
			this.label4.TabIndex = 3;
			this.label4.Text = resources.GetString("label4.Text");
			//
			// _environmentNotReadyLabel
			//
			this._environmentNotReadyLabel.AutoSize = true;
			this._environmentNotReadyLabel.ForeColor = System.Drawing.Color.Red;
			this._environmentNotReadyLabel.Location = new System.Drawing.Point(16, 288);
			this._environmentNotReadyLabel.MaximumSize = new System.Drawing.Size(500, 100);
			this._environmentNotReadyLabel.Name = "_environmentNotReadyLabel";
			this._environmentNotReadyLabel.Size = new System.Drawing.Size(217, 13);
			this._environmentNotReadyLabel.TabIndex = 3;
			this._environmentNotReadyLabel.Text = "This machine is not ready to use this feature.\r\n";
			//
			// BackupPlanControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.label3);
			this.Controls.Add(this._environmentNotReadyLabel);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._browseButton);
			this.Controls.Add(this._pathText);
			this.Controls.Add(this.label1);
			this.Name = "BackupPlanControl";
			this.Size = new System.Drawing.Size(614, 385);
			this.Load += new System.EventHandler(this.BackupPlanControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _pathText;
		private System.Windows.Forms.Button _browseButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label _environmentNotReadyLabel;

	}
}
