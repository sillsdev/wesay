#region copyright info
//
// Written by Anup. V (anupshubha@yahoo.com)
// Copyright (c) 2006.
//
// This code may be used in compiled form in any way you desire. This
// file may be redistributed by any means PROVIDING it is not sold for
// for profit without the authors written consent, and providing that
// this notice and the authors name is included. If the source code in
// this file is used in any commercial application then acknowledgement
// must be made to the author of this file (in whatever form you wish).
//
// This file is provided "as is" with no expressed or implied warranty.
//
// Please use and enjoy. Please let me know of any bugs/mods/improvements
// that you have found/implemented and I will fix/incorporate them into
// this file.
#endregion copyright info

using System;
using System.Drawing;
using System.Globalization;
using System.Drawing.Drawing2D;

namespace GraphComponents
{
	/// <summary>
	/// This class provides the functionality for drawing a simple bar (rectangle)
	/// in a specified area.
	/// </summary>
	class BasicBar : IGraphElement
	{
		// TODO: Duplication of data here and in BarGraph. Should have a
		// BarProperties class and populate that and pass data around. But,
		// the problem was that the VS designer was not updating the graph
		// instantly. Got to think of a way though..

		#region variables
		/// <summary>
		/// Maximum value of the bar
		/// </summary>
		private float maximumValue = 100;

		/// <summary>
		/// Minimum value of the bar
		/// </summary>
		private float minimumValue;

		/// <summary>
		/// Current value of the bar. Gets or sets the value to display on the bar
		/// </summary>
		private float barValue = 35;
		/// <summary>
		/// Color of the bar
		/// </summary>
		private Color barColor = Color.Lime;
		/// <summary>
		/// Color of the graph's border that includes only the area
		/// that is filled up and not the entire rectangle
		/// </summary>
		private Color graphBorderColor = Color.Black;
		/// <summary>
		/// Thickness of the border
		/// </summary>
		private float borderThickness  = 1;

		/// <summary>
		/// Bar orientation, whether horizontal or vertical
		/// </summary>
		private Orientation barOrientation = Orientation.Vertical;
		/// <summary>
		/// A value beyond which the readings are above normal.
		/// The graph can be displayed in a different color if it
		/// goes above this range
		/// Eg. body temp. above 40 degree C
		/// </summary>
		private float aboveRangeValue;
		/// <summary>
		/// A value beyond which the readings are below normal.
		/// The graph can be displayed in a different color if it
		/// goes below this range
		/// Eg. body temp. below 35 degree C
		/// </summary>
		private float belowRangeValue;
		/// <summary>
		/// The color to display the arrow marks with if the value is outside range
		/// </summary>
		private Color outOfRangeArrowColor = Color.Salmon;
		/// <summary>
		/// The alignment of the value text
		/// </summary>
		private TextAlignment valueAlignment = TextAlignment.Smart;
		/// <summary>
		/// A flag indicating whether lines for above and below normal range are to be displayed
		/// </summary>
		private bool showRangeLines = true;
		/// <summary>
		/// A flag indicating whether the actual values of above and below range are to be displayed
		/// </summary>
		private bool showRangeValues;
		/// <summary>
		/// The location where the graduations must appear
		/// </summary>
		private Graduation barGraduation = Graduation.Edge2;
		/// <summary>
		/// The .NET style of format for displaying the value
		/// </summary>
		private string valueFormat = "";

		/// <summary>
		/// The rectangle of the BasicBar
		/// </summary>
		private Rectangle clientRectangle;

		/// <summary>
		/// The color in which the values are displayed.
		/// </summary>
		private Color foreColor = Color.Black;
		/// <summary>
		/// The font in which the values are displayed
		/// </summary>
		private Font textFont;

		#endregion variables

		#region properties
		#region BarValue
		/// <summary>
		/// Current value of the bar. Gets or sets the value to display on the bar
		/// </summary>
		public float BarValue
		{
			get { return barValue; }
			set
			{
				if (value != barValue)
					barValue = value;
			}
		}
		#endregion BarValue

		#region MaximumValue
		/// <summary>
		/// Maximum value of the bar
		/// </summary>
		public float MaximumValue
		{
			get { return maximumValue;  }
			set { maximumValue = value; }
		}
		#endregion MaximumValue

		#region MaximumValue
		/// <summary>
		/// Minimum value of the bar
		/// </summary>
		public float MinimumValue
		{
			get { return minimumValue; }
			set
			{
				minimumValue = value;
			}
		}
		#endregion MinimumValue

		#region BarColor
		/// <summary>
		/// Color of the bar
		/// </summary>
		public Color BarColor
		{
			get { return barColor;  }
			set { barColor = value; }
		}
		#endregion BarColor

		#region ForeColor
		/// <summary>
		/// The color in which the values are displayed.
		/// </summary>
		public Color ForeColor
		{
			get { return foreColor;  }
			set { foreColor = value; }
		}
		#endregion ForeColor

		#region BorderColor
		/// <summary>
		/// Color of the graph's border that includes only the area
		/// that is filled up and not the entire rectangle
		/// </summary>
		public Color BorderColor
		{
			get { return graphBorderColor;  }
			set { graphBorderColor = value; }
		}
		#endregion BorderColor

		#region BarOrientation
		/// <summary>
		/// Bar orientation, whether horizontal or vertical
		/// </summary>
		public Orientation BarOrientation
		{
			get { return barOrientation;  }
			set { barOrientation = value; }
		}

		#endregion BarOrientation

		#region AboveRangeValue
		/// <summary>
		/// A value beyond which the readings are above normal.
		/// The graph can be displayed in a different color if it
		/// goes above this range
		/// Eg. body temp. above 40 degree C
		/// </summary>
		public float AboveRangeValue
		{
			get { return aboveRangeValue;  }
			set { aboveRangeValue = value; }
		}

		#endregion

		#region BelowRangeValue
		/// <summary>
		/// A value beyond which the readings are below normal.
		/// The graph can be displayed in a different color if it
		/// goes below this range
		/// Eg. body temp. below 35 degree C
		/// </summary>
		public float BelowRangeValue
		{
			get { return belowRangeValue;  }
			set { belowRangeValue = value; }
		}

		#endregion BelowRangeValue

		#region OutOfRangeArrowColor
		/// <summary>
		/// The color to display the arrow marks with if the value is outside range
		/// </summary>
		public Color OutOfRangeArrowColor
		{
			get { return outOfRangeArrowColor;  }
			set { outOfRangeArrowColor = value; }
		}
		#endregion OutOfRangeArrowColor

		#region TextAlignment
		/// <summary>
		/// The alignment of the value text
		/// </summary>
		public TextAlignment ValueAlignment
		{
			get { return valueAlignment; }
			set
			{
				valueAlignment = value;
			}
		}
		#endregion TextAlignment

		#region ShowRangeLines
		/// <summary>
		/// A flag indicating whether lines for above and below normal range are to be displayed
		/// </summary>
		public bool ShowRangeLines
		{
			get { return showRangeLines; }
			set
			{
				showRangeLines = value;
			}
		}
		#endregion ShowRangeLines

		#region ShowRangeValues
		/// <summary>
		/// A flag indicating whether the actual values of above and below range are to be displayed
		/// </summary>
		public bool ShowRangeValues
		{
			get { return showRangeValues; }
			set
			{
				showRangeValues = value;
			}
		}
		#endregion ShowRangeValues

		#region BarGraduation
		/// <summary>
		/// The location where the graduations must appear
		/// </summary>
		public Graduation BarGraduation
		{
			get { return barGraduation; }
			set
			{
				barGraduation = value;
			}
		}
		#endregion BarGraduation

		#region ValueFormat
		/// <summary>
		/// The .NET style of format for displaying the value
		/// </summary>
		public string ValueFormat
		{
			get { return valueFormat;  }
			set { valueFormat = value; }
		}
		#endregion ValueFormat

		#region ClientRectangle
		/// <summary>
		/// The rectangle of the BasicBar
		/// </summary>
		public Rectangle ClientRectangle
		{
			get { return clientRectangle;  }
			set { clientRectangle = value; }
		}
		#endregion ClientRectangle

		#region TextFont
		/// <summary>
		/// The font in which the values are displayed
		/// </summary>
		public Font TextFont
		{
			get { return textFont;  }
			set { textFont = value; }
		}
		#endregion TextFont
		#endregion properties

		#region methods
		public BasicBar()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region IGraphElement Members
		/// <summary>
		/// Draws the bar with the properties specified in its member variables
		/// </summary>
		/// <param name="graphics"></param>
		public void Draw(Graphics graphics)
		{
			graphics.SetClip (ClientRectangle);

			graphics.SmoothingMode = SmoothingMode.AntiAlias;

			#region draw bar
			SolidBrush barBrush = new SolidBrush (BarColor);
			float barHeight = 0;
			float barWidth  = 0;
			RectangleF barRect;
			float barRange = MaximumValue - MinimumValue;

			if (BarOrientation == Orientation.Vertical)
			{
				barHeight = (float) ClientRectangle.Height * ( (BarValue - MinimumValue) / barRange);
				barWidth  = ClientRectangle.Width;

				barRect   = new RectangleF (ClientRectangle.Left, ClientRectangle.Bottom - barHeight, barWidth, barHeight);
			}
			else
			{
				barHeight = ClientRectangle.Height;
				barWidth  = (float) ClientRectangle.Width * ( (BarValue - MinimumValue) / barRange);

				barRect   = new RectangleF (ClientRectangle.Left, ClientRectangle.Top, barWidth, barHeight);

			}

			graphics.FillRectangle (barBrush, barRect);
			barBrush.Dispose ();

			#endregion draw bar

			DrawBarValue (graphics, barRect);
			DrawRangeLines (graphics, barRect);
			DrawRangeValues (graphics, barRect);

			#region border
			Pen borderPen = new Pen (new SolidBrush (BorderColor), borderThickness);
			if (BarOrientation == Orientation.Vertical)
				graphics.DrawRectangle (borderPen, ClientRectangle.Left, ClientRectangle.Bottom - barHeight, barWidth - borderThickness, barHeight);
			else
				graphics.DrawRectangle (borderPen, ClientRectangle.Left, ClientRectangle.Bottom - barHeight, barWidth - borderThickness, barHeight - borderThickness);
			borderPen.Dispose ();
			#endregion border

			DrawGraduations (graphics, barRect);
			DrawOutOfRangeArrows (graphics, barRect);
		}

		#endregion

		/// <summary>
		/// Draws the value of the bar
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="barRect"></param>
		private void DrawBarValue (Graphics graphics, RectangleF barRect)
		{
			if (ValueAlignment == TextAlignment.None)
				return;

			Brush textBrush = new SolidBrush (this.ForeColor);

			StringFormat sf = new StringFormat ();
			sf.Trimming     = StringTrimming.Character;
			sf.FormatFlags  = StringFormatFlags.NoWrap;

			RectangleF textRect;

			string val = "";
			if (ValueFormat.Length != 0)
				val = string.Format (CultureInfo.CurrentUICulture, ValueFormat, BarValue);
			else
				val = BarValue.ToString (CultureInfo.CurrentUICulture);

			if (ValueAlignment == TextAlignment.AbsoluteCenter)
			{
				sf.Alignment     = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				textRect         = ClientRectangle;
			}
			else if (ValueAlignment == TextAlignment.BarValueCenter)
			{
				sf.Alignment     = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				textRect         = barRect;
			}
			else
			{
				// smart text alignment
				if (BarOrientation == Orientation.Vertical)
				{
					sf.Alignment     = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Near;

					if (barRect.Top - ClientRectangle.Top - 2 > TextFont.Height)
						textRect = new RectangleF (barRect.Left, barRect.Top - 2 - TextFont.Height, barRect.Width, barRect.Height);
					else
						textRect = barRect;
				}
				else
				{
					sf.Alignment     = StringAlignment.Far;
					sf.LineAlignment = StringAlignment.Center;

					SizeF textSize = graphics.MeasureString (val, TextFont);

					if (barRect.Width + textSize.Width + 2 < ClientRectangle.Width)
						textRect = new RectangleF (barRect.Left, barRect.Top, barRect.Width + textSize.Width + 2, barRect.Height);
					else
						textRect = barRect;
				}

			}

			graphics.DrawString (val, TextFont, textBrush, textRect, sf);

			textBrush.Dispose ();

		}

		/// <summary>
		/// Draws the upper and lower range lines
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="barRect"></param>
		private void DrawRangeLines (Graphics graphics, RectangleF barRect)
		{
			if (! ShowRangeLines)
				return;

			Pen pen = new Pen (Color.Black);
			pen.DashStyle = DashStyle.Dash;

			if (BarOrientation == Orientation.Vertical)
			{
				float aboveRangeHeight = (float) ClientRectangle.Height * (AboveRangeValue / MaximumValue);
				float belowRangeHeight = (float) ClientRectangle.Height * (BelowRangeValue / MaximumValue);

				graphics.DrawLine (pen, barRect.Left, ClientRectangle.Height - aboveRangeHeight, barRect.Right, ClientRectangle.Height - aboveRangeHeight);
				graphics.DrawLine (pen, barRect.Left, ClientRectangle.Height - belowRangeHeight, barRect.Right, ClientRectangle.Height - belowRangeHeight);
			}
			else
			{
				float aboveRangeWidth = (float) ClientRectangle.Width * (AboveRangeValue / MaximumValue);
				float belowRangeWidth = (float) ClientRectangle.Width * (BelowRangeValue / MaximumValue);

				graphics.DrawLine (pen, aboveRangeWidth, barRect.Top, aboveRangeWidth, barRect.Bottom);
				graphics.DrawLine (pen, belowRangeWidth, barRect.Top, belowRangeWidth, barRect.Bottom);
			}

			pen.Dispose ();
		}

		/// <summary>
		/// Draws the upper and lower range values
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="barRect"></param>
		private void DrawRangeValues (Graphics graphics, RectangleF barRect)
		{
			if (! ShowRangeValues)
				return;

			Brush textBrush = new SolidBrush (this.ForeColor);

			StringFormat sf = new StringFormat ();
			sf.Trimming     = StringTrimming.Character;
			sf.FormatFlags  = StringFormatFlags.NoWrap;

			RectangleF aboveTextRect;
			RectangleF belowTextRect;

			if (BarOrientation == Orientation.Vertical)
			{
				float aboveRangeHeight = (float) ClientRectangle.Height * (AboveRangeValue / MaximumValue);
				float belowRangeHeight = (float) ClientRectangle.Height * (BelowRangeValue / MaximumValue);

				RectangleF aboveRangeRect = new RectangleF (barRect.Left, ClientRectangle.Height - aboveRangeHeight, barRect.Width, aboveRangeHeight);
				RectangleF belowRangeRect = new RectangleF (barRect.Left, ClientRectangle.Height - belowRangeHeight, barRect.Width, belowRangeHeight);

				sf.Alignment     = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Near;

				if (aboveRangeRect.Top - 2 > TextFont.Height)
					aboveTextRect = new RectangleF (aboveRangeRect.Left, aboveRangeRect.Top - 2 - TextFont.Height, aboveRangeRect.Right, aboveRangeRect.Bottom);
				else
					aboveTextRect = aboveRangeRect;

				if (belowRangeRect.Top - 2 > TextFont.Height)
					belowTextRect = new RectangleF (belowRangeRect.Left, belowRangeRect.Top - 2 - TextFont.Height, belowRangeRect.Right, belowRangeRect.Bottom);
				else
					belowTextRect = belowRangeRect;

			}
			else
			{
				float aboveRangeWidth = (float) ClientRectangle.Width * (AboveRangeValue / MaximumValue);
				float belowRangeWidth = (float) ClientRectangle.Width * (BelowRangeValue / MaximumValue);

				RectangleF aboveRangeRect = new RectangleF (barRect.Left, barRect.Top, aboveRangeWidth, barRect.Height);
				RectangleF belowRangeRect = new RectangleF (barRect.Left, barRect.Top, belowRangeWidth, barRect.Height);

				sf.Alignment     = StringAlignment.Far;
				sf.LineAlignment = StringAlignment.Center;

				SizeF aboveTextSize = graphics.MeasureString (AboveRangeValue.ToString (CultureInfo.CurrentUICulture), TextFont);
				SizeF belowTextSize = graphics.MeasureString (BelowRangeValue.ToString (CultureInfo.CurrentUICulture), TextFont);

				if (aboveRangeWidth + aboveTextSize.Width + 2 < ClientRectangle.Width)
					aboveTextRect = new RectangleF (aboveRangeRect.Left, aboveRangeRect.Top, aboveRangeRect.Width + aboveTextSize.Width + 2, aboveRangeRect.Bottom);
				else
					aboveTextRect = aboveRangeRect;

				if (belowRangeWidth + belowTextSize.Width + 2 < ClientRectangle.Width)
					belowTextRect = new RectangleF (belowRangeRect.Left, belowRangeRect.Top, belowRangeRect.Width + belowTextSize.Width + 2, belowRangeRect.Bottom);
				else
					belowTextRect = belowRangeRect;

			}

			graphics.DrawString (AboveRangeValue.ToString (CultureInfo.CurrentUICulture), TextFont, textBrush, aboveTextRect, sf);
			graphics.DrawString (BelowRangeValue.ToString (CultureInfo.CurrentUICulture), TextFont, textBrush, belowTextRect, sf);

			textBrush.Dispose ();
		}

		/// <summary>
		/// Draws the bar graduations
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="barRect"></param>
		private void DrawGraduations (Graphics graphics, RectangleF barRect)
		{
			if (BarOrientation == Orientation.Vertical)
			{
				if (BarGraduation == Graduation.Center)
				{
					Pen marksPen = new Pen (new SolidBrush (Color.Black), 0.25F);
					graphics.DrawLine (marksPen, barRect.Left + (barRect.Width / 2), ClientRectangle.Top, barRect.Left + (barRect.Width / 2), ClientRectangle.Bottom);

					for (int height = 0; height < ClientRectangle.Height; height += ClientRectangle.Height / 5)
					{
						graphics.DrawLine (marksPen, barRect.Left + (barRect.Width / 2) - 5, ClientRectangle.Top + height, barRect.Left + (barRect.Width / 2) + 5, ClientRectangle.Top + height);
					}
				}
				else if (BarGraduation == Graduation.Edge1)
				{
					Pen marksPen = new Pen (new SolidBrush (Color.Black), 0.25F);
					graphics.DrawLine (marksPen, barRect.Left, ClientRectangle.Top, barRect.Left, ClientRectangle.Bottom);

					for (int height = 0; height < ClientRectangle.Height; height += ClientRectangle.Height / 5)
					{
						graphics.DrawLine (marksPen, barRect.Left, ClientRectangle.Top + height, barRect.Left + 10, ClientRectangle.Top + height);
					}
				}
				else if (BarGraduation == Graduation.Edge2)
				{
					Pen marksPen = new Pen (new SolidBrush (Color.Black), 0.25F);
					graphics.DrawLine (marksPen, barRect.Right - 0.25F, ClientRectangle.Top, barRect.Right - 0.25F, ClientRectangle.Bottom);

					for (int height = 0; height < ClientRectangle.Height; height += ClientRectangle.Height / 5)
					{
						graphics.DrawLine (marksPen, barRect.Right, ClientRectangle.Top + height, barRect.Right - 10, ClientRectangle.Top + height);
					}
				}
			}
			else
			{
				if (BarGraduation == Graduation.Center)
				{
					Pen marksPen = new Pen (new SolidBrush (Color.Black), 0.25F);
					graphics.DrawLine (marksPen, ClientRectangle.Left, barRect.Top + barRect.Height / 2, ClientRectangle.Right, barRect.Top + barRect.Height / 2);

					for (int width = 0; width < ClientRectangle.Width; width += ClientRectangle.Width / 5)
					{
						graphics.DrawLine (marksPen, ClientRectangle.Left + width, ClientRectangle.Top + ClientRectangle.Height / 2 - 5, ClientRectangle.Left + width, ClientRectangle.Top + ClientRectangle.Height / 2 + 5);
					}
				}
				else if (BarGraduation == Graduation.Edge1)
				{
					Pen marksPen = new Pen (new SolidBrush (Color.Black), 0.25F);
					graphics.DrawLine (marksPen, ClientRectangle.Left, barRect.Top, ClientRectangle.Right, barRect.Top);

					for (int width = 0; width < ClientRectangle.Width; width += ClientRectangle.Width / 5)
					{
						graphics.DrawLine (marksPen, ClientRectangle.Left + width, ClientRectangle.Top, ClientRectangle.Left + width, ClientRectangle.Top + 10);
					}

				}
				else if (BarGraduation == Graduation.Edge2)
				{
					Pen marksPen = new Pen (new SolidBrush (Color.Black), 0.25F);
					graphics.DrawLine (marksPen, ClientRectangle.Left, barRect.Bottom, ClientRectangle.Right, barRect.Bottom);

					for (int width = 0; width < ClientRectangle.Width; width += ClientRectangle.Width / 5)
					{
						graphics.DrawLine (marksPen, ClientRectangle.Left + width, ClientRectangle.Bottom, ClientRectangle.Left + width, ClientRectangle.Bottom - 10);
					}
				}

			}

		}

		/// <summary>
		/// Draws the arrows when the bar value is outside the specified upper
		/// or below ranges
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="barRect"></param>
		private void DrawOutOfRangeArrows (Graphics graphics, RectangleF barRect)
		{
			#region above the max value
			if (BarValue > MaximumValue)
			{
				Brush outOfRangeBrush = new SolidBrush (OutOfRangeArrowColor);

				if (BarOrientation == Orientation.Vertical)
				{
					float arrowLength = ((float) ClientRectangle.Height) / 10;

					Pen arrowPen = new Pen (outOfRangeBrush, barRect.Width / 5);
					arrowPen.EndCap = LineCap.ArrowAnchor;

					graphics.DrawLine (arrowPen, barRect.Left + (barRect.Width / 2), ClientRectangle.Top + (1.5F * arrowLength),  barRect.Left + (barRect.Width / 2), ClientRectangle.Top + (0.5F * arrowLength));

				}
				else
				{
					float arrowLength = ((float) ClientRectangle.Width) / 10;

					Pen arrowPen = new Pen (outOfRangeBrush, barRect.Height / 5);
					arrowPen.EndCap = LineCap.ArrowAnchor;

					graphics.DrawLine (arrowPen, ClientRectangle.Right - (1.5F * arrowLength), barRect.Bottom - (barRect.Height / 2), ClientRectangle.Right - (0.5F * arrowLength),  barRect.Bottom - (barRect.Height / 2));

				}

				outOfRangeBrush.Dispose ();
			}
			#endregion above the max value
			#region below the min value
			else if (BarValue < MinimumValue)
			{
				Brush outOfRangeBrush = new SolidBrush (OutOfRangeArrowColor);

				if (BarOrientation == Orientation.Vertical)
				{
					float arrowLength = ((float) ClientRectangle.Height) / 10;

					Pen arrowPen = new Pen (outOfRangeBrush, barRect.Width / 5);
					arrowPen.EndCap = LineCap.ArrowAnchor;

					graphics.DrawLine (arrowPen, barRect.Left + (barRect.Width / 2), ClientRectangle.Bottom - (1.5F * arrowLength),  barRect.Left + (barRect.Width / 2), ClientRectangle.Bottom - (0.5F * arrowLength));

				}
				else
				{
					float arrowLength = ((float) ClientRectangle.Width) / 10;

					Pen arrowPen = new Pen (outOfRangeBrush, barRect.Height / 5);
					arrowPen.EndCap = LineCap.ArrowAnchor;

					graphics.DrawLine (arrowPen, ClientRectangle.Left + (1.5F * arrowLength), barRect.Bottom - (barRect.Height / 2), ClientRectangle.Left + (0.5F * arrowLength),  barRect.Bottom - (barRect.Height / 2));

				}

				outOfRangeBrush.Dispose ();
			}
			#endregion below the min value
		}
		#endregion methods
	}
}
