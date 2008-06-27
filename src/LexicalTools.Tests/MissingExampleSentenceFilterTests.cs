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
			Field field =
					new Field(Field.FieldNames.ExampleSentence.ToString(),
							  "LexExampleSentence",
							  new string[] {"vernacular"});
			_missingExampleSentenceFilter = new MissingFieldQuery(field);
		}

		[Test]
		public void SenseExampleSentenceHasVernacularWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
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
			entry.Senses.AddNew();
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
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.ExampleSentences.AddNew();
			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleSentenceWritingSystemNoVernacular()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["analysis"] = "filler";
			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleSentenceWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			sense = (LexSense) entry.Senses.AddNew();
			sense.ExampleSentences.AddNew();

			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleSentenceWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			sense = (LexSense) entry.Senses.AddNew();
			example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["analysis"] = "filler";

			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleSentenceWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";
			sense.ExampleSentences.AddNew();
			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleSentenceWritingSystemWithOneWithoutVernacular()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["analysis"] = "filler";

			Assert.AreEqual(true, _missingExampleSentenceFilter.FilteringPredicate(entry));
		}
	}
}