using System;
using System.Threading;
using NUnit.Framework;
using WeSay.Data;

namespace WeSay.Data.Tests
{
	public class IRepositoryStateUnitializedTests<T> where T: class, new()
	{
		private IRepository<T> _repositoryUnderTest;
		private readonly Query query = new Query(typeof(T));

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if(_repositoryUnderTest == null)
				{
					throw new InvalidOperationException("RepositoryUnderTest must be set before the tests are run.");
				}
				return _repositoryUnderTest;
			}
			set { _repositoryUnderTest = value; }
		}

		[Test]
		public void CreateItem_NotNull()
		{
			Assert.IsNotNull(RepositoryUnderTest.CreateItem());
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItem_ItemDoesNotExist_Throws()
		{
			T item = new T();
			RepositoryUnderTest.DeleteItem(item);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItemById_ItemDoesNotExist_Throws()
		{
			MyRepositoryId id = new MyRepositoryId();
			RepositoryUnderTest.DeleteItem(id);
		}

		[Test]
		public void CountAllItems_NoItemsInTheRepostory_ReturnsZero()
		{
			Assert.AreEqual(0, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		//IsThisNeeded?
		public void GetId_Item_ReturnsTypeRepositoryId()
		{
			T item = RepositoryUnderTest.CreateItem();
			Assert.IsInstanceOfType(typeof(RepositoryId), RepositoryUnderTest.GetId(item));
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetId_ItemNotInRepository_Throws()
		{
			T item = new T();
			RepositoryUnderTest.GetId(item);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetItem_IdNotInRepository_Throws()
		{
			MyRepositoryId id = new MyRepositoryId();
			RepositoryUnderTest.GetItem(id);
		}

		[Test]
		public void LastModified_ReturnsMinimumPossibleTime()
		{
			Assert.AreEqual(DateTime.MinValue, RepositoryUnderTest.LastModified);
		}

		[Test]
		public void GetAllItems_ReturnsEmptyArray()
		{
			Assert.IsEmpty(RepositoryUnderTest.GetAllItems());
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Save_Throws()
		{
			T item = new T();
			RepositoryUnderTest.SaveItem(item);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void GetItemsMatchingQuery_CanQueryIsFalse_Throws()
		{
			if(!RepositoryUnderTest.CanQuery())
			{
				RepositoryUnderTest.GetItemsMatching(query);
			}
			else
			{
				Assert.Ignore("This repository supports queries.");
			}
		}

		[Test]
		public virtual void GetItemMatchingQuery_Query_ReturnsEmpty()
		{
			if(RepositoryUnderTest.CanQuery())
			{
				Assert.AreEqual(0, RepositoryUnderTest.GetItemsMatching(query).Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		class MyRepositoryId : RepositoryId
		{
			public override int CompareTo(RepositoryId other)
			{
				return 0;
			}

			public override bool Equals(RepositoryId other)
			{
				return true;
			}
		}
	}

	public class IRepositoryCreateItemTransitionTests<T> where T : class, new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;
		private readonly Query query = new Query(typeof(T));

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if (_repositoryUnderTest == null)
				{
					throw new InvalidOperationException("RepositoryUnderTest must be set before the tests are run.");
				}
				return _repositoryUnderTest;
			}
			set { _repositoryUnderTest = value; }
		}

		public void SetState()
		{
			item = RepositoryUnderTest.CreateItem();
			id = RepositoryUnderTest.GetId(item);
		}

		[Test]
		public void CountAllItems_ReturnsNumberOfItemsInRepository()
		{
			SetState();
			Assert.AreEqual(1, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		public void GetItem_Id_ReturnsItemWithId()
		{
			SetState();
			Assert.AreSame(item, RepositoryUnderTest.GetItem(id));
		}

		[Test]
		public void GetItem_CalledTwiceWithSameId_ReturnsSameItem()
		{
			SetState();
			Assert.AreSame(RepositoryUnderTest.GetItem(id), RepositoryUnderTest.GetItem(id));
		}

		[Test]
		//Same as test above
		public void GetId_Item_ReturnsIdOfItem()
		{
			SetState();
			Assert.AreSame(item, RepositoryUnderTest.GetItem(id));
		}

		[Test]
		public void GetId_CalledTwiceWithSameItem_ReturnsSameId()
		{
			SetState();
			Assert.AreEqual(RepositoryUnderTest.GetId(item), RepositoryUnderTest.GetId(item));
		}

		[Test]
		public void LastModified_IsChanged()
		{
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			SetState();

			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}


		[Test]
		public void LastModified_IsSetInUTC()
		{
			SetState();
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public void GetAllItems_ReturnsIdsOfExistingItems()
		{
			SetState();
			Assert.AreEqual(RepositoryUnderTest.GetId(item),RepositoryUnderTest.GetAllItems()[0]);
		}

		[Test]
		public void GetAllItems_ReturnsCorrectNumberOfExistingItems()
		{
			SetState();
			Assert.AreEqual(1, RepositoryUnderTest.GetAllItems().Length);
		}

		[Test]
		public void LastModified_SaveItem_ReturnsCorrectTime()
		{
			SetState();
			Thread.Sleep(50);
			DateTime timeOfSave = DateTime.UtcNow;
			RepositoryUnderTest.SaveItem(item);
			Assert.AreEqual(timeOfSave, RepositoryUnderTest.LastModified);
		}

		[Test]
		public void GetItemMatchingQuery_Query_ReturnsItemsMatchingQuery()
		{
			SetState();
			if(RepositoryUnderTest.CanQuery())
			{
				ResultSet<T> resultSet = RepositoryUnderTest.GetItemsMatching(query);
				Assert.AreEqual(1, resultSet.Count);
				Assert.AreEqual(item, resultSet[0].RealObject);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		public virtual void ItemHasBeenPersisted()
		{
			SetState();
			if(!RepositoryUnderTest.CanPersist())
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				throw new NotImplementedException("You must implement a test to verify that the created item has been persisted.");
			}
		}
	}

	public class IRepositoryDeleteItemTransitionTests<T> where T : class, new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;
		private readonly Query query = new Query(typeof(T));

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if (_repositoryUnderTest == null)
				{
					throw new InvalidOperationException("RepositoryUnderTest must be set before the tests are run.");
				}
				return _repositoryUnderTest;
			}
			set { _repositoryUnderTest = value; }
		}

		public void SetState()
		{
			CreateInitialItem();
			WaitThenDeleteItem();
		}

		private void WaitThenDeleteItem() {
			Thread.Sleep(50);
			RepositoryUnderTest.DeleteItem(this.item);
		}

		private void CreateInitialItem() {
			this.item = RepositoryUnderTest.CreateItem();
			this.id = RepositoryUnderTest.GetId(this.item);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItem_ItemDoesNotExist_Throws()
		{
			SetState();
			RepositoryUnderTest.DeleteItem(item);
		}

		[Test]
		public void CountAllItems_ReturnsNumberOfItemsInRepository()
		{
			SetState();
			Assert.AreEqual(0, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetId_DeletedItemWithId_Throws()
		{
			SetState();
			RepositoryUnderTest.GetId(item);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetItem_DeletedItem_Throws()
		{
			SetState();
			RepositoryUnderTest.GetItem(id);
		}

		[Test]
		public void LastModified_IsChanged()
		{
			CreateInitialItem();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			WaitThenDeleteItem();
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}

		[Test]
		public void LastModified_IsSetInUTC()
		{
			SetState();
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public void GetAllItems_ReturnsIdsOfExistingItems()
		{
			SetState();
			Assert.IsEmpty(RepositoryUnderTest.GetAllItems());
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Save_Throws()
		{
			SetState();
			RepositoryUnderTest.SaveItem(item);
		}

		[Test]
		public virtual void DeletionOfItemHasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist())
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				throw new NotImplementedException("You must implement a test to verify that the item deletion has been persisted.");
			}
		}

		[Test]
		public virtual void GetItemMatchingQuery_Query_ReturnsEmpty()
		{
			if (RepositoryUnderTest.CanQuery())
			{
				Assert.AreEqual(0, RepositoryUnderTest.GetItemsMatching(query).Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}
	}

	public class IRepositoryDeleteIdTransitionTests<T> where T : class, new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;
		private readonly Query query = new Query(typeof(T));

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if (_repositoryUnderTest == null)
				{
					throw new InvalidOperationException("RepositoryUnderTest must be set before the tests are run.");
				}
				return _repositoryUnderTest;
			}
			set { _repositoryUnderTest = value; }
		}

		public void SetState()
		{
			CreateItemToTest();
			WaitThenDeleteItem();
		}

		private void WaitThenDeleteItem() {
			Thread.Sleep(50);
			RepositoryUnderTest.DeleteItem(this.id);
		}

		private void CreateItemToTest() {
			this.item = RepositoryUnderTest.CreateItem();
			this.id = RepositoryUnderTest.GetId(this.item);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItem_ItemDoesNotExist_Throws()
		{
			SetState();
			RepositoryUnderTest.DeleteItem(id);
		}

		[Test]
		public void CountAllItems_DeleteItem_ReturnsNumberOfItemsInRepository()
		{
			SetState();
			Assert.AreEqual(0, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetId_DeletedItemWithId_Throws()
		{
			SetState();
			RepositoryUnderTest.GetId(item);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetItem_DeletedItem_Throws()
		{
			SetState();
			RepositoryUnderTest.GetItem(id);
		}

		[Test]
		public void LastModified_ItemIsDeleted_IsChanged()
		{
			CreateItemToTest();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			WaitThenDeleteItem();
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}

		[Test]
		public void LastModified_ItemIsDeleted_IsSetInUTC()
		{
			SetState();
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public void GetAllItems_ReturnsIdsOfExistingItems()
		{
			SetState();
			Assert.IsEmpty(RepositoryUnderTest.GetAllItems());
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Save_Throws()
		{
			SetState();
			RepositoryUnderTest.SaveItem(item);
		}

		[Test]
		public virtual void DeletionOfItemHasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist())
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				throw new NotImplementedException("You must implement a test to verify that the item deletion has been persisted.");
			}
		}

		[Test]
		public virtual void GetItemMatchingQuery_Query_ReturnsEmpty()
		{
			if (RepositoryUnderTest.CanQuery())
			{
				Assert.AreEqual(0, RepositoryUnderTest.GetItemsMatching(query).Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}
	}
}
