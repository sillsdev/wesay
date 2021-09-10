using NUnit.Framework;
using SIL.Tests.Data;
using System;
using System.Collections.Generic;

namespace WeSay.Data.Tests
{
	internal class Result : Dictionary<string, object>
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
			get { return key; }
		}

		public object Value
		{
			get { return value; }
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
		public void In_Null_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new Query(typeof(PalasoTestItem)).In(null));
		}

		[Test]
		public void In_PropertyDoesNotExist_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).In("IDontExist"));
		}

		[Test]
		public void In_EmptyString_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).In(string.Empty));
		}

		[Test]
		public void ForEach_Null_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new Query(typeof(PalasoTestItem)).ForEach(null));
		}

		[Test]
		public void ForEach_EmptyString_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).ForEach(string.Empty));
		}

		[Test]
		public void ForEach_PropertyDoesNotExist_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).ForEach("IDontExist"));
		}

		[Test]
		public void ForEach_PropertyDoesNotReturnIEnumerableOfT_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).ForEach("Child"));
		}

		[Test]
		public void ForEach_PropertyReturnsIEnumerableOfT_Okay()
		{
			Assert.IsNotNull(new Query(typeof(PalasoTestItem)).ForEach("ChildItemList"));
		}

		[Test]
		public void Show_Null_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new Query(typeof(PalasoTestItem)).Show(null));
		}

		[Test]
		public void Show_EmptyString_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).Show(string.Empty));
		}

		[Test]
		public void Show_PropertyDoesNotExist_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).Show("IDontExist"));
		}

		[Test]
		public void ShowEach_Null_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new Query(typeof(PalasoTestItem)).ShowEach(null));
		}

		[Test]
		public void ShowEach_EmptyString_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).ShowEach(string.Empty));
		}

		[Test]
		public void ShowEach_PropertyDoesNotExist_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).ShowEach("IDontExist"));
		}

		[Test]
		public void ShowEach_PropertyIsIEnumerable_GetsInnerType()
		{
			Assert.IsNotNull(new Query(typeof(PalasoTestItem)).ShowEach("ChildItemList"));
		}

		[Test]
		//even though a string is IEnumerable<char> we don't really want to treat it that way
		public void ShowEach_PropertyIsString_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).ShowEach("StoredString"));
		}

		[Test]
		public void ShowEach_PropertyIsNotIEnumerable_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).ShowEach("Child"));
		}

		[Test]
		public void ForEach_PropertyIsIEnumerable_GetsInnerType()
		{
			Assert.IsNotNull(new Query(typeof(PalasoTestItem)).ForEach("ChildItemList").Show("StoredInt"));
		}

		[Test]
		public void GetResults_Null_Throws()
		{
			Query q = new Query(typeof(PalasoTestItem));
			IEnumerable<IDictionary<string, object>> results = q.GetResults(null);
			// we have to actually use the IEnumerable or it won't execute
			// since it is generated
			Assert.Throws<ArgumentNullException>(() => results.GetEnumerator().MoveNext());
		}

		[Test]
		public void GetResults_NotInstanceOfType_Throws()
		{
			Query q = new Query(typeof(PalasoTestItem));
			IEnumerable<IDictionary<string, object>> results = q.GetResults(new object());
			// we have to actually use the IEnumerable or it won't execute
			// since it is generated
			Assert.Throws<ArgumentOutOfRangeException>(() => results.GetEnumerator().MoveNext());
		}

		[Test]
		public void Show_LabelAlreadyUsed_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Query(typeof(PalasoTestItem)).Show("StoredString").ForEach("ChildItemList").Show(
					"StoredString"));
		}

		[Test]
		public void Show_DifferentLabel_DoesNotThrow()
		{
			new Query(typeof(PalasoTestItem)).Show("StoredString").ForEach("ChildItemList").Show(
					"StoredString", "ChildrenStoredString");
		}

		[Test]
		public void Show_LabelAlreadyUsedInBranch_Throws()
		{
			Query query = new Query(typeof(PalasoTestItem));
			query.In("Child").Show("StoredString");
			Assert.Throws<ArgumentOutOfRangeException>(() => query.ForEach("ChildItemList").Show("StoredString"));
		}

		[Test]
		public void Show_DifferentLabelInBranch_Okay()
		{
			Query query = new Query(typeof(PalasoTestItem));
			query.In("Child").Show("StoredString", "ChildStoredString");
			query.ForEach("ChildItemList").Show("StoredString", "ChildrenStoredString");
		}

		[Test]
		public void ShowEach_LabelAlreadyUsed_Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				new Query(typeof(TestMultiple)).ShowEach("Strings", "a").ShowEach("KeyValuePairs", "a"));
		}

		[Test]
		public void ShowEach_DifferentLabel_Okay()
		{
			new Query(typeof(TestMultiple)).ShowEach("Strings", "a").ShowEach("KeyValuePairs", "b");
		}
	}

	[TestFixture]
	public class EmptyItemQueryTests
	{
		private PalasoTestItem item;

		[SetUp]
		public void Setup()
		{
			item = new PalasoTestItem();
		}

		[Test]
		public void GetResultsOnQuery_WithNoShow_NoItems()
		{
			Query all = new Query(typeof(PalasoTestItem));
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResultsOnInQuery_WithNoShow_NoItems()
		{
			Query all = new Query(typeof(PalasoTestItem)).In("Child");
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResultsOnQuery_AtLeastOneWithShow_OneItem()
		{
			// In the query below AtLeastOne and Show are siblings,
			// both are children of 'all'.
			QueryAdapter<PalasoTestItem> all = new QueryAdapter<PalasoTestItem>();
			all.AtLeastOne().Show("StoredString");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(null, results[0]["StoredString"]);
		}

		[Test]
		public void GetResultsOnQuery_AtLeastOneButNoShow_OneItem()
		{
			// In the query below AtLeastOne and Show are siblings,
			// both are children of 'all'.
			Query all = new Query(typeof(PalasoTestItem)).AtLeastOne();
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(null, results[0][""]);
		}

		[Test]
		public void GetResults_Show_OneNullItem()
		{
			Query all = new Query(typeof(PalasoTestItem)).Show("StoredString");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					all.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(null, results[0]["StoredString"]);
		}

		[Test]
		public void GetResults_ForEachButNoShow_NoItems()
		{
			Query all = new Query(typeof(PalasoTestItem)).ForEach("ChildItemList");
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ShowStoredIntForEachChildren_NoItems()
		{
			Query allStoredIntsInChildren =
					new Query(typeof(PalasoTestItem)).ForEach("ChildItemList").Show("StoredInt");
			IEnumerable<IDictionary<string, object>> results =
					allStoredIntsInChildren.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}
	}

	[TestFixture]
	public class NestedItemQueryTests
	{
		private PalasoTestItem item;

		[SetUp]
		public void Setup()
		{
			item = new PalasoTestItem("top", 1, DateTime.UtcNow);
			item.Child = new PalasoChildTestItem(null, 24, DateTime.Now);
			item.Child.Child = new PalasoChildTestItem("hello", 16, DateTime.Now);

			item.ChildItemList = new List<PalasoChildTestItem>();
			item.ChildItemList.Add(new PalasoChildTestItem("1", 1, DateTime.Now));
			item.ChildItemList.Add(new PalasoChildTestItem("2", 2, DateTime.Now));
			item.ChildItemList.Add(new PalasoChildTestItem("3", 3, DateTime.Now));
			item.ChildItemList[0].Child = new PalasoChildTestItem("hello1", 1, DateTime.Now);
			item.ChildItemList[1].Child = new PalasoChildTestItem("hello2", 2, DateTime.Now);
			item.ChildItemList[2].Child = new PalasoChildTestItem("hello3", 3, DateTime.Now);
		}

		[Test]
		public void GetResultsOnInQuery_WithNoShow_NoItems()
		{
			Query all = new Query(typeof(PalasoTestItem)).In("Child");
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResultsOnInQuery_WithShow_OneIntItem()
		{
			Query all = new Query(typeof(PalasoTestItem)).In("Child").Show("StoredInt");
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {new Result(new KV("StoredInt", 24))};

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}



		[Test]
		public void GetResultsOnInQuery_WithShowSelectingNullItem_OneNullItem()
		{
			Query all = new Query(typeof(PalasoTestItem)).In("Child").Show("StoredString");
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(
																			  new KV(
																					  "StoredString",
																					  null))
															  };

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnInQuery_With3NestedChildThirdIsNull_NoItems()
		{
			Query all = new Query(typeof(PalasoTestItem)).In("Child").In("Child").In("Child").Show("StoredString");
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[] { };

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnInQuery_AtLeastOneWithShow_SameAsWithShow()
		{
			Query queryWithAtLeastOne = new Query(typeof(PalasoTestItem)).In("Child").AtLeastOne().Show("StoredString");
			Query queryWithOutAtLeastOne = new Query(typeof(PalasoTestItem)).In("Child").Show("StoredString");
			IEnumerable<IDictionary<string, object>> results = queryWithAtLeastOne.GetResults(item);
			IEnumerable<IDictionary<string, object>> expectedResult = queryWithOutAtLeastOne.GetResults(item);
			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnInQuery_WhereTrue_Item()
		{
			Query query = new Query(typeof(PalasoTestItem)).In("Child").
				Where("StoredInt",
					delegate (IDictionary<string, object> data)
					{
						return (int)data["StoredInt"] == 24;
					}).
				AtLeastOne().Show("StoredInt");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(24, results[0]["StoredInt"]);
		}

		[Test]
		public void GetResultsOnInQuery_WhereFalse_NoItems()
		{
			Query query = new Query(typeof(PalasoTestItem)).In("Child").
				Where("StoredInt",
					delegate (IDictionary<string, object> data)
					{
						int storedInt = (int)data["StoredInt"];
						return storedInt != 24; //will not match the item
					}).Show("StoredInt");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetResultsOnInQuery_WhereFilterAllItemsAtLeastOne_HasOne()
		{
			Query query = new Query(typeof(PalasoTestItem)).In("Child").
				Where("StoredInt",
					delegate (IDictionary<string, object> data)
					{
						int storedInt = (int)data["StoredInt"];
						return storedInt != 24; //will not match the item
					}).
				AtLeastOne().Show("StoredInt");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(null, results[0]["StoredInt"]);
		}

		[Test]
		public void GetResultsOnQuery_WhereTrue_Item()
		{
			Query query = new Query(typeof(PalasoTestItem)).
				Where("StoredString",
				delegate (IDictionary<string, object> data)
					{
						string storedString = (string)data["StoredString"];
						return storedString == "top";
					}).
				Show("StoredString");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
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
			Query query = new Query(typeof(PalasoTestItem)).
				Where("StoredString",
					delegate (IDictionary<string, object> data)
					{
						return (string)data["StoredString"] == "cantMatchMe";
					}).
				Show("StoredString");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetResultsOnQuery_WhereFilterAllItemsAtLeastOne_HasOne()
		{
			Query query = new Query(typeof(PalasoTestItem)).
				Where("StoredString",
					delegate (IDictionary<string, object> data)
					{
						return (string)data["StoredString"] == "cantMatchMe";
					}).
				AtLeastOne().Show("StoredString");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(null, results[0]["StoredString"]);
		}

		[Test]
		public void GetResultsOnQuery_AtLeastOneWithShow_SameAsWithShow()
		{
			Query queryWithAtLeastOne = new Query(typeof(PalasoTestItem)).AtLeastOne().Show("StoredString");
			Query queryWithOutAtLeastOne = new Query(typeof(PalasoTestItem)).Show("StoredString");
			IEnumerable<IDictionary<string, object>> results = queryWithAtLeastOne.GetResults(item);
			IEnumerable<IDictionary<string, object>> expectedResult = queryWithOutAtLeastOne.GetResults(item);
			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnForEachQuery_WhereTrue_Item()
		{
			Query query = new Query(typeof(PalasoTestItem)).ForEach("ChildItemList").
				Where("StoredInt",
					delegate (IDictionary<string, object> data)
					{
						return (int)data["StoredInt"] < 2; // This will return one item
					}).
				Show("StoredInt");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(1, results[0]["StoredInt"]);
		}

		[Test]
		public void GetResultsOnForEachQuery_WhereFalse_NoItems()
		{
			Query query = new Query(typeof(PalasoTestItem)).ForEach("ChildItemList").
				Where("StoredInt",
					delegate (IDictionary<string, object> data)
					{
						return (int)data["StoredInt"] < 0; // Will filter all items
					}).
				Show("StoredInt");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetResultsOnForEachQuery_WhereFilterAllItemsAtLeastOne_HasOne()
		{
			Query query = new Query(typeof(PalasoTestItem)).ForEach("ChildItemList").
				Where("StoredInt",
					delegate (IDictionary<string, object> data)
					{
						return (int)data["StoredInt"] < 0; // Will filter all items;
					}).
				AtLeastOne().Show("StoredInt");
			List<IDictionary<string, object>> results = new List<IDictionary<string, object>>
				(
					query.GetResults(item)
				);
			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(null, results[0]["StoredInt"]);
		}

		[Test]
		public void GetResultsOnForEachQuery_AtLeastOneWithShow_SameAsWithShow()
		{
			Query queryWithAtLeastOne = new Query(typeof(PalasoTestItem)).ForEach("ChildItemList").AtLeastOne().Show("StoredString");
			Query queryWithOutAtLeastOne = new Query(typeof(PalasoTestItem)).ForEach("ChildItemList").Show("StoredString");
			IEnumerable<IDictionary<string, object>> results = queryWithAtLeastOne.GetResults(item);
			IEnumerable<IDictionary<string, object>> expectedResult = queryWithOutAtLeastOne.GetResults(item);
			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResultsOnQuery_WhereReturnsFalseAtHigherQueryLevel_NoItems()
		{
			Query query = new Query(typeof(PalasoTestItem))
				.Where("StoredString",
					delegate (IDictionary<string, object> data)
						{
							return false;
						})
				.In("Child").Show("StoredString");
			List<IDictionary<string, object>> results =
				new List<IDictionary<string, object>>(query.GetResults(item));
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetResultsOnInQuery_WhereReturnsFalseAtHigherQueryLevel_NoItems()
		{
			Query query = new Query(typeof(PalasoTestItem)).In("Child")
				.Where("StoredString",
					delegate (IDictionary<string, object> data)
					{
						return false;
					})
				.In("Child").Show("StoredString");
			List<IDictionary<string, object>> results =
				new List<IDictionary<string, object>>(query.GetResults(item));
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetResultsOnForEachQuery_WhereReturnsFalseAtHigherQueryLevel_NoItems()
		{
			Query query = new Query(typeof(PalasoTestItem)).ForEach("ChildItemList")
				.Where("StoredString",
					delegate (IDictionary<string, object> data)
					{
						return false;
					})
				.In("Child").Show("StoredString");
			List<IDictionary<string, object>> results =
				new List<IDictionary<string, object>>(query.GetResults(item));
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}

		[Test]
		public void GetResultsOnInQuery_WhereUsesMultipleFieldsToFilter_NoEntries()
		{
			Query query = new Query(typeof(PalasoTestItem))
				.Where(new string[] { "StoredString", "StoredInt" },
					delegate (IDictionary<string, object> data)
					{
						if ((string)data["StoredString"] == "top")
						{
							if ((int)data["StoredInt"] == 1)
							{
								return false;
							}
						}
						return true;
					})
				.Show("StoredString");
			List<IDictionary<string, object>> results =
				new List<IDictionary<string, object>>(query.GetResults(item));
			Assert.IsNotNull(results);
			Assert.AreEqual(0, results.Count);
		}
	}

	[TestFixture]
	public class ItemWithNullQueryTests
	{
		private PalasoTestItem item;

		[SetUp]
		public void Setup()
		{
			item = new PalasoTestItem("top", 1, DateTime.UtcNow);
			item.Child = new PalasoChildTestItem(null, 24, DateTime.Now);
			item.ChildItemList = new List<PalasoChildTestItem>();
			item.ChildItemList.Add(new PalasoChildTestItem("1", 1, DateTime.Now));
			item.ChildItemList.Add(new PalasoChildTestItem("2", 2, DateTime.Now));
			item.ChildItemList.Add(new PalasoChildTestItem("3", 3, DateTime.Now));
		}

		[Test]
		public void GetResults_NoShow_NoItems()
		{
			Query all = new Query(typeof(PalasoTestItem));
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ForEachButNoShow_NoItems()
		{
			Query all = new Query(typeof(PalasoTestItem)).ForEach("ChildItemList");
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ShowStoredIntForEachChildren_ThreeItems()
		{
			Query allStoredIntsInChildren =
					new Query(typeof(PalasoTestItem)).ForEach("ChildItemList").Show("StoredInt");

			IEnumerable<IDictionary<string, object>> results =
					allStoredIntsInChildren.GetResults(item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(new KV("StoredInt", 1)),
																	  new Result(new KV("StoredInt", 2)),
																	  new Result(new KV("StoredInt", 3))
															  };

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowStoredStringAndStoredStringsForEachChildren_ThreeItems()
		{
			Query allStoredIntsInChildren =
					new Query(typeof(PalasoTestItem)).Show("StoredString").ForEach("ChildItemList").Show(
							"StoredString", "ChildStoredString");

			IEnumerable<IDictionary<string, object>> results =
					allStoredIntsInChildren.GetResults(item);

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
			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void
				GetResults_ShowStoredStringChildStoredStringAndStoredStringsForEachChildren_ThreeItems
				()
		{
			Query query = new Query(typeof(PalasoTestItem)).Show("StoredString");
			query.In("Child").Show("StoredString", "ChildStoredString");
			query.ForEach("ChildItemList").Show("StoredString", "ChildrenStoredString");

			IEnumerable<IDictionary<string, object>> results = query.GetResults(item);

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

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
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
			Query all = new Query(typeof(TestMultiple));
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_InButNoShow_NoItems()
		{
			Query all = new Query(typeof(TestMultiple)).In("String");
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ForEachButNoShow_NoItems()
		{
			Query all = new Query(typeof(TestMultiple)).ForEach("Strings");
			IEnumerable<IDictionary<string, object>> results = all.GetResults(item);
			Assert.IsNotNull(results);
			Assert.IsFalse(results.GetEnumerator().MoveNext()); // has no items
		}

		[Test]
		public void GetResults_ShowEachStrings_AllStrings()
		{
			Query allStrings = new Query(typeof(TestMultiple)).ShowEach("Strings");
			IEnumerable<IDictionary<string, object>> results = allStrings.GetResults(item);

			Dictionary<string, object>[] expectedResult = new Dictionary<string, object>[]
															  {
																	  new Result(new KV("Strings",
																						"string1")),
																	  new Result(new KV("Strings",
																						"string2"))
															  };

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowEachStringsMergedWithEachValueOfKeyValuePairs()
		{
			Query allStrings =
					new Query(typeof(TestMultiple)).ShowEach("Strings").ForEach("KeyValuePairs").
							Show("Value");
			IEnumerable<IDictionary<string, object>> results = allStrings.GetResults(item);

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

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowEachStringsMergedWithEachKeyValueOfKeyValuePairs()
		{
			Query allStrings =
					new Query(typeof(TestMultiple)).ShowEach("Strings").ForEach("KeyValuePairs").
							Show("Key").Show("Value");
			IEnumerable<IDictionary<string, object>> results = allStrings.GetResults(item);

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

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowEachStringsMergedWithEachKeyValueOfKeyValuePairsMergedWithString()
		{
			Query allStrings =
					new Query(typeof(TestMultiple)).ShowEach("Strings").Show("String").ForEach(
							"KeyValuePairs").Show("Key").Show("Value");
			IEnumerable<IDictionary<string, object>> results = allStrings.GetResults(item);

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

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}

		[Test]
		public void GetResults_ShowStringMergedWithEachStringsMergedWithEachKeyValueOfKeyValuePairs()
		{
			Query allStrings =
					new Query(typeof(TestMultiple)).Show("String").ShowEach("Strings").ForEach(
							"KeyValuePairs").Show("Key").Show("Value");
			IEnumerable<IDictionary<string, object>> results = allStrings.GetResults(item);

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

			Asserter.Assert(new DictionaryContentAsserter<string, object>(expectedResult, results));
		}
	}
}