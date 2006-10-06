using System;
using NUnit.Framework;

namespace WeSay.UI.Tests
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
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullFields_Throws()
		{
			FieldInventory fieldInventory = new FieldInventory(null);
			Assert.IsNotNull(fieldInventory);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_NullField_Throws()
		{
			FieldInventory fieldInventory = new FieldInventory(null,null);
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

		private static FieldInventory PopulateFieldInventory() {
			Field field1 = new Field("field1", "en", "br", "th");
			Field field2 = new Field("field2", "th");
			Field field3 = new Field("field2", "en", "br");
			return new FieldInventory(field1, field2, field3);
		}
	}

}