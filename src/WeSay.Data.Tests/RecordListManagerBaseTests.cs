using System;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	public abstract class RecordListManagerBaseTests
	{
		private IRecordListManager _recordListManager;
		private IRecordList<SimpleIntTestClass> _sourceRecords;
		private SimpleIntFilter _filter10to19;
		private SimpleIntFilter _filter11to12;
		private SimpleIntFilter _filter11to17;
		private SimpleIntFilter _filter11to20;

		protected IRecordListManager RecordListManager
		{
			get { return _recordListManager; }
		}

		protected SimpleIntFilter Filter10to19
		{
			get { return _filter10to19; }
		}

		protected SimpleIntFilter Filter11to12
		{
			get { return _filter11to12; }
		}

		protected SimpleIntFilter Filter11to17
		{
			get { return _filter11to17; }
		}

		protected SimpleIntFilter Filter11to20
		{
			get { return _filter11to20; }
		}

		public virtual void Setup()
		{
			_recordListManager = CreateRecordListManager();

			PopupateMasterRecordList();
			CreateFilters();
			RegisterFilters();
		}

		private void PopupateMasterRecordList()
		{
			_sourceRecords = RecordListManager.Get<SimpleIntTestClass>();
			if (_sourceRecords.Count != 50)
			{
				for (int i = 0; i < 50; i++)
				{
					_sourceRecords.Add(new SimpleIntTestClass(i));
				}
			}
		}

		private void RegisterFilters()
		{
			RecordListManager.Register<SimpleIntTestClass>(Filter10to19);
			RecordListManager.Register<SimpleIntTestClass>(Filter11to12);
			RecordListManager.Register<SimpleIntTestClass>(Filter11to17);
			RecordListManager.Register<SimpleIntTestClass>(Filter11to20);
		}

		private void CreateFilters()
		{
			_filter10to19 = new SimpleIntFilter(10, 19);
			_filter11to12 = new SimpleIntFilter(11, 12);
			_filter11to17 = new SimpleIntFilter(11, 17);
			_filter11to20 = new SimpleIntFilter(11, 20);
		}

		protected abstract IRecordListManager CreateRecordListManager();

		public virtual void TearDown()
		{
			RecordListManager.Dispose();
		}

		[Test]
		public void Create()
		{
			Assert.IsNotNull(RecordListManager);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void Get_UnregisteredFilter_ThrowsException()
		{
			RecordListManager.Get<SimpleIntTestClass>(new SimpleIntFilter(21, 30));
		}

		[Test]
		public void Get_NoFilter_GivesAllRecords()
		{
			IRecordList<SimpleIntTestClass> data = RecordListManager.Get<SimpleIntTestClass>();
			Assert.IsNotNull(data);
			Assert.AreEqual(_sourceRecords, data);
		}

		[Test]
		public void Get_Filter_GivesFilterdRecords()
		{
			IRecordList<SimpleIntTestClass> data = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			Assert.IsNotNull(data);
			Assert.AreEqual(10, data.Count);
			Assert.AreEqual(11, data[0].I);
			Assert.AreNotEqual(_sourceRecords, data);
		}

		[Test]
		public void Get_SecondTime_GivesSameRecordList()
		{
			IRecordList<SimpleIntTestClass> recordList = RecordListManager.Get<SimpleIntTestClass>();
			Assert.AreSame(recordList, RecordListManager.Get<SimpleIntTestClass>());
		}

		[Test]
		public void Get_SameFilter_GivesSameRecordList()
		{
			IRecordList<SimpleIntTestClass> recordList = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			Assert.AreSame(recordList, RecordListManager.Get<SimpleIntTestClass>(Filter11to20));
		}

		[Test]
		public void Get_DifferentFilter_GivesDifferentRecordList()
		{
			IRecordList<SimpleIntTestClass> recordList = RecordListManager.Get(Filter11to20);
			Assert.AreNotSame(recordList, RecordListManager.Get(Filter11to12));
		}

		[Test]
		public void ChangeRecord_NoLongerMeetsFilterCriteria_RemovedFromRecordList_StillInMaster()
		{
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			Assert.AreEqual(10, recordList11to20.Count);
			recordList11to20[0].I = 10;
			Assert.AreEqual(9, recordList11to20.Count);
			Assert.AreEqual(12, recordList11to20[0].I);
			Assert.AreEqual(50, _sourceRecords.Count);
		}

		[Test]
		public void ChangeRecord_NoLongerMeetsFilterCriteria_RemovedFromSimilarRecordList()
		{
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.Get<SimpleIntTestClass>(Filter11to17);
			recordList11to20[0].I = 10;
			Assert.AreEqual(6, recordList11to17.Count);
			Assert.AreEqual(12, recordList11to17[0].I);
			Assert.AreEqual(12, recordList11to20[0].I);
		}

		[Test]
		public void ChangeRecord_MeetsFilterCriteria_RemainsInSimilarRecordList()
		{
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.Get<SimpleIntTestClass>(Filter11to17);
			recordList11to20[0].I = 12;
			Assert.AreEqual(7, recordList11to17.Count);
			Assert.AreEqual(10, recordList11to20.Count);
			Assert.AreEqual(12, recordList11to17[0].I);
			Assert.AreEqual(12, recordList11to20[0].I);
		}

		[Test]
		public void AddRecordToFiltered_AddedToMaster()
		{
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			recordList11to20.Add(new SimpleIntTestClass(15));
			Assert.AreEqual(51, _sourceRecords.Count);
		}

		[Test]
		public void AddRecordToFiltered_AddedToRelevantRecordLists()
		{
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.Get<SimpleIntTestClass>(Filter11to17);
			recordList11to20.Add(new SimpleIntTestClass(15));
			Assert.AreEqual(8, recordList11to17.Count);
			Assert.AreEqual(11, recordList11to20.Count);
		}

		[Test]
		public void AddRecordToMaster_AddedToRelevantRecordLists()
		{
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.Get<SimpleIntTestClass>(Filter11to17);
			_sourceRecords.Add(new SimpleIntTestClass(15));
			Assert.AreEqual(8, recordList11to17.Count);
			Assert.AreEqual(11, recordList11to20.Count);
		}

		[Test]
		public void AddRecordToFiltered_DoesNotMeetFilterCriteria_AddedToMasterOnly()
		{
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			recordList11to20.Add(new SimpleIntTestClass(50));
			Assert.AreEqual(10, recordList11to20.Count);
			Assert.AreEqual(51, _sourceRecords.Count);
		}

		[Test]
		public void AddRecordToFiltered_AlreadyExists_NotAdded()
		{
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.Get<SimpleIntTestClass>(Filter11to20);
			recordList11to20.Add(_sourceRecords[12]);
			Assert.AreEqual(10, recordList11to20.Count);
			Assert.AreEqual(50, _sourceRecords.Count);
		}

		[Test]
		public void RemoveRecordFromMaster_RemovedFromFilteredRecordLists()
		{
			IRecordList<SimpleIntTestClass> recordList10to19 = RecordListManager.Get<SimpleIntTestClass>(Filter10to19);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.Get<SimpleIntTestClass>(Filter11to17);
			_sourceRecords.Remove(recordList10to19[1]);
			Assert.AreEqual(49, _sourceRecords.Count);
			Assert.AreEqual(12, _sourceRecords[11].I);
			Assert.AreEqual(6, recordList11to17.Count);
			Assert.AreEqual(12, recordList11to17[0].I);
			Assert.AreEqual(9, recordList10to19.Count);
			Assert.AreEqual(12, recordList10to19[1].I);
		}

		[Test]
		public void RemoveRecordFromFiltered_RemovedFromFilteredRecordListsAndMaster()
		{
			IRecordList<SimpleIntTestClass> recordList10to19 = RecordListManager.Get<SimpleIntTestClass>(Filter10to19);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.Get<SimpleIntTestClass>(Filter11to17);
			recordList10to19.Remove(recordList10to19[1]);
			Assert.AreEqual(6, recordList11to17.Count);
			Assert.AreEqual(12, recordList11to17[0].I);
			Assert.AreEqual(9, recordList10to19.Count);
			Assert.AreEqual(12, recordList10to19[1].I);
			Assert.AreEqual(49, _sourceRecords.Count);
			Assert.AreEqual(12, _sourceRecords[11].I);
		}
	}
}