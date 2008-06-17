using System;
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
		public void CountAllItems_NoItemsInTheRepostory_ReturnsZero()
		{
			Assert.AreEqual(0, RepositoryUnderTest.CountAllItems());
		}

		[Test]
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
			RepositoryId id = new RepositoryId();
			RepositoryUnderTest.GetItem(id);
		}

		//[Test]
		//public void GetAllItems_ReturnsAllItems()
		//{
		//    Assert.Fail("Not implemented");
		//}

		//[Test]
		//public void GetItemsMatchingQuery_ReturnsAllItemsMatchingQuery()
		//{
		//    Assert.Fail("Not implemented");
		//}
	}

	public class IRepositoryCreateItemTransitionTests<T> where T : new()
	{
		private IRepository<T> _repositoryUnderTest;

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

		[Test]
		public void CreateItem_NotNull()
		{
			Assert.IsNotNull(RepositoryUnderTest.CreateItem());
		}

		[Test]
		public void CountAllItems_Createitem_ReturnsNumberOfItemsInRepository()
		{
			RepositoryUnderTest.CreateItem();
			RepositoryUnderTest.CreateItem();
			Assert.AreEqual(2, RepositoryUnderTest.CountAllItems());
		}

		[Test]
		public void GetItem_Id_ReturnsItemWithId()
		{
			T item = RepositoryUnderTest.CreateItem();
			RepositoryId itemId = RepositoryUnderTest.GetId(item);
			Assert.AreSame(item, RepositoryUnderTest.GetItem(itemId));
		}
	}

	public class IRepositoryDeleteItemTransitionTests<T> where T : new()
	{
		private IRepository<T> _repositoryUnderTest;

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

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteItem_ItemDoesNotExist_Throws()
		{
			T item = new T();
			RepositoryUnderTest.DeleteItem(item);
		}
	}

	public class IRepositoryDeleteIdTransitionTests<T> where T : new()
	{
		private IRepository<T> _repositoryUnderTest;

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

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DeleteId_ItemWithIdDoesNotExist_Throws()
		{
			Assert.Fail("Not implemented");
		}
	}
}