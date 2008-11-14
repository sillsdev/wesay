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
	public class LiftRepository : MemoryRepository<LexEntry>
	{
		private readonly ProgressState _progressState;
		private readonly string _liftFilePath;
		private FileStream _liftFileStreamForLocking;
		private bool _loadingAllEntries;
		private int _nextFileOrder;

		public LiftRepository(string filePath, ProgressState progressState)
		{
			//set to true so that an exception in the constructor does not cause the destructor to throw
			_disposed = true;
			if (progressState == null)
			{
				throw new ArgumentNullException("progressState");
			}
			_liftFilePath = filePath;
			_progressState = progressState;
			CreateEmptyLiftFileIfNeeded(filePath);
			LockLift();
			MigrateLiftIfNeeded(progressState);
			LastModified = DateTime.MinValue;
			LoadAllLexEntries();
			//Now that the constructor has not thrown we can set this back to false
			_disposed = false;
		}

		private void CreateEmptyLiftFileIfNeeded(string filePath)
		{
			FileInfo fileInfo = new FileInfo(_liftFilePath);
			bool DoesEmptyLiftFileNeedtoBeCreated = !fileInfo.Exists || fileInfo.Length == 0;
			if (DoesEmptyLiftFileNeedtoBeCreated)
			{
				CreateEmptyLiftFile(filePath);
			}
		}

		private void MigrateLiftIfNeeded(ProgressState progressState)
		{
			LiftPreparer preparer = new LiftPreparer(_liftFilePath);
			UnLockLift();
			if (preparer.IsMigrationNeeded())
			{
				preparer.MigrateLiftFile(progressState);
			}
			preparer.PopulateDefinitions(progressState);
			LockLift();
		}

		private void CreateEmptyLiftFile(string filePath)
		{
			LiftExporter exporter = new LiftExporter(filePath);
			exporter.End();
		}

		public LiftRepository(string filePath)
			: this(filePath, new ProgressState())
		{ }

		public override LexEntry CreateItem()
		{
			LexEntry item = base.CreateItem();
			item.OrderInFile = NextFileOrder;
			if (!_loadingAllEntries)
			{
				UpdateLiftFileWithNew(item);
			}
			return item;
		}

		private int NextFileOrder
		{
			get
			{
				return ++_nextFileOrder;
			}
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
			foreach (RepositoryId id in idsOfEntriesInRepository)
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
			const string status = "Loading entries";
			Logger.WriteEvent(status);
			_progressState.StatusLabel = status;

			try
			{
				UnLockLift();
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
					catch (LiftFormatException)
					{
						throw;
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
				LockLift();
				_loadingAllEntries = false;
			}
		}

#if DEBUG
		~LiftRepository()
		{
			if (!_disposed)
			{
				throw new ApplicationException("Disposed not explicitly called on LiftRepository.");
			}
		}
#endif

		public override void Dispose()
		{
			UnLockLift();
			base.Dispose();
		}

		private void parser_ParsingWarning(object sender,
										   LiftParser
												   <WeSayDataObject, LexEntry, LexSense,
												   LexExampleSentence>.ErrorArgs e)
		{
			_progressState.ExceptionThatWasEncountered = e.Exception;
		}

		private void parser_SetStepsCompleted(object sender,
											  LiftParser
													  <WeSayDataObject, LexEntry, LexSense,
													  LexExampleSentence>.ProgressEventArgs e)
		{
			_progressState.NumberOfStepsCompleted = e.Progress;
			e.Cancel = _progressState.Cancel;
		}

		private void parser_SetTotalNumberSteps(object sender,
												LiftParser
														<WeSayDataObject, LexEntry, LexSense,
														LexExampleSentence>.StepsArgs e)
		{
			_progressState.TotalNumberOfSteps = e.Steps;
		}

		private string LiftDirectory
		{
			get { return Path.GetDirectoryName(_liftFilePath); }
		}

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

		private void UpdateLiftFileWithNew(LexEntry entryToUpdate)
		{
			CreateFileContainingNew(entryToUpdate);
			MergeIncrementFiles();
		}

		private void UpdateLiftFileWithDeleted(LexEntry entryToDelete)
		{
			CreateFileContainingDeleted(entryToDelete);
			MergeIncrementFiles();
		}

		private void CreateFileContainingNew(LexEntry entry)
		{
			LiftExporter exporter = new LiftExporter(MakeIncrementFileName(PreciseDateTime.UtcNow));
			//!!!exporter.Start(); //!!! Would be nice to have this CJP 2008-07-09
			exporter.AddNewEntry(entry);
			exporter.End();
		}

		private void CreateFileContainingModified(LexEntry entryToUpdate)
		{
			LiftExporter exporter = new LiftExporter(MakeIncrementFileName(PreciseDateTime.UtcNow));
			//!!!exporter.Start(); //!!! Would be nice to have this CJP 2008-07-09
			exporter.Add(entryToUpdate);
			exporter.End();
		}

		private void CreateFileContainingModified(IEnumerable<LexEntry> entriesToUpdate)
		{
			LiftExporter exporter = null;
			foreach (LexEntry entry in entriesToUpdate)
			{
				if (exporter == null)
				{
					exporter = new LiftExporter(MakeIncrementFileName(PreciseDateTime.UtcNow));
					//!!!exporter.Start(); //!!! Would be nice to have this CJP 2008-07-09
				}
				exporter.Add(entry);
			}
			if (exporter != null)
			{
				exporter.End();
			}
		}

		private void CreateFileContainingDeleted(LexEntry entryToDelete)
		{
			LiftExporter exporter = new LiftExporter(MakeIncrementFileName(PreciseDateTime.UtcNow));
			//!!!exporter.Start(); //!!! Would be nice to have this CJP 2008-07-09
			exporter.AddDeletedEntry(entryToDelete);
			exporter.End();
		}

		private void CreateFileContainingDeleted(IEnumerable<LexEntry> entriesToDelete)
		{
			LiftExporter exporter = null;
			foreach (LexEntry entry in entriesToDelete)
			{
				if (exporter == null)
				{
					exporter = new LiftExporter(MakeIncrementFileName(PreciseDateTime.UtcNow));
					//!!!exporter.Start(); //!!! Would be nice to have this CJP 2008-07-09
				}
				exporter.AddDeletedEntry(entry);
			}
			if (exporter != null)
			{
				exporter.End();
			}
		}



		public RightToAccessLiftExternally GetRightToAccessLiftExternally()
		{
			return new RightToAccessLiftExternally(this);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>false if it failed (and it would have already reported the error)</returns>
		private void MergeIncrementFiles()
		{
			//merge the increment files

			if (SynchronicMerger.GetPendingUpdateFiles(_liftFilePath).Length > 0)
			{
				Logger.WriteEvent("Running Synchronic Merger");
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
					//return false; //!!! remove CJP
				}
				catch (Exception e)
				{
					ErrorReport.ReportNonFatalMessage(
							"Could not finish updating LIFT dictionary file. Will try again later."+Environment.NewLine+" ("+e.Message+")");
				}
				finally
				{
					LockLift();
				}
			}
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

		/// <remark>
		/// The protection provided by this simple approach is obviously limited;
		/// it will keep the lift file safe normally... but could lead to non-data-losing crashes
		/// if some automated process was sitting out there, just waiting to open as soon as we realease
		/// </summary>
		private void UnLockLift()
		{
			Debug.Assert(_liftFileStreamForLocking != null);
			_liftFileStreamForLocking.Close();
			_liftFileStreamForLocking.Dispose();
			_liftFileStreamForLocking = null;
		}

		public bool IsLiftFileLocked
		{
			get
			{
				return _liftFileStreamForLocking != null;
			}
		}

		private void LockLift()
		{
			if (_liftFileStreamForLocking != null)
			{
				throw new IOException("WeSay was not able to acquire a lock on the Lift file. Is it open in another program?");
			}
			_liftFileStreamForLocking = new FileStream(_liftFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
		}

		/// <summary>
		/// this is pretty simplistic, at the moment
		/// </summary>
		public class RightToAccessLiftExternally : IDisposable
		{
			private readonly LiftRepository _repository;

			public RightToAccessLiftExternally(LiftRepository r)
			{
				_repository = r;
				if (_repository.IsLiftFileLocked)
				{
					_repository.UnLockLift();
				}
				else
				{
					_repository = null;
				}
			}

#if DEBUG
			~RightToAccessLiftExternally()
			{
				if (!_disposed)
				{
					throw new ApplicationException(
							"Disposed not explicitly called on RightToAccessLiftExternally.");
				}
			}
#endif

			private bool _disposed;

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!_disposed)
				{
					if (disposing)
					{
						if (_repository != null) // will be null if the repo wasn't locked, as happens when shutting down wesay and backing up last thing
						{
							_repository.LockLift();
						}
					}
				}
				_disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("LexEntryRepository");
			}
		}

	}

}