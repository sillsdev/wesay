using System;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.Project;

namespace WeSay
{
	public class ImportLIFTCommand : BasicCommand
	{
		private string _sourceLIFTPath;
		protected ProgressState _progress;

		public ImportLIFTCommand(string sourceLIFTPath)
		{
			_sourceLIFTPath = sourceLIFTPath;
		}


		public string SourceLIFTPath
		{
			get
			{
				return _sourceLIFTPath;
			}

		}

		protected override void DoWork2(ProgressState progress)
		{
			CacheBuilder b = new CacheBuilder(_sourceLIFTPath);
			b.DoWork(progress);
		}

		protected override void DoWork(InitializeProgressCallback initializeCallback, ProgressCallback progressCallback,
									   StatusCallback primaryStatusTextCallback,
									   StatusCallback secondaryStatusTextCallback)
		{
			throw new NotImplementedException();
		}
	}

}