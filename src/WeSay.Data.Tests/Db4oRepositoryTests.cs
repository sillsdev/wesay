using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oRepositoryStateUnitializedTests: IRepositoryStateUnitializedTests<TestItem>
	{
		private string _name;

		[SetUp]
		public override void SetUp()
		{
			this._name = Path.GetTempFileName();
			RepositoryUnderTest = new Db4oRepository<TestItem>(_name);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}
	}

	[TestFixture]
	public class Db4oRepositoryCreatedFromPersistedData:
			IRepositoryPopulateFromPersistedTests<TestItem>
	{
		private string _persistedFilePath;
		private DateTime _timeAtWhichPersisted;
		[SetUp]
		public override void SetUp()
		{
			this._persistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._persistedFilePath);
			TestItem item = RepositoryUnderTest.CreateItem();
			item.StoredInt = 5;
			item.StoredString = "Sonne";
			RepositoryUnderTest.SaveItem(item);
			_timeAtWhichPersisted = RepositoryUnderTest.LastModified;

			CreateNewRepositoryFromPersistedData();
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(this._persistedFilePath);
		}

		protected override void LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime_v()
		{
			Assert.AreEqual(_timeAtWhichPersisted, RepositoryUnderTest.LastModified);
		}

		protected override void  GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery_v()
		{
			QueryAdapter<TestItem> query = new QueryAdapter<TestItem>();
			query.Show("StoredString");
			ResultSet<TestItem> resultsOfQuery = RepositoryUnderTest.GetItemsMatching(query);
			Assert.AreEqual(1, resultsOfQuery.Count);
			Assert.AreEqual("Sonne", resultsOfQuery[0]["StoredString"]);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new Db4oRepository<TestItem>(_persistedFilePath);
		}
	}

	[TestFixture]
	public class Db4oRepositoryCreateItemTransitionTests: IRepositoryCreateItemTransitionTests<TestItem>
	{
		private string _name;

		[SetUp]
		public override void SetUp()
		{
			this._name = Path.GetTempFileName();
			CreateRepository();
		}

		private void CreateRepository()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._name);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}

		[Test]
		protected override void  GetItemsMatchingQuery_QueryWithShow_ReturnAllItemsMatchingQuery_v()
		{
			Item.StoredInt = 123;
			Item.StoredString = "I was stored!";
			QueryAdapter<TestItem> query = new QueryAdapter<TestItem>();
			query.Show("StoredInt").Show("StoredString");
			ResultSet<TestItem> resultsOfQuery = RepositoryUnderTest.GetItemsMatching(query);
			Assert.AreEqual(1, resultsOfQuery.Count);
			Assert.AreEqual(123, resultsOfQuery[0]["StoredInt"]);
			Assert.AreEqual("I was stored!", resultsOfQuery[0]["StoredString"]);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			CreateRepository();
		}
	}

	[TestFixture]
	public class Db4oRepositoryDeleteItemTransitionTests:
			IRepositoryDeleteItemTransitionTests<TestItem>
	{
		private string _name;

		[SetUp]
		public override void SetUp()
		{
			this._name = Path.GetTempFileName();
			CreateRepository();
		}

		private void CreateRepository()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._name);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			CreateRepository();
		}
	}

	[TestFixture]
	public class Db4oRepositoryDeleteIdTransitionTests: IRepositoryDeleteIdTransitionTests<TestItem>
	{
		private string _name;

		[SetUp]
		public override void SetUp()
		{
			this._name = Path.GetTempFileName();
			CreateRepository();
		}

		private void CreateRepository()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._name);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			CreateRepository();
		}
	}

	[TestFixture]
	public class Db4oRepositoryDeleteAllItemsTransitionTests:
			IRepositoryDeleteAllItemsTransitionTests<TestItem>
	{
		private string _name;

		[SetUp]
		public override void SetUp()
		{
			this._name = Path.GetTempFileName();
			CreateRepository();
		}

		private void CreateRepository()
		{
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._name);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_name);
		}

		protected override void RepopulateRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			CreateRepository();
		}
	}
}