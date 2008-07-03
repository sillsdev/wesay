using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Db4objects.Db4o;
using LiftIO.Parsing;
using LiftIO.Validation;
using Palaso.Progress;
using Palaso.Reporting;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using Debug=System.Diagnostics.Debug;

namespace WeSay.Project
{
	public class CacheManager
	{
		/// <summary>
		/// This is used to let us ship, with the installer, a version
		/// which won't rebuild the cache just because the installer maker
		/// changed the modified dates so the don't match anymore.
		/// </summary>
		public const string kCacheIsFreshIndicator = "AssumeCacheIsFresh";

		public static void RemoveAssumeCacheIsFreshIndicator()
		{
			string s = Path.Combine(WeSayWordsProject.Project.PathToCache, kCacheIsFreshIndicator);
			if (File.Exists(s))
			{
				File.Delete(s);
			}
		}

		public static CacheBuilder GetCacheBuilderIfNeeded(WeSayWordsProject project)
		{
			string pathToCacheDirectory = project.PathToCache;
			if (GetCacheIsOutOfDate(project))
			{
				if (Directory.Exists(pathToCacheDirectory) &&
					Directory.GetFileSystemEntries(pathToCacheDirectory).Length != 0)
				{
					try
					{
						Directory.Delete(pathToCacheDirectory, true);
					}
					catch (IOException)
					{
						// avoid crash if we managed to delete all files but not the directory itself
						if (Directory.GetFileSystemEntries(pathToCacheDirectory).Length != 0)
						{
							throw;
						}
					}
				}
			}
			if (!Directory.Exists(pathToCacheDirectory) ||
				!File.Exists(project.PathToDb4oLexicalModelDB))
			{
				return new CacheBuilder(project.PathToLiftFile);
			}
			return null;
		}

		public static bool GetCacheIsOutOfDate(WeSayWordsProject project)
		{
			string pathToCacheDirectory = project.PathToCache;
			string db4oPath = project.PathToDb4oLexicalModelDB;
			if (!Directory.Exists(pathToCacheDirectory) || (!File.Exists(db4oPath)))
			{
				return true;
			}

			if (GetAssumeCacheIsFresh(pathToCacheDirectory))
			{
				return false;
			}
			//|| (File.GetLastWriteTimeUtc(project.PathToLiftFile) > File.GetLastWriteTimeUtc(db4oPath)));

			using (Db4oDataSource ds = new Db4oDataSource(project.PathToDb4oLexicalModelDB))
			{
				return
						(!GetSyncPointInCacheMatches(ds,
													 File.GetLastWriteTimeUtc(project.PathToLiftFile))) ||
						(!GetModelVersionInCacheMatches(ds));
			}
		}

		public static bool GetAssumeCacheIsFresh(string pathToCacheDirectory)
		{
			return File.Exists(Path.Combine(pathToCacheDirectory, kCacheIsFreshIndicator));
		}

		private class SyncPoint
		{
			public DateTime when;
			private string _modelVersion;

			public string GetCurrentModelVersion()
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				Debug.Assert(assembly != null);
				object[] attributes =
						assembly.GetCustomAttributes(typeof (AssemblyFileVersionAttribute), false);
				Debug.Assert(attributes.Length > 0);
				return ((AssemblyFileVersionAttribute) attributes[0]).Version;
			}

			public void SetModelVersion()
			{
				_modelVersion = GetCurrentModelVersion();
			}

			public bool ModelMatchesCurrentVersion()
			{
				return _modelVersion == GetCurrentModelVersion();
			}
		}

		[CLSCompliant(false)]
		public static void UpdateSyncPointInCache(IObjectContainer db, DateTime when)
		{
			SyncPoint point;
			IList<SyncPoint> result = db.Query<SyncPoint>();
			if (result.Count > 1)
			{
				throw new ApplicationException("The cache should not have more than 1 syncpoint.");
			}
			if (result.Count == 0)
			{
				point = new SyncPoint();
			}
			else
			{
				point = result[0];
			}

			point.when = when;
			point.SetModelVersion();
			db.Set(point);
			db.Commit();
		}

		public static bool GetSyncPointInCacheMatches(Db4oDataSource dataSource, DateTime when)
		{
			IList<SyncPoint> result = dataSource.Data.Query<SyncPoint>();
			if (result.Count > 1)
			{
				throw new ApplicationException("The cache should not have more than 1 syncpoint.");
			}
			if (result.Count == 0)
			{
				return false;
			}
			else
			{
				SyncPoint syncPoint = result[0];
				//workaround db4o 6 bug that loses the utc bit
				if (syncPoint.when.Kind != DateTimeKind.Utc)
				{
					result[0].when = new DateTime(result[0].when.Ticks, DateTimeKind.Utc);
				}

				return result[0].when == when;
			}
		}

		public static bool GetModelVersionInCacheMatches(Db4oDataSource dataSource)
		{
			IList<SyncPoint> result = dataSource.Data.Query<SyncPoint>();
			if (result.Count > 1)
			{
				throw new ApplicationException("The cache should not have more than 1 syncpoint.");
			}
			if (result.Count == 0)
			{
				return false;
			}
			else
			{
				return result[0].ModelMatchesCurrentVersion();
			}
		}
	}

	public class CacheBuilder
	{
		private string _sourceLIFTPath;
		private ProgressState _progress;
		private BackgroundWorker _backgroundWorker;

		public CacheBuilder(string sourceLIFTPath)
		{
			// _destinationDatabasePath = destinationDatabasePath;
			_sourceLIFTPath = sourceLIFTPath;
		}

		/// <summary>
		/// Used when staring this from a "System.ComponentModel.BackgroundWorker"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnDoWork(object sender, DoWorkEventArgs e)
		{
			_backgroundWorker = sender as BackgroundWorker;
			DoWork(e.Argument as ProgressState);
			e.Result = e.Argument as ProgressState;

			//this is weird, but needed for the caller to know that we quite because
			// of a cancellation
			if (_backgroundWorker.CancellationPending)
			{
				e.Cancel = true;
			}
		}

		public string SourceLIFTPath
		{
			get { return _sourceLIFTPath; }
			set { _sourceLIFTPath = value; }
		}

		public void DoWork(ProgressState progress)
		{
			Logger.WriteEvent("Building Caches");
			_progress = progress;
			_progress.State = ProgressState.StateValue.Busy;
			_progress.StatusLabel = "Validating...";

			try
			{
				string errors = Validator.GetAnyValidationErrors(_sourceLIFTPath);
				if (errors != null && errors != string.Empty)
				{
					progress.StatusLabel = "Problem with file format...";
					_progress.State = ProgressState.StateValue.StoppedWithError;
					_progress.WriteToLog(
							string.Format("There is a problem with the format of {0}. {1}",
										  _sourceLIFTPath,
										  errors));
					Logger.WriteEvent("LIFT failed to validate.");
					return;
				}

				progress.StatusLabel = "Building Caches...";
				string tempCacheDirectory =
						Directory.CreateDirectory(
								Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
				WeSayWordsProject.Project.CacheLocationOverride = tempCacheDirectory;

				//same name, but in a temp directory
				string db4oFileName =
						Path.GetFileName(WeSayWordsProject.Project.PathToDb4oLexicalModelDB);
				string tempDb4oFilePath = Path.Combine(tempCacheDirectory, db4oFileName);

				using (
						LexEntryRepository lexEntryRepository =
								new LexEntryRepository(tempDb4oFilePath))
				{
					//MONO bug as of 1.1.18 cannot bitwise or FileShare on FileStream constructor
					//                    using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
					SetupTasksToBuildCaches(lexEntryRepository);

					DoParsing(lexEntryRepository);
					if (_backgroundWorker != null && _backgroundWorker.CancellationPending)
					{
						return;
					}
				}
				WeSayWordsProject.Project.CacheLocationOverride = null;
				ClearTheIncrementalBackupDirectory();

				//if we got this far without an error, move it
				//string backupPath = _destinationDatabasePath + ".bak";
				//not needed File.Delete(backupPath);
				string cacheFolderName = WeSayWordsProject.Project.PathToCache;
				// _destinationDatabasePath + " Cache";
				// if directory is empty, don't delete because it may be in use
				if (Directory.Exists(cacheFolderName) &&
					Directory.GetFileSystemEntries(cacheFolderName).Length != 0)
				{
					try
					{
						Directory.Delete(cacheFolderName, true);
					}
					catch (IOException)
					{
						// avoid crash if we managed to delete all files but not the directory itself
						if (Directory.GetFileSystemEntries(cacheFolderName).Length != 0)
						{
							throw;
						}
					}
					//fails if temp dir is on a different volume:
					//Directory.Move(tempTarget + " Cache", cacheFolderName);
				}
				SafeMoveDirectory(tempCacheDirectory, cacheFolderName);

				using (
						Db4oDataSource ds =
								new Db4oDataSource(
										WeSayWordsProject.Project.PathToDb4oLexicalModelDB))
				{
					CacheManager.UpdateSyncPointInCache(ds.Data,
														File.GetLastWriteTimeUtc(_sourceLIFTPath));
					File.WriteAllText(
							Path.Combine(WeSayWordsProject.Project.PathToCache,
										 CacheManager.kCacheIsFreshIndicator),
							"");
				}

				_progress.State = ProgressState.StateValue.Finished;
			}
			catch (Exception e)
			{
				//currently, error reporter can choke because this is
				//being called from a non sta thread.
				//so let's leave it to the progress dialog to report the error
				//                Reporting.ErrorReporter.ReportException(e,null, false);
				_progress.ExceptionThatWasEncountered = e;
				_progress.WriteToLog(e.Message);
				_progress.State = ProgressState.StateValue.StoppedWithError;
			}
			finally
			{
				try // EVIL STATICS!
				{
					if (WeSayWordsProject.Project != null)
					{
						WeSayWordsProject.Project.CacheLocationOverride = null;
					}
				}
				catch (Exception)
				{
					//swallow
				}
			}
			Logger.WriteEvent("Done Building Caches");
		}

		private void DoParsing(LexEntryRepository lexEntryRepository)
		{
			//                entriesList.WriteCacheSize = 0; //don't write after every record
			using (LiftMerger merger = new LiftMerger())
			{
				foreach (string name in WeSayWordsProject.Project.OptionFieldNames)
				{
					merger.ExpectedOptionTraits.Add(name);
					//_importer.ExpectedOptionTraits.Add(name);
				}
				foreach (string name in WeSayWordsProject.Project.OptionCollectionFieldNames)
				{
					merger.ExpectedOptionCollectionTraits.Add(name);
				}

				RunParser(merger);
			}

			UpdateDashboardStats();
		}

		private static void UpdateDashboardStats()
		{
			foreach (ITask task in WeSayWordsProject.Project.Tasks)
			{
				if (task is IFinishCacheSetup)
				{
					((IFinishCacheSetup) task).FinishCacheSetup();
				}
			}
		}

		private static void SetupTasksToBuildCaches(LexEntryRepository lexEntryRepository)
		{
			//todo unit of work?
			//lexEntryRepository.DelayWritingCachesUntilDispose = true;
			ConfigFileTaskBuilder taskBuilder;
			using (
					FileStream configFile =
							new FileStream(WeSayWordsProject.Project.PathToConfigFile,
										   FileMode.Open,
										   FileAccess.Read,
										   FileShare.ReadWrite))
			{
				taskBuilder =
						new ConfigFileTaskBuilder(configFile,
												  WeSayWordsProject.Project,
												  new EmptyCurrentWorkTask(),
												  lexEntryRepository);
			}

			WeSayWordsProject.Project.Tasks = taskBuilder.Tasks;

			//give the tasks a chance to register with the recordlistmanager
			foreach (ITask task in taskBuilder.Tasks)
			{
				if (task.MustBeActivatedDuringPreCache)
				{
					task.Activate();
					task.Deactivate();
				}
			}
		}

		private void RunParser(
				ILexiconMerger<WeSayDataObject, LexEntry, LexSense, LexExampleSentence> merger)
		{
			LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence> parser =
					new LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>(merger);

			parser.SetTotalNumberSteps += parser_SetTotalNumberSteps;
			parser.SetStepsCompleted += parser_SetStepsCompleted;

			parser.ParsingWarning += parser_ParsingWarning;

			try
			{
				parser.ReadLiftFile(_sourceLIFTPath);
			}
			catch (Exception)
			{
				//our parser failed.  Hopefully, because of bad lift. Validate it now  to
				//see if that's the problem.
				Validator.CheckLiftWithPossibleThrow(_sourceLIFTPath);

				//if it got past that, ok, send along the error the parser encountered.
				throw;
			}
		}

		private void parser_ParsingWarning(object sender,
										   LiftParser
												   <WeSayDataObject, LexEntry, LexSense,
												   LexExampleSentence>.ErrorArgs e)
		{
			_progress.WriteToLog(e.Exception.Message);
		}

		private void parser_SetStepsCompleted(object sender,
											  LiftParser
													  <WeSayDataObject, LexEntry, LexSense,
													  LexExampleSentence>.ProgressEventArgs e)
		{
			_progress.NumberOfStepsCompleted = e.Progress;
			if (_backgroundWorker != null)
			{
				e.Cancel = _backgroundWorker.CancellationPending;
			}
		}

		private void parser_SetTotalNumberSteps(object sender,
												LiftParser
														<WeSayDataObject, LexEntry, LexSense,
														LexExampleSentence>.StepsArgs e)
		{
			_progress.TotalNumberOfSteps = e.Steps;
		}

		/*public for unit-tests */

		public static void ClearTheIncrementalBackupDirectory()
		{
			if (!Directory.Exists(WeSayWordsProject.Project.PathToLiftBackupDir))
			{
				return;
			}
			string[] p = Directory.GetFiles(WeSayWordsProject.Project.PathToLiftBackupDir, "*.*");
			if (p.Length > 0)
			{
				string newPath = WeSayWordsProject.Project.PathToLiftBackupDir + ".old";

				int i = 0;
				while (Directory.Exists(newPath + i))
				{
					i++;
				}
				newPath += i;
				Directory.Move(WeSayWordsProject.Project.PathToLiftBackupDir, newPath);
			}
		}

		/// <summary>
		/// Recursive procedure to copy folder to another volume
		/// cleaned up from http://www.codeproject.com/cs/files/CopyFiles.asp
		/// </summary>
		private static void SafeMoveDirectory(string sourceDir, string destinationDir)
		{
			DirectoryInfo sourceDirInfo = new DirectoryInfo(sourceDir);
			FileInfo[] files = sourceDirInfo.GetFiles();
			Directory.CreateDirectory(destinationDir);
			foreach (FileInfo file in files)
			{
				file.CopyTo(Path.Combine(destinationDir, file.Name));
				file.Delete();
			}
			// Recursive copy children dirs
			DirectoryInfo[] directories = sourceDirInfo.GetDirectories();
			foreach (DirectoryInfo di in directories)
			{
				SafeMoveDirectory(di.FullName, Path.Combine(destinationDir, di.Name));
			}
			sourceDirInfo.Delete();
		}
	}
}