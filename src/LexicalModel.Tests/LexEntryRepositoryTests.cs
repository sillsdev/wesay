using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using LiftIO.Parsing;
using NUnit.Framework;
using Palaso.Text;
using WeSay.Data;
using WeSay.Data.Tests;
using WeSay.Foundation;
using WeSay.LexicalModel.Db4oSpecific;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryTests
	{
		private string _filePath;
		private LexEntryRepository _lexEntryRepository;
		private WritingSystem _headwordWritingSystem;

		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();
			_lexEntryRepository = new LexEntryRepository(_filePath);
			_headwordWritingSystem = new WritingSystem();
			_headwordWritingSystem.Id = "primary";
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			File.Delete(_filePath);
		}

		private void MakeTestLexEntry(string writingSystemId, string lexicalForm)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(writingSystemId, lexicalForm);
			_lexEntryRepository.SaveItem(entry);
			return;
		}

		private delegate MultiText GetMultiTextFromLexEntryDelegate(LexEntry entry);

		private void CreateThreeDifferentLexEntriesToBeSorted(GetMultiTextFromLexEntryDelegate getMultiTextFromLexEntryDelegate)
		{
			LexEntry[] lexEntriesToSort = new LexEntry[3];
			MultiText[] propertyOfLexentry = new MultiText[3];

			lexEntriesToSort[0] = _lexEntryRepository.CreateItem();
			propertyOfLexentry[0] = getMultiTextFromLexEntryDelegate(lexEntriesToSort[0]);
			propertyOfLexentry[0].SetAlternative("de", "de Word2");

			lexEntriesToSort[1] = _lexEntryRepository.CreateItem();
			propertyOfLexentry[1] = getMultiTextFromLexEntryDelegate(lexEntriesToSort[1]);
			propertyOfLexentry[1].SetAlternative("de", "de Word3");

			lexEntriesToSort[2] = _lexEntryRepository.CreateItem();
			propertyOfLexentry[2] = getMultiTextFromLexEntryDelegate(lexEntriesToSort[2]);
			propertyOfLexentry[2].SetAlternative("de", "de Word1");

			_lexEntryRepository.SaveItem(lexEntriesToSort[0]);
			_lexEntryRepository.SaveItem(lexEntriesToSort[1]);
			_lexEntryRepository.SaveItem(lexEntriesToSort[2]);
		}

		[Test]
		public void GetAllEntriesSortedByHeadword_RepositoryIsEmpty_ReturnsEmptyList()
		{
			Assert.AreEqual(0, _lexEntryRepository.GetAllEntriesSortedByHeadword(new WritingSystem()).Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetAllEntriesSortedByHeadword_Null_Throws()
		{
			_lexEntryRepository.GetAllEntriesSortedByHeadword(null);
		}

		[Test]
		public void GetAllEntriesSortedByHeadword_CitationFormExistsInWritingSystemForAllEntries_ReturnsListSortedByCitationForm()
		{
			CreateThreeDifferentLexEntriesToBeSorted(delegate(LexEntry e) { return e.CitationForm; });
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByHeadWord = _lexEntryRepository.GetAllEntriesSortedByHeadword(german);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByHeadWord[0]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByHeadWord[1]["Form"]);
			Assert.AreEqual("de Word3", listOfLexEntriesSortedByHeadWord[2]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByHeadword_CitationAndLexicalFormInWritingSystemDoNotExist_ReturnsEmptyForThatEntry()
		{
			LexEntry lexEntryWithOutFrenchHeadWord = _lexEntryRepository.CreateItem();
			lexEntryWithOutFrenchHeadWord.CitationForm.SetAlternative("de", "de Word1");
			lexEntryWithOutFrenchHeadWord.LexicalForm.SetAlternative("de", "de Word1");
			WritingSystem french = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByHeadWord = _lexEntryRepository.GetAllEntriesSortedByHeadword(french);
			Assert.AreEqual("", listOfLexEntriesSortedByHeadWord[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByHeadword_CitationFormInWritingSystemDoesNotExistButLexicalFormDoes_SortsByLexicalFormForThatEntry()
		{
			CreateThreeDifferentLexEntriesToBeSorted(delegate (LexEntry e) {return e.CitationForm;});
			LexEntry lexEntryWithOutGermanCitationForm = _lexEntryRepository.CreateItem();
			lexEntryWithOutGermanCitationForm.CitationForm.SetAlternative("fr", "fr Word4");
			lexEntryWithOutGermanCitationForm.LexicalForm.SetAlternative("de", "de Word0");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByHeadWord = _lexEntryRepository.GetAllEntriesSortedByHeadword(german);
			Assert.AreEqual("de Word0", listOfLexEntriesSortedByHeadWord[0]["Form"]);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByHeadWord[1]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByHeadWord[2]["Form"]);
			Assert.AreEqual("de Word3", listOfLexEntriesSortedByHeadWord[3]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByHeadword_CitationFormAndLexicalFormAreIdenticalInAnEntry_EntryOnlyApearsOnce()
		{
			LexEntry lexEntryWithIdenticalCitationandLexicalForm = _lexEntryRepository.CreateItem();
			lexEntryWithIdenticalCitationandLexicalForm.CitationForm.SetAlternative("de", "de Word1");
			lexEntryWithIdenticalCitationandLexicalForm.LexicalForm.SetAlternative("de", "de Word1");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByHeadWord = _lexEntryRepository.GetAllEntriesSortedByHeadword(german);
			Assert.AreEqual(1, listOfLexEntriesSortedByHeadWord.Count);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalForm_RepositoryIsEmpty_ReturnsEmptyList()
		{
			Assert.AreEqual(0, _lexEntryRepository.GetAllEntriesSortedByLexicalForm(new WritingSystem()).Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetAllEntriesSortedByLexicalForm_Null_Throws()
		{
			_lexEntryRepository.GetAllEntriesSortedByLexicalForm(null);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalForm_LexicalFormExistsInWritingSystemForAllEntries_ReturnsListSortedByLexicalForm()
		{
			CreateThreeDifferentLexEntriesToBeSorted(delegate(LexEntry e) { return e.LexicalForm; });
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByLexicalForm = _lexEntryRepository.GetAllEntriesSortedByLexicalForm(german);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByLexicalForm[0]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByLexicalForm[1]["Form"]);
			Assert.AreEqual("de Word3", listOfLexEntriesSortedByLexicalForm[2]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalForm_LexicalFormInWritingSystemDoesNotExist_ReturnsEmptyForThatEntry()
		{
			LexEntry lexEntryWithOutFrenchLexicalForm = _lexEntryRepository.CreateItem();
			lexEntryWithOutFrenchLexicalForm.LexicalForm.SetAlternative("de", "de Word1");
			WritingSystem french = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByLexicalForm = _lexEntryRepository.GetAllEntriesSortedByLexicalForm(french);
			Assert.AreEqual("", listOfLexEntriesSortedByLexicalForm[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_RepositoryIsEmpty_ReturnsEmptyList()
		{
			Assert.AreEqual(0, _lexEntryRepository.GetAllEntriesSortedByDefinition(new WritingSystem()).Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetAllEntriesSortedByDefinition_Null_Throws()
		{
			_lexEntryRepository.GetAllEntriesSortedByDefinition(null);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionExistsInWritingSystemForAllEntries_ReturnsListSortedByDefinition()
		{
			CreateThreeDifferentLexEntriesToBeSorted(delegate(LexEntry e)
														 {
															 e.Senses.Add(new LexSense());
															 return e.Senses[0].Definition;
														 });
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinition(german);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByDefinition[1]["Form"]);
			Assert.AreEqual("de Word3", listOfLexEntriesSortedByDefinition[2]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_GlossExistsInWritingSystemForAllEntries_ReturnsListSortedByGloss()
		{
			CreateThreeDifferentLexEntriesToBeSorted(delegate(LexEntry e)
														 {
															 e.Senses.Add(new LexSense());
															 return e.Senses[0].Gloss;
														 });
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByGloss = _lexEntryRepository.GetAllEntriesSortedByDefinition(german);
			Assert.AreEqual(3, listOfLexEntriesSortedByGloss.Count);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByGloss[0]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByGloss[1]["Form"]);
			Assert.AreEqual("de Word3", listOfLexEntriesSortedByGloss[2]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionAndGlossExistInWritingSystem_ReturnsSortedListWithBothDefinitionAndGloss()
		{
			LexEntry lexEntryWithBothDefinitionAndAGloss = _lexEntryRepository.CreateItem();
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Definition.SetAlternative("de", "de Word2");
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Gloss.SetAlternative("de", "de Word1");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinition(german);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.Fail("NotTesting what it says it is.");
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionAndGlossInWritingSystemDoNotExist_ReturnsEmptyForThatEntry()
		{
			LexEntry lexEntryWithOutFrenchGloss = _lexEntryRepository.CreateItem();
			lexEntryWithOutFrenchGloss.Senses.Add(new LexSense());
			lexEntryWithOutFrenchGloss.Senses[0].Definition.SetAlternative("de", "de Word1");
			WritingSystem french = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinition(french);
			Assert.AreEqual(1 , listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual("", listOfLexEntriesSortedByDefinition[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionAndGlossOfOneEntryAreIdentical_ReturnsOnlyOneRecordToken()
		{
			LexEntry lexEntryWithBothDefinitionAndAGloss = _lexEntryRepository.CreateItem();
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Definition.SetAlternative("de", "de Word1");
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Gloss.SetAlternative("de", "de Word1");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinition(german);
			Assert.AreEqual(1, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionOfTwoEntriesAreIdentical_ReturnsBothRecordToken()
		{
			LexEntry lexEntryWithBothDefinition = _lexEntryRepository.CreateItem();
			lexEntryWithBothDefinition.Senses.Add(new LexSense());
			lexEntryWithBothDefinition.Senses[0].Definition.SetAlternative("de", "de Word1");
			LexEntry lexEntryTwoWithBothDefinition = _lexEntryRepository.CreateItem();
			lexEntryTwoWithBothDefinition.Senses.Add(new LexSense());
			lexEntryTwoWithBothDefinition.Senses[0].Definition.SetAlternative("de", "de Word1");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinition(german);
			Assert.AreEqual(2, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[1]["Form"]);
		}

		[Test]
		public void GetEntriesWithSimilarLexicalForm_RepositoryContainsEntriesWithDifferingWritingSystems_OnlyFindEntriesMatchingTheGivenWritingSystem()
		{
			//CreateThreeLexEntriesWithDifferentLexicalForms();
			WritingSystem ws = new WritingSystem("fr", SystemFonts.DefaultFont);

			ResultSet<LexEntry> matches =
					_lexEntryRepository.GetEntriesWithSimilarLexicalForm("fr Wor",
																		 ws,
																		 ApproximateMatcherOptions.
																				 IncludePrefixedForms);
			Assert.AreEqual(1, matches.Count);
			Assert.AreEqual("fr Word", matches[0]["Form"]);
		}

		[Test]
		public void GetEntriesWithMatchingLexicalForm_RepositoryContainsTwoEntriesWithDifferingLexicalForms_OnlyEntryWithmatchingLexicalFormIsFound()
		{
			LexEntry entryToFind = _lexEntryRepository.CreateItem();
			entryToFind.LexicalForm["en"] = "find me";
			_lexEntryRepository.SaveItem(entryToFind);
			//don't want to find this one
			LexEntry entryToIgnore = _lexEntryRepository.CreateItem();
			entryToIgnore.LexicalForm["en"] = "don't find me";
			_lexEntryRepository.SaveItem(entryToIgnore);
			WritingSystem writingSystem = new WritingSystem("en", SystemFonts.DefaultFont);
			ResultSet<LexEntry> list =
					_lexEntryRepository.GetEntriesWithMatchingLexicalForm("find me", writingSystem);
			Assert.AreEqual(1, list.Count);
		}

		[Test]
		public void GetLexEntryWithMatchingGuid_FindEntryFromGuid()
		{
			Guid g = SetupEntryWithGuid();

			LexEntry found = _lexEntryRepository.GetLexEntryWithMatchingGuid(g);
			Assert.AreEqual("hello", found.LexicalForm["en"]);
		}

		private Guid SetupEntryWithGuid()
		{
			AddEntryWithGloss("hello");

			Guid g = Guid.NewGuid();
			CreateEntryWithGuid(g);

			AddEntryWithGloss("world");
			return g;
		}

		private void CreateEntryWithGuid(Guid g)
		{
			Extensible extensible = new Extensible();
			extensible.Guid = g;
			LexEntry entry = _lexEntryRepository.CreateItem(extensible);
			entry.LexicalForm["en"] = "hello";
			_lexEntryRepository.SaveItem(entry);
		}

		[Test]
		public void GetLexEntryWithMatchingGuid_GuidDoesNotExist_ReturnsNull()
		{
			SetupEntryWithGuid();
			LexEntry found = _lexEntryRepository.GetLexEntryWithMatchingGuid(Guid.NewGuid());
			Assert.IsNull(found);
		}

		[Test]
		[ExpectedException(typeof (ApplicationException))]
		public void GetLexEntryWithMatchingGuid_MultipleGuidMatchesInRepo_Throws()
		{
			Guid g = SetupEntryWithGuid();
			CreateEntryWithGuid(g);
			_lexEntryRepository.GetLexEntryWithMatchingGuid(g);
		}

		[Test]
		public void
			GetEntriesWithMatchingGlossSortedByLexicalForm_RepositoryContainsTwoEntriesWithDifferingLexicalForms_OnlyEntryWithmatchingLexicalFormIsFound()
		{
			string glossToFind = "Gloss To Find.";
			AddEntryWithGloss(glossToFind);
			AddEntryWithGloss("Gloss Not To Find.");
			LanguageForm glossLanguageForm = new LanguageForm("en", glossToFind, new MultiText());
			WritingSystem writingSystem = new WritingSystem("en", SystemFonts.DefaultFont);
			ResultSet<LexEntry> list =
					_lexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(
							glossLanguageForm, writingSystem);
			Assert.AreEqual(1, list.Count);
		}

		private void AddEntryWithGloss(string gloss)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			sense.Gloss["en"] = gloss;
			_lexEntryRepository.SaveItem(entry);
		}

		[Test]
		public void GetHomographNumber_OnlyOneEntry_Returns0()
		{
			LexEntry entry1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Assert.AreEqual(0,
							_lexEntryRepository.GetHomographNumber(entry1, _headwordWritingSystem));
		}

		[Test, Ignore("Homograph order is not well defined CJP")]
		public void GetHomographNumber_FirstEntryWithFollowingHomograph_Returns1()
		{
			LexEntry entry1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Assert.AreEqual(1,
							_lexEntryRepository.GetHomographNumber(entry1, _headwordWritingSystem));
		}

		[Test, Ignore("Homograph order is not well defined CJP")]
		public void GetHomographNumber_SecondEntry_Returns2()
		{
			MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			LexEntry entry2 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Assert.AreEqual(2,
							_lexEntryRepository.GetHomographNumber(entry2, _headwordWritingSystem));
		}

		[Test]
		public void GetHomographNumber_AssignesUniqueNumbers()
		{
			LexEntry entryOther = MakeEntryWithLexemeForm("en", "blue");
			Assert.AreNotEqual("en", _headwordWritingSystem.Id);
			LexEntry[] entries = new LexEntry[3];
			entries[0] = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			entries[1]= MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			entries[2] = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			List<int> ids = new List<int>(entries.Length);
			foreach (LexEntry entry in entries)
			{
				int homographNumber = _lexEntryRepository.GetHomographNumber(entry, _headwordWritingSystem);
				Assert.IsFalse(ids.Contains(homographNumber));
				ids.Add(homographNumber);
			}
		}

		[Test, Ignore("Homograph order is not well defined CJP")]
		public void GetHomographNumber_ThirdEntry_Returns3()
		{
			LexEntry entryOther = MakeEntryWithLexemeForm("en", "blue");
			Assert.AreNotEqual("en", _headwordWritingSystem.Id);
			LexEntry entry1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			LexEntry entry2 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			LexEntry entry3 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Console.WriteLine("ID0: {0}", _lexEntryRepository.GetId(entryOther));
			Console.WriteLine("ID1: {0}", _lexEntryRepository.GetId(entry1));
			Console.WriteLine("ID2: {0}", _lexEntryRepository.GetId(entry2));
			Console.WriteLine("ID3: {0}", _lexEntryRepository.GetId(entry3));
			Assert.AreEqual(3, _lexEntryRepository.GetHomographNumber(entry3, _headwordWritingSystem));
			Assert.AreEqual(2, _lexEntryRepository.GetHomographNumber(entry2, _headwordWritingSystem));
			Assert.AreEqual(1, _lexEntryRepository.GetHomographNumber(entry1, _headwordWritingSystem));
		}

		[Test, Ignore("Homograph order is not well defined CJP")]
		public void GetHomographNumber_3SameLexicalForms_Returns123()
		{
			LexEntry entry1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			LexEntry entry2 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			LexEntry entry3 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Assert.AreEqual(1,
							_lexEntryRepository.GetHomographNumber(entry1, _headwordWritingSystem));
			Assert.AreEqual(3,
							_lexEntryRepository.GetHomographNumber(entry3, _headwordWritingSystem));
			Assert.AreEqual(2,
							_lexEntryRepository.GetHomographNumber(entry2, _headwordWritingSystem));
		}

		[Test, Ignore("Homograph order is not well defined CJP")]
		public void GetHomographNumber_3SameLexicalFormsAnd3OtherLexicalForms_Returns123()
		{
			LexEntry red1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "red");
			//Thread.Sleep(1100);
			LexEntry blue1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			//Thread.Sleep(1100);
			LexEntry red2 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "red");
			//Thread.Sleep(1100);
			LexEntry blue2 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			//Thread.Sleep(1100);
			LexEntry red3 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "red");
			//Thread.Sleep(1100);
			LexEntry blue3 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Assert.AreEqual(1,
							_lexEntryRepository.GetHomographNumber(blue1, _headwordWritingSystem));
			Assert.AreEqual(3,
							_lexEntryRepository.GetHomographNumber(blue3, _headwordWritingSystem));
			Assert.AreEqual(2,
							_lexEntryRepository.GetHomographNumber(blue2, _headwordWritingSystem));
			Assert.AreEqual(1,
							_lexEntryRepository.GetHomographNumber(red1, _headwordWritingSystem));
			Assert.AreEqual(3,
							_lexEntryRepository.GetHomographNumber(red3, _headwordWritingSystem));
			Assert.AreEqual(2,
							_lexEntryRepository.GetHomographNumber(red2, _headwordWritingSystem));
		}

		[Test]
		[Ignore("not implemented")]
		public void GetHomographNumber_HonorsOrderAttribute() {}

		private LexEntry MakeEntryWithLexemeForm(string writingSystemId, string lexicalUnit)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(writingSystemId, lexicalUnit);
			_lexEntryRepository.SaveItem(entry);
			return entry;
		}

		[Test]
		public void GetAllEntriesSortedByHeadword_3EntriesWithLexemeForms_TokensAreSorted()
		{
			LexEntry e1 = _lexEntryRepository.CreateItem();
			e1.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "bank");
			_lexEntryRepository.SaveItem(e1);
			RepositoryId bankId = _lexEntryRepository.GetId(e1);

			LexEntry e2 = _lexEntryRepository.CreateItem();
			e2.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "apple");
			_lexEntryRepository.SaveItem(e2);
			RepositoryId appleId = _lexEntryRepository.GetId(e2);

			LexEntry e3 = _lexEntryRepository.CreateItem();
			e3.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "xa");
			//has to be something low in the alphabet to test a bug we had
			_lexEntryRepository.SaveItem(e3);
			RepositoryId xaId = _lexEntryRepository.GetId(e3);

			ResultSet<LexEntry> list =
					_lexEntryRepository.GetAllEntriesSortedByHeadword(_headwordWritingSystem);

			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(appleId, list[0].Id);
			Assert.AreEqual(bankId, list[1].Id);
			Assert.AreEqual(xaId, list[2].Id);
		}
	}

		[TestFixture]
		public class LexEntryRepositoryStateUnitializedTests:IRepositoryStateUnitializedTests<LexEntry>
		{
			private string _persistedFilePath;
			[SetUp]
			public void Setup()
			{
				this._persistedFilePath = Path.GetRandomFileName();
				_persistedFilePath = Path.GetFullPath(_persistedFilePath);
				this.RepositoryUnderTest = new LexEntryRepository(this._persistedFilePath);
			}

			[TearDown]
			public void Teardown()
			{
				RepositoryUnderTest.Dispose();
				File.Delete(this._persistedFilePath);
			}

			[Test, Ignore("Locking needs to be implemented in LiftRepository!")]
			[ExpectedException(typeof(IOException))]
			public void Constructor_FileIsWriteableAfterRepositoryIsCreated_Throws()
			{
				using (File.OpenWrite(_persistedFilePath))
				{
				}
			}

			[Test]
			[ExpectedException(typeof(IOException))]
			public void Constructor_FileIsNotWriteableWhenRepositoryIsCreated_Throws()
			{
				using (File.OpenWrite(_persistedFilePath))
				{
					LiftRepository repository = new LiftRepository(_persistedFilePath);
				}
			}
		}

		[TestFixture]
		public class LexEntryRepositoryCreatedFromPersistedData : IRepositoryPopulateFromPersistedTests<LexEntry>
		{
			private string _persistedFilePath;
			[SetUp]
			public void Setup()
			{
				this._persistedFilePath = LiftFileInitializer.MakeFile();
				this.RepositoryUnderTest = new LexEntryRepository(this._persistedFilePath);
			}

			[TearDown]
			public void Teardown()
			{
				RepositoryUnderTest.Dispose();
				File.Delete(this._persistedFilePath);
			}

			[Test]
			public override void SaveItem_LastModifiedIsChangedToLaterTime()
			{
				SetState();
				DateTime modifiedTimePreSave = RepositoryUnderTest.LastModified;
				MakeItemDirty(Item);
				RepositoryUnderTest.SaveItem(Item);
				Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreSave);
			}

			[Test]
			public override void SaveItems_LastModifiedIsChangedToLaterTime()
			{
				SetState();
				List<LexEntry> itemsToSave = new List<LexEntry>();
				itemsToSave.Add(Item);
				DateTime modifiedTimePreSave = RepositoryUnderTest.LastModified;
				MakeItemDirty(Item);
				RepositoryUnderTest.SaveItems(itemsToSave);
				Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreSave);
			}

			private static void MakeItemDirty(LexEntry Item)
			{
				Item.LexicalForm["de"] = "Sonne";
			}

			[Test]
			public override void LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime()
			{
				SetState();
				Assert.AreEqual(Item.ModificationTime, RepositoryUnderTest.LastModified);
			}

		[Test]
		public void Constructor_LexEntryIsDirtyIsFalse()
		{
			SetState();
			Assert.IsFalse(Item.IsDirty);
		}

			[Test]
			public override void GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery()
			{
				SetState();
				Query query = new Query(typeof(LexEntry)).Show("LexicalForm");
				ResultSet<LexEntry> resultsOfQuery = RepositoryUnderTest.GetItemsMatching(query);
				Assert.AreEqual(1, resultsOfQuery.Count);
				MultiText lexicalForm = (MultiText) resultsOfQuery[0]["LexicalForm"];
				Assert.AreEqual("Sonne", lexicalForm.Forms[0].Form);
			}

			protected override void CreateNewRepositoryFromPersistedData()
			{
				RepositoryUnderTest.Dispose();
				RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
			}
		}

		[TestFixture]
		public class LexEntryRepositoryCreateItemTransitionTests : IRepositoryCreateItemTransitionTests<LexEntry>
		{
			private string _persistedFilePath;
			[SetUp]
			public void Setup()
			{
				this._persistedFilePath = Path.GetRandomFileName();
				_persistedFilePath = Path.GetFullPath(_persistedFilePath);
				this.RepositoryUnderTest = new LexEntryRepository(this._persistedFilePath);
			}

			[TearDown]
			public void Teardown()
			{
				RepositoryUnderTest.Dispose();
				File.Delete(this._persistedFilePath);
			}



			[Test]
			public void SaveItem_LexEntryIsDirtyIsFalse()
			{
				SetState();
				RepositoryUnderTest.SaveItem(Item);
				Assert.IsFalse(Item.IsDirty);
			}

			[Test]
			public void SaveItems_LexEntryIsDirtyIsFalse()
			{
				SetState();
				List<LexEntry> itemsToBeSaved = new List<LexEntry>();
				itemsToBeSaved.Add(Item);
				RepositoryUnderTest.SaveItems(itemsToBeSaved);
				Assert.IsFalse(Item.IsDirty);
			}

			[Test]
			public void Constructor_LexEntryIsDirtyIsTrue()
			{
				SetState();
				Assert.IsTrue(Item.IsDirty);
			}

			[Test]
			public override void GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery()
			{
				SetState();
				Item.LexicalForm["de"] = "Sonne";
				Query query = new Query(typeof(LexEntry)).Show("LexicalForm");
				ResultSet<LexEntry> resultsOfQuery = RepositoryUnderTest.GetItemsMatching(query);
				Assert.AreEqual(1, resultsOfQuery.Count);
				Assert.AreEqual("Sonne", resultsOfQuery[0]["LexicalForm"].ToString());
			}

			protected override void CreateNewRepositoryFromPersistedData()
			{
				RepositoryUnderTest.Dispose();
				RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
			}
		}

		[TestFixture]
		public class LexEntryRepositoryDeleteItemTransitionTests : IRepositoryDeleteItemTransitionTests<LexEntry>
		{
			private string _persistedFilePath;
			[SetUp]
			public void Setup()
			{
				this._persistedFilePath = Path.GetRandomFileName();
				_persistedFilePath = Path.GetFullPath(_persistedFilePath);
				this.RepositoryUnderTest = new LexEntryRepository(this._persistedFilePath);
			}

			[TearDown]
			public void Teardown()
			{
				RepositoryUnderTest.Dispose();
				File.Delete(this._persistedFilePath);
			}

			[Test]
			[ExpectedException(typeof(ArgumentOutOfRangeException))]
			public override void SaveItem_ItemDoesNotExist_Throws()
			{
				SetState();
				Item.Senses.Add(new LexSense());
				RepositoryUnderTest.SaveItem(Item);
			}

			protected override void CreateNewRepositoryFromPersistedData()
			{
				RepositoryUnderTest.Dispose();
				RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
			}
		}

		[TestFixture]
		public class LexEntryRepositoryDeleteIdTransitionTests : IRepositoryDeleteIdTransitionTests<LexEntry>
		{
			private string _persistedFilePath;
			[SetUp]
			public void Setup()
			{
				this._persistedFilePath = Path.GetRandomFileName();
				_persistedFilePath = Path.GetFullPath(_persistedFilePath);
				this.RepositoryUnderTest = new LexEntryRepository(this._persistedFilePath);
			}

			[TearDown]
			public void Teardown()
			{
				RepositoryUnderTest.Dispose();
				File.Delete(this._persistedFilePath);
			}

			[Test]
			[ExpectedException(typeof(ArgumentOutOfRangeException))]
			public override void SaveItem_ItemDoesNotExist_Throws()
			{
				SetState();
				Item.Senses.Add(new LexSense());
				RepositoryUnderTest.SaveItem(Item);
			}

			protected override void CreateNewRepositoryFromPersistedData()
			{
				RepositoryUnderTest.Dispose();
				RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
			}
		}

	[TestFixture]
	public class LexEntryRepositoryDeleteAllItemsTransitionTests : IRepositoryDeleteAllItemsTransitionTests<LexEntry>
	{
		private string _persistedFilePath;

		[SetUp]
		public void Setup()
		{
			_persistedFilePath = Path.GetRandomFileName();
			_persistedFilePath = Path.GetFullPath(_persistedFilePath);
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
		}

		protected override void RepopulateRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}
	}