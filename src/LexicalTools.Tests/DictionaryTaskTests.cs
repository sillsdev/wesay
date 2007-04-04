using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class DictionaryTaskTests : TaskBaseTests
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
			this._viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry",vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field("Note", "LexEntry", new string[]{"en"}, Field.MultiplicityType.ZeroOr1, "MultiText" ));
			//            this._viewTemplate.Add(new Field(Field.FieldNames.SenseGloss.ToString(), "LexSense", analysisWritingSystemIds));
//            this._viewTemplate.Add(new Field(Field.FieldNames.ExampleSentence.ToString(), "LexExampleSentence", vernacularWritingSystemIds));
//            this._viewTemplate.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), "LexExampleSentence",analysisWritingSystemIds));
//
			_recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _filePath);
			Db4oLexModelHelper.Initialize(((Db4oRecordListManager)_recordListManager).DataSource.Data);

			_task = new DictionaryTask(_recordListManager, this._viewTemplate);
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
			new DictionaryTask(null, this._viewTemplate);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			using (IRecordListManager recordListManager = new InMemoryRecordListManager())
			{
				new DictionaryTask(recordListManager, null);
			}
		}


	}

}