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

namespace GraphComponents
{
	/// <summary>
	/// Style in which the gridlines appear on the graph
	/// </summary>
	[Flags]
	public enum GridStyles : int
	{
		None       = 0,
		Horizontal = 1,
		Vertical   = 2,
		Grid       = 3,
	}

	/// <summary>
	/// Orientation of the bar
	/// </summary>
	public enum Orientation
	{
		Vertical,
		Horizontal,
	}

	/// <summary>
	/// The location where the graduations must appear
	/// </summary>
	public enum Graduation
	{
		None,
		Center,
		Edge1,
		Edge2,
	}

	/// <summary>
	/// Style in which the bar's values are to be displayed on the bar
	/// </summary>
	public enum TextAlignment
	{
		AbsoluteCenter,
		BarValueCenter,
		Smart,
		None,
	}

	/// <summary>
	/// Style in which the values on the time axis is to be shown
	/// </summary>
	public enum TimeAxisStyle
	{
		/// <summary>
		/// Shows the values in millisecond. eg. 10, 20, ..., 50000
		/// </summary>
		Millisecond,
		/// <summary>
		/// Shows the values in millisecond with the 'ms' suffix. eg. 10ms, 20ms, ..., 50000ms
		/// </summary>
		MillisecondWithUnitDisplay,
		/// <summary>
		/// Shows the values in second. eg. 10, 20, ..., 50000
		/// </summary>
		Second,
		/// <summary>
		/// Shows the values in second with the 's' suffix. eg. 10s, 20s, ..., 50000s
		/// </summary>
		SecondWithUnitDisplay,
		/// <summary>
		/// Shows the value in the most appropriate format as we go on plotting, eg. 1:02:45:638
		/// </summary>
		Smart,
	}

}
