using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Db4objects.Db4o;
using MultithreadProgress;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.Setup.Tests
{
	[TestFixture]
	public class TestExportLIFTCommand
	{
		private string _sourceDb4oPath;
		private string _destLiftPath;
		private ProgressDialogHandler _progressHandler;
		private ExportLIFTCommand _exportCommand;
		private bool _finished;
		private ProgressState _progress;
		private string _log;

		[SetUp]
		public void Setup()
		{
			WeSay.Project.WeSayWordsProject.InitializeForTests();
			_sourceDb4oPath = Path.GetTempFileName();
			_destLiftPath = Path.GetTempFileName();

			MakeLittleDbToExport();

			_exportCommand = new ExportLIFTCommand(_destLiftPath, _sourceDb4oPath);

			_progressHandler = new ProgressDialogHandler(new System.Windows.Forms.Form(), _exportCommand);
			_progressHandler.Finished += new EventHandler(_progressHandler_Finished);
			_progress = new ProgressState(_progressHandler);
			_finished = false;
		}

		private void MakeLittleDbToExport()
		{
			using (Db4oDataSource db = new Db4oDataSource(_sourceDb4oPath))
			{
				Db4oLexModelHelper.Initialize(db.Data);
				LexEntry entry = new LexEntry();
				entry.LexicalForm["red"] = "sunset";
				db.Data.Set(entry);
				Db4oLexModelHelper.Deinitialize(db.Data);
			}
		}

		private void _progressHandler_Finished(object sender, EventArgs e)
		{
			_finished = true;
		}

		[TearDown]
		public void TearDown()
		{
			_progress.Dispose();
			File.Delete(_sourceDb4oPath);
			File.Delete(_destLiftPath);
		}

		public void WaitForFinish()
		{
			while (!this._finished)
			{
				Application.DoEvents();
				Thread.Sleep(5);
			}
		}
		/// <summary>
		/// Details of the export are tested by the exporter tests.
		/// We just want to make sure it runs from the context of this command object.
		/// </summary>
		[Test]
		public void SmokeTest()
		{
				_exportCommand.BeginInvoke(_progress);
				WaitForFinish();
				Assert.AreEqual(ProgressState.StatusValue.Finished, _progress.Status);
		}

	}

}