using System;
using System.IO;
using System.Windows.Forms;
using Palaso.Progress;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay
{
	 public class ExportLIFTCommand : BasicCommand
	{
		protected string _destinationLIFTPath;
		protected string _sourceWordsPath;
		 protected ProgressState _progress;

		public ExportLIFTCommand(
			string destinationLIFTPath, string sourceWordsPath        )
		{
			_destinationLIFTPath = destinationLIFTPath;
			_sourceWordsPath = sourceWordsPath;
		}
		 protected override void DoWork2(ProgressState progress)
		 {
			 _progress = progress;
			 _progress.StatusLabel="Exporting...";
			 _progress.State = ProgressState.StateValue.Busy;
			 WeSay.LexicalModel.LiftExporter exporter = null;
			 try
			 {
				 exporter = new LiftExporter(/*WeSayWordsProject.Project.GetFieldToOptionListNameDictionary(),*/ _destinationLIFTPath);

				 if(!VerifyWeCanOpenTheFile())
				 {
					 return;
				 }
				 using (Db4oDataSource ds = new Db4oDataSource(_sourceWordsPath))
				 {
				   Db4oLexModelHelper.Initialize(ds.Data);

					 using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
					 {
						 _progress.TotalNumberOfSteps = entries.Count;
						 for (int i = 0; i < entries.Count; )
						 {
							 int howManyAtATime = 100;
							 howManyAtATime = Math.Min(100, entries.Count - i);
							 exporter.Add(entries, i, howManyAtATime);
							 i += howManyAtATime;
							 _progress.NumberOfStepsCompleted = i;
							 if (_progress.Cancel)
							 {
								 break; ;
							 }
						 }
					 }
				 }
				 _progress.State = ProgressState.StateValue.Finished;

			 }
			 catch (Exception e)
			 {
				   _progress.WriteToLog(e.Message);
				 _progress.State = ProgressState.StateValue.StoppedWithError;
				 return; //don't go on to try to validate
			 }
			 finally
			 {
				 if (exporter != null)
				 {
					 exporter.End();
				 }
			 }

			 try
			 {
				 _progress.StatusLabel = "Validating...";
				 string errors = LiftIO.Validator.GetAnyValidationErrors(_destinationLIFTPath);
				 if (errors != null && errors != String.Empty)
				 {
					 _progress.WriteToLog(errors);
				 }
			 }
			 catch
			 {
				 _progress.WriteToLog("Could not run validator.");
				 _progress.State = ProgressState.StateValue.StoppedWithError;
			 }
		 }

		 private bool VerifyWeCanOpenTheFile()
		 {
			 if (!File.Exists(_sourceWordsPath))
			 {
				 Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
					 string.Format(
						 "Sorry, {0} cannot find a file which is necessary to perform the export on this project ({1})",
						 Application.ProductName, _sourceWordsPath));
				 return false;
			 }


			 try
			 {
				 using (FileStream fs = File.OpenWrite(_sourceWordsPath))
				 {
					 fs.Close();
				 }
			 }
			 catch
			 {
				 _progress.WriteToLog(String.Format("The Exporter could not open the file. Make sure no other program (e.g. WeSay) has it open. As a last resort, restart your computer. ({0})", _sourceWordsPath));
				 _progress.State = ProgressState.StateValue.StoppedWithError;
				 return false;
			 }
			 return true;
		 }

		 protected override void DoWork(
			InitializeProgressCallback initializeCallback,
			ProgressCallback progressCallback,
			StatusCallback primaryStatusTextCallback,
			StatusCallback secondaryStatusTextCallback
			)
		{
			throw new NotImplementedException();

//            primaryStatusTextCallback.Invoke("Exporting...");
//           WeSay.LexicalModel.LiftExporter exporter=null;
//            try
//            {
//                exporter = new LiftExporter(_destinationLIFTPath);
//
//                using (Db4oDataSource ds = new Db4oDataSource(_sourceWordsPath))
//                {
//                    using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
//                    {
//                        initializeCallback(0, entries.Count);
//                        for (int i = 0; i < entries.Count; )
//                        {
//                            const int howManyAtATime = 100;
//                            exporter.Add(entries, i, howManyAtATime);
//                            i += howManyAtATime;
//                            progressCallback(i);
//                            if( Canceling )
//                            {
//                                return;
//                            }
//                        }
//                    }
//                }
//            }
//            finally
//            {
//                exporter.End();
//            }
		}

	}
}