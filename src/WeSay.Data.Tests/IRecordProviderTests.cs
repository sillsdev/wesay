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
			RecordProviderInventory recordProviderInventory = new RecordProviderInventory(_sourceRecords);
			IRecordProvider data = recordProviderInventory.Get<SimpleIntTestClass>();
			Assert.IsNotNull(data);
		}

		[Test]
		public void NoFilter_GivesAllRecords()
		{
			RecordProviderInventory recordProviderInventory = new RecordProviderInventory(_sourceRecords);
			IRecordProvider data = recordProviderInventory.Get <SimpleIntTestClass>();
			IBindingList records = data.Records;
			Assert.IsNotNull(records);
			Assert.AreSame(_sourceRecords, records);
		}

		[Test]
		public void Filter_GivesFilterdRecords()
		{
			RecordProviderInventory recordProviderInventory = new RecordProviderInventory(_sourceRecords);
			IRecordProvider data = recordProviderInventory.Get(new SimpleIntFilter(11, 20));
			IBindingList records = data.Records;
			Assert.IsNotNull(records);
			Assert.AreEqual(10, records.Count);
			Assert.AreEqual(11, ((SimpleIntTestClass)records[0]).I);
			Assert.AreNotEqual(_sourceRecords, records);
		}
	}
}
