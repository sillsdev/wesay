using System.Windows.Forms;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;
using WeSay.UI.TextBoxes;

namespace WeSay.LexicalTools.DictionaryBrowseAndEdit
{
	partial class DictionaryControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;



		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryControl));
			this._searchAndListBoxTable = new System.Windows.Forms.TableLayoutPanel();
			this._recordsListBox = WeSayWordsProject.Project.ServiceLocator.GetService(typeof(IWeSayListView)) as IWeSayListView;
			this._searchTextBoxControl = new WeSay.LexicalTools.DictionaryBrowseAndEdit.SearchBoxControl();
			this._showAllFieldsToggleButton = new System.Windows.Forms.Button();
			this._btnDeleteWord = new System.Windows.Forms.Button();
			this._btnNewWord = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._bottomButtonsAndRestTable = new System.Windows.Forms.TableLayoutPanel();
			this._bottomButtonTable = new System.Windows.Forms.TableLayoutPanel();
			this._listBoxAndSplitterAndDictionaryViewTable = new System.Windows.Forms.Panel();
			this.panelDetail = new System.Windows.Forms.Panel();
			this._splitter = new WeSay.UI.CollapsibleSplitter();
			this._searchAndListBoxTable.SuspendLayout();
			this._bottomButtonsAndRestTable.SuspendLayout();
			this._bottomButtonTable.SuspendLayout();
			this._listBoxAndSplitterAndDictionaryViewTable.SuspendLayout();
			this._splitter.SuspendLayout();
			this.panelDetail.SuspendLayout();
			this.SuspendLayout();
			//
			// _searchAndListBoxTable
			//
			this._searchAndListBoxTable.ColumnCount = 1;
			this._searchAndListBoxTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._searchAndListBoxTable.Controls.Add((Control)this._recordsListBox, 0, 1);
			this._searchAndListBoxTable.Controls.Add(this._searchTextBoxControl, 0, 0);
			this._searchAndListBoxTable.Dock = System.Windows.Forms.DockStyle.Left;
			this._searchAndListBoxTable.Location = new System.Drawing.Point(0, 0);
			this._searchAndListBoxTable.Name = "_searchAndListBoxTable";
			this._searchAndListBoxTable.RowCount = 2;
			this._searchAndListBoxTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._searchAndListBoxTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._searchAndListBoxTable.Size = new System.Drawing.Size(146, 257);
			this._searchAndListBoxTable.TabIndex = 7;
			//
			// _todoRecordsListBox
			//
			this._recordsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._recordsListBox.Location = new System.Drawing.Point(3, 42);
			this._recordsListBox.Name = "_recordsListBox";
			this._recordsListBox.Size = new System.Drawing.Size(140, 212);
			this._recordsListBox.TabIndex = 4;
			this._recordsListBox.View = System.Windows.Forms.View.SmallIcon;
			//
			// _searchTextBoxControl
			//
			this._searchTextBoxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._searchTextBoxControl.AutoSize = true;
			this._searchTextBoxControl.BackColor = System.Drawing.Color.White;
			this._searchTextBoxControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._searchTextBoxControl.Location = new System.Drawing.Point(3, 3);
			this._searchTextBoxControl.Name = "_searchTextBoxControl";
			this._searchTextBoxControl.Size = new System.Drawing.Size(140, 33);
			this._searchTextBoxControl.TabIndex = 6;
			//
			// _showAllFieldsToggleButton
			//
			this._showAllFieldsToggleButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._showAllFieldsToggleButton.AutoSize = true;
			this._showAllFieldsToggleButton.FlatAppearance.BorderSize = 0;
			this._showAllFieldsToggleButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this._showAllFieldsToggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._showAllFieldsToggleButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._showAllFieldsToggleButton.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
			this._showAllFieldsToggleButton.Location = new System.Drawing.Point(387, 0);
			this._showAllFieldsToggleButton.Margin = new System.Windows.Forms.Padding(0);
			this._showAllFieldsToggleButton.Name = "_showAllFieldsToggleButton";
			this._showAllFieldsToggleButton.Size = new System.Drawing.Size(100, 25);
			this._showAllFieldsToggleButton.TabIndex = 2;
			this._showAllFieldsToggleButton.Text = "&Show All Fields";
			this._showAllFieldsToggleButton.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this._showAllFieldsToggleButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._showAllFieldsToggleButton.Click += new System.EventHandler(this.OnShowAllFields_Click);
			//
			// _btnDeleteWord
			//
			this._btnDeleteWord.Anchor = System.Windows.Forms.AnchorStyles.None;
			this._btnDeleteWord.AutoSize = true;
			this._btnDeleteWord.FlatAppearance.BorderSize = 0;
			this._btnDeleteWord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this._btnDeleteWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnDeleteWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._btnDeleteWord.Image = global::WeSay.LexicalTools.Properties.Resources.DeleteIcon;
			this._btnDeleteWord.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._btnDeleteWord.Location = new System.Drawing.Point(176, 0);
			this._btnDeleteWord.Margin = new System.Windows.Forms.Padding(0);
			this._btnDeleteWord.Name = "_btnDeleteWord";
			this._btnDeleteWord.Size = new System.Drawing.Size(134, 25);
			this._btnDeleteWord.TabIndex = 1;
			this._btnDeleteWord.Text = "~&Delete This Word";
			this._btnDeleteWord.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this._btnDeleteWord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._btnDeleteWord.Click += new System.EventHandler(this.OnDeleteWord_Click);
			//
			// _btnNewWord
			//
			this._btnNewWord.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._btnNewWord.AutoSize = true;
			this._btnNewWord.FlatAppearance.BorderSize = 0;
			this._btnNewWord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this._btnNewWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnNewWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._btnNewWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnNewWord.Image")));
			this._btnNewWord.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._btnNewWord.Location = new System.Drawing.Point(0, 0);
			this._btnNewWord.Margin = new System.Windows.Forms.Padding(0);
			this._btnNewWord.Name = "_btnNewWord";
			this._btnNewWord.Size = new System.Drawing.Size(99, 25);
			this._btnNewWord.TabIndex = 0;
			this._btnNewWord.Text = "~&New Word";
			this._btnNewWord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.toolTip1.SetToolTip(this._btnNewWord, "Ctrl+N");
			this._btnNewWord.Click += new System.EventHandler(this.OnNewWord_Click);
			//
			// _bottomButtonsAndRestTable
			//
			this._bottomButtonsAndRestTable.ColumnCount = 1;
			this._bottomButtonsAndRestTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._bottomButtonsAndRestTable.Controls.Add(this._bottomButtonTable, 0, 1);
			this._bottomButtonsAndRestTable.Controls.Add(this._listBoxAndSplitterAndDictionaryViewTable, 0, 0);
			this._bottomButtonsAndRestTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bottomButtonsAndRestTable.Location = new System.Drawing.Point(0, 0);
			this._bottomButtonsAndRestTable.Name = "_bottomButtonsAndRestTable";
			this._bottomButtonsAndRestTable.RowCount = 2;
			this._bottomButtonsAndRestTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._bottomButtonsAndRestTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._bottomButtonsAndRestTable.Size = new System.Drawing.Size(493, 294);
			this._bottomButtonsAndRestTable.TabIndex = 6;
			this._bottomButtonsAndRestTable.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
			//
			// _bottomButtonTable
			//
			this._bottomButtonTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._bottomButtonTable.AutoSize = true;
			this._bottomButtonTable.ColumnCount = 3;
			this._bottomButtonTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this._bottomButtonTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this._bottomButtonTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this._bottomButtonTable.Controls.Add(this._showAllFieldsToggleButton, 2, 0);
			this._bottomButtonTable.Controls.Add(this._btnNewWord, 0, 0);
			this._bottomButtonTable.Controls.Add(this._btnDeleteWord, 1, 0);
			this._bottomButtonTable.Location = new System.Drawing.Point(3, 266);
			this._bottomButtonTable.Name = "_bottomButtonTable";
			this._bottomButtonTable.RowCount = 1;
			this._bottomButtonTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._bottomButtonTable.Size = new System.Drawing.Size(487, 25);
			this._bottomButtonTable.TabIndex = 7;
			//
			// _listBoxAndSplitterAndDictionaryViewTable
			//
			this._listBoxAndSplitterAndDictionaryViewTable.AutoSize = true;
			this._listBoxAndSplitterAndDictionaryViewTable.Controls.Add(this.panelDetail);
			this._listBoxAndSplitterAndDictionaryViewTable.Controls.Add(this._splitter);
			this._listBoxAndSplitterAndDictionaryViewTable.Controls.Add(this._searchAndListBoxTable);
			this._listBoxAndSplitterAndDictionaryViewTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listBoxAndSplitterAndDictionaryViewTable.Location = new System.Drawing.Point(3, 3);
			this._listBoxAndSplitterAndDictionaryViewTable.Name = "_listBoxAndSplitterAndDictionaryViewTable";
			this._listBoxAndSplitterAndDictionaryViewTable.Size = new System.Drawing.Size(487, 257);
			this._listBoxAndSplitterAndDictionaryViewTable.TabIndex = 1;
			//
			// panelDetail
			//
			this.panelDetail.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelDetail.Location = new System.Drawing.Point(154, 0);
			this.panelDetail.Name = "panelDetail";
			this.panelDetail.Size = new System.Drawing.Size(333, 257);
			this.panelDetail.TabIndex = 9;
			//
			// _splitter
			//
			this._splitter.BackColorEnd = System.Drawing.Color.Empty;
			this._splitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
			this._splitter.ControlToHide = this._searchAndListBoxTable;
			this._splitter.GripLength = 90;
			this._splitter.GripperLocation = WeSay.UI.GripperLocations.Center;
			this._splitter.GripperStyle = WeSay.UI.GripperStyles.Mozilla;
			this._splitter.Location = new System.Drawing.Point(146, 0);
			this._splitter.Name = "_splitter";
			this._splitter.TabIndex = 8;
			this._splitter.TabStop = false;
			//
			// DictionaryControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._bottomButtonsAndRestTable);
			this.Name = "DictionaryControl";
			this.Size = new System.Drawing.Size(493, 294);
			this.Leave += new System.EventHandler(this.DictionaryControl_Leave);
			this._searchAndListBoxTable.ResumeLayout(false);
			this._bottomButtonsAndRestTable.ResumeLayout(false);
			this._bottomButtonTable.ResumeLayout(false);
			this._listBoxAndSplitterAndDictionaryViewTable.ResumeLayout(false);
			this._splitter.ResumeLayout(false);
			this.panelDetail.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private IWeSayListView _recordsListBox;
		private Button _btnDeleteWord;
		private Button _btnNewWord;
		private Button _showAllFieldsToggleButton;
		private ToolTip toolTip1;
		private SearchBoxControl _searchTextBoxControl;
		private TableLayoutPanel _bottomButtonsAndRestTable;
		private Panel _listBoxAndSplitterAndDictionaryViewTable;
		private TableLayoutPanel _searchAndListBoxTable;
		private TableLayoutPanel _bottomButtonTable;
		private CollapsibleSplitter _splitter;
		private Panel panelDetail;

	}
}
