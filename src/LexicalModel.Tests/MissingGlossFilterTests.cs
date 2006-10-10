using NUnit.Framework;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class MissingGlossFilterTests
	{
		private MissingGlossFilter _missingGlossFilter;

		[SetUp]
		public void Setup()
		{
			Field field = new Field("Gloss", new string[] { "analysis" });
			this._missingGlossFilter = new MissingGlossFilter(field);
		}

		[Test]
		public void SenseGlossHasAnalysisWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";

			Assert.AreEqual(false, _missingGlossFilter.Inquire(entry));
		}

		[Test]
		public void NoSenses()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(true, _missingGlossFilter.Inquire(entry));
		}

		[Test]
		public void SenseGlossNoWritingSystems()
		{
			LexEntry entry = new LexEntry();
			entry.Senses.AddNew();
			Assert.AreEqual(true, _missingGlossFilter.Inquire(entry));
		}

		[Test]
		public void SenseGlossWritingSystemNoAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["vernacular"] = "filler";
			Assert.AreEqual(true, _missingGlossFilter.Inquire(entry));
		}

		[Test]
		public void OneSenseGlossWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";
			entry.Senses.AddNew();
			Assert.AreEqual(true, _missingGlossFilter.Inquire(entry));
		}

		[Test]
		public void OneSenseGlossWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";
			sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["vernacular"] = "filler";
			Assert.AreEqual(true, _missingGlossFilter.Inquire(entry));
		}
	}
}
