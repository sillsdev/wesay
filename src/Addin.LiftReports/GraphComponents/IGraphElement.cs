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

using System.Drawing;

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// Defines the drawing constraint that all graph types must implement.
	/// </summary>
	internal interface IGraphElement
	{
		void Draw(Graphics graphics);
	}
}