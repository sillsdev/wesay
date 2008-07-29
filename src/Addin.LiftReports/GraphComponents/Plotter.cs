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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// Summary description for Plotter.
	/// </summary>
	public class Plotter: Graph
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
		/// The lower margin width of the graph. In stopped/pause mode of the plotter,
		/// a scroll bar appears if the graphs dont fit. So, take the scroll bar height
		/// also into account
		/// </summary>
		private int graphMarginBottom = 20;

		/// <summary>
		/// The collection that contains all the individual channels
		/// </summary>
		private readonly ChannelCollection channelCollection;

		/// <summary>
		/// Used to draw the gridline
		/// </summary>
		private readonly Gridline gridline;

		/// <summary>
		/// Used to draw the x and y axis line
		/// </summary>
		private readonly AxisLine axisLineXandY;

		/// <summary>
		/// The max value to display on the X axis
		/// </summary>
		private TimeSpan xRange = new TimeSpan(0, 0, 9);

		/// <summary>
		/// Time in milisecond when the plotter must plot the next point
		/// ie. The X distance between 2 points
		/// </summary>
		private int plotRate = 300;

		/// <summary>
		/// The total time elapsed in milisecond since plotting began.
		/// </summary>
		private int totalTimeElapsed;

		/// <summary>
		/// The display limit on the left extreme of the graph
		/// </summary>
		private int leftDisplayLimit;

		/// <summary>
		/// The number of points to remove from the left side
		/// as we go on scrolling. If the scroll bar is at the
		/// complete left, the number of points to remove is 0
		/// If it is at the complete right, then pointsToRemove
		/// is totalPointsToRemove
		/// </summary>
		private int pointsToRemove;

		private HScrollBar plotterHScrollBar;

		/// <summary>
		/// The number of points to remove from the left as the
		/// plotter goes on scrolling (this value goes on accumalating)
		/// </summary>
		private int totalPointsToRemove;

		/// <summary>
		/// The index of the currently active channel
		/// </summary>
		private int activeChannelIndex;

		/// <summary>
		/// Width of the small buttons used for changing the channels
		/// </summary>
		private readonly int buttonWidth;

		/// <summary>
		/// Height of the small buttons used for changing the channels
		/// </summary>
		private readonly int buttonHeight;

		/// <summary>
		/// Coordinates of the mouse when it is hovering over the plotter.
		/// This is used for displaying the coordinates of the point that
		/// the mouse is hovering over. eg (234.54, 27)
		/// </summary>
		private Point mouseHoverCoordinates;

		/// <summary>
		/// Initial plot rate. This is required as the user can change the
		/// plot rate during the plotting.. and we need some way to track
		/// the old plotting rate.
		/// </summary>
		private int initialPlotRate;

		/// <summary>
		/// The time value on the right edge of the plotter
		/// </summary>
		private int rightDisplayLimit;

		/// <summary>
		/// The left display limit when the plotter was stopped
		/// </summary>
		private int stoppedLeftDisplayLimit;

		#region compressed mode vars

		/// <summary>
		/// A flag indicating whether the plotter is showing the graphs
		/// in compressed format
		/// </summary>
		private bool compressedMode;

		/// <summary>
		/// The previously stored x range. This is used while toggling the compressedMode.
		/// </summary>
		private TimeSpan savedXRange;

		/// <summary>
		/// The previously stored leftDisplayLimit. This is used while toggling the compressedMode.
		/// </summary>
		private int savedLeftDisplayLimit;

		/// <summary>
		/// The previously stored pointsToRemove. This is used while toggling the compressedMode.
		/// </summary>
		private int savedPointsToRemove;

		#endregion compressed mode vars

		/// <summary>
		/// The possible states that the plotter can take
		/// </summary>
		public enum PlotterState
		{
			/// <summary>
			/// The plotter is reset. All the graphs are erased. It is waiting to run
			/// </summary>
			Reset,
			/// <summary>
			/// The plotter is running. It could be stopped or paused or reset.
			/// </summary>
			Running,
			/// <summary>
			/// The plotter has stopped plotting. The graphs can now be viewed.
			/// </summary>
			Stopped
		}

		/// <summary>
		/// The current state of the plotter
		/// </summary>
		private PlotterState currentState;

		private Button buttonPrevChannel;
		private Button buttonNextChannel;
		private Button buttonUpperYPlus;
		private Button buttonUpperYMinus;

		/// <summary>
		/// Style in which the values on the time axis (X axis) is to be shown
		/// </summary>
		private TimeAxisStyle timeDisplayStyle = TimeAxisStyle.Smart;

		#endregion variables

		#region properties

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
		/// The lower margin width of the graph. In stopped/pause mode of the plotter,
		/// a scroll bar appears if the graphs dont fit. So, take the scroll bar height
		/// also into account
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

		#region Channels

		[Browsable(false)]
		public ChannelCollection Channels
		{
			get { return channelCollection; }
		}

		#endregion Channels

		#region XRange

		public TimeSpan XRange
		{
			get { return xRange; }
			set { xRange = value; }
		}

		#endregion XRange

		#region PlotRate

		public int PlotRate
		{
			get { return plotRate; }
			set
			{
				plotRate = value;
				if (plotRate > initialPlotRate)
				{
					initialPlotRate = plotRate;
				}

				foreach (Channel channel in Channels)
				{
					channel.PlotRate = plotRate;
				}
			}
		}

		#endregion PlotRate

		#region CompressedMode

		/// <summary>
		/// Set this to true if the plotter is to display the graphs in a compressed
		/// format so that all the data fits in the graph area. This mode can be set
		/// only when the plotter is either paused or stopped.
		/// </summary>
		[Browsable(false)]
		public bool CompressedMode
		{
			get { return compressedMode; }
			set
			{
				if (currentState != PlotterState.Stopped)
				{
					compressedMode = false;
					xRange = savedXRange;
					return;
				}

				if (compressedMode == value)
				{
					return;
				}

				compressedMode = value;

				if (compressedMode)
				{
					// save the current settings
					savedXRange = xRange;
					savedLeftDisplayLimit = leftDisplayLimit;
					savedPointsToRemove = pointsToRemove;

					// set the new settings
					xRange = new TimeSpan(totalTimeElapsed * TimeSpan.TicksPerMillisecond);
					leftDisplayLimit = 0;
					pointsToRemove = 0;
				}
				else
				{
					// restore the settings back again
					xRange = savedXRange;
					leftDisplayLimit = savedLeftDisplayLimit;
					pointsToRemove = savedPointsToRemove;
				}

				plotterHScrollBar.Visible = !compressedMode;
				RefreshDisplay();
			}
		}

		#endregion CompressedMode

		#region CurrentState

		/// <summary>
		/// Indicates the current state of the plotter.
		/// </summary>
		[Browsable(false)]
		public PlotterState CurrentState
		{
			get { return currentState; }
		}

		#endregion CurrentState

		#region ActiveChannel

		/// <summary>
		/// Gets the index of the currently active channel. To change to the next channel,
		/// use NextChannel () and PrevChannel ()
		/// </summary>
		[Browsable(false)]
		public int ActiveChannelIndex
		{
			get { return activeChannelIndex; }
		}

		/// <summary>
		/// Gets the index of the currently active channel. To change to the next channel,
		/// use NextChannel () and PrevChannel ()
		/// </summary>
		[Browsable(false)]
		public Channel ActiveChannel
		{
			get { return channelCollection[activeChannelIndex]; }
		}

		#endregion ActiveChannel

		#region TimeDisplayStyle

		/// <summary>
		/// Style in which the values on the time axis (X axis) is to be shown
		/// </summary>
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Appearance")]
		[Description("Style in which the values on the time axis (X axis) is to be shown")]
		public TimeAxisStyle TimeDisplayStyle
		{
			get { return timeDisplayStyle; }
			set { timeDisplayStyle = value; }
		}

		#endregion TimeDisplayStyle

		#region TotalTimeElapsed

		[Browsable(false)]
		public int TotalTimeElapsed
		{
			get { return totalTimeElapsed; }
			set { totalTimeElapsed = value; }
		}

		#endregion TotalTimeElapsed

		#endregion properties

		#region events

		/// <summary>
		/// Represents the method that handles the event when the plotter's parameters
		/// change.
		/// </summary>
		public delegate void PlotterEventHandler(object sender, PlotterEventArgs e);

		/// <summary>
		/// Occurs whenever a parameter of the plotter has changed and the clients
		/// require a notification.
		/// </summary>
		/// <remarks>Customize/Use this to raise an event wherever required.</remarks>
		public event PlotterEventHandler PlotterStateChanged;

		#endregion events

		#region methods

		/// <summary>
		/// Creates a plotter graph component. The default has 4 channels with 2 enabled
		/// and 2 disabled. The default ranges and channel names are also provided for
		/// all the 4 channels
		/// </summary>
		public Plotter()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
			channelCollection = new ChannelCollection();
			Channels.Add(new Channel(0, 100, "Voltage", true, Color.Blue));
			Channels.Add(new Channel(0, 5, "Current", true, Color.Red));
			Channels.Add(new Channel(0, 100, "2", false, Color.Green));
			Channels.Add(new Channel(0, 100, "3", false, Color.Purple));

			gridline = new Gridline(this);
			axisLineXandY = new AxisLine(this);

			SetStyle(
					ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
					ControlStyles.AllPaintingInWmPaint,
					true);
			UpdateStyles();

			PlotRate = 300;
			initialPlotRate = PlotRate;

			GraphArea = new Rectangle(ClientRectangle.Left + graphMarginLeft,
									  ClientRectangle.Top + graphMarginTop,
									  ClientRectangle.Width - graphMarginRight - graphMarginLeft,
									  ClientRectangle.Height - graphMarginBottom - graphMarginTop);

			Debug.Assert(GraphArea.Height == (GraphArea.Bottom - GraphArea.Top), "Problem Ctor");

			int xRangeInMs = (int) (xRange.Duration().Ticks / TimeSpan.TicksPerMillisecond);
			plotterHScrollBar.Maximum = xRangeInMs / PlotRate;
			plotterHScrollBar.Value = xRangeInMs / PlotRate;
			plotterHScrollBar.Visible = false;

			rightDisplayLimit = xRangeInMs - leftDisplayLimit;
			savedXRange = xRange;

			ValueFormat = "{0:F}";

			buttonWidth = buttonPrevChannel.Width;
			buttonHeight = buttonPrevChannel.Height;
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
			this.plotterHScrollBar = new System.Windows.Forms.HScrollBar();
			this.buttonPrevChannel = new System.Windows.Forms.Button();
			this.buttonNextChannel = new System.Windows.Forms.Button();
			this.buttonUpperYPlus = new System.Windows.Forms.Button();
			this.buttonUpperYMinus = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// plotterHScrollBar
			//
			this.plotterHScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.plotterHScrollBar.Location = new System.Drawing.Point(0, 239);
			this.plotterHScrollBar.Name = "plotterHScrollBar";
			this.plotterHScrollBar.Size = new System.Drawing.Size(344, 17);
			this.plotterHScrollBar.TabIndex = 1;
			this.plotterHScrollBar.Scroll +=
					new System.Windows.Forms.ScrollEventHandler(this.plotterHScrollBar_Scroll);
			//
			// buttonPrevChannel
			//
			this.buttonPrevChannel.BackColor = System.Drawing.SystemColors.Control;
			this.buttonPrevChannel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonPrevChannel.Font = new System.Drawing.Font("Microsoft Sans Serif",
																  8.25F,
																  System.Drawing.FontStyle.Bold,
																  System.Drawing.GraphicsUnit.Point,
																  ((System.Byte) (0)));
			this.buttonPrevChannel.Location = new System.Drawing.Point(24, 40);
			this.buttonPrevChannel.Name = "buttonPrevChannel";
			this.buttonPrevChannel.Size = new System.Drawing.Size(16, 18);
			this.buttonPrevChannel.TabIndex = 2;
			this.buttonPrevChannel.Text = "<";
			this.buttonPrevChannel.Click += new System.EventHandler(this.buttonPrevChannel_Click);
			//
			// buttonNextChannel
			//
			this.buttonNextChannel.BackColor = System.Drawing.SystemColors.Control;
			this.buttonNextChannel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonNextChannel.Font = new System.Drawing.Font("Microsoft Sans Serif",
																  8.25F,
																  System.Drawing.FontStyle.Bold,
																  System.Drawing.GraphicsUnit.Point,
																  ((System.Byte) (0)));
			this.buttonNextChannel.Location = new System.Drawing.Point(40, 40);
			this.buttonNextChannel.Name = "buttonNextChannel";
			this.buttonNextChannel.Size = new System.Drawing.Size(16, 18);
			this.buttonNextChannel.TabIndex = 3;
			this.buttonNextChannel.Text = ">";
			this.buttonNextChannel.Click += new System.EventHandler(this.buttonNextChannel_Click);
			//
			// buttonUpperYPlus
			//
			this.buttonUpperYPlus.BackColor = System.Drawing.SystemColors.Control;
			this.buttonUpperYPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonUpperYPlus.Font = new System.Drawing.Font("Microsoft Sans Serif",
																 8.25F,
																 System.Drawing.FontStyle.Bold,
																 System.Drawing.GraphicsUnit.Point,
																 ((System.Byte) (0)));
			this.buttonUpperYPlus.Location = new System.Drawing.Point(24, 64);
			this.buttonUpperYPlus.Name = "buttonUpperYPlus";
			this.buttonUpperYPlus.Size = new System.Drawing.Size(16, 18);
			this.buttonUpperYPlus.TabIndex = 4;
			this.buttonUpperYPlus.Text = "+";
			this.buttonUpperYPlus.Click += new System.EventHandler(this.buttonUpperYPlus_Click);
			//
			// buttonUpperYMinus
			//
			this.buttonUpperYMinus.BackColor = System.Drawing.SystemColors.Control;
			this.buttonUpperYMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonUpperYMinus.Font = new System.Drawing.Font("Microsoft Sans Serif",
																  8.25F,
																  System.Drawing.FontStyle.Bold,
																  System.Drawing.GraphicsUnit.Point,
																  ((System.Byte) (0)));
			this.buttonUpperYMinus.Location = new System.Drawing.Point(40, 64);
			this.buttonUpperYMinus.Name = "buttonUpperYMinus";
			this.buttonUpperYMinus.Size = new System.Drawing.Size(16, 18);
			this.buttonUpperYMinus.TabIndex = 5;
			this.buttonUpperYMinus.Text = "-";
			this.buttonUpperYMinus.Click += new System.EventHandler(this.buttonUpperYMinus_Click);
			//
			// Plotter
			//
			this.Controls.Add(this.buttonUpperYMinus);
			this.Controls.Add(this.buttonUpperYPlus);
			this.Controls.Add(this.buttonNextChannel);
			this.Controls.Add(this.buttonPrevChannel);
			this.Controls.Add(this.plotterHScrollBar);
			this.Name = "Plotter";
			this.Size = new System.Drawing.Size(344, 256);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Plotter_MouseMove);
			this.ResumeLayout(false);
		}

		#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
			if (! Visible)
			{
				return;
			}

			Graphics graphics = e.Graphics;

			try
			{
				Draw(graphics);
			}
			catch (NullReferenceException ex)
			{
				Trace.WriteLine(ex);
			}

			base.OnPaint(e);
		}

		/// <summary>
		/// The main drawing routine that renders the points onto the graph
		/// </summary>
		/// <param name="graphics"></param>
		public override void Draw(Graphics graphics)
		{
			graphics.SmoothingMode = SmoothingMode.AntiAlias;

			CalculateGraphArea();

			if (GraphArea.Width == 0 || GraphArea.Height == 0)
			{
				return;
			}

			graphics.SetClip(GraphArea);

			gridline.Draw(graphics);

			base.YAxisColor = ActiveChannel.ChannelColor;
			axisLineXandY.Draw(graphics);

			DrawYAxisValues(graphics);
			DrawXAxisValues(graphics);

			DrawChannelChangingButtons();
			DrawLimitChangingButtons();

			graphics.SetClip(GraphArea);

			if (currentState == PlotterState.Reset)
			{
				return;
			}

			DrawCrossHair(graphics);

			DrawXYText(graphics);

			ActiveChannel.ChannelCursor.GraphArea = GraphArea;
			ActiveChannel.ChannelCursor.Draw(graphics);

			#region check for scrolling

			int xRangeInMs = (int) (xRange.Duration().Ticks / TimeSpan.TicksPerMillisecond);
			rightDisplayLimit = leftDisplayLimit + xRangeInMs;

			if (TotalTimeElapsed > rightDisplayLimit)
			{
				if (currentState == PlotterState.Running)
				{
					plotterHScrollBar.Maximum = pointsToRemove;
					plotterHScrollBar.Value = pointsToRemove;
					pointsToRemove ++;

					PointF leftPosition = (PointF) ActiveChannel.Points[pointsToRemove];

					#region adjust scrolling rate

					// if the user has changed the plotting rate.. then we need to adjust the
					// scrolling at the left, as it has to scroll at the old plotting rate.
					// or faster than that. if the scrolling is too slow, the new points wont
					// appear on the right edge, rather they are outside the right edge.
					// so adjust the scrolling.
					int leftDisplayLimitDelta = (int) (leftPosition.X - leftDisplayLimit);

					if (leftDisplayLimitDelta > plotRate)
					{
						leftDisplayLimit = (int) leftPosition.X;
					}
					else
					{
						leftDisplayLimit += plotRate;
					}

					#endregion adjust scrolling rate
				}
			}

			#endregion check for scrolling

			#region draw points for each channel

			bool scrollAdjusted = false;

			for (int i = 0;i < Channels.Count;i ++)
			{
				Channel channel = Channels[i];

				if (! channel.Enabled)
				{
					continue;
				}

				if (! scrollAdjusted)
				{
					if (TotalTimeElapsed > rightDisplayLimit)
					{
						if (currentState == PlotterState.Running)
						{
							plotterHScrollBar.Maximum = pointsToRemove;
							plotterHScrollBar.Value = pointsToRemove;
							pointsToRemove ++;

							PointF leftPosition = (PointF) channel.Points[pointsToRemove];

							#region adjust scrolling rate

							// if the user has changed the plotting rate.. then we need to adjust the
							// scrolling at the left, as it has to scroll at the old plotting rate.
							// or faster than that. if the scrolling is too slow, the new points wont
							// appear on the right edge, rather they are outside the right edge.
							// so adjust the scrolling.
							int leftDisplayLimitDelta = (int) (leftPosition.X - leftDisplayLimit);

							if (leftDisplayLimitDelta > plotRate)
							{
								leftDisplayLimit = (int) leftPosition.X;
							}
							else
							{
								leftDisplayLimit += plotRate;
							}

							scrollAdjusted = true;

							#endregion adjust scrolling rate
						}
					}
				}

				// DrawCursor (graphics, channel);

				ArrayList pointslist = new ArrayList();

				Trace.WriteLine(channel.Points.Count);

				TotalTimeElapsed = channel.TotalTimeElapsed;

				for (int j = 0;j < channel.Points.Count - pointsToRemove;j ++)
				{
					PointF pointStored = (PointF) channel.Points[j + pointsToRemove];
					Point p = GetPixelFromValue(channel, (int) pointStored.X, pointStored.Y);
					pointslist.Add(p);
				}

				if (pointslist.Count < 2)
				{
					continue;
				}

				Point[] pointsToPlot = new Point[pointslist.Count];
				for (int k = 0;k < pointslist.Count;k ++)
				{
					pointsToPlot[k] = (Point) pointslist[k];
				}

				graphics.SetClip(GraphArea);
				if (i == activeChannelIndex)
				{
					graphics.DrawLines(new Pen(channel.ChannelColor, 1.75F), pointsToPlot);
				}
				else
				{
					graphics.DrawLines(new Pen(channel.ChannelColor, 1.5F), pointsToPlot);
				}
			} // for each channel

			#endregion draw points for each channel
		}

		/// <summary>
		/// Calculates the graph area
		/// </summary>
		private void CalculateGraphArea()
		{
			GraphArea = new Rectangle(ClientRectangle.Left + graphMarginLeft,
									  ClientRectangle.Top + graphMarginTop,
									  ClientRectangle.Width - graphMarginRight - graphMarginLeft,
									  ClientRectangle.Height - graphMarginBottom - graphMarginTop);
		}

		/// <summary>
		/// Draw the next and previous (< >) buttons on the graph
		/// </summary>
		private void DrawChannelChangingButtons()
		{
			buttonPrevChannel.Left = GraphArea.Left + buttonWidth / 2;
			buttonPrevChannel.Top = GraphArea.Top + buttonHeight / 2;
			buttonNextChannel.Left = buttonPrevChannel.Right + 1;
			buttonNextChannel.Top = GraphArea.Top + buttonHeight / 2;

			buttonPrevChannel.ForeColor = Channels[activeChannelIndex].ChannelColor;
			buttonNextChannel.ForeColor = Channels[activeChannelIndex].ChannelColor;

			buttonPrevChannel.BackColor = GraphAreaColor;
			buttonNextChannel.BackColor = GraphAreaColor;
		}

		private void DrawLimitChangingButtons()
		{
			Color chColor = Channels[activeChannelIndex].ChannelColor;

			buttonUpperYPlus.Left = GraphArea.Left + buttonWidth / 2;
			buttonUpperYPlus.Top = buttonPrevChannel.Bottom + 2;
			// 2 pixels after the prev channel button
			buttonUpperYMinus.Left = buttonUpperYPlus.Right + 1;
			buttonUpperYMinus.Top = buttonPrevChannel.Bottom + 2;

			buttonUpperYPlus.ForeColor = chColor;
			buttonUpperYMinus.ForeColor = chColor;
			buttonUpperYPlus.BackColor = GraphAreaColor;
			buttonUpperYMinus.BackColor = GraphAreaColor;
		}

		/// <summary>
		/// Converts a value of the channel to the pixels corresponding to the graphArea
		/// so that the value could be easily plotted.
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="XinValue"></param>
		/// <param name="YinValue"></param>
		/// <returns></returns>
		private Point GetPixelFromValue(Channel channel, int XinValue, float YinValue)
		{
			float yRange = channel.MaximumValue - channel.MinimumValue;

			float y = (YinValue - channel.MinimumValue) / yRange;

			int yInPixel = GraphArea.Bottom - (int) (y * GraphArea.Height);

			int xOffsetValue = XinValue;
			xOffsetValue -= leftDisplayLimit;
			int xRangeInMs = (int) (xRange.Duration().Ticks / TimeSpan.TicksPerMillisecond);
			float xOffsetAbs = xOffsetValue / (float) xRangeInMs;
			int xInPixel = GraphArea.Left + (int) (xOffsetAbs * GraphArea.Width);

			return new Point(xInPixel, yInPixel);
		}

		/// <summary>
		/// Gets the value corresponding to a particular channel from a pixel. Used when
		/// the mouse coordinates need to be tapped to values on the graph
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="XInPixel"></param>
		/// <param name="YInPixel"></param>
		/// <returns></returns>
		private PointF GetValueFromPixel(Channel channel, int XInPixel, int YInPixel)
		{
			float yAbsolute = (GraphArea.Bottom - YInPixel) / (float) GraphArea.Height;
			float yRange = channel.MaximumValue - channel.MinimumValue;

			float yInValue = channel.MinimumValue + (yAbsolute * yRange);
			int xRangeInMs = (int) (xRange.Duration().Ticks / TimeSpan.TicksPerMillisecond);
			float xOffsetPixel = (XInPixel - GraphArea.Left) / (float) GraphArea.Width;
			float xInValue = xOffsetPixel * xRangeInMs;

			return new PointF(xInValue, yInValue);
		}

		/// <summary>
		/// When the mouse moves, the coordinates are captured. This is required for
		/// displaying the coordinate text box in stop mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Plotter_MouseMove(object sender, MouseEventArgs e)
		{
			if (currentState != PlotterState.Stopped)
			{
				return;
			}

			mouseHoverCoordinates.X = e.X;
			mouseHoverCoordinates.Y = e.Y;

			RefreshDisplay();
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

		/// <summary>
		/// Starts the plotting
		/// </summary>
		public void Start()
		{
			if (currentState == PlotterState.Running)
			{
				return;
			}

			// come here only when we had either paused or reset the plotter

			// in case compression mode is set, remove it
			// this if check is important. simply CompressedMode = false; will not work
			// debug and check for yourself
			if (CompressedMode)
			{
				CompressedMode = false;
			}

			currentState = PlotterState.Running;

			plotterHScrollBar.Visible = false;
			pointsToRemove = totalPointsToRemove;
			leftDisplayLimit = stoppedLeftDisplayLimit;

			// string message  = string.Format ("Left display limit = {0}, points to remove = {1}", leftDisplayLimit, pointsToRemove);
			// debugLabel.Text = message;
		}

		/// <summary>
		/// Stops the plotting. The user can now view the graphs
		/// </summary>
		public void Stop()
		{
			currentState = PlotterState.Stopped;

			if (pointsToRemove == 0)
			{
				return;
			}

			stoppedLeftDisplayLimit = leftDisplayLimit;
			plotterHScrollBar.Visible = true;
			plotterHScrollBar.Maximum = pointsToRemove + plotterHScrollBar.LargeChange;
			// TODO: Figure out how this works
			plotterHScrollBar.Value = 0;
			totalPointsToRemove = pointsToRemove - 1;

			// string message  = string.Format ("Left display limit = {0}, points to remove = {1}", leftDisplayLimit, pointsToRemove);
			// debugLabel.Text = message;

			RefreshDisplay();
		}

		/// <summary>
		/// Resets the plotter. Erases the graphs and gets ready to start the
		/// whole plotting process again. To start once again, call Start ()
		/// after calling Reset ().
		/// </summary>
		public void Reset()
		{
			if (currentState == PlotterState.Running)
			{
				Stop();
			}

			CompressedMode = false;

			currentState = PlotterState.Reset;

			leftDisplayLimit = 0;
			savedLeftDisplayLimit = 0;
			totalPointsToRemove = 0;
			pointsToRemove = 0;
			stoppedLeftDisplayLimit = 0;

			int xRangeInMs = (int) (xRange.Duration().Ticks / TimeSpan.TicksPerMillisecond);

			plotterHScrollBar.Visible = false;
			plotterHScrollBar.Maximum = xRangeInMs / initialPlotRate;
			plotterHScrollBar.Value = plotterHScrollBar.Maximum;
			TotalTimeElapsed = 0;

			foreach (Channel channel in Channels)
			{
				channel.CursorOffset = 0;
				channel.Points.Clear();
				channel.TotalTimeElapsed = 0;
			}

			RefreshDisplay();
		}

		/// <summary>
		/// Set the scroll position and the no. of points to remove from the LHS of
		/// the display.
		/// TODO: Get this function perfect
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void plotterHScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			pointsToRemove = e.NewValue;
			leftDisplayLimit = pointsToRemove * initialPlotRate;

			// string message  = string.Format ("Left display limit = {0}, points to remove = {1}", leftDisplayLimit, pointsToRemove);
			// debugLabel.Text = message;

			RefreshDisplay();
		}

		/// <summary>
		/// Draws the cross hair vertical line in Stop mode
		/// </summary>
		/// <param name="graphics"></param>
		private void DrawCrossHair(Graphics graphics)
		{
			if (currentState != PlotterState.Stopped)
			{
				return;
			}

			if (mouseHoverCoordinates.X < GraphArea.Left ||
				mouseHoverCoordinates.X > GraphArea.Right)
			{
				return;
			}

			if (mouseHoverCoordinates.Y < GraphArea.Top ||
				mouseHoverCoordinates.Y > GraphArea.Bottom)
			{
				return;
			}

			Pen crossHairPen = new Pen(GridlineColor);

			// horizontal
			// graphics.DrawLine (crossHairPen, GraphArea.Left, mouseCoordinates.Y, GraphArea.Right, mouseCoordinates.Y);

			// vertical
			graphics.DrawLine(crossHairPen,
							  mouseHoverCoordinates.X,
							  GraphArea.Top,
							  mouseHoverCoordinates.X,
							  GraphArea.Bottom);

			crossHairPen.Dispose();
		}

		/// <summary>
		/// Draws the X and Y coordinates for the active channel when the plotter has stopped
		/// </summary>
		/// <param name="graphics"></param>
		private void DrawXYText(Graphics graphics)
		{
			if (currentState != PlotterState.Stopped)
			{
				return;
			}

			if (mouseHoverCoordinates.X < GraphArea.Left ||
				mouseHoverCoordinates.X > GraphArea.Right)
			{
				return;
			}

			if (mouseHoverCoordinates.Y < GraphArea.Top ||
				mouseHoverCoordinates.Y > GraphArea.Bottom)
			{
				return;
			}

			Channel activeCh = Channels[activeChannelIndex];

			// Get the value in plotter coordinates
			PointF hoveringValue = GetValueFromPixel(activeCh,
													 mouseHoverCoordinates.X,
													 mouseHoverCoordinates.Y);

			int actualX = leftDisplayLimit + (int) hoveringValue.X;

			// actualX could be like 5436 since the mouse coordinates are exact,
			// but the plotter coordinates are limited to the plotRate.
			// ie. the Least Count (LC) of the mouse is 1 pixel, whereas for the
			// plotter it is plotRate
			// so we got to make that to 5400
			int modulo = actualX % plotRate;
			actualX -= modulo;

			int pointsOffset = actualX / plotRate;

			float y = float.NaN;
			string coordinate;

			if (activeCh.Points.ContainsKey(pointsOffset))
			{
				PointF selectedPoint = (PointF) activeCh.Points[pointsOffset];
				y = selectedPoint.Y;
			}

			if (float.IsNaN(y))
			{
				coordinate = GetFormatForTime(actualX) + @", -";
			}
			else
			{
				string val;
				if (ValueFormat.Length != 0)
				{
					val = string.Format(CultureInfo.InstalledUICulture, ValueFormat, y);
				}
				else
				{
					val = string.Format(CultureInfo.InstalledUICulture, "{0:F}", y);
				}

				coordinate = GetFormatForTime(actualX) + @", " + val;
			}

			StringFormat sf = new StringFormat();
			sf.Trimming = StringTrimming.Character;
			sf.FormatFlags = StringFormatFlags.NoWrap;
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;

			// measure the width of the rectangle. pad the coordinate with extra spaces
			// so that the bounding rectangle looks nice
			SizeF size = graphics.MeasureString(" " + coordinate + " ", Font, GraphArea.Width, sf);

			// the transparency is 50
			Color rectColor = Color.FromArgb(50, activeCh.ChannelColor);
			Brush rectBrush = new SolidBrush(rectColor);

			Rectangle rect = GetCoordinateTextRect(size);

			Pen borderPen = new Pen(GridlineColor);
			Brush textBrush = new SolidBrush(activeCh.ChannelColor);

			graphics.DrawRectangle(borderPen, rect);
			graphics.FillRectangle(rectBrush, rect);
			graphics.DrawString(coordinate, Font, textBrush, rect, sf);

			textBrush.Dispose();
			borderPen.Dispose();
		}

		/// <summary>
		/// Draws the y axis value for the currently selected channel
		/// </summary>
		/// <param name="graphics"></param>
		private void DrawYAxisValues(Graphics graphics)
		{
			graphics.SetClip(ClientRectangle);

			StringFormat sf = new StringFormat();
			sf.Trimming = StringTrimming.Character;
			sf.FormatFlags = StringFormatFlags.NoWrap;
			sf.Alignment = StringAlignment.Far;
			sf.LineAlignment = StringAlignment.Center;

			Brush textBrush = new SolidBrush(ActiveChannel.ChannelColor);

			float offset = GraphArea.Top - Font.Height / 2;
			float graduationPixelDiff = GraphArea.Height / GraduationsY;
			float valueOffset = 0;

			float graduationDiff = (ActiveChannel.MaximumValue - ActiveChannel.MinimumValue) /
								   GraduationsY;

			for (int i = GraduationsY;i >= 0;i --)
			{
				RectangleF axisValuesRect = new RectangleF(ClientRectangle.Left,
														   offset,
														   GraphMarginLeft - Font.Height / 2,
														   Font.Height);
				float graduationValue = ActiveChannel.MaximumValue - graduationDiff * valueOffset;
				valueOffset ++;

				string val;
				if (ValueFormat.Length != 0)
				{
					val = string.Format(CultureInfo.CurrentUICulture, ValueFormat, graduationValue);
				}
				else
				{
					val = string.Format(CultureInfo.CurrentUICulture, "{0:F}", graduationValue);
				}

				graphics.DrawString(val, Font, textBrush, axisValuesRect, sf);
				offset += graduationPixelDiff;
			}
		}

		/// <summary>
		/// Draws the x axis value for the currently selected channel
		/// </summary>
		/// <param name="graphics"></param>
		private void DrawXAxisValues(Graphics graphics)
		{
			graphics.SetClip(ClientRectangle);

			StringFormat sf = new StringFormat();
			sf.Trimming = StringTrimming.Character;
			sf.FormatFlags = StringFormatFlags.NoWrap;
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;

			Brush textBrush = new SolidBrush(XAxisColor);

			float offset = GraphArea.Left;
			float graduationPixelDiff = GraphArea.Width / GraduationsX;

			int xRangeInMs = (int) (xRange.Duration().Ticks / TimeSpan.TicksPerMillisecond);

			float graduationDiff = xRangeInMs / GraduationsX;

			for (int i = 0;i <= GraduationsX;i ++)
			{
				float graduationValue = leftDisplayLimit + graduationDiff * i;

				string val = GetFormatForTime((int) graduationValue);

				// string val = string.Format (CultureInfo.CurrentUICulture, "{0:G}", graduationValue);

				SizeF numberSize = graphics.MeasureString(val, Font, (int) graduationPixelDiff);

				RectangleF axisValuesRect = new RectangleF(offset - numberSize.Width / 2,
														   GraphArea.Bottom + 4,
														   numberSize.Width,
														   Font.Height);

				graphics.DrawString(val, Font, textBrush, axisValuesRect, sf);

				offset += graduationPixelDiff;
			}
		}

		/// <summary>
		/// Changes to the previous channel. Just like a remote control channel changer
		/// for a TV. Some specific settings like Y axis limits are changed to show
		/// the previous channels data
		/// </summary>
		public void PreviousChannel()
		{
			activeChannelIndex --;

			if (activeChannelIndex < 0)
			{
				activeChannelIndex = Channels.Count - 1;
			}

			// refresh only if we are not running. when we are running,
			// refresh automatically happens
			if (currentState != PlotterState.Running)
			{
				RefreshDisplay();
			}

			if (PlotterStateChanged != null)
			{
				PlotterStateChanged(this, new PlotterEventArgs(this));
			}
		}

		/// <summary>
		/// Changes to the next channel. Just like a remote control channel changer
		/// for a TV. Some specific settings like Y axis limits are changed to show
		/// the next channels data
		/// </summary>
		public void NextChannel()
		{
			activeChannelIndex ++;

			if (activeChannelIndex == Channels.Count)
			{
				activeChannelIndex = 0;
			}

			// refresh only if we are not running. when we are running,
			// refresh automatically happens
			if (currentState != PlotterState.Running)
			{
				RefreshDisplay();
			}

			if (PlotterStateChanged != null)
			{
				PlotterStateChanged(this, new PlotterEventArgs(this));
			}
		}

		/// <summary>
		/// Gets a string representation of the time to be displayed on the X axis
		/// </summary>
		/// <param name="timeInMilisecond"></param>
		/// <returns></returns>
		private string GetFormatForTime(int timeInMilisecond)
		{
			string timeValue = "";

			switch (timeDisplayStyle)
			{
				case TimeAxisStyle.Millisecond:
					timeValue = string.Format(CultureInfo.CurrentUICulture,
											  "{0:G}",
											  timeInMilisecond);
					break;
				case TimeAxisStyle.MillisecondWithUnitDisplay:
					timeValue = string.Format(CultureInfo.CurrentUICulture,
											  "{0:G}ms",
											  timeInMilisecond);
					break;
				case TimeAxisStyle.Second:
					timeValue = string.Format(CultureInfo.CurrentUICulture,
											  "{0:D2}:{1:D3}",
											  timeInMilisecond / 1000,
											  timeInMilisecond % 1000);
					break;
				case TimeAxisStyle.SecondWithUnitDisplay:
					timeValue = string.Format(CultureInfo.CurrentUICulture,
											  "{0:D2}:{1:D3}s",
											  timeInMilisecond / 1000,
											  timeInMilisecond % 1000);
					break;

				case TimeAxisStyle.Smart:
					if (plotRate < 1000)
					{
						if (timeInMilisecond < 10000) // 10000ms
						{
							timeValue = string.Format(CultureInfo.CurrentUICulture,
													  "{0:G}",
													  timeInMilisecond);
						}
						else if (timeInMilisecond >= 10000 && timeInMilisecond < 60000)
						{
							int ms = timeInMilisecond % 1000;
							int sec = timeInMilisecond / 1000;
							timeValue = string.Format(CultureInfo.CurrentUICulture,
													  "{0:D2}:{1:D3}",
													  sec,
													  ms);
						}
						else if (timeInMilisecond >= 60000 && timeInMilisecond < 3600000)
						{
							int tempTime = timeInMilisecond;
							int ms = tempTime % 1000;
							tempTime /= 1000;
							int sec = tempTime % 60;
							tempTime /= 60;
							int min = tempTime;

							timeValue = string.Format(CultureInfo.CurrentUICulture,
													  "{0}:{1:D2}:{2:D3}",
													  min,
													  sec,
													  ms);
						}
						else if (timeInMilisecond >= 3600000)
						{
							int tempTime = timeInMilisecond;
							int ms = tempTime % 1000;
							tempTime /= 1000;
							int sec = tempTime % 60;
							tempTime /= 60;
							int min = tempTime % 60;
							tempTime /= 60;
							int hr = tempTime;

							timeValue = string.Format(CultureInfo.CurrentUICulture,
													  "{0}:{1:D2}:{2:D2}:{3:D3}",
													  hr,
													  min,
													  sec,
													  ms);
						}
					}
					else
					{
						// if the plot rate is more than 1000, then dont show the milisecond part.
						if (timeInMilisecond < 60000)
						{
							int sec = timeInMilisecond / 1000;
							timeValue = string.Format(CultureInfo.CurrentUICulture, "{0:G}", sec);
						}
						else if (timeInMilisecond >= 60000 && timeInMilisecond < 3600000)
						{
							int tempTime = timeInMilisecond / 1000;
							int sec = tempTime % 60;
							tempTime /= 60;
							int min = tempTime;

							timeValue = string.Format(CultureInfo.CurrentUICulture,
													  "{0}:{1:D2}",
													  min,
													  sec);
						}
						else if (timeInMilisecond >= 3600000)
						{
							int tempTime = timeInMilisecond / 1000;
							int sec = tempTime % 60;
							tempTime /= 60;
							int min = tempTime % 60;
							tempTime /= 60;
							int hr = tempTime;

							timeValue = string.Format(CultureInfo.CurrentUICulture,
													  "{0}:{1:D2}:{2:D2}",
													  hr,
													  min,
													  sec);
						}
					}
					break;
			}

			return timeValue;
		}

		private void buttonNextChannel_Click(object sender, EventArgs e)
		{
			NextChannel();
		}

		private void buttonPrevChannel_Click(object sender, EventArgs e)
		{
			PreviousChannel();
		}

		/// <summary>
		/// Gets the coordinates of the coordinate text box that is used to
		/// display the current value of the active channel graph on the plotter
		/// </summary>
		/// <param name="stringSize"></param>
		/// <returns></returns>
		private Rectangle GetCoordinateTextRect(SizeF stringSize)
		{
			int left;
			if (mouseHoverCoordinates.X + stringSize.Width + 2 > GraphArea.Right)
			{
				left = mouseHoverCoordinates.X - (int) (stringSize.Width + 0.5) - 2;
			}
			else
			{
				left = mouseHoverCoordinates.X + 2;
			}

			int rectHeight = 2 * Font.Height;
			int top = GraphArea.Bottom - rectHeight - 2;

			// adding with 0.5 is to round it to the next integer.
			// eg 45.4 becomes 45 and 45.6 becomes 46
			Rectangle rect = new Rectangle(left, top, (int) (stringSize.Width + 0.5), rectHeight);
			return rect;
		}

		/// <summary>
		/// Raises the active channel graph by 5%
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonUpperYPlus_Click(object sender, EventArgs e)
		{
			// raise graph by 5%
			float graduationDiff = (ActiveChannel.MaximumValue - ActiveChannel.MinimumValue) / 20;
			ActiveChannel.MaximumValue += graduationDiff;
			ActiveChannel.MinimumValue += graduationDiff;

			if (currentState != PlotterState.Running)
			{
				RefreshDisplay();
			}
		}

		/// <summary>
		/// Lowers the active channel graph by 5%
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonUpperYMinus_Click(object sender, EventArgs e)
		{
			// reduce graph by 5%
			float graduationDiff = (ActiveChannel.MaximumValue - ActiveChannel.MinimumValue) / 20;
			ActiveChannel.MaximumValue -= graduationDiff;
			ActiveChannel.MinimumValue -= graduationDiff;

			if (currentState != PlotterState.Running)
			{
				RefreshDisplay();
			}
		}

		/// <summary>
		/// TODO:
		/// Write the plotter data in an xml file... this could be used with Excel
		/// any other application.
		/// </summary>
		/// <param name="fileName"></param>
		public void SaveToFile(string fileName)
		{
			// TODO:
			// Write the plotter data in an xml file... this could be used with Excel
			// any other application.

			// TODO: Define the max capacity of the plotter.
			// automatically save if it has reached capacity, reset and then start again
		}

		/// <summary>
		/// TODO:
		/// Load the plotter with an old graph that was saved in an Xml file
		/// </summary>
		/// <param name="fileName"></param>
		public void LoadFromFile(string fileName) {}

		#endregion methods
	}
}