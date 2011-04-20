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
		[Category("WritingSystemRefactor")]
		public void NoSetupDefaultFont()
		{
			Assert.Fail("cjh: Delete me. This is now a bogus test since it is testing the WritingSystem constructor taking a Font, and we removed that constructor.");
			//WritingSystem ws = new WritingSystem("xx", new Font("Times", 33));
			//Assert.AreEqual(33, WritingSystemInfo.CreateFont(ws).Size);
		}

		[Test]
		[Category("WritingSystemRefactor")]
		public void Construct_DefaultFont()
		{
			Assert.Fail("cjh: This is a bogus test since the constructor no longer sets a default Font; this test is replaced by the two tests below it");
			//WritingSystem ws = new WritingSystem();
			//Assert.IsNotNull(ws.Font);
		}

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
		[Category("WritingSystemRefactor")]
		public void SortUsingOtherLanguage_Null_SetToId()
		{
			// Not convinced that this needs to be true. Given that the sort method is known to be OtherLanguage then
			// the implementation can just ignore sort rules and use the id instead.
			Assert.Fail("cjh: See comment in test.  Not sure about the validity of this contract. Palaso returns empty string, but wesay expects the id.  TA thinks that SortUsingOtherLanguage(null) should throw.");
			var writingSystem = WritingSystemDefinition.FromLanguage("th");
			writingSystem.SortUsingOtherLanguage(null);
			Assert.AreEqual(writingSystem.Id, writingSystem.SortRules);
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
		[Category("WritingSystemRefactor")]
		public void GetHashCode_SameIdDefaultsDifferentFont_Same()
		{
			Assert.Fail("cjh: GetHashCode is only used in the old WeSay WritingSystem_V1 class... delete test");
			WritingSystemDefinition writingSystem1 = WritingSystemDefinition.FromLanguage("en");
			writingSystem1.DefaultFontName = "Arial";
			writingSystem1.DefaultFontSize = 12;
			WritingSystemDefinition writingSystem2 = WritingSystemDefinition.FromLanguage("en");
			writingSystem2.DefaultFontName = "Arial";
			writingSystem2.DefaultFontSize = 22;

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		[Category("WritingSystemRefactor")]
		public void GetHashCode_SameIdSortUsingNoCustomRules_Same()
		{
			Assert.Fail("cjh: GetHashCode is only used in the old WeSay WritingSystem_V1 class... delete test");
			WritingSystemDefinition writingSystem1 = WritingSystemDefinition.FromLanguage("en");
			writingSystem1.SortUsingOtherLanguage("th");
			WritingSystemDefinition writingSystem2 = WritingSystemDefinition.FromLanguage("en");
			writingSystem2.SortUsingOtherLanguage("th");

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		[Category("WritingSystemRefactor")]
		public void GetHashCode_SameIdSortUsingCustomRules_Same()
		{
			Assert.Fail("cjh: GetHashCode is only used in the old WeSay WritingSystem_V1 class... delete test");
			WritingSystemDefinition writingSystem1 = WritingSystemDefinition.FromLanguage("en");
			writingSystem1.SortUsingCustomSimple("A");

			WritingSystemDefinition writingSystem2 = WritingSystemDefinition.FromLanguage("en");
			writingSystem1.SortUsingCustomSimple("A");

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		[Category("WritingSystemRefactor")]
		public void GetHashCode_DifferentId_Different()
		{
			Assert.Fail("cjh: GetHashCode is only used in the old WeSay WritingSystem_V1 class... delete test");
			WritingSystemDefinition writingSystem1 = WritingSystemDefinition.FromLanguage("en");
			WritingSystemDefinition writingSystem2 = WritingSystemDefinition.FromLanguage("th");

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		[Category("WritingSystemRefactor")]
		public void GetHashCode_DifferentSortUsing_Different()
		{
			Assert.Fail("cjh: GetHashCode is only used in the old WeSay WritingSystem_V1 class... delete test");
			WritingSystemDefinition writingSystem1 = WritingSystemDefinition.FromLanguage("en");
			writingSystem1.SortUsingOtherLanguage("th");
			WritingSystemDefinition writingSystem2 = WritingSystemDefinition.FromLanguage("en");
			writingSystem1.SortUsingOtherLanguage("th-TH");

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		[Category("WritingSystemRefactor")]
		public void GetHashCode_DifferentCustomSortRuleTypes_Different()
		{
			Assert.Fail("cjh: GetHashCode is only used in the old WeSay WritingSystem_V1 class... delete test");
			var writingSystem1 = WritingSystemDefinition.FromLanguage("en");
			writingSystem1.SortUsingCustomSimple("A");

			var writingSystem2 = WritingSystemDefinition.FromLanguage("en");
			writingSystem2.SortUsingCustomICU("A");

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		[Category("WritingSystemRefactor")]
		public void GetHashCode_DifferentCustomSortRules_Different()
		{
			Assert.Fail("cjh: GetHashCode is only used in the old WeSay WritingSystem_V1 class... delete test");
			var writingSystem1 = WritingSystemDefinition.FromLanguage("en");
			writingSystem1.SortUsingCustomSimple("A");

			var writingSystem2 = WritingSystemDefinition.FromLanguage("en");
			writingSystem2.SortUsingCustomSimple("A a");

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetSpellCheckingId_Uninitialized_ReturnsId()
		{
			var writingSystem = new WritingSystemDefinition();
			writingSystem.ISO639 = "en";
			Assert.AreEqual("en", writingSystem.SpellCheckingId);
		}

		[Test]
		public void GetAbbreviation_Uninitialized_ReturnsId()
		{
			var writingSystem = new WritingSystemDefinition();
			writingSystem.ISO639 = "en";
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