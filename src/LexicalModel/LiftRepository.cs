using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LiftIO;
using LiftIO.Merging;
using LiftIO.Parsing;
using LiftIO.Validation;
using Palaso.Progress;
using Palaso.Reporting;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel.Migration;

namespace WeSay.LexicalModel
{
	internal class LiftRepository : MemoryRepository<LexEntry>
	{
		private readonly string _liftFilePath;
		private FileStream _liftFileStreamForLocking;
		private bool _loadingAllEntries;
		public LiftRepository(string filePath)
		{
			_liftFilePath = filePath;
			FileInfo fileInfo = new FileInfo(_liftFilePath);
			//check if file is writeable
			using (FileStream fileStream = fileInfo.OpenWrite())
			{
				fileStream.Close();
			}
			if (!fileInfo.Exists || fileInfo.Length == 0)
			{
				LiftExporter exporter = new LiftExporter(filePath);
				exporter.End();
			}
			LockLift();
			LastModified = DateTime.MinValue;
			LoadAllLexEntries();
		}

		public override void Startup(ProgressState state)
		{
			LiftPreparer preparer = new LiftPreparer(_liftFilePath);
			if (preparer.IsMigrationNeeded())
			{
				preparer.MigrateLiftFile(state);
			}
			preparer.PopulateDefinitions(state);
		}

		public override LexEntry CreateItem()
		{
			LexEntry item = base.CreateItem();
			if (!_loadingAllEntries)
			{
				UpdateLiftFileWithModified(item);
			}
			return item;
		}

		public override void DeleteItem(RepositoryId id)
		{
			LexEntry itemToDelete = GetItem(id);
			UpdateLiftFileWithDeleted(itemToDelete);
			base.DeleteItem(id);
		}

		public override void DeleteItem(LexEntry item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (!objectToIdHashtable.ContainsKey(item))
			{
				throw new ArgumentOutOfRangeException("item");
			}

			UpdateLiftFileWithDeleted(item);
			idToObjectHashtable.Remove(objectToIdHashtable[item]);
			objectToIdHashtable.Remove(item);
			LastModified = PreciseDateTime.UtcNow;
		}

		public override void DeleteAllItems()
		{
			RepositoryId[] idsOfEntriesInRepository = GetAllItems();
			foreach(RepositoryId id in idsOfEntriesInRepository)
			{
				DeleteItem(id);
			}
		}

		public override void SaveItem(LexEntry item)
		{
			base.SaveItem(item);
			UpdateLiftFileWithModified(item);
		}

		public override void SaveItems(IEnumerable<LexEntry> items)
		{
			base.SaveItems(items);
			UpdateLiftFileWithModified(items);
		}

		public override bool CanPersist
		{
			get { return true; }
		}

		private void LoadAllLexEntries()
		{
			_loadingAllEntries = true;
			try
			{
				using (LiftMerger merger = new LiftMerger(this))
				{
					LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence> parser =
							new LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>(
									merger);

					parser.SetTotalNumberSteps += parser_SetTotalNumberSteps;
					parser.SetStepsCompleted += parser_SetStepsCompleted;

					parser.ParsingWarning += parser_ParsingWarning;

					try
					{
						parser.ReadLiftFile(_liftFilePath);
					}
					catch (Exception)
					{
						//our parser failed.  Hopefully, because of bad lift. Validate it now  to
						//see if that's the problem.
						Validator.CheckLiftWithPossibleThrow(_liftFilePath);

						//if it got past that, ok, send along the error the parser encountered.
						throw;
					}
				}
			}
			finally
			{
				_loadingAllEntries = false;
			}
		}

		public override void Dispose()
		{
			UnLockLift();
			base.Dispose();
		}

		//???? Eric added these and I'm not sure why. TA 2008-07-10
		private void parser_ParsingWarning(object sender,
										   LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.ErrorArgs
											   e)
		{
		}

		private void parser_SetStepsCompleted(object sender,
											  LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.
												  ProgressEventArgs e)
		{
		}

		private void parser_SetTotalNumberSteps(object sender,
												LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.
													StepsArgs e)
		{
		}

		//private DateTime _timeOfLastQueryForNewRecords;

		private string LiftDirectory
		{
			get { return Path.GetDirectoryName(_liftFilePath); }
		}

		//Don't think this is needed anymore TA 7-4-2008
		///// <summary>
		///// Give a chance to do incremental update if warranted
		///// </summary>
		///// <remarks>
		///// If the caller doesn't know when actual comitts happen, that's ok.
		///// Just call at reasonable intervals.
		///// </remarks>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//public void OnDataCommitted(object sender, EventArgs e)
		//{
		//    _commitCount++;
		//    if (_commitCount < _checkFrequency)
		//    {
		//        return;
		//    }
		//    _commitCount = 0;
		//    DoLiftUpdateNow(false);
		//}

		private void UpdateLiftFileWithModified(IEnumerable<LexEntry> entriesToUpdate)
		{
			CreateFileContainingModified(entriesToUpdate);
			MergeIncrementFiles();
		}

		private void UpdateLiftFileWithModified(LexEntry entryToUpdate)
		{
			CreateFileContainingModified(entryToUpdate);
			MergeIncrementFiles();
		}

		private void UpdateLiftFileWithDeleted(IEnumerable<LexEntry> entriesToDelete)
		{
			CreateFileContainingDeleted(entriesToDelete);
			MergeIncrementFiles();
		}

		private void UpdateLiftFileWithDeleted(LexEntry entryToDelete)
		{
			CreateFileContainingDeleted(entryToDelete);
			MergeIncrementFiles();
		}

		private void CreateFileContainingModified(LexEntry entryToUpdate)
		{
			LiftExporter exporter = new LiftExporter(MakeIncrementFileName(PreciseDateTime.UtcNow));
			//!!!exporter.Start(); //!!! Would be nice to have this CJP 2008-07-09
			exporter.Add(entryToUpdate);
			exporter.End();

			RecordUpdateTime(PreciseDateTime.UtcNow); //Why do we need to call this??? TA 7-4-2008
		}

		private void CreateFileContainingModified(IEnumerable<LexEntry> entriesToUpdate)
		{
				LiftExporter exporter = null;
				foreach (LexEntry entry in entriesToUpdate)
				{
					if (exporter == null)
					{
						exporter =
							new LiftExporter(MakeIncrementFileName(PreciseDateTime.UtcNow));
						//!!!exporter.Start(); //!!! Would be nice to have this CJP 2008-07-09
					}
					exporter.Add(entry);
				}
				if (exporter != null)
				{
					exporter.End();
				}

				RecordUpdateTime(PreciseDateTime.UtcNow); //Why do we need to call this??? TA 7-4-2008
		}

		private void CreateFileContainingDeleted(LexEntry entryToDelete)
		{
			LiftExporter exporter = new LiftExporter(MakeIncrementFileName(PreciseDateTime.UtcNow));
			//!!!exporter.Start(); //!!! Would be nice to have this CJP 2008-07-09
			exporter.AddDeletedEntry(entryToDelete);
			exporter.End();

			RecordUpdateTime(PreciseDateTime.UtcNow); //Why do we need to call this??? TA 7-4-2008
		}

		private void CreateFileContainingDeleted(IEnumerable<LexEntry> entriesToDelete)
		{
			LiftExporter exporter = null;
			foreach (LexEntry entry in entriesToDelete)
			{
				if (exporter == null)
				{
					exporter =
						new LiftExporter(MakeIncrementFileName(PreciseDateTime.UtcNow));
					//!!!exporter.Start(); //!!! Would be nice to have this CJP 2008-07-09
				}
				exporter.AddDeletedEntry(entry);
			}
			if (exporter != null)
			{
				exporter.End();
			}

			RecordUpdateTime(PreciseDateTime.UtcNow); //Why do we need to call this??? TA 7-4-2008
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>false if it failed (and it would have already reported the error)</returns>
		public bool MergeIncrementFiles()
		{
			//merge the increment files

			if (
				SynchronicMerger.GetPendingUpdateFiles(_liftFilePath)
					.Length > 0)
			{
				//Logger.WriteEvent("Running Synchronic Merger"); //needed??? TA 2008-07-09
				try
				{
					SynchronicMerger merger = new SynchronicMerger();
					UnLockLift();
					merger.MergeUpdatesIntoFile(_liftFilePath);
				}
				catch (BadUpdateFileException error)
				{
					string contents = File.ReadAllText(error.PathToNewFile);
					if (contents.Trim().Length == 0)
					{
						ErrorReport.ReportNonFatalMessage(
							"It looks as though WeSay recently crashed while attempting to save.  It will try again to preserve your work, but you will want to check to make sure nothing was lost.");
						File.Delete(error.PathToNewFile);
					}
					else
					{
						File.Move(error.PathToNewFile, error.PathToNewFile + ".bad");
						ErrorReport.ReportNonFatalMessage(
							"WeSay was unable to save some work you did in the previous session.  The work might be recoverable from the file {0}. The next screen will allow you to send a report of this to the developers.",
							error.PathToNewFile + ".bad");
						ErrorNotificationDialog.ReportException(error, null, false);
					}
					return false;
				}
				catch (Exception e)
				{
					throw new ApplicationException(
						"Could not finish updating LIFT dictionary file.", e);
				}
				finally
				{
					LockLift();
				}
			}
			return true;
		}

		private string MakeIncrementFileName(DateTime time)
		{
			while (true)
			{
				string timeString = time.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'-'FFFFFFF UTC");
				string path = Path.Combine(LiftDirectory, timeString);
				path += SynchronicMerger.ExtensionOfIncrementalFiles;
				if (!File.Exists(path))
				{
					return path;
				}
				time = time.AddTicks(1);
			}
		}

		//I don't think this is needed anymore TA 7-9-2008
		///// <summary>
		///// wierd name!
		///// </summary>
		//public static void LiftIsFreshNow()
		//{
		//    RecordUpdateTime(DateTime.UtcNow);
		//}

		//What is this method for??? TA 7-4-2008
		private void RecordUpdateTime(DateTime time)
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
			bool wasLocked = IsLiftFileLocked;
			if (wasLocked)
			{
				UnLockLift();
			}
			//File.SetLastWriteTimeUtc(_liftFilePath, time);
			//Debug.Assert(time == GetLastUpdateTime());
			if (wasLocked)
			{
				LockLift();
			}
		}

		//I don't think this is needed anymore!!! TA 7-9-2008
		//private static DateTime GetLastUpdateTime()
		//{
		//    Debug.Assert(Directory.Exists(LiftDirectory));
		//    return File.GetLastWriteTimeUtc(_liftFilePath);
		//}


		//I don't think this is needed anymore TA 7-9-2008
		//public IList<RepositoryId> GetRecordsNeedingUpdateInLift()
		//{
		//    DateTime last = GetLastUpdateTime();
		//    _timeOfLastQueryForNewRecords = DateTime.UtcNow;
		//    return _lexEntryRepository.GetItemsModifiedSince(last);
		//}

		/// <remark>
		/// The protection provided by this simple approach is obviously limited;
		/// it will keep the lift file safe normally... but could lead to non-data-losing crashes
		/// if some automated process was sitting out there, just waiting to open as soon as we realease
		/// </summary>
		private void UnLockLift()
		{
			//Debug.Assert(_liftFileStreamForLocking != null);
			//_liftFileStreamForLocking.Close();
			//_liftFileStreamForLocking.Dispose();
			//_liftFileStreamForLocking = null;
		}

		public bool IsLiftFileLocked
		{
			get { return _liftFileStreamForLocking != null; }
		}

		private void LockLift()
		{
			//Debug.Assert(_liftFileStreamForLocking == null);
			//_liftFileStreamForLocking = File.OpenRead(_liftFilePath);
		}
	}
}