using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Db4objects.Db4o.Ext;

namespace WeSay.Data
{
	public interface IDb4oModelConfiguration
	{
		void Configure();
	}

	public class DoNothingModelConfiguration : IDb4oModelConfiguration
	{
		public void Configure() {}
	}

	public class PrivateDb4oRecordListManager : IPrivateRecordListManager
	{
		private Db4oDataSource _dataSource;
		private string _cachePath;
		private bool _delayWritingCachesUntilDispose=false;
		private Hashtable _filteredRecordLists;
		private bool _disposed;

		public PrivateDb4oRecordListManager(IDb4oModelConfiguration config, string pathToDb4oFile)
		{
			_filteredRecordLists = new Hashtable();

			try
			{
				config.Configure();
				// _cachePath = pathToDb4oFile + " Cache";
				_cachePath = Path.GetDirectoryName(pathToDb4oFile); // use same dir
				_dataSource = new Db4oDataSource(pathToDb4oFile);
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		public Db4oDataSource DataSource
		{
			get { return _dataSource; }
		}

		public string CachePath
		{
			get { return _cachePath; }
		}

		public bool DelayWritingCachesUntilDispose
		{
			get
			{
				return _delayWritingCachesUntilDispose;
			}
			set
			{
				_delayWritingCachesUntilDispose = value;
				foreach (IControlCachingBehavior list in _filteredRecordLists)
				{
					list.DelayWritingCachesUntilDispose = value;
				}
			}
		}

		public bool IsDisposed
		{
			get
			{
				return _disposed;
			}
			private
					set
			{
				_disposed = value;
			}
		}

		protected Hashtable RecordLists
		{
			get { return this._filteredRecordLists; }
		}

		public List<RecordToken> GetSortedList<T>(ISortHelper<T> sortHelper) where T : class, new()
		{
			if (sortHelper == null)
			{
				throw new ArgumentNullException();
			}
			//string recordListKey = RecordListKey<T>(null, sortHelper.Name);
			//if (!RecordLists.ContainsKey(recordListKey))
			{
				List<RecordToken> recordTokens = sortHelper.GetRecordTokensForMatchingRecords();

				recordTokens.Sort(new RecordTokenComparer(sortHelper.KeyComparer));
				return recordTokens;

//                RecordLists.Add(recordListKey, recordTokens);
			}
//            return (List<RecordToken>)RecordLists[recordListKey];
		}

		private IRecordList<T> CreateMasterRecordList<T>() where T: class, new()
		{
			Db4oRecordList<T> recordList = new Db4oRecordList<T>(_dataSource);
			recordList.DelayWritingCachesUntilDispose = DelayWritingCachesUntilDispose;
			return recordList;
		}

		private IRecordList<T> CreateFilteredRecordList<T>(IFilter<T> filter,
																		   ISortHelper<T> sortHelper)
			 where T : class, new()
		{
			FilteredDb4oRecordList<T> list =
					new FilteredDb4oRecordList<T>(GetListOfType<T>(), filter, sortHelper, this, CachePath, false);
			return list;
		}

		private IRecordList<T> CreateFilteredRecordListUnlessSlow<T>(IFilter<T> filter,
																					 ISortHelper<T> sortHelper)
			 where T : class, new()
		{
			IRecordList<T> recordList = null;
			try
			{
				recordList = new FilteredDb4oRecordList<T>(GetListOfType<T>(), filter, sortHelper, this, CachePath, true);
			}
			catch (OperationCanceledException) {}
			return recordList;
		}


		internal class FilteredDb4oRecordList<T> : Db4oRecordList<T>, IBindingList where T : class, new()
		{
			private bool _isSorted;
			private IRecordList<T> _masterRecordList;
			private bool _isSourceMasterRecord;
			private IFilter<T> _isRelevantFilter;
			private string _cachePath;
			private bool _isInitializingFromCache;
			private ISortHelper<T> _sortHelper;
			private PrivateDb4oRecordListManager _recordListManager;

			public Predicate<T> RelevancePredicate
			{
				get { return _isRelevantFilter.FilteringPredicate; }
			}

			public FilteredDb4oRecordList(IRecordList<T> sourceRecords, IFilter<T> filter,
										  ISortHelper<T> sortedList, PrivateDb4oRecordListManager recordListManager,
										  string cachePath, bool constructOnlyIfFilterIsCached)
					: base((Db4oRecordList<T>) sourceRecords)
			{
				WriteCacheSize = 0;
				_cachePath = cachePath;
				_isRelevantFilter = filter;
				_sortHelper = sortedList;
				_masterRecordList = sourceRecords;
				_recordListManager = recordListManager;
				if (constructOnlyIfFilterIsCached)
				{
					((Db4oList<T>) Records).ItemIds.Clear();
				}

				DelayWritingCachesUntilDispose = MasterRecordList.DelayWritingCachesUntilDispose;

				ApplyFilter(RelevancePredicate);
				_isSorted = false;

				//At this point, either you have no records (either because constructOnlyIfFilterIsCached==true,
				// or there just are no records that fit the filter), or you have some and they satisfy the
				// filter.

				if (constructOnlyIfFilterIsCached)
				{
					List<long> itemIds;
					if (!TryGetDeserializedRecordIds(out itemIds))
					{
						_isInitializingFromCache = true;
						Dispose();
						throw new OperationCanceledException("Filter is not cached.");
					}
					if (itemIds == null)
					{
						((Db4oList<T>) Records).ItemIds.Clear();
					}
					else
					{
						((Db4oList<T>) Records).ItemIds = itemIds;
					}
				}
				else
				{
					SerializeRecordIds();
				}
				MasterRecordList.ListChanged += new ListChangedEventHandler(OnMasterRecordListListChanged);
				MasterRecordList.ContentOfItemInListChanged +=
						new ListChangedEventHandler(OnMasterRecordListContentOfItemInListChanged);
				MasterRecordList.DeletingRecord +=
						new EventHandler<RecordListEventArgs<T>>(OnMasterRecordListDeletingRecord);
			}

			// use the SortedList to determine the order of the filtered list items.
			//
			private void Sort()
			{
				int oldCount = Count;
				IList<RecordToken> sortedList = _recordListManager.GetSortedList(_sortHelper);

				((Db4oList<T>)Records).ItemIds.Sort(new IdListComparer(sortedList));
				Debug.Assert(oldCount == Count);
				OnListReset();
			}

			private class IdListComparer : IComparer<long>
			{
				private readonly Dictionary<long, int> _mapIdToIndex;

				public IdListComparer(IList<RecordToken> baseList)
				{
					_mapIdToIndex = new Dictionary<long, int>(baseList.Count);
					for(int i = 0; i < baseList.Count;++i)
					{
						_mapIdToIndex[((Db4oRepositoryId)baseList[i].Id).Db4oId] = i;
					}
				}

				#region IComparer<long> Members

				public int Compare(long x, long y)
				{
					return Comparer<int>.Default.Compare(_mapIdToIndex[x], _mapIdToIndex[y]);
				}

				#endregion
			}

			private string CacheFilePath
			{
				get { return Path.Combine(_cachePath, CacheHelper.EscapeFileNameString(_isRelevantFilter.Key) + ".cache"); }
			}

			public IRecordList<T> MasterRecordList
			{
				get { return _masterRecordList; }
			}

			protected override void OnItemContentChanged(int newIndex)
			{
				base.OnItemContentChanged(newIndex);
				TriggerChangeInMaster(newIndex);
			}

			private void TriggerChangeInMaster(int newIndex)
			{
				if (!_isSourceMasterRecord)
				{
					T item = this[newIndex];
					//trigger change in master (it may not be active in master and we need it to perculate to all filtered ones
					int i = MasterRecordList.IndexOf(item);
					MasterRecordList[i] = item;
				}
			}

			private void SerializeRecordIds()
			{
				try
				{
					if (IsFiltered)
					{
						if (!Directory.Exists(_cachePath))
						{
							Directory.CreateDirectory(_cachePath);
						}
						using (FileStream fs = File.Create(CacheFilePath))
						{
							BinaryFormatter formatter = new BinaryFormatter();
							try
							{
								formatter.Serialize(fs, GetDatabaseLastModified());
								formatter.Serialize(fs, GetFilterHashCode());
								VerifySorted();
								formatter.Serialize(fs, ((Db4oList<T>) Records).ItemIds);
							}
							finally
							{
								fs.Close();
							}
						}
					}
					else
					{
						File.Delete(CacheFilePath);
					}
				}
				catch {}
			}

			private int GetFilterHashCode()
			{
				byte[] bytes = ((Db4oList<T>) Records).Filter.Method.GetMethodBody().GetILAsByteArray();
				int hashCode = 0;
				for (int i = 0; i < bytes.Length; i++)
				{
					byte b = bytes[i];
					hashCode ^= b;
				}
				return hashCode;
			}

			private bool TryGetDeserializedRecordIds(out List<long> itemIds)
			{
				bool successful = false;
				itemIds = null;
				if (File.Exists(CacheFilePath))
				{
					using (FileStream fs = File.Open(CacheFilePath, FileMode.Open))
					{
						BinaryFormatter formatter = new BinaryFormatter();
						try
						{
							DateTime databaseLastModified = (DateTime) formatter.Deserialize(fs);
							if (databaseLastModified == GetDatabaseLastModified())
							{
								int filterHashCode = (int) formatter.Deserialize(fs);
								if (filterHashCode == GetFilterHashCode())
								{
									itemIds = (List<long>) formatter.Deserialize(fs);
									successful = true;
									_isSorted = true;
								}
							}
						}
						catch {}
						finally
						{
							fs.Close();
						}
					}
				}
				return successful;
			}

			private void OnMasterRecordListDeletingRecord(object sender, RecordListEventArgs<T> e)
			{
				_isSourceMasterRecord = true;
				HandleItemDeletedFromMaster(e.Item);
				_isSourceMasterRecord = false;
			}

			private void OnMasterRecordListContentOfItemInListChanged(object sender, ListChangedEventArgs e)
			{
				IRecordList<T> masterRecordList = (IRecordList<T>) sender;
				_isSourceMasterRecord = true;
				HandleItemChangedInMaster(masterRecordList[e.NewIndex]);
				_isSourceMasterRecord = false;
			}

			private void OnMasterRecordListListChanged(object sender, ListChangedEventArgs e)
			{
				IRecordList<T> masterRecordList = (IRecordList<T>) sender;
				VerifyNotDisposed();
				_isSourceMasterRecord = true;
				switch (e.ListChangedType)
				{
					case ListChangedType.ItemAdded:
						HandleItemAddedToMaster(masterRecordList[e.NewIndex]);
						break;
					case ListChangedType.ItemChanged:
						HandleItemChangedInMaster(masterRecordList[e.NewIndex]);
						break;
					case ListChangedType.ItemDeleted:
						break;
					case ListChangedType.Reset:
						HandleItemsClearedFromMaster(masterRecordList);
						break;
				}
				_isSourceMasterRecord = false;
			}

			protected override bool ShouldAddRecord(T item)
			{
				bool shouldAdd = base.ShouldAddRecord(item);
				if (shouldAdd)
				{
					if (RelevancePredicate(item))
					{
						shouldAdd = !Contains(item);
					}
					else
					{
						shouldAdd = false;
						if (!_isSourceMasterRecord && MasterRecordList != null)
						{
							MasterRecordList.Add(item);
						}
					}
				}
				if (shouldAdd)
				{
					_isSorted = false;
				}
				return shouldAdd;
			}

			protected override bool ShouldDeleteRecord(T item)
			{
				if (!_isSourceMasterRecord && MasterRecordList != null)
				{
					MasterRecordList.Remove(item);
				}
				return base.ShouldDeleteRecord(item);
			}

			private void HandleItemAddedToMaster(T item)
			{
				AddIfRelevantElseRemove(item);
			}

			private void HandleItemChangedInMaster(T item)
			{
				AddIfRelevantElseRemove(item);
			}

			private void AddIfRelevantElseRemove(T item)
			{
				if (RelevancePredicate(item))
				{
					if (!Contains(item))
					{
						Add(item);
					}
#if DEBUG
					if (!MasterRecordList.DelayWritingCachesUntilDispose)
					{
						SerializeRecordIds();
					}
#endif
				}
				else if (Contains(item))
				{
					int index = IndexOf(item);
					((Db4oList<T>) Records).Refresh(item);
					OnItemDeleted(index);
#if DEBUG
					if (!MasterRecordList.DelayWritingCachesUntilDispose)
					{
						SerializeRecordIds();
					}
#endif
				}
			}

			private void HandleItemDeletedFromMaster(T item)
			{
				if (Contains(item))
				{
					int index = Records.IndexOf(item);
					if (index != -1)
					{
						((Db4oList<T>) Records).ItemIds.RemoveAt(index);
						OnItemDeleted(index);
					}

//                    Remove(item);
#if DEBUG
					if (!MasterRecordList.DelayWritingCachesUntilDispose)
					{
						SerializeRecordIds();
					}
#endif
				}
			}

			private void HandleItemsClearedFromMaster(IRecordList<T> masterRecordList)
			{
				// The reset event can be raised when items are cleared and also when sorting or filtering occurs
				if (masterRecordList.Count == 0)
				{
					Clear();
#if DEBUG
					if (!MasterRecordList.DelayWritingCachesUntilDispose)
					{
						SerializeRecordIds();
					}
#endif
				}
			}

			protected override void OnItemAdded(int newIndex)
			{
				_isSorted = true; //allow me to get the record associated with newIndex
				//without triggering a sort and changing it from out
				//from under me
				if (!_isSourceMasterRecord && MasterRecordList != null)
				{
					MasterRecordList.Add(this[newIndex]);
				}
				_isSorted = false; // may have lost sort order.
				base.OnItemAdded(newIndex);
			}

			//public string GetKey(int index)
			//{
			//    long id = ((Db4oList<T>) Records).ItemIds[index];
			//    return _sortedList.GetKeyFromId(id);
			//}

			public T GetValue(int index)
			{
				return this[index];
			}

			// returns the value object. use GetKey to return the Key
			object IList.this[int index]
			{
				get
				{
					VerifyNotDisposed();
					VerifySorted();
					return GetValue(index);
				}
				set
				{
					VerifyNotDisposed();
					throw new NotSupportedException();
				}
			}

			public override int IndexOf(T item)
			{
				VerifyNotDisposed();
//                VerifySorted();
				return base.IndexOf(item);
			}

			int IList.IndexOf(object value)
			{
				VerifyNotDisposed();
				VerifySorted();
				return IndexOf((T) value);
			}

			protected override bool ShouldReplaceRecord(int index, T value)
			{
				_isSorted = false;
				return true;
			}

			public override T this[int index]
			{
				get
				{
					VerifyNotDisposed();
					VerifySorted();
					return base[index];
				}
				set
				{
					VerifyNotDisposed();
					_isSorted = false;
					base[index] = value;
				}
			}

			private void VerifySorted()
			{
				if (!_isSorted)
				{
					_isSorted = true; // this needs to come before the so
					Sort();
				}
			}

			#region IDisposable Members

			protected override void Dispose(bool disposing)
			{
				if (!IsDisposed)
				{
					if (disposing)
					{
						// dispose-only, i.e. non-finalizable logic
						if (MasterRecordList != null)
						{
							MasterRecordList.ListChanged -= OnMasterRecordListListChanged;
							_masterRecordList = null;
						}
						if (!_isInitializingFromCache)
						{
							SerializeRecordIds();
						}
					}
				}
				base.Dispose(disposing);
			}

			#endregion
		}

		private bool CommitIfNeeded()
		{
			//right now we don't check to see if committing is needed
			_dataSource.Data.Commit();
			return true;
		}

		public T1 GetItem<T1>(long id)
		{
			IExtObjectContainer database = this._dataSource.Data.Ext();
			T1 item = (T1) database.GetByID(id);
			if(item != null)
			{
				if (!database.IsActive(item))
				{
					database.Activate(item, int.MaxValue);
				}
			}
			return item;
		}

		protected static string RecordListKey<T>(string filterName, string sorterName) where T : class, new()
		{
			string prefix = "!"; // for normal filtered, sorted lists
			if(string.IsNullOrEmpty(filterName))
			{
				if (string.IsNullOrEmpty(sorterName))
				{
					// this is our master record list
					prefix = "#";
				}
				else
				{
					// this is a master sorted list
					prefix = "@";
				}
			}
			else
			{
				if (string.IsNullOrEmpty(sorterName))
				{
					throw new ArgumentOutOfRangeException("sorterName","cannot be null or empty when filterName is present");
				}
			}

			return prefix + typeof(T).FullName + filterName + sorterName;
		}

		public static bool IsMasterRecordList(DictionaryEntry dictionaryEntry)
		{
			return ((string) dictionaryEntry.Key).StartsWith("#");
		}

		public static bool IsMasterSortedRecordList(DictionaryEntry dictionaryEntry)
		{
			return ((string)dictionaryEntry.Key).StartsWith("@");
		}

		public static bool IsFilteredRecordList(DictionaryEntry dictionaryEntry)
		{
			return ((string)dictionaryEntry.Key).StartsWith("!");
		}

		public void Register<T>(IFilter<T> filter, ISortHelper<T> sortHelper) where T : class, new()
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (sortHelper == null)
			{
				throw new ArgumentNullException("sortHelper");
			}

			string recordListKey = RecordListKey<T>(filter.Key, sortHelper.Name);
			if (!RecordLists.ContainsKey(recordListKey))
			{
				RecordLists.Add(recordListKey, CreateFilteredRecordListUnlessSlow(filter, sortHelper));
			}
		}

		public IRecordList<T> GetListOfType<T>() where T : class, new()
		{
			string recordListKey = RecordListKey<T>(null, null);
			if (!RecordLists.ContainsKey(recordListKey))
			{
				IRecordList<T> MasterRecordList = CreateMasterRecordList<T>();
				MasterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(MasterRecordList_DeletingRecord<T>);
				RecordLists.Add(recordListKey, MasterRecordList);
			}
			return (IRecordList<T>)RecordLists[recordListKey];
		}

		private void MasterRecordList_DeletingRecord<T>(object sender, RecordListEventArgs<T> e) where T : class, new()
		{
			DataDeleted.Invoke(this, new DeletedItemEventArgs(e.Item));
		}

		public IRecordList<T> GetListOfTypeFilteredFurther<T>(IFilter<T> filter, ISortHelper<T> sortHelper) where T : class, new()
		{
			if(filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (sortHelper == null)
			{
				throw new ArgumentNullException("sortHelper");
			}
			string recordListKey = RecordListKey<T>(filter.Key, sortHelper.Name);
			if (!RecordLists.ContainsKey(recordListKey))
			{
				throw new InvalidOperationException("Filter must be registered before it can be retrieved with GetListOfType.");
			}
			IRecordList<T> recordList = (IRecordList<T>)RecordLists[recordListKey];
			if (recordList == null)
			{
				recordList = CreateFilteredRecordList(filter, sortHelper);
				RecordLists[recordListKey] = recordList;
			}
			return recordList;
		}

		~PrivateDb4oRecordListManager()
		{
			if (!this._disposed)
			{
				throw new InvalidOperationException("Disposed not explicitly called on " + GetType().FullName + ".");
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic
					// we need to dispose masters last
					foreach (DictionaryEntry dictionaryEntry in RecordLists)
					{
						if (IsFilteredRecordList(dictionaryEntry))
						{
							IDisposable disposable = dictionaryEntry.Value as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
					foreach (DictionaryEntry dictionaryEntry in RecordLists)
					{
						if (IsMasterSortedRecordList(dictionaryEntry))
						{
							IDisposable disposable = dictionaryEntry.Value as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}

					foreach (DictionaryEntry dictionaryEntry in RecordLists)
					{
						if (IsMasterRecordList(dictionaryEntry))
						{
							IDisposable disposable = dictionaryEntry.Value as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}

					_filteredRecordLists = null;
				}
				if (DataSource != null)
				{
					DataSource.Dispose();
				}

				// shared (dispose and finalizable) cleanup logic
				IsDisposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		public event EventHandler DataCommitted = delegate {};

		public event EventHandler<DeletedItemEventArgs> DataDeleted = delegate
																	  {
																	  };

		public void GoodTimeToCommit()
		{
			if (CommitIfNeeded())
			{
				DataCommitted.Invoke(this, null);
			}
		}
	}
}