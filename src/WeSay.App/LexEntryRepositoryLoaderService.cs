using System;
using System.ComponentModel;
using System.Windows.Forms;
using Palaso.Progress;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.Progress;
using WeSay.LexicalModel;

namespace WeSay.App
{
	public class LexEntryRepositoryLoaderService
	{
		private string _pathToLift;
		private LexEntryRepository _lexEntryRepository;

		public LexEntryRepositoryLoaderService(string pathToLift)
		{
			_pathToLift = pathToLift;
		}

		/// <summary>
		/// Get the LexEntryRepository, loading it with a progress UI, if needed
		/// </summary>
		/// <returns></returns>
		public LexEntryRepository GetLexEntryRepository()
		{
			if(_lexEntryRepository == null)
				Load();
			return _lexEntryRepository;
		}        //this was the orginal way to use this.... JH added the other stuff testing out an load-only-if-needed approach for the config app        static public LexEntryRepository CreateLexEntryRepository(string pathToLift)        {
			LexEntryRepositoryLoaderService x = new LexEntryRepositoryLoaderService(pathToLift);
			return x.GetLexEntryRepository();
		}        private void Load()        {
			using (ProgressDialog dlg = new ProgressDialog())
			{
				dlg.Overview = "Please wait while WeSay loads your data.";
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += delegate (object sender, DoWorkEventArgs args)
									 {
										 ProgressState progressState = (ProgressState) args.Argument;
										 try
										 {

											 _lexEntryRepository = new LexEntryRepository(_pathToLift, progressState);
											 args.Result = _lexEntryRepository;

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
			}
		}
	}
}