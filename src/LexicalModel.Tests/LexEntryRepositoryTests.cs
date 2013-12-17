using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using Palaso.Text;
using WeSay.Data;
using WeSay.Data.Tests;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.Foundation.Tests.TestHelpers;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryTests
	{
		private TemporaryFolder _temporaryFolder;
		private LexEntryRepository _lexEntryRepository;
		private WritingSystem _headwordWritingSystem;

		[SetUp]
		public void Setup()
		{
			_temporaryFolder = new TemporaryFolder();
			string filePath = _temporaryFolder.GetTemporaryFile();
			_lexEntryRepository = new LexEntryRepository(filePath);
			_headwordWritingSystem = new WritingSystem();
			_headwordWritingSystem.Id = "primary";
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			_temporaryFolder.Delete();
		}

		private void MakeTestLexEntry(string writingSystemId, string lexicalForm)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(writingSystemId, lexicalForm);
			_lexEntryRepository.SaveItem(entry);
			return;
		}

		private delegate MultiText GetMultiTextFromLexEntryDelegate(LexEntry entry);

		private void CreateThreeDifferentLexEntries(GetMultiTextFromLexEntryDelegate getMultiTextFromLexEntryDelegate)
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
			CreateThreeDifferentLexEntries(delegate(LexEntry e) { return e.CitationForm; });
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByHeadWord = _lexEntryRepository.GetAllEntriesSortedByHeadword(german);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByHeadWord[0]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByHeadWord[1]["Form"]);
			Assert.AreEqual("de Word3", listOfLexEntriesSortedByHeadWord[2]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByHeadword_CitationAndLexicalFormInWritingSystemDoNotExist_ReturnsNullForThatEntry()
		{
			LexEntry lexEntryWithOutFrenchHeadWord = _lexEntryRepository.CreateItem();
			lexEntryWithOutFrenchHeadWord.LexicalForm.SetAlternative("de", "de Word1");
			lexEntryWithOutFrenchHeadWord.CitationForm.SetAlternative("de", "de Word1");
			WritingSystem french = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByHeadWord = _lexEntryRepository.GetAllEntriesSortedByHeadword(french);
			Assert.AreEqual(null, listOfLexEntriesSortedByHeadWord[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByHeadword_CitationFormInWritingSystemDoesNotExistButLexicalFormDoes_SortsByLexicalFormForThatEntry()
		{
			CreateThreeDifferentLexEntries(delegate(LexEntry e) { return e.CitationForm; });
			LexEntry lexEntryWithOutGermanCitationForm = _lexEntryRepository.CreateItem();
			lexEntryWithOutGermanCitationForm.CitationForm.SetAlternative("fr", "fr Word4");
			lexEntryWithOutGermanCitationForm.LexicalForm.SetAlternative("de", "de Word0");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByHeadWord = _lexEntryRepository.GetAllEntriesSortedByHeadword(german);
			Assert.AreEqual(4, listOfLexEntriesSortedByHeadWord.Count);
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
		public void GetAllEntriesSortedByLexicalFormOrAlternative_RepositoryIsEmpty_ReturnsEmptyList()
		{
			Assert.AreEqual(0, _lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem()).Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetAllEntriesSortedByLexicalFormOrAlternative_Null_Throws()
		{
			_lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(null);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalFormOrAlternative_LexicalFormExistsInWritingSystemForAllEntries_ReturnsListSortedByLexicalForm()
		{
			CreateThreeDifferentLexEntries(delegate(LexEntry e) { return e.LexicalForm; });
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByLexicalForm = _lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(german);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByLexicalForm[0]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByLexicalForm[1]["Form"]);
			Assert.AreEqual("de Word3", listOfLexEntriesSortedByLexicalForm[2]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalFormOrAlternative_LexicalFormDoesNotExistInAnyWritingSystem_ReturnsNullForThatEntry()
		{
			LexEntry lexEntryWithOutFrenchHeadWord = _lexEntryRepository.CreateItem();
			_lexEntryRepository.SaveItem(lexEntryWithOutFrenchHeadWord);
			WritingSystem french = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByLexicalForm = _lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(french);
			Assert.AreEqual(null, listOfLexEntriesSortedByLexicalForm[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalFormOrAlternative_LexicalFormDoesNotExistInPrimaryWritingSystemButDoesInAnother_ReturnsAlternativeThatEntry()
		{
			LexEntry lexEntryWithOutFrenchHeadWord = _lexEntryRepository.CreateItem();
			lexEntryWithOutFrenchHeadWord.LexicalForm.SetAlternative("de", "de word1");
			_lexEntryRepository.SaveItem(lexEntryWithOutFrenchHeadWord);
			WritingSystem french = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByLexicalForm = _lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(french);
			Assert.AreEqual("de word1", listOfLexEntriesSortedByLexicalForm[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalFormOrAlternative_LexicalFormExistsInWritingSystem_ReturnsWritingSystem()
		{
			LexEntry lexEntryWithOutFrenchHeadWord = _lexEntryRepository.CreateItem();
			lexEntryWithOutFrenchHeadWord.LexicalForm.SetAlternative("fr", "fr word1");
			_lexEntryRepository.SaveItem(lexEntryWithOutFrenchHeadWord);
			WritingSystem french = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByLexicalForm = _lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(french);
			Assert.AreEqual("fr", listOfLexEntriesSortedByLexicalForm[0]["WritingSystem"]);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalFormOrAlternative_LexicalFormDoesNotExistInPrimaryWritingSystemButDoesInAnother_ReturnsWritingSystem()
		{
			LexEntry lexEntryWithOutFrenchHeadWord = _lexEntryRepository.CreateItem();
			lexEntryWithOutFrenchHeadWord.LexicalForm.SetAlternative("de", "de word1");
			_lexEntryRepository.SaveItem(lexEntryWithOutFrenchHeadWord);
			WritingSystem french = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByLexicalForm = _lexEntryRepository.GetAllEntriesSortedByLexicalFormOrAlternative(french);
			Assert.AreEqual("de", listOfLexEntriesSortedByLexicalForm[0]["WritingSystem"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_RepositoryIsEmpty_ReturnsEmptyList()
		{
			Assert.AreEqual(0, _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem()).Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetAllEntriesSortedByDefinition_Null_Throws()
		{
			_lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(null);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionExistsInWritingSystemForAllEntries_ReturnsListSortedByDefinition()
		{
			CreateThreeDifferentLexEntries(delegate(LexEntry e)
														 {
															 e.Senses.Add(new LexSense());
															 return e.Senses[0].Definition;
														 });
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(german);
			Assert.AreEqual(3, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByDefinition[1]["Form"]);
			Assert.AreEqual("de Word3", listOfLexEntriesSortedByDefinition[2]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionsDoesNotExistInWritingSystemButGlossDoesForAllEntries_ReturnsListSortedByGloss()
		{
			CreateThreeDifferentLexEntries(delegate(LexEntry e)
														 {
															 e.Senses.Add(new LexSense());
															 return e.Senses[0].Gloss;
														 });
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByGloss = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(german);
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
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(german);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByDefinition[1]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionIsIdenticalInWritingSystemInMultipleSenses_ReturnsSortedListWithBothDefinitions()
		{
			LexEntry lexEntryWithBothDefinitionAndAGloss = _lexEntryRepository.CreateItem();
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Definition.SetAlternative("de", "de Word1");
			lexEntryWithBothDefinitionAndAGloss.Senses[1].Definition.SetAlternative("de", "de Word1");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(german);
			Assert.AreEqual(2, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[1]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_GlossIsIdenticalInWritingSystemInMultipleSenses_ReturnsSortedListWithBothDefinitions()
		{
			LexEntry lexEntryWithBothDefinitionAndAGloss = _lexEntryRepository.CreateItem();
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Gloss.SetAlternative("de", "de Word1");
			lexEntryWithBothDefinitionAndAGloss.Senses[1].Gloss.SetAlternative("de", "de Word1");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(german);
			Assert.AreEqual(2, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[1]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionAndGlossAreNullInWritingSystemInMultipleSenses_ReturnsSortedListWithBothDefinitions()
		{
			LexEntry lexEntryWithBothDefinitionAndAGloss = _lexEntryRepository.CreateItem();
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(german);
			Assert.AreEqual(2, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual(null, listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual(null, listOfLexEntriesSortedByDefinition[1]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionAndGlossAreIdenticalInWritingSystemInMultipleSenses_ReturnsSortedListWithBothDefinitions()
		{
			LexEntry lexEntryWithBothDefinitionAndAGloss = _lexEntryRepository.CreateItem();
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Definition.SetAlternative("de", "de Word1");
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Gloss.SetAlternative("de", "de Word1");
			lexEntryWithBothDefinitionAndAGloss.Senses[1].Gloss.SetAlternative("de", "de Word1");
			lexEntryWithBothDefinitionAndAGloss.Senses[1].Gloss.SetAlternative("de", "de Word1");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(german);
			Assert.AreEqual(2, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[1]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionAndGlossInWritingSystemDoNotExist_ReturnsNullForThatEntry()
		{
			LexEntry lexEntryWithOutFrenchGloss = _lexEntryRepository.CreateItem();
			lexEntryWithOutFrenchGloss.Senses.Add(new LexSense());
			lexEntryWithOutFrenchGloss.Senses[0].Definition.SetAlternative("de", "de Word1");
			WritingSystem french = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(french);
			Assert.AreEqual(1, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual(null, listOfLexEntriesSortedByDefinition[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionAndGlossOfOneEntryAreIdentical_ReturnsOnlyOneRecordToken()
		{
			CreateThreeDifferentLexEntries(delegate(LexEntry e)
														 {
															 e.Senses.Add(new LexSense());
															 return e.Senses[0].Definition;
														 });
			LexEntry lexEntryWithBothDefinitionAndAGloss = _lexEntryRepository.CreateItem();
			lexEntryWithBothDefinitionAndAGloss.Senses.Add(new LexSense());
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Definition.SetAlternative("de", "de Word4");
			lexEntryWithBothDefinitionAndAGloss.Senses[0].Gloss.SetAlternative("de", "de Word4");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(german);
			Assert.AreEqual(4, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual("de Word2", listOfLexEntriesSortedByDefinition[1]["Form"]);
			Assert.AreEqual("de Word3", listOfLexEntriesSortedByDefinition[2]["Form"]);
			Assert.AreEqual("de Word4", listOfLexEntriesSortedByDefinition[3]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DefinitionOfTwoEntriesAreIdentical_ReturnsTwoRecordTokens()
		{
			LexEntry lexEntryWithBothDefinition = _lexEntryRepository.CreateItem();
			lexEntryWithBothDefinition.Senses.Add(new LexSense());
			lexEntryWithBothDefinition.Senses[0].Definition.SetAlternative("de", "de Word1");
			LexEntry lexEntryTwoWithBothDefinition = _lexEntryRepository.CreateItem();
			lexEntryTwoWithBothDefinition.Senses.Add(new LexSense());
			lexEntryTwoWithBothDefinition.Senses[0].Definition.SetAlternative("de", "de Word1");
			WritingSystem german = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> listOfLexEntriesSortedByDefinition = _lexEntryRepository.GetAllEntriesSortedByDefinitionOrGloss(german);
			Assert.AreEqual(2, listOfLexEntriesSortedByDefinition.Count);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[0]["Form"]);
			Assert.AreEqual("de Word1", listOfLexEntriesSortedByDefinition[1]["Form"]);
		}

		[Test]
		public void GetEntriesWithSemanticDomainSortedBySemanticDomain_EntriesWithDifferingSemanticDomains_EntriesAreSortedBySemanticDomain()
		{
			CreateLexEntryWithSemanticDomain("SemanticDomain2");
			CreateLexEntryWithSemanticDomain("SemanticDomain1");
			ResultSet<LexEntry> sortedResults = _lexEntryRepository.GetEntriesWithSemanticDomainSortedBySemanticDomain(LexSense.WellKnownProperties.SemanticDomainsDdp4);
			Assert.AreEqual(2, sortedResults.Count);
			Assert.AreEqual("SemanticDomain1", sortedResults[0]["SemanticDomain"]);
			Assert.AreEqual("SemanticDomain2", sortedResults[1]["SemanticDomain"]);
		}

		[Test]
		public void GetEntriesWithSemanticDomainSortedBySemanticDomain_EntryWithoutSemanticDomain_ReturnEmpty()
		{
			LexEntry lexEntryWithoutSemanticDomain = _lexEntryRepository.CreateItem();
			ResultSet<LexEntry> sortedResults = _lexEntryRepository.GetEntriesWithSemanticDomainSortedBySemanticDomain(LexSense.WellKnownProperties.SemanticDomainsDdp4);
			Assert.AreEqual(0, sortedResults.Count);
		}

		private void CreateLexEntryWithSemanticDomain(string semanticDomain)
		{
			LexEntry lexEntryWithSemanticDomain = _lexEntryRepository.CreateItem();
			lexEntryWithSemanticDomain.Senses.Add(new LexSense());
			OptionRefCollection o =
				lexEntryWithSemanticDomain.Senses[0].GetOrCreateProperty<OptionRefCollection>(
					LexSense.WellKnownProperties.SemanticDomainsDdp4);
			o.Add(semanticDomain);
			_lexEntryRepository.SaveItem(lexEntryWithSemanticDomain);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetEntriesWithSimilarLexicalForm_WritingSystemNull_Throws()
		{
			WritingSystem ws = null;
			ResultSet<LexEntry> matches =
					 _lexEntryRepository.GetEntriesWithSimilarLexicalForm("",
																		  ws,
																		  ApproximateMatcherOptions.
																				  IncludePrefixedForms);
		}

		[Test]
		public void GetEntriesWithSimilarLexicalForm_MultipleEntriesWithEqualAndLowestMatchingDistance_ReturnsThoseEntries()
		{
			CreateThreeDifferentLexEntries(delegate(LexEntry e) { return e.LexicalForm; });
			WritingSystem ws = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches =
					 _lexEntryRepository.GetEntriesWithSimilarLexicalForm("",
																		  ws,
																		  ApproximateMatcherOptions.
																				  IncludePrefixedForms);
			Assert.AreEqual(3, matches.Count);
		}

		[Test]
		public void GetEntriesWithSimilarLexicalForm_MultipleEntriesWithDifferentMatchingDistanceAndAnyEntriesBeginningWithWhatWeAreMatchingHaveZeroDistance_ReturnsAllEntriesBeginningWithTheFormToMatch()
		{
			//Matching distance as compared to Empty string
			LexEntry lexEntryWithMatchingDistance1 = _lexEntryRepository.CreateItem();
			lexEntryWithMatchingDistance1.LexicalForm.SetAlternative("de", "d");
			LexEntry lexEntryWithMatchingDistance2 = _lexEntryRepository.CreateItem();
			lexEntryWithMatchingDistance2.LexicalForm.SetAlternative("de", "de");
			LexEntry lexEntryWithMatchingDistance3 = _lexEntryRepository.CreateItem();
			lexEntryWithMatchingDistance3.LexicalForm.SetAlternative("de", "de_");
			WritingSystem ws = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches =
					 _lexEntryRepository.GetEntriesWithSimilarLexicalForm("",
																		  ws,
																		  ApproximateMatcherOptions.IncludePrefixedForms);
			Assert.AreEqual(3, matches.Count);
			Assert.AreEqual("d", matches[0]["Form"]);
			Assert.AreEqual("de", matches[1]["Form"]);
			Assert.AreEqual("de_", matches[2]["Form"]);
		}

		[Test]
		public void GetEntriesWithSimilarLexicalForm_MultipleEntriesWithDifferentMatchingDistanceAndAnyEntriesBeginningWithWhatWeAreMatchingHaveZeroDistanceAndWeWantTheTwoClosestMatches_ReturnsAllEntriesBeginningWithTheFormToMatchAsWellAsTheNextClosestMatch()
		{
			LexEntry lexEntryWithMatchingDistance0 = _lexEntryRepository.CreateItem();
			lexEntryWithMatchingDistance0.LexicalForm.SetAlternative("de", "de Word1");
			LexEntry lexEntryWithMatchingDistance1 = _lexEntryRepository.CreateItem();
			lexEntryWithMatchingDistance1.LexicalForm.SetAlternative("de", "fe");
			LexEntry lexEntryWithMatchingDistance2 = _lexEntryRepository.CreateItem();
			lexEntryWithMatchingDistance2.LexicalForm.SetAlternative("de", "ft");
			WritingSystem ws = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches =
					 _lexEntryRepository.GetEntriesWithSimilarLexicalForm("de",
																		  ws,
																		  ApproximateMatcherOptions.IncludePrefixedAndNextClosestForms);
			Assert.AreEqual(2, matches.Count);
			Assert.AreEqual("de Word1", matches[0]["Form"]);
			Assert.AreEqual("fe", matches[1]["Form"]);
		}

		[Test]
		public void GetEntriesWithSimilarLexicalForm_MultipleEntriesWithDifferentMatchingDistanceAndWeWantTheTwoClosestForms_ReturnsTwoEntriesWithLowestMatchingDistance()
		{
			//Matching distance as compared to Empty string
			LexEntry lexEntryWithMatchingDistance1 = _lexEntryRepository.CreateItem();
			lexEntryWithMatchingDistance1.LexicalForm.SetAlternative("de", "d");
			LexEntry lexEntryWithMatchingDistance2 = _lexEntryRepository.CreateItem();
			lexEntryWithMatchingDistance2.LexicalForm.SetAlternative("de", "de");
			LexEntry lexEntryWithMatchingDistance3 = _lexEntryRepository.CreateItem();
			lexEntryWithMatchingDistance3.LexicalForm.SetAlternative("de", "de_");
			WritingSystem ws = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches =
					 _lexEntryRepository.GetEntriesWithSimilarLexicalForm("",
																		  ws,
																		  ApproximateMatcherOptions.IncludeNextClosestForms);
			Assert.AreEqual(2, matches.Count);
			Assert.AreEqual("d", matches[0]["Form"]);
			Assert.AreEqual("de", matches[1]["Form"]);
		}

		[Test]
		public void GetEntriesWithSimilarLexicalForm_EntriesWithDifferingWritingSystems_OnlyFindEntriesMatchingTheGivenWritingSystem()
		{
			CreateThreeDifferentLexEntries(delegate(LexEntry e) { return e.LexicalForm; });
			LexEntry lexEntryWithFrenchLexicalForm = _lexEntryRepository.CreateItem();
			lexEntryWithFrenchLexicalForm.LexicalForm.SetAlternative("fr", "de Word2");
			WritingSystem ws = new WritingSystem("de", SystemFonts.DefaultFont);

			ResultSet<LexEntry> matches =
					_lexEntryRepository.GetEntriesWithSimilarLexicalForm("de Wor",
																		 ws,
																		 ApproximateMatcherOptions.
																				 IncludePrefixedForms);
			Assert.AreEqual(3, matches.Count);
			Assert.AreEqual("de Word1", matches[0]["Form"]);
			Assert.AreEqual("de Word2", matches[1]["Form"]);
			Assert.AreEqual("de Word3", matches[2]["Form"]);
		}

		[Test]
		public void
				GetEntriesWithMatchingLexicalForm_RepositoryContainsTwoEntriesWithDifferingLexicalForms_OnlyEntryWithmatchingLexicalFormIsFound
				()
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
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetEntriesWithMatchingLexicalForm_WritingSystemNull_Throws()
		{
			WritingSystem lexicalFormWritingSystem = null;
			ResultSet<LexEntry> matches = _lexEntryRepository.GetEntriesWithMatchingLexicalForm("de Word1", lexicalFormWritingSystem);
		}

		[Test]
		public void GetEntriesWithMatchingLexicalForm_MultipleMatchingEntries_ReturnsMatchingEntries()
		{
			CreateLexEntryWithLexicalForm("de Word1");
			CreateLexEntryWithLexicalForm("de Word1");
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches = _lexEntryRepository.GetEntriesWithMatchingLexicalForm("de Word1", lexicalFormWritingSystem);
			Assert.AreEqual(2, matches.Count);
		}

		private void CreateLexEntryWithLexicalForm(string lexicalForm)
		{
			LexEntry lexEntryWithLexicalForm = _lexEntryRepository.CreateItem();
			lexEntryWithLexicalForm.LexicalForm["de"] = lexicalForm;
			_lexEntryRepository.SaveItem(lexEntryWithLexicalForm);
		}

		[Test]
		public void GetEntriesWithMatchingLexicalForm_NoMatchingEntries_ReturnsEmpty()
		{
			CreateLexEntryWithLexicalForm("de Word1");
			CreateLexEntryWithLexicalForm("de Word1");
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches = _lexEntryRepository.GetEntriesWithMatchingLexicalForm("de Word2", lexicalFormWritingSystem);
			Assert.AreEqual(0, matches.Count);
		}

		[Test]
		public void GetEntriesWithMatchingLexicalForm_NoMatchesInWritingSystem_ReturnsEmpty()
		{
			CreateLexEntryWithLexicalForm("de Word1");
			CreateLexEntryWithLexicalForm("de Word1");
			WritingSystem lexicalFormWritingSystem = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches = _lexEntryRepository.GetEntriesWithMatchingLexicalForm("de Word2", lexicalFormWritingSystem);
			Assert.AreEqual(0, matches.Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetLexEntryWithMatchingGuid_GuidIsEmpty_Throws()
		{
			LexEntry found = _lexEntryRepository.GetLexEntryWithMatchingGuid(Guid.Empty);
		}

		[Test]
		public void GetLexEntryWithMatchingGuid_GuidExists_ReturnsEntryWithGuid()
		{
			LexEntry lexEntryWithGuid = _lexEntryRepository.CreateItem();
			Guid guidToFind = Guid.NewGuid();
			lexEntryWithGuid.Guid = guidToFind;
			LexEntry found = _lexEntryRepository.GetLexEntryWithMatchingGuid(guidToFind);
			Assert.AreSame(lexEntryWithGuid, found);
		}

		[Test]
		public void GetLexEntryWithMatchingGuid_GuidDoesNotExist_ReturnsNull()
		{
			LexEntry found = _lexEntryRepository.GetLexEntryWithMatchingGuid(Guid.NewGuid());
			Assert.IsNull(found);
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void GetLexEntryWithMatchingGuid_MultipleGuidMatchesInRepo_Throws()
		{
			LexEntry lexEntryWithGuid = _lexEntryRepository.CreateItem();
			Guid guidToFind = Guid.NewGuid();
			lexEntryWithGuid.Guid = guidToFind;
			LexEntry lexEntryWithConflictingGuid = _lexEntryRepository.CreateItem();
			lexEntryWithConflictingGuid.Guid = guidToFind;
			_lexEntryRepository.GetLexEntryWithMatchingGuid(guidToFind);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetLexEntryWithMatchingId_IdIsEmpty_Throws()
		{
			LexEntry found = _lexEntryRepository.GetLexEntryWithMatchingId("");
		}

		[Test]
		public void GetLexEntryWithMatchingId_IdExists_ReturnsEntryWithId()
		{
			LexEntry lexEntryWithId = _lexEntryRepository.CreateItem();
			string idToFind = "This is the id.";
			lexEntryWithId.Id = idToFind;
			LexEntry found = _lexEntryRepository.GetLexEntryWithMatchingId(idToFind);
			Assert.AreSame(lexEntryWithId, found);
		}

		[Test]
		public void GetLexEntryWithMatchingId_IdDoesNotExist_ReturnsNull()
		{
			LexEntry found = _lexEntryRepository.GetLexEntryWithMatchingId("This is a nonexistent Id.");
			Assert.IsNull(found);
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void GetLexEntryWithMatchingId_MultipleIdMatchesInRepo_Throws()
		{
			LexEntry lexEntryWithId = _lexEntryRepository.CreateItem();
			string idToFind = "This is an id";
			lexEntryWithId.Id = idToFind;
			LexEntry lexEntryWithConflictingId = _lexEntryRepository.CreateItem();
			lexEntryWithConflictingId.Id = idToFind;
			_lexEntryRepository.GetLexEntryWithMatchingId(idToFind);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetEntriesWithMatchingGlossSortedByLexicalForm_WritingSystemNull_Throws()
		{
			WritingSystem writingSystem = new WritingSystem("en", SystemFonts.DefaultFont);
			ResultSet<LexEntry> list =
					_lexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(
							null, writingSystem);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetEntriesWithMatchingGlossSortedByLexicalForm_LanguageFormNull_Throws()
		{
			LanguageForm glossLanguageForm = new LanguageForm("en", "en Gloss", new MultiText());
			ResultSet<LexEntry> list =
					_lexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(
							glossLanguageForm, null);
		}

		[Test]
		public void GetEntriesWithMatchingGlossSortedByLexicalForm_TwoEntriesWithDifferingGlosses_OnlyEntryWithmatchingGlossIsFound()
		{
			const string glossToFind = "Gloss To Find.";
			AddEntryWithGloss(glossToFind);
			AddEntryWithGloss("Gloss Not To Find.");
			LanguageForm glossLanguageForm = new LanguageForm("en", glossToFind, new MultiText());
			WritingSystem writingSystem = new WritingSystem("en", SystemFonts.DefaultFont);
			ResultSet<LexEntry> list =
					_lexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(
							glossLanguageForm, writingSystem);
			Assert.AreEqual(1, list.Count);
			Assert.AreSame(glossToFind, list[0].RealObject.Senses[0].Gloss["en"]);
		}

		[Test]
		public void GetEntriesWithMatchingGlossSortedByLexicalForm_GlossDoesNotExist_ReturnsEmpty()
		{
			WritingSystem ws = new WritingSystem("en", SystemFonts.DefaultFont);
			LanguageForm glossThatDoesNotExist = new LanguageForm("en", "I don't exist!", new MultiText());
			ResultSet<LexEntry> matches = _lexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(glossThatDoesNotExist, ws);
			Assert.AreEqual(0, matches.Count);
		}

		[Test]
		public void GetEntriesWithMatchingGlossSortedByLexicalForm_TwoEntriesWithSameGlossButDifferentLexicalForms_ReturnsListSortedByLexicalForm()
		{
			LanguageForm glossToMatch = new LanguageForm("de", "de Gloss", new MultiText());
			CreateEntryWithLexicalFormAndGloss(glossToMatch, "en", "en LexicalForm2");
			CreateEntryWithLexicalFormAndGloss(glossToMatch, "en", "en LexicalForm1");
			WritingSystem lexicalFormWritingSystem = new WritingSystem("en", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches = _lexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(glossToMatch, lexicalFormWritingSystem);
			Assert.AreEqual("en LexicalForm1", matches[0]["Form"]);
			Assert.AreEqual("en LexicalForm2", matches[1]["Form"]);
		}

		private void CreateEntryWithLexicalFormAndGloss(LanguageForm glossToMatch,
														string lexicalFormWritingSystem, string lexicalForm)
		{
			LexEntry entryWithGlossAndLexicalForm = _lexEntryRepository.CreateItem();
			entryWithGlossAndLexicalForm.Senses.Add(new LexSense());
			entryWithGlossAndLexicalForm.Senses[0].Gloss.SetAlternative(glossToMatch.WritingSystemId, glossToMatch.Form);
			entryWithGlossAndLexicalForm.LexicalForm.SetAlternative(lexicalFormWritingSystem, lexicalForm);
		}

		[Test]
		public void GetEntriesWithMatchingGlossSortedByLexicalForm_EntryHasNoLexicalFormInWritingSystem_ReturnsNullForThatEntry()
		{
			LanguageForm glossToMatch = new LanguageForm("de", "de Gloss", new MultiText());
			CreateEntryWithLexicalFormAndGloss(glossToMatch, "en", "en LexicalForm2");
			WritingSystem lexicalFormWritingSystem = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches = _lexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(glossToMatch, lexicalFormWritingSystem);
			Assert.AreEqual(null, matches[0]["Form"]);
		}

		[Test]
		public void GetEntriesWithMatchingGlossSortedByLexicalForm_EntryHasIdenticalSenses_ReturnsBoth()
		{
			LanguageForm identicalGloss = new LanguageForm("de", "de Gloss", new MultiText());

			LexEntry entryWithGlossAndLexicalForm = _lexEntryRepository.CreateItem();
			entryWithGlossAndLexicalForm.LexicalForm.SetAlternative("en", "en Word1");
			entryWithGlossAndLexicalForm.Senses.Add(new LexSense());
			entryWithGlossAndLexicalForm.Senses[0].Gloss.SetAlternative(identicalGloss.WritingSystemId, identicalGloss.Form);
			entryWithGlossAndLexicalForm.Senses.Add(new LexSense());
			entryWithGlossAndLexicalForm.Senses[1].Gloss.SetAlternative(identicalGloss.WritingSystemId, identicalGloss.Form);

			WritingSystem lexicalFormWritingSystem = new WritingSystem("en", SystemFonts.DefaultFont);
			ResultSet<LexEntry> matches = _lexEntryRepository.GetEntriesWithMatchingGlossSortedByLexicalForm(identicalGloss, lexicalFormWritingSystem);
			Assert.AreEqual(2, matches.Count);
			Assert.AreEqual("en Word1", matches[0]["Form"]);
			Assert.AreEqual("en Word1", matches[1]["Form"]);
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
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_FieldNull_Throws()
		{
			Field fieldToFill = null;
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, lexicalFormWritingSystem);
			Assert.AreEqual(0, sortedResults.Count);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_FieldNameDoesNotExist_ReturnsEmpty()
		{
			Field fieldToFill = new Field("I do not exist!", "LexEntry", new string[] { "fr" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, lexicalFormWritingSystem);
			Assert.AreEqual(0, sortedResults.Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_WritingSystemNull_Throws()
		{
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "fr" });
			WritingSystem lexicalFormWritingSystem = null;
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, lexicalFormWritingSystem);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_EntryInWritingSystemInFieldDoesNotExist_ReturnsEntries()
		{
			CreateLexentryWithCitation("de", "de Word2");
			CreateLexentryWithCitation("de", "de Word1");
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "fr" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, lexicalFormWritingSystem);
			Assert.AreEqual(2, sortedResults.Count);
			Assert.AreEqual(null, sortedResults[0]["Form"]);
			Assert.AreEqual(null, sortedResults[1]["Form"]);
		}

		private void CreateLexentryWithCitation(string citationWritingSystem, string lexicalForm)
		{
			LexEntry lexEntryWithMissingCitation = _lexEntryRepository.CreateItem();
			lexEntryWithMissingCitation.CitationForm[citationWritingSystem] = lexicalForm;
			_lexEntryRepository.SaveItem(lexEntryWithMissingCitation);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_MultipleEntriesWithMissingFieldExist_ReturnsEntriesSortedByLexicalForm()
		{
			CreateLexentryWithLexicalFormButWithoutCitation("de Word2");
			CreateLexentryWithLexicalFormButWithoutCitation("de Word1");
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, lexicalFormWritingSystem);
			Assert.AreEqual(2, sortedResults.Count);
			Assert.AreEqual("de Word1", sortedResults[0]["Form"]);
			Assert.AreEqual("de Word2", sortedResults[1]["Form"]);
		}

		private void CreateLexentryWithLexicalFormButWithoutCitation(string lexicalForm)
		{
			LexEntry lexEntryWithMissingCitation = _lexEntryRepository.CreateItem();
			lexEntryWithMissingCitation.LexicalForm["de"] = lexicalForm;
			_lexEntryRepository.SaveItem(lexEntryWithMissingCitation);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_LexicalFormDoesNotExistInWritingSystem_ReturnsNullForThatEntry()
		{
			LexEntry lexEntryWithMissingCitation = _lexEntryRepository.CreateItem();
			_lexEntryRepository.SaveItem(lexEntryWithMissingCitation);
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("fr", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, lexicalFormWritingSystem);
			Assert.AreEqual(1, sortedResults.Count);
			Assert.AreEqual(null, sortedResults[0]["Form"]);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_FieldExists_DoesNotReturnResultForThatLexEntry()
		{
			CreateLexentryWithLexicalFormButWithoutCitation("de Word1");
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.LexicalUnit, "LexEntry", new string[] { "de" });
			WritingSystem lexicalFormWritingSystem = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> sortedResults =
				_lexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, lexicalFormWritingSystem);
			Assert.AreEqual(0, sortedResults.Count);
		}

		[Test]
		public void GetHomographNumber_OnlyOneEntry_Returns0()
		{
			LexEntry entry1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Assert.AreEqual(0,
							_lexEntryRepository.GetHomographNumber(entry1, _headwordWritingSystem));
		}

		[Test]
		public void GetHomographNumber_FirstEntryWithFollowingHomograph_Returns1()
		{
			LexEntry entry1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Assert.AreEqual(1,
							_lexEntryRepository.GetHomographNumber(entry1, _headwordWritingSystem));
		}

		[Test]
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
			entries[1] = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			entries[2] = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			List<int> ids = new List<int>(entries.Length);
			foreach (LexEntry entry in entries)
			{
				int homographNumber = _lexEntryRepository.GetHomographNumber(entry,
																			 _headwordWritingSystem);
				Assert.IsFalse(ids.Contains(homographNumber));
				ids.Add(homographNumber);
			}
		}

		[Test]
		public void GetHomographNumber_ThirdEntry_Returns3()
		{
			LexEntry entryOther = MakeEntryWithLexemeForm("en", "blue");
			Assert.AreNotEqual("en", _headwordWritingSystem.Id);
			LexEntry entry1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			LexEntry entry2 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			LexEntry entry3 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Assert.AreEqual(3,
							_lexEntryRepository.GetHomographNumber(entry3, _headwordWritingSystem));
			Assert.AreEqual(2,
							_lexEntryRepository.GetHomographNumber(entry2, _headwordWritingSystem));
			Assert.AreEqual(1,
							_lexEntryRepository.GetHomographNumber(entry1, _headwordWritingSystem));
		}

		[Test]
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

		[Test]
		public void GetHomographNumber_3SameLexicalFormsAnd3OtherLexicalForms_Returns123()
		{
			LexEntry red1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "red");
			LexEntry blue1 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			LexEntry red2 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "red");
			LexEntry blue2 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			LexEntry red3 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "red");
			LexEntry blue3 = MakeEntryWithLexemeForm(_headwordWritingSystem.Id, "blue");
			Assert.AreEqual(1, _lexEntryRepository.GetHomographNumber(blue1, _headwordWritingSystem));
			Assert.AreEqual(3, _lexEntryRepository.GetHomographNumber(blue3, _headwordWritingSystem));
			Assert.AreEqual(2, _lexEntryRepository.GetHomographNumber(blue2, _headwordWritingSystem));
			Assert.AreEqual(1, _lexEntryRepository.GetHomographNumber(red1, _headwordWritingSystem));
			Assert.AreEqual(3, _lexEntryRepository.GetHomographNumber(red3, _headwordWritingSystem));
			Assert.AreEqual(2, _lexEntryRepository.GetHomographNumber(red2, _headwordWritingSystem));
		}

		[Test]
		[Ignore("not implemented")]
		public void GetHomographNumber_HonorsOrderAttribute() { }

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
	public class LexEntryRepositoryStateUnitializedTests : IRepositoryStateUnitializedTests<LexEntry>
	{
		private string _persistedFilePath;
		private TemporaryFolder _tempFolder;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder();
			_persistedFilePath = _tempFolder.GetTemporaryFile();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			_tempFolder.Delete();
		}

		[Test]
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
	public class LexEntryRepositoryCreatedFromPersistedData :
			IRepositoryPopulateFromPersistedTests<LexEntry>
	{
		private TemporaryFolder _tempFolder;
		private string _persistedFilePath;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder();
			_persistedFilePath = LiftFileInitializer.MakeFile(_tempFolder.GetTemporaryFile());
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			_tempFolder.Delete();
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

		protected override void  LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime_v()
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

		protected override void  GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery_v()
		{
			SetState();
			QueryAdapter<LexEntry> query = new QueryAdapter<LexEntry>();
			query.Show("LexicalForm");
			ResultSet<LexEntry> resultsOfQuery = RepositoryUnderTest.GetItemsMatching(query);
			Assert.AreEqual(1, resultsOfQuery.Count);
			MultiText lexicalForm = (MultiText)resultsOfQuery[0]["LexicalForm"];
			Assert.AreEqual("Sonne", lexicalForm.Forms[0].Form);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}

	[TestFixture]
	public class LexEntryRepositoryCreateItemTransitionTests :
			IRepositoryCreateItemTransitionTests<LexEntry>
	{
		private TemporaryFolder _tempFolder;
		private string _persistedFilePath;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder();
			_persistedFilePath = _tempFolder.GetTemporaryFile();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			_tempFolder.Delete();
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

		protected override void  GetItemsMatchingQuery_QueryWithShow_ReturnAllItemsMatchingQuery_v()
		{
			Item.LexicalForm["de"] = "Sonne";
			QueryAdapter<LexEntry> query = new QueryAdapter<LexEntry>();
			query.Show("LexicalForm");
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
	public class LexEntryRepositoryDeleteItemTransitionTests :
			IRepositoryDeleteItemTransitionTests<LexEntry>
	{
		private string _persistedFilePath;
		private TemporaryFolder _tempFolder;


		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder();
			_persistedFilePath = _tempFolder.GetTemporaryFile();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			_tempFolder.Delete();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public override void SaveItem_ItemDoesNotExist_Throws()
		{
			SetState();
			Item.Senses.Add(new LexSense());    //make Lexentry dirty
			RepositoryUnderTest.SaveItem(Item);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}

	[TestFixture]
	public class LexEntryRepositoryDeleteIdTransitionTests :
			IRepositoryDeleteIdTransitionTests<LexEntry>
	{
		private string _persistedFilePath;
		private TemporaryFolder _tempFolder;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder();
			_persistedFilePath = _tempFolder.GetTemporaryFile();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			_tempFolder.Delete();
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
	public class LexEntryRepositoryDeleteAllItemsTransitionTests :
			IRepositoryDeleteAllItemsTransitionTests<LexEntry>
	{
		private string _persistedFilePath;
		private TemporaryFolder _tempFolder;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder();
			_persistedFilePath = _tempFolder.GetTemporaryFile();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			_tempFolder.Delete();
		}

		protected override void RepopulateRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}

	[TestFixture]
	public class LexEntryRepositoryCachingTests
	{
		private TemporaryFolder _tempfolder;
		private LexEntryRepository _repository;
		private ResultSet<LexEntry> firstResults;
		private bool _afterEntryDeletedCalled;
		private bool _afterEntrySavedCalled;
		private bool _afterEntryModifiedCalled;

		[SetUp]
		public void Setup()
		{
			_tempfolder = new TemporaryFolder();
			string persistedFilePath = _tempfolder.GetTemporaryFile();
			_repository = new LexEntryRepository(persistedFilePath);

			_afterEntryDeletedCalled = _afterEntrySavedCalled = _afterEntryModifiedCalled = false;
		}

		[TearDown]
		public void Teardown()
		{
			_repository.Dispose();
		}

		[Test]
		public void GetAllEntriesSortedByHeadWord_CreateItemAfterFirstCall_EntryIsReturnedAndSortedInResultSet()
		{
			LexEntry entryBeforeFirstQuery = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 1");

			_repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));

			LexEntry entryAfterFirstQuery = _repository.CreateItem();

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual(null, results[0]["Form"]);
			Assert.AreEqual("word 1", results[1]["Form"]);
		}

		private LexEntry CreateEntryWithLexicalFormBeforeFirstQuery(string writingSystem, string lexicalForm)
		{
			LexEntry entryBeforeFirstQuery = _repository.CreateItem();
			entryBeforeFirstQuery.LexicalForm.SetAlternative(writingSystem, lexicalForm);
			_repository.SaveItem(entryBeforeFirstQuery);
			return entryBeforeFirstQuery;
		}

		[Test]
		public void GetAllEntriesSortedByHeadWord_ModifyAndSaveAfterFirstCall_EntryIsModifiedAndSortedInResultSet()
		{
			LexEntry entryBeforeFirstQuery = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));

			entryBeforeFirstQuery.LexicalForm.SetAlternative("de", "word 1");
			_repository.SaveItem(entryBeforeFirstQuery);

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("word 1", results[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByHeadWord_ModifyAndSaveMultipleAfterFirstCall_EntriesModifiedAndSortedInResultSet()
		{
			List<LexEntry> entriesToModify = new List<LexEntry>();
			entriesToModify.Add(CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0"));
			entriesToModify.Add(CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 1"));

			_repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));

			entriesToModify[0].LexicalForm["de"] = "word 3";
			entriesToModify[1].LexicalForm["de"] = "word 2";
			_repository.SaveItems(entriesToModify);

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual("word 2", results[0]["Form"]);
			Assert.AreEqual("word 3", results[1]["Form"]);

		}

		[Test]
		public void GetAllEntriesSortedByHeadWord_DeleteAfterFirstCall_EntryIsDeletedInResultSet()
		{
			LexEntry entrytoBeDeleted = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteItem(entrytoBeDeleted);

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetAllEntriesSortedByHeadWord_DeleteByIdAfterFirstCall_EntryIsDeletedInResultSet()
		{
			LexEntry entrytoBeDeleted = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteItem(_repository.GetId(entrytoBeDeleted));

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetAllEntriesSortedByHeadWord_DeleteAllItemsAfterFirstCall_EntryIsDeletedInResultSet()
		{
			LexEntry entrytoBeDeleted = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteAllItems();

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByHeadword(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalForm_CreateItemAfterFirstCall_EntryIsReturnedAndSortedInResultSet()
		{
			LexEntry entryBeforeFirstQuery = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 1");

			_repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));

			LexEntry entryAfterFirstQuery = _repository.CreateItem();

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual(null, results[0]["Form"]);
			Assert.AreEqual("word 1", results[1]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalForm_ModifyAndSaveAfterFirstCall_EntryIsModifiedAndSortedInResultSet()
		{
			LexEntry entryBeforeFirstQuery = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));

			entryBeforeFirstQuery.LexicalForm.SetAlternative("de", "word 1");
			_repository.SaveItem(entryBeforeFirstQuery);

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("word 1", results[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalForm_ModifyAndSaveMultipleAfterFirstCall_EntriesModifiedAndSortedInResultSet()
		{
			List<LexEntry> entriesToModify = new List<LexEntry>();
			entriesToModify.Add(CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0"));
			entriesToModify.Add(CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 1"));

			_repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));

			entriesToModify[0].LexicalForm["de"] = "word 3";
			entriesToModify[1].LexicalForm["de"] = "word 2";
			_repository.SaveItems(entriesToModify);

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual("word 2", results[0]["Form"]);
			Assert.AreEqual("word 3", results[1]["Form"]);

		}

		[Test]
		public void GetAllEntriesSortedByLexicalForm_DeleteAfterFirstCall_EntryIsDeletedInResultSet()
		{
			LexEntry entrytoBeDeleted = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteItem(entrytoBeDeleted);

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalForm_DeleteByIdAfterFirstCall_EntryIsDeletedInResultSet()
		{
			LexEntry entrytoBeDeleted = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteItem(_repository.GetId(entrytoBeDeleted));

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetAllEntriesSortedByLexicalForm_DeleteAllItemsAfterFirstCall_EntryIsDeletedInResultSet()
		{
			LexEntry entrytoBeDeleted = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteAllItems();

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByLexicalFormOrAlternative(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}



		[Test]
		public void NotifyThatLexEntryHasBeenUpdated_LexEntry_CachesAreUpdated()
		{
			LexEntry entryToUpdate = _repository.CreateItem();
			entryToUpdate.LexicalForm.SetAlternative("de", "word 0");
			_repository.SaveItem(entryToUpdate);
			CreateCaches();
			entryToUpdate.LexicalForm.SetAlternative("de", "word 1");
			_repository.NotifyThatLexEntryHasBeenUpdated(entryToUpdate);
			WritingSystem writingSystemToMatch = new WritingSystem("de", SystemFonts.DefaultFont);
			ResultSet<LexEntry> headWordResults = _repository.GetAllEntriesSortedByHeadword(writingSystemToMatch);
			ResultSet<LexEntry> lexicalFormResults = _repository.GetAllEntriesSortedByLexicalFormOrAlternative(writingSystemToMatch);
			Assert.AreEqual("word 1", headWordResults[0]["Form"]);
			Assert.AreEqual("word 1", lexicalFormResults[0]["Form"]);
		}

		private void CreateCaches()
		{
			WritingSystem writingSystemToMatch = new WritingSystem("de", SystemFonts.DefaultFont);
			_repository.GetAllEntriesSortedByHeadword(writingSystemToMatch);
			_repository.GetAllEntriesSortedByLexicalFormOrAlternative(writingSystemToMatch);
			//_repository.GetAllEntriesSortedByDefinitionOrGloss(writingSystemToMatch);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NotifyThatLexEntryHasBeenUpdated_Null_Throws()
		{
			_repository.NotifyThatLexEntryHasBeenUpdated(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void NotifyThatLexEntryHasBeenUpdated_LexEntryDoesNotExistInRepository_Throws()
		{
			LexEntry entryToUpdate = new LexEntry();
			_repository.NotifyThatLexEntryHasBeenUpdated(entryToUpdate);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_CreateItemAfterFirstCall_EntryIsReturnedAndSortedInResultSet()
		{
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });
			LexEntry entryBeforeFirstQuery = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 1");

			_repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));

			LexEntry entryAfterFirstQuery = _repository.CreateItem();

			ResultSet<LexEntry> results = _repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual(null, results[0]["Form"]);
			Assert.AreEqual("word 1", results[1]["Form"]);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_ModifyAndSaveAfterFirstCall_EntryIsModifiedAndSortedInResultSet()
		{
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });

			LexEntry entryBeforeFirstQuery = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));

			entryBeforeFirstQuery.LexicalForm.SetAlternative("de", "word 1");
			_repository.SaveItem(entryBeforeFirstQuery);

			ResultSet<LexEntry> results = _repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("word 1", results[0]["Form"]);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_ModifyAndSaveMultipleAfterFirstCall_EntriesModifiedAndSortedInResultSet()
		{
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });

			List<LexEntry> entriesToModify = new List<LexEntry>();
			entriesToModify.Add(CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0"));
			entriesToModify.Add(CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 1"));

			_repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));

			entriesToModify[0].LexicalForm["de"] = "word 3";
			entriesToModify[1].LexicalForm["de"] = "word 2";
			_repository.SaveItems(entriesToModify);

			ResultSet<LexEntry> results = _repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual("word 2", results[0]["Form"]);
			Assert.AreEqual("word 3", results[1]["Form"]);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_DeleteAfterFirstCall_EntryIsDeletedInResultSet()
		{
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });

			LexEntry entrytoBeDeleted = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteItem(entrytoBeDeleted);

			ResultSet<LexEntry> results = _repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_DeleteByIdAfterFirstCall_EntryIsDeletedInResultSet()
		{
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });

			LexEntry entrytoBeDeleted = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteItem(_repository.GetId(entrytoBeDeleted));

			ResultSet<LexEntry> results = _repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetEntriesWithMissingFieldSortedByLexicalUnit_DeleteAllItemsAfterFirstCall_EntryIsDeletedInResultSet()
		{
			Field fieldToFill = new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "de" });

			LexEntry entrytoBeDeleted = CreateEntryWithLexicalFormBeforeFirstQuery("de", "word 0");

			_repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteAllItems();

			ResultSet<LexEntry> results = _repository.GetEntriesWithMissingFieldSortedByLexicalUnit(fieldToFill, new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		private LexEntry CreateEntryWithDefinitionBeforeFirstQuery(string writingSystem, string lexicalForm)
		{
			LexEntry entryBeforeFirstQuery = _repository.CreateItem();
			entryBeforeFirstQuery.Senses.Add(new LexSense());
			entryBeforeFirstQuery.Senses[0].Definition.SetAlternative(writingSystem, lexicalForm);
			_repository.SaveItem(entryBeforeFirstQuery);
			return entryBeforeFirstQuery;
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_CreateItemAfterFirstCall_EntryIsReturnedAndSortedInResultSet()
		{
			LexEntry entryBeforeFirstQuery = CreateEntryWithDefinitionBeforeFirstQuery("de", "word 1");

			 _repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));

			 LexEntry entryAfterFirstQuery = CreateEntryWithDefinitionBeforeFirstQuery("de", "word 2");

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual("word 1", results[0]["Form"]);
			Assert.AreEqual("word 2", results[1]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_ModifyAndSaveAfterFirstCall_EntryIsModifiedAndSortedInResultSet()
		{
			LexEntry entryBeforeFirstQuery = CreateEntryWithDefinitionBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));

			entryBeforeFirstQuery.Senses[0].Definition.SetAlternative("de", "word 1");
			_repository.SaveItem(entryBeforeFirstQuery);

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("word 1", results[0]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_ModifyAndSaveMultipleAfterFirstCall_EntriesModifiedAndSortedInResultSet()
		{
			List<LexEntry> entriesToModify = new List<LexEntry>();
			entriesToModify.Add(CreateEntryWithDefinitionBeforeFirstQuery("de", "word 0"));
			entriesToModify.Add(CreateEntryWithDefinitionBeforeFirstQuery("de", "word 1"));

			_repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));

			entriesToModify[0].Senses[0].Definition["de"] = "word 3";
			entriesToModify[1].Senses[0].Definition["de"] = "word 2";
			_repository.SaveItems(entriesToModify);

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(2, results.Count);
			Assert.AreEqual("word 2", results[0]["Form"]);
			Assert.AreEqual("word 3", results[1]["Form"]);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DeleteAfterFirstCall_EntryIsDeletedInResultSet()
		{
			LexEntry entrytoBeDeleted = CreateEntryWithDefinitionBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteItem(entrytoBeDeleted);

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DeleteByIdAfterFirstCall_EntryIsDeletedInResultSet()
		{
			LexEntry entrytoBeDeleted = CreateEntryWithDefinitionBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteItem(_repository.GetId(entrytoBeDeleted));

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetAllEntriesSortedByDefinition_DeleteAllItemsAfterFirstCall_EntryIsDeletedInResultSet()
		{
			LexEntry entrytoBeDeleted = CreateEntryWithDefinitionBeforeFirstQuery("de", "word 0");

			_repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));

			_repository.DeleteAllItems();

			ResultSet<LexEntry> results = _repository.GetAllEntriesSortedByDefinitionOrGloss(new WritingSystem("de", SystemFonts.DefaultFont));
			Assert.AreEqual(0, results.Count);
		}


	}

	[TestFixture]
	public class LexEntryRepositoryEventTests
	{
		private bool _gotEventCall;

		[SetUp]
		public void Setup()
		{
			_gotEventCall = false;
		}


		[Test]
		public void NewEntry_ByEntry_TriggersModifiedEntryAdded()
		{
			using (TemporaryFolder f = new TemporaryFolder("eventTests"))
			{
				using (LexEntryRepository r = new LexEntryRepository(f.GetPathForNewTempFile(true)))
				{
					r.AfterEntryModified += OnEvent;
					LexEntry entry = r.CreateItem();
					r.SaveItem(entry);
					Assert.IsTrue(_gotEventCall);
				}
			}
		}

		[Test]
		public void ModifiedEntry_ByEntry_TriggersModifiedEntryAdded()
		{
			using (TemporaryFolder f = new TemporaryFolder("eventTests"))
			{
				using (LexEntryRepository r = new LexEntryRepository(f.GetPathForNewTempFile(true)))
				{
					LexEntry entry = r.CreateItem();
					r.SaveItem(entry);
					 r.AfterEntryModified += OnEvent;
					entry.Senses.Add(new LexSense());
					r.SaveItem(entry);
					Assert.IsTrue(_gotEventCall);
				}
			}
		}

		[Test]
		public void DeleteEntry_ById_TriggersAfterEntryDeleted()
		{
			using (TemporaryFolder f = new TemporaryFolder("eventTests"))
			{
				using (LexEntryRepository r = new LexEntryRepository(f.GetPathForNewTempFile(true)))
				{
					r.AfterEntryDeleted +=OnEvent;

					LexEntry entry = r.CreateItem();
					r.SaveItem(entry);

					r.DeleteItem(r.GetId(entry));
					Assert.IsTrue(_gotEventCall);
				}
			}
		}

		[Test]
		public void DeleteEntry_ByEntry_TriggersAfterEntryDeleted()
		{
			using (TemporaryFolder f = new TemporaryFolder("eventTests"))
			{
				using (LexEntryRepository r = new LexEntryRepository(f.GetPathForNewTempFile(true)))
				{
					r.AfterEntryDeleted += OnEvent;

					LexEntry entry = r.CreateItem();
					r.SaveItem(entry);

					r.DeleteItem(entry);
					Assert.IsTrue(_gotEventCall);
				}
			}
		}


		void OnEvent(object sender, LexEntryRepository.EntryEventArgs e)
		{
			_gotEventCall = true;
		}
	}
}
