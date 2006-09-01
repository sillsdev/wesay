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

	[TestFixture]
	public class Db4oSpeedTest
	{

		Db4oDataSource _dataSource;
		Db4oBindingList<SimpleIntTestClass> _bindingList;
		string _filePath;
		const int _bindingListSize = 5000;
		Predicate<SimpleIntTestClass> _filter = delegate(SimpleIntTestClass item)
									 {
										 return item.I > 100 && item.I <= 200;
									 };


		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			com.db4o.config.Configuration configuration = com.db4o.Db4o.Configure();
			com.db4o.config.ObjectClass objectClass = configuration.ObjectClass(typeof(SimpleIntTestClass));
			com.db4o.config.ObjectField objectField = objectClass.ObjectField("_i");
			objectField.Indexed(true);
		}
		[SetUp]
		public void SetUp()
		{
			_filePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_filePath);
			this._bindingList = new Db4oBindingList<SimpleIntTestClass>(this._dataSource);
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
			TearDown();
			SetUp();
			for (int i = 0; i < _bindingListSize; ++i)
			{
				this._bindingList.Add(new SimpleIntTestClass(i));
			}
		}

		[Test]
		public void ConstructTime()
		{
			Construct();
			Assert.Greater((decimal)TimeCommitAllAtOnce().ElapsedTicks, (decimal)TimeCommitOneAtATime().ElapsedTicks);
		}

		[Test]
		public void CompareQueryWithAndWithoutFilter()
		{
			Assert.Less((decimal)TimeQueryNoFilter().ElapsedTicks, (decimal)TimeQueryWithFilter().ElapsedTicks);
		}

		[Test]
		public void CompareApplyFilterWithNoApplyFilter()
		{
			Assert.Less((decimal)TimeQueryNoFilter().ElapsedTicks, (decimal)TimeQueryThenApplyFilter().ElapsedTicks);
		}

		[Test]
		public void CompareQueryWithFilterAndQueryThenApplyFilter()
		{
			Assert.Less((decimal)TimeQueryWithFilter().ElapsedTicks, (decimal)TimeQueryThenApplyFilter().ElapsedTicks);
		}

		private delegate void Strategy();

		private System.Diagnostics.Stopwatch Time(string label, Strategy strategy){
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();

			strategy.Invoke();

			stopwatch.Stop();
			System.Console.WriteLine(label + " {0}", stopwatch.Elapsed);
			return stopwatch;
		}

		private System.Diagnostics.Stopwatch TimeCommitAllAtOnce()
		{
			return Time("Commit all at once",
						delegate()
						{
							_bindingList.WriteCacheSize = 0;
							Construct();
							_bindingList.WriteCacheSize = 1;
						});
		}

		private System.Diagnostics.Stopwatch TimeCommitOneAtATime()
		{
			return Time("Commit one at a time",
						delegate()
						{
							Construct();
						});
		}


		private System.Diagnostics.Stopwatch TimeQueryNoFilter()
		{
			Construct();
			return Time("No filter",
						delegate()
						{
							using (Db4oBindingList<SimpleIntTestClass> newBindingList = new Db4oBindingList<SimpleIntTestClass>(this._dataSource))
							{
								Assert.AreEqual(_bindingListSize, newBindingList.Count);
							}
						});
		}

		private System.Diagnostics.Stopwatch TimeQueryWithFilter()
		{
			Construct();
			return Time("Constructed with filter",
						delegate()
						{
							using (Db4oBindingList<SimpleIntTestClass> newBindingList = new Db4oBindingList<SimpleIntTestClass>(this._dataSource, _filter))
							{
								Assert.AreEqual(100, newBindingList.Count);
							}
						});
		}

		private System.Diagnostics.Stopwatch TimeQueryThenApplyFilter()
		{
			Construct();
			return Time("Constructed then filter applied",
						delegate()
						{
							using (Db4oBindingList<SimpleIntTestClass> newBindingList = new Db4oBindingList<SimpleIntTestClass>(this._dataSource))
							{
								newBindingList.ApplyFilter(_filter);
								Assert.AreEqual(100, newBindingList.Count);
							}
						});
		}
	}
}
