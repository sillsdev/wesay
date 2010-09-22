using System.Windows.Forms;
using WeSay.UI;
using WeSay.UI.Buttons;

namespace WeSay.LexicalTools.AddMissingInfo
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
				//_records.ListChanged -= OnRecordsListChanged;

				_recordsListBox.SelectedIndexChanged -= OnRecordSelectionChanged;
				//_recordsListBox.Enter += _recordsListBox_Enter;
				//_recordsListBox.Leave += _recordsListBox_Leave;
				//_recordsListBox.DataSource = null; // without this, the currency manager keeps trying to work

				_completedRecordsListBox.SelectedIndexChanged -= OnCompletedRecordSelectionChanged;
				//_completedRecordsListBox.Enter += _completedRecordsListBox_Enter;
				//_completedRecordsListBox.Leave += _completedRecordsListBox_Leave;

			   // Debug.Assert(_recordsListBox.BindingContext.Contains(_records) == false);
				//if (_recordsListBox.BindingContext != null)
				//{
				//    ((CurrencyManager) _recordsListBox.BindingContext[_records]).SuspendBinding();
				//    _recordsListBox.BindingContext = new BindingContext();
				//}


				if (CurrentEntry != null)
				{
					CurrentEntry.PropertyChanged -= OnCurrentRecordPropertyChanged;
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
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.label1 = new System.Windows.Forms.Label();
			this._recordsListBox = new WeSay.UI.WeSayListView();
			this._completedRecordsLabel = new System.Windows.Forms.Label();
			this._completedRecordsListBox = new WeSay.UI.WeSayListView();
			this._buttonPanel = new System.Windows.Forms.Panel();
			this.labelNextHotKey = new System.Windows.Forms.Label();
			this._btnNextWord = new WeSay.UI.Buttons.NextButton();
			this._btnPreviousWord = new WeSay.UI.Buttons.PreviousButton();
			this._entryViewControl = new WeSay.LexicalTools.EntryViewControl();
			this._congratulationsControl = new WeSay.LexicalTools.CongratulationsControl();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this._buttonPanel.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this._buttonPanel);
			this.splitContainer1.Panel2.Controls.Add(this._entryViewControl);
			this.splitContainer1.Panel2.Controls.Add(this._congratulationsControl);
			this.splitContainer1.Size = new System.Drawing.Size(641, 407);
			this.splitContainer1.SplitterDistance = 126;
			this.splitContainer1.TabIndex = 9;
			this.splitContainer1.TabStop = false;
			//
			// splitContainer2
			//
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			//
			// splitContainer2.Panel1
			//
			this.splitContainer2.Panel1.Controls.Add(this.tableLayoutPanel1);
			//
			// splitContainer2.Panel2
			//
			this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel2);
			this.splitContainer2.Size = new System.Drawing.Size(126, 407);
			this.splitContainer2.SplitterDistance = 178;
			this.splitContainer2.TabIndex = 10;
			this.splitContainer2.TabStop = false;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 16);
			this.label1.TabIndex = 11;
			this.label1.Text = "To Do:";
			//
			// _recordsListBox
			//
			this._recordsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._recordsListBox.Location = new System.Drawing.Point(3, 19);
			this._recordsListBox.MinimumSize = new System.Drawing.Size(4, 50);
			this._recordsListBox.Name = "_recordsListBox";
			this._recordsListBox.Size = new System.Drawing.Size(120, 156);
			this._recordsListBox.TabIndex = 2;
			this._recordsListBox.View = System.Windows.Forms.View.SmallIcon;
			//
			// _completedRecordsLabel
			//
			this._completedRecordsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._completedRecordsLabel.AutoSize = true;
			this._completedRecordsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this._completedRecordsLabel.Location = new System.Drawing.Point(3, 0);
			this._completedRecordsLabel.Name = "_completedRecordsLabel";
			this._completedRecordsLabel.Size = new System.Drawing.Size(120, 16);
			this._completedRecordsLabel.TabIndex = 0;
			this._completedRecordsLabel.Text = "Completed:";
			//
			// _completedRecordsListBox
			//
			this._completedRecordsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._completedRecordsListBox.Location = new System.Drawing.Point(3, 19);
			this._completedRecordsListBox.MinimumSize = new System.Drawing.Size(4, 50);
			this._completedRecordsListBox.Name = "_completedRecordsListBox";
			this._completedRecordsListBox.Size = new System.Drawing.Size(120, 203);
			this._completedRecordsListBox.TabIndex = 3;
			this._completedRecordsListBox.View = System.Windows.Forms.View.SmallIcon;
			//
			// _buttonPanel
			//
			this._buttonPanel.Controls.Add(this.labelNextHotKey);
			this._buttonPanel.Controls.Add(this._btnNextWord);
			this._buttonPanel.Controls.Add(this._btnPreviousWord);
			this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._buttonPanel.Location = new System.Drawing.Point(0, 355);
			this._buttonPanel.Name = "_buttonPanel";
			this._buttonPanel.Size = new System.Drawing.Size(511, 52);
			this._buttonPanel.TabIndex = 1;
			//
			// labelNextHotKey
			//
			this.labelNextHotKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNextHotKey.AutoSize = true;
			this.labelNextHotKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.labelNextHotKey.ForeColor = System.Drawing.Color.DarkGray;
			this.labelNextHotKey.Location = new System.Drawing.Point(379, 14);
			this.labelNextHotKey.Name = "labelNextHotKey";
			this.labelNextHotKey.Size = new System.Drawing.Size(112, 16);
			this.labelNextHotKey.TabIndex = 2;
			this.labelNextHotKey.Text = "(Page Down Key)";
			//
			// _btnNextWord
			//
			this._btnNextWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnNextWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnNextWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnNextWord.Image")));
			this._btnNextWord.Location = new System.Drawing.Point(325, 0);
			this._btnNextWord.Name = "_btnNextWord";
			this._btnNextWord.Size = new System.Drawing.Size(50, 50);
			this._btnNextWord.TabIndex = 1;
			this._btnNextWord.TabStop = false;
			this._btnNextWord.Click += new System.EventHandler(this.OnBtnNextWordClick);
			//
			// _btnPreviousWord
			//
			this._btnPreviousWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnPreviousWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnPreviousWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnPreviousWord.Image")));
			this._btnPreviousWord.Location = new System.Drawing.Point(289, 10);
			this._btnPreviousWord.Name = "_btnPreviousWord";
			this._btnPreviousWord.Size = new System.Drawing.Size(30, 30);
			this._btnPreviousWord.TabIndex = 0;
			this._btnPreviousWord.TabStop = false;
			this._btnPreviousWord.Click += new System.EventHandler(this.OnBtnPreviousWordClick);
			//
			// _entryViewControl
			//
			this._entryViewControl.DataSource = null;
			this._entryViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._entryViewControl.Location = new System.Drawing.Point(0, 0);
			this._entryViewControl.Margin = new System.Windows.Forms.Padding(0);
			this._entryViewControl.Name = "_entryViewControl";
			this._entryViewControl.ShowNormallyHiddenFields = false;
			this._entryViewControl.Size = new System.Drawing.Size(511, 407);
			this._entryViewControl.TabIndex = 0;
			//
			// _congratulationsControl
			//
			this._congratulationsControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._congratulationsControl.Location = new System.Drawing.Point(0, 0);
			this._congratulationsControl.Name = "_congratulationsControl";
			this._congratulationsControl.Size = new System.Drawing.Size(511, 407);
			this._congratulationsControl.TabIndex = 9;
			//
			// tableLayoutPanel1
			//
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._recordsListBox, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(126, 178);
			this.tableLayoutPanel1.TabIndex = 12;
			//
			// tableLayoutPanel2
			//
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this._completedRecordsLabel, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._completedRecordsListBox, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(126, 225);
			this.tableLayoutPanel2.TabIndex = 4;
			//
			// MissingInfoControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.splitContainer1);
			this.Name = "MissingInfoControl";
			this.Size = new System.Drawing.Size(641, 407);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this._buttonPanel.ResumeLayout(false);
			this._buttonPanel.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer splitContainer1;
		private SplitContainer splitContainer2;

		internal WeSayListView _recordsListBox;
		private EntryViewControl _entryViewControl;
		private CongratulationsControl _congratulationsControl;
		private Label label1;
		private Label _completedRecordsLabel;
		internal WeSayListView _completedRecordsListBox;
		private NextButton _btnNextWord;
		private Label labelNextHotKey;
		private PreviousButton _btnPreviousWord;
		private Panel _buttonPanel;
		private TableLayoutPanel tableLayoutPanel1;
		private TableLayoutPanel tableLayoutPanel2;

	}
}
