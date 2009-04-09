using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Addin.Transform.PdfDictionary;
using NUnit.Framework;
using Palaso.Test;
using WeSay.Data;
using WeSay.Foundation;
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

			_project.DefaultPrintingTemplate.GetField(LexEntry.WellKnownProperties.CrossReference).Enabled = true;
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
		   AssertHasAtLeastOneMatch(GetXhtmlContents(null), "html/body");
		}

		[Test]
		public void Entry_ProperDivCreated()
		{
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
		public void SensesGetSenseNumbersWhenMoreThanOne()
		{
			AddSenseWithDefinition();
			AddSenseWithDefinition();

			var path = "div/div/div[@class='entry']/span[@class='senses']/span[@class='sense']/span[@class='xsensenumber' and text()='{0}']";
			AssertBodyHas(path,1);
			AssertBodyHas(path, 2);
		}

		[Test]
		public void SensePicture()
		{
			AddSenseWithPicture();

			var pathToPicture= "div/div/div[@class='entry']/span[@class='senses']/span[@class='sense']/span[@class='pictureRight']";
			AssertBodyHas(pathToPicture);
			AssertBodyHas(pathToPicture + "/img[@src='..{0}pictures{0}pretend.png']", Path.DirectorySeparatorChar);
			//todo caption?
		}

		[Test]
		public void PictureGetsAutoCaption()
		{
			AddSenseWithPicture();

			var pathToPicture = "div/div/div[@class='entry']/span[@class='senses']/span[@class='sense']/span[@class='pictureRight']";
			AssertBodyHas(pathToPicture);
			AssertBodyHas(pathToPicture +
						  "/div[@class='pictureCaption']/span[@class='pictureLabel' and text()='apple']");
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


		private void AddSenseWithPicture()
		{
		   var sense = new LexSense(_entry);
			_entry.Senses.Add(sense);
			var pict = sense.GetOrCreateProperty<PictureRef>(LexSense.WellKnownProperties.Picture);
			pict.Caption = new MultiText();
			pict.Caption.SetAlternative("en", "test caption");
			pict.Value = "pretend.png";
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
			pineapple.LexicalForm.SetAlternative("v", "-pineapple");//should skip hyphen

			var pear = _repo.CreateItem();
			entries.Add(pear);
			pear.LexicalForm.SetAlternative("v", "pear");

			var contents = GetXhtmlContents(entries);

			//should only be two sections, not three
			AssertHasAtLeastOneMatch(contents, "html/body[count(div[@class='letHead'])=2]");

			AssertHasAtLeastOneMatch(contents, "html/body/div[@class='letHead']/div[@class='letter' and text()='A a']");
			AssertHasAtLeastOneMatch(contents, "html/body/div[@class='letHead']/div[@class='letData']");

			AssertHasAtLeastOneMatch(contents, "html/body/div[@class='letHead']/div[@class='letter' and text()='P p']");
		}

		[Test]
		public void TwoEntryCrossRefferences()
		{
			var targetOne = _repo.CreateItem();
			targetOne.LexicalForm.SetAlternative("v", "targetOne");
			var targetTwo = _repo.CreateItem();
			targetTwo.LexicalForm.SetAlternative("v", "targetTwo");

			var crossRefs =_entry.GetOrCreateProperty<LexRelationCollection>(LexEntry.WellKnownProperties.CrossReference);
			crossRefs.Relations.Add(new LexRelation(LexEntry.WellKnownProperties.CrossReference, targetOne.Id, _entry));
			crossRefs.Relations.Add(new LexRelation(LexEntry.WellKnownProperties.CrossReference, targetTwo.Id, _entry));

					   ;
		   // var dummy =GetXhtmlContents(new List<LexEntry>(new LexEntry[] {_entry}));

			//AssertBodyHas("*//span[@class='crossrefs']/span[@class='crossref-type' and text()='cf']");
			AssertBodyHas("*//span[@class='crossrefs']/span[@class='crossref-targets' and count(span[@class='xitem']) = 2]");
			AssertBodyHas("*//span[@class='xitem']/span[@class='crossref' and text()='targetOne']");
			AssertBodyHas("*//span[@class='xitem']/span[@class='crossref' and text()='targetTwo']");
		}

		[Test]
		public void TwoHomographicEntries_HaveHomographs()
		{
			List<LexEntry> entries = new List<LexEntry>();
			entries.Add(_entry);

			var secondOne = _repo.CreateItem();
			entries.Add(secondOne);
			secondOne.LexicalForm.SetAlternative("v", _entry.GetHeadWordForm("v"));

			var contents = GetXhtmlContents(entries);

			AssertHasAtLeastOneMatch(contents, "//span[@class='xhomographnumber' and text()='1']");
			AssertHasAtLeastOneMatch(contents, "//span[@class='xhomographnumber' and text()='2']");

		  }

		[Test]
		public void TwoNonHomographicEntries_NoHomographs()
		{
			List<LexEntry> entries = new List<LexEntry>();
			entries.Add(_entry);

			var secondOne = _repo.CreateItem();
			entries.Add(secondOne);
			secondOne.LexicalForm.SetAlternative("v", "banana");

			var contents = GetXhtmlContents(entries);

			HasNoMatch(contents, "//span[@class='xhomographnumber']");

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
					x.Write(new StringReader(pliftbuilder.ToString()), w);
				}
//                catch(Exception e)
//                {
//                    Console.WriteLine(pliftbuilder.ToString());
//                    throw e;
//                }
			}
			return builder.ToString();
		}

		private  void AssertBodyHas(string xpath, params object[] args)
		{
			IList<LexEntry> entries = new List<LexEntry>(new LexEntry[] {_entry});
			AssertHasAtLeastOneMatch(GetXhtmlContents(entries), "html/body/" + string.Format(xpath,args));
		}


		private static void AssertHasAtLeastOneMatch(string xml, string xpath)
		{
			AssertThatXmlIn.String(xml).
			   HasAtLeastOneMatchForXpath(xpath);

//
//            XmlDocument doc = GetDoc(xml);
//            XmlNode node = doc.SelectSingleNode(xpath);
//            if (node == null)
//            {
//                Console.WriteLine("Could not match "+xpath);
//                Console.WriteLine();
//                XmlWriterSettings settings = new XmlWriterSettings();
//                settings.Indent = true;
//                settings.ConformanceLevel = ConformanceLevel.Fragment;
//                XmlWriter writer = XmlWriter.Create(Console.Out, settings);
//                doc.WriteContentTo(writer);
//                writer.Flush();
//            }
//            Assert.IsNotNull(node);
		}

		private static void HasNoMatch(string xml, string xpath)
		{
			AssertThatXmlIn.String(xml).HasNoMatchForXpath(xpath);
//
//
//            XmlDocument doc = GetDoc(xml);
//            XmlNode node = doc.SelectSingleNode(xpath);
//            if (node != null)
//            {
//                Console.WriteLine("Was not supposed to match " + xpath);
//                Console.WriteLine();
//                XmlWriterSettings settings = new XmlWriterSettings();
//                settings.Indent = true;
//                settings.ConformanceLevel = ConformanceLevel.Fragment;
//                XmlWriter writer = XmlWriter.Create(Console.Out, settings);
//                doc.WriteContentTo(writer);
//                writer.Flush();
//            }
//            Assert.IsNull(node);
		}

//        private static XmlDocument GetDoc(string xml)
//        {
//            XmlDocument doc = new XmlDocument();
//            try
//            {
//                doc.LoadXml(xml);
//            }
//            catch (Exception err)
//            {
//                Console.WriteLine(err.Message);
//                Console.WriteLine(xml);
//            }
//            return doc;
//        }
	}
}