using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	public class Db4oRecordList<T> : IRecordList<T> where T : class, new()
	{
		Db4o.Binding.Db4oList<T> _records;
		PropertyDescriptor _sortProperty;
		ListSortDirection _listSortDirection;
		private static int defaultWriteCacheSize = 0;
		PropertyDescriptorCollection _pdc;


		private void Initialize(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort, Db4o.Binding.SODAQueryProvider sodaQuery)
		{
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}
			_pdc = TypeDescriptor.GetProperties(typeof(T));

			_records = new Db4o.Binding.Db4oList<T>((com.db4o.ObjectContainer)dataSource.Data, new List<T>(), filter, sort);
			_records.SortingInDatabase = false;
			_records.ReadCacheSize = 0; // I think this could go back to lower
			_records.WriteCacheSize = defaultWriteCacheSize;
			_records.PeekPersistedActivationDepth = 99;
			_records.ActivationDepth = 99;
			_records.RefreshActivationDepth = 99;
			_records.SetActivationDepth = 99;
			//            _records.RequeryAndRefresh(false);
			_records.Storing += new EventHandler<Db4o.Binding.Db4oListEventArgs<T>>(OnRecordStoring);
			if (_records.FilteringInDatabase && filter != null)
			{
				_records.Commit();
			}
			try
			{
				if (sodaQuery != null)
				{
					_records.SODAQuery = sodaQuery;
				}
				else
				{
					_records.Requery(false);
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw (e);
			}
		}

		public Db4oRecordList(Db4oDataSource dataSource)
		{
			Initialize(dataSource, null, null, null);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Db4o.Binding.SODAQueryProvider sodaQuery)
		{
			Initialize(dataSource, null, null, sodaQuery);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Predicate<T> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			Initialize(dataSource, filter, null, null);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (sort == null)
			{
				throw new ArgumentNullException("sort");
			}
			Initialize(dataSource, filter, sort, null);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Comparison<T> sort)
		{
			if (sort == null)
			{
				throw new ArgumentNullException("sort");
			}
			Initialize(dataSource, null, sort, null);
		}

		void OnRecordStoring(object sender, Db4o.Binding.Db4oListEventArgs<T> e)
		{
			if (e.PropertyName == string.Empty)
			{
				OnItemChanged(_records.IndexOf(e.Item));
			}
			else
			{
				OnItemChanged(_records.IndexOf(e.Item), e.PropertyName);
			}
		}

		public Db4o.Binding.SODAQueryProvider SODAQuery
		{
			get
			{
				VerifyNotDisposed();
				return _records.SODAQuery;
			}
			set
			{
				VerifyNotDisposed();
				_records.SODAQuery = value;
			}
		}

		public void Add(IEnumerator<T> enumerator)
		{
			VerifyNotDisposed();
			_records.WriteCacheSize = 0;
			while (enumerator.MoveNext())
			{
				Add(enumerator.Current);
			}
			_records.Commit();
			_records.WriteCacheSize = defaultWriteCacheSize;
		}

		public bool Commit()
		{
			VerifyNotDisposed();
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
			if (_records.FilteringInDatabase)
			{
				_records.Commit();
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

		public void RefreshFilter()
		{
			VerifyNotDisposed();
			if (_records.IsFiltered)
			{
				if (_records.FilteringInDatabase)
				{
					this._records.Requery(true);
					OnListReset();
				}
				else
				{
					Predicate<T> filter = _records.Filter;
					_records.Filter = null;
					ApplyFilter(filter);
				}
			}
			else if (_records.SODAQuery != null)
			{
				_records.Requery(true);
			}

		}

		public bool IsFiltered
		{
			get
			{
				VerifyNotDisposed();
				return _records.IsFiltered || _records.SODAQuery != null;
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
			_sortProperty = property;
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

		protected virtual void OnItemChanged(int newIndex, string field)
		{
			PropertyDescriptor propertyDescriptor = _pdc.Find(field, false);
			if (propertyDescriptor == null)
			{
				OnItemChanged(newIndex);
			}

			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, newIndex, propertyDescriptor));
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
			this.ListChanged(this, e);
		}

		public event ListChangedEventHandler ListChanged = delegate
		{
		};

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
				_sortProperty = null;
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
				return _sortProperty;
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
			OnListReset();
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

#if DEBUG
		~Db4oRecordList()
		{
			if (!this._disposed)
			{
				throw new ApplicationException("Disposed not explicitly called on Db4oRecordList.");
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
					this._records.Dispose();
				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("Db4oRecordList");
			}
		}
		#endregion
		#region IEquatable<IRecordList<T>> Members

		public bool Equals(IRecordList<T> other)
		{
			if (other == null)
			{
				return false;
			}
			if (this.Count != other.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Count; i++)
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
			int hashCode = _records.GetHashCode();

			if (_records.IsSorted)
			{
				hashCode ^= _sortProperty.GetHashCode() ^ _listSortDirection.GetHashCode();

			}

			return hashCode;
		}


	}

}
