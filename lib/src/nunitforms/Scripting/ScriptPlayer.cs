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

namespace NUnit.Extensions.Forms
{
	public class ScriptPlayer : NUnitFormTest
	{
		public delegate void AfterExecuteHandler(int lineNumber, bool success, bool Assert, string message);

		public delegate void BeforeExecuteHandler(int lineNumber);

		public delegate void SuccessHandler(bool success);

		public event BeforeExecuteHandler BeforeExecute;

		public event AfterExecuteHandler AfterExecute;

		public event SuccessHandler Success;

		private bool useHidden = true;

		private Speed speed = new Speed();

		public Speed Speed
		{
			get
			{
				return speed;
			}
		}

		public void setHidden(bool val)
		{
			useHidden = val;
		}

		public override bool UseHidden
		{
			get
			{
				return useHidden;
			}
		}

		public void Play(string scriptString)
		{
			init();

			try
			{
				Script script = new Parser().Parse(scriptString);
				script.Player = this;

				RunScript(script);
			}
			finally
			{
				Verify();
			}
		}

		private void RunScript(Script script)
		{
			bool failure = false;

			foreach(ICommand command in script)
			{
				FireBefore(command.LineNumber);

				try
				{
					command.Execute(Speed);
					FireAfter(command.LineNumber, true, command is AssertCommand, "Success");
				}
				catch(FormsTestAssertionException ae)
				{
					failure = true;
					FireAfter(command.LineNumber, false, command is AssertCommand, ae.Message);
				}
				catch(NoSuchControlException nsce)
				{
					failure = true;
					FireAfter(command.LineNumber, false, command is AssertCommand,
							  string.Format("Could not find control {0}", nsce.Message));
				}
			}

			FireSuccess(!failure);
		}

		private void FireBefore(int i)
		{
			if(BeforeExecute != null)
			{
				BeforeExecute(i);
			}
		}

		private void FireSuccess(bool success)
		{
			if(Success != null)
			{
				Success(success);
			}
		}

		private void FireAfter(int i, bool success, bool assert, string message)
		{
			if(AfterExecute != null)
			{
				AfterExecute(i, success, assert, message);
				;
			}
		}
	}
}