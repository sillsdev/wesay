using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WeSay.UI.Buttons
{
	public enum PointingDirection
	{
		Left,
		Right,
		Top,
		Bottom
	}

	[Description("Tag Button Control")]
	public class TagButton: RegionButton
	{
		private PointingDirection _pointingDirection;
		private int _arrowHeadHeight;

		public TagButton()
		{
			Size = new Size(100, 30);
			PointingDirection = PointingDirection.Left;
			ArrowHeadHeight = ClientRectangle.Height / 2;
		}

		protected override void MakeRegion()
		{
			GraphicsPath path = new GraphicsPath();
			Rectangle rectangle = ClientRectangle;
			switch (PointingDirection)
			{
				case PointingDirection.Left:
					path.AddLines(new Point[]
									  {
											  new Point(rectangle.Left + ArrowHeadHeight, rectangle.Top),
											  new Point(rectangle.Right, rectangle.Top),
											  new Point(rectangle.Right, rectangle.Bottom),
											  new Point(rectangle.Left + ArrowHeadHeight, rectangle.Bottom)
											  ,
											  new Point(rectangle.Left,
														rectangle.Top + rectangle.Height / 2)
									  });
					TextAndImageRectangle = new Rectangle(rectangle.Left + ArrowHeadHeight,
														  rectangle.Top,
														  rectangle.Width - ArrowHeadHeight,
														  rectangle.Height);

					break;
				case PointingDirection.Right:
					path.AddLines(new Point[]
									  {
											  new Point(rectangle.Right - ArrowHeadHeight, rectangle.Top),
											  new Point(rectangle.Left, rectangle.Top),
											  new Point(rectangle.Left, rectangle.Bottom),
											  new Point(rectangle.Right - ArrowHeadHeight, rectangle.Bottom)
											  ,
											  new Point(rectangle.Right,
														rectangle.Top + rectangle.Height / 2)
									  });
					TextAndImageRectangle = new Rectangle(rectangle.Left,
														  rectangle.Top,
														  rectangle.Width - ArrowHeadHeight,
														  rectangle.Height);
					break;
				case PointingDirection.Top:
					path.AddLines(new Point[]
									  {
											  new Point(rectangle.Left, rectangle.Top + ArrowHeadHeight),
											  new Point(rectangle.Left, rectangle.Bottom),
											  new Point(rectangle.Right, rectangle.Bottom),
											  new Point(rectangle.Right, rectangle.Top + ArrowHeadHeight),
											  new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top)
									  });
					TextAndImageRectangle = new Rectangle(rectangle.Left,
														  rectangle.Top + ArrowHeadHeight,
														  rectangle.Width,
														  rectangle.Height - ArrowHeadHeight);
					break;
				case PointingDirection.Bottom:
					path.AddLines(new Point[]
									  {
											  new Point(rectangle.Left, rectangle.Bottom - ArrowHeadHeight)
											  , new Point(rectangle.Left, rectangle.Top),
											  new Point(rectangle.Right, rectangle.Top),
											  new Point(rectangle.Right, rectangle.Bottom - ArrowHeadHeight)
											  ,
											  new Point(rectangle.Left + rectangle.Width / 2,
														rectangle.Bottom)
									  });
					TextAndImageRectangle = new Rectangle(rectangle.Left,
														  rectangle.Top,
														  rectangle.Width,
														  rectangle.Height - ArrowHeadHeight);
					break;
			}

			path.CloseFigure();
			Path = path;
			Invalidate();
		}

		public PointingDirection PointingDirection
		{
			get { return _pointingDirection; }
			set
			{
				_pointingDirection = value;
				MakeRegion();
			}
		}

		public int ArrowHeadHeight
		{
			get { return _arrowHeadHeight; }
			set
			{
				_arrowHeadHeight = value;
				MakeRegion();
			}
		}
	}
}