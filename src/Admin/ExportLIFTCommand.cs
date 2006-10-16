using MultithreadProgress;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay
{
	 public class ExportLIFTCommand : BasicCommand
	{
		protected string _destinationLIFTPath;
		protected string _sourceWordsPath;

		public ExportLIFTCommand(
			string destinationLIFTPath, string sourceWordsPath        )
		{
			_destinationLIFTPath = destinationLIFTPath;
			_sourceWordsPath = sourceWordsPath;
		}

		protected override void DoWork(
			InitializeProgressCallback initializeCallback,
			ProgressCallback progressCallback,
			StatusCallback primaryStatusTextCallback,
			StatusCallback secondaryStatusTextCallback
			)
		{
			primaryStatusTextCallback.Invoke("Exporting...");
		   WeSay.LexicalModel.LiftExporter exporter=null;
			try
			{
				exporter = new LiftExporter(_destinationLIFTPath);

				using (Db4oDataSource ds = new Db4oDataSource(_sourceWordsPath))
				{
					using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
					{
						initializeCallback(0, entries.Count);
						for (int i = 0; i < entries.Count; )
						{
							const int howManyAtATime = 100;
							exporter.Add(entries, i, howManyAtATime);
							i += howManyAtATime;
							progressCallback(i);
							if( Canceling )
							{
								return;
							}
						}
					}
				}
			}
			finally
			{
				exporter.End();
			}
		}


	}
}