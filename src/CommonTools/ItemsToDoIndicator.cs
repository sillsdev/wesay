using System;
using System.Drawing;
using System.Windows.Forms;

namespace WeSay.CommonTools
{
	public partial class ItemsToDoIndicator: UserControl
	{
		private int _count = 327;
		private int _referenceCount = 1000;

		private static int _largestExpectedNumber = 0;

		public ItemsToDoIndicator()
		{
			InitializeComponent();
		}

		/// <summary>
		/// if we're showing a stack of these, it's nice if they are all the same width
		/// </summary>
		/// <param name="largestExpectedNumber"></param>
		public static void MakeAllInstancesSameWidth(int largestExpectedNumber)
		{
			_largestExpectedNumber = largestExpectedNumber;
		}

		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				Invalidate();
			}
		}

		public int ReferenceCount
		{
			get { return _referenceCount; }
			set
			{
				_referenceCount = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_referenceCount < 0)
			{
				return; // a count is not relevant for this item, at least at this time
			}

			const int paperWidth = 15;
			int paperDistanceFromLeftEdge = 4;

			//we will draw the tray big enough for the task with the most items, so all trays are the same size
			int referenceNumberForWidth = _largestExpectedNumber > 0
												  ? _largestExpectedNumber
												  : _count;

			float globalCountWidth =
					e.Graphics.MeasureString(referenceNumberForWidth.ToString(), Font).Width;
			float countWidth = e.Graphics.MeasureString(_count.ToString(), Font).Width;
			Width = (int) (globalCountWidth + paperDistanceFromLeftEdge + paperWidth + 10);

			DrawIntrayBounds(e, ClientRectangle);
			DrawPaperStack(e, paperDistanceFromLeftEdge, paperWidth, ClientRectangle);

			//center our count
			float centeringNudge = (globalCountWidth - countWidth) / 2;
			string countString = _count.ToString();
			if (_count < 0)
			{
				countString = "?";
			}
			e.Graphics.DrawString(countString,
								  Font,
								  Brushes.Black,
								  ClientRectangle.Left + paperDistanceFromLeftEdge + paperWidth + 3 +
								  centeringNudge,
								  ClientRectangle.Top + 3);
		}

		private static void DrawIntrayBounds(PaintEventArgs e, Rectangle r)
		{
			Point topleft = new Point(r.Left, r.Top + 3);
			Point bottomLeft = new Point(r.Left, r.Bottom - 1);
			Point bottomRight = new Point(r.Right - 2, r.Bottom - 1);
			Point topRight = new Point(r.Right - 2, r.Top + 3);

			Pen boxPen = Pens.Gray;
			e.Graphics.DrawLine(boxPen, topleft, bottomLeft);
			e.Graphics.DrawLine(boxPen, bottomLeft, bottomRight);
			e.Graphics.DrawLine(boxPen, bottomRight, topRight);
		}

		private void DrawPaperStack(PaintEventArgs e,
									int paperDistanceFromLeftEdge,
									int paperWidth,
									Rectangle r)
		{
			int distanceBetweenPapers = 4;
			int maximumNumberOfPapers = 5;
			int numberOfPieces = 0;
			if (_count > 0 && _referenceCount > 0)
			{
				numberOfPieces = (int) (maximumNumberOfPapers * (_count / (float) _referenceCount));
				numberOfPieces = Math.Max(1, numberOfPieces); //at least one if there count is > 0
			}
			for (int i = 0;i < numberOfPieces;i++)
			{
				DrawOnePieceOfPaper(paperDistanceFromLeftEdge,
									e,
									paperWidth,
									r.Bottom - (5 + (distanceBetweenPapers * i)));
			}
		}

		private void DrawOnePieceOfPaper(int distanceFromLeftEdge,
										 PaintEventArgs e,
										 int paperWidth,
										 int y)
		{
			using (Pen paperPen = new Pen(Brushes.Black, 2))
			{
				e.Graphics.DrawLine(paperPen,
									ClientRectangle.Left + distanceFromLeftEdge,
									y,
									ClientRectangle.Left + distanceFromLeftEdge + paperWidth,
									y);
			}
		}
	}
}