using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oFilterableTest
	{

		Db4oDataSource _dataSource;
		Db4oBindingList<TestItem> _bindingList;
		string _filePath;
		TestItem _jared, _gianna;


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

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			com.db4o.config.Configuration configuration = com.db4o.Db4o.Configure();
			com.db4o.config.ObjectClass objectClass = configuration.ObjectClass(typeof(TestItem));
			objectClass.ObjectField("_storedInt").Indexed(true);
			objectClass.ObjectField("_storedString").Indexed(true);
		}

		[SetUp]
		public void SetUp()
		{
			_filePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_filePath);
			this._bindingList = new Db4oBindingList<TestItem>(this._dataSource);
			this._bindingList.ListChanged += new ListChangedEventHandler(_adaptor_ListChanged);

			_jared = new TestItem("Jared", 1, new DateTime(2003, 7, 10));
			_gianna = new TestItem("Gianna", 2, new DateTime(2006, 7, 17));
			this._bindingList.Add(_jared);
			this._bindingList.Add(_gianna);
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
		public void Filter()
		{
			Assert.AreEqual(2, this._bindingList.Count);
			Assert.IsFalse(_listChanged);
			this._bindingList.ApplyFilter(delegate(TestItem item)
			{
				return item.StoredInt == 1;
			});
			Assert.AreEqual(1, this._bindingList.Count);
			Assert.AreEqual(_jared, this._bindingList[0]);
			AssertListChanged();
		}
		[Test]
		public void ClearFilter()
		{
			Filter();
			this._bindingList.RemoveFilter();
			Assert.AreEqual(2, this._bindingList.Count);
			AssertListChanged();
		}

		[Test]
		public void ChangeFilter()
		{
			Filter();

			this._bindingList.ApplyFilter(delegate(TestItem item)
			{
				return item.StoredString.StartsWith("Gia");
			});
			Assert.AreEqual(1, this._bindingList.Count);
			Assert.AreEqual(_gianna, this._bindingList[0]);
			AssertListChanged();
		}

		[Test]
		public void RefreshFilter()
		{
			Filter();

			_gianna.StoredInt = 1;
			Assert.AreEqual(1, this._bindingList.Count);
			this._bindingList.RefreshFilter();
			Assert.AreEqual(2, this._bindingList.Count);
			AssertListChanged();
		}
	}

	[TestFixture]
	public class IndexedFilters
	{
		Db4oDataSource _dataSource;
		Db4oBindingList<SimpleIntTestClass> _bindingList;
		string _filePath;
		const int _bindingListSize = 1000;
		Predicate<SimpleIntTestClass> _filter = delegate(SimpleIntTestClass item)
									 {
										 return item.I > 100 && item.I <= 200;
									 };

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			com.db4o.config.Configuration configuration = com.db4o.Db4o.Configure();
			com.db4o.config.ObjectClass objectClass = configuration.ObjectClass(typeof(SimpleIntTestClass));
			objectClass.ObjectField("_i").Indexed(true);
		}

		[SetUp]
		public void SetUp()
		{
			_filePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_filePath);
			this._bindingList = new Db4oBindingList<SimpleIntTestClass>(this._dataSource);
			Construct();
		}

		[TearDown]
		public void TearDown()
		{
			this._bindingList.Dispose();
			this._dataSource.Dispose();
			System.IO.File.Delete(_filePath);
		}

		public void Construct()
		{
			for (int i = 0; i < _bindingListSize; ++i)
			{
				this._bindingList.Add(new SimpleIntTestClass(i));
			}
		}

		[Test]
		public void ConstructWithFilter()
		{
			using (Db4oBindingList<SimpleIntTestClass> newBindingList = new Db4oBindingList<SimpleIntTestClass>(this._dataSource, _filter))
			{
				Assert.AreEqual(100, newBindingList.Count);
			}
		}

		[Test]
		public void ApplyFilter()
		{
			using (Db4oBindingList<SimpleIntTestClass> newBindingList = new Db4oBindingList<SimpleIntTestClass>(this._dataSource))
			{
				newBindingList.ApplyFilter(_filter);
				Assert.AreEqual(100, newBindingList.Count);
			}
		}

	}
}
