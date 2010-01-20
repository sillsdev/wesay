using WeSay.ConfigTool.Properties;
using Resources=WeSay.ConfigTool.Properties.Resources;

namespace WeSay.ConfigTool
{
	partial class ConfigurationWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationWindow));
			this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._chooseProjectLocationDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.toolStrip2 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton9 = new System.Windows.Forms.ToolStripButton();
			this.openProjectInWeSayToolStripMenuItem = new System.Windows.Forms.ToolStripButton();
			this._versionToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this._saveACopyForFLEx54ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripHelpButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip2.SuspendLayout();
			this.SuspendLayout();
			//
			// projectToolStripMenuItem
			//
			this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
			this.projectToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.projectToolStripMenuItem.Text = "&Project";
			//
			// toolStripSeparator1
			//
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(202, 6);
			//
			// newProjectToolStripMenuItem
			//
			this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
			this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.newProjectToolStripMenuItem.Text = "&New Project...";
			this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.OnCreateProject);
			//
			// openProjectToolStripMenuItem
			//
			this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
			this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.openProjectToolStripMenuItem.Text = "&Open Project...";
			this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.OnChooseProject);
			//
			// toolStripMenuItem2
			//
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(202, 6);
			//
			// exitToolStripMenuItem
			//
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnExit_Click);
			//
			// helpToolStripMenuItem
			//
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			//
			// toolStrip2
			//
			this.toolStrip2.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripButton8,
			this.toolStripButton9,
			this.openProjectInWeSayToolStripMenuItem,
			this._versionToolStripLabel,
			this.toolStripDropDownButton1,
			this.toolStripHelpButton});
			this.toolStrip2.Location = new System.Drawing.Point(0, 0);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip2.Size = new System.Drawing.Size(781, 25);
			this.toolStrip2.TabIndex = 2;
			this.toolStrip2.Text = "toolStrip2";
			//
			// toolStripButton8
			//
			this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton8.Image = global::WeSay.ConfigTool.Properties.Resources.NewProject;
			this.toolStripButton8.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton8.Margin = new System.Windows.Forms.Padding(10, 1, 15, 2);
			this.toolStripButton8.Name = "toolStripButton8";
			this.toolStripButton8.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton8.ToolTipText = "Create new WeSay Project...";
			this.toolStripButton8.Click += new System.EventHandler(this.OnCreateProject);
			//
			// toolStripButton9
			//
			this.toolStripButton9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton9.Image = global::WeSay.ConfigTool.Properties.Resources.openProject;
			this.toolStripButton9.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton9.Margin = new System.Windows.Forms.Padding(0, 1, 15, 2);
			this.toolStripButton9.Name = "toolStripButton9";
			this.toolStripButton9.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton9.ToolTipText = "Open another WeSay project...";
			this.toolStripButton9.Click += new System.EventHandler(this.OnChooseProject);
			//
			// openProjectInWeSayToolStripMenuItem
			//
			this.openProjectInWeSayToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.openProjectInWeSayToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.openProjectInWeSayToolStripMenuItem.Image = global::WeSay.ConfigTool.Properties.Resources.WeSayMenuSized;
			this.openProjectInWeSayToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openProjectInWeSayToolStripMenuItem.Name = "openProjectInWeSayToolStripMenuItem";
			this.openProjectInWeSayToolStripMenuItem.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.openProjectInWeSayToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
			this.openProjectInWeSayToolStripMenuItem.Text = "Open in WeSay";
			this.openProjectInWeSayToolStripMenuItem.ToolTipText = "Open this in WeSay";
			this.openProjectInWeSayToolStripMenuItem.Click += new System.EventHandler(this.OnOpenThisProjectInWeSay);
			//
			// _versionToolStripLabel
			//
			this._versionToolStripLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this._versionToolStripLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
			this._versionToolStripLabel.Name = "_versionToolStripLabel";
			this._versionToolStripLabel.Size = new System.Drawing.Size(42, 22);
			this._versionToolStripLabel.Text = "version";
			//
			// toolStripDropDownButton1
			//
			this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this._saveACopyForFLEx54ToolStripMenuItem});
			this.toolStripDropDownButton1.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(72, 22);
			this.toolStripDropDownButton1.Text = "FieldWorks";
			//
			// _saveACopyForFLEx54ToolStripMenuItem
			//
			this._saveACopyForFLEx54ToolStripMenuItem.Name = "_saveACopyForFLEx54ToolStripMenuItem";
			this._saveACopyForFLEx54ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this._saveACopyForFLEx54ToolStripMenuItem.Text = "Save a copy for FLEx 5.4...";
			this._saveACopyForFLEx54ToolStripMenuItem.Click += new System.EventHandler(this.OnSaveACopyForFLEx5Pt4);
			//
			// toolStripHelpButton
			//
			this.toolStripHelpButton.AutoToolTip = false;
			this.toolStripHelpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripHelpButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.helpToolStripMenuItem1,
			this.aboutToolStripMenuItem1});
			this.toolStripHelpButton.Image = global::WeSay.ConfigTool.Properties.Resources.about;
			this.toolStripHelpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripHelpButton.Name = "toolStripHelpButton";
			this.toolStripHelpButton.Size = new System.Drawing.Size(29, 22);
			//
			// helpToolStripMenuItem1
			//
			this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
			this.helpToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
			this.helpToolStripMenuItem1.Text = "&Help";
			this.helpToolStripMenuItem1.Click += new System.EventHandler(this.OnHelpToolStrip_Click);
			//
			// aboutToolStripMenuItem1
			//
			this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
			this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
			this.aboutToolStripMenuItem1.Text = "&About WeSay";
			this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.OnAboutToolStrip_Click);
			//
			// ConfigurationWindow
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(781, 522);
			this.Controls.Add(this.toolStrip2);
			this.Icon = global::WeSay.ConfigTool.Properties.Resources.WeSaySetupApplicationIcon;
			this.Name = "ConfigurationWindow";
			this.Text = "WeSay Configuration Tool";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AdminWindow_FormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdminWindow_FormClosing);
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
		private System.Windows.Forms.FolderBrowserDialog _chooseProjectLocationDialog;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStrip toolStrip2;
		private System.Windows.Forms.ToolStripButton toolStripButton8;
		private System.Windows.Forms.ToolStripButton toolStripButton9;
		private System.Windows.Forms.ToolStripButton openProjectInWeSayToolStripMenuItem;
		private System.Windows.Forms.ToolStripLabel _versionToolStripLabel;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
		private System.Windows.Forms.ToolStripMenuItem _saveACopyForFLEx54ToolStripMenuItem;
		private System.Windows.Forms.ToolStripDropDownButton toolStripHelpButton;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
	}
}
