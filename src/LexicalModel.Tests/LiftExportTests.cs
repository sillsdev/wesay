using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LiftExportTests
	{
		private LiftExporter _exporter;
		private StringBuilder _stringBuilder;
		private Dictionary<string, string> _fieldToOptionListName = new Dictionary<string, string>();

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			WeSay.Project.WeSayWordsProject.InitializeForTests();
			_stringBuilder = new StringBuilder();
			PrepWriterForFragment();
		}

		private void PrepWriterForFragment()
		{
			_exporter = new LiftExporter(_fieldToOptionListName, _stringBuilder, true);
		}

		private void PrepWriterForFullDocument()
		{
			_exporter = new LiftExporter(_fieldToOptionListName, _stringBuilder, false);
		}


		[TearDown]
		public void TearDown()
		{

		}

		[Test]
		public void DocumentStart()
		{
			PrepWriterForFullDocument();
			//NOTE: the utf-16 here is an artifact of the xmlwriter when writing to a stringbuilder,
			//which is what we use for tests.  The file version puts out utf-8
			//CheckAnswer("<?xml version=\"1.0\" encoding=\"utf-16\"?><lift producer=\"WeSay.1Pt0Alpha\"/>");// xmlns:flex=\"http://fieldworks.sil.org\" />");
			_exporter.End();
			AssertXPathNotNull(string.Format("lift[@version='{0}']", LiftIO.Validator.LiftVersion));
			AssertXPathNotNull("lift[@producer]");
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
				_exporter = new LiftExporter(_fieldToOptionListName, filePath);
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
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry entry = entries.AddNew();
				entry.LexicalForm["red"] = "sunset";
				entry = entries.AddNew();
				entry.LexicalForm["yellow"] = "flower";

				_exporter.Add(entries);
				_exporter.End();
			}
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
		public void LexemeForm()
		{
			LexEntry e = new LexEntry();
			e.LexicalForm["xx"] = "foo";
			_exporter.Add(e);
			_exporter.End();

			AssertXPathNotNull("//lexical-unit/form[@lang='xx']");
		}

		private void AssertXPathNotNull(string xpath)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(_stringBuilder.ToString());
			XmlNode node = doc.SelectSingleNode(xpath);
			if (node == null)
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.ConformanceLevel = ConformanceLevel.Fragment;
				XmlWriter writer = XmlTextWriter.Create(Console.Out, settings);
				doc.WriteContentTo(writer);
				writer.Flush();
		   }
			Assert.IsNotNull(node);
		}

		[Test]
		public void Sense()
		{
			LexSense sense = new LexSense();
			sense.Gloss["blue"] = "ocean";
			_exporter.Add(sense);
			_exporter.End();
			AssertXPathNotNull("sense/gloss[@lang='blue']");
		}


		[Test]
		public void GlossWithProblematicCharacters()
		{
			LexSense sense = new LexSense();
			sense.Gloss["blue"] = "LessThan<GreaterThan>Ampersan&";
			_exporter.Add(sense);
			CheckAnswer("<sense><gloss lang=\"blue\">LessThan&lt;GreaterThan&gt;Ampersan&amp;</gloss></sense>");
		}

		[Test]
		public void AttributesWithProblematicCharacters()
		{
			LexSense sense = new LexSense();
			sense.Gloss["x\"y"] = "test";
			_exporter.Add(sense);
			CheckAnswer("<sense><gloss lang=\"x&quot;y\">test</gloss></sense>");
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
			CheckAnswer("<example><form lang=\"blue\">ocean's eleven</form><form lang=\"red\">red sunset tonight</form></example>");
		}

		[Test]
		public void FullExample()
		{
			LexExampleSentence example = new LexExampleSentence();
			example.Sentence["blue"] = "ocean's eleven";
			example.Sentence["red"] = "red sunset tonight";
			example.Translation["green"] = "blah blah";
			_exporter.Add(example);
			CheckAnswer("<example><form lang=\"blue\">ocean's eleven</form><form lang=\"red\">red sunset tonight</form><translation><form lang=\"green\">blah blah</form></translation></example>");
		}

		[Test]
		public void BlankSense()
		{
			_exporter.Add(new LexSense());
			CheckAnswer("<sense />");
		}

		[Test]
		public void BlankEntryId()
		{
			LexEntry entry = new LexEntry();
			_exporter.Add(entry);
			ShouldContain(string.Format("guid=\"{0}\"", entry.Guid));
		}

		[Test]
		public void DeletedEntry()
		{
			LexEntry entry = new LexEntry();
			_exporter.AddDeletedEntry(entry);
			_exporter.End();
			Assert.IsNotNull(GetStringAttributeOfTopElement("dateDeleted"));
		}

		private string GetStringAttributeOfTopElement(string attribute)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(_stringBuilder.ToString());
			return doc.FirstChild.Attributes[attribute].ToString();
		}

		private void ShouldContain(string s)
		{
			_exporter.End();
			Assert.IsTrue(
				_stringBuilder.ToString().Contains(
					s));
		}

		[Test]
		public void EntryHasDateTimes()
		{
			LexEntry entry = new LexEntry();
			_exporter.Add(entry);
			_exporter.End();
			ShouldContain(string.Format("dateCreated=\"{0}\"", entry.CreationTime.ToString("yyyy-MM-ddThh:mm:ssZ")));
			ShouldContain(string.Format("dateModified=\"{0}\"", entry.ModificationTime.ToString("yyyy-MM-ddThh:mm:ssZ")));
	   }

		[Test]
		public void DuplicateFormsGetHomographNumbers()
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm["blue"] = "ocean";
			_exporter.Add(entry);
			entry = new LexEntry();
			entry.LexicalForm["blue"] = "ocean";
			_exporter.Add(entry);
			entry = new LexEntry();
			entry.LexicalForm["blue"] = "ocean";
			_exporter.Add(entry);
			  _exporter.End();
			Debug.WriteLine(_stringBuilder.ToString());
			Assert.IsTrue(_stringBuilder.ToString().Contains("\"ocean\""));
			Assert.IsTrue(_stringBuilder.ToString().Contains("ocean_2"));
			Assert.IsTrue(_stringBuilder.ToString().Contains("ocean_3"));
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

			ShouldContain(string.Format("<sense><gloss lang=\"a\">aaa</gloss></sense><sense><gloss lang=\"b\">bbb</gloss></sense>"));
		}

		[Test]
		public void MultipleGlossesSplitIntoSeparateEntries()
		{
			LexSense sense = new LexSense();
			sense.Gloss["a"] = "aaa; bbb; ccc";
			sense.Gloss["x"] = "xx";
			_exporter.Add(sense);
			_exporter.End();
			AssertXPathNotNull("sense[count(gloss)=4]");
			AssertXPathNotNull("sense/gloss[@lang='a' and text()='aaa']");
			AssertXPathNotNull("sense/gloss[@lang='a' and text()='bbb']");
			AssertXPathNotNull("sense/gloss[@lang='a' and text()='ccc']");
			AssertXPathNotNull("sense/gloss[@lang='x' and text()='xx']");
	  }

		[Test]
		public void NoteOnSenseOutputAsNote()
		{
			LexSense sense = new LexSense();
			MultiText m = sense.GetOrCreateProperty<MultiText>(WeSayDataObject.WellKnownProperties.Note);
			m["zz"] = "orange";
			_exporter.Add(sense);
			_exporter.End();
			AssertXPathNotNull("sense/note");
			AssertXPathNotNull("sense/note/form[@lang='zz']");
			AssertXPathNotNull("sense/note/form[text()='orange']");
			AssertXPathNotNull("sense[not(field)]");
		}


		[Test]
		public void DefinitionOnSenseOutputAsDef()
		{
			LexSense sense = new LexSense();
			MultiText m = sense.GetOrCreateProperty<MultiText>(LexSense.WellKnownProperties.Definition);
			m["zz"] = "orange";
			_exporter.Add(sense);
			_exporter.End();
			AssertXPathNotNull("sense/def");
			AssertXPathNotNull("sense/def/form[@lang='zz']");
			AssertXPathNotNull("sense/def/form[text()='orange']");
			AssertXPathNotNull("sense[not(field)]");
		}

		[Test]
		public void CitationOnEntryOutputAsCitation()
		{
			LexEntry entry = new LexEntry();
			MultiText m = entry.GetOrCreateProperty<MultiText>(LexEntry.WellKnownProperties.Citation);
			m["zz"] = "orange";
			_exporter.Add(entry);
			_exporter.End();
			AssertXPathNotNull("entry/citation");
			AssertXPathNotNull("entry/citation/form[@lang='zz']");
			AssertXPathNotNull("entry/citation/form[text()='orange']");
			AssertXPathNotNull("entry[not(field)]");
		}

		[Test]
		public void CustomMultiText()
		{
			LexSense sense = new LexSense();
			MultiText m = sense.GetOrCreateProperty<MultiText>("flubadub");
			m["zz"] = "orange";
			_exporter.Add(sense);
			_exporter.End();
			AssertXPathNotNull("sense/field");
			AssertXPathNotNull("sense/field[@tag='flubadub']");
			AssertXPathNotNull("sense/field/form[@lang='zz']");
		 }

		[Test]
		public void CustomOptionRef()
		{
			_fieldToOptionListName.Add("flub", "kindsOfFlubs");
			LexSense sense = new LexSense();
			OptionRef o = sense.GetOrCreateProperty<OptionRef>("flub");
			o.Value = "orange";
			_exporter.Add(sense);
			_exporter.End();
			Debug.WriteLine(_stringBuilder.ToString());

			Assert.AreEqual("<sense><trait name=\"flub\" value=\"orange\" /></sense>", _stringBuilder.ToString());
		}

		[Test]
		public void CustomOptionRefCollection()
		{
			_fieldToOptionListName.Add("flubs", "colors");
			LexSense sense = new LexSense();
			OptionRefCollection o = sense.GetOrCreateProperty<OptionRefCollection>("flubs");
			o.Keys.AddRange(new string[] {"orange", "blue"});
			_exporter.Add(sense);
			_exporter.End();
			Debug.WriteLine(_stringBuilder.ToString());

			Assert.AreEqual("<sense><trait name=\"flubs\" value=\"orange\" /><trait name=\"flubs\" value=\"blue\" /></sense>", _stringBuilder.ToString());
		}

		[Test]
		public void GoodGrammi()
		{
			LexSense sense = new LexSense();
			OptionRef o = sense.GetOrCreateProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);
			o.Value = "orange";
			_exporter.Add(sense);
			 _exporter.End();
			AssertXPathNotNull("sense/grammatical-info[@value='orange']");
		}

		[Test]
		public void SenseWithExample()
		{
			LexSense sense = new LexSense();
			LexExampleSentence example = new LexExampleSentence();
			example.Sentence["red"] = "red sunset tonight";
			sense.ExampleSentences.Add(example);
			_exporter.Add(sense);
			CheckAnswer("<sense><example><form lang=\"red\">red sunset tonight</form></example></sense>");
		}



		private void CheckAnswer(string answer)
		{
			_exporter.End();
			Assert.AreEqual(answer,
							_stringBuilder.ToString());
		}
	}
}
