namespace WeSay.LexicalTools
{
	partial class LexFieldControl
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
			this._lexicalEntryView = new System.Windows.Forms.RichTextBox();
			this._entryDetailControl = new EntryDetailControl();
			this.SuspendLayout();
			//
			// _lexicalEntryView
			//
			this._lexicalEntryView.BackColor = System.Drawing.SystemColors.Control;
			this._lexicalEntryView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lexicalEntryView.Location = new System.Drawing.Point(3, 3);
			this._lexicalEntryView.Name = "_lexicalEntryView";
			this._lexicalEntryView.ReadOnly = true;
			this._lexicalEntryView.Size = new System.Drawing.Size(450, 85);
			this._lexicalEntryView.TabIndex = 0;
			this._lexicalEntryView.Text = "";
			//
			// _entryDetailControl
			//
			this._entryDetailControl.Location = new System.Drawing.Point(3, 95);
			this._entryDetailControl.Name = "_entryDetailControl";
			this._entryDetailControl.Size = new System.Drawing.Size(450, 250);
			this._entryDetailControl.TabIndex = 1;
			//
			// LexFieldControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._lexicalEntryView);
			this.Controls.Add(this._entryDetailControl);
			this.Name = "LexFieldControl";
			this.Size = new System.Drawing.Size(474, 370);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox _lexicalEntryView;
		private EntryDetailControl _entryDetailControl;
	}
}
