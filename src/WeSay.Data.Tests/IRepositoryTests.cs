using System;
using System.Collections.Generic;
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
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItem_Null_Throws()
		{
			RepositoryUnderTest.DeleteItem((T) null);
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
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemById_Null_Throws()
		{
			RepositoryUnderTest.DeleteItem((RepositoryId) null);
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
		[ExpectedException(typeof(ArgumentNullException))]
		public void Save_Null_Throws()
		{
			RepositoryUnderTest.SaveItem(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Save_ItemDoesNotExist_Throws()
		{
			T item = new T();
			RepositoryUnderTest.SaveItem(item);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void GetItemsMatchingQuery_CanQueryIsFalse_Throws()
		{
			if(!RepositoryUnderTest.CanQuery)
			{
				RepositoryUnderTest.GetItemsMatching(query);
			}
			else
			{
				Assert.Ignore("Repository supports queries.");
			}
		}

		[Test]
		public void GetItemMatchingQuery_Query_ReturnsEmpty()
		{
			if(RepositoryUnderTest.CanQuery)
			{
				Assert.AreEqual(0, RepositoryUnderTest.GetItemsMatching(query).Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SaveItems_Null_Throws()
		{
			RepositoryUnderTest.SaveItems(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SaveItems_ItemDoesNotExist_Throws()
		{
			T item = new T();
			List <T> itemsToSave = new List<T>();
			itemsToSave.Add(item);
			RepositoryUnderTest.SaveItems(itemsToSave);
		}

		[Test]
		public void SaveItems_ListIsEmpty_DoNotChangeLastModified()
		{
			Assert.Fail("Problem with DateTime resolution.");
			List<T> itemsToSave = new List<T>();
			DateTime timePreSave = DateTime.UtcNow;
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.AreEqual(timePreSave, RepositoryUnderTest.LastModified);
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

	public abstract class IRepositoryCreateItemTransitionTests<T> where T : class, new()
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

		//This method is used to test whether data has been persisted.
		//This method should dispose of the current repository and reload it from persisted data
		//For repositories that don't support persistence this method should do nothing
		protected abstract void RepopulateRepositoryFromPersistedData();

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
		public void SaveItem_LastModifiedIsChangedToLaterTime()
		{
			Assert.Fail("Problem with DateTime resolution.");
			SetState();
			DateTime timePreSave = DateTime.UtcNow;
			RepositoryUnderTest.SaveItem(item);
			Assert.Greater((decimal) RepositoryUnderTest.LastModified.Ticks, timePreSave.Ticks);
		}

		[Test]
		public void SaveItem_LastModifiedIsSetInUTC()
		{
			SetState();
			Thread.Sleep(50);
			RepositoryUnderTest.SaveItem(item);
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public void SaveItem_ItemHasBeenPersisted()
		{
			Assert.Ignore(@"This Test is highly dependant on the type of objects that are
							being managed by the repository and as such should be tested elsewhere.");
		}

		[Test]
		public void GetItemMatchingQuery_Query_ReturnsItemsMatchingQuery()
		{
			SetState();
			if(RepositoryUnderTest.CanQuery)
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
		public void CreatedItemHasBeenPersisted()
		{
			SetState();
			if(!RepositoryUnderTest.CanPersist)
			{
			}
			else
			{
				RepopulateRepositoryFromPersistedData();
				T itemFromPersistedData = RepositoryUnderTest.GetItem(id);
				Assert.AreEqual(item, itemFromPersistedData);
			}
		}

		[Test]
		public void SaveItems_LastModifiedIsChangedToLaterTime()
		{
			Assert.Fail("Problem with DateTime resolution.");
			SetState();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(item);
			DateTime timePreSave = DateTime.UtcNow;
			Thread.Sleep(1000);
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.Greater((decimal)RepositoryUnderTest.LastModified.Ticks, timePreSave.Ticks);
		}

		[Test]
		public void SaveItems_LastModifiedIsSetInUTC()
		{
			SetState();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(item);
			Thread.Sleep(50);
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public void SaveItems_ItemHasBeenPersisted()
		{
			Assert.Ignore(@"This Test is highly dependant on the type of objects that are
							being managed by the repository and as such should be tested elsewhere.");
		}
	}

	public abstract class IRepositoryDeleteItemTransitionTests<T> where T : class, new()
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

		//This method is used to test whether data has been persisted.
		//This method should dispose of the current repository and reload it from persisted data
		//For repositories that don't support persistence this method should do nothing
		protected abstract void RepopulateRepositoryFromPersistedData();

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
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItem_HasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist())
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				RepopulateRepositoryFromPersistedData();
				RepositoryUnderTest.GetItem(id);
			}
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
		public void LastModified_IsChangedToLaterTime()
		{
			Assert.Fail("Problem with DateTime resolution.");
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
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				throw new NotImplementedException("You must implement a test to verify that the item deletion has been persisted.");
			}
		}

		[Test]
		public void GetItemMatchingQuery_Query_ReturnsEmpty()
		{
			if (RepositoryUnderTest.CanQuery)
			{
				Assert.AreEqual(0, RepositoryUnderTest.GetItemsMatching(query).Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}
	}

	public abstract class IRepositoryDeleteIdTransitionTests<T> where T : class, new()
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

		//This method is used to test whether data has been persisted.
		//This method should dispose of the current repository and reload it from persisted data
		//For repositories that don't support persistence this method should do nothing
		protected abstract void RepopulateRepositoryFromPersistedData();

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
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItem_HasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist())
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				RepopulateRepositoryFromPersistedData();
				RepositoryUnderTest.GetItem(id);
			}
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
		public void LastModified_ItemIsDeleted_IsChangedToLaterTime()
		{
			Assert.Fail("Problem with DateTime resolution.");
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
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				throw new NotImplementedException("You must implement a test to verify that the item deletion has been persisted.");
			}
		}

		[Test]
		public void GetItemMatchingQuery_Query_ReturnsEmpty()
		{
			if (RepositoryUnderTest.CanQuery)
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
