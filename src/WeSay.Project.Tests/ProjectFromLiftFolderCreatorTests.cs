using NUnit.Framework;
using Palaso.DictionaryServices.Model;
using Palaso.Reporting;
using Palaso.TestUtilities;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class ProjectFromLiftFolderCreatorTests
	{
		[Test]
		public void SetWritingSystemsForFields_LiftFileContainsWritingsystemsForWhichThereIsNoDefinition_TellsUser()
		{
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = false;

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

				Assert.IsFalse(collection.ContainsKey("blah"));
				var creator = new ProjectFromLiftFolderCreator(lift.Path, vt, collection);

				Assert.Throws<ErrorReport.ProblemNotificationSentToUserException>(
					 () => creator.SetWritingSystemsForFields()
				 );

				Assert.IsTrue(collection.ContainsKey("blah"));
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
				var writingSystems = new WritingSystemCollection();
				writingSystems.Add(WritingSystem.IdForUnknownVernacular/*v*/, new WritingSystem());
				writingSystems.Add("en", new WritingSystem());
				var vt = ViewTemplate.MakeMasterTemplate(writingSystems);
				//put one guy in there already
				int originalCount = writingSystems.Count;// collection.Count;

				var creator = new ProjectFromLiftFolderCreator(lift.Path, vt, writingSystems);
				creator.SetWritingSystemsForFields();
				Assert.IsTrue(writingSystems.ContainsKey("x-spec"));
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
				var writingSystems = new WritingSystemCollection();
				writingSystems.Add("en", new WritingSystem());
				writingSystems.Add("fromLU", new WritingSystem());
				writingSystems.Add("fromGloss", new WritingSystem());
				writingSystems.Add("fromDef", new WritingSystem());
				writingSystems.Add("fromExample", new WritingSystem());
				writingSystems.Add("fromTrans", new WritingSystem());
				var vt = ViewTemplate.MakeMasterTemplate(writingSystems);

				var creator = new ProjectFromLiftFolderCreator(lift.Path, vt, writingSystems);
				creator.SetWritingSystemsForFields();
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

				Assert.IsTrue(writingSystems.ContainsKey("en"));

			}
		}

		[Test]
		public void SetWritingSystemsForFields_LiftIsEmpty_Survives()
		{
			using (var lift = new TempLiftFile(@"
			 ", "0.12"))
			{
				var writingSystems = new WritingSystemCollection();
				writingSystems.Add(WritingSystem.IdForUnknownAnalysis, new WritingSystem());
				writingSystems.Add(WritingSystem.IdForUnknownVernacular, new WritingSystem());
				var vt = ViewTemplate.MakeMasterTemplate(writingSystems);

				var creator = new ProjectFromLiftFolderCreator(lift.Path, vt, writingSystems);
				creator.SetWritingSystemsForFields();
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.LexicalUnit, WritingSystem.IdForUnknownVernacular);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.Citation, WritingSystem.IdForUnknownVernacular);
				AssertFieldHasWritingSystem(vt, LexEntry.WellKnownProperties.BaseForm, WritingSystem.IdForUnknownVernacular);
				Assert.IsTrue(writingSystems.ContainsKey(WritingSystem.IdForUnknownVernacular));
				Assert.IsTrue(writingSystems.ContainsKey(WritingSystem.IdForUnknownAnalysis));

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
