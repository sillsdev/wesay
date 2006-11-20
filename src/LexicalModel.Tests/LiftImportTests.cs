using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LiftImportTests
	{
		private LiftImporter _importer;
		private XmlDocument _doc;
		private IList<LexEntry> _entries;

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();

			_entries = new List<LexEntry>();
			_importer = new LiftImporter(_entries);
			_doc = new XmlDocument();
		}



		[TearDown]
		public void TearDown()
		{

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

				_importer = new LiftImporter(_entries);
				_importer.ReadFile(filePath);
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
			LiftImporter.ReadMultiText(this._doc.SelectSingleNode("wrap"),text);
			Assert.AreEqual("ocean", text["blue"]);
			Assert.AreEqual("sunset", text["red"]);
		}

		[Test]
		public void MissingMultiText()
		{
			_doc.LoadXml("<wrap></wrap>");
			MultiText text = new MultiText();
			LiftImporter.ReadMultiText(_doc.SelectSingleNode("wrap"),text);
			Assert.AreEqual(0,text.Count);
		}

		[Test]
		public void BlankExample()
		{
			_doc.LoadXml("<example></example>");
			LexExampleSentence example = LiftImporter.ReadExample(_doc.SelectSingleNode("example"));
			Assert.IsNotNull(example);
		}



		[Test]
		public void FullExample()
		{
			_doc.LoadXml("<example><source><form lang=\"blue\">ocean's eleven</form><form lang=\"red\">red sunset tonight</form></source><trans><form lang=\"green\">blah blah</form></trans></example>");
			LexExampleSentence example = LiftImporter.ReadExample(_doc.SelectSingleNode("example"));
			Assert.AreEqual("ocean's eleven",example.Sentence["blue"]);
			Assert.AreEqual("red sunset tonight", example.Sentence["red"]);
			Assert.AreEqual("blah blah", example.Translation["green"]);
		}

		[Test]
		public void BlankSense()
		{
			_doc.LoadXml("<sense />");
			LexSense sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.IsNotNull(sense);
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
		public void BlankEntry()
		{
			Guid g = Guid.NewGuid();
			_doc.LoadXml(string.Format("<entry id=\"{0}\" />", g.ToString()));
			LexEntry entry = _importer.ReadEntry (_doc.SelectSingleNode("entry"));
			Assert.IsNotNull(entry);
			Assert.AreEqual(g.ToString(), entry.Guid.ToString());
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
