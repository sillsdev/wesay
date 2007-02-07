using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oRecordListManagerTests : RecordListManagerBaseTests
	{
		private string _filePath;

		[SetUp]
		public override void Setup()
		{
			_filePath = System.IO.Path.GetTempFileName();
			ForceCreateFilterCaches();
			base.Setup();
		}

		private void ForceCreateFilterCaches() {
			base.Setup();
			RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter10to19);
			RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to12);
			RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to20);
			base.TearDown();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();

			string cacheDirectory = Path.Combine(Path.GetDirectoryName(this._filePath), "Cache");
			if(Directory.Exists(cacheDirectory))
			{
				Directory.Delete(cacheDirectory, true);
			}
			System.IO.File.Delete(_filePath);
		}

		protected override IRecordListManager CreateRecordListManager()
		{
			return new Db4oRecordListManager(new DoNothingModelConfiguration(), _filePath);
		}

		[Test]
		public void ChangeRecord_NoLongerMeetsFilterCriteria_RemovedFromCachedRecordLists()
		{
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			int index11 = masterRecordList.Find(SimpleIntTestClass.IPropertyDescriptor, 11);
			masterRecordList[index11].I = 10;
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			Assert.AreEqual(6, recordList11to17.Count);

			int recordList11to17Count11 = CountMatching<SimpleIntTestClass>(recordList11to17,
									delegate(SimpleIntTestClass item)
									{
										return item.I == 11;
									});
			Assert.AreEqual(0, recordList11to17Count11);

			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to20);
			Assert.AreEqual(9, recordList11to20.Count);
			int recordList11to20Count11 = CountMatching<SimpleIntTestClass>(recordList11to20,
									delegate(SimpleIntTestClass item)
									{
										return item.I == 11;
									});
			Assert.AreEqual(0, recordList11to20Count11);
		}

		[Test]
		public void ChangeRecord_NoLongerMeetsFilterCriteria_RemainsInMasterRecordLists()
		{
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			int index11 = masterRecordList.Find(SimpleIntTestClass.IPropertyDescriptor, 11);
			masterRecordList[index11].I = 10;

			Assert.AreEqual(50, masterRecordList.Count);

			int Count11 = CountMatching<SimpleIntTestClass>(masterRecordList,
									delegate(SimpleIntTestClass item)
									{
										return item.I == 11;
									});
			Assert.AreEqual(0, Count11);

			int Count10 = CountMatching<SimpleIntTestClass>(masterRecordList,
									delegate(SimpleIntTestClass item)
									{
										return item.I == 10;
									});
			Assert.AreEqual(2, Count10);
		}

		[Test]
		public void ChangeRecord_NoLongerMeetsFilterCriteria_RemovedFromNotifyListSoCanChangeAgain()
		{
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to20);
			SimpleIntTestClass record = recordList11to20[0];
			record.I = 10;
			RecordListManager.GoodTimeToCommit();
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			int Count10 = CountMatching<SimpleIntTestClass>(masterRecordList,
						delegate(SimpleIntTestClass item)
						{
							return item.I == 10;
						});
			Assert.AreEqual(2, Count10);

			record.I = 11;

			int Count11 = CountMatching<SimpleIntTestClass>(masterRecordList,
						delegate(SimpleIntTestClass item)
						{
							return item.I == 11;
						});
			Assert.AreEqual(1, Count11);

		}


		[Test]
		public void ChangeRecord_MeetsFilterCriteria_AddedToCachedRecordList()
		{
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			masterRecordList[0].I = 12;
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to20);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			Assert.AreEqual(8, recordList11to17.Count);
			Assert.AreEqual(11, recordList11to20.Count);

			int recordList11to17Count12 = CountMatching<SimpleIntTestClass>(recordList11to17,
						delegate(SimpleIntTestClass item)
						{
							return item.I == 12;
						});
			Assert.AreEqual(2, recordList11to17Count12);
			int recordList11to20Count12 = CountMatching<SimpleIntTestClass>(recordList11to20,
									delegate(SimpleIntTestClass item)
									{
										return item.I == 12;
									});
			Assert.AreEqual(2, recordList11to20Count12);
		}

		[Test]
		public void AddRecord_AddedToCachedRecordLists()
		{
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			masterRecordList.Add(new SimpleIntTestClass(15));
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to20);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			Assert.AreEqual(8, recordList11to17.Count);
			Assert.AreEqual(11, recordList11to20.Count);
		}

		[Test]
		public void RemoveRecord_RemovedFromCachedRecordLists()
		{
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			int index11 = masterRecordList.Find(SimpleIntTestClass.IPropertyDescriptor, 11);
			masterRecordList.RemoveAt(index11);
			IRecordList<SimpleIntTestClass> recordList10to19 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter10to19);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			Assert.AreEqual(6, recordList11to17.Count);
			int recordList11to17Count11 = CountMatching<SimpleIntTestClass>(recordList11to17,
						delegate(SimpleIntTestClass item)
						{
							return item.I == 11;
						});
			Assert.AreEqual(0, recordList11to17Count11);

			Assert.AreEqual(9, recordList10to19.Count);
			int recordList10to19Count11 = CountMatching<SimpleIntTestClass>(recordList10to19,
			delegate(SimpleIntTestClass item)
			{
				return item.I == 11;
			});
			Assert.AreEqual(0, recordList10to19Count11);
		}

		private delegate void ChangeRecordList<T>(IRecordList<T> recordList) where T:class, new();

		private void ChangeDatabaseOutFromUnderRecordListManager(ChangeRecordList<SimpleIntTestClass> change)
		{
			base.TearDown();
			using(Db4oDataSource dataSource = new Db4oDataSource(_filePath))
			{
				using (Db4oRecordList<SimpleIntTestClass> recordList = new Db4oRecordList<SimpleIntTestClass>(dataSource))
				{
					change(recordList);
				}
			}
			base.Setup();
		}

		[Test]
		public void SomeoneElseChangedRecord_NoLongerMeetsFilterCriteria_RemovedFromCachedRecordLists()
		{
			ChangeDatabaseOutFromUnderRecordListManager(delegate(IRecordList<SimpleIntTestClass> recordList)
														{
															int index11 = recordList.Find(SimpleIntTestClass.IPropertyDescriptor, 11);
															recordList[index11].I = 10;
														});
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);

			int recordList11to17Count11 = CountMatching<SimpleIntTestClass>(recordList11to17,
			delegate(SimpleIntTestClass item)
			{
				return item.I == 11;
			});
			Assert.AreEqual(0, recordList11to17Count11);
			Assert.AreEqual(6, recordList11to17.Count);

			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to20);
			int recordList11to20Count11 = CountMatching<SimpleIntTestClass>(recordList11to20,
													delegate(SimpleIntTestClass item)
													{
														return item.I == 11;
													});

			Assert.AreEqual(0, recordList11to20Count11);
			Assert.AreEqual(9, recordList11to20.Count);
		}

		[Test]
		public void SomeoneElseChangedRecord_MeetsFilterCriteria_AddedToCachedRecordList()
		{
			ChangeDatabaseOutFromUnderRecordListManager(delegate(IRecordList<SimpleIntTestClass> recordList)
														{
															recordList[0].I = 12;
														});
			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to20);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			Assert.AreEqual(8, recordList11to17.Count);
			Assert.AreEqual(11, recordList11to20.Count);
			int recordList11to17Count12 = CountMatching<SimpleIntTestClass>(recordList11to17,
			delegate(SimpleIntTestClass item)
			{
				return item.I == 12;
			});
			Assert.AreEqual(2, recordList11to17Count12);

			int recordList11to20Count12 = CountMatching<SimpleIntTestClass>(recordList11to20,
																delegate(SimpleIntTestClass item)
																{
																	return item.I == 12;
																});
			Assert.AreEqual(2, recordList11to20Count12);
		}

		[Test]
		public void SomeoneElseAddedRecord_AddedToCachedRecordLists()
		{
			ChangeDatabaseOutFromUnderRecordListManager(delegate(IRecordList<SimpleIntTestClass> recordList)
											{
												recordList.Add(new SimpleIntTestClass(15));
											});

			IRecordList<SimpleIntTestClass> recordList11to20 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to20);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			int recordList11to17Count15 = CountMatching<SimpleIntTestClass>(recordList11to17,
										delegate(SimpleIntTestClass item)
										{
											return item.I == 15;
										});
			Assert.AreEqual(2, recordList11to17Count15);

			Assert.AreEqual(8, recordList11to17.Count);

			Assert.AreEqual(11, recordList11to20.Count);
			int recordList11to20Count15 = CountMatching<SimpleIntTestClass>(recordList11to20,
													delegate(SimpleIntTestClass item)
													{
														return item.I == 15;
													});
			Assert.AreEqual(2, recordList11to20Count15);
		}

		[Test]
		public void SomeoneElseRemovedRecord_RemovedFromCachedRecordLists()
		{
			ChangeDatabaseOutFromUnderRecordListManager(delegate(IRecordList<SimpleIntTestClass> recordList)
											{
												int index11 = recordList.Find(SimpleIntTestClass.IPropertyDescriptor, 11);

												recordList.RemoveAt(index11);
											});

			IRecordList<SimpleIntTestClass> recordList10to19 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter10to19);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			int recordList11to17Count11 = CountMatching<SimpleIntTestClass>(recordList11to17,
																			delegate(SimpleIntTestClass item)
																			{
																				return item.I == 11;
																			});
			Assert.AreEqual(0, recordList11to17Count11);
			Assert.AreEqual(6, recordList11to17.Count);

			int recordList10to19Count11 = CountMatching<SimpleIntTestClass>(recordList10to19,
																			delegate(SimpleIntTestClass item)
																			{
																				return item.I == 11;
																			});
			Assert.AreEqual(0, recordList10to19Count11);
			Assert.AreEqual(9, recordList10to19.Count);
		}
	}

	[TestFixture]
	public class Db4oRecordListManagerCacheTests
	{
		private string _filePath;
		private IRecordListManager _recordListManager;
		private IRecordList<SimpleIntTestClass> _sourceRecords;
		private SimpleIntFilter _filter11to17;

		protected IRecordListManager RecordListManager
		{
			get
			{
				return _recordListManager;
			}
		}

		protected SimpleIntFilter Filter11to17
		{
			get
			{
				return _filter11to17;
			}
		}

		[SetUp]
		public void Setup()
		{
			_filePath = System.IO.Path.GetTempFileName();
			_recordListManager = new Db4oRecordListManager(new DoNothingModelConfiguration(), _filePath);
			PopupateMasterRecordList();

			_filter11to17 = new SimpleIntFilter(11, 17);
			Filter11to17.UseInverseFilter = true;
			RecordListManager.Register<SimpleIntTestClass>(Filter11to17);
			RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			RecordListManager.Dispose();
			_recordListManager = new Db4oRecordListManager(new DoNothingModelConfiguration(), _filePath);

			_filter11to17 = new SimpleIntFilter(11, 17);
			RecordListManager.Register<SimpleIntTestClass>(Filter11to17);

		}

		private void PopupateMasterRecordList()
		{
			_sourceRecords = RecordListManager.GetListOfType<SimpleIntTestClass>();
			for (int i = 0; i < 50; i++)
			{
				_sourceRecords.Add(new SimpleIntTestClass(i));
			}
		}

		[TearDown]
		public void TearDown()
		{
			RecordListManager.Dispose();

			string cacheDirectory = Path.Combine(Path.GetDirectoryName(this._filePath), "Cache");
			if (Directory.Exists(cacheDirectory))
			{
				Directory.Delete(cacheDirectory, true);
			}
			System.IO.File.Delete(_filePath);
		}
		private int CountMatching<T>(IEnumerable<T> enumerable, Predicate<T> match)
		{
			int count = 0;
			foreach (T t in enumerable)
			{
				if(match(t))
				{
					count++;
				}
			}
			return count;
		}

		[Test]
		public void FilterModified_ChangedRecord_NoLongerMeetsFilterCriteria_RemovedFromCachedRecordLists()
		{
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			int index11 = masterRecordList.Find(SimpleIntTestClass.IPropertyDescriptor, 11);
			masterRecordList[index11].I = 10;
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			Assert.AreEqual(6, recordList11to17.Count);
			int count11 = CountMatching<SimpleIntTestClass>(recordList11to17,
															delegate(SimpleIntTestClass item)
															{
																return item.I == 11;
															});
			Assert.AreEqual(0, count11);
		}

		[Test]
		public void FilterModified_ChangedRecord_MeetsFilterCriteria_AddedToCachedRecordList()
		{
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			masterRecordList[0].I = 12;
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			Assert.AreEqual(8, recordList11to17.Count);
			int count12 = CountMatching<SimpleIntTestClass>(recordList11to17,
												delegate(SimpleIntTestClass item)
												{
													return item.I == 12;
												});
			Assert.AreEqual(2, count12);
		}

		[Test]
		public void FilterModified_AddedRecord_AddedToCachedRecordLists()
		{
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			masterRecordList.Add(new SimpleIntTestClass(15));
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			Assert.AreEqual(8, recordList11to17.Count);
			int count15 = CountMatching<SimpleIntTestClass>(recordList11to17,
												delegate(SimpleIntTestClass item)
												{
													return item.I == 15;
												});
			Assert.AreEqual(2, count15);

		}

		[Test]
		public void FilterModified_RemovedRecord_RemovedFromCachedRecordLists()
		{
			IRecordList<SimpleIntTestClass> masterRecordList = RecordListManager.GetListOfType<SimpleIntTestClass>();
			int index11 = masterRecordList.Find(SimpleIntTestClass.IPropertyDescriptor, 11);
			masterRecordList.RemoveAt(index11);
			IRecordList<SimpleIntTestClass> recordList11to17 = RecordListManager.GetListOfTypeFilteredFurther<SimpleIntTestClass>(Filter11to17);
			Assert.AreEqual(6, recordList11to17.Count);
			int count11 = CountMatching<SimpleIntTestClass>(recordList11to17,
												delegate(SimpleIntTestClass item)
												{
													return item.I == 11;
												});
			Assert.AreEqual(0, count11);
		}
	}
}
