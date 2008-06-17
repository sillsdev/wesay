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

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// Provides a simple structure for getting and setting properties for
	/// a stacked bar graph.
	/// </summary>
	public class Bar
	{
		/// <summary>
		/// Name of the bar
		/// </summary>
		private string name;

		/// <summary>
		/// Current value of the bar
		/// </summary>
		private float barValue;

		/// <summary>
		/// Name of the bar
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Current value of the bar
		/// </summary>
		public float BarValue
		{
			get { return barValue; }
			set { barValue = value; }
		}

		public Bar(string name, float barValue)
		{
			this.name = name;
			this.barValue = barValue;
		}

		public Bar()
		{
			name = "";
			barValue = 35.0F;
		}
	}
}