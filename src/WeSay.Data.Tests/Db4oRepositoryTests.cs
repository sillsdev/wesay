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
		public void Setup()
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

		[SetUp]
		public void Setup()
		{
			this._persistedFilePath = Path.GetTempFileName();
			RepositoryUnderTest = new Db4oRepository<TestItem>(this._persistedFilePath);
			TestItem item = RepositoryUnderTest.CreateItem();
			item.StoredInt = 5;
			item.StoredString = "Sonne";
			RepositoryUnderTest.SaveItem(item);

			CreateNewRepositoryFromPersistedData();
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(this._persistedFilePath);
		}

		[Test]
		public override void SaveItem_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			DateTime modifiedTimePreSave = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItem(Item);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreSave);
		}

		[Test]
		public override void SaveItems_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			List<TestItem> itemsToSave = new List<TestItem>();
			itemsToSave.Add(Item);
			DateTime modifiedTimePreSave = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreSave);
		}

		[Test]
		public override void LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime() {}

		[Test]
		public override void
				GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery()
		{
			SetState();
			Query query = new Query(typeof (TestItem)).Show("StoredString");
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
	public class Db4oRepositoryCreateItemTransitionTests:
			IRepositoryCreateItemTransitionTests<TestItem>
	{
		private string _name;

		[SetUp]
		public void Setup()
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
		public override void
				GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery()
		{
			SetState();
			Item.StoredInt = 123;
			Item.StoredString = "I was stored!";
			Query query = new Query(typeof (TestItem)).Show("StoredInt").Show("StoredString");
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
		public void Setup()
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
		public void Setup()
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
		public void Setup()
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