using System;
using System.Runtime.Remoting.Messaging;
using MultithreadProgress;
//using WeSay.Data;
//using WeSay.LexicalModel;

namespace WeSay
{
	public abstract class BasicCommand : AsyncCommand
	{
		protected InitializeProgressCallback _initializeCallback;
		protected ProgressCallback _progressCallback;
		protected StatusCallback _primaryStatusTextCallback;
		protected   StatusCallback _secondaryStatusTextCallback;

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

		/// <summary>
		/// Implementation of the async work invoker
		/// </summary>
		/// <remarks>
		/// We're using the delegate BeginInvoke() / EndInvoke() pattern here
		/// </remarks>
		protected override void BeginInvokeCore()
		{
			WorkInvoker worker = new WorkInvoker( DoWork );
			worker.BeginInvoke(
				_initializeCallback,
				_progressCallback ,
				_primaryStatusTextCallback ,
				_secondaryStatusTextCallback,
				new AsyncCallback( EndWork ), null );
		}

		protected abstract void DoWork(
			InitializeProgressCallback initializeCallback,
			ProgressCallback progressCallback,
			StatusCallback primaryStatusTextCallback,
			StatusCallback secondaryStatusTextCallback
			);

		private void EndWork( IAsyncResult result )
		{
			AsyncResult asyncResult = (AsyncResult)result;
			WorkInvoker asyncDelegate = (WorkInvoker)asyncResult.AsyncDelegate;
			try
			{
				asyncDelegate.EndInvoke( result );
				OnFinish( EventArgs.Empty );
			}
			catch( Exception e )
			{
				// Marshal exceptions back to the UI
				OnError( new ErrorEventArgs( e ) );
			}
			catch
			{
				// Do our exception handling; include a default catch
				// block because this is the final handler on the stack for this
				// thread, and we need to log these kinds of problems
				OnError( new ErrorEventArgs( null ) );
			}
		}
	}
}