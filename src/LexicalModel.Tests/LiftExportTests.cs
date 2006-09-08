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
	public class LiftExportTests
	{
		private LiftExporter _exporter;
		private StringBuilder _stringBuilder;

		[SetUp]
		public void Setup()
		{
			_stringBuilder = new StringBuilder();
			PrepWriterForFragment();
		}

		private void PrepWriterForFragment()
		{
			_exporter = new LiftExporter(_stringBuilder, true);
		}

		private void PrepWriterForFullDocument()
		{
			_exporter = new LiftExporter(_stringBuilder, false);
		}


		[TearDown]
		public void TearDown()
		{

		}

		[Test]
		public void DocStructure()
		{
			PrepWriterForFullDocument();
			CheckAnswer("<?xml version=\"1.0\" encoding=\"utf-16\"?><lift />");
		}


		[Test]
		public void TwoEntries()
		{
			PrepWriterForFullDocument();
			WriteTwoEntries();
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(_stringBuilder.ToString());
			Assert.AreEqual(2, doc.SelectNodes("lift/entry").Count);
		}

		[Test]
		public void WriteToFile()
		{
			string filePath = Path.GetTempFileName();
			try
			{
				_exporter = new LiftExporter(filePath);
				WriteTwoEntries();
				XmlDocument doc = new XmlDocument();
				doc.Load(filePath);
				Assert.AreEqual(2, doc.SelectNodes("lift/entry").Count);
				}
			finally
			{
				File.Delete(filePath);
			}
		}

		private void WriteTwoEntries()
		{
			InMemoryBindingList<LexEntry> entries = new InMemoryBindingList<LexEntry>();
			LexEntry entry = entries.AddNew();
			entry.LexicalForm["red"] = "sunset";
			entry = entries.AddNew();
			entry.LexicalForm["yellow"] = "flower";

			_exporter.Add(entries);
			_exporter.End();
		}


		[Test]
		public void MultiText()
		{
			MultiText text = new MultiText();
			text["blue"] = "ocean";
			text ["red"] = "sunset";
			_exporter.Add(text);
			CheckAnswer("<form lang=\"blue\">ocean</form><form lang=\"red\">sunset</form>");
		}


		[Test]
		public void Sense()
		{
			LexSense sense = new LexSense();
			sense.Gloss["blue"] = "ocean";
			_exporter.Add(sense);
			CheckAnswer("<sense><gloss><form lang=\"blue\">ocean</form></gloss></sense>");
		}


		[Test]
		public void BlankMultiText()
		{
			_exporter.Add(new MultiText());
			CheckAnswer("");
		}

		[Test]
		public void BlankExample()
		{
			_exporter.Add(new LexExampleSentence());
			CheckAnswer("<example />");
		}

		[Test]
		public void SmallExample()
		{
			LexExampleSentence example = new LexExampleSentence();
			example.Sentence["blue"] = "ocean's eleven";
			example.Sentence["red"] = "red sunset tonight";
			_exporter.Add(example);
			CheckAnswer("<example><source><form lang=\"blue\">ocean's eleven</form><form lang=\"red\">red sunset tonight</form></source></example>");
		}

		[Test]
		public void FullExample()
		{
			LexExampleSentence example = new LexExampleSentence();
			example.Sentence["blue"] = "ocean's eleven";
			example.Sentence["red"] = "red sunset tonight";
			example.Translation["green"] = "blah blah";
			_exporter.Add(example);
			CheckAnswer("<example><source><form lang=\"blue\">ocean's eleven</form><form lang=\"red\">red sunset tonight</form></source><trans><form lang=\"green\">blah blah</form></trans></example>");
		}

		[Test]
		public void BlankSense()
		{
			_exporter.Add(new LexSense());
			CheckAnswer("<sense />");
		}

		[Test]
		public void BlankEntry()
		{
			_exporter.Add(new LexEntry());
			CheckAnswer("<entry />");
		}

		[Test]
		public void SmallEntry()
		{
			 LexEntry entry = new LexEntry();
			 entry.LexicalForm["blue"] = "ocean";
			 _exporter.Add(entry);
			CheckAnswer("<entry><form lang=\"blue\">ocean</form></entry>");
		}

		[Test]
		public void EntryWithSenses()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["blue"] = "ocean";
			LexSense sense = new LexSense();
			sense.Gloss["a"] = "aaa";
			entry.Senses.Add(sense);
			sense = new LexSense();
			sense.Gloss["b"] = "bbb";
			entry.Senses.Add(sense);
			_exporter.Add(entry);
			CheckAnswer("<entry><form lang=\"blue\">ocean</form></entry><sense><gloss><form lang=\"a\">aaa</form></gloss></sense><sense><gloss><form lang=\"b\">bbb</form></gloss></sense></entry>");
		}


		[Test]
		public void SenseWithExample()
		{
			LexSense sense = new LexSense();
			LexExampleSentence example = new LexExampleSentence();
			example.Sentence["red"] = "red sunset tonight";
			sense.ExampleSentences.Add(example);
			_exporter.Add(sense);
			CheckAnswer("<sense><example><source><form lang=\"red\">red sunset tonight</form></source></example></sense>");
		}



		private void CheckAnswer(string answer)
		{
			_exporter.End();
			Assert.AreEqual(answer,
							_stringBuilder.ToString());
		}
	}
}
