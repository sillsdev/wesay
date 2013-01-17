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

				_todoRecordsListBox.ItemSelectionChanged -= OnTodoRecordSelectionChanged;
				_completedRecordsListBox.ItemSelectionChanged -= OnCompletedRecordSelectionChanged;

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
			this._todoBox = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this._todoRecordsListBox = new WeSay.UI.WeSayListView();
			this._completedBox = new System.Windows.Forms.TableLayoutPanel();
			this._completedRecordsLabel = new System.Windows.Forms.Label();
			this._completedRecordsListBox = new WeSay.UI.WeSayListView();
			this._entryViewAndButtons = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._btnPreviousWord = new WeSay.UI.Buttons.PreviousButton();
			this._btnNextWord = new WeSay.UI.Buttons.NextButton();
			this.labelNextHotKey = new System.Windows.Forms.Label();
			this._entryViewControl = new WeSay.LexicalTools.EntryViewControl();
			this._congratulationsControl = new WeSay.LexicalTools.CongratulationsControl();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this._todoBox.SuspendLayout();
			this._completedBox.SuspendLayout();
			this._entryViewAndButtons.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
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
			this.splitContainer1.Panel2.Controls.Add(this._entryViewAndButtons);
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
			this.splitContainer2.Panel1.Controls.Add(this._todoBox);
			//
			// splitContainer2.Panel2
			//
			this.splitContainer2.Panel2.Controls.Add(this._completedBox);
			this.splitContainer2.Size = new System.Drawing.Size(126, 407);
			this.splitContainer2.SplitterDistance = 178;
			this.splitContainer2.TabIndex = 10;
			this.splitContainer2.TabStop = false;
			//
			// _todoBox
			//
			this._todoBox.ColumnCount = 1;
			this._todoBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._todoBox.Controls.Add(this.label1, 0, 0);
			this._todoBox.Controls.Add(this._todoRecordsListBox, 0, 1);
			this._todoBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._todoBox.Location = new System.Drawing.Point(0, 0);
			this._todoBox.Name = "_todoBox";
			this._todoBox.RowCount = 2;
			this._todoBox.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._todoBox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._todoBox.Size = new System.Drawing.Size(126, 178);
			this._todoBox.TabIndex = 12;
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
			// _todoRecordsListBox
			//
			this._todoRecordsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._todoRecordsListBox.Location = new System.Drawing.Point(3, 19);
			this._todoRecordsListBox.MinimumSize = new System.Drawing.Size(4, 50);
			this._todoRecordsListBox.Name = "_todoRecordsListBox";
			this._todoRecordsListBox.Size = new System.Drawing.Size(120, 156);
			this._todoRecordsListBox.TabIndex = 2;
			this._todoRecordsListBox.View = System.Windows.Forms.View.SmallIcon;
			//
			// _completedBox
			//
			this._completedBox.ColumnCount = 1;
			this._completedBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._completedBox.Controls.Add(this._completedRecordsLabel, 0, 0);
			this._completedBox.Controls.Add(this._completedRecordsListBox, 0, 1);
			this._completedBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._completedBox.Location = new System.Drawing.Point(0, 0);
			this._completedBox.Name = "_completedBox";
			this._completedBox.RowCount = 2;
			this._completedBox.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._completedBox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._completedBox.Size = new System.Drawing.Size(126, 225);
			this._completedBox.TabIndex = 4;
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
			// _entryViewAndButtons
			//
			this._entryViewAndButtons.ColumnCount = 1;
			this._entryViewAndButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._entryViewAndButtons.Controls.Add(this.flowLayoutPanel1, 0, 1);
			this._entryViewAndButtons.Controls.Add(this._entryViewControl, 0, 0);
			this._entryViewAndButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this._entryViewAndButtons.Location = new System.Drawing.Point(0, 0);
			this._entryViewAndButtons.Name = "_entryViewAndButtons";
			this._entryViewAndButtons.RowCount = 2;
			this._entryViewAndButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._entryViewAndButtons.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._entryViewAndButtons.Size = new System.Drawing.Size(511, 407);
			this._entryViewAndButtons.TabIndex = 11;
			//
			// flowLayoutPanel1
			//
			this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._btnPreviousWord);
			this.flowLayoutPanel1.Controls.Add(this._btnNextWord);
			this.flowLayoutPanel1.Controls.Add(this.labelNextHotKey);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(150, 348);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(210, 56);
			this.flowLayoutPanel1.TabIndex = 10;
			//
			// _btnPreviousWord
			//
			this._btnPreviousWord.Anchor = System.Windows.Forms.AnchorStyles.None;
			this._btnPreviousWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnPreviousWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnPreviousWord.Image")));
			this._btnPreviousWord.Location = new System.Drawing.Point(3, 13);
			this._btnPreviousWord.Name = "_btnPreviousWord";
			this._btnPreviousWord.Size = new System.Drawing.Size(30, 30);
			this._btnPreviousWord.TabIndex = 0;
			this._btnPreviousWord.TabStop = false;
			this._btnPreviousWord.Click += new System.EventHandler(this.OnBtnPreviousWordClick);
			//
			// _btnNextWord
			//
			this._btnNextWord.Anchor = System.Windows.Forms.AnchorStyles.None;
			this._btnNextWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnNextWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnNextWord.Image")));
			this._btnNextWord.Location = new System.Drawing.Point(39, 3);
			this._btnNextWord.Name = "_btnNextWord";
			this._btnNextWord.Size = new System.Drawing.Size(50, 50);
			this._btnNextWord.TabIndex = 1;
			this._btnNextWord.TabStop = false;
			this._btnNextWord.Click += new System.EventHandler(this.OnBtnNextWordClick);
			//
			// labelNextHotKey
			//
			this.labelNextHotKey.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelNextHotKey.AutoSize = true;
			this.labelNextHotKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.labelNextHotKey.ForeColor = System.Drawing.Color.DarkGray;
			this.labelNextHotKey.Location = new System.Drawing.Point(95, 20);
			this.labelNextHotKey.Name = "labelNextHotKey";
			this.labelNextHotKey.Size = new System.Drawing.Size(112, 16);
			this.labelNextHotKey.TabIndex = 2;
			this.labelNextHotKey.Text = "(Page Down Key)";
			//
			// _entryViewControl
			//
			this._entryViewControl.DataSource = null;
			this._entryViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._entryViewControl.Location = new System.Drawing.Point(0, 0);
			this._entryViewControl.Margin = new System.Windows.Forms.Padding(0);
			this._entryViewControl.Name = "_entryViewControl";
			this._entryViewControl.ShowNormallyHiddenFields = false;
			this._entryViewControl.Size = new System.Drawing.Size(511, 345);
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
			this._todoBox.ResumeLayout(false);
			this._todoBox.PerformLayout();
			this._completedBox.ResumeLayout(false);
			this._completedBox.PerformLayout();
			this._entryViewAndButtons.ResumeLayout(false);
			this._entryViewAndButtons.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer splitContainer1;
		private SplitContainer splitContainer2;

		internal WeSayListView _todoRecordsListBox;
		private EntryViewControl _entryViewControl;
		private CongratulationsControl _congratulationsControl;
		private Label label1;
		private Label _completedRecordsLabel;
		internal WeSayListView _completedRecordsListBox;
		private NextButton _btnNextWord;
		private Label labelNextHotKey;
		private PreviousButton _btnPreviousWord;
		private TableLayoutPanel _todoBox;
		private TableLayoutPanel _completedBox;
		private FlowLayoutPanel flowLayoutPanel1;
		private TableLayoutPanel _entryViewAndButtons;

	}
}
