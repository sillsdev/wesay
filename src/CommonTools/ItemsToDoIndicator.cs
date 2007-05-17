using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WindowsControlLibrary1
{
	public partial class ItemsToDoIndicator : UserControl
	{
		private int _count=327;
		private int _referenceCount = 1000;

		private string _status;

		public ItemsToDoIndicator()
		{
			InitializeComponent();
		}



		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
				Invalidate();
			}
		}

		public int ReferenceCount
		{
			get
			{
				return _referenceCount;
			}
			set
			{
				_referenceCount = value;
				Invalidate();
			}
		}


		/// <summary>
		/// this exists because some tasks give a string under some conditions, rather than a number.
		/// </summary>
		public string Status
		{
			set
			{
				//store it only if it is not a number
				if(!Int32.TryParse(value, out _count))
				{
					_status = value;
				}
			}
		}

		protected override void  OnPaint(PaintEventArgs e)
		{
			if (_referenceCount < 0)
			{
				return; // a count is not relevant for this item, at least at this time
			}

		   const int paperWidth = 15;
		   int paperDistanceFromLeftEdge = 4;
		   SizeF countSize = e.Graphics.MeasureString(_count.ToString(), this.Font);
		   this.Width = (int) (countSize.Width + paperDistanceFromLeftEdge + paperWidth + 10);

		   Rectangle r = new Rectangle(this.ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Width, ClientRectangle.Height);
		   Point topleft = new Point(r.Left, r.Top+3);
		   Point bottomLeft = new Point(r.Left, r.Bottom-1);
		   Point bottomRight = new Point(r.Right-2, r.Bottom-1);
		   Point topRight = new Point(r.Right-2, r.Top+3);

			Pen boxPen = Pens.Gray;
			e.Graphics.DrawLine(boxPen, topleft, bottomLeft);
		   e.Graphics.DrawLine(boxPen, bottomLeft, bottomRight);
		   e.Graphics.DrawLine(boxPen, bottomRight, topRight);

		   int distanceBetweenPapers = 4   ;
			int maximumNumberOfPapers = 5;
			int numberOfPieces = 0;
			if (_count > 0 && _referenceCount > 0)
			{
				numberOfPieces =(int) ((float)maximumNumberOfPapers * ((float)_count / (float)_referenceCount));
				numberOfPieces = Math.Max(1, numberOfPieces);//at least one if there count is > 0
			}
			for (int i = 0; i < numberOfPieces; i++)
		   {
			   DrawOnePieceOfPaper(paperDistanceFromLeftEdge, e, paperWidth, r.Bottom - (5+(distanceBetweenPapers*i)));
		   }

		   e.Graphics.DrawString(_count.ToString(), this.Font, Brushes.Black, r.Left + paperDistanceFromLeftEdge + paperWidth + 3,
								 r.Top + 3);
//            e.Graphics.FillRectangle(Brushes.Cyan, Bounds);
//             e.Graphics.FillRectangle(Brushes.Yellow , e.ClipRectangle);
	   }

		private void DrawOnePieceOfPaper(int distanceFromLeftEdge, PaintEventArgs e, int paperWidth, int y)
		{
			using (Pen paperPen = new Pen(Brushes.Black, 2))
			{
				e.Graphics.DrawLine(paperPen, this.ClientRectangle.Left + distanceFromLeftEdge, y,
									this.ClientRectangle.Left + distanceFromLeftEdge + paperWidth, y);
			}
		}
	}
}