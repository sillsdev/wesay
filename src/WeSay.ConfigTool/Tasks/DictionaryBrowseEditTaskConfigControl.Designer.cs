namespace WeSay.ConfigTool.Tasks
{
	partial class DictionaryBrowseEditTaskConfigControl
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
			this._definition = new System.Windows.Forms.RadioButton();
			this._storeMeaning = new System.Windows.Forms.GroupBox();
			this._gloss = new System.Windows.Forms.RadioButton();
			this._storeMeaning.SuspendLayout();
			this.SuspendLayout();
			//
			// _setupLabel
			//
			this._setupLabel.Enabled = false;
			this._setupLabel.Location = new System.Drawing.Point(84, 185);
			this._setupLabel.Size = new System.Drawing.Size(94, 13);
			this._setupLabel.Text = "Meaning Field: ";
			//
			// _definition
			//
			this._definition.AutoSize = true;
			this._definition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._definition.Location = new System.Drawing.Point(16, 19);
			this._definition.Name = "_definition";
			this._definition.Size = new System.Drawing.Size(69, 17);
			this._definition.TabIndex = 24;
			this._definition.TabStop = true;
			this._definition.Text = "Definition";
			this._definition.UseVisualStyleBackColor = true;
			this._definition.Click += new System.EventHandler(this.OnDefinition_RadioClicked);
			//
			// _storeMeaning
			//
			this._storeMeaning.Controls.Add(this._gloss);
			this._storeMeaning.Controls.Add(this._definition);
			this._storeMeaning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._storeMeaning.Location = new System.Drawing.Point(17, 185);
			this._storeMeaning.Name = "_storeMeaning";
			this._storeMeaning.Size = new System.Drawing.Size(377, 78);
			this._storeMeaning.TabIndex = 25;
			this._storeMeaning.TabStop = false;
			this._storeMeaning.Text = "Store Meaning in";
			//
			// _gloss
			//
			this._gloss.AutoSize = true;
			this._gloss.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._gloss.Location = new System.Drawing.Point(16, 43);
			this._gloss.Name = "_gloss";
			this._gloss.Size = new System.Drawing.Size(276, 17);
			this._gloss.TabIndex = 25;
			this._gloss.TabStop = true;
			this._gloss.Text = "Gloss (preferred choice when sharing data with FLEx)";
			this._gloss.UseVisualStyleBackColor = true;
			this._gloss.CheckedChanged += new System.EventHandler(this.OnGloss_RadioClicked);
			//
			// DictionaryBrowseEditTaskConfigControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._storeMeaning);
			this.Name = "DictionaryBrowseEditTaskConfigControl";
			this.Size = new System.Drawing.Size(488, 289);
			this.Controls.SetChildIndex(this._setupLabel, 0);
			this.Controls.SetChildIndex(this._storeMeaning, 0);
			this._storeMeaning.ResumeLayout(false);
			this._storeMeaning.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton _definition;
		private System.Windows.Forms.GroupBox _storeMeaning;
		private System.Windows.Forms.RadioButton _gloss;
	}
}
