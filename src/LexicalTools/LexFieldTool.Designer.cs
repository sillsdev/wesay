namespace WeSay.LexicalTools
{
	partial class LexFieldTool
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
				_recordsListBox.SelectedIndexChanged -= OnRecordSelectionChanged;
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
			this._recordsListBox = new ListBox.BindingListGrid();
			this._lexFieldDetailPanel = new WeSay.LexicalTools.LexFieldControl(_fieldInventory);
			this.SuspendLayout();
			//
			// _recordsListBox
			//
			this._recordsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this._recordsListBox.GridToolTipActive = true;
			this._recordsListBox.Location = new System.Drawing.Point(4, 5);
			this._recordsListBox.MinimumSize = new System.Drawing.Size(0, 100);
			this._recordsListBox.Name = "_recordsListBox";
			this._recordsListBox.SelectedIndex = 0;
			this._recordsListBox.Size = new System.Drawing.Size(116, 122);
			this._recordsListBox.SpecialKeys = ((SourceGrid3.GridSpecialKeys)(((((((SourceGrid3.GridSpecialKeys.Arrows | SourceGrid3.GridSpecialKeys.Tab)
						| SourceGrid3.GridSpecialKeys.PageDownUp)
						| SourceGrid3.GridSpecialKeys.Enter)
						| SourceGrid3.GridSpecialKeys.Escape)
						| SourceGrid3.GridSpecialKeys.Control)
						| SourceGrid3.GridSpecialKeys.Shift)));
			this._recordsListBox.StyleGrid = null;
			this._recordsListBox.TabIndex = 5;
			//
			// _lexFieldDetailPanel
			//
			this._lexFieldDetailPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lexFieldDetailPanel.AutoScroll = true;
			this._lexFieldDetailPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this._lexFieldDetailPanel.DataSource = null;
			this._lexFieldDetailPanel.Location = new System.Drawing.Point(126, 5);
			this._lexFieldDetailPanel.Name = "_lexFieldDetailPanel";
			this._lexFieldDetailPanel.Size = new System.Drawing.Size(367, 122);
			this._lexFieldDetailPanel.TabIndex = 4;
			//
			// LexFieldTool
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._recordsListBox);
			this.Controls.Add(this._lexFieldDetailPanel);
			this.Name = "LexFieldTool";
			this.Size = new System.Drawing.Size(493, 169);
			this.ResumeLayout(false);

		}

		#endregion

		private LexFieldControl _lexFieldDetailPanel;
		private ListBox.BindingListGrid _recordsListBox;
	}
}
