using System.Collections.Generic;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class ResultSetTests
	{
		private ResultSet<TestItem> _resultSet;
		List<RecordToken<TestItem>> _results;
		MemoryRepository<TestItem> _repository;


		[SetUp]
		public void Setup()
		{
			_repository = new MemoryRepository<TestItem>();
			_results = new List<RecordToken<TestItem>>();

			_results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(8)));
			_results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(12)));
			_results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(1)));
			_results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(3)));

			_resultSet = new ResultSet<TestItem>(_repository, _results);
		}

		[TearDown]
		public void Teardown()
		{
			_repository.Dispose();
		}

		[Test]
		public void FindFirstIndex_RepositoryIdEqualToOneInList_Index()
		{
			int index = _resultSet.FindFirstIndex(new TestRepositoryId(1));
			Assert.AreEqual(2, index);
		}

		[Test]
		public void Constructor_HasNoDuplicate_OrderRetained()
		{
			List<RecordToken<TestItem>> results = new List<RecordToken<TestItem>>();

			results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(2)));

			ResultSet<TestItem> resultSet = new ResultSet<TestItem>(_repository, results);
			Assert.AreEqual(2, resultSet.Count);

			int i = 0;
			foreach (RecordToken<TestItem> token in resultSet)
			{
				Assert.AreEqual(results[i], token);
				++i;
			}
		}

		[Test]
		public void Constructor_ResultsModifiedAfter_ResultSetNotModified()
		{
			ResultSet<TestItem> resultSet = new ResultSet<TestItem>(_repository, _results);
			_results.Add(new RecordToken<TestItem>(_repository, _resultSet[2].Id));
			Assert.AreNotEqual(_results.Count, resultSet.Count);

		}

	}
	[TestFixture ]
	public class ResultSetWithQueryTests
	{
		MemoryRepository<TestItem> _repository;
		Dictionary<string, object> _queryResultsEmpty;
		Dictionary<string, object> _queryResultsB;
		Dictionary<string, object> _queryResultsA;

		[SetUp]
		public void Setup()
		{
			_repository = new MemoryRepository<TestItem>();
			_queryResultsA = new Dictionary<string, object>();
			_queryResultsA.Add("string", "A");
			_queryResultsB = new Dictionary<string, object>();
			_queryResultsB.Add("string", "B");
			_queryResultsEmpty = new Dictionary<string, object>();
			_queryResultsEmpty.Add("string", string.Empty);
		}

		[TearDown]
		public void Teardown()
		{
			_repository.Dispose();
		}

		[Test]
		public void Constructor_HasNoDuplicates_NoneRemoved()
		{
			List<RecordToken<TestItem>> results = new List<RecordToken<TestItem>>();

			results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(_repository, _queryResultsEmpty, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(_repository, _queryResultsA, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(_repository, _queryResultsB, new TestRepositoryId(8)));

			ResultSet<TestItem> resultSet = new ResultSet<TestItem>(_repository, results);
			Assert.AreEqual(4, resultSet.Count);
		}

		[Test]
		public void Coalesce_NoItemsCanBeRemoved_NoneRemoved()
		{
			List<RecordToken<TestItem>> results = new List<RecordToken<TestItem>>();

			results.Add(new RecordToken<TestItem>(_repository, _queryResultsA, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(_repository, _queryResultsB, new TestRepositoryId(12)));

			ResultSet<TestItem> resultSet = new ResultSet<TestItem>(_repository, results);
			resultSet.Coalesce("string", delegate(object o)
													{
														return string.IsNullOrEmpty((string)o);
													});
			Assert.AreEqual(2, resultSet.Count);
		}

		[Test]
		public void Coalesce_ItemCanBeRemovedButNoItemThatCannot_NoneRemoved()
		{
			List<RecordToken<TestItem>> results = new List<RecordToken<TestItem>>();

			results.Add(new RecordToken<TestItem>(_repository, _queryResultsEmpty, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(12)));

			ResultSet<TestItem> resultSet = new ResultSet<TestItem>(_repository, results);
			resultSet.Coalesce("string", delegate(object o)
													{
														return string.IsNullOrEmpty((string)o);
													});

			Assert.AreEqual(2, resultSet.Count);
		}

		[Test]
		public void Coalesce_ItemThatCanBeRemovedItemThatCannot_ItemRemoved()
		{
			List<RecordToken<TestItem>> results = new List<RecordToken<TestItem>>();

			results.Add(new RecordToken<TestItem>(_repository, _queryResultsEmpty, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(12)));
			results.Add(new RecordToken<TestItem>(_repository, _queryResultsA, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(_repository, _queryResultsB, new TestRepositoryId(12)));

			ResultSet<TestItem> resultSet = new ResultSet<TestItem>(_repository, results);
			resultSet.Coalesce("string", delegate(object o)
													{
														return string.IsNullOrEmpty((string)o);
													});

			Assert.AreEqual(2, resultSet.Count);

		}

		[Test]
		public void Coalesce_ItemsCanBeRemoved_DoesNotChangeOrder()
		{
			List<RecordToken<TestItem>> results = new List<RecordToken<TestItem>>();

			results.Add(new RecordToken<TestItem>(_repository, new TestRepositoryId(12)));
			results.Add(new RecordToken<TestItem>(_repository, _queryResultsEmpty, new TestRepositoryId(8)));
			results.Add(new RecordToken<TestItem>(_repository, _queryResultsB, new TestRepositoryId(12)));
			results.Add(new RecordToken<TestItem>(_repository, _queryResultsA, new TestRepositoryId(8)));

			ResultSet<TestItem> resultSet = new ResultSet<TestItem>(_repository, results);
			resultSet.Coalesce("string", delegate(object o)
													{
														return string.IsNullOrEmpty((string)o);
													});

			Assert.AreEqual("B", resultSet[0]["string"]);
			Assert.AreEqual("A", resultSet[1]["string"]);

		}

	}
}
