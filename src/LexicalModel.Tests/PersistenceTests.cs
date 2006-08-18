using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class PersistenceTests
	{
		private Db4oBindingList<LexEntry> _entriesList;
		private Db4oDataSource _dataSource;
		private string _filePath ;
		protected bool _didNotify;


		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();
		}
		[TearDown]
		public void TearDown()
		{
			this._dataSource.Dispose();
			File.Delete(_filePath);
		}

		void _entry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_didNotify = true;
		}

		protected LexEntry CycleDatabase()
		{
			if (_dataSource != null)
			{
				_dataSource.Dispose();
			}
			_dataSource = new WeSay.Data.Db4oDataSource(_filePath);
			_entriesList = new WeSay.Data.Db4oBindingList<LexEntry>(_dataSource);
			if (_entriesList.Count > 0)
				return _entriesList[0];
			else
				return null;
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
		public void DeepChange()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			entry.LexicalForm["en"] = "12";
			LexSense sense = entry.Senses.AddNew();
			LexExampleSentence example= sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			_entriesList.Add(entry);
			entry = CycleDatabase();
			entry.Senses[0].ExampleSentences[0].Sentence["th"]="sawadee";
			entry = CycleDatabase();
			Assert.AreEqual("sawadee", entry.Senses[0].ExampleSentences[0].Sentence["th"]);
	   }

		[Test]
		public void DeepNotifyAfterDepersist()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			LexSense sense = entry.Senses.AddNew();
			LexExampleSentence example= sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			_entriesList.Add(entry);
			entry = CycleDatabase();
			entry.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_entry_PropertyChanged);
			_didNotify = false;
			entry.Senses[0].ExampleSentences[0].Sentence["th"]="sawadeekap";
			Assert.IsTrue(_didNotify);
		}
	}
}
