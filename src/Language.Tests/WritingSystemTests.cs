using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using WeSay.Language;
using System.Collections;

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
			StreamWriter writer = File.CreateText(path);
			writer.Write(TestResources.WritingSystemPrefs);
			writer.Close();
		}

		[Test]
		public void NoSetupDefaultFont()
		{
			WritingSystem ws = new WritingSystem("xx", new System.Drawing.Font("Times", 33));
			Assert.AreEqual(33, ws.Font.Size);
		}


		[Test]
		public void RightFont()
		{
			WriteSampleWritingSystemFile(_path);
			_collection.Load(_path);
			WritingSystem ws = _collection.AnalysisWritingSystemDefault;
			Assert.AreEqual("ANA", ws.Id);
			Assert.AreEqual("Wingdings", ws.Font.Name);
			Assert.AreEqual(20, ws.Font.Size);

		}


		[Test]
		public void SerializeOne()
		{
			WritingSystem ws = new WritingSystem("one", new Font("Arial", 99));
			string s = NetReflector.Write(ws);
			Assert.AreEqual("<WritingSystem><FontName>Arial</FontName><FontSize>99</FontSize><Id>one</Id><RightToLeft>False</RightToLeft></WritingSystem>", s);
		}


		[Test]
		public void DeserializeOne()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(WritingSystem));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem ws = r.Read("<WritingSystem><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>") as WritingSystem ;
			Assert.IsNotNull(ws);
			Assert.AreEqual("Tahoma", ws.FontName);
			Assert.AreEqual("one", ws.Id);
			 Assert.AreEqual(99,ws.FontSize);
	   }



		[Test]
		public void SerializeCollection()
		{
			string s = MakeXmlFromCollection();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(s);

			Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection/AnalysisWritingSystemDefaultId").Count);
			Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection/VernacularWritingSystemDefaultId").Count);
			Assert.AreEqual(2, doc.SelectNodes("WritingSystemCollection/members/WritingSystem").Count);
		}

		private static string MakeXmlFromCollection()
		{
			WritingSystemCollection c = MakeSampleCollection();

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			XmlWriter writer = XmlWriter.Create(builder);
			c.Write(writer);

			return builder.ToString();
		}

		private static WritingSystemCollection MakeSampleCollection()
		{
			WritingSystemCollection c = new WritingSystemCollection();
			c.Add("one", new WritingSystem("one", new Font("Arial", 11)));
			c.Add("two", new WritingSystem("two", new Font("Times New Roman", 22)));
			c.AnalysisWritingSystemDefaultId = "one";
			c.VernacularWritingSystemDefaultId = "two";
			return c;
		}



		[Test]
		public void DeserializeCollection()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(WritingSystemCollection));
			t.Add(typeof(WritingSystem));

			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystemCollection c = r.Read(MakeXmlFromCollection()) as WritingSystemCollection;
			Assert.IsNotNull(c);
			Assert.AreEqual(2,c.Values.Count);
			Assert.AreEqual("one", c.AnalysisWritingSystemDefaultId);
		}
		[Test]
		public void DeserializeCollectionViaLoad()
		{
			MakeSampleCollection().Write(XmlWriter.Create(_path));

			WritingSystemCollection c = new WritingSystemCollection();
			c.Load(_path);
			Assert.IsNotNull(c);
			Assert.AreEqual(2, c.Values.Count);
			Assert.AreEqual("one", c.AnalysisWritingSystemDefaultId);
		}
	}
}
