using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class PersistenceTests : BaseDb4oSpecificTests
	{
		protected bool _didNotify;


		void _entry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_didNotify = true;
		}


		[Test]
		public void HelperGetsActivationCall()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative("en", "test");
			_entriesList.Add(entry);

			LexSense sense = (LexSense) entry.Senses.AddNew();

			Assert.AreEqual(entry,sense.Parent);

			int activations = Db4oLexModelHelper.Singleton.ActivationCount;
			entry = CycleDatabase();
			Assert.AreEqual(1, _entriesList.Count);
			Assert.AreEqual(1, entry.Senses.Count);
			Assert.AreEqual(activations + 1 /*entry*/ + 1 /*sense*/, Db4oLexModelHelper.Singleton.ActivationCount);

		}

		[Test]
		public void EntryOnlyActivatedOnce()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			_entriesList.Add(entry);

			int activations = Db4oLexModelHelper.Singleton.ActivationCount;
			entry = CycleDatabase();
			Assert.AreEqual(activations + 1, Db4oLexModelHelper.Singleton.ActivationCount);
			//get the same entry again
			IList<LexEntry> matches = _dataSource.Data.Query<LexEntry>();
			Assert.AreEqual(activations+1, Db4oLexModelHelper.Singleton.ActivationCount);
		}

		[Test]
		public void ShallowChange()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			_entriesList.Add(entry);
			entry = CycleDatabase();
			Assert.AreEqual(1, _entriesList.Count);
			entry.LexicalForm["en"] = "x";
			entry = CycleDatabase();
			Assert.AreEqual("x", entry.LexicalForm["en"]);
		}


		[Test]
		public void SaveCustomTextField()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			entry.GetProperty<MultiText>("testField")["en"] = "test";
			_entriesList.Add(entry);
			entry = CycleDatabase();
			Assert.AreEqual("test", entry.GetProperty<MultiText>("testField")["en"]);
		}

		[Test]
		public void SaveOptionRefField()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			Option z = new Option("test", "t", Guid.NewGuid());
			entry.GetProperty<OptionRef>("testOption").Value = z;
			_entriesList.Add(entry);
			entry = CycleDatabase();
			Assert.AreEqual("test", entry.GetProperty<OptionRef>("testOption").Value.Name);
		}

		[Test]
		public void DeepChange()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			entry.LexicalForm["en"] = "12";
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example= (LexExampleSentence)sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			_entriesList.Add(entry);
			entry = CycleDatabase();
			((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence["th"]="sawadee";
			entry = CycleDatabase();
			Assert.AreEqual("sawadee", ((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence["th"]);
	   }

		[Test]
		public void DeepNotifyAfterDepersist()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			LexExampleSentence example= (LexExampleSentence) sense.ExampleSentences.AddNew();
			example.Sentence["th"] = "sawa";
			_entriesList.Add(entry);
			entry = CycleDatabase();
			entry.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_entry_PropertyChanged);
			_didNotify = false;
			((LexExampleSentence)((LexSense)entry.Senses[0]).ExampleSentences[0]).Sentence["th"]="sawadeekap";
			Assert.IsTrue(_didNotify);
		}


	}
}
