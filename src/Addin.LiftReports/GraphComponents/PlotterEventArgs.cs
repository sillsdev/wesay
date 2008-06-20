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

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// EventArgs class containing information about the plotter
	/// </summary>
	public class PlotterEventArgs: EventArgs
	{
		/// <summary>
		/// The plotter that caused the event to be raised
		/// </summary>
		private readonly Plotter plotter;

		/// <summary>
		/// The plotter that caused the event to be raised
		/// </summary>
		public Plotter CurrentPlotter
		{
			get { return plotter; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="plotter">The plotter that caused the event to be raised</param>
		public PlotterEventArgs(Plotter plotter)
		{
			this.plotter = plotter;
		}
	}
}