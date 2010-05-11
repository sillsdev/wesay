using System;
using System.Collections.Generic;
using System.Drawing;
using Exortech.NetReflector;
using NUnit.Framework;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class WritingSystemTests
	{
		[Test]
		public void NoSetupDefaultFont()
		{
			WritingSystem ws = new WritingSystem("xx", new Font("Times", 33));
			Assert.AreEqual(33, ws.Font.Size);
		}

		[Test]
		public void Construct_DefaultFont()
		{
			WritingSystem ws = new WritingSystem();
			Assert.IsNotNull(ws.Font);
		}


		[Test]
		public void SerializeOne()
		{
			// since Linux may not have Arial, we
			// need to test against the font mapping
			Font font = new Font("Arial", 99);
			WritingSystem ws = new WritingSystem("one", font);
			string s = NetReflector.Write(ws);
			string expected = "<WritingSystem><Abbreviation>one</Abbreviation><FontName>" +
							  font.Name + "</FontName><FontSize>" + font.Size +
							  "</FontSize><Id>one</Id><IsAudio>False</IsAudio><IsUnicode>True</IsUnicode><RightToLeft>False</RightToLeft><SortUsing>one</SortUsing>" +
							  "<SpellCheckingId>one</SpellCheckingId></WritingSystem>";
			Assert.AreEqual(expected, s);
		}

		[Test]
		public void DeserializeOne()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem ws =
				(WritingSystem)
				r.Read(
					"<WritingSystem><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id><SortUsing>one</SortUsing></WritingSystem>");
			// since Linux may not have Tahoma, we
			// need to test against the font mapping
			Font font = new Font("Tahoma", 99);
			Assert.IsNotNull(ws);
			Assert.AreEqual(font.Name, ws.FontName);
			Assert.AreEqual("one", ws.Id);
			Assert.AreEqual(font.Size, ws.FontSize);
		}



		[Test]
		public void Compare_fr_sortsLikeFrench()
		{
			WritingSystem writingSystem = new WritingSystem("one",
															new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = "fr";
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute
			Assert.Less(writingSystem.Compare("\u00c8dit", "Ed\u00edt"), 0);
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
			WritingSystem ws = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			foreach (string testValue in testValues)
			{
				ws.SortUsing = testValue;
				Assert.AreEqual(testValue, ws.SortUsing);
			}
		}

		[Test]
		public void Compare_en_sortsLikeEnglish()
		{
			WritingSystem writingSystem = new WritingSystem("one",
															new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = "en-US";
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute
			Assert.Greater(writingSystem.Compare("\u00c8dit", "Ed\u00edt"), 0);
		}

		[Test]
		public void Constructor_IsAudio_SetToFalse()
		{
			WritingSystem writingSystem = new WritingSystem("one",
															new Font(FontFamily.GenericSansSerif, 11));
			Assert.IsFalse(writingSystem.IsAudio);
		}

		[Test]
		public void Constructor_IsUnicode_SetToTrue()
		{
			WritingSystem writingSystem = new WritingSystem("one",
															new Font(FontFamily.GenericSansSerif, 11));
			Assert.IsTrue(writingSystem.IsUnicode);
		}

		[Test]
		public void SortUsing_customWithNoRules_sortsLikeInvariant()
		{
			WritingSystem writingSystem = new WritingSystem("one",
															new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			// hard to test because half of the system locales use the invariant table: http://blogs.msdn.com/michkap/archive/2004/12/29/344136.aspx
		}

		[Test]
		public void SortUsing_null_Id()
		{
			WritingSystem writingSystem = new WritingSystem("one",
															new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = null;
			Assert.AreEqual(writingSystem.Id, writingSystem.SortUsing);
		}

		[Test]
		public void SortUsing_HasCustomSortRulesSortUsingNotCustom_ClearsCustomSortRules()
		{
			WritingSystem writingSystem = new WritingSystem("one",
															new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = CustomSortRulesType.CustomICU.ToString();
			string rules = "&n < ng <<< Ng <<< NG";
			writingSystem.CustomSortRules = rules;
			Assert.IsNotNull(writingSystem.CustomSortRules);
			writingSystem.SortUsing = "two";
			Assert.IsNull(writingSystem.CustomSortRules);
			writingSystem.SortUsing = CustomSortRulesType.CustomICU.ToString();
			Assert.IsEmpty(writingSystem.CustomSortRules);
		}

		[Test]
		public void Id_Set_RemovesExistingRegionVariantAndScriptInfo()
		{
			WritingSystem writingSystem = new WritingSystem();
			writingSystem.GetAsPalasoWritingSystemDefinition().ISO = "Iso";
			writingSystem.GetAsPalasoWritingSystemDefinition().Region = "Region";
			writingSystem.GetAsPalasoWritingSystemDefinition().Variant = "Variant";
			writingSystem.GetAsPalasoWritingSystemDefinition().Script = "Script";
			Assert.AreEqual(writingSystem.Id, "Iso-Script-Region-Variant");
			writingSystem.Id = "Id";
			Assert.AreEqual(writingSystem.Id, "Id");
		}

		[Test]
		public void CustomSortRules_SortUsingNotCustom_NotSet()
		{
			WritingSystem writingSystem = new WritingSystem("one",
															new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = "two";
			string rules = "&n < ng <<< Ng <<< NG";
			writingSystem.CustomSortRules = rules;
			Assert.IsNull(writingSystem.CustomSortRules);
		}

		[Test]
		public void CustomSortRules_SortUsingCustomSortRules_Set()
		{
			WritingSystem writingSystem = new WritingSystem("one",
															new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = CustomSortRulesType.CustomICU.ToString();

			string rules = "&n < ng <<< Ng <<< NG";
			writingSystem.CustomSortRules = rules;
			Assert.AreEqual(rules, writingSystem.CustomSortRules);
		}

		[Test]
		public void CustomSortRules_SerializeAndDeserialize()
		{
			WritingSystem ws = new WritingSystem("one", new Font("Arial", 99));
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
			Font font1 = new Font("Arial", 12);
			WritingSystem writingSystem1 = new WritingSystem("ws", font1);
			Font font2 = new Font("Arial", 22);
			WritingSystem writingSystem2 = new WritingSystem("ws", font2);

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_SameIdSortUsingNoCustomRules_Same()
		{
			Font font1 = new Font("Arial", 12);
			WritingSystem writingSystem1 = new WritingSystem("ws", font1);
			writingSystem1.SortUsing = "th";
			Font font2 = new Font("Arial", 22);
			WritingSystem writingSystem2 = new WritingSystem("ws", font2);
			writingSystem2.SortUsing = "th";

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_SameIdSortUsingCustomRules_Same()
		{
			Font font1 = new Font("Arial", 12);
			WritingSystem writingSystem1 = new WritingSystem("ws", font1);
			writingSystem1.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem1.CustomSortRules = "A";

			Font font2 = new Font("Arial", 22);
			WritingSystem writingSystem2 = new WritingSystem("ws", font2);
			writingSystem2.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem2.CustomSortRules = "A";

			Assert.AreEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentId_Different()
		{
			Font font1 = new Font("Arial", 12);
			WritingSystem writingSystem1 = new WritingSystem("ws", font1);
			WritingSystem writingSystem2 = new WritingSystem("sw", font1);

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentSortUsing_Different()
		{
			Font font1 = new Font("Arial", 12);
			WritingSystem writingSystem1 = new WritingSystem("ws", font1);
			writingSystem1.SortUsing = "th";
			WritingSystem writingSystem2 = new WritingSystem("ws", font1);
			writingSystem2.SortUsing = "th-TH";

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentCustomSortRuleTypes_Different()
		{
			Font font1 = new Font("Arial", 12);
			WritingSystem writingSystem1 = new WritingSystem("ws", font1);
			writingSystem1.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem1.CustomSortRules = "A";

			WritingSystem writingSystem2 = new WritingSystem("ws", font1);
			writingSystem2.SortUsing = CustomSortRulesType.CustomICU.ToString();
			writingSystem2.CustomSortRules = "A";

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentCustomSortRules_Different()
		{
			Font font1 = new Font("Arial", 12);
			WritingSystem writingSystem1 = new WritingSystem("ws", font1);
			writingSystem1.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem1.CustomSortRules = "A";

			WritingSystem writingSystem2 = new WritingSystem("ws", font1);
			writingSystem2.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			writingSystem2.CustomSortRules = "A a";

			Assert.AreNotEqual(writingSystem1.GetHashCode(), writingSystem2.GetHashCode());
		}

		[Test]
		public void GetSpellCheckingId_Uninitialized_ReturnsId()
		{
			WritingSystem writingSystem = new WritingSystem();
			writingSystem.Id = "en";
			Assert.AreEqual("en", writingSystem.SpellCheckingId);
		}

		[Test]
		public void GetAbbreviation_Uninitialized_ReturnsId()
		{
			WritingSystem writingSystem = new WritingSystem();
			writingSystem.Id = "en";
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

		[Test]
		public void Font_SetNull_GetReturnsGenericSansSerif()
		{
			WritingSystem ws = new WritingSystem();
			ws.Font = null;
			Assert.AreEqual(FontFamily.GenericSansSerif, ws.Font.FontFamily);
		}

		[Test]
		public void Font_SetNull_GetFontSizeIs12()
		{
			WritingSystem ws = new WritingSystem();
			ws.Font = null;
			Assert.AreEqual(12, ws.Font.Size);
		}

		[Test]
		public void Font_SetNull_GetFontNameIsIdenticalToDefaultFontName()
		{
			WritingSystem ws = new WritingSystem();
			ws.Font = null;
			Assert.AreEqual(ws.Font.Name, ws.FontName);
		}

		[Test]
		public void Font_SetValidFont_GetReturnsFont()
		{
			WritingSystem ws = new WritingSystem();
			Font font = new Font(FontFamily.GenericSerif, 14);
			ws.Font = font;
			Assert.AreEqual(font, ws.Font);
		}

		[Test]
		public void Font_SetValidFont_GetFontNameReturnsFontName()
		{
			WritingSystem ws = new WritingSystem();
			string name = FontFamily.GenericSerif.Name;
			ws.Font = new Font(name, 14);
			Assert.AreEqual(name, ws.FontName);
		}

		[Test]
		public void Font_SetValidFont_GetFontSizeReturnsFontSize()
		{
			WritingSystem ws = new WritingSystem();
			int size = 14;
			ws.Font = new Font(FontFamily.GenericSerif.Name, size);
			Assert.AreEqual(size, ws.FontSize);
		}

		[Test]
		public void FontSize_SetSize_GetFontReturnsFontWithSize()
		{
			WritingSystem ws = new WritingSystem();
			int size = 14;
			ws.Font = new Font(FontFamily.GenericSerif.Name, 8);
			ws.FontSize = size;
			Assert.AreEqual(size, ws.Font.Size);
		}

		[Test]
		public void FontSize_SetSizeAndFontIsNull_GetFontReturnsFontWithSize()
		{
			WritingSystem ws = new WritingSystem();
			int size = 14;
			ws.Font = null;
			ws.FontSize = size;
			Assert.AreEqual(size, ws.Font.Size);
		}

		[Test]
		public void FontName_SetName_GetFontReturnsFontWithName()
		{
			WritingSystem ws = new WritingSystem();
			string name = FontFamily.GenericSerif.Name;
			ws.Font = new Font(name, 12);
			Assert.AreEqual(name, ws.Font.Name);
		}

		[Test]
		public void FontName_SetBogusName_GetFontReturnsDefaultFont()
		{
			WritingSystem ws = new WritingSystem();
			string name = "bogus";
			ws.Font = new Font(name, 12);
			Assert.AreEqual(FontFamily.GenericSansSerif.Name, ws.Font.Name);
		}
	}
}