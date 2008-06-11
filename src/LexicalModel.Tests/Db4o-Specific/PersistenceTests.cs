using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel.Tests
{
	using System.IO;

	[TestFixture]
	public class PersistenceTests : BaseDb4oSpecificTests
	{
		protected bool _didNotify;


		void _entry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_didNotify = true;
		}


		[Test]
		public void HelperGetsActivationCall()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative("en", "test");
			_entriesList.Add(entry);

			LexSense sense = (LexSense) entry.Senses.AddNew();

			Assert.AreEqual(entry,sense.Parent);

			int activations = Db4oLexModelHelper.Singleton.ActivationCount;
			entry = CycleDatabase();
			Assert.AreEqual(1, _entriesList.Count);
			Assert.AreEqual(1, entry.Senses.Count);
			Assert.AreEqual(activations + 1 /*entry*/ + 1 /*sense*/, Db4oLexModelHelper.Singleton.ActivationCount);

		}

		[Test]
		public void EntryOnlyActivatedOnce()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			_entriesList.Add(entry);

			int activations = Db4oLexModelHelper.Singleton.ActivationCount;
			CycleDatabase();
			Assert.AreEqual(activations + 1, Db4oLexModelHelper.Singleton.ActivationCount);
			//get the same entry again
			_dataSource.Data.Query<LexEntry>();
			Assert.AreEqual(activations+1, Db4oLexModelHelper.Singleton.ActivationCount);
		}

		[Test]
		public void ShallowChange()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			_entriesList.Add(entry);
			entry = CycleDatabase();
			Assert.AreEqual(1, _entriesList.Count);
			entry.LexicalForm["en"] = "x";
			entry = CycleDatabase();
			Assert.AreEqual("x", entry.LexicalForm["en"]);
		}


	[Test]
	public void TempCreateLittleFile()
	{
		string path = Path.GetTempFileName();
		using (Db4oDataSource ds = new Db4oDataSource(path))
		{
			if (Db4oLexModelHelper.Singleton != null)
			{
				Db4oLexModelHelper.Singleton.Dispose();
			}

			using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
			{
				Db4oLexModelHelper.Initialize(ds.Data);
				LexEntry one = new LexEntry();
				//one.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
				entries.Add(one);
				LexEntry two = new LexEntry();
				// two.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
				entries.Add(two);
			}
			ds.Data.Commit();
			ds.Data.Close();
		}
		File.Delete(path);
	}

//    [Test]
//    public void QueryAfterImportCrash()
//    {
//        string path = @"C:\WeSay\SampleProjects\Thai\wesay\tiny.words";
//        using (LexEntryRepository recordListManager = new LexEntryRepository(new WeSayWordsDb4oModelConfiguration(), path))
//        {
//            LexEntryRepository listManager = recordListManager as LexEntryRepository;
//
//            IQuery q = listManager.DataSource.Data.Query();
//            q.Constrain(typeof (LexEntry));
//            IList records = q.Execute();
//
//            LexEntry entry = (LexEntry) records[0];
////            listManager.DataSource.Data.Deactivate(entry, 99);
////            listManager.DataSource.Data.Activate(entry, 99);
//            LexSense sense = (LexSense) entry.Senses[0];
//         //   listManager.DataSource.Data.Activate(sense, 99);
//            CheckSense(sense);
//        }
//    }

//    [Test]
//    public void GetAfterImportCrash()
//    {
//        string path = @"C:\WeSay\SampleProjects\Thai\wesay\tiny.words";
//        using (LexEntryRepository recordListManager = new LexEntryRepository(new WeSayWordsDb4oModelConfiguration(), path))
//        {
//            LexEntryRepository listManager = recordListManager as LexEntryRepository;
//
//            IList records = new WeSay.Data.Db4oRecordList<LexEntry>(listManager.DataSource);
//
//            LexEntry entry = (LexEntry)records[0];
//            LexSense sense = (LexSense)entry.Senses[0];
//            CheckSense(sense);
//        }
//    }
//
//    private void CheckSense(LexSense sense)
//    {
//        Assert.IsNotNull(sense.Properties);
//        Assert.AreNotEqual(0, sense.Properties.Count);
//        foreach (KeyValuePair<string, object> pair in sense.Properties)
//        {
//            if (pair.Value is OptionRefCollection)
//            {
//                OptionRefCollection c = pair.Value as OptionRefCollection;
//                Assert.AreEqual(2, c.Keys.Count);
//            }
//        }
//    }
//
//        [Test]
//        public void LoadAfterImportCrash()
//        {
//            _filePath = @"C:\WeSay\SampleProjects\Thai\wesay\tiny.words";
//
//            LexEntry entry= CycleDatabase();
//            _filePath = "";
//            LexSense sense = (LexSense)entry.Senses[0];
//            Assert.IsNotNull(sense.Properties);
//            Assert.AreNotEqual(0, sense.Properties.Count);
//            foreach (KeyValuePair<string, object> pair in sense.Properties)
//            {
//                if (pair.Value is OptionRefCollection)
//                {
//                    OptionRefCollection c = pair.Value as OptionRefCollection;
//                    Assert.AreEqual(2, c.Keys.Count);
//                }
//            }
//            _filePath = "";
//        }
//
//        [Test]
//        public void DeletionCausesPropertyDictionaryCorruption2()
//        {
//            _filePath = Path.GetTempFileName();
//            using (Db4oDataSource ds = new WeSay.Data.Db4oDataSource(_filePath))
//            {
//                if (Db4oLexModelHelper.Singleton != null)
//                {
//                    Db4oLexModelHelper.Singleton.Dispose();
//                }
//
//                using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
//                {
//                    Db4oLexModelHelper.Initialize(ds.Data);
//                    LexEntry one = new LexEntry();
//                    //one.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
//                    entries.Add(one);
//                    LexEntry two = new LexEntry();
//                    // two.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
//                    entries.Add(two);
//                }
//                ds.Data.Commit();
//                ds.Data.Close();
//            }
//            Db4oLexModelHelper.Singleton.Dispose();
//
//            CycleDatabase();
//            LexEntry first = _entriesList[0];
//            _entriesList.Remove(first);
//           // LexEntry entry = CycleDatabase();
//            LexEntry entry = _entriesList[0];
//            entry.Properties.Add("die!", new OptionRef());
//        }
//
//        [Test]
//        public void DeletionCausesPropertyDictionaryCorruption3()
//        {
//            string path = Path.GetTempFileName();
//            using (Db4oDataSource ds = new WeSay.Data.Db4oDataSource(path))
//            {
//                if (Db4oLexModelHelper.Singleton != null)
//                {
//                    Db4oLexModelHelper.Singleton.Dispose();
//                }
//
//                using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
//                {
//                    Db4oLexModelHelper.Initialize(ds.Data);
//                    LexEntry one = new LexEntry();
//                    //one.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
//                    entries.Add(one);
//                    LexEntry two = new LexEntry();
//                   // two.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
//                    entries.Add(two);
//
//
//                }
//                ds.Data.Commit();
//                ds.Data.Close();
//            }
//
//            Db4oLexModelHelper.Singleton.Dispose();
//            using (Db4oDataSource ds = new WeSay.Data.Db4oDataSource(path))
//            {
//                using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
//                {
//                    Db4oLexModelHelper.Initialize(ds.Data);
//                    LexEntry one = entries[0];
//                    entries.Remove(one);
//                    LexEntry two = entries[0];
//                    two.Properties.Add("die!", new OptionRef());
//                }
//            }
//            File.Delete(path);
//            CycleDatabase();//so teardown doesn't fail
//        }
//
//        [Test]
//        public void DeletionCausesPropertyDictionaryCorruption4()
//        {
//            CycleDatabase();
//            LexEntry one = new LexEntry();
//            one.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
//            _entriesList.Add(one);
//            LexEntry two = new LexEntry();
//            two.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
//            _entriesList.Add(two);
//
//            LexEntry entry = CycleDatabase();
//            _entriesList.Remove(entry);
//            entry = CycleDatabase();
//
//            entry.Properties.Add("die!", new OptionRef());
//        }


		[Test]
		public void SaveCustomTextField()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			entry.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
			_entriesList.Add(entry);
			entry = CycleDatabase();
			Assert.AreEqual("test", entry.GetOrCreateProperty<MultiText>("testField")["en"]);
		}

		[Test]
		public void SaveOptionRefField()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			//Option z = new Option("test", "t", Guid.NewGuid());
			entry.GetOrCreateProperty<OptionRef>("testOption").Value = "test";
			_entriesList.Add(entry);
			entry = CycleDatabase();
			Assert.AreEqual("test", entry.GetOrCreateProperty<OptionRef>("testOption").Value);
		}

		[Test]
		public void DeepChange()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			entry.LexicalForm["en"] = "12";
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example= (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			_entriesList.Add(entry);
			entry = CycleDatabase();
			((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence["th"]="sawadee";
			entry = CycleDatabase();
			Assert.AreEqual("sawadee", ((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence["th"]);
	   }

		[Test]
		public void DeepNotifyAfterDepersist()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example= (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			_entriesList.Add(entry);
			entry = CycleDatabase();
			entry.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_entry_PropertyChanged);
			_didNotify = false;
			((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence["th"]="sawadeekap";
			Assert.IsTrue(_didNotify);
		}


	}
}
