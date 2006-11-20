using System.Collections;
using System.IO;
using NUnit.Framework;
using WeSay.App;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class BackupTests
	{
		protected string _dbFile;
		protected string _directory;
		protected Db4oDataSource _dataSource;
		private Db4oRecordList<LexEntry> _records;
		private BackupService _service;

		[SetUp]
		public void Setup()
		{
			_dbFile = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_dbFile);
			Db4oLexModelHelper.Initialize(_dataSource.Data);
			this._records = new Db4oRecordList<LexEntry>(this._dataSource);

			this._directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(_directory);

			_service = new BackupService(_directory, _dataSource);

		}

		[TearDown]
		public void TearDown()
		{
			_records.Dispose();
			_dataSource.Dispose();
			Directory.Delete(this._directory,true);
			File.Delete(_dbFile);
		}

		[Test]
		public void MissingFileAndEmptyRecordList()
		{
			IList newGuys = _service.GetRecordsNeedingBackup();

			Assert.AreEqual(_dataSource.Data.Query<LexEntry>().Count, newGuys.Count);
		}

		[Test]
		public void MissingBackupFileWouldBackUpAllRecords()
		{
			_records.Add(new LexEntry());
			_records.Add(new LexEntry());

			IList newGuys= _service.GetRecordsNeedingBackup();
			Assert.AreEqual(_records.Count, newGuys.Count);
		}

		[Test]
		public void WouldBackUpOnlyNewRecords()
		{
			_records.Add(new LexEntry());
			_records.Add(new LexEntry());
			_service.DoIncrementalXmlBackupNow();

			_records.Add(new LexEntry());
			_records.Add(new LexEntry());
			_records.Add(new LexEntry());

			IList newGuys = _service.GetRecordsNeedingBackup();
			Assert.AreEqual(3, newGuys.Count);
		}

	}

}