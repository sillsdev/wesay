using System;
using Palaso.UI.WindowsForms.Widgets.Flying;
using WeSay.UI;
using WeSay.UI.Buttons;
using WeSay.UI.TextBoxes;

namespace WeSay.LexicalTools.GatherByWordList
{
	partial class GatherWordListControl
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("stuff");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("blah");
			this._instructionLabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._boxForeignWord = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._flyingLabel = new Palaso.UI.WindowsForms.Widgets.Flying.FlyingLabel();
			this._verticalWordListView = new WeSay.UI.WeSayListView();
			this._vernacularBox = new WeSay.UI.TextBoxes.MultiTextControl();
			this._listViewOfWordsMatchingCurrentItem = new WeSay.UI.WeSayListBox();
			this._congratulationsControl = new WeSay.LexicalTools.CongratulationsControl();
			this._btnAddWord = new WeSay.UI.Buttons.RectangularImageButton();
			this._btnNextWord = new WeSay.UI.Buttons.RectangularImageButton();
			this.SuspendLayout();
			//
			// _instructionLabel
			//
			this._instructionLabel.AutoSize = true;
			this._instructionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F);
			this._instructionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(118)))), ((int)(((byte)(17)))));
			this._instructionLabel.Location = new System.Drawing.Point(8, 8);
			this._instructionLabel.Name = "_instructionLabel";
			this._instructionLabel.Size = new System.Drawing.Size(434, 20);
			this._instructionLabel.TabIndex = 2;
			this._instructionLabel.Text = "Try thinking of ways to say these words in your language.";
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F);
			this.label3.Location = new System.Drawing.Point(215, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 20);
			this.label3.TabIndex = 6;
			this.label3.Text = "Word:";
			//
			// _boxForeignWord
			//
			this._boxForeignWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this._boxForeignWord.BackColor = System.Drawing.SystemColors.Control;
			this._boxForeignWord.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._boxForeignWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._boxForeignWord.Location = new System.Drawing.Point(275, 69);
			this._boxForeignWord.Name = "_boxForeignWord";
			this._boxForeignWord.ReadOnly = true;
			this._boxForeignWord.Size = new System.Drawing.Size(315, 26);
			this._boxForeignWord.TabIndex = 7;
			this._boxForeignWord.TabStop = false;
			this._boxForeignWord.Text = "Foobar";
			//
			// panel1
			//
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(118)))), ((int)(((byte)(17)))));
			this.panel1.ForeColor = System.Drawing.Color.Transparent;
			this.panel1.Location = new System.Drawing.Point(0, 36);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(804, 1);
			this.panel1.TabIndex = 8;
			//
			// label4
			//
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(118)))), ((int)(((byte)(17)))));
			this.label4.Location = new System.Drawing.Point(655, 120);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(73, 16);
			this.label4.TabIndex = 2;
			this.label4.Text = "(Enter Key)";
			//
			// label5
			//
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(118)))), ((int)(((byte)(17)))));
			this.label5.Location = new System.Drawing.Point(653, 74);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(112, 16);
			this.label5.TabIndex = 2;
			this.label5.Text = "(Page Down Key)";
			//
			// _flyingLabel
			//
			this._flyingLabel.BackColor = System.Drawing.Color.Transparent;
			this._flyingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._flyingLabel.Location = new System.Drawing.Point(0, 0);
			this._flyingLabel.Name = "_flyingLabel";
			this._flyingLabel.Size = new System.Drawing.Size(100, 23);
			this._flyingLabel.TabIndex = 10;
			this._flyingLabel.Visible = false;
			//
			// _verticalWordListView
			//
			this._verticalWordListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)));
			this._verticalWordListView.Location = new System.Drawing.Point(12, 67);
			this._verticalWordListView.Name = "_verticalWordListView";
			this._verticalWordListView.Size = new System.Drawing.Size(177, 346);
			this._verticalWordListView.TabIndex = 11;
			this._verticalWordListView.View = System.Windows.Forms.View.SmallIcon;
			//
			// _vernacularBox
			//
			this._vernacularBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this._vernacularBox.AutoSize = true;
			this._vernacularBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._vernacularBox.BackColor = System.Drawing.Color.White;
			this._vernacularBox.ColumnCount = 3;
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.IsSpellCheckingEnabled = false;
			this._vernacularBox.Location = new System.Drawing.Point(275, 120);
			this._vernacularBox.MinimumSize = new System.Drawing.Size(50, 20);
			this._vernacularBox.Name = "_vernacularBox";
			this._vernacularBox.ShowAnnotationWidget = false;
			this._vernacularBox.Size = new System.Drawing.Size(315, 20);
			this._vernacularBox.TabIndex = 0;
			//
			// _listViewOfWordsMatchingCurrentItem
			//
			this._listViewOfWordsMatchingCurrentItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this._listViewOfWordsMatchingCurrentItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._listViewOfWordsMatchingCurrentItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._listViewOfWordsMatchingCurrentItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._listViewOfWordsMatchingCurrentItem.ItemHeight = 20;
			this._listViewOfWordsMatchingCurrentItem.Items.AddRange(new object[] {
			listViewItem1,
			listViewItem2});
			this._listViewOfWordsMatchingCurrentItem.ItemToNotDrawYet = null;
			this._listViewOfWordsMatchingCurrentItem.Location = new System.Drawing.Point(275, 169);
			this._listViewOfWordsMatchingCurrentItem.MinimumSize = new System.Drawing.Size(315, 84);
			this._listViewOfWordsMatchingCurrentItem.MultiColumn = true;
			this._listViewOfWordsMatchingCurrentItem.Name = "_listViewOfWordsMatchingCurrentItem";
			this._listViewOfWordsMatchingCurrentItem.Size = new System.Drawing.Size(315, 102);
			this._listViewOfWordsMatchingCurrentItem.TabIndex = 4;
			this._listViewOfWordsMatchingCurrentItem.TabStop = false;
			this._listViewOfWordsMatchingCurrentItem.Click += new System.EventHandler(this.OnListViewOfWordsMatchingCurrentItem_Click);
			//
			// _congratulationsControl
			//
			this._congratulationsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this._congratulationsControl.Location = new System.Drawing.Point(12, 43);
			this._congratulationsControl.Name = "_congratulationsControl";
			this._congratulationsControl.Size = new System.Drawing.Size(791, 370);
			this._congratulationsControl.TabIndex = 9;
			//
			// _btnAddWord
			//
			this._btnAddWord.Anchor = System.Windows.Forms.AnchorStyles.None;
			this._btnAddWord.BackColor = System.Drawing.Color.Transparent;
			this._btnAddWord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this._btnAddWord.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this._btnAddWord.FlatAppearance.BorderSize = 0;
			this._btnAddWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnAddWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._btnAddWord.ForeColor = System.Drawing.Color.Transparent;
			this._btnAddWord.Image = global::WeSay.LexicalTools.Properties.Resources.AddWord;
			this._btnAddWord.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._btnAddWord.Location = new System.Drawing.Point(598, 119);
			this._btnAddWord.Name = "_btnAddWord";
			this._btnAddWord.Size = new System.Drawing.Size(54, 27);
			this._btnAddWord.TabIndex = 17;
			this._btnAddWord.UseVisualStyleBackColor = false;
			this._btnAddWord.Click += new System.EventHandler(this._btnAddWord_Click);
			//
			// _btnNextWord
			//
			this._btnNextWord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this._btnNextWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnNextWord.Image = global::WeSay.LexicalTools.Properties.Resources.right_arrow;
			this._btnNextWord.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._btnNextWord.Location = new System.Drawing.Point(598, 61);
			this._btnNextWord.Name = "_btnNextWord";
			this._btnNextWord.Size = new System.Drawing.Size(49, 47);
			this._btnNextWord.TabIndex = 16;
			this._btnNextWord.UseVisualStyleBackColor = true;
			this._btnNextWord.Click += new System.EventHandler(this._btnNextWord_Click);
			//
			// GatherWordListControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.Controls.Add(this._btnAddWord);
			this.Controls.Add(this._btnNextWord);
			this.Controls.Add(this._verticalWordListView);
			this.Controls.Add(this._vernacularBox);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._boxForeignWord);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._listViewOfWordsMatchingCurrentItem);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._instructionLabel);
			this.Controls.Add(this._congratulationsControl);
			this.Controls.Add(this._flyingLabel);
			this.Name = "GatherWordListControl";
			this.Size = new System.Drawing.Size(806, 429);
			this.Load += new System.EventHandler(this.GatherWordListControl_Load);
			this.BackColorChanged += new System.EventHandler(this.GatherWordListControl_BackColorChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _instructionLabel;
		private WeSayListBox _listViewOfWordsMatchingCurrentItem;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _boxForeignWord;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private MultiTextControl _vernacularBox;
		private CongratulationsControl _congratulationsControl;
		private FlyingLabel _flyingLabel;
		private WeSayListView _verticalWordListView;
		private RectangularImageButton _btnNextWord;
		private RectangularImageButton _btnAddWord;


	}
}
