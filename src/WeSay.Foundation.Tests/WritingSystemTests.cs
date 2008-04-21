using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using WeSay.Foundation.Tests;
using WeSay.Language;

namespace WeSay.Foundation.Tests
{
	[TestFixture]
	public class WritingSystemTests
	{
		private string _path;
		private WritingSystemCollection _collection;

		[SetUp]
		public void Setup()
		{
			this._path = Path.GetTempFileName();
			this._collection = new WritingSystemCollection();
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(this._path);
		}

		public static void WriteSampleWritingSystemFile(string path)
		{
			using (StreamWriter writer = File.CreateText(path))
			{
				writer.Write(TestResources.WritingSystemPrefs);
				writer.Close();
			}
		}

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
		public void MissingIdIsHandledOk()
		{
			WritingSystemCollection x = new WritingSystemCollection();
			WritingSystem ws = x["unheardof"];
			Assert.IsNotNull(ws);
			Assert.AreSame(ws, x["unheardof"], "Expected to get exactly the same one each time");
		}

		[Test]
		public void RightFont()
		{
			WriteSampleWritingSystemFile(this._path);
			this._collection.Load(this._path);
			WritingSystem ws = this._collection["PretendAnalysis"];
			Assert.AreEqual("PretendAnalysis", ws.Id);
			// since Linux may not have CourierNew, we
			// need to test against the font mapping
			Font expectedFont = new Font("Courier New", 10);
			Assert.AreEqual(expectedFont.Name, ws.Font.Name);
			Assert.AreEqual(expectedFont.Size, ws.Font.Size);
		}

		[Test]
		public void SerializeOne()
		{
			// since Linux may not have Arial, we
			// need to test against the font mapping
			Font font = new Font("Arial", 99);
			WritingSystem ws = new WritingSystem("one", font);
			string s = NetReflector.Write(ws);
			string expected = "<WritingSystem><Abbreviation>one</Abbreviation><FontName>"
							  + font.Name
							  + "</FontName><FontSize>"
							  + font.Size
							  + "</FontSize><Id>one</Id><RightToLeft>False</RightToLeft><SortUsing>one</SortUsing></WritingSystem>";
			Assert.AreEqual(expected,s);
		}

		[Test]
		public void DeserializeOne()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem ws = (WritingSystem)
							   r.Read(
									   "<WritingSystem><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id><SortUsing>one</SortUsing></WritingSystem>")
					;
			// since Linux may not have Tahoma, we
			// need to test against the font mapping
			Font font = new Font("Tahoma", 99);
			Assert.IsNotNull(ws);
			Assert.AreEqual(font.Name, ws.FontName);
			Assert.AreEqual("one", ws.Id);
			Assert.AreEqual(font.Size, ws.FontSize);
		}

		[Test]
		public void SerializeCollection()
		{
			string s = MakeXmlFromCollection();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(s);

			//            Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection/AnalysisWritingSystemDefaultId").Count);
			//            Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection/VernacularWritingSystemDefaultId").Count);
			Assert.AreEqual(2, doc.SelectNodes("WritingSystemCollection/members/WritingSystem").Count);
		}

		private static string MakeXmlFromCollection()
		{
			WritingSystemCollection c = MakeSampleCollection();

			StringBuilder builder = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(builder);
			c.Write(writer);

			return builder.ToString();
		}

		private static WritingSystemCollection MakeSampleCollection()
		{
			WritingSystemCollection c = new WritingSystemCollection();
			c.Add("one", new WritingSystem("one", new Font("Arial", 11)));
			c.Add("two", new WritingSystem("two", new Font("Times New Roman", 22)));
			return c;
		}

		[Test]
		public void WritingSystemCollection_HasUnknownVernacular()
		{
			WritingSystemCollection c = new WritingSystemCollection();
			Assert.IsNotNull(c.UnknownVernacularWritingSystem);
		}

		[Test]
		public void WritingSystemCollection_HasUnknownAnalysis()
		{
			WritingSystemCollection c = new WritingSystemCollection();
			Assert.IsNotNull(c.UnknownAnalysisWritingSystem);
		}

		[Test]
		public void DeserializeCollection()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystemCollection));
			t.Add(typeof (WritingSystem));

			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystemCollection c = r.Read(MakeXmlFromCollection()) as WritingSystemCollection;
			Assert.IsNotNull(c);
			Assert.AreEqual(2, c.Values.Count);
		}

		[Test]
		public void DeserializeCollectionViaLoad()
		{
			MakeSampleCollection().Write(XmlWriter.Create(this._path));

			WritingSystemCollection c = new WritingSystemCollection();
			c.Load(this._path);
			Assert.IsNotNull(c);
			Assert.AreEqual(2, c.Values.Count);
		}

		[Test]
		public void Compare_fr_sortsLikeFrench()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = "fr";
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute
			Assert.Less(writingSystem.Compare("\u00c8dit", "Ed\u00edt"), 0);
		}

		[Test]
		public void Compare_en_sortsLikeEnglish()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = "en-US";
			//u00c8 is Latin Capital Letter E with Grave
			//u00ed is Latin small letter i with acute
			Assert.Greater(writingSystem.Compare("\u00c8dit", "Ed\u00edt"), 0);
		}

		[Test]
		public void SortUsing_customWithNoRules_sortsLikeInvariant()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = CustomSortRulesType.CustomSimple.ToString();
			// hard to test because half of the system locales use the invariant table: http://blogs.msdn.com/michkap/archive/2004/12/29/344136.aspx
		}

		[Test]
		public void SortUsing_null_Id()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = null;
			Assert.AreEqual(writingSystem.Id, writingSystem.SortUsing);
		}

		[Test]
		public void SortUsing_HasCustomSortRulesSortUsingNotCustom_ClearsCustomSortRules()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = "custom";
			string rules = "&n < ng <<< Ng <<< NG";
			writingSystem.CustomSortRules = rules;
			writingSystem.SortUsing = "two";
			Assert.IsNull(writingSystem.CustomSortRules);
			writingSystem.SortUsing = "custom";
			Assert.IsNull(writingSystem.CustomSortRules);
		}

		[Test]
		public void CustomSortRules_SortUsingNotCustom_NotSet()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = "two";
			string rules = "&n < ng <<< Ng <<< NG";
			writingSystem.CustomSortRules = rules;
			Assert.IsNull(writingSystem.CustomSortRules);
		}

		[Test]
		public void CustomSortRules_SortUsingCustomSortRules_Set()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
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
			WritingSystem ws = (WritingSystem)
							   r.Read("<WritingSystem><CustomSortRules>test</CustomSortRules><SortUsing>CustomSimple</SortUsing><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
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
			WritingSystem ws = (WritingSystem)
							   r.Read("<WritingSystem><SortUsing>CustomSimple</SortUsing><CustomSortRules>test</CustomSortRules><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
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
			WritingSystem ws = (WritingSystem)
							   r.Read("<WritingSystem><CustomSortRules>test</CustomSortRules><SortUsing>one</SortUsing><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
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
			WritingSystem ws = (WritingSystem)
							   r.Read("<WritingSystem><SortUsing>one</SortUsing><CustomSortRules>test</CustomSortRules><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
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

	}
}