using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.Project;
using Debug=System.Diagnostics.Debug;

namespace WeSay.App
{
	public class LiftUpdateService
	{
		private const string s_updatePointFileName = "updatePoint";
		private const int _checkFrequency = 10;
		private int _commitCount;
		private Db4oDataSource _datasource;
		private DateTime _timeOfLastQueryForNewRecords;

		event EventHandler Updating;

		public LiftUpdateService(Db4oDataSource datasource)
		{
			_datasource = datasource;
		}



		private static string LiftDirectory
		{
			get
			{
				return Path.GetDirectoryName(Project.WeSayWordsProject.Project.PathToLiftFile);
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

			LiftExporter exporter = new LiftExporter(WeSayWordsProject.Project.GetFieldToOptionListNameDictionary(),
				MakeIncrementFileName(DateTime.UtcNow));
			exporter.AddDeletedEntry(entry);
			exporter.End();
		}



		public void DoLiftUpdateNow(bool mergeIntoSingleFileBeforeReturning)
		{
			Reporting.Logger.WriteEvent("Incremental Update Start");

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
						new LiftExporter(WeSayWordsProject.Project.GetFieldToOptionListNameDictionary(),
										 MakeIncrementFileName(_timeOfLastQueryForNewRecords));
					exporter.AddNoGeneric(records);
					exporter.End();

					WriteUpdatePointFile(_timeOfLastQueryForNewRecords);
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
				Reporting.Logger.WriteEvent("Running Synchronic Merger");
				//merge the increment files

				try
				{
					WeSayWordsProject.Project.ReleaseLockOnLift();
					LiftIO.SynchronicMerger merger = new LiftIO.SynchronicMerger();
					merger.MergeUpdatesIntoFile(Project.WeSayWordsProject.Project.PathToLiftFile);
					CacheManager.UpdateSyncPointInCache(_datasource.Data,
														File.GetLastWriteTimeUtc(
															WeSayWordsProject.Project.PathToLiftFile));
				}
				finally
				{
					WeSayWordsProject.Project.LockLift();
				}
			}

			Reporting.Logger.WriteEvent("Incremental Update Done");

			//the granularity of the file access time stamp is too blunt, so we
			//avoid missing changes with this hack, for now (have *not* tested how small it could be)
			Thread.Sleep(50);


		}



		private  string MakeIncrementFileName(DateTime time)
		{
//            if (File.Exists(LiftIO.SynchronicMerger.BaseLiftFileName))
//            {
				string timeString = time.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'-'FFFFF UTC");
				string path = Path.Combine(LiftDirectory, timeString);
				path += LiftIO.SynchronicMerger.ExtensionOfIncrementalFiles;
				return path;
//            }
//            else
//            {
//                return PathToBaseLiftFile;
//            }
		}

//        public string PathToBaseLiftFile
//        {
//            get
//            {
//                return Path.Combine(LiftDirectory, LiftIO.SynchronicMerger.BaseLiftFileName);
//            }
//        }

		/// <summary>
		/// wierd name!
		/// </summary>
		public static void LiftIsFreshNow()
		{
			WriteUpdatePointFile(DateTime.UtcNow);
		}

		protected static void WriteUpdatePointFile(DateTime time)
		{
			string file = Path.Combine(LiftDirectory, s_updatePointFileName);
			if (!File.Exists(file))
			{
				File.Create(file).Close();
			}

			File.SetLastWriteTimeUtc(file, time);
		}

		private  DateTime GetLastUpdateTime()
		{
			Debug.Assert(Directory.Exists(LiftDirectory));
			string file = Path.Combine(LiftDirectory, s_updatePointFileName);
			if (!File.Exists(file))
			{
				return DateTime.MinValue;
			}
			else
			{
				return File.GetLastWriteTimeUtc(file);
			}
		}

		public IList  GetRecordsNeedingUpdateInLift()
		{
			DateTime last = GetLastUpdateTime();
			IQuery q =this._datasource.Data.Query();
			q.Constrain(typeof(LexEntry));
			//REVIEW: this is >, not >=. Could a change get lost if the
			//record was modified milliseconds before the last update?
			q.Descend("_modificationTime").Constrain(last).Greater();
			_timeOfLastQueryForNewRecords = DateTime.UtcNow;
			return q.Execute();
		}
	}
}