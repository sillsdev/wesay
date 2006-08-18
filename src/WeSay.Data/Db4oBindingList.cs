using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	public class Db4oBindingList<T> : IBindingList, IFilterable<T>, IList<T>, ICollection<T>, IEnumerable<T>, IDisposable where T : class, INotifyPropertyChanged, new()
	{
		Db4o.Binding.Db4oList<T> _records;
		PropertyDescriptor	_propertyDescriptor;
		ListSortDirection   _listSortDirection;
		private static int defaultWriteCacheSize = 1;

		private void Initialize(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort )
		{
			if(dataSource == null){
				throw new ArgumentNullException("dataSource");
			}

			_records = new Db4o.Binding.Db4oList<T>((com.db4o.ObjectContainer)dataSource.Data, new List<T>(), filter, sort);
			_records.ReadCacheSize = 0;
			_records.WriteCacheSize = defaultWriteCacheSize;
			_records.PeekPersistedActivationDepth = 99;
			_records.ActivationDepth = 99;
			_records.RefreshActivationDepth = 99;
			_records.SetActivationDepth = 99;
			_records.Requery(false);
		   }

		public Db4oBindingList(Db4oDataSource dataSource)
		{
			Initialize(dataSource, null, null);
		}

		public Db4oBindingList(Db4oDataSource dataSource, Predicate<T> filter)
		{
			if(filter == null){
				throw new ArgumentNullException("filter");
			}
			Initialize(dataSource, filter, null);
		}

		public Db4oBindingList(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort)
		{
			if(filter == null){
				throw new ArgumentNullException("filter");
			}
			if(sort == null){
				throw new ArgumentNullException("sort");
			}
			Initialize(dataSource, filter, sort);
		}

		public Db4oBindingList(Db4oDataSource dataSource, Comparison<T> sort)
		{
			if(sort == null){
				throw new ArgumentNullException("sort");
			}
			Initialize(dataSource, null, sort);
		}

		public void Add(IEnumerator<T> enumerator)
		{
			_records.WriteCacheSize = 0;
			while (enumerator.MoveNext())
			{
				Add(enumerator.Current);
			}
			_records.Commit();
			_records.WriteCacheSize = defaultWriteCacheSize;
		}

		public void Add(System.Collections.IEnumerator enumerator)
		{
			_records.WriteCacheSize = 0;
			while (enumerator.MoveNext())
			{
				Add((T)enumerator.Current);
			}
			_records.Commit();
			_records.WriteCacheSize = defaultWriteCacheSize;
		}

		public bool Commit()
		{
			return _records.Commit();
		}

		public int WriteCacheSize
		{
			get
			{
				VerifyNotDisposed();
				return _records.WriteCacheSize;
			}
			set
			{
				VerifyNotDisposed();
				_records.WriteCacheSize = value;
			}
		}

		#region IFilterable Members

		public void ApplyFilter(Predicate<T> filter)
		{
			VerifyNotDisposed();
			if (filter == null)
			{
				throw new ArgumentNullException();
			}
			_records.Filter = filter;
			OnListReset();
		}

		public void RemoveFilter()
		{
			VerifyNotDisposed();
			_records.Filter = null;
			OnListReset();
		}

		public bool IsFiltered
		{
			get
			{
				VerifyNotDisposed();
				return _records.IsFiltered;
			}
		}

		#endregion

		#region IBindingList Members

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
		}

		object IBindingList.AddNew()
		{
			T o = new T();
			Add(o);
			return o;
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return true;
			}
		}
		bool IBindingList.AllowNew
		{
			get
			{
				return true;
			}
		}
		bool IBindingList.AllowRemove
		{
			get
			{
				return true;
			}
		}

		public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			VerifyNotDisposed();
			Comparison<T> sort = delegate(T item1, T item2)
			{
				PropertyComparison<T> propertySorter = ComparisonHelper<T>.GetPropertyComparison(ComparisonHelper<T>.DefaultPropertyComparison, direction);
				return propertySorter(item1, item2, property);
			};

			_records.Sort(sort);
			_propertyDescriptor = property;
			_listSortDirection = direction;
			OnListReset();
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		public bool IsSorted
		{
			get
			{
				VerifyNotDisposed();
				return _records.IsSorted;
			}
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
			if (this.ListChanged != null)
			{
				this.ListChanged(this, e);
			}
		}

		public event ListChangedEventHandler ListChanged;

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			VerifyNotDisposed();
		}

		public void RemoveSort()
		{
			VerifyNotDisposed();

			if (IsSorted)
			{
				_records.RemoveSort();
				_propertyDescriptor = null;
				OnListReset();
			}
		}

		public ListSortDirection SortDirection
		{
			get
			{
				VerifyNotDisposed();
				return _listSortDirection;
			}
		}

		public PropertyDescriptor SortProperty
		{
			get
			{
				VerifyNotDisposed();
				return _propertyDescriptor;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				VerifyNotDisposed();
				return true;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				VerifyNotDisposed();
				return true;
			}
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
			_records.Insert(index, item);
			OnItemChanged(index);
		}

		public void RemoveAt(int index)
		{
			VerifyNotDisposed();
			_records.RemoveAt(index);
			OnItemDeleted(index);
		}

		public T this[int index]
		{
			get
			{
				VerifyNotDisposed();
				return _records[index];
			}
			set
			{
				VerifyNotDisposed();
				_records[index] = value;
				OnItemChanged(index);
			}
		}

		#endregion

		#region IList Members

		int System.Collections.IList.Add(object value)
		{
			VerifyNotDisposed();
			T item = (T)value;
			Add(item);
			return IndexOf(item);
		}

		void System.Collections.IList.Clear()
		{
			VerifyNotDisposed();
			Clear();
		}

		bool System.Collections.IList.Contains(object value)
		{
			VerifyNotDisposed();
			return Contains((T)value);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			VerifyNotDisposed();
			return IndexOf((T)value);
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			VerifyNotDisposed();
			Insert(index, (T)value);
		}

		public bool IsFixedSize
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				VerifyNotDisposed();
				return IsReadOnly;
			}
		}

		void System.Collections.IList.Remove(object value)
		{
			VerifyNotDisposed();
			Remove((T)value);
		}

		private void CheckIndex(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException();
			}
		}
		void System.Collections.IList.RemoveAt(int index)
		{
			VerifyNotDisposed();
			CheckIndex(index);
			RemoveAt(index);
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				VerifyNotDisposed();
				CheckIndex(index);
				return _records[index];
			}
			set
			{
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
			_records.Add(item);
			OnItemAdded(IndexOf(item));
		}

		public void Clear()
		{
			VerifyNotDisposed();
			int count = _records.Count;
			_records.Clear();
			for(int i=0; i < count; ++i)
			{
				OnItemDeleted(i);
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
			get
			{
				VerifyNotDisposed();
				return _records.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				VerifyNotDisposed();
				return _records.IsReadOnly;
			}
		}

		public bool Remove(T item)
		{
			VerifyNotDisposed();
			return _records.Remove(item);
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(Array array, int index)
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
			if (array.Rank > 1){
				throw new ArgumentException("array cannot be multidimensional", "array");
			}

			T[] tArray = new T[Count];
			CopyTo(tArray, 0);
			foreach (T t in tArray)
			{
				array.SetValue(t, index++);
			}
		}

		int System.Collections.ICollection.Count
		{
			get
			{
				VerifyNotDisposed();
				return Count;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				VerifyNotDisposed();
				return false;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				VerifyNotDisposed();
				return this;
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			VerifyNotDisposed();
			return ((System.Collections.IEnumerable)_records).GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			VerifyNotDisposed();
			return ((IEnumerable<T>)_records).GetEnumerator();
		}

		#endregion

		#region IDisposable Members

		private bool _isDisposed = false;
		public void Dispose()
		{
			if (!this._isDisposed)
			{
				this._records.Dispose();
				_isDisposed = true;
			}
			GC.SuppressFinalize(this);
		}
		protected void VerifyNotDisposed(){
			if (this._isDisposed)
			{
				throw new ObjectDisposedException("Db4oBindingList");
			}
		}
		~Db4oBindingList()
		{
			if (!this._isDisposed)
			{
				throw new ApplicationException("Disposed not explicitly called");
			}
		}


		#endregion
	}

}


