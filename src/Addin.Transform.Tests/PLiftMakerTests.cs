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
		private ViewTemplate _viewTemplate;
		private List<string> _writingSystemIds;

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			_maker = new PLiftMaker();
			_outputPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = false;
		}

		[Test, Ignore("broken")]
		public void EntryMakeItToXHtml()
		{
			string xmlForEntries = @"<entry id='foo1'><lexical-unit><form lang='v'><text>fooOne</text></form></lexical-unit></entry>";

			using (Db4oProjectSetupForTesting projectSetup = new Db4oProjectSetupForTesting(xmlForEntries))
			{
				Lexicon.Init(projectSetup._recordListManager);
				PLiftMaker maker = new PLiftMaker();
				string outputPath = Path.Combine(projectSetup._project.PathToExportDirectory, projectSetup._project.Name + ".xhtml");
				maker.MakeXHtmlFile(outputPath, projectSetup._recordListManager, projectSetup._project);
				Assert.IsTrue(File.ReadAllText(outputPath).Contains("<span class=\"v\">fooOne"));
			}
		}

		[Test, Ignore("not a real test")]
		public void MakePLiftForBiatah2()
		{
			using (WeSay.Project.WeSayWordsProject p = new WeSayWordsProject())
			{
				p.LoadFromProjectDirectoryPath(@"E:\Users\John\Documents\WeSay\biatah");

				using (
					Db4oRecordListManager recordListManager =
						new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), p.PathToDb4oLexicalModelDB))
				{
					Lexicon.Init(recordListManager);

					IHomographCalculator homographCalculator =
						new HomographCalculator(recordListManager, p.DefaultViewTemplate.HeadwordWritingSytem);

					PLiftMaker maker = new PLiftMaker();
					IEnumerable<LexEntry> entries = Lexicon.GetAllEntriesSortedByHeadword(p.HeadWordWritingSystem);
					Db4oLexEntryFinder finder = new Db4oLexEntryFinder(recordListManager.DataSource);
					string path = maker.MakePLiftTempFile(entries, p.DefaultViewTemplate, homographCalculator, finder);
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