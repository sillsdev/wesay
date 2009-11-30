using System;
using System.Drawing;
using NUnit.Framework;
using Palaso.Data;
using Palaso.DictionaryServices.Model;
using Palaso.TestUtilities;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepository_GetMissingItemTests
	{
		private TemporaryFolder _temporaryFolder;
		private LexEntryRepository _lexEntryRepository;
		private readonly WritingSystem _lexicalFormWritingSystem =  new WritingSystem("de", SystemFonts.DefaultFont);

		[SetUp]
		public void Setup()
		{
			_temporaryFolder = new TemporaryFolder();
			string filePath = _temporaryFolder.GetTemporaryFile();
			_lexEntryRepository = new LexEntryRepository(filePath);
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			_temporaryFolder.Delete();
		}

		private void CreateLexentryWithOnlyCitationForm(string citationForm, string writingSystemId)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.CitationForm[writingSystemId] = citationForm;
			_lexEntryRepository.SaveItem(entry);
		}

		private void CreateLexentryWithLexicalFormButWithoutCitation(string lexicalForm, string writingSystemId)
		{
			LexEntry lexEntryWithMissingCitation = _lexEntryRepository.CreateItem();
			lexEntryWithMissingCitation.LexicalForm[writingSystemId] = lexicalForm;
			_lexEntryRepository.SaveItem(lexEntryWithMissingCitation);
		}

		private LexEntry CreateEntryWithDefinition(params string[] definitionWritingSystems)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(_lexicalFormWritingSystem.Id, "theForm");

			entry.Senses.Add(new LexSense());
			foreach (var id in definitionWritingSystems)
			{
				entry.Senses[0].Definition.SetAlternative(id, "the definition for "+id);
			}

			return entry;
		}
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Get_FieldNull_Throws()
		{
			Field fieldToFill = null;
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem);
			Assert.AreEqual(0, sortedResults.Count);
		}

		[Test]
		public void Get_FieldNameDoesNotExist_ReturnsEmpty()
		{
			Field fieldToFill = new Field("I do not exist!", "LexEntry", new string[] { "fr" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem);
			Assert.AreEqual(0, sortedResults.Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Get_WritingSystemNull_Throws()
		{
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "fr" });
			WritingSystem lexicalFormWritingSystem = null;
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem);
		}

		[Test]
		public void Get_MultipleEntriesWithMissingFieldExist_ReturnsEntriesSortedByLexicalForm()
		{
			CreateLexentryWithLexicalFormButWithoutCitation("de Word2", "de");
			CreateLexentryWithLexicalFormButWithoutCitation("de Word1", "de");
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem);
			Assert.AreEqual(2, sortedResults.Count);
			Assert.AreEqual("de Word1", sortedResults[0]["Form"]);
			Assert.AreEqual("de Word2", sortedResults[1]["Form"]);
		}


		[Test]
		public void Get_LexicalFormDoesNotExistInWritingSystem_ReturnsNullForThatEntry()
		{
			LexEntry lexEntryWithMissingCitation = _lexEntryRepository.CreateItem();
			_lexEntryRepository.SaveItem(lexEntryWithMissingCitation);
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem);
			Assert.AreEqual(1, sortedResults.Count);
			Assert.AreEqual(null, sortedResults[0]["Form"]);
		}

		[Test]
		public void Get_FieldExists_DoesNotReturnResultForThatLexEntry()
		{
			CreateLexentryWithLexicalFormButWithoutCitation("de Word1", "de");
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.LexicalUnit, "LexEntry", new string[] { "de" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem);
			Assert.AreEqual(0, sortedResults.Count);
		}

		[Test]
		public void Get_EntryInWritingSystemInFieldDoesNotExist_ReturnsEntries()
		{
			CreateLexentryWithOnlyCitationForm("de Word2", "de");
			CreateLexentryWithOnlyCitationForm("de Word1", "de");
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "fr" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem);
			Assert.AreEqual(2, sortedResults.Count);
			Assert.AreEqual(null, sortedResults[0]["Form"]);
			Assert.AreEqual(null, sortedResults[1]["Form"]);
		}

		private void TestWritingSystemSearch(int expectedCount, string[] fieldWritingSystems, string[] fillInWritingSystems, string[] searchInWritingSystems)
		{
			CreateEntryWithDefinition(fillInWritingSystems);
			Field fieldToFill = new Field(LexSense.WellKnownProperties.Definition, "LexSense", fieldWritingSystems);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, searchInWritingSystems, _lexicalFormWritingSystem);
			Assert.AreEqual(expectedCount, sortedResults.Count);
		}

		[Test]
		public void Get_NoSearchWs_MissingInFirstOne_Returned()
		{
			TestWritingSystemSearch(1, new[] { "de", "en" },  //field spec
								   new[] { "en" },           //fill these in
								   new string[] { });          // search on these
		}

		[Test]
		public void Get_NoSearchWs_MissingInSecondOne_Returned()
		{
			TestWritingSystemSearch(1, new[] { "de", "en" },  //field spec
								   new[] { "de" },           //fill these in
								   new string[]{});          // search on these
		}

		[Test]
		public void Get_NullSearchWs_MissingInOne_Returned()
		{
			TestWritingSystemSearch(1, new[] { "de", "en" },  //field spec
								   new[] { "en"},           //fill these in
								   null);                   // search on these
		}

		/// <summary>
		/// This is the real core test.  If the record has German, and that's all
		/// we want to search for, this record should not be returned even though
		/// it is missing English.
		/// </summary>
		[Test]
		public void Get_1SearchWs_NotMissingData_NotReturned()
		{
			TestWritingSystemSearch(0, new[] {"de", "en" },  //field spec
								   new[] {"de"},             //fill these in
								   new[] {"de"});            // search on these
		}


		[Test]
		public void Get_1SearchWs_MissingData_Returned()
		{
			TestWritingSystemSearch(1, new[] { "de", "en" },  //field spec
								   new[] { "en" },           //fill these in
								   new[] { "de" });          // search on these
		}

		[Test]
		public void Get_2SearchWs_NoMissingData_NotReturned()
		{
			TestWritingSystemSearch(0, new[] { "de", "en" },  //field spec
								   new[] { "en", "de" },           //fill these in
								   new[] { "de", "en" });          // search on these
		}

		[Test]
		public void Get_2SearchWs_MissingDataInOne_Returned()
		{
			TestWritingSystemSearch(1, new[] { "de", "en" },  //field spec
								   new[] { "en" },           //fill these in
								   new[] { "de", "en" });          // search on these
		}
	}
}