using System;
using System.IO;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	public class HelperForTestsRequiringDb4o: IDisposable
	{
		private readonly string _filePath;
		private readonly LexEntryRepository _manager;

		public HelperForTestsRequiringDb4o()
		{
			_filePath = Path.GetTempFileName();
			_manager = new LexEntryRepository(new DoNothingModelConfiguration(), _filePath);
			Lexicon.Init(RecordListManager);
		}

		public LexEntryRepository RecordListManager
		{
			get { return _manager; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			RecordListManager.Dispose();
			File.Delete(_filePath);
		}

		#endregion
	}
}