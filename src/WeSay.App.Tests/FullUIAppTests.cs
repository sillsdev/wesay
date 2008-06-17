namespace WeSay.App.Tests
{
#if forFuture
	[TestFixture]
	public class FullUIAppTests : NUnitFormTest
	{
		private TabbedForm tabbedForm;
		private string _projectFolder;
		private FormTester _mainWindowTester;

		WeSayWordsProject _project;
		private LexEntryRepository _lexEntryRepository;



		public override void Setup()
		{
			base.Setup();
			this.tabbedForm = new TabbedForm();
			this.tabbedForm.Show();
			string name = new Finder().Name(this.tabbedForm);
			_mainWindowTester = new FormTester(name);


			_project = new WeSayWordsProject();
			_project.StringCatalogSelector = "en";
			_project.LoadFromProjectDirectoryPath(WeSayWordsProject.GetPretendProjectDirectory());
			_project.Tasks = new List<ITask>();
			_project.Tasks.Add(new MockTask("Dashboard", "The control center.", true));


			_lexEntryRepository = new InMemoryRecordListManager();
			LexEntry entry = new LexEntry();
			_lexEntryRepository.Get<LexEntry>().Add(entry);
			_project.Tasks.Add(new WeSay.LexicalTools.EntryDetailTask(_lexEntryRepository));

		}


		public override void TearDown()
		{
			base.TearDown();
//            if (Directory.Exists(_projectFolder))
//            {
//                Directory.Delete(_projectFolder, true);
//            }

//            _project.Tasks.Clear();
			this.tabbedForm.Dispose();
			_project.Dispose();
			_lexEntryRepository.Dispose();

		}

		[Test]
		public void AAA()
		{
			tabbedForm.ActiveTask = _project.Tasks[1];
			tabbedForm.ActiveTask = _project.Tasks[0];
		}


	}
#endif
}