using System;
using System.IO;
using System.Text;
using System.Xml;
using LiftIO.Validation;
using NUnit.Framework;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.Project.Tests;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class FlexCompatibleHtmlWriterTests
	{
		private ProjectDirectorySetupForTesting _projectDir;
		private WeSayWordsProject _project;
		private LexEntryRepository _repo;
		private LexEntry _entry;

		[SetUp]
		public void Setup()
		{
			_projectDir = new WeSay.Project.Tests.ProjectDirectorySetupForTesting("");
			_project = _projectDir.CreateLoadedProject();
			_repo = _project.GetLexEntryRepository();
			_entry = _repo.CreateItem();
		}
		[TearDown]
		public void TearDown()
		{
			_projectDir.Dispose();
		}

		[Test]
		public void Entry_ProperDivCreated()
		{
			_entry.LexicalForm.SetAlternative("v", "voop");
			AssertXPathNotNull(GetXhtmlContents(), "div[@class='entry']");
		}

		[Test]
		public void Entry_Headword()
		{
			_entry.LexicalForm.SetAlternative("v", "voop");
			AssertXPathNotNull(GetXhtmlContents(), "div[@class='entry']");
		}

		private string GetXhtmlContents()
		{
			_repo.SaveItem(_entry);
			var builder = new StringBuilder();
			using (var w = new StringWriter(builder))
			{
				var x = new FLExCompatibleXhtmlWriter(_repo, _project.DefaultPrintingTemplate);
				x.Write(w);
			}
			return builder.ToString();
		}

		private static void AssertXPathNotNull(string xml, string xpath)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.LoadXml(xml);
			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
				Console.WriteLine(xml);
			}
			XmlNode node = doc.SelectSingleNode(xpath);
			if (node == null)
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.ConformanceLevel = ConformanceLevel.Fragment;
				XmlWriter writer = XmlWriter.Create(Console.Out, settings);
				doc.WriteContentTo(writer);
				writer.Flush();
			}
			Assert.IsNotNull(node);
		}
	}
}