using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;


namespace WeSay.Data.Tests.IEnumerableTests
{
	/// <summary>
	/// A collection of _itemCount items should be initialized in _enumerable.
	/// </summary>
	public class IEnumerableBaseTest<T>
	{
		protected IEnumerable _enumerable;
		protected int _itemCount;

		[Test]
		public void Enumerator()
		{
			IEnumerator enumerator = this._enumerable.GetEnumerator();
			Assert.IsNotNull(enumerator);
		}

		[Test]
		public void ForEach()
		{
			int i = 0;
			foreach (T item in this._enumerable)
			{
				Assert.IsNotNull(item);
				i++;
			}
			Assert.AreEqual(_itemCount, i);
		}

		[Test]
		public void Current()
		{
			IEnumerator enumerator = this._enumerable.GetEnumerator();
			T prevCurrent = default(T);
			for (int i = 0; i < _itemCount; ++i)
			{
				Assert.IsTrue(enumerator.MoveNext());
				T current = (T)enumerator.Current;
				Assert.IsNotNull(current);
				Assert.AreNotSame(prevCurrent, current);
				prevCurrent = current;
			}
			Assert.IsFalse(enumerator.MoveNext());
		}

		[Test]
		public void MoveNext()
		{
			IEnumerator enumerator = this._enumerable.GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext())
			{
				i++;
			}
			Assert.AreEqual(_itemCount, i);
		}

		[Test]
		public void Reset()
		{
			IEnumerator enumerator = this._enumerable.GetEnumerator();
			enumerator.Reset();
			if (this._itemCount > 0)
			{
				Assert.IsTrue(enumerator.MoveNext());

				T first = (T)enumerator.Current;
				enumerator.Reset();
				Assert.IsTrue(enumerator.MoveNext());
				Assert.AreEqual(first, enumerator.Current);
				if (!first.GetType().IsValueType)
				{
					Assert.AreSame(first, enumerator.Current);
				}
			}
		}


		/// <summary>
		/// After an enumerator is created, the MoveNext method must be
		/// called to advance the enumerator to the first element of the
		/// collection before reading the value of the Current property.
		/// </summary>
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CurrentThrowsBeforeMoveNext()
		{
			IEnumerator enumerator = this._enumerable.GetEnumerator();
			T current = (T)enumerator.Current;
		}

		/// <summary>
		/// After the Reset method is called, the MoveNext method must be
		/// called to advance the enumerator to the first element of the
		/// collection before reading the value of the Current property.
		/// </summary>
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CurrentThrowsAfterResetBeforeMoveNext()
		{
			IEnumerator enumerator = this._enumerable.GetEnumerator();
			enumerator.MoveNext();
			enumerator.Reset();
			T current = (T)enumerator.Current;
		}

		/// <summary>
		/// Current also throws an exception if the last call to
		/// MoveNext returned false, which indicates the end of the collection.
		/// </summary>
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CurrentThrowsAtEndOfCollection()
		{
			IEnumerator enumerator = this._enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
			}
			T current = (T)enumerator.Current;
		}
	}

	[TestFixture]
	public class IEnumerableTest : IEnumerableBaseTest<int>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			List<int> list = new List<int>();

			list.Add(1);
			list.Add(3);
			list.Add(5);
			list.Add(7);
			list.Add(9);

			this._enumerable = list;
			this._itemCount = list.Count;
		}
	}

	[TestFixture]
	public class IEnumerableStringListTest : IEnumerableBaseTest<string>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			List<string> list = new List<string>();

			list.Add("1");
			list.Add("3");
			list.Add("5");
			list.Add("7");
			list.Add("9");

			this._enumerable = list;
			this._itemCount = list.Count;
		}
	}

	[TestFixture]
	public class IEnumerableNoDataTest : IEnumerableBaseTest<string>
	{
		[SetUp]
		public void SetUp()
		{
			this._enumerable = new List<string>();
			this._itemCount = 0;
		}
	}
}
