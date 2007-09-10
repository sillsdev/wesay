using System.Collections.Generic;
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalTools;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingExampleSentenceFilterTests
	{
		private MissingItemFilter _missingExampleSentenceFilter;
		private string kNameOfSkipTrait = "intentionalMissing_LexExampleSentence";

		[SetUp]
	public void Setup()
		{
			Field field = new Field(Field.FieldNames.ExampleSentence.ToString(), "LexExampleSentence", new string[] { "vernacular" });
			this._missingExampleSentenceFilter = new MissingItemFilter(field);
		}

		[Test]
		public void SenseExampleSentenceHasVernacularWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			Assert.AreEqual(false, this._missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void NoSenses()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(false, this._missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void NoExamples()
		{
			LexEntry entry = new LexEntry();
			entry.Senses.AddNew();
			Assert.AreEqual(true, this._missingExampleSentenceFilter.FilteringPredicate(entry));
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
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.ExampleSentences.AddNew();
			Assert.AreEqual(true, this._missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleSentenceWritingSystemNoVernacular()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["analysis"] = "filler";
			Assert.AreEqual(true, this._missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleSentenceWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			sense = (LexSense)entry.Senses.AddNew();
			sense.ExampleSentences.AddNew();

			Assert.AreEqual(true, this._missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleSentenceWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			sense = (LexSense)entry.Senses.AddNew();
			example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["analysis"] = "filler";

			Assert.AreEqual(true, this._missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleSentenceWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";
			sense.ExampleSentences.AddNew();
			Assert.AreEqual(true, this._missingExampleSentenceFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleSentenceWritingSystemWithOneWithoutVernacular()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["analysis"] = "filler";

			Assert.AreEqual(true, this._missingExampleSentenceFilter.FilteringPredicate(entry));
		}
	}
}
