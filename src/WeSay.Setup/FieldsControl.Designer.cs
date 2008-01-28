using System.Windows.Forms;
using WeSay.Setup.Properties;

namespace WeSay.Setup
{
	partial class FieldsControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FieldsControl));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._fieldsListBox = new System.Windows.Forms.CheckedListBox();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this._btnAddField = new System.Windows.Forms.ToolStripButton();
			this._btnDeleteField = new System.Windows.Forms.ToolStripButton();
			this._tabControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this._descriptionBox = new System.Windows.Forms.Label();
			this._setupTab = new System.Windows.Forms.TabPage();
			this._fieldSetupControl = new WeSay.Setup.FieldDetailControl();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this._tabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this._setupTab.SuspendLayout();
			this.SuspendLayout();
			//
			// splitContainer1
			//
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.splitContainer1.Location = new System.Drawing.Point(3, 28);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._fieldsListBox);
			this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this._tabControl);
			this.splitContainer1.Size = new System.Drawing.Size(576, 399);
			this.splitContainer1.SplitterDistance = 164;
			this.splitContainer1.TabIndex = 0;
			//
			// _fieldsListBox
			//
			this._fieldsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._fieldsListBox.FormattingEnabled = true;
			this._fieldsListBox.Items.AddRange(new object[] {
			"Word",
			"Meaning",
			"Definition",
			"Example Sentence",
			"Translation of Example Sentence",
			"Semantic Domains",
			"Grammatical Category"});
			this._fieldsListBox.Location = new System.Drawing.Point(1, 0);
			this._fieldsListBox.Name = "_fieldsListBox";
			this._fieldsListBox.Size = new System.Drawing.Size(160, 349);
			this._fieldsListBox.TabIndex = 1;
			this._fieldsListBox.SelectedIndexChanged += new System.EventHandler(this.OnSelectedFieldChanged);
			//
			// toolStrip1
			//
			this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.toolStrip1.BackColor = System.Drawing.SystemColors.Window;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this._btnAddField,
			this._btnDeleteField});
			this.toolStrip1.Location = new System.Drawing.Point(3, 367);
			this.toolStrip1.MinimumSize = new System.Drawing.Size(200, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(200, 25);
			this.toolStrip1.TabIndex = 17;
			this.toolStrip1.Text = "toolStrip1";
			//
			// _btnAddField
			//
			this._btnAddField.ForeColor = System.Drawing.SystemColors.WindowText;
			this._btnAddField.Image = global::WeSay.Setup.Properties.Resources.genericLittleNewButton;
			this._btnAddField.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAddField.Name = "_btnAddField";
			this._btnAddField.Size = new System.Drawing.Size(74, 22);
			this._btnAddField.Text = "New Field";
			this._btnAddField.Click += new System.EventHandler(this.OnAddField_Click);
			//
			// _btnDeleteField
			//
			this._btnDeleteField.ForeColor = System.Drawing.SystemColors.WindowText;
			this._btnDeleteField.Image = global::WeSay.Setup.Properties.Resources.GenericLittleDeletionButton;
			this._btnDeleteField.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnDeleteField.Name = "_btnDeleteField";
			this._btnDeleteField.Size = new System.Drawing.Size(83, 22);
			this._btnDeleteField.Text = "Delete Field";
			this._btnDeleteField.Click += new System.EventHandler(this.OnDeleteField_Click);
			//
			// _tabControl
			//
			this._tabControl.Controls.Add(this.tabPage1);
			this._tabControl.Controls.Add(this._setupTab);
			this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tabControl.Location = new System.Drawing.Point(0, 0);
			this._tabControl.Name = "_tabControl";
			this._tabControl.SelectedIndex = 0;
			this._tabControl.Size = new System.Drawing.Size(408, 399);
			this._tabControl.TabIndex = 1;
			//
			// tabPage1
			//
			this.tabPage1.Controls.Add(this._descriptionBox);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(400, 373);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "About";
			this.tabPage1.UseVisualStyleBackColor = true;
			//
			// _descriptionBox
			//
			this._descriptionBox.AutoSize = true;
			this._descriptionBox.Location = new System.Drawing.Point(19, 30);
			this._descriptionBox.MaximumSize = new System.Drawing.Size(300, 0);
			this._descriptionBox.Name = "_descriptionBox";
			this._descriptionBox.Size = new System.Drawing.Size(294, 52);
			this._descriptionBox.TabIndex = 2;
			this._descriptionBox.Text = resources.GetString("_descriptionBox.Text");
			//
			// _setupTab
			//
			this._setupTab.Controls.Add(this._fieldSetupControl);
			this._setupTab.Location = new System.Drawing.Point(4, 22);
			this._setupTab.Name = "_setupTab";
			this._setupTab.Padding = new System.Windows.Forms.Padding(3);
			this._setupTab.Size = new System.Drawing.Size(349, 373);
			this._setupTab.TabIndex = 1;
			this._setupTab.Text = "Setup";
			this._setupTab.UseVisualStyleBackColor = true;
			//
			// _fieldSetupControl
			//
			this._fieldSetupControl.AutoScroll = true;
			this._fieldSetupControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldSetupControl.Location = new System.Drawing.Point(3, 3);
			this._fieldSetupControl.Name = "_fieldSetupControl";
			this._fieldSetupControl.Size = new System.Drawing.Size(343, 367);
			this._fieldSetupControl.TabIndex = 0;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(268, 13);
			this.label1.TabIndex = 18;
			this.label1.Text = "Check each field you want WeSay to display. ";
			//
			// FieldsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "FieldsControl";
			this.Size = new System.Drawing.Size(586, 427);
			this.Load += new System.EventHandler(this.FieldsControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this._tabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this._setupTab.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}



		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage _setupTab;
		private System.Windows.Forms.ToolStripButton _btnAddField;
		private System.Windows.Forms.ToolStripButton _btnDeleteField;
		private System.Windows.Forms.CheckedListBox _fieldsListBox;
		private FieldDetailControl _fieldSetupControl;
		private Label _descriptionBox;
	}
}
