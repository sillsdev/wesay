using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;


namespace WeSay.Data.Tests.IListTests
{
	// Four Types of IList : FixedSizeReadOnly, FixedSizeReadWrite,
	//                       VariableSizeReadOnly, VariableSizeReadWrite


	public class IListBaseTest<T>
	{
		protected IList _list;
		protected T _firstItem;
		protected T _newItem;
		protected bool _isSorted;

		[Test]
		public void GetItem()
		{
			if (_list.Count > 0)
			{
				Assert.AreEqual(this._firstItem, this._list[0]);
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetItemNegativeIndex()
		{
			T item = (T)this._list[-1];
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetItemPastTheEnd()
		{
			T item = (T)this._list[this._list.Count];
		}

		[Test]
		public void Contains()
		{
			Assert.IsFalse(this._list.Contains(default(T)));
			if (_list.Count > 0)
			{
				Assert.IsTrue(this._list.Contains(this._firstItem));
			}
		}

		[Test]
		public void IndexOf()
		{
			Assert.AreEqual(-1, this._list.IndexOf(default(T)));
			if (this._list.Count > 0)
			{
				Assert.AreEqual(0, this._list.IndexOf(this._firstItem));
				Assert.AreEqual(this._list.Count - 1, this._list.IndexOf(this._list[this._list.Count - 1]));
			}
		}

		#region Read-Write
		public void SetItem()
		{
			if (!_isSorted)
			{
				if (this._list.Count == 0)
				{
					this._list.Add(this._newItem);
					this._list.Add(this._firstItem);
					this._list.Add(this._newItem);
				}
				Assert.AreNotEqual(this._newItem, this._list[1]);
				this._list[1] = this._newItem;
				Assert.AreEqual(this._newItem, this._list[1]);
			}
		}
		public void SetItemNotSupported()
		{
			if (_isSorted)
			{
				this._list[1] = this._firstItem;
			}
			else
			{
				throw new NotSupportedException();
			}
		}
		public void SetItemNegativeIndex()
		{
			if (_isSorted)
			{
				throw new ArgumentOutOfRangeException();
			}
			else
			{
				this._list[-1] = this._firstItem;
			}
		}

		public void SetItemPastTheEnd()
		{
			if (_isSorted)
			{
				throw new ArgumentOutOfRangeException();
			}
			else
			{
				this._list[this._list.Count] = this._firstItem;
			}
		}
		#endregion

		#region Variable-Size
		public void Add()
		{
			int count = this._list.Count;
			this._list.Add(_newItem);
			Assert.AreEqual(count + 1, this._list.Count);
			Assert.AreEqual(_newItem, this._list[count]);
		}

		public void Clear()
		{
			this._list.Clear();
			Assert.AreEqual(0, this._list.Count);
		}

		public void Insert()
		{
			if (!_isSorted)
			{
				int count = this._list.Count;
				this._list.Insert(0, this._newItem);
				Assert.AreEqual(count + 1, this._list.Count);
				Assert.AreEqual(this._newItem, this._list[0]);
				if (count > 0)
				{
					Assert.AreEqual(this._firstItem, this._list[1]);
					Assert.AreNotEqual(this._firstItem, this._list[2]);
					Assert.AreNotEqual(this._newItem, this._list[this._list.Count - 1]);
				}
				this._list.Insert(this._list.Count, this._newItem);
				Assert.AreEqual(count + 2, this._list.Count);
				Assert.AreEqual(this._newItem, this._list[this._list.Count - 1]);
			}
		}

		public void InsertNotSupported()
		{
			if (_isSorted)
			{
				this._list.Insert(0, this._newItem);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		public void InsertNegativeIndex()
		{
			if (_isSorted)
			{
				throw new ArgumentOutOfRangeException();
			}
			else
			{
				this._list.Insert(-1, this._newItem);
			}
		}

		public void InsertPastTheEnd()
		{
			if (_isSorted)
			{
				throw new ArgumentOutOfRangeException();
			}
			else
			{
				this._list.Insert(this._list.Count + 1, this._newItem);
			}
		}

		public void Remove()
		{
			this._list.Add(this._newItem);
			int count = this._list.Count;
			this._list.Remove(this._newItem);
			Assert.AreEqual(count - 1, this._list.Count);
			if (this._list.Count > 0)
			{
				count = this._list.Count;
				this._list.Remove(this._firstItem);
				Assert.AreEqual(count - 1, this._list.Count);
			}
			if (this._list.Count > 0)
			{
				Assert.AreNotEqual(this._firstItem, this._list[0]);
			}

			count = this._list.Count;
			this._list.Remove(this._firstItem);
			Assert.AreEqual(count, this._list.Count);
		}
		public void RemoveAt()
		{
			int count = this._list.Count;
			if (count == 0)
			{
				this._list.Add(this._newItem);
				count = 1;
			}
			this._list.RemoveAt(0);
			if (this._list.Count > 0)
			{
				Assert.AreNotEqual(this._newItem, this._list[0]);
			}
			Assert.AreEqual(count - 1, this._list.Count);
		}

		public void RemoveAtNegativeIndex()
		{
			this._list.RemoveAt(-1);
		}

		public void RemoveAtPastTheEnd()
		{
			this._list.RemoveAt(this._list.Count);
		}
		#endregion
	}
	public class IListVariableSizeReadWriteBaseTest<T> : IListBaseTest<T>
	{
		[Test]
		public void IsFixedSize()
		{
			Assert.IsFalse(this._list.IsFixedSize);
		}

		[Test]
		public void IsReadOnly()
		{
			Assert.IsFalse(this._list.IsReadOnly);
		}

		[Test]
		public new void SetItem()
		{
			base.SetItem();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public new void SetItemNegativeIndex()
		{
			base.SetItemNegativeIndex();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public new void SetItemPastTheEnd()
		{
			base.SetItemPastTheEnd();
		}


		[Test]
		public new void Add()
		{
			base.Add();
		}

		[Test]
		public new void Clear()
		{
			base.Clear();
		}

		[Test]
		public new void Insert()
		{
			base.Insert();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public new void InsertNegativeIndex()
		{
			base.InsertNegativeIndex();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public new void InsertPastTheEnd()
		{
			base.InsertPastTheEnd();
		}

		[Test]
		public new void Remove()
		{
			base.Remove();
		}

		[Test]
		public new void RemoveAt()
		{
			base.RemoveAt();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public new void RemoveAtNegativeIndex()
		{
			base.RemoveAtNegativeIndex();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public new void RemoveAtPastTheEnd()
		{
			base.RemoveAtPastTheEnd();
		}

	}

	public class IListVariableSizeReadOnlyBaseTest<T> : IListBaseTest<T>
	{
		[Test]
		public void IsFixedSize()
		{
			Assert.IsFalse(this._list.IsFixedSize);
		}

		[Test]
		public void IsReadOnly()
		{
			Assert.IsTrue(this._list.IsReadOnly);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void SetItem()
		{
			base.SetItem();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Add()
		{
			base.Add();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Clear()
		{
			base.Clear();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Insert()
		{
			base.Insert();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Remove()
		{
			base.Remove();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void RemoveAt()
		{
			base.RemoveAt();
		}
	}

	public class IListFixedSizeReadWriteBaseTest<T> : IListBaseTest<T>
	{
		[Test]
		public void IsFixedSize()
		{
			Assert.IsTrue(this._list.IsFixedSize);
		}

		[Test]
		public void IsReadOnly()
		{
			Assert.IsFalse(this._list.IsReadOnly);
		}
		[Test]
		public new void SetItem()
		{
			base.SetItem();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public new void SetItemNegativeIndex()
		{
			base.SetItemNegativeIndex();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public new void SetItemPastTheEnd()
		{
			base.SetItemPastTheEnd();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Add()
		{
			base.Add();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Clear()
		{
			base.Clear();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Insert()
		{
			base.Insert();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Remove()
		{
			base.Remove();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void RemoveAt()
		{
			base.RemoveAt();
		}
	}

	public class IListFixedSizeReadOnlyBaseTest<T> : IListBaseTest<T>
	{
		[Test]
		public void IsFixedSize()
		{
			Assert.IsTrue(this._list.IsFixedSize);
		}

		[Test]
		public void IsReadOnly()
		{
			Assert.IsTrue(this._list.IsReadOnly);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void SetItem()
		{
			base.SetItem();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Add()
		{
			base.Add();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Clear()
		{
			base.Clear();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Insert()
		{
			base.Insert();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void Remove()
		{
			base.Remove();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void RemoveAt()
		{
			base.RemoveAt();
		}

	}



	[TestFixture]
	public class IListTest : IListVariableSizeReadWriteBaseTest<int>
	{
		[SetUp]
		public void SetUp()
		{
			List<int> list = new List<int>();

			list.Add(1);
			list.Add(3);
			list.Add(5);
			list.Add(7);
			list.Add(9);

			this._list = list;
			this._firstItem = 1;
			this._newItem = 11;
		}
	}

	[TestFixture]
	public class IListStringTest : IListVariableSizeReadWriteBaseTest<string>
	{
		[SetUp]
		public void SetUp()
		{
			List<string> list = new List<string>();

			list.Add("1");
			list.Add("3");
			list.Add("5");
			list.Add("7");
			list.Add("9");

			this._list = list;
			this._firstItem = "1";
			this._newItem = "11";
		}
	}

	[TestFixture]
	public class IListNoDataTest : IListVariableSizeReadWriteBaseTest<string>
	{
		[SetUp]
		public void SetUp()
		{
			this._list = new List<string>();
			this._firstItem = null;
			this._newItem = "11";
		}
	}

	[TestFixture]
	public class IListFixedSizeReadOnlyTest : IListFixedSizeReadOnlyBaseTest<string>
	{
		[SetUp]
		public void SetUp()
		{
			ArrayList list = new ArrayList();

			list.Add("1");
			list.Add("3");
			list.Add("5");
			list.Add("7");
			list.Add("9");

			this._list = ArrayList.ReadOnly(list);
			this._firstItem = "1";
			this._newItem = "11";
		}
	}

	[TestFixture]
	public class IListFixedSizeReadWriteTest : IListFixedSizeReadWriteBaseTest<string>
	{
		[SetUp]
		public void SetUp()
		{
			ArrayList list = new ArrayList();

			list.Add("1");
			list.Add("3");
			list.Add("5");
			list.Add("7");
			list.Add("9");

			this._list = ArrayList.FixedSize(list);
			this._firstItem = "1";
			this._newItem = "11";
		}
	}


}
