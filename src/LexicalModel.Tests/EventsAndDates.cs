using System;
using NUnit.Framework;
using System.Diagnostics;
using System.Xml;
using System.Collections.Generic;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace LexicalModel.Tests
{
	[TestFixture]
	public class EventsAndDates
	{
		protected bool _didNotify;
		protected LexEntry _entry;

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
		   _entry = new LexEntry();
		   _entry.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_entry_PropertyChanged);
			_didNotify = false;
		}

		void _entry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_didNotify = true;
		}

		[Test]
		public void CreationDate()
		{
			Assert.AreEqual(_entry.CreationTime.Date, DateTime.Today);
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
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
			_entry.LexicalForm["foo"] = "hello";
			Assert.Greater((decimal)_entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}

		[Test]
		public void ModifiedDateAfterLexSenseVectorChanges()
		{
			long start = _entry.ModificationTime.Ticks;
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
			_entry.Senses.AddNew();
			Assert.Greater((decimal)_entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}
		[Test]
		public void ModifiedDateAfterLexSenseGlossChange()
		{
			LexSense sense = (LexSense) _entry.Senses.AddNew();
			long start = _entry.ModificationTime.Ticks;
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
			sense.Gloss["foo"] = "hello";
			Assert.Greater((decimal)_entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}
		[Test]
		public void ModifiedDateAfterAddingExampleSentence()
		{
			LexSense sense = (LexSense) _entry.Senses.AddNew();
			 long start = _entry.ModificationTime.Ticks;
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
		   LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			Assert.Greater((decimal)_entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}
		[Test]
		public void ModifiedDateAfterChangingExampleSentence()
		{
			LexSense sense = (LexSense) _entry.Senses.AddNew();
			long start = _entry.ModificationTime.Ticks;
			System.Threading.Thread.Sleep(100);//else modtime doesn't change
			LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["foo"] = "hello";
			Assert.Greater((decimal)_entry.ModificationTime.Ticks, start);
			Assert.IsTrue(_didNotify);
		}
	}
}
