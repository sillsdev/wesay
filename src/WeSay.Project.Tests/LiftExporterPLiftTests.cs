using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;

namespace WeSay.Project.Tests
{
	/// <summary>
	/// Tests of the LiftExporter, focussed on its support for the PLIFT scenario
	/// </summary>
	[TestFixture]
	public class LiftExporterPLiftTests
	{
		private string _outputPath;
		private ViewTemplate _viewTemplate;
		private List<string> _writingSystemIds;
		private WritingSystem _headwordWritingSystem;
		private LexEntryRepository _lexEntryRepository;
		private string _FilePath;

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();

			_FilePath = Path.GetTempFileName();
			_lexEntryRepository = new LexEntryRepository(_FilePath);

			_outputPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

			_writingSystemIds = new List<string>(new string[] {"red", "green", "blue"});
			_headwordWritingSystem =
					new WritingSystem(_writingSystemIds[0],
									  new Font(FontFamily.GenericSansSerif, 10));

			_viewTemplate = new ViewTemplate();

			_viewTemplate.Add(
					new Field(LexEntry.WellKnownProperties.Citation,
							  "LexEntry",
							  new string[] {"blue", "red"}));
			_viewTemplate.Add(
					new Field(LexEntry.WellKnownProperties.LexicalUnit,
							  "LexEntry",
							  new string[] {"red", "green", "blue"}));
			_viewTemplate.Add(
					new Field(LexEntry.WellKnownProperties.BaseForm, "LexEntry", _writingSystemIds));

			Field visibleCustom =
					new Field("VisibleCustom",
							  "LexEntry",
							  _writingSystemIds,
							  Field.MultiplicityType.ZeroOr1,
							  "MultiText");
			visibleCustom.Visibility = CommonEnumerations.VisibilitySetting.Visible;
			visibleCustom.DisplayName = "VisibleCustom";
			_viewTemplate.Add(visibleCustom);
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			File.Delete(_FilePath);
			if (File.Exists(_outputPath))
			{
				File.Delete(_outputPath);
			}
		}

		[Test]
		public void NonHomographicEntryHasNoHomographNumber()
		{
			LexEntry e1 = MakeTestLexEntryInHeadwordWritingSystem("two");
			LexEntry e2 = MakeTestLexEntryInHeadwordWritingSystem("flower");
			LexEntry e3 = MakeTestLexEntryInHeadwordWritingSystem("one");
			Make(_viewTemplate, _outputPath);
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e1.Id + "' and not(@order)]");
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e2.Id + "' and not(@order)]");
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e3.Id + "' and not(@order)]");
		}

		private void Make(ViewTemplate template, string path)
		{
			PLiftExporter exporter = new PLiftExporter(path,
													   _lexEntryRepository,
													   template);

			ResultSet<LexEntry> allEntriesSortedByHeadword = this._lexEntryRepository.GetAllEntriesSortedByHeadword(this._headwordWritingSystem);
			foreach (RecordToken<LexEntry> token in allEntriesSortedByHeadword)
			{
				int homographNumber = 0;
				if ((bool)token["HasHomograph"])
				{
					homographNumber = (int)token["HomographNumber"];
				}
				exporter.Add(token.RealObject, homographNumber);
			}
			exporter.End();
		}

		[Test]
		public void HomographicEntriesHaveHomographNumber()
		{
			LexEntry e1 = MakeTestLexEntryInHeadwordWritingSystem("sunset");
			LexEntry e2 = MakeTestLexEntryInHeadwordWritingSystem("flower");
			LexEntry e3 = MakeTestLexEntryInHeadwordWritingSystem("sunset");

			Make(_viewTemplate, _outputPath);
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e1.Id + "' and @order='1']");
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e2.Id + "' and not(@order)]");
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e3.Id + "' and @order='2']");
		}

		[Test]
		public void HiddenFields_AreNotOutput()
		{
			LexEntry e1 = _lexEntryRepository.CreateItem();
			e1.LexicalForm["test"] = "sunset";
			e1.GetOrCreateProperty<MultiText>("color").SetAlternative(_writingSystemIds[0], "red");
			_lexEntryRepository.SaveItem(e1);

			Field color =
					new Field("color",
							  "LexEntry",
							  _writingSystemIds,
							  Field.MultiplicityType.ZeroOr1,
							  "MultiText");
			color.DisplayName = "color";
			_viewTemplate.Add(color);

			Make(_viewTemplate, _outputPath);
			AssertXPathNotNull(_outputPath,
							   "lift/entry[@id='" + e1.Id + "']/field[@type='" + "color" + "']");

			//now make it invisible and it should disappear
			_viewTemplate.GetField("color").Enabled = false;

			Make(_viewTemplate, _outputPath);
			AssertNoMatchForXPath(_outputPath, "lift/entry[@id='" + e1.Id + "']/field");
		}

		[Test]
		public void LexemeForm_DisabledWritingSystems_AreNotOutput()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(_writingSystemIds[1], "one");
			_lexEntryRepository.SaveItem(entry);

			Make(_viewTemplate, _outputPath);
			AssertXPathNotNull(_outputPath, "lift/entry/lexical-unit/form[text='one']");
			AssertNoMatchForXPath(_outputPath, "lift/entry/lexical-unit/form[text='red']");
		}

		[Test]
		public void WritingSystems_AreOutputInPrescribedOrder()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(_writingSystemIds[1], "one");
			entry.LexicalForm.SetAlternative(_writingSystemIds[2], "two");
			entry.LexicalForm.SetAlternative(_writingSystemIds[0], "zero");
			_lexEntryRepository.SaveItem(entry);

			Make(_viewTemplate, _outputPath);
			XmlNodeList forms = GetNodes("lift/entry/lexical-unit/form", _outputPath);

			Assert.AreEqual(_writingSystemIds[0], forms[0].Attributes["lang"].InnerText);
			Assert.AreEqual(_writingSystemIds[1], forms[1].Attributes["lang"].InnerText);
			Assert.AreEqual(_writingSystemIds[2], forms[2].Attributes["lang"].InnerText);
		}

		[Test]
		public void HeadWordField_CitationFieldEnabled_UsesCitationFormSettings()
		{
			_viewTemplate.GetField(LexEntry.WellKnownProperties.Citation).Enabled = true;

			MakeEntry();
			Make(_viewTemplate, _outputPath);
			Assert.AreEqual(2,
							_viewTemplate.GetField(LexEntry.WellKnownProperties.Citation).
									WritingSystemIds.Count);
			AssertXPathNotNullWithArgs(_outputPath,
									   "lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
									   _viewTemplate.GetField(LexEntry.WellKnownProperties.Citation)
											   .WritingSystemIds[0],
									   "blueCitation");
			//should fall through to lexeme form on red
			AssertXPathNotNullWithArgs(_outputPath,
									   "lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
									   _viewTemplate.GetField(LexEntry.WellKnownProperties.Citation)
											   .WritingSystemIds[1],
									   "redLexemeForm");

			AssertNoMatchForXPath(_outputPath,
								  "lift/entry/field[@type='headword']/form[@lang='green']");
		}

		[Test]
		public void HeadWordField_CitationFieldDisabled_UsesLexemeFormSettings()
		{
			_viewTemplate.GetField(LexEntry.WellKnownProperties.Citation).Enabled = false;

			MakeEntry();
			Make(_viewTemplate, _outputPath);
			AssertXPathNotNullWithArgs(_outputPath,
									   "lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
									   _headwordWritingSystem.Id,
									   "redLexemeForm");

			//nb: it's not clear what the "correct" behavior is, if the citation for is disabled for this user
			//but a citation form does exist for this ws.

			AssertXPathNotNullWithArgs(_outputPath,
									   "lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
									   _writingSystemIds[1],
									   "greenCitation");
			AssertXPathNotNullWithArgs(_outputPath,
									   "lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
									   _writingSystemIds[2],
									   "blueCitation");
		}

		private void MakeEntry()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative("red", "redLexemeForm");
			entry.LexicalForm.SetAlternative("green", "greenLexemeForm");
			entry.LexicalForm.SetAlternative("blue", "blueLexemeForm");
			//leave this blank entry.CitationForm.SetAlternative("red", "redCitation");
			entry.CitationForm.SetAlternative("green", "greenCitation");
			entry.CitationForm.SetAlternative("blue", "blueCitation");
			_lexEntryRepository.SaveItem(entry);
		}

		[Test]
		public void RelationEntry_Empty_NothingExported()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "Gary");
			entry.AddRelationTarget("brother", string.Empty);
			_lexEntryRepository.SaveItem(entry);

			Make(_viewTemplate, _outputPath);
			CheckRelationNotOutput("brother");
		}

		[Test]
		public void RelationEntry_NotFound_NothingExported()
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "Gary");
			entry.AddRelationTarget("brother", "notGonnaFindIt");
			_lexEntryRepository.SaveItem(entry);

			Make(_viewTemplate, _outputPath);
			CheckRelationNotOutput("brother");
		}

		[Test]
		public void RelationEntry_Found_HeadWordExported()
		{
			LexEntry targetEntry = _lexEntryRepository.CreateItem();

			targetEntry.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "RickLexeme");
			targetEntry.CitationForm.SetAlternative(_headwordWritingSystem.Id, "Rick");
			_lexEntryRepository.SaveItem(targetEntry);

			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm.SetAlternative(_headwordWritingSystem.Id, "Gary");
			_lexEntryRepository.SaveItem(entry);

			entry.AddRelationTarget("brother", targetEntry.Id);

			Make(_viewTemplate, _outputPath);
			CheckRelationOutput(targetEntry, "brother");
		}

		private void CheckRelationOutput(LexEntry targetEntry, string relationName)
		{
			AssertXPathNotNullWithArgs(_outputPath,
									   "lift/entry/relation/field[@type='headword-of-target']/form[@lang='{1}']/text[text() = '{2}']",
									   relationName,
									   _headwordWritingSystem.Id,
									   targetEntry.GetHeadWordForm(_headwordWritingSystem.Id));
		}

		private void CheckRelationNotOutput(string relationName)
		{
			AssertNoMatchForXPathWithArgs(_outputPath,
										  "lift/entry/field[@type='{0}-relation-headword']",
										  relationName,
										  _headwordWritingSystem.Id);
		}

		private LexEntry MakeTestLexEntryInHeadwordWritingSystem(string lexicalForm)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();
			entry.LexicalForm[_headwordWritingSystem.Id] = lexicalForm;
			_lexEntryRepository.SaveItem(entry);
			return entry;
		}

		private static XmlNodeList GetNodes(string xpath, string filePath)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(filePath);
			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
				Console.WriteLine(File.ReadAllText(filePath));
			}
			XmlNodeList nodes = doc.SelectNodes(xpath);
			if (nodes == null || nodes.Count == 0)
			{
				PrintNodeToConsole(doc);
				Assert.Fail("Could not match " + xpath);
			}
			return nodes;
		}

		private static void AssertXPathNotNullWithArgs(string filePath,
													   string xpathWithArgs,
													   params object[] args)
		{
			AssertXPathNotNull(filePath, string.Format(xpathWithArgs, args));
		}

		private static void AssertNoMatchForXPathWithArgs(string filePath,
														  string xpathWithArgs,
														  params object[] args)
		{
			AssertNoMatchForXPath(filePath, string.Format(xpathWithArgs, args));
		}

		private static void AssertXPathNotNull(string filePath, string xpath)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(filePath);
			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
				Console.WriteLine(File.ReadAllText(filePath));
			}
			XmlNode node = doc.SelectSingleNode(xpath);
			if (node == null)
			{
				Console.WriteLine("Could not match " + xpath);
				PrintNodeToConsole(doc);
			}
			Assert.IsNotNull(node);
		}

		public static void AssertNoMatchForXPath(string filePath, string xpath)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(filePath);
			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
				Console.WriteLine(File.ReadAllText(filePath));
			}
			XmlNode node = doc.SelectSingleNode(xpath);
			if (node != null)
			{
				Console.WriteLine("Unexpected match for " + xpath);
				PrintNodeToConsole(node);
			}
			Assert.IsNull(node);
		}

		private static void PrintNodeToConsole(XmlNode node)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			XmlWriter writer = XmlTextWriter.Create(Console.Out, settings);
			node.WriteContentTo(writer);
			writer.Flush();
		}
	}
}