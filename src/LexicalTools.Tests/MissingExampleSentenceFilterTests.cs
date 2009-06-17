using NUnit.Framework;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingExampleSentenceFilterTests
	{
		private MissingFieldQuery _missingExampleSentenceFilter;

		[SetUp]
		public void Setup()
		{
			Field field = new Field(Field.FieldNames.ExampleSentence.ToString(),
									"LexExampleSentence",
									new string[] {"vernacular"});
			_missingExampleSentenceFilter = new MissingFieldQuery(field, null, null);
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


		/// <summary>
		/// ws-1417
		/// </summary>
		[Test]
		public void ExampleSentenceHasOneWSButNotTheOneWeWant_False()
		{
			var entry = MakeEntryWithExampleSentence(new string[] { "two" });

			Field field = new Field(LexExampleSentence.WellKnownProperties.ExampleSentence, "LexExampleSentence", new string[] { "one", "two" });
			MissingFieldQuery f = new MissingFieldQuery(field, new string[] { "two" }, null);

			Assert.IsFalse(f.FilteringPredicate(entry));
		}

		/// <summary>
		/// ws-1417
		/// </summary>
		[Test]
		public void ExampleSentenceHasOneWSButNotOneWeDontCareAbout_True()
		{
			var entry = MakeEntryWithExampleSentence(new string[] { "one" });

			Field field = new Field(LexExampleSentence.WellKnownProperties.ExampleSentence, "LexExampleSentence", new string[] { "one", "two" });
			MissingFieldQuery f = new MissingFieldQuery(field, new string[] { "two" }, null);

			Assert.IsTrue(f.FilteringPredicate(entry));
		}

		/// <summary>
		/// ws-1417
		/// The scenario in this test is that the example is missing altogether
		/// </summary>
		[Test]
		public void ExampleSentenceMissingAndThereAreNoRequiredWritingSystems_True()
		{
			var entry = new LexEntry();
			var sense = new LexSense();
			entry.Senses.Add(sense);

			Field field = new Field(LexExampleSentence.WellKnownProperties.ExampleSentence, "LexExampleSentence", new string[] { "one", "two" });
			MissingFieldQuery f = new MissingFieldQuery(field, new string[] { "two" }, null);

			Assert.IsTrue(f.FilteringPredicate(entry));
		}

		/// <summary>
		/// ws-1417
		/// The scenario in this test is that the example is missing altogether, but some ws's are required
		/// </summary>
		[Test]
		public void ExampleSentenceMissingAndThereIsARequiredWritingSystem_False()
		{
			var entry = new LexEntry();
			var sense = new LexSense();
			entry.Senses.Add(sense);

			Field field = new Field(LexExampleSentence.WellKnownProperties.ExampleSentence, "LexExampleSentence", new string[] { "one", "two" });
			MissingFieldQuery f = new MissingFieldQuery(field, new string[] { "two" }, new string[] { "one" });

			Assert.IsFalse(f.FilteringPredicate(entry));
		}

		/// <summary>
		/// ws-1417
		/// The scenario in this test is that the example has a ws roman, but is missing ws thai, so that it doesn't show up
		/// for the taks that wants to fill in voice only if thai is present
		/// </summary>
		[Test]
		public void ExampleSentenceMissingOurWSButAlsoMissingARequiredWS_False()
		{
			var entry = MakeEntryWithExampleSentence(new string[] { "roman" });

			Field field = new Field(LexExampleSentence.WellKnownProperties.ExampleSentence, "LexExampleSentence", new string[] { "roman", "thai", "voice" });
			var writingSystemsWhichWeWantToFillIn = new string[] { "voice" };
			var writingSystemsWhichAreRequired = new string[] { "thai" };

			MissingFieldQuery f = new MissingFieldQuery(field, writingSystemsWhichWeWantToFillIn, writingSystemsWhichAreRequired);

			Assert.IsFalse(f.FilteringPredicate(entry));
		}

		private static LexEntry MakeEntryWithExampleSentence(string[] writingSystemsToFillIn)
		{
			var entry = new LexEntry();
			var sense = new LexSense();
			entry.Senses.Add(sense);
			LexExampleSentence example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			foreach (var id in writingSystemsToFillIn)
			{
				example.Sentence.SetAlternative(id, "foo");
			}
			return entry;
		}
	}
}