using System.Collections.Generic;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.Project.Tests;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexiconTests
	{
		private Db4oProjectSetupForTesting _projectSetupSharedByAllTests;
		private IRecordList<LexEntry> _entries;

		/// <summary>
		/// Db4oProjectSetupForTesting is extremely time consuming to setup, so we reuse it.
		/// </summary>
	   [TestFixtureSetUp]
		public void SetupFixture()
		{
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = false;
			_projectSetupSharedByAllTests = new Db4oProjectSetupForTesting(string.Empty);
			_entries = _projectSetupSharedByAllTests._recordListManager.GetListOfType<LexEntry>();
		}

		[SetUp]
		public void Setup()
		{
			_entries.Clear();//let each test have a clean slate
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			_projectSetupSharedByAllTests.Dispose();
		}

		private static LexEntry MakeTestLexEntry(IRecordList<LexEntry> entries, string writingSystemId, string lexicalForm)
		{
			LexEntry entry = (LexEntry) entries.AddNew();
			entry.LexicalForm.SetAlternative(writingSystemId,lexicalForm);
			return entry;
		}

		[Test]
		public void GetEntriesByApproximateLexicalFormShouldNotContainMatchesFromOtherWritingSystems()
		{
			MakeTestLexEntry(_entries, "v","foo1");
			MakeTestLexEntry(_entries, "en","foo2");
			LexEntry foo3 = MakeTestLexEntry(_entries, "v","foo3");
			WritingSystem ws;
			_projectSetupSharedByAllTests._project.WritingSystems.TryGetValue("v", out ws);

			IList<LexEntry> matches= Lexicon.GetEntriesWithSimilarLexicalForms("foo", ws, ApproximateMatcherOptions.IncludePrefixedForms, int.MaxValue);
			Assert.AreEqual(2, matches.Count);
			Assert.AreEqual(foo3, matches[1]);
		}


	}
}
