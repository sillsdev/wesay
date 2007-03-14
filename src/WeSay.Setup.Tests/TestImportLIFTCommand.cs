using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MultithreadProgress;
using NUnit.Framework;
using WeSay.Foundation.Progress;

namespace WeSay.Setup.Tests
{
	[TestFixture]
	public class TestImportLIFTCommand
	{
		private string _outputPath;
		private string _sourcePath;
		private ProgressDialogHandler _progressHandler;
		private ImportLIFTCommand _command;
		private bool _finished;
		private ProgressState _progress;
		private string _backupPath;

		[SetUp]
		public void Setup()
		{
			WeSay.Project.WeSayWordsProject.InitializeForTests();
			_outputPath = Path.GetTempFileName();
			_sourcePath = Path.GetTempFileName();
			_command = new ImportLIFTCommand(_outputPath, _sourcePath);
			_progressHandler = new ProgressDialogHandler(new System.Windows.Forms.Form(), _command);
			_progressHandler.Finished += new EventHandler(_progressHandler_Finished);
			_progress = new ProgressState(_progressHandler);
			_finished=false;
			_backupPath = _outputPath + ".bak";

		}

		private void _progressHandler_Finished(object sender, EventArgs e)
		{
			_finished = true;
		}

		[TearDown]
		public void TearDown()
		{
			_progress.Dispose();
			File.Delete(_outputPath);
			File.Delete(_sourcePath);
			string dirToEmptyOfBackupDirs = Directory.GetParent(WeSay.Project.WeSayWordsProject.Project.PathToLiftBackupDir).FullName;
			string[] backUpDirs = Directory.GetDirectories(dirToEmptyOfBackupDirs, "*incremental*");
			foreach (string dir in backUpDirs)
			{
				Directory.Delete(dir,true);
			}
		}

		[Test]
		public void WritesSomething()
		{
			File.WriteAllText(_sourcePath, "<?xml version='1.0' encoding='utf-8'?><lift/>");

			_command.BeginInvoke(_progress);
			WaitForFinish();
			Assert.IsTrue(File.ReadAllText(_outputPath).Length > 10);
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
			File.WriteAllText(_outputPath, "old current");
			File.Delete(_backupPath);
			MakeBackupOfExistingDBCore(_backupPath);
			Assert.IsTrue(File.Exists(_backupPath));
			Assert.IsTrue(File.ReadAllText(_backupPath) == "old current");
		}

		[Test]
		public void OkIfHasExistingBackupToRemoveFirst()
		{
			File.WriteAllText(_backupPath, "old backup");
			File.WriteAllText(_outputPath, "old current");
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
			File.WriteAllText(_sourcePath, "<?xml version='1.0' encoding='utf-8'?><lift/>");
			_command.BeginInvoke(_progress);
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
			File.WriteAllText(_sourcePath, "<?xml version='1.0' encoding='utf-8'?><lift/>");

			_command.BeginInvoke(_progress);
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