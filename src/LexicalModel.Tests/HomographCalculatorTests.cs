using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class HomographCalculatorTests
	{
		private HomographCalculator _calculator;
		private Db4oRecordList<LexEntry> _records;
		protected string _dbFile;
		protected Db4oDataSource _dataSource;
		Db4oRecordListManager _recordListManager;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			_dbFile = Path.GetTempFileName();
			_recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _dbFile);
			Db4oLexModelHelper.Initialize(_recordListManager.DataSource.Data);
			_records = (Db4oRecordList<LexEntry>) _recordListManager.GetListOfType<LexEntry>();
			_calculator = new HomographCalculator(_recordListManager, WeSayWordsProject.Project.WritingSystems.UnknownAnalysisWritingSystem);
		}

		[Test]
		public void GetHomographNumber_NoHomograph_Returns0()
		{
			LexEntry entry = new LexEntry();
			Assert.AreEqual(0, _calculator.GetHomographNumber(entry));
		}

		[Test,Ignore("not implemented")]
		public void GetHomographNumber_3SameLexicalForms_Returns123()
		{
			FluentEntry entry1 = MakeEntry().WithLexemeForm("one", "blue");
			FluentEntry entry2 = MakeEntry().WithLexemeForm("one", "blue");
			FluentEntry entry3 = MakeEntry().WithLexemeForm("one", "blue");
			Assert.AreEqual(1, _calculator.GetHomographNumber(entry1));
			Assert.AreEqual(3, _calculator.GetHomographNumber(entry3));
			Assert.AreEqual(2, _calculator.GetHomographNumber(entry2));
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
		}
	}
}
