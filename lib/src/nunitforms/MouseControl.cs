#region Copyright (c) 2003-2005, Luke T. Maxon

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
' All rights reserved.
'
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
'
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
'
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
'
' * Neither the name of the author nor the names of its contributors may be used to endorse or
' 	promote products derived from this software without specific prior written permission.
'
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
'*******************************************************************************************************************/

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms
{
	internal class MouseControl
	{
		private ControlTester controlTester;

		internal MouseControl(ControlTester controlTester)
		{
			this.controlTester = controlTester;
		}

		internal void Focus()
		{
			Control.FindForm().Activate();
		}

		internal PointF Resolution
		{
			get
			{
				using(Graphics g = Control.CreateGraphics())
				{
					return new PointF(g.DpiX, g.DpiY);
				}
			}
		}

		/// <summary>
		///   Translation between mouse position and screen position.
		/// </summary>
		/// <param name="p">
		///   A <see cref="PointF"/> mouse coordinate relative to
		///   the origin control and specified in PositionUnit.
		/// </param>
		/// <param name="scale">
		/// The scale to convert by.
		/// </param>
		/// <returns>
		///   A <see cref="Win32.Point"/> mouse coordinate relative to
		///   the screen and specified in pixels.
		/// </returns>
		internal Win32.Point Convert(PointF p, PointF scale)
		{
			Point pixel = new Point((int) Math.Round(p.X*scale.X), (int) Math.Round(p.Y*scale.Y));
			Point screen = Control.PointToScreen(pixel);
			return new Win32.Point(screen.X, screen.Y);
		}

		internal PointF Convert(Win32.Point screen, PointF scale)
		{
			Point client = Control.PointToClient(new Point(screen.x, screen.y));
			return new PointF(client.X/scale.X, client.Y/scale.Y);
		}

		private Control Control
		{
			get
			{
				Control control = controlTester.Control;

				if(!control.IsHandleCreated)
				{
					Application.DoEvents();
				}

				// TODO: throw if not visible.
				return control;
			}
		}
	}
}