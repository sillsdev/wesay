using System;
using System.ComponentModel;
using System.Threading;
using NUnit.Framework;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class EventsAndDates
	{
		protected bool _didNotify;
		protected LexEntry _entry;

		[SetUp]
		public void Setup()
		{
			_entry = new LexEntry();
			_entry.PropertyChanged += _entry_PropertyChanged;
			_didNotify = false;
		}

		private void _entry_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_didNotify = true;
		}

		[Test]
		public void CreationDate()
		{
			Assert.AreEqual(_entry.CreationTime.Date, DateTime.UtcNow.Date);
		}

		[Test]
		public void ModifiedDateOnCreation()
		{
			Assert.AreEqual(_entry.ModificationTime, _entry.CreationTime);
		}

		[Test]
		public void ModifiedDateAfterMultiTextChange()
		{
			long start = _entry.ModificationTime.Ticks;
			Thread.Sleep(1000); //else modtime doesn't change
			_entry.LexicalForm["foo"] = "hello";
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}

		[Test]
		public void ModifiedDateAfterLexSenseVectorChanges()
		{
			long start = _entry.ModificationTime.Ticks;
			Thread.Sleep(1000); //else modtime doesn't change
			_entry.Senses.Add(new LexSense());;
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}

		[Test]
		public void ModifiedDateAfterLexSenseGlossChange()
		{
			LexSense sense = new LexSense();
			_entry.Senses.Add(sense);
			long start = _entry.ModificationTime.Ticks;
			Thread.Sleep(1000); //else modtime doesn't change
			sense.Gloss["foo"] = "hello";
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}

		[Test]
		public void ModifiedDateAfterAddingExampleSentence()
		{
			LexSense sense = new LexSense();
			_entry.Senses.Add(sense);
			long start = _entry.ModificationTime.Ticks;
			Thread.Sleep(1000); //else modtime doesn't change
			sense.ExampleSentences.Add(new LexExampleSentence());;
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}

		[Test]
		public void ModifiedDateAfterChangingExampleSentence()
		{
			LexSense sense = new LexSense();
			_entry.Senses.Add(sense);
			long start = _entry.ModificationTime.Ticks;
			Thread.Sleep(1000); //else modtime doesn't change
			LexExampleSentence example = new LexExampleSentence();
			sense.ExampleSentences.Add(example);
			example.Sentence["foo"] = "hello";
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}
	}
}