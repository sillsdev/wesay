using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalTools.Properties;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	partial class EntryDetailControl
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
			if (disposing && !IsDisposed)
			{
				_recordsListBox.SelectedIndexChanged -= OnRecordSelectionChanged;
				_findText.KeyDown -= _findText_KeyDown;
			}
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
			this._entryDetailPanel = new WeSay.LexicalTools.LexPreviewWithEntryControl();

			this.panel1 = new System.Windows.Forms.Panel();
			this._recordsListBox = new System.Windows.Forms.ListBox();
			this._btnNewWord = new System.Windows.Forms.Button();
			this._btnDeleteWord = new System.Windows.Forms.Button();
			this._btnFind = new System.Windows.Forms.Button();
			this._findText = new WeSay.UI.WeSayAutoCompleteTextBox();
			this._findWritingSystemId = new Label();

			this._findText.AutoCompleteChoiceSelected += new System.EventHandler(_findText_AutoCompleteChoiceSelected);
			this._writingSystemChooser = new Button();

			this.panel1.SuspendLayout();
			this.SuspendLayout();

			//
			// _entryDetailPanel
			//
			this.Control_EntryDetailPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Control_EntryDetailPanel.AutoScroll = true;
			this.Control_EntryDetailPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.Control_EntryDetailPanel.DataSource = null;
			this.Control_EntryDetailPanel.Location = new System.Drawing.Point(126, 0);
			this.Control_EntryDetailPanel.Name = "_entryDetailPanel";
			this.Control_EntryDetailPanel.Size = new System.Drawing.Size(367, 127);
			this.Control_EntryDetailPanel.TabIndex = 0;
			this.Control_EntryDetailPanel.BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;

			//
			// _findWritingSystemId
			//
			this._findWritingSystemId.Location = new System.Drawing.Point(3, 7);
			this._findWritingSystemId.Size = new System.Drawing.Size(18, 13);
			this._findWritingSystemId.AutoSize = true;
			this._findWritingSystemId.ForeColor = System.Drawing.Color.LightGray;
			this._findWritingSystemId.BackColor = Color.White;
			this._findWritingSystemId.TextAlign = ContentAlignment.MiddleLeft;
			this._findWritingSystemId.MouseClick += new MouseEventHandler(_findWritingSystemId_MouseClick);

			//
			// _findText
			//
			this._findText.Location = new System.Drawing.Point(18, 7);
			this._findText.Name = "_findText";
			this._findText.Size = new System.Drawing.Size(118, 13);
			this._findText.TabIndex = 1;
			this._findText.TabStop = true;
			this._findText.Text = "";

			//
			// _btnFind
			//
			this._btnFind.AutoSize = false;
			this._btnFind.Location = new System.Drawing.Point(80, this._findText.Top);
			this._btnFind.Name = "_btnFind";
			this._btnFind.TabIndex = 2;
			this._btnFind.TabStop = true;
			this._btnFind.Click += new System.EventHandler(_btnFind_Click);
			this._btnFind.BackColor = _findText.BackColor;
			this._btnFind.FlatStyle = FlatStyle.Flat;
			this._btnFind.FlatAppearance.MouseOverBackColor = Color.Orange;
			this._btnFind.FlatAppearance.BorderSize = 0;


			//
			// _writingSystemChooser
			//
			this._writingSystemChooser.AutoSize = false;
			this._writingSystemChooser.Location = new System.Drawing.Point(107, this._findText.Top);
			 this._writingSystemChooser.Name = "_writingSystemChooser";
		   this._writingSystemChooser.Size = new Size(15, this._btnFind.Height);
			this._writingSystemChooser.TabStop = true;
			this._writingSystemChooser.TabIndex = 3;
			this._writingSystemChooser.Text = "";
			this._writingSystemChooser.BackColor = _findText.BackColor;
			this._writingSystemChooser.FlatStyle = FlatStyle.Flat;
			this._writingSystemChooser.FlatAppearance.MouseOverBackColor = Color.Orange;
			this._writingSystemChooser.FlatAppearance.BorderSize = 0;
			this._writingSystemChooser.Image = Resources.Expand.GetThumbnailImage(6,
										   6,
										   ReturnFalse,
										   IntPtr.Zero);
			this._writingSystemChooser.Click += new EventHandler(_writingSystemChooser_Click);



			//
			// _recordsListBox
			//
			this._recordsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this._recordsListBox.Location = new System.Drawing.Point(3, 20);
			this._recordsListBox.Name = "_recordsListBox";
			this._recordsListBox.Size = new System.Drawing.Size(118, 120);
			this._recordsListBox.TabIndex = 4;
			//
			// panel1
			//
			this.panel1.Controls.Add(this._btnDeleteWord);
			this.panel1.Controls.Add(this._btnNewWord);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 135);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(493, 34);
			this.panel1.TabIndex = 5;
			this.panel1.BackColor = DisplaySettings.Default.BackgroundColor;

			//
			// _btnNewWord
			//
			this._btnNewWord.AutoSize = true;
			this._btnNewWord.Location = new System.Drawing.Point(-4, 2);
			this._btnNewWord.Name = "_btnNewWord";
			this._btnNewWord.Size = new System.Drawing.Size(80, 24);
			this._btnNewWord.TabIndex = 0;
			this._btnNewWord.TabStop = true;
			this._btnNewWord.TextImageRelation = TextImageRelation.ImageBeforeText;
			this._btnNewWord.Text = "&New Word";
			this._btnNewWord.Image = Resources.NewWord.GetThumbnailImage(22, 22, ReturnFalse, IntPtr.Zero);
			this._btnNewWord.ImageAlign = ContentAlignment.TopLeft;
			this._btnNewWord.FlatStyle = FlatStyle.Flat;
			this._btnNewWord.FlatAppearance.MouseOverBackColor = Color.Orange;
			this._btnNewWord.FlatAppearance.BorderSize = 0;
			this._btnNewWord.Padding = new Padding();
			this._btnNewWord.Margin = new Padding();
			this._btnNewWord.TextAlign = ContentAlignment.BottomLeft;
			this._btnNewWord.Click += this._btnNewWord_LinkClicked;

			//
			// _btnDeleteWord
			//
			this._btnDeleteWord.AutoSize = true;
			this._btnDeleteWord.Location = new System.Drawing.Point(125, 2);
			this._btnDeleteWord.Name = "_btnDeleteWord";
			this._btnDeleteWord.Size = new System.Drawing.Size(120, 24);
			this._btnDeleteWord.TabIndex = 1;
			this._btnDeleteWord.TabStop = true;
			this._btnDeleteWord.TextImageRelation = TextImageRelation.ImageBeforeText;
			this._btnDeleteWord.Text = "&Delete This Word";
			this._btnDeleteWord.Image = Resources.DeleteWord.GetThumbnailImage(22, 22, ReturnFalse, IntPtr.Zero);
			this._btnDeleteWord.ImageAlign = ContentAlignment.TopLeft;
			this._btnDeleteWord.FlatStyle = FlatStyle.Flat;
			this._btnDeleteWord.FlatAppearance.MouseOverBackColor = Color.Orange;
			this._btnDeleteWord.FlatAppearance.BorderSize = 0;
			this._btnDeleteWord.Padding = new Padding();
			this._btnDeleteWord.Margin = new Padding();
			this._btnDeleteWord.TextAlign = ContentAlignment.BottomLeft;
			this._btnDeleteWord.Click += this._btnDeleteWord_LinkClicked;


			//
			// EntryDetailTask
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = DisplaySettings.Default.BackgroundColor;
			this.Controls.Add(this._recordsListBox);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.Control_EntryDetailPanel);
			this.Controls.Add(this._btnFind);
			this.Controls.Add(this._findText);
			this.Controls.Add(this._writingSystemChooser);
			this.Controls.Add(this._findWritingSystemId);
			this.Name = "EntryDetailTask";
			this.Size = new System.Drawing.Size(493, 169);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;

		}

		#endregion

		private static bool ReturnFalse()
		{
			return false;
		}
		private LexPreviewWithEntryControl _entryDetailPanel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ListBox _recordsListBox;
		private System.Windows.Forms.Button _btnDeleteWord;
		private System.Windows.Forms.Button _btnNewWord;
		private System.Windows.Forms.Button _btnFind;
		private WeSayAutoCompleteTextBox _findText;
		private Button  _writingSystemChooser;
		private Label _findWritingSystemId;

	}
}
