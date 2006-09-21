using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.ComponentModel;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class IRecordProviderTests
	{
		InMemoryRecordList<SimpleIntTestClass> _sourceRecords;

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			_sourceRecords = new InMemoryRecordList<SimpleIntTestClass>();
			for (int i = 0; i < 50; i++)
			{
				_sourceRecords.Add(new SimpleIntTestClass(i));
			}
		}

		[Test]
		public void Create()
		{
			InMemoryRecordListManager<SimpleIntTestClass> recordListManager = new InMemoryRecordListManager<SimpleIntTestClass>(_sourceRecords);
			IRecordList<SimpleIntTestClass> data = recordListManager.Get();
			Assert.IsNotNull(data);
		}

		[Test]
		public void NoFilter_GivesAllRecords()
		{
			InMemoryRecordListManager<SimpleIntTestClass> recordListManager = new InMemoryRecordListManager<SimpleIntTestClass>(_sourceRecords);
			IRecordList<SimpleIntTestClass> data = recordListManager.Get();
			Assert.IsNotNull(data);
			Assert.AreEqual(_sourceRecords, data);
		}

		[Test]
		public void Filter_GivesFilterdRecords()
		{
			InMemoryRecordListManager<SimpleIntTestClass> recordListManager = new InMemoryRecordListManager<SimpleIntTestClass>(_sourceRecords);
			IRecordList<SimpleIntTestClass> data = recordListManager.Get(new SimpleIntFilter(11, 20));
			Assert.IsNotNull(data);
			Assert.AreEqual(10, data.Count);
			Assert.AreEqual(11, ((SimpleIntTestClass)data[0]).I);
			Assert.AreNotEqual(_sourceRecords, data);
		}

	}
}
