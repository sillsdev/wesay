using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeSay.UI.Buttons
{
	[Description("Region Button Control")]
	public abstract class RegionButton: Button
	{
		private Rectangle _textAndImageRectangle;

		public RegionButton()
		{
			Name = "RegionButton";
			Path = new GraphicsPath();
			Path.AddRectangle(ClientRectangle);
			_textAndImageRectangle = ClientRectangle;
			FlatStyle = FlatStyle.Flat;
		}

		protected abstract void MakeRegion();

		protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			_textAndImageRectangle = ClientRectangle;
			MakeRegion();
		}

		private bool _buttonIsHot;

		protected bool IsHot
		{
			get { return _buttonIsHot; }
		}

		private bool _buttonIsDown;

		protected bool IsDown
		{
			get { return _buttonIsDown; }
		}

		private GraphicsPath _path;

		protected GraphicsPath Path
		{
			get { return _path; }
			set
			{
				_path = value;
				Region = new Region(value);
			}
		}

		protected Rectangle TextAndImageRectangle
		{
			get { return _textAndImageRectangle; }
			set
			{
				_textAndImageRectangle = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			FillBackground(g, ClientRectangle);

			GraphicsPath buttonPath =
					new GraphicsPath(Path.PathPoints, Path.PathTypes, Path.FillMode);

			// size so it will fit in clip region
			float depth = 4;
			ShrinkAndOffsetGraphicsPathByDepth(buttonPath, depth);

			Color hotColor = Color.Orange;
			Color activeColor = Color.LightBlue;
			Color startColor = BackColor;
			if (FlatStyle != FlatStyle.Flat)
			{
				startColor = (IsDown)
									 ? ControlPaint.Dark(BackColor)
									 : ControlPaint.Light(BackColor, 1f);
			}

			if (!IsDown)
			{
				if (IsHot)
				{
					startColor = ControlPaint.Light(hotColor, 1f);
				}
				else if (IsDefault)
				{
					startColor = ControlPaint.Light(activeColor, 1);
				}
			}

			Color endColor = BackColor;
			if (FlatStyle != FlatStyle.Flat)
			{
				endColor = (IsDown)
								   ? ControlPaint.LightLight(BackColor)
								   : ControlPaint.Dark(BackColor);
			}
			if (!IsDown)
			{
				if (IsHot)
				{
					endColor = hotColor; // ControlPaint.Dark(hotColor);
				}
				else if (IsDefault)
				{
					endColor = activeColor;
				}
			}

			using (
					LinearGradientBrush brush =
							new LinearGradientBrush(ClientRectangle,
													startColor,
													endColor,
													LinearGradientMode.Vertical))
			{
				Blend blend = new Blend();
				blend.Positions = new float[] {0, .1f, .35f, .7f, .9f, 1};
				blend.Factors = new float[] {0, 0, .5f, .5f, 1, 1};
				brush.Blend = blend;
				g.FillPath(brush, buttonPath); // 3d effect
			}

			using (Pen pen = new Pen(ControlPaint.DarkDark(BackColor), 1))
			{
				g.DrawPath(pen, buttonPath); //outline
			}

			ShrinkAndOffsetGraphicsPathByDepth(buttonPath, 6);

			////hot bounds
			if (Focused)
			{
				using (Pen pen = new Pen(ControlPaint.Dark(BackColor), 1))
				{
					pen.DashStyle = DashStyle.Dash;
					pen.DashPattern = new float[] {3f, 1.5f};
					g.DrawPath(pen, buttonPath); //outline
				}
			}

			using (
					Brush brush =
							new SolidBrush((IsDown)
												   ? ControlPaint.Dark(BackColor, .01f)
												   : ControlPaint.Light(BackColor, .01f)))
			{
				g.FillPath(brush, buttonPath); // top surface of button
			}

			Rectangle buttonRect = TextAndImageRectangle;
			if (_buttonIsDown)
			{
				buttonRect.Offset(1, 1); // Apply the offset if we've been clicked
			}

			DrawText(g, buttonRect);

			DrawImage(g, buttonRect);
		}

		private static void ShrinkAndOffsetGraphicsPathByDepth(GraphicsPath buttonPath, float depth)
		{
			Matrix matrix = new Matrix();
			RectangleF bounds = buttonPath.GetBounds();
			bounds.Inflate(2, 2);
			float buttonWidth = bounds.Width;
			float buttonHeight = bounds.Height;
			matrix.Scale(1 - depth / buttonWidth, 1 - depth / buttonHeight);
			buttonPath.Transform(matrix);

			matrix = new Matrix();
			bounds = buttonPath.GetBounds();
			bounds.Inflate(2, 2);
			float xDepth = buttonWidth - bounds.Width;
			float yDepth = buttonHeight - bounds.Height;

			matrix.Translate((float) Math.Ceiling(xDepth / 2),
							 (float) Math.Ceiling(yDepth / 2),
							 MatrixOrder.Append);
			buttonPath.Transform(matrix);
		}

		private void DrawImage(Graphics g, Rectangle buttonRect)
		{
			if (Image != null)
			{
				int x = buttonRect.X;
				int y = buttonRect.Y;
				switch (ImageAlign)
				{
					case ContentAlignment.BottomCenter:
						x += (buttonRect.Width - Image.Width) / 2;
						y += buttonRect.Height - Image.Height;
						break;
					case ContentAlignment.BottomLeft:
						x += 0;
						y += buttonRect.Height - Image.Height;
						break;
					case ContentAlignment.BottomRight:
						x += buttonRect.Width - Image.Width;
						y += buttonRect.Height - Image.Height;
						break;
					case ContentAlignment.MiddleCenter:
						x += (buttonRect.Width - Image.Width) / 2;
						y += (buttonRect.Height - Image.Height) / 2;
						break;
					case ContentAlignment.MiddleLeft:
						x += 0;
						y += (buttonRect.Height - Image.Height) / 2;
						break;
					case ContentAlignment.MiddleRight:
						x += buttonRect.Width - Image.Width;
						y += (buttonRect.Height - Image.Height) / 2;
						break;
					case ContentAlignment.TopCenter:
						x += (buttonRect.Width - Image.Width) / 2;
						y += 0;
						break;
					case ContentAlignment.TopLeft:
						x += 0;
						y += 0;
						break;
					case ContentAlignment.TopRight:
						x += buttonRect.Width - Image.Width;
						y += 0;
						break;
					default:
						break;
				}
				g.DrawImage(Image, x, y, Image.Width, Image.Height);
				if (!Enabled)
				{
					ControlPaint.DrawImageDisabled(g, Image, x, y, Color.Transparent);
				}
			}
		}

		private void FillBackground(Graphics g, Rectangle rect)
		{
			rect.Inflate(1, 1);
			using (SolidBrush brush = new SolidBrush(Parent.BackColor))
			{
				g.FillRectangle(brush, rect);
			}
		}

		/// <summary>
		/// Write the text on the button
		/// </summary>
		/// <param name="g">Graphics Object</param>
		/// <param name="textRect">Rectangle defining the button text area</param>
		private void DrawText(IDeviceContext g, Rectangle textRect)
		{
			TextFormatFlags flags = TextFormatFlags.LeftAndRightPadding;

			switch (TextAlign)
			{
				case ContentAlignment.BottomCenter:
					flags |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
					break;
				case ContentAlignment.BottomLeft:
					flags |= TextFormatFlags.Bottom | TextFormatFlags.Left;
					break;
				case ContentAlignment.BottomRight:
					flags |= TextFormatFlags.Bottom | TextFormatFlags.Right;
					break;
				case ContentAlignment.MiddleCenter:
					flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
					break;
				case ContentAlignment.MiddleLeft:
					flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
					break;
				case ContentAlignment.MiddleRight:
					flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
					break;
				case ContentAlignment.TopCenter:
					flags |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
					break;
				case ContentAlignment.TopLeft:
					flags |= TextFormatFlags.Top | TextFormatFlags.Left;
					break;
				case ContentAlignment.TopRight:
					flags |= TextFormatFlags.Top | TextFormatFlags.Right;
					break;
				default:
					break;
			}

			// If button is not enabled, "grey out" the text.
			Color foreColor = ForeColor;
			if (!Enabled)
			{
				foreColor = SystemColors.GrayText;
			}

			//Write the text
			TextRenderer.DrawText(g, Text, Font, textRect, foreColor, Color.Transparent, flags);
		}

		protected override void OnMouseDown(MouseEventArgs mevent)
		{
			buttonDown();
			base.OnMouseDown(mevent);
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			buttonUp();
			base.OnMouseUp(mevent);
		}

		private void buttonDown()
		{
			_buttonIsDown = true;
			Invalidate();
		}

		private void buttonUp()
		{
			_buttonIsDown = false;
			Invalidate();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				buttonDown();
			}
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				buttonUp();
			}
			base.OnKeyUp(e);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			_buttonIsHot = Enabled;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_buttonIsHot = false;
			Invalidate();
		}
	}
}