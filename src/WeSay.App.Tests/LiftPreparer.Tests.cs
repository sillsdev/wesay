using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using LiftIO;
using NUnit.Framework;
using Palaso.Progress;
using WeSay.Project;
using WeSay.Project.Tests;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class LiftPreparerTests
	{
		[SetUp]
		public void Setup()
		{
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = false;
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
			using (ProjectDirectorySetupForTesting dir = new ProjectDirectorySetupForTesting(string.Empty, "0.10"))
			{
				using (WeSayWordsProject project = dir.CreateLoadedProject())
				{
					LiftPreparer preparer = new LiftPreparer(project);
					Assert.IsTrue(preparer.MigrateIfNeeded(), "MigrateIfNeeded Failed");
					Assert.AreEqual(Validator.LiftVersion, Validator.GetLiftVersion(dir.PathToLiftFile));
				}
			}
		}

		[Test]
		public void MigrateIfNeeded_LiftIsLockedByProject_LockedAgainAfterMigration()
		{
			using (ProjectDirectorySetupForTesting dir = new ProjectDirectorySetupForTesting(string.Empty, "0.10"))
			{
				using (WeSayWordsProject proj = dir.CreateLoadedProject())
				{
					proj.LockLift();
					LiftPreparer preparer = new LiftPreparer(proj);
					Assert.IsTrue(preparer.MigrateIfNeeded(), "MigrateIfNeeded Failed");
					Assert.IsTrue(proj.LiftIsLocked);
				}
			}
		}

		[Test]
		public void MigrateIfNeeded_AlreadyCurrentLift_LiftUntouched()
		{
			using (ProjectDirectorySetupForTesting dir = new ProjectDirectorySetupForTesting(string.Empty, Validator.LiftVersion))
			{
				using (WeSayWordsProject project = dir.CreateLoadedProject())
				{
					DateTime startModTime = File.GetLastWriteTimeUtc(dir.PathToLiftFile);
					LiftPreparer preparer = new LiftPreparer(project);
					Assert.IsTrue(preparer.MigrateIfNeeded(), "MigrateIfNeeded Failed");
					DateTime finishModTime = File.GetLastWriteTimeUtc(dir.PathToLiftFile);
					Assert.AreEqual(startModTime, finishModTime);
				}
			}
		}

		[Test]
		public void PopulateDefinitions_EmptyLift()
		{
			XmlDocument dom =  GetTransformedDom("");
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
			ExpectSingleInstanceWithInnerXml(dom, "lift/entry/sense/definition/form[@lang='en']/text", "one; two");
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
			ExpectSingleInstanceWithInnerXml(dom, "lift/entry/sense/definition/form[@lang='a']/text", "a definition");
			ExpectSingleInstanceWithInnerXml(dom, "lift/entry/sense/definition/form[@lang='b']/text", "b definition");
			ExpectSingleInstanceWithInnerXml(dom, "lift/entry/sense/definition/form[@lang='c']/text", "c gloss");
		}

		private void Expect(XmlDocument dom, string xpath, int expectedCount)
		{
			Assert.AreEqual(expectedCount, dom.SelectNodes(xpath).Count);
		}

		private void ExpectSingleInstanceWithInnerXml(XmlDocument dom, string xpath, string expectedValue)
		{
			Assert.AreEqual(1, dom.SelectNodes(xpath).Count);
			Assert.AreEqual(expectedValue, dom.SelectNodes(xpath)[0].InnerXml);
		}

		private XmlDocument GetTransformedDom(string entriesXml)
		{
			XmlDocument doc = new XmlDocument();
			using (Project.Tests.ProjectDirectorySetupForTesting pd = new ProjectDirectorySetupForTesting(entriesXml))
			{
				using (WeSayWordsProject project = pd.CreateLoadedProject())
				{
					LiftPreparer preparer = new LiftPreparer(project);
					string outputPath = preparer.PopulateDefinitions(project.PathToLiftFile);
					Assert.IsTrue(File.Exists(outputPath));
					doc.Load(outputPath);
			   }
			}
			return doc;
		}
	}
}
