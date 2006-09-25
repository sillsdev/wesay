using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using WeSay.Data.Tests.IEnumerableTests;
using WeSay.Data.Tests.ICollectionTests;
using WeSay.Data.Tests.IListTests;
using WeSay.Data.Tests.IBindingListTests;

namespace WeSay.Data.Tests.Db4oRecordListTests
{
	[TestFixture]
	public class Db4oRecordListIEnumerableTests : IEnumerableBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _recordList;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._recordList = new Db4oRecordList<TestItem>(this._dataSource);

			this._enumerable = this._recordList;
			this._recordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._recordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListIEnumerableWithNoDataTests : IEnumerableBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _recordList;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._recordList = new Db4oRecordList<TestItem>(this._dataSource);

			this._enumerable = this._recordList;
			this._itemCount = 0;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}


	[TestFixture]
	public class Db4oRecordListICollectionTest : ICollectionBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _recordList;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._recordList = new Db4oRecordList<TestItem>(this._dataSource);

			this._collection = this._recordList;
			this._recordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._recordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}


	[TestFixture]
	public class Db4oRecordListICollectionWithNoDataTest : ICollectionBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _recordList;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._recordList = new Db4oRecordList<TestItem>(this._dataSource);

			this._collection = this._recordList;
			this._itemCount = 0;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListIListTest : IListVariableSizeReadWriteBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _recordList;
		string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._recordList = new Db4oRecordList<TestItem>(this._dataSource);

			this._list = this._recordList;
			TestItem firstItem = new TestItem("Jared", 1, new DateTime(2003, 7, 10));
			this._recordList.Add(firstItem);
			this._recordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._firstItem = firstItem;
			this._newItem = new TestItem();
			this._isSorted = true;
		}

		[TearDown]
		public void TearDown()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

	}

	[TestFixture]
	public class Db4oRecordListIListWithNoDataTest : IListVariableSizeReadWriteBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _recordList;
		string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._recordList = new Db4oRecordList<TestItem>(this._dataSource);

			this._list = this._recordList;
			this._firstItem = null;
			this._newItem = new TestItem();
			this._isSorted = true;
		}

		[TearDown]
		public void TearDown()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

	}

	[TestFixture]
	public class Db4oRecordListIBindingListTest : IBindingListBaseTest<TestItem, int>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _recordList;
		string _FilePath;

		[SetUp]
		public override void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._recordList = new Db4oRecordList<TestItem>(this._dataSource);
			this._bindingList = this._recordList;

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			this._property = pdc.Find("StoredInt", false);

			this._newItem = new TestItem();
			this._key = 1;
			base.SetUp();

			this._recordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._recordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this.ResetListChanged();
		}

		[TearDown]
		public void TearDown()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		protected override void VerifySortAscending()
		{
			Assert.AreEqual(1, _recordList[0].StoredInt);
			Assert.AreEqual(2, _recordList[1].StoredInt);
			base.VerifySortAscending();
		}

		protected override void VerifySortDescending()
		{
			Assert.AreEqual(2, _recordList[0].StoredInt);
			Assert.AreEqual(1, _recordList[1].StoredInt);
			base.VerifySortDescending();
		}

		protected override void VerifyUnsorted()
		{
			base.VerifyUnsorted();
		}
	}

	[TestFixture]
	public class Db4oRecordListIBindingListWithNoDataTest : IBindingListBaseTest<TestItem, int>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _db4oRecordList;
		string _FilePath;

		[SetUp]
		public override void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._db4oRecordList = new Db4oRecordList<TestItem>(this._dataSource);
			this._bindingList = this._db4oRecordList;

			this._newItem = new TestItem();
			this._key = 1;
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			this._property = pdc.Find("StoredInt", false);
			base.SetUp();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			this._db4oRecordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListIRecordListTest : IRecordListBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _db4oRecordList;
		string _FilePath;

		[SetUp]
		public override void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._db4oRecordList = new Db4oRecordList<TestItem>(this._dataSource);
			this._recordList = this._db4oRecordList;

			this._changedFieldName = "StoredInt";
			base.SetUp();

			this._db4oRecordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._db4oRecordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this.ResetListChanged();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			this._db4oRecordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		protected override void Change(TestItem item)
		{
			item.StoredInt++;
		}
	}

	[TestFixture]
	public class Db4oRecordListIRecordListWithNoDataTest : IRecordListBaseTest<TestItem>
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _db4oRecordList;
		string _FilePath;

		[SetUp]
		public override void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._db4oRecordList = new Db4oRecordList<TestItem>(this._dataSource);
			this._recordList = this._db4oRecordList;
			this._changedFieldName = "StoredInt";
			base.SetUp();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			this._db4oRecordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		protected override void Change(TestItem item)
		{
			item.StoredInt++;
		}
	}

	[TestFixture]
	public class Db4oRecordListSortedTest
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _recordList;
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
			this._recordList = new Db4oRecordList<TestItem>(this._dataSource);
			this._recordList.ListChanged += new ListChangedEventHandler(_adaptor_ListChanged);

			_eric = new TestItem("Eric", 1, new DateTime(2006, 2, 28));
			_allison = new TestItem("Allison", 2, new DateTime(2006, 1, 08));
			_jared = new TestItem("Jared", 3, new DateTime(2006, 7, 10));
			_gianna = new TestItem("Gianna", 4, new DateTime(2006, 7, 17));
			this._recordList.Add(_jared);
			this._recordList.Add(_gianna);
			this._recordList.Add(_eric);
			this._recordList.Add(_allison);
			ResetListChanged();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_filePath);
		}

		[Test]
		public void SortIntAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredInt", false);
			((IBindingList)this._recordList).ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_eric, this._recordList[0]);
			Assert.AreEqual(_allison, this._recordList[1]);
			Assert.AreEqual(_jared, this._recordList[2]);
			Assert.AreEqual(_gianna, this._recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortIntDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredInt", false);
			((IBindingList)this._recordList).ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_gianna, this._recordList[0]);
			Assert.AreEqual(_jared, this._recordList[1]);
			Assert.AreEqual(_allison, this._recordList[2]);
			Assert.AreEqual(_eric, this._recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDateAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			((IBindingList)this._recordList).ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_allison, this._recordList[0]);
			Assert.AreEqual(_eric, this._recordList[1]);
			Assert.AreEqual(_jared, this._recordList[2]);
			Assert.AreEqual(_gianna, this._recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDateDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			((IBindingList)this._recordList).ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_gianna, this._recordList[0]);
			Assert.AreEqual(_jared, this._recordList[1]);
			Assert.AreEqual(_eric, this._recordList[2]);
			Assert.AreEqual(_allison, this._recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortStringAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredString", false);
			((IBindingList)this._recordList).ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_allison, this._recordList[0]);
			Assert.AreEqual(_eric, this._recordList[1]);
			Assert.AreEqual(_gianna, this._recordList[2]);
			Assert.AreEqual(_jared, this._recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortStringDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredString", false);
			((IBindingList)this._recordList).ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_jared, this._recordList[0]);
			Assert.AreEqual(_gianna, this._recordList[1]);
			Assert.AreEqual(_eric, this._recordList[2]);
			Assert.AreEqual(_allison, this._recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void ChangeSort()
		{
			SortIntAscending();
			SortDateDescending();
		}
	}

	[TestFixture]
	public class Db4oRecordListUpdatesToDataTest
	{
		Db4oDataSource _dataSource;
		Db4oRecordList<TestItem> _recordList;
		string _filePath;
		TestItem _jared, _gianna;

		[SetUp]
		public void SetUp()
		{
			_filePath = System.IO.Path.GetTempFileName();
			OpenDatabase();

			_jared = new TestItem("Jared", 3, new DateTime(2006, 7, 10));
			_gianna = new TestItem("Gianna", 4, new DateTime(2006, 7, 17));
			this._recordList.Add(_jared);
			this._recordList.Add(_gianna);
		}

		[TearDown]
		public void TearDown()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_filePath);
		}

		[Test]
		public void ChangeAfterAdd()
		{
			_jared.StoredInt = 1;

			Assert.AreEqual(_jared, _recordList[0]);
			Assert.AreEqual(1, _recordList[0].StoredInt);

			ReOpenDatabase();

			Assert.AreEqual(_jared, _recordList[0]);
			Assert.AreEqual(1, _recordList[0].StoredInt);
		}

		[Test]
		public void ChangeAfterClosed()
		{
			Assert.IsTrue(_recordList.Contains(_jared));
			ReOpenDatabase();
			Assert.IsFalse(_recordList.Contains(_jared)); // should no longer contain this instance
			Assert.AreEqual(_jared, _recordList[0]);
			_jared.StoredInt = 1; // should not be wired up to throw events to old bindinglist anymore
			Assert.AreNotEqual(_jared, _recordList[0]);
		}

		[Test]
		public void ChangeAfterReopen()
		{
			ReOpenDatabase();
			Assert.AreEqual(3, _recordList[0].StoredInt);

			TestItem item = _recordList[0];
			item.StoredInt = 1;
			Assert.AreEqual(1, _recordList[0].StoredInt);

			ReOpenDatabase();

			Assert.AreEqual(1, _recordList[0].StoredInt);
		}

		[Test]
		public void ChangeAfterGet()
		{
			TestItem item = _recordList[0];
			item.StoredInt = 1;
			item = _recordList[1];
			item.StoredInt = 2;

			ReOpenDatabase();

			Assert.AreEqual(1, _recordList[0].StoredInt);
			Assert.AreEqual(2, _recordList[1].StoredInt);
		}

		[Test]
		public void ChangeAfterAddNew()
		{
			TestItem item = (TestItem)((IBindingList)_recordList).AddNew();
			int index = _recordList.IndexOf(item);
			Assert.AreEqual(_recordList.Count - 1, index);
			Assert.AreEqual(0, item.StoredInt);
			item.StoredInt = 11;

			ReOpenDatabase();
			Assert.IsFalse(_recordList.Contains(item)); // when the database is reopened, the link to the database is lost
			Assert.AreEqual(item, _recordList[0]);
			Assert.AreEqual(11, _recordList[0].StoredInt);
		}

		private void ReOpenDatabase()
		{
			this._recordList.Dispose();
			this._dataSource.Dispose();
			OpenDatabase();
		}

		private void OpenDatabase()
		{
			this._dataSource = new Db4oDataSource(_filePath);
			this._recordList = new Db4oRecordList<TestItem>(this._dataSource);
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			this._recordList.ApplySort(pd, ListSortDirection.Ascending);
		}

		[Test]
		[Ignore("By enforcing this, the performance goes way down. We get around this by calling activate in the OnActivateDepth method of our data classes.")]
		public void NestedStructure()
		{
			const int items = 10;
			this._recordList.Clear();

			TestItem root = new TestItem("Root Item", 0, DateTime.Now);
			root.Child = new ChildTestItem("Child Item", 1, DateTime.Now);

			ChildTestItem parent = root.Child;
			for (int i = 2; i < items; ++i)
			{
				parent.Child = new ChildTestItem("Child Item", i, DateTime.Now);
				parent = parent.Child;
			}

			Assert.AreEqual(items, root.Depth);
			this._recordList.Add(root);
			Assert.AreEqual(1, _recordList.Count);
			Assert.AreEqual(root, this._recordList[0]);
			Assert.AreEqual(items, this._recordList[0].Depth);

			ReOpenDatabase();
			Assert.AreEqual(1, _recordList.Count);
			Assert.AreEqual(root, _recordList[0]);
			Assert.AreEqual(items, this._recordList[0].Depth);
		}

		[Test]
		[Ignore("Come back to this. Right now we get around this by calling activate in the OnActivateDepth method of our data classes.")]
		public void NestedStructureActivation()
		{
			const int items = 10;
			this._recordList.Clear();

			TestItem root = new TestItem("Root Item", 0, DateTime.Now);
			root.Child = new ChildTestItem("Child Item", 1, DateTime.Now);

			ChildTestItem parent = root.Child;
			for (int i = 2; i < items; ++i)
			{
				parent.Child = new ChildTestItem("Child Item", i, DateTime.Now);
				parent = parent.Child;
			}

			Assert.AreEqual(items, root.Depth);
			this._recordList.Add(root);

			ReOpenDatabase();
			Assert.AreEqual(items, this._recordList[0].OnActivateDepth);
		}
	}

	[TestFixture]
	public class Db4oRecordListTests
	{
		Db4oDataSource _dataSource;
		string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		[Test]
		public void CopyConstructor()
		{
			using (Db4oRecordList<SimpleIntTestClass> recordList = new Db4oRecordList<SimpleIntTestClass>(this._dataSource))
			{
				for (int i = 0; i < 50; i++)
				{
					recordList.Add(new SimpleIntTestClass(i));
				}

				using (Db4oRecordList<SimpleIntTestClass> recordListCopy = new Db4oRecordList<SimpleIntTestClass>(recordList))
				{
					Assert.IsNotNull(recordListCopy);
					Assert.AreNotSame(recordList, recordListCopy);
					Assert.AreEqual(recordList, recordListCopy);
				}
			}
		}
	}
}
