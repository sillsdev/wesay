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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// Summary description for StackedBarGraph.
	/// </summary>
	public class StackedBarGraph: Graph
	{
		#region variables

		/// <summary>
		/// The left side margin width of the graph
		/// </summary>
		private int graphMarginLeft = 50;

		/// <summary>
		/// The upper margin width of the graph
		/// </summary>
		private int graphMarginTop = 20;

		/// <summary>
		/// The right side margin width of the graph
		/// </summary>
		private int graphMarginRight = 20;

		/// <summary>
		/// The lower margin width of the graph
		/// </summary>
		private int graphMarginBottom = 20;

		/// <summary>
		/// Maximum value of the bar
		/// </summary>
		private float maximumValue = 100;

		/// <summary>
		/// Minimum value of the bar
		/// </summary>
		private float minimumValue;

		/// <summary>
		/// Color of the bar
		/// </summary>
		private Color barColor = Color.Lime;

		/// <summary>
		/// The color to display if the value is above normal
		/// </summary>
		private Color aboveRangeColor = Color.Salmon;

		/// <summary>
		/// The color to display if the value is below normal
		/// </summary>
		private Color belowRangeColor = Color.Thistle;

		/// <summary>
		/// Bar orientation, whether horizontal or vertical
		/// </summary>
		private Orientation barOrientation = Orientation.Vertical;

		/// <summary>
		/// Color of the graph's border that includes only the area
		/// that is filled up and not the entire rectangle
		/// </summary>
		private readonly Color graphBorderColor = Color.Black;

		/// <summary>
		/// The alignment of the value text
		/// </summary>
		private TextAlignment valueAlignment = TextAlignment.Smart;

		/// <summary>
		/// The collection that contains all the individual bars
		/// </summary>
		private readonly BarCollection barCollection;

		/// <summary>
		/// Used to draw the gridline
		/// </summary>
		private readonly Gridline gridline;

		/// <summary>
		/// Used to draw the x and y axis line
		/// </summary>
		private readonly AxisLine axisLineXandY;

		private RectangleF aboveRangeRect;
		private RectangleF belowRangeRect;

		/// <summary>
		/// A ratio that defines the ratio between the width of each bar
		/// to the spacing from its adjacent bar
		/// </summary>
		private float barWidthToSpacingRatio = 1;

		/// <summary>
		/// The basic bar object that does the actual rendering.
		/// </summary>
		private readonly BasicBar basicBar;

		/// <summary>
		/// A value beyond which the readings are above normal.
		/// The graph can be displayed in a different color if it
		/// goes above this range
		/// Eg. body temp. above 40 degree C
		/// </summary>
		private float aboveRangeValue = 70;

		/// <summary>
		/// A value beyond which the readings are below normal.
		/// The graph can be displayed in a different color if it
		/// goes below this range
		/// Eg. body temp. below 35 degree C
		private float belowRangeValue = 30;

		/// <summary>
		/// Flag indicating whether lines for above and below normal limits are to be displayed
		/// </summary>
		private bool showRangeLines = true;

		/// <summary>
		/// Flag indicating whether the values of above and below ranges are to be displayed
		/// </summary>
		private bool showRangeValues;

		#endregion variables

		#region properties

		#region ShowRangeLines

		/// <summary>
		/// Flag indicating whether lines for above and below normal limits are to be displayed
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description(
				"Flag indicating whether lines for above and below normal limits are to be displayed"
				)]
		public bool ShowRangeLines
		{
			get { return showRangeLines; }
			set
			{
				showRangeLines = value;
				RefreshDisplay();
			}
		}

		#endregion ShowRangeLines

		#region ShowRangeValues

		/// <summary>
		/// Flag indicating whether the values of above and below ranges are to be displayed
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description(
				"Flag indicating whether the values of above and below ranges are to be displayed")]
		public bool ShowRangeValues
		{
			get { return showRangeValues; }
			set
			{
				showRangeValues = value;
				RefreshDisplay();
			}
		}

		#endregion ShowRangeValues

		#region AboveRangeValue

		/// <summary>
		/// A value beyond which the readings are above normal.
		/// The bar can be displayed in a different color if it
		/// goes above this range
		/// Eg. body temp. above 40 degree C
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description(
				"A value beyond which the readings are above normal. The bar can be displayed in a different color if it goes above this range."
				)]
		public float AboveRangeValue
		{
			get { return aboveRangeValue; }
			set
			{
				aboveRangeValue = value;
				RefreshDisplay();
			}
		}

		#endregion AboveRangeValue

		#region BelowRangeValue

		/// <summary>
		/// A value beyond which the readings are below normal.
		/// The bar can be displayed in a different color if it
		/// goes below this range
		/// Eg. body temp. below 35 degree C
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description(
				"A value beyond which the readings are below normal. The bar can be displayed in a different color if it goes below this range"
				)]
		public float BelowRangeValue
		{
			get { return belowRangeValue; }
			set
			{
				belowRangeValue = value;
				RefreshDisplay();
			}
		}

		#endregion BelowRangeValue

		#region Bars

		[Browsable(false)]
		public BarCollection Bars
		{
			get { return barCollection; }
		}

		#endregion Bars

		#region GraphMargin

		/// <summary>
		/// The left side margin width of the graph
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The left side margin width of the graph")]
		public int GraphMarginLeft
		{
			get { return graphMarginLeft; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Invalid property value. Margin cannot be negative.");
				}

				graphMarginLeft = value;
				RefreshDisplay();
			}
		}

		/// <summary>
		/// The upper margin width of the graph
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The upper margin width of the graph")]
		public int GraphMarginTop
		{
			get { return graphMarginTop; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Invalid property value. Margin cannot be negative.");
				}

				graphMarginTop = value;
				RefreshDisplay();
			}
		}

		/// <summary>
		/// The right side margin width of the graph
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The right side margin width of the graph")]
		public int GraphMarginRight
		{
			get { return graphMarginRight; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Invalid property value. Margin cannot be negative.");
				}

				graphMarginRight = value;
				RefreshDisplay();
			}
		}

		/// <summary>
		/// The lower margin width of the graph
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The lower margin width of the graph")]
		public int GraphMarginBottom
		{
			get { return graphMarginBottom; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Invalid property value. Margin cannot be negative.");
				}

				graphMarginBottom = value;
				RefreshDisplay();
			}
		}

		#endregion GraphMargin

		#region AboveRangeColor

		/// <summary>
		/// The color to display if the value is above normal
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The color to display if the value is above normal")]
		public Color AboveRangeColor
		{
			get { return aboveRangeColor; }
			set
			{
				aboveRangeColor = value;
				RefreshDisplay();
			}
		}

		#endregion AboveRangeColor

		#region BelowRangeColor

		/// <summary>
		/// The color to display if the value is below normal
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The color to display if the value is below normal")]
		public Color BelowRangeColor
		{
			get { return belowRangeColor; }
			set
			{
				belowRangeColor = value;
				RefreshDisplay();
			}
		}

		#endregion BelowRangeColor

		#region MaximumValue

		/// <summary>
		/// Maximum value of the bar
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Maximum value of the bar")]
		public float MaximumValue
		{
			get { return maximumValue; }
			set
			{
				maximumValue = value;
				RefreshDisplay();
			}
		}

		#endregion MaximumValue

		#region MinimumValue

		/// <summary>
		/// Minimum value of the bar
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Minimum value of the bar")]
		public float MinimumValue
		{
			get { return minimumValue; }
			set
			{
				minimumValue = value;
				RefreshDisplay();
			}
		}

		#endregion MinimumValue

		#region BarColor

		/// <summary>
		/// Color of the bar
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Color of the bar")]
		public Color BarColor
		{
			get { return barColor; }
			set
			{
				barColor = value;
				RefreshDisplay();
			}
		}

		#endregion BarColor

		#region BarWidthToSpacingRatio

		/// <summary>
		/// A ratio that defines the ratio between the width
		/// of each bar to the spacing from its adjacent bar
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[NotifyParentProperty(true)]
		[Category("Appearance")]
		[Description(
				"A ratio that defines the ratio between the width of each bar to the spacing from its adjacent bar"
				)]
		public float BarWidthToSpacingRatio
		{
			get { return barWidthToSpacingRatio; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException(
							"Invalid property value. Ratio has to be greater than 0.");
				}

				barWidthToSpacingRatio = value;
				RefreshDisplay();
			}
		}

		#endregion BarWidthToSpacingRatio

		#region BarCount

		/// <summary>
		/// Number of bars to display on the stacked bar graph
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Number of bars to display on the stacked bar graph")]
		public int BarCount
		{
			get { return barCollection.Count; } //			set
			//			{
			//				if (value < 0)
			//					throw new ArgumentException ("Invalid property value. Bar count cannot be negative.");
			//
			//				if (BarCount < value)
			//				{
			//					// we need to add a few bars here...
			//					int numOfNewBars = value - BarCount;
			//					for (int i = 0; i < numOfNewBars; i ++)
			//						Bars.Add (new Bar (i.ToString (CultureInfo.CurrentUICulture), 50));
			//				}
			//				else if (BarCount > value)
			//				{
			//					// we need to remove a few bars from the back side..
			//					int numOfBarsToRemove = value - BarCount;
			//
			//					for (int i = 0; i < numOfBarsToRemove; i ++)
			//						Bars.RemoveAt (BarCount - 1 - i);
			//				}
			//
			//				//BarCount = value;
			////				Bars.Clear ();
			////				for (int i = 0; i < barCount; i ++)
			////					Bars.Add (new Bar (i.ToString (CultureInfo.CurrentUICulture), 50));
			//
			//				RefreshDisplay ();
			//			}
		}

		#endregion BarCount

		#region BarOrientation

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Bar orientation, whether horizontal or vertical")]
		public Orientation BarOrientation
		{
			get { return barOrientation; }
			set
			{
				barOrientation = value;
				RefreshDisplay();
			}
		}

		#endregion BarOrientation

		#region TextAlignment

		/// <summary>
		/// The alignment of the value's text
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The alignment of the value's text")]
		public TextAlignment ValueAlignment
		{
			get { return valueAlignment; }
			set
			{
				valueAlignment = value;
				RefreshDisplay();
			}
		}

		#endregion TextAlignment

		#endregion properties

		#region methods

		/// <summary>
		/// Creates a stacked bar graph user control with default properties
		/// </summary>
		public StackedBarGraph()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
			basicBar = new BasicBar();
			basicBar.ShowRangeLines = false;
			basicBar.ShowRangeValues = false;
			basicBar.BarGraduation = Graduation.None;

			gridline = new Gridline(this);
			axisLineXandY = new AxisLine(this);

			barCollection = new BarCollection();

			for (int i = 0;i < BarCount;i ++)
			{
				Bars.Add(new Bar(i.ToString(CultureInfo.CurrentUICulture), 50));
			}

			SetStyle(
					ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
					ControlStyles.AllPaintingInWmPaint,
					true);
			UpdateStyles();

			GraphArea =
					new Rectangle(ClientRectangle.Left + graphMarginLeft,
								  ClientRectangle.Top + graphMarginTop,
								  ClientRectangle.Width - graphMarginRight - graphMarginLeft,
								  ClientRectangle.Height - graphMarginBottom - graphMarginTop);

			Debug.Assert(GraphArea.Height == (GraphArea.Bottom - GraphArea.Top), "Problem Ctor");
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			//
			// StackedBarGraph
			//
			this.Name = "StackedBarGraph";
			this.Size = new System.Drawing.Size(344, 256);
		}

		#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
			if (! Visible)
			{
				return;
			}

			Graphics graphics = e.Graphics;

			Draw(graphics);

			base.OnPaint(e);
		}

		#region IGraphElement Members

		public override void Draw(Graphics graphics)
		{
			// TODO:  Add StackedBarGraph.Draw implementation
			graphics.SmoothingMode = SmoothingMode.AntiAlias;

			CalculateGraphArea();

			if (GraphArea.Width == 0 || GraphArea.Height == 0)
			{
				return;
			}

			if (MinimumValue >= MaximumValue)
			{
				return;
			}

			graphics.SetClip(GraphArea);

			gridline.Draw(graphics);

			DrawRangeLinesAndValues(graphics);

			#region comments describing bar spacing

			// For a vertical bar orientation...
			// We need to leave some gap at the left edge of the Y axis, otherwise,
			// the bar graph will look ugly
			// suppose we have a barWidthToSpacingRatio of 1.5 and barCount is 5,
			// then if the graphArea.Width is 100, we need the bars to be as follows
			// space Bar1 space1 Bar2 space2 Bar3 space3 Bar4 space4 Bar5 space5
			// So, we have 5 bars and 6 space regions
			// and barWidth = 1.5 times barSpacing
			// so, barSpacing is calculated as follows
			// 6*barSpacing + 1.5*5*barSpacing = 100
			// 13.5*barSpacing = 100

			#endregion comments describing bar spacing

			float x = (BarCount * barWidthToSpacingRatio) + (BarCount + 1);

			float barSpacing;
			if (barOrientation == Orientation.Vertical)
			{
				barSpacing = GraphArea.Width / x;
			}
			else
			{
				barSpacing = GraphArea.Height / x;
			}

			float barWidth = barSpacing * barWidthToSpacingRatio;
			float barArea = barWidth + barSpacing;

			float currentBarOffset;

			if (barOrientation == Orientation.Vertical)
			{
				currentBarOffset = GraphArea.Left + barSpacing;
			}
			else
			{
				currentBarOffset = GraphArea.Bottom - barSpacing - barWidth;
			}

			basicBar.MaximumValue = minimumValue;
			basicBar.MaximumValue = maximumValue;

			for (int i = 0;i < BarCount;i ++)
			{
				float barValue = barCollection[i].BarValue;

				#region bar color

				Color graphColor = barColor;

				if (barValue > aboveRangeValue)
				{
					graphColor = aboveRangeColor;
				}
				else if (barValue < belowRangeValue)
				{
					graphColor = belowRangeColor;
				}

				graphColor = Color.FromArgb(Transparency, graphColor);

				#endregion bar color

				#region draw bar

				if (barOrientation == Orientation.Vertical)
				{
					basicBar.ClientRectangle =
							new Rectangle((int) currentBarOffset,
										  GraphArea.Top,
										  (int) barWidth,
										  GraphArea.Height);
				}
				else
				{
					basicBar.ClientRectangle =
							new Rectangle(GraphArea.Left,
										  (int) currentBarOffset,
										  GraphArea.Width,
										  (int) barWidth);
				}

				basicBar.BarColor = graphColor;
				basicBar.BarValue = barValue;
				basicBar.BarOrientation = barOrientation;
				basicBar.ForeColor = ForeColor;
				basicBar.ValueFormat = ValueFormat;
				basicBar.ValueAlignment = valueAlignment;
				basicBar.TextFont = Font;
				basicBar.BorderColor = graphBorderColor;
				basicBar.OutOfRangeArrowColor = BarColor;

				basicBar.Draw(graphics);

				if (barOrientation == Orientation.Vertical)
				{
					currentBarOffset += barArea;
				}
				else
				{
					currentBarOffset -= barArea;
				}

				#endregion draw bar
			}

			axisLineXandY.Draw(graphics);
			DrawBarNames(graphics, barArea, barSpacing);
			DrawBarAxisValues(graphics);
		}

		private void DrawBarAxisValues(Graphics graphics)
		{
			graphics.SetClip(ClientRectangle);

			if (barOrientation == Orientation.Vertical)
			{
				StringFormat sf = new StringFormat();
				sf.Trimming = StringTrimming.Character;
				sf.FormatFlags = StringFormatFlags.NoWrap;
				sf.Alignment = StringAlignment.Far;
				sf.LineAlignment = StringAlignment.Center;

				Brush textBrush = new SolidBrush(YAxisColor);

				float offset = GraphArea.Top - Font.Height / 2;
				float graduationPixelDiff = GraphArea.Height / GraduationsY;
				float valueOffset = 0;

				float graduationDiff = (maximumValue - minimumValue) / GraduationsY;

				for (int i = GraduationsY;i >= 0;i --)
				{
					RectangleF axisValuesRect =
							new RectangleF(ClientRectangle.Left,
										   offset,
										   GraphMarginLeft - Font.Height / 2,
										   Font.Height);
					float graduationValue = maximumValue - graduationDiff * valueOffset;
					valueOffset ++;

					if (axisValuesRect.IntersectsWith(aboveRangeRect) ||
						axisValuesRect.IntersectsWith(belowRangeRect))
					{
						offset += graduationPixelDiff;
						continue;
					}

					string val;
					if (ValueFormat.Length != 0)
					{
						val =
								string.Format(CultureInfo.CurrentUICulture,
											  ValueFormat,
											  graduationValue);
					}
					else
					{
						val = graduationValue.ToString(CultureInfo.CurrentUICulture);
					}

					graphics.DrawString(val, Font, textBrush, axisValuesRect, sf);
					offset += graduationPixelDiff;
				}
			}
			else
			{
				if (GraduationsX == 0)
				{
					return;
				}
				StringFormat sf = new StringFormat();
				sf.Trimming = StringTrimming.Character;
				sf.FormatFlags = StringFormatFlags.NoWrap;
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;

				Brush textBrush = new SolidBrush(XAxisColor);

				float offset = GraphArea.Left;
				float graduationPixelDiff = GraphArea.Width / GraduationsX;

				float graduationDiff = (maximumValue - minimumValue) / GraduationsX;

				for (int i = 0;i <= GraduationsX;i ++)
				{
					float graduationValue = minimumValue + graduationDiff * i;

					string val;
					if (ValueFormat.Length != 0)
					{
						val =
								string.Format(CultureInfo.CurrentUICulture,
											  ValueFormat,
											  graduationValue);
					}
					else
					{
						val = graduationValue.ToString(CultureInfo.CurrentUICulture);
					}

					SizeF numberSize = graphics.MeasureString(val, Font, (int) graduationPixelDiff);

					RectangleF axisValuesRect =
							new RectangleF(offset - numberSize.Width / 2,
										   GraphArea.Bottom + 2,
										   numberSize.Width,
										   Font.Height);

					if (axisValuesRect.IntersectsWith(aboveRangeRect) ||
						axisValuesRect.IntersectsWith(belowRangeRect))
					{
						offset += graduationPixelDiff;
						continue;
					}

					graphics.DrawString(val, Font, textBrush, axisValuesRect, sf);

					offset += graduationPixelDiff;
				}
			}
		}

		private void DrawBarNames(Graphics graphics, float barArea, float barSpacing)
		{
			graphics.SetClip(ClientRectangle);

			StringFormat sf = new StringFormat();
			sf.Trimming = StringTrimming.Character;
			sf.FormatFlags = StringFormatFlags.NoWrap;
			sf.LineAlignment = StringAlignment.Center;

			float offset;

			if (barOrientation == Orientation.Vertical)
			{
				Brush textBrush = new SolidBrush(XAxisColor);
				sf.Alignment = StringAlignment.Center;
				offset = GraphArea.Left + barSpacing / 2;

				for (int i = 0;i < BarCount;i ++)
				{
					string name = barCollection[i].Name;
					RectangleF nameRect =
							new RectangleF(offset, GraphArea.Bottom, barArea, Font.Height * 2F);
					graphics.DrawString(name, Font, textBrush, nameRect, sf);
					offset += barArea;
				}
				textBrush.Dispose();
			}
			else
			{
				Brush textBrush = new SolidBrush(YAxisColor);
				sf.Alignment = StringAlignment.Far;
				float barWidth = barArea - barSpacing;
				offset = GraphArea.Bottom - barSpacing - (barWidth / 2) - (FontHeight / 2);

				for (int i = 0;i < BarCount;i ++)
				{
					string name = barCollection[i].Name;
					RectangleF nameRect =
							new RectangleF(ClientRectangle.Left,
										   offset,
										   GraphMarginLeft - FontHeight / 2,
										   Font.Height);
					graphics.DrawString(name, Font, textBrush, nameRect, sf);
					offset -= barArea;
				}
				textBrush.Dispose();
			}
		}

		private void CalculateGraphArea()
		{
			GraphArea =
					new Rectangle(ClientRectangle.Left + graphMarginLeft,
								  ClientRectangle.Top + graphMarginTop,
								  ClientRectangle.Width - graphMarginRight - graphMarginLeft,
								  ClientRectangle.Height - graphMarginBottom - graphMarginTop);
		}

		private void DrawRangeLinesAndValues(Graphics graphics)
		{
			graphics.SetClip(ClientRectangle);

			#region draw range lines

			if (ShowRangeLines)
			{
				if (barOrientation == Orientation.Vertical)
				{
					float aboveRangeHeight = GraphArea.Height *
											 ((AboveRangeValue - minimumValue) /
											  (maximumValue - minimumValue));
					float belowRangeHeight = GraphArea.Height *
											 ((BelowRangeValue - minimumValue) /
											  (maximumValue - minimumValue));

					Pen pen = new Pen(Color.Black);
					pen.DashStyle = DashStyle.Dash;
					graphics.DrawLine(pen,
									  GraphArea.Left,
									  GraphArea.Bottom - aboveRangeHeight,
									  GraphArea.Right,
									  GraphArea.Bottom - aboveRangeHeight);
					graphics.DrawLine(pen,
									  GraphArea.Left,
									  GraphArea.Bottom - belowRangeHeight,
									  GraphArea.Right,
									  GraphArea.Bottom - belowRangeHeight);
				}
				else
				{
					float aboveRangeWidth = GraphArea.Width *
											((AboveRangeValue - minimumValue) /
											 (maximumValue - minimumValue));
					float belowRangeWidth = GraphArea.Width *
											((BelowRangeValue - minimumValue) /
											 (maximumValue - minimumValue));

					Pen pen = new Pen(Color.Black);
					pen.DashStyle = DashStyle.Dash;
					graphics.DrawLine(pen,
									  GraphArea.Left + aboveRangeWidth,
									  GraphArea.Top,
									  GraphArea.Left + aboveRangeWidth,
									  GraphArea.Bottom);
					graphics.DrawLine(pen,
									  GraphArea.Left + belowRangeWidth,
									  GraphArea.Top,
									  GraphArea.Left + belowRangeWidth,
									  GraphArea.Bottom);
				}
			}

			#endregion draw range lines

			#region draw range values

			if (ShowRangeValues)
			{
				StringFormat sf = new StringFormat();
				Brush textBrush = new SolidBrush(ForeColor);

				if (barOrientation == Orientation.Vertical)
				{
					float aboveRangeHeight = GraphArea.Height *
											 ((AboveRangeValue - minimumValue) /
											  (maximumValue - minimumValue));
					float belowRangeHeight = GraphArea.Height *
											 ((BelowRangeValue - minimumValue) /
											  (maximumValue - minimumValue));

					aboveRangeRect =
							new RectangleF(ClientRectangle.Left,
										   GraphArea.Bottom - aboveRangeHeight - Font.Height / 2,
										   GraphMarginLeft - Font.Height / 2,
										   Font.Height);
					belowRangeRect =
							new RectangleF(ClientRectangle.Left,
										   GraphArea.Bottom - belowRangeHeight - Font.Height / 2,
										   GraphMarginLeft - Font.Height / 2,
										   Font.Height);

					sf.Trimming = StringTrimming.Character;
					sf.FormatFlags = StringFormatFlags.NoWrap;
					sf.Alignment = StringAlignment.Far;
					sf.LineAlignment = StringAlignment.Center;

					string above;
					string below;
					if (ValueFormat.Length != 0)
					{
						above =
								string.Format(CultureInfo.CurrentUICulture,
											  ValueFormat,
											  aboveRangeValue);
						below =
								string.Format(CultureInfo.CurrentUICulture,
											  ValueFormat,
											  belowRangeValue);
					}
					else
					{
						above = aboveRangeValue.ToString(CultureInfo.CurrentUICulture);
						below = belowRangeValue.ToString(CultureInfo.CurrentUICulture);
					}
					graphics.DrawString(above, Font, textBrush, aboveRangeRect, sf);
					graphics.DrawString(below, Font, textBrush, belowRangeRect, sf);
				}
				else
				{
					float aboveRangeWidth = GraphArea.Width *
											((AboveRangeValue - minimumValue) /
											 (maximumValue - minimumValue));
					float belowRangeWidth = GraphArea.Width *
											((BelowRangeValue - minimumValue) /
											 (maximumValue - minimumValue));

					string above;
					string below;
					if (ValueFormat.Length != 0)
					{
						above =
								string.Format(CultureInfo.CurrentUICulture,
											  ValueFormat,
											  aboveRangeValue);
						below =
								string.Format(CultureInfo.CurrentUICulture,
											  ValueFormat,
											  belowRangeValue);
					}
					else
					{
						above = aboveRangeValue.ToString(CultureInfo.CurrentUICulture);
						below = belowRangeValue.ToString(CultureInfo.CurrentUICulture);
					}

					SizeF aboveSize = graphics.MeasureString(above, Font);
					SizeF belowSize = graphics.MeasureString(below, Font);

					aboveRangeRect =
							new RectangleF(GraphArea.Left + aboveRangeWidth - aboveSize.Width / 2,
										   GraphArea.Bottom + 2,
										   aboveSize.Width,
										   Font.Height);
					belowRangeRect =
							new RectangleF(GraphArea.Left + belowRangeWidth - belowSize.Width / 2,
										   GraphArea.Bottom + 2,
										   belowSize.Width,
										   Font.Height);

					sf.Alignment = StringAlignment.Far;
					sf.LineAlignment = StringAlignment.Center;

					graphics.DrawString(above, Font, textBrush, aboveRangeRect, sf);
					graphics.DrawString(below, Font, textBrush, belowRangeRect, sf);
				}
			}

			#endregion draw range values
		}

		/// <summary>
		/// Updates the display of the graph. Call this method once you
		/// set all the values so that the changes are reflected on the
		/// graph
		/// </summary>
		public void UpdateDisplay()
		{
			RefreshDisplay();
		}

		#endregion

		#endregion methods
	}
}