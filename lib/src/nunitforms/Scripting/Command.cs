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
using System.Collections;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms
{
	public abstract class Command : ICommand
	{
		private string control;

		private string action;

		private string args;

		private Script script;

		public Command(string control, string action, string args)
		{
			this.control = control;
			this.action = action;
			this.args = args;
		}

		protected string Control
		{
			get
			{
				return control;
			}
		}

		protected string Action
		{
			get
			{
				return action;
			}
		}

		protected string Args
		{
			get
			{
				return args;
			}
		}

		public int LineNumber
		{
			get
			{
				return Script.IndexOf(this);
			}
		}

		public Script Script
		{
			get
			{
				return script;
			}
			set
			{
				script = value;
			}
		}

		private static Hashtable testerTypes = new Hashtable();

		static Command()
		{
			testerTypes[typeof(TextBox)] = typeof(TextBoxTester);
		}

		public abstract void Execute(Speed speed);

		protected ControlTester GetTester()
		{
			ControlTester tester = new ControlTester(control);

			Type testerType = (Type) testerTypes[tester.Control.GetType()];
			if(testerType == null)
			{
				return tester;
			}

			return (ControlTester) Activator.CreateInstance(testerType, new object[] {control});
		}

		private void PaintAllForms()
		{
			FormCollection forms = new FormFinder().FindAll();
			foreach(Form refresh in forms)
			{
				if((refresh is IKeepAlive) && (((IKeepAlive) refresh).KeepAlive))
				{
					continue;
				}
				refresh.Invalidate();
				refresh.Focus();
				Application.DoEvents();
			}
		}

		protected void SlowDownAndPaint(Speed speed)
		{
			if(speed.Value < 100 && !speed.StepMode)
			{
				PaintAllForms();
			}
			speed.Delay();
		}
	}
}