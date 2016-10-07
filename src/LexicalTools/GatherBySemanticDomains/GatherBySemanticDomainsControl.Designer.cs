using System;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.Widgets.Flying;
using WeSay.Project;
using WeSay.UI;
using WeSay.UI.Buttons;
using WeSay.UI.TextBoxes;

namespace WeSay.LexicalTools.GatherBySemanticDomains
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ListViewItem listViewItem61 = new System.Windows.Forms.ListViewItem("blah");
			System.Windows.Forms.ListViewItem listViewItem62 = new System.Windows.Forms.ListViewItem("stuff");
			this._domainListComboBox = WeSayWordsProject.Project.ServiceLocator.GetService(typeof(IWeSayComboBox)) as IWeSayComboBox;
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._instructionLabel = new System.Windows.Forms.Label();
			this._question = (Control)(WeSayWordsProject.Project.ServiceLocator.GetService(typeof(IWeSayTextBox)) as IWeSayTextBox);
			this._description = new System.Windows.Forms.Label();
			this._reminder = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._meaningLabel = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._questionIndicator = new WeSay.UI.CirclesProgressIndicator();
			this.panel1 = new System.Windows.Forms.Panel();
			this._btnNext = new WeSay.UI.Buttons.RectangularImageButton();
			this._btnPrevious = new WeSay.UI.Buttons.RectangularImageButton();
			this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
			this._vernacularBox = new WeSay.UI.TextBoxes.MultiTextControl(null, WeSayWordsProject.Project.ServiceLocator);
			this._meaningBox = new WeSay.UI.TextBoxes.MultiTextControl(null, WeSayWordsProject.Project.ServiceLocator);
			this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
			this._btnAddWord = new WeSay.UI.Buttons.RectangularImageButton();
			this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
			this._listViewWords = WeSayWordsProject.Project.ServiceLocator.GetService(typeof(IWeSayListBox)) as IWeSayListBox;
			this._flyingLabel = new Palaso.UI.WindowsForms.Widgets.Flying.FlyingLabel();
			this.multiTextControl2 = new WeSay.UI.TextBoxes.MultiTextControl(null, WeSayWordsProject.Project.ServiceLocator);
			this.multiTextControl1 = new WeSay.UI.TextBoxes.MultiTextControl(null, WeSayWordsProject.Project.ServiceLocator);
			this.multiTextControl3 = new WeSay.UI.TextBoxes.MultiTextControl(null, WeSayWordsProject.Project.ServiceLocator);
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			this.tableLayoutPanel6.SuspendLayout();
			this.tableLayoutPanel8.SuspendLayout();
			this.tableLayoutPanel7.SuspendLayout();
			this.SuspendLayout();
			//
			// _domainListComboBox
			//
			this._domainListComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
																				| System.Windows.Forms.AnchorStyles.Right)));
			this._domainListComboBox.BackColor = System.Drawing.SystemColors.Control;
			this._domainListComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._domainListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._domainListComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._domainListComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._domainListComboBox.Location = new System.Drawing.Point(3, 3);
			this._domainListComboBox.Margin = new System.Windows.Forms.Padding(0);
			this._domainListComboBox.Name = "_domainListComboBox";
			this._domainListComboBox.Size = new System.Drawing.Size(434, 27);
			this._domainListComboBox.TabIndex = 20;
			this._domainListComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this._domainName_DrawItem);
			this._domainListComboBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this._domainName_MeasureItem);
			this._domainListComboBox.SelectedValueChanged += new System.EventHandler(this._domainName_SelectedIndexChanged);
			//
			// label3
			//
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
			this.label3.Location = new System.Drawing.Point(3, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 15);
			this.label3.TabIndex = 19;
			this.label3.Text = "Word";
			//
			// label4
			//
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(118)))), ((int)(((byte)(17)))));
			this.label4.Location = new System.Drawing.Point(83, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(73, 16);
			this.label4.TabIndex = 14;
			this.label4.Text = "(Enter Key)";
			//
			// _instructionLabel
			//
			this._instructionLabel.AutoSize = true;
			this._instructionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F);
			this._instructionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(118)))), ((int)(((byte)(17)))));
			this._instructionLabel.Location = new System.Drawing.Point(3, 0);
			this._instructionLabel.Name = "_instructionLabel";
			this._instructionLabel.Size = new System.Drawing.Size(423, 20);
			this._instructionLabel.TabIndex = 15;
			this._instructionLabel.Text = "Try thinking of words you use to talk about these things.";
			//
			// _question
			//
			this._question.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._question.BackColor = System.Drawing.Color.LightSeaGreen;
			this._question.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.75F, System.Drawing.FontStyle.Bold);
			this._question.Location = new System.Drawing.Point(3, 140);
			this._question.Name = "_question";
			this._question.Size = new System.Drawing.Size(400, 40);
			this._question.TabIndex = 23;
			((IWeSayTextBox)this._question).ReadOnly = true;
			//
			// _description
			//
			this._description.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._description.AutoEllipsis = true;
			this._description.AutoSize = true;
			this._description.BackColor = System.Drawing.Color.MistyRose;
			this._description.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F);
			this._description.Location = new System.Drawing.Point(3, 123);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(151, 17);
			this._description.TabIndex = 24;
			this._description.Text = "Description goes here.";
			//
			// _reminder
			//
			this._reminder.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._reminder.AutoEllipsis = true;
			this._reminder.AutoSize = true;
			this._reminder.BackColor = System.Drawing.Color.LightYellow;
			this._reminder.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._reminder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(118)))), ((int)(((byte)(17)))));
			this._reminder.Location = new System.Drawing.Point(3, 158);
			this._reminder.Name = "_reminder";
			this._reminder.Size = new System.Drawing.Size(242, 18);
			this._reminder.TabIndex = 26;
			this._reminder.Text = "\"Don\'t translate\" warning goes here.";
			//
			// label5
			//
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(118)))), ((int)(((byte)(17)))));
			this.label5.Location = new System.Drawing.Point(530, 43);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(112, 16);
			this.label5.TabIndex = 16;
			this.label5.Text = "(Page Down Key)";
			//
			// _meaningLabel
			//
			this._meaningLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._meaningLabel.AutoSize = true;
			this._meaningLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold);
			this._meaningLabel.Location = new System.Drawing.Point(3, 15);
			this._meaningLabel.Name = "_meaningLabel";
			this._meaningLabel.Size = new System.Drawing.Size(63, 15);
			this._meaningLabel.TabIndex = 19;
			this._meaningLabel.Text = "Meaning";
			//
			// tableLayoutPanel1
			//
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label5, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 20);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(645, 103);
			this.tableLayoutPanel1.TabIndex = 28;
			//
			// tableLayoutPanel2
			//
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this._questionIndicator, 0, 1);
			this.tableLayoutPanel2.Controls.Add(((System.Windows.Forms.Control)this._domainListComboBox), 0, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 34);
			this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 34, 3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(434, 54);
			this.tableLayoutPanel2.TabIndex = 29;
			//
			// _questionIndicator
			//
			this._questionIndicator.AutoSize = true;
			this._questionIndicator.BulletColor = System.Drawing.Color.Azure;
			this._questionIndicator.BulletColorEnd = System.Drawing.Color.MediumBlue;
			this._questionIndicator.BulletPadding = new System.Windows.Forms.Padding(1);
			this._questionIndicator.Location = new System.Drawing.Point(3, 30);
			this._questionIndicator.Name = "_questionIndicator";
			this._questionIndicator.Size = new System.Drawing.Size(70, 7);
			this._questionIndicator.TabIndex = 0;
			this._questionIndicator.TabStop = false;
			//
			// panel1
			//
			this.panel1.Controls.Add(this._btnNext);
			this.panel1.Controls.Add(this._btnPrevious);
			this.panel1.ForeColor = System.Drawing.Color.Maroon;
			this.panel1.Location = new System.Drawing.Point(443, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(81, 97);
			this.panel1.TabIndex = 30;
			//
			// _btnNext
			//
			this._btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnNext.Image = global::WeSay.LexicalTools.Properties.Resources.right_arrow;
			this._btnNext.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._btnNext.Location = new System.Drawing.Point(36, 28);
			this._btnNext.Name = "_btnNext";
			this._btnNext.Size = new System.Drawing.Size(44, 52);
			this._btnNext.TabIndex = 14;
			this._btnNext.UseVisualStyleBackColor = true;
			this._btnNext.Click += new System.EventHandler(this._btnNext_Click);
			//
			// _btnPrevious
			//
			this._btnPrevious.Anchor = System.Windows.Forms.AnchorStyles.None;
			this._btnPrevious.BackColor = System.Drawing.Color.Transparent;
			this._btnPrevious.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this._btnPrevious.Enabled = false;
			this._btnPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnPrevious.ForeColor = System.Drawing.Color.Transparent;
			this._btnPrevious.Image = global::WeSay.LexicalTools.Properties.Resources.left_arrow;
			this._btnPrevious.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._btnPrevious.Location = new System.Drawing.Point(6, 36);
			this._btnPrevious.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this._btnPrevious.Name = "_btnPrevious";
			this._btnPrevious.Size = new System.Drawing.Size(30, 25);
			this._btnPrevious.TabIndex = 13;
			this._btnPrevious.UseVisualStyleBackColor = false;
			this._btnPrevious.Click += new System.EventHandler(this._btnPrevious_Click);
			//
			// tableLayoutPanel5
			//
			this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
																				| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel5.AutoSize = true;
			this.tableLayoutPanel5.ColumnCount = 2;
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 0);
			this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel8, 1, 0);
			this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 344);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			this.tableLayoutPanel5.RowCount = 1;
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.Size = new System.Drawing.Size(648, 39);
			this.tableLayoutPanel5.TabIndex = 30;
			//
			// tableLayoutPanel6
			//
			this.tableLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel6.AutoSize = true;
			this.tableLayoutPanel6.ColumnCount = 2;
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel6.Controls.Add(this._vernacularBox, 1, 0);
			this.tableLayoutPanel6.Controls.Add(this._meaningBox, 1, 1);
			this.tableLayoutPanel6.Controls.Add(this._meaningLabel, 0, 1);
			this.tableLayoutPanel6.Controls.Add(this.label3, 0, 0);
			this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 4);
			this.tableLayoutPanel6.Name = "tableLayoutPanel6";
			this.tableLayoutPanel6.RowCount = 2;
			this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel6.Size = new System.Drawing.Size(477, 30);
			this.tableLayoutPanel6.TabIndex = 1;
			//
			// _vernacularBox
			//
			this._vernacularBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._vernacularBox.AutoSize = true;
			this._vernacularBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._vernacularBox.BackColor = System.Drawing.Color.Maroon;
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
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._vernacularBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._vernacularBox.IsSpellCheckingEnabled = false;
			this._vernacularBox.Location = new System.Drawing.Point(72, 7);
			this._vernacularBox.Name = "_vernacularBox";
			this._vernacularBox.ShowAnnotationWidget = false;
			this._vernacularBox.Size = new System.Drawing.Size(402, 0);
			this._vernacularBox.TabIndex = 1;
			this._vernacularBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._boxVernacularWord_KeyDown);
			//
			// _meaningBox
			//
			this._meaningBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._meaningBox.AutoSize = true;
			this._meaningBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._meaningBox.BackColor = System.Drawing.Color.Maroon;
			this._meaningBox.ColumnCount = 3;
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._meaningBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._meaningBox.IsSpellCheckingEnabled = false;
			this._meaningBox.Location = new System.Drawing.Point(72, 22);
			this._meaningBox.Name = "_meaningBox";
			this._meaningBox.ShowAnnotationWidget = false;
			this._meaningBox.Size = new System.Drawing.Size(402, 0);
			this._meaningBox.TabIndex = 1;
			this._meaningBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._boxVernacularWord_KeyDown);
			//
			// tableLayoutPanel8
			//
			this.tableLayoutPanel8.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.tableLayoutPanel8.AutoSize = true;
			this.tableLayoutPanel8.ColumnCount = 2;
			this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel8.Controls.Add(this._btnAddWord, 0, 0);
			this.tableLayoutPanel8.Controls.Add(this.label4, 1, 0);
			this.tableLayoutPanel8.Location = new System.Drawing.Point(486, 3);
			this.tableLayoutPanel8.Name = "tableLayoutPanel8";
			this.tableLayoutPanel8.RowCount = 1;
			this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel8.Size = new System.Drawing.Size(159, 33);
			this.tableLayoutPanel8.TabIndex = 32;
			//
			// _btnAddWord
			//
			this._btnAddWord.Anchor = System.Windows.Forms.AnchorStyles.None;
			this._btnAddWord.BackColor = System.Drawing.Color.Transparent;
			this._btnAddWord.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this._btnAddWord.FlatAppearance.BorderSize = 0;
			this._btnAddWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._btnAddWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._btnAddWord.ForeColor = System.Drawing.Color.Transparent;
			this._btnAddWord.Image = global::WeSay.LexicalTools.Properties.Resources.AddWord;
			this._btnAddWord.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._btnAddWord.Location = new System.Drawing.Point(13, 3);
			this._btnAddWord.Name = "_btnAddWord";
			this._btnAddWord.Size = new System.Drawing.Size(54, 27);
			this._btnAddWord.TabIndex = 10;
			this._btnAddWord.UseVisualStyleBackColor = false;
			this._btnAddWord.Click += new System.EventHandler(this._btnAddWord_Click);
			//
			// tableLayoutPanel7
			//
			this.tableLayoutPanel7.ColumnCount = 1;
			this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel7.Controls.Add(this._instructionLabel, 0, 0);
			this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel5, 0, 6);
			this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel1, 0, 1);
			this.tableLayoutPanel7.Controls.Add(this._description, 0, 2);
			this.tableLayoutPanel7.Controls.Add(this._question, 0, 3);
			this.tableLayoutPanel7.Controls.Add(this._reminder, 0, 4);
			this.tableLayoutPanel7.Controls.Add(this._listViewWords.Control, 0, 5);
			this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel7.MaximumSize = new System.Drawing.Size(2048, 600);
			this.tableLayoutPanel7.Name = "tableLayoutPanel7";
			this.tableLayoutPanel7.RowCount = 7;
			this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel7.Size = new System.Drawing.Size(654, 386);
			this.tableLayoutPanel7.TabIndex = 31;
			//
			// _listViewWords
			//
			this._listViewWords.ColumnWidth = 100;
			this._listViewWords.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listViewWords.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._listViewWords.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._listViewWords.ItemHeight = 20;
			this._listViewWords.AddRange(new object[] {
			listViewItem61,
			listViewItem62});
			this._listViewWords.ItemToNotDrawYet = null;
			this._listViewWords.Location = new System.Drawing.Point(3, 179);
			this._listViewWords.MultiColumn = true;
			this._listViewWords.Name = "_listViewWords";
			this._listViewWords.Size = new System.Drawing.Size(648, 159);
			this._listViewWords.TabIndex = 17;
			this._listViewWords.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._listViewWords_KeyPress);
			//
			// _flyingLabel
			//
			this._flyingLabel.BackColor = System.Drawing.Color.Transparent;
			this._flyingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this._flyingLabel.Location = new System.Drawing.Point(0, 0);
			this._flyingLabel.Name = "_flyingLabel";
			this._flyingLabel.Size = new System.Drawing.Size(100, 23);
			this._flyingLabel.TabIndex = 25;
			this._flyingLabel.Visible = false;
			//
			// multiTextControl2
			//
			this.multiTextControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
																				| System.Windows.Forms.AnchorStyles.Right)));
			this.multiTextControl2.AutoSize = true;
			this.multiTextControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.multiTextControl2.BackColor = System.Drawing.Color.Maroon;
			this.multiTextControl2.ColumnCount = 3;
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl2.IsSpellCheckingEnabled = false;
			this.multiTextControl2.Location = new System.Drawing.Point(68, 374);
			this.multiTextControl2.Name = "multiTextControl2";
			this.multiTextControl2.ShowAnnotationWidget = false;
			this.multiTextControl2.Size = new System.Drawing.Size(200, 0);
			this.multiTextControl2.TabIndex = 1;
			this.multiTextControl2.KeyDown += new System.Windows.Forms.KeyEventHandler(this._boxVernacularWord_KeyDown);
			//
			// multiTextControl1
			//
			this.multiTextControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
																				| System.Windows.Forms.AnchorStyles.Right)));
			this.multiTextControl1.AutoSize = true;
			this.multiTextControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.multiTextControl1.BackColor = System.Drawing.Color.White;
			this.multiTextControl1.ColumnCount = 3;
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl1.IsSpellCheckingEnabled = false;
			this.multiTextControl1.Location = new System.Drawing.Point(297, 885);
			this.multiTextControl1.Name = "multiTextControl1";
			this.multiTextControl1.ShowAnnotationWidget = false;
			this.multiTextControl1.Size = new System.Drawing.Size(200, 0);
			this.multiTextControl1.TabIndex = 27;
			//
			// multiTextControl3
			//
			this.multiTextControl3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
											| System.Windows.Forms.AnchorStyles.Right)));
			this.multiTextControl3.AutoSize = true;
			this.multiTextControl3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.multiTextControl3.BackColor = System.Drawing.Color.Maroon;
			this.multiTextControl3.ColumnCount = 3;
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.multiTextControl3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.multiTextControl3.IsSpellCheckingEnabled = false;
			this.multiTextControl3.Location = new System.Drawing.Point(346, 374);
			this.multiTextControl3.Name = "multiTextControl3";
			this.multiTextControl3.ShowAnnotationWidget = false;
			this.multiTextControl3.Size = new System.Drawing.Size(140, 0);
			this.multiTextControl3.TabIndex = 1;
			this.multiTextControl3.KeyDown += new System.Windows.Forms.KeyEventHandler(this._boxVernacularWord_KeyDown);
			//
			// GatherBySemanticDomainsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.tableLayoutPanel7);
			this.Controls.Add(this.multiTextControl2);
			this.Controls.Add(this.multiTextControl1);
			this.Controls.Add(this.multiTextControl3);
			this.Controls.Add(this._flyingLabel);
			this.Name = "GatherBySemanticDomainsControl";
			this.Size = new System.Drawing.Size(654, 386);
			this.Load += new System.EventHandler(this.GatherBySemanticDomainsControl_Load);
			this.BackColorChanged += new System.EventHandler(this.GatherWordListControl_BackColorChanged);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel5.ResumeLayout(false);
			this.tableLayoutPanel5.PerformLayout();
			this.tableLayoutPanel6.ResumeLayout(false);
			this.tableLayoutPanel6.PerformLayout();
			this.tableLayoutPanel8.ResumeLayout(false);
			this.tableLayoutPanel8.PerformLayout();
			this.tableLayoutPanel7.ResumeLayout(false);
			this.tableLayoutPanel7.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		#endregion


		private MultiTextControl _vernacularBox;
		private IWeSayComboBox _domainListComboBox;
		private System.Windows.Forms.Label label3;
		private IWeSayListBox _listViewWords;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label _instructionLabel;
		private RectangularImageButton _btnAddWord;
		private Control _question;
		private System.Windows.Forms.Label _description;
		private WeSay.UI.CirclesProgressIndicator _questionIndicator;
		private FlyingLabel _flyingLabel;
		private System.Windows.Forms.Label _reminder;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ToolTip toolTip1;
		private MultiTextControl multiTextControl1;
		private MultiTextControl _meaningBox;
		private System.Windows.Forms.Label _meaningLabel;
		private MultiTextControl multiTextControl2;
		private MultiTextControl multiTextControl3;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
		private System.Windows.Forms.Panel panel1;
		private RectangularImageButton _btnPrevious;
		private RectangularImageButton _btnNext;

	}
}
