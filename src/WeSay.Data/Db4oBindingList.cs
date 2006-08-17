using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	public class Db4oBindingList<T> : IBindingList, IFilterable<T>, IList<T>, ICollection<T>, IEnumerable<T> where T : class, INotifyPropertyChanged, new()
	{
		Db4o.Binding.Db4oList<T> _records;
		PropertyDescriptor	_propertyDescriptor;
		ListSortDirection   _listSortDirection;

		private void Initialize(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort )
		{
			if(dataSource == null){
				throw new ArgumentNullException("dataSource");
			}

			_records = new Db4o.Binding.Db4oList<T>((com.db4o.ObjectContainer)dataSource.Data, new List<T>(), filter, sort);
			_records.ReadCacheSize = 0;
			_records.WriteCacheSize = 1;
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

		public int WriteCacheSize
		{
			get
			{
				return _records.WriteCacheSize;
			}
			set
			{
				_records.WriteCacheSize = value;
			}
		}

		#region IFilterable Members

		public void ApplyFilter(Predicate<T> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException();
			}
			_records.Filter = filter;
			OnListReset();
		}

		public void RemoveFilter()
		{
			_records.Filter = null;
			OnListReset();
		}

		public bool IsFiltered
		{
			get
			{
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
		}

		public void RemoveSort()
		{
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
				return _listSortDirection;
			}
		}

		public PropertyDescriptor SortProperty
		{
			get
			{
				return _propertyDescriptor;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return _records.IndexOf(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			_records.Insert(index, item);
			OnItemChanged(index);
		}

		public void RemoveAt(int index)
		{
			_records.RemoveAt(index);
			OnItemDeleted(index);
		}

		public T this[int index]
		{
			get
			{
				return _records[index];
			}
			set
			{
				_records[index] = value;
				OnItemChanged(index);
			}
		}

		#endregion

		#region IList Members

		int System.Collections.IList.Add(object value)
		{
			T item = (T)value;
			Add(item);
			return IndexOf(item);
		}

		void System.Collections.IList.Clear()
		{
			Clear();
		}

		bool System.Collections.IList.Contains(object value)
		{
			return Contains((T)value);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return IndexOf((T)value);
		}

		void System.Collections.IList.Insert(int index, object value)
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

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return IsReadOnly;
			}
		}

		void System.Collections.IList.Remove(object value)
		{
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
			CheckIndex(index);
			RemoveAt(index);
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				CheckIndex(index);
				return _records[index];
			}
			set
			{
				CheckIndex(index);
				_records[index] = (T)value;
				OnItemChanged(index);
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			_records.Add(item);
			OnItemAdded(IndexOf(item));
		}

		public void Clear()
		{
			int count = _records.Count;
			_records.Clear();
			for(int i=0; i < count; ++i)
			{
				OnItemDeleted(i);
			}
		}

		public bool Contains(T item)
		{
			return _records.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_records.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return _records.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return _records.IsReadOnly;
			}
		}

		public bool Remove(T item)
		{
			return _records.Remove(item);
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
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
				return Count;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((System.Collections.IEnumerable)_records).GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return ((IEnumerable<T>)_records).GetEnumerator();
		}

		#endregion
}

}


