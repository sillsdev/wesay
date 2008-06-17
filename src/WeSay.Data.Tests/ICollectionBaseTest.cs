using System;
using System.Collections;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	public class ICollectionBaseTest<T>
	{
		protected ICollection _collection;
		protected int _itemCount;

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
		[ExpectedException(typeof (ArgumentNullException))]
		public void CopyToNullArray()
		{
			_collection.CopyTo(null, 0);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void CopyToIndexLessThanZero()
		{
			T[] array = new T[_itemCount];
			_collection.CopyTo(array, -1);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void CopyToArrayIsMultidimensional()
		{
			T[,] array = new T[_itemCount + 1,_itemCount + 1];
			_collection.CopyTo(array, 0);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void CopyToIndexEqualLengthOfArray()
		{
			T[] array = new T[_itemCount + 1];
			_collection.CopyTo(array, _itemCount + 1);
			if (_itemCount == 0)
			{
				throw new ArgumentException();
			}
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void CopyToIndexGreaterThanLengthOfArray()
		{
			T[] array = new T[_itemCount];
			_collection.CopyTo(array, _itemCount + 1);
		}

		/// <summary>
		/// The number of elements in the source ICollection is
		/// greater than the available space from index to the
		/// end of the destination array.
		/// </summary>
		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void CopyToOutOfSpace()
		{
			T[] array = new T[_itemCount];
			_collection.CopyTo(array, 1);
		}

		private class MyClass
		{
			private readonly int t = 0;

			public int T
			{
				get { return t; }
			}
		}

		[Test]
		[ExpectedException(typeof (InvalidCastException))]
		public void CopyToInvalidCast()
		{
			MyClass[] array = new MyClass[_itemCount + 1];
			_collection.CopyTo(array, 0);
			if (_itemCount == 0)
			{
				throw new InvalidCastException();
			}
		}
	}

	[TestFixture]
	public class ICollectionIntTest: ICollectionBaseTest<int>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
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
		[TestFixtureSetUp]
		public void FixtureSetUp()
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
		public void SetUp()
		{
			_collection = new ArrayList();
			_itemCount = 0;
		}
	}
}