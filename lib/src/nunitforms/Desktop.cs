#region Copyright (c) 2003-2005, Luke T. Maxon (Levi Khatskevitch is the original author)

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

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// Do not use directly.  Just use base test class and it takes care of it.
	/// </summary>
	/// <remarks>
	/// 	CreateWindowStation()
	///     SetProcessWindowStation()
	///     CreateDesktop()
	///     SetThreadDesktop()
	///     SwitchDesktop()
	/// Then,
	///     CloseWindowStation()
	///     CloseDesktop()
	///
	///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/createwindowstation.asp
	///
	///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/setprocesswindowstation.asp
	///
	///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/createdesktop.asp
	///
	///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/setthreaddesktop.asp
	///
	///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/switchdesktop.asp
	///
	///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/closewindowstation.asp
	///
	///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/closedesktop.asp
	///
	///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/createdesktop.asp
	///
	///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/window_station_and_desktop_creation.asp
	///
	/// It does work.  I wrote a test to verify.
	///
	/// The test, running from my login session, first enumerates all top level
	/// windows (EnumWindows).  It enumerated 1228 windows.  It then creates a new
	/// desktop (hNewDesk = CreateDesktop), changes the thread desktop to the new
	/// desktop (SetThreadDesktop(hNewDesk)), and then creates a new window (a
	/// button control).  It then enumerates windows again, and this time enumerates
	/// only 3 windows (my button plus two others the system must have created for
	/// the new desktop).
	///
	/// One thing I found out - you cannot call SetThreadDesktop if the thread (or
	/// maybe even the process - not sure) owns a window.  I was creating the button
	/// and then calling SetThreadDesktop, but the SetThreadDesktop call failed with
	/// 0xaa-  the resource is in use or something like that.  I changed the test to
	/// create the button *after* calling SetThreadDesktop and that worked fine.
	///
	/// One other thing: your process may not have permission to switch to the other
	/// desktop.  I was at first trying to switch to the WinLogon desktop - I should
	/// have known better - access denied.  You may have to adjust your
	/// permissions/privileges for it to work.  Services can do it (that's primarly
	/// who the SetThreadDesktop API was created for), but I think they have to run
	/// under local system account to do it.
	///
	/// http://vbaccelerator.com/home/VB/Code/Libraries/Windows/Creating_New_Desktops/article.asp
	///
	/// Thread Connection to a Desktop
	/// After a process connects to a window station, the system assigns a desktop to the thread making the connection. The
	/// system determines the desktop to assign to the thread according to the following rules:
	///
	///
	/// If the thread has called the SetThreadDesktop function, it connects to the specified desktop.
	/// If the thread did not call SetThreadDesktop, it connects to the desktop inherited from the parent process.
	/// If the thread did not call SetThreadDesktop and did not inherit a desktop, the system attempts to open for MAXIMUM_ALLOWED access and connect to a desktop as follows:
	///
	/// If a desktop name was specified in the lpDesktop member of the STARTUPINFO structure that was passed to the CreateProcess function when the process was created, the thread connects to the specified desktop.
	/// Otherwise, the thread connects to the default desktop of the window station to which the process connected.
	/// The desktop assigned during this connection process cannot be closed by calling the CloseDesktop function.
	///
	/// <b>
	/// When a process is connecting to a desktop, the system searches the process's handle table for inherited
	/// handles. The system uses the first desktop handle it finds. If you want a child process to connect to
	/// a particular inherited desktop, you must ensure that the only the desired handle is marked inheritable. If a
	/// child process inherits multiple desktop handles, the results of the desktop connection are undefined.
	///
	/// Handles to a desktop that the system opens while connecting a process to a desktop are not inheritable.
	/// </b>
	///
	/// </remarks>
	/// <summary>
	/// Do not use directly.  Just use base test class and it takes care of it.
	/// </summary>
	public class Desktop : IDisposable
	{
		private IntPtr handle;

		private IntPtr realDesktop;

		private bool display;

		public Desktop() : this("NUnitForms Test Desktop")
		{
		}

		public Desktop(string name) : this(name, false)
		{
		}

		public Desktop(string name, bool display)
		{
			this.display = display;

			realDesktop = CurrentHandle();

			handle = Win32.CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, Win32.GENERIC_ALL, IntPtr.Zero);

			Win32.SetThreadDesktop(handle);

			Switch(display);
		}

		~Desktop()
		{
			Destroy();
		}

		public void Dispose()
		{
			Destroy();
			GC.SuppressFinalize(this);
		}

		private void Destroy()
		{
			Win32.SetThreadDesktop(realDesktop);

			Switch(display);

			Win32.CloseDesktop(handle);
		}

		private void Switch(bool Switch)
		{
			if(Switch)
			{
				Win32.SwitchDesktop(CurrentHandle());
			}
		}

		private IntPtr CurrentHandle()
		{
			//return Win32.GetThreadDesktop( AppDomain.GetCurrentThreadId());
			return Win32.GetThreadDesktop(Win32.GetCurrentThreadId());
		}
	}

	/*Code to experiment with desktop switch.*/
	//public class Desktop : IDisposable
	//{
	//    private IntPtr handle;
	//    private IntPtr realDesktop;
	//    private bool display;

	//    public Desktop( ) : this("NUnitForms Test Desktop")
	//    {
	//    }

	//    public Desktop( string name ) : this(name, false)
	//    {
	//    }

	//    /// <summary>
	//    ///
	//    /// </summary>
	//    /// <param name="name"></param>
	//    /// <param name="display"></param>
	//    /// <remarks>
	//    ///
	//    /// </remarks>
	//    public Desktop( string name, bool display )
	//    {

	//        Win32.EnumDesktops(Win32.GetProcessWindowStation(), new Win32.EnumDesktopsDelegate(Win32.EnumDesktopsCallback), IntPtr.Zero);
	//        ///     CreateWindowStation()
	//        ///     SetProcessWindowStation()
	//        ///     CreateDesktop()
	//        ///     SetThreadDesktop()
	//        ///     SwitchDesktop()
	//        ///
	//        originalWindowStation = Win32.GetProcessWindowStation();
	//        windowStation = Win32.CreateWindowStation("CreateWindowStation", 0,Win32.GENERIC_ALL, IntPtr.Zero);
	//        Win32.OpenWindowStation("CreateWindowStation", true, Win32.GENERIC_ALL);
	//        //Win32.OpenInputDesktop(0x0001, true, Win32.GENERIC_ALL);
	//        if (windowStation == IntPtr.Zero)
	//        {
	//            System.Console.WriteLine("fout in CreateWindowStation, handler is niet effectief");
	//        }

	//        bool setProcessWindowStation = Win32.SetProcessWindowStation(windowStation);
	//        System.Console.WriteLine("error code SetProcessWindowStation : " + setProcessWindowStation);


	//        //System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("nunit-console");
	//        //System.Console.WriteLine("Process number : " + process.Length);
	//        //foreach (System.Diagnostics.Process proc in process)
	//        //{
	//        //    foreach (System.Diagnostics.ProcessThread pt in proc.Threads)
	//        //    {
	//        //        bool enumDesktop = Win32.EnumThreadWindows((uint)pt.Id, new Win32.EnumThreadDelegate(Win32.EnumThreadCallback), IntPtr.Zero);
	//        //        System.Console.WriteLine("Enum Thread Desktop geslaagd : " + enumDesktop + " error : " + " " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
	//        //    }
	//        //}
	//        System.Console.WriteLine("Desktop constructor display : " + display);
	//        this.display = display;

	//        realDesktop = CurrentHandle();

	//        handle = Win32.CreateDesktop( name, IntPtr.Zero, IntPtr.Zero, 0, Win32.GENERIC_ALL, IntPtr.Zero );
	//        System.Console.WriteLine("Desktop constructor realDesktop A : " + realDesktop + " handle " + handle + " current handle " + CurrentHandle());
	//        Win32.EnumDesktops(windowStation, new Win32.EnumDesktopsDelegate(Win32.EnumDesktopsCallback), IntPtr.Zero);
	//        Win32.SetThreadDesktop( handle );
	//        System.Console.WriteLine("Desktop constructor realDesktop B : " + realDesktop + " handle " + handle + " current handle " + CurrentHandle());
	//        IntPtr nUnitFormsDesktop = Win32.OpenDesktop("NUnitForms Test Desktop", 0x0001, true, Win32.GENERIC_ALL);
	//        Win32.SetThreadDesktop(nUnitFormsDesktop);
	//        int switchError = Win32.SwitchDesktop(nUnitFormsDesktop);
	//        System.Console.WriteLine("Desktop constructor Switch error : " + switchError);
	//        //Switch( true );
	//        System.Console.WriteLine("Desktop constructor realDesktop C : " + realDesktop + " handle " + handle + " current handle " + CurrentHandle());
	//    }

	//    ~Desktop()
	//    {
	//        Dispose(false);
	//    }

	//    public void Dispose()
	//    {
	//        Dispose(true);
	//        GC.SuppressFinalize( this );
	//    }

	//    /// <summary>
	//    ///
	//    /// </summary>
	//    /// <remarks>
	//    /// Loopt deze methode in de correcte thread ?
	//    /// </remarks>
	//    protected virtual void Dispose(bool dispose)
	//    {
	//        ///     CloseWindowStation()
	//        ///     CloseDesktop()
	//        if (!disposed)
	//        {
	//            if (dispose)
	//            {
	//                //TODO laaste lijn in deze methode vooraan geplaatst, is de 170 hiermee van de baan ?
	//                // int errorClose = Win32.CloseDesktop(handle); //-> maakt niets uit
	//                //System.Console.WriteLine("Destroy - Close desktop " + errorClose + " " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
	//                //TODO hier zit precies een bug : errorCode is niet gelijk aan nul, maar 170, dwz dat de resource "bezig is"
	//                //bool enumDesktop = Win32.EnumDesktopWindows(realDesktop, new Win32.EnumDelegate(Win32.EnumWindowsProc), IntPtr.Zero);
	//                //System.Console.WriteLine("Enum Desktop geslaagd : " + enumDesktop);
	//                //System.Console.WriteLine("Destroying Desktop");
	//                //int errorCode = Win32.SetThreadDesktop(realDesktop);

	//                //System.Console.WriteLine("Destroy - SetThreadDesktop : " + errorCode + " " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());

	//                ////TODO Bart
	//                //Switch(display);
	//                //if (display)
	//                //{
	//                //    int result = Win32.SwitchDesktop(realDesktop);
	//                //    System.Console.WriteLine("Destroy - SwitchDesktop " + result);
	//                //}

	//                //enumDesktop = Win32.EnumDesktopWindows(handle, new Win32.EnumDelegate(Win32.EnumWindowsProc), IntPtr.Zero);
	//                //System.Console.WriteLine("Enum Desktop geslaagd : " + enumDesktop + " error : " + " " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
	//                //System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("nunit-console");
	//                //System.Console.WriteLine("Process number : " + process.Length);
	//                //foreach (System.Diagnostics.Process proc in process)
	//                //{
	//                //    foreach (System.Diagnostics.ProcessThread pt in proc.Threads)
	//                //    {
	//                //        enumDesktop = Win32.EnumThreadWindows((uint)pt.Id, new Win32.EnumThreadDelegate(Win32.EnumThreadCallback), IntPtr.Zero);
	//                //        System.Console.WriteLine(proc + " with Enum Thread " + pt.Id + " Desktop geslaagd : " + enumDesktop + " error : " + " " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
	//                //    }
	//                //}
	//                bool closeWindowStation = Win32.CloseWindowStation(windowStation);
	//                Win32.EnumDesktops(Win32.GetProcessWindowStation(), new Win32.EnumDesktopsDelegate(Win32.EnumDesktopsCallback), IntPtr.Zero);
	//                System.Console.WriteLine("error code CloseWindowStation : " + closeWindowStation + " " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
	//                int switchError = Win32.SwitchDesktop(realDesktop);
	//                System.Console.WriteLine("Switch error : " + switchError);
	//                bool setProcessWindowStation = Win32.SetProcessWindowStation(originalWindowStation);
	//                System.Console.WriteLine("error SetProcessWindowStation : " + setProcessWindowStation);
	//                int errorCode = Win32.SetThreadDesktop(realDesktop);

	//                System.Console.WriteLine("Destroy - SetThreadDesktop : " + errorCode + " " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());

	//                closeWindowStation = Win32.CloseWindowStation(windowStation);
	//                Win32.EnumDesktops(Win32.GetProcessWindowStation(), new Win32.EnumDesktopsDelegate(Win32.EnumDesktopsCallback), IntPtr.Zero);
	//                System.Console.WriteLine("error code CloseWindowStation : " + closeWindowStation + " " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
	//                int errorClose = Win32.CloseDesktop(handle);
	//                System.Console.WriteLine("Destroy - Close desktop " + errorClose + " " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
	//            }
	//            disposed = true;
	//        }
	//    }

	//    private void Switch( bool Switch )
	//    {
	//        if (Switch)
	//        {
	//            System.Console.WriteLine("Calling Switch in Desktop.Switch " + this + " . current handle : " + CurrentHandle().ToInt32() + " realDesktop : " + realDesktop.ToInt32() + " handle " + handle.ToInt32());
	//            //TODO Bart
	//            Win32.SwitchDesktop( CurrentHandle() );
	//            //Win32.SwitchDesktop(realDesktop);
	//        }
	//    }

	//    /// <summary>
	//    ///
	//    /// </summary>
	//    /// <returns>
	//    /// The handle to the desktop assigned to the current running thread.
	//    /// </returns>
	//    private IntPtr CurrentHandle()
	//    {
	//        //return Win32.GetThreadDesktop( AppDomain.GetCurrentThreadId());
	//        return Win32.GetThreadDesktop(Win32.GetCurrentThreadId());
	//    }

	//    private bool disposed = false;

	//    IntPtr windowStation = IntPtr.Zero;

	//    IntPtr originalWindowStation = IntPtr.Zero;
	//}
}