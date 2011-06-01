using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Palaso.Data;
using Palaso.DictionaryServices.Model;
using Palaso.IO;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using Palaso.Xml;
using WeSay.LexicalModel;
using Palaso.Lift;

namespace WeSay.Project.Tests
{
	/// <summary>
	/// Tests of the WeSayLiftWriter, focussed on its support for the PLIFT scenario
	/// </summary>
	[TestFixture]
	public class PLiftExporterTests
	{
		private class EnvironmentForTest : IDisposable
		{
			private readonly TempFile _outputFile;
			private readonly ProjectDirectorySetupForTesting _projectDir;

			public const string Red = "qaa-x-red";
			public const string Blue = "qaa-x-blue";
			public const string Green = "qaa-x-green";
			public const string Voice = "qaa-Zxxx-x-audio";

			public EnvironmentForTest()
			{
				_projectDir = new ProjectDirectorySetupForTesting("");
				var project = _projectDir.CreateLoadedProject();
				_outputFile = new TempFile();
				Repo = new LexEntryRepository(_projectDir.PathToLiftFile);
				WritingSystemIds = new List<string>(new[] { Red, Green, Blue, Voice });
				HeadwordWritingSystem = WritingSystemDefinition.Parse(Red);
				project.WritingSystems.Set(HeadwordWritingSystem);
				project.WritingSystems.Set(WritingSystemDefinition.Parse(Green));
				project.WritingSystems.Set(WritingSystemDefinition.Parse(Blue));
				project.WritingSystems.Set(WritingSystemDefinition.Parse(Voice));

				Template = new ViewTemplate
			{
				new Field(
					LexEntry.WellKnownProperties.Citation,
					"LexEntry",
					new[] {Blue, Red, Voice}),
				new Field(
					LexEntry.WellKnownProperties.LexicalUnit,
					"LexEntry",
					new[] {Red, Green, Blue, Voice}),
				new Field(
					LexEntry.WellKnownProperties.BaseForm,
					"LexEntry",
					WritingSystemIds),
				new Field(
					"brother",
					"LexEntry",
					WritingSystemIds)
			};

				var visibleCustom = new Field(
					"VisibleCustom",
					"LexEntry",
					WritingSystemIds,
					Field.MultiplicityType.ZeroOr1,
					"MultiText"
				);
				visibleCustom.Visibility = CommonEnumerations.VisibilitySetting.Visible;
				visibleCustom.DisplayName = "VisibleCustom";
				Template.Add(visibleCustom);

			}

			public List<string> WritingSystemIds { get; private set; }

			public LexEntryRepository Repo { get; private set; }

			public WritingSystemDefinition HeadwordWritingSystem { get; private set; }

			public ViewTemplate Template { get; private set; }

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
				_projectDir.Dispose();
				_outputFile.Dispose();
			}

			public void AssertHasAtLeastOneMatch(string xpath)
			{
				AssertThatXmlIn.File(_outputFile.Path).HasAtLeastOneMatchForXpath(xpath);
			}

			public void AssertNoMatchForXPath(string xpath)
			{
				AssertThatXmlIn.File(_outputFile.Path).HasNoMatchForXpath(xpath);
			}

			public void AssertHasAtLeastOneMatchWithArgs(string xpathWithArgs, params object[] args)
			{
				AssertHasAtLeastOneMatch(string.Format(xpathWithArgs, args));
			}

			public void AssertNoMatchForXPathWithArgs(string xpathWithArgs, params object[] args)
			{
				AssertNoMatchForXPath(string.Format(xpathWithArgs, args));
			}

			public void DoExport()
			{
				using (var exporter = new PLiftExporter(_outputFile.Path, Repo, Template))
				{
					var allEntriesSortedByHeadword = Repo.GetAllEntriesSortedByHeadword(
						HeadwordWritingSystem
					);
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
				entry.LexicalForm.SetAlternative(Red, "redLexemeForm");
				entry.LexicalForm.SetAlternative(Green, "greenLexemeForm");
				entry.LexicalForm.SetAlternative(Blue, "blueLexemeForm");
				//leave this blank entry.CitationForm.SetAlternative(Red, "redCitation");
				entry.CitationForm.SetAlternative(Green, "greenCitation");
				entry.CitationForm.SetAlternative(Blue, "blueCitation");
				Repo.SaveItem(entry);
			}

			public XmlNodeList GetNodes(string xpath)
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
				var doc = new XmlDocument();
				doc.Load(_outputFile.Path);
				PrintNodeToConsole(doc);
			}

			private static void PrintNodeToConsole(XmlNode node)
			{
				var writer = XmlWriter.Create(Console.Out, CanonicalXmlSettings.CreateXmlWriterSettings(ConformanceLevel.Fragment));
				node.WriteContentTo(writer);
				writer.Flush();
				Console.WriteLine();
			}

		}

		[Test]
		public void NonHomographicEntryHasNoHomographNumber()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry e1 = environment.MakeTestLexEntryInHeadwordWritingSystem("two");
				LexEntry e2 = environment.MakeTestLexEntryInHeadwordWritingSystem("flower");
				LexEntry e3 = environment.MakeTestLexEntryInHeadwordWritingSystem("one");
				environment.DoExport();
				environment.AssertHasAtLeastOneMatch("lift/entry[@id='" + e1.Id + "' and not(@order)]");
				environment.AssertHasAtLeastOneMatch("lift/entry[@id='" + e2.Id + "' and not(@order)]");
				environment.AssertHasAtLeastOneMatch("lift/entry[@id='" + e3.Id + "' and not(@order)]");
			}
		}

		/// <summary>
		/// until we have a proper way to output audio, one thing we can safely do is creat a phonetic
		/// </summary>
		[Test]
		public void Export_LexicalUnitHasVoice_PhoneticMediaFieldIsOutput()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry e1 = environment.Repo.CreateItem();
				e1.LexicalForm[EnvironmentForTest.Red] = "r";
				e1.LexicalForm[EnvironmentForTest.Voice] = "pretendFileName";

				environment.DoExport();
				var path = string.Format("..{0}audio{0}pretendFileName", Path.DirectorySeparatorChar);
				environment.AssertHasAtLeastOneMatch("lift/entry/pronunciation/media[@href='"+path+"']");
			}
		}


		/// <summary>
		/// until we have a proper way to output audio, another thing we can safely do is put them in a special field
		/// </summary>
		[Test]
		public void Export_ExampleSentenceHasVoice_TraitWithAudioPathIsOutput()
		{
			using (var environment = new EnvironmentForTest())
			{
				environment.Template.Add(new Field(Field.FieldNames.ExampleSentence.ToString(),
							 "LexExampleSentence",
							 environment.WritingSystemIds));

				LexEntry e1 = environment.Repo.CreateItem();
				e1.LexicalForm[EnvironmentForTest.Red] = "r";
				var sense = new LexSense(e1);
				e1.Senses.Add(sense);
				var example = new LexExampleSentence(sense);
				sense.ExampleSentences.Add(example);
				example.Sentence[EnvironmentForTest.Green] = "a sentence";
				example.Sentence[EnvironmentForTest.Voice] = "pretendFileName";

				environment.DoExport();
				environment.AssertHasAtLeastOneMatchWithArgs("lift/entry/sense/example/form[@lang='{0}']", EnvironmentForTest.Green);
				var path = string.Format("..{0}audio{0}pretendFileName", Path.DirectorySeparatorChar);
				environment.AssertHasAtLeastOneMatch("lift/entry/sense/example/trait[@name='audio' and @value='"+path+"']");
			}
		}

		[Test]
		public void Export_LexicalUnitHasVoice_VoiceNotListed()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry e1 = environment.Repo.CreateItem();
				e1.LexicalForm[EnvironmentForTest.Red] = "r";
				e1.LexicalForm[EnvironmentForTest.Voice] = "v";
				e1.LexicalForm[EnvironmentForTest.Blue] = "b";

				environment.DoExport();
				environment.AssertHasAtLeastOneMatchWithArgs("lift/entry/lexical-unit/form[@lang='{0}']", EnvironmentForTest.Blue);//sanity check
				environment.AssertNoMatchForXPathWithArgs("lift/entry/lexical-unit/form[@lang='{0}']", EnvironmentForTest.Voice);
			}
		}

		[Test]
		public void Export_CitationHasVoice_CitationOmitsVoice()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry e1 = environment.Repo.CreateItem();
				e1.CitationForm[EnvironmentForTest.Red] = "r";
				e1.CitationForm[EnvironmentForTest.Voice] = "v";
				e1.CitationForm[EnvironmentForTest.Blue] = "b";

				environment.DoExport();
				environment.PrintResult();
				environment.AssertHasAtLeastOneMatchWithArgs("lift/entry/citation/form[@lang='{0}']", EnvironmentForTest.Blue);//sanity check
				environment.AssertNoMatchForXPathWithArgs("lift/entry/citation/form[@lang='{0}']", EnvironmentForTest.Voice);
			}
		}

		[Test]
		public void Export_CitationHasVoice_HeadwordOmitsVoice()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry e1 = environment.Repo.CreateItem();
				e1.CitationForm[EnvironmentForTest.Red] = "r";
				e1.CitationForm[EnvironmentForTest.Voice] = "g";
				e1.CitationForm[EnvironmentForTest.Blue] = "b";

				environment.DoExport();
				environment.PrintResult();
				environment.AssertHasAtLeastOneMatchWithArgs(
					"lift/entry/field[@type='headword']/form[@lang='{0}']",
					EnvironmentForTest.Blue
				);//sanity check
				environment.AssertNoMatchForXPath("lift/entry/field[@type='headword']/form[@lang='voice']");
			}
		}

		[Test]
		public void HomographicEntriesHaveHomographNumber()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry e1 = environment.MakeTestLexEntryInHeadwordWritingSystem("sunset");
				LexEntry e2 = environment.MakeTestLexEntryInHeadwordWritingSystem("flower");
				LexEntry e3 = environment.MakeTestLexEntryInHeadwordWritingSystem("sunset");

				environment.DoExport();
				environment.AssertHasAtLeastOneMatch("lift/entry[@id='" + e1.Id + "' and @order='1']");
				environment.AssertHasAtLeastOneMatch("lift/entry[@id='" + e2.Id + "' and not(@order)]");
				environment.AssertHasAtLeastOneMatch("lift/entry[@id='" + e3.Id + "' and @order='2']");
			}
		}

		[Test]
		public void HiddenFields_AreNotOutput()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry e1 = environment.Repo.CreateItem();
				e1.LexicalForm[EnvironmentForTest.Green] = "sunset";
				e1.GetOrCreateProperty<MultiText>("color").SetAlternative(environment.WritingSystemIds[0], EnvironmentForTest.Red);
				environment.Repo.SaveItem(e1);

				var color = new Field(
					"color",
					"LexEntry",
					environment.WritingSystemIds,
					Field.MultiplicityType.ZeroOr1,
					"MultiText"
				);
				color.DisplayName = "color";
				environment.Template.Add(color);

				environment.DoExport();
				environment.AssertHasAtLeastOneMatch("lift/entry[@id='" + e1.Id + "']/field[@type='" + "color" + "']");

				//now make it invisible and it should disappear
				environment.Template.GetField("color").Enabled = false;

				environment.DoExport();
				environment.AssertNoMatchForXPath("lift/entry[@id='" + e1.Id + "']/field");
			}
		}

		[Test]
		public void LexemeForm_DisabledWritingSystems_AreNotOutput()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry entry = environment.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(environment.WritingSystemIds[1], "one");
				environment.Repo.SaveItem(entry);

				environment.DoExport();
				environment.AssertHasAtLeastOneMatch("lift/entry/lexical-unit/form[text='one']");
				environment.AssertNoMatchForXPathWithArgs("lift/entry/lexical-unit/form[text='{0}']", EnvironmentForTest.Red);
			}
		}

		[Test]
		public void WritingSystems_AreOutputInPrescribedOrder()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry entry = environment.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(environment.WritingSystemIds[1], "greenForm");
				entry.LexicalForm.SetAlternative(environment.WritingSystemIds[2], "blueForm");
				entry.LexicalForm.SetAlternative(environment.WritingSystemIds[0], "redForm");
				environment.Repo.SaveItem(entry);

				environment.DoExport();
				XmlNodeList forms = environment.GetNodes("lift/entry/lexical-unit/form");

				Assert.AreEqual(environment.WritingSystemIds[0], forms[0].Attributes["lang"].InnerText);
				Assert.AreEqual(environment.WritingSystemIds[1], forms[1].Attributes["lang"].InnerText);
				Assert.AreEqual(environment.WritingSystemIds[2], forms[2].Attributes["lang"].InnerText);
			}
		}

		[Test]
		public void HeadWordField_CitationFieldEnabled_UsesCitationFormSettings()
		{
			using (var environment = new EnvironmentForTest())
			{
				environment.Template.GetField(LexEntry.WellKnownProperties.Citation).Enabled = true;

				environment.MakeEntry();
				environment.DoExport();
				environment.AssertHasAtLeastOneMatchWithArgs(
					"lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
					environment.Template.GetField(LexEntry.WellKnownProperties.Citation).WritingSystemIds[0],
					"blueCitation"
				);
				//should fall through to lexeme form on red
				environment.AssertHasAtLeastOneMatchWithArgs(
					"lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
					environment.Template.GetField(LexEntry.WellKnownProperties.Citation).WritingSystemIds[1],
					"redLexemeForm"
				);

				environment.AssertNoMatchForXPathWithArgs("lift/entry/field[@type='headword']/form[@lang='{0}']", EnvironmentForTest.Green);
			}
		}

		[Test]
		public void HeadWordField_CitationFieldDisabled_UsesLexemeFormSettings()
		{
			using (var environment = new EnvironmentForTest())
			{
				environment.Template.GetField(LexEntry.WellKnownProperties.Citation).Enabled = false;

				environment.MakeEntry();
				environment.DoExport();
				environment.AssertHasAtLeastOneMatchWithArgs(
					"lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
					environment.HeadwordWritingSystem.Id,
					"redLexemeForm"
				);

				//nb: it's not clear what the "correct" behavior is, if the citation
				//form is disabled for this user but a citation form does exist for this ws.

				environment.AssertHasAtLeastOneMatchWithArgs(
					"lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
					environment.WritingSystemIds[1],
					"greenCitation"
				);
				environment.AssertHasAtLeastOneMatchWithArgs(
					"lift/entry/field[@type='headword']/form[@lang='{0}']/text[text() = '{1}']",
					environment.WritingSystemIds[2],
					"blueCitation"
				);
			}
		}

		[Test]
		public void RelationEntry_Empty_NothingExported()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry entry = environment.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(environment.HeadwordWritingSystem.Id, "Gary");
				entry.AddRelationTarget("brother", string.Empty);
				environment.Repo.SaveItem(entry);

				environment.DoExport();
				environment.CheckRelationNotOutput("brother");
			}
		}

		[Test]
		public void RelationEntry_NotFound_NothingExported()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry entry = environment.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(environment.HeadwordWritingSystem.Id, "Gary");
				entry.AddRelationTarget("brother", "notGonnaFindIt");
				environment.Repo.SaveItem(entry);

				environment.DoExport();
				environment.CheckRelationNotOutput("brother");
			}
		}

		[Test]
		public void RelationEntry_Found_HeadWordExported()
		{
			using (var environment = new EnvironmentForTest())
			{
				LexEntry targetEntry = environment.Repo.CreateItem();

				targetEntry.LexicalForm.SetAlternative(environment.HeadwordWritingSystem.Id, "RickLexeme");
				targetEntry.CitationForm.SetAlternative(environment.HeadwordWritingSystem.Id, "Rick");
				environment.Repo.SaveItem(targetEntry);

				LexEntry entry = environment.Repo.CreateItem();
				entry.LexicalForm.SetAlternative(environment.HeadwordWritingSystem.Id, "Gary");
				environment.Repo.SaveItem(entry);

				entry.AddRelationTarget("brother", targetEntry.Id);

				environment.DoExport();
				environment.CheckRelationOutput(targetEntry, "brother");
			}
		}

	}

}