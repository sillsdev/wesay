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
  public class FileDialogTester
  {
	#region Private/Protected attributes.

	protected const int OpenButton = 1;
	protected const int CancelButton = 2;
	protected const int FileNameCheckBox = 1148;

	/// <summary>
	/// The filename to use when simulate an open file operation
	/// </summary>
	protected string _fileName = "";


	protected IntPtr _handle = new IntPtr(0);

	protected IntPtr _wParam;

	/// <summary>
	/// Name/title of the OpenFileDialog
	/// </summary>
	protected string name = "Open";

	#endregion
	/// <summary>
	/// Default constructor...
	/// </summary>
	public FileDialogTester(string title)
	{
	  name = title;
	}

/*
	public void OpenSaveFile(string file)
	{
	  _fileName = file;
	  System.Threading.Thread thr = new System.Threading.Thread(new System.Threading.ThreadStart(OpenFileNameHandler));
	  thr.Start();
	}
	*/
	/// <summary>
	/// This handler will be called when the OpenFileDialog is shown and the
	/// user have choose to open a file.
	/// </summary>
	protected void FileNameHandler()
	{
	  SetFileName(_fileName);
	}

	/// <summary>
	/// Simulates a click on  cancel.
	/// For some reason we need to spawn a new thread because the FileDialog Caption
	/// will not change to correct name if we just posts the message.
	/// If we Calls the ClickCancelHandler directly we need to set the title
	/// of the FileDialog to "Open". (Strange)
	/// </summary>
	virtual public void ClickCancel()
	{
	  System.Threading.Thread thr = new System.Threading.Thread(new System.Threading.ThreadStart(ClickCancelHandler));
	  thr.Start();
	}
	/// <summary>
	/// Clicks the cancel button of a OpenFiledialog.
	/// </summary>
	public void ClickCancelHandler()
	{
	  IntPtr box = FindFileDialog();
	  IntPtr cancel_btn = Win32.GetDlgItem(box, CancelButton);
	  Win32.PostMessage(cancel_btn, (uint)Win32.BM_CLICK, (IntPtr)0, IntPtr.Zero);
	}

	/// <summary>
	/// Simulates that the open button is pressed.
	/// </summary>
	protected void ClickOpenSaveOpen()
	{
	  IntPtr box = FindFileDialog();
	  IntPtr open_btn = Win32.GetDlgItem(box, OpenButton);
	  Win32.PostMessage(open_btn, (uint)Win32.BM_CLICK, (IntPtr)0, IntPtr.Zero);
	}

	/// <summary>
	/// Sets the filename in the filename ComboBox and presses the OpenSave button.
	/// </summary>
	/// <param name="file_name"></param>
	protected void SetFileName(string file_name)
	{
	  IntPtr box = FindFileDialog();
	  Win32.SetDlgItemText(box, FileNameCheckBox, file_name);
	  IntPtr open_btn = Win32.GetDlgItem(box, OpenButton);
	  Win32.PostMessage(open_btn, (uint)Win32.BM_CLICK, (IntPtr)0, IntPtr.Zero);

	}


	/// <summary>
	/// Finds the OpenFileDialog.
	/// </summary>
	/// <returns></returns>
	protected IntPtr FindFileDialog()
	{
	  if (_handle != new IntPtr(0))
	  {
		return _handle;
	  }

	  lock (this)
	  {
		IntPtr desktop = Win32.GetDesktopWindow();
		Win32.EnumChildWindows(desktop, new Win32.WindowEnumProc(OnEnumWindow), IntPtr.Zero);
		if (_wParam == IntPtr.Zero)
		{
		  throw new ControlNotVisibleException("Open File Dialog is not visible");
		}
		return _wParam;
	  }
	}

	internal static string GetCaption(IntPtr handle)
	{
	  StringBuilder buffer = new StringBuilder(255);
	  Win32.GetWindowText(handle, buffer, 255);
	  return buffer.ToString();
	}

	private string GetClassName(IntPtr wParam)
	{
	  StringBuilder className = new StringBuilder();
	  className.Capacity = 255;
	  Win32.GetClassName(wParam, className, 255);
	  return className.ToString();

	}
	private bool IsDialog(IntPtr wParam)
	{
	  return ("#32770" == GetClassName(wParam));
	}

	private int OnEnumWindow(IntPtr hwnd, IntPtr lParam)
	{
	  if (IsDialog(hwnd))
	  {
		if (this.name == null || GetCaption(hwnd) == this.name)
		{
		  _wParam = hwnd;
		}
	  }
	  return 1;
	}
  }
}
