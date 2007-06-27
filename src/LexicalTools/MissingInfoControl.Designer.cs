using System.Diagnostics;
using System.Windows.Forms;
using WeSay.UI;
using WeSay.UI.Buttons;

namespace WeSay.LexicalTools
{
	partial class MissingInfoControl
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
				_recordsListBox.Enter += _recordsListBox_Enter;
				_recordsListBox.Leave += _recordsListBox_Leave;
				_recordsListBox.DataSource = null; // without this, the currency manager keeps trying to work

				_completedRecordsListBox.SelectedIndexChanged -= OnCompletedRecordSelectionChanged;
				_completedRecordsListBox.Enter += _completedRecordsListBox_Enter;
				_completedRecordsListBox.Leave += _completedRecordsListBox_Leave;

			   // Debug.Assert(_recordsListBox.BindingContext.Contains(_records) == false);
				((CurrencyManager) _recordsListBox.BindingContext[_records]).SuspendBinding();
				_recordsListBox.BindingContext = new BindingContext();


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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MissingInfoControl));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._congratulationsControl = new WeSay.LexicalTools.CongratulationsControl();
			this._entryViewControl = new WeSay.LexicalTools.EntryViewControl();
			this._recordsListBox = new WeSay.UI.WeSayListBox();
			this._completedRecordsListBox = new WeSay.UI.WeSayListBox();
			this._completedRecordsLabel = new WeSay.UI.LocalizableLabel();
			this._btnPreviousWord = new WeSay.UI.Buttons.PreviousButton();
			this._btnNextWord = new WeSay.UI.Buttons.NextButton();
			this.labelNextHotKey = new WeSay.UI.LocalizableLabel();
			this.label1 = new WeSay.UI.LocalizableLabel();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			//
			// splitContainer1
			//
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this._completedRecordsLabel);
			this.splitContainer1.Panel1.Controls.Add(this._completedRecordsListBox);
			this.splitContainer1.Panel1.Controls.Add(this._recordsListBox);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this._btnNextWord);
			this.splitContainer1.Panel2.Controls.Add(this.labelNextHotKey);
			this.splitContainer1.Panel2.Controls.Add(this._btnPreviousWord);
			this.splitContainer1.Panel2.Controls.Add(this._entryViewControl);
			this.splitContainer1.Panel2.Controls.Add(this._congratulationsControl);
			this.splitContainer1.Size = new System.Drawing.Size(641, 407);
			this.splitContainer1.SplitterDistance = 153;
			this.splitContainer1.TabIndex = 9;
			//
			// _congratulationsControl
			//
			this._congratulationsControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._congratulationsControl.Location = new System.Drawing.Point(0, 0);
			this._congratulationsControl.Name = "_congratulationsControl";
			this._congratulationsControl.Size = new System.Drawing.Size(484, 407);
			this._congratulationsControl.TabIndex = 9;
			//
			// _entryViewControl
			//
			this._entryViewControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._entryViewControl.AutoScroll = true;
			this._entryViewControl.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this._entryViewControl.DataSource = null;
			this._entryViewControl.Location = new System.Drawing.Point(-1, 3);
			this._entryViewControl.Name = "_entryViewControl";
			this._entryViewControl.Size = new System.Drawing.Size(482, 345);
			this._entryViewControl.TabIndex = 10;
			//
			// _recordsListBox
			//
			this._recordsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._recordsListBox.Location = new System.Drawing.Point(0, 21);
			this._recordsListBox.MinimumSize = new System.Drawing.Size(4, 50);
			this._recordsListBox.Name = "_recordsListBox";
			this._recordsListBox.Size = new System.Drawing.Size(150, 173);
			this._recordsListBox.TabIndex = 2;
			//
			// _completedRecordsListBox
			//
			this._completedRecordsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._completedRecordsListBox.Location = new System.Drawing.Point(0, 255);
			this._completedRecordsListBox.MinimumSize = new System.Drawing.Size(4, 50);
			this._completedRecordsListBox.Name = "_completedRecordsListBox";
			this._completedRecordsListBox.Size = new System.Drawing.Size(150, 147);
			this._completedRecordsListBox.TabIndex = 3;
			//
			// _completedRecordsLabel
			//
			this._completedRecordsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._completedRecordsLabel.AutoSize = true;
			this._completedRecordsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._completedRecordsLabel.Location = new System.Drawing.Point(3, 229);
			this._completedRecordsLabel.Name = "_completedRecordsLabel";
			this._completedRecordsLabel.Size = new System.Drawing.Size(70, 15);
			this._completedRecordsLabel.TabIndex = 4;
			this._completedRecordsLabel.Text = "Completed:";
			//
			// _btnPreviousWord
			//
			this._btnPreviousWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnPreviousWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnPreviousWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnPreviousWord.Image")));
			this._btnPreviousWord.Location = new System.Drawing.Point(289, 364);
			this._btnPreviousWord.Name = "_btnPreviousWord";
			this._btnPreviousWord.Size = new System.Drawing.Size(30, 30);
			this._btnPreviousWord.TabIndex = 11;
			this._btnPreviousWord.TabStop = false;
			this._btnPreviousWord.Click += new System.EventHandler(this.OnBtnPreviousWordClick);
			//
			// _btnNextWord
			//
			this._btnNextWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnNextWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnNextWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnNextWord.Image")));
			this._btnNextWord.Location = new System.Drawing.Point(325, 354);
			this._btnNextWord.Name = "_btnNextWord";
			this._btnNextWord.Size = new System.Drawing.Size(50, 50);
			this._btnNextWord.TabIndex = 12;
			this._btnNextWord.TabStop = false;
			this._btnNextWord.Click += new System.EventHandler(this.OnBtnNextWordClick);
			//
			// labelNextHotKey
			//
			this.labelNextHotKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNextHotKey.AutoSize = true;
			this.labelNextHotKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNextHotKey.ForeColor = System.Drawing.Color.DarkGray;
			this.labelNextHotKey.Location = new System.Drawing.Point(379, 371);
			this.labelNextHotKey.Name = "labelNextHotKey";
			this.labelNextHotKey.Size = new System.Drawing.Size(102, 15);
			this.labelNextHotKey.TabIndex = 13;
			this.labelNextHotKey.Text = "~(Page Down Key)";
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 15);
			this.label1.TabIndex = 4;
			this.label1.Text = "To Do:";
			//
			// MissingInfoControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "MissingInfoControl";
			this.Size = new System.Drawing.Size(641, 407);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer splitContainer1;
		private WeSayListBox _recordsListBox;
		private EntryViewControl _entryViewControl;
		private CongratulationsControl _congratulationsControl;
		private Label label1;
		private Label _completedRecordsLabel;
		private WeSayListBox _completedRecordsListBox;
		private NextButton _btnNextWord;
		private Label labelNextHotKey;
		private PreviousButton _btnPreviousWord;

	}
}
