using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace WeSay.Data
{
	public interface IDb4oModelConfiguration
	{
		void Configure();
	}
	public class DoNothingModelConfiguration : IDb4oModelConfiguration
	{
		public void Configure()
		{
		}
	}

	public class Db4oRecordListManager : AbstractRecordListManager
	{
		private Db4oDataSource _dataSource;
		private string _cachePath;

		public Db4oRecordListManager(IDb4oModelConfiguration config, string pathToDb4oFile)
		: base()
		{
			try
			{
				config.Configure();
			   // _cachePath = pathToDb4oFile + " Cache";
				_cachePath = Path.GetDirectoryName(pathToDb4oFile); // use same dir
				_dataSource = new Db4oDataSource(pathToDb4oFile);
			}
			catch(Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public Db4oDataSource DataSource
		{
			get { return this._dataSource; }
		}

		public string CachePath
		{
			get { return this._cachePath; }
		}

		public CachedSortedDb4oList<K, T> GetSortedList<K, T>(IDb4oSortHelper<K, T> sortHelper) where T : class, new()
		{
			string key = sortHelper.Name;
			if (!FilteredRecordLists.ContainsKey(key))
			{
				FilteredRecordLists.Add(key, new CachedSortedDb4oList<K, T>(this, sortHelper));
			}
			return (CachedSortedDb4oList<K, T>)FilteredRecordLists[key];
		}
		protected override IRecordList<T> CreateMasterRecordList<T>()
		{
			return new Db4oRecordList<T>(this._dataSource);
		}

		protected override IRecordList<T> CreateFilteredRecordList<T>(IFilter<T> filter)
		{
			FilteredDb4oRecordList<T> list = new FilteredDb4oRecordList<T>(GetListOfType<T>(), filter, CachePath, false);
			list.DelayWritingCachesUntilDispose = this.DelayWritingCachesUntilDispose;
			return list;
		}

		protected override IRecordList<T> CreateFilteredRecordListUnlessSlow<T>(IFilter<T> filter)
		{
			IRecordList<T> recordList = null;
			try
			{
				recordList = new FilteredDb4oRecordList<T>(GetListOfType<T>(), filter, CachePath, true);
				recordList.DelayWritingCachesUntilDispose = this.DelayWritingCachesUntilDispose;
			}
			catch (OperationCanceledException) {}
			return recordList;
		}

		protected override void Dispose(bool disposing)
		{
			bool canBeDisposed = !IsDisposed;
			base.Dispose(disposing);
			if (canBeDisposed && disposing)
			{
				if (DataSource != null)
				{
					DataSource.Dispose();
				}
			}
		}

		class FilteredDb4oRecordList<T> : Db4oRecordList<T> where T : class, new()
		{
			IRecordList<T> _masterRecordList;
			bool _isSourceMasterRecord;
			IFilter<T> _isRelevantFilter;
			string _cachePath;
			bool _isInitializingFromCache;

			public Predicate<T> RelevancePredicate
			{
				get
				{
					return _isRelevantFilter.FilteringPredicate;
				}
			}

			public FilteredDb4oRecordList(IRecordList<T> sourceRecords, IFilter<T> filter, string cachePath, bool constructOnlyIfFilterIsCached)
			: base((Db4oRecordList<T>)sourceRecords)
			{
				this.WriteCacheSize = 0;
				_cachePath = cachePath;
				_isRelevantFilter = filter;

				if (constructOnlyIfFilterIsCached)
				{
					((Db4oList<T>)Records).ItemIds.Clear();
				}

				ApplyFilter(RelevancePredicate);

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
						((Db4oList<T>)Records).ItemIds.Clear();
					}
					else
					{
						((Db4oList<T>)Records).ItemIds = itemIds;
					}
				}
				else
				{
					SerializeRecordIds();
				}
				_masterRecordList = sourceRecords;
				_masterRecordList.ListChanged += new ListChangedEventHandler(OnMasterRecordListListChanged);
				_masterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(OnMasterRecordListDeletingRecord);
			}

			private string CacheFilePath
			{
				get
				{
					return Path.Combine(_cachePath, CacheHelper.EscapeFileNameString(_isRelevantFilter.Key) + ".cache");
				}
			}

			protected override void OnItemChanged(int newIndex)
			{
				base.OnItemChanged(newIndex);
				TriggerChangeInMaster(newIndex);
			}

			private void TriggerChangeInMaster(int newIndex) {
				T item = this[newIndex];
				//trigger change in master (it may not be active in master and we need it to perculate to all filtered ones
				int i = this._masterRecordList.IndexOf(item);
				this._masterRecordList[i] = item;
			}

			void SerializeRecordIds()
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
								formatter.Serialize(fs, ((Db4oList<T>)Records).ItemIds);
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
				catch
				{
				}
			}

			private int GetFilterHashCode() {
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
							DateTime databaseLastModified = (DateTime)formatter.Deserialize(fs);
							if (databaseLastModified == GetDatabaseLastModified())
							{
								int filterHashCode = (int)formatter.Deserialize(fs);
								if (filterHashCode == GetFilterHashCode())
								{
									itemIds = (List<long>)formatter.Deserialize(fs);
									successful = true;
								}
							}
						}
						catch{}
						finally
						{
							fs.Close();
						}
					}
				}
				return successful;
			}

			void OnMasterRecordListDeletingRecord(object sender, RecordListEventArgs<T> e)
			{
				_isSourceMasterRecord = true;
				HandleItemDeletedFromMaster(e.Item);
				_isSourceMasterRecord = false;
			}

			void OnMasterRecordListListChanged(object sender, ListChangedEventArgs e)
			{
				IRecordList<T> masterRecordList = (IRecordList<T>)sender;
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
					if(RelevancePredicate(item))
					{
						shouldAdd = !Contains(item);
					}
					else
					{
						shouldAdd = false;
						if (!_isSourceMasterRecord && _masterRecordList != null)
						{
							_masterRecordList.Add(item);
						}
					}

				}
				return shouldAdd;
			}
			protected override bool ShouldDeleteRecord(T item)
			{
				if (!_isSourceMasterRecord && _masterRecordList != null)
				{
					_masterRecordList.Remove(item);
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
					SerializeRecordIds();
#endif
				}
				else if (Contains(item))
				{
					int index = IndexOf(item);
					((Db4oList<T>)Records).Refresh(item);
					OnItemDeleted(index);
#if DEBUG
					SerializeRecordIds();
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
						((Db4oList<T>)Records).ItemIds.RemoveAt(index);
						OnItemDeleted(index);
					}

//                    Remove(item);
#if DEBUG
					SerializeRecordIds();
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
					SerializeRecordIds();
#endif
				}
			}

			protected override void OnItemAdded(int newIndex)
			{
				base.OnItemAdded(newIndex);
				if (!_isSourceMasterRecord && _masterRecordList != null)
				{
					_masterRecordList.Add(this[newIndex]);
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
						if (_masterRecordList != null)
						{
							_masterRecordList.ListChanged -= OnMasterRecordListListChanged;
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

		protected override bool CommitIfNeeded()
		{
			//right now we don't check to see if committing is needed
			_dataSource.Data.Commit();
			return true;
		}
	}
}
