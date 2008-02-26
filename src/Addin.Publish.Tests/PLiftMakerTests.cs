using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace Addin.Publish.Tests
{
	[TestFixture]
	public class PLiftMakerTests
	{
		private PLiftMaker _maker;
		private string _outputPath;
		private ViewTemplate _viewTemplate;
		private List<string> _writingSystemIds;

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			_maker = new PLiftMaker();
			_outputPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

			_writingSystemIds = new List<string>(new string[] { "first","second","third" });

			_viewTemplate = new ViewTemplate();

			_viewTemplate.Add(new Field(Field.FieldNames.EntryLexicalForm.ToString(), "LexEntry", _writingSystemIds));

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

		[Test, Ignore("Not Implemented")]
		public void HomographicEntriesHaveHomographNumber()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry e1 = MakeTestLexEntry(entries, "sunset");
				MakeTestLexEntry(entries, "flower");
				LexEntry e2 =MakeTestLexEntry(entries, "sunset");

				_maker.Make(entries, _viewTemplate, _outputPath);
				AssertXPathNotNull("lift/entry[@id='"+e1.Id+"' and @order='1']", _outputPath);
				AssertXPathNotNull("lift/entry[@id='"+e2.Id+"' and @order='2']", _outputPath);
			}
		 }

		[Test]
		public void NonHomographicEntryHasNoHomographNumber()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				MakeTestLexEntry(entries, "sunset");
				LexEntry e1 = MakeTestLexEntry(entries, "flower");
				MakeTestLexEntry(entries, "sunset");

				_maker.Make(entries, _viewTemplate, _outputPath);
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

				_maker.Make(entries, _viewTemplate, _outputPath);
				AssertXPathNotNull("lift/entry[@id='" + e1.Id + "']/field[@tag='"+"color"+"']", _outputPath);

				//now make it invisible and it should disappear
				_viewTemplate.GetField("color").Enabled = false;

				_maker.Make(entries, _viewTemplate, _outputPath);
				AssertXPathIsNull("lift/entry[@id='" + e1.Id + "']/field", _outputPath);

			}
		}

		[Test]
		public void LexemeForm_DisabledWritingSystems_AreNotOutput()
		{
			using (InMemoryRecordList<LexEntry> entries = new InMemoryRecordList<LexEntry>())
			{
				LexEntry entry = entries.AddNew();
				entry.LexicalForm.SetAlternative(_writingSystemIds[1], "one");
				_maker.Make(entries, _viewTemplate, _outputPath);
				AssertXPathNotNull("lift/entry/lexical-unit/form[text='one']", _outputPath);
				AssertXPathIsNull("lift/entry/lexical-unit/form[text='red']", _outputPath);
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
				_maker.Make(entries, _viewTemplate, _outputPath);
				XmlNodeList forms = GetNodes("lift/entry/lexical-unit/form", _outputPath);

				Assert.AreEqual(_writingSystemIds[0], forms[0].Attributes["lang"].InnerText);
				Assert.AreEqual(_writingSystemIds[1], forms[1].Attributes["lang"].InnerText);
				Assert.AreEqual(_writingSystemIds[2], forms[2].Attributes["lang"].InnerText);
			}
		}

		[Test, Ignore("Not Implemented")]
		public void Test_SomethingWithRelations()
		{
			Assert.Fail();
		}

		[Test, Ignore("")]
		public void Test_SomethingWithBaseEntry()
		{
			Assert.Fail();
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



		public static void AssertXPathIsNull(string xpath, string filePath)
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
