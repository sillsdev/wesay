using System;
using System.Collections.Generic;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class PersistenceTests
	{
		private Db4oRecordList<LexEntry> _entriesList;
		private Db4oDataSource _dataSource;
		private string _filePath ;
		protected bool _didNotify;


		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();
		}
		[TearDown]
		public void TearDown()
		{
			this._entriesList.Dispose();
			this._dataSource.Dispose();
			File.Delete(_filePath);
		}

		void _entry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_didNotify = true;
		}

		protected LexEntry CycleDatabase()
		{
			if (_dataSource != null)
			{
				_entriesList.Dispose();
				_dataSource.Dispose();
			}
			_dataSource = new WeSay.Data.Db4oDataSource(_filePath);
			Db4oLexModelHelper.Initialize(_dataSource.Data);

			_entriesList = new WeSay.Data.Db4oRecordList<LexEntry>(_dataSource);
			if (_entriesList.Count > 0)
				return _entriesList[0];
			else
				return null;
		}


		[Test]
		public void HelperGetsActivationCall()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			_entriesList.Add(entry);
			int activations = Db4oLexModelHelper.Singleton.ActivationCount;
			entry = CycleDatabase();
			Assert.AreEqual(1, _entriesList.Count);
			Assert.AreEqual(activations + 1, Db4oLexModelHelper.Singleton.ActivationCount);
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

		[Test]
		public void FindEntriesFromLexemeForm()
		{
			CycleDatabase();
			LexEntry entry = new LexEntry();
			entry.LexicalForm["en"] = "findme";
			_entriesList.Add(entry);
			CycleDatabase();
			//don't want to find this one
			_dataSource.Data.Set(new LanguageForm("en", "findme", null));

			List<LexEntry> list = FindObjectsFromLanguageForm<LexEntry, LexicalFormMultiText>("findme");
			Assert.AreEqual(1, list.Count);
		}

		[Test]
		public void FindEntriesFromGloss()
		{
			CycleDatabase();
			string gloss = "ant";
			AddEntryWithGloss(gloss);
			CycleDatabase();
			//don't want to find this one
			_dataSource.Data.Set(new LanguageForm("en", gloss, null));

			List<LexEntry> list = FindObjectsFromLanguageForm<LexEntry, SenseGlossMultiText>(gloss);
			Assert.AreEqual(1, list.Count);
		}

		private void AddEntryWithGloss(string gloss)
		{
			LexEntry entry = new LexEntry();
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss["en"] = gloss;
			_entriesList.Add(entry);
		}

		private List<AncestorType> FindObjectsFromLanguageForm<AncestorType,MultiTextType>(string match) where AncestorType : class where MultiTextType : MultiText
		{
			Db4objects.Db4o.Query.IQuery q = _dataSource.Data.Query();
			q.Constrain(typeof(LanguageForm));
			q.Descend("_form").Constrain(match);
			q.Descend("_parent").Constrain(typeof(MultiTextType));
			IObjectSet matches = q.Execute();

			return FindAncestorsOfLanguageForms<AncestorType, MultiTextType>(matches);
		}

		private static List<AncestorType> FindAncestorsOfLanguageForms<AncestorType, MultiTextType>(IObjectSet matches)
			where AncestorType : class
			where MultiTextType : MultiText
		{
			List<AncestorType> list = new List<AncestorType>(matches.Count);
			foreach (LanguageForm languageForm in matches)
			{
				MultiTextType multiText = languageForm.Parent as MultiTextType;

				//walk up the tree until we find a parent of the desired class
				WeSayDataObject parent = multiText.ParentAsObject as WeSayDataObject;
				while ((parent != null) && !(parent is AncestorType))
				{
					parent = parent.Parent;
				}
				if (parent != null)
				{
					list.Add(parent as AncestorType);
				}
			}
			return list;
		}

		private bool HasMatchingLexemeForm(LanguageForm form)
		{
			return form.Form == "findme" && form.Parent.GetType() == typeof( LexicalFormMultiText) ;
		}
	}
}
