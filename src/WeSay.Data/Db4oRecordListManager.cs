using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WeSay.Data
{
	public class Db4oRecordListManager : AbstractRecordListManager
	{
		private Db4oDataSource _dataSource;
		private string _dataPath;

		public Db4oRecordListManager(string filePath)
		: base()
		{
			_dataPath = Path.GetDirectoryName(filePath);
			_dataSource = new Db4oDataSource(filePath);
		}

		protected override IRecordList<T> CreateMasterRecordList<T>()
		{
			return new Db4oRecordList<T>(_dataSource);
		}

		protected override IRecordList<T> CreateFilteredRecordList<T>(IFilter<T> filter)
		{
			return new FilteredDb4oRecordList<T>(Get<T>(), filter, _dataPath, false);
		}

		protected override IRecordList<T> CreateFilteredRecordListUnlessSlow<T>(IFilter<T> filter)
		{
			IRecordList<T> recordList = null;
			try
			{
				recordList = new FilteredDb4oRecordList<T>(Get<T>(), filter, _dataPath, true);
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
				_dataSource.Dispose();
			}
		}

		class FilteredDb4oRecordList<T> : Db4oRecordList<T> where T : class, new()
		{
			IRecordList<T> _masterRecordList;
			bool _isSourceMasterRecord;
			IFilter<T> _isRelevantFilter;
			string _cachePath;
			bool _isInitializingFromCache;

			public Predicate<T> IsRelevant
			{
				get
				{
					return _isRelevantFilter.Inquire;
				}
			}

			public FilteredDb4oRecordList(IRecordList<T> sourceRecords, IFilter<T> filter, string dataPath, bool throwIfFilterNotOptimizable)
			: base((Db4oRecordList<T>)sourceRecords)
			{
				_cachePath = Path.Combine(dataPath, "Cache");
				_isRelevantFilter = filter;

				if (throwIfFilterNotOptimizable)
				{
					((Db4oList<T>)Records).ItemIds = new List<long>();
				}

				ApplyFilter(IsRelevant);

				if (throwIfFilterNotOptimizable)
				{
					_isInitializingFromCache = true;
					try
					{
						DeserializeRecordIds();
					}
					catch
					{
						Dispose();
						throw new OperationCanceledException();
					}
					_isInitializingFromCache = false;
				}
				_masterRecordList = sourceRecords;
				_masterRecordList.ListChanged += new ListChangedEventHandler(OnMasterRecordListListChanged);
				_masterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(OnMasterRecordListDeletingRecord);
			}

			private string CacheFilePath
			{
				get
				{
					return Path.Combine(_cachePath, _isRelevantFilter.Key + ".cache");
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

			protected override void OnItemChanged(int newIndex, string field)
			{
				base.OnItemChanged(newIndex, field);
				TriggerChangeInMaster(newIndex);
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

			void DeserializeRecordIds()
			{
				List<long> itemIds;
				using (FileStream fs = File.Open(CacheFilePath, FileMode.Open))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					try
					{
						itemIds = (List<long>)formatter.Deserialize(fs);
						((Db4oList<T>)Records).ItemIds = itemIds;
					}
					finally
					{
						fs.Close();
					}
				}
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
					if(IsRelevant(item))
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
				if (IsRelevant(item))
				{
					if (!Contains(item))
					{
						Add(item);
					}
				}
				else if (Contains(item))
				{
					Remove(item);
				}
			}

			private void HandleItemDeletedFromMaster(T item)
			{
				if (Contains(item))
				{
					Remove(item);
				}
			}

			private void HandleItemsClearedFromMaster(IRecordList<T> masterRecordList)
			{
				// The reset event can be raised when items are cleared and also when sorting or filtering occurs
				if (masterRecordList.Count == 0)
				{
					Clear();
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

	}
}
