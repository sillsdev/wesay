using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingSenseExampleCustomFieldFilterTests
	{
		private MissingFieldQuery _missingCustomFieldFilter;

		[SetUp]
		public void Setup()
		{
			Field field =
					new Field("customField", "LexExampleSentence", new string[] {"vernacular"});
			_missingCustomFieldFilter = new MissingFieldQuery(field);
		}

		private static LexEntry CreateEmptyLexEntryWithOneEmptySentence()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			sense.ExampleSentences.Add(new LexExampleSentence());
			return entry;
		}

		[Test]
		public void SenseExampleCustomFieldHasVernacularWritingSystem()
		{
			LexEntry entry = CreateEmptyLexEntryWithOneEmptySentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleCustomFieldNoGrandparent()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleCustomFieldNoParent()
		{
			LexEntry entry = new LexEntry();
			entry.Senses.Add(new LexSense());
			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleCustomFieldHasNoWritingSystems()
		{
			LexEntry entry = CreateEmptyLexEntryWithOneEmptySentence();

			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void CustomFieldWritingSystemNoVernacular()
		{
			LexEntry entry = CreateEmptyLexEntryWithOneEmptySentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleCustomFieldWithOneWithoutWritingSystems()
		{
			LexEntry entry = CreateEmptyLexEntryWithOneEmptySentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			sense.ExampleSentences.Add(new LexExampleSentence());

			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleCustomFieldWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = CreateEmptyLexEntryWithOneEmptySentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			LexSense sense = new LexSense();
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
			LexEntry entry = CreateEmptyLexEntryWithOneEmptySentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";
			entry.Senses[0].ExampleSentences.Add(new LexExampleSentence());
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleCustomWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = CreateEmptyLexEntryWithOneEmptySentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
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
			Field field = new Field("customField", "LexEntry", new string[] {"vernacular"});
			_missingCustomFieldFilter = new MissingFieldQuery(field);
		}

		[Test]
		public void LexEntryCustomFieldHasVernacularWritingSystem()
		{
			LexEntry entry = new LexEntry();
			MultiText custom = entry.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void LexEntryCustomFieldHasNoWritingSystems()
		{
			LexEntry entry = new LexEntry();
			entry.GetOrCreateProperty<MultiText>("customField");
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void CustomFieldWritingSystemNoVernacular()
		{
			LexEntry entry = new LexEntry();
			MultiText custom = entry.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}
	}

	[TestFixture]
	public class MissingSenseCustomFieldFilterTests
	{
		private MissingFieldQuery _missingCustomFieldFilter;

		[SetUp]
		public void Setup()
		{
			Field field =
					new Field("customField",
							  "LexSense",
							  new string[] {"vernacular"},
							  Field.MultiplicityType.ZeroOr1,
							  "Option");
			_missingCustomFieldFilter = new MissingFieldQuery(field);
		}

		[Test]
		public void SenseHasCustomField()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			OptionRef custom = sense.GetOrCreateProperty<OptionRef>("customField");
			custom.Value = "filler";

			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseCustomFieldNoParent()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(false, _missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseCustomFieldHasNoValue()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			sense.Gloss["analysis"] = "filler";
			sense.GetOrCreateProperty<OptionRef>("customField");
			Assert.AreEqual(true, _missingCustomFieldFilter.FilteringPredicate(entry));
		}
	}
}