using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryTests
	{
		private LexEntry _entry;
		private LexSense _sense;
		private LexExampleSentence _examples;

		[SetUp]
		public void Setup()
		{
			this._entry = new LexEntry();
			this._sense = (LexSense) this._entry.Senses.AddNew();
			this._sense.Gloss["th"] = "sense";
			this._examples = (LexExampleSentence)this._sense.ExampleSentences.AddNew();
			this._examples.Sentence["th"] = "example";
			this._examples.Translation["en"] = "translation";
		}

		[TearDown]
		public void TearDown()
		{

		}

		[Test]
		public void EmptyExampleSentencesRemoved()
		{
			Assert.IsFalse(this._examples.Empty);
			ClearExampleSentence();
			Assert.IsTrue(this._examples.Empty);
			Assert.AreEqual(0, _sense.ExampleSentences.Count);
		}

		private void ClearExampleSentence() {
			this._examples.Sentence["th"] = string.Empty;
			this._examples.Translation["en"] = string.Empty;
		}

		[Test]
		public void EmptySensesRemoved()
		{
			ClearExampleSentence();
			Assert.IsFalse(this._sense.Empty);
			this._sense.Gloss["th"] = string.Empty;
			Assert.IsTrue(this._sense.Empty);
			Assert.AreEqual(0, _entry.Senses.Count);
		}

	}

}