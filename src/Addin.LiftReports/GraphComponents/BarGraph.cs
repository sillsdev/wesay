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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// This class provides all the functionality for displaying a
	/// BarGraph as a user control.
	/// </summary>
	public class BarGraph: Graph
	{
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
		/// Bar orientation, whether horizontal or vertical
		/// </summary>
		private Orientation barOrientation = Orientation.Vertical;

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
		/// The color to display if the value is above normal
		/// </summary>
		private Color aboveRangeColor = Color.Salmon;

		/// <summary>
		/// The color to display if the value is below normal
		/// </summary>
		private Color belowRangeColor = Color.Thistle;

		/// <summary>
		/// The alignment of the value's text
		/// </summary>
		private TextAlignment valueAlignment = TextAlignment.Smart;

		/// <summary>
		/// Flag indicating whether lines for above and below normal limits are to be displayed
		/// </summary>
		private bool showRangeLines = true;

		/// <summary>
		/// Flag indicating whether the values of above and below ranges are to be displayed
		/// </summary>
		private bool showRangeValues;

		/// <summary>
		/// The location where the graduations must appear
		/// </summary>
		private Graduation barGraduation = Graduation.Edge2;

		/// <summary>
		/// The basic bar object that does the actual rendering.
		/// </summary>
		private readonly BasicBar basicBar;

		#endregion variables

		#region properties

		#region BarValue

		/// <summary>
		/// Current value of the bar. Gets or sets the value to display on the bar
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Current value of the bar. Gets or sets the value to display on the bar")]
		public float BarValue
		{
			get { return barValue; }
			set
			{
				if (value != barValue)
				{
					barValue = value;
				}

				RefreshDisplay();
			}
		}

		#endregion BarValue

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

		#region BorderColor

		/// <summary>
		/// Color of the border of the filled up portion of the graph
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Color of the border of the filled up portion of the graph")]
		public Color BorderColor
		{
			get { return graphBorderColor; }
			set
			{
				graphBorderColor = value;
				RefreshDisplay();
			}
		}

		#endregion BorderColor

		#region BarOrientation

		/// <summary>
		/// Bar orientation, whether horizontal or vertical
		/// </summary>
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

		#region AboveRangeValue

		/// <summary>
		/// A value beyond which the readings are above normal.
		/// The graph can be displayed in a different color if it
		/// goes above this range
		/// Eg. body temp. above 40 degree C
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description(
				"A value beyond which the readings are above normal. The graph can be displayed in a different color if it goes above this range."
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

		#endregion

		#region BelowRangeValue

		/// <summary>
		/// A value beyond which the readings are below normal.
		/// The graph can be displayed in a different color if it
		/// goes below this range
		/// Eg. body temp. below 35 degree C
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description(
				"A value beyond which the readings are below normal. The graph can be displayed in a different color if it goes below this range"
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

		#region BarGraduation

		/// <summary>
		/// The location where the graduations must appear
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The location where the graduations must appear")]
		public Graduation BarGraduation
		{
			get { return barGraduation; }
			set
			{
				barGraduation = value;
				RefreshDisplay();
			}
		}

		#endregion BarGraduation

		#region XAxisColor

		/// <summary>
		/// Color of the X axis
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Color of the X axis")]
		[Obsolete("This property is not valid for the bar graph.")]
		public override Color XAxisColor
		{
			get { return base.XAxisColor; }
			set { base.XAxisColor = value; }
		}

		#endregion XAxisColor

		#region YAxisColor

		/// <summary>
		/// Color of the Y axis
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Color of the Y axis")]
		[Obsolete("This property is not valid for the bar graph.")]
		public override Color YAxisColor
		{
			get { return base.YAxisColor; }
			set { base.YAxisColor = value; }
		}

		#endregion YAxisColor

		#region GraphAreaColor

		/// <summary>
		/// Color of the graph's area
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Color of the graph's area")]
		[Obsolete("This property is not valid for the bar graph.")]
		public override Color GraphAreaColor
		{
			get { return base.GraphAreaColor; }
			set { base.GraphAreaColor = value; }
		}

		#endregion GraphAreaColor

		#region Gridlines

		/// <summary>
		/// Style of the gridlines
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Style of the gridlines")]
		[Obsolete("This property is not valid for the bar graph.")]
		public override GridStyles Gridlines
		{
			get { return base.Gridlines; }
			set { base.Gridlines = value; }
		}

		#endregion Gridlines

		#endregion properties

		#region methods

		/// <summary>
		/// Creates a bar graph user control with default properties
		/// </summary>
		public BarGraph()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
			basicBar = new BasicBar();

			SetStyle(
					ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
					ControlStyles.AllPaintingInWmPaint,
					true);
			UpdateStyles();
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
			// BarGraph
			//
			this.Name = "BarGraph";
			this.Size = new System.Drawing.Size(112, 360);
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
			graphics.SmoothingMode = SmoothingMode.AntiAlias;

			#region assign bar color

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

			basicBar.BarColor = graphColor;

			#endregion assign bar color

			if (ClientRectangle.Width == 0 || ClientRectangle.Height == 0)
			{
				return;
			}

			if (MinimumValue >= MaximumValue)
			{
				return;
			}

			basicBar.ClientRectangle = ClientRectangle;
			basicBar.MinimumValue = minimumValue;
			basicBar.MaximumValue = maximumValue;
			basicBar.BarValue = BarValue;
			basicBar.AboveRangeValue = AboveRangeValue;
			basicBar.BelowRangeValue = BelowRangeValue;
			basicBar.BarOrientation = barOrientation;
			basicBar.ForeColor = ForeColor;
			basicBar.ValueFormat = ValueFormat;
			basicBar.ValueAlignment = valueAlignment;
			basicBar.TextFont = Font;
			basicBar.ShowRangeLines = showRangeLines;
			basicBar.ShowRangeValues = showRangeValues;
			basicBar.BorderColor = graphBorderColor;
			basicBar.BarGraduation = barGraduation;
			basicBar.OutOfRangeArrowColor = BarColor;
			// display the arrow in bar color if out of range

			basicBar.Draw(graphics);
		}

		#endregion

		#endregion methods
	}
}