using System;
using System.IO;
using NUnit.Framework;
using Palaso.DictionaryServices.Model;
using Palaso.Reporting;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.ConfigTool.NewProjectCreation;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace WeSay.ConfigTool.Tests.NewProjectCreation
{
	[TestFixture]
	public class ProjectFromFlexCreatorTests
	{

		[Test]
		public void SetWritingSystemsForFields_LiftFileContainsWritingsystemsForWhichThereIsNoDefinition_TellsUser()
		{
			ErrorReport.IsOkToInteractWithUser = false;

			using (var tempFolder = new TemporaryFolder("ProjectFromFLExCreatorTests"))
			using (var lift = new TempLiftFile(@"
				<entry id='foo'>
					<lexical-unit>
						<form lang='aaa'><text></text></form>
						<form lang='en'><text></text></form>
					</lexical-unit>
				</entry>", "0.12"))
			{
				var collection = new LdmlInFolderWritingSystemRepository(tempFolder.Path);
				collection.Set(WritingSystemDefinition.FromLanguage(WritingSystemInfo.IdForUnknownVernacular));
				collection.Set(WritingSystemDefinition.FromLanguage("en"));
				var vt = ViewTemplate.MakeMasterTemplate(collection);
				//put one guy in there already
				int originalCount = collection.Count; // collection.Count;

				Assert.IsFalse(collection.Contains("aaa"));

				Assert.Throws<ErrorReport.ProblemNotificationSentToUserException>(
					() => ProjectFromFLExCreator.SetWritingSystemsForFields(lift.Path, vt, collection)
					);

				Assert.IsTrue(collection.Contains("aaa"));
			}
		}

		[Test]
		public void SetWritingSystemsForFields_LiftFileContainsPrivateUse_TellsUser()
		{
			ErrorReport.IsOkToInteractWithUser = false;

			using (var tempFolder = new TemporaryFolder("ProjectFromFLExCreatorTests"))
			using (var lift = new TempLiftFile(@"
				<entry id='foo'>
					<lexical-unit>
						<form lang='aaa-fonipa-x-etic'><text></text></form>
						<form lang='en'><text></text></form>
					</lexical-unit>
				</entry>", "0.12"))
			{
				var collection = new LdmlInFolderWritingSystemRepository(tempFolder.Path);
				collection.Set(WritingSystemDefinition.FromLanguage(WritingSystemInfo.IdForUnknownVernacular));
				collection.Set(WritingSystemDefinition.FromLanguage("en"));
				var vt = ViewTemplate.MakeMasterTemplate(collection);
				//put one guy in there already
				int originalCount = collection.Count; // collection.Count;

				Assert.IsFalse(collection.Contains("aaa-fonipa-x-etic"));

				Assert.Throws<ErrorReport.ProblemNotificationSentToUserException>(
					() => ProjectFromFLExCreator.SetWritingSystemsForFields(lift.Path, vt, collection)
					);

				Assert.IsTrue(collection.Contains("aaa-fonipa-x-etic"));
			}
		}


		[Test]
		public void SetWritingSystemsForFields_LiftFileContainsWritingsystemNamed_xspec_xspecIsAddedToWritingSystems()
		{
			using (var tempFolder = new TemporaryFolder("ProjectFromFLExCreatorTests"))
			using (var lift = new TempLiftFile(@"
				<entry id='foo'>
					<lexical-unit>
						<form lang='x-spec'><text></text></form>
						<form lang='en'><text></text></form>
					</lexical-unit>
				</entry>", "0.12"))
			{
				var collection = new LdmlInFolderWritingSystemRepository(tempFolder.Path);
				collection.Set(WritingSystemDefinition.FromLanguage(WritingSystemInfo.IdForUnknownVernacular));
				collection.Set(WritingSystemDefinition.FromLanguage("en"));
				var vt = ViewTemplate.MakeMasterTemplate(collection);
				//put one guy in there already
				int originalCount = collection.Count;// collection.Count;

				ProjectFromFLExCreator.SetWritingSystemsForFields(lift.Path, vt, collection);
				Assert.IsTrue(collection.Contains("qaa-x-spec"));
			}
		}

		[Test]
		public void SetWritingSystemsForFields_Normal_FixesUpWritingSystemsForFields()
		{
			using (var tempFolder = new TemporaryFolder("ProjectFromFLExCreatorTests"))
			using (var lift = new TempLiftFile(@"
				<entry id='foo'>
					<lexical-unit>
						<form lang='aaa-x-fromLU'><text></text></form>
					</lexical-unit>
				<sense	id='***_a86d6759-f9ea-4710-a818-2b4be9b81c98'>
				  <gloss lang='aab-x-fromGloss'>
					  <text>meaning</text>
				  </gloss>
				  <definition>
					<form lang='aac-x-fromDef'>
					  <text>meaning</text>
					</form>
				  </definition>
				  <example>
					<form lang='aad-x-fromExample'>
					  <text>example</text>
					</form>
					<translation>
					  <form lang='aae-x-fromTrans'>
						<text>translation</text>
					  </form>
					</translation>
				  </example>
				</sense>
		</entry>", "0.12"))
			{
				var collection = new LdmlInFolderWritingSystemRepository(tempFolder.Path);
				collection.Set(WritingSystemDefinition.FromLanguage(WritingSystemInfo.IdForUnknownVernacular));
				collection.Set(WritingSystemDefinition.FromLanguage("one"));
				collection.Set(WritingSystemDefinition.FromRFC5646Subtags("aaa", String.Empty, String.Empty, "x-fromLU"));
				collection.Set(WritingSystemDefinition.FromRFC5646Subtags("aab", String.Empty, String.Empty, "x-fromGloss"));
				collection.Set(WritingSystemDefinition.FromRFC5646Subtags("aac", String.Empty, String.Empty, "x-fromDef"));
				collection.Set(WritingSystemDefinition.FromRFC5646Subtags("aad", String.Empty, String.Empty, "x-fromExample"));
				collection.Set(WritingSystemDefinition.FromRFC5646Subtags("aae", String.Empty, String.Empty, "x-fromTrans"));
				var vt = ViewTemplate.MakeMasterTemplate(collection);

				ProjectFromFLExCreator.SetWritingSystemsForFields(lift.Path, vt, collection);
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "one");
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.Citation, "one");
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, "one");

				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "aaa-x-fromLU");
				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.Citation, "aaa-x-fromLU");
				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, "aaa-x-fromLU");

				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "aaa-x-fromLU");
				AssertFieldHasWritingSystem(vt, LexSense.WellKnownProperties.Gloss, "aab-x-fromGloss");
				AssertFieldHasWritingSystem(vt, LexSense.WellKnownProperties.Definition, "aac-x-fromDef");
				AssertFieldLacksWritingSystem(vt, LexExampleSentence.WellKnownProperties.ExampleSentence, WritingSystemInfo.IdForUnknownVernacular);
				AssertFieldHasWritingSystem(vt, LexExampleSentence.WellKnownProperties.ExampleSentence, "aad-x-fromExample");
				AssertFieldHasWritingSystem(vt, LexExampleSentence.WellKnownProperties.Translation, "aae-x-fromTrans");

				Assert.IsTrue(collection.Contains("one"));

			}
		}

		[Test]
		public void SetWritingSystemsForFields_LiftIsEmpty_Survives()
		{
			using (var tempFolder = new TemporaryFolder("ProjectFromFLExCreatorTests"))
			using (var lift = new TempLiftFile(@"
			 ", "0.12"))
			{
				var collection = new LdmlInFolderWritingSystemRepository(tempFolder.Path);
				collection.Set(WritingSystemDefinition.FromLanguage(WritingSystemInfo.IdForUnknownVernacular));
				collection.Set(WritingSystemDefinition.FromLanguage(WritingSystemInfo.IdForUnknownAnalysis));
				var vt = ViewTemplate.MakeMasterTemplate(collection);

				ProjectFromFLExCreator.SetWritingSystemsForFields(lift.Path, vt, collection);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, WritingSystemInfo.IdForUnknownVernacular);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.Citation, WritingSystemInfo.IdForUnknownVernacular);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, WritingSystemInfo.IdForUnknownVernacular);
				Assert.IsTrue(collection.Contains(WritingSystemInfo.IdForUnknownVernacular));
				Assert.IsTrue(collection.Contains(WritingSystemInfo.IdForUnknownAnalysis));

 }
		}

		private static void AssertFieldFirstWritingSystem(ViewTemplate vt, string fieldName, string wsId)
		{
			var f = vt.GetField(fieldName);
			Assert.AreEqual(wsId,f.WritingSystemIds[0]);
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

		[Test]
		public void Create_JustLiftFound_CreatesProjectAndCopiesLIFT()
		{
			using(var testDir = new TemporaryFolder("NormalSituation_CreatesProject"))
			{
				using (var lift = new TempLiftFile(testDir,  "<entry id='foo'></entry>", "0.12"))
				{
					var targetDir = testDir.Combine("target");

					Assert.IsTrue(ProjectFromFLExCreator.Create(targetDir, lift.Path));
					var projectName = Path.GetFileNameWithoutExtension(targetDir);
					Assert.IsTrue(Directory.Exists(targetDir));
					var liftName = projectName + ".lift";
					AssertFileExistsInTargetDir(targetDir, liftName);
					AssertFileExistsInTargetDir(targetDir, projectName+".WeSayConfig");

					var liftPath = Path.Combine(targetDir, liftName);
					AssertThatXmlIn.File(liftPath).HasAtLeastOneMatchForXpath("//entry[@id='foo']");
				}
			}
		}

		[Test]
		public void Create_LiftAndRangesFound_CopiesRanges()
		{
			using (var testDir = new TemporaryFolder("NormalSituation_CreatesProject"))
			{
				using (var lift = new TempLiftFile(testDir, "", "0.12"))
				{
					File.WriteAllText(lift.Path+"-ranges", "hello");
					var targetDir = testDir.Combine("target");
					Assert.IsTrue(ProjectFromFLExCreator.Create(targetDir, lift.Path));

					var projectName = Path.GetFileNameWithoutExtension(targetDir);
					 AssertFileExistsInTargetDir(targetDir, projectName + ".lift-ranges");
				}
			}
		}

		[Test]
		public void Create_LdmlWritingSystemsFound_CopiesWritingSystems()
		{
			string ldmlText =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<ldml>
	<identity>
		<version number="""" />
		<generation date=""0001-01-01T00:00:00"" />
		<language type=""qaa"" />
	</identity>
	<collations />
	<special xmlns:palaso=""urn://palaso.org/ldmlExtensions/v1"">
		<palaso:abbreviation value=""test"" />
		<palaso:defaultFontFamily value=""Arial"" />
		<palaso:defaultFontSize value=""12"" />
	</special>
</ldml>";

			using (var testDir = new TemporaryFolder("NormalSituation_CreatesProject"))
			{
				var lift = new TempLiftFile(testDir, "", "0.12");
				using (var ldmlFile = new TempFile(ldmlText))
				{
					ldmlFile.MoveTo(Path.Combine(testDir.Path, "qaa.ldml"));
					var targetDir = testDir.Combine("target");
					Assert.IsTrue(ProjectFromFLExCreator.Create(targetDir, lift.Path));
					AssertFileExistsInTargetDir(Path.Combine(targetDir, "WritingSystems"), "qaa.ldml");
				}
			}
		}

		private static void AssertFileExistsInTargetDir(string targetDir, string fileName)
		{
			Assert.IsTrue(File.Exists(Path.Combine(targetDir, fileName)));
		}


		[Test]
		public void Create_SourceLiftLocked_GivesMessageReturnsFalse()
		{
			using(var lift = new TempLiftFile(""))
			{
				using (var stream = File.OpenWrite(lift.Path))
				{
					var targetDir = Path.Combine(Path.GetTempPath(), "ProjectFromFLExCreatorTests");

					using(var x = new ErrorReport.NonFatalErrorReportExpected())
					{
						Assert.IsFalse(ProjectFromFLExCreator.Create(targetDir, lift.Path));
					}
				}
			}
		}

	}
}
