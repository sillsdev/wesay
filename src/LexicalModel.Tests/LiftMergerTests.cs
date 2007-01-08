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
	public class LiftMergerTests : LiftIO.ILiftMergerTestSuite
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
		public  void NewEntryWithGuid()
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo.Id = Guid.NewGuid().ToString();
			LexEntry e= _merger.GetOrMakeEntry(extensibleInfo);
			Assert.AreEqual(extensibleInfo.Id, e.Guid.ToString());
		}


		[Test]
		public  void NewEntryWithTextIdIgnoresIt()
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo.Id = "hello";
			LexEntry e = _merger.GetOrMakeEntry(extensibleInfo);
			//no attempt is made to use that id
			Assert.IsNotNull(e.Guid);
			Assert.AreNotSame(Guid.Empty, e.Guid);
		}

		[Test]
		public  void NewEntryTakesGivenDates()
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo = AddDates(extensibleInfo);

			LexEntry e = _merger.GetOrMakeEntry(extensibleInfo);
			Assert.AreEqual(extensibleInfo.CreationTime, e.CreationTime);
			Assert.AreEqual(extensibleInfo.ModificationTime, e.ModificationTime);
		}




		[Test]
		public  void NewEntryNoDatesUsesNow()
		{
			LexEntry e = MakeSimpleEntry();
			Assert.IsTrue(TimeSpan.FromTicks(DateTime.UtcNow.Ticks - e.CreationTime.Ticks).Seconds < 2);
			Assert.IsTrue(TimeSpan.FromTicks(DateTime.UtcNow.Ticks - e.ModificationTime.Ticks).Seconds < 2);
		}

		private LexEntry MakeSimpleEntry()
		{
			Extensible extensibleInfo = new Extensible();
			return _merger.GetOrMakeEntry(extensibleInfo);
		}

		[Test]
		public  void EntryGetsEmptyLexemeForm()
		{
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInLexemeForm(e, new SimpleMultiText());
			Assert.AreEqual(0, e.LexicalForm.Count);
		}

		#region ILiftMergerTestSuite Members

		[Test, Ignore("not yet")]
		public void NewWritingSystemAlternativeHandled()
		{
		}

		#endregion

		[Test]
		public  void EntryGetsLexemeFormWithUnheardOfLanguage()
		{
			LexEntry e = MakeSimpleEntry();
			SimpleMultiText forms = new SimpleMultiText();
			forms.Add("x99", "hello");
			_merger.MergeInLexemeForm(e, forms);
			Assert.AreEqual("hello", e.LexicalForm["x99"]);
		}

		[Test]
		public  void NewEntryGetsLexemeForm()
		{
			LexEntry e = MakeSimpleEntry();
			SimpleMultiText forms = new SimpleMultiText();
			forms.Add("x", "hello");
			forms.Add("y", "bye");
			_merger.MergeInLexemeForm(e, forms);
			Assert.AreEqual(2, e.LexicalForm.Count);
		}


		[Test]
		public  void TryCompleteEntry()
		{
			Extensible extensibleInfo = new Extensible();
			LexEntry e = MakeSimpleEntry();
			LexSense s= _merger.GetOrMakeSense(e, extensibleInfo);
			LexExampleSentence ex =_merger.GetOrMakeExample(s, extensibleInfo);

			Assert.AreEqual(e, ex.Parent.Parent);
		}

		[Test, Ignore("Haven't implemented protecting modified dates of, e.g., the entry as you add/merge its children.")]
		public  void ModifiedDatesRetained()
		{
		}

		[Test]
		public  void ChangedEntryFound()
		{
			Guid g = Guid.NewGuid();
			Extensible extensibleInfo = CreateFullextensibleInfo(g);

			LexEntry e = new LexEntry(g);
			e.CreationTime = extensibleInfo.CreationTime;
			e.ModificationTime = new DateTime(e.CreationTime.Ticks + 100);
			_entries.Add(e);

			LexEntry found = _merger.GetOrMakeEntry(extensibleInfo);
			Assert.AreEqual(extensibleInfo.CreationTime, found.CreationTime);
		}

		[Test]
		public  void UnchangedEntryPruned()
		{
			Guid g = Guid.NewGuid();
			Extensible extensibleInfo = CreateFullextensibleInfo( g);

			LexEntry e = new LexEntry(g);
			e.CreationTime = extensibleInfo.CreationTime;
			e.ModificationTime = extensibleInfo.ModificationTime;
			_entries.Add(e);

			LexEntry found = _merger.GetOrMakeEntry(extensibleInfo);
			Assert.IsNull(found);
		}

		[Test]
		public  void EntryWithIncomingUnspecifiedModTimeNotPruned()
		{
			Guid g = Guid.NewGuid();
			Extensible extensibleInfo = CreateFullextensibleInfo(g);
			LexEntry e = new LexEntry(g);
			e.CreationTime = extensibleInfo.CreationTime;
			e.ModificationTime = extensibleInfo.ModificationTime;
			_entries.Add(e);

		   //strip out the time
			extensibleInfo.ModificationTime = DateTime.Parse("2005-01-01");
			Assert.AreEqual(DateTimeKind.Unspecified ,extensibleInfo.ModificationTime.Kind );

			LexEntry found = _merger.GetOrMakeEntry(extensibleInfo);
			Assert.IsNotNull(found);
		}

		[Test, Ignore("Haven't implemented this.")]
		public  void MergingSameEntryLackingGuidId_TwiceFindMatch()
		{
		}

		private static Extensible AddDates(Extensible extensibleInfo)
		{
			extensibleInfo.CreationTime = DateTime.Parse("2003-08-07T08:42:42+07:00");
			extensibleInfo.ModificationTime = DateTime.Parse("2005-01-01T01:11:11+01:00");
			return extensibleInfo;
		}

		private static Extensible CreateFullextensibleInfo(Guid g)
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo.Id = g.ToString();
			extensibleInfo = AddDates(extensibleInfo);
			return extensibleInfo;
		}
	}

}