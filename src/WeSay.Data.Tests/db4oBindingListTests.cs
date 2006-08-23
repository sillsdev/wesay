using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;



namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oBindingListIEnumerableTests : IEnumerableBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _bindingList;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{

			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._bindingList = new Db4oBindingList<TestItem>(this._dataSource);

			this._enumerable = this._bindingList;
			this._bindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._bindingList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oBindingListIEnumerableWithNoDataTests : IEnumerableBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _bindingList;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{

			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._bindingList = new Db4oBindingList<TestItem>(this._dataSource);

			this._enumerable = this._bindingList;
			this._itemCount = 0;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._bindingList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}


	[TestFixture]
	public class Db4oBindingListICollectionTest : ICollectionBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _bindingList;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._bindingList = new Db4oBindingList<TestItem>(this._dataSource);

			this._collection = this._bindingList;
			this._bindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._bindingList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}


	[TestFixture]
	public class Db4oBindingListICollectionWithNoDataTest : ICollectionBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _bindingList;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._bindingList = new Db4oBindingList<TestItem>(this._dataSource);

			this._collection = this._bindingList;
			this._itemCount = 0;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._bindingList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oBindingListIListTest : IListVariableSizeReadWriteBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _bindingList;
		string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._bindingList = new Db4oBindingList<TestItem>(this._dataSource);

			this._list = this._bindingList;
			TestItem firstItem = new TestItem("Jared", 1, new DateTime(2003, 7, 10));
			this._bindingList.Add(firstItem);
			this._bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._firstItem = firstItem;
			this._newItem = new TestItem();
			this._isSorted = true;
		}

		[TearDown]
		public void TearDown()
		{

			this._bindingList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

	}

	[TestFixture]
	public class Db4oBindingListIListWithNoDataTest : IListVariableSizeReadWriteBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _bindingList;
		string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._bindingList = new Db4oBindingList<TestItem>(this._dataSource);

			this._list = this._bindingList;
			this._firstItem = null;
			this._newItem = new TestItem();
			this._isSorted = true;
		}

		[TearDown]
		public void TearDown()
		{
			this._bindingList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

	}

	[TestFixture]
	public class Db4oBindingListIBindingListTest : IBindingListBaseTest<TestItem, int>
	{

		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _db4oBindingList;
		string _FilePath;


		[SetUp]
		public override void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._db4oBindingList = new Db4oBindingList<TestItem>(this._dataSource);
			this._bindingList = this._db4oBindingList;

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			this._property = pdc.Find("StoredInt", false);

			this._newItem = new TestItem();
			this._key = 1;
			base.SetUp();

			this._db4oBindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._db4oBindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this.ResetListChanged();
		}

		[TearDown]
		public void TearDown()
		{
			((IDisposable)this._bindingList).Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		protected override void VerifySort()
		{
			base.VerifySort();
			Assert.AreEqual(1, _db4oBindingList[0].StoredInt);
			Assert.AreEqual(2, _db4oBindingList[1].StoredInt);
		}

		protected override void VerifyUnsorted()
		{
			base.VerifyUnsorted();
			//Assert.AreEqual(2, _db4oBindingList[0].StoredInt);
			//Assert.AreEqual(1, _db4oBindingList[1].StoredInt);
		}
	}

	[TestFixture]
	public class Db4oBindingListIBindingListWithNoDataTest : IBindingListBaseTest<TestItem, int>
	{

		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _db4oBindingList;
		string _FilePath;

		[SetUp]
		public override void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._db4oBindingList = new Db4oBindingList<TestItem>(this._dataSource);
			this._bindingList = this._db4oBindingList;

			this._newItem = new TestItem();
			this._key = 1;
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			this._property = pdc.Find("StoredInt", false);
			base.SetUp();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			((IDisposable)this._bindingList
	).Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

	}


	[TestFixture]
	public class Db4oBindingListSortedTest
	{

		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _bindingList;
		string _filePath;
		TestItem _jared, _gianna, _eric, _allison;


		private bool _listChanged;
		private ListChangedEventArgs _listChangedEventArgs;

		public void _adaptor_ListChanged(object sender, ListChangedEventArgs e)
		{
			_listChanged = true;
			_listChangedEventArgs = e;
		}

		private void AssertListChanged()
		{
			Assert.IsTrue(_listChanged);
			Assert.AreEqual(ListChangedType.Reset, this._listChangedEventArgs.ListChangedType);
			ResetListChanged();
		}
		private void ResetListChanged()
		{
			this._listChanged = false;
			this._listChangedEventArgs = null;
		}

		[SetUp]
		public void SetUp()
		{
			_filePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_filePath);
			this._bindingList = new Db4oBindingList<TestItem>(this._dataSource);
			this._bindingList.ListChanged += new ListChangedEventHandler(_adaptor_ListChanged);

			_eric = new TestItem("Eric", 1, new DateTime(2006, 2, 28));
			_allison = new TestItem("Allison", 2, new DateTime(2006, 1, 08));
			_jared = new TestItem("Jared", 3, new DateTime(2006, 7, 10));
			_gianna = new TestItem("Gianna", 4, new DateTime(2006, 7, 17));
			this._bindingList.Add(_jared);
			this._bindingList.Add(_gianna);
			this._bindingList.Add(_eric);
			this._bindingList.Add(_allison);
			ResetListChanged();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			this._bindingList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_filePath);
		}

		[Test]
		public void SortInt()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredInt", false);
			((IBindingList)this._bindingList).ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_eric, this._bindingList[0]);
			Assert.AreEqual(_allison, this._bindingList[1]);
			Assert.AreEqual(_jared, this._bindingList[2]);
			Assert.AreEqual(_gianna, this._bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDate()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			((IBindingList)this._bindingList).ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_allison, this._bindingList[0]);
			Assert.AreEqual(_eric, this._bindingList[1]);
			Assert.AreEqual(_jared, this._bindingList[2]);
			Assert.AreEqual(_gianna, this._bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortString()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredString", false);
			((IBindingList)this._bindingList).ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_allison, this._bindingList[0]);
			Assert.AreEqual(_eric, this._bindingList[1]);
			Assert.AreEqual(_gianna, this._bindingList[2]);
			Assert.AreEqual(_jared, this._bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void ChangeSort()
		{
			SortInt();
			SortDate();
			SortString();
		}
	}

	[TestFixture]
	public class Db4oBindingListUpdatesToDataTest
	{
		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _bindingList;
		string _filePath;
		TestItem _jared, _gianna;

		[SetUp]
		public void SetUp()
		{
			_filePath = System.IO.Path.GetTempFileName();
			OpenDatabase();

			_jared = new TestItem("Jared", 3, new DateTime(2006, 7, 10));
			_gianna = new TestItem("Gianna", 4, new DateTime(2006, 7, 17));
			this._bindingList.Add(_jared);
			this._bindingList.Add(_gianna);
		}

		[TearDown]
		public void TearDown()
		{
			this._bindingList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_filePath);
		}

		[Test]
		public void ChangeAfterAdd()
		{
			_jared.StoredInt = 1;

			Assert.AreEqual(_jared, _bindingList[0]);
			Assert.AreEqual(1, _bindingList[0].StoredInt);

			ReOpenDatabase();

			Assert.AreEqual(_jared, _bindingList[0]);
			Assert.AreEqual(1, _bindingList[0].StoredInt);
		}

		[Test]
		public void ChangeAfterClosed()
		{
			Assert.IsTrue(_bindingList.Contains(_jared));
			ReOpenDatabase();
			Assert.IsFalse(_bindingList.Contains(_jared)); // should no longer contain this instance
			Assert.AreEqual(_jared, _bindingList[0]);
			_jared.StoredInt = 1; // should not be wired up to throw events to old bindinglist anymore
			Assert.AreNotEqual(_jared, _bindingList[0]);
		}

		[Test]
		public void ChangeAfterReopen()
		{
			ReOpenDatabase();
			Assert.AreEqual(3, _bindingList[0].StoredInt);

			TestItem item = _bindingList[0];
			item.StoredInt = 1;
			Assert.AreEqual(1, _bindingList[0].StoredInt);

			ReOpenDatabase();

			Assert.AreEqual(1, _bindingList[0].StoredInt);
		}

		[Test]
		public void ChangeAfterGet()
		{
			TestItem item = _bindingList[0];
			item.StoredInt = 1;
			item = _bindingList[1];
			item.StoredInt = 2;

			ReOpenDatabase();

			Assert.AreEqual(1, _bindingList[0].StoredInt);
			Assert.AreEqual(2, _bindingList[1].StoredInt);
		}

		[Test]
		public void ChangeAfterAddNew()
		{
			TestItem item = (TestItem)((IBindingList)_bindingList).AddNew();
			int index = _bindingList.IndexOf(item);
			Assert.AreEqual(_bindingList.Count - 1, index);
			Assert.AreEqual(0, item.StoredInt);
			item.StoredInt = 11;

			ReOpenDatabase();
			Assert.IsFalse(_bindingList.Contains(item)); // when the database is reopened, the link to the database is lost
			Assert.AreEqual(item, _bindingList[0]);
			Assert.AreEqual(11, _bindingList[0].StoredInt);
		}

		private void ReOpenDatabase()
		{
			this._bindingList.Dispose();
			this._dataSource.Dispose();
			OpenDatabase();
		}

		private void OpenDatabase()
		{
			this._dataSource = new Db4oDataSource(_filePath);
			this._bindingList = new Db4oBindingList<TestItem>(this._dataSource);
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			this._bindingList.ApplySort(pd, ListSortDirection.Descending);
		}

		private int TestItemDepth(TestItem item)
		{
			int depth = 0;
			while (item != null)
			{
				++depth;
				item = item.ChildTestItem;
			}
			return depth;
		}

		[Test]
		public void NestedStructure()
		{
			const int items = 10;
			this._bindingList.Clear();

			TestItem root = new TestItem("Root Item", 0, DateTime.Now);
			TestItem parent = root;
			for (int i = 1; i < items; ++i)
			{
				TestItem child = new TestItem("Child Item", i, DateTime.Now);
				parent.ChildTestItem = child;
				parent = child;
			}

			Assert.AreEqual(items, TestItemDepth(root));
			this._bindingList.Add(root);
			Assert.AreEqual(root, this._bindingList[0]);
			Assert.AreEqual(items, TestItemDepth(this._bindingList[0]));

			ReOpenDatabase();

			Assert.AreEqual(items, TestItemDepth(this._bindingList[0]));
		}
	}
}
