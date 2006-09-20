using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class RecordProviderInventoryTests
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
			Assert.IsNotNull(recordProviderInventory);
		}

		[Test]
		public void Get_SecondTime_GivesSameProvider()
		{
			RecordProviderInventory recordProviderInventory = new RecordProviderInventory(_sourceRecords);
			IRecordProvider recordProvider = recordProviderInventory.Get<SimpleIntTestClass>();
			Assert.AreSame(recordProvider, recordProviderInventory.Get<SimpleIntTestClass>());
		}

		[Test]
		public void Get_SameFilter_GivesSameProvider()
		{
			RecordProviderInventory recordProviderInventory = new RecordProviderInventory(_sourceRecords);
			SimpleIntFilter intFilter = new SimpleIntFilter(11, 20);
			IRecordProvider recordProvider = recordProviderInventory.Get(intFilter);
			Assert.AreSame(recordProvider, recordProviderInventory.Get<SimpleIntTestClass>(intFilter));
		}

		[Test]
		public void Get_DifferentFilter_GivesDifferentProvider()
		{
			RecordProviderInventory recordProviderInventory = new RecordProviderInventory(_sourceRecords);
			IRecordProvider recordProvider = recordProviderInventory.Get(new SimpleIntFilter(11, 20));
			Assert.AreNotSame(recordProvider, recordProviderInventory.Get<SimpleIntTestClass>(new SimpleIntFilter(11, 12)));
		}


	}
}
