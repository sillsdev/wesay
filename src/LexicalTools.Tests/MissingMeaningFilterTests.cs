using NUnit.Framework;
using Palaso.LexicalModel.Options;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingMeaningFilterTests
	{
		private MissingFieldQuery _missingMeaningFilter;

		[SetUp]
		public void Setup()
		{
			Field field = new Field(LexSense.WellKnownProperties.Definition,
									"LexSense",
									new string[] {"analysis"});
			_missingMeaningFilter = new MissingFieldQuery(field, null, null);
		}

		private static LexEntry CreateEmptyEntryWithOneSense()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			return entry;
		}

		[Test]
		public void SenseDefinitionHasAnalysisWritingSystem()
		{
			LexEntry entry = CreateEmptyEntryWithOneSense();
			LexSense sense = entry.Senses[0];

			sense.Definition["analysis"] = "filler";

			Assert.AreEqual(false, _missingMeaningFilter.FilteringPredicate(entry));
		}

		[Test]
		public void NoSenses()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(true, _missingMeaningFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseDefinitionNoWritingSystems()
		{
			LexEntry entry = CreateEmptyEntryWithOneSense();

			Assert.AreEqual(true, _missingMeaningFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseDefinitionWritingSystemNoAnalysis()
		{
			LexEntry entry = CreateEmptyEntryWithOneSense();
			LexSense sense = entry.Senses[0];

			sense.Definition["vernacular"] = "filler";
			Assert.AreEqual(true, _missingMeaningFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseDefinitionWithOneWithoutWritingSystems()
		{
			LexEntry entry = CreateEmptyEntryWithOneSense();
			LexSense sense = entry.Senses[0];

			sense.Definition["analysis"] = "filler";
			entry.Senses.Add(new LexSense());
			Assert.AreEqual(true, _missingMeaningFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseDefinitionWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = CreateEmptyEntryWithOneSense();
			LexSense sense = entry.Senses[0];

			sense.Definition["analysis"] = "filler";
			sense = new LexSense();
			entry.Senses.Add(sense);
			sense.Definition["vernacular"] = "filler";
			Assert.AreEqual(true, _missingMeaningFilter.FilteringPredicate(entry));
		}

		/// <summary>
		/// ws-1417
		/// </summary>
		[Test]
		public void DefinitionHasOneWSButNotTheOneWeWant_False()
		{
			var entry = MakeEntryWithSense(new string[] { "two" });

			Field field = new Field(LexSense.WellKnownProperties.Definition, "LexSense", new string[] { "one", "two" });
			MissingFieldQuery f = new MissingFieldQuery(field, new string[] { "two" }, null);

			Assert.IsFalse(f.FilteringPredicate(entry));
		}

		/// <summary>
		/// ws-1417
		/// </summary>
		[Test]
		public void DefinitionHasOneWSButNotOneWeDontCareAbout_True()
		{
			var entry = MakeEntryWithSense(new string[] { "one" });

			Field field = new Field(LexSense.WellKnownProperties.Definition, "LexSense", new string[] { "one", "two" });
			MissingFieldQuery f = new MissingFieldQuery(field, new string[] { "two" }, null);

			Assert.IsTrue(f.FilteringPredicate(entry));
		}

		/// <summary>
		/// ws-1417
		/// The scenario in this test is that the definition is missing altogether, and no ws's are required
		/// </summary>
		[Test]
		public void DefinitionMissing_True()
		{
			var entry = new LexEntry();
			var sense = new LexSense();
			entry.Senses.Add(sense);

			Field field = new Field(LexSense.WellKnownProperties.Definition, "LexSense", new string[] { "one", "two" });
			MissingFieldQuery f = new MissingFieldQuery(field, new string[] { "two" }, null);

			Assert.IsTrue(f.FilteringPredicate(entry));
		}

		/// <summary>
		/// ws-1417
		/// The scenario in this test is that the def is missing altogether, but some ws's are required
		/// </summary>
		[Test]
		public void DefinitionMissingButSomeWsIsRequired_False()
		{
			var entry = new LexEntry();
			var sense = new LexSense();
			entry.Senses.Add(sense);

			Field field = new Field(LexSense.WellKnownProperties.Definition, "LexSense", new string[] { "one", "two" });
			MissingFieldQuery f = new MissingFieldQuery(field, new string[] { "two" }, new string[] { "one" });

			Assert.IsFalse(f.FilteringPredicate(entry));
		}

		/// <summary>
		/// ws-1417
		/// The scenario in this test is that the example has a ws roman, but is missing ws thai, so that it doesn't show up
		/// for the taks that wants to fill in voice only if thai is present
		/// </summary>
		[Test]
		public void DefinitionSentenceMissingOurWSButAlsoMissingARequiredWS_False()
		{
			var entry = MakeEntryWithSense(new string[] { "roman" });

			Field field = new Field(LexSense.WellKnownProperties.Definition, "LexSense", new string[] { "roman", "thai", "voice" });
			var writingSystemsWhichWeWantToFillIn = new string[] { "voice" };
			var writingSystemsWhichAreRequired = new string[] { "thai" };

			MissingFieldQuery f = new MissingFieldQuery(field, writingSystemsWhichWeWantToFillIn, writingSystemsWhichAreRequired);

			Assert.IsFalse(f.FilteringPredicate(entry));
		}

		private static LexEntry MakeEntryWithSense(string[] writingSystemsToFillIn)
		{
			var entry = new LexEntry();
			var sense = new LexSense();
			entry.Senses.Add(sense);
			foreach (var id in writingSystemsToFillIn)
			{
				sense.Definition.SetAlternative(id, "foo");
			}
			return entry;
		}
	}
}