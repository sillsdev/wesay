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
			CheckAnswer("<?xml version=\"1.0\" encoding=\"utf-16\"?><lift producer=\"WeSay.1Pt0Alpha\" xmlns:flex=\"http://fieldworks.sil.org\" />");
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
		public void Sense()
		{
			LexSense sense = new LexSense();
			sense.Gloss["blue"] = "ocean";
			_exporter.Add(sense);
			CheckAnswer("<sense><gloss><form lang=\"blue\">ocean</form></gloss></sense>");
		}


		[Test]
		public void MultiTextFormWithProblematicCharacters()
		{
			LexSense sense = new LexSense();
			sense.Gloss["blue"] = "LessThan<GreaterThan>Ampersan&";
			_exporter.Add(sense);
			CheckAnswer("<sense><gloss><form lang=\"blue\">LessThan&lt;GreaterThan&gt;Ampersan&amp;</form></gloss></sense>");
		}

		[Test]
		public void AttributesWithProblematicCharacters()
		{
			LexSense sense = new LexSense();
			sense.Gloss["x\"y"] = "test";
			_exporter.Add(sense);
			CheckAnswer("<sense><gloss><form lang=\"x&quot;y\">test</form></gloss></sense>");
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
		public void BlankEntryId()
		{
			LexEntry entry = new LexEntry();
			_exporter.Add(entry);
			ShouldContain(string.Format("flex:id=\"{0}\"", entry.Guid));
		}

		[Test]
		public void DeletedEntry()
		{
			LexEntry entry = new LexEntry();
			_exporter.AddDeletedEntry(entry);
			ShouldContain(string.Format("<trait range='status' value='deleted'", entry.Guid));
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

			ShouldContain(string.Format("<sense><gloss><form lang=\"a\">aaa</form></gloss></sense><sense><gloss><form lang=\"b\">bbb</form></gloss></sense>"));
		}

		[Test]
		public void CustomMultiText()
		{
			LexSense sense = new LexSense();
			MultiText m = sense.GetOrCreateProperty<MultiText>("flubadub");
			m["zz"] = "orange";
			_exporter.Add(sense);
			_exporter.End();
			Debug.WriteLine(_stringBuilder.ToString());
			Assert.IsTrue(_stringBuilder.ToString().Contains("<flubadub><form lang=\"zz\">orange</form></flubadub>"));

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

			Assert.AreEqual("<sense><trait name=\"flub\" value=\"orange\" range=\"kindsOfFlubs\" /></sense>", _stringBuilder.ToString());
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

			Assert.AreEqual("<sense><trait name=\"flubs\" value=\"orange\" range=\"colors\" /><trait name=\"flubs\" value=\"blue\" range=\"colors\" /></sense>", _stringBuilder.ToString());
		}

		[Test]
		public void GoodGrammi()
		{
			LexSense sense = new LexSense();
			OptionRef o = sense.GetOrCreateProperty<OptionRef>("PartOfSpeech");
			o.Value = "orange";
			_exporter.Add(sense);
			 _exporter.End();
			Debug.WriteLine(_stringBuilder.ToString());
			Assert.AreEqual("<sense><grammi value=\"orange\" /></sense>", _stringBuilder.ToString());

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
