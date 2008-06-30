using System.Collections.Generic;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
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