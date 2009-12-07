using System.Windows.Forms;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;

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
			this.panelWordList = new System.Windows.Forms.Panel();
			this._searchTextBoxControl = new WeSay.LexicalTools.DictionaryBrowseAndEdit.SearchBoxControl();
			this._recordsListBox = new WeSay.UI.WeSayListView();
			this._bottomButtonPanel = new System.Windows.Forms.Panel();
			this._showAllFieldsToggleButton = new System.Windows.Forms.Button();
			this._btnDeleteWord = new System.Windows.Forms.Button();
			this._btnNewWord = new System.Windows.Forms.Button();
			this.panelDetail = new System.Windows.Forms.Panel();
			this._splitter = new WeSay.UI.CollapsibleSplitter();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.panelWordList.SuspendLayout();
			this._bottomButtonPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// panelWordList
			//
			this.panelWordList.Controls.Add(this._searchTextBoxControl);
			this.panelWordList.Controls.Add(this._recordsListBox);
			this.panelWordList.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelWordList.Location = new System.Drawing.Point(0, 0);
			this.panelWordList.Name = "panelWordList";
			this.panelWordList.Size = new System.Drawing.Size(140, 259);
			this.panelWordList.TabIndex = 2;
			//
			// _searchTextBoxControl
			//
			this._searchTextBoxControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._searchTextBoxControl.BackColor = System.Drawing.Color.White;
			this._searchTextBoxControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._searchTextBoxControl.Location = new System.Drawing.Point(0, 4);
			this._searchTextBoxControl.Name = "_searchTextBoxControl";
			this._searchTextBoxControl.Size = new System.Drawing.Size(140, 30);
			this._searchTextBoxControl.TabIndex = 6;
			//
			// _recordsListBox
			//
			this._recordsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._recordsListBox.Location = new System.Drawing.Point(3, 48);
			this._recordsListBox.Name = "_recordsListBox";
			this._recordsListBox.Size = new System.Drawing.Size(136, 211);
			this._recordsListBox.TabIndex = 4;
			this._recordsListBox.View = System.Windows.Forms.View.SmallIcon;
			//
			// _bottomButtonPanel
			//
			this._bottomButtonPanel.Controls.Add(this._showAllFieldsToggleButton);
			this._bottomButtonPanel.Controls.Add(this._btnDeleteWord);
			this._bottomButtonPanel.Controls.Add(this._btnNewWord);
			this._bottomButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._bottomButtonPanel.Location = new System.Drawing.Point(0, 259);
			this._bottomButtonPanel.Name = "_bottomButtonPanel";
			this._bottomButtonPanel.Size = new System.Drawing.Size(493, 35);
			this._bottomButtonPanel.TabIndex = 5;
			//
			// _showAllFieldsToggleButton
			//
			this._showAllFieldsToggleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._showAllFieldsToggleButton.AutoSize = true;
			this._showAllFieldsToggleButton.FlatAppearance.BorderSize = 0;
			this._showAllFieldsToggleButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this._showAllFieldsToggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._showAllFieldsToggleButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._showAllFieldsToggleButton.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
			this._showAllFieldsToggleButton.Location = new System.Drawing.Point(329, 6);
			this._showAllFieldsToggleButton.Margin = new System.Windows.Forms.Padding(0);
			this._showAllFieldsToggleButton.Name = "_showAllFieldsToggleButton";
			this._showAllFieldsToggleButton.Size = new System.Drawing.Size(154, 25);
			this._showAllFieldsToggleButton.TabIndex = 2;
			this._showAllFieldsToggleButton.Text = "&Show All Fields";
			this._showAllFieldsToggleButton.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this._showAllFieldsToggleButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._showAllFieldsToggleButton.Click += new System.EventHandler(this.OnShowAllFields_Click);
			//
			// _btnDeleteWord
			//
			this._btnDeleteWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._btnDeleteWord.AutoSize = true;
			this._btnDeleteWord.FlatAppearance.BorderSize = 0;
			this._btnDeleteWord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this._btnDeleteWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnDeleteWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._btnDeleteWord.Image = global::WeSay.LexicalTools.Properties.Resources.DeleteWord;
			this._btnDeleteWord.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._btnDeleteWord.Location = new System.Drawing.Point(239, 6);
			this._btnDeleteWord.Margin = new System.Windows.Forms.Padding(0);
			this._btnDeleteWord.Name = "_btnDeleteWord";
			this._btnDeleteWord.Size = new System.Drawing.Size(159, 25);
			this._btnDeleteWord.TabIndex = 1;
			this._btnDeleteWord.Text = "~&Delete This Word";
			this._btnDeleteWord.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this._btnDeleteWord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._btnDeleteWord.Click += new System.EventHandler(this.OnDeleteWord_Click);
			//
			// _btnNewWord
			//
			this._btnNewWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._btnNewWord.AutoSize = true;
			this._btnNewWord.FlatAppearance.BorderSize = 0;
			this._btnNewWord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this._btnNewWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnNewWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._btnNewWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnNewWord.Image")));
			this._btnNewWord.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._btnNewWord.Location = new System.Drawing.Point(0, 6);
			this._btnNewWord.Margin = new System.Windows.Forms.Padding(0);
			this._btnNewWord.Name = "_btnNewWord";
			this._btnNewWord.Size = new System.Drawing.Size(139, 25);
			this._btnNewWord.TabIndex = 0;
			this._btnNewWord.Text = "~&New Word";
			this._btnNewWord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.toolTip1.SetToolTip(this._btnNewWord, "Ctrl+N");
			this._btnNewWord.Click += new System.EventHandler(this.OnNewWord_Click);
			//
			// panelDetail
			//
			this.panelDetail.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelDetail.Location = new System.Drawing.Point(148, 0);
			this.panelDetail.Margin = new System.Windows.Forms.Padding(0);
			this.panelDetail.Name = "panelDetail";
			this.panelDetail.Size = new System.Drawing.Size(345, 259);
			this.panelDetail.TabIndex = 0;
			//
			// _splitter
			//
			this._splitter.BackColorEnd = System.Drawing.Color.Empty;
			this._splitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.Adjust;
			this._splitter.ControlToHide = this.panelWordList;
			this._splitter.GripLength = 15;
			this._splitter.GripperLocation = WeSay.UI.GripperLocations.RightOrBottom;
			this._splitter.GripperStyle = WeSay.UI.GripperStyles.DoubleDots;
			this._splitter.Location = new System.Drawing.Point(140, 0);
			this._splitter.MinSize = 50;
			this._splitter.Name = "_splitter";
			this._splitter.TabIndex = 1;
			this._splitter.TabStop = false;
			//
			// DictionaryControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.panelDetail);
			this.Controls.Add(this._splitter);
			this.Controls.Add(this.panelWordList);
			this.Controls.Add(this._bottomButtonPanel);
			this.Name = "DictionaryControl";
			this.Size = new System.Drawing.Size(493, 294);
			this.Leave += new System.EventHandler(this.DictionaryControl_Leave);
			this.panelWordList.ResumeLayout(false);
			this._bottomButtonPanel.ResumeLayout(false);
			this._bottomButtonPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Panel panelWordList;
		private Panel panelDetail;
		private Panel _bottomButtonPanel;
		private WeSayListView _recordsListBox;
		private Button _btnDeleteWord;
		private Button _btnNewWord;
		private CollapsibleSplitter _splitter;
		private Button _showAllFieldsToggleButton;
		private ToolTip toolTip1;
		private SearchBoxControl _searchTextBoxControl;

	}
}
