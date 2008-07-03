using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class DashboardButton : UserControl, IButtonControl
	{
		private DialogResult _dialogResult;
		private Color _borderColor = Color.Blue;
		private Color _doneColor = Color.Blue;
		private bool _mouseIsDown;
		private bool _mouseInControl;
		private bool _keyIsDown;
		// default format flags for text rendering
		protected const TextFormatFlags _defaultTextFormatFlags = TextFormatFlags.WordBreak |
																  TextFormatFlags.NoFullWidthCharacterBreak |
																  TextFormatFlags.LeftAndRightPadding;

		protected const int ShadowWidth = 3;
		protected const int ButtonDownHorizontalNudge = ShadowWidth;
		protected const int LeftMarginWidth = 8;
		protected const int RightMarginWidth = 8 + ShadowWidth;
		protected const int TopMarginWidth = 11;
		protected const int BottomMarginWidth = 9 + ShadowWidth;
		protected const int ProgressBarHeight = 5;
		protected const int ProgressBarTopMargin = 5;
		protected virtual int ProgressBarLeftMargin { get { return 5; } }
		protected virtual int ProgressBarRightMargin { get { return 5; } }
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
			int diameter = radius*2;
			g.AddArc(x + lw, y, diameter, diameter, 180, 90);
			g.AddArc(x + (width - diameter - lw), y, diameter, diameter, 270, 90);
			g.AddArc(x + (width - diameter - lw),
					 y + (height - diameter - lw),
					 diameter,
					 diameter,
					 360,
					 90);
			g.AddArc(x + lw, y + (height - diameter - lw), diameter, diameter, 90, 90);
			g.CloseFigure();
			return g;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			Invalidate();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			int borderWidth = Focused ? 2 : 1;
			int radius = 8;

			//draw shadow
			Rectangle rectangle;
			GraphicsPath path;
			if (CurrentMouseButtonNudge == 0)
			{
				rectangle = new Rectangle(ClientRectangle.Left + ShadowWidth,
										  ClientRectangle.Top + ShadowWidth,
										  ClientRectangle.Width - ShadowWidth,
										  ClientRectangle.Height - ShadowWidth);
				path = GetButtonShapePath(rectangle, radius + ShadowWidth, 0);
				e.Graphics.FillPath(Brushes.LightGray, path);
			}

			//draw the front part
			rectangle = new Rectangle(ClientRectangle.Left + CurrentMouseButtonNudge,
									  ClientRectangle.Top + CurrentMouseButtonNudge + borderWidth - 1,
									  ClientRectangle.Width - ShadowWidth,
									  ClientRectangle.Height - ShadowWidth - borderWidth + 1);
			path = GetButtonShapePath(rectangle, radius, borderWidth);

			e.Graphics.FillPath(Brushes.White, path);
			e.Graphics.DrawPath(new Pen(_borderColor, borderWidth), path);
			if (Focused)
			{
				rectangle.X += 4;
				rectangle.Y += 3;
				rectangle.Width -= 9;
				rectangle.Height -= 8;
				path = GetButtonShapePath(rectangle, radius, 1);
				Pen p = new Pen(_borderColor, 1);
				p.DashStyle = DashStyle.Dot;
				e.Graphics.DrawPath(p, path);
			}

			PaintContents(e);
		}

		protected int CurrentMouseButtonNudge
		{
			get { return ((_mouseIsDown && _mouseInControl) || _keyIsDown) ? ButtonDownHorizontalNudge : 0; }
		}

		public IThingOnDashboard ThingToShowOnDashboard
		{
			get { return _thingToShowOnDashboard; }
		}

		protected virtual void PaintContents(PaintEventArgs e)
		{
			int nudge = CurrentMouseButtonNudge;

			int textBottom = ClientRectangle.Bottom - BottomMarginWidth;
			int left = ClientRectangle.Left + LeftMarginWidth;

			if (HasProgressBar())
			{
				textBottom -= ProgressBarHeight + ProgressBarTopMargin;
				PaintProgressBar(e.Graphics);
			}
			int textTop = ClientRectangle.Top + TopMarginWidth;
			// +1 is to fix off-by-one of width = right-left+1
			TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(left + nudge, textTop + nudge,
																		ClientRectangle.Right - left - RightMarginWidth +
																		1,
																		textBottom - textTop + 1),
								  Color.Black, FormatFlags);
		}

		protected virtual void PaintProgressBar(Graphics graphics)
		{
			ITask task = _thingToShowOnDashboard as ITask;
			//if we don't know the actual count, or it is irrelevant, don't show the bar
			if (task == null || task.GetReferenceCount() <= 0 || task.GetRemainingCount() < 0)
			{
				return;
			}

			Color doneColor = _doneColor;
			Color todoColor = Color.FromArgb(100, doneColor);
			if (DisplaySettings.Default.UsingProjectorScheme)
			{
				byte rgbMax = Math.Max(doneColor.R, Math.Max(doneColor.G, doneColor.B));
				doneColor =
						Color.FromArgb(doneColor.R == rgbMax ? 255 : 0,
									   doneColor.G == rgbMax ? 255 : 0,
									   doneColor.B == rgbMax ? 255 : 0);
				todoColor = Color.FromArgb(50, 0, 0, 0);
			}
			Pen pen = new Pen(doneColor, ProgressBarHeight);

			int nudge = CurrentMouseButtonNudge;
			int left = ClientRectangle.Left + LeftMarginWidth;
			int rightEdge = ClientRectangle.Right - RightMarginWidth;
			int progressBarTop = ClientRectangle.Bottom - BottomMarginWidth - (HasProgressBar() ? ProgressBarHeight : 0);
			float percentDone = (float)100.0 * (task.GetReferenceCount() - task.GetRemainingCount()) /
								task.GetReferenceCount();
			percentDone = Math.Max(Math.Min(percentDone, 100), 0); // ensure that 0 <= percentDone <= 100

			float rightEdgeOfDonePart = (float) (percentDone/100.0)*(rightEdge - left - ProgressBarLeftMargin - ProgressBarRightMargin)
										+ left + ProgressBarLeftMargin;
			graphics.DrawLine(pen, left + nudge + ProgressBarLeftMargin,
							  progressBarTop + nudge,
							  rightEdgeOfDonePart + nudge,
							  progressBarTop + nudge);

			pen = new Pen(todoColor, ProgressBarHeight);
			graphics.DrawLine(pen, rightEdgeOfDonePart + nudge,
							  progressBarTop + nudge,
							  rightEdge + nudge - ProgressBarRightMargin,
							  progressBarTop + nudge);
		}

		public virtual bool HasProgressBar()
		{
			ITask task = _thingToShowOnDashboard as ITask;
			return task != null && task.AreCountsRelevant();
		}

		public virtual IEnumerable<Size> GetPossibleButtonSizes()
		{
			List<Size> textSizes = GetPossibleTextSizes();
			List<Size> possibleSizes = new List<Size>(textSizes.Count);
			foreach (Size size in textSizes)
			{
				possibleSizes.Add(new Size(size.Width + LeftMarginWidth + RightMarginWidth,
										   size.Height + TopMarginWidth + BottomMarginWidth +
										   (HasProgressBar() ? ProgressBarTopMargin + ProgressBarHeight : 0)));
			}
			return possibleSizes;
		}

		/// <summary>
		/// Computes possible minimum requires sizes to fit the button text
		/// </summary>
		/// <returns>Possible sizes</returns>
		protected List<Size> GetPossibleTextSizes()
		{
			Graphics g = Graphics.FromHwnd(Handle);
			List<Size> possibleSizes = DisplaySettings.GetPossibleTextSizes(g, Text, Font, FormatFlags);
			g.Dispose();
			return possibleSizes;
		}

		private static GraphicsPath GetButtonShapePath(Rectangle rectangle, int radius, int borderWidth)
		{
			return RoundRect(rectangle.Left,
							 rectangle.Top,
							 rectangle.Width,
							 rectangle.Height,
							 radius,
							 borderWidth);
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			Selected(this, e);
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (!Enabled)
			{
				return;
			}
			_mouseIsDown = false;
			Invalidate();
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (!Enabled)
			{
				return;
			}
			_mouseIsDown = true;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (!Enabled)
			{
				return;
			}
			if (ClientRectangle.Contains(e.Location) != _mouseInControl)
			{
				_mouseInControl = ClientRectangle.Contains(e.Location);
				if (_mouseIsDown)
				{
					Invalidate();
				}
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!Enabled)
			{
				return;
			}
			if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
			{
				_keyIsDown = true;
				Invalidate();
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (!Enabled)
			{
				return;
			}
			if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
			{
				if (_keyIsDown)
				{
					OnClick(EventArgs.Empty);
				}
				_keyIsDown = false;
				Invalidate();
			}
		}

		protected override bool ProcessMnemonic(char charCode)
		{
			if (IsMnemonic(charCode, Text))
			{
				OnClick(EventArgs.Empty);
				return true;
			}
			return base.ProcessMnemonic(charCode);
		}

		protected TextFormatFlags FormatFlags
		{
			get
			{
				if (RightToLeft == RightToLeft.Yes)
				{
					return _defaultTextFormatFlags | TextFormatFlags.RightToLeft;
				}
				return _defaultTextFormatFlags & ~TextFormatFlags.RightToLeft;
			}
		}

		#region IButtonControl Members

		public DialogResult DialogResult
		{
			get
			{
				return _dialogResult;
			}
			set
			{
				if (Enum.IsDefined(typeof(DialogResult), value))
				{
					_dialogResult = value;
				}
			}
		}

		public void NotifyDefault(bool value) {}

		public void PerformClick()
		{
			if (CanSelect)
			{
				OnClick(EventArgs.Empty);
			}
		}

		#endregion
	}
}
