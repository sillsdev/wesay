using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Palaso.Data;
using Palaso.TestUtilities;
using WeSay.LexicalModel;
using Palaso.Lift;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project.Tests
{
	/// <summary>
	/// Tests of the WeSayLiftWriter, focussed on its support for the PLIFT scenario
	/// </summary>
	[TestFixture]
	public class PLiftExporterTests
	{
		//        private string _outputPath;
		//        private ViewTemplate session.Template;
		//        private List<string> session. WritingSystemIds;
		//        private WritingSystem session.HeadwordWritingSystem;
		//        private LexEntryRepository _lexEntryRepository;
		//        private string _FilePath;



		[Test]
		public void NonHomographicEntryHasNoHomographNumber()
		{
			using (var session = new ExportSession())
			{
				LexEntry e1 = session.MakeTestLexEntryInHeadwordWritingSystem("two");
				LexEntry e2 = session.MakeTestLexEntryInHeadwordWritingSystem("flower");
				LexEntry e3 = session.MakeTestLexEntryInHeadwordWritingSystem("one");
				session.DoExport();
				session.AssertHasAtLeastOneMatch("lift/entry[@id='" + e1.Id + "' and not(@order)]");
				session.AssertHasAtLeastOneMatch("lift/entry[@id='" + e2.Id + "' and not(@order)]");
				session.AssertHasAtLeastOneMatch("lift/entry[@id='" + e3.Id + "' and not(@order)]");
			}
		}

		/// <summary>
		/// until we have a proper way to output audio, one thing we can safely do is creat a phonetic
		/// </summary>
		[Test]
		public void Export_LexicalUnitHasVoice_PhoneticMediaFieldIsOutput()
		{
			using (var session = new ExportSession())
			{
				LexEntry e1 = session.Repo.CreateItem();
				e1.LexicalForm["red"] = "r";
				e1.LexicalForm["voice"] = "pretendFileName";

				session.DoExport();
				var path = string.Format("..{0}audio{0}pretendFileName", Path.DirectorySeparatorChar);
				session.AssertHasAtLeastOneMatch("lift/entry/pronunciation/media[@href='"+path+"']");
			}
		}


		/// <summary>
		/// until we have a proper way to output audio, another thing we can safely do is put them in a special field
		/// </summary>
		[Test]
		public void Export_ExampleSentenceHasVoice_TraitWithAudioPathIsOutput()
		{
			using (var session = new ExportSession())
			{
				session.Template.Add(new Field(Field.FieldNames.ExampleSentence.ToString(),
							 "LexExampleSentence",
							 session.WritingSystemIds));

				LexEntry e1 = session.Repo.CreateItem();
				e1.LexicalForm["red"] = "r";
				var sense = new LexSense(e1);
				e1.Senses.Add(sense);
				var example = new LexExampleSentence(sense);
				sense.ExampleSentences.Add(example);
				example.Sentence["green"] = "a sentence";
				example.Sentence["voice"] = "pretendFileName";

				session.DoExport();
				session.AssertHasAtLeastOneMatch("lift/entry/sense/example/form[@lang='green']");
				var path = string.Format("..{0}audio{0}pretendFileName", Path.DirectorySeparatorChar);
				session.AssertHasAtLeastOneMatch("lift/entry/sense/example/trait[@name='audio' and @value='"+path+"']");
			}
		}

		[Test]
		public void Export_LexicalUnitHasVoice_VoiceNotListed()
		{
			using (var session = new ExportSession())
			{
				LexEntry e1 = session.Repo.CreateItem();
				e1.LexicalForm["red"] = "r";
				e1.LexicalForm["voice"] = "v";
				e1.LexicalForm["blue"] = "b";

				session.DoExport();
				session.AssertHasAtLeastOneMatch("lift/entry/lexical-unit/form[@lang='blue']");//sanity check
				session.AssertNoMatchForXPath("lift/entry/lexical-unit/form[@lang='voice']");
			}
		}

		[Test]
		public void Export_CitationHasVoice_CitationOmitsVoice()
		{
			using (var session = new ExportSession())
			{
				LexEntry e1 = session.Repo.CreateItem();
				e1.CitationForm["red"] = "r";
				e1.CitationForm["voice"] = "v";
				e1.CitationForm["blue"] = "b";

				session.DoExport();
				session.PrintResult();
				session.AssertHasAtLeastOneMatch("lift/entry/citation/form[@lang='blue']");//sanity check
				session.AssertNoMatchForXPath("lift/entry/citation/form[@lang='voice']");
			}
		}

		[Test]
		public void Export_CitationHasVoice_HeadwordOmitsVoice()
		{
			using (var session = new ExportSession())
			{
				LexEntry e1 = session.Repo.CreateItem();
				e1.CitationForm["red"] = "r";
				e1.CitationForm["voice"] = "g";
				e1.CitationForm["blue"] = "b";

				session.DoExport();
				session.PrintResult();
				session.AssertHasAtLeastOneMatch("lift/entry/field[@type='headword']/form[@lang='blue']");//sanity check
				session.AssertNoMatchForXPath("lift/entry/field[@type='headword']/form[@lang='voice']");
			}
		}

		[Test]
		public void HomographicEntriesHaveHomographNumber()
		{
			using (var session = new ExportSession())
			{
				LexEntry e1 = session.MakeTestLexEntryInHeadwordWritingSystem("sunset");
				LexEntry e2 = session.MakeTestLexEntryInHeadwordWritingSystem("flower");
				LexEntry e3 = session.MakeTestLexEntryInHeadwordWritingSystem("sunset");

				session.DoExport();
				session.AssertHasAtLeastOneMatch("lift/entry[@id='" + e1.Id + "' and @order='1']");
				session.AssertHasAtLeastOneMatch("lift/entry[@id='" + e2.Id + "' and not(@order)]");
				session.AssertHasAtLeastOneMatch("lift/entry[@id='" + e3.Id + "' and @order='2']");
			}
		}

		[Test]
		public void HiddenFields_AreNotOutput()
		{
			using (var session = new ExportSession())
			{
				LexEntry e1 = session.Repo.CreateItem();
				e1.LexicalForm["test"] = "sunset";
				e1.GetOrCreateProperty<MultiText>("color").SetAlternative(session.WritingSystemIds[0], "red");
				session.Repo.SaveItem(e1);

				Field color = new Field("color",
										"LexEntry",
										session.WritingSystemIds,
										Field.MultiplicityType.ZeroOr1,
										"MultiText");
				color.DisplayName = "color";
				session.Template.Add(color);

				session.DoExport();
				session.AssertHasAtLeastOneMatch("lift/entry[@id='" + e1.Id + "']/field[@type='" + "color" + "']");

				//now make it invisible and it should disappear
				session.Template.GetField("color").Enabled = false;

				session.DoExport();
				session.AssertNoMatchForXPath("lift/entry[@id='" + e1.Id + "']/field");
			}
		}

		[Test]
		public void LexemeForm_DisabledWritingSystems_AreNotOutput()
		{
			using (var session = new ExportSession())
			{
				LexEntry entry = session.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(session.WritingSystemIds[1], "one");
				session.Repo.SaveItem(entry);

				session.DoExport();
				session.AssertHasAtLeastOneMatch("lift/entry/lexical-unit/form[text='one']");
				session.AssertNoMatchForXPath("lift/entry/lexical-unit/form[text='red']");
			}
		}

		[Test]
		public void WritingSystems_AreOutputInPrescribedOrder()
		{
			using (var session = new ExportSession())
			{
				LexEntry entry = session.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(session.WritingSystemIds[1], "greenForm");
				entry.LexicalForm.SetAlternative(session.WritingSystemIds[2], "blueForm");
				entry.LexicalForm.SetAlternative(session.WritingSystemIds[0], "redForm");
				session.Repo.SaveItem(entry);

				session.DoExport();
				XmlNodeList forms = session.GetNodes("lift/entry/lexical-unit/form");

				Assert.AreEqual(session.WritingSystemIds[0], forms[0].Attributes["lang"].InnerText);
				Assert.AreEqual(session.WritingSystemIds[1], forms[1].Attributes["lang"].InnerText);
				Assert.AreEqual(session.WritingSystemIds[2], forms[2].Attributes["lang"].InnerText);
			}
		}

		[Test]
		public void HeadWordField_CitationFieldEnabled_UsesCitationFormSettings()
		{
			using (var session = new ExportSession())
			{
				session.Template.GetField(LexEntry.WellKnownProperties.Citation).Enabled = true;

				session.MakeEntry();
				session.DoExport();
				Assert.AreEqual(2,
								session.Template.GetField(LexEntry.WellKnownProperties.Citation).
									WritingSystemIds.Count);
				session.AssertHasAtLeastOneMatchWithArgs("lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
										   session.Template.GetField(LexEntry.WellKnownProperties.Citation)
											   .WritingSystemIds[0],
										   "blueCitation");
				//should fall through to lexeme form on red
				session.AssertHasAtLeastOneMatchWithArgs("lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
										   session.Template.GetField(LexEntry.WellKnownProperties.Citation)
											   .WritingSystemIds[1],
										   "redLexemeForm");

				session.AssertNoMatchForXPath("lift/entry/field[@type='headword']/form[@lang='green']");
			}
		}

		[Test]
		public void HeadWordField_CitationFieldDisabled_UsesLexemeFormSettings()
		{
			using (var session = new ExportSession())
			{
				session.Template.GetField(LexEntry.WellKnownProperties.Citation).Enabled = false;

				session.MakeEntry();
				session.DoExport();
				session.AssertHasAtLeastOneMatchWithArgs("lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
										   session.HeadwordWritingSystem.Id,
										   "redLexemeForm");

				//nb: it's not clear what the "correct" behavior is, if the citation
				//form is disabled for this user but a citation form does exist for this ws.

				session.AssertHasAtLeastOneMatchWithArgs("lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
											session.WritingSystemIds[1],
										   "greenCitation");
				session.AssertHasAtLeastOneMatchWithArgs(
										   "lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
										   session.WritingSystemIds[2],
										   "blueCitation");
			}
		}



		//        private void MakeEntry()
		//        {
		//            LexEntry entry = _lexEntryRepository.CreateItem();
		//            entry.LexicalForm.SetAlternative("red", "redLexemeForm");
		//            entry.LexicalForm.SetAlternative("green", "greenLexemeForm");
		//            entry.LexicalForm.SetAlternative("blue", "blueLexemeForm");
		//            //leave this blank entry.CitationForm.SetAlternative("red", "redCitation");
		//            entry.CitationForm.SetAlternative("green", "greenCitation");
		//            entry.CitationForm.SetAlternative("blue", "blueCitation");
		//            _lexEntryRepository.SaveItem(entry);
		//        }

		[Test]
		public void RelationEntry_Empty_NothingExported()
		{
			using (var session = new ExportSession())
			{
				LexEntry entry = session.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(session.HeadwordWritingSystem.Id, "Gary");
				entry.AddRelationTarget("brother", string.Empty);
				session.Repo.SaveItem(entry);

				session.DoExport();
				session.CheckRelationNotOutput("brother");
			}
		}

		[Test]
		public void RelationEntry_NotFound_NothingExported()
		{
			using (var session = new ExportSession())
			{
				LexEntry entry = session.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(session.HeadwordWritingSystem.Id, "Gary");
				entry.AddRelationTarget("brother", "notGonnaFindIt");
				session.Repo.SaveItem(entry);

				session.DoExport();
				session.CheckRelationNotOutput("brother");
			}
		}

		[Test]
		public void RelationEntry_Found_HeadWordExported()
		{
			using (var session = new ExportSession())
			{
				LexEntry targetEntry = session.Repo.CreateItem();

				targetEntry.LexicalForm.SetAlternative(session.HeadwordWritingSystem.Id, "RickLexeme");
				targetEntry.CitationForm.SetAlternative(session.HeadwordWritingSystem.Id, "Rick");
				session.Repo.SaveItem(targetEntry);

				LexEntry entry = session.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(session.HeadwordWritingSystem.Id, "Gary");
				session.Repo.SaveItem(entry);

				entry.AddRelationTarget("brother", targetEntry.Id);

				session.DoExport();
				session.CheckRelationOutput(targetEntry, "brother");
			}
		}




		//        private static void AssertHasAtLeastOneMatchWithArgs(string filePath,
		//                                                       string xpathWithArgs,
		//                                                       params object[] args)
		//        {
		//            AssertHasAtLeastOneMatch(filePath, string.Format(xpathWithArgs, args));
		//        }

		//        private static void AssertNoMatchForXPathWithArgs(string filePath,
		//                                                          string xpathWithArgs,
		//                                                          params object[] args)
		//        {
		//            session.AssertNoMatchForXPath(filePath, string.Format(xpathWithArgs, args));
		//        }

		//        private static void AssertHasAtLeastOneMatch(string filePath, string xpath)
		//        {
		//            XmlDocument doc = new XmlDocument();
		//            try
		//            {
		//                doc.Load(filePath);
		//            }
		//            catch (Exception err)
		//            {
		//                Console.WriteLine(err.Message);
		//                Console.WriteLine(File.ReadAllText(filePath));
		//            }
		//            XmlNode node = doc.SelectSingleNode(xpath);
		//            if (node == null)
		//            {
		//                Console.WriteLine("Could not match " + xpath);
		//                PrintNodeToConsole(doc);
		//            }
		//            Assert.IsNotNull(node);
		//        }
		//
		//        public static void AssertNoMatchForXPath(string filePath, string xpath)
		//        {
		//            XmlDocument doc = new XmlDocument();
		//            try
		//            {
		//                doc.Load(filePath);
		//            }
		//            catch (Exception err)
		//            {
		//                Console.WriteLine(err.Message);
		//                Console.WriteLine(File.ReadAllText(filePath));
		//            }
		//            XmlNode node = doc.SelectSingleNode(xpath);
		//            if (node != null)
		//            {
		//                Console.WriteLine("Unexpected match for " + xpath);
		//                PrintNodeToConsole(node);
		//            }
		//            Assert.IsNull(node);
		//        }


	}

	class ExportSession : IDisposable
	{
		private TempFile _outputFile;
		private ProjectDirectorySetupForTesting _projectDir;
		public List<string> WritingSystemIds { get; set; }

		public ExportSession()
		{
			_projectDir = new ProjectDirectorySetupForTesting("");
			var project = _projectDir.CreateLoadedProject();
			_outputFile = new TempFile();
			Repo = new LexEntryRepository(_projectDir.PathToLiftFile);
			WritingSystemIds = new List<string>(new string[] { "red", "green", "blue", "voice" });
			HeadwordWritingSystem =project.WritingSystems.AddSimple("red");
			project.WritingSystems.AddSimple("green");
			project.WritingSystems.AddSimple("blue");
			project.WritingSystems.AddSimple("voice").IsAudio = true;

			Template = new ViewTemplate();

			Template.Add(new Field(LexEntry.WellKnownProperties.Citation,
							"LexEntry",
							new string[] { "blue", "red" }));
			Template.Add(new Field(LexEntry.WellKnownProperties.LexicalUnit,
										"LexEntry",
										new string[] { "red", "green", "blue", "voice" }));
			Template.Add(new Field(LexEntry.WellKnownProperties.BaseForm,
										"LexEntry",
										WritingSystemIds));

			Template.Add(new Field("brother",
							"LexEntry",
							WritingSystemIds));



			Field visibleCustom = new Field("VisibleCustom",
											"LexEntry",
											WritingSystemIds,
											Field.MultiplicityType.ZeroOr1,
											"MultiText");
			visibleCustom.Visibility = CommonEnumerations.VisibilitySetting.Visible;
			visibleCustom.DisplayName = "VisibleCustom";
			Template.Add(visibleCustom);

		}

		public LexEntryRepository Repo { get; set; }

		public WritingSystem HeadwordWritingSystem { get; set; }

		public ViewTemplate Template { get; set; }

		public LexEntry MakeTestLexEntryInHeadwordWritingSystem(string lexicalForm)
		{
			var entry = Repo.CreateItem();
			entry.LexicalForm[HeadwordWritingSystem.Id] = lexicalForm;
			Repo.SaveItem(entry);
			return entry;
		}

		public void Dispose()
		{
			Repo.Dispose();
			_outputFile.Dispose();
			_projectDir.Dispose();
			Repo.Dispose();
		}

		public void AssertHasAtLeastOneMatch(string xpath)
		{
			AssertThatXmlIn.File(_outputFile.Path).
			   HasAtLeastOneMatchForXpath(xpath);

//
//            XmlDocument doc = new XmlDocument();
//            try
//            {
//                doc.Load(_outputFile.Path);
//            }
//            catch (Exception err)
//            {
//                Console.WriteLine(err.Message);
//                Console.WriteLine(File.ReadAllText(_outputFile.Path));
//            }
//            XmlNode node = doc.SelectSingleNode(xpath);
//            if (node == null)
//            {
//                Console.WriteLine("Could not match " + xpath);
//                PrintNodeToConsole(doc);
//            }
//            Assert.IsNotNull(node);
		}

		public void AssertNoMatchForXPath(string xpath)
		{
			var doc = new XmlDocument();
			try
			{
				doc.Load(_outputFile.Path);
			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
				Console.WriteLine(File.ReadAllText(_outputFile.Path));
			}
			var node = doc.SelectSingleNode(xpath);
			if (node != null)
			{
				Console.WriteLine("Unexpected match for " + xpath);
				PrintNodeToConsole(node);
			}
			Assert.IsNull(node);
		}

		public void AssertHasAtLeastOneMatchWithArgs(string xpathWithArgs,
											   params object[] args)
		{
			AssertHasAtLeastOneMatch(string.Format(xpathWithArgs, args));
		}

		public void AssertNoMatchForXPathWithArgs(string xpathWithArgs,
												  params object[] args)
		{
			AssertNoMatchForXPath(string.Format(xpathWithArgs, args));
		}

		public void DoExport()
		{
			using (PLiftExporter exporter = new PLiftExporter(_outputFile.Path, Repo, Template))
			{
				ResultSet<LexEntry> allEntriesSortedByHeadword =
					this.Repo.GetAllEntriesSortedByHeadword(
						this.HeadwordWritingSystem);
				foreach (RecordToken<LexEntry> token in allEntriesSortedByHeadword)
				{
					int homographNumber = 0;
					if ((bool) token["HasHomograph"])
					{
						homographNumber = (int) token["HomographNumber"];
					}
					exporter.Add(token.RealObject, homographNumber);
				}
				exporter.End();
			}
		}

		public void CheckRelationOutput(LexEntry targetEntry, string relationName)
		{
			AssertHasAtLeastOneMatchWithArgs("lift/entry/relation/field[@type='headword-of-target']/form[@lang='{1}']/text[text() = '{2}']",
									   relationName,
									   HeadwordWritingSystem.Id,
									   targetEntry.GetHeadWordForm(HeadwordWritingSystem.Id));
		}

		public void CheckRelationNotOutput(string relationName)
		{
			AssertNoMatchForXPathWithArgs("lift/entry/field[@type='{0}-relation-headword']",
										  relationName,
										  HeadwordWritingSystem.Id);
		}

		public void MakeEntry()
		{
			LexEntry entry = Repo.CreateItem();
			entry.LexicalForm.SetAlternative("red", "redLexemeForm");
			entry.LexicalForm.SetAlternative("green", "greenLexemeForm");
			entry.LexicalForm.SetAlternative("blue", "blueLexemeForm");
			//leave this blank entry.CitationForm.SetAlternative("red", "redCitation");
			entry.CitationForm.SetAlternative("green", "greenCitation");
			entry.CitationForm.SetAlternative("blue", "blueCitation");
			Repo.SaveItem(entry);
		}

		public XmlNodeList GetNodes(string xpath)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(_outputFile.Path);
			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
				Console.WriteLine(File.ReadAllText(_outputFile.Path));
			}
			XmlNodeList nodes = doc.SelectNodes(xpath);
			if (nodes == null || nodes.Count == 0)
			{
				PrintNodeToConsole(doc);
				Assert.Fail("Could not match " + xpath);
			}
			return nodes;
		}

		public void PrintResult()
		{
		  XmlDocument doc = new XmlDocument();
				doc.Load(_outputFile.Path);
			PrintNodeToConsole(doc);
		}

		public  void PrintNodeToConsole(XmlNode node)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			XmlWriter writer = XmlWriter.Create(Console.Out, settings);
			node.WriteContentTo(writer);
			writer.Flush();
			Console.WriteLine();
		}

	}
}