using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	public class RecordListEventArgs<T> : CancelEventArgs
	{
		/// <summary>
		/// Constructor with <see cref="CancelEventArgs.Cancel"/> false and <see cref="Item"/> set to <paramref name="item"/>.
		/// </summary>
		/// <param name="item">Item.</param>
		public RecordListEventArgs(T item)
		{
			this._item = item;
		}

		/// <summary>
		/// Item which the event is about.
		/// </summary>
		public T Item
		{
			get
			{
				return _item;
			}
		}

		/// <summary>
		/// See <see cref="Item"/>.
		/// </summary>
		private T _item;
	}


	public interface IRecordList<T> : IBindingList, IList<T>, ICollection<T>, IFilterable<T>, IDisposable, IEquatable<IRecordList<T>>, IEnumerable<T> where T : class, new()
	{
		event EventHandler<RecordListEventArgs<T>> AddingRecord;
		event EventHandler<RecordListEventArgs<T>> DeletingRecord;

		/// <summary>
		/// Indicates that changes that have been made should be persisted if possible
		/// </summary>
		/// <returns>True if successful</returns>
  //      bool Commit();

		/// <summary>
		/// Gets count of elements in container
		/// </summary>
		new int Count
		{
			get;
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		/// <exception cref="System.NotSupportedException">The property is set and the System.Collections.Generic.IList<T> is read-only</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">index is not a valid index in the System.Collections.Generic.IList<T>.</exception>
		new T this[int index]
		{
			get;
			set;
		}

		new void RemoveAt(int index);

		 /// <summary>
		/// Removes all items from the collection
		/// </summary>
		new void Clear();

		int GetIndexFromId(long id);
	}
}
