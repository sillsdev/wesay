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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntryDetailTask));
			this._entryDetailPanel = new WeSay.LexicalTools.EntryDetailControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this._recordsListBox = new ListBox.BindingListGrid();
			this._btnNewWord = new System.Windows.Forms.LinkLabel();
			this._btnDeleteWord = new System.Windows.Forms.LinkLabel();
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
			this._entryDetailPanel.Name = "_entryDetailPanel";
			this._entryDetailPanel.Size = new System.Drawing.Size(367, 127);
			this._entryDetailPanel.TabIndex = 4;
			//
			// panel1
			//
			this.panel1.Controls.Add(this._btnDeleteWord);
			this.panel1.Controls.Add(this._btnNewWord);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 135);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(493, 34);
			this.panel1.TabIndex = 7;
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
			// _btnNewWord
			//
			this._btnNewWord.AutoSize = true;
			this._btnNewWord.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this._btnNewWord.LinkColor = System.Drawing.Color.Black;
			this._btnNewWord.Location = new System.Drawing.Point(15, 7);
			this._btnNewWord.Name = "_btnNewWord";
			this._btnNewWord.Size = new System.Drawing.Size(58, 13);
			this._btnNewWord.TabIndex = 8;
			this._btnNewWord.TabStop = true;
			this._btnNewWord.Text = "New Word";
			this._btnNewWord.VisitedLinkColor = System.Drawing.Color.Black;
			this._btnNewWord.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._btnNewWord_LinkClicked);
			//
			// _btnDeleteWord
			//
			this._btnDeleteWord.AutoSize = true;
			this._btnDeleteWord.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this._btnDeleteWord.LinkColor = System.Drawing.Color.Black;
			this._btnDeleteWord.Location = new System.Drawing.Point(135, 7);
			this._btnDeleteWord.Name = "_btnDeleteWord";
			this._btnDeleteWord.Size = new System.Drawing.Size(90, 13);
			this._btnDeleteWord.TabIndex = 9;
			this._btnDeleteWord.TabStop = true;
			this._btnDeleteWord.Text = "Delete This Word";
			this._btnDeleteWord.VisitedLinkColor = System.Drawing.Color.Black;
			this._btnDeleteWord.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._btnDeleteWord_LinkClicked);
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
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private EntryDetailControl _entryDetailPanel;
		private System.Windows.Forms.Panel panel1;
		private ListBox.BindingListGrid _recordsListBox;
		private System.Windows.Forms.LinkLabel _btnDeleteWord;
		private System.Windows.Forms.LinkLabel _btnNewWord;
	}
}
