using System;
using System.IO;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	public class HelperForTestsRequiringDb4o: IDisposable
	{
		private readonly string _filePath;
		private readonly Db4oRecordListManager _manager;

		public HelperForTestsRequiringDb4o()
		{
			_filePath = Path.GetTempFileName();
			_manager = new Db4oRecordListManager(new DoNothingModelConfiguration(), _filePath);
			Lexicon.Init(RecordListManager);
		}

		public Db4oRecordListManager RecordListManager
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