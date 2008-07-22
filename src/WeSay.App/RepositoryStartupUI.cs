using System;
using System.ComponentModel;
using System.Windows.Forms;

using Palaso.Progress;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.Progress;

using WeSay.LexicalModel;

namespace WeSay.App
{
	internal class RepositoryStartupUI
	{
		private LexEntryRepository _lexEntryRepository;

		public RepositoryStartupUI(LexEntryRepository lexEntryRepository)
		{
			_lexEntryRepository = lexEntryRepository;
		}

		public void RepositoryStartup()
		{
			using (ProgressDialog dlg = new ProgressDialog())
			{
				dlg.Overview =
						"Please wait while WeSay migrates your lift database to the required version.";
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += DoRepositoryStartup;
				dlg.BackgroundWorker = worker;
				dlg.CanCancel = false;

				dlg.ShowDialog();
				if (dlg.DialogResult != DialogResult.OK)
				{
					Exception err = dlg.ProgressStateResult.ExceptionThatWasEncountered;
					if (err != null)
					{
						ErrorNotificationDialog.ReportException(err, null, false);
					}
					else if (dlg.ProgressStateResult.State ==
							 ProgressState.StateValue.StoppedWithError)
					{
						ErrorReport.ReportNonFatalMessage(
								"Failed." + dlg.ProgressStateResult.LogString,
								null,
								false);
					}
				}
			}
		}

		private void DoRepositoryStartup(object sender, DoWorkEventArgs args)
		{
			ProgressState progressState = (ProgressState)args.Argument;
			_lexEntryRepository.Startup(progressState);
		}


	}
}
