using System;
using System.Collections.Generic;
using NUnit.Framework;
using Palaso.Data;
using Palaso.Tests.Data;

namespace WeSay.Data.Tests
{
    [TestFixture]
	public class SynchronicRepositoryStateUnitializedTests :
		IRepositoryStateUnitializedTests<PalasoTestItem>
    {
        [SetUp]
		public override void SetUp()
        {
			DataMapperUnderTest =
					new SynchronicRepository<PalasoTestItem>(new MemoryDataMapper<PalasoTestItem>(),
													   new MemoryDataMapper<PalasoTestItem>());
        }

        [TearDown]
		public override void TearDown()
        {
			DataMapperUnderTest.Dispose();
        }
    }

    [TestFixture]
	public class SynchronicRepositoryCreateItemTransitionTests:
			IRepositoryCreateItemTransitionTests<PalasoTestItem>
    {
        [SetUp]
		public override void SetUp()
        {
			DataMapperUnderTest =
					new SynchronicRepository<PalasoTestItem>(new MemoryDataMapper<PalasoTestItem>(),
													   new MemoryDataMapper<PalasoTestItem>());
        }

        [TearDown]
		public override void TearDown()
        {
			DataMapperUnderTest.Dispose();
        }

		/* todo cp move to query tests
		[Test]
		protected override void GetItemsMatchingQuery_QueryWithShow_ReturnAllItemsMatchingQuery_v()
		{
			Item.StoredInt = 123;
			Item.StoredString = "I was stored!";
			QueryAdapter<PalasoTestItem> query = new QueryAdapter<PalasoTestItem>();
			query.Show("StoredInt").Show("StoredString");
			ResultSet<PalasoTestItem> resultsOfQuery = DataMapperUnderTest.GetItemsMatching(query);
			Assert.AreEqual(1, resultsOfQuery.Count);
			Assert.AreEqual(123, resultsOfQuery[0]["StoredInt"]);
			Assert.AreEqual("I was stored!", resultsOfQuery[0]["StoredString"]);
		}
		*/
		protected override void CreateNewRepositoryFromPersistedData()
        {
            //Do nothing.
        }
    }

    [TestFixture]
	public class SynchronicRepositoryDeleteItemTransitionTests:
			IRepositoryDeleteItemTransitionTests<PalasoTestItem>
    {
        [SetUp]
		public override void SetUp()
        {
			DataMapperUnderTest =
					new SynchronicRepository<PalasoTestItem>(new MemoryDataMapper<PalasoTestItem>(),
													   new MemoryDataMapper<PalasoTestItem>());
        }

        [TearDown]
		public override void TearDown()
        {
			DataMapperUnderTest.Dispose();
        }

		protected override void CreateNewRepositoryFromPersistedData()
        {
            //Do nothing.
        }
    }

    [TestFixture]
	public class SynchronicRepositoryDeleteIdTransitionTests:
			IRepositoryDeleteIdTransitionTests<PalasoTestItem>
    {
        [SetUp]
		public override void SetUp()
        {
			DataMapperUnderTest =
					new SynchronicRepository<PalasoTestItem>(new MemoryDataMapper<PalasoTestItem>(),
													   new MemoryDataMapper<PalasoTestItem>());
		}

		[TearDown]
		public override void TearDown()
		{
			DataMapperUnderTest.Dispose();
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			//Do nothing.
		}
	}

	[TestFixture]
	public class SynchronicRepositoryDeleteAllItemsTransitionTests:
			IRepositoryDeleteAllItemsTransitionTests<PalasoTestItem>
	{
		[SetUp]
		public override void SetUp()
		{
			DataMapperUnderTest =
					new SynchronicRepository<PalasoTestItem>(new MemoryDataMapper<PalasoTestItem>(),
													   new MemoryDataMapper<PalasoTestItem>());
        }

        [TearDown]
		public override void TearDown()
        {
			DataMapperUnderTest.Dispose();
        }

        protected override void RepopulateRepositoryFromPersistedData()
        {
            //Do nothing.
        }
    }

    [TestFixture]
    public class SynchronicRepositoryTests
    {
		private IDataMapper<PalasoTestItem> _primary;
		private IDataMapper<PalasoTestItem> _secondary;
		private SynchronicRepository<PalasoTestItem> _synchronic;

        [SetUp]
        public void Setup()
        {
			_primary = new MemoryDataMapper<PalasoTestItem>();
			_secondary = new MemoryDataMapper<PalasoTestItem>();
			_synchronic = new SynchronicRepository<PalasoTestItem>(_primary, _secondary);
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
			using (IDataMapper<PalasoTestItem> dataMapper = new MemoryDataMapper<PalasoTestItem>())
			{
				using (new SynchronicRepository<PalasoTestItem>(dataMapper, dataMapper)) {}
			}
        }

        [Test]
        public void CreateItem_ItemExistsInBothRepositories()
        {
			PalasoTestItem item = _synchronic.CreateItem();
            _synchronic.SaveItem(item);
            Assert.AreEqual(item, _primary.GetItem(_primary.GetAllItems()[0]));
            Assert.AreEqual(item, _secondary.GetItem(_secondary.GetAllItems()[0]));
            Assert.AreEqual(1, _primary.CountAllItems());
            Assert.AreEqual(1, _secondary.CountAllItems());
        }

        [Test]
        public void DeleteItemByItem_ItemDeletedInBothRepositories()
        {
			PalasoTestItem item = _synchronic.CreateItem();
            _synchronic.DeleteItem(item);
            Assert.AreEqual(0, _primary.CountAllItems());
            Assert.AreEqual(0, _secondary.CountAllItems());
        }

        [Test]
        public void DeleteItemById_ItemDeletedInBothRepositories()
        {
			PalasoTestItem item = _synchronic.CreateItem();
            _synchronic.DeleteItem(_synchronic.GetId(item));
            Assert.AreEqual(0, _primary.CountAllItems());
            Assert.AreEqual(0, _secondary.CountAllItems());
        }

        [Test]
        public void ChangeItem_ItemChangedInBothRepositories()
        {
			PalasoTestItem item = _synchronic.CreateItem();
            item.StoredString = "changed";
            _synchronic.SaveItem(item);
            Assert.AreEqual("changed", _primary.GetItem(_primary.GetAllItems()[0]).StoredString);
            Assert.AreEqual("changed", _secondary.GetItem(_secondary.GetAllItems()[0]).StoredString);
        }

        [Test]
        public void StartWithItemsInSecondary_ItemsCopiedToPrimary()
        {
			PalasoTestItem item = _secondary.CreateItem();
            item.StoredString = "item one";
            item = _secondary.CreateItem();
            item.StoredString = "item two";
			_synchronic.Dispose();
			_synchronic = new SynchronicRepository<PalasoTestItem>(_primary, _secondary);
            Assert.AreEqual(2, _primary.CountAllItems());
			var strings = new List<string>(2)
			{
				_primary.GetItem(_primary.GetAllItems()[0]).StoredString,
				_primary.GetItem(_primary.GetAllItems()[1]).StoredString
			};
            Assert.Contains("item one", strings);
            Assert.Contains("item two", strings);
        }

        [Test]
        public void StartWithItemsInPrimary_ItemsCopiedToSecondary()
        {
			PalasoTestItem item = _primary.CreateItem();
            item.StoredString = "item one";
			_primary.SaveItem(item);
            item = _primary.CreateItem();
            item.StoredString = "item two";
			_primary.SaveItem(item);
			_synchronic.Dispose();
			_synchronic = new SynchronicRepository<PalasoTestItem>(_primary, _secondary);
            Assert.AreEqual(2, _secondary.CountAllItems());
			var strings = new List<string>(2)
			{
				_secondary.GetItem(_secondary.GetAllItems()[0]).StoredString,
				_secondary.GetItem(_secondary.GetAllItems()[1]).StoredString
			};
            Assert.Contains("item one", strings);
            Assert.Contains("item two", strings);
        }

        [Test]
        public void StartWithItemsInBothButSecondaryNewer_NewestOneWins()
        {
			PalasoTestItem item = _primary.CreateItem();
            item.StoredString = "item one";
            _primary.SaveItem(item);
            item = _secondary.CreateItem();
            item.StoredString = "item two";
            _secondary.SaveItem(item);
			_synchronic.Dispose();
			_synchronic = new SynchronicRepository<PalasoTestItem>(_primary, _secondary);
            Assert.AreEqual(1, _primary.CountAllItems());
            Assert.AreEqual(1, _secondary.CountAllItems());
            Assert.AreEqual("item two", _primary.GetItem(_primary.GetAllItems()[0]).StoredString);
            Assert.AreEqual("item two", _secondary.GetItem(_secondary.GetAllItems()[0]).StoredString);
        }

        [Test]
        public void StartWithItemsInBothButPrimaryNewer_NewestOneWins()
        {
			PalasoTestItem item = _secondary.CreateItem();
            item.StoredString = "item one";
            _secondary.SaveItem(item);
            item = _primary.CreateItem();
            item.StoredString = "item two";
            _primary.SaveItem(item);
			_synchronic.Dispose();
			_synchronic = new SynchronicRepository<PalasoTestItem>(_primary, _secondary);
            Assert.AreEqual(1, _primary.CountAllItems());
            Assert.AreEqual(1, _secondary.CountAllItems());
            Assert.AreEqual("item two", _primary.GetItem(_primary.GetAllItems()[0]).StoredString);
            Assert.AreEqual("item two", _secondary.GetItem(_secondary.GetAllItems()[0]).StoredString);
        }
    }
}