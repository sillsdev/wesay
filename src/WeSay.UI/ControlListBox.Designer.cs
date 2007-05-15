namespace WindowsApplication2
{
	partial class ControlListBox
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
			this._panel = new System.Windows.Forms.Panel();
			this._table = new System.Windows.Forms.TableLayoutPanel();
			this._tableOld = new System.Windows.Forms.TableLayoutPanel();
			this._panel.SuspendLayout();
			this.SuspendLayout();
			//
			// _panel
			//
			this._panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._panel.BackColor = System.Drawing.SystemColors.Window;
			this._panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._panel.Controls.Add(this._table);
			this._panel.Controls.Add(this._tableOld);
			this._panel.Location = new System.Drawing.Point(0, 0);
			this._panel.Name = "_panel";
			this._panel.Size = new System.Drawing.Size(220, 211);
			this._panel.TabIndex = 1;
			this._panel.Resize += new System.EventHandler(this._panel_Resize);
			//
			// _table
			//
			this._table.AutoScroll = true;
			this._table.ColumnCount = 1;
			this._table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._table.Dock = System.Windows.Forms.DockStyle.Fill;
			this._table.Location = new System.Drawing.Point(0, 0);
			this._table.Name = "_table";
			this._table.RowCount = 1;
			this._table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._table.Size = new System.Drawing.Size(218, 209);
			this._table.TabIndex = 1;
			this._table.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
			//
			// _tableOld
			//
			this._tableOld.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._tableOld.AutoScroll = true;
			this._tableOld.BackColor = System.Drawing.SystemColors.Window;
			this._tableOld.ColumnCount = 1;
			this._tableOld.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableOld.Cursor = System.Windows.Forms.Cursors.Arrow;
			this._tableOld.Location = new System.Drawing.Point(19, 132);
			this._tableOld.Name = "_tableOld";
			this._tableOld.RowCount = 6;
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableOld.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableOld.Size = new System.Drawing.Size(69, 345);
			this._tableOld.TabIndex = 0;
			this._tableOld.Resize += new System.EventHandler(this._table_Resize);
			//
			// ControlListBox
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._panel);
			this.Name = "ControlListBox";
			this.Size = new System.Drawing.Size(220, 211);
			this.Resize += new System.EventHandler(this.ControlListBox_Resize);
			this._panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _panel;
		private System.Windows.Forms.TableLayoutPanel _tableOld;

		public void Clear()
		{
			_tableOld.Controls.Clear();
			_tableOld.RowStyles.Clear();
			_firstOne = true;
		}

		private System.Windows.Forms.TableLayoutPanel _table;
	}
}
