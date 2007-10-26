using System;
using WeSay.UI;
using WeSay.UI.Buttons;

namespace WeSay.LexicalTools
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("stuff");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("blah");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GatherWordListControl));
			this._instructionLabel = new System.Windows.Forms.Label();
			this._listViewOfWordsMatchingCurrentItem = new System.Windows.Forms.ListView();
			this.label3 = new System.Windows.Forms.Label();
			this._boxForeignWord = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._vernacularBox = new WeSay.UI.MultiTextControl();
			this._btnPreviousWord = new WeSay.UI.Buttons.PreviousButton();
			this._btnNextWord = new WeSay.UI.Buttons.NextButton();
			this._btnAddWord = new WeSay.UI.Buttons.AddButton();
			this._congratulationsControl = new WeSay.LexicalTools.CongratulationsControl();
			this.localizationHelper1 = new WeSay.UI.LocalizationHelper(this.components);
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).BeginInit();
			this.SuspendLayout();
			//
			// _instructionLabel
			//
			this._instructionLabel.AutoSize = true;
			this._instructionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._instructionLabel.ForeColor = System.Drawing.Color.DarkGray;
			this._instructionLabel.Location = new System.Drawing.Point(8, 8);
			this._instructionLabel.Name = "_instructionLabel";
			this._instructionLabel.Size = new System.Drawing.Size(415, 20);
			this._instructionLabel.TabIndex = 2;
			this._instructionLabel.Text = "~Try thinking of ways to say these words in your language.";
			//
			// _listViewOfWordsMatchingCurrentItem
			//
			this._listViewOfWordsMatchingCurrentItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._listViewOfWordsMatchingCurrentItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._listViewOfWordsMatchingCurrentItem.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
			listViewItem1,
			listViewItem2});
			this._listViewOfWordsMatchingCurrentItem.Location = new System.Drawing.Point(11, 169);
			this._listViewOfWordsMatchingCurrentItem.MinimumSize = new System.Drawing.Size(315, 84);
			this._listViewOfWordsMatchingCurrentItem.MultiSelect = false;
			this._listViewOfWordsMatchingCurrentItem.Name = "_listViewOfWordsMatchingCurrentItem";
			this._listViewOfWordsMatchingCurrentItem.Size = new System.Drawing.Size(315, 126);
			this._listViewOfWordsMatchingCurrentItem.TabIndex = 4;
			this._listViewOfWordsMatchingCurrentItem.TabStop = false;
			this._listViewOfWordsMatchingCurrentItem.UseCompatibleStateImageBehavior = false;
			this._listViewOfWordsMatchingCurrentItem.View = System.Windows.Forms.View.List;
			this._listViewOfWordsMatchingCurrentItem.Click += new System.EventHandler(this._listViewOfWordsMatchingCurrentItem_Click);
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(8, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 20);
			this.label3.TabIndex = 6;
			this.label3.Text = "~Word:";
			//
			// _boxForeignWord
			//
			this._boxForeignWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._boxForeignWord.BackColor = System.Drawing.SystemColors.Control;
			this._boxForeignWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._boxForeignWord.Location = new System.Drawing.Point(78, 69);
			this._boxForeignWord.Name = "_boxForeignWord";
			this._boxForeignWord.ReadOnly = true;
			this._boxForeignWord.Size = new System.Drawing.Size(248, 26);
			this._boxForeignWord.TabIndex = 7;
			this._boxForeignWord.TabStop = false;
			this._boxForeignWord.Text = "Foobar";
			//
			// panel1
			//
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.LightGray;
			this.panel1.ForeColor = System.Drawing.Color.Transparent;
			this.panel1.Location = new System.Drawing.Point(0, 36);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(509, 1);
			this.panel1.TabIndex = 8;
			//
			// label4
			//
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.DarkGray;
			this.label4.Location = new System.Drawing.Point(405, 125);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(74, 15);
			this.label4.TabIndex = 2;
			this.label4.Text = "~(Enter Key)";
			//
			// label5
			//
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.DarkGray;
			this.label5.Location = new System.Drawing.Point(410, 72);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(109, 15);
			this.label5.TabIndex = 2;
			this.label5.Text = "~(Page Down Key)";
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
			this._vernacularBox.Location = new System.Drawing.Point(78, 120);
			this._vernacularBox.MinimumSize = new System.Drawing.Size(50, 20);
			this._vernacularBox.Name = "_vernacularBox";
			this._vernacularBox.ShowAnnotationWidget = false;
			this._vernacularBox.Size = new System.Drawing.Size(248, 20);
			this._vernacularBox.TabIndex = 0;
			//
			// _btnPreviousWord
			//
			this._btnPreviousWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnPreviousWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnPreviousWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnPreviousWord.Image")));
			this._btnPreviousWord.Location = new System.Drawing.Point(329, 67);
			this._btnPreviousWord.Name = "_btnPreviousWord";
			this._btnPreviousWord.Size = new System.Drawing.Size(30, 30);
			this._btnPreviousWord.TabIndex = 1;
			this._btnPreviousWord.Click += new System.EventHandler(this._btnPreviousWord_Click);
			//
			// _btnNextWord
			//
			this._btnNextWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnNextWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnNextWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnNextWord.Image")));
			this._btnNextWord.Location = new System.Drawing.Point(361, 56);
			this._btnNextWord.Name = "_btnNextWord";
			this._btnNextWord.Size = new System.Drawing.Size(50, 50);
			this._btnNextWord.TabIndex = 1;
			this._btnNextWord.Click += new System.EventHandler(this._btnNextWord_Click);
			//
			// _btnAddWord
			//
			this._btnAddWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddWord.ArrowHeadHeight = 15;
			this._btnAddWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnAddWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._btnAddWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnAddWord.Image")));
			this._btnAddWord.Location = new System.Drawing.Point(331, 118);
			this._btnAddWord.Name = "_btnAddWord";
			this._btnAddWord.PointingDirection = WeSay.UI.Buttons.PointingDirection.Left;
			this._btnAddWord.Size = new System.Drawing.Size(70, 27);
			this._btnAddWord.TabIndex = 0;
			this._btnAddWord.Click += new System.EventHandler(this._btnAddWord_Click);
			//
			// _congratulationsControl
			//
			this._congratulationsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._congratulationsControl.Location = new System.Drawing.Point(0, 44);
			this._congratulationsControl.Name = "_congratulationsControl";
			this._congratulationsControl.Size = new System.Drawing.Size(507, 370);
			this._congratulationsControl.TabIndex = 9;
			//
			// localizationHelper1
			//
			this.localizationHelper1.Parent = this;
			//
			// GatherWordListControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.Controls.Add(this._vernacularBox);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._boxForeignWord);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._listViewOfWordsMatchingCurrentItem);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._instructionLabel);
			this.Controls.Add(this._btnPreviousWord);
			this.Controls.Add(this._btnNextWord);
			this.Controls.Add(this._btnAddWord);
			this.Controls.Add(this._congratulationsControl);
			this.Name = "GatherWordListControl";
			this.Size = new System.Drawing.Size(511, 429);
			this.Load += new System.EventHandler(this.GatherWordListControl_Load);
			this.BackColorChanged += new System.EventHandler(this.GatherWordListControl_BackColorChanged);
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private AddButton _btnAddWord;
		private NextButton _btnNextWord;
		private PreviousButton _btnPreviousWord;
	  private System.Windows.Forms.Label _instructionLabel;
		private System.Windows.Forms.ListView _listViewOfWordsMatchingCurrentItem;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _boxForeignWord;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private MultiTextControl _vernacularBox;
		private CongratulationsControl _congratulationsControl;
		private LocalizationHelper localizationHelper1;

	 }
}
