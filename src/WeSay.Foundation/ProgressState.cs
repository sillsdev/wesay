using System;
using System.Diagnostics;
using MultithreadProgress;

namespace WeSay.Foundation.Progress
{
	/// <summary>
	/// Long-running tasks can be written to take one of these as an argument, and use it to notify others of their progress
	/// </summary>
	public class ProgressState : IDisposable
	{
		private readonly ProgressDialogHandler _progressHandler;
		private int _numberOfSteps;
		private int _numberOfStepsCompleted;
		private string _statusLabel;

		public class LogEvent : System.EventArgs
		{
			public string message;

			public LogEvent(string message)
			{
				this.message = message;
			}
		}
		public event System.EventHandler<LogEvent> Log;

		public enum StatusValue
		{
			NotStarted=0,
			Busy,
			Finished,
			StoppedWithError
		} ;

		private StatusValue _status= StatusValue.NotStarted;

		protected  bool _doCancel = false;

		public ProgressState()
		{
			_numberOfStepsCompleted = 0;
		}



		public void WriteToLog(string message)
		{
			if (this.Log != null)
			{
				Log.Invoke(this, new LogEvent(message));
			}
		}

		/// <summary>
		/// How much the task is done
		/// </summary>
		public virtual int NumberOfStepsCompleted
		{
			get
			{
				return _numberOfStepsCompleted;
			}
			set
			{
				_numberOfStepsCompleted = value;
			}
		}

		/// <summary>
		/// a label which describes what we are busy doing
		/// </summary>
		public virtual string StatusLabel
		{
			get
			{
				return _statusLabel;
			}

			set
			{
				_statusLabel = value;
			}
		}

		public virtual int NumberOfSteps
		{
			get
			{
				return _numberOfSteps;
			}
			set
			{
				_numberOfSteps = value;
			}
		}
		public virtual bool Cancel
		{
			get
			{
				return _doCancel;
			}
			set
			{
				_doCancel = value;
			}
		}


		#region IDisposable & Co. implementation
		//Courtesy  of Randy Regnier

		/// <summary>
		/// True, if the object has been disposed.
		/// </summary>
		private bool _isDisposed = false;



		/// <summary>
		/// See if the object has been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get { return _isDisposed; }
		}

		public StatusValue Status
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;
			}
		}

		/// <summary>
		/// Finalizer, in case client doesn't dispose it.
		/// Force Dispose(false) if not already called (i.e. _isDisposed is true)
		/// </summary>
		/// <remarks>
		/// In case some clients forget to dispose it directly.
		/// </remarks>
		~ProgressState()
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
			if (_isDisposed)
				return;
			//
			//            if (disposing)
			//            {
			//                // Dispose managed resources here.
			//                if (_progressBar != null)
			//                {
			//                    _progressBar.ClearStateProvider();
			//                    //_progressBar.Dispose(); // We don't own this!! (JohnT)
			//                }
			//            }

			// Dispose unmanaged resources here, whether disposing is true or false.
			//            _progressBar = null;
			_statusLabel = null;

			_isDisposed = true;
		}

		#endregion IDisposable & Co. implementation



	}

}
