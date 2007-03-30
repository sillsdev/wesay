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
			this._vernacularBox = new WeSay.UI.MultiTextControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this._domainName = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._listViewWords = new System.Windows.Forms.ListBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._btnPrevious = new ArrowButton.ArrowButton();
			this._btnNext = new ArrowButton.ArrowButton();
			this._btnAddWord = new ArrowButton.ArrowButton();
			this._question = new System.Windows.Forms.Label();
			this._description = new System.Windows.Forms.Label();
			this._congratulationsControl = new WeSay.LexicalTools.CongratulationsControl();
			this.SuspendLayout();
			//
			// _vernacularBox
			//
			this._vernacularBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._vernacularBox.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
			this._vernacularBox.BackColor = System.Drawing.Color.Green;
			this._vernacularBox.Location = new System.Drawing.Point(47, 344);
			this._vernacularBox.Name = "_vernacularBox";
			this._vernacularBox.ShowAnnotationWidget = false;
			this._vernacularBox.Size = new System.Drawing.Size(409, 30);
			this._vernacularBox.TabIndex = 1;
			//
			// panel1
			//
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.LightGray;
			this.panel1.ForeColor = System.Drawing.Color.Transparent;
			this.panel1.Location = new System.Drawing.Point(3, 35);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(530, 1);
			this.panel1.TabIndex = 21;
			//
			// _domainName
			//
			this._domainName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._domainName.AutoEllipsis = true;
			this._domainName.BackColor = System.Drawing.SystemColors.Control;
			this._domainName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._domainName.Location = new System.Drawing.Point(15, 59);
			this._domainName.Name = "_domainName";
			this._domainName.Size = new System.Drawing.Size(441, 20);
			this._domainName.TabIndex = 20;
			this._domainName.Text = "Some Topic";
			//
			// label3
			//
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(10, 355);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 13);
			this.label3.TabIndex = 19;
			this.label3.Text = "Word";
			//
			// _listViewWords
			//
			this._listViewWords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._listViewWords.ColumnWidth = 100;
			this._listViewWords.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._listViewWords.ItemHeight = 20;
			this._listViewWords.Items.AddRange(new object[] {
			listViewItem1,
			listViewItem2});
			this._listViewWords.Location = new System.Drawing.Point(15, 233);
			this._listViewWords.MultiColumn = true;
			this._listViewWords.Name = "_listViewWords";
			this._listViewWords.Size = new System.Drawing.Size(441, 104);
			this._listViewWords.Sorted = true;
			this._listViewWords.TabIndex = 17;
			this._listViewWords.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._listViewWords_KeyPress);
			this._listViewWords.Click += new System.EventHandler(this._listViewWords_Click);
			//
			// label5
			//
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.DarkGray;
			this.label5.Location = new System.Drawing.Point(535, 63);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(102, 15);
			this.label5.TabIndex = 16;
			this.label5.Text = "(Page Down Key)";
			//
			// label4
			//
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.DarkGray;
			this.label4.Location = new System.Drawing.Point(536, 344);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(67, 15);
			this.label4.TabIndex = 14;
			this.label4.Text = "(Enter Key)";
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.DarkGray;
			this.label1.Location = new System.Drawing.Point(11, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(399, 20);
			this.label1.TabIndex = 15;
			this.label1.Text = "Try thinking of words you use to talk about these things.";
			//
			// _btnPrevious
			//
			this._btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnPrevious.ArrowEnabled = true;
			this._btnPrevious.HoverEndColor = System.Drawing.Color.Blue;
			this._btnPrevious.HoverStartColor = System.Drawing.Color.White;
			this._btnPrevious.Location = new System.Drawing.Point(462, 60);
			this._btnPrevious.Name = "_btnPrevious";
			this._btnPrevious.NormalEndColor = System.Drawing.Color.White;
			this._btnPrevious.NormalStartColor = System.Drawing.Color.White;
			this._btnPrevious.Rotation = 270;
			this._btnPrevious.Size = new System.Drawing.Size(24, 24);
			this._btnPrevious.StubbyStyle = false;
			this._btnPrevious.TabIndex = 12;
			this._btnPrevious.Click += new System.EventHandler(this._btnPrevious_Click);
			//
			// _btnNext
			//
			this._btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnNext.ArrowEnabled = true;
			this._btnNext.HoverEndColor = System.Drawing.Color.Blue;
			this._btnNext.HoverStartColor = System.Drawing.Color.White;
			this._btnNext.Location = new System.Drawing.Point(487, 51);
			this._btnNext.Name = "_btnNext";
			this._btnNext.NormalEndColor = System.Drawing.Color.White;
			this._btnNext.NormalStartColor = System.Drawing.Color.White;
			this._btnNext.Rotation = 90;
			this._btnNext.Size = new System.Drawing.Size(43, 43);
			this._btnNext.StubbyStyle = false;
			this._btnNext.TabIndex = 13;
			this._btnNext.Click += new System.EventHandler(this._btnNext_Click);
			//
			// _btnAddWord
			//
			this._btnAddWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddWord.ArrowEnabled = true;
			this._btnAddWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._btnAddWord.HoverEndColor = System.Drawing.Color.Blue;
			this._btnAddWord.HoverStartColor = System.Drawing.Color.White;
			this._btnAddWord.Location = new System.Drawing.Point(450, 321);
			this._btnAddWord.Name = "_btnAddWord";
			this._btnAddWord.NormalEndColor = System.Drawing.Color.White;
			this._btnAddWord.NormalStartColor = System.Drawing.Color.White;
			this._btnAddWord.Rotation = 270;
			this._btnAddWord.Size = new System.Drawing.Size(80, 80);
			this._btnAddWord.StubbyStyle = true;
			this._btnAddWord.TabIndex = 10;
			this._btnAddWord.Text = "   +";
			this._btnAddWord.Click += new System.EventHandler(this._btnAddWord_Click);
			//
			// _question
			//
			this._question.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._question.AutoEllipsis = true;
			this._question.BackColor = System.Drawing.Color.MistyRose;
			this._question.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._question.Location = new System.Drawing.Point(15, 179);
			this._question.Name = "_question";
			this._question.Size = new System.Drawing.Size(441, 51);
			this._question.TabIndex = 23;
			this._question.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			//
			// _description
			//
			this._description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._description.AutoEllipsis = true;
			this._description.BackColor = System.Drawing.Color.MistyRose;
			this._description.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._description.Location = new System.Drawing.Point(15, 79);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(441, 100);
			this._description.TabIndex = 24;
			//
			// _congratulationsControl
			//
			this._congratulationsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._congratulationsControl.Location = new System.Drawing.Point(3, 43);
			this._congratulationsControl.Name = "_congratulationsControl";
			this._congratulationsControl.Size = new System.Drawing.Size(617, 340);
			this._congratulationsControl.TabIndex = 22;
			//
			// GatherBySemanticDomainsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._description);
			this.Controls.Add(this._question);
			this.Controls.Add(this._vernacularBox);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._domainName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._listViewWords);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._btnPrevious);
			this.Controls.Add(this._btnNext);
			this.Controls.Add(this._btnAddWord);
			this.Controls.Add(this._congratulationsControl);
			this.Name = "GatherBySemanticDomainsControl";
			this.Size = new System.Drawing.Size(654, 386);
			this.BackColorChanged += new System.EventHandler(this.GatherWordListControl_BackColorChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private WeSay.UI.MultiTextControl _vernacularBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label _domainName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox _listViewWords;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private ArrowButton.ArrowButton _btnPrevious;
		private ArrowButton.ArrowButton _btnNext;
		private ArrowButton.ArrowButton _btnAddWord;
		private CongratulationsControl _congratulationsControl;
		private System.Windows.Forms.Label _question;
		private System.Windows.Forms.Label _description;
	}
}
