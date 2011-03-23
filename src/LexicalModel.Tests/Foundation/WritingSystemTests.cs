using System;
using System.Collections.Generic;
using System.Drawing;
using Exortech.NetReflector;
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
			WritingSystem ws = WritingSystem.FromRFC5646("xx");
			Assert.AreEqual(33, WritingSystemInfo.CreateFont(ws).Size);
		}

		[Test]
		public void Construct_DefaultFont()
		{
			WritingSystem ws = new WritingSystem();
			Assert.IsNotNull(WritingSystemInfo.CreateFont(ws));
		}


		/*[Test]
		//This test is obsolete as we no longer serialize writing systems for to the old WritingSystemPrefs.xml file but instead use LDML
		public void SerializeOne()
		{
			// since Linux may not have Arial, we
			// need to test against the font mapping
			Font font = new Font("Arial", 99);
			WritingSystem ws = WritingSystem.FromRFC5646("one");
			string s = NetReflector.Write(ws);
			string expected = "<WritingSystem><Abbreviation>one</Abbreviation><FontName>" +
							  font.Name + "</FontName><FontSize>" + font.Size +
							  "</FontSize><IsAudio>False</IsAudio><Id>one</Id><IsUnicode>True</IsUnicode><WindowsKeyman /><RightToLeft>False</RightToLeft><SortUsing>one</SortUsing>" +
							  "<SpellCheckingId>one</SpellCheckingId></WritingSystem>";
			Assert.AreEqual(expected, s);
		}*/

		[Test]
		public void DeserializeOne()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem ws =
				(WritingSystem)
				r.Read(
					"<WritingSystem><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id><SortUsing>one</SortUsing></WritingSystem>");
			// since Linux may not have Tahoma, we
			// need to test against the font mapping
			Font font = new Font("Tahoma", 99);
			Assert.IsNotNull(ws);
			Assert.AreEqual(font.Name, ws.DefaultFontName);
			Assert.AreEqual("", ws.Keyboard);
			Assert.AreEqual("one", ws.Id);
			Assert.AreEqual(font.Size, ws.DefaultFontSize);
		}

		[Test]
		public void Compare_fr_sortsLikeFrench()
		{
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsingOtherLanguage("fr");
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute

			Assert.Less(writingSystem.Collator.Compare("\u00c8dit", "Ed\u00edt"), 0);
		}

		[Test]
		public void SortUsing_ValueSet_IsValueReturned()
		{
			List<string> testValues = new List<string>()
										   {
											   CustomSortRulesType.CustomSimple.ToString(),
											   CustomSortRulesType.CustomICU.ToString(),
											   "NotNullOrEmpty"
										   };
			WritingSystem ws = WritingSystem.FromRFC5646("one");
			foreach (string testValue in testValues)
			{
				ws.SortUsing = testValue;
				Assert.AreEqual(testValue, ws.SortUsing);
			}
		}

		[Test]
		public void Compare_en_sortsLikeEnglish()
		{
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsing = "en-US";
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute
			Assert.Greater(writingSystem.Collator.Compare("\u00c8dit", "Ed\u00edt"), 0);
		}

		[Test]
		public void Constructor_IsAudio_SetToFalse()
		{
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			Assert.IsFalse(writingSystem.IsVoice);
		}

		[Test]
		public void Constructor_IsUnicode_SetToTrue()
		{
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			Assert.IsTrue(writingSystem.IsUnicodeEncoded);
		}

		[Test]
		public void SortUsing_customWithNoRules_sortsLikeInvariant()
		{
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			// hard to test because half of the system locales use the invariant table: http://blogs.msdn.com/michkap/archive/2004/12/29/344136.aspx
		}

		[Test]
		public void SortUsing_null_Id()
		{
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsing = null;
			Assert.AreEqual(writingSystem.Id, writingSystem.SortUsing);
		}

		[Test]
		public void SortUsing_HasCustomSortRulesSortUsingNotCustom_ClearsCustomSortRules()
		{
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsing = CustomSortRulesType.CustomICU.ToString();
			string rules = "&n < ng <<< Ng <<< NG";
			writingSystem.CustomSortRules = rules;
			Assert.IsNotNull(writingSystem.CustomSortRules);
			writingSystem.SortUsing = "two";
			Assert.IsNull(writingSystem.CustomSortRules);
			writingSystem.SortUsing = CustomSortRulesType.CustomICU.ToString();
			Assert.IsEmpty(writingSystem.CustomSortRules.ToString());
		}

		[Test]
		public void CustomSortRules_SortUsingNotCustom_NotSet()
		{
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsing = "two";
			string rules = "&n < ng <<< Ng <<< NG";
			writingSystem.CustomSortRules = rules;
			Assert.IsNull(writingSystem.CustomSortRules);
		}

		[Test]
		public void CustomSortRules_SortUsingCustomSortRules_Set()
		{
			WritingSystem writingSystem = WritingSystem.FromRFC5646("one");
			writingSystem.SortUsing = CustomSortRulesType.CustomICU.ToString();

			string rules = "&n < ng <<< Ng <<< NG";
			writingSystem.CustomSortRules = rules;
			Assert.AreEqual(rules, writingSystem.CustomSortRules);
		}

		[Test]
		public void CustomSortRules_SerializeAndDeserialize()
		{
			WritingSystem ws = WritingSystem.FromRFC5646("one");
			ws.DefaultFontName = "Arial";
			ws.DefaultFontSize = 99;
			ws.SortUsing = CustomSortRulesType.CustomICU.ToString();

			string rules = "&n < ng <<< Ng <<< NG";
			ws.CustomSortRules = rules;

			string s = NetReflector.Write(ws);

			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem wsRead = (WritingSystem) r.Read(s);
			Assert.IsNotNull(wsRead);
			Assert.AreEqual(rules, ws.CustomSortRules);
		}

		[Test]
		public void DeserializeOne_CustomSortRules_Before_SortUsing()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem ws =
				(WritingSystem)
				r.Read(
					"<WritingSystem><CustomSortRules>test</CustomSortRules><SortUsing>CustomSimple</SortUsing><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
			Assert.IsNotNull(ws);
			Assert.AreEqual("test", ws.CustomSortRules);
			Assert.AreEqual(CustomSortRulesType.CustomSimple.ToString(), ws.SortUsing);
		}

		[Test]
		public void DeserializeOne_SortUsing_Before_CustomSortRules()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem ws =
				(WritingSystem)
				r.Read(
					"<WritingSystem><SortUsing>CustomSimple</SortUsing><CustomSortRules>test</CustomSortRules><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
			Assert.IsNotNull(ws);
			Assert.AreEqual("test", ws.CustomSortRules);
			Assert.AreEqual(CustomSortRulesType.CustomSimple.ToString(), ws.SortUsing);
		}

		[Test]
		public void DeserializeOne_CustomSortRules_Before_NonCustomSortUsing()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem ws =
				(WritingSystem)
				r.Read(
					"<WritingSystem><CustomSortRules>test</CustomSortRules><SortUsing>one</SortUsing><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
			Assert.IsNotNull(ws);
			Assert.IsNull(ws.CustomSortRules);
			Assert.AreEqual("one", ws.SortUsing);
		}

		[Test]
		public void DeserializeOne_NonCustomSortUsing_Before_CustomSortRules()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem ws =
				(WritingSystem)
				r.Read(
					"<WritingSystem><SortUsing>one</SortUsing><CustomSortRules>test</CustomSortRules><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
			Assert.IsNotNull(ws);
			Assert.IsNull(ws.CustomSortRules);
			Assert.AreEqual("one", ws.SortUsing);
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
			writingSystem1.SortUsing = "th";
			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem2.SortUsing = "th";

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_SameIdSortUsingCustomRules_Same()
		{
			WritingSystem writingSystem1 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem1.CustomSortRules = "A";

			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem2.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem2.CustomSortRules = "A";

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
			writingSystem1.SortUsing = "th";
			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem2.SortUsing = "th-TH";

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentCustomSortRuleTypes_Different()
		{
			WritingSystem writingSystem1 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem1.CustomSortRules = "A";

			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem2.SortUsing = CustomSortRulesType.CustomICU.ToString();
			writingSystem2.CustomSortRules = "A";

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentCustomSortRules_Different()
		{
			WritingSystem writingSystem1 = WritingSystem.FromRFC5646("ws");
			writingSystem1.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem1.CustomSortRules = "A";

			WritingSystem writingSystem2 = WritingSystem.FromRFC5646("ws");
			writingSystem2.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem2.CustomSortRules = "A a";

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetSpellCheckingId_Uninitialized_ReturnsId()
		{
			WritingSystem writingSystem = new WritingSystem();
			writingSystem.ISO = "en";
			Assert.AreEqual("en", writingSystem.SpellCheckingId);
		}

		[Test]
		public void GetAbbreviation_Uninitialized_ReturnsId()
		{
			WritingSystem writingSystem = new WritingSystem();
			writingSystem.ISO = "en";
			Assert.AreEqual("en", writingSystem.Abbreviation);
		}

		[Test]
		public void GetSpellcheckingId_SpellcheckingIdIsSet_ReturnsSpellCheckingId()
		{
			WritingSystem writingSystem = new WritingSystem();
			writingSystem.SpellCheckingId = "en_US";
			Assert.AreEqual("en_US", writingSystem.SpellCheckingId);
		}

		[Test]
		public void GetAbbreviation_AbbreviationIsSet_ReturnsAbbreviation()
		{
			WritingSystem writingSystem = new WritingSystem();
			writingSystem.Abbreviation = "eng";
			Assert.AreEqual("eng", writingSystem.Abbreviation);
		}

	}
}