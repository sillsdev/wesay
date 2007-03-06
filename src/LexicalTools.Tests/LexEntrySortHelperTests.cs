using System;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
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

		[SetUp]
		public void Setup()
		{
			string[] vernacularWritingSystemIds = new string[] { "pretendVernacular" };

			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), vernacularWritingSystemIds));

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
			LexEntrySortHelper h = new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, "pretendVernacular", true);
			Assert.IsNotNull(h);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullDataSource_Throws()
		{
			new LexEntrySortHelper(null, "pretendVernacular", true);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Construct_NullWritingSystemId_Throws()
		{
			new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource, null, true);
		}
	}
}
