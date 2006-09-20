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
	public class MissingExampleSentenceFilterTests
	{
		[Test]
		public void SenseExampleSentenceHasVernacularWritingSystem()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			MissingExampleSentenceFilter missingExampleSentence = new MissingExampleSentenceFilter("vernacular");
			Assert.AreEqual(false, missingExampleSentence.Inquire(entry));
		}

		[Test]
		public void NoSenses()
		{
			LexEntry entry = new LexEntry();

			MissingExampleSentenceFilter missingExampleSentence = new MissingExampleSentenceFilter("vernacular");
			Assert.AreEqual(true, missingExampleSentence.Inquire(entry));
		}

		[Test]
		public void NoExamples()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();

			MissingExampleSentenceFilter missingExampleSentence = new MissingExampleSentenceFilter("vernacular");
			Assert.AreEqual(true, missingExampleSentence.Inquire(entry));
		}

		[Test]
		public void SenseExampleSentenceNoWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();

			MissingExampleSentenceFilter missingExampleSentence = new MissingExampleSentenceFilter("vernacular");
			Assert.AreEqual(true, missingExampleSentence.Inquire(entry));
		}

		[Test]
		public void SenseExampleSentenceWritingSystemNoVernacular()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["analysis"] = "filler";

			MissingExampleSentenceFilter missingExampleSentence = new MissingExampleSentenceFilter("vernacular");
			Assert.AreEqual(true, missingExampleSentence.Inquire(entry));
		}

		[Test]
		public void OneSenseExampleSentenceWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			sense = (LexSense)entry.Senses.AddNew();
			example = (LexExampleSentence)sense.ExampleSentences.AddNew();

			MissingExampleSentenceFilter missingExampleSentence = new MissingExampleSentenceFilter("vernacular");
			Assert.AreEqual(true, missingExampleSentence.Inquire(entry));
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

			MissingExampleSentenceFilter missingExampleSentence = new MissingExampleSentenceFilter("vernacular");
			Assert.AreEqual(true, missingExampleSentence.Inquire(entry));
		}

		[Test]
		public void SenseOneExampleSentenceWithOneWithoutWritingSystems()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";
			example = (LexExampleSentence)sense.ExampleSentences.AddNew();

			MissingExampleSentenceFilter missingExampleSentence = new MissingExampleSentenceFilter("vernacular");
			Assert.AreEqual(true, missingExampleSentence.Inquire(entry));
		}

		[Test]
		public void SenseOneExampleSentenceWritingSystemWithOneWithoutAnalysis()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense)entry.Senses.AddNew();
			LexExampleSentence example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["vernacular"] = "filler";

			example = (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["analysis"] = "filler";

			MissingExampleSentenceFilter missingExampleSentence = new MissingExampleSentenceFilter("vernacular");
			Assert.AreEqual(true, missingExampleSentence.Inquire(entry));
		}


	}
}
