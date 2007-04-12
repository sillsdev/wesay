using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class LiftUpdateTests
	{
		protected string _dbFile;
		protected string _directory;
		protected Db4oDataSource _dataSource;
		private Db4oRecordList<LexEntry> _records;
		private LiftUpdateService _service;
		private Dictionary<string, Guid> _guidDictionary = new Dictionary<string, Guid>();

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			_dbFile = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_dbFile);
			Db4oLexModelHelper.Initialize(_dataSource.Data);
			this._records = new Db4oRecordList<LexEntry>(this._dataSource);

			this._directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(_directory);

			_service = new LiftUpdateService(_dataSource);

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
			IList newGuys = _service.GetRecordsNeedingUpdateInLift();

			Assert.AreEqual(_dataSource.Data.Query<LexEntry>().Count, newGuys.Count);
		}

//        [Test]
//        public void MissingLIFTFileWouldUpdateAllRecords()
//        {
//            _records.Add(new LexEntry());
//            _records.Add(new LexEntry());
//
//            IList newGuys= _service.GetRecordsNeedingUpdateInLift();
//            Assert.AreEqual(_records.Count, newGuys.Count);
//        }

		[Test]
		public void WouldUpdateOnlyNewRecords()
		{
			_records.Add(new LexEntry());
			_records.Add(new LexEntry());
			_service.DoLiftUpdateNow(false);

			_records.Add(new LexEntry());
			_records.Add(new LexEntry());
			_records.Add(new LexEntry());

			IList newGuys = _service.GetRecordsNeedingUpdateInLift();
			Assert.AreEqual(3, newGuys.Count);
		}



		[Test]
		public void DeletionIsRecorded()
		{
			SetupDeletionSituation();
			Assert.AreEqual(1,
							GetLiftDoc().SelectNodes("//entry[@id='boo' and @dateDeleted]")
								.Count);
		}

		private void SetupDeletionSituation()
		{
			LiftIO.Utilities.CreateEmptyLiftFile(Project.WeSayWordsProject.Project.PathToLiftFile, "test", true);
			_records.Add(new LexEntry());
			LexEntry entryToDelete = MakeEntry("boo");
			_records.Add(new LexEntry());

			_service.DoLiftUpdateNow(false);

			//now delete it
			_records.Remove(entryToDelete);
			//this deletion even comes from a higher-level class we aren't using, so we raise it ourselves here:
			_service.OnDataDeleted(this, new DeletedItemEventArgs(entryToDelete));
			_service.DoLiftUpdateNow(true);
		}

		[Test]
		public void DeletionIsExpungedIfSameIdReused()
		{
			SetupDeletionSituation();

			//now make an entry with the same id and add it
			MakeEntry("boo");
			_service.DoLiftUpdateNow(true);
			Assert.AreEqual(0, GetLiftDoc().SelectNodes("//entry[@id='boo' and @dateDeleted]").Count);
			Assert.AreEqual(1, GetLiftDoc().SelectNodes("//entry[@id='boo' and not(@dateDeleted)]").Count);
		}

		private LexEntry MakeEntry(string id)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["zzz"] = id;

			Guid g;
			if (!_guidDictionary.TryGetValue(id, out g))
			{
				g = Guid.NewGuid();
				_guidDictionary.Add(id,g);
			}

			entry.Guid = g;
			_records.Add(entry);
			return entry;
		}

		private XmlDocument GetLiftDoc()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(Project.WeSayWordsProject.Project.PathToLiftFile);// _service.PathToBaseLiftFile);
			//Console.WriteLine(doc.OuterXml);
			return doc;
		}


//
//        [Test]
//        public void BackupAfterImportCrashOriginal()
//        {
//            string path = @"C:\WeSay\SampleProjects\Thai\wesay\tiny.words";
//            WeSayWordsProject project = new WeSayWordsProject();
//            project.LoadFromLiftLexiconPath(path);
//            IRecordListManager recordListManager;
//            recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), project.PathToDb4oLexicalModelDB);
//            Db4oLexModelHelper.Initialize(((Db4oRecordListManager)recordListManager).DataSource.Data);
//            Db4oRecordListManager ds = recordListManager as Db4oRecordListManager;
//            BackupService backupService = new BackupService(project.PathToLocalBackup, ds.DataSource);
//            ds.DataCommitted += new EventHandler(backupService.OnDataCommitted);
//            backupService.DoLiftUpdateNow();
//        }
	}

}