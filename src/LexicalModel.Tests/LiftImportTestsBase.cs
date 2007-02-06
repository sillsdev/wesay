using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	public abstract class  LiftImportTestsBase
	{
		protected LiftImporter _importer;
		protected XmlDocument _doc;
		protected IList<LexEntry> _entries;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();
			Db4oLexModelHelper.InitializeForNonDbTests();

			_entries = new List<LexEntry>();
			_importer = CreateImporter();
			_doc = new XmlDocument();
		}

		protected abstract  LiftImporter CreateImporter();



		[TearDown]
		public void TearDown()
		{

		}

		[Test]
		public void MissingMultiText()
		{
			_doc.LoadXml("<wrap></wrap>");
			MultiText text = new MultiText();
			_importer.ReadMultiText(_doc.SelectSingleNode("wrap"), text);
			Assert.AreEqual(0,text.Count);
		}

		[Test]
		public void EmptyGloss()
		{
			_doc.LoadXml("<sense><gloss/></sense>");
			_importer.ReadSense(_doc.SelectSingleNode("sense"));
		}


		[Test]
		public void EmptyDef()
		{
			_doc.LoadXml(@"<sense><def/></sense>");
			_importer.ReadSense(_doc.SelectSingleNode("sense"));
		}
		[Test]
		public void EmptyExample()
		{
			_doc.LoadXml("<example></example>");
			LexExampleSentence example = _importer.ReadExample(_doc.SelectSingleNode("example"));
			Assert.IsNotNull(example);
		}

		[Test]
		public void EmptySense()
		{
			_doc.LoadXml("<sense />");
			LexSense sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.IsNotNull(sense);
		}

		[Test]
		public void EmptyEntry()
		{
			Guid g = Guid.NewGuid();
			_doc.LoadXml(string.Format("<entry id=\"{0}\" />", g.ToString()));
			LexEntry entry = _importer.ReadEntry (_doc.SelectSingleNode("entry"));
			Assert.IsNotNull(entry);
			Assert.AreEqual(g.ToString(), entry.Guid.ToString());
		}

		[Test]
		public void EntryWithNonGuidIdGetsNewGuid()
		{
			//review: what else should happen?
			//should this test be moved to a "non-automated import"?
			_doc.LoadXml("<entry id=\"super duper\"></entry>");
			LexEntry entry = _importer.ReadEntry(_doc.SelectSingleNode("entry"));
			Assert.IsNotNull(entry.Guid);
		}


		[Test]
		public void GuidIdIsImported()
		{
			//review: what else should happen?
			//should this test be moved to a "non-automated import"?
			_doc.LoadXml("<entry id=\"38ed6cca-8a56-481a-9292-2bd2b435dd36\"></entry>");
			LexEntry entry = _importer.ReadEntry(_doc.SelectSingleNode("entry"));
			Assert.AreEqual("38ed6cca-8a56-481a-9292-2bd2b435dd36", entry.Guid.ToString());
		}

		[Test]
		public virtual void SenseWithGrammi()
		{
			_doc.LoadXml("<sense><grammi value=\"verb\"/></sense>");
			LexSense sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.AreEqual("verb", sense.GetOrCreateProperty<OptionRef>("PartOfSpeech").Value);

			//something not in the exiting optionsList
			_doc.LoadXml("<sense><grammi value=\"neverHeadOfThisBefore\"/></sense>");
			sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.AreEqual("neverHeadOfThisBefore", sense.GetOrCreateProperty<OptionRef>("PartOfSpeech").Value);
		}

		[Test]
		public void UnexpectedAtomicTraitDropped()
		{
			_doc.LoadXml("<sense><trait name=\"flub\" value=\"flubadub\" range=\"flubs\"/></sense>");
			LexSense sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.AreEqual(0, sense.Properties.Count);
		}

		[Test]
		public void ExpectedAtomicTrait()
		{
			_importer.ExpectedOptionTraits.Add("flub");
			_doc.LoadXml("<sense><trait name=\"flub\" value=\"flubadub\" range=\"flubs\"/></sense>");
			LexSense sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.AreEqual(1, sense.Properties.Count);
			Assert.AreEqual("flubadub", sense.GetOrCreateProperty<OptionRef>("flub").Value);
		}


		[Test]
		public void ExpectedCollectionTrait()
		{
			_importer.ExpectedOptionCollectionTraits.Add("SemanticDomain");
			_doc.LoadXml("<sense><trait name=\"SemanticDomain\" value=\"earth\" range=\"SemanticDomain\"/><trait name=\"SemanticDomain\" value=\"wind\" range=\"SemanticDomain\"/></sense>");
			LexSense sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.AreEqual(1, sense.Properties.Count);
			OptionRefCollection domains= sense.GetOrCreateProperty<OptionRefCollection>("SemanticDomain");
			Assert.AreEqual(2, domains.Keys.Count);
			Assert.AreEqual("earth", domains.Keys[0]);
			Assert.AreEqual("wind", domains.Keys[1]);
		}
		////////////////////




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
			this._doc.LoadXml(Adjust("<wrap><form lang=\"blue\">ocean</form><form lang=\"red\">sunset</form></wrap>"));
			MultiText text = new MultiText();
			_importer.ReadMultiText(this._doc.SelectSingleNode("wrap"), text);
			Assert.AreEqual("ocean", text["blue"]);
			Assert.AreEqual("sunset", text["red"]);
		}

		//subclasses can use to fix up for early versions of lift checking
		protected virtual string Adjust(string s)
		{
			return s;
		}

		[Test]
		public void FullExample()
		{
			_doc.LoadXml(Adjust("<example><source><form lang=\"blue\">ocean's eleven</form><form lang=\"red\">red sunset tonight</form></source><trans><form lang=\"green\">blah blah</form></trans></example>"));
			LexExampleSentence example = _importer.ReadExample(_doc.SelectSingleNode("example"));
			Assert.AreEqual("ocean's eleven", example.Sentence["blue"]);
			Assert.AreEqual("red sunset tonight", example.Sentence["red"]);
			Assert.AreEqual("blah blah", example.Translation["green"]);
		}

		[Test]
		public void SenseWithExample()
		{
			_doc.LoadXml(Adjust("<sense><gloss><form lang=\"blue\">sunset</form></gloss><example><source><form lang=\"red\">red sunset tonight</form></source></example></sense>"));
			LexSense sense = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			Assert.IsNotNull(sense);
			Assert.AreEqual("sunset", sense.Gloss["blue"]);
			Assert.AreEqual(1, sense.ExampleSentences.Count);
			Assert.AreEqual("red sunset tonight", ((LexExampleSentence)sense.ExampleSentences[0]).Sentence["red"]);
		}



		[Test]
		public void EntryWithSenses()
		{
			_doc.LoadXml(Adjust("<entry><form lang=\"blue\">ocean</form><sense><gloss><form lang=\"a\">aaa</form></gloss></sense><sense><gloss><form lang=\"b\">bbb</form></gloss></sense></entry>"));
			LexEntry entry = _importer.ReadEntry(_doc.SelectSingleNode("entry"));
			Assert.AreEqual("ocean", entry.LexicalForm["blue"]);
			Assert.AreEqual(2, entry.Senses.Count);
		}
	}
}
