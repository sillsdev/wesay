using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Db4objects.Db4o;
using LiftIO.Validation;
using NUnit.Framework;
using Palaso.Reporting;
using WeSay.LexicalModel;
using WeSay.LexicalTools;
using WeSay.Project;
using WeSay.Project.Tests;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class LiftPreparerTests
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;
		}

		#endregion

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
			using (ProjectDirectorySetupForTesting pd = new ProjectDirectorySetupForTesting(entriesXml))
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

		[Test]
		public void DoWork_CanGetChanges_QueriesForChanges()
		{
			using (
				ProjectDirectorySetupForTesting dir = new ProjectDirectorySetupForTesting(string.Empty,
																						  Validator.LiftVersion))
			{
				SetupInitialProjectAndCache(dir);

				//now, while the project is closed, we change the lift out from under it

				//change hello to goodbye, remove entry 'two', add entry 'three'
				string newContents ="<entry id='one'><sense><gloss lang='en'><text>goodbye</text></gloss></sense></entry><entry id='three'/>";
				using (WeSayWordsProject project = dir.CreateLoadedProject(newContents,Validator.LiftVersion))
				{
				   // project.MakeRecordListManager();

					project.LockLift();
					Assert.IsTrue(project.LiftChangeDetector.CanProvideChangeRecord);

					LiftPreparer preparer = new LiftPreparer(project);
					//hack: it's a shame that the renderer is brought to life and all, just so the at indexing
					//can happen when we bring in new/editted entries.  That should be architected away (indices
					//should be activatable without activating ui tools (!!!!).  Anyhow, in this test there is
					//actual data already in the db, so we were getting a crash because this was needed but
					//not set.

					RtfRenderer.HeadWordWritingSystemId = project.WritingSystems.TestWritingSystemVernId;

					preparer.MakeCacheAndLiftReady();

					using (
						IObjectContainer db = Db4oFactory.OpenFile(WeSayWordsProject.Project.PathToDb4oLexicalModelDB))
					{
						IList<LexEntry> x = db.Query<LexEntry>();
						Assert.AreEqual(2, x.Count);

						int index = (x[0].Id == "one") ? 0 : 1;
						Assert.AreEqual("one", x[index].Id);
						Assert.AreEqual(1, x[index].Senses.Count);
						Assert.AreEqual("goodbye", ((LexSense) x[index].Senses[0]).Gloss.GetExactAlternative("en"));

						index = (x[0].Id == "three") ? 0 : 1;
						Assert.AreEqual("three", x[index].Id);

						//the above also prove that "two" was removed
					}
					// Assert.AreEqual(ProgressState.StateValue.Finished, _progress.State);
					//  Assert.IsTrue(_log.Contains("partial cache update"));
				}
			}
		}

		private void SetupInitialProjectAndCache(ProjectDirectorySetupForTesting dir)
		{
			using (WeSayWordsProject project = dir.CreateLoadedProject())
			{
				string initial =
					string.Format(
						"<?xml version='1.0' encoding='utf-8'?><lift version='{0}'><entry id='one'><sense><gloss lang='en'><text>hello</text></gloss></sense></entry><entry id='two'/></lift>",
						Validator.LiftVersion);

				File.WriteAllText(project.PathToLiftFile, initial);
				project.LockLift();
				LiftPreparer preparer = new LiftPreparer(project);
				preparer.MakeCacheAndLiftReady();

				using (IObjectContainer db = Db4oFactory.OpenFile(project.PathToDb4oLexicalModelDB))
				{
					IList<LexEntry> x = db.Query<LexEntry>();
					Assert.AreEqual(2, x.Count);
				}
			}
			Lexicon.DeInitialize(false);
		}

		[Test]
		public void MigrateIfNeeded_AlreadyCurrentLift_LiftUntouched()
		{
			using (
				ProjectDirectorySetupForTesting dir = new ProjectDirectorySetupForTesting(string.Empty,
																						  Validator.LiftVersion))
			{
				using (WeSayWordsProject project = dir.CreateLoadedProject())
				{
					DateTime startModTime = File.GetLastWriteTimeUtc(dir.PathToLiftFile);
					LiftPreparer preparer = new LiftPreparer(project);
					Assert.IsTrue(preparer.MigrateIfNeeded(), "MigrateIfNeeded Failed");
					DateTime finishModTime = File.GetLastWriteTimeUtc(dir.PathToLiftFile);
					Assert.AreEqual((object) startModTime, (object) finishModTime);
				}
			}
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
	}
}