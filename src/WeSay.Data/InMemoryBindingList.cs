using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace WeSay.Data
{
	public sealed class InMemoryBindingList<T> : IBindingList, IEquatable<IBindingList>, IList<T> where T : class, new()
	{
		readonly List<T> _list;
		PropertyDescriptor _sortProperty;
		ListSortDirection _listSortDirection;
		bool _isSorted;

		public InMemoryBindingList()
		{
			_list = new List<T>();
		}

		public InMemoryBindingList(IBindingList original)
			: this()
		{
			AddRange(original);
			_isSorted = original.IsSorted;
			_sortProperty = original.SortProperty;
			_listSortDirection = original.SortDirection;
		}

		public void AddRange(IEnumerable<T> collection)
		{
			IEnumerator<T> enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Add(enumerator.Current);
			}
		}

		public void AddRange(IEnumerable collection)
		{
			IEnumerator enumerator = collection.GetEnumerator();
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
				_sortProperty = property;
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

		private void OnItemAdded(int newIndex)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, newIndex));
		}

		private void OnItemChanged(int newIndex)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, newIndex));
		}

		private void OnItemDeleted(int oldIndex)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, oldIndex));
		}

		private void OnListReset()
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		private void OnListChanged(ListChangedEventArgs e)
		{
			ListChanged(this, e);
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
				_sortProperty = null;
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
				return _sortProperty;
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

		int IList.Add(object value)
		{
			T item = (T)value;
			Add(item);
			return IndexOf(item);
		}

		void IList.Clear()
		{
			Clear();
		}

		bool IList.Contains(object value)
		{
			return Contains((T)value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T)value);
		}

		void IList.Insert(int index, object value)
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

		bool IList.IsReadOnly
		{
			get
			{
				return IsReadOnly;
			}
		}

		void IList.Remove(object value)
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

		void IList.RemoveAt(int index)
		{
			CheckIndex(index);
			RemoveAt(index);
		}

		object IList.this[int index]
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
			_list.Clear();
			OnListReset();
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
			int index = _list.IndexOf(item);
			if(index != -1){
				_list.RemoveAt(index);
				OnItemDeleted(index);
				return true;
			}
			return false;
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index)
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

		int ICollection.Count
		{
			get
			{
				return Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
		  get
		  {
			return this;
		  }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) _list).GetEnumerator();
		}

	  #endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return ((IEnumerable<T>)_list).GetEnumerator();
		}

		#endregion


		#region IEquatable<IBindingList> Members

		public bool Equals(IBindingList other)
		{
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
				if (!this[i].Equals(other[i]))
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
			IBindingList recordList = obj as IBindingList;
			if (recordList == null)
			{
				return false;
			}

			return Equals(recordList);
		}
		#endregion

		public override int GetHashCode()
		{
			int hashCode = _list.GetHashCode();

			if (_isSorted)
			{
				hashCode ^= _sortProperty.GetHashCode() ^ _listSortDirection.GetHashCode();
			}
			return hashCode;
		}
	}
}
