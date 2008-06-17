using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	internal class Db4oRecordList<T> : IRecordList<T> where T : class, new()
	{
  //committing is handled by LexEntryRepository when told to.
		private static int defaultWriteCacheSize = 0; // 0 means never commit until database closes, 1 means commit after each write
		private IList<T> _records;
		private PropertyDescriptor _sortProperty;
		private ListSortDirection _listSortDirection;
		private bool _disposed = false;
		private bool _delayWritingCachesUntilDispose = false;

		private void Initialize(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort, SodaQueryProvider sodaQuery)
		{
			if (dataSource == null)
			{
				Dispose();
				throw new ArgumentNullException("dataSource");
			}
			Db4oList<T> records = new Db4oList<T>(dataSource.Data, new List<T>(), filter, sort);
			Records = records;
			InitializeDb4oListBehavior(records);
			try
			{
				if (sodaQuery != null)
				{
					records.SODAQuery = sodaQuery;
				}
				else
				{
					records.Requery(records.FilteringInDatabase && filter != null);
				}
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		private void InitializeDb4oListBehavior(Db4oList<T> records) {
			records.SortingInDatabase = false;
			records.FilteringInDatabase = false;
			records.ReadCacheSize = 0; // I think this could go back to lower
			records.WriteCacheSize = defaultWriteCacheSize;
			records.PeekPersistedActivationDepth = 99;
			records.ActivationDepth = 99;
			records.RefreshActivationDepth = 99;
			records.SetActivationDepth = 99;
			//            records.RequeryAndRefresh(false);
			records.Storing += OnRecordStoring;
		}

		public Db4oRecordList(Db4oDataSource dataSource)
		{
			Initialize(dataSource, null, null, null);
		}

		public Db4oRecordList(Db4oRecordList<T> source )
		{
			ListSortDirection = source.ListSortDirection;
			SortProperty = source.SortProperty;

			Db4oList<T> sourceRecords = (Db4oList<T>) source.Records;
			Db4oList<T> records = new Db4oList<T>(sourceRecords.Database, sourceRecords.ItemIds, sourceRecords.Filter, sourceRecords.Sorter);
			InitializeDb4oListBehavior(records);
			Records = records;
		}

		public Db4oRecordList(Db4oDataSource dataSource, SodaQueryProvider sodaQuery)
		{
			Initialize(dataSource, null, null, sodaQuery);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Predicate<T> filter)
		{
			if (filter == null)
			{
				Dispose();
				throw new ArgumentNullException("filter");
			}
			Initialize(dataSource, filter, null, null);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort)
		{
			if (filter == null)
			{
				Dispose();
				throw new ArgumentNullException("filter");
			}
			if (sort == null)
			{
				Dispose();
				throw new ArgumentNullException("sort");
			}
			Initialize(dataSource, filter, sort, null);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Comparison<T> sort)
		{
			if (sort == null)
			{
				Dispose();
				throw new ArgumentNullException("sort");
			}
			Initialize(dataSource, null, sort, null);
		}

		void OnRecordStoring(object sender, Db4oListEventArgs<T> e)
		{
			int index = IndexOf(e.Item);
			if (index != -1) // if it is -1 then this record will be added
			{
				OnItemContentChanged(index);
			}
		}

//        public override bool Commit()
//        {
//            VerifyNotDisposed();
//            if (((Db4oList<T>)Records).Commit())
//            {
//                if (DataCommitted != null)
//                {
//                    DataCommitted.Invoke(this,null);
//                }
//                return true;
//            }
//            return false;
//        }

		public SodaQueryProvider SodaQuery
		{
			get {
				VerifyNotDisposed();
				return ((Db4oList<T>)Records).SODAQuery;
			}
			set {
				VerifyNotDisposed();
				((Db4oList<T>)Records).SODAQuery = value;
			}
		}

		public void Add(IEnumerator<T> enumerator)
		{
			VerifyNotDisposed();
			Db4oList<T> records = (Db4oList<T>)Records;
			records.WriteCacheSize = 0;
			while (enumerator.MoveNext())
			{
				Add(enumerator.Current);
			}
			records.Commit();
			records.WriteCacheSize = defaultWriteCacheSize;
		}


		public int WriteCacheSize
		{
			get {
				VerifyNotDisposed();
				return ((Db4oList<T>)Records).WriteCacheSize;
			}
			set {
				VerifyNotDisposed();
				((Db4oList<T>)Records).WriteCacheSize = value;
			}
		}

		private void DoFilter(Predicate<T> filter)
		{
			Db4oList<T> records = (Db4oList<T>) Records;
			if (records.FilteringInDatabase)
			{
				records.Commit();
			}
			else
			{
				if (records.IsFiltered)
				{
					RemoveFilter();
				}
			}

			//here's the big long step
			records.Filter = filter;
		}
		private void DoRemoveFilter()
		{
			Db4oList<T> records = (Db4oList<T>)Records;

			records.Filter = null;
			if (!records.FilteringInDatabase)
			{
				records.Requery(false);
			}
		}

		public bool IsFiltered
		{
			get {
				VerifyNotDisposed();
				Db4oList<T> records = (Db4oList<T>)Records;

				return records.IsFiltered || records.SODAQuery != null;
			}
		}

		private void DoSort(Comparison<T> sort)
		{
			((Db4oList<T>)Records).Sort(sort);
		}

		public bool IsSorted
		{
			get {
				VerifyNotDisposed();
				return ((Db4oList<T>)Records).IsSorted;
			}
		}

		protected IList<T> Records
		{
			get {
				return _records;
			}
			set {
				if(_records != null)
				{
					IDisposable disposable = _records as IDisposable;
					if(disposable != null)
					{
						disposable.Dispose();
					}

				}
				_records = value;
			}
		}

		protected ListSortDirection ListSortDirection
		{
			get {
				return _listSortDirection;
			}
			set {
				_listSortDirection = value;
			}
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
			}
		}

		bool IBindingList.AllowEdit
		{
			get {
				VerifyNotDisposed();
				return AllowEdit;
			}
		}

		static protected bool AllowEdit
		{
			get {
				return true;
			}
		}

		bool IBindingList.AllowNew
		{
			get {
				VerifyNotDisposed();
				return AllowNew;
			}
		}

		static protected bool AllowNew
		{
			get {
				return true;
			}
		}

		bool IBindingList.AllowRemove
		{
			get {
				VerifyNotDisposed();
				return AllowRemove;
			}
		}

		static protected bool AllowRemove
		{
			get {
				return true;
			}
		}

		public ListSortDirection SortDirection
		{
			get {
				VerifyNotDisposed();
				return _listSortDirection;
			}
		}

		public PropertyDescriptor SortProperty
		{
			get {
				VerifyNotDisposed();
				return _sortProperty;
			}
			protected set {
				_sortProperty = value;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get {
				VerifyNotDisposed();
				return SupportsChangeNotification;
			}
		}

		static protected bool SupportsChangeNotification
		{
			get {
				return true;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get {
				VerifyNotDisposed();
				return SupportsSearching;
			}
		}

		static protected bool SupportsSearching
		{
			get {
				return true;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get {
				VerifyNotDisposed();
				return SupportsSorting;
			}
		}

		static protected bool SupportsSorting
		{
			get {
				return true;
			}
		}

		public bool IsFixedSize
		{
			get {
				VerifyNotDisposed();
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get {
				VerifyNotDisposed();
				return IsReadOnly;
			}
		}

		public int Count
		{
			get {
				VerifyNotDisposed();
				return _records.Count;
			}
		}

		public bool IsReadOnly
		{
			get {
				VerifyNotDisposed();
				return _records.IsReadOnly;
			}
		}

		int ICollection.Count
		{
			get {
				VerifyNotDisposed();
				return Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get {
				VerifyNotDisposed();
				return IsSynchronized;
			}
		}

		static protected bool IsSynchronized
		{
			get {
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get {
				VerifyNotDisposed();
				return SyncRoot;
			}
		}

		protected object SyncRoot
		{
			get {
				return this;
			}
		}

		public bool IsDisposed
		{
			get {
				return _disposed;
			}
		}

		public int GetIndexFromId(long id)
		{
			int index = ((Db4oList<T>) Records).ItemIds.IndexOf(id);
			if(index == -1)
			{
				throw new ArgumentOutOfRangeException("id");
			}
			return index;
		}

		public  long GetId(T item)
		{
			int index = IndexOf(item);
			if(index == -1)
			{
				return -1;
			}
			return ((Db4oList<T>)Records).ItemIds[index];
		}

		public DateTime GetDatabaseLastModified()
		{
			IList<DatabaseModified> modifiedList = ((Db4oList<T>)Records).Database.Query<DatabaseModified>();
			if (modifiedList.Count == 1)
			{
				return modifiedList[0].LastModified;
			}
			return DateTime.UtcNow;
		}

		private void DoRemoveSort()
		{
			((Db4oList<T>)Records).RemoveSort();
		}

		protected  virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					if (Records != null)
					{
						((Db4oList<T>)Records).Dispose();
					}
					// dispose-only, i.e. non-finalizable logic
					_records = null;
				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}


		public event EventHandler<RecordListEventArgs<T>> AddingRecord = delegate
												 {
												 };

		public event EventHandler<RecordListEventArgs<T>> DeletingRecord = delegate
												   {
												   };

		protected virtual bool ShouldAddRecord(T item)
		{
			RecordListEventArgs<T> args = new RecordListEventArgs<T>(item);
			AddingRecord(this, args);
			return !args.Cancel;
		}

		protected virtual bool ShouldDeleteRecord(T item)
		{
			RecordListEventArgs<T> args = new RecordListEventArgs<T>(item);
			DeletingRecord(this, args);
			return !args.Cancel;
		}

		protected virtual bool ShouldReplaceRecord(int index, T value)
		{
			return true;
		}

		protected virtual bool ShouldClearRecords()
		{
			return true;
		}

		public void AddRange(IEnumerable<T> collection)
		{
			VerifyNotDisposed();
			if(collection == null)
			{
				throw new ArgumentNullException();
			}
			IEnumerator<T> enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Add(enumerator.Current);
			}
		}

		public void AddRange(IEnumerable collection)
		{
			VerifyNotDisposed();
			if (collection == null)
			{
				throw new ArgumentNullException();
			}
			IEnumerator enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Add((T)enumerator.Current);
			}
		}

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			VerifyNotDisposed();
			AddIndex(property);
		}

		static protected void AddIndex(PropertyDescriptor property)
		{
		}

		public T AddNew()
		{
			VerifyNotDisposed();
			T o = new T();
			Add(o);
			return o;
		}

		object IBindingList.AddNew()
		{
			VerifyNotDisposed();
			return AddNew();
		}

		public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			VerifyNotDisposed();
			if (_records.Count > 1)
			{
				Comparison<T> sort = delegate(T item1, T item2)
									 {
										 PropertyComparison<T> propertySorter = ComparisonHelper<T>.GetPropertyComparison(ComparisonHelper<T>.DefaultPropertyComparison, direction);
										 return propertySorter(item1, item2, property);
									 };

				DoSort(sort);
				_sortProperty = property;
				_listSortDirection = direction;
				OnListReset();
			}
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			VerifyNotDisposed();
			return Find(property, key);
		}

		protected int Find(PropertyDescriptor property, object key)
		{

			for (int i = 0; i < Count; ++i)
			{
				if (ComparisonHelper<object>.DefaultEqualityComparison(property.GetValue(_records[i]), key))
				{
					return i;
				}
			}
			return -1;
		}

		protected virtual void OnItemAdded(int newIndex)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, newIndex));
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

		protected virtual void OnItemContentChanged(int index)
		{
			ContentOfItemInListChanged(this, new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}

		public event ListChangedEventHandler ContentOfItemInListChanged = delegate
																		  {
																		  };

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			VerifyNotDisposed();
			RemoveIndex(property);
		}

		static protected void RemoveIndex(PropertyDescriptor property)
		{
		}

		public void RemoveSort()
		{
			VerifyNotDisposed();
			if (IsSorted)
			{
				DoRemoveSort();
				_sortProperty = null;
				OnListReset();
			}
		}

		public void ApplyFilter(Predicate<T> filter)
		{
			VerifyNotDisposed();
			if (filter == null)
			{
				throw new ArgumentNullException();
			}
			DoFilter(filter);
			OnListReset();
		}

		public void RemoveFilter()
		{
			VerifyNotDisposed();
			DoRemoveFilter();
			OnListReset();
		}

		public virtual int IndexOf(T item)
		{
			VerifyNotDisposed();
			return _records.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			VerifyNotDisposed();
			if (ShouldAddRecord(item))
			{
				_records.Insert(index, item);
				OnItemChanged(index);
			}
		}

		public void RemoveAt(int index)
		{
			VerifyNotDisposed();
			Remove(this[index]);
		}

		public virtual T this[int index]
		{
			get {
				VerifyNotDisposed();
				return _records[index];
			}
			set {
				VerifyNotDisposed();
				if (ShouldReplaceRecord(index, value))
				{
					_records[index] = value;
					OnItemChanged(index);
				}
			}
		}

		int IList.Add(object value)
		{
			VerifyNotDisposed();
			T item = (T)value;
			Add(item);
			return IndexOf(item);
		}

		void IList.Clear()
		{
			VerifyNotDisposed();
			Clear();
		}

		bool IList.Contains(object value)
		{
			VerifyNotDisposed();
			return Contains((T)value);
		}

		int IList.IndexOf(object value)
		{
			VerifyNotDisposed();
			return IndexOf((T)value);
		}

		void IList.Insert(int index, object value)
		{
			VerifyNotDisposed();
			Insert(index, (T)value);
		}

		void IList.Remove(object value)
		{
			VerifyNotDisposed();
			Remove((T)value);
		}

		protected void CheckIndex(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		void IList.RemoveAt(int index)
		{
			VerifyNotDisposed();
			CheckIndex(index);
			RemoveAt(index);
		}

		object IList.this[int index]
		{
			get {
				VerifyNotDisposed();
				CheckIndex(index);
				return _records[index];
			}
			set {
				VerifyNotDisposed();
				CheckIndex(index);
				_records[index] = (T)value;
				OnItemChanged(index);
			}
		}

		public void Add(T item)
		{
			VerifyNotDisposed();
			if (ShouldAddRecord(item))
			{
				_records.Add(item);
				OnItemAdded(IndexOf(item));
			}
		}

		public void Clear()
		{
			VerifyNotDisposed();
			if (ShouldClearRecords())
			{
				_records.Clear();
				OnListReset();
			}
		}

		public bool Contains(T item)
		{
			VerifyNotDisposed();
			return _records.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			VerifyNotDisposed();
			_records.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			VerifyNotDisposed();
			bool itemRemoved = false;
			if (ShouldDeleteRecord(item))
			{
				int index = _records.IndexOf(item);
				if (index != -1)
				{
					itemRemoved = true;
					_records.RemoveAt(index);
					OnItemDeleted(index);
				}
			}

			return itemRemoved;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			VerifyNotDisposed();
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "must be >= 0");
			}
			if (index + Count > array.Length)
			{
				throw new ArgumentException("array not large enough to fit collection starting at index");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("array cannot be multidimensional", "array");
			}

			T[] tArray = new T[Count];
			CopyTo(tArray, 0);
			foreach (T t in tArray)
			{
				array.SetValue(t, index++);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			VerifyNotDisposed();
			return GetEnumerator();
		}

		protected IEnumerator GetEnumerator()
		{
			return ((IEnumerable) _records).GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			VerifyNotDisposed();
			return _records.GetEnumerator();
		}

		public bool Equals(IRecordList<T> other)
		{
			VerifyNotDisposed();
			if (other == null)
			{
				return false;
			}
			if (Count != other.Count)
			{
				return false;
			}
			for (int i = 0; i < Count; i++)
			{
				// must be in same order to be equal
				if (this[i] != other[i])
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			VerifyNotDisposed();
			if (obj == null)
			{
				return false;
			}
			IRecordList<T> recordList = obj as IRecordList<T>;
			if (recordList == null)
			{
				return false;
			}

			return Equals(recordList);
		}

		public override int GetHashCode()
		{
			VerifyNotDisposed();
			int hashCode = _records.GetHashCode();

			if (IsSorted)
			{
				hashCode ^= _sortProperty.GetHashCode() ^ _listSortDirection.GetHashCode();
			}

			return hashCode;
		}

		~Db4oRecordList()
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


		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}
	}
}
