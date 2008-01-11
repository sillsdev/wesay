using NUnit.Framework;
using Palaso.Services.Dictionary;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.Project.Tests;

namespace WeSay.App.Tests
{
	/// <summary>
	/// These tests just test the details of some calls; the tests in
	/// _dictionaryServiceProviders_CrossApplicationTests are more realistic and more
	/// likely to catch threading issues, but they are slow to run.
	///
	/// Tests here are easier to debug, because we can step into the execution.
	/// </summary>
	[TestFixture]
	public class _dictionaryServiceProviders_SameTheadTests
	{
		private Db4oProjectSetupForTesting _projectSetupSharedByAllTests;
		private IRecordList<LexEntry> _entries;
		private DictionaryServiceProvider _dictionaryServiceProvider;

		/// <summary>
		/// Db4oProjectSetupForTesting is extremely time consuming to setup, so we reuse it.
		/// </summary>
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			_projectSetupSharedByAllTests = new Db4oProjectSetupForTesting(string.Empty);
			_entries = _projectSetupSharedByAllTests._recordListManager.GetListOfType<LexEntry>();
		}

		[SetUp]
		public void Setup()
		{
			_entries.Clear();//let each test have a clean slate
			_dictionaryServiceProvider = new DictionaryServiceProvider(null, _projectSetupSharedByAllTests._project);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			_projectSetupSharedByAllTests.Dispose();
		}

		private static LexEntry MakeTestLexEntry(IRecordList<LexEntry> entries, string writingSystemId, string lexicalForm)
		{
			LexEntry entry = (LexEntry)entries.AddNew();
			entry.LexicalForm.SetAlternative(writingSystemId, lexicalForm);
			return entry;
		}

		[Test]
		public void FindsNoExactMatch()
		{
			MakeTestLexEntry(_entries, "v", "foo1");
			string[] ids;
			string[] forms;
			_dictionaryServiceProvider.GetMatchingEntries("v", "foo", FindMethods.Exact, out ids,out forms);
			Assert.AreEqual(0, ids.Length);
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

			MakeTestLexEntry(_entries, "v", "foo1");
			LexEntry foo2 = MakeTestLexEntry(_entries, "v2", "foo2");
			LexEntry foo3 = MakeTestLexEntry(_entries, "v", "foo3");

			string[] ids;
			string[] forms;
			_dictionaryServiceProvider.GetMatchingEntries("v", "foo", FindMethods.DefaultApproximate, out ids, out forms);
			Assert.AreEqual(2, ids.Length);
		}



		[Test]
		public void CreateNewEntryWithExampleButNoDef()
		{
			MakeTestLexEntry(_entries, "v", "foo1");
		   string id = _dictionaryServiceProvider.AddEntry("v", "voom", null, null, "v", "vlah voom!");
		   Assert.IsNotNull(id);
		   string[] ids;
		   string[] forms;
		   _dictionaryServiceProvider.GetMatchingEntries("v", "voom", FindMethods.Exact, out ids, out forms);
		   Assert.AreEqual(id, ids[0]);
		   string html = _dictionaryServiceProvider.GetHmtlForEntry(id);
		   Assert.IsTrue(html.Contains("vlah voom!"));
		}


		[Test]
		public void CreateNewEntryWithAllFieldsDoesCreateIt()
		{
		   string id = _dictionaryServiceProvider.AddEntry("v", "voom", "en", "def of voom", "v", "vlah voom!");
		   Assert.IsNotNull(id);
		   string[] ids;
		   string[] forms;
		   _dictionaryServiceProvider.GetMatchingEntries("v", "voom", FindMethods.Exact, out ids, out forms);
		   Assert.AreEqual(id, ids[0]);
		   string html = _dictionaryServiceProvider.GetHmtlForEntry(id);
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
		   string[] ids;
		   string[] forms;
		   _dictionaryServiceProvider.GetMatchingEntries("v", "voom", FindMethods.Exact, out ids, out forms);
		   Assert.AreEqual(id, ids[0]);
		}

	}
}
