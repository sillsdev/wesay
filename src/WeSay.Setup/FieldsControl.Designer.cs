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
			this.fieldSettingsTabControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._writingSystemListBox = new System.Windows.Forms.CheckedListBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._descriptionBox = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this._fieldPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this._btnAddField = new System.Windows.Forms.ToolStripButton();
			this._btnDeleteField = new System.Windows.Forms.ToolStripButton();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.fieldSettingsTabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			//
			// splitContainer1
			//
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(10, 23);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._fieldsListBox);
			this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this.fieldSettingsTabControl);
			this.splitContainer1.Size = new System.Drawing.Size(590, 402);
			this.splitContainer1.SplitterDistance = 222;
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
			"Meaning (Gloss)",
			"Definition",
			"Example Sentence",
			"Translation of Example Sentence",
			"Semantic Domains",
			"Grammatical Category"});
			this._fieldsListBox.Location = new System.Drawing.Point(1, 0);
			this._fieldsListBox.Name = "_fieldsListBox";
			this._fieldsListBox.Size = new System.Drawing.Size(219, 364);
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
			this.toolStrip1.Location = new System.Drawing.Point(3, 370);
			this.toolStrip1.MinimumSize = new System.Drawing.Size(200, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(200, 25);
			this.toolStrip1.TabIndex = 17;
			this.toolStrip1.Text = "toolStrip1";
			//
			// fieldSettingsTabControl
			//
			this.fieldSettingsTabControl.Controls.Add(this.tabPage1);
			this.fieldSettingsTabControl.Controls.Add(this.tabPage2);
			this.fieldSettingsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fieldSettingsTabControl.Location = new System.Drawing.Point(0, 0);
			this.fieldSettingsTabControl.Name = "fieldSettingsTabControl";
			this.fieldSettingsTabControl.SelectedIndex = 0;
			this.fieldSettingsTabControl.Size = new System.Drawing.Size(364, 402);
			this.fieldSettingsTabControl.TabIndex = 1;
			//
			// tabPage1
			//
			this.tabPage1.Controls.Add(this.tableLayoutPanel1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(356, 376);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Usage";
			this.tabPage1.UseVisualStyleBackColor = true;
			//
			// tableLayoutPanel1
			//
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(350, 370);
			this.tableLayoutPanel1.TabIndex = 1;
			//
			// groupBox1
			//
			this.groupBox1.Controls.Add(this.btnMoveDown);
			this.groupBox1.Controls.Add(this.btnMoveUp);
			this.groupBox1.Controls.Add(this._writingSystemListBox);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 188);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(7);
			this.groupBox1.Size = new System.Drawing.Size(344, 179);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Show these Writing Systems in this field";
			//
			// _writingSystemListBox
			//
			this._writingSystemListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._writingSystemListBox.FormattingEnabled = true;
			this._writingSystemListBox.Items.AddRange(new object[] {
			"Foo(IPA)",
			"Foo(Thai)",
			"Thai",
			"English"});
			this._writingSystemListBox.Location = new System.Drawing.Point(31, 20);
			this._writingSystemListBox.Name = "_writingSystemListBox";
			this._writingSystemListBox.Size = new System.Drawing.Size(306, 139);
			this._writingSystemListBox.TabIndex = 0;
			this._writingSystemListBox.SelectedIndexChanged += new System.EventHandler(this._writingSystemListBox_SelectedIndexChanged);
			this._writingSystemListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._writingSystemListBox_ItemCheck);
			//
			// groupBox2
			//
			this.groupBox2.Controls.Add(this._descriptionBox);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(3, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(344, 179);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "About This Field";
			this.groupBox2.SizeChanged += new System.EventHandler(this.groupBox2_SizeChanged);
			//
			// _descriptionBox
			//
			this._descriptionBox.AutoSize = true;
			this._descriptionBox.Location = new System.Drawing.Point(15, 27);
			this._descriptionBox.MaximumSize = new System.Drawing.Size(300, 0);
			this._descriptionBox.Name = "_descriptionBox";
			this._descriptionBox.Size = new System.Drawing.Size(294, 52);
			this._descriptionBox.TabIndex = 0;
			this._descriptionBox.Text = resources.GetString("_descriptionBox.Text");
			//
			// tabPage2
			//
			this.tabPage2.Controls.Add(this._fieldPropertyGrid);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(356, 376);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Properties";
			this.tabPage2.UseVisualStyleBackColor = true;
			//
			// _fieldPropertyGrid
			//
			this._fieldPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldPropertyGrid.LineColor = System.Drawing.SystemColors.Control;
			this._fieldPropertyGrid.Location = new System.Drawing.Point(3, 3);
			this._fieldPropertyGrid.Name = "_fieldPropertyGrid";
			this._fieldPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this._fieldPropertyGrid.SelectedObject = this.btnMoveDown;
			this._fieldPropertyGrid.Size = new System.Drawing.Size(350, 370);
			this._fieldPropertyGrid.TabIndex = 0;
			this._fieldPropertyGrid.ToolbarVisible = false;
			this._fieldPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.OnPropertyValueChanged);
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(224, 13);
			this.label1.TabIndex = 18;
			this.label1.Text = "Check each field you want WeSay to display. ";
			//
			// _btnAddField
			//
			this._btnAddField.Image = global::WeSay.Setup.Properties.Resources.genericLittleNewButton;
			this._btnAddField.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAddField.Name = "_btnAddField";
			this._btnAddField.Size = new System.Drawing.Size(73, 22);
			this._btnAddField.Text = "New Field";
			this._btnAddField.Click += new System.EventHandler(this._btnAddField_Click);
			//
			// _btnDeleteField
			//
			this._btnDeleteField.Image = global::WeSay.Setup.Properties.Resources.GenericLittleDeletionButton;
			this._btnDeleteField.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnDeleteField.Name = "_btnDeleteField";
			this._btnDeleteField.Size = new System.Drawing.Size(83, 22);
			this._btnDeleteField.Text = "Delete Field";
			this._btnDeleteField.Click += new System.EventHandler(this.OnDeleteField_Click);
			//
			// btnMoveDown
			//
			this.btnMoveDown.FlatAppearance.BorderSize = 0;
			this.btnMoveDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveDown.Image")));
			this.btnMoveDown.Location = new System.Drawing.Point(7, 46);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(18, 21);
			this.btnMoveDown.TabIndex = 2;
			this.toolTip1.SetToolTip(this.btnMoveDown, "List this writing system later");
			this.btnMoveDown.UseVisualStyleBackColor = true;
			this.btnMoveDown.Click += new System.EventHandler(this.OnBtnMoveDownClick);
			//
			// btnMoveUp
			//
			this.btnMoveUp.FlatAppearance.BorderSize = 0;
			this.btnMoveUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveUp.Image")));
			this.btnMoveUp.Location = new System.Drawing.Point(8, 21);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(17, 19);
			this.btnMoveUp.TabIndex = 1;
			this.toolTip1.SetToolTip(this.btnMoveUp, "List this writing system earlier");
			this.btnMoveUp.UseVisualStyleBackColor = true;
			this.btnMoveUp.Click += new System.EventHandler(this.OnBtnMoveUpClick);
			//
			// FieldsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "FieldsControl";
			this.Padding = new System.Windows.Forms.Padding(7);
			this.Size = new System.Drawing.Size(604, 425);
			this.Load += new System.EventHandler(this.FieldsControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.fieldSettingsTabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}



		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabControl fieldSettingsTabControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Button btnMoveUp;
		private System.Windows.Forms.CheckedListBox _writingSystemListBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.PropertyGrid _fieldPropertyGrid;
		private System.Windows.Forms.ToolStripButton _btnAddField;
		private System.Windows.Forms.ToolStripButton _btnDeleteField;
		private System.Windows.Forms.CheckedListBox _fieldsListBox;
		private System.Windows.Forms.Label _descriptionBox;
	}
}
