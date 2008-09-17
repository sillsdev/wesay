using System.IO;
using NUnit.Framework;
using Palaso.Services.Dictionary;
using TestUtilities;
using WeSay.App.Services;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests.Db4oSpecific;

namespace WeSay.App.Tests.Services
{
	/// <summary>
	/// These tests just test the details of some calls; the tests in
	/// _dictionaryServiceProviders_CrossApplicationTests are more realistic and more
	/// likely to catch threading issues, but they are slow to run.
	///
	/// Tests here are easier to debug, because we can step into the execution.
	/// </summary>
	[TestFixture]
	public class DictionaryService_SameTheadTests
	{
		private LexEntryRepository _lexEntryRepository;
		private TemporaryFolder _tempFolder;
		private string _filePath;

		private Db4oProjectSetupForTesting _projectSetupSharedByAllTests;
		private DictionaryServiceProvider _dictionaryServiceProvider;

		/// <summary>
		/// Db4oProjectSetupForTesting is extremely time consuming to setup, so we reuse it.
		/// </summary>
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			_tempFolder = new TemporaryFolder();
			_projectSetupSharedByAllTests = new Db4oProjectSetupForTesting(string.Empty);
		}

		[SetUp]
		public void Setup()
		{
			_filePath = _tempFolder.GetTemporaryFile();
			_lexEntryRepository = new LexEntryRepository(_filePath);
			_dictionaryServiceProvider = new DictionaryServiceProvider(_lexEntryRepository,
																	   null,
																	   _projectSetupSharedByAllTests
																			   ._project);
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			File.Delete(_filePath);
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			_projectSetupSharedByAllTests.Dispose();
			_tempFolder.Delete();
		}

		private void MakeTestLexEntry(string writingSystemId, string lexicalForm)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(writingSystemId, lexicalForm);
			_lexEntryRepository.SaveItem(entry);
		}

		[Test]
		public void FindsNoExactMatch()
		{
			MakeTestLexEntry("v", "foo1");

			FindResult r = _dictionaryServiceProvider.GetMatchingEntries("v",
																		 "foo",
																		 FindMethods.Exact.ToString());
			Assert.AreEqual(0, r.ids.Length);
		}

		/* These were good tests, but the method under test has been removed froom the service
	   * until we see if it is actually  needed.
	   [Test]
		public void GetFormsFromIdsWhenEachIsOk()
		{
			string a = MakeTestLexEntry(_entries, "v", "foo1").Id;
			string b = MakeTestLexEntry(_entries, "v", "foo2").Id;
			string[] forms = _dictionaryServiceProvider.GetFormsFromIds("v", new string[] {a,b });
		   Assert.AreEqual(2, forms.Length);
		   Assert.AreEqual("foo1", forms[0]);
		   Assert.AreEqual("foo2", forms[1]);
		}

		[Test]
		public void GetFormsFromIdsWithABogusOneInMiddleGivesEmptyString()
		{
			string a = MakeTestLexEntry(_entries, "v", "foo1").Id;
			string b = MakeTestLexEntry(_entries, "v", "foo2").Id;
			string[] forms = _dictionaryServiceProvider.GetFormsFromIds("v", new string[] { a, "bogus", b });
			Assert.AreEqual(3, forms.Length);
			Assert.AreEqual("foo1", forms[0]);
			Assert.AreEqual(string.Empty, forms[1]);
			Assert.AreEqual("foo2", forms[2]);
		}
	   * */

		[Test]
		public void ShouldFindApproximates()
		{
			//NB: specifics of the matching behavior belong in lower level tests of the matcher; we're
			//just checking that it generally does something

			MakeTestLexEntry("v", "foo1");
			MakeTestLexEntry("v2", "foo2");
			MakeTestLexEntry("v", "foo3");

			FindResult r = _dictionaryServiceProvider.GetMatchingEntries("v",
																		 "foo",
																		 FindMethods.
																				 DefaultApproximate.
																				 ToString());
			Assert.AreEqual(2, r.ids.Length);
		}

		[Test]
		public void CreateNewEntryWithExampleButNoDef()
		{
			MakeTestLexEntry("v", "foo1");
			string id = _dictionaryServiceProvider.AddEntry("v",
															"voom",
															null,
															null,
															"v",
															"vlah voom!");
			Assert.IsNotNull(id);

			FindResult r = _dictionaryServiceProvider.GetMatchingEntries("v",
																		 "voom",
																		 FindMethods.Exact.ToString());
			Assert.AreEqual(id, r.ids[0]);
			string html = _dictionaryServiceProvider.GetHtmlForEntries(new string[] {id});
			Assert.IsTrue(html.Contains("vlah voom!"));
		}

		[Test]
		public void CreateNewEntryWithAllFieldsDoesCreateIt()
		{
			string id = _dictionaryServiceProvider.AddEntry("v",
															"voom",
															"en",
															"def of voom",
															"v",
															"vlah voom!");
			Assert.IsNotNull(id);

			FindResult r = _dictionaryServiceProvider.GetMatchingEntries("v",
																		 "voom",
																		 FindMethods.Exact.ToString());
			Assert.AreEqual(id, r.ids[0]);
			string html = _dictionaryServiceProvider.GetHtmlForEntries(new string[] {id});
			Assert.IsTrue(html.Contains("vlah voom!"));
			Assert.IsTrue(html.Contains("def of voom"));
		}

		[Test]
		public void CreateNewEntryWithUnknownWritingSystemReturnsNull()
		{
			string id = _dictionaryServiceProvider.AddEntry("bogus", "voom", null, null, null, null);
			Assert.IsNull(id);
		}

		[Test]
		public void CreateNewEntryWithEmptyLexemeFormReturnsNull()
		{
			string id = _dictionaryServiceProvider.AddEntry("v", "", null, null, null, null);
			Assert.IsNull(id);
		}

		[Test]
		public void CreateNewEntryWithOnlyLexemeFormDoesCreateIt()
		{
			string id = _dictionaryServiceProvider.AddEntry("v", "voom", null, null, null, null);
			Assert.IsNotNull(id);

			FindResult r = _dictionaryServiceProvider.GetMatchingEntries("v",
																		 "voom",
																		 FindMethods.Exact.ToString());
			Assert.AreEqual(id, r.ids[0]);
		}
	}
}