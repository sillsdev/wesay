using System.Drawing;

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// Summary description for Cursor.
	/// </summary>
	internal class Cursor: IGraphElement
	{
		private Rectangle graphArea;
		private readonly Channel channel;

		public Rectangle GraphArea
		{
			get { return graphArea; }
			set { graphArea = value; }
		}

		public Cursor(Channel channel)
		{
			this.channel = channel;
		}

		#region IGraphElement Members

		public void Draw(Graphics graphics)
		{
			// TODO:  Add Cursor.Draw implementation

			int halfGraphHeight = GraphArea.Height / 2;
			float range = channel.MaximumValue - channel.MinimumValue;

			int offsetInPixel = (int) (GraphArea.Height / range * channel.CursorOffset);

			Point leftPoint = new Point();
			leftPoint.X = GraphArea.Left;
			leftPoint.Y = GraphArea.Bottom - halfGraphHeight + offsetInPixel;

			Point rightPoint = new Point(GraphArea.Right, leftPoint.Y);

			graphics.DrawLine(new Pen(channel.ChannelColor), leftPoint, rightPoint);
		}

		#endregion
	}
}