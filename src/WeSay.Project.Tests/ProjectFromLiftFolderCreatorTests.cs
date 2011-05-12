using System;
using System.IO;
using NUnit.Framework;
using Palaso.DictionaryServices.Model;
using Palaso.Reporting;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class ProjectFromLiftFolderCreatorTests
	{
		private class TestEnvironment : IDisposable
		{
			private TemporaryFolder _folder;
			private TempLiftFile _liftFile;
			private IWritingSystemRepository _writingSystems;

			public TestEnvironment()
			{
				ErrorReport.IsOkToInteractWithUser = false;
				_folder = new TemporaryFolder("ProjectFromLiftFolderCreatorTests");
			}

			public IWritingSystemRepository WritingSystems
			{
				get {
					if (_writingSystems == null)
					{
						_writingSystems = new LdmlInFolderWritingSystemRepository(WritingSystemsPath);
					}
					return _writingSystems;
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
				collection.Set(WritingSystemDefinition.Parse(WritingSystemInfo.OtherIdForTest));
				collection.Set(WritingSystemDefinition.Parse(WritingSystemInfo.AnalysisIdForTest));
				var vt = ViewTemplate.MakeMasterTemplate(collection);
				//put one guy in there already
				int originalCount = collection.Count;// collection.Count;

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
				writingSystems.Set(WritingSystemDefinition.Parse(WritingSystemInfo.OtherIdForTest));
				writingSystems.Set(WritingSystemDefinition.Parse(WritingSystemInfo.AnalysisIdForTest));
				var vt = ViewTemplate.MakeMasterTemplate(writingSystems);
				//put one guy in there already
				int originalCount = writingSystems.Count;// collection.Count;

				var creator = new ProjectFromLiftFolderCreator(e.LiftFilePath, vt, writingSystems);
				creator.SetWritingSystemsForFields();
				Assert.IsTrue(writingSystems.Contains("x-spec"));
			}
		}

		[Test]
		public void SetWritingSystemsForFields_Normal_FixesUpWritingSystemsForFields()
		{
			using (var e = new TestEnvironment())
			{
				e.CreateLiftFile(
					@"<entry id='foo'>
						<lexical-unit>
							<form lang='qaa-x-fromLU'><text></text></form>
						</lexical-unit>
						<sense	id='***_a86d6759-f9ea-4710-a818-2b4be9b81c98'>
							<gloss lang='qaa-x-fromGloss'>
								<text>meaning</text>
							</gloss>
							<definition>
								<form lang='qaa-x-fromDef'>
									<text>meaning</text>
								</form>
							</definition>
							<example>
								<form lang='qaa-x-fromExample'>
									<text>example</text>
								</form>
								<translation>
									<form lang='qaa-x-fromTrans'>
										<text>translation</text>
									</form>
								</translation>
							</example>
						</sense>
					</entry>",
					"0.12"
				);
				var writingSystems = e.WritingSystems;
				writingSystems.Set(WritingSystemDefinition.Parse("en"));
				writingSystems.Set(WritingSystemDefinition.Parse("qaa-x-fromLU"));
				writingSystems.Set(WritingSystemDefinition.Parse("qaa-x-fromGloss"));
				writingSystems.Set(WritingSystemDefinition.Parse("qaa-x-fromDef"));
				writingSystems.Set(WritingSystemDefinition.Parse("qaa-x-fromExample"));
				writingSystems.Set(WritingSystemDefinition.Parse("qaa-x-fromTrans"));
				var vt = ViewTemplate.MakeMasterTemplate(writingSystems);

				var creator = new ProjectFromLiftFolderCreator(e.LiftFilePath, vt, writingSystems);
				creator.SetWritingSystemsForFields();
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, WritingSystemInfo.OtherIdForTest);
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.Citation, WritingSystemInfo.OtherIdForTest);
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, WritingSystemInfo.OtherIdForTest);

				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "qaa-x-fromLU");
				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.Citation, "qaa-x-fromLU");
				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, "qaa-x-fromLU");

				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "qaa-x-fromLU");
				AssertFieldHasWritingSystem(vt, LexSense.WellKnownProperties.Gloss, "qaa-x-fromGloss");
				AssertFieldHasWritingSystem(vt, LexSense.WellKnownProperties.Definition, "qaa-x-fromDef");
				AssertFieldLacksWritingSystem(vt, LexExampleSentence.WellKnownProperties.ExampleSentence, "v");
				AssertFieldHasWritingSystem(vt, LexExampleSentence.WellKnownProperties.ExampleSentence, "qaa-x-fromExample");
				AssertFieldHasWritingSystem(vt, LexExampleSentence.WellKnownProperties.Translation, "qaa-x-fromTrans");

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
				writingSystems.Set(WritingSystemDefinition.Parse(WritingSystemInfo.OtherIdForTest));
				writingSystems.Set(WritingSystemDefinition.Parse(WritingSystemInfo.AnalysisIdForTest));
				var vt = ViewTemplate.MakeMasterTemplate(writingSystems);

				var creator = new ProjectFromLiftFolderCreator(e.LiftFilePath, vt, writingSystems);
				creator.SetWritingSystemsForFields();
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, WritingSystemInfo.OtherIdForTest);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.Citation, WritingSystemInfo.OtherIdForTest);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, WritingSystemInfo.OtherIdForTest);
				Assert.IsTrue(writingSystems.Contains(WritingSystemInfo.OtherIdForTest));
				Assert.IsTrue(writingSystems.Contains(WritingSystemInfo.AnalysisIdForTest));

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
