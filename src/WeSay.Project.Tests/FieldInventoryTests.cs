using System;
using NUnit.Framework;
using WeSay.Language;
using WeSay.Project;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class FieldInventoryTests : WeSay.Data.Tests.IEnumerableTests.IEnumerableBaseTest<Field>
	{
		[SetUp]
		public void Setup()
		{
			_enumerable = new FieldInventory();
			_itemCount = 0;
		}

		[TearDown]
		public void TearDown()
		{

		}

		[Test]
		public void Create()
		{
			FieldInventory fieldInventory = new FieldInventory();
			Assert.IsNotNull(fieldInventory);
		}

		[Test]
		public void Contains_HasFieldDefinition_True()
		{
			FieldInventory fieldInventory = PopulateFieldInventory();
			Assert.IsTrue(fieldInventory.Contains("field1"));
		}

		[Test]
		public void Contains_DoesNotHaveFieldDefinition_False()
		{
			FieldInventory fieldInventory = PopulateFieldInventory();
			Assert.IsFalse(fieldInventory.Contains("none"));
		}

		[Test]
		public void Index_HasFieldDefinition_FieldDefinition()
		{
			FieldInventory fieldInventory = PopulateFieldInventory();
			Assert.IsNotNull(fieldInventory["field1"]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Index_DoesNotHaveFieldDefinition_Throws()
		{
			FieldInventory fieldInventory = PopulateFieldInventory();
			Field field = fieldInventory["none"];
		}

		[Test]
		public void TryGetField_DoesNotHaveFieldDefinition_False()
		{
			FieldInventory fieldInventory = PopulateFieldInventory();
			Field field;
			Assert.IsFalse(fieldInventory.TryGetField("none", out field));
		}

		[Test]
		public void TryGetField_HasFieldDefinition_True()
		{
			FieldInventory fieldInventory = PopulateFieldInventory();
			Field field;
			Assert.IsTrue(fieldInventory.TryGetField("field2", out field));
			Assert.IsNotNull(field);
			Assert.AreEqual("field2", field.FieldName);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TryGetField_NullKey_Throws()
		{
			FieldInventory fieldInventory = PopulateFieldInventory();
			Field field;
			fieldInventory.TryGetField(null, out field);
		}

		private static FieldInventory PopulateFieldInventory() {
			FieldInventory f = new FieldInventory();
			f.Add(new Field("field1", new string[] { "en", "br", "th" }));
			f.Add(new Field("field2",  new string[]{"th"}));
			f.Add(new Field("field2",  new string[]{"en", "br"}));
			return f;
		}


		[Test]
		public void MergeWithEmptyInventory()
		{
			FieldInventory master = MakeMasterInventory();
			int count = master.Count;
			FieldInventory empty = new FieldInventory ();
			FieldInventory.SynchronizeInventories(master, empty);

			Assert.AreEqual(count, master.Count);
		}

		private static FieldInventory MakeMasterInventory()
		{
			WritingSystemCollection w = new WritingSystemCollection();
			w.Add("red", new WritingSystem("red", new System.Drawing.Font("arial", 12)));
			w.Add("white", new WritingSystem("white", new System.Drawing.Font("arial", 12)));
			return FieldInventory.MakeMasterInventory(w);
		}

		[Test]
		public void UserInvWithVisibleFieldConveyedToMaster()
		{
			FieldInventory master = MakeMasterInventory();
			Assert.IsFalse(master.Contains(Field.FieldNames.ExampleTranslation.ToString()),"If translation is turned on by default, you must fix the test which sees if it is turned on by the user inventory");
			int count = master.Count;
			FieldInventory simple = new FieldInventory ();
			simple.Add(new Field(Field.FieldNames.ExampleTranslation.ToString(), new String[] {"en"}));
			FieldInventory.SynchronizeInventories(master, simple);

			Assert.AreEqual(count, master.Count);
			Assert.IsTrue(master.Contains(Field.FieldNames.ExampleTranslation.ToString()));
		}

		[Test]
		public void ExtraFieldDiscarded()
		{
			FieldInventory master = MakeMasterInventory();
			int count = master.Count;
			FieldInventory simple = new FieldInventory();
			simple.Add(new Field("dummy", new String[] { "en" }));
			FieldInventory.SynchronizeInventories(master, simple);
			Assert.IsFalse(master.Contains("dummy"));
		}

	}

}