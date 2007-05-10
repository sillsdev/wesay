using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GraphComponents
{
	/// <summary>
	/// Draws the grid lines for the graph.
	/// </summary>
	class Gridline : IGraphElement
	{
		Graph parentGraph;

		public Gridline(Graph parentGraph)
		{
			this.parentGraph = parentGraph;
		}

		#region IGraphElement Members

		public void Draw(System.Drawing.Graphics graphics)
		{
			Pen graphAreaPen       = new Pen (parentGraph.GridlineColor);
			graphAreaPen.DashStyle = DashStyle.Dash;

			Brush graphAreaBrush   = new SolidBrush (parentGraph.GraphAreaColor);

			graphics.FillRectangle (graphAreaBrush, parentGraph.GraphArea);
			graphics.DrawRectangle (graphAreaPen, parentGraph.GraphArea);

			if ((parentGraph.Gridlines & GridStyles.Horizontal) == GridStyles.Horizontal)
			{
				graphics.SetClip (parentGraph.GraphArea);

				int gridSize = parentGraph.GraphArea.Height / parentGraph.GraduationsY;
				for (int i = 0; i < parentGraph.GraphArea.Height; i += gridSize)
				{
					graphics.DrawLine (graphAreaPen, parentGraph.GraphArea.Left, parentGraph.GraphArea.Top + i, parentGraph.GraphArea.Right, parentGraph.GraphArea.Top + i);
				}
			}

			if ((parentGraph.Gridlines & GridStyles.Vertical) == GridStyles.Vertical)
			{
				int gridSize = parentGraph.GraphArea.Width / parentGraph.GraduationsX;

				for (int i = 0; i < parentGraph.GraphArea.Width; i += gridSize)
				{
					graphics.DrawLine (graphAreaPen, parentGraph.GraphArea.Left + i, parentGraph.GraphArea.Bottom, parentGraph.GraphArea.Left + i, parentGraph.GraphArea.Top);
				}
			}

			graphAreaPen.Dispose ();
			graphAreaBrush.Dispose ();

		}

		#endregion
	}
}
