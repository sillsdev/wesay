using System;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class EntryDetailTaskTests : TaskBaseTests
	{
		IRecordListManager _recordListManager;
		ViewTemplate _viewTemplate;
		private string _filePath;

		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();

			BasilProject.InitializeForTests();
			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemVernId };
			_viewTemplate = new ViewTemplate();
			this._viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), analysisWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), analysisWritingSystemIds));

			_recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _filePath);
			Db4oLexModelHelper.Initialize(((Db4oRecordListManager)_recordListManager).DataSource.Data);

			_task = new EntryDetailTask(_recordListManager, this._viewTemplate);
		}

		[TearDown]
		public void TearDown()
		{
			_recordListManager.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void Create()
		{
			Assert.IsNotNull(_task);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullRecordListManager_Throws()
		{
			new EntryDetailTask(null, this._viewTemplate);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			using (IRecordListManager recordListManager = new InMemoryRecordListManager())
			{
				new EntryDetailTask(recordListManager, null);
			}
		}

	}

}