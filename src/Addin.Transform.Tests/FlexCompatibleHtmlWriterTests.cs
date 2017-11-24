using System.Collections.Generic;
using System.IO;
using System.Text;
using Addin.Transform.PdfDictionary;
using NUnit.Framework;
using SIL.DictionaryServices.Model;
using SIL.Lift; // review: really?
using SIL.Lift.Options;
using SIL.TestUtilities;
using SIL.Data;
using SIL.Linq;
using SIL.WritingSystems;
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
			Sldr.Initialize(true);
			_projectDir = new ProjectDirectorySetupForTesting("");
			_project = _projectDir.CreateLoadedProject();
			var ws = _project.WritingSystems.Get("en");
			ws.DefaultCollation = new IcuRulesCollationDefinition("standard");
			_project.WritingSystems.Set(ws);
			ws = _project.WritingSystems.Get("qaa");
			ws.DefaultCollation = new IcuRulesCollationDefinition("standard");
			_project.WritingSystems.Set(ws);

            _project.WritingSystems.Set(new WritingSystemDefinition("fr")
            {
	            DefaultCollation = new IcuRulesCollationDefinition("standard")
            });
            _repo = _project.GetLexEntryRepository();
            _entry = _repo.CreateItem();
            _entry.LexicalForm.SetAlternative("qaa", "apple");


            _project.DefaultPrintingTemplate.GetField(LexSense.WellKnownProperties.Definition).WritingSystemIds.Add("fr");
            _project.DefaultPrintingTemplate.GetField(LexExampleSentence.WellKnownProperties.Translation).WritingSystemIds.Add("fr");

            _project.DefaultPrintingTemplate.GetField(LexEntry.WellKnownProperties.CrossReference).Enabled = true;
        }

        [TearDown]
        public void TearDown()
        {
            _project.Dispose();
            _projectDir.Dispose();
            _repo.Dispose();
			Sldr.Cleanup();
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
			AssertBodyHas(pathToExamples + "/span[@class='example' and @lang='qaa' and text()='first example']");
			AssertBodyHas(pathToExamples + "/span[@class='example' and @lang='qaa' and text()='second example']");
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
			e.Sentence.SetAlternative("qaa", "first example");
            e.Translation.SetAlternative("en", "english translation");
            e.Translation.SetAlternative("fr", "un exemple");

            e = new LexExampleSentence(sense);
            sense.ExampleSentences.Add(e);
			e.Sentence.SetAlternative("qaa", "second example");
            e.Translation.SetAlternative("en", "english translation");
            e.Translation.SetAlternative("fr", "un autre exemple");
        }

        private void AddSenseWithDefinition()
        {
            var sense = new LexSense(_entry);
            _entry.Senses.Add(sense);
            sense.Definition.SetAlternative("en", "fruit");
            sense.Definition.SetAlternative("fr", "pomme");
			sense.GetOrCreateProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech).Key = "Noun";
        }

        [Test]
        public void Entries_PrecededByLetterDivs()
        {
            // Note: When the Palaso WritingSystemDefinition changed to use ICU for DefaultOrdering this test broke.
            // The windows SystemCollator using CultureInvariant sorts hyphens with the text, rather than default unicode
            // order.
            // To make this test pass we provide a custom ICU sort rule to do the same.
	        var ws = _project.WritingSystems.Get("qaa");
            const string icuRules = "&[last primary ignorable] <<< '-' <<< ' '";
            var cd = new IcuRulesCollationDefinition("standard")
            {
                IcuRules = icuRules,
            };
            ws.DefaultCollation = cd;
            _project.WritingSystems.Set(ws);
            var entries = new List<LexEntry>();
            entries.Add(_entry);

            var pineapple = _repo.CreateItem();
            entries.Add(pineapple);
			pineapple.LexicalForm.SetAlternative("qaa", "-pineapple");//should skip hyphen

            var pear = _repo.CreateItem();
            entries.Add(pear);
			pear.LexicalForm.SetAlternative("qaa", "pear");

            var contents = GetXhtmlContents(entries);

            //should only be two sections, not three
            AssertHasAtLeastOneMatch(contents, "html/body[count(div[@class='letHead'])=2]");

            AssertHasAtLeastOneMatch(contents, "html/body/div[@class='letHead']/div[@class='letter' and text()='A a']");
            AssertHasAtLeastOneMatch(contents, "html/body/div[@class='letHead']/div[@class='letData']");

            AssertHasAtLeastOneMatch(contents, "html/body/div[@class='letHead']/div[@class='letter' and text()='P p']");
        }


        /// <summary>
        /// regression WS-1412
        /// Note, we don't yet have a way to know where the other-case letter is, so we just expect the exact same case to have a section
        /// </summary>
        [Test]
        public void LetterDiv_PrivateUseCharacter_GeneratesMatchingLetterDivWithOnlyOneCase()
        {
            Assert.AreEqual(System.Globalization.UnicodeCategory.PrivateUse, char.GetUnicodeCategory(('')));

            var entries = new List<LexEntry>();

            var word = _repo.CreateItem();
            entries.Add(word);
			word.LexicalForm.SetAlternative("qaa", "ee");

            var contents = GetXhtmlContents(entries);

            AssertHasAtLeastOneMatch(contents, "html/body/div[@class='letHead']/div[@class='letter' and text()='']");
        }

        [Test]
        public void TwoEntryCrossRefferences()
        {
            var targetOne = _repo.CreateItem();
			targetOne.LexicalForm.SetAlternative("qaa", "targetOne");
            var targetTwo = _repo.CreateItem();
			targetTwo.LexicalForm.SetAlternative("qaa", "targetTwo");

            var crossRefs =_entry.GetOrCreateProperty<LexRelationCollection>(LexEntry.WellKnownProperties.CrossReference);
            crossRefs.Relations.Add(new LexRelation(LexEntry.WellKnownProperties.CrossReference, targetOne.Id, _entry));
            crossRefs.Relations.Add(new LexRelation(LexEntry.WellKnownProperties.CrossReference, targetTwo.Id, _entry));

            // var dummy =GetXhtmlContents(new List<LexEntry>(new LexEntry[] {_entry}));

            //AssertBodyHas("*//span[@class='crossrefs']/span[@class='crossref-type' and text()='cf']");
            AssertBodyHas("*//span[@class='crossrefs']/span[@class='crossref-targets' and count(span[@class='xitem']) = 2]");
            AssertBodyHas("*//span[@class='xitem']/span[@class='crossref' and text()='targetOne']");
            AssertBodyHas("*//span[@class='xitem']/span[@class='crossref' and text()='targetTwo']");
        }

		/// <summary>
		/// Regression test for Olga's incident where print to pdf failed because the target had been deleted
		/// </summary>
		[Test]
		public void RelationToADeletedEntryIgnored()
		{
			var targetOne = _repo.CreateItem();
			targetOne.LexicalForm.SetAlternative("qaa", "targetOne");

			var crossRefs = _entry.GetOrCreateProperty<LexRelationCollection>(LexEntry.WellKnownProperties.CrossReference);
			crossRefs.Relations.Add(new LexRelation(LexEntry.WellKnownProperties.CrossReference, "longGone", _entry));

			IList<LexEntry> entries = new List<LexEntry>(new[] { _entry });

			AssertHasNoMatch(GetXhtmlContents(entries), "//span[@class='crossrefs']");
        }

        [Test]
        public void TwoHomographicEntries_HaveHomographs()
        {
            var entries = new List<LexEntry>();
            entries.Add(_entry);

            var secondOne = _repo.CreateItem();
            entries.Add(secondOne);
			secondOne.LexicalForm.SetAlternative("qaa", _entry.GetHeadWordForm("qaa"));

            var contents = GetXhtmlContents(entries);

            AssertHasAtLeastOneMatch(contents, "//span[@class='xhomographnumber' and text()='1']");
            AssertHasAtLeastOneMatch(contents, "//span[@class='xhomographnumber' and text()='2']");

          }

        [Test]
        public void TwoNonHomographicEntries_NoHomographs()
        {
            var entries = new List<LexEntry>();
            entries.Add(_entry);

            var secondOne = _repo.CreateItem();
            entries.Add(secondOne);
			secondOne.LexicalForm.SetAlternative("qaa", "banana");

            var contents = GetXhtmlContents(entries);

            AssertHasNoMatch(contents, "//span[@class='xhomographnumber']");

        }


        private string GetXhtmlContents(IList<LexEntry> entries)
        {
            if(entries!=null)
            {
                entries.ForEach(e=>_repo.SaveItem(e));
            }


            var pliftbuilder = new StringBuilder();
            using (var pexp = new PLiftExporter(pliftbuilder, false, _repo, _project.DefaultPrintingTemplate))
            {
                ResultSet<LexEntry> recordTokens =
                    _repo.GetAllEntriesSortedByHeadword(_project.DefaultPrintingTemplate.HeadwordWritingSystems[0]);


                foreach (RecordToken<LexEntry> token in recordTokens)
                {
                    pexp.Add(token.RealObject);
                }
                pexp.End();
            }
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
            IList<LexEntry> entries = new List<LexEntry>(new[] {_entry});
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

        private static void AssertHasNoMatch(string xml, string xpath)
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
