using WeSay.UI.TextBoxes;
using Resources=WeSay.ConfigTool.Properties.Resources;

namespace WeSay.ConfigTool
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
			this.splitContainer1 = new SplitContainer2();
			this._listBox = new System.Windows.Forms.ListBox();
			this._nameLabel = new System.Windows.Forms.Label();
			this._nameMultiTextControl = new WeSay.UI.TextBoxes.MultiTextControl();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this._btnAdd = new System.Windows.Forms.ToolStripButton();
			this._btnDelete = new System.Windows.Forms.ToolStripButton();
			this._fieldChooser = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this._keyLabel = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			//
			// splitContainer1
			//
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(0, 26);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this._listBox);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this._keyLabel);
			this.splitContainer1.Panel2.Controls.Add(this._nameLabel);
			this.splitContainer1.Panel2.Controls.Add(this._nameMultiTextControl);
			this.splitContainer1.Size = new System.Drawing.Size(392, 182);
			this.splitContainer1.SplitterDistance = 101;
			this.splitContainer1.TabIndex = 1;
			//
			// _listBox
			//
			this._listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._listBox.FormattingEnabled = true;
			this._listBox.Location = new System.Drawing.Point(1, 8);
			this._listBox.Name = "_listBox";
			this._listBox.Size = new System.Drawing.Size(96, 173);
			this._listBox.TabIndex = 0;
			this._listBox.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
			//
			// _nameLabel
			//
			this._nameLabel.AutoSize = true;
			this._nameLabel.Location = new System.Drawing.Point(2, 11);
			this._nameLabel.Name = "_nameLabel";
			this._nameLabel.Size = new System.Drawing.Size(35, 13);
			this._nameLabel.TabIndex = 1;
			this._nameLabel.Text = "Name";
			//
			// _nameMultiTextControl
			//
			this._nameMultiTextControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._nameMultiTextControl.AutoSize = true;
			this._nameMultiTextControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._nameMultiTextControl.BackColor = System.Drawing.Color.Red;
			this._nameMultiTextControl.ColumnCount = 3;
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
			this._nameMultiTextControl.IsSpellCheckingEnabled = false;
			this._nameMultiTextControl.Location = new System.Drawing.Point(56, 11);
			this._nameMultiTextControl.MinimumSize = new System.Drawing.Size(140, 20);
			this._nameMultiTextControl.Name = "_nameMultiTextControl";
			this._nameMultiTextControl.ShowAnnotationWidget = false;
			this._nameMultiTextControl.Size = new System.Drawing.Size(140, 20);
			this._nameMultiTextControl.TabIndex = 0;
			this._toolTip.SetToolTip(this._nameMultiTextControl, "__");
			//
			// _toolTip
			//
			this._toolTip.AutomaticDelay = 200;
			this._toolTip.AutoPopDelay = 8000;
			this._toolTip.InitialDelay = 200;
			this._toolTip.ReshowDelay = 40;
			//
			// toolStrip1
			//
			this.toolStrip1.BackColor = System.Drawing.SystemColors.Window;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this._btnAdd,
			this._btnDelete});
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
			this._btnAdd.Image = global::WeSay.ConfigTool.Properties.Resources.NewOptionListItem;
			this._btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAdd.Name = "_btnAdd";
			this._btnAdd.Size = new System.Drawing.Size(91, 22);
			this._btnAdd.Text = "New Option";
			this._btnAdd.Click += new System.EventHandler(this._btnAdd_Click);
			//
			// _btnDelete
			//
			this._btnDelete.Image = global::WeSay.ConfigTool.Properties.Resources.deleteOptionListItem;
			this._btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnDelete.Name = "_btnDelete";
			this._btnDelete.Size = new System.Drawing.Size(100, 22);
			this._btnDelete.Text = "Delete Option";
			this._btnDelete.Click += new System.EventHandler(this._btnDelete_Click);
			//
			// _fieldChooser
			//
			this._fieldChooser.Location = new System.Drawing.Point(53, 2);
			this._fieldChooser.Name = "_fieldChooser";
			this._fieldChooser.Size = new System.Drawing.Size(121, 21);
			this._fieldChooser.Sorted = true;
			this._fieldChooser.TabIndex = 27;
			this._fieldChooser.SelectedIndexChanged += new System.EventHandler(this.OnFieldChooser_SelectedIndexChanged);
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 15);
			this.label1.TabIndex = 26;
			this.label1.Text = "Field:";
			//
			// _keyLabel
			//
			this._keyLabel.AutoSize = true;
			this._keyLabel.ForeColor = System.Drawing.Color.Silver;
			this._keyLabel.Location = new System.Drawing.Point(53, 44);
			this._keyLabel.Name = "_keyLabel";
			this._keyLabel.Size = new System.Drawing.Size(98, 13);
			this._keyLabel.TabIndex = 4;
			this._keyLabel.Text = "Key in LIFT file: foo";
			//
			// OptionListControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this._fieldChooser);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "OptionListControl";
			this.Size = new System.Drawing.Size(420, 237);
			this.Load += new System.EventHandler(this.OnLoad);
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

		private SplitContainer2 splitContainer1;
		private System.Windows.Forms.ListBox _listBox;
		private MultiTextControl _nameMultiTextControl;
		private System.Windows.Forms.ToolTip _toolTip;
		private System.Windows.Forms.Label _nameLabel;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton _btnAdd;
		private System.Windows.Forms.ToolStripButton _btnDelete;
		private System.Windows.Forms.ComboBox _fieldChooser;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label _keyLabel;

	}
}
