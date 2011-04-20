using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using WeSay.LexicalModel.Foundation;
using WeSay.LexicalModel.Tests.Foundation.Migration;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class WritingCollectionSystemTests
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
		[Category("For review")]
		public void TrimToActualTextWritingSystemIds_RemovesAudio()
		{
			Assert.Fail("The method under test here should NEVER be used again... it is not relevant in the context of migration and this class should ONLY be used in the context of migration. This test should be ignored/ removed imho. TA 4/18/2011");
			/*var WritingSystemCollection_V1 = new WritingSystemCollection_V1();
			WritingSystemCollection_V1.Add("en", new WritingSystem_V1("en", new Font("Arial", 12)));
			var audio = new WritingSystem_V1("en", new Font("Arial", 12));
			audio.IsAudio = true;
			WritingSystemCollection_V1.Add("voice", audio);

			var ids = WritingSystemCollection_V1.TrimToActualTextWritingSystemIds(new List<string>() { "en", "voice" });
			Assert.AreEqual(1, ids.Count);
			Assert.AreEqual("en", ids[0]);*/
		}

		[Test]
		[Category("For review")]
		public void MissingIdIsHandledOk()
		{
			Assert.Fail("The method under test creates a new writing system if it does not exist. It is NOT testing that a given writing system exists on creation. We should never have a need to create writing systems apart from those in the file to be migrated. This test should be ignored/ removed imho. TA 4/18/2011");
			/*WritingSystemCollection_V1 x = new WritingSystemCollection_V1();
			WritingSystem_V1 ws = x["unheardof"];
			Assert.IsNotNull(ws);
			Assert.AreSame(ws, x["unheardof"], "Expected to get exactly the same one each time");*/
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


		[Test]
		[Category("For review")]
		public void SerializeCollection()
		{
			Assert.Fail("We should never be writing out (ie serializing) a writing system collection again... it is not relevant in the context of migration and this class should ONLY be used in the context of migration. This test should be ignored/ removed imho. TA 4/18/2011");
			/*string s = MakeXmlFromCollection();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(s);

			//            Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection_V1/AnalysisWritingSystemDefaultId").Count);
			//            Assert.AreEqual(1, doc.SelectNodes("WritingSystemCollection_V1/VernacularWritingSystemDefaultId").Count);
			Assert.AreEqual(2,
							doc.SelectNodes("WritingSystemCollection_V1/members/WritingSystem").Count);*/
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

		[Test]
		[Category("For review")]
		public void WritingSystemCollection_V1_HasUnknownVernacular()
		{
			Assert.Fail("The method under test actually creates a new writing system if it does not exist. It is NOT testing that a given writing system exists on creation. Ie if I write out a new WritingSystemCollection_V1 it will NOT contain 'UnknownVernacularWritingSystem'. We should never have a need to create writing systems apart from those in the file to be migrated. This test should be ignored/ removed imho. TA 4/18/2011");
			/*WritingSystemCollection_V1 c = new WritingSystemCollection_V1();
			Assert.IsNotNull(c.UnknownVernacularWritingSystem);*/
		}

		[Test]
		[Category("For review")]
		public void WritingSystemCollection_V1_HasUnknownAnalysis()
		{
			Assert.Fail("The method under test actually creates a new writing system if it does not exist. It is NOT testing that a given writing system exists on creation. Ie if I write out a new WritingSystemCollection_V1 it will NOT contain 'UnknownAnalysisWritingSystem'. We should never have a need to create writing systems apart from those in the file to be migrated. This test should be ignored/ removed imho. TA 4/18/2011");
			/*WritingSystemCollection_V1 c = new WritingSystemCollection_V1();
			Assert.IsNotNull(c.UnknownAnalysisWritingSystem);*/
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