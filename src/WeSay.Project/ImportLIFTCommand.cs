using System;
using System.IO;
using System.Xml;
using LiftIO;
///using WeSay.App;
using WeSay.App;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay
{
	public class ImportLIFTCommand : BasicCommand
	{
		private string _sourceLIFTPath;
		protected WeSay.Foundation.Progress.ProgressState _progress;

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