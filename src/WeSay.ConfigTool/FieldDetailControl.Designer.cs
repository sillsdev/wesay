namespace WeSay.ConfigTool
{
	partial class FieldDetailControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._fieldName = new System.Windows.Forms.TextBox();
			this._displayLabel = new System.Windows.Forms.Label();
			this._fieldNameLabel = new System.Windows.Forms.Label();
			this._displayName = new System.Windows.Forms.TextBox();
			this._classLabel = new System.Windows.Forms.Label();
			this._dataTypeLabel = new System.Windows.Forms.Label();
			this._classNameCombo = new System.Windows.Forms.ComboBox();
			this._dataTypeCombo = new System.Windows.Forms.ComboBox();
			this._optionsFileName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this._writingSystemsControl = new WeSay.ConfigTool.WritingSystemForFieldControl();
			this._normallyHidden = new System.Windows.Forms.CheckBox();
			this._description = new System.Windows.Forms.TextBox();
			this._descriptionLabel = new System.Windows.Forms.Label();
			this._normallyHiddenLabel = new System.Windows.Forms.Label();
			this._enableSpellingLabel = new System.Windows.Forms.Label();
			this._optionListFileLabel = new System.Windows.Forms.Label();
			this._enableSpelling = new System.Windows.Forms.CheckBox();
			this.spellingNotEnabledWarning = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this._multiParagraph = new System.Windows.Forms.CheckBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			//
			// tableLayoutPanel1
			//
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.66667F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.33333F));
			this.tableLayoutPanel1.Controls.Add(this._fieldName, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._displayLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._fieldNameLabel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._displayName, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._classLabel, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._dataTypeLabel, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._classNameCombo, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this._dataTypeCombo, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this._optionsFileName, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this._normallyHidden, 1, 8);
			this.tableLayoutPanel1.Controls.Add(this._normallyHiddenLabel, 0, 8);
			this.tableLayoutPanel1.Controls.Add(this._enableSpellingLabel, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this._optionListFileLabel, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this._enableSpelling, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.spellingNotEnabledWarning, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this._description, 1, 11);
			this.tableLayoutPanel1.Controls.Add(this._descriptionLabel, 0, 11);
			this.tableLayoutPanel1.Controls.Add(this.label6, 0, 10);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 9);
			this.tableLayoutPanel1.Controls.Add(this._writingSystemsControl, 1, 10);
			this.tableLayoutPanel1.Controls.Add(this._multiParagraph, 1, 9);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 17);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 12;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(300, 513);
			this.tableLayoutPanel1.TabIndex = 0;
			//
			// _fieldName
			//
			this._fieldName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._fieldName.Location = new System.Drawing.Point(119, 29);
			this._fieldName.Name = "_fieldName";
			this._fieldName.Size = new System.Drawing.Size(178, 20);
			this._fieldName.TabIndex = 1;
			this.toolTip1.SetToolTip(this._fieldName, "The name that will be used in the LIFT xml file. Only a small set of punctuation " +
					"characters are allowed.");
			this._fieldName.TextChanged += new System.EventHandler(this._fieldName_TextChanged);
			//
			// _displayLabel
			//
			this._displayLabel.AutoSize = true;
			this._displayLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._displayLabel.Location = new System.Drawing.Point(3, 0);
			this._displayLabel.Name = "_displayLabel";
			this._displayLabel.Size = new System.Drawing.Size(110, 16);
			this._displayLabel.TabIndex = 0;
			this._displayLabel.Text = "Name for display";
			this.toolTip1.SetToolTip(this._displayLabel, "Name as the user will see it");
			//
			// _fieldNameLabel
			//
			this._fieldNameLabel.AutoSize = true;
			this._fieldNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._fieldNameLabel.Location = new System.Drawing.Point(3, 26);
			this._fieldNameLabel.Name = "_fieldNameLabel";
			this._fieldNameLabel.Size = new System.Drawing.Size(78, 16);
			this._fieldNameLabel.TabIndex = 1;
			this._fieldNameLabel.Text = "Name in file";
			this.toolTip1.SetToolTip(this._fieldNameLabel, "Name as it will be stored in the xml file");
			//
			// _displayName
			//
			this._displayName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._displayName.Location = new System.Drawing.Point(119, 3);
			this._displayName.Name = "_displayName";
			this._displayName.Size = new System.Drawing.Size(178, 20);
			this._displayName.TabIndex = 0;
			this.toolTip1.SetToolTip(this._displayName, "The name the user will see.");
			this._displayName.TextChanged += new System.EventHandler(this.OnDisplayName_TextChanged);
			this._displayName.Leave += new System.EventHandler(this.OnLeaveDisplayName);
			//
			// _classLabel
			//
			this._classLabel.AutoSize = true;
			this._classLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._classLabel.Location = new System.Drawing.Point(3, 52);
			this._classLabel.Name = "_classLabel";
			this._classLabel.Size = new System.Drawing.Size(72, 16);
			this._classLabel.TabIndex = 4;
			this._classLabel.Text = "Belongs to";
			this.toolTip1.SetToolTip(this._classLabel, "The part of the entry that this field is a part of.");
			//
			// _dataTypeLabel
			//
			this._dataTypeLabel.AutoSize = true;
			this._dataTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._dataTypeLabel.Location = new System.Drawing.Point(3, 79);
			this._dataTypeLabel.Name = "_dataTypeLabel";
			this._dataTypeLabel.Size = new System.Drawing.Size(72, 16);
			this._dataTypeLabel.TabIndex = 5;
			this._dataTypeLabel.Text = "Data Type";
			this.toolTip1.SetToolTip(this._dataTypeLabel, "The kind of data goes in this field.");
			//
			// _classNameCombo
			//
			this._classNameCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._classNameCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._classNameCombo.FormattingEnabled = true;
			this._classNameCombo.Location = new System.Drawing.Point(119, 55);
			this._classNameCombo.Name = "_classNameCombo";
			this._classNameCombo.Size = new System.Drawing.Size(178, 21);
			this._classNameCombo.TabIndex = 2;
			this._classNameCombo.SelectedIndexChanged += new System.EventHandler(this._classNameCombo_SelectedIndexChanged);
			//
			// _dataTypeCombo
			//
			this._dataTypeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._dataTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._dataTypeCombo.FormattingEnabled = true;
			this._dataTypeCombo.Location = new System.Drawing.Point(119, 82);
			this._dataTypeCombo.Name = "_dataTypeCombo";
			this._dataTypeCombo.Size = new System.Drawing.Size(178, 21);
			this._dataTypeCombo.TabIndex = 3;
			this._dataTypeCombo.SelectedIndexChanged += new System.EventHandler(this.OnDataTypeCombo_SelectedIndexChanged);
			//
			// _optionsFileName
			//
			this._optionsFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._optionsFileName.Enabled = false;
			this._optionsFileName.Location = new System.Drawing.Point(119, 109);
			this._optionsFileName.Name = "_optionsFileName";
			this._optionsFileName.Size = new System.Drawing.Size(178, 20);
			this._optionsFileName.TabIndex = 4;
			this._optionsFileName.TextChanged += new System.EventHandler(this._optionsFileName_TextChanged);
			//
			// label6
			//
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(3, 230);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(104, 16);
			this.label6.TabIndex = 10;
			this.label6.Text = "Writing Systems";
			this.toolTip1.SetToolTip(this.label6, "Mark which writing systems to show for this field.");
			//
			// _writingSystemsControl
			//
			this._writingSystemsControl.CurrentField = null;
			this._writingSystemsControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._writingSystemsControl.Location = new System.Drawing.Point(119, 233);
			this._writingSystemsControl.Name = "_writingSystemsControl";
			this._writingSystemsControl.Size = new System.Drawing.Size(178, 100);
			this._writingSystemsControl.TabIndex = 6;
			//
			// _normallyHidden
			//
			this._normallyHidden.AutoSize = true;
			this._normallyHidden.Location = new System.Drawing.Point(119, 187);
			this._normallyHidden.Name = "_normallyHidden";
			this._normallyHidden.Size = new System.Drawing.Size(140, 17);
			this._normallyHidden.TabIndex = 7;
			this._normallyHidden.Text = "Normally hidden if empty";
			this.toolTip1.SetToolTip(this._normallyHidden, ".");
			this._normallyHidden.UseVisualStyleBackColor = true;
			this._normallyHidden.CheckedChanged += new System.EventHandler(this._normallyHidden_CheckedChanged);
			//
			// _description
			//
			this._description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._description.Enabled = false;
			this._description.Location = new System.Drawing.Point(119, 339);
			this._description.Multiline = true;
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(178, 99);
			this._description.TabIndex = 5;
			this._description.TextChanged += new System.EventHandler(this._description_TextChanged);
			//
			// _descriptionLabel
			//
			this._descriptionLabel.AutoSize = true;
			this._descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._descriptionLabel.Location = new System.Drawing.Point(3, 336);
			this._descriptionLabel.Name = "_descriptionLabel";
			this._descriptionLabel.Size = new System.Drawing.Size(76, 16);
			this._descriptionLabel.TabIndex = 9;
			this._descriptionLabel.Text = "Description";
			this.toolTip1.SetToolTip(this._descriptionLabel, "Information about the use of this field.");
			//
			// _normallyHiddenLabel
			//
			this._normallyHiddenLabel.AutoSize = true;
			this._normallyHiddenLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._normallyHiddenLabel.Location = new System.Drawing.Point(3, 184);
			this._normallyHiddenLabel.Name = "_normallyHiddenLabel";
			this._normallyHiddenLabel.Size = new System.Drawing.Size(57, 16);
			this._normallyHiddenLabel.TabIndex = 10;
			this._normallyHiddenLabel.Text = "Visibility";
			this.toolTip1.SetToolTip(this._normallyHiddenLabel, "Tick this box for fields which are  not used in most records");
			//
			// _enableSpellingLabel
			//
			this._enableSpellingLabel.AutoSize = true;
			this._enableSpellingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._enableSpellingLabel.Location = new System.Drawing.Point(3, 132);
			this._enableSpellingLabel.Name = "_enableSpellingLabel";
			this._enableSpellingLabel.Size = new System.Drawing.Size(57, 16);
			this._enableSpellingLabel.TabIndex = 11;
			this._enableSpellingLabel.Text = "Spelling";
			this.toolTip1.SetToolTip(this._enableSpellingLabel, "Tick this box to enable spell checking for writing systems with installed spell c" +
					"hecker support");
			//
			// _optionListFileLabel
			//
			this._optionListFileLabel.AutoSize = true;
			this._optionListFileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._optionListFileLabel.Location = new System.Drawing.Point(3, 106);
			this._optionListFileLabel.Name = "_optionListFileLabel";
			this._optionListFileLabel.Size = new System.Drawing.Size(95, 16);
			this._optionListFileLabel.TabIndex = 8;
			this._optionListFileLabel.Text = "Option List File";
			this.toolTip1.SetToolTip(this._optionListFileLabel, "Options are stored in a xml file under this name.");
			//
			// _enableSpelling
			//
			this._enableSpelling.AutoSize = true;
			this._enableSpelling.Location = new System.Drawing.Point(119, 135);
			this._enableSpelling.Name = "_enableSpelling";
			this._enableSpelling.Size = new System.Drawing.Size(130, 17);
			this._enableSpelling.TabIndex = 12;
			this._enableSpelling.Text = "Enable spell checking";
			this._enableSpelling.UseVisualStyleBackColor = true;
			this._enableSpelling.CheckedChanged += new System.EventHandler(this._enableSpelling_CheckedChanged);
			//
			// spellingNotEnabledWarning
			//
			this.spellingNotEnabledWarning.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.spellingNotEnabledWarning, 2);
			this.spellingNotEnabledWarning.ForeColor = System.Drawing.Color.Crimson;
			this.spellingNotEnabledWarning.Location = new System.Drawing.Point(3, 155);
			this.spellingNotEnabledWarning.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.spellingNotEnabledWarning.Name = "spellingNotEnabledWarning";
			this.spellingNotEnabledWarning.Size = new System.Drawing.Size(263, 26);
			this.spellingNotEnabledWarning.TabIndex = 13;
			this.spellingNotEnabledWarning.Text = "Spellchecking has not been installed on this machine. Please install Enchant to e" +
				"nable this feature.";
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 207);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 9;
			this.label1.Text = "Paragraph";
			//
			// _multiParagraph
			//
			this._multiParagraph.AutoSize = true;
			this._multiParagraph.Location = new System.Drawing.Point(119, 210);
			this._multiParagraph.Name = "_multiParagraph";
			this._multiParagraph.Size = new System.Drawing.Size(145, 17);
			this._multiParagraph.TabIndex = 7;
			this._multiParagraph.Text = "Allow multiple paragraphs";
			this._multiParagraph.UseVisualStyleBackColor = true;
			this._multiParagraph.CheckedChanged += new System.EventHandler(this._multiParagraph_CheckedChanged);
			//
			// FieldDetailControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "FieldDetailControl";
			this.Size = new System.Drawing.Size(306, 566);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label _displayLabel;
		private System.Windows.Forms.Label _fieldNameLabel;
		private System.Windows.Forms.TextBox _displayName;
		private System.Windows.Forms.TextBox _fieldName;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label _classLabel;
		private System.Windows.Forms.Label _dataTypeLabel;
		private System.Windows.Forms.ComboBox _classNameCombo;
		private System.Windows.Forms.ComboBox _dataTypeCombo;
		private System.Windows.Forms.Label _optionListFileLabel;
		private System.Windows.Forms.TextBox _optionsFileName;
		private System.Windows.Forms.Label _descriptionLabel;
		private System.Windows.Forms.Label label6;
		internal System.Windows.Forms.TextBox _description;
		private WritingSystemForFieldControl _writingSystemsControl;
		private System.Windows.Forms.Label _normallyHiddenLabel;
		private System.Windows.Forms.CheckBox _normallyHidden;
		private System.Windows.Forms.Label _enableSpellingLabel;
		internal System.Windows.Forms.CheckBox _enableSpelling;
		private System.Windows.Forms.Label spellingNotEnabledWarning;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox _multiParagraph;
	}
}
