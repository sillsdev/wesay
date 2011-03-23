using NUnit.Framework;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class WritingSystemTests
	{
		[Test]
		public void NoSetupDefaultFont()
		{
			var ws = WritingSystem.FromRFC5646("xx");
			Assert.AreEqual(33, WritingSystemInfo.CreateFont(ws).Size);
		}

		[Test]
		public void Construct_DefaultFont()
		{
			var ws = new WritingSystem();
			Assert.IsNotNull(WritingSystemInfo.CreateFont(ws));
		}

		[Test]
		public void Compare_fr_sortsLikeFrench()
		{
			var writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsingOtherLanguage("fr");
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute

			Assert.Less(writingSystem.Collator.Compare("\u00c8dit", "Ed\u00edt"), 0);
		}

		[Test]
		public void Compare_en_sortsLikeEnglish()
		{
			var writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsingOtherLanguage("en-US");
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute
			Assert.Greater(writingSystem.Collator.Compare("\u00c8dit", "Ed\u00edt"), 0);
		}

		[Test]
		public void Constructor_IsAudio_SetToFalse()
		{
			var writingSystem = WritingSystem.FromRFC5646("one");
			Assert.IsFalse(writingSystem.IsVoice);
		}

		[Test]
		public void Constructor_IsUnicode_SetToTrue()
		{
			var writingSystem = WritingSystem.FromRFC5646("one");
			Assert.IsTrue(writingSystem.IsUnicodeEncoded);
		}

		[Test, Ignore]
		public void SortUsing_CustomSimpleWithNoRules_sortsLikeInvariant()
		{
			var writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsingCustomSimple("");
			// hard to test because half of the system locales use the invariant table: http://blogs.msdn.com/michkap/archive/2004/12/29/344136.aspx
		}

		[Test]
		public void SortUsingOtherLanguage_Null_SetToId()
		{
			// Not convinced that this needs to be true. Given that the sort method is known to be OtherLanguage then
			// the implementation can just ignore sort rules and use the id instead.
			var writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsingOtherLanguage(null);
			Assert.AreEqual(writingSystem.Id, writingSystem.SortRules);
		}

		[Test]
		public void SortUsingCustomICU_WithSortRules_SetsSortRulesAndSortUsing()
		{
			const string rules = "&n < ng <<< Ng <<< NG";
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsingCustomICU(rules);
			Assert.AreEqual(rules, writingSystem.SortRules);
			Assert.AreEqual(WritingSystemDefinition.SortRulesType.CustomICU, writingSystem.SortUsing);
		}

		[Test]
		public void GetHashCode_SameIdDefaultsDifferentFont_Same()
		{
			WritingSystem writingSystem1 = WritingSystem.FromRFC5646("ws");
			writingSystem1.DefaultFontName = "Arial";
			writingSystem1.DefaultFontSize = 12;
			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem2.DefaultFontName = "Arial";
			writingSystem2.DefaultFontSize = 22;

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_SameIdSortUsingNoCustomRules_Same()
		{
			WritingSystem writingSystem1 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsingOtherLanguage("th");
			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem2.SortUsingOtherLanguage("th");

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_SameIdSortUsingCustomRules_Same()
		{
			WritingSystem writingSystem1 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsingCustomSimple("A");

			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsingCustomSimple("A");

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentId_Different()
		{
			WritingSystem writingSystem1 = WritingSystem.FromRFC5646("ws");
			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("sw");

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentSortUsing_Different()
		{
			WritingSystem writingSystem1 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsingOtherLanguage("th");
			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsingOtherLanguage("th-TH");

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentCustomSortRuleTypes_Different()
		{
			var writingSystem1 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsingCustomSimple("A");

			var writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem2.SortUsingCustomICU("A");

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentCustomSortRules_Different()
		{
			var writingSystem1 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsingCustomSimple("A");

			var writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem2.SortUsingCustomSimple("A a");

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetSpellCheckingId_Uninitialized_ReturnsId()
		{
			var writingSystem = new WritingSystem();
			writingSystem.ISO639 = "en";
			Assert.AreEqual("en", writingSystem.SpellCheckingId);
		}

		[Test]
		public void GetAbbreviation_Uninitialized_ReturnsId()
		{
			var writingSystem = new WritingSystem();
			writingSystem.ISO639 = "en";
			Assert.AreEqual("en", writingSystem.Abbreviation);
		}

		[Test]
		public void GetSpellcheckingId_SpellcheckingIdIsSet_ReturnsSpellCheckingId()
		{
			var writingSystem = new WritingSystem();
			writingSystem.SpellCheckingId = "en_US";
			Assert.AreEqual("en_US", writingSystem.SpellCheckingId);
		}

		[Test]
		public void GetAbbreviation_AbbreviationIsSet_ReturnsAbbreviation()
		{
			// Expect that this will now throw! en should preferred over eng
			var writingSystem = new WritingSystem();
			writingSystem.Abbreviation = "eng";
			Assert.AreEqual("eng", writingSystem.Abbreviation);
		}

	}
}