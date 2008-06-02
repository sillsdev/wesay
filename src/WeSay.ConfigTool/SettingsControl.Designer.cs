using System.Windows.Forms;
using Resources=WeSay.ConfigTool.Properties.Resources;

namespace WeSay.ConfigTool
{
	partial class SettingsControl
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
			this._areasToolStrip = new System.Windows.Forms.ToolStrip();
			this._tasksButton = new System.Windows.Forms.ToolStripButton();
			this._fieldsButton = new System.Windows.Forms.ToolStripButton();
			this._writingSystemButton = new System.Windows.Forms.ToolStripButton();
			this._actionsButton = new System.Windows.Forms.ToolStripButton();
			this._interfaceLanguageButton = new System.Windows.Forms.ToolStripButton();
			this._backupButton = new System.Windows.Forms.ToolStripButton();
			this._optionsListButton = new System.Windows.Forms.ToolStripButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this._areaPanel = new System.Windows.Forms.Panel();
			this._areaHeader = new System.Windows.Forms.RichTextBox();
			this._areasToolStrip.SuspendLayout();
			this._areaPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// _areasToolStrip
			//
			this._areasToolStrip.BackColor = System.Drawing.SystemColors.ControlLight;
			this._areasToolStrip.Dock = System.Windows.Forms.DockStyle.Left;
			this._areasToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._areasToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._areasToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this._tasksButton,
			this._writingSystemButton,
			this._fieldsButton,
			this._actionsButton,
			this._interfaceLanguageButton,
			this._backupButton,
			this._optionsListButton});
			this._areasToolStrip.Location = new System.Drawing.Point(0, 20);
			this._areasToolStrip.Name = "_areasToolStrip";
			this._areasToolStrip.Padding = new System.Windows.Forms.Padding(0);
			this._areasToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this._areasToolStrip.Size = new System.Drawing.Size(178, 349);
			this._areasToolStrip.TabIndex = 0;
			this._areasToolStrip.Text = "toolStrip1";
			//
			// _tasksButton
			//
			this._tasksButton.Checked = true;
			this._tasksButton.CheckOnClick = true;
			this._tasksButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this._tasksButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._tasksButton.ForeColor = System.Drawing.Color.DimGray;
			this._tasksButton.Image = Resources.ConfigTasks;
			this._tasksButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._tasksButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._tasksButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._tasksButton.Margin = new System.Windows.Forms.Padding(6, 0, 0, 2);
			this._tasksButton.Name = "_tasksButton";
			this._tasksButton.Size = new System.Drawing.Size(171, 36);
			this._tasksButton.Text = "Tasks";
			this._tasksButton.Click += new System.EventHandler(this.OnAreaButton_Click);
			//
			// _fieldsButton
			//
			this._fieldsButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._fieldsButton.ForeColor = System.Drawing.Color.DimGray;
			this._fieldsButton.Image = Resources.ConfigFields;
			this._fieldsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._fieldsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._fieldsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._fieldsButton.Margin = new System.Windows.Forms.Padding(6, 10, 0, 2);
			this._fieldsButton.Name = "_fieldsButton";
			this._fieldsButton.Size = new System.Drawing.Size(171, 25);
			this._fieldsButton.Text = "Fields";
			this._fieldsButton.Click += new System.EventHandler(this.OnAreaButton_Click);
			//
			// _writingSystemButton
			//
			this._writingSystemButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._writingSystemButton.ForeColor = System.Drawing.Color.DimGray;
			this._writingSystemButton.Image = Resources.ConfigWritingSystems;
			this._writingSystemButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._writingSystemButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._writingSystemButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._writingSystemButton.Margin = new System.Windows.Forms.Padding(6, 10, 0, 2);
			this._writingSystemButton.Name = "_writingSystemButton";
			this._writingSystemButton.Size = new System.Drawing.Size(171, 34);
			this._writingSystemButton.Text = "Writing Systems";
			this._writingSystemButton.Click += new System.EventHandler(this.OnAreaButton_Click);
			//
			// _actionsButton
			//
			this._actionsButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._actionsButton.ForeColor = System.Drawing.Color.DimGray;
			this._actionsButton.Image = Resources.ConfigActions;
			this._actionsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._actionsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._actionsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._actionsButton.Margin = new System.Windows.Forms.Padding(6, 10, 0, 2);
			this._actionsButton.Name = "_actionsButton";
			this._actionsButton.Size = new System.Drawing.Size(171, 41);
			this._actionsButton.Text = "Actions";
			this._actionsButton.Click += new System.EventHandler(this.OnAreaButton_Click);
			//
			// _interfaceLanguageButton
			//
			this._interfaceLanguageButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._interfaceLanguageButton.ForeColor = System.Drawing.Color.DimGray;
			this._interfaceLanguageButton.Image = Resources.ConfigInterfaceLanguage;
			this._interfaceLanguageButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._interfaceLanguageButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._interfaceLanguageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._interfaceLanguageButton.Margin = new System.Windows.Forms.Padding(6, 10, 0, 2);
			this._interfaceLanguageButton.Name = "_interfaceLanguageButton";
			this._interfaceLanguageButton.Size = new System.Drawing.Size(171, 33);
			this._interfaceLanguageButton.Text = "Interface Language";
			this._interfaceLanguageButton.Click += new System.EventHandler(this.OnAreaButton_Click);
			//
			// _backupButton
			//
			this._backupButton.BackColor = System.Drawing.SystemColors.ControlLight;
			this._backupButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._backupButton.ForeColor = System.Drawing.Color.DimGray;
			this._backupButton.Image = Resources.ConfigBackup;
			this._backupButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._backupButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._backupButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._backupButton.Margin = new System.Windows.Forms.Padding(6, 10, 0, 2);
			this._backupButton.Name = "_backupButton";
			this._backupButton.Size = new System.Drawing.Size(171, 36);
			this._backupButton.Text = "Backup Plan";
			this._backupButton.Click += new System.EventHandler(this.OnAreaButton_Click);
			//
			// _optionsListButton
			//
			this._optionsListButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
			this._optionsListButton.ForeColor = System.Drawing.Color.DimGray;
			this._optionsListButton.Image = Resources.ConfigOptionLists;
			this._optionsListButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._optionsListButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._optionsListButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._optionsListButton.Margin = new System.Windows.Forms.Padding(6, 10, 0, 2);
			this._optionsListButton.Name = "_optionsListButton";
			this._optionsListButton.Size = new System.Drawing.Size(171, 36);
			this._optionsListButton.Text = "Option Lists";
			this._optionsListButton.Click += new System.EventHandler(this.OnAreaButton_Click);
			//
			// panel1
			//
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(610, 20);
			this.panel1.TabIndex = 2;
			//
			// _areaPanel
			//
			this._areaPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._areaPanel.BackColor = System.Drawing.SystemColors.Window;
			this._areaPanel.Controls.Add(this._areaHeader);
			this._areaPanel.Location = new System.Drawing.Point(198, 26);
			this._areaPanel.Name = "_areaPanel";
			this._areaPanel.Size = new System.Drawing.Size(399, 328);
			this._areaPanel.TabIndex = 5;
			this._areaPanel.Paint += new System.Windows.Forms.PaintEventHandler(this._areaPanel_Paint);
			//
			// _areaHeader
			//
			this._areaHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._areaHeader.BackColor = System.Drawing.SystemColors.Window;
			this._areaHeader.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._areaHeader.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this._areaHeader.ForeColor = System.Drawing.Color.DimGray;
			this._areaHeader.Location = new System.Drawing.Point(14, 3);
			this._areaHeader.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
			this._areaHeader.Multiline = false;
			this._areaHeader.Name = "_areaHeader";
			this._areaHeader.ReadOnly = true;
			this._areaHeader.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this._areaHeader.Size = new System.Drawing.Size(385, 25);
			this._areaHeader.TabIndex = 5;
			this._areaHeader.TabStop = false;
			this._areaHeader.Text = "Header";
			this._areaHeader.WordWrap = false;
			//
			// SettingsControl
			//
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this._areaPanel);
			this.Controls.Add(this._areasToolStrip);
			this.Controls.Add(this.panel1);
			this.Name = "SettingsControl";
			this.Size = new System.Drawing.Size(610, 369);
			this.Load += new System.EventHandler(this.OnLoad);
			this._areasToolStrip.ResumeLayout(false);
			this._areasToolStrip.PerformLayout();
			this._areaPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip _areasToolStrip;
		private System.Windows.Forms.ToolStripButton _tasksButton;
		private System.Windows.Forms.ToolStripButton _fieldsButton;
		private System.Windows.Forms.ToolStripButton _actionsButton;
		private System.Windows.Forms.ToolStripButton _interfaceLanguageButton;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStripButton _backupButton;
		private System.Windows.Forms.ToolStripButton _writingSystemButton;
		private ToolStripButton _optionsListButton;
		private Panel _areaPanel;
		private RichTextBox _areaHeader;
	}
}