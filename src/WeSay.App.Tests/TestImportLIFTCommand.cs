using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Db4objects.Db4o;
using MultithreadProgress;
using NUnit.Framework;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class TestImportLIFTCommand
	{
		private ProgressDialogHandler _progressHandler;
		private ImportLIFTCommand _importCommand;
		private bool _finished;
		private ProgressState _progress;
		private string _backupPath;
		private string _simpleGoodLiftContents = string.Format("<?xml version='1.0' encoding='utf-8'?><lift version='{0}'><entry id='one'><sense><gloss lang='en'><text>hello</text></gloss></sense></entry><entry id='two'/></lift>", LiftIO.Validator.LiftVersion);
		private string _log;

		protected string BackupPath
		{
			get
			{
				return _importCommand.DestinationDatabasePath + ".bak";
			}
		}

		[SetUp]
		public void Setup()
		{
			WeSay.Project.WeSayWordsProject.InitializeForTests();
			_importCommand = new ImportLIFTCommand(Path.GetTempFileName(), Path.GetTempFileName());
			_progressHandler = new ProgressDialogHandler(new System.Windows.Forms.Form(), _importCommand);
			_progressHandler.Finished += new EventHandler(_progressHandler_Finished);
			_progress = new ProgressState(_progressHandler);
			_progress.Log += new EventHandler<ProgressState.LogEvent>(OnLog);
			_finished = false;
		}

		private void _progressHandler_Finished(object sender, EventArgs e)
		{
			_finished = true;
		}

		[TearDown]
		public void TearDown()
		{
			_progress.Dispose();
			if (File.Exists(_importCommand.DestinationDatabasePath))
			{
				File.Delete(_importCommand.DestinationDatabasePath );
			}
			if (File.Exists(_importCommand.SourceLIFTPath))
			{
				File.Delete(_importCommand.SourceLIFTPath);
			}
			string dirToEmptyOfBackupDirs = Directory.GetParent(WeSay.Project.WeSayWordsProject.Project.PathToLiftBackupDir).FullName;
			string[] backUpDirs = Directory.GetDirectories(dirToEmptyOfBackupDirs, "*incremental*");
			foreach (string dir in backUpDirs)
			{
				Directory.Delete(dir,true);
			}
		}
		[Test]
		public void GoodLiftStopsWithProgressInFinishedState()
		{
			SimpleGoodLiftCore(false);
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
				string dir = Path.GetDirectoryName(_importCommand.DestinationDatabasePath);
				Directory.CreateDirectory(dir);
				string oldCache = _importCommand.DestinationDatabasePath + " Cache";
				Directory.CreateDirectory(oldCache);

				File.WriteAllText(Path.Combine(oldCache, "foo"), "hello");
			}

			File.WriteAllText(_importCommand.SourceLIFTPath, _simpleGoodLiftContents);
			Assert.AreEqual(ProgressState.StatusValue.NotStarted, _progress.Status);
			_importCommand.BeginInvoke(_progress);
			WaitForFinish();
			//  Console.WriteLine(_log);
			Assert.AreEqual(ProgressState.StatusValue.Finished, _progress.Status, _log);
		}

		[Test, Ignore("Run this by hand if you have an E: volume (windows only)")]
		public void WorksWithTempDirectoryOnADifferentVolumne()
		{
			string dir = "E:\\WeSayTest\\";
			Directory.CreateDirectory(dir);
			_importCommand.DestinationDatabasePath = Path.Combine(dir, Path.GetRandomFileName() );
			try
			{
				SimpleGoodLiftCore(true);
			}
			finally
			{
				Directory.Delete(dir, true);
			}
		}

		[Test]
		public void BadLiftStopsWithProgressInErrorState()
		{
			TryToLoadBadLift();
			Assert.AreEqual(ProgressState.StatusValue.StoppedWithError, _progress.Status);
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
			File.WriteAllText(_importCommand.SourceLIFTPath, _simpleGoodLiftContents.Replace("</lift>", "<x/></lift>"));
			Assert.AreEqual(ProgressState.StatusValue.NotStarted, _progress.Status);
			_importCommand.BeginInvoke(_progress);
			WaitForFinish();
		}

		void OnLog(object sender, ProgressState.LogEvent e)
		{
			_log += e.message;
		}

		[Test]
		public void CreatesDb4oFileWhichContainsEntriesAndSenses()
		{
			File.WriteAllText(_importCommand.SourceLIFTPath, _simpleGoodLiftContents);

			_importCommand.BeginInvoke(_progress);
			WaitForFinish();
			using(IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(_importCommand.DestinationDatabasePath))
			{
				IList<LexEntry> x = db.Query<LexEntry>();
				Assert.AreEqual(2, x.Count);
				Assert.AreEqual("one", x[1].Id); //got the wrong order here
				Assert.AreEqual(1, x[1].Senses.Count); // sensitive to order (shame)
			}
			Assert.AreEqual(ProgressState.StatusValue.Finished, _progress.Status);
		}

		[Test]
		public void OkIfNoExistingDbToBackup()
		{
			File.Delete(BackupPath);
			MakeBackupOfExistingDBCore(BackupPath);
		}

		[Test]
		public void OkIfHasExistingDbToBackup()
		{
			File.WriteAllText(_importCommand.DestinationDatabasePath, "old current");
			File.Delete(BackupPath);
			MakeBackupOfExistingDBCore(BackupPath);
			Assert.IsTrue(File.Exists(BackupPath));
			Assert.IsTrue(File.ReadAllText(BackupPath) == "old current");
		}

		[Test]
		public void OkIfHasExistingBackupToRemoveFirst()
		{

			File.WriteAllText(BackupPath, "old backup");
			File.WriteAllText(_importCommand.DestinationDatabasePath, "old current");
			MakeBackupOfExistingDBCore(BackupPath);
			Assert.IsTrue(File.Exists(BackupPath));
			Assert.IsTrue(File.ReadAllText(BackupPath) == "old current");
		}

		[Test]
		public void MakesBackupOfExistingDB()
		{
			//just in case
			File.Delete(BackupPath);

			MakeBackupOfExistingDBCore(BackupPath);
		}

		private void MakeBackupOfExistingDBCore(string backupPath)
		{
			File.WriteAllText(_importCommand.SourceLIFTPath, _simpleGoodLiftContents);
			_importCommand.BeginInvoke(_progress);
			WaitForFinish();
		}


		[Test]
		public void ClearIncrementalBackupCache_Command_DeletesIt()
		{
			string dir = WeSay.Project.WeSayWordsProject.Project.PathToLiftBackupDir;
			Directory.CreateDirectory(dir);
			string deleteThisGuy = Path.Combine(dir, "deleteMe.txt");
			File.WriteAllText(deleteThisGuy, "hello");
			ImportLIFTCommand.ClearTheIncrementalBackupDirectory();
			Assert.IsFalse(File.Exists(deleteThisGuy));
		}

		[Test]
		public void ClearsIncrementalBackupCache()
		{
			string dir = WeSay.Project.WeSayWordsProject.Project.PathToLiftBackupDir;
			Directory.CreateDirectory(dir);
			string deleteThisGuy = Path.Combine(dir, "deleteMe.txt");
			File.WriteAllText(deleteThisGuy,"doesn't matter");
			File.WriteAllText(_importCommand.SourceLIFTPath, _simpleGoodLiftContents);

			_importCommand.BeginInvoke(_progress);
			WaitForFinish();
			Assert.IsFalse(File.Exists(deleteThisGuy));
	   }

		//jh added
		public void WaitForFinish()
		{
			while (!this._finished)
			{
				Application.DoEvents();
				Thread.Sleep(5);
			}
		}
	}

}