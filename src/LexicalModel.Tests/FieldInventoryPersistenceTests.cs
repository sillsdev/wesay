using System.IO;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class FieldInventoryPeristenceTests
	{

		private string _path;
		private FieldInventory _collection;

		[SetUp]
		public void Setup()
		{
			_path = Path.GetTempFileName();
			_collection = new FieldInventory();

		}


		[TearDown]
		public void TearDown()
		{
			File.Delete(_path);
		}

		public static void WriteSampleFieldInventoryFile(string path)
		{
			StreamWriter writer = File.CreateText(path);
			writer.Write(TestResources.FieldInventory);
			writer.Close();
		}




		[Test]
		public void SerializedFieldHasExpectedXml()
		{
			Field f = new Field("one", new string[] { "xx", "yy" });
			string s = NetReflector.Write(f);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(s);

			Assert.AreEqual(1, doc.SelectNodes("field/name").Count);
			Assert.AreEqual("one", doc.SelectNodes("field/name")[0].InnerText);

			Assert.AreEqual(2, doc.SelectNodes("field/writingSystems/writingSystem/id").Count);
			Assert.AreEqual("xx", doc.SelectNodes("field/writingSystems/writingSystem/id")[0].InnerText);
			Assert.AreEqual("yy", doc.SelectNodes("field/writingSystems/writingSystem/id")[1].InnerText);
		}


//        [Test]
//        public void DeserializeOne()
//        {
//            NetReflectorTypeTable t = new NetReflectorTypeTable();
//            t.Add(typeof(FieldInventory));
//            NetReflectorReader r = new NetReflectorReader(t);
//            FieldInventory ws = r.Read(TestResources.FieldInventory) as FieldInventory;
//            Assert.IsNotNull(ws);
//            Assert.AreEqual("Tahoma", ws.FontName);
//            Assert.AreEqual("one", ws.Id);
//            Assert.AreEqual(99, ws.FontSize);
//        }



		[Test]
		public void SerializedInvHasRightNumberOfFields()
		{
			string s = MakeXmlFromInventory();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(s);

			Assert.AreEqual(2, doc.SelectNodes("fieldInventory/fields/field").Count);
		}

		private static string MakeXmlFromInventory()
		{
			FieldInventory f = MakeSampleInventory();

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			XmlWriter writer = XmlWriter.Create(builder);
			f.Write(writer);

			return builder.ToString();
		}

		private static FieldInventory MakeSampleInventory()
		{
			FieldInventory f = new FieldInventory();
			f.Add(new Field("LexicalForm", new string[] { "xx", "yy" }));
			f.Add(new Field("Gloss", new string[] { "zz" }));
			return f;
		}



		[Test]
		public void DeserializeInventoryFromXmlString()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(Field));
			t.Add(typeof(FieldInventory));

			FieldInventory f = new FieldInventory();
			f.LoadFromString(TestResources.FieldInventory);
			CheckInventoryMatchesDefinitionInResource(f);
		}

		private static void CheckInventoryMatchesDefinitionInResource(FieldInventory f)
		{
			Assert.AreEqual(2,f.Count);
			Assert.AreEqual("LexicalForm", f[0].FieldName);
			Assert.AreEqual(2, f[0].WritingSystemIds.Count);
			Assert.AreEqual("xx", f[0].WritingSystemIds[0]);
			Assert.AreEqual("yy", f[0].WritingSystemIds[1]);
			Assert.AreEqual("Gloss", f[1].FieldName);
			Assert.AreEqual(1, f[1].WritingSystemIds.Count);
			Assert.AreEqual("zz", f[1].WritingSystemIds[0]);
		}

		[Test]
		public void DeserializeInvAndLoadBackIn()
		{
			MakeSampleInventory().Write(XmlWriter.Create(_path));

			FieldInventory f = new FieldInventory();
			f.Load(_path);
			Assert.IsNotNull(f);
			CheckInventoryMatchesDefinitionInResource(f);

		}
	}
}
