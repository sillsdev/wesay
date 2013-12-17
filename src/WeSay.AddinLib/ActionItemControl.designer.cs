namespace WeSay.AddinLib
{
	partial class ActionItemControl
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
			this._description = new System.Windows.Forms.TextBox();
			this._actionName = new System.Windows.Forms.Label();
			this._setupButton = new System.Windows.Forms.LinkLabel();
			this._launchButton = new System.Windows.Forms.Button();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._toggleShowInWeSay = new System.Windows.Forms.LinkLabel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			//
			// _description
			//
			this._description.BackColor = System.Drawing.SystemColors.Window;
			this._description.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._description.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._description.ForeColor = System.Drawing.SystemColors.WindowText;
			this._description.Location = new System.Drawing.Point(3, 23);
			this._description.Multiline = true;
			this._description.Name = "_description";
			this._description.ReadOnly = true;
			this._description.Size = new System.Drawing.Size(247, 42);
			this._description.TabIndex = 1;
			this._description.TabStop = false;
			this._description.Text = "blah blah blah";
			//
			// _actionName
			//
			this._actionName.AutoSize = true;
			this._actionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._actionName.Location = new System.Drawing.Point(3, 0);
			this._actionName.MaximumSize = new System.Drawing.Size(247, 0);
			this._actionName.Name = "_actionName";
			this._actionName.Size = new System.Drawing.Size(111, 20);
			this._actionName.TabIndex = 3;
			this._actionName.Text = "Action Name";
			//
			// _setupButton
			//
			this._setupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._setupButton.AutoSize = true;
			this._setupButton.LinkColor = System.Drawing.Color.Navy;
			this._setupButton.Location = new System.Drawing.Point(393, 4);
			this._setupButton.Name = "_setupButton";
			this._setupButton.Size = new System.Drawing.Size(47, 13);
			this._setupButton.TabIndex = 4;
			this._setupButton.TabStop = true;
			this._setupButton.Text = "Set up...";
			this._setupButton.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnSetupClicked);
			//
			// _launchButton
			//
			this._launchButton.Image = global::WeSay.AddinLib.Properties.Resources.construction;
			this._launchButton.Location = new System.Drawing.Point(3, 4);
			this._launchButton.Name = "_launchButton";
			this._launchButton.Size = new System.Drawing.Size(75, 65);
			this._launchButton.TabIndex = 0;
			this._launchButton.UseVisualStyleBackColor = true;
			this._launchButton.Click += new System.EventHandler(this._launchButton_Click);
			//
			// _toggleShowInWeSay
			//
			this._toggleShowInWeSay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._toggleShowInWeSay.AutoSize = true;
			this._toggleShowInWeSay.LinkColor = System.Drawing.Color.Navy;
			this._toggleShowInWeSay.Location = new System.Drawing.Point(393, 27);
			this._toggleShowInWeSay.Name = "_toggleShowInWeSay";
			this._toggleShowInWeSay.Size = new System.Drawing.Size(87, 13);
			this._toggleShowInWeSay.TabIndex = 4;
			this._toggleShowInWeSay.TabStop = true;
			this._toggleShowInWeSay.Text = "Visible In WeSay";
			this._toggleShowInWeSay.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._toggleShowInWeSay_LinkClicked);
			//
			// flowLayoutPanel1
			//
			this.flowLayoutPanel1.Controls.Add(this._actionName);
			this.flowLayoutPanel1.Controls.Add(this._description);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(84, 4);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(303, 65);
			this.flowLayoutPanel1.TabIndex = 5;
			this.flowLayoutPanel1.WrapContents = false;
			//
			// ActionItemControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this._toggleShowInWeSay);
			this.Controls.Add(this._setupButton);
			this.Controls.Add(this._launchButton);
			this.Name = "ActionItemControl";
			this.Size = new System.Drawing.Size(480, 77);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _description;
		private System.Windows.Forms.Button _launchButton;
		private System.Windows.Forms.Label _actionName;
		private System.Windows.Forms.LinkLabel _setupButton;
		private System.Windows.Forms.ToolTip _toolTip;
		private System.Windows.Forms.LinkLabel _toggleShowInWeSay;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
	}
}
