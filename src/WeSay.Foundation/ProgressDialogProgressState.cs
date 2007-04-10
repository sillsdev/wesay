using System;
using MultithreadProgress;
using WeSay.Foundation.Progress;

namespace WeSay.Foundation
{
	public class ProgressDialogProgressState : ProgressState
	{
		private readonly ProgressDialogHandler _progressHandler;

		public ProgressDialogProgressState(ProgressDialogHandler _progressHandler): base()
		{
			this._progressHandler = _progressHandler;
			//System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
			if (_progressHandler != null)
			{
				_progressHandler.Cancelled += new EventHandler(_progressHandler_Cancelled);
			}
		}


		void _progressHandler_Cancelled(object sender, EventArgs e)
		{
			Cancel = true;
		}

		/// <summary>
		/// How much the task is done
		/// </summary>
		public override int NumberOfStepsCompleted
		{
			get
			{
				return base.NumberOfStepsCompleted;
			}
			set
			{
				base.NumberOfStepsCompleted = value;
				_progressHandler.UpdateProgress(NumberOfStepsCompleted);
			}
		}

		/// <summary>
		/// a label which describes what we are busy doing
		/// </summary>
		public override string StatusLabel
		{
			get
			{
				return base.StatusLabel;
			}

			set
			{
				base.StatusLabel = value;
				_progressHandler.UpdateStatus1(value);
			}
		}

		public override int NumberOfSteps
		{
			get
			{
				return base.NumberOfSteps;
			}
			set
			{
				base.NumberOfSteps = value;
				_progressHandler.InitializeProgress(0, value);
			}
		}

	}

}
