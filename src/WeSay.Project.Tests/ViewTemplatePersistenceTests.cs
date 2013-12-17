using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.LexicalModel;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class viewTemplatePeristenceTests
	{
		private string _path;

		[SetUp]
		public void Setup()
		{
			_path = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(_path);
		}

		public static void WriteSampleviewTemplateFile(string path)
		{
			using (StreamWriter writer = File.CreateText(path))
			{
				writer.Write(TestResources.viewTemplate);
				writer.Close();
			}
		}

		[Test]
		public void SerializedFieldHasExpectedXml()
		{
			Field f = new Field("one", "LexEntry", new string[] {"xx", "yy"});
			string s = NetReflector.Write(f);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(s);

			Assert.AreEqual(1, doc.SelectNodes("field/fieldName").Count);
			Assert.AreEqual("one", doc.SelectNodes("field/fieldName")[0].InnerText);

			Assert.AreEqual("Visible", doc.SelectNodes("field/visibility")[0].InnerText);

			Assert.AreEqual(2, doc.SelectNodes("field/writingSystems/id").Count);
			Assert.AreEqual("xx", doc.SelectNodes("field/writingSystems/id")[0].InnerText);
			Assert.AreEqual("yy", doc.SelectNodes("field/writingSystems/id")[1].InnerText);
		}

		//        [Test]
		//        public void DeserializeOne()
		//        {
		//            NetReflectorTypeTable t = new NetReflectorTypeTable();
		//            t.Add(typeof(viewTemplate));
		//            NetReflectorReader r = new NetReflectorReader(t);
		//            viewTemplate ws = r.Read(TestResources.viewTemplate) as viewTemplate;
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

			Assert.AreEqual(2, doc.SelectNodes("viewTemplate/fields/field").Count);
		}

		private static string MakeXmlFromInventory()
		{
			ViewTemplate f = MakeSampleInventory();

			StringBuilder builder = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(builder);
			f.Write(writer);
			writer.Close();
			return builder.ToString();
		}

		private static ViewTemplate MakeSampleInventory()
		{
			ViewTemplate f = new ViewTemplate();
			f.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(),
							"LexEntry",
							new string[] {"xx", "yy"}));
			Field field = new Field(LexSense.WellKnownProperties.Gloss,
									"LexSense",
									new string[] {"zz"});
			field.Enabled = false;
			//field.Visibility = CommonEnumerations.VisibilitySetting.Invisible;
			f.Add(field);
			return f;
		}

		[Test]
		public void DeserializeInventoryFromXmlString()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (Field));
			t.Add(typeof (ViewTemplate));

			ViewTemplate f = new ViewTemplate();
			f.LoadFromString(TestResources.viewTemplate);
			CheckInventoryMatchesDefinitionInResource(f);
		}

		private static void CheckInventoryMatchesDefinitionInResource(IList<Field> f)
		{
			Assert.AreEqual(2, f.Count);
			Assert.AreEqual(Field.FieldNames.EntryLexicalForm.ToString(), f[0].FieldName);
			Assert.AreEqual(CommonEnumerations.VisibilitySetting.Visible, f[0].Visibility);
			Assert.IsTrue(f[0].Enabled);
			Assert.AreEqual(2, f[0].WritingSystemIds.Count);
			Assert.AreEqual("xx", f[0].WritingSystemIds[0]);
			Assert.AreEqual("yy", f[0].WritingSystemIds[1]);
			Assert.AreEqual(LexSense.WellKnownProperties.Gloss, f[1].FieldName);
			Assert.AreEqual(1, f[1].WritingSystemIds.Count);
			Assert.AreEqual(false, f[1].Enabled);
			Assert.AreEqual("zz", f[1].WritingSystemIds[0]);
		}

		[Test]
		public void DeserializeInvAndLoadBackIn()
		{
			XmlWriter writer = XmlWriter.Create(_path);
			MakeSampleInventory().Write(writer);
			writer.Close();
			ViewTemplate f = new ViewTemplate();
			f.Load(_path);
			Assert.IsNotNull(f);
			CheckInventoryMatchesDefinitionInResource(f);
		}
	}
}