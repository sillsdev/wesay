using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	partial class LexFieldControl
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
				_records.ListChanged -= OnRecordsListChanged;

				_recordsListBox.SelectedIndexChanged -= OnRecordSelectionChanged;
				_completedRecordsListBox.SelectedIndexChanged -= OnCompletedRecordSelectionChanged;
				if (_currentRecord != null)
				{
					_currentRecord.PropertyChanged -= OnCurrentRecordPropertyChanged;
				}
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
			this._recordsListBox = new ListBox.BindingListGrid();
			this._completedRecordsListBox = new ListBox.BindingListGrid();
			this._completedRecordsLabel = new System.Windows.Forms.Label();
			this._lexFieldDetailPanel = new WeSay.LexicalTools.LexPreviewWithEntryControl();
			this._btnPreviousWord = new ArrowButton.ArrowButton();
			this._btnNextWord = new ArrowButton.ArrowButton();
			this.labelNextHotKey = new System.Windows.Forms.Label();
			this._congratulationsControl = new WeSay.LexicalTools.CongratulationsControl();
			this.SuspendLayout();
			//
			// _recordsListBox
			//
			this._recordsListBox.GridToolTipActive = true;
			this._recordsListBox.Location = new System.Drawing.Point(4, 5);
			this._recordsListBox.MinimumSize = new System.Drawing.Size(0, 50);
			this._recordsListBox.Name = "_recordsListBox";
			this._recordsListBox.SelectedIndex = 0;
			this._recordsListBox.Size = new System.Drawing.Size(116, 170);
			this._recordsListBox.SpecialKeys = ((SourceGrid3.GridSpecialKeys)((((((SourceGrid3.GridSpecialKeys.Arrows | SourceGrid3.GridSpecialKeys.PageDownUp)
						| SourceGrid3.GridSpecialKeys.Enter)
						| SourceGrid3.GridSpecialKeys.Escape)
						| SourceGrid3.GridSpecialKeys.Control)
						| SourceGrid3.GridSpecialKeys.Shift)));
			this._recordsListBox.StyleGrid = null;
			this._recordsListBox.TabIndex = 1;
			//
			// _completedRecordsListBox
			//
			this._completedRecordsListBox.GridToolTipActive = true;
			this._completedRecordsListBox.Location = new System.Drawing.Point(4, 220);
			this._completedRecordsListBox.MinimumSize = new System.Drawing.Size(0, 50);
			this._completedRecordsListBox.Name = "_completedRecordsListBox";
			this._completedRecordsListBox.SelectedIndex = 0;
			this._completedRecordsListBox.Size = new System.Drawing.Size(116, 170);
			this._completedRecordsListBox.SpecialKeys = ((SourceGrid3.GridSpecialKeys)((((((SourceGrid3.GridSpecialKeys.Arrows | SourceGrid3.GridSpecialKeys.PageDownUp)
						| SourceGrid3.GridSpecialKeys.Enter)
						| SourceGrid3.GridSpecialKeys.Escape)
						| SourceGrid3.GridSpecialKeys.Control)
						| SourceGrid3.GridSpecialKeys.Shift)));
			this._completedRecordsListBox.StyleGrid = null;
			this._completedRecordsListBox.TabIndex = 2;
			//
			// _completedRecordsLabel
			//
			this._completedRecordsLabel.AutoSize = true;
			this._completedRecordsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._completedRecordsLabel.Location = new System.Drawing.Point(4, 200);
			this._completedRecordsLabel.Name = "_completedRecordsLabel";
			this._completedRecordsLabel.Size = new System.Drawing.Size(70, 15);
			this._completedRecordsLabel.TabIndex = 3;
			this._completedRecordsLabel.Text = "Completed:";
			//
			// _lexFieldDetailPanel
			//
			this._lexFieldDetailPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lexFieldDetailPanel.AutoScroll = true;
			this._lexFieldDetailPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this._lexFieldDetailPanel.DataSource = null;
			this._lexFieldDetailPanel.Location = new System.Drawing.Point(126, 5);
			this._lexFieldDetailPanel.Name = "_lexFieldDetailPanel";
			this._lexFieldDetailPanel.Size = new System.Drawing.Size(367, 122);
			this._lexFieldDetailPanel.TabIndex = 0;
			//
			// _btnPreviousWord
			//
			this._btnPreviousWord.ArrowEnabled = true;
			this._btnPreviousWord.HoverEndColor = System.Drawing.Color.Blue;
			this._btnPreviousWord.HoverStartColor = System.Drawing.Color.White;
			this._btnPreviousWord.Location = new System.Drawing.Point(422, 369);
			this._btnPreviousWord.Name = "_btnPreviousWord";
			this._btnPreviousWord.NormalEndColor = System.Drawing.Color.White; ;
			this._btnPreviousWord.NormalStartColor = System.Drawing.Color.White;
			this._btnPreviousWord.Rotation = 270;
			this._btnPreviousWord.Size = new System.Drawing.Size(24, 24);
			this._btnPreviousWord.StubbyStyle = false;
			this._btnPreviousWord.TabIndex = 4;
			this._btnPreviousWord.TabStop = false;
			this._btnPreviousWord.Click += new System.EventHandler(this.OnBtnPreviousWordClick);
			//
			// _btnNextWord
			//
			this._btnNextWord.ArrowEnabled = true;
			this._btnNextWord.HoverEndColor = System.Drawing.Color.Blue;
			this._btnNextWord.HoverStartColor = System.Drawing.Color.White;
			this._btnNextWord.Location = new System.Drawing.Point(446, 360);
			this._btnNextWord.Name = "_btnNextWord";
			this._btnNextWord.NormalEndColor = System.Drawing.Color.White; ;
			this._btnNextWord.NormalStartColor = System.Drawing.Color.White;
			this._btnNextWord.Rotation = 90;
			this._btnNextWord.Size = new System.Drawing.Size(43, 43);
			this._btnNextWord.StubbyStyle = false;
			this._btnNextWord.TabIndex = 5;
			this._btnNextWord.TabStop = false;
			this._btnNextWord.Click += new System.EventHandler(this.OnBtnNextWordClick);
			//
			// labelNextHotKey
			//
			this.labelNextHotKey.AutoSize = true;
			this.labelNextHotKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNextHotKey.ForeColor = System.Drawing.Color.DarkGray;
			this.labelNextHotKey.Location = new System.Drawing.Point(495, 372);
			this.labelNextHotKey.Name = "labelNextHotKey";
			this.labelNextHotKey.Size = new System.Drawing.Size(102, 15);
			this.labelNextHotKey.TabIndex = 6;
			this.labelNextHotKey.Text = "(Page Down Key)";
			//
			// _congratulationsControl
			//
			this._congratulationsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._congratulationsControl.Location = new System.Drawing.Point(126, 9);
			this._congratulationsControl.Name = "_congratulationsControl";
			this._congratulationsControl.Size = new System.Drawing.Size(367, 144);
			this._congratulationsControl.TabIndex = 8;
			//
			// LexFieldControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._recordsListBox);
			this.Controls.Add(this._completedRecordsListBox);
			this.Controls.Add(this._lexFieldDetailPanel);
			this.Controls.Add(this._completedRecordsLabel);
			this.Controls.Add(this._btnPreviousWord);
			this.Controls.Add(this._btnNextWord);
			this.Controls.Add(this.labelNextHotKey);
			this.Controls.Add(this._congratulationsControl);
			this.Name = "LexFieldControl";
			this.Size = new System.Drawing.Size(493, 169);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private LexPreviewWithEntryControl _lexFieldDetailPanel;
		private ListBox.BindingListGrid _recordsListBox;
		private ListBox.BindingListGrid _completedRecordsListBox;
		private System.Windows.Forms.Label _completedRecordsLabel;
		private ArrowButton.ArrowButton _btnNextWord;
		private ArrowButton.ArrowButton _btnPreviousWord;
		private Label labelNextHotKey;
		private CongratulationsControl _congratulationsControl;

	}
}
