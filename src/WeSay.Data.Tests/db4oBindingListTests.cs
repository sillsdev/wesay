using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.ComponentModel;



namespace WeSay.Data.Tests
{
	public class StoredItem
	{
		int _storedInt;
		string _storedString;
		DateTime _storedDateTime;

		public StoredItem()
		{
		}

		public StoredItem(string s, int i, DateTime d)
		{
			_storedString = s;
			_storedInt = i;
			_storedDateTime = d;
		}

		public override string ToString()
		{
			return StoredInt.ToString() + ". " + StoredString + " " + StoredDateTime.ToString();
		}

		public int StoredInt
		{
			get
			{
				return this._storedInt;
			}
			set
			{
				this._storedInt = value;
			}
		}

		public string StoredString
		{
			get
			{
				return this._storedString;
			}
			set
			{
				this._storedString = value;
			}
		}

		public DateTime StoredDateTime
		{
			get
			{
				return this._storedDateTime;
			}
			set
			{
				this._storedDateTime = value;
			}
		}
	}

	[TestFixture]
	public class Db4oDataAdaptorIEnumerableTests : IEnumerableBaseTest<StoredItem>
	{
		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{

			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._adaptor = new Db4oDataAdaptor<StoredItem>(new Db4oBindingListConfiguration<StoredItem>(this._dataSource));

			this._enumerable = this._adaptor;
			this._adaptor.Add(new StoredItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._adaptor.Add(new StoredItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oDataAdaptorIEnumerableWithNoDataTests : IEnumerableBaseTest<StoredItem>
	{
		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{

			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._adaptor = new Db4oDataAdaptor<StoredItem>(new Db4oBindingListConfiguration<StoredItem>(this._dataSource));

			this._enumerable = this._adaptor;
			this._itemCount = 0;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}


	[TestFixture]
	public class Db4oDataAdaptorICollectionTest : ICollectionBaseTest<StoredItem>
	{
		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._adaptor = new Db4oDataAdaptor<StoredItem>(new Db4oBindingListConfiguration<StoredItem>(this._dataSource));

			this._collection = this._adaptor;
			this._adaptor.Add(new StoredItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._adaptor.Add(new StoredItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this._itemCount = 2;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}


	[TestFixture]
	public class Db4oDataAdaptorICollectionWithNoDataTest : ICollectionBaseTest<StoredItem>
	{
		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _FilePath;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._adaptor = new Db4oDataAdaptor<StoredItem>(new Db4oBindingListConfiguration<StoredItem>(this._dataSource));

			this._collection = this._adaptor;
			this._itemCount = 0;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}
	}

	[TestFixture]
	public class Db4oDataAdaptorIListTest : IListVariableSizeReadWriteBaseTest<StoredItem>
	{
		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			Db4oBindingListConfiguration<StoredItem> config = new Db4oBindingListConfiguration<StoredItem>(this._dataSource);

			this._adaptor = new Db4oDataAdaptor<StoredItem>(config);

			this._list = this._adaptor;
			this._adaptor.Add(new StoredItem("Jared", 1, new DateTime(2003, 7, 10)));
			StoredItem firstItem = new StoredItem("Gianna", 2, new DateTime(2006, 7, 17));
			this._adaptor.Add(firstItem);
			this._firstItem = firstItem;
			this._newItem = new StoredItem();
			this._isSorted = true;
		}

		[TearDown]
		public void TearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void InsertNotSupported()
		{
			base.InsertNotSupported();
		}

	}

	[TestFixture]
	public class Db4oDataAdaptorIListWithNoDataTest : IListVariableSizeReadWriteBaseTest<StoredItem>
	{
		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._adaptor = new Db4oDataAdaptor<StoredItem>(new Db4oBindingListConfiguration<StoredItem>(this._dataSource));

			this._list = this._adaptor;
			this._firstItem = null;
			this._newItem = new StoredItem();
			this._isSorted = true;
		}

		[TearDown]
		public void TearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public new void InsertNotSupported()
		{
			base.InsertNotSupported();
		}

	}

	[TestFixture]
	public class Db4oDataAdaptorIBindingListTest : IBindingListBaseTest<StoredItem, int>
	{

		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _FilePath;

		[TestFixtureSetUp]
		public new void TestFixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._adaptor = new Db4oDataAdaptor<StoredItem>(new Db4oBindingListConfiguration<StoredItem>(this._dataSource));
			this._bindingList = this._adaptor;

			this._newItem = new StoredItem();
			this._key = 1;
			this._property = null;
			base.TestFixtureSetUp();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		[SetUp]
		public void SetUp()
		{
			this._adaptor.Add(new StoredItem("Jared", 1, new DateTime(2003, 7, 10)));
			this._adaptor.Add(new StoredItem("Gianna", 2, new DateTime(2006, 7, 17)));
			this.ResetListChanged();
		}

		[TearDown]
		public void TearDown()
		{
			this._adaptor.Clear();
		}

		public override void ListChangedOnChange()
		{
		}

	}

	[TestFixture]
	public class Db4oDataAdaptorIBindingListWithNoDataTest : IBindingListBaseTest<StoredItem, int>
	{

		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _FilePath;

		[TestFixtureSetUp]
		public new void TestFixtureSetUp()
		{
			_FilePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_FilePath);
			this._adaptor = new Db4oDataAdaptor<StoredItem>(new Db4oBindingListConfiguration<StoredItem>(this._dataSource));
			this._bindingList = this._adaptor;

			this._newItem = new StoredItem();
			this._key = 1;
			this._property = null;
			base.TestFixtureSetUp();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_FilePath);
		}

		public override void ListChangedOnChange()
		{
		}
	}


	[TestFixture]
	public class Db4oDataAdaptorFilteredTest
	{

		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _filePath;
		StoredItem _jared, _gianna;


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
			this._adaptor = new Db4oDataAdaptor<StoredItem>(new Db4oBindingListConfiguration<StoredItem>(this._dataSource));
			this._adaptor.ListChanged += new ListChangedEventHandler(_adaptor_ListChanged);

			_jared = new StoredItem("Jared", 1, new DateTime(2003, 7, 10));
			_gianna = new StoredItem("Gianna", 2, new DateTime(2006, 7, 17));
			this._adaptor.Add(_jared);
			this._adaptor.Add(_gianna);
			ResetListChanged();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_filePath);
		}

		[Test]
		public void Filter()
		{
			Assert.AreEqual(2, this._adaptor.Count);
			Assert.IsFalse(_listChanged);
			this._adaptor.ApplyFilter(delegate(StoredItem item)
			{
				return item.StoredInt == 1;
			});
			Assert.AreEqual(1, this._adaptor.Count);
			Assert.AreEqual(_jared, this._adaptor[0]);
			AssertListChanged();
		}
		[Test]
		public void ClearFilter()
		{
			Filter();
			this._adaptor.RemoveFilter();
			Assert.AreEqual(2, this._adaptor.Count);
			AssertListChanged();
		}

		[Test]
		public void ChangeFilter()
		{
			Filter();

			this._adaptor.ApplyFilter(delegate(StoredItem item)
			{
				return item.StoredString.StartsWith("Gia");
			});
			Assert.AreEqual(1, this._adaptor.Count);
			Assert.AreEqual(_gianna, this._adaptor[0]);
			AssertListChanged();
		}

	}

	[TestFixture]
	public class Db4oDataAdaptorSortedTest
	{

		Db4oDataSource _dataSource;
		Db4oDataAdaptor<StoredItem> _adaptor;
		string _filePath;
		StoredItem _jared, _gianna, _eric, _allison;


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
			this._adaptor = new Db4oDataAdaptor<StoredItem>(new Db4oBindingListConfiguration<StoredItem>(this._dataSource));
			this._adaptor.ListChanged += new ListChangedEventHandler(_adaptor_ListChanged);

			_eric = new StoredItem("Eric", 1, new DateTime(2006, 2, 28));
			_allison = new StoredItem("Allison", 2, new DateTime(2006, 1, 08));
			_jared = new StoredItem("Jared", 3, new DateTime(2006, 7, 10));
			_gianna = new StoredItem("Gianna", 4, new DateTime(2006, 7, 17));
			this._adaptor.Add(_jared);
			this._adaptor.Add(_gianna);
			this._adaptor.Add(_eric);
			this._adaptor.Add(_allison);
			ResetListChanged();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_filePath);
		}

		class StoredItemIntComparer : Comparer<StoredItem>
		{
			public override int Compare(StoredItem x, StoredItem y)
			{
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				return -(x.StoredInt - y.StoredInt);
			}
		}

		class StoredItemStringComparer : Comparer<StoredItem>
		{
			public override int Compare(StoredItem x, StoredItem y)
			{
				int i = string.Compare(x.StoredString, y.StoredString);
				return -i;
			}
		}

		class StoredItemDateComparer : Comparer<StoredItem>
		{
			public override int Compare(StoredItem x, StoredItem y)
			{
				int i = DateTime.Compare(x.StoredDateTime, y.StoredDateTime);
				return -i;
			}
		}

		[Test]
		public void SortInt()
		{
			Assert.IsFalse(_listChanged);
			this._adaptor.Sort = new StoredItemIntComparer();
			Assert.AreEqual(_eric, this._adaptor[0]);
			Assert.AreEqual(_allison, this._adaptor[1]);
			Assert.AreEqual(_jared, this._adaptor[2]);
			Assert.AreEqual(_gianna, this._adaptor[3]);
			AssertListChanged();
		}

		[Test]
		public void SortDate()
		{
			Assert.IsFalse(_listChanged);
			this._adaptor.Sort = new StoredItemDateComparer();
			Assert.AreEqual(_allison, this._adaptor[0]);
			Assert.AreEqual(_eric, this._adaptor[1]);
			Assert.AreEqual(_jared, this._adaptor[2]);
			Assert.AreEqual(_gianna, this._adaptor[3]);
			AssertListChanged();
		}

		[Test]
		public void SortString()
		{
			Assert.IsFalse(_listChanged);
			this._adaptor.Sort = new StoredItemStringComparer();
			Assert.AreEqual(_allison, this._adaptor[0]);
			Assert.AreEqual(_eric, this._adaptor[1]);
			Assert.AreEqual(_gianna, this._adaptor[2]);
			Assert.AreEqual(_jared, this._adaptor[3]);
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
}
