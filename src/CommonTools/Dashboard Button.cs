using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
using WeSay.Project;

namespace WeSay.CommonTools
{
	public partial class DashboardButton : UserControl
	{
		private Color _borderColor=Color.Blue;
		private Color _doneColor = Color.Blue;
		private Color _todoColor = Color.LightBlue;

		public event EventHandler Selected = delegate { };

		public DashboardButton()
		{
			InitializeComponent();
		}

		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		public Color DoneColor
		{
			get { return _doneColor; }
			set { _doneColor = value; }
		}

		public Color TodoColor
		{
			get { return _todoColor; }
			set { _todoColor = value; }
		}


		private void DashboardButton_Click(object sender, EventArgs e)
		{
			Selected(this, e);
		}

		/// <summary>
		/// From a comment in http://www.codeproject.com/KB/GDI-plus/ExtendedGraphics.aspx
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="radius"></param>
		/// <param name="lw"></param>
		/// <returns></returns>
		public static GraphicsPath RoundRect(int x, int y,
				int width, int height,
				int radius, int lw)
		// x,y - top left corner of rounded rectangle
		// width, height - width and height of round rect
		// radius - radius for corners
		// lw - line width (for Graphics.Pen)
		{
			GraphicsPath g = new GraphicsPath();
			int diameter = radius * 2;
			g.AddArc(x + lw, y, diameter, diameter, 180, 90);
			g.AddArc(x + (width - diameter - lw), y, diameter, diameter, 270, 90);
			g.AddArc(x + (width - diameter - lw), y + (height - diameter - lw),
					 diameter, diameter, 360, 90);
			g.AddArc(x + lw, y + (height - diameter - lw), diameter, diameter, 90, 90);
			g.CloseFigure();
			return g;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			this._label.Text = this.Text;

			int borderWidth = 1;
			int radius = 8;
			int shadowWidth=3;

			//draw shadow
			Rectangle rectangle = new Rectangle(this.ClientRectangle.Left + shadowWidth,
											this.ClientRectangle.Top + shadowWidth,
											this.ClientRectangle.Width - shadowWidth,
											this.ClientRectangle.Height - shadowWidth);
			GraphicsPath path = GetButtonShapePath(rectangle, radius+shadowWidth, borderWidth);
			e.Graphics.FillPath(Brushes.LightGray, path);

			//draw the fron part
			rectangle = new Rectangle(this.ClientRectangle.Left,
														this.ClientRectangle.Top,
														this.ClientRectangle.Width - shadowWidth,
														this.ClientRectangle.Height - shadowWidth);
			path = GetButtonShapePath(rectangle, radius, borderWidth);
			e.Graphics.FillPath(Brushes.White, path);
			e.Graphics.DrawPath(new Pen(_borderColor, borderWidth), path);

			PaintContents(e);

			base.OnPaint(e);

		}

		protected virtual void PaintContents(PaintEventArgs e)
		{
//Color doneColor =Contrast(ForeColor, (float)1.2);
			Pen pen = new Pen(_doneColor, 5);

			int y = ClientRectangle.Bottom - 16;
			int left = this._label.Left;
			int rightEdge = ClientRectangle.Right - 15;
			float percentDone = 30;
			float rightEdgeOfDonePart =(float) (percentDone/100.0)*(rightEdge-left) + left;
			e.Graphics.DrawLine(pen, left,
								y,
								rightEdgeOfDonePart,
								y);

			// Color todoColor = Contrast(ForeColor, (float)1.9);
			pen = new Pen(_todoColor, 5);
			e.Graphics.DrawLine(pen, rightEdgeOfDonePart,
								y,
								rightEdge,
								y);
		}

		private GraphicsPath GetButtonShapePath(Rectangle rectangle, int radius, int borderWidth)
		{
			return RoundRect(rectangle.Left,
							 rectangle.Top,
							 rectangle.Width,
							 rectangle.Height,
							 radius,
							 borderWidth);
		}

		/*
		 *
		 * Didn't reall do what we wanted

		private Color Contrast(Color color, float howMuch)
		{
			return Color.FromArgb(ContrastComponent(color.R, howMuch),
								  ContrastComponent(color.G,howMuch), ContrastComponent(color.B,howMuch));
		}

		private byte ContrastComponent(byte g, float howMuch)
		{
			double v;
			v = g / 255.0;
			v -= 0.5;
			v *= howMuch;
			v += 0.5;
			v *= 255;
			if (v < 0) v = 0;
			if (v > 255) v = 255;
			return (byte)v;

		}
*/
		private void DashboardButton_FontChanged(object sender, EventArgs e)
		{
			this._label.Font = this.Font;
		}
	}
}
