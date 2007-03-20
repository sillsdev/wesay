using NUnit.Framework;
using WeSay.Language;

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
			MultiText customFieldInSense = this._sense.GetOrCreateProperty<MultiText>("customFieldInSense");
			customFieldInSense["th"] = "custom";
			this._examples = (LexExampleSentence)this._sense.ExampleSentences.AddNew();
			this._examples.Sentence["th"] = "example";
			this._examples.Translation["en"] = "translation";
			MultiText customFieldInExample = this._examples.GetOrCreateProperty<MultiText>("customFieldInExample");
			customFieldInExample["th"] = "custom";
		}

		void _entry_EmptyObjectsRemoved(object sender, System.EventArgs e)
		{
			_removed = true;
		}

		private void ClearExampleSentence()
		{
			this._examples.Sentence["th"] = string.Empty;
		}
		private void ClearExampleTranslation()
		{
			this._examples.Translation["en"] = string.Empty;
		}
		private void ClearExampleCustom()
		{
			MultiText customFieldInExample = this._examples.GetOrCreateProperty<MultiText>("customFieldInExample");
			customFieldInExample["th"] = string.Empty;
		}
		private void ClearSenseGloss()
		{
			this._sense.Gloss["th"] = string.Empty;
		}
		private void ClearSenseExample()
		{
			ClearExampleSentence();
			ClearExampleTranslation();
			ClearExampleCustom();
			_removed = false;
		}
		private void ClearSenseCustom()
		{
			MultiText customFieldInSense = this._sense.GetOrCreateProperty<MultiText>("customFieldInSense");
			customFieldInSense["th"] = string.Empty;
		}

		[Test]
		public void Example_Empty_False()
		{
			Assert.IsFalse(this._examples.Empty);
		}


		[Test]
		public void ExampleWithOnlySentence_Empty_False()
		{
			ClearExampleTranslation();
			ClearExampleCustom();
			Assert.IsFalse(this._examples.Empty);
		}

		[Test]
		public void ExampleWithOnlyTranslation_Empty_False()
		{
			ClearExampleSentence();
			ClearExampleCustom();
			Assert.IsFalse(this._examples.Empty);
		}
		[Test]
		public void ExampleWithOnlyCustomField_Empty_False()
		{
			ClearExampleSentence();
			ClearExampleTranslation();
			Assert.IsFalse(this._examples.Empty);
		}

		[Test]
		public void ExampleWithNoFields_Empty_True()
		{
			ClearExampleSentence();
			ClearExampleTranslation();
			ClearExampleCustom();
			Assert.IsTrue(this._examples.Empty);
		}

		[Test]
		public void EmptyExampleRemoved()
		{
			ClearExampleSentence();
			ClearExampleTranslation();
			ClearExampleCustom();
			Assert.IsTrue(this._removed);
			Assert.AreEqual(0, _sense.ExampleSentences.Count);
		}


		[Test]
		public void SenseWithOnlyGloss_Empty_False()
		{
			ClearSenseExample();
			ClearSenseCustom();
			Assert.IsFalse(this._sense.Empty);
		}

		[Test]
		public void SenseWithOnlyExample_Empty_False()
		{
			ClearSenseGloss();
			ClearSenseCustom();
			Assert.IsFalse(this._sense.Empty);
		}

		[Test]
		public void SenseWithOnlyCustom_Empty_False()
		{
			ClearSenseGloss();
			ClearSenseExample();
			Assert.IsFalse(this._sense.Empty);
		}

		[Test]
		public void SenseWithNoExampleOrField_Empty_True()
		{
			ClearSenseGloss();
			ClearSenseExample();
			ClearSenseCustom();
			Assert.IsTrue(this._sense.Empty);
		}

		[Test]
		public void EmptySensesRemoved()
		{
			ClearSenseGloss();
			ClearSenseExample();
			ClearSenseCustom();
			Assert.IsTrue(this._removed);
			Assert.AreEqual(0, _entry.Senses.Count);
		}
	}
}