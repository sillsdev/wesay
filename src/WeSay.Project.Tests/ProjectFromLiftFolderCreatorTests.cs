using System;
using System.IO;
using NUnit.Framework;
using SIL.DictionaryServices.Model;
using SIL.TestUtilities;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.TestUtilities;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class ProjectFromLiftFolderCreatorTests
	{
		private class TestEnvironment : IDisposable
		{
			private readonly TemporaryFolder _folder;
			private TempLiftFile _liftFile;
			private IWritingSystemRepository _writingSystems;

			public TestEnvironment()
			{
				ErrorReport.IsOkToInteractWithUser = false;
				Sldr.Initialize(true);
				_folder = new TemporaryFolder("ProjectFromLiftFolderCreatorTests");
			}

			public IWritingSystemRepository WritingSystems
			{
				get
				{
					return _writingSystems ?? (_writingSystems = LdmlInFolderWritingSystemRepository.Initialize(
						WritingSystemsPath));
				}
			}

			private string WritingSystemsPath
			{
				get { return Path.Combine(_folder.Path, "WritingSystems"); }
			}

			public string LiftFilePath
			{
				get
				{
					return _liftFile == null ? String.Empty : _liftFile.Path;
				}
			}

			public void Dispose()
			{
				if (_liftFile != null)
				{
					_liftFile.Dispose();
				}
				_folder.Dispose();
				Sldr.Cleanup();
			}

			public void CreateLiftFile(string content, string version)
			{
				_liftFile = new TempLiftFile(_folder, content, version);
			}

		}

		[Test]
		public void SetWritingSystemsForFields_LiftFileContainsWritingsystemsForWhichThereIsNoDefinition_TellsUser()
		{
			using (var e = new TestEnvironment())
			{
				e.CreateLiftFile(
					@"<entry id='foo'>
						<lexical-unit>
							<form lang='qaa-x-blah'><text></text></form>
							<form lang='en'><text></text></form>
						</lexical-unit>
					  </entry>",
					  "0.12"
				);
				var collection = e.WritingSystems;
				collection.Set(new WritingSystemDefinition(WritingSystemsIdsForTests.OtherIdForTest));
				collection.Set(new WritingSystemDefinition(WritingSystemsIdsForTests.AnalysisIdForTest));
				var vt = ViewTemplate.MakeMasterTemplate(collection);
				//put one guy in there already

				Assert.IsFalse(collection.Contains("qaa-x-blah"));
				var creator = new ProjectFromLiftFolderCreator(e.LiftFilePath, vt, collection);

				Assert.Throws<ErrorReport.ProblemNotificationSentToUserException>(
					 creator.SetWritingSystemsForFields
				 );

				Assert.IsTrue(collection.Contains("qaa-x-blah"));
			}
		}

		[Test]
		public void SetWritingSystemsForFields_LiftFileContainsWritingsystemNamed_xspec_xspecIsAddedToWritingSystems()
		{
			using (var e = new TestEnvironment())
			{
				e.CreateLiftFile(
					@"<entry id='foo'>
						<lexical-unit>
							<form lang='x-spec'><text></text></form>
							<form lang='en'><text></text></form>
						</lexical-unit>
					</entry>",
					"0.12"
				);
				var writingSystems = e.WritingSystems;
				writingSystems.Set(new WritingSystemDefinition(WritingSystemsIdsForTests.OtherIdForTest));
				writingSystems.Set(new WritingSystemDefinition(WritingSystemsIdsForTests.AnalysisIdForTest));
				var vt = ViewTemplate.MakeMasterTemplate(writingSystems);
				//put one guy in there already

				var creator = new ProjectFromLiftFolderCreator(e.LiftFilePath, vt, writingSystems);
				creator.SetWritingSystemsForFields();
				Assert.IsTrue(writingSystems.Contains("x-spec"));
			}
		}

		[Test]
		public void SetWritingSystemsForFields_Normal_FixesUpWritingSystemsForFields()
		{
			// Private use subtags can be 2-8 characters in length
			using (var e = new TestEnvironment())
			{
				e.CreateLiftFile(
					@"<entry id='foo'>
						<lexical-unit>
							<form lang='qaa-x-fromLU'><text></text></form>
						</lexical-unit>
						<sense	id='***_a86d6759-f9ea-4710-a818-2b4be9b81c98'>
							<gloss lang='qaa-x-fromGl'>
								<text>meaning</text>
							</gloss>
							<definition>
								<form lang='qaa-x-fromDef'>
									<text>meaning</text>
								</form>
							</definition>
							<example>
								<form lang='qaa-x-fromEx'>
									<text>example</text>
								</form>
								<translation>
									<form lang='qaa-x-fromTrns'>
										<text>translation</text>
									</form>
								</translation>
							</example>
						</sense>
					</entry>",
					"0.12"
				);
				var writingSystems = e.WritingSystems;
				writingSystems.Set(new WritingSystemDefinition("en"));
				writingSystems.Set(new WritingSystemDefinition("qaa-x-fromLU"));
				writingSystems.Set(new WritingSystemDefinition("qaa-x-fromGl"));
				writingSystems.Set(new WritingSystemDefinition("qaa-x-fromDef"));
				writingSystems.Set(new WritingSystemDefinition("qaa-x-fromEx"));
				writingSystems.Set(new WritingSystemDefinition("qaa-x-fromTrns"));
				var vt = ViewTemplate.MakeMasterTemplate(writingSystems);

				var creator = new ProjectFromLiftFolderCreator(e.LiftFilePath, vt, writingSystems);
				creator.SetWritingSystemsForFields();
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, WritingSystemsIdsForTests.OtherIdForTest);
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.Citation, WritingSystemsIdsForTests.OtherIdForTest);
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, WritingSystemsIdsForTests.OtherIdForTest);

				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "qaa-x-fromLU");
				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.Citation, "qaa-x-fromLU");
				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, "qaa-x-fromLU");

				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "qaa-x-fromLU");
				AssertFieldHasWritingSystem(vt, LexSense.WellKnownProperties.Gloss, "qaa-x-fromGl");
				AssertFieldHasWritingSystem(vt, LexSense.WellKnownProperties.Definition, "qaa-x-fromDef");
				AssertFieldLacksWritingSystem(vt, LexExampleSentence.WellKnownProperties.ExampleSentence, "v");
				AssertFieldHasWritingSystem(vt, LexExampleSentence.WellKnownProperties.ExampleSentence, "qaa-x-fromEx");
				AssertFieldHasWritingSystem(vt, LexExampleSentence.WellKnownProperties.Translation, "qaa-x-fromTrns");

				Assert.IsTrue(writingSystems.Contains("en"));

			}
		}

		[Test]
		public void SetWritingSystemsForFields_LiftIsEmpty_Survives()
		{
			using (var e = new TestEnvironment())
			{
				e.CreateLiftFile("", "0.12");
				var writingSystems = e.WritingSystems;
				writingSystems.Set(new WritingSystemDefinition(WritingSystemsIdsForTests.OtherIdForTest));
				writingSystems.Set(new WritingSystemDefinition(WritingSystemsIdsForTests.AnalysisIdForTest));
				var vt = ViewTemplate.MakeMasterTemplate(writingSystems);

				var creator = new ProjectFromLiftFolderCreator(e.LiftFilePath, vt, writingSystems);
				creator.SetWritingSystemsForFields();
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, WritingSystemsIdsForTests.OtherIdForTest);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.Citation, WritingSystemsIdsForTests.OtherIdForTest);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, WritingSystemsIdsForTests.OtherIdForTest);
				Assert.IsTrue(writingSystems.Contains(WritingSystemsIdsForTests.OtherIdForTest));
				Assert.IsTrue(writingSystems.Contains(WritingSystemsIdsForTests.AnalysisIdForTest));

			}
		}

		private static void AssertFieldFirstWritingSystem(ViewTemplate vt, string fieldName, string wsId)
		{
			var f = vt.GetField(fieldName);
			Assert.AreEqual(wsId, f.WritingSystemIds[0]);
		}

		private static void AssertFieldHasWritingSystem(ViewTemplate vt, string fieldName, string wsId)
		{
			var f = vt.GetField(fieldName);
			Assert.IsTrue(f.WritingSystemIds.Contains(wsId));
		}

		private static void AssertFieldLacksWritingSystem(ViewTemplate vt, string fieldName, string wsId)
		{
			var f = vt.GetField(fieldName);
			Assert.IsFalse(f.WritingSystemIds.Contains(wsId));
		}
	}
}
