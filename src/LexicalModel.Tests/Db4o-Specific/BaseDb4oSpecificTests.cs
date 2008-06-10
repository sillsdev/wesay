using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel.Tests
{
	public class BaseDb4oSpecificTests
	{
		internal Db4oRecordList<LexEntry> _entriesList;
		protected Db4oDataSource _dataSource;
		protected string _filePath ;
		protected Db4oRecordListManager _recordListManager=null;

		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			if (_entriesList != null)
			{
				this._entriesList.Dispose();
			}
			if (_recordListManager != null)
			{
				_recordListManager.Dispose();
			}
			if (_dataSource != null)
			{
				this._dataSource.Dispose();
			}

			if (_filePath != "")
			{
				File.Delete(_filePath);
			}
		}

		protected LexEntry CycleDatabase()
		{
			if (_recordListManager != null)
			{
				_entriesList.Dispose();
				_dataSource.Dispose();
				_recordListManager.Dispose();
			}
			   _recordListManager = new Db4oRecordListManager(new DoNothingModelConfiguration(), _filePath);

			   _dataSource = _recordListManager.DataSource;
			Db4oLexModelHelper.Initialize(_dataSource.Data);
			Lexicon.Init(_recordListManager);

			_entriesList = new Db4oRecordList<LexEntry>(_dataSource);
			if (_entriesList.Count > 0)
				return _entriesList[0];
			else
				return null;
		}
	}
}