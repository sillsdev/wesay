using NUnit.Framework;
using SIL.DictionaryServices.Model;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingExampleTranslationFilterTests
	{
		private MissingFieldQuery _missingExampleTranslationFilter;

		[SetUp]
		public void Setup()
		{
			Field field = new Field(Field.FieldNames.ExampleTranslation.ToString(),
									"LexExampleSentence",
									new string[] {"analysis"});
			_missingExampleTranslationFilter = new MissingFieldQuery(field, null, null);
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
		public void SenseExampleTranslation_MissingOneUnsearchedWritingSystem_NotReturned()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			LexExampleSentence example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);

			example.Translation["one"] = "filler";
			Field field = new Field(Field.FieldNames.ExampleTranslation.ToString(),
								   "LexExampleSentence",
								   new string[] { "one", "two" });

			var filter = new MissingFieldQuery(field, new[]{"one"}, null);//notice, we don't want to search in "two"

			Assert.AreEqual(false, filter.FilteringPredicate(entry));
		}


		[Test]
		public void SenseExampleTranslationHasAnalysisWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			LexExampleSentence example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);

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
			entry.Senses.Add(new LexSense());
			Assert.AreEqual(false, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleTranslationNoWritingSystems()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();

			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseExampleTranslationWritingSystemNoAnalysis()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Translation["vernacular"] = "filler";
			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleTranslationWithOneWithoutWritingSystems()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Translation["analysis"] = "filler";

			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			sense.ExampleSentences.Add(new LexExampleSentence());

			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseExampleTranslationWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Translation["analysis"] = "filler";

			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			example.Translation["vernacular"] = "filler";

			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleTranslationWithOneWithoutWritingSystems()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Translation["analysis"] = "filler";
			entry.Senses[0].ExampleSentences.Add(new LexExampleSentence());
			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseOneExampleTranslationWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = CreateEmptyEntryWithOneExampleSentence();
			LexExampleSentence example = entry.Senses[0].ExampleSentences[0];

			example.Translation["analysis"] = "filler";

			example = new LexExampleSentence();
			entry.Senses[0].ExampleSentences.Add(example);
			example.Translation["vernacular"] = "filler";

			Assert.AreEqual(true, _missingExampleTranslationFilter.FilteringPredicate(entry));
		}
	}
}