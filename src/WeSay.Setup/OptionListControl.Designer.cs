namespace WeSay.Setup
{
	partial class OptionListControl
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
			this.panel1 = new System.Windows.Forms.Panel();
			this._listDescriptionLabel = new System.Windows.Forms.Label();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._listBox = new System.Windows.Forms.ListBox();
			this._keyText = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._keyLabel = new System.Windows.Forms.Label();
			this._nameMultiTextControl = new WeSay.UI.MultiTextControl();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this._btnAdd = new System.Windows.Forms.ToolStripButton();
			this._btnDelete = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this._btnEditList = new System.Windows.Forms.ToolStripButton();
			this._btnNewList = new System.Windows.Forms.ToolStripButton();
			this.panel1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			//
			// panel1
			//
			this.panel1.Controls.Add(this._listDescriptionLabel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(420, 36);
			this.panel1.TabIndex = 0;
			//
			// _listDescriptionLabel
			//
			this._listDescriptionLabel.AutoSize = true;
			this._listDescriptionLabel.Location = new System.Drawing.Point(3, 9);
			this._listDescriptionLabel.Name = "_listDescriptionLabel";
			this._listDescriptionLabel.Size = new System.Drawing.Size(50, 13);
			this._listDescriptionLabel.TabIndex = 0;
			this._listDescriptionLabel.Text = "blah blah";
			//
			// splitContainer1
			//
			this.splitContainer1.Location = new System.Drawing.Point(0, 36);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._listBox);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this._keyText);
			this.splitContainer1.Panel2.Controls.Add(this.label3);
			this.splitContainer1.Panel2.Controls.Add(this._keyLabel);
			this.splitContainer1.Panel2.Controls.Add(this._nameMultiTextControl);
			this.splitContainer1.Size = new System.Drawing.Size(392, 172);
			this.splitContainer1.SplitterDistance = 132;
			this.splitContainer1.TabIndex = 1;
			//
			// _listBox
			//
			this._listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._listBox.FormattingEnabled = true;
			this._listBox.Location = new System.Drawing.Point(1, 26);
			this._listBox.Name = "_listBox";
			this._listBox.Size = new System.Drawing.Size(127, 147);
			this._listBox.TabIndex = 0;
			this._listBox.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
			//
			// _keyText
			//
			this._keyText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._keyText.Location = new System.Drawing.Point(56, 94);
			this._keyText.Name = "_keyText";
			this._keyText.Size = new System.Drawing.Size(100, 20);
			this._keyText.TabIndex = 2;
			this.toolTip1.SetToolTip(this._keyText, "The key that will show up in the xml file; if this option list were to be lost, i" +
					"t is the key that would need to communicate what value the field is set to.");
			this._keyText.TextChanged += new System.EventHandler(this.OnKeyTextChanged);
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(2, 31);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(33, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Label";
			//
			// _keyLabel
			//
			this._keyLabel.AutoSize = true;
			this._keyLabel.Location = new System.Drawing.Point(3, 94);
			this._keyLabel.Name = "_keyLabel";
			this._keyLabel.Size = new System.Drawing.Size(25, 13);
			this._keyLabel.TabIndex = 1;
			this._keyLabel.Text = "Key";
			//
			// _nameMultiTextControl
			//
			this._nameMultiTextControl.AutoSize = true;
			this._nameMultiTextControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._nameMultiTextControl.BackColor = System.Drawing.Color.Red;
			this._nameMultiTextControl.ColumnCount = 2;
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._nameMultiTextControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._nameMultiTextControl.Location = new System.Drawing.Point(39, 31);
			this._nameMultiTextControl.Name = "_nameMultiTextControl";
			this._nameMultiTextControl.RowCount = 2;
			this._nameMultiTextControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._nameMultiTextControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._nameMultiTextControl.ShowAnnotationWidget = false;
			this._nameMultiTextControl.Size = new System.Drawing.Size(0, 0);
			this._nameMultiTextControl.TabIndex = 0;
			//
			// toolTip1
			//
			this.toolTip1.AutomaticDelay = 200;
			this.toolTip1.AutoPopDelay = 8000;
			this.toolTip1.InitialDelay = 200;
			this.toolTip1.ReshowDelay = 40;
			//
			// toolStrip1
			//
			this.toolStrip1.BackColor = System.Drawing.SystemColors.Window;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this._btnAdd,
			this._btnDelete,
			this.toolStripSeparator1,
			this._btnEditList,
			this._btnNewList});
			this.toolStrip1.Location = new System.Drawing.Point(0, 212);
			this.toolStrip1.MinimumSize = new System.Drawing.Size(200, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(420, 25);
			this.toolStrip1.TabIndex = 20;
			this.toolStrip1.Text = "toolStrip1";
			//
			// _btnAdd
			//
			this._btnAdd.Image = global::WeSay.Setup.Properties.Resources.NewOptionListItem;
			this._btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAdd.Name = "_btnAdd";
			this._btnAdd.Size = new System.Drawing.Size(73, 22);
			this._btnAdd.Text = "New Item";
			this._btnAdd.Click += new System.EventHandler(this._btnAdd_Click);
			//
			// _btnDelete
			//
			this._btnDelete.Image = global::WeSay.Setup.Properties.Resources.deleteOptionListItem;
			this._btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnDelete.Name = "_btnDelete";
			this._btnDelete.Size = new System.Drawing.Size(83, 22);
			this._btnDelete.Text = "Delete Item";
			this._btnDelete.Click += new System.EventHandler(this._btnDelete_Click);
			//
			// toolStripSeparator1
			//
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			//
			// _btnEditList
			//
			this._btnEditList.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this._btnEditList.Enabled = false;
			this._btnEditList.Image = global::WeSay.Setup.Properties.Resources.openOptionsList;
			this._btnEditList.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnEditList.Name = "_btnEditList";
			this._btnEditList.Size = new System.Drawing.Size(114, 22);
			this._btnEditList.Text = "Edit another list...";
			this._btnEditList.Click += new System.EventHandler(this._btnEditList_Click);
			//
			// _btnNewList
			//
			this._btnNewList.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this._btnNewList.Enabled = false;
			this._btnNewList.Image = global::WeSay.Setup.Properties.Resources.NewOptionsList;
			this._btnNewList.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnNewList.Name = "_btnNewList";
			this._btnNewList.Size = new System.Drawing.Size(120, 22);
			this._btnNewList.Text = "Create a new list...";
			//
			// OptionListControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.panel1);
			this.Name = "OptionListControl";
			this.Size = new System.Drawing.Size(420, 237);
			this.Load += new System.EventHandler(this.OnLoad);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox _listBox;
		private System.Windows.Forms.Label _listDescriptionLabel;
		private WeSay.UI.MultiTextControl _nameMultiTextControl;
		private System.Windows.Forms.TextBox _keyText;
		private System.Windows.Forms.Label _keyLabel;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton _btnAdd;
		private System.Windows.Forms.ToolStripButton _btnDelete;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton _btnEditList;
		private System.Windows.Forms.ToolStripButton _btnNewList;

	}
}
