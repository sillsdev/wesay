using System.Windows.Forms;
using WeSay.UI;

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
			this._splitter = new WeSay.UI.CollapsibleSplitter();
			this._panelEntry = new System.Windows.Forms.Panel();
			this._panelEntry.SuspendLayout();
			this.SuspendLayout();
			//
			// _lexicalEntryPreview
			//
			this._lexicalEntryPreview.BackColor = System.Drawing.SystemColors.Control;
			this._lexicalEntryPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lexicalEntryPreview.Dock = System.Windows.Forms.DockStyle.Top;
			this._lexicalEntryPreview.Location = new System.Drawing.Point(0, 0);
			this._lexicalEntryPreview.Name = "_lexicalEntryPreview";
			this._lexicalEntryPreview.ReadOnly = true;
			this._lexicalEntryPreview.Size = new System.Drawing.Size(474, 85);
			this._lexicalEntryPreview.TabIndex = 0;
			this._lexicalEntryPreview.TabStop = false;
			this._lexicalEntryPreview.Text = "";
			//
			// _detailListControl
			//
			this._detailListControl.AutoScroll = true;
			this._detailListControl.BackColor = System.Drawing.SystemColors.Control;
			this._detailListControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._detailListControl.Location = new System.Drawing.Point(0, 0);
			this._detailListControl.Margin = new System.Windows.Forms.Padding(0);
			this._detailListControl.Name = "_detailListControl";
			this._detailListControl.Size = new System.Drawing.Size(474, 277);
			this._detailListControl.TabIndex = 1;
			//
			// _splitter
			//
			this._splitter.BackColorEnd = System.Drawing.Color.Empty;
			this._splitter.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
			this._splitter.ControlToHide = this._lexicalEntryPreview;
			this._splitter.Cursor = System.Windows.Forms.Cursors.HSplit;
			this._splitter.Dock = System.Windows.Forms.DockStyle.Top;
			this._splitter.GripLength = 30;
			this._splitter.Location = new System.Drawing.Point(0, 85);
			this._splitter.Name = "_splitter";
			this._splitter.TabIndex = 1;
			this._splitter.TabStop = false;
			this._splitter.VisualStyle = WeSay.UI.VisualStyles.DoubleDots;
			//
			// _panelEntry
			//
			this._panelEntry.AutoScroll = true;
			this._panelEntry.Controls.Add(this._detailListControl);
			this._panelEntry.Dock = System.Windows.Forms.DockStyle.Fill;
			this._panelEntry.Location = new System.Drawing.Point(0, 93);
			this._panelEntry.Name = "_panelEntry";
			this._panelEntry.Size = new System.Drawing.Size(474, 277);
			this._panelEntry.TabIndex = 1;
			//
			// EntryViewControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._panelEntry);
			this.Controls.Add(this._splitter);
			this.Controls.Add(this._lexicalEntryPreview);
			this.Name = "EntryViewControl";
			this.Size = new System.Drawing.Size(474, 370);
			this.BackColorChanged += new System.EventHandler(this.LexPreviewWithEntryControl_BackColorChanged);
			this._panelEntry.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox _lexicalEntryPreview;
		private WeSay.UI.DetailList _detailListControl;
		private CollapsibleSplitter _splitter;
		private System.Windows.Forms.Panel _panelEntry;

	}
}
