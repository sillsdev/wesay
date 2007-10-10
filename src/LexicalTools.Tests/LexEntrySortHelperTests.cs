using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
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
			foreach (string s in h.GetKeys(new LexEntry()))
			{
				Assert.AreEqual("*No Gloss", s);
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
			foreach (string k in h.GetKeys(e))
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
		public void GetKeys_ForCompoundGlossFallBackWritingSystem_SeparateKeys()
		{
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, _writingSystem, false);
			LexEntry e = new LexEntry();
			LexSense s = (LexSense)e.Senses.AddNew();
			s.Gloss.SetAlternative("analysis", "gloss 1; gloss 2");
			int count = 0;
			foreach (string k in h.GetKeys(e))
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

	}
}
