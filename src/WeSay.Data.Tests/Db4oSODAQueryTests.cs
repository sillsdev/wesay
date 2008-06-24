using System;
using System.Collections.Generic;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class db4oBindingListSODAQuery
	{
		private Db4oDataSource _dataSource;
		private Db4oRecordList<TestItem> _bindingList;
		private string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_FilePath);
			_bindingList = new Db4oRecordList<TestItem>(_dataSource);

			_bindingList.Add(new TestItem("Jared", 1, new DateTime(2003, 7, 10)));
			_bindingList.Add(new TestItem("Gianna", 2, new DateTime(2006, 7, 17)));
			_bindingList.Add(new TestItem("Third", 2, new DateTime(2006, 9, 17)));

			List<ChildTestItem> jaredchildren = new List<ChildTestItem>();
			jaredchildren.Add(
					new ChildTestItem("Jared Child of Jared", 3, new DateTime(2003, 7, 10)));
			jaredchildren.Add(
					new ChildTestItem("Gianna Child of Jared", 4, new DateTime(2006, 7, 17)));
			_bindingList[0].Children = jaredchildren;

			List<ChildTestItem> giannachildren = new List<ChildTestItem>();
			giannachildren.Add(
					new ChildTestItem("Jared Child of Gianna", 5, new DateTime(2003, 7, 10)));
			giannachildren.Add(new ChildTestItem(String.Empty, 6, new DateTime(2006, 7, 17)));
			_bindingList[1].Children = giannachildren;

			_bindingList[2].Children = new List<ChildTestItem>();
		}

		[TearDown]
		public void TearDown()
		{
			_bindingList.Dispose();
			_dataSource.Dispose();
			File.Delete(_FilePath);
		}

		[Test]
		public void Everything()
		{
			_bindingList.SodaQuery = delegate(IQuery query)
									 {
										 query.Constrain(typeof (TestItem));
										 return query;
									 };
			Assert.AreEqual(3, _bindingList.Count);
		}

		[Test]
		public void StoredStringEqualGianna()
		{
			_bindingList.SodaQuery = delegate(IQuery query)
									 {
										 query.Constrain(typeof (TestItem));
										 query.Descend("_storedString").Constrain("Gianna");
										 return query;
									 };
			Assert.AreEqual(1, _bindingList.Count);
			Assert.AreEqual("Gianna", _bindingList[0].StoredString);
		}

		[Test]
		public void TestItemHasChildWithEmptyString()
		{
			_bindingList.SodaQuery = TestItemHasChildWithEmptyString;
			//Console.WriteLine("Items having child with empty string:");
			//foreach (TestItem item in _bindingList)
			//{
			//    Console.WriteLine("  {0}", item);
			//}
			Assert.AreEqual(1, _bindingList.Count);
			Assert.AreEqual("Gianna", _bindingList[0].StoredString);
		}

		public IQuery TestItemHasChildWithEmptyString(IQuery query)
		{
			query.Constrain(typeof (TestItem));
			ConstrainTestItemHasChildWithEmptyString(query);
			return query;
		}

		public static IConstraint ConstrainTestItemHasChildWithEmptyString(IQuery query)
		{
			return
					query.Descend("_childTestItems").Descend("_storedString").Constrain(string.Empty)
							.Equal();
		}

		[Test]
		public void TestItemWithNoChildren()
		{
			_bindingList.SodaQuery = TestItemWithNoChildren;
			//Console.WriteLine("Items with no children:");
			//foreach (TestItem item in _bindingList)
			//{
			//    Console.WriteLine("  {0}", item);
			//}
			Assert.AreEqual(1, _bindingList.Count);
			Assert.AreEqual("Third", _bindingList[0].StoredString);
		}

		public IQuery TestItemWithNoChildren(IQuery query)
		{
			query.Constrain(typeof (TestItem));
			ConstrainTestItemWithNoChildren(query);
			return query;
		}

		public static IConstraint ConstrainTestItemWithNoChildren(IQuery query)
		{
			return query.Descend("_childTestItems").Constrain(typeof (ChildTestItem)).Not();
		}

		[Test]
		[Ignore("Waiting for db4o fix")]
		public void TestItemWithNoChildrenOrWithChildWithEmptyString()
		{
			_bindingList.SodaQuery = TestItemWithNoChildrenOrWithChildWithEmptyString;
			//Console.WriteLine("Items with no children or having child with empty string:");
			//foreach (TestItem item in _bindingList)
			//{
			//    Console.WriteLine("  {0}", item);
			//}
			Assert.AreEqual(2, _bindingList.Count);
			Assert.AreEqual("Gianna", _bindingList[0].StoredString);
			Assert.AreEqual("Third", _bindingList[1].StoredString);
		}

		public IQuery TestItemWithNoChildrenOrWithChildWithEmptyString(IQuery query)
		{
			query.Constrain(typeof (TestItem));
			ConstrainTestItemHasChildWithEmptyString(query).Or(
					ConstrainTestItemWithNoChildren(query));
			return query;
		}

		// changing Query refreshes data
	}

	[TestFixture]
	public class db4oIssue
	{
		public class Time
		{
			public int Hour;
			public int Minute;

			public Time(int hour, int minute)
			{
				Hour = hour;
				Minute = minute;
			}

			public Time() {}

			public override string ToString()
			{
				return Hour + ":" + Minute;
			}
		}

		public class TestClass
		{
			public string Label;
			public List<Time> Times;

			public override string ToString()
			{
				string result = Label;
				foreach (Time time in Times)
				{
					result += " " + time;
				}
				return result;
			}
		}

		private IExtObjectContainer _db;
		private string _FilePath;

		[SetUp]
		public void SetUp()
		{
			_FilePath = Path.GetTempFileName();
			_db = Db4oFactory.OpenFile(_FilePath).Ext();

			TestClass testClass = new TestClass();
			testClass.Label = "Meal times";
			testClass.Times = new List<Time>();
			testClass.Times.Add(new Time(7, 30));
			testClass.Times.Add(new Time(12, 0));
			testClass.Times.Add(new Time(17, 20));
			_db.Set(testClass, 5);

			testClass = new TestClass();
			testClass.Label = "Free time";
			testClass.Times = new List<Time>();
			_db.Set(testClass, 5);

			testClass = new TestClass();
			testClass.Label = "Bed time";
			testClass.Times = new List<Time>();
			testClass.Times.Add(new Time(21, 30));
			_db.Set(testClass, 5);

			_db.Commit();
		}

		[TearDown]
		public void TearDown()
		{
			_db.Dispose();
			File.Delete(_FilePath);
		}

		[Test]
		public void AllTestClasses()
		{
			IQuery query = _db.Query();
			query.Constrain(typeof (TestClass));
			IObjectSet result = query.Execute();
			Assert.AreEqual(3, result.Count);
		}

		[Test]
		public void StoredStringEqualBedTime()
		{
			IQuery query = _db.Query();
			query.Constrain(typeof (TestClass));
			query.Descend("Label").Constrain("Bed time");

			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("Bed time", ((TestClass) result[0]).Label);
		}

		[Test]
		public void HasTime12oClock()
		{
			IQuery query = _db.Query();
			query.Constrain(typeof (TestClass));
			query.Descend("Times").Descend("Hour").Constrain(12).Equal();

			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("Meal times", ((TestClass) result[0]).Label);
		}

		[Test]
		public void HasNoTimes()
		{
			IQuery query = _db.Query();
			query.Constrain(typeof (TestClass));
			query.Descend("Times").Constrain(typeof (Time)).Not();

			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("Free time", ((TestClass) result[0]).Label);
		}

		[Test]
		[Ignore("Waiting for db4o fix")]
		public void HasTime12oClockOrHasNoTimes()
		{
			IQuery query = _db.Query();
			IConstraint hasTime12oClock =
					query.Descend("Times").Descend("Hour").Constrain(12).Equal();
			IConstraint hasNoTimes = query.Descend("Times").Constrain(typeof (Time)).Not();
			query.Constrain(typeof (TestClass));
			query.Constrain(hasTime12oClock.Or(hasNoTimes));

			IObjectSet result = query.Execute();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("Meal times", ((TestClass) result[0]).Label);
			Assert.AreEqual("Free times", ((TestClass) result[1]).Label);
		}
	}
}