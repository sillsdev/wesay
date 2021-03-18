using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using SIL.Data;
using SIL.DictionaryServices.Model;
using SIL.TestUtilities;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepository_GetMissingItemTests
	{
		private TemporaryFolder _temporaryFolder;
		private LexEntryRepository _lexEntryRepository;
		private WritingSystemDefinition _lexicalFormWritingSystem;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Sldr.Initialize(true);
		}

		[OneTimeTearDown]
		public void OneTimeTeardown()
		{
			Sldr.Cleanup();
		}

		[SetUp]
		public void Setup()
		{
			_lexicalFormWritingSystem = new WritingSystemDefinition("de")
			{
				DefaultCollation = new IcuRulesCollationDefinition("standard")
			};
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

		private LexEntry CreateEntryWithDefinition(IEnumerable<string> populatedWritingSystems, IEnumerable<string> emptyWritingSystems)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(_lexicalFormWritingSystem.LanguageTag, "theForm");

			entry.Senses.Add(new LexSense());
			foreach (var id in populatedWritingSystems)
			{
				entry.Senses[0].Definition.SetAlternative(id, "the definition for "+ id);
			}
			foreach (var id in emptyWritingSystems)
			{
				entry.Senses[0].Definition.SetAlternative(id, "");
			}

			return entry;
		}

		[Test]
		public void Get_FieldNull_Throws()
		{
			Field fieldToFill = null;
			WritingSystemDefinition lexicalFormWritingSystem = new WritingSystemDefinition("de");
			Assert.Throws<ArgumentNullException>(() =>
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem));
		}

		[Test]
		public void Get_FieldNameDoesNotExist_ReturnsEmpty()
		{
			Field fieldToFill = new Field("I do not exist!", "LexEntry", new string[] { "fr" });
			WritingSystemDefinition lexicalFormWritingSystem = new WritingSystemDefinition("de");
			lexicalFormWritingSystem.DefaultCollation = new IcuRulesCollationDefinition("standard");
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem);
			Assert.AreEqual(0, sortedResults.Count);
		}

		[Test]
		public void Get_WritingSystemNull_Throws()
		{
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "fr" });
			WritingSystemDefinition lexicalFormWritingSystem = null;
			Assert.Throws<ArgumentNullException>(() =>
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem));
		}

		[Test]
		public void Get_MultipleEntriesWithMissingFieldExist_ReturnsEntriesSortedByLexicalForm()
		{
			CreateLexentryWithLexicalFormButWithoutCitation("de Word2", "de");
			CreateLexentryWithLexicalFormButWithoutCitation("de Word1", "de");
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });
			WritingSystemDefinition lexicalFormWritingSystem = new WritingSystemDefinition("de");
			lexicalFormWritingSystem.DefaultCollation = new IcuRulesCollationDefinition("standard");
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
			WritingSystemDefinition lexicalFormWritingSystem = new WritingSystemDefinition("fr");
			lexicalFormWritingSystem.DefaultCollation = new IcuRulesCollationDefinition("standard");
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
			WritingSystemDefinition lexicalFormWritingSystem = new WritingSystemDefinition("de");
			lexicalFormWritingSystem.DefaultCollation = new IcuRulesCollationDefinition("standard");
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
			WritingSystemDefinition lexicalFormWritingSystem = new WritingSystemDefinition("de");
			lexicalFormWritingSystem.DefaultCollation = new IcuRulesCollationDefinition("standard");
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, null, lexicalFormWritingSystem);
			Assert.AreEqual(2, sortedResults.Count);
			Assert.AreEqual(null, sortedResults[0]["Form"]);
			Assert.AreEqual(null, sortedResults[1]["Form"]);
		}

		private void TestWritingSystemSearch(int expectedCount, string[] fieldWritingSystems, string[] populatedWritingSystems, string[] searchInWritingSystems)
		{
			CreateEntryWithDefinition(populatedWritingSystems, new string[]{});
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

		private class TestEnvironment:IDisposable
		{
			private LexEntryRepository _repository;
			private TemporaryFolder _temporaryFolder;
			private readonly WritingSystemDefinition _vernacularWritingSystem = new WritingSystemDefinition("de")
			{
				DefaultCollation = new IcuRulesCollationDefinition("standard")
			};

			public TestEnvironment()
			{
				_temporaryFolder = new TemporaryFolder();
				string filePath = _temporaryFolder.GetTemporaryFile();
				_repository = new LexEntryRepository(filePath);
			}

			public void TestFilter(int result, IEnumerable<string> emptyWsInField, IEnumerable<string> populatedWsInField,
							  IEnumerable<string> wsThatMustBeEmpty, IEnumerable<string> wsThatMustBePopulated)
			{
				var allFieldWs = emptyWsInField.Concat(populatedWsInField).Concat(wsThatMustBeEmpty).Concat(wsThatMustBePopulated);
				CreateEntryWithDefinitionAndWs(populatedWsInField, emptyWsInField);
				Field fieldToFill = new Field(LexSense.WellKnownProperties.Definition, "LexSense", allFieldWs);
				var missingFieldFilter = new MissingFieldQuery(fieldToFill, wsThatMustBeEmpty.ToArray(), wsThatMustBePopulated.ToArray());
				ResultSet<LexEntry> sortedResults =
					_repository.GetEntriesWithMissingFieldSortedByLexicalUnit(missingFieldFilter, _vernacularWritingSystem);
				Assert.AreEqual(result, sortedResults.Count);
			}


			private LexEntry CreateEntryWithDefinitionAndWs(IEnumerable<string> populatedWritingSystems, IEnumerable<string> emptyWritingSystems)
			{
				LexEntry entry = _repository.CreateItem();
				entry.LexicalForm.SetAlternative(_vernacularWritingSystem.LanguageTag, "theForm");

				entry.Senses.Add(new LexSense());
				foreach (var id in populatedWritingSystems)
				{
					entry.Senses[0].Definition.SetAlternative(id, "the definition for " + id);
				}
				foreach (var id in emptyWritingSystems)
				{
					entry.Senses[0].Definition.SetAlternative(id, "");
				}

				return entry;
			}

			public void Dispose()
			{
				_repository.Dispose();
				_temporaryFolder.Delete();
			}
		}

		//At the times that these tests were written (1/17/13) the UI did not allow filtering on writing systems not specified in the Field, so we are not going to test those cases at this time
		#region 00
		[Test]
		public void Get_Require0EmptyAnd0PopulatedWs_2Empty_Returns1()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(1, new string[] {"en", "de"}, new string[] {}, new string[] {}, new string[] {});
			}
		}

		[Test]
		public void Get_Require0EmptyAnd0PopulatedWs_1Empty1Populated_Returns1()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(1, new string[] { "en" }, new string[] { "de" }, new string[] { }, new string[] { });
			}
		}

		[Test]
		public void Get_Require0EmptyAnd0PopulatedWs_2Populated_Returns0()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { }, new string[] {"en", "de" }, new string[] { }, new string[] { });
			}
		}
		#endregion
		#region 10
		[Test]
		public void Get_Require1EmptyAnd0PopulatedWs_2Empty_Returns1()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(1, new string[] { "en", "de" }, new string[] { }, new [] { "en" }, new string[] { });
			}
		}

		[Test]
		public void Get_Require1EmptyAnd0PopulatedWs_1Empty1PopulatedEmptyIsNotTheOneWeWant_Returns1()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { "de" }, new string[] { "en" }, new [] { "en" }, new string[] { });
			}
		}

		[Test]
		public void Get_Require1EmptyAnd0PopulatedWs_1Empty1PopulatedEmptyIsTheOneWeWant_Returns1()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(1, new string[] { "en" }, new string[] { "de" }, new [] { "en" }, new string[] { });
			}
		}

		[Test]
		public void Get_Require1EmptyAnd0PopulatedWs_2Populated_Returns0()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { }, new string[] { "en", "de" }, new [] { "en" }, new string[] { });
			}
		}
		#endregion

		#region 01
		[Test]
		public void Get_Require0EmptyAnd1PopulatedWs_2Empty_Returns0()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { "en", "de" }, new string[] { }, new string[] { }, new [] { "de" });
			}
		}

		[Test]
		public void Get_Require0EmptyAnd1PopulatedWs_1Empty1PopulatedPopulatedIsNotTheOneWeWant_Returns0()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { "de" }, new string[] { "en" }, new string[] { }, new[] { "de" });
			}
		}

		[Test]
		public void Get_Require0EmptyAnd1PopulatedWs_1Empty1PopulatedpopulatedIsTheOneWeWant_Returns1()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(1, new string[] { "en" }, new string[] { "de" }, new string[] { }, new[] { "de" });
			}
		}

		[Test]
		public void Get_Require0EmptyAnd1PopulatedWs_2Populated_Returns0()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { }, new string[] { "en", "de" }, new string[] { }, new[] { "de" });
			}
		}

		[Test]
		public void Get_Require0EmptyAnd1PopulatedWs_2PopulatedOtherEmpty_Returns1()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(1, new string[] { "it" }, new string[] { "en", "de" }, new string[] { }, new[] { "de" });
			}
		}
		#endregion


		#region 11
		[Test]
		public void Get_Require1EmptyAnd1PopulatedWs_2Empty_Returns0()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { "en", "de" }, new string[] { }, new [] { "en" }, new [] { "de" });
			}
		}

		[Test]
		public void Get_Require1EmptyAnd1PopulatedWs_2Empty1PopulatedPopulatedIsNotTheOneWeWant_Returns0()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { "de", "en" }, new string[] { "it" }, new[] { "en" }, new[] { "de" });
			}
		}

		[Test]
		public void Get_Require1EmptyAnd1PopulatedWs_1Empty2PopulatedEmptyIsNotTheOneWeWant_Returns0()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { "it" }, new string[] { "de", "en" }, new[] { "en" }, new[] { "de" });
			}
		}

		[Test]
		public void Get_Require1EmptyAnd1PopulatedWs_1Empty1PopulatedAreTheOnesWeWant_Returns1()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(1, new string[] {"en" }, new string[] { "de" }, new[] { "en" }, new[] { "de" });
			}
		}

		[Test]
		public void Get_Require1EmptyAnd1PopulatedWs_2Populated_Returns0()
		{
			using (var e = new TestEnvironment())
			{
				e.TestFilter(0, new string[] { }, new string[] { }, new[] { "en" }, new[] { "de" });
			}
		}
		#endregion
	}
}