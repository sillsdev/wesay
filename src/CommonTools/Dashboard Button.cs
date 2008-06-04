using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.Foundation.Dashboard;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class DashboardButton : UserControl
	{
		private Color _borderColor=Color.Blue;
		private Color _doneColor = Color.Blue;
		private bool _mouseIsDown;
		protected const int ButtonDownHorizontalNudge = 2;
		protected const int LeftMarginWidth= 5;
		private readonly IThingOnDashboard _thingToShowOnDashboard;

		public event EventHandler Selected = delegate { };

		public DashboardButton(IThingOnDashboard thingToShowOnDashboard)
		{
			_thingToShowOnDashboard = thingToShowOnDashboard;
			InitializeComponent();

		}

		public DashboardButton()
		{
			InitializeComponent();

		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				int requiredWidth = GetRequiredWidth();
				if (requiredWidth > Width)
				{
					Width = requiredWidth;
				}
			}
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

			if(!_mouseIsDown)
			{
				rectangle = new Rectangle(this.ClientRectangle.Left,
										  this.ClientRectangle.Top,
										  this.ClientRectangle.Width - shadowWidth,
										  this.ClientRectangle.Height - shadowWidth);
			}
			else
			{
				rectangle = new Rectangle(this.ClientRectangle.Left+2,
										  this.ClientRectangle.Top+2,
										  this.ClientRectangle.Width - shadowWidth,
										  this.ClientRectangle.Height - shadowWidth);
			}
			path = GetButtonShapePath(rectangle, radius, borderWidth);

			e.Graphics.FillPath(Brushes.White, path);
			e.Graphics.DrawPath(new Pen(_borderColor, borderWidth), path);

			PaintContents(e);

			base.OnPaint(e);

		}

		protected int CurrentMouseButtonNudge
		{
			get { return _mouseIsDown ? ButtonDownHorizontalNudge : 0; }
		}

		public IThingOnDashboard ThingToShowOnDashboard
		{
			get { return this._thingToShowOnDashboard; }
		}

		protected virtual void PaintContents(PaintEventArgs e)
		{
			int nudge = CurrentMouseButtonNudge;
			Color doneColor = _doneColor;
			Color todoColor = Color.FromArgb(100, doneColor);
			if (DisplaySettings.Default.UsingProjectorScheme)
			{
				byte rgbMax = Math.Max(doneColor.R, Math.Max(doneColor.G, doneColor.B));
				doneColor = Color.FromArgb(doneColor.R == rgbMax ? 255 : 0, doneColor.G == rgbMax ? 255 : 0,
										   doneColor.B == rgbMax ? 255 : 0);
				todoColor = Color.FromArgb(50, 0, 0, 0);
			}
			Pen pen = new Pen(doneColor, 5);

			int y = ClientRectangle.Bottom - 16;
			int left = ClientRectangle.Left + LeftMarginWidth;
			int rightEdge = ClientRectangle.Right - 15;
			ITask task = this._thingToShowOnDashboard as ITask;

			//if we don't know the actual count, or it is irrelevant, don't show the bar
			if (task != null && task.GetReferenceCount() >0 && task.GetRemainingCount() >=0)
			{
				float percentDone = (float)100.0 * (task.GetReferenceCount()-task.GetRemainingCount())/task.GetReferenceCount();

				 float rightEdgeOfDonePart = (float) (percentDone/100.0)*(rightEdge - left) + left;
				e.Graphics.DrawLine(pen, left + nudge,
									y + nudge,
									rightEdgeOfDonePart + nudge,
									y + nudge);

				pen = new Pen(todoColor, 5);
				e.Graphics.DrawLine(pen, rightEdgeOfDonePart + nudge,
									y + nudge,
									rightEdge + nudge,
									y + nudge);
			}
			e.Graphics.DrawString(this.Text, this.Font, Brushes.Black, left+nudge, 10+nudge);
		}


		public virtual int GetRequiredWidth()
		{
			int textWidth= TextRenderer.MeasureText(Text, this.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.LeftAndRightPadding).Width + ButtonDownHorizontalNudge;
			int unknownHack = 20;
			return textWidth + ButtonDownHorizontalNudge + LeftMarginWidth + unknownHack;
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



		protected void DashboardButton_MouseDown(object sender, MouseEventArgs e)
		{
			_mouseIsDown = true;
			Invalidate();
		}

		protected void DashboardButton_MouseUp(object sender, MouseEventArgs e)
		{
			_mouseIsDown = false;
			Invalidate();
		}

		protected void DashboardButton_MouseLeave(object sender, EventArgs e)
		{
			_mouseIsDown = false;
			Invalidate();
		}
	}
}
