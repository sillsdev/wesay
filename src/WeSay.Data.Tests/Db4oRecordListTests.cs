using System;
using System.ComponentModel;
using System.IO;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oRecordListIEnumerableTests: IEnumerableBaseTest<TestItem>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _recordList;
		private string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_recordList = new Db4oRecordList<TestItem>(_dataSource);

			_enumerable = _recordList;
			_recordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			_recordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_itemCount = 2;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListIEnumerableWithNoDataTests: IEnumerableBaseTest<TestItem>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _recordList;
		private string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_recordList = new Db4oRecordList<TestItem>(_dataSource);

			_enumerable = _recordList;
			_itemCount = 0;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListICollectionTest: ICollectionBaseTest<TestItem>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _recordList;
		private string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_recordList = new Db4oRecordList<TestItem>(_dataSource);

			_collection = _recordList;
			_recordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			_recordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_itemCount = 2;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListICollectionWithNoDataTest: ICollectionBaseTest<TestItem>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _recordList;
		private string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_recordList = new Db4oRecordList<TestItem>(_dataSource);

			_collection = _recordList;
			_itemCount = 0;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListIListTest: IListVariableSizeReadWriteBaseTest<TestItem>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _recordList;
		private string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_recordList = new Db4oRecordList<TestItem>(_dataSource);

			_list = _recordList;
			TestItem firstItem = new TestItem("Jared", 1, new DateTime(2003, 7, 10));
			_recordList.Add(firstItem);
			_recordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_firstItem = firstItem;
			_newItem = new TestItem();
			_isSorted = true;
		}

		[TearDown]
		public void TearDown()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListIListWithNoDataTest: IListVariableSizeReadWriteBaseTest<TestItem>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _recordList;
		private string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_recordList = new Db4oRecordList<TestItem>(_dataSource);

			_list = _recordList;
			_firstItem = null;
			_newItem = new TestItem();
			_isSorted = true;
		}

		[TearDown]
		public void TearDown()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListIBindingListTest: IBindingListBaseTest<TestItem, int>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _recordList;
		private string _FilePath;

		[SetUp]
		public override void SetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_recordList = new Db4oRecordList<TestItem>(_dataSource);
			_bindingList = _recordList;

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			_property = pdc.Find("StoredInt", false);

			_newItem = new TestItem();
			_key = 1;
			base.SetUp();

			_recordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_recordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			ResetListChanged();
		}

		[TearDown]
		public void TearDown()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
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
	}

	[TestFixture]
	public class Db4oRecordListIBindingListWithNoDataTest: IBindingListBaseTest<TestItem, int>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _db4oRecordList;
		private string _FilePath;

		[SetUp]
		public override void SetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_db4oRecordList = new Db4oRecordList<TestItem>(_dataSource);
			_bindingList = _db4oRecordList;

			_newItem = new TestItem();
			_key = 1;
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			_property = pdc.Find("StoredInt", false);
			base.SetUp();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			_db4oRecordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oRecordListIRecordListTest: IRecordListBaseTest<TestItem>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _db4oRecordList;
		private string _FilePath;

		[SetUp]
		public override void SetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_db4oRecordList = new Db4oRecordList<TestItem>(_dataSource);
			_recordList = _db4oRecordList;

			_changedFieldName = "StoredInt";
			base.SetUp();

			_db4oRecordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_db4oRecordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			ResetListChanged();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			_db4oRecordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}

		protected override void Change(TestItem item)
		{
			item.StoredInt++;
		}
	}

	[TestFixture]
	public class Db4oRecordListIRecordListWithNoDataTest: IRecordListBaseTest<TestItem>
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _db4oRecordList;
		private string _FilePath;

		[SetUp]
		public override void SetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_db4oRecordList = new Db4oRecordList<TestItem>(_dataSource);
			_recordList = _db4oRecordList;
			_changedFieldName = "StoredInt";
			base.SetUp();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			_db4oRecordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}

		protected override void Change(TestItem item)
		{
			item.StoredInt++;
		}
	}

	[TestFixture]
	public class Db4oRecordListSortedTest
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _recordList;
		private string _filePath;
		private TestItem _jared, _gianna, _eric, _allison;

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
			Assert.AreEqual(ListChangedType.Reset, _listChangedEventArgs.ListChangedType);
			ResetListChanged();
		}

		private void ResetListChanged()
		{
			_listChanged = false;
			_listChangedEventArgs = null;
		}

		[SetUp]
		public void SetUp()
		{
			_filePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_filePath);
			_recordList = new Db4oRecordList<TestItem>(_dataSource);
			_recordList.ListChanged += _adaptor_ListChanged;

			_eric = new TestItem("Eric", 1, new DateTime(2006, 2, 28));
			_allison = new TestItem("Allison", 2, new DateTime(2006, 1, 08));
			_jared = new TestItem("Jared", 3, new DateTime(2006, 7, 10));
			_gianna = new TestItem("Gianna", 4, new DateTime(2006, 7, 17));
			_recordList.Add(_jared);
			_recordList.Add(_gianna);
			_recordList.Add(_eric);
			_recordList.Add(_allison);
			ResetListChanged();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void SortIntAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredInt", false);
			_recordList.ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_eric, _recordList[0]);
			Assert.AreEqual(_allison, _recordList[1]);
			Assert.AreEqual(_jared, _recordList[2]);
			Assert.AreEqual(_gianna, _recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortIntDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredInt", false);
			_recordList.ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_gianna, _recordList[0]);
			Assert.AreEqual(_jared, _recordList[1]);
			Assert.AreEqual(_allison, _recordList[2]);
			Assert.AreEqual(_eric, _recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDateAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			_recordList.ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_allison, _recordList[0]);
			Assert.AreEqual(_eric, _recordList[1]);
			Assert.AreEqual(_jared, _recordList[2]);
			Assert.AreEqual(_gianna, _recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDateDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			_recordList.ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_gianna, _recordList[0]);
			Assert.AreEqual(_jared, _recordList[1]);
			Assert.AreEqual(_eric, _recordList[2]);
			Assert.AreEqual(_allison, _recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortStringAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredString", false);
			_recordList.ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_allison, _recordList[0]);
			Assert.AreEqual(_eric, _recordList[1]);
			Assert.AreEqual(_gianna, _recordList[2]);
			Assert.AreEqual(_jared, _recordList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortStringDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredString", false);
			_recordList.ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_jared, _recordList[0]);
			Assert.AreEqual(_gianna, _recordList[1]);
			Assert.AreEqual(_eric, _recordList[2]);
			Assert.AreEqual(_allison, _recordList[3]);
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
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _recordList;
		private string _filePath;
		private TestItem _jared, _gianna;

		[SetUp]
		public void SetUp()
		{
			_filePath = Path.GetTempFileName();
			OpenDatabase();

			_jared = new TestItem("Jared", 3, new DateTime(2006, 7, 10));
			_gianna = new TestItem("Gianna", 4, new DateTime(2006, 7, 17));
			_recordList.Add(_jared);
			_recordList.Add(_gianna);
		}

		[TearDown]
		public void TearDown()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			File.Delete(_filePath);
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
			_jared.StoredInt = 1;
			// should not be wired up to throw events to old bindinglist anymore
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
			TestItem item = (TestItem) ((IBindingList) _recordList).AddNew();
			int index = _recordList.IndexOf(item);
			Assert.AreEqual(_recordList.Count - 1, index);
			Assert.AreEqual(0, item.StoredInt);
			item.StoredInt = 11;

			ReOpenDatabase();
			Assert.IsFalse(_recordList.Contains(item));
			// when the database is reopened, the link to the database is lost
			Assert.AreEqual(item, _recordList[0]);
			Assert.AreEqual(11, _recordList[0].StoredInt);
		}

		private void ReOpenDatabase()
		{
			_recordList.Dispose();
			_dataSource.Dispose();
			OpenDatabase();
		}

		private void OpenDatabase()
		{
			_dataSource = new Db4oDataSource(_filePath);
			_recordList = new Db4oRecordList<TestItem>(_dataSource);
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			_recordList.ApplySort(pd, ListSortDirection.Ascending);
		}

		[Test]
		[Ignore(
				"By enforcing this, the performance goes way down. We get around this by calling activate in the OnActivateDepth method of our data classes."
				)]
		public void NestedStructure()
		{
			const int items = 10;
			_recordList.Clear();

			TestItem root = new TestItem("Root Item", 0, DateTime.Now);
			root.Child = new ChildTestItem("Child Item", 1, DateTime.Now);

			ChildTestItem parent = root.Child;
			for (int i = 2;i < items;++i)
			{
				parent.Child = new ChildTestItem("Child Item", i, DateTime.Now);
				parent = parent.Child;
			}

			Assert.AreEqual(items, root.Depth);
			_recordList.Add(root);
			Assert.AreEqual(1, _recordList.Count);
			Assert.AreEqual(root, _recordList[0]);
			Assert.AreEqual(items, _recordList[0].Depth);

			ReOpenDatabase();
			Assert.AreEqual(1, _recordList.Count);
			Assert.AreEqual(root, _recordList[0]);
			Assert.AreEqual(items, _recordList[0].Depth);
		}

		[Test]
		[Ignore(
				"Come back to this. Right now we get around this by calling activate in the OnActivateDepth method of our data classes."
				)]
		public void NestedStructureActivation()
		{
			const int items = 10;
			_recordList.Clear();

			TestItem root = new TestItem("Root Item", 0, DateTime.Now);
			root.Child = new ChildTestItem("Child Item", 1, DateTime.Now);

			ChildTestItem parent = root.Child;
			for (int i = 2;i < items;++i)
			{
				parent.Child = new ChildTestItem("Child Item", i, DateTime.Now);
				parent = parent.Child;
			}

			Assert.AreEqual(items, root.Depth);
			_recordList.Add(root);

			ReOpenDatabase();
			Assert.AreEqual(items, _recordList[0].OnActivateDepth);
		}
	}

	[TestFixture]
	public class Db4oRecordListTests
	{
		private Db4oDataSource _dataSource;
		private string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}

		[Test]
		public void CopyConstructor()
		{
			using (
					Db4oRecordList<SimpleIntTestClass> recordList =
							new Db4oRecordList<SimpleIntTestClass>(_dataSource))
			{
				for (int i = 0;i < 50;i++)
				{
					recordList.Add(new SimpleIntTestClass(i));
				}

				using (
						Db4oRecordList<SimpleIntTestClass> recordListCopy =
								new Db4oRecordList<SimpleIntTestClass>(recordList))
				{
					Assert.IsNotNull(recordListCopy);
					Assert.AreNotSame(recordList, recordListCopy);
					Assert.AreEqual(recordList, recordListCopy);
				}
			}
		}

		[Test]
		[Ignore(
				"We shouldn't fix this. It will go away when we switch our database model. April 30, 2008 Eric Albright"
				)]
		public void FilterACopyShouldReleasePropertyChangedEventHandler()
		{
			SimpleIntTestClass item = new SimpleIntTestClass(0);
			using (
					Db4oRecordList<SimpleIntTestClass> recordList =
							new Db4oRecordList<SimpleIntTestClass>(_dataSource))
			{
				recordList.Add(item);
				for (int i = 1;i < 50;i++)
				{
					recordList.Add(new SimpleIntTestClass(i));
				}

				using (
						Db4oRecordList<SimpleIntTestClass> recordListCopy =
								new Db4oRecordList<SimpleIntTestClass>(recordList))
				{
					recordListCopy.ApplyFilter(
							delegate(SimpleIntTestClass simpleIntTest) { return (11 == simpleIntTest.I); });
				}
			}
			item.I = 12;
		}
	}
}