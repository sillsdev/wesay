using System;
using System.Collections.Generic;
using System.IO;
using Db4objects.Db4o;
using LiftIO.Merging;
using LiftIO.Validation;
using NUnit.Framework;
using Palaso.Progress;
using WeSay.Foundation.Tests;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.Project.Tests
{



	[TestFixture]
	public class CacheBuilderTests
	{
		private CacheBuilder _cacheBuilder;
		private ProgressState _progress;
		private readonly string _simpleGoodLiftContents = string.Format("<?xml version='1.0' encoding='utf-8'?><lift version='{0}'><entry id='one'><sense><gloss lang='en'><text>hello</text></gloss></sense></entry><entry id='two'/></lift>", Validator.LiftVersion);
		private string _log;

		static protected string BackupPath
		{
			get
			{
				return WeSayWordsProject.Project.PathToLiftBackupDir;
			}
		}

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			_cacheBuilder = new CacheBuilder(WeSayWordsProject.Project.PathToLiftFile);
			_progress = new ConsoleProgress();
			_progress.Log += OnLog;
		}


		[TearDown]
		public void TearDown()
		{
			_progress.Dispose();
			if (Directory.Exists(WeSayWordsProject.Project.PathToCache))
			{
				Directory.Delete(WeSayWordsProject.Project.PathToCache, true);
			}

			if (File.Exists(_cacheBuilder.SourceLIFTPath))
			{
				File.Delete(_cacheBuilder.SourceLIFTPath);
			}
			if (Directory.Exists(WeSayWordsProject.Project.PathToLiftBackupDir))
			{
				string dirToEmptyOfBackupDirs =
						Directory.GetParent(WeSayWordsProject.Project.PathToLiftBackupDir).FullName;
				string[] backUpDirs = Directory.GetDirectories(dirToEmptyOfBackupDirs, "*incremental*");
				foreach (string dir in backUpDirs)
				{
					Directory.Delete(dir, true);
				}
			}
			WeSayWordsProject.Project.Dispose();
		}




		[Test]
		public void GoodLiftStopsWithProgressInFinishedState()
		{
			SimpleGoodLiftCore(false);
		}

		[Test]
		public void LeavesSynchronized()
		{
			SimpleGoodLiftCore(false);
			Assert.IsFalse(CacheManager.GetCacheIsOutOfDate(WeSayWordsProject.Project));
		}

		[Test]
		public void ReplacesExistingFiles()
		{
			SimpleGoodLiftCore(true);
		}


		private void SimpleGoodLiftCore(bool doMakeExistingFilesThatNeedToBeReplaced)
		{
			if (doMakeExistingFilesThatNeedToBeReplaced)
			{
				string dir = WeSayWordsProject.Project.PathToCache;
				Directory.CreateDirectory(dir);
				string oldCache = WeSayWordsProject.Project.PathToCache;
				Directory.CreateDirectory(oldCache);

				File.WriteAllText(Path.Combine(oldCache, "foo"), "hello");
			}

			File.WriteAllText(_cacheBuilder.SourceLIFTPath, _simpleGoodLiftContents);
			Assert.AreEqual(ProgressState.StateValue.NotStarted, _progress.State);
			_cacheBuilder.DoWork(_progress);
			Assert.AreEqual(ProgressState.StateValue.Finished, _progress.State, _log);
		}

		[Test]//, Ignore("Run this by hand if you have an E: volume (windows only)")]
		public void WorksWithTempDirectoryOnADifferentVolumne()
	   {
		   if (Environment.OSVersion.Platform == PlatformID.Unix)
		   {
			   Console.WriteLine("Ignored on non-Windows");
		   }
		   else
		   {
			   //testing approach: it's harder to get the temp locaiton changed, so we
			   // instead put the destination project over on the non-default volume
			   DriveInfo[] drives = DriveInfo.GetDrives();

			   // get a drive I might be able to use
			   string driveName = string.Empty;
			   foreach (DriveInfo drive in drives)
			   {
				   if (drive.IsReady &&
					   drive.DriveType != DriveType.CDRom &&
					   drive.Name != "C:\\")
				   {
					   driveName = drive.Name;
					   break;
				   }
			   }
			   if (driveName.Length == 0)
			   {
				   Console.WriteLine("Ignored when there is not an additional volume");
			   }
			   else
			   {
				   string directory;
				   do
				   {
					   directory = Path.Combine(driveName, Path.GetRandomFileName());

				   } while(Directory.Exists(directory));

				   string target = Path.Combine(directory, "PRETEND.lift");
				   Directory.CreateDirectory(directory);
				   try
				   {
					   //Directory.CreateDirectory(Path.Combine(directory, "WeSay"));
					   WeSayWordsProject.Project.SetupProjectDirForTests(target);
					   Assert.AreEqual(directory, WeSayWordsProject.Project.ProjectDirectoryPath);
					   SimpleGoodLiftCore(true);
				   }
				   finally
				   {
					   Directory.Delete(directory, true);
				   }
			   }
		   }
	   }

		[Test]
		public void BadLiftStopsWithProgressInErrorState()
		{
			TryToLoadBadLift();
			Assert.AreEqual(ProgressState.StateValue.StoppedWithError, _progress.State);
		}

		[Test]
		public void BadLiftOutputsToLog()
		{
			TryToLoadBadLift();
		  //  System.Diagnostics.Debug.WriteLine(_log);
			Assert.IsTrue(_log.Contains("Invalid") );
		}

		private void TryToLoadBadLift()
		{
			File.WriteAllText(_cacheBuilder.SourceLIFTPath, _simpleGoodLiftContents.Replace("</lift>", "<x/></lift>"));
			Assert.AreEqual(ProgressState.StateValue.NotStarted, _progress.State);
			_cacheBuilder.DoWork(_progress);
		   // WaitForFinish();
		}




		void OnLog(object sender, ProgressState.LogEvent e)
		{
			_log += e.message;
		}

		[Test]
		public void CreatesDb4oFileWhichContainsEntriesAndSenses()
		{
			File.WriteAllText(_cacheBuilder.SourceLIFTPath, _simpleGoodLiftContents);

			_cacheBuilder.DoWork(_progress);
		 //   WaitForFinish();
			string dbPath = WeSayWordsProject.Project.PathToDb4oLexicalModelDB;
			using (IObjectContainer db = Db4oFactory.OpenFile(dbPath))
			{
				IList<LexEntry> x = db.Query<LexEntry>();
				Assert.AreEqual(2, x.Count);

				int index = (x[0].Id == "one")?0:1;
				Assert.AreEqual("one", x[index].Id); //got the wrong order here
				Assert.AreEqual(1, x[index].Senses.Count); // sensitive to order (shame)
			}
			Assert.AreEqual(ProgressState.StateValue.Finished, _progress.State);
		}

		[Test]
		public void OkIfNoExistingDbToBackup()
		{
			File.Delete(BackupPath);
			MakeBackupOfExistingDBCore();
		}

		[Test, Ignore("Do we really need a backup of the cache?")]
		public void OkIfHasExistingDbToBackup()
		{
	//        File.WriteAllText(_cacheBuilder.DestinationDatabasePath, "old current");
			File.Delete(BackupPath);
			MakeBackupOfExistingDBCore();
			Assert.IsTrue(File.Exists(BackupPath));
			Assert.IsTrue(File.ReadAllText(BackupPath) == "old current");
		}

		[Test, Ignore("Do we really need a backup of the cache?")]
		public void OkIfHasExistingBackupToRemoveFirst()
		{

			File.WriteAllText(BackupPath, "old backup");
	  //      File.WriteAllText(_cacheBuilder.DestinationDatabasePath, "old current");
			MakeBackupOfExistingDBCore();
			Assert.IsTrue(File.Exists(BackupPath));
			Assert.IsTrue(File.ReadAllText(BackupPath) == "old current");
		}

		[Test]
		public void MakesBackupOfExistingDB()
		{
			//just in case
			File.Delete(BackupPath);

			MakeBackupOfExistingDBCore();
		}

		private void MakeBackupOfExistingDBCore()
		{
			File.WriteAllText(_cacheBuilder.SourceLIFTPath, _simpleGoodLiftContents);
			_cacheBuilder.DoWork(_progress);
	  //      WaitForFinish();
		}


		[Test]
		public void ClearIncrementalBackupCache_Command_DeletesIt()
		{
			string dir = WeSayWordsProject.Project.PathToLiftBackupDir;
			Directory.CreateDirectory(dir);
			string deleteThisGuy = Path.Combine(dir, "deleteMe.txt");
			File.WriteAllText(deleteThisGuy, "hello");
			CacheBuilder.ClearTheIncrementalBackupDirectory();
			Assert.IsFalse(File.Exists(deleteThisGuy));
		}

		[Test]
		public void ClearsIncrementalBackupCache()
		{
			string dir = WeSayWordsProject.Project.PathToLiftBackupDir;
			Directory.CreateDirectory(dir);
			string deleteThisGuy = Path.Combine(dir, "deleteMe.txt");
			File.WriteAllText(deleteThisGuy,"doesn't matter");
			File.WriteAllText(_cacheBuilder.SourceLIFTPath, _simpleGoodLiftContents);

			_cacheBuilder.DoWork(_progress);
		   // WaitForFinish();
			Assert.IsFalse(File.Exists(deleteThisGuy));
	   }


	}

	public class DummyLiftChangeDetector : ILiftChangeDetector
	{
		private bool _haveCache = false;

		public DummyLiftChangeDetector()
		{
			Reset();
		}
		public void Reset()
		{
			_haveCache = true;
		}

		public void ClearCache()
		{
			_haveCache = false;
		}

		public ILiftChangeReport GetChangeReport(IProgress progress)
		{
			throw new System.NotImplementedException();
		}

		public bool CanProvideChangeRecord
		{
			get { return _haveCache; }
		}
	}
}
