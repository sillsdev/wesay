using System;
using NUnit.Framework;

namespace WeSay.LexicalModel.Tests
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
	}

}