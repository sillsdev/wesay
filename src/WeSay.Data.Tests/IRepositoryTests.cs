using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	public class IRepositoryStateUnitializedTests<T> where T : class, new()
	{
		private IRepository<T> _repositoryUnderTest;
		private readonly Query query = new Query(typeof (T));

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if (_repositoryUnderTest == null)
				{
					throw new InvalidOperationException(
							"RepositoryUnderTest must be set before the tests are run.");
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
		[ExpectedException(typeof (ArgumentNullException))]
		public void DeleteItem_Null_Throws()
		{
			RepositoryUnderTest.DeleteItem((T) null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void DeleteItem_ItemDoesNotExist_Throws()
		{
			T item = new T();
			RepositoryUnderTest.DeleteItem(item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void DeleteItemById_Null_Throws()
		{
			RepositoryUnderTest.DeleteItem((RepositoryId) null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void DeleteItemById_ItemDoesNotExist_Throws()
		{
			MyRepositoryId id = new MyRepositoryId();
			RepositoryUnderTest.DeleteItem(id);
		}

		[Test]
		public void DeleteAllItems_NothingInRepository_StillNothingInRepository()
		{
			RepositoryUnderTest.DeleteAllItems();
			Assert.AreEqual(0, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		public void CountAllItems_NoItemsInTheRepostory_ReturnsZero()
		{
			Assert.AreEqual(0, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		public void GetAllItems_ReturnsEmptyArray()
		{
			Assert.IsEmpty(RepositoryUnderTest.GetAllItems());
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void GetId_ItemNotInRepository_Throws()
		{
			T item = new T();
			RepositoryUnderTest.GetId(item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void GetItem_IdNotInRepository_Throws()
		{
			MyRepositoryId id = new MyRepositoryId();
			RepositoryUnderTest.GetItem(id);
		}

		[Test]
		[ExpectedException(typeof (NotSupportedException))]
		public void GetItemsMatchingQuery_CanQueryIsFalse_Throws()
		{
			if (!RepositoryUnderTest.CanQuery)
			{
				RepositoryUnderTest.GetItemsMatching(query);
			}
			else
			{
				Assert.Ignore("Test not relevant. This repository supports queries.");
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

		[Test]
		public void LastModified_ReturnsMinimumPossibleTime()
		{
			Assert.AreEqual(RepositoryUnderTest.LastModified, DateTime.MinValue);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Save_Null_Throws()
		{
			RepositoryUnderTest.SaveItem(null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Save_ItemDoesNotExist_Throws()
		{
			T item = new T();
			RepositoryUnderTest.SaveItem(item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SaveItems_Null_Throws()
		{
			RepositoryUnderTest.SaveItems(null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SaveItems_ItemDoesNotExist_Throws()
		{
			T item = new T();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(item);
			RepositoryUnderTest.SaveItems(itemsToSave);
		}

		[Test]
		public void SaveItems_ListIsEmpty_DoNotChangeLastModified()
		{
			List<T> itemsToSave = new List<T>();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.AreEqual(modifiedTimePreTestedStateSwitch, RepositoryUnderTest.LastModified);
		}

		private class MyRepositoryId: RepositoryId
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

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if (_repositoryUnderTest == null)
				{
					throw new InvalidOperationException(
							"RepositoryUnderTest must be set before the tests are run.");
				}
				return _repositoryUnderTest;
			}
			set { _repositoryUnderTest = value; }
		}

		protected T Item
		{
			get { return item; }
			set { item = value; }
		}

		protected RepositoryId Id
		{
			get { return id; }
			set { id = value; }
		}

		public void SetState()
		{
			Item = RepositoryUnderTest.CreateItem();
			Id = RepositoryUnderTest.GetId(Item);
		}

		//This method is used to test whether data has been persisted.
		//This method should dispose of the current repository and reload it from persisted data
		//For repositories that don't support persistence this method should do nothing
		protected abstract void CreateNewRepositoryFromPersistedData();

		[Test]
		public void CreateItem_ReturnsUniqueItem()
		{
			SetState();
			Assert.AreNotEqual(Item, RepositoryUnderTest.CreateItem());
		}

		[Test]
		public void CreatedItemHasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist) {}
			else
			{
				CreateNewRepositoryFromPersistedData();
				RepositoryId[] listOfItems = RepositoryUnderTest.GetAllItems();
				Assert.AreEqual(1, listOfItems.Length);
				//Would be nice if this worked.. but it doesn't because we have equals for LexEntry is still by reference
				//T itemFromPersistedData = RepositoryUnderTest.GetItem(listOfItems[0]);
				//Assert.AreEqual(item, itemFromPersistedData);
			}
		}

		[Test]
		public void CountAllItems_ReturnsOne()
		{
			SetState();
			Assert.AreEqual(1, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		public void GetAllItems_ReturnsIdItem()
		{
			SetState();
			Assert.AreEqual(RepositoryUnderTest.GetId(Item), RepositoryUnderTest.GetAllItems()[0]);
		}

		[Test]
		public void GetAllItems_ReturnsCorrectNumberOfExistingItems()
		{
			SetState();
			Assert.AreEqual(1, RepositoryUnderTest.GetAllItems().Length);
		}

		[Test]
		public void GetId_CalledTwiceWithSameItem_ReturnsSameId()
		{
			SetState();
			Assert.AreEqual(RepositoryUnderTest.GetId(Item), RepositoryUnderTest.GetId(Item));
		}

		[Test]
		public void GetId_Item_ReturnsIdOfItem()
		{
			SetState();
			Assert.AreEqual(Id, RepositoryUnderTest.GetId(Item));
		}

		[Test]
		public void GetItem_Id_ReturnsItemWithId()
		{
			SetState();
			Assert.AreSame(Item, RepositoryUnderTest.GetItem(Id));
		}

		[Test]
		public void GetItem_CalledTwiceWithSameId_ReturnsSameItem()
		{
			SetState();
			Assert.AreSame(RepositoryUnderTest.GetItem(Id), RepositoryUnderTest.GetItem(Id));
		}

		[Test]
		public void GetItemsMatchingQuery_QueryWithOutShow_ReturnsNoItems()
		{
			Query queryWithoutShow = new Query(typeof (T));
			SetState();
			if (RepositoryUnderTest.CanQuery)
			{
				ResultSet<T> resultSet = RepositoryUnderTest.GetItemsMatching(queryWithoutShow);
				Assert.AreEqual(0, resultSet.Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		public void GetItemsMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery()
		{
			SetState();
			GetItemsMatchingQuery_QueryWithShow_ReturnAllItemsMatchingQuery_v();
		}

		protected virtual void GetItemsMatchingQuery_QueryWithShow_ReturnAllItemsMatchingQuery_v()
		{
			if (RepositoryUnderTest.CanQuery)
			{
				Assert.Fail(
					@"This Test is highly dependant on the type of objects that are
							being managed by the repository and as such should be overridden.");
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		public void SaveItem_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItem(Item);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}

		[Test]
		public void SaveItem_LastModifiedIsSetInUTC()
		{
			SetState();
			RepositoryUnderTest.SaveItem(Item);
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public void SaveItem_ItemHasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted");
			}
			else
			{
				RepositoryUnderTest.SaveItem(Item);
				CreateNewRepositoryFromPersistedData();
				Assert.AreEqual(1, RepositoryUnderTest.CountAllItems());
			}
		}

		[Test]
		public void SaveItems_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(Item);
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}

		[Test]
		public void SaveItems_LastModifiedIsSetInUTC()
		{
			SetState();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(Item);
			Thread.Sleep(50);
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public void SaveItems_ItemHasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted");
			}
			else
			{
				List<T> itemsToBeSaved = new List<T>();
				itemsToBeSaved.Add(Item);
				RepositoryUnderTest.SaveItems(itemsToBeSaved);
				CreateNewRepositoryFromPersistedData();
				Assert.AreEqual(1, RepositoryUnderTest.CountAllItems());
			}
		}
	}

	public abstract class IRepositoryPopulateFromPersistedTests<T> where T : class, new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if (_repositoryUnderTest == null)
				{
					throw new InvalidOperationException(
							"RepositoryUnderTest must be set before the tests are run.");
				}
				return _repositoryUnderTest;
			}
			set { _repositoryUnderTest = value; }
		}

		protected T Item
		{
			get { return item; }
			set { item = value; }
		}

		protected RepositoryId Id
		{
			get { return id; }
			set { id = value; }
		}

		public void SetState()
		{
			RepositoryId[] idsFrompersistedData = RepositoryUnderTest.GetAllItems();
			Id = idsFrompersistedData[0];
			Item = RepositoryUnderTest.GetItem(Id);
		}

		//This method is used to test whether data has been persisted.
		//This method should dispose of the current repository and reload it from persisted data
		//For repositories that don't support persistence this method should do nothing
		protected abstract void CreateNewRepositoryFromPersistedData();

		[Test]
		public void CreateItem_ReturnsUniqueItem()
		{
			SetState();
			Assert.AreNotEqual(Item, RepositoryUnderTest.CreateItem());
		}

		[Test]
		public void CreatedItemHasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist) {}
			else
			{
				CreateNewRepositoryFromPersistedData();
				RepositoryId[] listOfItems = RepositoryUnderTest.GetAllItems();
				Assert.AreEqual(1, listOfItems.Length);
				//Would be nice if this worked.. but it doesn't because we have equals for LexEntry is still by reference
				//T itemFromPersistedData = RepositoryUnderTest.GetItem(listOfItems[0]);
				//Assert.AreEqual(item, itemFromPersistedData);
			}
		}

		[Test]
		public void CountAllItems_ReturnsOne()
		{
			SetState();
			Assert.AreEqual(1, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		public void GetAllItems_ReturnsIdItem()
		{
			SetState();
			Assert.AreEqual(RepositoryUnderTest.GetId(Item), RepositoryUnderTest.GetAllItems()[0]);
		}

		[Test]
		public void GetAllItems_ReturnsCorrectNumberOfExistingItems()
		{
			SetState();
			Assert.AreEqual(1, RepositoryUnderTest.GetAllItems().Length);
		}

		[Test]
		public void GetId_CalledTwiceWithSameItem_ReturnsSameId()
		{
			SetState();
			Assert.AreEqual(RepositoryUnderTest.GetId(Item), RepositoryUnderTest.GetId(Item));
		}

		[Test]
		public void GetId_Item_ReturnsIdOfItem()
		{
			SetState();
			Assert.AreEqual(Id, RepositoryUnderTest.GetId(Item));
		}

		[Test]
		public void GetItem_Id_ReturnsItemWithId()
		{
			SetState();
			Assert.AreSame(Item, RepositoryUnderTest.GetItem(Id));
		}

		[Test]
		public void GetItem_CalledTwiceWithSameId_ReturnsSameItem()
		{
			SetState();
			Assert.AreSame(RepositoryUnderTest.GetItem(Id), RepositoryUnderTest.GetItem(Id));
		}

		[Test]
		public void GetItemMatchingQuery_QueryWithOutShow_ReturnsNoItems()
		{
			Query queryWithoutShow = new Query(typeof (T));
			SetState();
			if (RepositoryUnderTest.CanQuery)
			{
				ResultSet<T> resultSet = RepositoryUnderTest.GetItemsMatching(queryWithoutShow);
				Assert.AreEqual(0, resultSet.Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		public void GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery()
		{
			SetState();
			GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery_v();
		}

		protected virtual void GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery_v()
		{
			if (RepositoryUnderTest.CanQuery)
			{
				Assert.Fail(
					@"This Test is highly dependant on the type of objects that are
							being managed by the repository and as such should be tested elsewhere.");
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		public void LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime()
		{
			CreateNewRepositoryFromPersistedData();
			SetState();
			LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime_v();
		}

		protected virtual void LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime_v()
		{
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted");
			}
			else
			{
				Assert.Fail(
					"This test is dependant on how you are persisting your data, please override this test.");
			}
		}

		[Test]
		public void LastModified_IsSetInUTC()
		{
			SetState();
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		//This test is virtual because LexEntryRepository needs a special implementation
		[Test]
		public virtual void SaveItem_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			DateTime modifiedTimePreSave = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItem(Item);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreSave);
		}

		//This test is virtual because LexEntryRepository needs a special implementation
		[Test]
		public virtual void SaveItem_LastModifiedIsSetInUTC()
		{
			SetState();
			RepositoryUnderTest.SaveItem(Item);
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		//This test is virtual because LexEntryRepository needs a special implementation
		[Test]
		public virtual void SaveItems_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(Item);
			DateTime modifiedTimePreSave = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreSave);
		}

		[Test]
		public void SaveItems_LastModifiedIsSetInUTC()
		{
			SetState();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(Item);
			Thread.Sleep(50);
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}
	}

	public abstract class IRepositoryDeleteItemTransitionTests<T> where T : class, new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;
		private readonly Query query = new Query(typeof (T));

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if (_repositoryUnderTest == null)
				{
					throw new InvalidOperationException(
							"RepositoryUnderTest must be set before the tests are run.");
				}
				return _repositoryUnderTest;
			}
			set { _repositoryUnderTest = value; }
		}

		public T Item
		{
			get { return item; }
		}

		//This method is used to test whether data has been persisted.
		//This method should dispose of the current repository and reload it from persisted data
		//For repositories that don't support persistence this method should do nothing
		protected abstract void CreateNewRepositoryFromPersistedData();

		public void SetState()
		{
			CreateInitialItem();
			DeleteItem();
		}

		private void DeleteItem()
		{
			RepositoryUnderTest.DeleteItem(Item);
		}

		private void CreateInitialItem()
		{
			this.item = RepositoryUnderTest.CreateItem();
			this.id = RepositoryUnderTest.GetId(Item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void DeleteItem_ItemDoesNotExist_Throws()
		{
			SetState();
			RepositoryUnderTest.DeleteItem(Item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void DeleteItem_HasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				CreateNewRepositoryFromPersistedData();
				RepositoryUnderTest.GetItem(id);
			}
		}

		[Test]
		public void CountAllItems_ReturnsZero()
		{
			SetState();
			Assert.AreEqual(0, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		public void GetAllItems_ReturnsEmptyArray()
		{
			SetState();
			Assert.IsEmpty(RepositoryUnderTest.GetAllItems());
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void GetId_DeletedItemWithId_Throws()
		{
			SetState();
			RepositoryUnderTest.GetId(Item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void GetItem_DeletedItem_Throws()
		{
			SetState();
			RepositoryUnderTest.GetItem(id);
		}

		[Test]
		public void GetItemMatchingQuery_Query_ReturnsEmpty()
		{
			SetState();
			if (RepositoryUnderTest.CanQuery)
			{
				Assert.AreEqual(0, RepositoryUnderTest.GetItemsMatching(query).Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		public void LastModified_IsChangedToLaterTime()
		{
			CreateInitialItem();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			DeleteItem();
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}

		[Test]
		public void LastModified_IsSetInUTC()
		{
			SetState();
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		//This test is virtual because LexEntryRepository needs to override it
		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public virtual void SaveItem_ItemDoesNotExist_Throws()
		{
			SetState();
			RepositoryUnderTest.SaveItem(Item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SaveItems_ItemDoesNotExist_Throws()
		{
			SetState();
			T itemNotInRepository = new T();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(itemNotInRepository);
			RepositoryUnderTest.SaveItems(itemsToSave);
		}
	}

	public abstract class IRepositoryDeleteIdTransitionTests<T> where T : class, new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;
		private readonly Query query = new Query(typeof (T));

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if (_repositoryUnderTest == null)
				{
					throw new InvalidOperationException(
							"RepositoryUnderTest must be set before the tests are run.");
				}
				return _repositoryUnderTest;
			}
			set { _repositoryUnderTest = value; }
		}

		public T Item
		{
			get { return item; }
		}

		//This method is used to test whether data has been persisted.
		//This method should dispose of the current repository and reload it from persisted data
		//For repositories that don't support persistence this method should do nothing
		protected abstract void CreateNewRepositoryFromPersistedData();

		public void SetState()
		{
			CreateItemToTest();
			DeleteItem();
		}

		private void DeleteItem()
		{
			RepositoryUnderTest.DeleteItem(this.id);
		}

		private void CreateItemToTest()
		{
			this.item = RepositoryUnderTest.CreateItem();
			this.id = RepositoryUnderTest.GetId(Item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void DeleteItem_ItemDoesNotExist_Throws()
		{
			SetState();
			RepositoryUnderTest.DeleteItem(id);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void DeleteItem_HasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				CreateNewRepositoryFromPersistedData();
				RepositoryUnderTest.GetItem(id);
			}
		}

		[Test]
		public void CountAllItems_ReturnsZero()
		{
			SetState();
			Assert.AreEqual(0, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		public void GetAllItems_ReturnsEmptyArray()
		{
			SetState();
			Assert.IsEmpty(RepositoryUnderTest.GetAllItems());
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void GetId_DeletedItemWithId_Throws()
		{
			SetState();
			RepositoryUnderTest.GetId(Item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void GetItem_DeletedItem_Throws()
		{
			SetState();
			RepositoryUnderTest.GetItem(id);
		}

		[Test]
		public void GetItemMatchingQuery_Query_ReturnsEmpty()
		{
			SetState();
			if (RepositoryUnderTest.CanQuery)
			{
				Assert.AreEqual(0, RepositoryUnderTest.GetItemsMatching(query).Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		public void LastModified_ItemIsDeleted_IsChangedToLaterTime()
		{
			CreateItemToTest();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			DeleteItem();
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}

		[Test]
		public void LastModified_ItemIsDeleted_IsSetInUTC()
		{
			SetState();
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		//This test is virtual because LexEntryRepository needs to override it
		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public virtual void SaveItem_ItemDoesNotExist_Throws()
		{
			SetState();
			RepositoryUnderTest.SaveItem(Item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SaveItems_ItemDoesNotExist_Throws()
		{
			SetState();
			T itemNotInRepository = new T();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(itemNotInRepository);
			RepositoryUnderTest.SaveItems(itemsToSave);
		}
	}

	public abstract class IRepositoryDeleteAllItemsTransitionTests<T> where T : class, new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;
		private readonly Query query = new Query(typeof (T));

		public IRepository<T> RepositoryUnderTest
		{
			get
			{
				if (_repositoryUnderTest == null)
				{
					throw new InvalidOperationException(
							"RepositoryUnderTest must be set before the tests are run.");
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
			DeleteAllItems();
		}

		private void DeleteAllItems()
		{
			RepositoryUnderTest.DeleteAllItems();
		}

		private void CreateInitialItem()
		{
			this.item = RepositoryUnderTest.CreateItem();
			this.id = RepositoryUnderTest.GetId(this.item);
		}

		[Test]
		public void DeleteAllItems_ItemDoesNotExist_DoesNotThrow()
		{
			SetState();
			RepositoryUnderTest.DeleteAllItems();
		}

		[Test]
		public void DeleteAllItems_HasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				RepopulateRepositoryFromPersistedData();
				Assert.IsEmpty(RepositoryUnderTest.GetAllItems());
			}
		}

		[Test]
		public void CountAllItems_ReturnsZero()
		{
			SetState();
			Assert.AreEqual(0, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		public void GetAllItems_ReturnsEmptyArray()
		{
			SetState();
			Assert.IsEmpty(RepositoryUnderTest.GetAllItems());
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void GetId_DeletedItemWithId_Throws()
		{
			SetState();
			RepositoryUnderTest.GetId(item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void GetItem_DeletedItem_Throws()
		{
			SetState();
			RepositoryUnderTest.GetItem(id);
		}

		[Test]
		public void GetItemMatchingQuery_Query_ReturnsEmpty()
		{
			SetState();
			if (RepositoryUnderTest.CanQuery)
			{
				Assert.AreEqual(0, RepositoryUnderTest.GetItemsMatching(query).Count);
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		public void LastModified_IsChangedToLaterTime()
		{
			CreateInitialItem();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			DeleteAllItems();
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}

		[Test]
		public void LastModified_IsSetInUTC()
		{
			SetState();
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Save_ItemDoesNotExist_Throws()
		{
			SetState();
			RepositoryUnderTest.SaveItem(item);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SaveItems_ItemDoesNotExist_Throws()
		{
			T itemNotInRepository = new T();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(itemNotInRepository);
			RepositoryUnderTest.SaveItems(itemsToSave);
		}
	}
}