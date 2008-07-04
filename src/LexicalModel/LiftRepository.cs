using System;
using System.Collections.Generic;
using System.IO;
using LiftIO.Parsing;
using LiftIO.Validation;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;

namespace WeSay.LexicalModel
{
	public class LiftRepository:IRepository<LexEntry>
	{
		private class GuidRepositoryId:RepositoryId
		{
			private Guid _id;

			public GuidRepositoryId(Guid id)
			{
				this._id = id;
			}

			public override int CompareTo(RepositoryId other)
			{
				return CompareTo(other as GuidRepositoryId);
			}

			public int CompareTo(GuidRepositoryId other)
			{
				if(other == null)
				{
					return 1;
				}
				return Comparer<Guid>.Default.Compare(this._id, other._id);
			}

			public override bool Equals(RepositoryId other)
			{
				return Equals(other as GuidRepositoryId);
			}

			public bool Equals(GuidRepositoryId other)
			{
				if(other == null)
				{
					return false;
				}
				return Equals(this._id, other._id);
			}

		}

		private readonly string _liftFilePath;
		private DateTime lastModified;
		private readonly Dictionary<GuidRepositoryId, LexEntry> _entries;

		public LiftRepository(string filePath)
		{
			this._liftFilePath = filePath;
			this.lastModified = File.GetLastWriteTimeUtc(this._liftFilePath);
			_entries = new Dictionary<GuidRepositoryId, LexEntry>();
			LoadAllLexEntries();
		}

		public DateTime LastModified
		{
			get { return this.lastModified; }
		}

		public LexEntry CreateItem()
		{
			//todo write out new LexEntry
			return new LexEntry();
		}

		public int CountAllItems()
		{
			return GetAllItems().Length;
		}

		public RepositoryId GetId(LexEntry item)
		{
			return new GuidRepositoryId(item.Guid);
		}

		public LexEntry GetItem(RepositoryId id)
		{
			LoadAllLexEntries();
			return _entries[(GuidRepositoryId)id];
		}

		public void DeleteItem(LexEntry item)
		{
			//todo write out new LexEntry
			throw new ArgumentOutOfRangeException("item");
		}

		public void DeleteItem(RepositoryId id)
		{
			DeleteItem(GetItem(id));
		}

		public RepositoryId[] GetAllItems()
		{
			LoadAllLexEntries();
			GuidRepositoryId[] result = new GuidRepositoryId[_entries.Count];
			_entries.Keys.CopyTo(result, 0);
			return result;
		}

		public void SaveItem(LexEntry item)
		{
			//todo write out new LexEntry
			throw new ArgumentOutOfRangeException("item");
		}

		public bool CanQuery
		{
			get { return false; }
		}

		public bool CanPersist
		{
			get { return true; }
		}

		public void SaveItems(IEnumerable<LexEntry> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			UpdateLiftFile(items);
		}

		public ResultSet<LexEntry> GetItemsMatching(Query query)
		{
			throw new NotSupportedException("Querying is not supported");
		}

		#region IDisposable Members
#if DEBUG
		~LiftRepository()
		{
			if (!this._disposed)
			{
				throw new ApplicationException("Disposed not explicitly called on LiftRepository.");
			}
		}
#endif

		private bool _disposed = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic

				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("LiftRepository");
			}
		}
		#endregion
		private void LoadAllLexEntries()
		{
			_entries.Clear();
			using (LiftMerger merger = new LiftMerger())
			{
				merger.EntryCreatedEvent += OnEntryCreated;
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

		private void OnEntryCreated(object sender, LiftMerger.EntryCreatedEventArgs e)
		{
			_entries.Add(new GuidRepositoryId(e.Entry.Guid),
							e.Entry);
		}
		private void parser_ParsingWarning(object sender,
										   LiftParser
												   <WeSayDataObject, LexEntry, LexSense,
												   LexExampleSentence>.ErrorArgs e)
		{
		}

		private void parser_SetStepsCompleted(object sender,
											  LiftParser
													  <WeSayDataObject, LexEntry, LexSense,
													  LexExampleSentence>.ProgressEventArgs e)
		{
		}

		private void parser_SetTotalNumberSteps(object sender,
												LiftParser
														<WeSayDataObject, LexEntry, LexSense,
														LexExampleSentence>.StepsArgs e)
		{
		}

		//private const string s_updatePointFileName = "updatePoint";
		private const int _checkFrequency = 10;
		private int _commitCount;
		private readonly LexEntryRepository _lexEntryRepository;
		private DateTime _timeOfLastQueryForNewRecords;
		//private bool _didFindDataInCacheNeedingRecovery = false;

		private event EventHandler Updating;

		private static string LiftDirectory
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

		public void OnDataDeleted(object sender, EventArgs e)
		{
			LexEntry entry = (LexEntry)sender;
			if (entry == null)
			{
				return;
			}

			LiftExporter exporter =
					new LiftExporter( /*WeSayWordsProject.Project.GetFieldToOptionListNameDictionary(), */
							MakeIncrementFileName(DateTime.UtcNow));
			exporter.AddDeletedEntry(entry);
			exporter.End();
		}

		private void UpdateLiftFile(IEnumerable<LexEntry> entriesToUpdate)
		{
			CreateIncrementFile(entriesToUpdate);
			MergeIncrementFiles();
		}

		private void CreateIncrementFile(IEnumerable<LexEntry> entriesToUpdate)
		{
			//What is this for? TA 7-4-2008
			//if (Updating != null)
			//{
			//    Updating.Invoke(this, null);
			////}

			//Can't get the count of an IEnumerable. What to do? TA -7-4-2008
			//    LameProgressDialog dlg = null;
			//    if (repositoryIds.Count > 50)
			//    {
			//        //TODO: if we think this will actually ever be called, clean this up with a real, delayed-visibility dialog
			//        dlg =
			//                new LameProgressDialog(
			//                        string.Format("Doing incremental update of {0} records...",
			//                                      repositoryIds.Count));
			//        dlg.Show();
			//        Application.DoEvents();
			//    }

				try
				{
					bool startOfFileWritten = false;
					foreach (LexEntry entry in entriesToUpdate)
					{
						if (!startOfFileWritten)
						{
							LiftExporter exporter =
								new LiftExporter( /*WeSayWordsProject.Project.GetFieldToOptionListNameDictionary(),*/
												  MakeIncrementFileName(_timeOfLastQueryForNewRecords));
							startOfFileWritten = true;
						}
						exporter.Add(entry);
					}
					if (startOfFileWritten)
					{
						exporter.End();
					}

					RecordUpdateTime(_timeOfLastQueryForNewRecords);  //Why do we need to call this? TA 7-4-2008
				}
				finally
				{
				//See comment above. TA 7-4-2008
				//    if (dlg != null)
				//    {
				//        dlg.Close();
				//        dlg.Dispose();
				//    }
				}
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>false if it failed (and it would have already reported the error)</returns>
		public static bool MergeIncrementFiles()
		{
			//merge the increment files

			if (
					SynchronicMerger.GetPendingUpdateFiles(WeSayWordsProject.Project.PathToLiftFile)
							.Length > 0)
			{
				Logger.WriteEvent("Running Synchronic Merger");
				try
				{
					SynchronicMerger merger = new SynchronicMerger();
					WeSayWordsProject.Project.ReleaseLockOnLift();
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
					WeSayWordsProject.Project.LockLift();
				}
			}
			return true;
		}

		private static string MakeIncrementFileName(DateTime time)
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

		/// <summary>
		/// wierd name!
		/// </summary>
		public static void LiftIsFreshNow()
		{
			RecordUpdateTime(DateTime.UtcNow);
		}

		//What is this method for? TA 7-4-2008
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
			File.SetLastWriteTimeUtc(_liftFilePath, time);
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

		public IList<RepositoryId> GetRecordsNeedingUpdateInLift()
		{
			DateTime last = GetLastUpdateTime();
			_timeOfLastQueryForNewRecords = DateTime.UtcNow;
			return _lexEntryRepository.GetItemsModifiedSince(last);
		}

		/// <summary>
		/// Used to try again to get data out of the cache in case it crashed last time
		/// We can be successful at saving on startup if the crash was somehow related to the UI (as in WS-554)
		/// </summary>
		public void RecoverUnsavedChangesOutOfCacheIfNeeded()
		{
			try
			{
				if (CacheManager.GetAssumeCacheIsFresh(WeSayWordsProject.Project.PathToCache))
				{
					return;
					// setting permissions in the installer apparently was enough to mess this next line up on the sample data
				}

				IList<RepositoryId> records = GetRecordsNeedingUpdateInLift();
				if (records.Count == 0)
				{
					return;
				}

				try
				{
					ErrorReport.ReportNonFatalMessage(
							"It appears that WeSay did not exit normally last time.  WeSay will now attempt to recover the {0} records which were not saved.",
							records.Count);
					DoLiftUpdateNow(false);
					//                    _didFindDataInCacheNeedingRecovery = true;
					ErrorReport.ReportNonFatalMessage("Your work was successfully recovered.");
				}
				catch (Exception)
				{
					ErrorReport.ReportNonFatalMessage(
							"Sorry, WeSay was unable to recover some of your work.");
					WeSayWordsProject.Project.InvalidateCacheSilently();
				}
			}
			catch (Exception)
			{
				ErrorReport.ReportNonFatalMessage(
						"WeSay had a problem reading the cache.  It will now be rebuilt");
				WeSayWordsProject.Project.InvalidateCacheSilently();
			}
		}
	}
}