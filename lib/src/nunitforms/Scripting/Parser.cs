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
	internal class Parser
	{
		public Script Parse(string scriptString)
		{
			Script script = new Script();

			CommandFactory factory = new CommandFactory();

			scriptString = scriptString.Replace("\r\n", "\n");
			string[] lines = scriptString.Trim().Split('\n');

			script.Add(factory.CreateCommand(lines[0])); //first line is a special command to show form.

			for(int i = 1; i < lines.Length; i++)
			{
				if(lines[i].Trim() == string.Empty)
				{
					script.Add(factory.CreateCommand());
				}
				else if(lines[i].Trim().StartsWith("'"))
				{
					script.Add(factory.CreateCommand());
				}
				else
				{
					//this requires exactly one space between tokens.. *sigh*
					//don't want to split args or force quotes around strings with spaces..
					string thisLine = lines[i];
					int firstBreak = thisLine.IndexOf(" ");
					int secondBreak = thisLine.IndexOf(" ", firstBreak + 1);
					string control = thisLine.Substring(0, firstBreak);
					string action = thisLine.Substring(firstBreak + 1, secondBreak - firstBreak - 1);
					string args = thisLine.Substring(secondBreak + 1);
					script.Add(factory.CreateCommand(control, action, args));
				}
			}
			return script;
		}
	}
}