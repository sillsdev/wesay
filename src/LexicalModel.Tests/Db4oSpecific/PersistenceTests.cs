using System.ComponentModel;
using System.IO;
using Db4objects.Db4o.Events;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;

namespace WeSay.LexicalModel.Tests.Db4oSpecific
{
	[TestFixture]
	public class PersistenceTests
	{
		protected bool _didNotify;

		protected string _filePath;
		private Db4oRepository<LexEntry> _db4oRepository = null;
		/// <summary>
		/// for tests
		/// </summary>
		private int _activationCount = 0;

		private Db4oDataSource _container;

		/// <summary>
		/// how many times an Object has been activated
		/// </summary>
		public int ActivationCount
		{
			get { return this._activationCount; }
		}

		private void OnActivated(object sender, ObjectEventArgs args)
		{
			this._activationCount++;
		}
		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();
			CycleDatabase();
		}

		[TearDown]
		public void TearDown()
		{
			IEventRegistry r = EventRegistryFactory.ForObjectContainer(this._container.Data);
			r.Activated -= OnActivated;
			this._container = null;

			if (_db4oRepository != null)
			{
				_db4oRepository.Dispose();
			}

			if (File.Exists(_filePath))
			{
				File.Delete(_filePath);
			}
		}

		protected void CycleDatabase()
		{
			if (_db4oRepository != null)
			{
				_db4oRepository.Dispose();
			}
			_container = new Db4oDataSource(_filePath);
			IEventRegistry r = EventRegistryFactory.ForObjectContainer(_container.Data);
			r.Activated += OnActivated;

			_db4oRepository = new Db4oRepository<LexEntry>(_container);
		}

		private LexEntry GetFirstEntry()
		{
			RepositoryId[] repositoryIds = _db4oRepository.GetAllItems();
			if (repositoryIds != null && repositoryIds.Length > 0)
			{
				return _db4oRepository.GetItem(repositoryIds[0]);
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
			LexEntry entry = _db4oRepository.CreateItem();
			entry.LexicalForm.SetAlternative("en", "test");

			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			_db4oRepository.SaveItem(entry);

			Assert.AreEqual(entry, sense.Parent);

			int activations = ActivationCount;
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual(1, _db4oRepository.CountAllItems());
			Assert.AreEqual(1, entry.Senses.Count);
			Assert.AreEqual(activations + 1 /*entry*/+ 1 /*sense*/,
							ActivationCount);
		}

		[Test]
		public void EntryOnlyActivatedOnce()
		{
			LexEntry entry = _db4oRepository.CreateItem();
			_db4oRepository.SaveItem(entry);

			int activations = ActivationCount;
			CycleDatabase();
			GetFirstEntry();
			Assert.AreEqual(activations + 1, ActivationCount);
			//get the same entry again
			_db4oRepository.GetAllItems();
			Assert.AreEqual(activations + 1, ActivationCount);
		}

		[Test]
		public void ShallowChange()
		{
			_db4oRepository.CreateItem();

			CycleDatabase();
			LexEntry entry = GetFirstEntry();
			Assert.AreEqual(1, _db4oRepository.CountAllItems());
			entry.LexicalForm["en"] = "x";
			_db4oRepository.SaveItem(entry);
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("x", entry.LexicalForm["en"]);
		}

		[Test]
		public void SaveCustomTextField()
		{
			LexEntry entry = _db4oRepository.CreateItem();
			entry.GetOrCreateProperty<MultiText>("testField")["en"] = "test";
			_db4oRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("test", entry.GetOrCreateProperty<MultiText>("testField")["en"]);
		}

		[Test]
		public void SaveOptionRefField()
		{
			LexEntry entry = _db4oRepository.CreateItem();
			//Option z = new Option("test", "t", Guid.NewGuid());
			entry.GetOrCreateProperty<OptionRef>("testOption").Value = "test";
			_db4oRepository.SaveItem(entry);
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("test", entry.GetOrCreateProperty<OptionRef>("testOption").Value);
		}

		[Test]
		public void DeepChange()
		{
			LexEntry entry = _db4oRepository.CreateItem();
			entry.LexicalForm["en"] = "12";
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			LexExampleSentence example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			example.Sentence["th"] = "sawa";
			_db4oRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			entry.Senses[0].ExampleSentences[0].Sentence["th"] = "sawadee";
			_db4oRepository.SaveItem(entry);
			CycleDatabase();
			entry = GetFirstEntry();
			Assert.AreEqual("sawadee", entry.Senses[0].ExampleSentences[0].Sentence["th"]);
		}

		[Test]
		public void DeepNotifyAfterDepersist()
		{
			LexEntry entry = _db4oRepository.CreateItem();
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			LexExampleSentence example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			example.Sentence["th"] = "sawa";
			_db4oRepository.SaveItem(entry);

			CycleDatabase();
			entry = GetFirstEntry();
			entry.PropertyChanged += _entry_PropertyChanged;
			_didNotify = false;
			entry.Senses[0].ExampleSentences[0].Sentence["th"] = "sawadeekap";
			Assert.IsTrue(_didNotify);
		}
	}
}