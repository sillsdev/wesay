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
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms
{
	//TODO: should make sure caps lock is off and return it to its pretest state
	//make sure all keys are released at the end of each test.

	/// <summary>
	/// Provides testing control of the keyboard.
	/// </summary>
	/// <remarks>
	/// KeyboardController lets you send key presses to your application.  You can
	/// click, press, or release any key.  The UseOn() method allows you to assert
	/// focus on a specific control before sending the keys.  It also initializes
	/// the Keyboard controller for use by blocking user input.</remarks>
	/// <code>
	///  [Test]
	///	 public void TextBox()
	///  {
	///	   new TextBoxTestForm().Show();
	///	   TextBoxTester box = new TextBoxTester( "myTextBox", CurrentForm );
	/// 	  Assert.AreEqual( "default", box.Text );
	///
	/// 	  Keyboard.UseOn( box );
	///
	///	   Keyboard.Click( Key.A );
	///	   Keyboard.Click( Key.B );
	///	   Keyboard.Press( Key.SHIFT );
	///	   Keyboard.Click( Key.C );
	///	   Keyboard.Release( Key.SHIFT );
	///
	///	   Assert.AreEqual( "abC", box.Text );
	///  }
	/// </code>
	public class KeyboardController : IDisposable
	{
		private bool restoreUserInput = false;

		private KeyboardControl keyboardControl = null;

		internal KeyboardController()
		{
		}

		/// <summary>
		/// Creates a keyboard controller and sets the focus on the control
		/// indicated by the supplied ControlTester.
		/// </summary>
		/// <param name="controlTester">The ControlTester to use the keyboard
		/// on.</param>
		public KeyboardController(ControlTester controlTester)
		{
			UseOn(controlTester);
		}

		/// <summary>
		/// Initializes the KeyboardController, blocks user input, and sets
		/// the focus on the specified control.
		/// </summary>
		/// <param name="control">The ControlTester to use the keyboard on.</param>
		public void UseOn(ControlTester control)
		{
			if(control == null)
			{
				throw new ArgumentNullException("control");
			}

			keyboardControl = new KeyboardControl(control);

			if(!restoreUserInput)
			{
				//if this next line returns false, I used to throw an exception...
				Win32.BlockInput(true);
				restoreUserInput = true;
			}
		}

		/// <summary>
		/// Overloaded.  Allows you to specify the control by name.
		/// </summary>
		/// <remarks>
		/// You should probably use this one if you are not sure.</remarks>
		/// <param name="name">The name of the control.</param>
		public void UseOn(string name)
		{
			UseOn(new ControlTester(name));
		}

		/// <summary>
		/// Overloaded.  Allows you to specify the control by name and
		/// qualified by a form name.
		/// </summary>
		/// <param name="name">The name of the control.</param>
		/// <param name="formName">The name of the form.</param>
		public void UseOn(string name, string formName)
		{
			UseOn(new ControlTester(name, formName));
		}

		/// <summary>
		/// Overloaded.  Allows you to specify the control by name and
		/// qualified by a form instance.  This should be obsolete soon.
		/// Do not use it unless necessary.
		/// </summary>
		/// <param name="name">The name of the control.</param>
		/// <param name="form">The form instance to test on.</param>
		[Obsolete()]
		public void UseOn(string name, Form form)
		{
			UseOn(new ControlTester(name, form));
		}

		/// <summary>
		/// Implements the IDisposable interface.  This restores user input.
		/// It should eventually return the keyboard to its pre-test state.
		/// </summary>
		/// <remarks>
		/// If you are using the Keyboard controller through the base NUnitFormTest
		/// class, then you should not need to call this method or use finally or using
		/// blocks.  The base class handles this for you.</remarks>
		public void Dispose()
		{
			if(keyboardControl != null)
			{
				if(restoreUserInput)
				{
					//if this next line returns false, I used to throw an exception...
					Win32.BlockInput(false);
					restoreUserInput = false;
				}
			}
		}

		/// <summary>
		/// This will send a string of key inputs.
		/// </summary>
		/// <remarks>
		/// Use + for SHIFT
		/// Use ^ for CONTROL
		/// Use % for ALT
		/// Use {} to escape or to group examples
		/// ^a is Control A
		/// +{abc} is SHIFT A SHIFT B SHIFT C
		/// {%} is %
		/// My goal is to support the strings as shown here: http://www.rutherfurd.net/python/sendkeys/
		/// but it is not done yet
		/// </remarks>
		/// <param name="keyString">the keys to type</param>
		public void Type(string keyString)
		{
			Press(keyString);
			//string[] commands = ParseKeys(keyString.ToUpper());
			//for (int i = 0; i < commands.Length; i++)
			//{
			//    string command = commands[i];
			//    if (command == "PRESS")
			//    {
			//        Press(GetKey(commands[++i]));
			//    }
			//    else if (command == "RELEASE")
			//    {
			//        Release(GetKey(commands[++i]));
			//    }
			//    else
			//    {
			//        Click(GetKey(command));
			//    }

			//}
		}

		//private short GetKey(string key)
		//{
		//    FieldInfo field = typeof (Key).GetField(key, BindingFlags.Static | BindingFlags.Public);
		//    return (short) field.GetValue("");
		//}

		private string GetKey(string key)
		{
			string result = key;
			try
			{
				FieldInfo field = typeof(Key).GetField(key, BindingFlags.Static | BindingFlags.Public);
				if(field != null)
				{
					result = (string) field.GetValue("");
				}
			}
			catch(TargetInvocationException)
			{
			}
			return result;
		}

		private string[] ParseKeys(string keys)
		{
			string keyString = Digitize(keys);
			ArrayList commands = new ArrayList();
			for(int i = 0; i < keyString.Length; i++)
			{
				string modifier = (string) modifiers[keyString[i]];
				if(modifier != null)
				{
					i = HandleModifier(commands, keyString, i, modifier);
				}
				else
				{
					i = HandleKey(keyString, commands, i);
				}
			}
			return (string[]) commands.ToArray(typeof(string));
		}

		private string Digitize(string keys)
		{
			StringBuilder sb = new StringBuilder();
			char[] chars = keys.ToCharArray();
			bool inBrackets = false;
			foreach(char c in chars)
			{
				inBrackets = inBrackets && c != '}' || c == '{';
				if(!inBrackets && Char.IsDigit(c))
				{
					sb.Append("{DIGIT_");
					sb.Append(c);
					sb.Append("}");
				}
				else
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		//		public bool CheckBrackets(bool inBrackets, char c)
		//		{
		////			if (c=='}')
		////			{
		////					return false;
		////			}
		////			if (c=='{')
		////			{
		////				return true;
		////			}
		////			return inBrackets;
		////
		////			c=='{' || (inBrackets && c!='}')
		//
		//
		//		}

		private static Hashtable modifiers = new Hashtable();

		static KeyboardController()
		{
			modifiers['%'] = "ALT";
			modifiers['+'] = "SHIFT";
			modifiers['^'] = "CONTROL";
		}

		private int HandleModifier(ArrayList commands, string keyString, int i, string modifier)
		{
			commands.Add("PRESS");
			commands.Add(modifier);
			int next = HandleNext(keyString, commands, i);
			commands.Add("RELEASE");
			commands.Add(modifier);
			return next;
		}

		private int HandleKey(string keyString, ArrayList commands, int i)
		{
			int nextIndex = i++;
			string next = keyString[nextIndex].ToString();
			if(next == "{")
			{
				int endBracket = keyString.IndexOf('}', nextIndex);
				commands.Add(keyString.Substring(nextIndex + 1, endBracket - nextIndex - 1));
				nextIndex = endBracket;
			}
			else
			{
				commands.Add(next);
			}
			return nextIndex;
		}

		private int HandleNext(string keyString, ArrayList commands, int i)
		{
			string next = keyString[++i].ToString();
			if(next == "{")
			{
				int endBracket = keyString.IndexOf('}', i);
				commands.AddRange(ParseKeys(keyString.Substring(i + 1, endBracket - i - 1)));
				i = endBracket;
			}
			else
			{
				commands.Add(next);
			}
			return i;
		}

		/// <summary>
		/// Press and release a key.
		/// these constants.
		/// </summary>
		/// <remarks>
		/// Use the Key class (in Key.cs) to find these constants.
		/// </remarks>
		///// <param name="key">The key to click.</param>
		//public void Click(short key)
		//{
		//    Press(key);
		//    Release(key);
		//}
		/// <param name="key">The key to click.</param>
		public void Click(string key)
		{
			Press(key);
			Release(key);
		}

		/// <summary>
		/// Press a key.
		/// </summary>
		/// <remarks>
		/// Use the Key class (in Key.cs) to find these constants.
		/// </remarks>
		/// <param name="key">The key to press.</param>
		//public void Press(short key)
		//{
		//    keyboardControl.Focus();
		//    Win32.KEYBDINPUT ki = new Win32.KEYBDINPUT();
		//    ki.type = Win32.INPUT_KEYBOARD;
		//    //ki.dwExtraInfo = 0;
		//    ki.dwExtraInfo = Win32.GetMessageExtraInfo();
		//    ki.dwFlags = 0;
		//    ki.time = 0;
		//    ki.wScan = 0;
		//    ki.wVk = key;
		//    if (key == Key.DIGIT_3)
		//    {
		//        System.Console.WriteLine("digit 3 : " + key + " mvk : " + ki.wVk);
		//        SendKeys.SendWait("3");
		//    }
		//    else if (0 == Win32.SendKeyboardInput(1, ref ki, Marshal.SizeOf(ki)))
		//    {
		//        throw new Win32Exception();
		//    }
		//    Application.DoEvents();
		//}
		public void Press(string key)
		{
			keyboardControl.Focus();

			SendKeys.SendWait(key);

			Application.DoEvents();
		}

		/// <summary>
		/// Release a key.
		/// </summary>
		/// <remarks>
		/// Use the Key class (in Key.cs) to find these constants.
		/// </remarks>
		/// <param name="key">The key to release.</param>
		//public void Release(short key)
		//{
		//    Win32.KEYBDINPUT ki = new Win32.KEYBDINPUT();
		//    ki.type = Win32.INPUT_KEYBOARD;
		//    ki.dwExtraInfo = Win32.GetMessageExtraInfo();
		//    ki.dwFlags = Win32.KEYEVENTF_KEYUP;
		//    ki.time = 0;
		//    ki.wScan = 0;
		//    ki.wVk = key;
		//    if (0 == Win32.SendKeyboardInput(1, ref ki, Marshal.SizeOf(ki)))
		//    {
		//        throw new Win32Exception();
		//    }
		//    Application.DoEvents();
		//}
		public void Release(string key)
		{
			Application.DoEvents();
		}
	}
}