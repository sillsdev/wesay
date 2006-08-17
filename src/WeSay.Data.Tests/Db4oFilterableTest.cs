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
	public class Db4oFilterSpeedTest
	{
		public class SimpleTestClass: INotifyPropertyChanged, IComparable, IComparable<SimpleTestClass>
		{
			int _i;
			public SimpleTestClass()
			{
			}

			public SimpleTestClass(int i)
			{
				this._i = i;
			}

			public int I
			{
				get
				{
					return _i;
				}
				set
				{
					_i = value;
					OnPropertyChanged(new PropertyChangedEventArgs("I"));
				}
			}

			#region INotifyPropertyChanged Members

			public event PropertyChangedEventHandler PropertyChanged;
			protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
			{
				if (this.PropertyChanged != null)
				{
					this.PropertyChanged(this, e);
				}

			}

			#endregion

			#region IComparable Members

			int IComparable.CompareTo(object obj)
			{
				return CompareTo((SimpleTestClass)obj);
			}

			#endregion

			#region IComparable<SimpleTestClass> Members

			public int CompareTo(SimpleTestClass other)
			{
				if (other == null)
				{
					return 1;
				}
				return (other.I - I);
			}

			#endregion
		}

		Db4oDataSource _dataSource;
		Db4oBindingList<SimpleTestClass> _bindingList;
		string _filePath;

		[SetUp]
		public void SetUp()
		{
			_filePath = System.IO.Path.GetTempFileName();
			this._dataSource = new Db4oDataSource(_filePath);
			this._bindingList = new Db4oBindingList<SimpleTestClass>(this._dataSource);
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			this._dataSource.Dispose();
			System.IO.File.Delete(_filePath);
		}

		[Test]
		public void SlowFilter()
		{
			for (int i = 0; i < 1000; ++i)
			{
				this._bindingList.Add(new SimpleTestClass(i));
			}

			Assert.AreEqual(1000, this._bindingList.Count);
			this._bindingList.ApplyFilter(delegate(SimpleTestClass item)
			{
				return item.I > 100 && item.I <= 200;
			});
			Assert.AreEqual(100, this._bindingList.Count);
			Assert.AreEqual(101, this._bindingList[0].I);
		}

		[Test]
		public void SlowFilterOneCommit()
		{
			_bindingList.WriteCacheSize = 1000;
			for (int i = 0; i < 1000; ++i)
			{
				this._bindingList.Add(new SimpleTestClass(i));
			}
			_bindingList.WriteCacheSize = 1;

			Assert.AreEqual(1000, this._bindingList.Count);
			this._bindingList.ApplyFilter(delegate(SimpleTestClass item)
			{
				return item.I > 100 && item.I <= 200;
			});
			Assert.AreEqual(100, this._bindingList.Count);
			Assert.AreEqual(101, this._bindingList[0].I);
		}

	}

}
