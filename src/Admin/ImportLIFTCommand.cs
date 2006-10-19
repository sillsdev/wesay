using System;
using System.IO;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;

namespace WeSay
{
	public class ImportLIFTCommand : BasicCommand
	{
		protected string _destinationDatabasePath;
		protected string _sourceLIFTPath;
		private LiftImporter _importer;
		protected WeSay.Foundation.Progress.ProgressState _progress;

		public ImportLIFTCommand(string destinationDatabasePath, string sourceLIFTPath)
		{
			_destinationDatabasePath = destinationDatabasePath;
			_sourceLIFTPath =sourceLIFTPath;
		}

		protected override void DoWork2(ProgressState progress)
		{
			_progress = progress;
			if (File.Exists(_destinationDatabasePath)) // make backup of the file we're about to over-write
			{
				progress.Status = "Backing up existing file...";
				File.Move(_destinationDatabasePath, _destinationDatabasePath + ".bak");
			}

			progress.Status = "Importing...";
			using (Db4oDataSource ds = new WeSay.Data.Db4oDataSource(_destinationDatabasePath))
			{
				using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
				{
					_importer = new LiftImporter(entries);
					_importer.Progress = progress;
					_importer.ReadFile(_sourceLIFTPath);
				}
			}
		}

		protected override void DoWork(InitializeProgressCallback initializeCallback, ProgressCallback progressCallback,
									   StatusCallback primaryStatusTextCallback,
									   StatusCallback secondaryStatusTextCallback)
		{
			throw new NotImplementedException();
		}
	}

}