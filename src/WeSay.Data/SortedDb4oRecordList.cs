using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace WeSay.Data
{
	public class CachedSortedDb4oList<K, T>: IBindingList, IEnumerable<K> where T : class, new()
	{
		public delegate List<KeyValuePair<K, long>> Initializer ();
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
		IComparer<K> _keySorter;
		KeyProvider _keyProvider;
		bool _temporary;

		public CachedSortedDb4oList(Db4oRecordList<T> masterRecordList, Initializer initializer, KeyProvider keyProvider, IComparer<K> sorter, string dataPath)
		{
			_cachePath = Path.Combine(dataPath, "Cache");
			_sorter = new Comparer(sorter);
			_keySorter = sorter;
			_keyProvider = keyProvider;
			_masterRecordList = masterRecordList;

			Deserialize();
			if(_keyIdMap == null)
			{
				_keyIdMap = initializer();
				Sort();
				Serialize();
			}

			_masterRecordList.ListChanged += new ListChangedEventHandler(OnMasterRecordListListChanged);
			_masterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(OnMasterRecordListDeletingRecord);
		}

		public CachedSortedDb4oList(CachedSortedDb4oList<K, T> template, IEnumerable<KeyValuePair<K, long>> initialData)
		{
			if(template == null)
			{
				throw new ArgumentNullException("template");
			}
			if (initialData == null)
			{
				throw new ArgumentNullException("initialData");
			}
			_cachePath = template._cachePath;
			_sorter = template._sorter;
			_keyProvider = template._keyProvider;
			_masterRecordList = template._masterRecordList;
			_temporary = true;

			_keyIdMap = new List<KeyValuePair<K, long>>(initialData);

			_masterRecordList.ListChanged += new ListChangedEventHandler(OnMasterRecordListListChanged);
			_masterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(OnMasterRecordListDeletingRecord);
		}

		private string CacheFilePath
		{
			get
			{
				return Path.Combine(_cachePath, _sorter.GetType().Name + ".cache");
			}
		}

		private void Serialize()
		{
			if (_temporary)
			{
				return;
			}
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
			if (_temporary)
			{
				return;
			}
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
			Serialize();
		}

		private void Add(T item)
		{
			KeyValuePair<K, long> keyIdPair = new KeyValuePair<K,long>(_keyProvider(item),
																	   this._masterRecordList.GetId(item));

			int index = _keyIdMap.BinarySearch(keyIdPair, _sorter);
			if(index < 0) // not found, index is bitwise complement of the place to insert
			{
				_keyIdMap.Insert(~index, keyIdPair);
				OnItemAdded(~index);
			}
		}

		private void Update(T item)
		{
			long itemId = _masterRecordList.GetId(item);

			KeyValuePair<K, long> keyIdPair = new KeyValuePair<K, long>(_keyProvider(item),
														   itemId);
			//see if we actually need to do anything. If the key has not changed, then it
			//should be found
			int index = _keyIdMap.BinarySearch(keyIdPair, _sorter);
			if (index < 0) // not found, index is bitwise complement of the place to insert
			{
				//remove the old one first
				KeyValuePair<K, long> match = _keyIdMap.Find(delegate(KeyValuePair<K, long> i)
															 {
																 return i.Value == itemId;
															 });
				int oldIndex = _keyIdMap.IndexOf(match);
				if(oldIndex != -1)
				{
					_keyIdMap.RemoveAt(oldIndex);
				}

				// just in case old one came before our insert location, our insert location may have gotten messed up
				index = ~_keyIdMap.BinarySearch(keyIdPair, _sorter);
				_keyIdMap.Insert(index, keyIdPair);
				if (oldIndex != -1 && index != oldIndex)
				{
					OnItemMoved(index, oldIndex);
				}
				OnItemChanged(index);
			}
		}

		private void Remove(T item)
		{
			long itemId = _masterRecordList.GetId(item);
			KeyValuePair<K, long> match = _keyIdMap.Find(delegate(KeyValuePair<K, long> i)
														 {
															 return i.Value == itemId;
														 });
			int index = _keyIdMap.IndexOf(match);
			if(index != -1)
			{
				_keyIdMap.RemoveAt(index);
				OnItemDeleted(index);
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
			Serialize();

		}

		#region IBindingList Members

		public void AddIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}

		public object AddNew()
		{
			throw new NotSupportedException();
		}

		public bool AllowEdit
		{
			get
			{
				return false;
			}
		}

		public bool AllowNew
		{
			get
			{
				return false;
			}
		}

		public bool AllowRemove
		{
			get
			{
				return false;
			}
		}

		public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		public int Find(PropertyDescriptor property, object key)
		{
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
			KeyValuePair<K, long> keyIdPair = new KeyValuePair<K, long>(key, 0);

			int index = _keyIdMap.BinarySearch(keyIdPair, _sorter);
			if (index < 0)
			{
				if (_keySorter.Compare(_keyIdMap[~index].Key, key) == 0)
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
			throw new NotSupportedException();
		}

		public void RemoveSort()
		{
			throw new NotSupportedException();
		}

		public ListSortDirection SortDirection
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public PropertyDescriptor SortProperty
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		public bool SupportsSearching
		{
			get
			{
				return false;
			}
		}

		public bool SupportsSorting
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region IList Members

		public int Add(object value)
		{
			return ((IList)_masterRecordList).Add(value);
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(object value)
		{
			throw new NotSupportedException();
		}

		public int IndexOf(object value)
		{
			if (value is KeyValuePair<K, long>)
			{
				return ((IList)_keyIdMap).IndexOf(value);
			}
			else
			{
				long itemId = this._masterRecordList.GetId((T)value);

				KeyValuePair<K, long> match = _keyIdMap.Find(delegate(KeyValuePair<K, long> i)
															 {
																 return i.Value == itemId;
															 });

				return ((IList)_keyIdMap).IndexOf(match);

			}
		}

		public void Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public void Remove(object value)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			((IList)_masterRecordList).Remove(this[index]);
		}

		public object this[int index]
		{
			get
			{
				return _masterRecordList[_masterRecordList.GetIndexFromId(_keyIdMap[index].Value)];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public K GetKey(int index)
		{
			return _keyIdMap[index].Key;
		}

		public long GetId(int index)
		{
			return _keyIdMap[index].Value;
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				return _keyIdMap.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return _keyIdMap.GetEnumerator();
		}

		#endregion

		IEnumerator<K> IEnumerable<K>.GetEnumerator()
		{
			int count = _keyIdMap.Count;
			for (int i = 0; i < count; i++)
			{
				yield return _keyIdMap[i].Key;
			}
		}

	}
}
