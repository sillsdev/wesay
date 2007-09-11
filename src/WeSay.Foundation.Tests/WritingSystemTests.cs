using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using WeSay.Foundation.Tests;

namespace WeSay.Language.Tests
{
	[TestFixture]
	public class WritingSystemTests
	{
		private string _path;
		private WritingSystemCollection _collection;

		[SetUp]
		public void Setup()
		{
			_path = Path.GetTempFileName();
			_collection = new WritingSystemCollection();
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(_path);
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
			WriteSampleWritingSystemFile(_path);
			_collection.Load(_path);
			WritingSystem ws = _collection["PretendAnalysis"];
			Assert.AreEqual("PretendAnalysis", ws.Id);
			Assert.AreEqual("Wingdings", ws.Font.Name);
			Assert.AreEqual(10, ws.Font.Size);
		}

		[Test]
		public void SerializeOne()
		{
			WritingSystem ws = new WritingSystem("one", new Font("Arial", 99));
			string s = NetReflector.Write(ws);
			Assert.AreEqual(
					"<WritingSystem><Abbreviation>one</Abbreviation><FontName>Arial</FontName><FontSize>99</FontSize><Id>one</Id><RightToLeft>False</RightToLeft><SortUsing>one</SortUsing></WritingSystem>",
					s);
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
			Assert.IsNotNull(ws);
			Assert.AreEqual("Tahoma", ws.FontName);
			Assert.AreEqual("one", ws.Id);
			Assert.AreEqual(99, ws.FontSize);
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
			MakeSampleCollection().Write(XmlWriter.Create(_path));

			WritingSystemCollection c = new WritingSystemCollection();
			c.Load(_path);
			Assert.IsNotNull(c);
			Assert.AreEqual(2, c.Values.Count);
		}

		[Test]
		public void Compare_fr_sortsLikeFrench()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = "fr";
			Assert.Less(writingSystem.Compare("Èdit", "Edít"), 0);
		}

		[Test]
		public void Compare_en_sortsLikeEnglish()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = "en";
			Assert.Greater(writingSystem.Compare("Èdit", "Edít"), 0);
		}

		[Test]
		public void SortUsing_customWithNoRules_sortsLikeInvariant()
		{
			WritingSystem writingSystem = new WritingSystem("one", new Font(FontFamily.GenericSansSerif, 11));
			writingSystem.SortUsing = WritingSystem.SortUsingCustomSortRules;
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
			writingSystem.SortUsing = WritingSystem.SortUsingCustomSortRules;

			string rules = "&n < ng <<< Ng <<< NG";
			writingSystem.CustomSortRules = rules;
			Assert.AreEqual(rules, writingSystem.CustomSortRules);
		}

		[Test]
		public void CustomSortRules_SerializeAndDeserialize()
		{
			WritingSystem ws = new WritingSystem("one", new Font("Arial", 99));
			ws.SortUsing = WritingSystem.SortUsingCustomSortRules;

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
							   r.Read("<WritingSystem><CustomSortRules>test</CustomSortRules><SortUsing>custom</SortUsing><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
			Assert.IsNotNull(ws);
			Assert.AreEqual("test", ws.CustomSortRules);
			Assert.AreEqual(WritingSystem.SortUsingCustomSortRules, ws.SortUsing);
		}

		[Test]
		public void DeserializeOne_SortUsing_Before_CustomSortRules()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem ws = (WritingSystem)
							   r.Read("<WritingSystem><SortUsing>custom</SortUsing><CustomSortRules>test</CustomSortRules><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
			Assert.IsNotNull(ws);
			Assert.AreEqual("test", ws.CustomSortRules);
			Assert.AreEqual(WritingSystem.SortUsingCustomSortRules, ws.SortUsing);
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
	}
}