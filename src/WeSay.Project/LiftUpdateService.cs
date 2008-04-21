using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Db4objects.Db4o.Query;
using LiftIO.Merging;
using Palaso.Reporting;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;


/*  Analysis of what happens to the data when there is a crash
 *              (Note, you can simulate crash by doing shift-pause from a WeSayTextBox)
 *
 *  Analysis of what happens to the data when there is a crash
 *
 * DB Committed?    Lift Update File Written?   Update File Consumed yet?   Result
 * -------------    -------------------------   -------------------------   ------
 * N                N                           N                           No data to lose.
 * N                Y                           N                           On restart, we must consume update file first, then rebuild cache (see ws-281)
 * N                Y                           Y                           Cache will rebuild with correct data on restart
 * Y                N                           N                           On restart, RecoverUnsavedChangesOutOfCacheIfNeeded() finds newer changes than LIFT has, create update and consume it
 * Y                Y                           N                           On restart, will consume update file.
 * Y                Y                           Y                           No data to lose.
 * Y                corrupt, but exporter threw exception                   ? probably retain data lift.update.bad file may also contain info, if the exporter actually wrote out a half-finished update file
 * Y                corrupt, but exporter ended normally                    WOULD LOOSE DATA
 *
 */
namespace WeSay.Project
{
	public class LiftUpdateService
	{
		//private const string s_updatePointFileName = "updatePoint";
		private const int _checkFrequency = 10;
		private int _commitCount;
		private Db4oDataSource _datasource;
		private DateTime _timeOfLastQueryForNewRecords;
		//private bool _didFindDataInCacheNeedingRecovery = false;

		event EventHandler Updating;

		public LiftUpdateService(Db4oDataSource datasource)
		{
			_datasource = datasource;
		}



		private static string LiftDirectory
		{
			get
			{
				return Path.GetDirectoryName(WeSayWordsProject.Project.PathToLiftFile);
			}
		}

		/// <summary>
		/// Give a chance to do incremental update if warranted
		/// </summary>
		/// <remarks>
		/// If the caller doesn't know when actual comitts happen, that's ok.
		/// Just call at reasonable intervals.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnDataCommitted(object sender, EventArgs e)
		{
			_commitCount++;
			if (_commitCount < _checkFrequency)
			{
				return;
			}
			_commitCount = 0;
			DoLiftUpdateNow(false);
		}

		public void OnDataDeleted(object sender, DeletedItemEventArgs e)
		{
			LexEntry entry = e.ItemDeleted as LexEntry;
			if (entry == null)
			{
				return;
			}

			LiftExporter exporter = new LiftExporter(/*WeSayWordsProject.Project.GetFieldToOptionListNameDictionary(), */
				MakeIncrementFileName(DateTime.UtcNow));
			exporter.AddDeletedEntry(entry);
			exporter.End();
		}



		public void DoLiftUpdateNow(bool mergeIntoSingleFileBeforeReturning)
		{
		   // Logger.WriteEvent("Incremental Update Start");

			if (Updating != null)
			{
				Updating.Invoke(this, null);
			}

			IList records = GetRecordsNeedingUpdateInLift();
			if (records.Count != 0)
			{
				LameProgressDialog dlg = null;
				if (records.Count > 50)
				{
					//TODO: if we think this will actually ever be called, clean this up with a real, delayed-visibility dialog
					dlg = new LameProgressDialog(string.Format("Doing incremental update of {0} records...", records.Count));
					dlg.Show();
					Application.DoEvents();
				}

				try
				{
					LiftExporter exporter =
						new LiftExporter(/*WeSayWordsProject.Project.GetFieldToOptionListNameDictionary(),*/
										 MakeIncrementFileName(_timeOfLastQueryForNewRecords));
					exporter.AddNoGeneric(records);
					exporter.End();

					RecordUpdateTime(_timeOfLastQueryForNewRecords);
				}
				finally
				{
					if (dlg != null)
					{
						dlg.Close();
						dlg.Dispose();
					}
				}
			}

			if (mergeIntoSingleFileBeforeReturning)
			{
				if (ConsumePendingLiftUpdates())
				{
					CacheManager.UpdateSyncPointInCache(_datasource.Data,
															File.GetLastWriteTimeUtc(
																WeSayWordsProject.Project.PathToLiftFile));
				}
			}

			//Logger.WriteEvent("Incremental Update Done");
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>false if it failed (and it would have already reported the error)</returns>
		public static bool ConsumePendingLiftUpdates()
		{
			//merge the increment files

			if (SynchronicMerger.GetPendingUpdateFiles(WeSayWordsProject.Project.PathToLiftFile).Length > 0)
			{
				Logger.WriteEvent("Running Synchronic Merger");
				try
				{
					SynchronicMerger merger = new SynchronicMerger();
					WeSayWordsProject.Project.ReleaseLockOnLift();
					merger.MergeUpdatesIntoFile(WeSayWordsProject.Project.PathToLiftFile);
				}
				catch (LiftIO.BadUpdateFileException error)
				{
					string contents = File.ReadAllText(error.PathToNewFile);
					if (contents.Trim().Length == 0)
					{
					   Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
							"It looks as though WeSay recently crashed while attempting to save.  It will try again to preserve your work, but you will want to check to make sure nothing was lost.");
						File.Delete(error.PathToNewFile);
					}
					else
					{
						File.Move(error.PathToNewFile, error.PathToNewFile + ".bad");
						Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
							"WeSay was unable to save some work you did in the previous session.  The work might be recoverable from the file {0}. The next screen will allow you to send a report of this to the developers.", error.PathToNewFile + ".bad");
						Palaso.Reporting.ErrorNotificationDialog.ReportException(error, null, false);
					}
					return false;
				}
				catch (Exception e)
				{
					throw new ApplicationException("Could not finish updating LIFT dictionary file.", e);
				}
				finally
				{
					WeSayWordsProject.Project.LockLift();
				}
			}
		   return true;
		}


		private static string MakeIncrementFileName(DateTime time)
		{
			while(true){
				string timeString = time.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'-'FFFFFFF UTC");
				string path = Path.Combine(LiftDirectory, timeString);
				path += SynchronicMerger.ExtensionOfIncrementalFiles;
				if(!File.Exists(path))
				{
					return path;
				}
				time = time.AddTicks(1);
			}
		}

		/// <summary>
		/// wierd name!
		/// </summary>
		public static void LiftIsFreshNow()
		{
			RecordUpdateTime(DateTime.UtcNow);
		}

		protected static void RecordUpdateTime(DateTime time)
		{
			//// the resolution of the file modified time is a whole second on linux
			//// so we need to set this to the ceiling of the time in seconds and then
			//// wait until the actual time has passed this window
			//int millisecondsLostInResolution = 1000 - time.Millisecond;
			//time = time.AddMilliseconds(millisecondsLostInResolution);
			//TimeSpan timeout = time - DateTime.UtcNow;
			//if(timeout.Ticks > 0)
			//{
			//    Thread.Sleep(timeout);
			//}
			bool wasLocked = WeSayWordsProject.Project.LiftIsLocked;
			if (wasLocked)
			{
				WeSayWordsProject.Project.ReleaseLockOnLift();
			}
			File.SetLastWriteTimeUtc(WeSayWordsProject.Project.PathToLiftFile, time);
			//Debug.Assert(time == GetLastUpdateTime());
			if (wasLocked)
			{
				WeSayWordsProject.Project.LockLift();
			}
		}

		private static DateTime GetLastUpdateTime()
		{
			Debug.Assert(Directory.Exists(LiftDirectory));
			return File.GetLastWriteTimeUtc(WeSayWordsProject.Project.PathToLiftFile);
		}

		public IList  GetRecordsNeedingUpdateInLift()
		{
			// by moving back 1 milliseconds, we ensure that we
			// will get the correct records with just a > and not >= (see note below)
			DateTime last = GetLastUpdateTime().AddMilliseconds(-1);
			IQuery q =this._datasource.Data.Query();
			q.Constrain(typeof(LexEntry));
			//REVIEW: this is >, not >=. Could a change get lost if the
			//record was modified milliseconds before the last update?
			q.Descend("_modificationTime").Constrain(last).Greater();
			_timeOfLastQueryForNewRecords = DateTime.UtcNow;
			return q.Execute();
		}

		/// <summary>
		/// Used to try again to get data out of the cache in case it crashed last time
		/// We can be successful at saving on startup if the crash was somehow related to the UI (as in WS-554)
		/// </summary>
		public void RecoverUnsavedChangesOutOfCacheIfNeeded()
		{
			try
			{
				if (CacheManager.GetAssumeCacheIsFresh(Project.WeSayWordsProject.Project.PathToCache))
				{
					return;// setting permissions in the installer apparently was enough to mess this next line up on the sample data
				}

				IList records = GetRecordsNeedingUpdateInLift();
				if (records.Count == 0)
				{
					return;
				}

				try
				{
					Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
						"It appears that WeSay did not exit normally last time.  WeSay will now attempt to recover the {0} records which were not saved.",
						records.Count);
					DoLiftUpdateNow(false);
//                    _didFindDataInCacheNeedingRecovery = true;
					Palaso.Reporting.ErrorReport.ReportNonFatalMessage("Your work was successfully recovered.");
				}
				catch (Exception)
				{
					Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
						"Sorry, WeSay was unable to recover some of your work.");
					Project.WeSayWordsProject.Project.InvalidateCacheSilently();
				}
			}
			catch (Exception)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
					"WeSay had a problem reading the cache.  It will now be rebuilt");
				Project.WeSayWordsProject.Project.InvalidateCacheSilently();
			}
		}
	}
}
