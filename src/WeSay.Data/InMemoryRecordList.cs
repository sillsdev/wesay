using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace WeSay.Data
{
	public class InMemoryRecordList<T> : IRecordList<T> where T : class, new()
	{
		List<T> _list;
		PropertyDescriptor _propertyDescriptor;
		ListSortDirection _listSortDirection;
		bool _isSorted;
		bool _isFiltered;

		public InMemoryRecordList()
		{
			_list = new List<T>();
		}

		public void Add(IEnumerator<T> enumerator)
		{
			while (enumerator.MoveNext())
			{
				Add(enumerator.Current);
			}
		}

		public void Add(IEnumerator enumerator)
		{
			while (enumerator.MoveNext())
			{
				Add((T)enumerator.Current);
			}
		}

		#region IBindingList Members

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
		}

		public T AddNew()
		{
			T o = new T();
			Add(o);
			return o;
		}

		object IBindingList.AddNew()
		{
			return AddNew();
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
			if (_list.Count > 1)
			{
				Comparison<T> sort = delegate(T item1, T item2)
				{
					PropertyComparison<T> propertySorter = ComparisonHelper<T>.GetPropertyComparison(ComparisonHelper<T>.DefaultPropertyComparison, direction);
					return propertySorter(item1, item2, property);
				};

				_list.Sort(sort);
				_propertyDescriptor = property;
				_listSortDirection = direction;
				_isSorted = true;
				OnListReset();
			}
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		public bool IsSorted
		{
			get
			{
				return _isSorted;
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
			this.ListChanged(this, e);
		}

		public event ListChangedEventHandler ListChanged = delegate
		{
		};

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		public void RemoveSort()
		{
			if (IsSorted)
			{
				_isSorted = false;
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

		#region IFilterable<T> Members

		public void ApplyFilter(Predicate<T> itemsToInclude)
		{
			if (itemsToInclude == null)
			{
				throw new ArgumentNullException();
			}
			Predicate<T> itemsToExclude = ComparisonHelper<T>.GetInversePredicate(itemsToInclude);
			_list.RemoveAll(itemsToExclude);
			_isFiltered = true;
			OnListReset();
		}

		public void RemoveFilter()
		{
			throw new NotImplementedException();
			//_records.Filter = null;
			//OnListReset();
		}

		public void RefreshFilter()
		{
			throw new NotImplementedException();
			//OnListReset();
		}

		public bool IsFiltered
		{
			get
			{
				return _isFiltered;
			}
		}
#endregion
		#region IList<T> Members

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_list.Insert(index, item);
			OnItemChanged(index);
		}

		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
			OnItemDeleted(index);
		}

		public T this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				_list[index] = value;
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
			Insert(index, (T)value);
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
				return _list[index];
			}
			set
			{
				CheckIndex(index);
				_list[index] = (T)value;
				OnItemChanged(index);
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			_list.Add(item);
			OnItemAdded(IndexOf(item));
		}

		public void Clear()
		{
			int count = _list.Count;
			_list.Clear();
			for (int i = 0; i < count; ++i)
			{
				OnItemDeleted(i);
			}
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(T item)
		{
			return _list.Remove(item);
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
			return ((System.Collections.IEnumerable)_list).GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return ((IEnumerable<T>)_list).GetEnumerator();
		}

		#endregion
	}

}
