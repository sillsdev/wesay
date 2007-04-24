using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using LiftIO;
using WeSay.App;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

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
			string s = Path.Combine(Project.WeSayWordsProject.Project.PathToCache, kCacheIsFreshIndicator);
			if(File.Exists(s))
			{
				File.Delete(s);
			}
		}

		public static CacheBuilder GetCacheBuilderIfNeeded(WeSayWordsProject project)
		{
			string pathToCacheDirectory = project.PathToCache;
			if (GetCacheIsOutOfDate(project))
			{
				if (Directory.Exists(pathToCacheDirectory))
				{
					Directory.Delete(pathToCacheDirectory, true);
				}
			}
			if (!Directory.Exists(pathToCacheDirectory))
			{
				return new CacheBuilder(project.PathToLiftFile);
			}
			return null;
		}

		public static bool GetCacheIsOutOfDate(WeSayWordsProject project)
		{
			string pathToCacheDirectory = project.PathToCache;
			string db4oPath = project.PathToDb4oLexicalModelDB;
			if (!Directory.Exists(pathToCacheDirectory) ||
				   (!File.Exists(db4oPath)))
			{
				return true;
			}

			if (AssumeCacheIsFresh(pathToCacheDirectory))
			{
				return false;
			}
			//|| (File.GetLastWriteTimeUtc(project.PathToLiftFile) > File.GetLastWriteTimeUtc(db4oPath)));

			using (Db4oDataSource ds = new Db4oDataSource(project.PathToDb4oLexicalModelDB))
			{
				return !GetSyncPointInCacheMatches(ds, File.GetLastWriteTimeUtc(project.PathToLiftFile));
			}
		}

		public static bool AssumeCacheIsFresh(string pathToCacheDirectory)
		{
			return File.Exists(Path.Combine(pathToCacheDirectory, kCacheIsFreshIndicator));
		}

		class SyncPoint
		{
			public DateTime when;
		}

		public static void UpdateSyncPointInCache(Db4objects.Db4o.IObjectContainer db, DateTime when)
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
			db.Set(point);
			db.Commit();
		}

		public static bool  GetSyncPointInCacheMatches(Db4oDataSource dataSource, DateTime when)
		{
//            SyncPoint point;
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
				//workaround db4o 6 bug that loses the utc bit
				if (result[0].when.Kind != DateTimeKind.Utc)
				{
					result[0].when = new DateTime(result[0].when.Ticks, DateTimeKind.Utc);
				}

				return result[0].when == when;
			}
		}
	}

	public class CacheBuilder
	{
		private string _sourceLIFTPath;
		protected ProgressState _progress;
		private Db4oRecordList<LexEntry> _prewiredEntries=null;
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

		/// <summary>
		/// caller can set this if you want some actions to happen as each entry is
		/// imported, such as building up indices.  The given entries list will be
		/// used, and you can act on the events it fires.
		/// </summary>
		public Db4oRecordList<LexEntry> EntriesAlreadyWiredUp
		{
			set
			{
				_prewiredEntries = value;
			}
		}

		public string SourceLIFTPath
		{
			get
			{
				return _sourceLIFTPath;
			}
			set
			{
				_sourceLIFTPath = value;
			}
		}

		public Db4oRecordList<LexEntry> GetEntries(Db4oDataSource ds)
		{

			if (_prewiredEntries == null)
			{
				return new Db4oRecordList<LexEntry>(ds);
			}

			return _prewiredEntries;
		}

		public void DoWork(ProgressState progress)
		{
			_progress = progress;
			_progress.State = ProgressState.StateValue.Busy;
			_progress.StatusLabel = "Validating...";
			string errors = Validator.GetAnyValidationErrors(_sourceLIFTPath);
			if (errors != null && errors != string.Empty)
			{
				progress.StatusLabel = "Problem with file format...";
				_progress.State = ProgressState.StateValue.StoppedWithError;
				_progress.WriteToLog(errors);
				return;
			}
			try
			{
				progress.StatusLabel = "Building Caches...";
				string tempCacheDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
				WeSayWordsProject.Project.CacheLocationOverride = tempCacheDirectory;

				//same name, but in a temp directory
				string db4oFileName = Path.GetFileName(WeSayWordsProject.Project.PathToDb4oLexicalModelDB);
				string tempDb4oFilePath = Path.Combine(tempCacheDirectory, db4oFileName);

				using (IRecordListManager recordListManager =
					new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), tempDb4oFilePath))
				{
					Db4oDataSource ds = ((Db4oRecordListManager)recordListManager).DataSource;
					Db4oLexModelHelper.Initialize(ds.Data);
					//  Db4oRecordListManager ds = recordListManager as Db4oRecordListManager;

					//MONO bug as of 1.1.18 cannot bitwise or FileShare on FileStream constructor
					//                    using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
					SetupTasksToBuildCaches(recordListManager);

					EntriesAlreadyWiredUp = (Db4oRecordList<LexEntry>)recordListManager.GetListOfType<LexEntry>();

					if (Db4oLexModelHelper.Singleton == null)
					{
						Db4oLexModelHelper.Initialize(ds.Data);
					}

					XmlDocument doc = new XmlDocument();
					doc.Load(_sourceLIFTPath);

					DoParsing(doc, ds);
					if (_backgroundWorker!=null && _backgroundWorker.CancellationPending)
					{
						return;
					}
				}
				 WeSayWordsProject.Project.CacheLocationOverride = null;
				ClearTheIncrementalBackupDirectory();

				//if we got this far without an error, move it
				//string backupPath = _destinationDatabasePath + ".bak";
				//not needed File.Delete(backupPath);
			   string cacheFolderName = WeSayWordsProject.Project.PathToCache;// _destinationDatabasePath + " Cache";
				if (Directory.Exists(cacheFolderName))
				{
					Directory.Delete(cacheFolderName, true);
					//fails if temp dir is on a different volume:
					//Directory.Move(tempTarget + " Cache", cacheFolderName);
				}
				SafeMoveDirectory(tempCacheDirectory, cacheFolderName);

				using (Db4oDataSource ds = new Db4oDataSource(WeSayWordsProject.Project.PathToDb4oLexicalModelDB))
				{
					CacheManager.UpdateSyncPointInCache(ds.Data, File.GetLastWriteTimeUtc(_sourceLIFTPath));
					File.WriteAllText(
						Path.Combine(Project.WeSayWordsProject.Project.PathToCache, CacheManager.kCacheIsFreshIndicator), "");
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
				WeSayWordsProject.Project.CacheLocationOverride = null;
			}
		}

		private void DoParsing(XmlDocument doc, Db4oDataSource ds)
		{
			Db4oRecordList<LexEntry> entriesList = GetEntries(ds);
			try
			{
				entriesList.WriteCacheSize = 0; //don't write after every record
				using (LiftMerger merger = new LiftMerger(ds, entriesList))
				{
					foreach (string name in WeSayWordsProject.Project.OptionFieldNames)
					{
						merger.ExpectedOptionTraits.Add(name);
						//_importer.ExpectedOptionTraits.Add(name);
					}
					foreach (
						string name in WeSayWordsProject.Project.OptionCollectionFieldNames)
					{
						merger.ExpectedOptionCollectionTraits.Add(name);
					}

					RunParser(doc, merger);
				}

				UpdateDashboardStats();
			}
			finally
			{
				if (entriesList != _prewiredEntries) //did we create it?
				{
					entriesList.Dispose();
				}
			}
		}

		static private void UpdateDashboardStats()
		{
			foreach (ITask task in WeSayWordsProject.Project.Tasks)
			{
				if (task is IFinishCacheSetup)
				{
					((IFinishCacheSetup) task).FinishCacheSetup();
				}
			}
		}

		private static void SetupTasksToBuildCaches(IRecordListManager recordListManager)
		{
			recordListManager.DelayWritingCachesUntilDispose = true;
			ConfigFileTaskBuilder taskBuilder;
			using (
				FileStream configFile =
					new FileStream(WeSayWordsProject.Project.PathToProjectTaskInventory,
								   FileMode.Open, FileAccess.Read,
								   FileShare.ReadWrite))
			{
				taskBuilder = new ConfigFileTaskBuilder(configFile, WeSayWordsProject.Project,
														new EmptyCurrentWorkTask(), recordListManager);
			}

			WeSayWordsProject.Project.Tasks = taskBuilder.Tasks;

			//give the tasks a chance to register with the recordlistmanager
			foreach (ITask task in taskBuilder.Tasks)
			{
				task.Activate();
				task.Deactivate();
			}
		}

		private void RunParser(XmlDocument doc, LiftMerger merger)
		{
			LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence> parser =
				new LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>(merger);

			parser.SetTotalNumberSteps +=
				new EventHandler
					<LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.StepsArgs>(
					parser_SetTotalNumberSteps);
			parser.SetStepsCompleted +=
				new EventHandler
					<
						LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.
							ProgressEventArgs>(
					parser_SetStepsCompleted);

			parser.ParsingWarning +=
				new EventHandler
					<LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.ErrorArgs>(
					parser_ParsingWarning);
			parser.ReadFile(doc);
		}

		void parser_ParsingWarning(object sender, LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.ErrorArgs e)
		{
			_progress.WriteToLog(e.Exception.Message);
		}



		void parser_SetStepsCompleted(object sender, LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.ProgressEventArgs e)
		{
			_progress.NumberOfStepsCompleted = e.Progress;
			if (_backgroundWorker != null)
			{
				e.Cancel = _backgroundWorker.CancellationPending;
			}
		}

		void parser_SetTotalNumberSteps(object sender, LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.StepsArgs e)
		{
			_progress.TotalNumberOfSteps =e.Steps;
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
				Directory.Move(WeSayWordsProject.Project.PathToLiftBackupDir,
							   newPath);
			}
		}


		/// <summary>
		/// Recursive procedure to copy folder to another volume
		/// cleaned up from http://www.codeproject.com/cs/files/CopyFiles.asp
		/// </summary>
		private static void SafeMoveDirectory(string sourceDir,
				string destinationDir)
		{
			DirectoryInfo sourceDirInfo = new DirectoryInfo(sourceDir);
			FileInfo[] files = sourceDirInfo.GetFiles();
			Directory.CreateDirectory(destinationDir);
			foreach (FileInfo file in files)
			{
				file.CopyTo(Path.Combine(destinationDir,file.Name));
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
