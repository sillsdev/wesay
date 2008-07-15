using System;
using System.IO;
using System.Xml;
using LiftIO.Validation;
using NUnit.Framework;
using Palaso.Reporting;
using WeSay.App.Migration;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.Project.Tests;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class LiftPreparerTests
	{
		private LexEntryRepository _lexEntryRepository;
		private string _filePath;

		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();
			_lexEntryRepository = new LexEntryRepository(_filePath);
			ErrorReport.IsOkToInteractWithUser = false;
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void MigrateIfNeeded_GivenLiftVersionPoint10_LiftFileHasCurrentLiftVersionNumber()
		{
			//nb: most migration testing is done in the LiftIO library where the actual
			//  migration happens.  So here we're ensuring that the migration mechanism was
			//  triggered, and that the process left us with a modified (but not renamed)
			//  lift file.
			//nb: 0.10 was the first version where we started provinding a migration path.
			//FLEx support for Lift started with 0.12
			using (
					ProjectDirectorySetupForTesting dir =
							new ProjectDirectorySetupForTesting(string.Empty, "0.10"))
			{
				using (WeSayWordsProject project = dir.CreateLoadedProject())
				{
					LiftPreparer preparer = new LiftPreparer(project, _lexEntryRepository);
					Assert.IsTrue(preparer.MigrateIfNeeded(), "MigrateIfNeeded Failed");
					Assert.AreEqual(Validator.LiftVersion,
									Validator.GetLiftVersion(dir.PathToLiftFile));
				}
			}
		}

		[Test]
		public void MigrateIfNeeded_LiftIsLockedByProject_LockedAgainAfterMigration()
		{
			using (
					ProjectDirectorySetupForTesting dir =
							new ProjectDirectorySetupForTesting(string.Empty, "0.10"))
			{
				using (WeSayWordsProject proj = dir.CreateLoadedProject())
				{
					proj.LockLift();
					LiftPreparer preparer = new LiftPreparer(proj, _lexEntryRepository);
					Assert.IsTrue(preparer.MigrateIfNeeded(), "MigrateIfNeeded Failed");
					Assert.IsTrue(proj.LiftIsLocked);
				}
			}
		}

		[Test]
		public void MigrateIfNeeded_AlreadyCurrentLift_LiftUntouched()
		{
			using (
					ProjectDirectorySetupForTesting dir =
							new ProjectDirectorySetupForTesting(string.Empty, Validator.LiftVersion)
					)
			{
				using (WeSayWordsProject project = dir.CreateLoadedProject())
				{
					DateTime startModTime = File.GetLastWriteTimeUtc(dir.PathToLiftFile);
					LiftPreparer preparer = new LiftPreparer(project, _lexEntryRepository);
					Assert.IsTrue(preparer.MigrateIfNeeded(), "MigrateIfNeeded Failed");
					DateTime finishModTime = File.GetLastWriteTimeUtc(dir.PathToLiftFile);
					Assert.AreEqual(startModTime, finishModTime);
				}
			}
		}

		[Test]
		public void PopulateDefinitions_EmptyLift()
		{
			XmlDocument dom = GetTransformedDom("");
			Expect(dom, "lift", 1);
		}

		[Test]
		public void PopulateDefinitions_GetsDefinitionWithConcatenatedGlosses()
		{
			string entriesXml =
					@"<entry id='foo1'>
						<sense>
							<gloss lang='en'>
								<text>one</text>
							</gloss>
							<gloss lang='en'>
								<text>two</text>
							</gloss>
						</sense>
					</entry>";
			XmlDocument dom = GetTransformedDom(entriesXml);
			Expect(dom, "lift/entry/sense/gloss", 2);
			Expect(dom, "lift/entry/sense/definition", 1);
			ExpectSingleInstanceWithInnerXml(dom,
											 "lift/entry/sense/definition/form[@lang='en']/text",
											 "one; two");
		}

		[Test]
		public void PopulateDefinitions_MergesInWritingSystemsWithExistingDefinition()
		{
			string entriesXml =
					@"<entry id='foo1'>
						<sense>
							<definition>
								<form lang='a'>
									<text>a definition</text>
								</form>
								<form lang='b'>
									<text>b definition</text>
								</form>
							</definition>
							<gloss lang='b'>
								<text>SHOULD NOT SEE IN DEF</text>
							</gloss>
							<gloss lang='c'>
								<text>c gloss</text>
							</gloss>
						</sense>
					</entry>";
			XmlDocument dom = GetTransformedDom(entriesXml);
			Expect(dom, "lift/entry/sense/gloss", 2);
			Expect(dom, "lift/entry/sense/definition", 1);
			ExpectSingleInstanceWithInnerXml(dom,
											 "lift/entry/sense/definition/form[@lang='a']/text",
											 "a definition");
			ExpectSingleInstanceWithInnerXml(dom,
											 "lift/entry/sense/definition/form[@lang='b']/text",
											 "b definition");
			ExpectSingleInstanceWithInnerXml(dom,
											 "lift/entry/sense/definition/form[@lang='c']/text",
											 "c gloss");
		}

		private static void Expect(XmlNode dom, string xpath, int expectedCount)
		{
			Assert.AreEqual(expectedCount, dom.SelectNodes(xpath).Count);
		}

		private static void ExpectSingleInstanceWithInnerXml(XmlNode dom,
															 string xpath,
															 string expectedValue)
		{
			Assert.AreEqual(1, dom.SelectNodes(xpath).Count);
			Assert.AreEqual(expectedValue, dom.SelectNodes(xpath)[0].InnerXml);
		}

		private static XmlDocument GetTransformedDom(string entriesXml)
		{
			XmlDocument doc = new XmlDocument();
			using (
					ProjectDirectorySetupForTesting pd =
							new ProjectDirectorySetupForTesting(entriesXml))
			{
				using (WeSayWordsProject project = pd.CreateLoadedProject())
				{
					string outputPath = LiftPreparer.PopulateDefinitions(project.PathToLiftFile);
					Assert.IsTrue(File.Exists(outputPath));
					doc.Load(outputPath);
				}
			}
			return doc;
		}
	}
}