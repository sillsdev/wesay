using System.ComponentModel;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel.Db4oSpecific;

namespace WeSay.LexicalModel.Tests.Db4oSpecific
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
			_filePath = Path.GetTempFileName();
			CycleDatabase();
		}

		[TearDown]
		public void TearDown()
		{
			if (_lexEntryRepository != null)
			{
				_lexEntryRepository.Dispose();
			}

			if (_filePath != "")
			{
				File.Delete(_filePath);
			}
		}

		protected void CycleDatabase()
		{
			if (_lexEntryRepository != null)
			{
				_lexEntryRepository.Dispose();
			}
			_lexEntryRepository = new LexEntryRepository(_filePath);
		}

		private LexEntry GetFirstEntry()
		{
			RepositoryId[] repositoryIds = _lexEntryRepository.GetAllEntries();
			if (repositoryIds != null && repositoryIds.Length > 0)
			{
				return _lexEntryRepository.GetItem(repositoryIds[0]);
			}
			return null;
		}

		private void _entry_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_didNotify = true;
		}

		[Test]
		public void HelperGetsActivationCall()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative("en", "test");

			LexSense sense = (LexSense) entry.Senses.AddNew();
			_lexEntryRepository.SaveItem(entry);

			Assert.AreEqual(entry, sense.Parent);

			int activations = Db4oLexModelHelper.Singleton.ActivationCount;
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual(1, _lexEntryRepository.CountAllEntries());
			Assert.AreEqual(1, entry.Senses.Count);
			Assert.AreEqual(activations + 1 /*entry*/+ 1 /*sense*/,
							Db4oLexModelHelper.Singleton.ActivationCount);
		}

		[Test]
		public void EntryOnlyActivatedOnce()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			_lexEntryRepository.SaveItem(entry);

			int activations = Db4oLexModelHelper.Singleton.ActivationCount;
			CycleDatabase();
			GetFirstEntry();
			Assert.AreEqual(activations + 1, Db4oLexModelHelper.Singleton.ActivationCount);
			//get the same entry again
			_lexEntryRepository.GetAllEntries();
			Assert.AreEqual(activations + 1, Db4oLexModelHelper.Singleton.ActivationCount);
		}

		[Test]
		public void ShallowChange()
		{
			_lexEntryRepository.CreateItem();

			CycleDatabase();
			LexEntry entry = GetFirstEntry();
			Assert.AreEqual(1, _lexEntryRepository.CountAllEntries());
			entry.LexicalForm["en"] = "x";
			_lexEntryRepository.SaveItem(entry);
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
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
			_lexEntryRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("test", entry.GetOrCreateProperty<MultiText>("testField")["en"]);
		}

		[Test]
		public void SaveOptionRefField()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			//Option z = new Option("test", "t", Guid.NewGuid());
			entry.GetOrCreateProperty<OptionRef>("testOption").Value = "test";
			_lexEntryRepository.SaveItem(entry);
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("test", entry.GetOrCreateProperty<OptionRef>("testOption").Value);
		}

		[Test]
		public void DeepChange()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm["en"] = "12";
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			_lexEntryRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			((LexExampleSentence) ((LexSense) entry.Senses[0]).ExampleSentences[0]).Sentence["th"] =
					"sawadee";
			_lexEntryRepository.SaveItem(entry);
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("sawadee",
							((LexExampleSentence) ((LexSense) entry.Senses[0]).ExampleSentences[0]).
									Sentence["th"]);
		}

		[Test]
		public void DeepNotifyAfterDepersist()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			_lexEntryRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			entry.PropertyChanged += _entry_PropertyChanged;
			_didNotify = false;
			((LexExampleSentence) ((LexSense) entry.Senses[0]).ExampleSentences[0]).Sentence["th"] =
					"sawadeekap";
			Assert.IsTrue(_didNotify);
		}
	}
}