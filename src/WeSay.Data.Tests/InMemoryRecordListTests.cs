using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using WeSay.Data.Tests.IEnumerableTests;
using WeSay.Data.Tests.ICollectionTests;
using WeSay.Data.Tests.IListTests;
using WeSay.Data.Tests.IBindingListTests;


namespace WeSay.Data.Tests.InMemoryRecordListTests
{
	[TestFixture]
	public class InMemoryRecordListIEnumerableTests: IEnumerableBaseTest<TestItem>
	{
		InMemoryRecordList<TestItem> _bindingList;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			this._bindingList = new InMemoryRecordList<TestItem>();

			this._enumerable = this._bindingList;
			this._bindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._bindingList.Dispose();
		}
	}

	[TestFixture]
	public class InMemoryRecordListIEnumerableWithNoDataTests : IEnumerableBaseTest<TestItem>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			this._enumerable = new InMemoryRecordList<TestItem>();
			this._itemCount = 0;
		}
		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			((IDisposable)this._enumerable).Dispose();
		}
	}


	[TestFixture]
	public class InMemoryRecordListICollectionTest : ICollectionBaseTest<TestItem>
	{
		InMemoryRecordList<TestItem> _bindingList;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			this._bindingList = new InMemoryRecordList<TestItem>();

			this._collection = this._bindingList;
			this._bindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}
		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._bindingList.Dispose();
		}
	}


	[TestFixture]
	public class InMemoryRecordListICollectionWithNoDataTest : ICollectionBaseTest<TestItem>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			this._collection = new InMemoryRecordList<TestItem>();
			this._itemCount = 0;
		}
		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			((IDisposable)this._collection).Dispose();
		}
	}

	[TestFixture]
	public class InMemoryRecordListIListTest : IListVariableSizeReadWriteBaseTest<TestItem>
	{
		InMemoryRecordList<TestItem> _bindingList;

		[SetUp]
		public void SetUp()
		{
			this._bindingList = new InMemoryRecordList<TestItem>();

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
		}
	}

	[TestFixture]
	public class InMemoryRecordListIListWithNoDataTest : IListVariableSizeReadWriteBaseTest<TestItem>
	{
		[SetUp]
		public void SetUp()
		{
			this._list = new InMemoryRecordList<TestItem>();
			this._firstItem = null;
			this._newItem = new TestItem();
			this._isSorted = true;
		}
		[TearDown]
		public void TearDown()
		{
			((IDisposable)this._list).Dispose();
		}
	}

	[TestFixture]
	public class InMemoryRecordListIBindingListTest : IBindingListBaseTest<TestItem, int>
	{
		InMemoryRecordList<TestItem> _inMemoryRecordList;

		[SetUp]
		public override void SetUp()
		{
			this._inMemoryRecordList = new InMemoryRecordList<TestItem>();
			this._bindingList = this._inMemoryRecordList;

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			this._property = pdc.Find("StoredInt", false);

			this._newItem = new TestItem();
			this._key = 1;
			base.SetUp();

			this._inMemoryRecordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._inMemoryRecordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this.ResetListChanged();
		}

		[TearDown]
		public void TearDown()
		{
			this._inMemoryRecordList.Dispose();
		}

		protected override void VerifySortAscending()
		{
			Assert.AreEqual(1, _inMemoryRecordList[0].StoredInt);
			Assert.AreEqual(2, _inMemoryRecordList[1].StoredInt);
			base.VerifySortAscending();
		}

		protected override void VerifySortDescending()
		{
			Assert.AreEqual(2, _inMemoryRecordList[0].StoredInt);
			Assert.AreEqual(1, _inMemoryRecordList[1].StoredInt);
			base.VerifySortDescending();
		}

		protected override void VerifyUnsorted()
		{
			base.VerifyUnsorted();
		}
	}

	[TestFixture]
	public class InMemoryRecordListIBindingListWithNoDataTest : IBindingListBaseTest<TestItem, int>
	{
		[SetUp]
		public override void SetUp()
		{
			this._bindingList = new InMemoryRecordList<TestItem>();

			this._newItem = new TestItem();
			this._key = 1;
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			this._property = pdc.Find("StoredInt", false);
			base.SetUp();
		}
		[TearDown]
		public void TearDown()
		{
			((IDisposable)this._bindingList).Dispose();
		}

	}

	[TestFixture]
	public class InMemoryRecordListIRecordListTest : IRecordListBaseTest<TestItem>
	{
		InMemoryRecordList<TestItem> _inMemoryRecordList;

		[SetUp]
		public override void SetUp()
		{
			this._inMemoryRecordList = new InMemoryRecordList<TestItem>();
			this._recordList = this._inMemoryRecordList;
			this._changedFieldName = "StoredInt";
			base.SetUp();

			this._inMemoryRecordList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._inMemoryRecordList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this.ResetListChanged();
		}

		[TearDown]
		public void TearDown()
		{
			this._inMemoryRecordList.Dispose();
		}

		protected override void Change(TestItem item)
		{
			item.StoredInt++;
		}
	}

	[TestFixture]
	public class InMemoryRecordListIRecordListWithNoDataTest : IRecordListBaseTest<TestItem>
	{
		[SetUp]
		public override void SetUp()
		{
			this._recordList = new InMemoryRecordList<TestItem>();
			this._changedFieldName = "StoredInt";
			base.SetUp();
		}
		[TearDown]
		public void TearDown()
		{
			this._recordList.Dispose();
		}

		protected override void Change(TestItem item)
		{
			item.StoredInt++;
		}
	}


	[TestFixture]
	public class InMemoryRecordListSortedTest
	{
		InMemoryRecordList<TestItem> _bindingList;

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
			this._bindingList = new InMemoryRecordList<TestItem>();
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
		public void TearDown()
		{
			this._bindingList.Dispose();
		}

		[Test]
		public void SortIntAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredInt", false);
			((IBindingList)this._bindingList).ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_eric, this._bindingList[0]);
			Assert.AreEqual(_allison, this._bindingList[1]);
			Assert.AreEqual(_jared, this._bindingList[2]);
			Assert.AreEqual(_gianna, this._bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortIntDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredInt", false);
			((IBindingList)this._bindingList).ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_gianna, this._bindingList[0]);
			Assert.AreEqual(_jared, this._bindingList[1]);
			Assert.AreEqual(_allison, this._bindingList[2]);
			Assert.AreEqual(_eric, this._bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDateAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			((IBindingList)this._bindingList).ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_allison, this._bindingList[0]);
			Assert.AreEqual(_eric, this._bindingList[1]);
			Assert.AreEqual(_jared, this._bindingList[2]);
			Assert.AreEqual(_gianna, this._bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDateDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			((IBindingList)this._bindingList).ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_gianna, this._bindingList[0]);
			Assert.AreEqual(_jared, this._bindingList[1]);
			Assert.AreEqual(_eric, this._bindingList[2]);
			Assert.AreEqual(_allison, this._bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortStringAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredString", false);
			((IBindingList)this._bindingList).ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_allison, this._bindingList[0]);
			Assert.AreEqual(_eric, this._bindingList[1]);
			Assert.AreEqual(_gianna, this._bindingList[2]);
			Assert.AreEqual(_jared, this._bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortStringDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			PropertyDescriptor pd = pdc.Find("StoredString", false);
			((IBindingList)this._bindingList).ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_jared, this._bindingList[0]);
			Assert.AreEqual(_gianna, this._bindingList[1]);
			Assert.AreEqual(_eric, this._bindingList[2]);
			Assert.AreEqual(_allison, this._bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void ChangeSort()
		{
			SortIntAscending();
			SortDateDescending();
		}
	}
}
