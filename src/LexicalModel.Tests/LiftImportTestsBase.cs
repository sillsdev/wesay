using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
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

	}
}
