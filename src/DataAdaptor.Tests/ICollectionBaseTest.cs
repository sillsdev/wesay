using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;


namespace WeSay.DataAdaptor.Tests
{
	public class ICollectionBaseTest<T>
	{
		protected ICollection _collection;
		protected int _itemCount;

		[Test]
		public void Count()
		{
			Assert.AreEqual(_itemCount, this._collection.Count);
		}

		[Test]
		public void SyncRoot()
		{
			Assert.IsNotNull(this._collection.SyncRoot);
			lock (this._collection.SyncRoot)
			{
				//some operation on the collection which is now thread safe
			}
		}

		[Test]
		public void CopyTo()
		{
			T[] array = new T[this._itemCount];
			this._collection.CopyTo(array, 0);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CopyToNullArray()
		{
			this._collection.CopyTo(null, 0);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CopyToIndexLessThanZero()
		{
			T[] array = new T[this._itemCount];
			this._collection.CopyTo(array, -1);
		}


		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void CopyToArrayIsMultidimensional()
		{
			T[,] array = new T[this._itemCount+1, this._itemCount+1];
			this._collection.CopyTo(array, 0);
		}


		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void CopyToIndexEqualLengthOfArray()
		{
			T[] array = new T[this._itemCount+1];
			this._collection.CopyTo(array, this._itemCount+1);
			if (this._itemCount == 0)
			{
				throw new ArgumentException();
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void CopyToIndexGreaterThanLengthOfArray()
		{
			T[] array = new T[this._itemCount];
			this._collection.CopyTo(array, this._itemCount + 1);
		}

		/// <summary>
		/// The number of elements in the source ICollection is
		/// greater than the available space from index to the
		/// end of the destination array.
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void CopyToOutOfSpace()
		{
			T[] array = new T[this._itemCount];
			this._collection.CopyTo(array, 1);
		}

		class MyClass
		{
			int t = 0;
			public int T
			{
				get
				{
					return t;
				}
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidCastException))]
		public void CopyToInvalidCast()
		{
			MyClass[] array = new MyClass[this._itemCount+1 ];
			this._collection.CopyTo(array, 0);
			if (this._itemCount == 0)
			{
				throw new InvalidCastException();
			}
		}


	}

	[TestFixture]
	public class ICollectionIntTest : ICollectionBaseTest<int>
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

			this._collection = list;
			this._itemCount = 5;
		}
	}

	[TestFixture]
	public class ICollectionStringTest : ICollectionBaseTest<string>
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

			this._collection = list;
			this._itemCount = 5;

		}
	}

	[TestFixture]
	public class ICollectionNoDataTest : ICollectionBaseTest<string>
	{
		[SetUp]
		public void SetUp()
		{
			this._collection = new ArrayList();
			this._itemCount = 0;
		}
	}
}
