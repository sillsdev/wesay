using System;
using System.ComponentModel;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oFilterableTest
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _bindingList;
		private string _filePath;
		private TestItem _jared, _gianna;

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

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			IConfiguration configuration = Db4oFactory.Configure();
			IObjectClass objectClass = configuration.ObjectClass(typeof (TestItem));
			objectClass.ObjectField("_storedInt").Indexed(true);
			objectClass.ObjectField("_storedString").Indexed(true);
		}

		[SetUp]
		public void SetUp()
		{
			_filePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_filePath);
			_bindingList = new Db4oRecordList<TestItem>(_dataSource);
			_bindingList.ListChanged += _adaptor_ListChanged;

			_jared = new TestItem("Jared", 1, new DateTime(2003, 7, 10));
			_gianna = new TestItem("Gianna", 2, new DateTime(2006, 7, 17));
			_bindingList.Add(_jared);
			_bindingList.Add(_gianna);
			ResetListChanged();
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			_bindingList.Dispose();
			_dataSource.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void Filter()
		{
			Assert.AreEqual(2, _bindingList.Count);
			Assert.IsFalse(_listChanged);
			_bindingList.ApplyFilter(delegate(TestItem item) { return item.StoredInt == 1; });
			Assert.AreEqual(1, _bindingList.Count);
			Assert.AreEqual(_jared, _bindingList[0]);
			AssertListChanged();
		}

		[Test]
		public void ClearFilter()
		{
			Filter();
			_bindingList.RemoveFilter();
			Assert.AreEqual(2, _bindingList.Count);
			AssertListChanged();
		}

		[Test]
		public void ChangeFilter()
		{
			Filter();

			_bindingList.ApplyFilter(
					delegate(TestItem item) { return item.StoredString.StartsWith("Gia"); });
			Assert.AreEqual(1, _bindingList.Count);
			Assert.AreEqual(_gianna, _bindingList[0]);
			AssertListChanged();
		}
	}

	[TestFixture]
	public class IndexedFilters
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<SimpleIntTestClass> _bindingList;
		private string _filePath;
		private const int _bindingListSize = 1000;

		private readonly Predicate<SimpleIntTestClass> _filter =
				delegate(SimpleIntTestClass item) { return item.I > 100 && item.I <= 200; };

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			IConfiguration configuration = Db4oFactory.Configure();
			IObjectClass objectClass = configuration.ObjectClass(typeof (SimpleIntTestClass));
			objectClass.ObjectField("_i").Indexed(true);
		}

		[SetUp]
		public void SetUp()
		{
			_filePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_filePath);
			_bindingList = new Db4oRecordList<SimpleIntTestClass>(_dataSource);
			Construct();
		}

		[TearDown]
		public void TearDown()
		{
			_bindingList.Dispose();
			_dataSource.Dispose();
			File.Delete(_filePath);
		}

		public void Construct()
		{
			for (int i = 0;i < _bindingListSize;++i)
			{
				_bindingList.Add(new SimpleIntTestClass(i));
			}
		}

		[Test]
		public void ConstructWithFilter()
		{
			_dataSource.Data.Commit();
			using (
					Db4oRecordList<SimpleIntTestClass> newBindingList =
							new Db4oRecordList<SimpleIntTestClass>(_dataSource, _filter))
			{
				Assert.AreEqual(100, newBindingList.Count);
			}
		}

		[Test]
		[Ignore("db4o bug gives improper result if _bindingList is not committed first")]
		public void ConstructWithFilterDb4oBug()
		{
			using (
					Db4oRecordList<SimpleIntTestClass> newBindingList =
							new Db4oRecordList<SimpleIntTestClass>(_dataSource, _filter))
			{
				Assert.AreEqual(100, newBindingList.Count);
			}
		}

		[Test]
		public void ApplyFilter()
		{
			using (
					Db4oRecordList<SimpleIntTestClass> newBindingList =
							new Db4oRecordList<SimpleIntTestClass>(_dataSource))
			{
				newBindingList.ApplyFilter(_filter);
				Assert.AreEqual(100, newBindingList.Count);
			}
		}
	}
}