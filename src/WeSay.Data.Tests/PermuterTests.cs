using NUnit.Framework;
using SIL.Tests.Data;
using System;
using System.Collections.Generic;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class PermuterTests
	{
		private class KI
		{
			private readonly string key;
			private readonly int value;

			public KI(string key, int value)
			{
				this.key = key;
				this.value = value;
			}

			public string Key
			{
				get { return this.key; }
			}

			public int Value
			{
				get { return this.value; }
			}
		}

		private static Dictionary<string, int> CreateResult(params KI[] kis)
		{
			Dictionary<string, int> result = new Dictionary<string, int>();

			foreach (KI ki in kis)
			{
				result.Add(ki.Key, ki.Value);
			}
			return result;
		}

		private static List<Dictionary<string, int>> GetSingleItemResult()
		{
			List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();
			result.Add(CreateResult(new KI("int", 2)));
			return result;
		}

		private static List<Dictionary<string, int>> GetSingleRowWithTwoItemsResult()
		{
			List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();
			result.Add(CreateResult(new KI("int", 2), new KI("int2", 4)));
			return result;
		}

		private static List<Dictionary<string, int>> GetEmptyResult()
		{
			return new List<Dictionary<string, int>>();
		}

		private static List<Dictionary<string, int>> GetTwoRowsWithSingleItemEachResult()
		{
			List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();
			result.Add(CreateResult(new KI("int", 2)));
			result.Add(CreateResult(new KI("int", 4)));
			return result;
		}

		private static List<Dictionary<string, int>> GetTwoRowsWithSingleItemEachAtInt5Result()
		{
			List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();
			result.Add(CreateResult(new KI("int5", 9)));
			result.Add(CreateResult(new KI("int5", 12)));
			return result;
		}

		private static List<Dictionary<string, int>> GetTwoRowsWithDoubleItemsResult()
		{
			List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();
			result.Add(CreateResult(new KI("int4", 9), new KI("int5", 10)));
			result.Add(CreateResult(new KI("int4", 12), new KI("int5", 13)));
			return result;
		}

		private static List<Dictionary<string, int>> GetOneRowWithTwoItemsOneRowWithSingleItemResult
				()
		{
			List<Dictionary<string, int>> result = new List<Dictionary<string, int>>();
			result.Add(CreateResult(new KI("int", 2), new KI("int2", 3)));
			result.Add(CreateResult(new KI("int", 4)));
			return result;
		}

		private static void AssertResult(IDictionary<string, int>[] expectedResult,
										 IEnumerable<Dictionary<string, int>> actualResult)
		{
			Asserter.Assert(new DictionaryContentAsserter<string, int>(expectedResult, actualResult));
		}

		private static void AssertResult(IEnumerable<IDictionary<string, int>> expectedResult,
										 IEnumerable<IDictionary<string, int>> actualResult)
		{
			Asserter.Assert(new DictionaryContentAsserter<string, int>(expectedResult, actualResult));
		}

		[Test]
		public void Permute_NullResultSingleItem_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => Permuter.Permute(null, "", 9));
		}

		[Test]
		public void Permute_NullSingleItem_Succeeds()
		{
			var result = new List<Dictionary<string, object>>();
			Permuter.Permute(result, "", (object)null);
		}

		[Test]
		public void Permute_NullResultListOfItems_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => Permuter.Permute(null, "", new int[] { 9 }));
		}

		[Test]
		public void Permute_NullListOfItems_Throws()
		{
			List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
			Assert.Throws<ArgumentNullException>(() => Permuter.Permute(result, null));
		}

		[Test]
		public void Permute_NullResultListOfListOfItems_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => Permuter.Permute(null, "", new int[][] { new int[] { 9 } }));
		}

		[Test]
		public void Permute_NullResultListResults_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => Permuter.Permute(null, GetSingleItemResult()));
		}

		[Test]
		public void Permute_Empty_WithSingleItem_SingleItem()
		{
			List<Dictionary<string, int>> result = GetEmptyResult();
			Permuter.Permute(result, "int", 9);

			IDictionary<string, int>[] expectedResult = new IDictionary<string, int>[]
														   {CreateResult(new KI("int", 9))};

			Asserter.Assert(new DictionaryContentAsserter<string, int>(expectedResult, result));
		}

		[Test]
		public void Permute_SingleItem_WithSingleItem_SingleRowWithBothItems()
		{
			List<Dictionary<string, int>> result = GetSingleItemResult();
			Permuter.Permute(result, "value", 9);

			IDictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("value", 9))
														   };
			Asserter.Assert(new DictionaryContentAsserter<string, int>(expectedResult, result));
		}

		[Test]
		public void Permute_SingleRowWithTwoItems_WithSingleItem_SingleRowWithThreeItems()
		{
			List<Dictionary<string, int>> result = GetSingleRowWithTwoItemsResult();
			Permuter.Permute(result, "int3", 9);

			IDictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int2", 4),
																				new KI("int3", 9))
														   };
			Asserter.Assert(new DictionaryContentAsserter<string, int>(expectedResult, result));
		}

		[Test]
		public void Permute_TwoRowsWithSingleItem_WithSingleItem_TwoRowsWithTwoItems()
		{
			List<Dictionary<string, int>> result = GetTwoRowsWithSingleItemEachResult();

			Permuter.Permute(result, "int2", 9);

			IDictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int2", 9)),
																   CreateResult(new KI("int", 4),
																				new KI("int2", 9))
														   };

			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_Empty_WithEmptyList_Empty()
		{
			List<Dictionary<string, int>> result = GetEmptyResult();
			Permuter.Permute(result, "int", new int[] { });

			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetEmptyResult(), result));
		}

		[Test]
		public void Permute_Empty_WithTwoItems_TwoRowsOfSingleItem()
		{
			List<Dictionary<string, int>> result = GetEmptyResult();
			Permuter.Permute(result, "int", new int[] { 9, 12 });

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 9)),
																   CreateResult(new KI("int", 12))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_SingleItem_WithTwoItems_TwoRowsWithSingleItemAndEachOfItems()
		{
			List<Dictionary<string, int>> result = GetSingleItemResult();
			Permuter.Permute(result, "int2", new int[] { 9, 12 });

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int2", 9)),
																   CreateResult(new KI("int", 2),
																				new KI("int2", 12))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_SingleItem_WithEmptyList_NoChange()
		{
			List<Dictionary<string, int>> result = GetSingleItemResult();
			Permuter.Permute(result, "int2", new int[] { });
			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetSingleItemResult(), result));
		}


		[Test]
		public void Permute_SingleRowWithTwoItems_WithTwoItems_TwoRowsWithTheTwoItemsAndEachOfItems()
		{
			List<Dictionary<string, int>> result = GetSingleRowWithTwoItemsResult();
			Permuter.Permute(result, "int3", new int[] { 9, 12 });

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int2", 4),
																				new KI("int3", 9)),
																   CreateResult(new KI("int", 2),
																				new KI("int2", 4),
																				new KI("int3", 12))
														   };

			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_SingleRowWithTwoItems_WithEmptyList_NoChange()
		{
			List<Dictionary<string, int>> result = GetSingleRowWithTwoItemsResult();
			Permuter.Permute(result, "int3", new int[] { });

			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetSingleRowWithTwoItemsResult(), result));
		}

		[Test]
		public void Permute_TwoRowsWithSingleItem_WithTwoItems_4RowsWithTwoItems()
		{
			List<Dictionary<string, int>> result = GetTwoRowsWithSingleItemEachResult();
			Permuter.Permute(result, "int2", new int[] { 9, 12 });

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int2", 9)),
																   CreateResult(new KI("int", 2),
																				new KI("int2", 12)),
																   CreateResult(new KI("int", 4),
																				new KI("int2", 9)),
																   CreateResult(new KI("int", 4),
																				new KI("int2", 12))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_TwoRowsWithSingleItem_WithEmptyList_NoChange()
		{
			List<Dictionary<string, int>> result = GetTwoRowsWithSingleItemEachResult();
			Permuter.Permute(result, "int2", new int[] { });

			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetTwoRowsWithSingleItemEachResult(), result));
		}

		[Test]
		public void
				Permute_OneRowWithTwoItemsOneRowWithSingleItem_WithTwoItems_4RowsWithTwoWithThreeItemsAndTwoWithTwoItems
				()
		{
			List<Dictionary<string, int>> result = GetOneRowWithTwoItemsOneRowWithSingleItemResult();

			Permuter.Permute(result, "int3", new int[] { 9, 12 });

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int2", 3),
																				new KI("int3", 9)),
																   CreateResult(new KI("int", 2),
																				new KI("int2", 3),
																				new KI("int3", 12)),
																   CreateResult(new KI("int", 4),
																				new KI("int3", 9)),
																   CreateResult(new KI("int", 4),
																				new KI("int3", 12))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_Empty_WithEmptyListOfList_Empty()
		{
			List<Dictionary<string, int>> result = GetEmptyResult();
			Permuter.Permute(result, GetEmptyResult());

			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetEmptyResult(), result));
		}

		[Test]
		public void Permute_Empty_WithTwoRowsOfSingleItems_TwoRowsOfSingleItem()
		{
			List<Dictionary<string, int>> result = GetEmptyResult();
			Permuter.Permute(result, GetTwoRowsWithSingleItemEachResult());

			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetTwoRowsWithSingleItemEachResult(), result));
		}

		[Test]
		public void Permute_Empty_WithTwoRowsOfDoubleItems_TwoRowsOfDoubleItems()
		{
			List<Dictionary<string, int>> result = GetEmptyResult();
			Permuter.Permute(result, GetTwoRowsWithDoubleItemsResult());

			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetTwoRowsWithDoubleItemsResult(), result));
		}

		[Test]
		public void Permute_SingleItem_WithTwoRowsOfSingleItems_TwoRowsWithSingleItemAndEachOfItems()
		{
			List<Dictionary<string, int>> result = GetSingleItemResult();
			Permuter.Permute(result, GetTwoRowsWithSingleItemEachAtInt5Result());

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int5", 9)),
																   CreateResult(new KI("int", 2),
																				new KI("int5", 12))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void
				Permute_SingleItem_WithTwoRowsOfDoubleItems_TwoRowsWithSingleItemAndEachOfDoubleItems
				()
		{
			List<Dictionary<string, int>> result = GetSingleItemResult();

			Permuter.Permute(result, GetTwoRowsWithDoubleItemsResult());

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int4", 9),
																				new KI("int5", 10)),
																   CreateResult(new KI("int", 2),
																				new KI("int4", 12),
																				new KI("int5", 13))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_SingleItem_WithEmptyListOfList_NoChange()
		{
			List<Dictionary<string, int>> result = GetSingleItemResult();

			Permuter.Permute(result, GetEmptyResult());

			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetSingleItemResult(), result));
		}

		[Test]
		public void
				Permute_SingleRowWithTwoItems_WithTwoRowsOfSingleItems_TwoRowsWithTheTwoItemsAndEachOfItems
				()
		{
			List<Dictionary<string, int>> result = GetSingleRowWithTwoItemsResult();
			Permuter.Permute(result, GetTwoRowsWithSingleItemEachAtInt5Result());

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int2", 4),
																				new KI("int5", 9)),
																   CreateResult(new KI("int", 2),
																				new KI("int2", 4),
																				new KI("int5", 12))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void
				Permute_SingleRowWithTwoItems_WithTwoRowsOfDoubleItems_TwoRowsWithTheTwoItemsAndEachOfDoubleItems
				()
		{
			List<Dictionary<string, int>> result = GetTwoRowsWithSingleItemEachResult();
			Permuter.Permute(result, GetTwoRowsWithDoubleItemsResult());

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int4", 9),
																				new KI("int5", 10)),
																   CreateResult(new KI("int", 2),
																				new KI("int4", 12),
																				new KI("int5", 13)),
																   CreateResult(new KI("int", 4),
																				new KI("int4", 9),
																				new KI("int5", 10)),
																   CreateResult(new KI("int", 4),
																				new KI("int4", 12),
																				new KI("int5", 13))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_SingleRowWithTwoItems_WithEmptyListOfList_NoChange()
		{
			List<Dictionary<string, int>> result = GetSingleRowWithTwoItemsResult();
			Permuter.Permute(result, GetEmptyResult());

			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetSingleRowWithTwoItemsResult(), result));
		}

		[Test]
		public void Permute_TwoRowsWithSingleItem_WithTwoRowsOfItems_4RowsWithTwoItems()
		{
			List<Dictionary<string, int>> result = GetTwoRowsWithSingleItemEachResult();
			Permuter.Permute(result, GetTwoRowsWithSingleItemEachAtInt5Result());

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int5", 9)),
																   CreateResult(new KI("int", 2),
																				new KI("int5", 12)),
																   CreateResult(new KI("int", 4),
																				new KI("int5", 9)),
																   CreateResult(new KI("int", 4),
																				new KI("int5", 12))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_TwoRowsWithSingleItem_WithTwoRowsOfDoubleItems_4RowsWithThreeItems()
		{
			List<Dictionary<string, int>> result = GetTwoRowsWithSingleItemEachResult();
			Permuter.Permute(result, GetTwoRowsWithDoubleItemsResult());

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int4", 9),
																				new KI("int5", 10)),
																   CreateResult(new KI("int", 2),
																				new KI("int4", 12),
																				new KI("int5", 13)),
																   CreateResult(new KI("int", 4),
																				new KI("int4", 9),
																				new KI("int5", 10)),
																   CreateResult(new KI("int", 4),
																				new KI("int4", 12),
																				new KI("int5", 13))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void Permute_TwoRowsWithSingleItem_WithEmptyListOfList_NoChange()
		{
			List<Dictionary<string, int>> result = GetTwoRowsWithSingleItemEachResult();
			Permuter.Permute(result, GetEmptyResult());

			Asserter.Assert(new DictionaryContentAsserter<string, int>(GetTwoRowsWithSingleItemEachResult(), result));
		}

		[Test]
		public void
				Permute_OneRowWithTwoItemsOneRowWithSingleItem_WithTwoRowsOfSingleItems_4RowsWithTwoWithThreeItemsAndTwoWithTwoItems
				()
		{
			List<Dictionary<string, int>> result = GetOneRowWithTwoItemsOneRowWithSingleItemResult();

			Permuter.Permute(result, GetTwoRowsWithSingleItemEachAtInt5Result());

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int2", 3),
																				new KI("int5", 9)),
																   CreateResult(new KI("int", 2),
																				new KI("int2", 3),
																				new KI("int5", 12)),
																   CreateResult(new KI("int", 4),
																				new KI("int5", 9)),
																   CreateResult(new KI("int", 4),
																				new KI("int5", 12))
														   };
			AssertResult(expectedResult, result);
		}

		[Test]
		public void
				Permute_OneRowWithTwoItemsOneRowWithSingleItem_WithTwoRowsOfDoubleItems_4RowsWithTwoWithFourItemsAndTwoWithThreeItems
				()
		{
			List<Dictionary<string, int>> result = GetOneRowWithTwoItemsOneRowWithSingleItemResult();
			Permuter.Permute(result, GetTwoRowsWithDoubleItemsResult());

			Dictionary<string, int>[] expectedResult = new Dictionary<string, int>[]
														   {
																   CreateResult(new KI("int", 2),
																				new KI("int2", 3),
																				new KI("int4", 9),
																				new KI("int5", 10)),
																   CreateResult(new KI("int", 2),
																				new KI("int2", 3),
																				new KI("int4", 12),
																				new KI("int5", 13)),
																   CreateResult(new KI("int", 4),
																				new KI("int4", 9),
																				new KI("int5", 10)),
																   CreateResult(new KI("int", 4),
																				new KI("int4", 12),
																				new KI("int5", 13))
														   };
			AssertResult(expectedResult, result);
		}
	}
}