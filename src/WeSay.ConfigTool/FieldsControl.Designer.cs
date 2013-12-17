using System.Windows.Forms;
using WeSay.ConfigTool.Properties;
using Resources=WeSay.ConfigTool.Properties.Resources;

namespace WeSay.ConfigTool
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
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Entry", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Sense", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Example", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Everywhere", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Lexical Form");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Definition");
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Sentence");
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Note");
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this._fieldsListBox = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this._btnAddField = new System.Windows.Forms.ToolStripButton();
			this._btnDeleteField = new System.Windows.Forms.ToolStripButton();
			this._tabControl = new System.Windows.Forms.TabControl();
			this._setupTab = new System.Windows.Forms.TabPage();
			this._fieldSetupControl = new WeSay.ConfigTool.FieldDetailControl();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this._tabControl.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.btnMoveDown);
			this.splitContainer1.Panel1.Controls.Add(this.btnMoveUp);
			this.splitContainer1.Panel1.Controls.Add(this._fieldsListBox);
			this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this._tabControl);
			this.splitContainer1.Size = new System.Drawing.Size(576, 399);
			this.splitContainer1.SplitterDistance = 200;
			this.splitContainer1.TabIndex = 0;
			//
			// btnMoveDown
			//
			this.btnMoveDown.FlatAppearance.BorderSize = 0;
			this.btnMoveDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveDown.Image")));
			this.btnMoveDown.Location = new System.Drawing.Point(3, 63);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(18, 21);
			this.btnMoveDown.TabIndex = 22;
			this.toolTip1.SetToolTip(this.btnMoveDown, "Move the selected field downwards in the display order.");
			this.btnMoveDown.UseVisualStyleBackColor = true;
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
			//
			// btnMoveUp
			//
			this.btnMoveUp.FlatAppearance.BorderSize = 0;
			this.btnMoveUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveUp.Image")));
			this.btnMoveUp.Location = new System.Drawing.Point(3, 42);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(17, 15);
			this.btnMoveUp.TabIndex = 21;
			this.toolTip1.SetToolTip(this.btnMoveUp, "Move the selected field upwards in the display order.");
			this.btnMoveUp.UseVisualStyleBackColor = true;
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
			//
			// _fieldsListBox
			//
			this._fieldsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._fieldsListBox.CheckBoxes = true;
			this._fieldsListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnHeader1});
			listViewGroup1.Header = "Entry";
			listViewGroup1.Name = "LexEntry";
			listViewGroup2.Header = "Sense";
			listViewGroup2.Name = "LexSense";
			listViewGroup3.Header = "Example";
			listViewGroup3.Name = "LexExampleSentence";
			listViewGroup4.Header = "Everywhere";
			listViewGroup4.Name = "WeSayDataObject";
			this._fieldsListBox.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
			listViewGroup1,
			listViewGroup2,
			listViewGroup3,
			listViewGroup4});
			this._fieldsListBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this._fieldsListBox.HideSelection = false;
			listViewItem1.Group = listViewGroup1;
			listViewItem1.StateImageIndex = 0;
			listViewItem2.Group = listViewGroup2;
			listViewItem2.StateImageIndex = 0;
			listViewItem3.Group = listViewGroup3;
			listViewItem3.StateImageIndex = 0;
			listViewItem4.Group = listViewGroup4;
			listViewItem4.StateImageIndex = 0;
			this._fieldsListBox.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
			listViewItem1,
			listViewItem2,
			listViewItem3,
			listViewItem4});
			this._fieldsListBox.Location = new System.Drawing.Point(26, 4);
			this._fieldsListBox.MultiSelect = false;
			this._fieldsListBox.Name = "_fieldsListBox";
			this._fieldsListBox.ShowItemToolTips = true;
			this._fieldsListBox.Size = new System.Drawing.Size(171, 345);
			this._fieldsListBox.TabIndex = 18;
			this._fieldsListBox.UseCompatibleStateImageBehavior = false;
			this._fieldsListBox.View = System.Windows.Forms.View.Details;
			this._fieldsListBox.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.OnSelectedFieldChanged);
			//
			// columnHeader1
			//
			this.columnHeader1.Text = "";
			this.columnHeader1.Width = 120;
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
			this._btnAddField.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAddField.Name = "_btnAddField";
			this._btnAddField.Size = new System.Drawing.Size(57, 22);
			this._btnAddField.Text = "New Field";
			this._btnAddField.Click += new System.EventHandler(this.OnAddField_Click);
			//
			// _btnDeleteField
			//
			this._btnDeleteField.ForeColor = System.Drawing.SystemColors.WindowText;
			this._btnDeleteField.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnDeleteField.Name = "_btnDeleteField";
			this._btnDeleteField.Size = new System.Drawing.Size(67, 22);
			this._btnDeleteField.Text = "Delete Field";
			this._btnDeleteField.Click += new System.EventHandler(this.OnDeleteField_Click);
			//
			// _tabControl
			//
			this._tabControl.Controls.Add(this._setupTab);
			this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tabControl.Location = new System.Drawing.Point(0, 0);
			this._tabControl.Name = "_tabControl";
			this._tabControl.SelectedIndex = 0;
			this._tabControl.Size = new System.Drawing.Size(372, 399);
			this._tabControl.TabIndex = 1;
			//
			// _setupTab
			//
			this._setupTab.Controls.Add(this._fieldSetupControl);
			this._setupTab.Location = new System.Drawing.Point(4, 22);
			this._setupTab.Name = "_setupTab";
			this._setupTab.Padding = new System.Windows.Forms.Padding(3);
			this._setupTab.Size = new System.Drawing.Size(364, 373);
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
			this._fieldSetupControl.Size = new System.Drawing.Size(358, 367);
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
		private System.Windows.Forms.TabPage _setupTab;
		private System.Windows.Forms.ToolStripButton _btnAddField;
		private System.Windows.Forms.ToolStripButton _btnDeleteField;
		private FieldDetailControl _fieldSetupControl;
		private ListView _fieldsListBox;
		private Button btnMoveDown;
		private Button btnMoveUp;
		private ColumnHeader columnHeader1;
	}
}
