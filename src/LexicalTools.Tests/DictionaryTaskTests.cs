using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class DictionaryTaskTests : TaskBaseTests
	{
		LexEntryRepository _lexEntryRepository;
		ViewTemplate _viewTemplate;
		private string _filePath;

		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();

			WeSayWordsProject.InitializeForTests();
			string[] analysisWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemAnalId };
			string[] vernacularWritingSystemIds = new string[] { BasilProject.Project.WritingSystems.TestWritingSystemVernId };
			_viewTemplate = new ViewTemplate();
			this._viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry",vernacularWritingSystemIds));
			this._viewTemplate.Add(new Field("Note", "LexEntry", new string[]{"en"}, Field.MultiplicityType.ZeroOr1, "MultiText" ));
			_lexEntryRepository = new LexEntryRepository(_filePath);
			_task = new DictionaryTask(_lexEntryRepository, _viewTemplate);
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
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
			DictionaryTask task = new DictionaryTask(_lexEntryRepository, null);
		}


	}

}