using System;
using System.IO;
using NUnit.Framework;
using Palaso.DictionaryServices.Model;
using Palaso.Reporting;
using Palaso.TestUtilities;
using WeSay.ConfigTool.NewProjectCreation;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace WeSay.ConfigTool.Tests.NewProjectCreation
{
	[TestFixture]
	public class ProjectFromFLExCreatorTests
	{

		[Test]
		public void SetWritingSystemsForFields_LiftFileContainsWritingsystemsForWhichThereIsNoDefinition_Throws()
		{
			using (var lift = new TempLiftFile(@"
				<entry id='foo'>
					<lexical-unit>
						<form lang='blah'><text></text></form>
						<form lang='en'><text></text></form>
					</lexical-unit>
				</entry>", "0.12"))
			{
				var collection = new WritingSystemCollection();
				collection.Add(WritingSystem.IdForUnknownVernacular/*v*/, new WritingSystem());
				collection.Add("en", new WritingSystem());
				var vt = ViewTemplate.MakeMasterTemplate(collection);
				//put one guy in there already
				int originalCount = collection.Count;// collection.Count;
				ErrorReport.IsOkToInteractWithUser = false;
				Assert.Throws<ErrorReport.ProblemNotificationSentToUserException>(
					() => ProjectFromFLExCreator.SetWritingSystemsForFields(lift.Path, vt, collection)
				);
			}
		}

		[Test]
		public void SetWritingSystemsForFields_LiftFileContainsWritingsystemNamed_xspec_xspecIsAddedToWritingSystems()
		{
			using (var lift = new TempLiftFile(@"
				<entry id='foo'>
					<lexical-unit>
						<form lang='x-spec'><text></text></form>
						<form lang='en'><text></text></form>
					</lexical-unit>
				</entry>", "0.12"))
			{
				var collection = new WritingSystemCollection();
				collection.Add(WritingSystem.IdForUnknownVernacular/*v*/, new WritingSystem());
				collection.Add("en", new WritingSystem());
				var vt = ViewTemplate.MakeMasterTemplate(collection);
				//put one guy in there already
				int originalCount = collection.Count;// collection.Count;

				ProjectFromFLExCreator.SetWritingSystemsForFields(lift.Path, vt, collection);
				Assert.IsTrue(collection.ContainsKey("x-spec"));
			}
		}

		[Test]
		public void SetWritingSystemsForFields_Normal_FixesUpWritingSystemsForFields()
		{
			using (var lift = new TempLiftFile(@"
				<entry id='foo'>
					<lexical-unit>
						<form lang='fromLU'><text></text></form>
					</lexical-unit>
				<sense	id='***_a86d6759-f9ea-4710-a818-2b4be9b81c98'>
				  <gloss lang='fromGloss'>
					  <text>meaning</text>
				  </gloss>
				  <definition>
					<form lang='fromDef'>
					  <text>meaning</text>
					</form>
				  </definition>
				  <example>
					<form lang='fromExample'>
					  <text>example</text>
					</form>
					<translation>
					  <form lang='fromTrans'>
						<text>translation</text>
					  </form>
					</translation>
				  </example>
				</sense>
		</entry>", "0.12"))
			{
				var collection = new WritingSystemCollection();
				collection.Add("en", new WritingSystem());
				collection.Add("fromLU", new WritingSystem());
				collection.Add("fromGloss", new WritingSystem());
				collection.Add("fromDef", new WritingSystem());
				collection.Add("fromExample", new WritingSystem());
				collection.Add("fromTrans", new WritingSystem());
				var vt = ViewTemplate.MakeMasterTemplate(collection);

				ProjectFromFLExCreator.SetWritingSystemsForFields(lift.Path, vt, collection);
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "v");
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.Citation, "v");
				AssertFieldLacksWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, "v");

				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "fromLU");
				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.Citation, "fromLU");
				AssertFieldFirstWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, "fromLU");

				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, "fromLU");
				AssertFieldHasWritingSystem(vt, LexSense.WellKnownProperties.Gloss, "fromGloss");
				AssertFieldHasWritingSystem(vt, LexSense.WellKnownProperties.Definition, "fromDef");
				AssertFieldLacksWritingSystem(vt, LexExampleSentence.WellKnownProperties.ExampleSentence, "v");
				AssertFieldHasWritingSystem(vt, LexExampleSentence.WellKnownProperties.ExampleSentence, "fromExample");
				AssertFieldHasWritingSystem(vt, LexExampleSentence.WellKnownProperties.Translation, "fromTrans");

				Assert.IsTrue(collection.ContainsKey("en"));

			}
		}

		[Test]
		public void SetWritingSystemsForFields_LiftIsEmpty_Survives()
		{
			using (var lift = new TempLiftFile(@"
			 ", "0.12"))
			{
				var collection = new WritingSystemCollection();
				collection.Add(WritingSystem.IdForUnknownAnalysis, new WritingSystem());
				collection.Add(WritingSystem.IdForUnknownVernacular, new WritingSystem());
				var vt = ViewTemplate.MakeMasterTemplate(collection);

				ProjectFromFLExCreator.SetWritingSystemsForFields(lift.Path, vt, collection);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, WritingSystem.IdForUnknownVernacular);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.Citation, WritingSystem.IdForUnknownVernacular);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, WritingSystem.IdForUnknownVernacular);
				Assert.IsTrue(collection.ContainsKey(WritingSystem.IdForUnknownVernacular));
				Assert.IsTrue(collection.ContainsKey(WritingSystem.IdForUnknownAnalysis));

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
		<language type=""test"" />
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
					ldmlFile.MoveTo(Path.Combine(testDir.Path, "test.ldml"));
					var targetDir = testDir.Combine("target");
					Assert.IsTrue(ProjectFromFLExCreator.Create(targetDir, lift.Path));
					AssertFileExistsInTargetDir(Path.Combine(targetDir, "WritingSystems"), "test.ldml");
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
