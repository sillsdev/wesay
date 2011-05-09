namespace WeSay.ConfigTool.NewProjectCreation
{
	partial class NewProjectFromFLExDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private readonly System.ComponentModel.IContainer components = null;

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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProjectFromFLExDialog));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._infoLabel = new WeSay.UI.BetterLabel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._liftPathTextBox = new System.Windows.Forms.TextBox();
			this._browseForLiftPathButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this._projectNameTextBox = new System.Windows.Forms.TextBox();
			this._pathtoProjectLabel = new System.Windows.Forms.Label();
			this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			this._btnOk = new System.Windows.Forms.Button();
			this._btnCancel = new System.Windows.Forms.Button();
			this._linkLabel = new System.Windows.Forms.LinkLabel();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			this.SuspendLayout();
			//
			// tableLayoutPanel1
			//
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._infoLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this._linkLabel, 0, 3);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(442, 298);
			this.tableLayoutPanel1.TabIndex = 0;
			//
			// _infoLabel
			//
			this._infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._infoLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._infoLabel.Location = new System.Drawing.Point(3, 3);
			this._infoLabel.Multiline = true;
			this._infoLabel.Name = "_infoLabel";
			this._infoLabel.ReadOnly = true;
			this._infoLabel.Size = new System.Drawing.Size(436, 115);
			this._infoLabel.TabIndex = 0;
			this._infoLabel.TabStop = false;
			this._infoLabel.Text = resources.GetString("_infoLabel.Text");
			//
			// tableLayoutPanel2
			//
			this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 124);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(436, 50);
			this.tableLayoutPanel2.TabIndex = 1;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(151, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Where is the LIFT file?";
			//
			// tableLayoutPanel3
			//
			this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this._liftPathTextBox, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this._browseForLiftPathButton, 1, 0);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 18);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(430, 29);
			this.tableLayoutPanel3.TabIndex = 1;
			//
			// _liftPathTextBox
			//
			this._liftPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._liftPathTextBox.Location = new System.Drawing.Point(3, 4);
			this._liftPathTextBox.Name = "_liftPathTextBox";
			this._liftPathTextBox.Size = new System.Drawing.Size(392, 20);
			this._liftPathTextBox.TabIndex = 0;
			//
			// _browseForLiftPathButton
			//
			this._browseForLiftPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._browseForLiftPathButton.AutoSize = true;
			this._browseForLiftPathButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._browseForLiftPathButton.Location = new System.Drawing.Point(401, 3);
			this._browseForLiftPathButton.Name = "_browseForLiftPathButton";
			this._browseForLiftPathButton.Size = new System.Drawing.Size(26, 23);
			this._browseForLiftPathButton.TabIndex = 1;
			this._browseForLiftPathButton.Text = "...";
			this._browseForLiftPathButton.UseVisualStyleBackColor = true;
			//
			// tableLayoutPanel4
			//
			this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel4.AutoSize = true;
			this.tableLayoutPanel4.ColumnCount = 1;
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this._projectNameTextBox, 0, 1);
			this.tableLayoutPanel4.Controls.Add(this._pathtoProjectLabel, 0, 2);
			this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 180);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 3;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.Size = new System.Drawing.Size(436, 54);
			this.tableLayoutPanel4.TabIndex = 2;
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(260, 15);
			this.label2.TabIndex = 0;
			this.label2.Text = "What would you like to call this project?";
			//
			// _projectNameTextBox
			//
			this._projectNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._projectNameTextBox.Location = new System.Drawing.Point(3, 18);
			this._projectNameTextBox.Name = "_projectNameTextBox";
			this._projectNameTextBox.Size = new System.Drawing.Size(430, 20);
			this._projectNameTextBox.TabIndex = 1;
			//
			// _pathtoProjectLabel
			//
			this._pathtoProjectLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._pathtoProjectLabel.Location = new System.Drawing.Point(3, 41);
			this._pathtoProjectLabel.Name = "_pathtoProjectLabel";
			this._pathtoProjectLabel.Size = new System.Drawing.Size(430, 13);
			this._pathtoProjectLabel.TabIndex = 2;
			this._pathtoProjectLabel.Text = "Path to new project:";
			//
			// tableLayoutPanel5
			//
			this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel5.AutoSize = true;
			this.tableLayoutPanel5.ColumnCount = 2;
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel5.Controls.Add(this._btnOk, 0, 0);
			this.tableLayoutPanel5.Controls.Add(this._btnCancel, 1, 0);
			this.tableLayoutPanel5.Location = new System.Drawing.Point(277, 266);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			this.tableLayoutPanel5.RowCount = 1;
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel5.Size = new System.Drawing.Size(162, 29);
			this.tableLayoutPanel5.TabIndex = 3;
			//
			// _btnOk
			//
			this._btnOk.Location = new System.Drawing.Point(3, 3);
			this._btnOk.Name = "_btnOk";
			this._btnOk.Size = new System.Drawing.Size(75, 23);
			this._btnOk.TabIndex = 0;
			this._btnOk.Text = "&Ok";
			this._btnOk.UseVisualStyleBackColor = true;
			//
			// _btnCancel
			//
			this._btnCancel.Location = new System.Drawing.Point(84, 3);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 1;
			this._btnCancel.Text = "&Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			//
			// _linkLabel
			//
			this._linkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._linkLabel.AutoSize = true;
			this._linkLabel.Location = new System.Drawing.Point(3, 237);
			this._linkLabel.Name = "_linkLabel";
			this._linkLabel.Size = new System.Drawing.Size(436, 26);
			this._linkLabel.TabIndex = 4;
			this._linkLabel.TabStop = true;
			this._linkLabel.Text = "Don\'t do this if you already have a corresponding WeSay project.  To learn how to" +
				" add changes from FLEx to an existing WeSay project, read this web page.";
			//
			// NewProjectFromFLExDialog
			//
			this.ClientSize = new System.Drawing.Size(442, 298);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "NewProjectFromFLExDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			this.tableLayoutPanel5.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private WeSay.UI.BetterLabel _infoLabel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.TextBox _liftPathTextBox;
		private System.Windows.Forms.Button _browseForLiftPathButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _projectNameTextBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
		private System.Windows.Forms.Button _btnOk;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Label _pathtoProjectLabel;
		private System.Windows.Forms.LinkLabel _linkLabel;
	}
}