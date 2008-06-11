using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel.Tests.Db4o_Specific
{
	[TestFixture]
	public class PersistenceTests
	{
		protected bool _didNotify;

		protected string _filePath;
		protected LexEntryRepository _lexEntryRepository = null;

		[SetUp]
		public void Setup()
		{
			this._filePath = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			if (this._lexEntryRepository != null)
			{
				this._lexEntryRepository.Dispose();
			}

			if (this._filePath != "")
			{
				File.Delete(this._filePath);
			}
		}

		protected void CycleDatabase()
		{
			if (this._lexEntryRepository != null)
			{
				this._lexEntryRepository.Dispose();
			}
			this._lexEntryRepository = new LexEntryRepository(this._filePath);
		}

		private LexEntry GetFirstEntry()
		{
			RepositoryId[] repositoryIds = this._lexEntryRepository.GetAllEntries();
			if (repositoryIds != null && repositoryIds.Length > 0)
			{
				return this._lexEntryRepository.GetItem(repositoryIds[0]);
			}
			return null;
		}

		void _entry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this._didNotify = true;
		}


		[Test]
		public void HelperGetsActivationCall()
		{
			LexEntry entry = this._lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative("en", "test");

			LexSense sense = (LexSense) entry.Senses.AddNew();
			this._lexEntryRepository.SaveItem(entry);

			Assert.AreEqual(entry,sense.Parent);

			int activations = Db4oLexModelHelper.Singleton.ActivationCount;
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual(1, this._lexEntryRepository.CountAllEntries());
			Assert.AreEqual(1, entry.Senses.Count);
			Assert.AreEqual(activations + 1 /*entry*/ + 1 /*sense*/, Db4oLexModelHelper.Singleton.ActivationCount);

		}

		[Test]
		public void EntryOnlyActivatedOnce()
		{
			LexEntry entry = this._lexEntryRepository.CreateItem();
			this._lexEntryRepository.SaveItem(entry);

			int activations = Db4oLexModelHelper.Singleton.ActivationCount;
			CycleDatabase();
			GetFirstEntry();
			Assert.AreEqual(activations + 1, Db4oLexModelHelper.Singleton.ActivationCount);
			//get the same entry again
			this._lexEntryRepository.GetAllEntries();
			Assert.AreEqual(activations + 1, Db4oLexModelHelper.Singleton.ActivationCount);
		}

		[Test]
		public void ShallowChange()
		{
			LexEntry entry = this._lexEntryRepository.CreateItem();
			this._lexEntryRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual(1, this._lexEntryRepository.CountAllEntries());
			entry.LexicalForm["en"] = "x";
			CycleDatabase();
			entry = GetFirstEntry();
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

		[Test]
		public void SaveCustomTextField()
		{
			LexEntry entry = this._lexEntryRepository.CreateItem();
			entry.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
			this._lexEntryRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("test", entry.GetOrCreateProperty<MultiText>("testField")["en"]);
		}

		[Test]
		public void SaveOptionRefField()
		{
			LexEntry entry = this._lexEntryRepository.CreateItem();
			//Option z = new Option("test", "t", Guid.NewGuid());
			entry.GetOrCreateProperty<OptionRef>("testOption").Value = "test";
			this._lexEntryRepository.SaveItem(entry);
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("test", entry.GetOrCreateProperty<OptionRef>("testOption").Value);
		}

		[Test]
		public void DeepChange()
		{
			LexEntry entry = this._lexEntryRepository.CreateItem();
			entry.LexicalForm["en"] = "12";
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example= (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			this._lexEntryRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence["th"] = "sawadee";
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("sawadee", ((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence["th"]);
		}

		[Test]
		public void DeepNotifyAfterDepersist()
		{
			LexEntry entry = this._lexEntryRepository.CreateItem();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example= (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			this._lexEntryRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			entry.PropertyChanged += _entry_PropertyChanged;
			this._didNotify = false;
			((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence["th"]="sawadeekap";
			Assert.IsTrue(this._didNotify);
		}


	}
}