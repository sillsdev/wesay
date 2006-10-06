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
			System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("stuff");
			System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("blah");
			this.label1 = new System.Windows.Forms.Label();
			this._listViewWords = new System.Windows.Forms.ListView();
			this.label3 = new System.Windows.Forms.Label();
			this._boxForeignWord = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this._boxVernacularWord = new WeSay.UI.WeSayTextBox();
			this._btnPreviousWord = new ArrowButton.ArrowButton();
			this._btnNextWord = new ArrowButton.ArrowButton();
			this._btnAddWord = new ArrowButton.ArrowButton();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.DarkGray;
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(406, 20);
			this.label1.TabIndex = 2;
			this.label1.Text = "Try thinking of ways to say these words in your language.";
			//
			// _listViewWords
			//
			this._listViewWords.BackColor = System.Drawing.Color.AliceBlue;
			this._listViewWords.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._listViewWords.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
			listViewItem11,
			listViewItem12});
			this._listViewWords.Location = new System.Drawing.Point(11, 169);
			this._listViewWords.Name = "_listViewWords";
			this._listViewWords.Size = new System.Drawing.Size(315, 97);
			this._listViewWords.TabIndex = 4;
			this._listViewWords.TabStop = false;
			this._listViewWords.UseCompatibleStateImageBehavior = false;
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(7, 121);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 20);
			this.label3.TabIndex = 6;
			this.label3.Text = "Thai:";
			//
			// _boxForeignWord
			//
			this._boxForeignWord.BackColor = System.Drawing.Color.AliceBlue;
			this._boxForeignWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._boxForeignWord.Location = new System.Drawing.Point(78, 69);
			this._boxForeignWord.Name = "_boxForeignWord";
			this._boxForeignWord.ReadOnly = true;
			this._boxForeignWord.Size = new System.Drawing.Size(248, 26);
			this._boxForeignWord.TabIndex = 7;
			this._boxForeignWord.TabStop = false;
			this._boxForeignWord.Text = "Foobar";
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(7, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 20);
			this.label2.TabIndex = 6;
			this.label2.Text = "English:";
			//
			// panel1
			//
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.LightGray;
			this.panel1.ForeColor = System.Drawing.Color.Transparent;
			this.panel1.Location = new System.Drawing.Point(0, 36);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(497, 1);
			this.panel1.TabIndex = 8;
			//
			// _boxVernacularWord
			//
			this._boxVernacularWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._boxVernacularWord.Location = new System.Drawing.Point(78, 117);
			this._boxVernacularWord.Name = "_boxVernacularWord";
			this._boxVernacularWord.Size = new System.Drawing.Size(248, 26);
			this._boxVernacularWord.TabIndex = 0;
			this._boxVernacularWord.TextChanged += new System.EventHandler(this._boxVernacularWord_TextChanged_1);
			this._boxVernacularWord.KeyDown += new System.Windows.Forms.KeyEventHandler(this._boxVernacularWord_KeyDown);
			//
			// _btnPreviousWord
			//
			this._btnPreviousWord.ArrowEnabled = true;
			this._btnPreviousWord.HoverEndColor = System.Drawing.Color.PeachPuff;
			this._btnPreviousWord.HoverStartColor = System.Drawing.Color.Salmon;
			this._btnPreviousWord.Location = new System.Drawing.Point(332, 69);
			this._btnPreviousWord.Name = "_btnPreviousWord";
			this._btnPreviousWord.NormalEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(235)))), ((int)(((byte)(214)))));
			this._btnPreviousWord.NormalStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(214)))));
			this._btnPreviousWord.Rotation = 270;
			this._btnPreviousWord.Size = new System.Drawing.Size(24, 24);
			this._btnPreviousWord.StubbyStyle = false;
			this._btnPreviousWord.TabIndex = 1;
			this._btnPreviousWord.Click += new System.EventHandler(this._btnPreviousWord_Click);
			//
			// _btnNextWord
			//
			this._btnNextWord.ArrowEnabled = true;
			this._btnNextWord.HoverEndColor = System.Drawing.Color.PeachPuff;
			this._btnNextWord.HoverStartColor = System.Drawing.Color.Salmon;
			this._btnNextWord.Location = new System.Drawing.Point(356, 60);
			this._btnNextWord.Name = "_btnNextWord";
			this._btnNextWord.NormalEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(235)))), ((int)(((byte)(214)))));
			this._btnNextWord.NormalStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(214)))));
			this._btnNextWord.Rotation = 90;
			this._btnNextWord.Size = new System.Drawing.Size(43, 43);
			this._btnNextWord.StubbyStyle = false;
			this._btnNextWord.TabIndex = 1;
			this._btnNextWord.Click += new System.EventHandler(this._btnNextWord_Click);
			//
			// _btnAddWord
			//
			this._btnAddWord.ArrowEnabled = true;
			this._btnAddWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._btnAddWord.HoverEndColor = System.Drawing.Color.PeachPuff;
			this._btnAddWord.HoverStartColor = System.Drawing.Color.Salmon;
			this._btnAddWord.Location = new System.Drawing.Point(319, 88);
			this._btnAddWord.Name = "_btnAddWord";
			this._btnAddWord.NormalEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(235)))), ((int)(((byte)(214)))));
			this._btnAddWord.NormalStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(214)))));
			this._btnAddWord.Rotation = 270;
			this._btnAddWord.Size = new System.Drawing.Size(80, 80);
			this._btnAddWord.StubbyStyle = true;
			this._btnAddWord.TabIndex = 0;
			this._btnAddWord.Text = "   +";
			this._btnAddWord.Click += new System.EventHandler(this._btnAddWord_Click);
			//
			// label4
			//
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.DarkGray;
			this.label4.Location = new System.Drawing.Point(405, 120);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(44, 15);
			this.label4.TabIndex = 2;
			this.label4.Text = "(Enter)";
			//
			// label5
			//
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.DarkGray;
			this.label5.Location = new System.Drawing.Point(405, 72);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(79, 15);
			this.label5.TabIndex = 2;
			this.label5.Text = "(Page Down)";
			//
			// GatherWordListControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.AliceBlue;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._boxForeignWord);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._boxVernacularWord);
			this.Controls.Add(this._listViewWords);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._btnPreviousWord);
			this.Controls.Add(this._btnNextWord);
			this.Controls.Add(this._btnAddWord);
			this.Name = "GatherWordListControl";
			this.Size = new System.Drawing.Size(499, 429);
			this.Load += new System.EventHandler(this.GatherWordListControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ArrowButton.ArrowButton _btnAddWord;
		private ArrowButton.ArrowButton _btnNextWord;
		private ArrowButton.ArrowButton _btnPreviousWord;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView _listViewWords;
		private WeSay.UI.WeSayTextBox _boxVernacularWord;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _boxForeignWord;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;

	 }
}
