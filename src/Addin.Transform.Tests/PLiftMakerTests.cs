using System.IO;
using NUnit.Framework;
using Palaso.DictionaryServices.Lift;
using SIL.Reporting;
using Palaso.TestUtilities;
using WeSay.Project;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class PLiftMakerTests
	{
		private string _outputPath;

		[SetUp]
		public void Setup()
		{
			_outputPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
			ErrorReport.IsOkToInteractWithUser = false;
		}

		[Test]
		public void EntryMakeItToPLift()
		{
			var xmlOfEntries = @" <entry id='foo1'>
										<lexical-unit><form lang='qaa-x-qaa'><text>hello</text></form></lexical-unit>
								 </entry>";
			using (var p = new WeSay.Project.Tests.ProjectDirectorySetupForTesting(xmlOfEntries))
			{
				PLiftMaker maker = new PLiftMaker();
				using (var project = p.CreateLoadedProject())
				{
					using (var repository = project.GetLexEntryRepository())
					{
						string outputPath = Path.Combine(project.PathToExportDirectory, project.Name + ".xhtml");
						maker.MakePLiftTempFile(outputPath, repository, project.DefaultPrintingTemplate, LiftWriter.ByteOrderStyle.BOM);
						AssertThatXmlIn.File(outputPath).
							HasAtLeastOneMatchForXpath("//field[@type='headword']/form[@lang='qaa-x-qaa']/text[text()='hello']");
					}
				}
			}
		}


		//Regression... there was a switch in how Lexique pro handled grammatical-info, such that it needs the raw-lift-style (previously we gave it what looked like a custom field)
		[Test]
		public void MakePLiftTempFile_ExportPartOfSpeechAsGrammaticalInfoElementSpecified_GrammaticalInfoOutputAsElement()
		{
			var xmlOfEntries = @" <entry id='foo1'>
										<sense><grammatical-info value='noun'></grammatical-info></sense>
								 </entry>";
			using (var p = new WeSay.Project.Tests.ProjectDirectorySetupForTesting(xmlOfEntries))
			{
				PLiftMaker maker = new PLiftMaker() { Options = PLiftExporter.DefaultOptions | PLiftExporter.Options.ExportPartOfSpeechAsGrammaticalInfoElement }; ;
				using (var project = p.CreateLoadedProject())
				{
					using (var repository = project.GetLexEntryRepository())
					{
						string outputPath = Path.Combine(project.PathToExportDirectory, project.Name + ".plift");
						maker.MakePLiftTempFile(outputPath, repository, project.DefaultPrintingTemplate, LiftWriter.ByteOrderStyle.BOM);
						AssertThatXmlIn.File(outputPath).
							HasAtLeastOneMatchForXpath("//sense/grammatical-info[@value='noun']");
					}
				}
			}
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_outputPath))
			{
				File.Delete(_outputPath);
			}
		}
	}
}