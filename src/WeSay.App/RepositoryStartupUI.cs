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
										 ProgressState progressState = (ProgressState) args.Argument;
										 try
										 {

											 lexEntryRepository = new LexEntryRepository(path, progressState);
											 args.Result = lexEntryRepository;

										 }
										 catch(Exception error)
										 {
											 args.Cancel = true;//review
											 args.Result = error;
											 progressState.ExceptionThatWasEncountered = error;
										 }
									 };
				dlg.BackgroundWorker = worker;
				dlg.CanCancel = false;

				dlg.ShowDialog();
				if (dlg.DialogResult != DialogResult.OK)
				{
					Exception err = dlg.ProgressStateResult.ExceptionThatWasEncountered;
					if (err != null)
					{
						throw err;
					}
					else if (dlg.ProgressStateResult.State ==
							 ProgressState.StateValue.StoppedWithError)
					{
						throw new ApplicationException("Failure while creating repository.");
					}
				}
				return lexEntryRepository;
			}
		}
	}
}