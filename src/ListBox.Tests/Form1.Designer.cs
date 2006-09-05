namespace ListBox
{
	partial class Form1
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._grid = new ListBox.BindingListGrid();
			this.SuspendLayout();
			//
			// _grid
			//
			this._grid.GridToolTipActive = true;
			this._grid.Location = new System.Drawing.Point(33, 29);
			this._grid.Name = "_grid";
			this._grid.SelectedIndex = 0;
			this._grid.Size = new System.Drawing.Size(227, 189);
			this._grid.SpecialKeys = ((SourceGrid3.GridSpecialKeys)(((((((SourceGrid3.GridSpecialKeys.Arrows | SourceGrid3.GridSpecialKeys.Tab)
						| SourceGrid3.GridSpecialKeys.PageDownUp)
						| SourceGrid3.GridSpecialKeys.Enter)
						| SourceGrid3.GridSpecialKeys.Escape)
						| SourceGrid3.GridSpecialKeys.Control)
						| SourceGrid3.GridSpecialKeys.Shift)));
			this._grid.StyleGrid = null;
			this._grid.TabIndex = 0;
			this._grid.SelectedIndexChanged += new System.EventHandler(this._grid_SelectionChanged);
			//
			// Form1
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this._grid);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private BindingListGrid _grid;


	}
}
