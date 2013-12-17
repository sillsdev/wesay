using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
    [TestFixture]
	public class SynchronicRepositoryStateUnitializedTests:
			IRepositoryStateUnitializedTests<TestItem>
    {
        [SetUp]
		public override void SetUp()
        {
			RepositoryUnderTest =
					new SynchronicRepository<TestItem>(new MemoryRepository<TestItem>(),
													   new MemoryRepository<TestItem>());
        }

        [TearDown]
		public void Teardown()
        {
            RepositoryUnderTest.Dispose();
        }
    }

    [TestFixture]
	public class SynchronicRepositoryCreateItemTransitionTests:
			IRepositoryCreateItemTransitionTests<TestItem>
    {
        [SetUp]
		public override void SetUp()
        {
			RepositoryUnderTest =
					new SynchronicRepository<TestItem>(new MemoryRepository<TestItem>(),
													   new MemoryRepository<TestItem>());
        }

        [TearDown]
		public void Teardown()
        {
            RepositoryUnderTest.Dispose();
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
            //Do nothing.
        }
    }

    [TestFixture]
	public class SynchronicRepositoryDeleteItemTransitionTests:
			IRepositoryDeleteItemTransitionTests<TestItem>
    {
        [SetUp]
		public override void SetUp()
        {
			RepositoryUnderTest =
					new SynchronicRepository<TestItem>(new MemoryRepository<TestItem>(),
													   new MemoryRepository<TestItem>());
        }

        [TearDown]
		public void Teardown()
        {
            RepositoryUnderTest.Dispose();
        }

		protected override void CreateNewRepositoryFromPersistedData()
        {
            //Do nothing.
        }
    }

    [TestFixture]
	public class SynchronicRepositoryDeleteIdTransitionTests:
			IRepositoryDeleteIdTransitionTests<TestItem>
    {
        [SetUp]
		public override void SetUp()
        {
			RepositoryUnderTest =
					new SynchronicRepository<TestItem>(new MemoryRepository<TestItem>(),
													   new MemoryRepository<TestItem>());
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			//Do nothing.
		}
	}

	[TestFixture]
	public class SynchronicRepositoryDeleteAllItemsTransitionTests:
			IRepositoryDeleteAllItemsTransitionTests<TestItem>
	{
		[SetUp]
		public override void SetUp()
		{
			RepositoryUnderTest =
					new SynchronicRepository<TestItem>(new MemoryRepository<TestItem>(),
													   new MemoryRepository<TestItem>());
        }

        [TearDown]
		public void Teardown()
        {
            RepositoryUnderTest.Dispose();
        }

        protected override void RepopulateRepositoryFromPersistedData()
        {
            //Do nothing.
        }
    }

    [TestFixture]
    public class SynchronicRepositoryTests
    {
		private IRepository<TestItem> _primary;
		private IRepository<TestItem> _secondary;
		private SynchronicRepository<TestItem> _synchronic;

        [SetUp]
        public void Setup()
        {
            _primary = new MemoryRepository<TestItem>();
            _secondary = new MemoryRepository<TestItem>();
            _synchronic = new SynchronicRepository<TestItem>(_primary, _secondary);
        }

        [TearDown]
        public void Teardown()
        {
            _synchronic.Dispose();
        }

        [Test]
		[ExpectedException(typeof (ArgumentException))]
        public void PassSameRepositoriesInConstructor_ThrowsArgumentException()
        {
			using (IRepository<TestItem> repository = new MemoryRepository<TestItem>())
			{
				using (new SynchronicRepository<TestItem>(repository, repository)) {}
			}
        }

        [Test]
        public void CreateItem_ItemExistsInBothRepositories()
        {
            TestItem item = _synchronic.CreateItem();
            _synchronic.SaveItem(item);
            Assert.AreEqual(item, _primary.GetItem(_primary.GetAllItems()[0]));
            Assert.AreEqual(item, _secondary.GetItem(_secondary.GetAllItems()[0]));
            Assert.AreEqual(1, _primary.CountAllItems());
            Assert.AreEqual(1, _secondary.CountAllItems());
        }

        [Test]
        public void DeleteItemByItem_ItemDeletedInBothRepositories()
        {
            TestItem item = _synchronic.CreateItem();
            _synchronic.DeleteItem(item);
            Assert.AreEqual(0, _primary.CountAllItems());
            Assert.AreEqual(0, _secondary.CountAllItems());
        }

        [Test]
        public void DeleteItemById_ItemDeletedInBothRepositories()
        {
            TestItem item = _synchronic.CreateItem();
            _synchronic.DeleteItem(_synchronic.GetId(item));
            Assert.AreEqual(0, _primary.CountAllItems());
            Assert.AreEqual(0, _secondary.CountAllItems());
        }

        [Test]
        public void ChangeItem_ItemChangedInBothRepositories()
        {
            TestItem item = _synchronic.CreateItem();
            item.StoredString = "changed";
            _synchronic.SaveItem(item);
            Assert.AreEqual("changed", _primary.GetItem(_primary.GetAllItems()[0]).StoredString);
            Assert.AreEqual("changed", _secondary.GetItem(_secondary.GetAllItems()[0]).StoredString);
        }

        [Test]
        public void StartWithItemsInSecondary_ItemsCopiedToPrimary()
        {
            TestItem item = _secondary.CreateItem();
            item.StoredString = "item one";
            item = _secondary.CreateItem();
            item.StoredString = "item two";
			_synchronic.Dispose();
            _synchronic = new SynchronicRepository<TestItem>(_primary, _secondary);
            Assert.AreEqual(2, _primary.CountAllItems());
            List<string> strings = new List<string>(2);
            strings.Add(_primary.GetItem(_primary.GetAllItems()[0]).StoredString);
            strings.Add(_primary.GetItem(_primary.GetAllItems()[1]).StoredString);
            Assert.Contains("item one", strings);
            Assert.Contains("item two", strings);
        }

        [Test]
        public void StartWithItemsInPrimary_ItemsCopiedToSecondary()
        {
            TestItem item = _primary.CreateItem();
            item.StoredString = "item one";
			_primary.SaveItem(item);
            item = _primary.CreateItem();
            item.StoredString = "item two";
			_primary.SaveItem(item);
			_synchronic.Dispose();
            _synchronic = new SynchronicRepository<TestItem>(_primary, _secondary);
            Assert.AreEqual(2, _secondary.CountAllItems());
            List<string> strings = new List<string>(2);
            strings.Add(_secondary.GetItem(_secondary.GetAllItems()[0]).StoredString);
            strings.Add(_secondary.GetItem(_secondary.GetAllItems()[1]).StoredString);
            Assert.Contains("item one", strings);
            Assert.Contains("item two", strings);
        }

        [Test]
        public void StartWithItemsInBothButSecondaryNewer_NewestOneWins()
        {
            TestItem item = _primary.CreateItem();
            item.StoredString = "item one";
            _primary.SaveItem(item);
            item = _secondary.CreateItem();
            item.StoredString = "item two";
            _secondary.SaveItem(item);
			_synchronic.Dispose();
            _synchronic = new SynchronicRepository<TestItem>(_primary, _secondary);
            Assert.AreEqual(1, _primary.CountAllItems());
            Assert.AreEqual(1, _secondary.CountAllItems());
            Assert.AreEqual("item two", _primary.GetItem(_primary.GetAllItems()[0]).StoredString);
            Assert.AreEqual("item two", _secondary.GetItem(_secondary.GetAllItems()[0]).StoredString);
        }

        [Test]
        public void StartWithItemsInBothButPrimaryNewer_NewestOneWins()
        {
            TestItem item = _secondary.CreateItem();
            item.StoredString = "item one";
            _secondary.SaveItem(item);
            item = _primary.CreateItem();
            item.StoredString = "item two";
            _primary.SaveItem(item);
			_synchronic.Dispose();
            _synchronic = new SynchronicRepository<TestItem>(_primary, _secondary);
            Assert.AreEqual(1, _primary.CountAllItems());
            Assert.AreEqual(1, _secondary.CountAllItems());
            Assert.AreEqual("item two", _primary.GetItem(_primary.GetAllItems()[0]).StoredString);
            Assert.AreEqual("item two", _secondary.GetItem(_secondary.GetAllItems()[0]).StoredString);
        }
    }
}