using System;

namespace WeSay.LexicalTools
{
	partial class EntryDetailTask
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
			this._entryDetailPanel = new WeSay.LexicalTools.EntryDetailControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this._recordsListBox = new ListBox.BindingListGrid();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			//
			// _entryDetailPanel
			//
			this._entryDetailPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._entryDetailPanel.AutoScroll = true;
			this._entryDetailPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this._entryDetailPanel.DataSource = null;
			this._entryDetailPanel.Location = new System.Drawing.Point(126, 0);
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				//this._entryDetailPanel.Margin = new System.Windows.Forms.Padding(5);
			}
			this._entryDetailPanel.Name = "_entryDetailPanel";
			this._entryDetailPanel.Size = new System.Drawing.Size(367, 127);
			this._entryDetailPanel.TabIndex = 4;
			//
			// panel1
			//
			this.panel1.Controls.Add(this.btnDelete);
			this.panel1.Controls.Add(this.btnAdd);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 135);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(493, 34);
			this.panel1.TabIndex = 7;
			//
			// btnDelete
			//
			this.btnDelete.Location = new System.Drawing.Point(163, 7);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(75, 23);
			this.btnDelete.TabIndex = 7;
			this.btnDelete.Text = "Delete Word";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			//
			// btnAdd
			//
			this.btnAdd.Location = new System.Drawing.Point(18, 8);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(75, 23);
			this.btnAdd.TabIndex = 6;
			this.btnAdd.Text = "New Word";
			this.btnAdd.UseVisualStyleBackColor = true;
			//
			// _recordsListBox
			//
			this._recordsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this._recordsListBox.GridToolTipActive = true;
			this._recordsListBox.Location = new System.Drawing.Point(3, 20);
			this._recordsListBox.Name = "_recordsListBox";
			this._recordsListBox.SelectedIndex = 0;
			this._recordsListBox.Size = new System.Drawing.Size(118, 110);
			this._recordsListBox.SpecialKeys = ((SourceGrid3.GridSpecialKeys)(((((((SourceGrid3.GridSpecialKeys.Arrows | SourceGrid3.GridSpecialKeys.Tab)
						| SourceGrid3.GridSpecialKeys.PageDownUp)
						| SourceGrid3.GridSpecialKeys.Enter)
						| SourceGrid3.GridSpecialKeys.Escape)
						| SourceGrid3.GridSpecialKeys.Control)
						| SourceGrid3.GridSpecialKeys.Shift)));
			this._recordsListBox.StyleGrid = null;
			this._recordsListBox.TabIndex = 8;
			//
			// EntryDetailTask
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._recordsListBox);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._entryDetailPanel);
			this.Name = "EntryDetailTask";
			this.Size = new System.Drawing.Size(493, 169);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private EntryDetailControl _entryDetailPanel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnAdd;
		private ListBox.BindingListGrid _recordsListBox;
	}
}
