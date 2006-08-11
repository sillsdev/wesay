using System;
using System.Collections.Generic;
using System.Text;
using com.db4o;
using System.ComponentModel;

namespace WeSay.Data
{
	public class Db4oDataAdaptor<T> : IBindingList, IFilterable<T>, IList<T>, ICollection<T>, IEnumerable<T> where T : new()
	{
		ObjectContainer _db;
		IList<T> _records;
		Predicate<T> _filter;
		IComparer<T> _sort;

		static Predicate<T> _noFilter = delegate(T o)
			{
				return true;
			};


		public void ApplyFilter(Predicate<T> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException();
			}
			_filter = filter;
			Load();
			OnListReset();
		}

		public void RemoveFilter()
		{
			_filter = _noFilter;
			Load();
			OnListReset();
		}

		public bool IsFiltered
		{
			get
			{
				return _filter == _noFilter;
			}
		}

		public IComparer<T> Sort
		{
			get
			{
				return _sort;
			}
			set
			{
				_sort = value;
				Load();
				OnListReset();
			}
		}

		public Db4oDataAdaptor(Db4oBindingListConfiguration<T> configuration)
		{
			this._db = (ObjectContainer)configuration.DataSource.Data;
			_filter = configuration.Filter;
			if (configuration.Filter == null)
			{
				_filter = _noFilter;
			}
			this._sort = configuration.Sort;
			Load();
		}

		private void Db4oSet(T o)
		{
			_db.Set(o);
			_db.Commit();
		}

		public void Update(T o)
		{
			Db4oSet(o);
		}

		public void Load()
		{
			if (this.Sort == null)
			{
				_records = _db.Query<T>(_filter);
			}
			else
			{
				_records = _db.Query<T>(_filter, this.Sort);
			}
		}

		protected virtual void OnItemAdded(int newIndex)
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, newIndex));
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


		#region IBindingList Members

//        private bool _isSorted;

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

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		bool IBindingList.IsSorted
		{
			get
			{
				throw new NotSupportedException();
				//                return _isSorted;
			}
		}

		public event ListChangedEventHandler ListChanged;

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
			//            _isSorted = false;
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				throw new NotSupportedException();
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
				return false;
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
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			Remove(this[index]);
		}

		public T this[int index]
		{
			get
			{
				return _records[index];
			}
			set
			{
				throw new NotSupportedException();
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

		void System.Collections.IList.RemoveAt(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException();
			}

			RemoveAt(index);
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException();
				}

				return this[index];
			}
			set
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException();
				}

				this[index] = (T)value;
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			Db4oSet(item);
			Load();
			OnItemAdded(IndexOf(item));
		}

		public void Clear()
		{
			foreach (T item in _records)
			{
				_db.Delete(item);
			}
			_db.Commit();
			Load();
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
				return false;
			}
		}

		public bool Remove(T item)
		{
			if (!Contains(item))
			{
				return false;
			}
			int index = this.IndexOf(item);
			_db.Delete(item);
			_db.Commit();
			Load();
			OnItemDeleted(index);
			return true;
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			if (index + Count > array.Length || array.Rank > 1)
			{
				throw new ArgumentException();
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

		#region Enumerator

		public struct Enumerator : IEnumerator<T>
		{
			Db4oDataAdaptor<T> _collection;
			int _index;

			public Enumerator(Db4oDataAdaptor<T> collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException();
				}
				_collection = collection;
				_index = -1;
			}
			private void CheckValidIndex(int ValidMinimum)
			{
				if (_index < ValidMinimum || _index >= _collection.Count)
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

			#region IEnumerator<T> Members

			public T Current
			{
				get
				{
					CheckValidIndex(0);
					CheckCollectionUnchanged();
					return ((IList<T>)_collection)[_index];
				}
			}

			#endregion

			#region IDisposable Members

			void IDisposable.Dispose()
			{
			}

			#endregion

			#region IEnumerator Members

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public bool MoveNext()
			{
				CheckValidIndex(-1);
				CheckCollectionUnchanged();
				return (++_index < _collection.Count);
			}

			public void Reset()
			{
				CheckCollectionUnchanged();
				_index = -1;
			}

			#endregion
		}

		public Enumerator GetEnumerator()
		{
			_isEnumerating = true;
			return new Enumerator(this);
		}


		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
		#endregion


		private bool _isEnumerating;
	}
}


//public class Db4oBindingList<T> : BindingList<T> where T : new()
//{
//    ObjectContainer _data;

//    public Db4oBindingList()
//        : base()
//    {
//    }

//    private void Db4oSet(T o)
//    {
//        _data.Set(o);
//        _data.Commit();
//    }

//    protected override object AddNewCore()
//    {
//        T o = new T();
//        Db4oSet(o);
//        this.Add(o);
//        return o;
//    }
//    //        protected override
//}
