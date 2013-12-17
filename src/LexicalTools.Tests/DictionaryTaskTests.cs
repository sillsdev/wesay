using System;
using System.IO;
using NUnit.Framework;
using WeSay.Foundation.Tests.TestHelpers;
using WeSay.LexicalModel;
using WeSay.LexicalTools.DictionaryBrowseAndEdit;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class DictionaryTaskTests: TaskBaseTests
	{
		private LexEntryRepository _lexEntryRepository;
		private ViewTemplate _viewTemplate;
		private string _filePath;
		private TemporaryFolder _tempFolder;

		[SetUp]
		public void Setup()
		{
			_tempFolder = new TemporaryFolder();
			_filePath = _tempFolder.GetTemporaryFile();

			WeSayWordsProject.InitializeForTests();
			string[] vernacularWritingSystemIds = new string[]
													  {
															  BasilProject.Project.WritingSystems.
																	  TestWritingSystemVernId
													  };
			_viewTemplate = new ViewTemplate();
			_viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(),
										"LexEntry",
										vernacularWritingSystemIds));
			_viewTemplate.Add(new Field("Note",
										"LexEntry",
										new string[] {"en"},
										Field.MultiplicityType.ZeroOr1,
										"MultiText"));
			_lexEntryRepository = new LexEntryRepository(_filePath);
			_task = new DictionaryTask( DictionaryBrowseAndEditConfiguration.CreateForTests(),  _lexEntryRepository, _viewTemplate);//, new UserSettingsForTask());
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
		public void CreateAndActivate_UserSettingsIsEmpty_Ok()
		{
			var task = new DictionaryTask(DictionaryBrowseAndEditConfiguration.CreateForTests(), _lexEntryRepository, _viewTemplate);//, new UserSettingsForTask());
			task.Activate();
			task.Deactivate();
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullRecordListManager_Throws()
		{
			new DictionaryTask(DictionaryBrowseAndEditConfiguration.CreateForTests(), null, _viewTemplate);//, new UserSettingsForTask());
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Create_NullviewTemplate_Throws()
		{
			new DictionaryTask(DictionaryBrowseAndEditConfiguration.CreateForTests(), _lexEntryRepository, null);//, new UserSettingsForTask());
		}
	}
}