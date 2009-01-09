using NUnit.Framework;
using WeSay.Foundation.Options;
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
			_missingMeaningFilter = new MissingFieldQuery(field);
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
	}
}