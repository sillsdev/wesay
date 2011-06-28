using System;
using NUnit.Framework;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class WritingSystemTests
	{

		[Test]
		public void DefaultConstructor_GetFontSize_Returns0()
		{
			var ws = new WritingSystemDefinition();
			Assert.AreEqual(0, ws.DefaultFontSize);
		}

		[Test]
		public void DefaultConstructor_GetFontName_ReturnsEmpty()
		{
			var ws = new WritingSystemDefinition();
			Assert.IsNullOrEmpty(ws.DefaultFontName);
		}

		[Test]
		public void Compare_fr_sortsLikeFrench()
		{
			var writingSystem = WritingSystemDefinition.FromLanguage("en");
			writingSystem.SortUsingOtherLanguage("fr");
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute

			Assert.Less(writingSystem.Collator.Compare("\u00c8dit", "Ed\u00edt"), 0);
		}

		[Test]
		public void Compare_en_sortsLikeEnglish()
		{
			var writingSystem = WritingSystemDefinition.FromLanguage("th");
			writingSystem.SortUsingOtherLanguage("en-US");
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute
			Assert.Greater(writingSystem.Collator.Compare("\u00c8dit", "Ed\u00edt"), 0);
		}

		[Test]
		public void Constructor_IsAudio_SetToFalse()
		{
			var writingSystem = WritingSystemDefinition.FromLanguage("th");
			Assert.IsFalse(writingSystem.IsVoice);
		}

		[Test]
		public void Constructor_IsUnicode_SetToTrue()
		{
			var writingSystem = WritingSystemDefinition.FromLanguage("th");
			Assert.IsTrue(writingSystem.IsUnicodeEncoded);
		}

		[Test, Ignore]
		public void SortUsing_CustomSimpleWithNoRules_sortsLikeInvariant()
		{
			var writingSystem = WritingSystemDefinition.FromLanguage("th");
			writingSystem.SortUsingCustomSimple("");
			// hard to test because half of the system locales use the invariant table: http://blogs.msdn.com/michkap/archive/2004/12/29/344136.aspx
		}

		[Test]
		public void SortUsingCustomICU_WithSortRules_SetsSortRulesAndSortUsing()
		{
			const string rules = "&n < ng <<< Ng <<< NG";
			WritingSystemDefinition writingSystem = WritingSystemDefinition.FromLanguage("th");
			writingSystem.SortUsingCustomICU(rules);
			Assert.AreEqual(rules, writingSystem.SortRules);
			Assert.AreEqual(WritingSystemDefinition.SortRulesType.CustomICU, writingSystem.SortUsing);
		}

		[Test]
		public void GetSpellCheckingId_Uninitialized_ReturnsId()
		{
			var writingSystem = new WritingSystemDefinition();
			writingSystem.Language = "en";
			Assert.AreEqual("en", writingSystem.SpellCheckingId);
		}

		[Test]
		public void GetAbbreviation_Uninitialized_ReturnsId()
		{
			var writingSystem = new WritingSystemDefinition();
			writingSystem.Language = "en";
			Assert.AreEqual("en", writingSystem.Abbreviation);
		}

		[Test]
		public void GetSpellcheckingId_SpellcheckingIdIsSet_ReturnsSpellCheckingId()
		{
			var writingSystem = new WritingSystemDefinition();
			writingSystem.SpellCheckingId = "en_US";
			Assert.AreEqual("en_US", writingSystem.SpellCheckingId);
		}

		[Test]
		public void GetAbbreviation_AbbreviationIsSet_ReturnsAbbreviation()
		{
			// Expect that this will now throw! en should preferred over eng
			var writingSystem = new WritingSystemDefinition();
			writingSystem.Abbreviation = "eng";
			Assert.AreEqual("eng", writingSystem.Abbreviation);
		}

	}
}