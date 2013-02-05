using System;
using System.Collections.Generic;
using NUnit.Framework;
using Palaso.DictionaryServices.Model;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;
using WeSay.Data.Tests;
using WeSay.LexicalModel;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class ViewTemplateTests: IEnumerableBaseTest<Field>
	{
		[SetUp]
		public override void SetUp()
		{
			_enumerable = new ViewTemplate();
			_itemCount = 0;
		}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void Create()
		{
			var viewTemplate = new ViewTemplate();
			Assert.IsNotNull(viewTemplate);
		}

		[Test]
		public void Contains_HasFieldDefinition_True()
		{
			var viewTemplate = PopulateViewTemplate();
			Assert.IsTrue(viewTemplate.Contains("field1"));
		}

		[Test]
		public void Contains_DoesNotHaveFieldDefinition_False()
		{
			var viewTemplate = PopulateViewTemplate();
			Assert.IsFalse(viewTemplate.Contains("none"));
		}

		[Test]
		public void Index_HasFieldDefinition_FieldDefinition()
		{
			var viewTemplate = PopulateViewTemplate();
			Assert.IsNotNull(viewTemplate["field1"]);
		}

		[Test]
		public void Index_DoesNotHaveFieldDefinition_Throws()
		{
			var viewTemplate = PopulateViewTemplate();
#pragma warning disable 219 // field is assigned but never used.
			Field field;
			Assert.Throws<ArgumentOutOfRangeException>(() => field = viewTemplate["none"]);
#pragma warning restore 219
		}

		[Test]
		public void TryGetField_DoesNotHaveFieldDefinition_False()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			Field field;
			Assert.IsFalse(viewTemplate.TryGetField("none", out field));
		}

		[Test]
		public void TryGetField_HasFieldDefinition_True()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			Field field;
			Assert.IsTrue(viewTemplate.TryGetField("field2", out field));
			Assert.IsNotNull(field);
			Assert.AreEqual("field2", field.FieldName);
		}

		[Test]
		public void TryGetField_NullKey_Throws()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			Field field;
			Assert.Throws<ArgumentNullException>(() => viewTemplate.TryGetField(null, out field));
		}

		private static ViewTemplate PopulateViewTemplate()
		{
			var f = new ViewTemplate();
			f.Add(new Field("field1", "LexEntry", new[] {"en", "br", "th"}));
			f.Add(new Field("field2", "LexEntry", new[] {"th"}));
			f.Add(new Field("field2", "LexEntry", new[] {"en", "br"}));
			return f;
		}

		[Test]
		public void SynchronizeInventories_nullMasterTemplate_throws()
		{
			Assert.Throws<ArgumentNullException>(() => ViewTemplate.UpdateUserViewTemplate(null, new ViewTemplate()));
		}

		[Test]
		public void SynchronizeInventories_nullUserTemplate_throws()
		{
			Assert.Throws<ArgumentNullException>(() => ViewTemplate.UpdateUserViewTemplate(new ViewTemplate(), null));
		}

		[Test]
		public void SynchronizeInventories_empty_empty()
		{
			var v = new ViewTemplate();
			ViewTemplate.UpdateUserViewTemplate(v, new ViewTemplate());
			Assert.IsEmpty(v);
		}

		[Test]
		public void MergeWithEmptyInventory()
		{
			ViewTemplate master = MakeMasterInventory();
			int count = master.Count;
			var empty = new ViewTemplate();
			ViewTemplate.UpdateUserViewTemplate(master, empty);

			Assert.AreEqual(count, master.Count);
		}

		private static ViewTemplate MakeMasterInventory()
		{
			using (var tempFolder = new TemporaryFolder("ProjectFromViewTemplateTests"))
			{
				IWritingSystemRepository w = LdmlInFolderWritingSystemRepository.Initialize(
					tempFolder.Path,
					OnWritingSystemMigration,
					OnWritingSystemLoadProblem,
					WritingSystemCompatibility.Flex7V0Compatible
				);
				w.Set(WritingSystemDefinition.Parse("aaa"));
				w.Set(WritingSystemDefinition.Parse("aab"));
				return ViewTemplate.MakeMasterTemplate(w);
			}
		}

		private static void OnWritingSystemLoadProblem(IEnumerable<WritingSystemRepositoryProblem> problems)
		{
			throw new ApplicationException("Unexpected input system load problem during test.");
		}

		private static void OnWritingSystemMigration(IEnumerable<LdmlVersion0MigrationStrategy.MigrationInfo> migrationinfo)
		{
			throw new ApplicationException("Unexpected input system migration during test.");
		}

		[Test]
		[Ignore("Currently all fields are on by default, so this test would need a major rewrite.")]
		public void UserInvWithVisibleFieldConveyedToMaster()
		{
			ViewTemplate master = MakeMasterInventory();
			Assert.IsFalse(master.Contains(Field.FieldNames.ExampleTranslation.ToString()),
						   "If translation is turned on by default, you must fix the test which sees if it is turned on by the user inventory");
			int count = master.Count;
			var simple = new ViewTemplate();
			simple.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(),
								 "LexExampleSentence",
								 new[] {"en"}));
			ViewTemplate.UpdateUserViewTemplate(master, simple);

			Assert.AreEqual(count, master.Count);
			Assert.IsTrue(master.Contains(Field.FieldNames.ExampleTranslation.ToString()));
		}

		[Test]
		public void CustomFieldRetained()
		{
			ViewTemplate master = MakeMasterInventory();
			int count = master.Count;
			var usersTemplate = new ViewTemplate();
			usersTemplate.Add(new Field("dummy", "LexEntry", new[] {"en"}));
			ViewTemplate.UpdateUserViewTemplate(master, usersTemplate);
			Assert.IsTrue(usersTemplate.Contains("dummy"));
		}

		[Test]
		public void ChangeWritingSystemId()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			viewTemplate.Fields[0].WritingSystemIds.Contains("en");
			viewTemplate.OnWritingSystemIDChange("en", "x");
			Assert.IsFalse(viewTemplate.Fields[0].WritingSystemIds.Contains("en"));
			Assert.IsTrue(viewTemplate.Fields[0].WritingSystemIds.Contains("x"));
		}

		[Test]
		public void UpdateUserViewTemplate_Jan2008Upgrade_DefinitionIsEnabled()
		{
			ViewTemplate master = MakeMasterInventory();
			var simple = new ViewTemplate();
			var definitionField = new Field(LexSense.WellKnownProperties.Definition,
											  "LexSense",
											  new[] {"en"});
			definitionField.Enabled = false;
			simple.Add(definitionField);
			ViewTemplate.UpdateUserViewTemplate(master, simple);
			Assert.IsTrue(definitionField.Enabled);
		}

		[Test]
		public void UpdateUserViewTemplate_Jan2008Upgrade_DefinitionGetsGlossWritingSystemsAdded()
		{
			ViewTemplate master = MakeMasterInventory();
			ViewTemplate simple = new ViewTemplate();
			Field definitionField = new Field(LexSense.WellKnownProperties.Definition,
											  "LexSense",
											  new[] {"en", "a", "b"});
			definitionField.Enabled = false;
			simple.Add(definitionField);
			Field glossField = new Field(LexSense.WellKnownProperties.Gloss,
										 "LexSense",
										 new[] {"b", "c"});
			simple.Add(glossField);
			ViewTemplate.UpdateUserViewTemplate(master, simple);
			Assert.AreEqual(4, definitionField.WritingSystemIds.Count);
			Assert.IsTrue(definitionField.WritingSystemIds.Contains("en"));
			Assert.IsTrue(definitionField.WritingSystemIds.Contains("a"));
			Assert.IsTrue(definitionField.WritingSystemIds.Contains("b"));
			Assert.IsTrue(definitionField.WritingSystemIds.Contains("c"));
		}

		[Test]
		public void IsWritingSystemInUse_TemplateContainsNoFields_ReturnsFalse()
		{
			ViewTemplate master = new ViewTemplate();
			Assert.That(master.IsWritingSystemInUse("en"), Is.False);
		}

		[Test]
		public void IsWritingSystemInUse_NoFieldIsUsingWritingSystem_ReturnsFalse()
		{
			ViewTemplate master = new ViewTemplate();
			Field field = new Field();
			field.WritingSystemIds.Add("de");
			master.Fields.Add(field);
			Assert.That(master.IsWritingSystemInUse("en"), Is.False);
		}

		[Test]
		public void IsWritingSystemInUse_FieldIsUsingWritingSystem_ReturnsTrue()
		{
			ViewTemplate master = new ViewTemplate();
			Field field = new Field();
			field.WritingSystemIds.Add("en");
			master.Fields.Add(field);
			Assert.That(master.IsWritingSystemInUse("en"), Is.True);
		}

		[Test]
		public void CreateChorusDisplaySettings_NotesFieldHasNoWritingSystems_DoesNotThrow()
		{
			BasilProjectTestHelper.InitializeForTests();
			var master = new ViewTemplate();
			var noteField = new Field();
			noteField.FieldName = LexSense.WellKnownProperties.Note;
			var lexicalFormField = new Field();
			lexicalFormField.FieldName = LexEntry.WellKnownProperties.LexicalUnit;
			lexicalFormField.WritingSystemIds.Add(WeSayWordsProject.VernacularWritingSystemIdForProjectCreation);
			master.Fields.Add(noteField);
			master.Fields.Add(lexicalFormField);
			Assert.DoesNotThrow(() => master.CreateChorusDisplaySettings());
		}
	}
}