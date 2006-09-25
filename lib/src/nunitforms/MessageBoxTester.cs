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
using System.Text;

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// A ControlTester for MessageBoxes.
	/// Allows you to handle and test MessageBoxes by pressing any of the
	/// buttons that ordinarily appear on them.
	/// </summary>
	/// <remarks>
	/// It does not extend ControlTester because MessageBoxes are not controls.</remarks>
	/// <code>
	/// public void MessageBoxHandler
	/// {
	/// 	MessageBoxTester messageBox = new MessageBoxTester( "MessageBoxName" );
	/// 	Assert.AreEqual( "MessageBoxText", messageBox.Text );
	///   Assert.AreEqual( "MessageBoxTitle", messageBox.Title );
	/// 	messageBox.SendCommand( MessageBoxTester.Command.OK );
	/// }
	/// </code>
	public class MessageBoxTester : ControlTester
	{
		/// <summary>
		/// Available commands you can send to the MessageBox.
		/// </summary>
		/// <remarks>
		/// There are convenience methods for OK and Cancel, so you should not need
		/// those.
		/// </remarks>
		public enum Command : int
		{
			OK = 1,
			Cancel = 2,
			Abort = 3,
			Retry = 4,
			Ignore = 5,
			Yes = 6,
			No = 7,
			Close = 8,
			Help = 9
		}

		private IntPtr handle = new IntPtr(0);

		/// <summary>
		/// Creates a MessageBoxTester with the specified handle.  NUnitForms
		/// users probably won't use this directly.  Use the other constructor.
		/// </summary>
		/// <param name="handle">The handle of the MessageBox to test.</param>
		public MessageBoxTester(IntPtr handle) : base(null)
		{
			this.handle = handle;
		}

		/// <summary>
		/// Creates a MessageBoxTester that finds MessageBoxes with the
		/// specified name.
		/// </summary>
		/// <param name="name">The name of the MessageBox to test.</param>
		public MessageBoxTester(string name) : base(name)
		{
		}

		/// <summary>
		/// Returns the caption on the message box we are testing.
		/// </summary>
		public string Title
		{
			get
			{
				return GetCaption(FindMessageBox());
			}
		}

		/// <summary>
		/// Returns the text of the message box we are testing.
		/// </summary>
		public override string Text
		{
			get
			{
				return GetText(FindMessageBox());
			}
		}

		/// <summary>
		/// Sends a command to the MessageBox.
		/// </summary>
		/// <param name="cmd">The command to send.. (ok, cancel, yes, abort, etc..)</param>
		public void SendCommand(Command cmd)
		{
			IntPtr box = FindMessageBox();
			Win32.SendMessage(box, (int) Win32.WindowMessages.WM_COMMAND, (UIntPtr) cmd, IntPtr.Zero);
		}

		/// <summary>
		/// Clicks the Ok button of a MessageBox.
		/// </summary>
		public void ClickOk()
		{
			SendCommand(Command.OK);
		}

		/// <summary>
		/// Clicks the cancel button of a MessageBox.
		/// </summary>
		public void ClickCancel()
		{
			SendCommand(Command.Cancel);
		}

		private static string GetText(IntPtr handle)
		{
			StringBuilder buffer = new StringBuilder(255);
			IntPtr handleToDialogText = Win32.GetDlgItem(handle, 0xFFFF);
			buffer = new StringBuilder(255);
			Win32.GetWindowText(handleToDialogText, buffer, 255);
			return buffer.ToString();
		}

		internal static string GetCaption(IntPtr handle)
		{
			StringBuilder buffer = new StringBuilder(255);
			Win32.GetWindowText(handle, buffer, 255);
			return buffer.ToString();
		}

		private IntPtr FindMessageBox()
		{
			if(handle != new IntPtr(0))
			{
				return handle;
			}

			lock(this)
			{
				IntPtr desktop = Win32.GetDesktopWindow();
				Win32.EnumChildWindows(desktop, new Win32.WindowEnumProc(OnEnumWindow), IntPtr.Zero);
				if(wParam == IntPtr.Zero)
				{
					throw new ControlNotVisibleException("Message Box not visible");
				}
				return wParam;
			}
		}

		protected bool IsDialog(IntPtr wParam)
		{
			StringBuilder className = new StringBuilder();
			className.Capacity = 255;
			Win32.GetClassName(wParam, className, 255);

			return ("#32770" == className.ToString());
		}

		private IntPtr wParam;

		protected int OnEnumWindow(IntPtr hwnd, IntPtr lParam)
		{
			if(IsDialog(hwnd))
			{
				if(name == null || GetCaption(hwnd) == name)
				{
					wParam = hwnd;
				}
			}
			return 1;
		}
	}
}