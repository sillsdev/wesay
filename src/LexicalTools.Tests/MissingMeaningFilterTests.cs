using NUnit.Framework;
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
			Field field =
					new Field(LexSense.WellKnownProperties.Definition,
							  "LexSense",
							  new string[] {"analysis"});
			_missingMeaningFilter = new MissingFieldQuery(field);
		}

		[Test]
		public void SenseDefinitionHasAnalysisWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
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
			LexEntry entry = new LexEntry();
			entry.Senses.AddNew();
			Assert.AreEqual(true, _missingMeaningFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseDefinitionWritingSystemNoAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Definition["vernacular"] = "filler";
			Assert.AreEqual(true, _missingMeaningFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseDefinitionWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Definition["analysis"] = "filler";
			entry.Senses.AddNew();
			Assert.AreEqual(true, _missingMeaningFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseDefinitionWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Definition["analysis"] = "filler";
			sense = (LexSense) entry.Senses.AddNew();
			sense.Definition["vernacular"] = "filler";
			Assert.AreEqual(true, _missingMeaningFilter.FilteringPredicate(entry));
		}
	}
}