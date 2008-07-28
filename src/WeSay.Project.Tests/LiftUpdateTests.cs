//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Threading;
//using System.Xml;
//using LiftIO;
//using NUnit.Framework;
//using WeSay.Data;
//using WeSay.LexicalModel;

//namespace WeSay.Project.Tests
//{
//    [TestFixture]
//    public class LiftUpdateTests
//    {
//        protected string _dbFile;
//        protected string _directory;
//        private LiftUpdateService _service;
//        private readonly Dictionary<string, Guid> _guidDictionary = new Dictionary<string, Guid>();
//        private LexEntryRepository _lexEntryRepository;

//        [SetUp]
//        public void Setup()
//        {
//            BasilProject.Project = null;

//            WeSayWordsProject.InitializeForTests();
//            _dbFile = Path.GetTempFileName();
//            _lexEntryRepository = new LexEntryRepository(_dbFile);

//            _directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
//            Directory.CreateDirectory(_directory);

//            _service = new LiftUpdateService(_lexEntryRepository);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _lexEntryRepository.Dispose();
//            Directory.Delete(_directory, true);
//            File.Delete(_dbFile);
//        }

//        [Test]
//        public void MissingFileAndEmptyRecordList()
//        {
//            IList<RepositoryId> newGuys = _service.GetRecordsNeedingUpdateInLift();

//            Assert.AreEqual(_lexEntryRepository.CountAllItems(), newGuys.Count);
//        }

//        //        [Test]
//        //        public void MissingLIFTFileWouldUpdateAllRecords()
//        //        {
//        //            _records.Add(new LexEntry());
//        //            _records.Add(new LexEntry());
//        //
//        //            IList newGuys= _service.GetRecordsNeedingUpdateInLift();
//        //            Assert.AreEqual(_records.Count, newGuys.Count);
//        //        }

//        [Test]
//        public void WouldUpdateOnlyNewRecords()
//        {
//            MakeEntry();
//            MakeEntry();
//            // Linux and fat32 has resolution of second not millisecond!
//            Thread.Sleep(1000);
//            _service.DoLiftUpdateNow(false);
//            MakeEntry();
//            MakeEntry();
//            MakeEntry();

//            IList<RepositoryId> newGuys = _service.GetRecordsNeedingUpdateInLift();
//            Assert.AreEqual(3, newGuys.Count);
//        }

//        private void MakeEntry()
//        {
//            LexEntry e = _lexEntryRepository.CreateItem();
//            // e.LexicalForm.SetAlternative("abc", id);
//            e.GetOrCreateId(true);
//            _lexEntryRepository.SaveItem(e);
//        }

//        [Test]
//        public void DeletionIsRecorded()
//        {
//            SetupDeletionSituation();
//            int count =
//                    GetLiftDoc().SelectNodes("//entry[contains(@id,'boo_') and @dateDeleted]").Count;
//            if (count != 1)
//            {
//                Debug.WriteLine(GetLiftDoc().OuterXml);
//            }
//            Assert.AreEqual(1, count);
//        }

//        private void SetupDeletionSituation()
//        {
//            Utilities.CreateEmptyLiftFile(WeSayWordsProject.Project.PathToLiftFile, "test", true);
//            MakeEntry();
//            LexEntry entryToDelete = MakeEntry("boo");
//            entryToDelete.GetOrCreateId(true);
//            MakeEntry();

//            WeSayWordsProject.Project.LockLift(); //the next call will expect this to be locked

//            _service.DoLiftUpdateNow(true);
//            // Linux and fat32 has resolution of second not millisecond!
//            Thread.Sleep(1000);

//            //now delete it
//            _lexEntryRepository.DeleteItem(entryToDelete);
//            //this deletion event comes from a higher-level class we aren't using, so we raise it ourselves here:
//            _service.OnDataDeleted(entryToDelete, new EventArgs());
//            _service.DoLiftUpdateNow(true);
//            // Linux and fat32 has resolution of second not millisecond!
//            Thread.Sleep(1000);
//        }

//        [Test]
//        public void DeletionIsExpungedIfSameIdReused()
//        {
//            SetupDeletionSituation();

//            //now make an entry with the same id and add it
//            MakeEntry("boo");
//            _service.DoLiftUpdateNow(true);
//            Assert.AreEqual(0,
//                            GetLiftDoc().SelectNodes(
//                                    "//entry[contains(@id,'boo_') and @dateDeleted]").Count);
//            Assert.AreEqual(1,
//                            GetLiftDoc().SelectNodes(
//                                    "//entry[contains(@id,'boo_') and not(@dateDeleted)]").Count);
//        }

//        private LexEntry MakeEntry(string id)
//        {
//            LexEntry entry = _lexEntryRepository.CreateItem();
//            entry.LexicalForm["zzz"] = id;

//            Guid g;
//            if (!_guidDictionary.TryGetValue(id, out g))
//            {
//                g = Guid.NewGuid();
//                _guidDictionary.Add(id, g);
//            }

//            entry.Guid = g;
//            _lexEntryRepository.SaveItem(entry);
//            return entry;
//        }

//        private static XmlDocument GetLiftDoc()
//        {
//            XmlDocument doc = new XmlDocument();
//            doc.Load(WeSayWordsProject.Project.PathToLiftFile); // _service.PathToBaseLiftFile);
//            //Console.WriteLine(doc.OuterXml);
//            return doc;
//        }

//        //
//        //        [Test]
//        //        public void BackupAfterImportCrashOriginal()
//        //        {
//        //            string path = @"C:\WeSay\SampleProjects\Thai\wesay\tiny.words";
//        //            WeSayWordsProject project = new WeSayWordsProject();
//        //            project.LoadFromLiftLexiconPath(path);
//        //            LexEntryRepository lexEntryRepository;
//        //            lexEntryRepository = new LexEntryRepository(new WeSayWordsDb4oModelConfiguration(), project.PathToRepository);
//        //            Db4oLexModelHelper.Initialize(((LexEntryRepository)lexEntryRepository).DataSource.Data);
//        //            LexEntryRepository ds = lexEntryRepository as LexEntryRepository;
//        //            BackupService backupService = new BackupService(project.PathToLocalBackup, ds.DataSource);
//        //            ds.DataCommitted += new EventHandler(backupService.OnDataCommitted);
//        //            backupService.DoLiftUpdateNow();
//        //        }

//        [Test]
//        public void LiftIsFreshNow_NotLocked()
//        {
//            Thread.Sleep(2000);
//            LiftUpdateService.LiftIsFreshNow();
//            DateTime timeStamp = File.GetLastWriteTimeUtc(WeSayWordsProject.Project.PathToLiftFile);
//            TimeSpan lastWriteTimeSpan = DateTime.UtcNow - timeStamp;
//            Assert.Less(lastWriteTimeSpan.Milliseconds, 2000);
//        }

//        [Test]
//        public void LiftIsFreshNow_Locked()
//        {
//            WeSayWordsProject.Project.LockLift();
//            LiftUpdateService.LiftIsFreshNow();
//            WeSayWordsProject.Project.ReleaseLockOnLift();
//        }
//    }
//}
