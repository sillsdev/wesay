using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingExampleTranslationFilterTests
	{
		private MissingItemFilter _missingExampleTranslationFilter;

		[SetUp]
		public void Setup()
		{
			Field field =
					new Field(Field.FieldNames.ExampleTranslation.ToString(),
							  "LexExampleSentence",
							  new string[] {"analysis"});
			_missingExampleTranslationFilter = new MissingItemFilter(field);
		}

		[Test]
		public void SenseExampleTranslationHasAnalysisWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Translation["analysis"] = "filler";

			Assert.AreEqual(false, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void NoSenses()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(false, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void NoExamples()
		{
			LexEntry entry = new LexEntry();
			entry.Senses.AddNew();
			Assert.AreEqual(false, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleTranslationNoWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.ExampleSentences.AddNew();
			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleTranslationWritingSystemNoAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Translation["vernacular"] = "filler";
			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleTranslationWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Translation["analysis"] = "filler";

			sense = (LexSense) entry.Senses.AddNew();
			sense.ExampleSentences.AddNew();

			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleTranslationWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Translation["analysis"] = "filler";

			sense = (LexSense) entry.Senses.AddNew();
			example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Translation["vernacular"] = "filler";

			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleTranslationWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Translation["analysis"] = "filler";
			sense.ExampleSentences.AddNew();
			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleTranslationWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Translation["analysis"] = "filler";

			example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Translation["vernacular"] = "filler";

			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}
	}
}