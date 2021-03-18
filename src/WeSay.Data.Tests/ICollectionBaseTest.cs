using System;
using System.Collections;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	public abstract class ICollectionBaseTest<T>
	{
		protected ICollection _collection;
		protected int _itemCount;

		[OneTimeSetUp]
		public virtual void OneTimeSetUp() {}

		[SetUp]
		public virtual void SetUp() {}

		[Test]
		public void Count()
		{
			Assert.AreEqual(_itemCount, _collection.Count);
		}

		[Test]
		public void SyncRoot()
		{
			Assert.IsNotNull(_collection.SyncRoot);
			lock (_collection.SyncRoot)
			{
				//some operation on the collection which is now thread safe
			}
		}

		[Test]
		public void CopyTo()
		{
			T[] array = new T[_itemCount];
			_collection.CopyTo(array, 0);
		}

		[Test]
		public void CopyToNullArray()
		{
			Assert.Throws<ArgumentNullException>(() => _collection.CopyTo(null, 0));
		}

		[Test]
		public void CopyToIndexLessThanZero()
		{
			T[] array = new T[_itemCount];
			Assert.Throws<ArgumentOutOfRangeException>(() => _collection.CopyTo(array, -1));
		}

		[Test]
		public void CopyToArrayIsMultidimensional()
		{
			T[,] array = new T[_itemCount + 1,_itemCount + 1];
			Assert.Throws<ArgumentException>(() => _collection.CopyTo(array, 0));
		}

		[Test]
		public virtual void CopyToIndexEqualLengthOfArray()
		{
			T[] array = new T[_itemCount + 1];
			Assert.Throws<ArgumentException>(() => _collection.CopyTo(array, _itemCount + 1));
		}

		[Test]
		public void CopyToIndexGreaterThanLengthOfArray()
		{
			T[] array = new T[_itemCount];
			Assert.Throws<ArgumentException>(() => _collection.CopyTo(array, _itemCount + 1));
		}

		/// <summary>
		/// The number of elements in the source ICollection is
		/// greater than the available space from index to the
		/// end of the destination array.
		/// </summary>
		[Test]
		public void CopyToOutOfSpace()
		{
			T[] array = new T[_itemCount];
			Assert.Throws<ArgumentException>(() => _collection.CopyTo(array, 1));
		}

		private class MyClass
		{
			public int T => 0;
		}

		[Test]
		public virtual void CopyToInvalidCast()
		{
			MyClass[] array = new MyClass[_itemCount + 1];
			Assert.Throws<InvalidCastException>(() => _collection.CopyTo(array, 0));
		}
	}

	[TestFixture]
	public class ICollectionIntTest: ICollectionBaseTest<int>
	{
		[OneTimeSetUp]
		public override void OneTimeSetUp()
		{
			ArrayList list = new ArrayList();

			list.Add(1);
			list.Add(3);
			list.Add(5);
			list.Add(7);
			list.Add(9);

			_collection = list;
			_itemCount = 5;
		}
	}

	[TestFixture]
	public class ICollectionStringTest: ICollectionBaseTest<string>
	{
		[OneTimeSetUp]
		public override void OneTimeSetUp()
		{
			ArrayList list = new ArrayList();
			list.Add("1");
			list.Add("3");
			list.Add("5");
			list.Add("7");
			list.Add("9");

			_collection = list;
			_itemCount = 5;
		}
	}

	[TestFixture]
	public class ICollectionNoDataTest: ICollectionBaseTest<string>
	{
		[SetUp]
		public override void SetUp()
		{
			_collection = new ArrayList();
			_itemCount = 0;
		}


		//These two tests are inherited from ICollectionBaseTest<T> and are meaningless for an empty ICollection.
		[Test]
		public override void CopyToIndexEqualLengthOfArray()
		{
			Assert.AreEqual(_itemCount, 0);
		}

		[Test]
		public override void CopyToInvalidCast()
		{
			Assert.AreEqual(_itemCount, 0);
		}

	}
}