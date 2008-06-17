using System;
using System.ComponentModel;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class InMemoryBindingListIEnumerableTests: IEnumerableBaseTest<TestItem>
	{
		private InMemoryBindingList<TestItem> _bindingList;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_bindingList = new InMemoryBindingList<TestItem>();

			_enumerable = _bindingList;
			_bindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			_bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_itemCount = 2;
		}
	}

	[TestFixture]
	public class InMemoryBindingListIEnumerableWithNoDataTests: IEnumerableBaseTest<TestItem>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_enumerable = new InMemoryBindingList<TestItem>();
			_itemCount = 0;
		}
	}

	[TestFixture]
	public class InMemoryBindingListICollectionTest: ICollectionBaseTest<TestItem>
	{
		private InMemoryBindingList<TestItem> _bindingList;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_bindingList = new InMemoryBindingList<TestItem>();

			_collection = _bindingList;
			_bindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			_bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_itemCount = 2;
		}
	}

	[TestFixture]
	public class InMemoryBindingListICollectionWithNoDataTest: ICollectionBaseTest<TestItem>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_collection = new InMemoryBindingList<TestItem>();
			_itemCount = 0;
		}
	}

	[TestFixture]
	public class InMemoryBindingListIListTest: IListVariableSizeReadWriteBaseTest<TestItem>
	{
		private InMemoryBindingList<TestItem> _bindingList;

		[SetUp]
		public void SetUp()
		{
			_bindingList = new InMemoryBindingList<TestItem>();

			_list = _bindingList;
			TestItem firstItem = new TestItem("Jared", 1, new DateTime(2003, 7, 10));
			_bindingList.Add(firstItem);
			_bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_firstItem = firstItem;
			_newItem = new TestItem();
			_isSorted = true;
		}
	}

	[TestFixture]
	public class InMemoryBindingListIListWithNoDataTest:
			IListVariableSizeReadWriteBaseTest<TestItem>
	{
		[SetUp]
		public void SetUp()
		{
			_list = new InMemoryBindingList<TestItem>();
			_firstItem = null;
			_newItem = new TestItem();
			_isSorted = true;
		}
	}

	[TestFixture]
	public class InMemoryBindingListIBindingListTest: IBindingListBaseTest<TestItem, int>
	{
		private InMemoryBindingList<TestItem> _inMemoryBindingList;

		[SetUp]
		public override void SetUp()
		{
			_inMemoryBindingList = new InMemoryBindingList<TestItem>();
			_bindingList = _inMemoryBindingList;

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			_property = pdc.Find("StoredInt", false);

			_newItem = new TestItem();
			_key = 1;
			base.SetUp();

			_inMemoryBindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_inMemoryBindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			ResetListChanged();
		}

		protected override void VerifySortAscending()
		{
			Assert.AreEqual(1, _inMemoryBindingList[0].StoredInt);
			Assert.AreEqual(2, _inMemoryBindingList[1].StoredInt);
			base.VerifySortAscending();
		}

		protected override void VerifySortDescending()
		{
			Assert.AreEqual(2, _inMemoryBindingList[0].StoredInt);
			Assert.AreEqual(1, _inMemoryBindingList[1].StoredInt);
			base.VerifySortDescending();
		}
	}

	[TestFixture]
	public class InMemoryBindingListIBindingListWithNoDataTest: IBindingListBaseTest<TestItem, int>
	{
		[SetUp]
		public override void SetUp()
		{
			_bindingList = new InMemoryBindingList<TestItem>();

			_newItem = new TestItem();
			_key = 1;
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			_property = pdc.Find("StoredInt", false);
			base.SetUp();
		}
	}

	[TestFixture]
	public class InMemoryBindingListSortedTest
	{
		private InMemoryBindingList<TestItem> _bindingList;

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
			_bindingList = new InMemoryBindingList<TestItem>();
			_bindingList.ListChanged += _adaptor_ListChanged;

			_eric = new TestItem("Eric", 1, new DateTime(2006, 2, 28));
			_allison = new TestItem("Allison", 2, new DateTime(2006, 1, 08));
			_jared = new TestItem("Jared", 3, new DateTime(2006, 7, 10));
			_gianna = new TestItem("Gianna", 4, new DateTime(2006, 7, 17));
			_bindingList.Add(_jared);
			_bindingList.Add(_gianna);
			_bindingList.Add(_eric);
			_bindingList.Add(_allison);
			ResetListChanged();
		}

		[Test]
		public void SortIntAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredInt", false);
			_bindingList.ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_eric, _bindingList[0]);
			Assert.AreEqual(_allison, _bindingList[1]);
			Assert.AreEqual(_jared, _bindingList[2]);
			Assert.AreEqual(_gianna, _bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortIntDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredInt", false);
			_bindingList.ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_gianna, _bindingList[0]);
			Assert.AreEqual(_jared, _bindingList[1]);
			Assert.AreEqual(_allison, _bindingList[2]);
			Assert.AreEqual(_eric, _bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDateAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			_bindingList.ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_allison, _bindingList[0]);
			Assert.AreEqual(_eric, _bindingList[1]);
			Assert.AreEqual(_jared, _bindingList[2]);
			Assert.AreEqual(_gianna, _bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDateDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredDateTime", false);
			_bindingList.ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_gianna, _bindingList[0]);
			Assert.AreEqual(_jared, _bindingList[1]);
			Assert.AreEqual(_eric, _bindingList[2]);
			Assert.AreEqual(_allison, _bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortStringAscending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredString", false);
			_bindingList.ApplySort(pd, ListSortDirection.Ascending);

			Assert.AreEqual(_allison, _bindingList[0]);
			Assert.AreEqual(_eric, _bindingList[1]);
			Assert.AreEqual(_gianna, _bindingList[2]);
			Assert.AreEqual(_jared, _bindingList[3]);
			AssertListChanged();
		}

		[Test]
		public void SortStringDescending()
		{
			Assert.IsFalse(_listChanged);

			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof (TestItem));
			PropertyDescriptor pd = pdc.Find("StoredString", false);
			_bindingList.ApplySort(pd, ListSortDirection.Descending);

			Assert.AreEqual(_jared, _bindingList[0]);
			Assert.AreEqual(_gianna, _bindingList[1]);
			Assert.AreEqual(_eric, _bindingList[2]);
			Assert.AreEqual(_allison, _bindingList[3]);
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