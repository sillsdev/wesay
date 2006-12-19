using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using LiftIO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LiftMergerTests
	{
		private LiftMerger _merger;
		protected Db4oDataSource _dataSource;
		protected Db4oRecordList<LexEntry> _entries;
		private string _tempFile;

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			WeSayWordsProject.InitializeForTests();

			_tempFile = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_tempFile);
			_entries = new WeSay.Data.Db4oRecordList<LexEntry>(_dataSource);
			_merger = new LiftMerger(_dataSource);
		}

		[TearDown]
		public void TearDown()
		{
			_merger.Dispose();
			_entries.Dispose();
			_dataSource.Dispose();
			File.Delete(_tempFile);
		}

		[Test]
		public void NewEntryWithGuid()
		{
			IdentifyingInfo idInfo = new IdentifyingInfo();
			idInfo.id = Guid.NewGuid().ToString();
			LexEntry e= _merger.GetOrMakeEntry(idInfo);
			Assert.AreEqual(idInfo.id, e.Guid.ToString());
		}


		[Test]
		public void NewEntryWithTextIdIgnoresIt()
		{
			IdentifyingInfo idInfo = new IdentifyingInfo();
			idInfo.id = "hello";
			LexEntry e = _merger.GetOrMakeEntry(idInfo);
			//no attempt is made to use that id
			Assert.IsNotNull(e.Guid);
		}

		[Test]
		public void NewEntryTakesGivenDates()
		{
			IdentifyingInfo idInfo = new IdentifyingInfo();
			idInfo = AddDates(idInfo);

			LexEntry e = _merger.GetOrMakeEntry(idInfo);
			Assert.AreEqual(idInfo.creationTime, e.CreationTime);
			Assert.AreEqual(idInfo.modificationTime, e.ModificationTime);
		}



		[Test]
		public void NewEntryNoDatesUsesNow()
		{
			LexEntry e = MakeSimpleEntry();
			Assert.IsTrue(DateTime.UtcNow.Ticks - e.CreationTime.Ticks < 10000);
			Assert.IsTrue(DateTime.UtcNow.Ticks - e.ModificationTime.Ticks < 10000);
		}

		private LexEntry MakeSimpleEntry()
		{
			IdentifyingInfo idInfo = new IdentifyingInfo();
			return _merger.GetOrMakeEntry(idInfo);
		}

		[Test]
		public void EntryGetsEmptyLexemeForm()
		{
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInLexemeForm(e, new StringDictionary());
			Assert.AreEqual(0, e.LexicalForm.Count);
		}

		[Test]
		public void EntryGetsLexemeFormWithUnheardOfLanguage()
		{
			LexEntry e = MakeSimpleEntry();
			StringDictionary forms = new StringDictionary();
			forms.Add("x99", "hello");
			_merger.MergeInLexemeForm(e, forms);
			Assert.AreEqual("hello", e.LexicalForm["x99"]);
		}

		[Test]
		public void NewEntryGetsLexemeForm()
		{
			LexEntry e = MakeSimpleEntry();
			StringDictionary forms = new StringDictionary();
			forms.Add("x", "hello");
			forms.Add("y", "bye");
			_merger.MergeInLexemeForm(e, forms);
			Assert.AreEqual(2, e.LexicalForm.Count);
		}


		[Test]
		public void TryCompleteEntry()
		{
			IdentifyingInfo id = new IdentifyingInfo();
			LexEntry e = MakeSimpleEntry();
			LexSense s= _merger.GetOrMergeSense(e, id);
			LexExampleSentence ex =_merger.GetOrMergeExample(s, id);

			Assert.AreEqual(e, ex.Parent.Parent);
		}

		[Test, Ignore("Haven't implemented protecting modified dates of, e.g., the entry as you add/merge its children.")]
		public void ModifiedDatesRetained()
		{
		}

		[Test]
		public void ChangedEntryFound()
		{
			Guid g = Guid.NewGuid();
			IdentifyingInfo idInfo = CreateFullIdInfo(g);

			LexEntry e = new LexEntry(g);
			e.CreationTime = idInfo.creationTime;
			e.ModificationTime = new DateTime(e.CreationTime.Ticks + 100);
			_entries.Add(e);

			LexEntry found = _merger.GetOrMakeEntry(idInfo);
			Assert.AreEqual(idInfo.creationTime, found.CreationTime);
		}

		[Test]
		public void UnchangedEntryPruned()
		{
			Guid g = Guid.NewGuid();
			IdentifyingInfo idInfo = CreateFullIdInfo( g);

			LexEntry e = new LexEntry(g);
			e.CreationTime = idInfo.creationTime;
			e.ModificationTime = idInfo.modificationTime;
			_entries.Add(e);

			LexEntry found = _merger.GetOrMakeEntry(idInfo);
			Assert.IsNull(found);
		}

		[Test]
		public void EntryWithIncomingUnspecifiedModTimeNotPruned()
		{
			Guid g = Guid.NewGuid();
			IdentifyingInfo idInfo = CreateFullIdInfo(g);
			LexEntry e = new LexEntry(g);
			e.CreationTime = idInfo.creationTime;
			e.ModificationTime = idInfo.modificationTime;
			_entries.Add(e);

		   //strip out the time
			idInfo.modificationTime = DateTime.Parse("2005-01-01");
			Assert.AreEqual(DateTimeKind.Unspecified ,idInfo.modificationTime.Kind );

			LexEntry found = _merger.GetOrMakeEntry(idInfo);
			Assert.IsNotNull(found);
		}

		[Test, Ignore("Haven't implemented this.")]
		public void MergingSameEntryLackingGuidId_TwiceFindMatch()
		{
		}

		private static IdentifyingInfo AddDates(IdentifyingInfo idInfo)
		{
			idInfo.creationTime = DateTime.Parse("2003-08-07T08:42:42+07:00");
			idInfo.modificationTime = DateTime.Parse("2005-01-01T01:11:11+01:00");
			return idInfo;
		}

		private static IdentifyingInfo CreateFullIdInfo(Guid g)
		{
			IdentifyingInfo idInfo = new IdentifyingInfo();
			idInfo.id = g.ToString();
			idInfo = AddDates(idInfo);
			return idInfo;
		}
	}

}