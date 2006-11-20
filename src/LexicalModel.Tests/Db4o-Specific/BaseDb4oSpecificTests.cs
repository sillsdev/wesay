using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel.Tests
{
	public class BaseDb4oSpecificTests
	{
		protected Db4oRecordList<LexEntry> _entriesList;
		protected Db4oDataSource _dataSource;
		private string _filePath ;

		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			this._entriesList.Dispose();
			this._dataSource.Dispose();
			File.Delete(_filePath);
		}

		protected LexEntry CycleDatabase()
		{
			if (_dataSource != null)
			{
				_entriesList.Dispose();
				_dataSource.Dispose();
			}
			_dataSource = new WeSay.Data.Db4oDataSource(_filePath);
			Db4oLexModelHelper.Initialize(_dataSource.Data);

			_entriesList = new WeSay.Data.Db4oRecordList<LexEntry>(_dataSource);
			if (_entriesList.Count > 0)
				return _entriesList[0];
			else
				return null;
		}
	}
}