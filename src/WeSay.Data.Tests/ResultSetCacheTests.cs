using System;
using System.Collections.Generic;
using Palaso.Data;
using WeSay.Data;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class ResultSetCacheTestsWithSingleRecordTokensFromQuery
	{
		private MemoryDataMapper<TestItem> _dataMapper;
		private SortDefinition[] _sortDefinitions;
		private ResultSet<TestItem> _results;
		private QueryAdapter<TestItem> _queryToCache;

		[SetUp]
		public void Setup()
		{
			_dataMapper = new MemoryDataMapper<TestItem>();
			PopulateRepositoryWithItemsForQuerying(_dataMapper);
			_queryToCache = new QueryAdapter<TestItem>();
			_queryToCache.Show("StoredString");
			_results = _dataMapper.GetItemsMatching(_queryToCache);
			_sortDefinitions = new SortDefinition[1];
			_sortDefinitions[0] = new SortDefinition("StoredString", Comparer<string>.Default);
		}

		[TearDown]
		public void Teardown()
		{
			_dataMapper.Dispose();
		}

		private void PopulateRepositoryWithItemsForQuerying(MemoryDataMapper<TestItem> dataMapper)
		{
			TestItem[] items = new TestItem[3];
			items[0] = dataMapper.CreateItem();
			items[0].StoredString = "Item 3";
			dataMapper.SaveItem(items[0]);
			items[1] = dataMapper.CreateItem();
			items[1].StoredString = "Item 0";
			dataMapper.SaveItem(items[1]);
			items[2] = dataMapper.CreateItem();
			items[2].StoredString = "Item 2";
			dataMapper.SaveItem(items[2]);
		}

		[Test]
		public void Constructor_IsPassedUnsortedResultSet_GetResultSetReturnsSorted()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			Assert.AreEqual(3, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_RepositoryNull_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(null, _sortDefinitions, _results, _queryToCache);
		}

		[Test]
		public void Constructor_SortDefinitionNull_SortedByRepositoryId()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, null, _results, _queryToCache);
			ResultSet<TestItem> resultSet = resultSetCacheUnderTest.GetResultSet();
			Assert.AreEqual(3, resultSet.Count);
			for (int recordTokenNum = 0; recordTokenNum < (resultSet.Count - 1); recordTokenNum++)
			{
				Assert.IsTrue(resultSet[recordTokenNum].Id.CompareTo(resultSet[recordTokenNum + 1].Id) < 0);
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void UpdateItemInCache_Null_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.UpdateItemInCache(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void UpdateItemInCache_ItemDoesNotExistInRepository_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			TestItem itemNotInRepository = new TestItem();
			resultSetCacheUnderTest.UpdateItemInCache(itemNotInRepository);
		}

		[Test]
		public void UpdateItemInCache_ItemDoesNotExistInCacheButDoesInRepository_ItemIsAddedToResultSetAndSortedCorrectly()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);

			TestItem itemCreatedAfterCache = _dataMapper.CreateItem();
			itemCreatedAfterCache.StoredString = "Item 1";
			_dataMapper.SaveItem(itemCreatedAfterCache);
			resultSetCacheUnderTest.UpdateItemInCache(itemCreatedAfterCache);

			Assert.AreEqual(4, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredString"]);
		}

		[Test]
		public void UpdateItemInCache_ItemExists_ResultSetIsUpdatedAndSorted()
		{
			TestItem itemToModify = _dataMapper.CreateItem();
			itemToModify.StoredString = "Item 5";
			_dataMapper.SaveItem(itemToModify);

			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);

			itemToModify.StoredString = "Item 1";
			_dataMapper.SaveItem(itemToModify);
			resultSetCacheUnderTest.UpdateItemInCache(itemToModify);

			Assert.AreEqual(4, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredString"]);
		}

		[Test]
		public void UpdateItemInCache_ItemHasNotChanged_ResultSetIsNotChanged()
		{
			TestItem unmodifiedItem = _dataMapper.CreateItem();
			unmodifiedItem.StoredString = "Item 1";
			_dataMapper.SaveItem(unmodifiedItem);

			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.UpdateItemInCache(unmodifiedItem);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(4, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredString"]);
		}

		[Test]
		public void DeleteItemFromCache_Item_ItemIsNoLongerInResultSet()
		{
			TestItem itemToDelete = _dataMapper.CreateItem();
			itemToDelete.StoredString = "Item 1";
			_dataMapper.SaveItem(itemToDelete);

			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(itemToDelete);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(3, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemFromCache_Null_Throws()
		{
			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache((TestItem) null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItemFromCache_ItemNotInRepository_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(new TestItem());
		}

		[Test]
		public void DeleteItemFromCacheById_Id_ItemIsNoLongerInResultSet()
		{
			TestItem itemToDelete = _dataMapper.CreateItem();
			itemToDelete.StoredString = "Item 1";
			_dataMapper.SaveItem(itemToDelete);

			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(itemToDelete);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(3, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemFromCacheById_Null_Throws()
		{
			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache((TestItem)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItemFromCacheById_ItemNotInRepository_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(RepositoryId.Empty);
		}

		[Test]
		public void DeleteAllItemsFromCache_AllItemsAreDeleted()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteAllItemsFromCache();
			Assert.AreEqual(0, resultSetCacheUnderTest.GetResultSet().Count);
		}
	}

	[TestFixture]
	public class ResultSetCacheTestsWithMultipleRecordTokensFromQuery
	{
		private MemoryDataMapper<TestItem> dataMapper;
		private SortDefinition[] _sortDefinitions;
		private ResultSet<TestItem> _results;
		private QueryAdapter<TestItem> _queryToCache = null;

		[SetUp]
		public void Setup()
		{
			dataMapper = new MemoryDataMapper<TestItem>();
			PopulateRepositoryWithItemsForQuerying(dataMapper);
			_queryToCache = new QueryAdapter<TestItem>();
			_queryToCache.ShowEach("StoredList");
			_results = dataMapper.GetItemsMatching(_queryToCache);
			_sortDefinitions = new SortDefinition[1];
			_sortDefinitions[0] = new SortDefinition("StoredList", Comparer<string>.Default);
		}

		[TearDown]
		public void Teardown()
		{
			dataMapper.Dispose();
		}

		private void PopulateRepositoryWithItemsForQuerying(MemoryDataMapper<TestItem> dataMapper)
		{
			TestItem[] items = new TestItem[2];
			items[0] = dataMapper.CreateItem();
			items[0].StoredList = PopulateListWith("Item 1", "Item 3");
			dataMapper.SaveItem(items[0]);

			items[1] = dataMapper.CreateItem();
			items[1].StoredList = PopulateListWith("Item 2", "Item 0");
			dataMapper.SaveItem(items[1]);
		}

		public List<string> PopulateListWith(string string1, string string2)
		{
			List<string> listTopopulate = new List<string>();
			listTopopulate.Add(string1);
			listTopopulate.Add(string2);
			return listTopopulate;
		}

		[Test]
		public void Constructor_IsPassedUnsortedResultSet_GetResultSetReturnsSorted()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			Assert.AreEqual(4, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredList"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredList"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredList"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredList"]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void UpdateItemInCache_Null_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.UpdateItemInCache(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void UpdateItemInCache_ItemDoesNotExistInRepository_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			TestItem itemNotInRepository = new TestItem();
			resultSetCacheUnderTest.UpdateItemInCache(itemNotInRepository);
		}

		[Test]
		public void UpdateItemInCache_ItemDoesNotExistInCacheButDoesInRepository_ItemIsAddedToResultSetAndSortedCorrectly()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			TestItem itemCreatedAfterCache = dataMapper.CreateItem();
			itemCreatedAfterCache.StoredList = PopulateListWith("Item 5", "Item 4");
			dataMapper.SaveItem(itemCreatedAfterCache);
			resultSetCacheUnderTest.UpdateItemInCache(itemCreatedAfterCache);

			Assert.AreEqual(6, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredList"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredList"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredList"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredList"]);
			Assert.AreEqual("Item 4", resultSetCacheUnderTest.GetResultSet()[4]["StoredList"]);
			Assert.AreEqual("Item 5", resultSetCacheUnderTest.GetResultSet()[5]["StoredList"]);
		}

		[Test]
		public void UpdateItemInCache_ItemExists_ResultSetIsUpdatedAndSorted()
		{
			TestItem itemToModify = dataMapper.CreateItem();
			itemToModify.StoredList = PopulateListWith("Change Me!", "Me 2!");
			dataMapper.SaveItem(itemToModify);

			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			itemToModify.StoredList = PopulateListWith("Item 5", "Item 4");
			dataMapper.SaveItem(itemToModify);
			resultSetCacheUnderTest.UpdateItemInCache(itemToModify);

			Assert.AreEqual(6, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredList"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredList"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredList"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredList"]);
			Assert.AreEqual("Item 4", resultSetCacheUnderTest.GetResultSet()[4]["StoredList"]);
			Assert.AreEqual("Item 5", resultSetCacheUnderTest.GetResultSet()[5]["StoredList"]);
		}

		[Test]
		public void UpdateItemInCache_ItemHasNotChanged_ResultSetIsNotChanged()
		{
			TestItem unmodifiedItem = dataMapper.CreateItem();
			unmodifiedItem.StoredList = PopulateListWith("Item 5", "Item 4");
			dataMapper.SaveItem(unmodifiedItem);

			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			resultSetCacheUnderTest.UpdateItemInCache(unmodifiedItem);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(6, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredList"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredList"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredList"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredList"]);
			Assert.AreEqual("Item 4", resultSetCacheUnderTest.GetResultSet()[4]["StoredList"]);
			Assert.AreEqual("Item 5", resultSetCacheUnderTest.GetResultSet()[5]["StoredList"]);
		}


		[Test]
		public void DeleteItemFromCache_Item_ItemIsNoLongerInResultSet()
		{
			TestItem itemToDelete = dataMapper.CreateItem();
			itemToDelete.StoredList = PopulateListWith("Item 5", "Item 4");
			dataMapper.SaveItem(itemToDelete);

			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(itemToDelete);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(4, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredList"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredList"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredList"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredList"]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemFromCache_Null_Throws()
		{
			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache((TestItem)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItemFromCache_ItemNotInRepository_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(new TestItem());
		}

		[Test]
		public void DeleteItemFromCacheById_Item_ItemIsNoLongerInResultSet()
		{
			TestItem itemToDelete = dataMapper.CreateItem();
			itemToDelete.StoredList = PopulateListWith("Item 5", "Item 4");
			dataMapper.SaveItem(itemToDelete);

			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(dataMapper.GetId(itemToDelete));

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(4, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredList"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredList"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredList"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredList"]);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemFromCacheById_Null_Throws()
		{
			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache((TestItem)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItemFromCacheById_ItemNotInRepository_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(RepositoryId.Empty);
		}

		[Test]
		public void DeleteAllItemsFromCache_AllItemsAreDeleted()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteAllItemsFromCache();
			Assert.AreEqual(0, resultSetCacheUnderTest.GetResultSet().Count);
		}
	}

	[TestFixture]
	public class ResultSetCacheWithMultipleQueries
	{
		private MemoryDataMapper<TestItem> dataMapper;
		private SortDefinition[] _sortDefinitions;
		private ResultSet<TestItem> _results;
		private QueryAdapter<TestItem> _queryToCache;

		[SetUp]
		public void SetUp()
		{
			dataMapper = new MemoryDataMapper<TestItem>();
			PopulateRepositoryWithItemsForQuerying(dataMapper);

			_queryToCache = new QueryAdapter<TestItem>();
			_queryToCache.Show("StoredString");

			_results = dataMapper.GetItemsMatching(_queryToCache);

			_sortDefinitions = new SortDefinition[1];
			_sortDefinitions[0] = new SortDefinition("StoredString", Comparer<string>.Default);
		}

		private void PopulateRepositoryWithItemsForQuerying(MemoryDataMapper<TestItem> dataMapper)
		{
			TestItem[] items = new TestItem[3];
			items[0] = dataMapper.CreateItem();
			items[0].StoredString = "Item 3";
			items[0].Child.StoredString = "Item 4";
			dataMapper.SaveItem(items[0]);
			items[1] = dataMapper.CreateItem();
			items[1].StoredString = "Item 0";
			items[1].Child.StoredString = "Item 1";
			dataMapper.SaveItem(items[1]);
			items[2] = dataMapper.CreateItem();
			items[2].StoredString = "Item 2";
			items[2].Child.StoredString = "Item 5";
			dataMapper.SaveItem(items[2]);
		}

		[TearDown]
		public void Teardown()
		{
			dataMapper.Dispose();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Add_ResultsSetNull_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<TestItem> secondQueryToCache = new QueryAdapter<TestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<TestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(null, secondQueryToCache);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Add_QueryNull_Throws()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<TestItem> secondQueryToCache = new QueryAdapter<TestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<TestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, null);
		}

		[Test]
		public void MultipleQueries_QueriedFieldsAreIdentical_ReturnsOnlyOneRecordToken()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<TestItem> secondQueryToCache = new QueryAdapter<TestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<TestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			TestItem itemCreatedAfterCache = dataMapper.CreateItem();
			itemCreatedAfterCache.StoredString = "Item 6";
			itemCreatedAfterCache.Child.StoredString = "Item 6";
			dataMapper.SaveItem(itemCreatedAfterCache);
			resultSetCacheUnderTest.UpdateItemInCache(itemCreatedAfterCache);

			Assert.AreEqual(7, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredString"]);
			Assert.AreEqual("Item 4", resultSetCacheUnderTest.GetResultSet()[4]["StoredString"]);
			Assert.AreEqual("Item 5", resultSetCacheUnderTest.GetResultSet()[5]["StoredString"]);
			Assert.AreEqual("Item 6", resultSetCacheUnderTest.GetResultSet()[6]["StoredString"]);
		}

		[Test]
		public void UpdateItemInCache_ItemDoesNotExistInCacheButDoesInRepository_ItemIsAddedToResultSetAndSortedCorrectly()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<TestItem> secondQueryToCache = new QueryAdapter<TestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<TestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			TestItem itemCreatedAfterCache = dataMapper.CreateItem();
			itemCreatedAfterCache.StoredString = "Item 6";
			dataMapper.SaveItem(itemCreatedAfterCache);
			resultSetCacheUnderTest.UpdateItemInCache(itemCreatedAfterCache);

			Assert.AreEqual(8, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual(null, resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[3]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[4]["StoredString"]);
			Assert.AreEqual("Item 4", resultSetCacheUnderTest.GetResultSet()[5]["StoredString"]);
			Assert.AreEqual("Item 5", resultSetCacheUnderTest.GetResultSet()[6]["StoredString"]);
			Assert.AreEqual("Item 6", resultSetCacheUnderTest.GetResultSet()[7]["StoredString"]);
		}

		[Test]
		public void UpdateItemInCache_ItemExists_ResultSetIsUpdatedAndSorted()
		{
			TestItem itemToModify = dataMapper.CreateItem();
			itemToModify.StoredString = "Item 6";
			dataMapper.SaveItem(itemToModify);

			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<TestItem> secondQueryToCache = new QueryAdapter<TestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<TestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			itemToModify.StoredString = "Item 7";
			dataMapper.SaveItem(itemToModify);
			resultSetCacheUnderTest.UpdateItemInCache(itemToModify);

			Assert.AreEqual(8, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual(null, resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[3]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[4]["StoredString"]);
			Assert.AreEqual("Item 4", resultSetCacheUnderTest.GetResultSet()[5]["StoredString"]);
			Assert.AreEqual("Item 5", resultSetCacheUnderTest.GetResultSet()[6]["StoredString"]);
			Assert.AreEqual("Item 7", resultSetCacheUnderTest.GetResultSet()[7]["StoredString"]);
		}

		[Test]
		public void UpdateItemInCache_ItemHasNotChanged_ResultSetIsNotChanged()
		{
			TestItem unmodifiedItem = dataMapper.CreateItem();
			unmodifiedItem.StoredString = "Item 6";
			dataMapper.SaveItem(unmodifiedItem);

			//_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<TestItem> secondQueryToCache = new QueryAdapter<TestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<TestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			resultSetCacheUnderTest.UpdateItemInCache(unmodifiedItem);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(8, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual(null, resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[3]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[4]["StoredString"]);
			Assert.AreEqual("Item 4", resultSetCacheUnderTest.GetResultSet()[5]["StoredString"]);
			Assert.AreEqual("Item 5", resultSetCacheUnderTest.GetResultSet()[6]["StoredString"]);
			Assert.AreEqual("Item 6", resultSetCacheUnderTest.GetResultSet()[7]["StoredString"]);
		}

		[Test]
		public void DeleteItemFromCache_Item_ItemIsNoLongerInResultSet()
		{
			TestItem itemToDelete = dataMapper.CreateItem();
			itemToDelete.StoredString = "Item 6";
			dataMapper.SaveItem(itemToDelete);

			//_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<TestItem> secondQueryToCache = new QueryAdapter<TestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<TestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			resultSetCacheUnderTest.DeleteItemFromCache(itemToDelete);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(6, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredString"]);
			Assert.AreEqual("Item 4", resultSetCacheUnderTest.GetResultSet()[4]["StoredString"]);
			Assert.AreEqual("Item 5", resultSetCacheUnderTest.GetResultSet()[5]["StoredString"]);
		}

		[Test]
		public void DeleteItemFromCacheById_Id_ItemIsNoLongerInResultSet()
		{
			TestItem itemToDelete = dataMapper.CreateItem();
			itemToDelete.StoredString = "Item 6";
			dataMapper.SaveItem(itemToDelete);

			//_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<TestItem> secondQueryToCache = new QueryAdapter<TestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<TestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			resultSetCacheUnderTest.DeleteItemFromCache(itemToDelete);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(6, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredString"]);
			Assert.AreEqual("Item 4", resultSetCacheUnderTest.GetResultSet()[4]["StoredString"]);
			Assert.AreEqual("Item 5", resultSetCacheUnderTest.GetResultSet()[5]["StoredString"]);
		}

		[Test]
		public void DeleteAllItemsFromCache_AllItemsAreDeleted()
		{
			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<TestItem> secondQueryToCache = new QueryAdapter<TestItem>();
			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<TestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			resultSetCacheUnderTest.DeleteAllItemsFromCache();
			Assert.AreEqual(0, resultSetCacheUnderTest.GetResultSet().Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ResultSetContainsIdenticalRecordTokens_Throws()
		{
			TestItem itemFromWhichToCreateIdenticalRecordTokens = dataMapper.CreateItem();

			itemFromWhichToCreateIdenticalRecordTokens.StoredString = "Get me twice!";

			QueryAdapter<TestItem> query1 = new QueryAdapter<TestItem>();
			query1.Show("StoredString");
			QueryAdapter<TestItem> query2 = new QueryAdapter<TestItem>();
			query2.Show("StoredString");

			ResultSetCache<TestItem> resultSetCacheUnderTest = new ResultSetCache<TestItem>(dataMapper, _sortDefinitions);
			resultSetCacheUnderTest.Add(dataMapper.GetItemsMatching(query1), query1);
			resultSetCacheUnderTest.Add(dataMapper.GetItemsMatching(query2), query2);
		}
	}

}
