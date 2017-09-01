using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	public abstract class IListBaseTest<T>
	{
		protected IList _list;
		protected T _firstItem;
		protected T _newItem;
		protected bool _isSorted;

		[SetUp]
		public virtual void SetUp() {}

		[Test]
		public void GetItem()
		{
			if (_list.Count > 0)
			{
				Assert.AreEqual(_firstItem, _list[0]);
			}
		}

		[Test]
		public void GetItemNegativeIndex()
		{
			T item;
			Assert.Throws<ArgumentOutOfRangeException>(() =>  item = (T) _list[-1]);
		}

		[Test]
		public void GetItemPastTheEnd()
		{
			T item;
			Assert.Throws<ArgumentOutOfRangeException>(() =>  item =(T) _list[_list.Count]);
		}

		[Test]
		public void Contains()
		{
			Assert.IsFalse(_list.Contains(default(T)));
			if (_list.Count > 0)
			{
				Assert.IsTrue(_list.Contains(_firstItem));
			}
		}

		[Test]
		public void IndexOf()
		{
			Assert.AreEqual(-1, _list.IndexOf(default(T)));
			if (_list.Count > 0)
			{
				Assert.AreEqual(0, _list.IndexOf(_firstItem));
				Assert.AreEqual(_list.Count - 1, _list.IndexOf(_list[_list.Count - 1]));
			}
		}

		#region Read-Write

		public void SetItem()
		{
			if (!_isSorted)
			{
				if (_list.Count == 0)
				{
					_list.Add(_newItem);
					_list.Add(_firstItem);
					_list.Add(_newItem);
				}
				Assert.AreNotEqual(_newItem, _list[1]);
				_list[1] = _newItem;
				Assert.AreEqual(_newItem, _list[1]);
			}
		}

		public void SetItemNotSupported()
		{
			if (_isSorted)
			{
				_list[1] = _firstItem;
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
				_list[-1] = _firstItem;
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
				_list[_list.Count] = _firstItem;
			}
		}

		#endregion

		#region Variable-Size

		public void Add()
		{
			int count = _list.Count;
			_list.Add(_newItem);
			Assert.AreEqual(count + 1, _list.Count);
			Assert.AreEqual(_newItem, _list[count]);
		}

		public void Clear()
		{
			_list.Clear();
			Assert.AreEqual(0, _list.Count);
		}

		public void Insert()
		{
			if (!_isSorted)
			{
				int count = _list.Count;
				_list.Insert(0, _newItem);
				Assert.AreEqual(count + 1, _list.Count);
				Assert.AreEqual(_newItem, _list[0]);
				if (count > 0)
				{
					Assert.AreEqual(_firstItem, _list[1]);
					Assert.AreNotEqual(_firstItem, _list[2]);
					Assert.AreNotEqual(_newItem, _list[_list.Count - 1]);
				}
				_list.Insert(_list.Count, _newItem);
				Assert.AreEqual(count + 2, _list.Count);
				Assert.AreEqual(_newItem, _list[_list.Count - 1]);
			}
		}

		public void InsertNotSupported()
		{
			if (_isSorted)
			{
				_list.Insert(0, _newItem);
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
				_list.Insert(-1, _newItem);
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
				_list.Insert(_list.Count + 1, _newItem);
			}
		}

		public void Remove()
		{
			_list.Add(_newItem);
			int count = _list.Count;
			_list.Remove(_newItem);
			Assert.AreEqual(count - 1, _list.Count);
			if (_list.Count > 0)
			{
				count = _list.Count;
				_list.Remove(_firstItem);
				Assert.AreEqual(count - 1, _list.Count);
			}
			if (_list.Count > 0)
			{
				Assert.AreNotEqual(_firstItem, _list[0]);
			}

			count = _list.Count;
			_list.Remove(_firstItem);
			Assert.AreEqual(count, _list.Count);
		}

		public void RemoveAt()
		{
			int count = _list.Count;
			if (count == 0)
			{
				_list.Add(_newItem);
				count = 1;
			}
			_list.RemoveAt(0);
			if (_list.Count > 0)
			{
				Assert.AreNotEqual(_newItem, _list[0]);
			}
			Assert.AreEqual(count - 1, _list.Count);
		}

		public void RemoveAtNegativeIndex()
		{
			_list.RemoveAt(-1);
		}

		public void RemoveAtPastTheEnd()
		{
			_list.RemoveAt(_list.Count);
		}

		#endregion
	}

	public abstract class IListVariableSizeReadWriteBaseTest<T>: IListBaseTest<T>
	{
		[Test]
		public void IsFixedSize()
		{
			Assert.IsFalse(_list.IsFixedSize);
		}

		[Test]
		public void IsReadOnly()
		{
			Assert.IsFalse(_list.IsReadOnly);
		}

		[Test]
		public new void SetItem()
		{
			base.SetItem();
		}

		[Test]
		public new void SetItemNegativeIndex()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => base.SetItemNegativeIndex());
		}

		[Test]
		public new void SetItemPastTheEnd()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => base.SetItemPastTheEnd());
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
		public new void InsertNegativeIndex()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => base.InsertNegativeIndex());
		}

		[Test]
		public new void InsertPastTheEnd()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => base.InsertPastTheEnd());
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
		public new void RemoveAtNegativeIndex()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => base.RemoveAtNegativeIndex());
		}

		[Test]
		public new void RemoveAtPastTheEnd()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => base.RemoveAtPastTheEnd());
		}
	}

	public abstract class IListVariableSizeReadOnlyBaseTest<T>: IListBaseTest<T>
	{
		[Test]
		public void IsFixedSize()
		{
			Assert.IsFalse(_list.IsFixedSize);
		}

		[Test]
		public void IsReadOnly()
		{
			Assert.IsTrue(_list.IsReadOnly);
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof (NotSupportedException))]
		public new void SetItem()
		{
			base.SetItem();
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof (NotSupportedException))]
		public new void Add()
		{
			base.Add();
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof (NotSupportedException))]
		public new void Clear()
		{
			base.Clear();
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof (NotSupportedException))]
		public new void Insert()
		{
			base.Insert();
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof (NotSupportedException))]
		public new void Remove()
		{
			base.Remove();
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof (NotSupportedException))]
		public new void RemoveAt()
		{
			base.RemoveAt();
		}
	}

	public abstract class IListFixedSizeReadWriteBaseTest<T>: IListBaseTest<T>
	{
		[Test]
		public void IsFixedSize()
		{
			Assert.IsTrue(_list.IsFixedSize);
		}

		[Test]
		public void IsReadOnly()
		{
			Assert.IsFalse(_list.IsReadOnly);
		}

		[Test]
		public new void SetItem()
		{
			base.SetItem();
		}

		[Test]
		public new void SetItemNegativeIndex()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>  base.SetItemNegativeIndex());
		}

		[Test]
		public new void SetItemPastTheEnd()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>  base.SetItemPastTheEnd());
		}

		[Test]
		public new void Add()
		{
			Assert.Throws<NotSupportedException>(() =>  base.Add());
		}

		[Test]
		public new void Clear()
		{
			Assert.Throws<NotSupportedException>(() => base.Clear());
		}

		[Test]
		public new void Insert()
		{
			Assert.Throws<NotSupportedException>(() => base.Insert());
		}

		[Test]
		public new void Remove()
		{
			Assert.Throws<NotSupportedException>(() => base.Remove());
		}

		[Test]
		public new void RemoveAt()
		{
			Assert.Throws<NotSupportedException>(() => base.RemoveAt());
		}
	}

	public abstract class IListFixedSizeReadOnlyBaseTest<T>: IListBaseTest<T>
	{
		[Test]
		public void IsFixedSize()
		{
			Assert.IsTrue(_list.IsFixedSize);
		}

		[Test]
		public void IsReadOnly()
		{
			Assert.IsTrue(_list.IsReadOnly);
		}

		[Test]
		public new void SetItem()
		{
			Assert.Throws<NotSupportedException>(() => base.SetItem());
		}

		[Test]
		public new void Add()
		{
			Assert.Throws<NotSupportedException>(() => base.Add());
		}

		[Test]
		public new void Clear()
		{
			Assert.Throws<NotSupportedException>(() => base.Clear());
		}

		[Test]
		public new void Insert()
		{
			Assert.Throws<NotSupportedException>(() => base.Insert());
		}

		[Test]
		public new void Remove()
		{
			Assert.Throws<NotSupportedException>(() => base.Remove());
		}

		[Test]
		public new void RemoveAt()
		{
			Assert.Throws<NotSupportedException>(() => base.RemoveAt());
		}
	}

	[TestFixture]
	public class IListTest: IListVariableSizeReadWriteBaseTest<int>
	{
		[SetUp]
		public override void SetUp()
		{
			List<int> list = new List<int>();

			list.Add(1);
			list.Add(3);
			list.Add(5);
			list.Add(7);
			list.Add(9);

			_list = list;
			_firstItem = 1;
			_newItem = 11;
		}
	}

	[TestFixture]
	public class IListStringTest: IListVariableSizeReadWriteBaseTest<string>
	{
		[SetUp]
		public override void SetUp()
		{
			List<string> list = new List<string>();

			list.Add("1");
			list.Add("3");
			list.Add("5");
			list.Add("7");
			list.Add("9");

			_list = list;
			_firstItem = "1";
			_newItem = "11";
		}
	}

	[TestFixture]
	public class IListNoDataTest: IListVariableSizeReadWriteBaseTest<string>
	{
		[SetUp]
		public override void SetUp()
		{
			_list = new List<string>();
			_firstItem = null;
			_newItem = "11";
		}
	}

	[TestFixture]
	public class IListFixedSizeReadOnlyTest: IListFixedSizeReadOnlyBaseTest<string>
	{
		[SetUp]
		public override void SetUp()
		{
			ArrayList list = new ArrayList();

			list.Add("1");
			list.Add("3");
			list.Add("5");
			list.Add("7");
			list.Add("9");

			_list = ArrayList.ReadOnly(list);
			_firstItem = "1";
			_newItem = "11";
		}
	}

	[TestFixture]
	public class IListFixedSizeReadWriteTest: IListFixedSizeReadWriteBaseTest<string>
	{
		[SetUp]
		public override void SetUp()
		{
			ArrayList list = new ArrayList();

			list.Add("1");
			list.Add("3");
			list.Add("5");
			list.Add("7");
			list.Add("9");

			_list = ArrayList.FixedSize(list);
			_firstItem = "1";
			_newItem = "11";
		}
	}
}