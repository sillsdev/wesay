using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class InMemoryRecordListManagerTests
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
			Assert.IsNotNull(recordListManager);
		}

		[Test]
		public void Get_SecondTime_GivesSameProvider()
		{
			InMemoryRecordListManager<SimpleIntTestClass> recordListManager = new InMemoryRecordListManager<SimpleIntTestClass>(_sourceRecords);
			IRecordList<SimpleIntTestClass> recordProvider = recordListManager.Get();
			Assert.AreSame(recordProvider, recordListManager.Get());
		}

		[Test]
		public void Get_SameFilter_GivesSameProvider()
		{
			InMemoryRecordListManager<SimpleIntTestClass> recordListManager = new InMemoryRecordListManager<SimpleIntTestClass>(_sourceRecords);
			SimpleIntFilter intFilter = new SimpleIntFilter(11, 20);
			IRecordList<SimpleIntTestClass> recordProvider = recordListManager.Get(intFilter);
			Assert.AreSame(recordProvider, recordListManager.Get(intFilter));
		}

		[Test]
		public void Get_DifferentFilter_GivesDifferentProvider()
		{
			InMemoryRecordListManager<SimpleIntTestClass> recordListManager = new InMemoryRecordListManager<SimpleIntTestClass>(_sourceRecords);
			IRecordList<SimpleIntTestClass> recordProvider = recordListManager.Get(new SimpleIntFilter(11, 20));
			Assert.AreNotSame(recordProvider, recordListManager.Get(new SimpleIntFilter(11, 12)));
		}


	}
}
