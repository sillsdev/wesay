using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	public abstract class AbstractRecordList<T> : IRecordList<T> where T : class, new()
	{
		private IList<T> _records;

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

		public abstract int GetIndexFromId(long id);

		private PropertyDescriptor _sortProperty;
		private ListSortDirection _listSortDirection;

		protected ListSortDirection ListSortDirection
		{
			get {
				return _listSortDirection;
			}
			set {
				_listSortDirection = value;
			}
		}

		public event EventHandler<RecordListEventArgs<T>> AddingRecord = delegate
		{
		};
		public event EventHandler<RecordListEventArgs<T>> DeletingRecord = delegate
		{
		};


		/// <summary>
		/// Used when importing, where we want to go fast and don't care to have a good cache if we crash
		/// </summary>
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
	  //  abstract public bool Commit();

		/// <summary>
		/// Determine if record should be allowed to be added
		/// </summary>
		/// <param name="item">The item that is about to be added</param>
		/// <returns>true if item should be added</returns>
		protected virtual bool ShouldAddRecord(T item)
		{
			RecordListEventArgs<T> args = new RecordListEventArgs<T>(item);
			AddingRecord(this, args);
			return !args.Cancel;
		}
		/// <summary>
		/// Determine if record should be allowed to be deleted
		/// </summary>
		/// <param name="item">The item that is about to be deleted</param>
		/// <returns>true if item should be deleted</returns>
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

		#region IBindingList Members

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

		abstract protected void DoSort(Comparison<T> sort);

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

		abstract public bool IsSorted
		{
			get;
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

		abstract protected void DoRemoveSort();

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

		#endregion

		#region IFilterable<T> Members

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

		abstract protected void DoFilter(Predicate<T> itemsToInclude);

		public void RemoveFilter()
		{
			VerifyNotDisposed();
			DoRemoveFilter();
			OnListReset();
		}

		abstract protected void DoRemoveFilter();

		abstract public bool IsFiltered
		{
			get;
		}
		#endregion
		#region IList<T> Members

		public int IndexOf(T item)
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

		#endregion

		#region IList Members

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

		#endregion

		#region ICollection<T> Members

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

		#endregion

		#region ICollection Members

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

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			VerifyNotDisposed();
			return GetEnumerator();
		}

		protected IEnumerator GetEnumerator()
		{
			return ((IEnumerable) _records).GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			VerifyNotDisposed();
			return _records.GetEnumerator();
		}

		#endregion


		#region IEquatable<IRecordList<T>> Members

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
		#endregion

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

		#region IDisposable Members
		#if DEBUG
		~AbstractRecordList()
		{
			if (!this._disposed)
			{
				throw new InvalidOperationException("Disposed not explicitly called on " + GetType().FullName + ".");
			}
		}
		#endif

		private bool _disposed = false;
		private bool _delayWritingCachesUntilDispose = false;

		public bool IsDisposed
		{
			get {
				return _disposed;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic
					_records = null;
				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		#endregion



	}
}
