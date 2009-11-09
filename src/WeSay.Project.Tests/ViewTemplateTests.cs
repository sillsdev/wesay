using System;
using System.Drawing;
using NUnit.Framework;
using WeSay.Data.Tests;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class viewTemplateTests: IEnumerableBaseTest<Field>
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
			ViewTemplate viewTemplate = new ViewTemplate();
			Assert.IsNotNull(viewTemplate);
		}

		[Test]
		public void Contains_HasFieldDefinition_True()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			Assert.IsTrue(viewTemplate.Contains("field1"));
		}

		[Test]
		public void Contains_DoesNotHaveFieldDefinition_False()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			Assert.IsFalse(viewTemplate.Contains("none"));
		}

		[Test]
		public void Index_HasFieldDefinition_FieldDefinition()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			Assert.IsNotNull(viewTemplate["field1"]);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Index_DoesNotHaveFieldDefinition_Throws()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			Field field = viewTemplate["none"];
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
		[ExpectedException(typeof (ArgumentNullException))]
		public void TryGetField_NullKey_Throws()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			Field field;
			viewTemplate.TryGetField(null, out field);
		}

		private static ViewTemplate PopulateViewTemplate()
		{
			ViewTemplate f = new ViewTemplate();
			f.Add(new Field("field1", "LexEntry", new string[] {"en", "br", "th"}));
			f.Add(new Field("field2", "LexEntry", new string[] {"th"}));
			f.Add(new Field("field2", "LexEntry", new string[] {"en", "br"}));
			return f;
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SynchronizeInventories_nullMasterTemplate_throws()
		{
			ViewTemplate.UpdateUserViewTemplate(null, new ViewTemplate());
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SynchronizeInventories_nullUserTemplate_throws()
		{
			ViewTemplate.UpdateUserViewTemplate(new ViewTemplate(), null);
		}

		[Test]
		public void SynchronizeInventories_empty_empty()
		{
			ViewTemplate v = new ViewTemplate();
			ViewTemplate.UpdateUserViewTemplate(v, new ViewTemplate());
			Assert.IsEmpty(v);
		}

		[Test]
		public void MergeWithEmptyInventory()
		{
			ViewTemplate master = MakeMasterInventory();
			int count = master.Count;
			ViewTemplate empty = new ViewTemplate();
			ViewTemplate.UpdateUserViewTemplate(master, empty);

			Assert.AreEqual(count, master.Count);
		}

		private static ViewTemplate MakeMasterInventory()
		{
			WritingSystemCollection w = new WritingSystemCollection();
			w.Add("red", new WritingSystem("red", new Font("arial", 12)));
			w.Add("white", new WritingSystem("white", new Font("arial", 12)));
			return ViewTemplate.MakeMasterTemplate(w);
		}

		[Test]
		[Ignore("Currently all fields are on by default, so this test would need a major rewrite.")]
		public void UserInvWithVisibleFieldConveyedToMaster()
		{
			ViewTemplate master = MakeMasterInventory();
			Assert.IsFalse(master.Contains(Field.FieldNames.ExampleTranslation.ToString()),
						   "If translation is turned on by default, you must fix the test which sees if it is turned on by the user inventory");
			int count = master.Count;
			ViewTemplate simple = new ViewTemplate();
			simple.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(),
								 "LexExampleSentence",
								 new String[] {"en"}));
			ViewTemplate.UpdateUserViewTemplate(master, simple);

			Assert.AreEqual(count, master.Count);
			Assert.IsTrue(master.Contains(Field.FieldNames.ExampleTranslation.ToString()));
		}

		[Test]
		public void CustomFieldRetained()
		{
			ViewTemplate master = MakeMasterInventory();
			int count = master.Count;
			ViewTemplate usersTemplate = new ViewTemplate();
			usersTemplate.Add(new Field("dummy", "LexEntry", new String[] {"en"}));
			ViewTemplate.UpdateUserViewTemplate(master, usersTemplate);
			Assert.IsTrue(usersTemplate.Contains("dummy"));
		}

		[Test]
		public void ChangeWritingSystemId()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			viewTemplate.Fields[0].WritingSystemIds.Contains("en");
			viewTemplate.ChangeWritingSystemId("en", "x");
			Assert.IsFalse(viewTemplate.Fields[0].WritingSystemIds.Contains("en"));
			Assert.IsTrue(viewTemplate.Fields[0].WritingSystemIds.Contains("x"));
		}

		[Test]
		public void UpdateUserViewTemplate_Jan2008Upgrade_DefinitionIsEnabled()
		{
			ViewTemplate master = MakeMasterInventory();
			ViewTemplate simple = new ViewTemplate();
			Field definitionField = new Field(LexSense.WellKnownProperties.Definition,
											  "LexSense",
											  new String[] {"en"});
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
											  new String[] {"en", "a", "b"});
			definitionField.Enabled = false;
			simple.Add(definitionField);
			Field glossField = new Field(LexSense.WellKnownProperties.Gloss,
										 "LexSense",
										 new String[] {"b", "c"});
			simple.Add(glossField);
			ViewTemplate.UpdateUserViewTemplate(master, simple);
			Assert.AreEqual(4, definitionField.WritingSystemIds.Count);
			Assert.IsTrue(definitionField.WritingSystemIds.Contains("en"));
			Assert.IsTrue(definitionField.WritingSystemIds.Contains("a"));
			Assert.IsTrue(definitionField.WritingSystemIds.Contains("b"));
			Assert.IsTrue(definitionField.WritingSystemIds.Contains("c"));
		}
	}
}