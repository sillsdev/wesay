using System;
using NUnit.Framework;
using WeSay.Data;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class SynchronicRepositoryStateUnitializedTests : IRepositoryStateUnitializedTests<TestItem>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new SynchronicRepository<TestItem>(new MemoryRepository<TestItem>(), new MemoryRepository<TestItem>());
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
		}

	}

	[TestFixture]
	public class SynchronicRepositoryCreateItemTransitionTests : IRepositoryCreateItemTransitionTests<TestItem>
	{
		[SetUp]
		protected void Setup()
		{
			RepositoryUnderTest = new SynchronicRepository<TestItem>(new MemoryRepository<TestItem>(), new MemoryRepository<TestItem>());
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
	public class SynchronicRepositoryDeleteItemTransitionTests : IRepositoryDeleteItemTransitionTests<TestItem>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new SynchronicRepository<TestItem>(new MemoryRepository<TestItem>(), new MemoryRepository<TestItem>());
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
	public class SynchronicRepositoryDeleteIdTransitionTests : IRepositoryDeleteIdTransitionTests<TestItem>
	{
		[SetUp]
		public void Setup()
		{
			RepositoryUnderTest = new SynchronicRepository<TestItem>(new MemoryRepository<TestItem>(), new MemoryRepository<TestItem>());
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
		IRepository<TestItem> _primary;
		IRepository<TestItem> _secondary;
		SynchronicRepository<TestItem> _synchronic;

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
		[ExpectedException(typeof(ArgumentException))]
		public void PassSameRepositoriesInConstructor_ThrowsArgumentException()
		{
			IRepository<TestItem> repository = new MemoryRepository<TestItem>();
			new SynchronicRepository<TestItem>(repository, repository);
		}

		[Test]
		public void CreateItem_ItemExistsInBothRepositories()
		{
			TestItem item = _synchronic.CreateItem();
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
			Assert.AreEqual("changed", _primary.GetItem(_primary.GetAllItems()[0]).StoredString);
			Assert.AreEqual("changed", _secondary.GetItem(_secondary.GetAllItems()[0]).StoredString);
		}
	}
}
