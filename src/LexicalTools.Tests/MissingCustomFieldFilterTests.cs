using NUnit.Framework;
using Palaso.DictionaryServices.Model;
using Palaso.Lift.Options;
using WeSay.LexicalModel;
using Palaso.Lift;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingSenseExampleCustomFieldFilterTests
	{
		private MissingFieldQuery _missingCustomFieldFilter;

		[SetUp]
		public void Setup()
		{
			var field = new Field("customField", "LexExampleSentence", new[] {"vernacular"});
			_missingCustomFieldFilter = new MissingFieldQuery(field, null, null);
		}

		private static LexEntry CreateEmptyLexEntryWithOneEmptySentence()
		{
			var entry = new LexEntry();
			var sense = new LexSense();
			entry.Senses.Add(sense);
			sense.ExampleSentences.Add(new LexExampleSentence());
			return entry;
		}

		[Test]
		public void SenseExampleCustomFieldHasVernacularWritingSystem()
		{
			var entry = CreateEmptyLexEntryWithOneEmptySentence();
			var example = entry.Senses[0].ExampleSentences[0];

			var custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleCustomFieldNoGrandparent()
		{
			var entry = new LexEntry();
			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleCustomFieldNoParent()
		{
			var entry = new LexEntry();
			entry.Senses.Add(new LexSense());
			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleCustomFieldHasNoWritingSystems()
		{
			var entry = CreateEmptyLexEntryWithOneEmptySentence();

			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void CustomFieldWritingSystemNoVernacular()
		{
			var entry = CreateEmptyLexEntryWithOneEmptySentence();
			var example = entry.Senses[0].ExampleSentences[0];

			var custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleCustomFieldWithOneWithoutWritingSystems()
		{
			var entry = CreateEmptyLexEntryWithOneEmptySentence();
			var example = entry.Senses[0].ExampleSentences[0];

			var custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			var sense = new LexSense();
			entry.Senses.Add(sense);
			sense.ExampleSentences.Add(new LexExampleSentence());

			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleCustomFieldWritingSystemWithOneWithoutAnalysis()
		{
			var entry = CreateEmptyLexEntryWithOneEmptySentence();
			var example = entry.Senses[0].ExampleSentences[0];

			var custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			var sense = new LexSense();
			entry.Senses.Add(sense);
			example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";

			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleCustomWithOneWithoutWritingSystems()
		{
			var entry = CreateEmptyLexEntryWithOneEmptySentence();
			var example = entry.Senses[0].ExampleSentences[0];

			var custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";
			entry.Senses[0].ExampleSentences.Add(new LexExampleSentence());
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleCustomWritingSystemWithOneWithoutAnalysis()
		{
			var entry = CreateEmptyLexEntryWithOneEmptySentence();
			var example = entry.Senses[0].ExampleSentences[0];

			var custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			example = new LexExampleSentence();
			entry.Senses[0].ExampleSentences.Add(example);
			custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}
	}

	[TestFixture]
	public class MissingEntryCustomFieldFilterTests
	{
		private MissingFieldQuery _missingCustomFieldFilter;

		[SetUp]
		public void Setup()
		{
			var field = new Field("customField", "LexEntry", new[] {"vernacular"});
			_missingCustomFieldFilter = new MissingFieldQuery(field, null, null);
		}

		[Test]
		public void LexEntryCustomFieldHasVernacularWritingSystem()
		{
			var entry = new LexEntry();
			var custom = entry.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void LexEntryCustomFieldHasNoWritingSystems()
		{
			var entry = new LexEntry();
			entry.GetOrCreateProperty<MultiText>("customField");
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void CustomFieldWritingSystemNoVernacular()
		{
			var entry = new LexEntry();
			var custom = entry.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void FilteringPredicate_HasRequiredButNotEmpty_IsChosen()
		{
			var entry = new LexEntry();
			var custom = entry.GetOrCreateProperty<MultiText>("customField");
			custom["en"] = "hello";

			var field = new Field("customField", "LexEntry", new[] { "en","fr" });
			var hasEnglishButNotFrenchFilter = new MissingFieldQuery(field, new string[]{"fr"}, new string[]{"en"});
			Assert.IsTrue(hasEnglishButNotFrenchFilter.FilteringPredicate(entry));
		}

		[Test]
		public void FilteringPredicate_EntryDoesNotHaveAnInstanceOfTheField_IsNotChosen()
		{
			var entry = new LexEntry();

			var field = new Field("customField", "LexEntry", new[] { "en", "fr" });
			var hasEnglishButNotFrenchFilter = new MissingFieldQuery(field, new string[] { "fr" }, new string[] { "en" });
			Assert.IsFalse(hasEnglishButNotFrenchFilter.FilteringPredicate(entry));
		}
	}

	[TestFixture]
	public class MissingSenseCustomFieldFilterTests
	{
		private MissingFieldQuery _missingCustomFieldFilter;

		[SetUp]
		public void Setup()
		{
			var field = new Field("customField",
									"LexSense",
									new[] {"vernacular"},
									Field.MultiplicityType.ZeroOr1,
									"Option");
			_missingCustomFieldFilter = new MissingFieldQuery(field, null, null);
		}

		[Test]
		public void SenseHasCustomField()
		{
			var entry = new LexEntry();
			var sense = new LexSense();
			entry.Senses.Add(sense);
			var custom = sense.GetOrCreateProperty<OptionRef>("customField");
			custom.Value = "filler";

			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseCustomFieldNoParent()
		{
			var entry = new LexEntry();
			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseCustomFieldHasNoValue()
		{
			var entry = new LexEntry();
			var sense = new LexSense();
			entry.Senses.Add(sense);
			sense.Gloss["analysis"] = "filler";
			sense.GetOrCreateProperty<OptionRef>("customField");
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}
	}
}