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
		static public LexEntryRepository CreateLexEntryRepository(string path)
		{
			LexEntryRepository lexEntryRepository = null;
			using (ProgressDialog dlg = new ProgressDialog())
			{
				dlg.Overview = "Please wait while WeSay loads your data.";
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += delegate (object sender, DoWorkEventArgs args)
								 {
									 ProgressState progressState = (ProgressState)args.Argument;
									 lexEntryRepository = new LexEntryRepository(path, progressState);
								 };
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
								"Failed." + dlg.ProgressStateResult.LogString, null, false);
					}
				}
				return lexEntryRepository;
			}
		}
	}
}