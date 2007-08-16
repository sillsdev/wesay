using System.Collections.Generic;
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
			this._sense = (LexSense) this._entry.Senses.AddNew();
			this._sense.Gloss["th"] = "sense";
			MultiText customFieldInSense = this._sense.GetOrCreateProperty<MultiText>("customFieldInSense");
			customFieldInSense["th"] = "custom";
			this._examples = (LexExampleSentence)this._sense.ExampleSentences.AddNew();
			this._examples.Sentence["th"] = "example";
			this._examples.Translation["en"] = "translation";
			MultiText customFieldInExample = this._examples.GetOrCreateProperty<MultiText>("customFieldInExample");
			customFieldInExample["th"] = "custom";
			this._entry.EmptyObjectsRemoved += new System.EventHandler(_entry_EmptyObjectsRemoved);
			this._entry.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_entry_PropertyChanged);
			this._removed = false;
		}

		void _entry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_entry.CleanUpEmptyObjects();
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
			_entry.CleanUpAfterEditting();
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
			_entry.CleanUpAfterEditting();
		}

		[Test]
		public void Example_Empty_False()
		{
			Assert.IsFalse(this._examples.IsEmpty);
		}


		[Test]
		public void ExampleWithOnlySentence_Empty_False()
		{
			ClearExampleTranslation();
			ClearExampleCustom();
			Assert.IsFalse(this._removed);
			Assert.IsFalse(this._examples.IsEmpty);
		}

		[Test]
		public void ExampleWithOnlyTranslation_Empty_False()
		{
			ClearExampleSentence();
			ClearExampleCustom();
			Assert.IsFalse(this._removed);
			Assert.IsFalse(this._examples.IsEmpty);
		}
		[Test]
		public void ExampleWithOnlyCustomField_Empty_False()
		{
			ClearExampleSentence();
			ClearExampleTranslation();
			Assert.IsFalse(this._removed);
			Assert.IsFalse(this._examples.IsEmpty);
		}

		[Test]
		public void ExampleWithNoFields_Empty_True()
		{
			ClearExampleSentence();
			ClearExampleTranslation();
			ClearExampleCustom();
			Assert.IsTrue(this._removed);
			Assert.IsTrue(this._examples.IsEmpty);
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
			Assert.IsFalse(this._removed);
			Assert.IsFalse(this._sense.IsEmpty);
		}

		[Test]
		public void SenseWithOnlyExample_Empty_False()
		{
			ClearSenseGloss();
			ClearSenseCustom();
			Assert.IsFalse(this._removed);
			Assert.IsFalse(this._sense.IsEmpty);
		}

		[Test]
		public void SenseWithOnlyCustom_Empty_False()
		{
			ClearSenseGloss();
			ClearSenseExample();
			Assert.IsFalse(this._removed);
			Assert.IsFalse(this._sense.IsEmpty);
		}

		[Test]
		public void SenseWithNoExampleOrField_Empty_True()
		{
			ClearSenseGloss();
			ClearSenseExample();
			ClearSenseCustom();
			Assert.IsTrue(this._removed);
			Assert.IsTrue(this._sense.IsEmpty);
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

		[Test]
		public void GetOrCreateSenseWithGloss_SenseDoesNotExist_NewSenseWithGloss()
		{
			MultiText gloss = new MultiText();
			gloss.SetAlternative("th", "new");

			LexSense sense = _entry.GetOrCreateSenseWithGloss(gloss);
			Assert.AreNotSame(_sense, sense);
			Assert.AreEqual("new", sense.Gloss.GetExactAlternative("th"));
		}

		[Test]
		public void GetOrCreateSenseWithGloss_SenseWithEmptyStringExists_ExistingSense()
		{
			ClearSenseGloss();

			MultiText gloss = new MultiText();
			gloss.SetAlternative("th", string.Empty);

			LexSense sense = _entry.GetOrCreateSenseWithGloss(gloss);
			Assert.AreSame(_sense, sense);
		}

		[Test]
		public void GetOrCreateSenseWithGloss_SenseDoesExists_ExistingSense()
		{
			MultiText gloss = new MultiText();
			gloss.SetAlternative("th", "sense");

			LexSense sense = _entry.GetOrCreateSenseWithGloss(gloss);
			Assert.AreSame(_sense, sense);
		}


	}
}