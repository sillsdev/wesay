using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using WeSay.Language;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LiftImportTestsWeSayVer1 : LiftImportTestsBase
	{
		protected override LiftImporter CreateImporter()
		{
			return new LiftImporterWeSay(_entries);
		}


		[Test]
		public void FromFile()
		{
			string filePath = Path.GetTempFileName();
			try
			{
				XmlWriter writer = XmlWriter.Create(filePath);
				writer.WriteStartDocument();
				writer.WriteStartElement("lift");

				writer.WriteStartElement("entry");
				writer.WriteStartElement("form");
				writer.WriteAttributeString("lang", "en");
				writer.WriteString("test word 1");
				writer.WriteEndElement();
				writer.WriteEndElement();

				writer.WriteStartElement("entry");
				writer.WriteStartElement("form");
				writer.WriteAttributeString("lang", "xyz");
				writer.WriteString("test word 2");
				writer.WriteEndElement();
				writer.WriteEndElement();

				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Close();

				LiftImporter.ReadFile(_entries, filePath, null);

				Assert.AreEqual(2, this._entries.Count);

				Assert.AreEqual("test word 2", this._entries[1].LexicalForm["xyz"]);
			}
			finally
			{
				File.Delete(filePath);
			}
		}

		[Test]
		public void MultiText()
		{
			this._doc.LoadXml("<wrap><form lang=\"blue\">ocean</form><form lang=\"red\">sunset</form></wrap>");
			MultiText text = new MultiText();
			_importer.ReadMultiText(this._doc.SelectSingleNode("wrap"), text);
			Assert.AreEqual("ocean", text["blue"]);
			Assert.AreEqual("sunset", text["red"]);
		}

		[Test]
		public void FullExample()
		{
			_doc.LoadXml("<example><source><form lang=\"blue\">ocean's eleven</form><form lang=\"red\">red sunset tonight</form></source><trans><form lang=\"green\">blah blah</form></trans></example>");
			LexExampleSentence example = _importer.ReadExample(_doc.SelectSingleNode("example"));
			Assert.AreEqual("ocean's eleven",example.Sentence["blue"]);
			Assert.AreEqual("red sunset tonight", example.Sentence["red"]);
			Assert.AreEqual("blah blah", example.Translation["green"]);
		}

		[Test]
		public void SenseWithExample()
		{
			_doc.LoadXml("<sense><gloss><form lang=\"blue\">sunset</form></gloss><example><source><form lang=\"red\">red sunset tonight</form></source></example></sense>");
			LexSense sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.IsNotNull(sense);
			Assert.AreEqual("sunset",sense.Gloss["blue"]);
			Assert.AreEqual(1, sense.ExampleSentences.Count);
			Assert.AreEqual("red sunset tonight", ((LexExampleSentence) sense.ExampleSentences[0]).Sentence["red"]);
		}


		[Test]
		public void EntryWithSenses()
		{
			_doc.LoadXml("<entry><form lang=\"blue\">ocean</form><sense><gloss><form lang=\"a\">aaa</form></gloss></sense><sense><gloss><form lang=\"b\">bbb</form></gloss></sense></entry>");
			LexEntry entry = _importer.ReadEntry(_doc.SelectSingleNode("entry"));
			Assert.AreEqual("ocean",entry.LexicalForm["blue"]);
			Assert.AreEqual(2, entry.Senses.Count);
		}
	}
}