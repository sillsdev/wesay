using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalTools;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingSenseExampleCustomFieldFilterTests
	{
		private MissingItemFilter _missingCustomFieldFilter;

		[SetUp]
		public void Setup()
		{
			Field field = new Field("customField", "LexExampleSentence", new string[] { "vernacular" });
			this._missingCustomFieldFilter = new MissingItemFilter(field);
		}

		[Test]
		public void SenseExampleCustomFieldHasVernacularWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			Assert.AreEqual(false, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleCustomFieldNoGrandparent()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(false, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleCustomFieldNoParent()
		{
			LexEntry entry = new LexEntry();
			entry.Senses.AddNew();
			Assert.AreEqual(false, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleCustomFieldHasNoWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.ExampleSentences.AddNew();
			Assert.AreEqual(true, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void CustomFieldWritingSystemNoVernacular()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";
			Assert.AreEqual(true, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleCustomFieldWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			sense = (LexSense)entry.Senses.AddNew();
			sense.ExampleSentences.AddNew();

			Assert.AreEqual(true, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleCustomFieldWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			sense = (LexSense)entry.Senses.AddNew();
			example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";

			Assert.AreEqual(true, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleCustomWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";
			sense.ExampleSentences.AddNew();
			Assert.AreEqual(true, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleCustomWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			MultiText custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			custom = example.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";
			Assert.AreEqual(true, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

	}

	[TestFixture]
	public class MissingEntryCustomFieldFilterTests
	{
		private MissingItemFilter _missingCustomFieldFilter;

		[SetUp]
		public void Setup()
		{
			Field field = new Field("customField", "LexEntry", new string[] { "vernacular" });
			this._missingCustomFieldFilter = new MissingItemFilter(field);
		}

		[Test]
		public void LexEntryCustomFieldHasVernacularWritingSystem()
		{
			LexEntry entry = new LexEntry();
			MultiText custom = entry.GetOrCreateProperty<MultiText>("customField");
			custom["vernacular"] = "filler";

			Assert.AreEqual(false, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void LexEntryCustomFieldHasNoWritingSystems()
		{
			LexEntry entry = new LexEntry();
			MultiText custom = entry.GetOrCreateProperty<MultiText>("customField");
			Assert.AreEqual(true, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void CustomFieldWritingSystemNoVernacular()
		{
			LexEntry entry = new LexEntry();
			MultiText custom = entry.GetOrCreateProperty<MultiText>("customField");
			custom["analysis"] = "filler";
			Assert.AreEqual(true, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}
	}

	[TestFixture]
	public class MissingSenseCustomFieldFilterTests
	{
		private MissingItemFilter _missingCustomFieldFilter;

		[SetUp]
		public void Setup()
		{
			Field field = new Field("customField", "LexSense", new string[] { "vernacular" }, Field.MultiplicityType.ZeroOr1, "Option");
			this._missingCustomFieldFilter = new MissingItemFilter(field);
		}

		[Test]
		public void SenseHasCustomField()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			OptionRef custom = sense.GetOrCreateProperty<OptionRef>("customField");
			custom.Value = "filler";

			Assert.AreEqual(false, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseCustomFieldNoParent()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(false, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseCustomFieldHasNoValue()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";
			OptionRef custom = sense.GetOrCreateProperty<OptionRef>("customField");
			Assert.AreEqual(true, this._missingCustomFieldFilter.FilteringPredicate(entry));
		}
	}

}
