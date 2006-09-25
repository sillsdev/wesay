#region Copyright (c) 2006, Luke T. Maxon

/********************************************************************************************************************
'
' Copyright (c) 2006, Luke T. Maxon
' All rights reserved.
' Author: Anders Lillrank
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
using System.Collections.Generic;
using System.Text;

namespace NUnit.Extensions.Forms
{
  /// <summary>
  /// This class is used to test the built-in OpenFileDialog. This class is not meant to be
  /// used directly. Instead you should use the ExpectOpenFileDialog and CancelOpenFileDialog functions
  /// in the NUnitFormTest
  /// class.
  /// </summary>
  public class OpenFileDialogTester : FileDialogTester
  {

	/// <summary>
	/// Default constructor...
	/// </summary>
	public OpenFileDialogTester(string title) :base(title)
	{
	}


	public void OpenFile(string file)
	{
	  _fileName = file;
	  System.Threading.Thread thr = new System.Threading.Thread(new System.Threading.ThreadStart(FileNameHandler));
	  thr.Start();
	}

	/// <summary>
	/// Simulates that the open button is pressed.
	/// </summary>
	private void ClickOpen()
	{
	  IntPtr box = FindFileDialog();
	  IntPtr open_btn = Win32.GetDlgItem(box, OpenButton);
	  Win32.PostMessage(open_btn, (uint)Win32.BM_CLICK, (IntPtr)0, IntPtr.Zero);
	}
  }
}
