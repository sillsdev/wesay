using System.Collections.Generic;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Foundation.Options;

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
#if GlossMeaning
			this._sense.Gloss["th"] = "sense";
#else
			this._sense.Definition["th"] = "sense";
#endif
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

			Db4o_Specific.Db4oLexModelHelper.InitializeForNonDbTests();
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
		private void ClearSenseMeaning()
		{
#if GlossMeaning
			this._sense.Gloss["th"] = string.Empty;
#else
			this._sense.Definition["th"] = string.Empty;
#endif
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
		public void SenseWithOnlyMeaning_Empty_False()
		{
			ClearSenseExample();
			ClearSenseCustom();
			Assert.IsFalse(this._removed);
			Assert.IsFalse(this._sense.IsEmpty);
		}

		[Test]
		public void SenseWithOnlyExample_Empty_False()
		{
			ClearSenseMeaning();
			ClearSenseCustom();
			Assert.IsFalse(this._removed);
			Assert.IsFalse(this._sense.IsEmpty);
		}

		[Test]
		public void SenseWithOnlyCustom_Empty_False()
		{
			ClearSenseMeaning();
			ClearSenseExample();
			Assert.IsFalse(this._removed);
			Assert.IsFalse(this._sense.IsEmpty);
		}

		[Test]
		public void SenseWithNoExampleOrField_Empty_True()
		{
			ClearSenseMeaning();
			ClearSenseExample();
			ClearSenseCustom();
			Assert.IsTrue(this._removed);
			Assert.IsTrue(this._sense.IsEmpty);
		}

		[Test]
		public void SenseWithOnlyPOS_ReadyForDeletion()
		{
			Assert.IsFalse(this._sense.IsEmptyForPurposesOfDeletion);
			ClearSenseMeaning();
			ClearSenseExample();
			ClearSenseCustom();
			Assert.IsTrue(this._sense.IsEmpty);
			OptionRef pos = _sense.GetOrCreateProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);
			pos.Value = "noun";
			Assert.IsFalse(this._sense.IsEmpty);
			Assert.IsTrue(this._sense.IsEmptyForPurposesOfDeletion);
		}

 //Not fixed yet       [Test]
		public void SenseWithAPicture_ReadyForDeletion()
		{
			Assert.IsFalse(this._sense.IsEmptyForPurposesOfDeletion);
			ClearSenseMeaning();
			ClearSenseExample();
			ClearSenseCustom();
			Assert.IsTrue(this._sense.IsEmpty);
			PictureRef pict = _sense.GetOrCreateProperty<PictureRef>(LexSense.WellKnownProperties.Picture);
			pict.Value = "dummy.png";
			Assert.IsFalse(this._sense.IsEmpty);
			Assert.IsTrue(this._sense.IsEmptyForPurposesOfDeletion);
		}

		[Test]
		public void EmptySensesRemoved()
		{
			ClearSenseMeaning();
			ClearSenseExample();
			ClearSenseCustom();
			Assert.IsTrue(this._removed);
			Assert.AreEqual(0, _entry.Senses.Count);
		}

		[Test]
		public void GetOrCreateSenseWithMeaning_SenseDoesNotExist_NewSenseWithMeaning()
		{
			MultiText meaning = new MultiText();
			meaning.SetAlternative("th", "new");

			LexSense sense = _entry.GetOrCreateSenseWithMeaning(meaning);
			Assert.AreNotSame(_sense, sense);
#if GlossMeaning
			Assert.AreEqual("new", sense.Gloss.GetExactAlternative("th"));
#else
			Assert.AreEqual("new", sense.Definition.GetExactAlternative("th"));
#endif
		}

		[Test]
		public void GetOrCreateSenseWithMeaning_SenseWithEmptyStringExists_ExistingSense()
		{
			ClearSenseMeaning();

			MultiText meaning = new MultiText();
			meaning.SetAlternative("th", string.Empty);

			LexSense sense = _entry.GetOrCreateSenseWithMeaning(meaning);
			Assert.AreSame(_sense, sense);
		}

		[Test]
		public void GetOrCreateSenseWithMeaning_SenseDoesExists_ExistingSense()
		{
			MultiText meaning = new MultiText();
			meaning.SetAlternative("th", "sense");

			LexSense sense = _entry.GetOrCreateSenseWithMeaning(meaning);
			Assert.AreSame(_sense, sense);
		}

		[Test]
		public void GetHeadword_EmptyEverything_ReturnsEmptyString()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(string.Empty, entry.GetHeadWord("a"));
		}

		[Test]
		public void GetHeadword_LexemeForm_ReturnsCorrectAlternative()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative("c", "can");
			entry.LexicalForm.SetAlternative("a", "apple");
			entry.LexicalForm.SetAlternative("b", "bart");
			Assert.AreEqual("apple", entry.GetHeadWord("a"));
			Assert.AreEqual("bart", entry.GetHeadWord("b"));
			Assert.AreEqual(string.Empty, entry.GetHeadWord("donthave"));
		}

		[Test]
		public void GetHeadword_CitationFormHasAlternative_CorrectForm()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative("a", "apple");
			MultiText citation = entry.GetOrCreateProperty<MultiText>(LexEntry.WellKnownProperties.Citation);
			citation.SetAlternative("b", "barter");
			citation.SetAlternative("a", "applishus");
			Assert.AreEqual("applishus", entry.GetHeadWord("a"));
			Assert.AreEqual("barter", entry.GetHeadWord("b"));
			Assert.AreEqual(string.Empty, entry.GetHeadWord("donthave"));
		}
		[Test]
		public void GetHeadword_CitationFormLacksAlternative_GetsFormFromLexemeForm()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative("a", "apple");
			MultiText citation = entry.GetOrCreateProperty<MultiText>(LexEntry.WellKnownProperties.Citation);
			citation.SetAlternative("b", "bater");
			Assert.AreEqual("apple", entry.GetHeadWord("a"));
		}
	}
}