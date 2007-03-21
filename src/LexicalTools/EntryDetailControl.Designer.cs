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
		  this._entryDetailControl = new WeSay.LexicalTools.LexPreviewWithEntryControl();
		  this.panelWordList = new Panel();
		  this.panelDetail = new Panel();
		  this.panelNew = new System.Windows.Forms.Panel();
		  this.panelDelete = new System.Windows.Forms.Panel();
		  this._btnNewWord = new System.Windows.Forms.Button();
		  this._btnDeleteWord = new System.Windows.Forms.Button();
		  this._recordsListBox = new System.Windows.Forms.ListBox();
		  this._btnFind = new System.Windows.Forms.Button();
		  this._findText = new WeSay.UI.WeSayAutoCompleteTextBox();
		  this._findWritingSystemId = new System.Windows.Forms.Label();
		  this._writingSystemChooser = new System.Windows.Forms.Button();
		  this._splitter = new CollapsibleSplitter();
		  this.panelNew.SuspendLayout();
		  this.panelDelete.SuspendLayout();
		  this.panelWordList.SuspendLayout();
		  this.panelDetail.SuspendLayout();
		  this.SuspendLayout();
		  //
		  // _entryDetailControl
		  //
		  this._entryDetailControl.AutoScroll = true;
		  this._entryDetailControl.DataSource = null;
		  this._entryDetailControl.Location = new System.Drawing.Point(3, 0);
		  this._entryDetailControl.Name = "_entryDetailPanel";
		  this._entryDetailControl.Size = new System.Drawing.Size(339, 254);
		  this._entryDetailControl.TabIndex = 0;
		  this._entryDetailControl.Dock = DockStyle.Fill;
		  //
		  // panelNew
		  //
		  this.panelNew.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		  this.panelNew.Controls.Add(this._btnNewWord);
		  this.panelNew.Location = new System.Drawing.Point(0, 260);
		  this.panelNew.Name = "panelNew";
		  this.panelNew.Size = new System.Drawing.Size(162, 34);
		  this.panelNew.Dock = DockStyle.Bottom;
		  this.panelNew.TabIndex = 5;
		  //
		  // _btnNewWord
		  //
		  this._btnNewWord.AutoSize = true;
		  this._btnNewWord.FlatAppearance.BorderSize = 0;
		  this._btnNewWord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
		  this._btnNewWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		  this._btnNewWord.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
		  this._btnNewWord.Location = new System.Drawing.Point(-4, 2);
		  this._btnNewWord.Margin = new System.Windows.Forms.Padding(0);
		  this._btnNewWord.Name = "_btnNewWord";
		  this._btnNewWord.Size = new System.Drawing.Size(80, 24);
		  this._btnNewWord.TabIndex = 0;
		  this._btnNewWord.Text = "&New Word";
		  this._btnNewWord.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
		  this._btnNewWord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		  this._btnNewWord.Click += _btnNewWord_Click;
		  //
		  // panelDelete
		  //
		  this.panelDelete.Controls.Add(this._btnDeleteWord);
		  this.panelDelete.Dock = System.Windows.Forms.DockStyle.Bottom;
		  this.panelDelete.Location = new System.Drawing.Point(0, 260);
		  this.panelDelete.Name = "panelDelete";
		  this.panelDelete.Size = new System.Drawing.Size(347, 34);
		  this.panelDelete.TabIndex = 5;
		  //
		  // _btnDeleteWord
		  //
		  this._btnDeleteWord.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		  this._btnDeleteWord.AutoSize = true;
		  this._btnDeleteWord.FlatAppearance.BorderSize = 0;
		  this._btnDeleteWord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
		  this._btnDeleteWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		  this._btnDeleteWord.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
		  this._btnDeleteWord.Location = new System.Drawing.Point(-1, 2);
		  this._btnDeleteWord.Margin = new System.Windows.Forms.Padding(0);
		  this._btnDeleteWord.Name = "_btnDeleteWord";
		  this._btnDeleteWord.Size = new System.Drawing.Size(120, 24);
		  this._btnDeleteWord.TabIndex = 1;
		  this._btnDeleteWord.Text = "&Delete This Word";
		  this._btnDeleteWord.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
		  this._btnDeleteWord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
		  this._btnDeleteWord.Click += new System.EventHandler(this._btnDeleteWord_Click);
		  //
		  // _recordsListBox
		  //
		  this._recordsListBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
					  | System.Windows.Forms.AnchorStyles.Left)
					  | System.Windows.Forms.AnchorStyles.Right)));
		  this._recordsListBox.IntegralHeight = false;
		  this._recordsListBox.Location = new System.Drawing.Point(3, 23);
		  this._recordsListBox.Name = "_recordsListBox";
		  this._recordsListBox.Size = new System.Drawing.Size(136, 231);
		  this._recordsListBox.TabIndex = 4;
		  //
		  // _btnFind
		  //
		  this._btnFind.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		  this._btnFind.BackColor = this._findText.BackColor;
		  this._btnFind.FlatAppearance.BorderSize = 0;
		  this._btnFind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
		  this._btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		  this._btnFind.Location = new System.Drawing.Point(105, 3);
		  this._btnFind.Name = "_btnFind";
		  this._btnFind.Size = new System.Drawing.Size(19, 19);
		  this._btnFind.TabIndex = 2;
		  this._btnFind.UseVisualStyleBackColor = false;
		  this._btnFind.Click += new System.EventHandler(this._btnFind_Click);

		  //
		  // _findText
		  //
		  this._findText.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
					  | System.Windows.Forms.AnchorStyles.Right)));
		  this._findText.BackColor = System.Drawing.Color.White;
		  this._findText.BorderStyle = System.Windows.Forms.BorderStyle.None;
		  this._findText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
		  this._findText.Location = new System.Drawing.Point(24, 3);
		  this._findText.Name = "_findText";
		  this._findText.PopupBorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		  this._findText.PopupOffset = new System.Drawing.Point(0, 0);
		  this._findText.PopupSelectionBackColor = System.Drawing.SystemColors.Highlight;
		  this._findText.PopupSelectionForeColor = System.Drawing.SystemColors.HighlightText;
		  this._findText.PopupWidth = 118;
		  this._findText.Size = new System.Drawing.Size(81, 19);
		  this._findText.TabIndex = 1;
		  this._findText.AutoCompleteChoiceSelected += new System.EventHandler(this._findText_AutoCompleteChoiceSelected);

		  //
		  // _findWritingSystemId
		  //
		  this._findWritingSystemId.BackColor = System.Drawing.Color.White;
		  this._findWritingSystemId.ForeColor = System.Drawing.Color.LightGray;
		  this._findWritingSystemId.Location = new System.Drawing.Point(4, 3);
		  this._findWritingSystemId.Name = "_findWritingSystemId";
		  this._findWritingSystemId.Size = new System.Drawing.Size(20, 19);
		  this._findWritingSystemId.TabIndex = 6;
		  this._findWritingSystemId.Text = "en";
		  this._findWritingSystemId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		  this._findWritingSystemId.MouseClick += new System.Windows.Forms.MouseEventHandler(this._findWritingSystemId_MouseClick);

		  //
		  // _writingSystemChooser
		  //
		  this._writingSystemChooser.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		  this._writingSystemChooser.BackColor = this._findText.BackColor;
		  this._writingSystemChooser.FlatAppearance.BorderSize = 0;
		  this._writingSystemChooser.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
		  this._writingSystemChooser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		  this._writingSystemChooser.Location = new System.Drawing.Point(124, 3);
		  this._writingSystemChooser.Name = "_writingSystemChooser";
		  this._writingSystemChooser.Size = new System.Drawing.Size(15, 19);
		  this._writingSystemChooser.TabIndex = 3;
		  this._writingSystemChooser.UseVisualStyleBackColor = false;
		  this._writingSystemChooser.Click += new System.EventHandler(this._writingSystemChooser_Click);
		  //
		  // panelWordList
		  //
		  this.panelWordList.Controls.Add(this._btnFind);
		  this.panelWordList.Controls.Add(this._findText);
		  this.panelWordList.Controls.Add(this._writingSystemChooser);
		  this.panelWordList.Controls.Add(this._findWritingSystemId);
		  this.panelWordList.Controls.Add(this._recordsListBox);
		  this.panelWordList.Controls.Add(this.panelNew);
		  this.panelWordList.Dock = DockStyle.Left;
		  this.panelWordList.Name = "panelWordList";
		  this.panelWordList.Size = new System.Drawing.Size(140, 294);
		  this.panelWordList.BorderStyle = BorderStyle.None;
		  //
		  // _splitter
		  //
		  this._splitter.Location = new System.Drawing.Point(140, 0);
		  this._splitter.Name = "_splitter";
		  this._splitter.GripLength = 90;
		  this._splitter.MinSize = 50;
		  this._splitter.ControlToHide = this.panelWordList;
		  this._splitter.Dock = DockStyle.Left;
		  this._splitter.VisualStyle = VisualStyles.DoubleDots;
		  this._splitter.BorderStyle3D = Border3DStyle.Raised;
		  this._splitter.Width = 10;
		  this._splitter.Height = 140;

		  //
		  // panelDetail
		  //
		  this.panelDetail.Controls.Add(this._entryDetailControl);
		  this.panelDetail.Controls.Add(this.panelDelete);
		  this.panelDetail.Dock = DockStyle.Fill;
		  this.panelDetail.Name = "panelDetail";
		  this.panelDetail.Size = new System.Drawing.Size(339, 294);
		  this.panelDetail.Location = new Point(168, 0);
		  //
		  // EntryDetailControl
		  //
		  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		  this.Controls.Add(this.panelDetail);
		  this.Controls.Add(this._splitter);
		  this.Controls.Add(this.panelWordList);
		  this.Name = "EntryDetailControl";
		  this.Size = new System.Drawing.Size(493, 294);
		  this.panelNew.ResumeLayout(false);
		  this.panelNew.PerformLayout();
		  this.panelDelete.ResumeLayout(false);
		  this.panelDelete.PerformLayout();
		  this.panelWordList.ResumeLayout(false);
		  this.panelWordList.PerformLayout();
		  this.panelDetail.ResumeLayout(false);
		  this.panelDetail.ResumeLayout(false);
		  this.ResumeLayout(false);
		}

		#endregion

		private static bool ReturnFalse()
		{
			return false;
		}
		private LexPreviewWithEntryControl _entryDetailControl;
		private Panel panelWordList;
		private Panel panelDetail;
		private Panel panelNew;
		private Panel panelDelete;
		private System.Windows.Forms.ListBox _recordsListBox;
		private Button _btnDeleteWord;
		private Button _btnNewWord;
		private Button _btnFind;
		private WeSayAutoCompleteTextBox _findText;
		private Button  _writingSystemChooser;
		private Label _findWritingSystemId;
		private CollapsibleSplitter _splitter;

	}
}
