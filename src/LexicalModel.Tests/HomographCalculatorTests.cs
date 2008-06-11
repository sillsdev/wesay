using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalModel.Tests
{
	/// <summary>
	/// NB: we don't test the citationform-lexemeform interactions here, as that is the job of
	/// the GetHeadword method of LexemeForm, which is tested elsewhere.
	/// </summary>
	[TestFixture]
	public class HomographCalculatorTests
	{
		private Db4oRecordList<LexEntry> _records;
		protected string _dbFile;
		protected Db4oDataSource _dataSource;
		Db4oRecordListManager _recordListManager;
		private WritingSystem _headwordWritingSystem;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			_dbFile = Path.GetTempFileName();
			_recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _dbFile);
			Lexicon.Init((Db4oRecordListManager)_recordListManager);
			Db4oLexModelHelper.Initialize(_recordListManager.DataSource.Data);
			_records = (Db4oRecordList<LexEntry>) _recordListManager.GetListOfType<LexEntry>();
			_headwordWritingSystem = new WritingSystem();
			this._headwordWritingSystem.Id = "primary";
			WeSay.Project.WeSayWordsProject.Project.WritingSystems.Add("primary", this._headwordWritingSystem);
		}
		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
		}

		[Test]
		public void GetHomographNumber_OnlyOneEntry_Returns0()
		{
			FluentEntry entry1 = MakeEntry().WithLexemeForm("primary", "blue");
			Assert.AreEqual(0, Lexicon.GetHomographNumber(entry1, _headwordWritingSystem));
		}



		[Test]
		public void GetHomographNumber_FirstEntryWithFollowingHomograph_Returns1()
		{
			FluentEntry entry1 = MakeEntry().WithLexemeForm("primary", "blue");
			FluentEntry entry2 = MakeEntry().WithLexemeForm("primary", "blue");
			Assert.AreEqual(1, Lexicon.GetHomographNumber(entry1, _headwordWritingSystem));
		}



		[Test]
		public void GetHomographNumber_SecondEntry_Returns2()
		{
			FluentEntry entry1 = MakeEntry().WithLexemeForm("primary", "blue");
			FluentEntry entry2 = MakeEntry().WithLexemeForm("primary", "blue");
			Assert.AreEqual(2, Lexicon.GetHomographNumber(entry2, _headwordWritingSystem));
		}



		[Test]
		public void GetHomographNumber_ThirdEntry_Returns3()
		{
			FluentEntry entry0 = MakeEntry().WithLexemeForm("en", "blue");
			FluentEntry entry1 = MakeEntry().WithLexemeForm("primary", "blue");
		   FluentEntry entry2 = MakeEntry().WithLexemeForm("primary", "blue");
			FluentEntry entry3 = MakeEntry().WithLexemeForm("primary", "blue");
			Assert.AreEqual(3, Lexicon.GetHomographNumber(entry3, _headwordWritingSystem));
		}

		[Test]
		public void GetHomographNumber_3SameLexicalForms_Returns123()
		{
			FluentEntry entry1 = MakeEntry().WithLexemeForm("primary", "blue");
			FluentEntry entry2 = MakeEntry().WithLexemeForm("primary", "blue");
			FluentEntry entry3 = MakeEntry().WithLexemeForm("primary", "blue");
			Assert.AreEqual(1, Lexicon.GetHomographNumber(entry1, _headwordWritingSystem));
			Assert.AreEqual(3, Lexicon.GetHomographNumber(entry3, _headwordWritingSystem));
			Assert.AreEqual(2, Lexicon.GetHomographNumber(entry2, _headwordWritingSystem));
		}

		[Test, Ignore("not implemented")]
		public void GetHomographNumber_HonorsOrderAttribute()
		{
		}

		private FluentEntry MakeEntry()
		{
			FluentEntry entry = new FluentEntry();
			 _records.Add(entry);
			return entry;
		}

		internal class FluentEntry : LexEntry
		{
			public FluentEntry WithLexemeForm(string writingSystemId, string lexicalunit)
			{
				this.LexicalForm.SetAlternative(writingSystemId, lexicalunit);
				return this;
			}
			public FluentEntry WithCitationForm(string writingSystemId, string lexicalunit)
			{
				this.GetOrCreateProperty<MultiText>(LexEntry.WellKnownProperties.Citation).SetAlternative(writingSystemId, lexicalunit);
				return this;
			}
		}
	}
}
