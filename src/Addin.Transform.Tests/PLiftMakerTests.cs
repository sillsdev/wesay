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
using WeSay.Project.Tests;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class PLiftMakerTests
	{
		private PLiftMaker _maker;
		private string _outputPath;

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			_maker = new PLiftMaker();
			_outputPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = false;
		}

//        [Test]
//        public void EntryMakeItToXHtml()
//        {
//            string xmlForEntries = @"<entry id='foo1'><lexical-unit><form lang='v'><text>fooOne</text></form></lexical-unit></entry>";
//
//            using (Db4oProjectSetupForTesting projectSetup = new Db4oProjectSetupForTesting(xmlForEntries))
//            {
//                PLiftMaker maker = new PLiftMaker();
//                string outputPath = Path.Combine(projectSetup._project.PathToExportDirectory, projectSetup._project.Name + ".xhtml");
//                maker.MakeXHtmlFile(outputPath, projectSetup._lexEntryRepository, projectSetup._project);
//                Assert.IsTrue(File.ReadAllText(outputPath).Contains("<span class=\"v\">fooOne"));
//            }
//        }

		[Test, Ignore("not a real test")]
		public void MakePLiftForBiatah2()
		{
			using (WeSay.Project.WeSayWordsProject p = new WeSayWordsProject())
			{
				p.LoadFromProjectDirectoryPath(@"E:\Users\John\Documents\WeSay\biatah");

				using (
					LexEntryRepository lexEntryRepository =
						new LexEntryRepository(new WeSayWordsDb4oModelConfiguration(), p.PathToDb4oLexicalModelDB))
				{
					PLiftMaker maker = new PLiftMaker();
					IEnumerable<LexEntry> entries = Lexicon.GetAllEntriesSortedByHeadword(p.HeadWordWritingSystem);
					Db4oLexEntryFinder finder = new Db4oLexEntryFinder(lexEntryRepository.DataSource);
					string path = maker.MakePLiftTempFile(entries, p.DefaultViewTemplate, finder);
					Console.WriteLine(path);
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