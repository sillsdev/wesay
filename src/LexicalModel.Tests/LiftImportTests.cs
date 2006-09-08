using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LiftImportTests
	{
		private LiftImporter _importer;
		private XmlDocument _doc;

		[SetUp]
		public void Setup()
		{
			_importer = new LiftImporter();
			_doc = new XmlDocument();
		}



		[TearDown]
		public void TearDown()
		{

		}



		[Test]
		public void FromFile()
		{
//            string filePath = Path.GetTempFileName();
//            try
//            {
//                _importer = new LiftExporter(filePath);
//                WriteTwoEntries();
//                XmlDocument doc = new XmlDocument();
//                doc.Load(filePath);
//                Assert.AreEqual(2, doc.SelectNodes("lift/entry").Count);
//            }
//            finally
//            {
//                File.Delete(filePath);
//            }
		}

		[Test]
		public void MultiText()
		{
			this._doc.LoadXml("<wrap><form lang=\"blue\">ocean</form><form lang=\"red\">sunset</form></wrap>");
			MultiText text = new MultiText();
			_importer.ReadMultiText(this._doc.SelectSingleNode("wrap"),text);
			Assert.AreEqual("ocean", text["blue"]);
			Assert.AreEqual("sunset", text["red"]);
		}

		[Test]
		public void MissingMultiText()
		{
			_doc.LoadXml("<wrap></wrap>");
			MultiText text = new MultiText();
			_importer.ReadMultiText(_doc.SelectSingleNode("wrap"),text);
			Assert.AreEqual(0,text.Count);
		}

		[Test]
		public void BlankExample()
		{
			_doc.LoadXml("<example></example>");
			LexExampleSentence example = _importer.ReadExample(_doc.SelectSingleNode("example"));
			Assert.IsNotNull(example);
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
			_doc.LoadXml("<entry />");
			LexEntry entry = _importer.ReadEntry (_doc.SelectSingleNode("entry"));
			Assert.IsNotNull(entry);
		}

		[Test]
		public void EntryWithSenses()
		{
			_doc.LoadXml("<entry><form lang=\"blue\">ocean</form><sense><gloss><form lang=\"a\">aaa</form></gloss></sense><sense><gloss><form lang=\"b\">bbb</form></gloss></sense></entry>");
			LexEntry entry = _importer.ReadEntry(_doc.SelectSingleNode("entry"));
			Assert.AreEqual("ocean",entry.LexicalForm["blue"]);
			Assert.AreEqual(2, entry.Senses.Count);

		}

//        [Test]
//        public void SmallEntry()
//        {
//            LexEntry entry = new LexEntry();
//            entry.LexicalForm["blue"] = "ocean";
//            _importer.Add(entry);
//            CheckAnswer("<entry><form lang=\"blue\">ocean</form></entry>");
//        }
//
//        [Test]
//        public void EntryWithSenses()
//        {
//            LexEntry entry = new LexEntry();
//            entry.LexicalForm["blue"] = "ocean";
//            LexSense sense = new LexSense();
//            sense.Gloss["a"] = "aaa";
//            entry.Senses.Add(sense);
//            sense = new LexSense();
//            sense.Gloss["b"] = "bbb";
//            entry.Senses.Add(sense);
//            _importer.Add(entry);
//            CheckAnswer("<entry><form lang=\"blue\">ocean</form></entry><sense><gloss><form lang=\"a\">aaa</form></gloss></sense><sense><gloss><form lang=\"b\">bbb</form></gloss></sense>");
//        }
//
//
//        [Test]
//        public void SenseWithExample()
//        {
//            LexSense sense = new LexSense();
//            LexExampleSentence example = new LexExampleSentence();
//            example.Sentence["red"] = "red sunset tonight";
//            sense.ExampleSentences.Add(example);
//            _importer.Add(sense);
//            CheckAnswer("<sense><example><source><form lang=\"red\">red sunset tonight</form></source></example></sense>");
//        }
//
//
//
//        private void CheckAnswer(string answer)
//        {
//            _importer.End();
//            Assert.AreEqual(answer,
//                            _stringBuilder.ToString());
//        }
	}
}
