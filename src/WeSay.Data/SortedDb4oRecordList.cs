using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WeSay.Data
{
	public class CachedSortedDb4oList<K, T>: IBindingList, IEnumerable<K>, IDisposable where T : class, new()
	{
		public delegate K KeyProvider(T item);

		class Comparer: IComparer<KeyValuePair<K, long>>{
			IComparer<K> _sorter;

			public Comparer(IComparer<K> sorter)
			{
				_sorter = sorter;
			}
			#region IComparer<KeyValuePair<K,long>> Members

			public int Compare(KeyValuePair<K, long> x, KeyValuePair<K, long> y)
			{
				int result = _sorter.Compare(x.Key, y.Key);
				if(result == 0)
				{
					if(x.Value < y.Value)
					{
						return -1;
					}
					if(x.Value > y.Value)
					{
						return 1;
					}
				}
				return result;
			}

			#endregion
		};

		List<KeyValuePair<K, long>> _keyIdMap;
		Db4oRecordList<T> _masterRecordList;
		string _cachePath;
		Comparer _sorter;
		IDb4oSortHelper<K, T> _sortHelper;

		public CachedSortedDb4oList(Db4oRecordListManager recordListManager, IDb4oSortHelper<K, T> sortHelper)
		{
			if (recordListManager == null)
			{
				Dispose();
				throw new ArgumentNullException("recordListManager");
			}
			if (sortHelper == null)
			{
				Dispose();
				throw new ArgumentNullException("sortHelper");
			}
			_sortHelper = sortHelper;
			_cachePath = recordListManager.CachePath;
			_sorter = new Comparer(sortHelper.KeyComparer);
			_masterRecordList = (Db4oRecordList<T>)recordListManager.GetListOfType<T>();

			Deserialize();
			if(_keyIdMap == null)
			{
				_keyIdMap = _sortHelper.GetKeyIdPairs();
				Sort();
			}

			_masterRecordList.ListChanged += new ListChangedEventHandler(OnMasterRecordListListChanged);
			_masterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(OnMasterRecordListDeletingRecord);
		}

		private string CacheFilePath
		{
			get
			{
				return Path.Combine(_cachePath, CacheHelper.EscapeFileNameString(_sortHelper.Name) + ".cache");
			}
		}

		private void Serialize()
		{
			try
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
						formatter.Serialize(fs, GetSorterHashCode());
						formatter.Serialize(fs, _keyIdMap);
					}
					finally
					{
						fs.Close();
					}
				}
			}
			catch
			{
			}
		}

		private int GetSorterHashCode() {
			byte[] bytes = _sorter.GetType().GetMethod("Compare").GetMethodBody().GetILAsByteArray();
			int hashCode = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = bytes[i];
				hashCode ^= b;
			}
			return hashCode;
		}

		private void Deserialize()
		{
			_keyIdMap = null;
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
							if (filterHashCode == GetSorterHashCode())
							{
								_keyIdMap = (List<KeyValuePair<K, long>>)formatter.Deserialize(fs);
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
		}

		private DateTime GetDatabaseLastModified()
		{
			return _masterRecordList.GetDatabaseLastModified();
		}

		void OnMasterRecordListDeletingRecord(object sender, RecordListEventArgs<T> e)
		{
			Remove(e.Item);
		}

		void OnMasterRecordListListChanged(object sender, ListChangedEventArgs e)
		{
			IRecordList<T> masterRecordList = (IRecordList<T>)sender;
			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					Add(masterRecordList[e.NewIndex]);
					break;
				case ListChangedType.ItemChanged:
					Update(masterRecordList[e.NewIndex]);
					break;
				case ListChangedType.ItemDeleted:
					break;
				case ListChangedType.Reset:
					Sort();
					break;
			}
			//Serialize();
		}

		private void Add(T item)
		{
			foreach (K key in _sortHelper.GetKeys(item))
			{
				KeyValuePair<K, long> keyIdPair = new KeyValuePair<K, long>(key,
																		   this._masterRecordList.GetId(item));

				int index = _keyIdMap.BinarySearch(keyIdPair, _sorter);
				if (index < 0) // not found, index is bitwise complement of the place to insert
				{
					_keyIdMap.Insert(~index, keyIdPair);
					OnItemAdded(~index);
				}
			}
		}

		private void Update(T item)
		{
			long itemId = _masterRecordList.GetId(item);

			List<int> indexesOfDeletedItems = new List<int>();
			List<int> indexesOfAddedItems = new List<int>();

			List<KeyValuePair<K, long>> oldItems = this._keyIdMap.FindAll(delegate(KeyValuePair<K, long> i)
																		  {
																			  return i.Value == itemId;
																		  } );
			foreach(KeyValuePair<K, long> toDelete in oldItems)
			{
				indexesOfDeletedItems.Add(_keyIdMap.IndexOf(toDelete));
			}

			indexesOfDeletedItems.Sort();

			for (int i = indexesOfDeletedItems.Count - 1; i >= 0 ; i--)
			{
				_keyIdMap.RemoveAt(indexesOfDeletedItems[i]);
			}

			foreach (K key in _sortHelper.GetKeys(item))
			{
				KeyValuePair<K, long> keyIdPair = new KeyValuePair<K, long>(key, itemId);
				//see if we actually need to do anything. If the key has not changed, then it
				//should be found except in the weird case where there are two keys of
				// the same value in the same record.
				int index = _keyIdMap.BinarySearch(keyIdPair, _sorter);
				if (index < 0)
				{
					index = ~index;
				}

				_keyIdMap.Insert(index, keyIdPair);

				if(indexesOfDeletedItems.Contains(index))
				{
					indexesOfDeletedItems.Remove(index);
					OnItemChanged(index);
				}
				else
				{
					indexesOfAddedItems.Add(index);
				}
			}

			int itemsMoved = Math.Min(indexesOfAddedItems.Count, indexesOfDeletedItems.Count);
			for (int i = 0; i < itemsMoved; i++)
			{
				OnItemMoved(indexesOfAddedItems[0],
							indexesOfDeletedItems[0]);
				indexesOfAddedItems.RemoveAt(0);
				indexesOfDeletedItems.RemoveAt(0);
			}
			foreach(int index in indexesOfAddedItems)
			{
				OnItemAdded(index);
			}
			foreach(int index in indexesOfDeletedItems)
			{
				OnItemDeleted(index);
			}
		}

		private void Remove(T item)
		{
			if (_masterRecordList.Contains(item))
			{
				long itemId = _masterRecordList.GetId(item);
				List<KeyValuePair<K, long>> matches =
						_keyIdMap.FindAll(delegate(KeyValuePair<K, long> i) { return i.Value == itemId; });
				foreach (KeyValuePair<K, long> match in matches)
				{
					int index = _keyIdMap.IndexOf(match);
					Debug.Assert(index != -1);
					_keyIdMap.RemoveAt(index);
					OnItemDeleted(index);
				}
			}
		}

		private void Sort()
		{
			// The reset event can be raised when items are cleared and also when sorting or filtering occurs
			if (_masterRecordList.Count == 0)
			{
				_keyIdMap.Clear();
			}
			else
			{
				_keyIdMap.Sort(_sorter);
			}
			OnListReset();
			if (!this._masterRecordList.DelayWritingCachesUntilDispose)
			{
				Serialize();
			}
		}

		#region IBindingList Members

		public void AddIndex(PropertyDescriptor property)
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		public object AddNew()
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		public bool AllowEdit
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		public bool AllowNew
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		public bool AllowRemove
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		public int Find(PropertyDescriptor property, object key)
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		/// <summary>
		/// Returns the first row where the key is equal
		/// </summary>
		/// <param name="key"></param>
		/// <returns>The zero based index of item if found; otherwise a negative number
		/// that is the bitwise complement of the index of the next element that is larger than item
		/// or if there is no larger element, the bitwise complement of Count.</returns>
		public int BinarySearch(K key)
		{
			VerifyNotDisposed();
			KeyValuePair<K, long> keyIdPair = new KeyValuePair<K, long>(key, 0);

			int index = _keyIdMap.BinarySearch(keyIdPair, _sorter);
			if (index < 0 && ~index !=_keyIdMap.Count)
			{
				if (_sortHelper.KeyComparer.Compare(_keyIdMap[~index].Key, key) == 0)
				{
					index = ~index;
				}
			}

			return index;
		}

		public bool IsSorted
		{
			get
			{
				VerifyNotDisposed();
				return true;
			}
		}

		protected virtual void OnItemAdded(int newIndex)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, newIndex));
		}
		protected virtual void OnItemMoved(int newIndex, int oldIndex)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, newIndex, oldIndex));
		}
		protected virtual void OnItemChanged(int newIndex)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, newIndex));
		}

		protected virtual void OnItemDeleted(int oldIndex)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, oldIndex));
		}

		protected virtual void OnListReset()
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			ListChanged(this, e);
		}

		public event ListChangedEventHandler ListChanged = delegate
														   {
														   };

		public void RemoveIndex(PropertyDescriptor property)
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		public void RemoveSort()
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		public ListSortDirection SortDirection
		{
			get
			{
				VerifyNotDisposed();
				throw new NotSupportedException();
			}
		}

		public PropertyDescriptor SortProperty
		{
			get
			{
				VerifyNotDisposed();
				throw new NotSupportedException();
			}
		}

		public bool SupportsChangeNotification
		{
			get
			{
				VerifyNotDisposed();
				return true;
			}
		}

		public bool SupportsSearching
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		public bool SupportsSorting
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		#endregion

		#region IList Members

		public int Add(object value)
		{
			VerifyNotDisposed();
			return ((IList)_masterRecordList).Add(value);
		}

		public void Clear()
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		public bool Contains(object value)
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		public int IndexOf(object value)
		{
			VerifyNotDisposed();

			if (!(value is T))
			{
				throw new ArgumentException("value must be of type T");
			}
			long itemId = this._masterRecordList.GetId((T)value);

			KeyValuePair<K, long> match = _keyIdMap.Find(delegate(KeyValuePair<K, long> i)
														 {
															 return i.Value == itemId;
														 });

			return ((IList)_keyIdMap).IndexOf(match);
		}

		public void Insert(int index, object value)
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		public bool IsFixedSize
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				VerifyNotDisposed();
				return true;
			}
		}

		public void Remove(object value)
		{
			VerifyNotDisposed();
			((IList)_masterRecordList).Remove(value);
		}

		public void RemoveAt(int index)
		{
			VerifyNotDisposed();
			((IList)_masterRecordList).Remove(GetValue(index));
		}


		// returns the key string. to get the value object use GetValue
		// this is so bindinglistgrid will display the right thing.
		public object this[int index]
		{
			get
			{
				VerifyNotDisposed();
				return GetKey(index);
			}
			set
			{
				VerifyNotDisposed();
				throw new NotSupportedException();
			}
		}

		public K GetKey(int index)
		{
			VerifyNotDisposed();
			return _keyIdMap[index].Key;
		}

		public T GetValue(int index)
		{
			VerifyNotDisposed();
			return _masterRecordList[_masterRecordList.GetIndexFromId(_keyIdMap[index].Value)];
		}

		public long GetId(int index)
		{
			VerifyNotDisposed();
			return _keyIdMap[index].Value;
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			VerifyNotDisposed();
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				VerifyNotDisposed();
				return _keyIdMap.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				VerifyNotDisposed();
				throw new NotSupportedException();
			}
		}

		#endregion

		#region IEnumerable Members        #region IEnumerable Members

		#region Enumerator

		public struct Enumerator : IEnumerator<K>
		{
			CachedSortedDb4oList<K, T> _collection;
			int _index;
			bool _isDisposed;

			public Enumerator(CachedSortedDb4oList<K, T> collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException();
				}
				_collection = collection;
				_index = -1;
				_isDisposed = false;
			}

			private void CheckValidIndex(int validMinimum)
			{
				if (_index < validMinimum || _index >= _collection.Count)
				{
					throw new InvalidOperationException();
				}
			}

			private void CheckCollectionUnchanged()
			{
				if (!_collection._isEnumerating)
				{
					throw new InvalidOperationException();
				}
			}

			private void VerifyNotDisposed()
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException("CachedSortedDb4oList<K, T>.Enumerator");
				}
			}

			#region IEnumerator<T> Members

			public K Current
			{
				get
				{
					VerifyNotDisposed();
					CheckValidIndex(0);
					CheckCollectionUnchanged();
					return _collection.GetKey(_index);
				}
			}

			#endregion

			#region IDisposable Members

			void IDisposable.Dispose()
			{
				_collection = null;
			}

			#endregion

			#region IEnumerator Members

			object System.Collections.IEnumerator.Current
			{
				get
				{
					VerifyNotDisposed();
					return Current;
				}
			}

			public bool MoveNext()
			{
				VerifyNotDisposed();
				CheckValidIndex(-1);
				CheckCollectionUnchanged();
				return (++_index < _collection.Count);
			}

			public void Reset()
			{
				VerifyNotDisposed();
				CheckCollectionUnchanged();
				_index = -1;
			}

			#endregion
		}

		private bool _isEnumerating;

		#endregion

		public Enumerator GetEnumerator()
		{
			VerifyNotDisposed();
			_isEnumerating = true;
			return new Enumerator(this);
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			VerifyNotDisposed();
			return GetEnumerator();
		}

		#endregion

		IEnumerator<K> IEnumerable<K>.GetEnumerator()
		{
			VerifyNotDisposed();
			int count = _keyIdMap.Count;
			for (int i = 0; i < count; i++)
			{
				yield return _keyIdMap[i].Key;
			}
		}
		#region IDisposable Members
#if DEBUG
		~CachedSortedDb4oList()
		{
			if (!this._disposed)
			{
				throw new ApplicationException("Disposed not explicitly called on CachedSortedDb4oList.");
			}
		}
#endif

		private bool _disposed = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic
					if (_masterRecordList != null)
					{
						_masterRecordList.ListChanged -= OnMasterRecordListListChanged;
						_masterRecordList.DeletingRecord -= OnMasterRecordListDeletingRecord;
					}
				   Serialize();
				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("CachedSortedDb4oList");
			}
		}
		#endregion
	}
}
