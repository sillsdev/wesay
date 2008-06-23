using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using WeSay.Data;

namespace WeSay.Data.Tests
{
	public class IRepositoryStateUnitializedTests<T> where T: new()
	{
		private IRepository<T> _repositoryUnderTest;

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
		public void ItemsModifiedSince_ReturnsEmptyArray()
		{
			Assert.IsEmpty(RepositoryUnderTest.GetItemsModifiedSince(DateTime.MinValue));
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

	public class IRepositoryCreateItemTransitionTests<T> where T : new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;
		private DateTime modifiedTimePreTestedStateSwitch;

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
			modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
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
			Assert.AreSame(RepositoryUnderTest.GetId(item), RepositoryUnderTest.GetId(item));
		}

		[Test]
		public void LastModified_IsChanged()
		{
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
		public void ItemsModifiedSince_SaveItem_ReturnsItem()
		{
			SetState();
			Thread.Sleep(50);
			DateTime timeOfSave = DateTime.UtcNow;
			RepositoryUnderTest.SaveItem(item);
			Assert.AreEqual(id, RepositoryUnderTest.GetItemsModifiedSince(timeOfSave)[0]);
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
		public void ItemsModifiedSince_ReturnsIdsOfItemsModifiedSince()
		{
			SetState();
			TimeSpan timeSpan = new TimeSpan(1, 0, 0); //1 hour
			Assert.AreEqual(RepositoryUnderTest.GetId(item), RepositoryUnderTest.GetItemsModifiedSince(DateTime.UtcNow - timeSpan)[0]);
		}


		[Test]
		//This test will not fail if you are in GMC+0 Timezone! If this is true for you, please be sure to transform
		//any input ItemsModifiedSince() to UTC. That is what this test checks for everybody else.
		public void ItemsModifiedSince_NonUTCDateTime_ReturnsIdsModifiedSince()
		{
			TimeSpan OffSet = new TimeSpan(0, 2, 0); //2 minutes
			SetState();
			Thread.Sleep(50);
			DateTime timeBetweenCreatedItems = DateTime.Now;
			Thread.Sleep(50);
			T item2 = RepositoryUnderTest.CreateItem();
			Assert.AreEqual(1, RepositoryUnderTest.GetItemsModifiedSince(timeBetweenCreatedItems).Length);
			Assert.AreEqual(RepositoryUnderTest.GetId(item2),
				RepositoryUnderTest.GetItemsModifiedSince(timeBetweenCreatedItems)[0]);
		}
	}

	public class IRepositoryDeleteItemTransitionTests<T> where T : new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;
		private DateTime modifiedTimePreTestedStateSwitch;

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
			modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			Thread.Sleep(50);
			RepositoryUnderTest.DeleteItem(item);
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
		public void ItemsModifiedSince_ReturnsEmptyArray()
		{
			SetState();
			Assert.IsEmpty(RepositoryUnderTest.GetItemsModifiedSince(DateTime.MinValue));
		}
	}

	public class IRepositoryDeleteIdTransitionTests<T> where T : new()
	{
		private IRepository<T> _repositoryUnderTest;
		private T item;
		private RepositoryId id;
		private DateTime modifiedTimePreTestedStateSwitch;

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
			modifiedTimePreTestedStateSwitch = RepositoryUnderTest.LastModified;
			Thread.Sleep(50);
			RepositoryUnderTest.DeleteItem(id);
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
			SetState();
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
		public void ItemsModifiedSince_ReturnsEmptyArray()
		{
			SetState();
			Assert.IsEmpty(RepositoryUnderTest.GetItemsModifiedSince(DateTime.MinValue));
		}
	}
}