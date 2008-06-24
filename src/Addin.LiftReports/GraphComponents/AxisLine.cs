using System.Drawing;
using System.Drawing.Drawing2D;

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// Draws the axis lines for the graph.
	/// </summary>
	public class AxisLine: IGraphElement
	{
		private readonly Graph parentGraph;

		public AxisLine(Graph parentGraph)
		{
			this.parentGraph = parentGraph;
		}

		#region IGraphElement Members

		public void Draw(Graphics graphics)
		{
			graphics.SetClip(parentGraph.ClientRectangle);

			Pen axisPen = new Pen(new SolidBrush(parentGraph.YAxisColor), 2F);
			axisPen.EndCap = LineCap.ArrowAnchor;

			RectangleF graphArea = parentGraph.GraphArea;

			graphics.DrawLine(axisPen,
							  graphArea.Left,
							  graphArea.Bottom,
							  graphArea.Left,
							  graphArea.Top);

			axisPen.Color = parentGraph.XAxisColor;
			axisPen.EndCap = LineCap.ArrowAnchor;

			graphics.DrawLine(axisPen,
							  graphArea.Left,
							  graphArea.Bottom,
							  graphArea.Right,
							  graphArea.Bottom);

			axisPen.Dispose();
		}

		#endregion
	}
}