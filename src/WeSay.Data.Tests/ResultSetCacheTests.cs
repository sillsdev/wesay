using System;
using System.Collections.Generic;
using Palaso.Data;
using Palaso.Tests.Data;
using WeSay.Data;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	// todo refactor this to using(var session = new TestSession) {} style
	// todo reimplement tests when QueryObject is changed
	[TestFixture]
	public class ResultSetCacheTestsWithSingleRecordTokensFromQuery
	{
		private MemoryDataMapper<PalasoTestItem> _dataMapper;
		private SortDefinition[] _sortDefinitions;
		private ResultSet<PalasoTestItem> _results;
		private QueryAdapter<PalasoTestItem> _queryToCache;

		[SetUp]
		public void Setup()
		{
			_dataMapper = new MemoryDataMapper<PalasoTestItem>();
			PopulateRepositoryWithItemsForQuerying(_dataMapper);
			_queryToCache = new QueryAdapter<PalasoTestItem>();
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

		private void PopulateRepositoryWithItemsForQuerying(MemoryDataMapper<PalasoTestItem> dataMapper)
		{
			var items = new PalasoTestItem[3];
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
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			Assert.AreEqual(3, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_RepositoryNull_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(null, _sortDefinitions, _results, _queryToCache);
		}

		[Test]
		public void Constructor_SortDefinitionNull_SortedByRepositoryId()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, null, _results, _queryToCache);
			ResultSet<PalasoTestItem> resultSet = resultSetCacheUnderTest.GetResultSet();
			Assert.AreEqual(3, resultSet.Count);
			for (int recordTokenNum = 0; recordTokenNum < (resultSet.Count - 1); recordTokenNum++)
			{
				Assert.IsTrue(resultSet[recordTokenNum].Id.CompareTo(resultSet[recordTokenNum + 1].Id) < 0);
			}
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentNullException))]
		public void UpdateItemInCache_Null_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.UpdateItemInCache(null);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void UpdateItemInCache_ItemDoesNotExistInRepository_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			PalasoTestItem itemNotInRepository = new PalasoTestItem();
			resultSetCacheUnderTest.UpdateItemInCache(itemNotInRepository);
		}

		[Test]
		public void UpdateItemInCache_ItemDoesNotExistInCacheButDoesInRepository_ItemIsAddedToResultSetAndSortedCorrectly()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);

			PalasoTestItem itemCreatedAfterCache = _dataMapper.CreateItem();
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
			PalasoTestItem itemToModify = _dataMapper.CreateItem();
			itemToModify.StoredString = "Item 5";
			_dataMapper.SaveItem(itemToModify);

			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);

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
			PalasoTestItem unmodifiedItem = _dataMapper.CreateItem();
			unmodifiedItem.StoredString = "Item 1";
			_dataMapper.SaveItem(unmodifiedItem);

			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
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
			PalasoTestItem itemToDelete = _dataMapper.CreateItem();
			itemToDelete.StoredString = "Item 1";
			_dataMapper.SaveItem(itemToDelete);

			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(itemToDelete);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(3, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemFromCache_Null_Throws()
		{
			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache((PalasoTestItem) null);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItemFromCache_ItemNotInRepository_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(new PalasoTestItem());
		}

		[Test]
		public void DeleteItemFromCacheById_Id_ItemIsNoLongerInResultSet()
		{
			PalasoTestItem itemToDelete = _dataMapper.CreateItem();
			itemToDelete.StoredString = "Item 1";
			_dataMapper.SaveItem(itemToDelete);

			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(itemToDelete);

			//Would be a better test but ResultSets don't support equality checks
			//Assert.AreEqual(resultsBeforeUpdate, resultsAfterUpdate);

			Assert.AreEqual(3, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredString"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[1]["StoredString"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[2]["StoredString"]);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemFromCacheById_Null_Throws()
		{
			_results = _dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache((PalasoTestItem)null);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItemFromCacheById_ItemNotInRepository_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(RepositoryId.Empty);
		}

		[Test]
		public void DeleteAllItemsFromCache_AllItemsAreDeleted()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(_dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteAllItemsFromCache();
			Assert.AreEqual(0, resultSetCacheUnderTest.GetResultSet().Count);
		}
	}

	[TestFixture]
	public class ResultSetCacheTestsWithMultipleRecordTokensFromQuery
	{
		private MemoryDataMapper<PalasoTestItem> dataMapper;
		private SortDefinition[] _sortDefinitions;
		private ResultSet<PalasoTestItem> _results;
		private QueryAdapter<PalasoTestItem> _queryToCache = null;

		[SetUp]
		public void Setup()
		{
			dataMapper = new MemoryDataMapper<PalasoTestItem>();
			PopulateRepositoryWithItemsForQuerying(dataMapper);
			_queryToCache = new QueryAdapter<PalasoTestItem>();
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

		private void PopulateRepositoryWithItemsForQuerying(MemoryDataMapper<PalasoTestItem> dataMapper)
		{
			PalasoTestItem[] items = new PalasoTestItem[2];
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
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			Assert.AreEqual(4, resultSetCacheUnderTest.GetResultSet().Count);
			Assert.AreEqual("Item 0", resultSetCacheUnderTest.GetResultSet()[0]["StoredList"]);
			Assert.AreEqual("Item 1", resultSetCacheUnderTest.GetResultSet()[1]["StoredList"]);
			Assert.AreEqual("Item 2", resultSetCacheUnderTest.GetResultSet()[2]["StoredList"]);
			Assert.AreEqual("Item 3", resultSetCacheUnderTest.GetResultSet()[3]["StoredList"]);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentNullException))]
		public void UpdateItemInCache_Null_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.UpdateItemInCache(null);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void UpdateItemInCache_ItemDoesNotExistInRepository_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			PalasoTestItem itemNotInRepository = new PalasoTestItem();
			resultSetCacheUnderTest.UpdateItemInCache(itemNotInRepository);
		}

		[Test]
		public void UpdateItemInCache_ItemDoesNotExistInCacheButDoesInRepository_ItemIsAddedToResultSetAndSortedCorrectly()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			PalasoTestItem itemCreatedAfterCache = dataMapper.CreateItem();
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
			PalasoTestItem itemToModify = dataMapper.CreateItem();
			itemToModify.StoredList = PopulateListWith("Change Me!", "Me 2!");
			dataMapper.SaveItem(itemToModify);

			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

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
			PalasoTestItem unmodifiedItem = dataMapper.CreateItem();
			unmodifiedItem.StoredList = PopulateListWith("Item 5", "Item 4");
			dataMapper.SaveItem(unmodifiedItem);

			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

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
			PalasoTestItem itemToDelete = dataMapper.CreateItem();
			itemToDelete.StoredList = PopulateListWith("Item 5", "Item 4");
			dataMapper.SaveItem(itemToDelete);

			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
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
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemFromCache_Null_Throws()
		{
			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache((PalasoTestItem)null);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItemFromCache_ItemNotInRepository_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(new PalasoTestItem());
		}

		[Test]
		public void DeleteItemFromCacheById_Item_ItemIsNoLongerInResultSet()
		{
			PalasoTestItem itemToDelete = dataMapper.CreateItem();
			itemToDelete.StoredList = PopulateListWith("Item 5", "Item 4");
			dataMapper.SaveItem(itemToDelete);

			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
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
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemFromCacheById_Null_Throws()
		{
			_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache((PalasoTestItem)null);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItemFromCacheById_ItemNotInRepository_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteItemFromCache(RepositoryId.Empty);
		}

		[Test]
		public void DeleteAllItemsFromCache_AllItemsAreDeleted()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);
			resultSetCacheUnderTest.DeleteAllItemsFromCache();
			Assert.AreEqual(0, resultSetCacheUnderTest.GetResultSet().Count);
		}
	}

	[TestFixture]
	public class ResultSetCacheWithMultipleQueries
	{
		private MemoryDataMapper<PalasoTestItem> dataMapper;
		private SortDefinition[] _sortDefinitions;
		private ResultSet<PalasoTestItem> _results;
		private QueryAdapter<PalasoTestItem> _queryToCache;

		[SetUp]
		public void SetUp()
		{
			dataMapper = new MemoryDataMapper<PalasoTestItem>();
			PopulateRepositoryWithItemsForQuerying(dataMapper);

			_queryToCache = new QueryAdapter<PalasoTestItem>();
			_queryToCache.Show("StoredString");

			_results = dataMapper.GetItemsMatching(_queryToCache);

			_sortDefinitions = new SortDefinition[1];
			_sortDefinitions[0] = new SortDefinition("StoredString", Comparer<string>.Default);
		}

		private void PopulateRepositoryWithItemsForQuerying(MemoryDataMapper<PalasoTestItem> dataMapper)
		{
			PalasoTestItem[] items = new PalasoTestItem[3];
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
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentNullException))]
		public void Add_ResultsSetNull_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<PalasoTestItem> secondQueryToCache = new QueryAdapter<PalasoTestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<PalasoTestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(null, secondQueryToCache);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentNullException))]
		public void Add_QueryNull_Throws()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<PalasoTestItem> secondQueryToCache = new QueryAdapter<PalasoTestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<PalasoTestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, null);
		}

		[Test]
		public void MultipleQueries_QueriedFieldsAreIdentical_ReturnsOnlyOneRecordToken()
		{
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<PalasoTestItem> secondQueryToCache = new QueryAdapter<PalasoTestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<PalasoTestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			PalasoTestItem itemCreatedAfterCache = dataMapper.CreateItem();
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
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<PalasoTestItem> secondQueryToCache = new QueryAdapter<PalasoTestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<PalasoTestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			PalasoTestItem itemCreatedAfterCache = dataMapper.CreateItem();
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
			PalasoTestItem itemToModify = dataMapper.CreateItem();
			itemToModify.StoredString = "Item 6";
			dataMapper.SaveItem(itemToModify);

			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<PalasoTestItem> secondQueryToCache = new QueryAdapter<PalasoTestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<PalasoTestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

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
			PalasoTestItem unmodifiedItem = dataMapper.CreateItem();
			unmodifiedItem.StoredString = "Item 6";
			dataMapper.SaveItem(unmodifiedItem);

			//_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<PalasoTestItem> secondQueryToCache = new QueryAdapter<PalasoTestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<PalasoTestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

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
			PalasoTestItem itemToDelete = dataMapper.CreateItem();
			itemToDelete.StoredString = "Item 6";
			dataMapper.SaveItem(itemToDelete);

			//_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<PalasoTestItem> secondQueryToCache = new QueryAdapter<PalasoTestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<PalasoTestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

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
			PalasoTestItem itemToDelete = dataMapper.CreateItem();
			itemToDelete.StoredString = "Item 6";
			dataMapper.SaveItem(itemToDelete);

			//_results = dataMapper.GetItemsMatching(_queryToCache);
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<PalasoTestItem> secondQueryToCache = new QueryAdapter<PalasoTestItem>();

			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<PalasoTestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

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
			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions, _results, _queryToCache);

			QueryAdapter<PalasoTestItem> secondQueryToCache = new QueryAdapter<PalasoTestItem>();
			secondQueryToCache.In("Child").Show("StoredString");
			ResultSet<PalasoTestItem> results = dataMapper.GetItemsMatching(secondQueryToCache);

			resultSetCacheUnderTest.Add(results, secondQueryToCache);

			resultSetCacheUnderTest.DeleteAllItemsFromCache();
			Assert.AreEqual(0, resultSetCacheUnderTest.GetResultSet().Count);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(ArgumentException))]
		public void ResultSetContainsIdenticalRecordTokens_Throws()
		{
			PalasoTestItem itemFromWhichToCreateIdenticalRecordTokens = dataMapper.CreateItem();

			itemFromWhichToCreateIdenticalRecordTokens.StoredString = "Get me twice!";

			QueryAdapter<PalasoTestItem> query1 = new QueryAdapter<PalasoTestItem>();
			query1.Show("StoredString");
			QueryAdapter<PalasoTestItem> query2 = new QueryAdapter<PalasoTestItem>();
			query2.Show("StoredString");

			ResultSetCache<PalasoTestItem> resultSetCacheUnderTest = new ResultSetCache<PalasoTestItem>(dataMapper, _sortDefinitions);
			resultSetCacheUnderTest.Add(dataMapper.GetItemsMatching(query1), query1);
			resultSetCacheUnderTest.Add(dataMapper.GetItemsMatching(query2), query2);
		}
	}

}
