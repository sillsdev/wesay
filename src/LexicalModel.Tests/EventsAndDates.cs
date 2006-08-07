using System;
using NUnit.Framework;
using System.Diagnostics;
using System.Xml;
using System.Collections.Generic;
using WeSay.LexicalModel;

namespace LexicalModel.Tests
{
	[TestFixture]
	public class EventsAndDates
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void CreationDate()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(entry.CreationDate.Date, DateTime.Today);
		}

		[Test]
		public void ModifiedDateOnCreation()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(entry.ModifiedDate, entry.CreationDate);
		}

		[Test]
		public void ModifiedDateAfterMultiTextChange()
		{
			LexEntry entry = new LexEntry();
			long start = entry.ModifiedDate.Ticks;
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
			entry.LexicalForm["foo"] = "hello";
			Assert.Greater((decimal)entry.ModifiedDate.Ticks, start);
		}

		[Test]
		public void ModifiedDateAfterLexSenseVectorChanges()
		{
			LexEntry entry = new LexEntry();
			long start = entry.ModifiedDate.Ticks;
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
			entry.Senses.AddNew();
			Assert.Greater((decimal)entry.ModifiedDate.Ticks, start);
		}
		[Test]
		public void ModifiedDateAfterLexSenseGlossChange()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = entry.Senses.AddNew();
			long start = entry.ModifiedDate.Ticks;
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
			sense.Gloss["foo"] = "hello";
			Assert.Greater((decimal)entry.ModifiedDate.Ticks, start);
		}
		[Test]
		public void ModifiedDateAfterAddingExampleSentence()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = entry.Senses.AddNew();
			 long start = entry.ModifiedDate.Ticks;
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
		   LexExampleSentence example = sense.ExampleSentences.AddNew();
			Assert.Greater((decimal)entry.ModifiedDate.Ticks, start);
		}
		[Test]
		public void ModifiedDateAfterChangingExampleSentence()
		{
			LexEntry entry = new LexEntry();
			LexSense sense = entry.Senses.AddNew();
			long start = entry.ModifiedDate.Ticks;
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
			LexExampleSentence example = sense.ExampleSentences.AddNew();
			example.Sentence["foo"] = "hello";
			Assert.Greater((decimal)entry.ModifiedDate.Ticks, start);
		}
	}
}
