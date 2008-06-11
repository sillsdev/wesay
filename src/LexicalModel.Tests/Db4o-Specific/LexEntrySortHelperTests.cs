using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntrySortHelperTests
	{
		ViewTemplate _viewTemplate;
		string _filePath;
		IRecordListManager _recordListManager;
		private WritingSystem _writingSystem;

		[SetUp]
		public void Setup()
		{
			this._writingSystem = new WritingSystem("pretendVernacular", new Font(FontFamily.GenericSansSerif, 24));
			string[] vernacularWritingSystemIds = new string[] { this._writingSystem.Id };

			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry", vernacularWritingSystemIds));

			_filePath = Path.GetTempFileName();

			_recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _filePath);

		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
			File.Delete(_filePath);
		}


		[Test]
		public void Constuct()
		{
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, true);
			Assert.IsNotNull(h);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullDataSource_Throws()
		{
			new LexEntrySortHelper(null, _writingSystem, true);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullWritingSystemId_Throws()
		{
			new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, null, true);
		}

		[Test]
		public void GetKeys_ForGloss_LexEntryHasNoSenses_NotEmpty()
		{
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, false);
			int count = 0;
			foreach (string s in h.GetDisplayStrings(new LexEntry()))
			{
				Assert.AreEqual("(No Gloss)", s);
				count++;
			}
			Assert.AreEqual(1, count);
		}

		[Test]
		public void GetKeys_ForCompoundGlossExactWritingSystem_SeparateKeys()
		{
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, false);
			LexEntry e = new LexEntry();
			LexSense s = (LexSense) e.Senses.AddNew();
			s.Gloss.SetAlternative(_writingSystem.Id, "gloss 1; gloss 2");
			int count = 0;
			foreach (string k in h.GetDisplayStrings(e))
			{
				switch(count)
				{
					case 0:
						Assert.AreEqual("gloss 1", k);
						break;
					case 1:
						Assert.AreEqual("gloss 2", k);
						break;
				}
				count++;
			}
			Assert.AreEqual(2, count);
		}

		[Test]
		public void GetKeys_ForSenseWithOnlyDefinition()
		{
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, false);
			LexEntry e = new LexEntry();
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Definition.SetAlternative(_writingSystem.Id, "a definition");

			List<string> keys = new List<string>(h.GetDisplayStrings(e));
			Assert.IsTrue(keys.Contains("a definition"));
			Assert.AreEqual(1, keys.Count);
		}


		[Test]
		public void GetKeys_ForSenseOnlyGloss()
		{
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, false);
			LexEntry e = new LexEntry();
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Gloss.SetAlternative(_writingSystem.Id, "a gloss");

			List<string> keys = new List<string>(h.GetDisplayStrings(e));
			Assert.IsTrue(keys.Contains("a gloss"));
			Assert.AreEqual(1, keys.Count);
		}

		[Test]
		public void GetKeys_ForSenseWithSameGlossAndDefinition_NoDuplicates()
		{
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, false);
			LexEntry e = new LexEntry();
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Definition.SetAlternative(_writingSystem.Id, "hamburger");
			s.Gloss.SetAlternative(_writingSystem.Id, "hamburger");

			List<string> keys = new List<string>(h.GetDisplayStrings(e));
			Assert.IsTrue(keys.Contains("hamburger"));
			Assert.AreEqual(1, keys.Count);
		}

		[Test]
		public void GetKeys_ForSenseWithDifferntGlossAndDefinition_GetBoth()
		{
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, false);
			LexEntry e = new LexEntry();
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Definition.SetAlternative(_writingSystem.Id, "hamburger");
			s.Gloss.SetAlternative(_writingSystem.Id, "fries");

			List<string> keys = new List<string>(h.GetDisplayStrings(e));
			Assert.IsTrue(keys.Contains("hamburger"));
			Assert.IsTrue(keys.Contains("fries"));
			Assert.AreEqual(2, keys.Count);
		}

		[Test]
		public void GetKeys_ForCompoundGlossFallBackWritingSystem_SeparateKeys()
		{
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, false);
			LexEntry e = new LexEntry();
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Gloss.SetAlternative("analysis", "gloss 1; gloss 2");
			int count = 0;
			foreach (string k in h.GetDisplayStrings(e))
			{
				switch (count)
				{
					case 0:
						Assert.AreEqual("gloss 1*", k);
						break;
					case 1:
						Assert.AreEqual("gloss 2*", k);
						break;
				}
				count++;
			}
			Assert.AreEqual(2, count);
		}


		[Test]
		public void GetKeyIdPairs_NoGlossesOrDefinitions()
		{
			GetKeyIdPairs("", "", new string[] {});
		}

		[Test]
		public void GetKeyIdPairs_Just2Glosses()
		{
			GetKeyIdPairs("a ; b", "", new string[] { "a", "b"});
		}

		[Test]
		public void GetKeyIdPairs_JustDefinition()
		{
			GetKeyIdPairs("", "a", new string[] { "a" });
		}
		[Test]
		public void GetKeyIdPairs_MixtureOfGlossesAndDefinitions()
		{
			GetKeyIdPairs("a ; b", "a; c", new string[] { "a", "b", "c" });
		}

		private void GetKeyIdPairs(string glosses, string definition, string[] expectedKeys)
		{
			Lexicon.Init((Db4oRecordListManager) _recordListManager);
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, false);
			LexEntry e = new LexEntry();


			LexSense s = (LexSense)e.Senses.AddNew();
			s.Gloss.SetAlternative(_writingSystem.Id, glosses);
			s.Definition.SetAlternative(_writingSystem.Id, definition);
			int count = 0;

			Db4oRecordList<LexEntry> entriesList;

			entriesList = (Db4oRecordList<LexEntry>) _recordListManager.GetListOfType<LexEntry>();
			entriesList.Add(e);
			long entryId = ((Db4oRecordListManager)_recordListManager).DataSource.Data.Ext().GetID(e);

			foreach (RecordToken recordToken in h.GetRecordTokensForMatchingRecords())
			{
				Assert.AreEqual(expectedKeys[count], recordToken.DisplayString);
				Assert.AreEqual(entryId, recordToken.Id);
				count++;
			}
			Assert.AreEqual(expectedKeys.Length, count);
		}
	}
}