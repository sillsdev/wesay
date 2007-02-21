using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


namespace Reporting
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Logs stuff to a file created in
	/// c:\Documents and Settings\Username\Local Settings\Temp\Companyname\Productname\Log.txt
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class Logger : IDisposable
	{
		private static Logger _singleton;
		protected StreamWriter m_out;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates the logger. The logging functions can't be used until this method is
		/// called.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void Init()
		{
			if(_singleton == null)
				_singleton = new Logger();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Shut down the logger. The logging functions can't be used after this method is
		/// called.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void ShutDown()
		{
			if (_singleton != null)
			{
				_singleton.Dispose();
				_singleton = null;
			}
		}

		private Logger(): this(true)
		{
		}

		private Logger(bool startWithNewFile)
		{
			try
			{
				m_out = null;
				if (startWithNewFile)
				{
					m_out = File.CreateText(LogPath);
					m_out.WriteLine(DateTime.Now.ToLongDateString());
				}
				else
					m_out = File.AppendText(LogPath);

				WriteEvent("App Launched with [" + System.Environment.CommandLine + "]");
			}
			catch
			{
				// If the output file can not be created then just disable logging.
				_singleton = null;
			}
		}

		#region IDisposable & Co. implementation
		// Region last reviewed: never

		/// <summary>
		/// Check to see if the object has been disposed.
		/// All public Properties and Methods should call this
		/// before doing anything else.
		/// </summary>
		public void CheckDisposed()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(String.Format("'{0}' in use after being disposed.", GetType().Name));
		}

		/// <summary>
		/// True, if the object has been disposed.
		/// </summary>
		private bool m_isDisposed = false;

		/// <summary>
		/// See if the object has been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get { return m_isDisposed; }
		}

		/// <summary>
		/// Finalizer, in case client doesn't dispose it.
		/// Force Dispose(false) if not already called (i.e. m_isDisposed is true)
		/// </summary>
		/// <remarks>
		/// In case some clients forget to dispose it directly.
		/// </remarks>
		~Logger()
		{
			Dispose(false);
			// The base class finalizer is called automatically.
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>Must not be virtual.</remarks>
		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Executes in two distinct scenarios.
		///
		/// 1. If disposing is true, the method has been called directly
		/// or indirectly by a user's code via the Dispose method.
		/// Both managed and unmanaged resources can be disposed.
		///
		/// 2. If disposing is false, the method has been called by the
		/// runtime from inside the finalizer and you should not reference (access)
		/// other managed objects, as they already have been garbage collected.
		/// Only unmanaged resources can be disposed.
		/// </summary>
		/// <param name="disposing"></param>
		/// <remarks>
		/// If any exceptions are thrown, that is fine.
		/// If the method is being done in a finalizer, it will be ignored.
		/// If it is thrown by client code calling Dispose,
		/// it needs to be handled by fixing the bug.
		///
		/// If subclasses override this method, they should call the base implementation.
		/// </remarks>
		protected virtual void Dispose(bool disposing)
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (m_isDisposed)
				return;

			if (disposing)
			{
				// Dispose managed resources here.
				if (m_out != null)
					m_out.Close();
			}

			// Dispose unmanaged resources here, whether disposing is true or false.
			m_out = null;

			m_isDisposed = true;
		}

		#endregion IDisposable & Co. implementation

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the entire text of the log file
		/// </summary>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public static string LogText
		{
			get
			{
				if (_singleton == null)
					return "No log available.";

				string logText = _singleton.GetLogTextAndStartOver();

				return logText;
			}
		}

		//enhance: why start over?
		private string GetLogTextAndStartOver()
		{
			CheckDisposed();
			m_out.Flush();
			m_out.Close();

			//get the old
			string contents;
			using (StreamReader reader = File.OpenText(LogPath))
			{
				contents = reader.ReadToEnd();
			}
			_singleton = new Logger(false);
			return contents;
		}

		private static string LogPath
		{
			get
			{
				string path = Path.Combine(Path.GetTempPath(),
					Path.Combine(Application.CompanyName, Application.ProductName));
				Directory.CreateDirectory(path);
				path = Path.Combine(path, "Log.txt");
				return path;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Writes an event to the logger. This method will do nothing if Init() is not called
		/// first.
		/// </summary>
		/// <param name="message"></param>
		/// ------------------------------------------------------------------------------------
		public static void WriteEvent(string message)
		{
			if (_singleton != null)
				_singleton.WriteEvent2(message);
		}

		private void WriteEvent2(string message)
		{
			CheckDisposed();
			if (m_out != null)
			{
				m_out.Write(DateTime.Now.ToLongTimeString() + "\t");
				m_out.WriteLine(message);
				m_out.Flush();//in case we crash
			}
		}
	}
}
