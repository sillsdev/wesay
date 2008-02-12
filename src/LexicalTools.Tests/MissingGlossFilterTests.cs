using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalTools;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingGlossFilterTests
	{
		private MissingItemFilter _missingGlossFilter;

		[SetUp]
		public void Setup()
		{
			Field field = new Field(LexSense.WellKnownProperties.Gloss, "LexSense", new string[] { "analysis" });
			this._missingGlossFilter = new MissingItemFilter(field);
		}

		[Test]
		public void SenseGlossHasAnalysisWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";

			Assert.AreEqual(false, _missingGlossFilter.FilteringPredicate(entry));
		}

		[Test]
		public void NoSenses()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(true, _missingGlossFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseGlossNoWritingSystems()
		{
			LexEntry entry = new LexEntry();
			entry.Senses.AddNew();
			Assert.AreEqual(true, _missingGlossFilter.FilteringPredicate(entry));
		}

		[Test]
		public void SenseGlossWritingSystemNoAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["vernacular"] = "filler";
			Assert.AreEqual(true, _missingGlossFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseGlossWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";
			entry.Senses.AddNew();
			Assert.AreEqual(true, _missingGlossFilter.FilteringPredicate(entry));
		}

		[Test]
		public void OneSenseGlossWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";
			sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["vernacular"] = "filler";
			Assert.AreEqual(true, _missingGlossFilter.FilteringPredicate(entry));
		}
	}
}
