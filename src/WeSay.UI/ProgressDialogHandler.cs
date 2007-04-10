using System;
using System.Windows.Forms;
using MultithreadProgress;
using WeSay;
using WeSay.Foundation;

namespace WeSay.UI
{
	public class ProgressDialogHandler
	{
		private ProgressDialog _progressDialog;
		private System.Windows.Forms.Form _parentForm;
		private AsyncCommand _currentCommand;
		private bool _parentFormIsClosing = false;
		public event EventHandler Finished;

		public ProgressDialogHandler(System.Windows.Forms.Form parentForm, BasicCommand  command)
		{
			_parentForm = parentForm;
			_currentCommand = command;
			command.InitializeCallback = new InitializeProgressCallback(InitializeProgress);
			command.ProgressCallback = new ProgressCallback(UpdateProgress);
			command.PrimaryStatusTextCallback = new StatusCallback(UpdateStatus1);
			command.SecondaryStatusTextCallback = new StatusCallback(UpdateStatus2);

			_currentCommand.BeginCancel += new EventHandler(OnCommand_BeginCancel);
			_currentCommand.EnabledChanged += new EventHandler(OnCommand_EnabledChanged);
			_currentCommand.Error += new MultithreadProgress.ErrorEventHandler(OnCommand_Error);
			_currentCommand.Finish += new EventHandler(OnCommand_Finish);

			_progressDialog = new ProgressDialog();
			_progressDialog.CancelRequested += new EventHandler(_progressDialog_Cancelled);
			_progressDialog.Owner = parentForm ;
			_progressDialog.CanCancel = true;
			//we don't yet have any actual background-safe stuff, but this dialog
			//doesn't seem to work (no progress) if it's called modally
			//_progressDialog.ShowDialog();
			//_progressDialog.DelayShow() <-- this one makes it come up only if the command turns out to be slow
			_progressDialog.Show();

		}

		public bool ParentFormIsClosing
		{
			get { return _parentFormIsClosing; }
			set { _parentFormIsClosing = value; }
		}

		public void InitializeProgress(int minimum, int maximum)
		{

			if (NeedInvoke())
			{
				_parentForm.BeginInvoke(
					new InitializeProgressCallback(InitializeProgressCore),
					new object[] { minimum, maximum });
			}
			else
			{
				InitializeProgressCore(minimum, maximum);
			}
		}

		public void UpdateProgress(int progress)
		{
			if (NeedInvoke())
			{
				_parentForm.BeginInvoke(
					new ProgressCallback(UpdateProgressCore),
					new object[] { progress });
			}
			else
			{
				UpdateProgressCore(progress);
			}
		}

		public void UpdateStatus1(string statusText)
		{
			if (_parentForm == null)
				return;

			if (NeedInvoke())
			{
				_parentForm.BeginInvoke(
					new StatusCallback(UpdateStatus1Core),
					new object[] { statusText });
			}
			else
			{
				UpdateStatus1Core(statusText);
			}
		}

		private bool NeedInvoke()
		{
			return _progressDialog.InvokeRequired;
		   //  return _parentForm != null && _parentForm.InvokeRequired;
	   }

		private void UpdateStatus2(string statusText)
		{
			if (NeedInvoke())
			{
				_parentForm.BeginInvoke(
					new StatusCallback(UpdateStatus2Core),
					new object[] { statusText });
			}
			else
			{
				UpdateStatus2Core(statusText);
			}
		}


		private void InitializeProgressCore(int minimum, int maximum)
		{
			_progressDialog.ProgressRangeMinimum = minimum;
			_progressDialog.ProgressRangeMaximum = maximum;
		}

		private void UpdateProgressCore(int progress)
		{
			_progressDialog.Progress = progress;
		}

		private void UpdateStatus1Core(string statusText)
		{
			_progressDialog.StatusText1 = statusText;
		}

		private void UpdateStatus2Core(string statusText)
		{
			_progressDialog.StatusText2 = statusText;
		}


		private void Finish()
		{
			_progressDialog.ForceClose();
			_progressDialog = null;
			if (Finished != null)
			{
				Finished.BeginInvoke(this, null, null, null);//jh changed this from Invoke()
			}

			if (ParentFormIsClosing)
			{
				_parentForm.Close();
			}
			else
			{
				_currentCommand.Enabled = true;
			}
		}

		private void Finish(Exception e)
		{
			MessageBox.Show(e.ToString());
			Finish();
		}

		private void FinishWithUnspecifiedError()
		{
			MessageBox.Show("An error occurred while processing your request.");
			Finish();
		}

		/// <summary>
		/// A delegate for a method which takes an exception as its only parmeter
		/// </summary>
		public delegate void ExceptionMethodInvoker(Exception e);
		public event EventHandler Cancelled;

		private void _progressDialog_Cancelled(object sender, EventArgs e)
		{
			if (Cancelled != null)
			{
				Cancelled.Invoke(this, null);//REVIEW jh wesay
			}
			_currentCommand.Cancel();
		}

		public void Close()
		{
			_progressDialog.Close();
		}



		private void OnCommand_BeginCancel(object sender, EventArgs e)
		{
			Close();
		}

		private void OnCommand_EnabledChanged(object sender, EventArgs e)
		{
			// button1.Enabled = _currentCommand.Enabled;
		}

		private void OnCommand_Error(object sender, MultithreadProgress.ErrorEventArgs e)
		{
			if (e.Exception == null)
			{
				if (NeedInvoke())
				{
					_parentForm.BeginInvoke(new MethodInvoker(FinishWithUnspecifiedError));
				}
				else
				{
					FinishWithUnspecifiedError();
				}
			}
			else
			{

				if (_parentForm != null && NeedInvoke())
				{
					 _parentForm.BeginInvoke(new ProgressDialogHandler.ExceptionMethodInvoker(Finish), new object[] { e.Exception });
				}
				else
				{
					Finish(e.Exception);
				}
			}
		}

		private void OnCommand_Finish(object sender, EventArgs e)
		{
			if (NeedInvoke())
			{
				_parentForm.BeginInvoke(new MethodInvoker(Finish));
			}
			else
			{
				Finish();
			}
		}

		public void CloseByCancellingThenCloseParent()
		{
			_parentFormIsClosing = true;
			Close();
		}
	}
}