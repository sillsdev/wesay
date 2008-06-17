using System;
using System.IO;
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class DictionaryTaskTests: TaskBaseTests
	{
		private LexEntryRepository _lexEntryRepository;
		private ViewTemplate _viewTemplate;
		private string _filePath;

		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();

			WeSayWordsProject.InitializeForTests();
			string[] vernacularWritingSystemIds =
					new string[] {BasilProject.Project.WritingSystems.TestWritingSystemVernId};
			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(
					new Field(Field.FieldNames.EntryLexicalForm.ToString(),
							  "LexEntry",
							  vernacularWritingSystemIds));
			_viewTemplate.Add(
					new Field("Note",
							  "LexEntry",
							  new string[] {"en"},
							  Field.MultiplicityType.ZeroOr1,
							  "MultiText"));
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
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullRecordListManager_Throws()
		{
			new DictionaryTask(null, _viewTemplate);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			new DictionaryTask(_lexEntryRepository, null);
		}
	}
}