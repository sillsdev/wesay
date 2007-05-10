namespace Addin.LiftReports
{
	partial class XPathChart
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
			this._graph = new GraphComponents.StackedBarGraph();
			this._label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _graph
			//
			this._graph.AboveRangeColor = System.Drawing.Color.Salmon;
			this._graph.AboveRangeValue = 99999F;
			this._graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._graph.BarColor = System.Drawing.Color.SteelBlue;
			this._graph.BarOrientation = GraphComponents.Orientation.Vertical;
			this._graph.BarWidthToSpacingRatio = 1F;
			this._graph.BelowRangeColor = System.Drawing.Color.Thistle;
			this._graph.BelowRangeValue = 30F;
			this._graph.GraduationsX = 5;
			this._graph.GraduationsY = 5;
			this._graph.GraphArea = new System.Drawing.Rectangle(50, 20, 274, 216);
			this._graph.GraphAreaColor = System.Drawing.Color.White;
			this._graph.GraphMarginBottom = 20;
			this._graph.GraphMarginLeft = 50;
			this._graph.GraphMarginRight = 20;
			this._graph.GraphMarginTop = 20;
			this._graph.GridlineColor = System.Drawing.Color.LightGray;
			this._graph.Gridlines = GraphComponents.GridStyles.None;
			this._graph.Location = new System.Drawing.Point(3, 3);
			this._graph.MaximumValue = 100F;
			this._graph.MinimumValue = 0F;
			this._graph.Name = "_graph";
			this._graph.ShowRangeLines = false;
			this._graph.ShowRangeValues = false;
			this._graph.Size = new System.Drawing.Size(344, 256);
			this._graph.TabIndex = 0;
			this._graph.Transparency = ((byte)(100));
			this._graph.ValueAlignment = GraphComponents.TextAlignment.Smart;
			this._graph.ValueFormat = "";
			this._graph.XAxisColor = System.Drawing.Color.Black;
			this._graph.YAxisColor = System.Drawing.Color.Black;
			//
			// _label
			//
			this._label.AutoSize = true;
			this._label.Location = new System.Drawing.Point(55, 246);
			this._label.Name = "_label";
			this._label.Size = new System.Drawing.Size(35, 13);
			this._label.TabIndex = 1;
			this._label.Text = "label1";
			//
			// XPathChart
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._label);
			this.Controls.Add(this._graph);
			this.Name = "XPathChart";
			this.Size = new System.Drawing.Size(353, 298);
			this.Load += new System.EventHandler(this.XPathChart_Load);
			this.Resize += new System.EventHandler(this.XPathChart_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private GraphComponents.StackedBarGraph _graph;
		private System.Windows.Forms.Label _label;
	}
}
