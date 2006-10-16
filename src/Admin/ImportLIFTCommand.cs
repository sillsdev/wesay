using System.IO;
using MultithreadProgress;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay
{
	public class ImportLIFTCommand : BasicCommand
	{
		protected string _destinationDatabasePath;
		protected string _sourceLIFTPath;
		private LiftImporter _importer;

		public ImportLIFTCommand(string destinationDatabasePath, string sourceLIFTPath)
		{
			_destinationDatabasePath = destinationDatabasePath;
			_sourceLIFTPath =sourceLIFTPath;
		}

		protected override void DoWork(
			InitializeProgressCallback initializeCallback,
			ProgressCallback progressCallback,
			StatusCallback primaryStatusTextCallback,
			StatusCallback secondaryStatusTextCallback
			)
		{
			if (File.Exists(_destinationDatabasePath)) // make backup of the file we're about to over-write
			{
				primaryStatusTextCallback.Invoke("Backing up existing file...");
				File.Move(_destinationDatabasePath, _destinationDatabasePath + ".bak");
			}

			primaryStatusTextCallback.Invoke("Importing...");
			using (Db4oDataSource ds = new WeSay.Data.Db4oDataSource(_destinationDatabasePath))
			{
				using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
				{
				   // initializeCallback(0, entries.Count);

					_importer = new LiftImporter(entries);
					_importer.Progress += new System.EventHandler(importer_Progress);
					_importer.ReadFile(_sourceLIFTPath);
				}
			}
		}

		void importer_Progress(object sender, System.EventArgs e)
		{
			ProgressEventArgs p = (ProgressEventArgs) e;
			if (p.Progress == 0)
			{
				_initializeCallback(0, _importer.NodesToImport);
			}
		   p.Cancel = this.Canceling;
			_progressCallback(p.Progress);
		}
	}
}