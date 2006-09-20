using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WeSay.Data;
using System.Diagnostics;
using WeSay.Language;
using com.db4o.query;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class MissingGlossFilterTests
	{
		[Test]
		public void SenseGlossHasAnalysisWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";

			MissingGlossFilter missingGloss = new MissingGlossFilter("analysis");
			Assert.AreEqual(false, missingGloss.Inquire(entry));
		}

		[Test]
		public void NoSenses()
		{
			LexEntry entry = new LexEntry();

			MissingGlossFilter missingGloss = new MissingGlossFilter("analysis");
			Assert.AreEqual(true, missingGloss.Inquire(entry));
		}

		[Test]
		public void SenseGlossNoWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();

			MissingGlossFilter missingGloss = new MissingGlossFilter("analysis");
			Assert.AreEqual(true, missingGloss.Inquire(entry));
		}

		[Test]
		public void SenseGlossWritingSystemNoAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["vernacular"] = "filler";

			MissingGlossFilter missingGloss = new MissingGlossFilter("analysis");
			Assert.AreEqual(true, missingGloss.Inquire(entry));
		}

		[Test]
		public void OneSenseGlossWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";
			sense = (LexSense)entry.Senses.AddNew();

			MissingGlossFilter missingGloss = new MissingGlossFilter("analysis");
			Assert.AreEqual(true, missingGloss.Inquire(entry));
		}

		[Test]
		public void OneSenseGlossWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["analysis"] = "filler";
			sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss["vernacular"] = "filler";

			MissingGlossFilter missingGloss = new MissingGlossFilter("analysis");
			Assert.AreEqual(true, missingGloss.Inquire(entry));
		}

	}
}
