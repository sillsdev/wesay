using System;
using System.ComponentModel;
using System.Threading;
using NUnit.Framework;
using WeSay.LexicalModel.Tests.Db4oSpecific;

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
			Thread.Sleep(100); //else modtime doesn't change
			_entry.LexicalForm["foo"] = "hello";
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}

		[Test]
		public void ModifiedDateAfterLexSenseVectorChanges()
		{
			long start = _entry.ModificationTime.Ticks;
			Thread.Sleep(100); //else modtime doesn't change
			_entry.Senses.AddNew();
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}

		[Test]
		public void ModifiedDateAfterLexSenseGlossChange()
		{
			LexSense sense = (LexSense) _entry.Senses.AddNew();
			long start = _entry.ModificationTime.Ticks;
			Thread.Sleep(100); //else modtime doesn't change
			sense.Gloss["foo"] = "hello";
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}

		[Test]
		public void ModifiedDateAfterAddingExampleSentence()
		{
			LexSense sense = (LexSense) _entry.Senses.AddNew();
			long start = _entry.ModificationTime.Ticks;
			Thread.Sleep(100); //else modtime doesn't change
			sense.ExampleSentences.AddNew();
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}

		[Test]
		public void ModifiedDateAfterChangingExampleSentence()
		{
			LexSense sense = (LexSense) _entry.Senses.AddNew();
			long start = _entry.ModificationTime.Ticks;
			Thread.Sleep(100); //else modtime doesn't change
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["foo"] = "hello";
			Assert.Greater((decimal) _entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}
	}
}