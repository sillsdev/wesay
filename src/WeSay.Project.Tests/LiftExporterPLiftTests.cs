using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

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
		private string _headwordWritingSystemId;
		private Db4oRecordListManager _recordListManager;
		string _FilePath;

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();
			Db4oLexModelHelper.InitializeForNonDbTests();

			_FilePath = System.IO.Path.GetTempFileName();
			_recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), _FilePath);
			Lexicon.Init((Db4oRecordListManager)_recordListManager);

			_outputPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

			_writingSystemIds = new List<string>(new string[] { "red","green","blue" });
			_headwordWritingSystemId = _writingSystemIds[0];

			_viewTemplate = new ViewTemplate();

			_viewTemplate.Add(new Field(LexEntry.WellKnownProperties.Citation, "LexEntry", new string[] { "blue", "red"}));
			_viewTemplate.Add(new Field(LexEntry.WellKnownProperties.LexicalUnit, "LexEntry", new string[] { "red", "green", "blue" }));
			_viewTemplate.Add(new Field(LexEntry.WellKnownProperties.BaseForm, "LexEntry", _writingSystemIds));

			Field visibleCustom = new Field("VisibleCustom", "LexEntry", _writingSystemIds, Field.MultiplicityType.ZeroOr1, "MultiText");
			visibleCustom.Visibility = WeSay.Foundation.CommonEnumerations.VisibilitySetting.Visible;
			visibleCustom.DisplayName = "VisibleCustom";
			_viewTemplate.Add(visibleCustom);

		}

		[TearDown]
		public void TearDown()
		{
			this._recordListManager.Dispose();
			System.IO.File.Delete(_FilePath);
			if (File.Exists(_outputPath))
			{
				File.Delete(_outputPath);
			}
		}

		[Test]
		public void NonHomographicEntryHasNoHomographNumber()
		{
			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e1 = MakeTestLexEntryInHeadwordWritingSystem(entries, "two");
			LexEntry e2 = MakeTestLexEntryInHeadwordWritingSystem(entries, "flower");
			LexEntry e3 = MakeTestLexEntryInHeadwordWritingSystem(entries, "one");
			Make(entries, _viewTemplate, _outputPath);
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e1.Id + "' and not(@order)]");
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e2.Id + "' and not(@order)]");
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e3.Id + "' and not(@order)]");
		}

		private void Make(IRecordList<LexEntry> entries, ViewTemplate template, string path)
		{
			LiftExporter exporter = new LiftExporter(path);
			Db4oLexEntryFinder finder = new Db4oLexEntryFinder(_recordListManager.DataSource);

			exporter.SetUpForPresentationLiftExport(template, finder);
			foreach (LexEntry entry in entries)
			{
				exporter.Add(entry);
			}
			exporter.End();
		}

		[Test]
		public void HomographicEntriesHaveHomographNumber()
		{
			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e1 = MakeTestLexEntryInHeadwordWritingSystem(entries, "sunset");
			LexEntry e2 = MakeTestLexEntryInHeadwordWritingSystem(entries, "flower");
			LexEntry e3 = MakeTestLexEntryInHeadwordWritingSystem(entries, "sunset");

			Make(entries, _viewTemplate, _outputPath);
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e1.Id + "' and @order='1']");
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e2.Id + "' and not(@order)]");
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e3.Id + "' and @order='2']");
		}

		[Test]
		public void HiddenFields_AreNotOutput()
		{
			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
			LexEntry e1 = (LexEntry)entries.AddNew();
			e1.LexicalForm["test"] = "sunset";
			e1.GetOrCreateProperty<MultiText>("color").SetAlternative(_writingSystemIds[0], "red");

			Field color = new Field("color", "LexEntry", _writingSystemIds, Field.MultiplicityType.ZeroOr1, "MultiText");
			color.DisplayName = "color";
			_viewTemplate.Add(color);

			Make(entries, _viewTemplate, _outputPath);
			AssertXPathNotNull(_outputPath, "lift/entry[@id='" + e1.Id + "']/field[@type='"+"color"+"']");

			//now make it invisible and it should disappear
			_viewTemplate.GetField("color").Enabled = false;

			Make(entries, _viewTemplate, _outputPath);
			AssertNoMatchForXPath(_outputPath, "lift/entry[@id='" + e1.Id + "']/field");

		}

		[Test]
		public void LexemeForm_DisabledWritingSystems_AreNotOutput()
		{
			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
			LexEntry entry = (LexEntry) entries.AddNew();
			entry.LexicalForm.SetAlternative(_writingSystemIds[1], "one");
			Make(entries, _viewTemplate, _outputPath);
			AssertXPathNotNull(_outputPath, "lift/entry/lexical-unit/form[text='one']");
			AssertNoMatchForXPath(_outputPath, "lift/entry/lexical-unit/form[text='red']");
		}

		[Test]
		public void WritingSystems_AreOutputInPrescribedOrder()
		{
			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
			LexEntry entry = (LexEntry) entries.AddNew();
			entry.LexicalForm.SetAlternative(_writingSystemIds[1], "one");
			entry.LexicalForm.SetAlternative(_writingSystemIds[2], "two");
			entry.LexicalForm.SetAlternative(_writingSystemIds[0], "zero");
			Make(entries, _viewTemplate, _outputPath);
			XmlNodeList forms = GetNodes("lift/entry/lexical-unit/form", _outputPath);

			Assert.AreEqual(_writingSystemIds[0], forms[0].Attributes["lang"].InnerText);
			Assert.AreEqual(_writingSystemIds[1], forms[1].Attributes["lang"].InnerText);
			Assert.AreEqual(_writingSystemIds[2], forms[2].Attributes["lang"].InnerText);
		}

		[Test]
		public void HeadWordField_CitationFieldEnabled_UsesCitationFormSettings()
		{
			_viewTemplate.GetField(LexEntry.WellKnownProperties.Citation).Enabled = true;

			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
			MakeEntry(entries);
			Make(entries, _viewTemplate, _outputPath);
			Assert.AreEqual(2, _viewTemplate.GetField(LexEntry.WellKnownProperties.Citation).WritingSystemIds.Count);
			AssertXPathNotNullWithArgs(_outputPath,
							   "lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
							   _viewTemplate.GetField(LexEntry.WellKnownProperties.Citation).WritingSystemIds[0],
							   "blueCitation");
			//should fall through to lexeme form on red
			AssertXPathNotNullWithArgs(_outputPath,
								"lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
								_viewTemplate.GetField(LexEntry.WellKnownProperties.Citation).WritingSystemIds[1],
								"redLexemeForm");

			AssertNoMatchForXPath(_outputPath,
								"lift/entry/field[@type='headword']/form[@lang='green']");
		}

		[Test]
		public void HeadWordField_CitationFieldDisabled_UsesLexemeFormSettings()
		{
			_viewTemplate.GetField(LexEntry.WellKnownProperties.Citation).Enabled = false;

			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
			MakeEntry(entries);
			Make(entries, _viewTemplate, _outputPath);
			AssertXPathNotNullWithArgs(_outputPath,
							   "lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
							   _headwordWritingSystemId, "redLexemeForm");


			//nb: it's not clear what the "correct" behavior is, if the citation for is disabled for this user
			//but a citation form does exist for this ws.

			AssertXPathNotNullWithArgs(_outputPath,
								"lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
								_writingSystemIds[1], "greenCitation");
			AssertXPathNotNullWithArgs(_outputPath,
								"lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
								_writingSystemIds[2], "blueCitation");
		}

		private void MakeEntry(IBindingList entries)
		{
			LexEntry entry = (LexEntry) entries.AddNew();
			entry.LexicalForm.SetAlternative("red", "redLexemeForm");
			entry.LexicalForm.SetAlternative("green", "greenLexemeForm");
			entry.LexicalForm.SetAlternative("blue", "blueLexemeForm");
			//leave this blank entry.CitationForm.SetAlternative("red", "redCitation");
			entry.CitationForm.SetAlternative("green", "greenCitation");
			entry.CitationForm.SetAlternative("blue", "blueCitation");
		}


		[Test]
		public void RelationEntry_Empty_NothingExported()
		{
			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
			LexEntry entry = (LexEntry) entries.AddNew();
			entry.LexicalForm.SetAlternative(_headwordWritingSystemId, "Gary");
			entry.AddRelationTarget("brother", string.Empty);

			Make(entries, _viewTemplate, _outputPath);
			CheckRelationNotOutput("brother");
		}

		[Test]
		public void RelationEntry_NotFound_NothingExported()
		{
			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
			LexEntry entry = (LexEntry) entries.AddNew();
			entry.LexicalForm.SetAlternative(_headwordWritingSystemId, "Gary");
			entry.AddRelationTarget("brother", "notGonnaFindIt");

			Make(entries, _viewTemplate, _outputPath);
			CheckRelationNotOutput("brother");
		}

		[Test]
		public void RelationEntry_Found_HeadWordExported()
		{
			IRecordList<LexEntry> entries = _recordListManager.GetListOfType<LexEntry>();
		   LexEntry targetEntry = (LexEntry) entries.AddNew();
		   targetEntry.LexicalForm.SetAlternative(_headwordWritingSystemId, "RickLexeme");
		   targetEntry.CitationForm.SetAlternative(_headwordWritingSystemId, "Rick");

		   LexEntry entry = (LexEntry) entries.AddNew();
		   entry.LexicalForm.SetAlternative(_headwordWritingSystemId, "Gary");

		   entry.AddRelationTarget("brother", targetEntry.Id);

			Make(entries, _viewTemplate, _outputPath);
			CheckRelationOutput(targetEntry, "brother");
		}

		private void CheckRelationOutput(LexEntry targetEntry, string relationName)
		{
			AssertXPathNotNullWithArgs(_outputPath,
									   "lift/entry/relation/field[@type='headword-of-target']/form[@lang='{1}']/text[text() = '{2}']",
									   relationName, _headwordWritingSystemId, targetEntry.GetHeadWordForm(_headwordWritingSystemId));
		}
		private void CheckRelationNotOutput(string relationName)
		{
			AssertNoMatchForXPathWithArgs(_outputPath,
									   "lift/entry/field[@type='{0}-relation-headword']",
									   relationName, _headwordWritingSystemId);
		}

		private LexEntry MakeTestLexEntryInHeadwordWritingSystem(IBindingList entries, string lexicalForm)
		{
			LexEntry entry = (LexEntry) entries.AddNew();
			entry.LexicalForm[_headwordWritingSystemId] = lexicalForm;
			return entry;
		}

		private XmlNodeList GetNodes(string xpath, string filePath)
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

		private void AssertXPathNotNullWithArgs( string filePath, string xpathWithArgs, params object[] args)
		{
			AssertXPathNotNull(filePath, string.Format(xpathWithArgs, args));
		}
		private void AssertNoMatchForXPathWithArgs(string filePath, string xpathWithArgs, params object[] args)
		{
			AssertNoMatchForXPath(filePath, string.Format(xpathWithArgs, args));
		}
		private void AssertXPathNotNull(string filePath, string xpath)
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