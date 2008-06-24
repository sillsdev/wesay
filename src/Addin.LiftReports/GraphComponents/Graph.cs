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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// The base class for graphs. Provides the basic framework of a graph.
	/// </summary>
	public class Graph: UserControl, IGraphElement
	{
		#region variables

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private readonly Container components = null;

		/// <summary>
		/// Area of the rectangle that is bounded by the 2 axes. This is the
		/// area of the graph without any margins
		/// </summary>
		private Rectangle graphArea;

		/// <summary>
		/// The amount of transparency in the bar. Values range from 0 to 255
		/// 0 is fully transparent, 255 is opaque.
		/// </summary>
		private byte transparency = 100;

		/// <summary>
		/// Color of the graphArea
		/// </summary>
		private Color graphAreaColor = Color.White;

		/// <summary>
		/// Color of the gridlines
		/// </summary>
		private Color gridlineColor = Color.LightGray;

		/// <summary>
		/// Style of the gridlines
		/// </summary>
		private GridStyles gridlines = GridStyles.Grid;

		/// <summary>
		/// No. of graduations on the X axis
		/// </summary>
		private int graduationsX = 5;

		/// <summary>
		/// No. of graduations on the Y axis
		/// </summary>
		private int graduationsY = 5;

		/// <summary>
		/// Color of the X axis
		/// </summary>
		private Color xAxisColor = Color.Black;

		/// <summary>
		/// Color of the Y axis
		/// </summary>
		private Color yAxisColor = Color.Blue;

		/// <summary>
		/// The .NET style of format for displaying the value. Eg. {0:F}, {0:E} etc
		/// </summary>
		private string valueFormat = "";

		#endregion variables

		#region properties

		#region ValueFormat

		/// <summary>
		/// The .NET style of format for displaying the value and axis. Eg. {0:F}, {0:E} etc
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("The .NET style of format for displaying the value. Eg. {0:F}, {0:E} etc")]
		public string ValueFormat
		{
			get { return valueFormat; }
			set
			{
				valueFormat = value;
				RefreshDisplay();
			}
		}

		#endregion ValueFormat

		#region XAxisColor

		/// <summary>
		/// Color of the X axis
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Color of the X axis")]
		public virtual Color XAxisColor
		{
			get { return xAxisColor; }
			set
			{
				xAxisColor = value;
				RefreshDisplay();
			}
		}

		#endregion XAxisColor

		#region YAxisColor

		/// <summary>
		/// Color of the Y axis
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Color of the Y axis")]
		public virtual Color YAxisColor
		{
			get { return yAxisColor; }
			set { yAxisColor = value; }
		}

		#endregion YAxisColor

		#region Trancparency

		/// <summary>
		/// The amount of transparency in the bar. Values range from 0 to 255.
		/// 0 is fully transparent, 255 is opaque.
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description(
				"The amount of transparency in the bar. Values range from 0 to 255. 0 is fully transparent, 255 is opaque."
				)]
		public byte Transparency
		{
			get { return transparency; }
			set
			{
				transparency = value;
				RefreshDisplay();
			}
		}

		#endregion Transparency

		#region GridlineColor

		/// <summary>
		/// Color of the gridlines
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Color of the grid lines")]
		public Color GridlineColor
		{
			get { return gridlineColor; }
			set
			{
				gridlineColor = value;
				RefreshDisplay();
			}
		}

		#endregion GridlineColor

		#region GraphAreaColor

		/// <summary>
		/// Color of the graph's area
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Color of the graph's area")]
		public virtual Color GraphAreaColor
		{
			get { return graphAreaColor; }
			set
			{
				graphAreaColor = value;
				RefreshDisplay();
			}
		}

		#endregion GraphAreaColor

		#region Gridlines

		[ /// <summary>
				/// Style of the gridlines
				/// </summary>
				Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Style of the gridlines")]
		public virtual GridStyles Gridlines
		{
			get { return gridlines; }
			set
			{
				gridlines = value;
				RefreshDisplay();
			}
		}

		#endregion Gridlines

		#region GraduationsX

		/// <summary>
		/// No. of graduations on the X axis
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("No. of graduations on the X axis")]
		public int GraduationsX
		{
			get { return graduationsX; }
			set
			{
				graduationsX = value;
				RefreshDisplay();
			}
		}

		#endregion GraduationsX

		#region GraduationsY

		/// <summary>
		/// No. of graduations on the Y axis
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("No. of graduations on the Y axis")]
		public int GraduationsY
		{
			get { return graduationsY; }
			set
			{
				graduationsY = value;
				RefreshDisplay();
			}
		}

		#endregion GraduationsY

		#region GraphArea

		/// <summary>
		/// Area of the rectangle that is bounded by the 2 axes. This is the
		/// area of the graph without any margins
		/// </summary>
		[Browsable(false)]
		public Rectangle GraphArea
		{
			get { return graphArea; }
			set { graphArea = value; }
		}

		#endregion GraphArea

		#endregion properties

		#region methods

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Refreshes the display by updating the control. This is the
		/// refresh method during runtime and in the designer.
		/// </summary>
		protected void RefreshDisplay()
		{
			Invalidate();
			Update();
		}

		/// <summary>
		/// Draws the bar. This method is overridden in the derived class.
		/// </summary>
		/// <param name="graphics"></param>
		public virtual void Draw(Graphics graphics) {}

		#endregion methods
	}
}