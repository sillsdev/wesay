using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Globalization;

namespace GraphComponents
{
	/// <summary>
	/// Summary description for Cursor.
	/// </summary>
	class Cursor : IGraphElement
	{
		private Rectangle graphArea;
		private Channel channel;

		public Rectangle GraphArea
		{
			get { return graphArea;  }
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

			int offsetInPixel = (int) ((float) (GraphArea.Height / range) * channel.CursorOffset);

			Point leftPoint = new Point ();
			leftPoint.X = GraphArea.Left;
			leftPoint.Y = GraphArea.Bottom - halfGraphHeight + offsetInPixel;

			Point rightPoint = new Point (GraphArea.Right, leftPoint.Y);

			graphics.DrawLine (new Pen (channel.ChannelColor), leftPoint, rightPoint);

		}

		#endregion

	}
}
