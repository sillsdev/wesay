using NUnit.Framework;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryTests
	{
		private LexEntry _entry;
		private LexSense _sense;
		private LexExampleSentence _examples;
		bool _removed;

		[SetUp]
		public void Setup()
		{
			this._entry = new LexEntry();
			this._entry.EmptyObjectsRemoved += new System.EventHandler(_entry_EmptyObjectsRemoved);
			this._sense = (LexSense) this._entry.Senses.AddNew();
			this._sense.Gloss["th"] = "sense";
			this._examples = (LexExampleSentence)this._sense.ExampleSentences.AddNew();
			this._examples.Sentence["th"] = "example";
			this._examples.Translation["en"] = "translation";
		}

		void _entry_EmptyObjectsRemoved(object sender, System.EventArgs e)
		{
			_removed = true;
		}

		[Test]
		public void EmptyExampleSentencesRemoved()
		{
			Assert.IsFalse(this._examples.Empty);
			ClearExampleSentence();
			Assert.IsTrue(this._examples.Empty);
			Assert.IsTrue(this._removed);
			Assert.AreEqual(0, _sense.ExampleSentences.Count);
		}

		private void ClearExampleSentence() {
			this._examples.Sentence["th"] = string.Empty;
			this._examples.Translation["en"] = string.Empty;
		}

		[Test]
		public void EmptySensesRemoved()
		{
			ClearExampleSentence();
			Assert.IsFalse(this._sense.Empty);
			this._removed = false;
			this._sense.Gloss["th"] = string.Empty;
			Assert.IsTrue(this._sense.Empty);
			Assert.IsTrue(this._removed);
			Assert.AreEqual(0, _entry.Senses.Count);
		}

	}

}