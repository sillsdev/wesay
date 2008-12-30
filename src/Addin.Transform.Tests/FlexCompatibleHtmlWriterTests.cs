using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using LiftIO.Validation;
using NUnit.Framework;
using WeSay.AddinLib;
using WeSay.Data;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.Project.Tests;
using System.Linq;

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
			_entry.LexicalForm.SetAlternative("v", "apple");


			_project.DefaultPrintingTemplate.GetField(LexSense.WellKnownProperties.Definition).WritingSystemIds.Add("fr");
			_project.DefaultPrintingTemplate.GetField(LexExampleSentence.WellKnownProperties.Translation).WritingSystemIds.Add("fr");
		}
		[TearDown]
		public void TearDown()
		{
			_project.Dispose();
			_projectDir.Dispose();
		}

		[Test]
		public void EmptyLexicon_OK()
		{
		   AssertXPathNotNull(GetXhtmlContents(null), "html/body");
		}

		[Test]
		public void Entry_ProperDivCreated()
		{
			_entry.LexicalForm.SetAlternative("v", "voop");
			AssertBodyHas("div/div/div[@class='entry']");
		}

		[Test]
		public void Entry_PrimaryHeadword()
		{
			AssertBodyHas("div/div/div[@class='entry']/span[@class='headword' and text()='apple']");
		}

		[Test]
		public void PartsOfSpeech()
		{
			AddSenseWithDefinition();

			var pathToPOS = "div/div/div[@class='entry']/span[@class='senses']/span[@class='sense']/span[@class='grammatical-info']/span[@class='partofspeech']";
			AssertBodyHas(pathToPOS);
			AssertBodyHas(pathToPOS + "/text()[.='noun']");
		}

		[Test]
		public void SenseDefinition()
		{
			AddSenseWithDefinition();
			AssertBodyHas("div/div/div[@class='entry']/span[@class='senses']");

			var pathToDefinition = "div/div/div[@class='entry']/span[@class='senses']/span[@class='sense']/span[@class='definition_L2']";
			AssertBodyHas(pathToDefinition);
			AssertBodyHas(pathToDefinition+"/span[@class='xitem' and @lang='en' and text()='fruit']");
			AssertBodyHas(pathToDefinition+"/span[@class='xitem' and @lang='fr' and text()='pomme']");
		}

		[Test]
		public void Example()
		{
			AddSenseWithTwoExamples();
			var pathToSense = "div/div/div[@class='entry']/span[@class='senses']/span[@class='sense']";
			var pathToExamples = pathToSense+"/span[@class='examples']";
			AssertBodyHas(pathToExamples);
			AssertBodyHas(pathToExamples + "/span[@class='example' and @lang='v' and text()='first example']");
			AssertBodyHas(pathToExamples + "/span[@class='example' and @lang='v' and text()='second example']");
			//notice, they aren't nested in example
			var pathToTranslations = pathToExamples+"/span[@class='translations']";
			AssertBodyHas(pathToTranslations);
			AssertBodyHas(pathToTranslations+"/span[@class='translation' and @lang='en']");
		}

		private void AddSenseWithTwoExamples()
		{
			var sense = new LexSense(_entry);
			_entry.Senses.Add(sense);
			var e = new LexExampleSentence(sense);
			sense.ExampleSentences.Add(e);
			e.Sentence.SetAlternative("v", "first example");
			e.Translation.SetAlternative("en", "english translation");
			e.Translation.SetAlternative("fr", "un exemple");

			e = new LexExampleSentence(sense);
			sense.ExampleSentences.Add(e);
			e.Sentence.SetAlternative("v", "second example");
			e.Translation.SetAlternative("en", "english translation");
			e.Translation.SetAlternative("fr", "un autre exemple");
		}

		private void AddSenseWithDefinition()
		{
			var sense = new LexSense(_entry);
			_entry.Senses.Add(sense);
			sense.Definition.SetAlternative("en", "fruit");
			sense.Definition.SetAlternative("fr", "pomme");
			sense.GetOrCreateProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech).Key = "noun";
		}

		[Test]
		public void Entries_PrecededByLetterDivs()
		{
			List<LexEntry> entries = new List<LexEntry>();
			entries.Add(_entry);

			var pineapple = _repo.CreateItem();
			entries.Add(pineapple);
			pineapple.LexicalForm.SetAlternative("v", "pineapple");

			var pear = _repo.CreateItem();
			entries.Add(pear);
			pear.LexicalForm.SetAlternative("v", "pear");

			var contents = GetXhtmlContents(entries);

			//should only be two sections, not three
			AssertXPathNotNull(contents, "html/body[count(div[@class='letHead'])=2]");

			AssertXPathNotNull(contents, "html/body/div[@class='letHead']/div[@class='letter' and text()='A a']");
			AssertXPathNotNull(contents, "html/body/div[@class='letHead']/div[@class='letData']");

			AssertXPathNotNull(contents, "html/body/div[@class='letHead']/div[@class='letter' and text()='P p']");
		}

		private string GetXhtmlContents(IList<LexEntry> entries)
		{
			if(entries!=null)
			{
				entries.ForEach(e=>_repo.SaveItem(e));
			}


			var pliftbuilder = new StringBuilder();
			var pexp = new PLiftExporter(pliftbuilder, false, _repo, _project.DefaultPrintingTemplate);
			ResultSet<LexEntry> recordTokens =
					_repo.GetAllEntriesSortedByHeadword(_project.DefaultPrintingTemplate.HeadwordWritingSystems[0]);


			foreach (RecordToken<LexEntry> token in recordTokens)
			{
				pexp.Add(token.RealObject);
			}
			pexp.End();

			var builder = new StringBuilder();
			using (var w = new StringWriter(builder))
			{
				var x = new FLExCompatibleXhtmlWriter();

			  //  try
				{
					x.Write(pliftbuilder.ToString(), w);
				}
//                catch(Exception e)
//                {
//                    Console.WriteLine(pliftbuilder.ToString());
//                    throw e;
//                }
			}
			return builder.ToString();
		}

		private  void AssertBodyHas(string xpath)
		{
			IList<LexEntry> entries = new List<LexEntry>(new LexEntry[] {_entry});
			AssertXPathNotNull(GetXhtmlContents(entries), "html/body/" + xpath);
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
				Console.WriteLine("Could not match "+xpath);
				Console.WriteLine();
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