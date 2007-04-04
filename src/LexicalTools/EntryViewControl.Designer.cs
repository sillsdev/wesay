namespace WeSay.LexicalTools
{
	partial class EntryViewControl
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
			if (disposing && !IsDisposed)
			{
				if (_record != null)
				{
					_record.PropertyChanged -= OnRecordPropertyChanged;
					_record.EmptyObjectsRemoved -= OnEmptyObjectsRemoved;
				}
				_detailListControl.ChangeOfWhichItemIsInFocus -= OnChangeOfWhichItemIsInFocus;
			}
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
			this._lexicalEntryPreview = new System.Windows.Forms.RichTextBox();
			this._detailListControl = new WeSay.UI.DetailList();
			this.SuspendLayout();
			//
			// _lexicalEntryPreview
			//
			this._lexicalEntryPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lexicalEntryPreview.BackColor = System.Drawing.SystemColors.Control;
			this._lexicalEntryPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lexicalEntryPreview.Location = new System.Drawing.Point(3, 3);
			this._lexicalEntryPreview.Name = "_lexicalEntryPreview";
			this._lexicalEntryPreview.ReadOnly = true;
			this._lexicalEntryPreview.Size = new System.Drawing.Size(450, 85);
			this._lexicalEntryPreview.TabStop = false;
			this._lexicalEntryPreview.Text = "";
			//
			// _detailListControl
			//
			this._detailListControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._detailListControl.BackColor =  System.Drawing.SystemColors.Control;
			this._detailListControl.Location = new System.Drawing.Point(3, 95);
			this._detailListControl.Name = "_detailListControl";
			this._detailListControl.Size = new System.Drawing.Size(450, 250);
			this._detailListControl.TabIndex = 1;
			//
			// EntryViewControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._lexicalEntryPreview);
			this.Controls.Add(this._detailListControl);
			this.Name = "LexPreviewWithEntryControl";
			this.Size = new System.Drawing.Size(474, 370);
			this.BackColorChanged += new System.EventHandler(this.LexPreviewWithEntryControl_BackColorChanged);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox _lexicalEntryPreview;
		private WeSay.UI.DetailList _detailListControl;
	}
}
