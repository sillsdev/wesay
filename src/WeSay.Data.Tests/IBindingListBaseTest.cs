using System;
using System.ComponentModel;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	public abstract class IBindingListBaseTest<T, K> where T : new()
	{
		protected IBindingList _bindingList;
		protected PropertyDescriptor _property;
		protected T _newItem;
		protected K _key;
		private bool _listChanged;
		private ListChangedEventArgs _listChangedEventArgs;

		protected void ResetListChanged()
		{
			_listChanged = false;
			_listChangedEventArgs = null;
		}

		public void _bindingList_ListChanged(object sender, ListChangedEventArgs e)
		{
			_listChanged = true;
			_listChangedEventArgs = e;
		}

		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp() {}

		[SetUp]
		public virtual void SetUp()
		{
			_bindingList.ListChanged += _bindingList_ListChanged;
			_listChanged = false;
			_listChangedEventArgs = null;
		}

		[Test]
		public void AddIndex()
		{
			_bindingList.AddIndex(_property); // can do nothing.
		}

		[Test]
		public void AddNew()
		{
			if (_bindingList.AllowNew)
			{
				Assert.IsFalse(_listChanged);
				int count = _bindingList.Count;
				object o = _bindingList.AddNew();
				Assert.IsNotNull(o);
				Assert.IsInstanceOfType(typeof (T), o);
				Assert.AreEqual(count + 1, _bindingList.Count);
				Assert.AreEqual(o, _bindingList[count]);
				if (_bindingList.SupportsChangeNotification)
				{
					Assert.IsTrue(_listChanged);
					Assert.IsTrue(_listChangedEventArgs.ListChangedType == ListChangedType.ItemAdded);
					Assert.AreEqual(count, _listChangedEventArgs.NewIndex);
					Assert.AreEqual(-1, _listChangedEventArgs.OldIndex);
				}
			}
			else
			{
				try
				{
					_bindingList.AddNew();
					Assert.Fail();
				}
				catch (NotSupportedException) {}
				catch
				{
					Assert.Fail("When AddNew is not supported, should throw not supported exception");
				}
			}
		}

		protected virtual void VerifySortAscending() {}

		protected virtual void VerifySortDescending() {}

		protected virtual void VerifySort(ListSortDirection direction)
		{
			if (direction == ListSortDirection.Ascending)
			{
				VerifySortAscending();
			}
			else
			{
				VerifySortDescending();
			}
			Assert.IsTrue(_bindingList.IsSorted);
		}

		[Test]
		public void ApplySortAscending()
		{
			ApplySort(ListSortDirection.Ascending);
		}

		[Test]
		public void ApplySortDescending()
		{
			ApplySort(ListSortDirection.Descending);
		}

		public void ApplySort(ListSortDirection direction)
		{
			if (_bindingList.SupportsSorting)
			{
				if (_property == null)
				{
					PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (T));
					_property = pdc[0];
				}
				Assert.IsFalse(_listChanged);
				VerifyUnsorted();

				_bindingList.ApplySort(_property, direction);
				if (_bindingList.Count > 1)
				{
					if (_bindingList.SupportsChangeNotification)
					{
						Assert.IsTrue(_listChanged);
						Assert.IsTrue(_listChangedEventArgs.ListChangedType == ListChangedType.Reset);
					}
					VerifySort(direction);
				}
				else
				{
					VerifyUnsorted();
				}
			}
			else
			{
				try
				{
					_bindingList.ApplySort(_property, direction);
					Assert.Fail();
				}
				catch (NotSupportedException) {}
				catch
				{
					Assert.Fail(
							"When Sorting is not supported, should throw not supported exception");
				}
			}
		}

		[Test]
		public void Find()
		{
			if (_bindingList.SupportsSearching)
			{
				Assert.IsNotNull(_property);
				int row = _bindingList.Find(_property, _key);
				if (_bindingList.Count > 1)
				{
					Assert.Greater(row, 0);
				}
				else
				{
					Assert.AreEqual(row, -1);
				}
			}
			else
			{
				try
				{
					_bindingList.Find(_property, _key);
					Assert.Fail();
				}
				catch (NotSupportedException) {}
				catch
				{
					Assert.Fail(
							"When Finding is not supported, should throw not supported exception");
				}
			}
		}

		[Test]
		public void RemoveIndex()
		{
			_bindingList.RemoveIndex(_property); // can do nothing
		}

		protected virtual void VerifyUnsorted()
		{
			Assert.IsFalse(_bindingList.IsSorted);
		}

		[Test]
		public void RemoveSort()
		{
			if (_bindingList.SupportsSorting)
			{
				Assert.IsFalse(_listChanged);
				Assert.IsFalse(_bindingList.IsSorted);
				_bindingList.RemoveSort();
				if (_bindingList.SupportsChangeNotification)
				{
					Assert.IsFalse(_listChanged);
				}
				if (_property == null)
				{
					PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (T));
					_property = pdc[0];
				}
				_bindingList.ApplySort(_property, ListSortDirection.Descending);
				ResetListChanged();
				_bindingList.RemoveSort();
				if (_bindingList.Count > 1)
				{
					if (_bindingList.SupportsChangeNotification)
					{
						Assert.IsTrue(_listChanged);
						Assert.IsTrue(_listChangedEventArgs.ListChangedType == ListChangedType.Reset);
					}
				}
				VerifyUnsorted();
			}
			else
			{
				try
				{
					_bindingList.RemoveSort();
					Assert.Fail();
				}
				catch (NotSupportedException) {}
				catch
				{
					Assert.Fail(
							"When Sorting is not supported, should throw not supported exception");
				}
			}
		}

		[Test]
		public void ListChangedOnAdd()
		{
			Assert.IsFalse(_listChanged);
			if (_bindingList.SupportsChangeNotification)
			{
				_bindingList.Add(_newItem);
				Assert.IsTrue(_listChanged);
				Assert.IsTrue(_listChangedEventArgs.ListChangedType == ListChangedType.ItemAdded);
				Assert.AreEqual(_bindingList.Count - 1, _listChangedEventArgs.NewIndex);
				Assert.AreEqual(-1, _listChangedEventArgs.OldIndex);
			}
		}

		[Test]
		public virtual void ListChangedOnChange()
		{
			Assert.IsFalse(_listChanged);
			if (_bindingList.SupportsChangeNotification)
			{
				if (_bindingList.Count == 0)
				{
					_bindingList.Add(_newItem);
					_listChanged = false;
					_listChangedEventArgs = null;
				}
				_bindingList[0] = new T();
				Assert.IsTrue(_listChanged);
				Assert.IsTrue(_listChangedEventArgs.ListChangedType == ListChangedType.ItemChanged);
				Assert.AreEqual(0, _listChangedEventArgs.NewIndex);
				Assert.AreEqual(-1, _listChangedEventArgs.OldIndex);
			}
		}

		[Test]
		public void ListChangedOnRemoveAt()
		{
			Assert.IsFalse(_listChanged);
			if (_bindingList.SupportsChangeNotification)
			{
				if (_bindingList.Count == 0)
				{
					_bindingList.Add(_newItem);
					_listChanged = false;
					_listChangedEventArgs = null;
				}
				_bindingList.RemoveAt(0);
				Assert.IsTrue(_listChanged);
				Assert.IsTrue(_listChangedEventArgs.ListChangedType == ListChangedType.ItemDeleted);
				Assert.AreEqual(-1, _listChangedEventArgs.OldIndex);
				Assert.AreEqual(0, _listChangedEventArgs.NewIndex);
			}
		}

		[Test]
		public void ListChangedOnRemove()
		{
			Assert.IsFalse(_listChanged);
			if (_bindingList.SupportsChangeNotification)
			{
				if (_bindingList.Count == 0)
				{
					_bindingList.Add(_newItem);
					_listChanged = false;
					_listChangedEventArgs = null;
				}
				_bindingList.Remove(_bindingList[0]);
				Assert.IsTrue(_listChanged);
				Assert.IsTrue(_listChangedEventArgs.ListChangedType == ListChangedType.ItemDeleted);
				Assert.AreEqual(-1, _listChangedEventArgs.OldIndex);
				Assert.AreEqual(0, _listChangedEventArgs.NewIndex);
			}
		}

		[Test]
		public void ListChangedOnClear()
		{
			Assert.IsFalse(_listChanged);
			if (_bindingList.SupportsChangeNotification)
			{
				if (_bindingList.Count == 0)
				{
					_bindingList.Add(_newItem);
					_listChanged = false;
					_listChangedEventArgs = null;
				}
				_bindingList.Clear();
				Assert.IsTrue(_listChanged);
				Assert.IsTrue(_listChangedEventArgs.ListChangedType == ListChangedType.Reset);
			}
		}
	}

	public class SimpleClass
	{
		private string _s;
		private int _i;

		public SimpleClass() {}

		public SimpleClass(string s, int i)
		{
			_s = s;
			_i = i;
		}

		public string String
		{
			get { return _s; }
			set { _s = value; }
		}

		public int Int
		{
			get { return _i; }
			set { _i = value; }
		}
	}

	[TestFixture]
	public class IBindingListStringTest: IBindingListBaseTest<SimpleClass, string>
	{
		[TestFixtureSetUp]
		public override void TestFixtureSetUp()
		{
			BindingList<SimpleClass> bindingList = new BindingList<SimpleClass>();

			bindingList.AddingNew += bindingList_AddingNew;

			bindingList.AllowEdit = true;
			bindingList.AllowNew = true;
			bindingList.AllowRemove = true;
			bindingList.RaiseListChangedEvents = true;
			bindingList.Add(new SimpleClass("1", 1));
			bindingList.Add(new SimpleClass("2", 2));
			bindingList.Add(new SimpleClass("3", 3));
			bindingList.Add(new SimpleClass("4", 4));
			bindingList.Add(new SimpleClass("5", 5));

			_bindingList = bindingList;
			_newItem = new SimpleClass("6", 6);
			_key = "1";
			_property = null;
		}

		private static void bindingList_AddingNew(object sender, AddingNewEventArgs e)
		{
			e.NewObject = new SimpleClass();
		}
	}

	[TestFixture]
	public class IBindingListNoDataTest: IBindingListBaseTest<SimpleClass, string>
	{
		[SetUp]
		public override void SetUp()
		{
			BindingList<SimpleClass> bindingList = new BindingList<SimpleClass>();

			bindingList.AddingNew += bindingList_AddingNew;

			bindingList.AllowEdit = true;
			bindingList.AllowNew = true;
			bindingList.AllowRemove = true;
			bindingList.RaiseListChangedEvents = true;

			_bindingList = bindingList;
			_newItem = new SimpleClass("6", 6);
			_key = "1";
			_property = null;

			base.SetUp();
		}

		private static void bindingList_AddingNew(object sender, AddingNewEventArgs e)
		{
			e.NewObject = new SimpleClass();
		}
	}
}