using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WeSay.Data
{
	public interface IRecordList<T> : IBindingList, IList<T>, ICollection<T>, IFilterable<T>, IDisposable, IEquatable<IRecordList<T>>, IEnumerable<T> where T : class, new()
	{
		/// <summary>
		/// Indicates that changes that have been made should be persisted if possible
		/// </summary>
		/// <returns>True if successful</returns>
		bool Commit();
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

	}
}
