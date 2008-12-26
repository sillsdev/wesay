using System;
using WeSay.UI;
using WeSay.UI.Buttons;
using WeSay.UI.TextBoxes;

namespace WeSay.LexicalTools
{
	partial class GatherBySemanticDomainsControl
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("blah");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("stuff");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GatherBySemanticDomainsControl));
			this._domainName = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this._listViewWords = new WeSay.UI.WeSayListBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._movingLabel = new WeSay.UI.MovingLabel();
			this._instructionLabel = new System.Windows.Forms.Label();
			this._question = new System.Windows.Forms.Label();
			this._description = new System.Windows.Forms.Label();
			this._questionIndicator = new WeSay.UI.CirclesProgressIndicator();
			this._vernacularBox = new MultiTextControl();
			this._btnPrevious = new WeSay.UI.Buttons.PreviousButton();
			this._btnNext = new WeSay.UI.Buttons.NextButton();
			this._btnAddWord = new WeSay.UI.Buttons.AddButton();
			this._reminder = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _domainName
			//
			this._domainName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._domainName.BackColor = System.Drawing.SystemColors.Control;
			this._domainName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._domainName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._domainName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._domainName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._domainName.Location = new System.Drawing.Point(13, 47);
			this._domainName.Name = "_domainName";
			this._domainName.Size = new System.Drawing.Size(434, 27);
			this._domainName.TabIndex = 20;
			this._domainName.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this._domainName_DrawItem);
			this._domainName.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this._domainName_MeasureItem);
			this._domainName.SelectedIndexChanged += new System.EventHandler(this._domainName_SelectedIndexChanged);
			//
			// label3
			//
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
			this.label3.Location = new System.Drawing.Point(10, 355);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 15);
			this.label3.TabIndex = 19;
			this.label3.Text = "Word";
			//
			// _listViewWords
			//
			this._listViewWords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._listViewWords.ColumnWidth = 100;
			this._listViewWords.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._listViewWords.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._listViewWords.ItemHeight = 20;
			this._listViewWords.Items.AddRange(new object[] {
			listViewItem1,
			listViewItem2});
			this._listViewWords.ItemToNotDrawYet = null;
			this._listViewWords.Location = new System.Drawing.Point(15, 273);
			this._listViewWords.MultiColumn = true;
			this._listViewWords.Name = "_listViewWords";
			this._listViewWords.Size = new System.Drawing.Size(622, 64);
			this._listViewWords.TabIndex = 17;
			this._listViewWords.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._listViewWords_KeyPress);
			this._listViewWords.Click += new System.EventHandler(this._listViewWords_Click);
			//
			// label5
			//
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.label5.ForeColor = System.Drawing.Color.DarkGray;
			this.label5.Location = new System.Drawing.Point(535, 54);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(112, 16);
			this.label5.TabIndex = 16;
			this.label5.Text = "(Page Down Key)";
			//
			// label4
			//
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.label4.ForeColor = System.Drawing.Color.DarkGray;
			this.label4.Location = new System.Drawing.Point(573, 352);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(73, 16);
			this.label4.TabIndex = 14;
			this.label4.Text = "(Enter Key)";
			//
			// _movingLabel
			//
			this._movingLabel.BackColor = System.Drawing.Color.Transparent;
			this._movingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._movingLabel.Location = new System.Drawing.Point(0, 0);
			this._movingLabel.Name = "_movingLabel";
			this._movingLabel.Size = new System.Drawing.Size(100, 23);
			this._movingLabel.TabIndex = 25;
			this._movingLabel.Visible = false;
			//
			// _instructionLabel
			//
			this._instructionLabel.AutoSize = true;
			this._instructionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F);
			this._instructionLabel.ForeColor = System.Drawing.Color.DarkGray;
			this._instructionLabel.Location = new System.Drawing.Point(11, 7);
			this._instructionLabel.Name = "_instructionLabel";
			this._instructionLabel.Size = new System.Drawing.Size(423, 20);
			this._instructionLabel.TabIndex = 15;
			this._instructionLabel.Text = "Try thinking of words you use to talk about these things.";
			//
			// _question
			//
			this._question.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._question.AutoEllipsis = true;
			this._question.BackColor = System.Drawing.Color.MistyRose;
			this._question.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.75F, System.Drawing.FontStyle.Bold);
			this._question.Location = new System.Drawing.Point(15, 189);
			this._question.Name = "_question";
			this._question.Size = new System.Drawing.Size(622, 50);
			this._question.TabIndex = 23;
			//
			// _description
			//
			this._description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._description.AutoEllipsis = true;
			this._description.BackColor = System.Drawing.Color.MistyRose;
			this._description.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
			this._description.Location = new System.Drawing.Point(15, 85);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(622, 90);
			this._description.TabIndex = 24;
			//
			// _questionIndicator
			//
			this._questionIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._questionIndicator.AutoSize = true;
			this._questionIndicator.BulletColor = System.Drawing.Color.Azure;
			this._questionIndicator.BulletColorEnd = System.Drawing.Color.MediumBlue;
			this._questionIndicator.BulletPadding = new System.Windows.Forms.Padding(1);
			this._questionIndicator.Location = new System.Drawing.Point(15, 179);
			this._questionIndicator.Name = "_questionIndicator";
			this._questionIndicator.Size = new System.Drawing.Size(70, 7);
			this._questionIndicator.TabIndex = 0;
			this._questionIndicator.TabStop = false;
			//
			// _vernacularBox
			//
			this._vernacularBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
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
			this._vernacularBox.Location = new System.Drawing.Point(49, 374);
			this._vernacularBox.Name = "_vernacularBox";
			this._vernacularBox.ShowAnnotationWidget = false;
			this._vernacularBox.Size = new System.Drawing.Size(439, 0);
			this._vernacularBox.TabIndex = 1;
			this._vernacularBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._boxVernacularWord_KeyDown);
			//
			// _btnPrevious
			//
			this._btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnPrevious.Image = ((System.Drawing.Image)(resources.GetObject("_btnPrevious.Image")));
			this._btnPrevious.Location = new System.Drawing.Point(454, 44);
			this._btnPrevious.Name = "_btnPrevious";
			this._btnPrevious.Size = new System.Drawing.Size(30, 30);
			this._btnPrevious.TabIndex = 12;
			this._btnPrevious.Click += new System.EventHandler(this._btnPrevious_Click);
			//
			// _btnNext
			//
			this._btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnNext.Image = ((System.Drawing.Image)(resources.GetObject("_btnNext.Image")));
			this._btnNext.Location = new System.Drawing.Point(485, 35);
			this._btnNext.Name = "_btnNext";
			this._btnNext.Size = new System.Drawing.Size(50, 50);
			this._btnNext.TabIndex = 13;
			this._btnNext.Click += new System.EventHandler(this._btnNext_Click);
			//
			// _btnAddWord
			//
			this._btnAddWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddWord.ArrowHeadHeight = 15;
			this._btnAddWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnAddWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._btnAddWord.Image = ((System.Drawing.Image)(resources.GetObject("_btnAddWord.Image")));
			this._btnAddWord.Location = new System.Drawing.Point(496, 348);
			this._btnAddWord.Name = "_btnAddWord";
			this._btnAddWord.PointingDirection = WeSay.UI.Buttons.PointingDirection.Left;
			this._btnAddWord.Size = new System.Drawing.Size(70, 27);
			this._btnAddWord.TabIndex = 10;
			this._btnAddWord.Click += new System.EventHandler(this._btnAddWord_Click);
			//
			// _reminder
			//
			this._reminder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._reminder.AutoEllipsis = true;
			this._reminder.BackColor = System.Drawing.Color.LightYellow;
			this._reminder.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._reminder.ForeColor = System.Drawing.Color.DarkGray;
			this._reminder.Location = new System.Drawing.Point(15, 242);
			this._reminder.Name = "_reminder";
			this._reminder.Size = new System.Drawing.Size(622, 28);
			this._reminder.TabIndex = 26;
			//
			// GatherBySemanticDomainsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._reminder);
			this.Controls.Add(this._questionIndicator);
			this.Controls.Add(this._description);
			this.Controls.Add(this._question);
			this.Controls.Add(this._vernacularBox);
			this.Controls.Add(this._domainName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._listViewWords);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._instructionLabel);
			this.Controls.Add(this._btnPrevious);
			this.Controls.Add(this._btnNext);
			this.Controls.Add(this._btnAddWord);
			this.Controls.Add(this._movingLabel);
			this.Name = "GatherBySemanticDomainsControl";
			this.Size = new System.Drawing.Size(654, 386);
			this.BackColorChanged += new System.EventHandler(this.GatherWordListControl_BackColorChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		#endregion


		private MultiTextControl _vernacularBox;
		private System.Windows.Forms.ComboBox _domainName;
		private System.Windows.Forms.Label label3;
		private WeSayListBox _listViewWords;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label _instructionLabel;
		private PreviousButton _btnPrevious;
		private NextButton _btnNext;
		private AddButton _btnAddWord;
		private System.Windows.Forms.Label _question;
		private System.Windows.Forms.Label _description;
		private WeSay.UI.CirclesProgressIndicator _questionIndicator;
		private MovingLabel _movingLabel;
		private System.Windows.Forms.Label _reminder;

	}
}
