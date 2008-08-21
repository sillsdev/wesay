using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	internal class Result: Dictionary<string, object>
	{
		public Result(params KV[] kvs)
		{
			foreach (KV kv in kvs)
			{
				Add(kv.Key, kv.Value);
			}
		}
	}

	internal class KV
	{
		private readonly string key;
		private readonly object value;

		public string Key
		{
			get { return this.key; }
		}

		public object Value
		{
			get { return this.value; }
		}

		public KV(string key, object value)
		{
			this.key = key;
			this.value = value;
		}
	}

	[TestFixture]
	public class QueryTests
	{
		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void In_Null_Throws()
		{
			new Query(typeof (TestItem)).In(null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void In_PropertyDoesNotExist_Throws()
		{
			new Query(typeof (TestItem)).In("IDontExist");
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void In_EmptyString_Throws()
		{
			new Query(typeof (TestItem)).In(string.Empty);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ForEach_Null_Throws()
		{
			new Query(typeof (TestItem)).ForEach(null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ForEach_EmptyString_Throws()
		{
			new Query(typeof (TestItem)).ForEach(string.Empty);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ForEach_PropertyDoesNotExist_Throws()
		{
			new Query(typeof (TestItem)).ForEach("IDontExist");
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ForEach_PropertyDoesNotReturnIEnumerableOfT_Throws()
		{
			new Query(typeof (TestItem)).ForEach("Child");
		}

		[Test]
		public void ForEach_PropertyReturnsIEnumerableOfT_Okay()
		{
			Assert.IsNotNull(new Query(typeof (TestItem)).ForEach("ChildItemList"));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Show_Null_Throws()
		{
			new Query(typeof (TestItem)).Show(null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Show_EmptyString_Throws()
		{
			new Query(typeof (TestItem)).Show(string.Empty);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Show_PropertyDoesNotExist_Throws()
		{
			new Query(typeof (TestItem)).Show("IDontExist");
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ShowEach_Null_Throws()
		{
			new Query(typeof (TestItem)).ShowEach(null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ShowEach_EmptyString_Throws()
		{
			new Query(typeof (TestItem)).ShowEach(string.Empty);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ShowEach_PropertyDoesNotExist_Throws()
		{
			new Query(typeof (TestItem)).ShowEach("IDontExist");
		}

		[Test]
		public void ShowEach_PropertyIsIEnumerable_GetsInnerType()
		{
			Assert.IsNotNull(new Query(typeof (TestItem)).ShowEach("ChildItemList"));
		}

		[Test]
		//even though a string is IEnumerable<char> we don't really want to treat it that way
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ShowEach_PropertyIsString_Throws()
		{
			Assert.IsNotNull(new Query(typeof (TestItem)).ShowEach("StoredString"));
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ShowEach_PropertyIsNotIEnumerable_Throws()
		{
			Assert.IsNotNull(new Query(typeof (TestItem)).ShowEach("Child"));
		}

		[Test]
		public void ForEach_PropertyIsIEnumerable_GetsInnerType()
		{
			Assert.IsNotNull(new Query(typeof (TestItem)).ForEach("ChildItemList").Show("StoredInt"));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void GetResults_Null_Throws()
		{
			Query q = new Query(typeof (TestItem));
			IEnumerable<Dictionary<string, object>> results = q.GetResults(null);
			// we have to actually use the IEnumerable or it won't execute
			// since it is generated
			results.GetEnumerator().MoveNext();
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void GetResults_NotInstanceOfType_Throws()
		{
			Query q = new Query(typeof (TestItem));
			IEnumerable<Dictionary<string, object>> results = q.GetResults(new object());
			// we have to actually use the IEnumerable or it won't execute
			// since it is generated
			results.GetEnumerator().MoveNext();
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Show_LabelAlreadyUsed_Throws()
		{
			new Query(typeof (TestItem)).Show("StoredString").ForEach("ChildItemList").Show(
					"StoredString");
		}

		[Test]
		public void Show_DifferentLabel_Okay()
		{
			new Query(typeof (TestItem)).Show("StoredString").ForEach("ChildItemList").Show(
					"StoredString", "ChildrenStoredString");
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Show_LabelAlreadyUsedInBranch_Throws()
		{
			Query query = new Query(typeof (TestItem));
			query.In("Child").Show("StoredString");
			query.ForEach("ChildItemList").Show("StoredString");
		}

		[Test]
		public void Show_DifferentLabelInBranch_Okay()
		{
			Query query = new Query(typeof (TestItem));
			query.In("Child").Show("StoredString", "ChildStoredString");
			query.ForEach("ChildItemList").Show("StoredString", "ChildrenStoredString");
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ShowEach_LabelAlreadyUsed_Throws()
		{
			new Query(typeof (TestMultiple)).ShowEach("Strings", "a").ShowEach("KeyValuePairs", "a");
		}

		[Test]
		public void ShowEach_DifferentLabel_Okay()
		{
			new Query(typeof (TestMultiple)).ShowEach("Strings", "a").ShowEach("KeyValuePairs", "b");
		}
	}

	[TestFixture]
	public class EmptyItemQueryTests
	{
		private TestItem item;

		[SetUp]
		public void Setup()
		{
			item = new TestItem();
		}

		[Test]
		public void GetResultsOnQuery_WithNoShow_NoItems()
		{
			Query all = new Query(typeof (TestItem));
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResultsOnInQuery_WithNoShow_NoItems()
		{
			Query all = new Query(typeof (TestItem)).In("Child");
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResultsOnQuery_AtLeastOneWithShow_OneItem()
		{
			// In the query below AtLeastOne and Show are siblings,
			// both are children of 'all'.
			Query all = new Query(typeof (TestItem)).AtLeastOne().Show("StoredString");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("", results[0]["StoredString"]);
		}

		[Test]
		public void GetResultsOnQuery_AtLeastOneButNoShow_OneItem()
		{
			// In the query below AtLeastOne and Show are siblings,
			// both are children of 'all'.
			Query all = new Query(typeof(TestItem)).AtLeastOne();
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("", results[0][""]);
		}

		[Test]
		public void GetResultsOnInQuery_AtLeastOneWithShow_OneItem()
		{
			// In the query below AtLeastOne and Show are siblings,
			// both are children of 'all'.
			Query all = new Query(typeof(TestItem)).In("Child").AtLeastOne().Show("StoredString");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("", results[0]["StoredString"]);
		}

		[Test]
		public void GetResultsOnInQuery_AtLeastOneButNoShow_OneItem()
		{
			// In the query below AtLeastOne and Show are siblings,
			// both are children of 'all'.
			Query all = new Query(typeof(TestItem)).In("Child").AtLeastOne();
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("", results[0][""]);
		}

		[Test]
		public void GetResultsOnForEachQuery_AtLeastOneWithShow_OneItem()
		{
			// In the query below AtLeastOne and Show are siblings,
			// both are children of 'all'.
			Query all = new Query(typeof(TestItem)).ForEach("ChildItemList").AtLeastOne().Show("StoredString");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("", results[0]["StoredString"]);
		}

		[Test]
		public void GetResultsOnForEachQuery_AtLeastOneButNoShow_OneItem()
		{
			// In the query below AtLeastOne and Show are siblings,
			// both are children of 'all'.
			Query all = new Query(typeof(TestItem)).ForEach("ChildItemList").AtLeastOne();
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("", results[0][""]);
		}

		[Test]
		public void GetResults_Show_OneNullItem()
		{
			Query all = new Query(typeof(TestItem)).Show("Child");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(null, results[0]["Child"]);
		}

		[Test]
		public void GetResults_ForEachButNoShow_NoItems()
		{
			Query all = new Query(typeof (TestItem)).ForEach("ChildItemList");
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ShowStoredIntForEachChildren_NoItems()
		{
			Query allStoredIntsInChildren =
					new Query(typeof (TestItem)).ForEach("ChildItemList").Show("StoredInt");
			IEnumerable<Dictionary<string, object>> results =
					allStoredIntsInChildren.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}
	}

	[TestFixture]
	public class NestedItemQueryTests
	{
		private TestItem item;

		[SetUp]
		public void Setup()
		{
			item = new TestItem("top", 1, DateTime.UtcNow);
			item.Child = new ChildTestItem(null, 24, DateTime.Now);

			item.ChildItemList = new List<ChildTestItem>();
			item.ChildItemList.Add(new ChildTestItem("1", 1, DateTime.Now));
			item.ChildItemList.Add(new ChildTestItem("2", 2, DateTime.Now));
			item.ChildItemList.Add(new ChildTestItem("3", 3, DateTime.Now));
		}

		[Test]
		public void GetResultsOnInQuery_WithNoShow_NoItems()
		{
			Query all = new Query(typeof (TestItem)).In("Child");
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResultsOnInQuery_WithShow_OneIntItem()
		{
			Query all = new Query(typeof (TestItem)).In("Child").Show("StoredInt");
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {new Result(new KV("StoredInt", 24))};

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnInQuery_WithShowSelectingNullItem_OneNullItem()
		{
			Query all = new Query(typeof (TestItem)).In("Child").Show("StoredString");
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(
																			  new KV(
																					  "StoredString",
																					  null))
															  };

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnInQuery_With2NestedChildSecondIsNull_NoItems()
		{
			Query all = new Query(typeof (TestItem)).In("Child").In("Child").Show("StoredString");
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[] {};

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnInQuery_AtLeastOneWithShow_SameAsWithShow()
		{
			Query queryWithAtLeastOne = new Query(typeof(TestItem)).In("Child").AtLeastOne().Show("StoredString");
			Query queryWithOutAtLeastOne = new Query(typeof(TestItem)).In("Child").Show("StoredString");
			IEnumerable<Dictionary<string, object>> results = queryWithAtLeastOne.GetResults(item);
			IEnumerable<Dictionary<string, object>> expectedResult = queryWithOutAtLeastOne.GetResults(item);
			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnInQuery_WhereTrue_Item()
		{
			Query query = new Query(typeof(TestItem)).In("Child").Where("StoredInt", delegate(object o)
																							{
																								return (int)o == 24;
																							}).AtLeastOne().Show("StoredInt");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("24", results[0]["StoredInt"]);
		}

		[Test]
		public void GetResultsOnInQuery_WhereFalse_NoItems()
		{
			Query query = new Query(typeof(TestItem)).In("Child").Where("StoredInt", delegate(object o)
																							{
																								return (int)o != 24; //will not match the item
																							}).AtLeastOne().Show("StoredInt");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetResultsOnInQuery_WhereFilterAllItemsAtLeastOne_HasOne()
		{
			Query query = new Query(typeof(TestItem)).In("Child").Where("StoredInt", delegate(object o)
																							{
																								return (int)o != 24; //will not match the item
																							}).AtLeastOne().Show("StoredInt");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("", results[0][""]);
		}

		[Test]
		public void GetResultsOnQuery_WhereTrue_Item()
		{
			Query query = new Query(typeof(TestItem)).Where("StoredString", delegate(object str)
																				{
																					return (string)str == "top";
																				}).Show("StoredString");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("top", results[0]["StoredString"]);
		}

		[Test]
		public void GetResultsOnQuery_WhereFalse_NoItems()
		{
			Query query = new Query(typeof(TestItem)).Where("StoredString", delegate(object str)
																				{
																					return (string)str == "cantMatchMe";
																				}).Show("StoredString");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetResultsOnQuery_WhereFilterAllItemsAtLeastOne_HasOne()
		{
			Query query = new Query(typeof(TestItem)).Where("StoredString", delegate(object str)
																							{
																								return (string)str == "cantMatchMe";
																							}).AtLeastOne().Show("StoredString");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("", results[0][""]);
		}

		[Test]
		public void GetResultsOnQuery_AtLeastOneWithShow_SameAsWithShow()
		{
			Query queryWithAtLeastOne = new Query(typeof(TestItem)).AtLeastOne().Show("StoredString");
			Query queryWithOutAtLeastOne = new Query(typeof(TestItem)).Show("StoredString");
			IEnumerable<Dictionary<string, object>> results = queryWithAtLeastOne.GetResults(item);
			IEnumerable<Dictionary<string, object>> expectedResult = queryWithOutAtLeastOne.GetResults(item);
			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnForEachQuery_WhereTrue_Item()
		{
			Query query = new Query(typeof(TestItem)).ForEach("ChildItemList").Where("StoredInt", delegate(object o)
																							{
																								return (int)o < 2;
																							}).AtLeastOne().Show("StoredInt");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("1", results[0]["StoredInt"]);
		}

		[Test]
		public void GetResultsOnForEachQuery_WhereFalse_NoItems()
		{
			Query query = new Query(typeof(TestItem)).ForEach("ChildItemList").Where("StoredInt", delegate(object o)
																							{
																								return (int)o < 0; //will filter all items
																							}).AtLeastOne().Show("StoredInt");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetResultsOnForEachQuery_WhereFilterAllItemsAtLeastOne_HasOne()
		{
			Query query = new Query(typeof(TestItem)).ForEach("ChildItemList").Where("StoredInt", delegate(object o)
																							{
																								return (int)o < 0; //will filter all items;
																							}).AtLeastOne().Show("StoredInt");
			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("", results[0][""]);
		}

		[Test]
		public void GetResultsOnForEachQuery_AtLeastOneWithShow_SameAsWithShow()
		{
			Query queryWithAtLeastOne = new Query(typeof(TestItem)).ForEach("ChildItemList").AtLeastOne().Show("StoredString");
			Query queryWithOutAtLeastOne = new Query(typeof(TestItem)).ForEach("ChildItemList").Show("StoredString");
			IEnumerable<Dictionary<string, object>> results = queryWithAtLeastOne.GetResults(item);
			IEnumerable<Dictionary<string, object>> expectedResult = queryWithOutAtLeastOne.GetResults(item);
			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}


	}

	[TestFixture]
	public class ItemWithNullQueryTests
	{
		private TestItem item;

		[SetUp]
		public void Setup()
		{
			item = new TestItem("top", 1, DateTime.UtcNow);
			item.Child = new ChildTestItem(null, 24, DateTime.Now);
			item.ChildItemList = new List<ChildTestItem>();
			item.ChildItemList.Add(new ChildTestItem("1", 1, DateTime.Now));
			item.ChildItemList.Add(new ChildTestItem("2", 2, DateTime.Now));
			item.ChildItemList.Add(new ChildTestItem("3", 3, DateTime.Now));
		}

		[Test]
		public void GetResults_NoShow_NoItems()
		{
			Query all = new Query(typeof (TestItem));
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ForEachButNoShow_NoItems()
		{
			Query all = new Query(typeof (TestItem)).ForEach("ChildItemList");
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ShowStoredIntForEachChildren_ThreeItems()
		{
			Query allStoredIntsInChildren =
					new Query(typeof (TestItem)).ForEach("ChildItemList").Show("StoredInt");

			IEnumerable<Dictionary<string, object>> results =
					allStoredIntsInChildren.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(new KV("StoredInt", 1)),
																	  new Result(new KV("StoredInt", 2)),
																	  new Result(new KV("StoredInt", 3))
															  };

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowStoredStringAndStoredStringsForEachChildren_ThreeItems()
		{
			Query allStoredIntsInChildren =
					new Query(typeof (TestItem)).Show("StoredString").ForEach("ChildItemList").Show(
							"StoredString", "ChildStoredString");

			IEnumerable<Dictionary<string, object>> results =
					allStoredIntsInChildren.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(
																			  new KV(
																					  "StoredString",
																					  "top"),
																			  new KV(
																					  "ChildStoredString",
																					  "1")),
																	  new Result(
																			  new KV(
																					  "StoredString",
																					  "top"),
																			  new KV(
																					  "ChildStoredString",
																					  "2")),
																	  new Result(
																			  new KV(
																					  "StoredString",
																					  "top"),
																			  new KV(
																					  "ChildStoredString",
																					  "3"))
															  };
			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void
				GetResults_ShowStoredStringChildStoredStringAndStoredStringsForEachChildren_ThreeItems
				()
		{
			Query query = new Query(typeof (TestItem)).Show("StoredString");
			query.In("Child").Show("StoredString", "ChildStoredString");
			query.ForEach("ChildItemList").Show("StoredString", "ChildrenStoredString");

			IEnumerable<Dictionary<string, object>> results = query.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(
																			  new KV(
																					  "StoredString",
																					  "top"),
																			  new KV(
																					  "ChildStoredString",
																					  null),
																			  new KV(
																					  "ChildrenStoredString",
																					  "1")),
																	  new Result(
																			  new KV(
																					  "StoredString",
																					  "top"),
																			  new KV(
																					  "ChildStoredString",
																					  null),
																			  new KV(
																					  "ChildrenStoredString",
																					  "2")),
																	  new Result(
																			  new KV(
																					  "StoredString",
																					  "top"),
																			  new KV(
																					  "ChildStoredString",
																					  null),
																			  new KV(
																					  "ChildrenStoredString",
																					  "3"))
															  };

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}
	}

	[TestFixture]
	public class ItemWithMultiplesTests
	{
		private TestMultiple item;

		[SetUp]
		public void Setup()
		{
			item = new TestMultiple();
			item.Strings.Add("string1");
			item.Strings.Add("string2");
			item.KeyValuePairs.Add(new KeyValuePair<string, string>("key1", "value1"));
			item.KeyValuePairs.Add(new KeyValuePair<string, string>("key2", "value2"));
			item.String = "string";
		}

		[Test]
		public void GetResults_NoShow_NoItems()
		{
			Query all = new Query(typeof (TestMultiple));
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_InButNoShow_NoItems()
		{
			Query all = new Query(typeof (TestMultiple)).In("String");
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ForEachButNoShow_NoItems()
		{
			Query all = new Query(typeof (TestMultiple)).ForEach("Strings");
			IEnumerable<Dictionary<string, object>> results = all.GetResults(this.item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ShowEachStrings_AllStrings()
		{
			Query allStrings = new Query(typeof (TestMultiple)).ShowEach("Strings");
			IEnumerable<Dictionary<string, object>> results = allStrings.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(new KV("Strings",
																						"string1")),
																	  new Result(new KV("Strings",
																						"string2"))
															  };

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowEachStringsMergedWithEachValueOfKeyValuePairs()
		{
			Query allStrings =
					new Query(typeof (TestMultiple)).ShowEach("Strings").ForEach("KeyValuePairs").
							Show("Value");
			IEnumerable<Dictionary<string, object>> results = allStrings.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(
																			  new KV("Strings",
																					 "string1"),
																			  new KV("Value",
																					 "value1")),
																	  new Result(
																			  new KV("Strings",
																					 "string1"),
																			  new KV("Value",
																					 "value2")),
																	  new Result(
																			  new KV("Strings",
																					 "string2"),
																			  new KV("Value",
																					 "value1")),
																	  new Result(
																			  new KV("Strings",
																					 "string2"),
																			  new KV("Value",
																					 "value2"))
															  };

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowEachStringsMergedWithEachKeyValueOfKeyValuePairs()
		{
			Query allStrings =
					new Query(typeof (TestMultiple)).ShowEach("Strings").ForEach("KeyValuePairs").
							Show("Key").Show("Value");
			IEnumerable<Dictionary<string, object>> results = allStrings.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(
																			  new KV("Strings",
																					 "string1"),
																			  new KV("Key", "key1"),
																			  new KV("Value",
																					 "value1")),
																	  new Result(
																			  new KV("Strings",
																					 "string1"),
																			  new KV("Key", "key2"),
																			  new KV("Value",
																					 "value2")),
																	  new Result(
																			  new KV("Strings",
																					 "string2"),
																			  new KV("Key", "key1"),
																			  new KV("Value",
																					 "value1")),
																	  new Result(
																			  new KV("Strings",
																					 "string2"),
																			  new KV("Key", "key2"),
																			  new KV("Value",
																					 "value2"))
															  };

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowEachStringsMergedWithEachKeyValueOfKeyValuePairsMergedWithString()
		{
			Query allStrings =
					new Query(typeof (TestMultiple)).ShowEach("Strings").Show("String").ForEach(
							"KeyValuePairs").Show("Key").Show("Value");
			IEnumerable<Dictionary<string, object>> results = allStrings.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(
																			  new KV("Strings",
																					 "string1"),
																			  new KV("String",
																					 "string"),
																			  new KV("Key", "key1"),
																			  new KV("Value",
																					 "value1")),
																	  new Result(
																			  new KV("Strings",
																					 "string1"),
																			  new KV("String",
																					 "string"),
																			  new KV("Key", "key2"),
																			  new KV("Value",
																					 "value2")),
																	  new Result(
																			  new KV("Strings",
																					 "string2"),
																			  new KV("String",
																					 "string"),
																			  new KV("Key", "key1"),
																			  new KV("Value",
																					 "value1")),
																	  new Result(
																			  new KV("Strings",
																					 "string2"),
																			  new KV("String",
																					 "string"),
																			  new KV("Key", "key2"),
																			  new KV("Value",
																					 "value2"))
															  };

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowStringMergedWithEachStringsMergedWithEachKeyValueOfKeyValuePairs()
		{
			Query allStrings =
					new Query(typeof (TestMultiple)).Show("String").ShowEach("Strings").ForEach(
							"KeyValuePairs").Show("Key").Show("Value");
			IEnumerable<Dictionary<string, object>> results = allStrings.GetResults(this.item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(
																			  new KV("String",
																					 "string"),
																			  new KV("Strings",
																					 "string1"),
																			  new KV("Key", "key1"),
																			  new KV("Value",
																					 "value1")),
																	  new Result(
																			  new KV("String",
																					 "string"),
																			  new KV("Strings",
																					 "string1"),
																			  new KV("Key", "key2"),
																			  new KV("Value",
																					 "value2")),
																	  new Result(
																			  new KV("String",
																					 "string"),
																			  new KV("Strings",
																					 "string2"),
																			  new KV("Key", "key1"),
																			  new KV("Value",
																					 "value1")),
																	  new Result(
																			  new KV("String",
																					 "string"),
																			  new KV("Strings",
																					 "string2"),
																			  new KV("Key", "key2"),
																			  new KV("Value",
																					 "value2"))
															  };

			Assert.DoAssert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}
	}
}