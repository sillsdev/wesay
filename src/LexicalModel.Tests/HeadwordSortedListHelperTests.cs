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
	public class HeadwordSortedListHelperTests
	{
		ViewTemplate _viewTemplate;
		string _filePath;
		LexEntryRepository _recordListManager;
		private WritingSystem _headwordWritingSystem;
		private IRecordList<LexEntry> _entries;

		[SetUp]
		public void Setup()
		{
			this._headwordWritingSystem = new WritingSystem("pretendVernacular", new Font(FontFamily.GenericSansSerif, 24));
			string[] vernacularWritingSystemIds = new string[] { this._headwordWritingSystem.Id };

			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry", vernacularWritingSystemIds));

			_filePath = Path.GetTempFileName();

			_recordListManager = new LexEntryRepository(new WeSayWordsDb4oModelConfiguration(), _filePath);
			Db4oLexModelHelper.Initialize(_recordListManager.DataSource.Data);
			_entries = _recordListManager.GetListOfType<LexEntry>();

		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void GetKeys_3EntriesWithLexemeForms_PairsAreSorted()
		{
			LexEntry e1 = (LexEntry) _entries.AddNew();
			e1.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "bank");
			long bankId = _recordListManager.DataSource.Data.Ext().GetID(e1);

			LexEntry e2 = (LexEntry)_entries.AddNew();
			e2.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "apple");
			long appleId = _recordListManager.DataSource.Data.Ext().GetID(e2);

			LexEntry e3 = (LexEntry)_entries.AddNew();
			e3.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "xa");//has to be something low in the alphabet to test a bug we had
			long pearId = _recordListManager.DataSource.Data.Ext().GetID(e3);

			HeadwordSortedListHelper helper = new HeadwordSortedListHelper(_recordListManager,
													 _headwordWritingSystem);
			List<RecordToken> list = _recordListManager.GetSortedList(helper);

			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(appleId, list[0].Id);
			Assert.AreEqual(bankId, list[1].Id);
			Assert.AreEqual(pearId, list[2].Id);
		}

	}
}