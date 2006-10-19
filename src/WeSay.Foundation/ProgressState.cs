using System;
using System.Diagnostics;
using MultithreadProgress;

namespace WeSay.Foundation.Progress
{
	/// <summary>
	/// Summary description for ProgressState.
	/// </summary>
	public class ProgressState : IDisposable
	{
		private readonly ProgressDialogHandler _progressHandler;
		private int _numberOfSteps;
		private int _numberOfStepsCompleted;
		private string _status;

		private bool _doCancel = false;

		public ProgressState(ProgressDialogHandler _progressHandler)
		{
			this._progressHandler = _progressHandler;
			//System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
			_numberOfStepsCompleted = 0;
			if (_progressHandler != null)
			{
				_progressHandler.Cancelled += new EventHandler(_progressHandler_Cancelled);
			}
		}

		void _progressHandler_Cancelled(object sender, EventArgs e)
		{
			_doCancel = true;
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
				_progressHandler.UpdateProgress(_numberOfStepsCompleted);
			}
		}

		/// <summary>
		/// a label which describes what we are busy doing
		/// </summary>
		public virtual string Status
		{
			get
			{
				return _status;
			}

			set
			{
				_status = value;
				_progressHandler.UpdateStatus1(_status);
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


		public virtual int NumberOfSteps
		{
			get
			{
				return _numberOfSteps;
			}
			set
			{
				_numberOfSteps = value;
			  _progressHandler.InitializeProgress(0, value);
				//_initializeCallback(0, value);
			}
		}
/*
		#region Stuff for Matthew Adams' dialog box

		protected InitializeProgressCallback _initializeCallback;
		protected ProgressCallback _progressCallback;
		protected StatusCallback _primaryStatusTextCallback;
		protected StatusCallback _secondaryStatusTextCallback;

		public InitializeProgressCallback InitializeCallback
		{
			set { _initializeCallback = value; }
		}

		public ProgressCallback ProgressCallback
		{
			set { _progressCallback = value; }
		}

		public StatusCallback PrimaryStatusTextCallback
		{
			set { _primaryStatusTextCallback = value; }
		}

		public StatusCallback SecondaryStatusTextCallback
		{
			set { _secondaryStatusTextCallback = value; }
		}
		#endregion
*/
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
			_status = null;

			_isDisposed = true;
		}

		#endregion IDisposable & Co. implementation



	}
}
