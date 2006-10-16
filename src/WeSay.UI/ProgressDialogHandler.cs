using System;
using System.Windows.Forms;
using WeSay;

namespace MultithreadProgress
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

			_currentCommand.BeginCancel += new EventHandler(OnExportCommand_BeginCancel);
			_currentCommand.EnabledChanged += new EventHandler(OnExportCommand_EnabledChanged);
			_currentCommand.Error += new MultithreadProgress.ErrorEventHandler(OnExportCommand_Error);
			_currentCommand.Finish += new EventHandler(OnExportCommand_Finish);

			_progressDialog = new ProgressDialog();
			_progressDialog.Cancelled += new EventHandler(_progressDialog_Cancelled);
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

		private void InitializeProgress(int minimum, int maximum)
		{

			if (_parentForm.InvokeRequired)
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

		private void UpdateProgress(int progress)
		{
			if (_parentForm.InvokeRequired)
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

		private void UpdateStatus1(string statusText)
		{
			if (_parentForm.InvokeRequired)
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

		private void UpdateStatus2(string statusText)
		{
			if (_parentForm.InvokeRequired)
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
				Finished.Invoke(this, null);
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

		private void _progressDialog_Cancelled(object sender, EventArgs e)
		{
			_currentCommand.Cancel();
		}

		public void Close()
		{
			_progressDialog.Close();
		}



		private void OnExportCommand_BeginCancel(object sender, EventArgs e)
		{
			Close();
		}

		private void OnExportCommand_EnabledChanged(object sender, EventArgs e)
		{
			// button1.Enabled = _currentCommand.Enabled;
		}

		private void OnExportCommand_Error(object sender, MultithreadProgress.ErrorEventArgs e)
		{
			if (e.Exception == null)
			{
				if (_parentForm.InvokeRequired)
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
				if (_parentForm.InvokeRequired)
				{
					_parentForm.BeginInvoke(new ProgressDialogHandler.ExceptionMethodInvoker(Finish), new object[] { e.Exception });
				}
				else
				{
					Finish(e.Exception);
				}
			}
		}

		private void OnExportCommand_Finish(object sender, EventArgs e)
		{
			if (_parentForm.InvokeRequired)
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