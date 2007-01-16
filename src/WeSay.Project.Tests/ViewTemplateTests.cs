using System;
using NUnit.Framework;
using WeSay.Language;
using WeSay.Project;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class viewTemplateTests : WeSay.Data.Tests.IEnumerableTests.IEnumerableBaseTest<Field>
	{
		[SetUp]
		public void Setup()
		{
			_enumerable = new ViewTemplate();
			_itemCount = 0;
		}

		[TearDown]
		public void TearDown()
		{

		}

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
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
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
		[ExpectedException(typeof(ArgumentNullException))]
		public void TryGetField_NullKey_Throws()
		{
			ViewTemplate viewTemplate = PopulateViewTemplate();
			Field field;
			viewTemplate.TryGetField(null, out field);
		}

		private static ViewTemplate PopulateViewTemplate() {
			ViewTemplate f = new ViewTemplate();
			f.Add(new Field("field1", new string[] { "en", "br", "th" }));
			f.Add(new Field("field2",  new string[]{"th"}));
			f.Add(new Field("field2",  new string[]{"en", "br"}));
			return f;
		}


		[Test]
		public void MergeWithEmptyInventory()
		{
			ViewTemplate master = MakeMasterInventory();
			int count = master.Count;
			ViewTemplate empty = new ViewTemplate ();
			ViewTemplate.SynchronizeInventories(master, empty);

			Assert.AreEqual(count, master.Count);
		}

		private static ViewTemplate MakeMasterInventory()
		{
			WritingSystemCollection w = new WritingSystemCollection();
			w.Add("red", new WritingSystem("red", new System.Drawing.Font("arial", 12)));
			w.Add("white", new WritingSystem("white", new System.Drawing.Font("arial", 12)));
			return ViewTemplate.MakeMasterTemplate(w);
		}

		[Test]
		public void UserInvWithVisibleFieldConveyedToMaster()
		{
			ViewTemplate master = MakeMasterInventory();
			Assert.IsFalse(master.Contains(Field.FieldNames.ExampleTranslation.ToString()),"If translation is turned on by default, you must fix the test which sees if it is turned on by the user inventory");
			int count = master.Count;
			ViewTemplate simple = new ViewTemplate ();
			simple.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), new String[] {"en"}));
			ViewTemplate.SynchronizeInventories(master, simple);

			Assert.AreEqual(count, master.Count);
			Assert.IsTrue(master.Contains(Field.FieldNames.ExampleTranslation.ToString()));
		}

		[Test]
		public void ExtraFieldDiscarded()
		{
			ViewTemplate master = MakeMasterInventory();
			int count = master.Count;
			ViewTemplate simple = new ViewTemplate();
			simple.Add(new Field("dummy", new String[] { "en" }));
			ViewTemplate.SynchronizeInventories(master, simple);
			Assert.IsFalse(master.Contains("dummy"));
		}

	}

}