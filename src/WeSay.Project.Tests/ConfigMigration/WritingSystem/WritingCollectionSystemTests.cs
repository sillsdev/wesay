using Exortech.NetReflector;
using NUnit.Framework;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using WeSay.Project.ConfigMigration.WritingSystem;

namespace WeSay.Project.Tests.ConfigMigration.WritingSystem
{
	[TestFixture]
	public class WritingSystemCollectionV1Tests
	{
		private string _path;
		private WritingSystemCollection_V1 _collection;

		[SetUp]
		public void Setup()
		{
			_path = Path.GetTempFileName();
			_collection = new WritingSystemCollection_V1();
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(_path);
		}

		private void CreateSampleWritingSystemFile(string path)
		{
			using (StreamWriter writer = File.CreateText(path))
			{
				writer.Write(@"<?xml version='1.0' encoding='utf-8'?>
					<WritingSystemCollection_V1>
					  <members>
						<WritingSystem>
						  <FontName>Courier New</FontName>
						  <FontSize>10</FontSize>
						  <Id>PretendAnalysis</Id>
						</WritingSystem>
						<WritingSystem>
						  <FontName>Courier New</FontName>
						  <FontSize>20</FontSize>
						  <Id>PretendVernacular</Id>
						</WritingSystem>
					  </members>
					</WritingSystemCollection_V1>");
				writer.Close();
			}
		}

		[Test]
		public void RightFont()
		{
			CreateSampleWritingSystemFile(_path);
			_collection.LoadFromLegacyWeSayFile(_path);
			WritingSystem_V1 ws = _collection["PretendAnalysis"];
			Assert.AreEqual("PretendAnalysis", ws.ISO);
			// since Linux may not have CourierNew, we
			// need to test against the font mapping
			Font expectedFont = new Font("Courier New", 10);
			Assert.AreEqual(expectedFont.Name, ws.Font.Name);
			Assert.AreEqual(expectedFont.Size, ws.Font.Size);
		}

		private static string MakeXmlFromCollection()
		{
			WritingSystemCollection_V1 c = MakeSampleCollection();

			StringBuilder builder = new StringBuilder();
			var writer = XmlWriter.Create(builder);
			writer.WriteStartDocument();
			NetReflectorWriter(writer).Write(c);
			writer.Close();
			return builder.ToString();
		}

		private static WritingSystemCollection_V1 MakeSampleCollection()
		{
			WritingSystemCollection_V1 c = new WritingSystemCollection_V1();
			c.Add("one", new WritingSystem_V1("one", new Font("Arial", 11)));
			c.Add("two", new WritingSystem_V1("two", new Font("Times New Roman", 22)));
			return c;
		}

		private static NetReflectorWriter NetReflectorWriter(XmlWriter writer)
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(WritingSystemCollection_V1));
			t.Add(typeof(WritingSystem_V1));

			return new NetReflectorWriter(writer);
		}

		private static NetReflectorReader NetReflectorReader
		{
			get
			{
				NetReflectorTypeTable t = new NetReflectorTypeTable();
				t.Add(typeof(WritingSystemCollection_V1));
				t.Add(typeof(WritingSystem_V1));

				return new NetReflectorReader(t);
			}
		}

		[Test]
		public void DeserializeCollection()
		{
			var collectionAsXml = MakeXmlFromCollection();
			WritingSystemCollection_V1 c = NetReflectorReader.Read(collectionAsXml) as WritingSystemCollection_V1;
			Assert.IsNotNull(c);
			Assert.AreEqual(2, c.Values.Count);
		}

		[Test]
		public void DeserializeCollectionViaLoad()
		{
			WritingSystemCollection_V1 collection = MakeSampleCollection();

			var writer = XmlWriter.Create(_path);
			writer.WriteStartDocument();
			NetReflectorWriter(writer).Write(collection);
			writer.Close();

			WritingSystemCollection_V1 c = new WritingSystemCollection_V1();
			c.LoadFromLegacyWeSayFile(_path);
			Assert.IsNotNull(c);
			Assert.AreEqual(2, c.Values.Count);
		}

	}
}