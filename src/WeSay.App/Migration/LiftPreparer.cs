using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using Palaso.Progress;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.Progress;
using WeSay.Project;

namespace WeSay.App
{
	internal class LiftPreparer
	{
		private WeSayWordsProject _project;
		public LiftPreparer(WeSayWordsProject project)
		{
			_project = project;
		}

		public bool MakeCacheAndLiftReady()
		{
			//NB: it's very important that any updates are consumed before the cache is rebuilt.
			//Otherwise, the cache and lift will fall out of sync.
			if (!LiftUpdateService.ConsumePendingLiftUpdates())
			{
				return false;
			}

			if (!BringCachesUpToDate())
			{
				return false;
			}
			if (CacheManager.AssumeCacheIsFresh(_project.PathToCache))
			{
				//prevent the update service from thinking the LIFT file is really old
				//compared to the cache, due to the installer messing with the dates.
				LiftUpdateService.LiftIsFreshNow();
			}
			//whether or not we're out of date, remove this indicator file, which is only to get
			//fresh-from-install launchign without an installer-induced
			//false dirty cache signal
			CacheManager.RemoveAssumeCacheIsFreshIndicator();

			return true;
		}

		private bool PreprocessLift()
		{
			using (ProgressDialog dlg = new ProgressDialog())
			{
				dlg.Overview = "Please wait while WeSay preprocesses your LIFT file.";
				BackgroundWorker preprocessWorker = new BackgroundWorker();
				preprocessWorker.DoWork += new DoWorkEventHandler(OnDoPreprocessLiftWork);
				dlg.BackgroundWorker = preprocessWorker;
				dlg.CanCancel = true;
				dlg.ShowDialog();
				if (dlg.ProgressStateResult.ExceptionThatWasEncountered != null)
				{
					ErrorReport.ReportNonFatalMessage(
						String.Format("WeSay encountered an error while preprocessing the file '{0}'.  Error was: {1}",
						_project.PathToLiftFile,
						dlg.ProgressStateResult.ExceptionThatWasEncountered.Message));
				}
				return (dlg.DialogResult == DialogResult.OK);
			}
		}

		private void OnDoPreprocessLiftWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorkerState state = (BackgroundWorkerState)e.Argument;
			state.StatusLabel = "Preprocessing...";
			try
			{
				_project.ReleaseLockOnLift();
				string pathToLift = _project.PathToLiftFile;
				string outputPath = LiftIO.Utilities.ProcessLiftForLaterMerging(pathToLift);
				//    int liftProducerVersion = GetLiftProducerVersion(pathToLift);

				outputPath = PopulateDefinitions(outputPath);
				MoveTempOverRealAndBackup(pathToLift, outputPath);
			}
			catch (Exception error)
			{
				state.ExceptionThatWasEncountered = error;
				state.State = ProgressState.StateValue.StoppedWithError;
				throw; // this will put the exception in the e.Error arg of the RunWorkerCompletedEventArgs
			}
			finally
			{
				_project.LockLift();
			}
		}

		internal string PopulateDefinitions(string pathToLift)
		{
			string outputPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineOnAttributes = true;

			using (Stream xsltStream =
								Assembly.GetExecutingAssembly().GetManifestResourceStream("WeSay.App.Migration.populateDefinitionFromGloss.xslt"))
			{
				TransformWithProgressDialog transformer =
					new TransformWithProgressDialog(pathToLift, outputPath, xsltStream, "//sense");
				transformer.TaskMessage = "Populating Definitions from Glosses";
				transformer.Transform(true);
				return outputPath;
			}
		}


		//        private int GetLiftProducerVersion(string pathToLift)
		//        {
		//            string s = FindFirstInstanceOfPatternInFile(pathToLift, "producer=\"()\"");
		//        }


		private static string FindFirstInstanceOfPatternInFile(string inputPath, string pattern)
		{
			Regex regex = new Regex(pattern);
			using (StreamReader reader = File.OpenText(inputPath))
			{
				while (!reader.EndOfStream)
				{
					Match m = regex.Match(reader.ReadLine());
					if (m != null)
					{
						return m.Value;
					}
				}
				reader.Close();
			}
			return string.Empty;
		}


		private static void MoveTempOverRealAndBackup(string existingPath, string newFilePath)
		{
			string backupName = existingPath + ".old";

			try
			{
				if (File.Exists(backupName))
				{
					File.Delete(backupName);
				}

				File.Move(existingPath, backupName);
			}
			catch
			{
				Logger.WriteEvent(String.Format("Couldn't write out to {0} ", backupName));
			}

			File.Copy(newFilePath, existingPath);
			File.Delete(newFilePath);
		}



		private bool BringCachesUpToDate()
		{
			Debug.Assert(!string.IsNullOrEmpty(_project.PathToLiftFile));
			CacheBuilder builder = CacheManager.GetCacheBuilderIfNeeded(_project);

			if (builder == null)
			{
				return true;
			}

			//ProgressState progressState = new WeSay.Foundation.ConsoleProgress();//new ProgressState(progressDialogHandler);
			using (ProgressDialog dlg = new ProgressDialog())
			{
				if (!PreprocessLift())
				{
					return false;
				}

				dlg.Overview = "Please wait while WeSay updates its caches to match the new or modified LIFT file.";
				BackgroundWorker cacheBuildingWork = new BackgroundWorker();
				cacheBuildingWork.DoWork += new DoWorkEventHandler(builder.OnDoWork);
				dlg.BackgroundWorker = cacheBuildingWork;
				dlg.CanCancel = true;
				dlg.ShowDialog();
				if (dlg.DialogResult != DialogResult.OK)
				{
					Exception err = dlg.ProgressStateResult.ExceptionThatWasEncountered;
					if (err != null)
					{
						if (err is LiftIO.LiftFormatException)
						{
							Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
								"WeSay had problems with the content of the dictionary file.\r\n\r\n" + err.Message);
						}
						else
						{
							ErrorNotificationDialog.ReportException(err, null, false);
						}
					}
					else if (dlg.ProgressStateResult.State == ProgressState.StateValue.StoppedWithError)
					{
						ErrorReport.ReportNonFatalMessage("Could not build caches. " + dlg.ProgressStateResult.LogString, null, false);
					}
					return false;
				}
				LiftUpdateService.LiftIsFreshNow();
			}
			return true;
		}
	}

}
