using System;
using System.Collections.Generic;
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
		private LiftExporter _exporter;
		private string _outputPath;
		private ViewTemplate _viewTemplate;
		private List<string> _writingSystemIds;
		private string _headwordWritingSystemId;

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();
			Db4oLexModelHelper.InitializeForNonDbTests();
			_outputPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

			_writingSystemIds = new List<string>(new string[] { "first","second","third" });
			_headwordWritingSystemId = _writingSystemIds[0];

			_viewTemplate = new ViewTemplate();

			_viewTemplate.Add(new Field(LexEntry.WellKnownProperties.LexicalUnit, "LexEntry", _writingSystemIds));
			_viewTemplate.Add(new Field(LexEntry.WellKnownProperties.BaseForm, "LexEntry", _writingSystemIds));

			Field visibleCustom = new Field("VisibleCustom", "LexEntry", _writingSystemIds, Field.MultiplicityType.ZeroOr1, "MultiText");
			visibleCustom.Visibility = WeSay.Foundation.CommonEnumerations.VisibilitySetting.Visible;
			visibleCustom.DisplayName = "VisibleCustom";
			_viewTemplate.Add(visibleCustom);

		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_outputPath))
			{
				File.Delete(_outputPath);
			}
		}

		[Test]
		public void HomographicEntriesHaveHomographNumber()
		{
			DummyHomographCalculator.NumberToGiveAsHomograph = 46;

			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry e1 = MakeTestLexEntry(entries, "two");
				MakeTestLexEntry(entries, "flower");
				LexEntry e2 =MakeTestLexEntry(entries, "one");
				Make(entries, _viewTemplate, _outputPath);
				AssertXPathNotNull("lift/entry[@id='"+e1.Id+"' and @order='46']", _outputPath);
				AssertXPathNotNull("lift/entry[@id='"+e2.Id+"' and @order='46']", _outputPath);
			}
		}

		private void Make(InMemoryRecordList<LexEntry> entries, ViewTemplate template, string path)
		{
			LiftExporter exporter = new LiftExporter(path);
			exporter.SetUpForPresentationLiftExport(template, new DummyHomographCalculator(), new InMemoryLexEntryFinder(entries));
			foreach (LexEntry entry in entries)
			{
				exporter.Add(entry);
			}
			exporter.End();
		}

		[Test]
		public void NonHomographicEntryHasNoHomographNumber()
		{
			DummyHomographCalculator.NumberToGiveAsHomograph = 0;

			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				MakeTestLexEntry(entries, "sunset");
				LexEntry e1 = MakeTestLexEntry(entries, "flower");
				MakeTestLexEntry(entries, "sunset");

				Make(entries, _viewTemplate, _outputPath);
				AssertXPathNotNull("lift/entry[@id='" + e1.Id + "' and not(@order)]", _outputPath);
			}
		}

		[Test]
		public void HiddenFields_AreNotOutput()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry e1 = MakeTestLexEntry(entries, "sunset");
				e1.GetOrCreateProperty<MultiText>("color").SetAlternative(_writingSystemIds[0], "red");

				Field color = new Field("color", "LexEntry", _writingSystemIds, Field.MultiplicityType.ZeroOr1, "MultiText");
				color.DisplayName = "color";
				_viewTemplate.Add(color);

				Make(entries, _viewTemplate, _outputPath);
				AssertXPathNotNull("lift/entry[@id='" + e1.Id + "']/field[@tag='"+"color"+"']", _outputPath);

				//now make it invisible and it should disappear
				_viewTemplate.GetField("color").Enabled = false;

				Make(entries, _viewTemplate, _outputPath);
				AssertNoMatchForXPath("lift/entry[@id='" + e1.Id + "']/field", _outputPath);

			}
		}

		[Test]
		public void LexemeForm_DisabledWritingSystems_AreNotOutput()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry entry = entries.AddNew();
				entry.LexicalForm.SetAlternative(_writingSystemIds[1], "one");
				Make(entries, _viewTemplate, _outputPath);
				AssertXPathNotNull("lift/entry/lexical-unit/form[text='one']", _outputPath);
				AssertNoMatchForXPath("lift/entry/lexical-unit/form[text='red']", _outputPath);
			}
		}

		[Test]
		public void WritingSystems_AreOutputInPrescribedOrder()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry entry = entries.AddNew();
				entry.LexicalForm.SetAlternative(_writingSystemIds[1], "one");
				entry.LexicalForm.SetAlternative(_writingSystemIds[2], "two");
				entry.LexicalForm.SetAlternative(_writingSystemIds[0], "zero");
				Make(entries, _viewTemplate, _outputPath);
				XmlNodeList forms = GetNodes("lift/entry/lexical-unit/form", _outputPath);

				Assert.AreEqual(_writingSystemIds[0], forms[0].Attributes["lang"].InnerText);
				Assert.AreEqual(_writingSystemIds[1], forms[1].Attributes["lang"].InnerText);
				Assert.AreEqual(_writingSystemIds[2], forms[2].Attributes["lang"].InnerText);
			}
		}


		[Test]
		public void HeadWordField_Exported()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry entry = entries.AddNew();
				entry.LexicalForm.SetAlternative(_headwordWritingSystemId, "thelexeme");
				entry.CitationForm.SetAlternative(_headwordWritingSystemId, "thecitation");


				Make(entries, _viewTemplate, _outputPath);
				AssertXPathNotNullWithArgs(_outputPath,
								   "lift/entry/field[@tag='headword']/form[@lang='{0}']/text[text() = '{1}']",
								   _headwordWritingSystemId, "thecitation");
			}
		}


		[Test]
		public void RelationEntry_Empty_NothingExported()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry entry = entries.AddNew();
				entry.LexicalForm.SetAlternative(_headwordWritingSystemId, "Gary");
				entry.AddRelationTarget("brother", string.Empty);

				Make(entries, _viewTemplate, _outputPath);
				CheckRelationNotOutput("brother");
			}
		}

		[Test]
		public void RelationEntry_NotFound_NothingExported()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry entry = entries.AddNew();
				entry.LexicalForm.SetAlternative(_headwordWritingSystemId, "Gary");
				entry.AddRelationTarget("brother", "notGonnaFindIt");

				Make(entries, _viewTemplate, _outputPath);
				CheckRelationNotOutput("brother");
			}
		}

		[Test]
		public void RelationEntry_Found_HeadWordExported()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
			   LexEntry targetEntry = entries.AddNew();
			   targetEntry.LexicalForm.SetAlternative(_headwordWritingSystemId, "RickLexeme");
			   targetEntry.CitationForm.SetAlternative(_headwordWritingSystemId, "Rick");

			   LexEntry entry = entries.AddNew();
			   entry.LexicalForm.SetAlternative(_headwordWritingSystemId, "Gary");

			   entry.AddRelationTarget("brother", targetEntry.Id);

				Make(entries, _viewTemplate, _outputPath);
				CheckRelationOutput(targetEntry, "brother");
			}
		}

		private void CheckRelationOutput(LexEntry targetEntry, string relationName)
		{
			AssertXPathNotNullWithArgs(_outputPath,
									   "lift/entry/field[@tag='{0}-relation-headword']/form[@lang='{1}']/text[text() = '{2}']",
									   relationName, _headwordWritingSystemId, targetEntry.GetHeadWordForm(_headwordWritingSystemId));
		}
		private void CheckRelationNotOutput(string relationName)
		{
			AssertNoMatchForXPathWithArgs(_outputPath,
									   "lift/entry/field[@tag='{0}-relation-headword']",
									   relationName, _headwordWritingSystemId);
		}

		private static LexEntry MakeTestLexEntry(InMemoryRecordList<LexEntry> entries, string lexicalForm)
		{
			LexEntry entry = entries.AddNew();
			entry.LexicalForm["test"] = lexicalForm;
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
			AssertXPathNotNull(string.Format(xpathWithArgs, args), filePath);
		}
		private void AssertNoMatchForXPathWithArgs(string filePath, string xpathWithArgs, params object[] args)
		{
			AssertNoMatchForXPath(string.Format(xpathWithArgs, args), filePath);
		}
		private void AssertXPathNotNull(string xpath, string filePath)
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



		public static void AssertNoMatchForXPath(string xpath, string filePath)
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

		internal class DummyHomographCalculator : IHomographCalculator
		{
			public static int NumberToGiveAsHomograph = -1;

			#region IHomographCalculator Members

			public int GetHomographNumber(LexEntry entry)
			{
				return NumberToGiveAsHomograph;
			}

			#endregion
		}
	}
}