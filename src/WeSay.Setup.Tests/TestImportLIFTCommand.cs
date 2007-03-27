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

namespace WeSay.Setup.Tests
{
	[TestFixture]
	public class TestImportLIFTCommand
	{
		private string _outputDb4oPath;
		private string _sourcePath;
		private ProgressDialogHandler _progressHandler;
		private ImportLIFTCommand _importCommand;
		private bool _finished;
		private ProgressState _progress;
		private string _backupPath;
		private string _simpleGoodLiftContents = string.Format("<?xml version='1.0' encoding='utf-8'?><lift version='{0}'><entry id='one'><sense><gloss lang='en'>hello</gloss></sense></entry><entry id='two'/></lift>", LiftIO.Validator.LiftVersion);
		private string _log;

		[SetUp]
		public void Setup()
		{
			WeSay.Project.WeSayWordsProject.InitializeForTests();
			_outputDb4oPath = Path.GetTempFileName();
			_sourcePath = Path.GetTempFileName();
			_importCommand = new ImportLIFTCommand(_outputDb4oPath, _sourcePath);
			_progressHandler = new ProgressDialogHandler(new System.Windows.Forms.Form(), _importCommand);
			_progressHandler.Finished += new EventHandler(_progressHandler_Finished);
			_progress = new ProgressState(_progressHandler);
			_progress.Log += new EventHandler<ProgressState.LogEvent>(OnLog);
			_finished = false;
			_backupPath = _outputDb4oPath + ".bak";

		}

		private void _progressHandler_Finished(object sender, EventArgs e)
		{
			_finished = true;
		}

		[TearDown]
		public void TearDown()
		{
			_progress.Dispose();
			File.Delete(_outputDb4oPath);
			File.Delete(_sourcePath);
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
			File.WriteAllText(_sourcePath, _simpleGoodLiftContents);
			Assert.AreEqual(ProgressState.StatusValue.NotStarted, _progress.Status);
			_importCommand.BeginInvoke(_progress);
			WaitForFinish();
		  //  Console.WriteLine(_log);
			Assert.AreEqual(ProgressState.StatusValue.Finished, _progress.Status, _log);
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
			File.WriteAllText(_sourcePath, _simpleGoodLiftContents.Replace("</lift>", "<x/></lift>"));
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
			File.WriteAllText(_sourcePath, _simpleGoodLiftContents);

			_importCommand.BeginInvoke(_progress);
			WaitForFinish();
			using(IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(_outputDb4oPath))
			{
				IList<LexEntry> x = db.Query<LexEntry>();
				Assert.AreEqual(2, x.Count);
				Assert.AreEqual(1, x[0].Senses.Count); // sensitive to order (shame)
			}
			Assert.AreEqual(ProgressState.StatusValue.Finished, _progress.Status);
		}

		[Test]
		public void OkIfNoExistingDbToBackup()
		{
			File.Delete(_backupPath);
			MakeBackupOfExistingDBCore(_backupPath);
		}

		[Test]
		public void OkIfHasExistingDbToBackup()
		{
			File.WriteAllText(_outputDb4oPath, "old current");
			File.Delete(_backupPath);
			MakeBackupOfExistingDBCore(_backupPath);
			Assert.IsTrue(File.Exists(_backupPath));
			Assert.IsTrue(File.ReadAllText(_backupPath) == "old current");
		}

		[Test]
		public void OkIfHasExistingBackupToRemoveFirst()
		{

			File.WriteAllText(_backupPath, "old backup");
			File.WriteAllText(_outputDb4oPath, "old current");
			MakeBackupOfExistingDBCore(_backupPath);
			Assert.IsTrue(File.Exists(_backupPath));
			Assert.IsTrue(File.ReadAllText(_backupPath) == "old current");
		}

		[Test]
		public void MakesBackupOfExistingDB()
		{
			//just in case
			File.Delete(_backupPath);

			MakeBackupOfExistingDBCore(_backupPath);
		}

		private void MakeBackupOfExistingDBCore(string backupPath)
		{
			File.WriteAllText(_sourcePath, _simpleGoodLiftContents);
			_importCommand.BeginInvoke(_progress);
			WaitForFinish();
		}


		[Test]
		public void ClearIncrementalBackupCache()
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
			File.WriteAllText(_sourcePath, _simpleGoodLiftContents);

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