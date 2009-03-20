using NUnit.Framework;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingExampleSentenceFilterTests
	{
		private MissingFieldQuery _missingExampleSentenceFilter;
		//private string kNameOfSkipTrait = "flag_skip_LexExampleSentence";

		[SetUp]
		public void Setup()
		{
			Field field = new Field(Field.FieldNames.ExampleSentence.ToString(),
									"LexExampleSentence",
									new string[] {"vernacular"});
			_missingExampleSentenceFilter = new MissingFieldQuery(field, null);
		}

		private static LexEntry CreateEmptyEntryWithOneExampleSentence()
		{
			LexEntry entry = new LexEntry();

			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			LexExampleSentence example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			return entry;
		}

		[Test]
		public void SenseExampleSentenceHasVernacularWritingSystem()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Sentence["vernacular"] = "filler";

			Assert.AreEqual(false, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void NoSenses()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(false, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void NoExamples()
		{
			LexEntry entry = new LexEntry();
			entry.Senses.Add(new LexSense());
			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		//        [Test]
		//        public void NoExamplesButHasSkipFlag()
		//        {
		//            LexEntry entry = new LexEntry();
		//            LexSense sense = (LexSense) entry.Senses.AddNew();
		//            sense.Properties.Add(new KeyValuePair<string, object>(kNameOfSkipTrait, true));
		//
		//            Assert.AreEqual(false, this._missingExampleSentenceFilter.FilteringPredicate(entry));
		//        }

		[Test]
		public void SenseExampleSentenceNoWritingSystems()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();

			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleSentenceWritingSystemNoVernacular()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Sentence["analysis"] = "filler";
			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleSentenceWithOneWithoutWritingSystems()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Sentence["vernacular"] = "filler";

			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);

			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleSentenceWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Sentence["vernacular"] = "filler";

			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			example.Sentence["analysis"] = "filler";

			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleSentenceWithOneWithoutWritingSystems()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Sentence["vernacular"] = "filler";

			example = new LexExampleSentence();
			entry.Senses[0].ExampleSentences.Add(example);
			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleSentenceWritingSystemWithOneWithoutVernacular()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Sentence["vernacular"] = "filler";

			example = new LexExampleSentence();
			entry.Senses[0].ExampleSentences.Add(example);
			example.Sentence["analysis"] = "filler";

			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}
	}
}