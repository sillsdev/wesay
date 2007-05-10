using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace GraphComponents
{
	/// <summary>
	/// This class represents a channel (the word 'Channel' is normally used while
	/// referring to the input terminals of a CRO. So, the same term is being used
	/// here also). In a typical CRO, there are 2 channels. A channel is characte-
	/// rized by the input, and the upper and lower limits.
	/// </summary>
	public class Channel
	{
		#region variables
		/// <summary>
		/// Minimum value to display on the Y axis
		/// </summary>
		private float minimumValue;
		/// <summary>
		/// Maximum value to display on the Y axis
		/// </summary>
		private float maximumValue = 100;
		/// <summary>
		/// Current y value of the channel
		/// </summary>
		private float currentValue;
		/// <summary>
		/// Name for the Y axis. Eg. Voltage, Current etc
		/// </summary>
		private string name;
		/// <summary>
		/// A flag indicating whether the channel is enabled or not
		/// </summary>
		private bool enabled;
		/// <summary>
		/// Color of the channel
		/// </summary>
		private Color channelColor = Color.Green;
		/// <summary>
		/// Offset of the cursor from the center line
		/// </summary>
		private float cursorOffset;

		/// <summary>
		/// Points stored as new values are updated to the plotter
		/// </summary>
		private Hashtable points = new Hashtable ();

		/// <summary>
		/// the X axis cursor line associated with the channel
		/// </summary>
		private Cursor channelCursor;

		/// <summary>
		/// Total time elapsed while plotting
		/// </summary>
		private int totalTimeElapsed;

		/// <summary>
		/// Time in milisecond when the plotter must plot the next point
		/// ie. The X distance between 2 points
		/// </summary>
		private int plotRate;
		#endregion variables

		#region properties
		/// <summary>
		/// Minimum value to display on the Y axis
		/// </summary>
		public float MinimumValue
		{
			get { return minimumValue;  }
			set { minimumValue = value; }
		}

		/// <summary>
		/// Maximum value to display on the Y axis
		/// </summary>
		public float MaximumValue
		{
			get { return maximumValue;  }
			set { maximumValue = value; }
		}

		/// <summary>
		/// Current y value of the channel
		/// </summary>
		public float CurrentValue
		{
			get { return currentValue;  }
			set
			{
				currentValue = value;
				PointF p = new PointF (totalTimeElapsed, currentValue);
				points.Add (points.Count, p);
				totalTimeElapsed += PlotRate;
			}
		}

		/// <summary>
		/// A flag indicating whether the channel is enabled or not.
		/// Set this to true to enable the channel and false to disable it.
		/// It is similar to switching it on and off
		/// </summary>
		public bool Enabled
		{
			get { return enabled;  }
			set { enabled = value; }
		}

		/// <summary>
		/// Name for the Y axis. Eg. Voltage, Current etc
		/// </summary>
		public string YAxisName
		{
			get { return name;  }
			set { name = value; }
		}

		/// <summary>
		/// Color of the channel
		/// </summary>
		public Color ChannelColor
		{
			get { return channelColor;  }
			set { channelColor = value; }
		}

		/// <summary>
		/// Offset of the cursor from the center line
		/// </summary>
		internal float CursorOffset
		{
			get { return cursorOffset;  }
			set
			{
				cursorOffset = value;
				maximumValue -= cursorOffset;
				minimumValue += cursorOffset;
			}
		}

		/// <summary>
		/// Time in milisecond when the plotter must plot the next point
		/// ie. The X distance between 2 points
		/// </summary>
		internal int TotalTimeElapsed
		{
			get { return totalTimeElapsed; }
			set { totalTimeElapsed = value; }
		}

		/// <summary>
		/// Time in milisecond when the plotter must plot the next point
		/// ie. The X distance between 2 points
		/// </summary>
		internal int PlotRate
		{
			get { return plotRate;  }
			set { plotRate = value; }
		}

		/// <summary>
		/// the X axis cursor line associated with the channel
		/// </summary>
		internal Cursor ChannelCursor
		{
			get { return channelCursor; }
		}

		/// <summary>
		/// Points stored as new values are updated to the plotter
		/// </summary>
		public Hashtable Points
		{
			get { return points; }
		}
		#endregion properties

		#region methods
		public Channel()
		{
			channelCursor = new Cursor (this);
		}

		public Channel (float minValue, float maxValue, string yAxisName)
		{
			this.minimumValue = minValue;
			this.maximumValue = maxValue;
			this.YAxisName    = yAxisName;

			channelCursor = new Cursor (this);
		}

		public Channel (float minValue, float maxValue, string yAxisName, bool enabled)
		{
			this.minimumValue = minValue;
			this.maximumValue = maxValue;
			this.YAxisName    = yAxisName;
			this.enabled      = enabled;

			channelCursor = new Cursor (this);
		}

		public Channel (float minValue, float maxValue, string yAxisName, bool enabled, Color channelColor)
		{
			this.minimumValue = minValue;
			this.maximumValue = maxValue;
			this.YAxisName    = yAxisName;
			this.enabled      = enabled;
			this.channelColor = channelColor;

			channelCursor = new Cursor (this);
		}
		#endregion methods
	}
}
