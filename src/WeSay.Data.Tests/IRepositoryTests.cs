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
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteItemById_Null_Throws()
		{
			RepositoryUnderTest.DeleteItem((RepositoryId) null);
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
		public void GetAllItems_ReturnsEmptyArray()
		{
			Assert.IsEmpty(RepositoryUnderTest.GetAllItems());
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
		[ExpectedException(typeof(NotSupportedException))]
		public void GetItemsMatchingQuery_CanQueryIsFalse_Throws()
		{
			if (!RepositoryUnderTest.CanQuery)
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
			Assert.Less(RepositoryUnderTest.LastModified, PreciseDateTime.UtcNow);
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
			List<T> itemsToSave = new List<T>();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.AreEqual(modifiedTimePreTestedStateSwitch, RepositoryUnderTest.LastModified);
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
		protected abstract void CreateNewRepositoryFromPersistedData();

		[Test]
		public void CreateItem_ReturnsUniqueItem()
		{
			SetState();
			Assert.AreNotEqual(item, RepositoryUnderTest.CreateItem());
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
			Assert.AreEqual(RepositoryUnderTest.GetId(item), RepositoryUnderTest.GetAllItems()[0]);
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
			Assert.AreEqual(RepositoryUnderTest.GetId(item), RepositoryUnderTest.GetId(item));
		}

		[Test]
		public void GetId_Item_ReturnsIdOfItem()
		{
			SetState();
			Assert.AreEqual(id, RepositoryUnderTest.GetId(item));
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
		public void GetItemMatchingQuery_QueryWithOutShow_ReturnsAllItems()
		{
			Query queryWithoutShow = new Query(typeof(T));
			SetState();
			if (RepositoryUnderTest.CanQuery)
			{
				ResultSet<T> resultSet = RepositoryUnderTest.GetItemsMatching(queryWithoutShow);
				Assert.AreEqual(1, resultSet.Count);
				Assert.AreEqual(item, resultSet[0].RealObject);
				Assert.AreEqual(id, resultSet[0].Id);
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
			if (RepositoryUnderTest.CanQuery)
			{
				Assert.Ignore(@"This Test is highly dependant on the type of objects that are
							being managed by the repository and as such should be overridden.");
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
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
		public void SaveItem_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItem(item);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}

		[Test]
		public void SaveItem_LastModifiedIsSetInUTC()
		{
			SetState();
			RepositoryUnderTest.SaveItem(item);
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public virtual void SaveItem_ItemHasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted");
			}
			else
			{
				CreateNewRepositoryFromPersistedData();
				T itemFromPersistedData = RepositoryUnderTest.GetItem(id);

				Assert.AreEqual(item, itemFromPersistedData);
			}
		}

		[Test]
		public void SaveItems_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(item);
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
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
		public virtual void SaveItems_ItemHasBeenPersisted()
		{
			SetState();
			if(!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted");
			}
			else
			{
				CreateNewRepositoryFromPersistedData();
				T itemFromPersistedData = RepositoryUnderTest.GetItem(id);
				Assert.AreEqual(item, itemFromPersistedData);
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
					throw new InvalidOperationException("RepositoryUnderTest must be set before the tests are run.");
				}
				return _repositoryUnderTest;
			}
			set { _repositoryUnderTest = value; }
		}

		public void SetState()
		{
			RepositoryId[] idsFrompersistedData = RepositoryUnderTest.GetAllItems();
			id = idsFrompersistedData[0];
			item = RepositoryUnderTest.GetItem(id);
		}

		//This method is used to test whether data has been persisted.
		//This method should dispose of the current repository and reload it from persisted data
		//For repositories that don't support persistence this method should do nothing
		protected abstract void CreateNewRepositoryFromPersistedData();

		[Test]
		public void CreateItem_ReturnsUniqueItem()
		{
			SetState();
			Assert.AreNotEqual(item, RepositoryUnderTest.CreateItem());
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
			Assert.AreEqual(RepositoryUnderTest.GetId(item), RepositoryUnderTest.GetAllItems()[0]);
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
			Assert.AreEqual(RepositoryUnderTest.GetId(item), RepositoryUnderTest.GetId(item));
		}

		[Test]
		public void GetId_Item_ReturnsIdOfItem()
		{
			SetState();
			Assert.AreEqual(id, RepositoryUnderTest.GetId(item));
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
		public void GetItemMatchingQuery_QueryWithOutShow_ReturnsAllItems()
		{
			Query queryWithoutShow = new Query(typeof(T));
			SetState();
			if (RepositoryUnderTest.CanQuery)
			{
				ResultSet<T> resultSet = RepositoryUnderTest.GetItemsMatching(queryWithoutShow);
				Assert.AreEqual(1, resultSet.Count);
				Assert.AreEqual(item, resultSet[0].RealObject);
				Assert.AreEqual(id, resultSet[0].Id);
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
			if (RepositoryUnderTest.CanQuery)
			{
				Assert.Ignore(@"This Test is highly dependant on the type of objects that are
							being managed by the repository and as such should be tested elsewhere.");
			}
			else
			{
				Assert.Ignore("Repository does not support queries.");
			}
		}

		[Test]
		public virtual void LastModified_IsSetToPersistedDatasLastChangedTime()
		{
			if(!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted");
			}
			else
			{
				Assert.Fail("This test is dependant on how you are persisting your data, please override this test.");
			}
		}

		[Test]
		public void LastModified_IsSetInUTC()
		{
			SetState();
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public void SaveItem_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItem(item);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
		}

		[Test]
		public void SaveItem_LastModifiedIsSetInUTC()
		{
			SetState();
			RepositoryUnderTest.SaveItem(item);
			Assert.AreEqual(DateTimeKind.Utc, RepositoryUnderTest.LastModified.Kind);
		}

		[Test]
		public void SaveItem_ItemHasBeenPersisted()
		{
			SetState();
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				CreateNewRepositoryFromPersistedData();
				T itemFromPersistedData = RepositoryUnderTest.GetItem(id);
				Assert.AreEqual(item, itemFromPersistedData);
			}
		}

		[Test]
		public void SaveItems_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(item);
			DateTime modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			RepositoryUnderTest.SaveItems(itemsToSave);
			Assert.Greater(RepositoryUnderTest.LastModified, modifiedTimePreTestedStateSwitch);
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
			SetState();
			if (!RepositoryUnderTest.CanPersist)
			{
				Assert.Ignore("Repository can not be persisted.");
			}
			else
			{
				CreateNewRepositoryFromPersistedData();
				T itemFromPersistedData = RepositoryUnderTest.GetItem(id);
				Assert.AreEqual(item, itemFromPersistedData);
			}
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
		protected abstract void CreateNewRepositoryFromPersistedData();

		public void SetState()
		{
			CreateInitialItem();
			SaveItem();
			DeleteItem();
		}

		private void DeleteItem() {
			RepositoryUnderTest.DeleteItem(this.item);
		}

		private void CreateInitialItem() {
			this.item = RepositoryUnderTest.CreateItem();
			this.id = RepositoryUnderTest.GetId(this.item);
		}

		private void SaveItem()
		{
			RepositoryUnderTest.SaveItem(item);
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

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SaveItem_ItemDoesNotExist_Throws()
		{
			SetState();
			RepositoryUnderTest.SaveItem(item);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
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
		protected abstract void CreateNewRepositoryFromPersistedData();

		public void SetState()
		{
			CreateItemToTest();
			SaveItem();
			DeleteItem();
		}

		private void DeleteItem() {
			RepositoryUnderTest.DeleteItem(this.id);
		}

		private void CreateItemToTest() {
			this.item = RepositoryUnderTest.CreateItem();
			this.id = RepositoryUnderTest.GetId(this.item);
		}

		private void SaveItem()
		{
			RepositoryUnderTest.SaveItem(item);
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

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SaveItem_ItemDoesNotExist_Throws()
		{
			SetState();
			RepositoryUnderTest.SaveItem(item);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SaveItems_ItemDoesNotExist_Throws()
		{
			SetState();
			T itemNotInRepository = new T();
			List<T> itemsToSave = new List<T>();
			itemsToSave.Add(itemNotInRepository);
			RepositoryUnderTest.SaveItems(itemsToSave);
		}
	}
}
