using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;



namespace WeSay.Data.Tests
{
	[TestFixture]
	public class InMemoryBindingListIEnumerableTests : IEnumerableBaseTest<TestItem>
	{
		InMemoryBindingList<TestItem> _bindingList;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			this._bindingList = new InMemoryBindingList<TestItem>();

			this._enumerable = this._bindingList;
			this._bindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}
	}

	[TestFixture]
	public class InMemoryBindingListIEnumerableWithNoDataTests : IEnumerableBaseTest<TestItem>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			this._enumerable = new InMemoryBindingList<TestItem>();
			this._itemCount = 0;
		}
	}


	[TestFixture]
	public class InMemoryBindingListICollectionTest : ICollectionBaseTest<TestItem>
	{
		InMemoryBindingList<TestItem> _bindingList;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			this._bindingList = new InMemoryBindingList<TestItem>();

			this._collection = this._bindingList;
			this._bindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}
	}


	[TestFixture]
	public class InMemoryBindingListICollectionWithNoDataTest : ICollectionBaseTest<TestItem>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			this._collection = new InMemoryBindingList<TestItem>();
			this._itemCount = 0;
		}
	}

	[TestFixture]
	public class InMemoryBindingListIListTest : IListVariableSizeReadWriteBaseTest<TestItem>
	{
		InMemoryBindingList<TestItem> _bindingList;

		[SetUp]
		public void SetUp()
		{
			this._bindingList = new InMemoryBindingList<TestItem>();

			this._list = this._bindingList;
			TestItem firstItem = new TestItem("Jared", 1, new DateTime(2003, 7, 10));
			this._bindingList.Add(firstItem);
			this._bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._firstItem = firstItem;
			this._newItem = new TestItem();
			this._isSorted = true;
		}
	}

	[TestFixture]
	public class InMemoryBindingListIListWithNoDataTest : IListVariableSizeReadWriteBaseTest<TestItem>
	{
		[SetUp]
		public void SetUp()
		{
			this._list = new InMemoryBindingList<TestItem>();
			this._firstItem = null;
			this._newItem = new TestItem();
			this._isSorted = true;
		}
	}

	[TestFixture]
	public class InMemoryBindingListIBindingListTest : IBindingListBaseTest<TestItem, int>
	{
		InMemoryBindingList<TestItem> _InMemoryBindingList;

		[SetUp]
		public override void SetUp()
		{
			this._InMemoryBindingList = new InMemoryBindingList<TestItem>();
			this._bindingList = this._InMemoryBindingList;

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			this._property = pdc.Find("StoredInt", false);

			this._newItem = new TestItem();
			this._key = 1;
			base.SetUp();

			this._InMemoryBindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._InMemoryBindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			this.ResetListChanged();
		}

		protected override void VerifySortAscending()
		{
			Assert.AreEqual(1, _InMemoryBindingList[0].StoredInt);
			Assert.AreEqual(2, _InMemoryBindingList[1].StoredInt);
			base.VerifySortAscending();
		}

		protected override void VerifySortDescending()
		{
			Assert.AreEqual(2, _InMemoryBindingList[0].StoredInt);
			Assert.AreEqual(1, _InMemoryBindingList[1].StoredInt);
			base.VerifySortDescending();
		}

		protected override void VerifyUnsorted()
		{
			base.VerifyUnsorted();
		}
	}

	[TestFixture]
	public class InMemoryBindingListIBindingListWithNoDataTest : IBindingListBaseTest<TestItem, int>
	{
		[SetUp]
		public override void SetUp()
		{
			this._bindingList = new InMemoryBindingList<TestItem>();

			this._newItem = new TestItem();
			this._key = 1;
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TestItem));
			this._property = pdc.Find("StoredInt", false);
			base.SetUp();
		}
	}


	[TestFixture]
	public class InMemoryBindingListSortedTest
	{
		InMemoryBindingList<TestItem> _bindingList;

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
			this._bindingList = new InMemoryBindingList<TestItem>();
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
