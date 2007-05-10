using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;

namespace GraphComponents
{
	/// <summary>
	/// Draws the axis lines for the graph.
	/// </summary>
	public class AxisLine : IGraphElement
	{
		Graph parentGraph;

		public AxisLine(Graph parentGraph)
		{
			this.parentGraph = parentGraph;
		}

		#region IGraphElement Members

		public void Draw(Graphics graphics)
		{
			graphics.SetClip (parentGraph.ClientRectangle);

			Pen axisPen = new Pen (new SolidBrush (parentGraph.YAxisColor), 2F);
			axisPen.EndCap = LineCap.ArrowAnchor;

			RectangleF graphArea = parentGraph.GraphArea;

			graphics.DrawLine (axisPen, graphArea.Left, graphArea.Bottom, graphArea.Left, graphArea.Top);

			axisPen.Color = parentGraph.XAxisColor;
			axisPen.EndCap = LineCap.ArrowAnchor;

			graphics.DrawLine (axisPen, graphArea.Left, graphArea.Bottom, graphArea.Right, graphArea.Bottom);

			axisPen.Dispose ();
		}

		#endregion
	}
}
