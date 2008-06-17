using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	public class ResultSet<T> : IEnumerable<RecordToken<T>>
	{
		private readonly List<RecordToken<T>> _results;
		private readonly IRepository<T> _repository;
		public ResultSet(IRepository<T> repository, List<RecordToken<T>> results) {
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			this._results = results;
			_repository = repository;
		}

		public ResultSet(IRepository<T> repository, IEnumerable<RecordToken<T>> results)
			: this(repository, new List<RecordToken<T>>(results))
		{}

		public RecordToken<T> this[int index]
		{
			get
			{
				return _results[index];
			}
		}

		public int Count
		{
			get
			{
				return _results.Count;
			}
		}

		private RecordToken<T> GetItemFromIndex(int index)
		{
			if (index < 0)
			{
				return null;
			}
			return this[index];
		}

		#region FindFirst(Index) Of DisplayString

		public RecordToken<T> FindFirstWithDisplayString(string displayString)
		{
			int index = FindFirstIndexWithDisplayString(displayString);
			return GetItemFromIndex(index);
		}

		public RecordToken<T> FindFirstWithDisplayString(string displayString, int startIndex)
		{
			int index = FindFirstIndexWithDisplayString(displayString, startIndex);
			return GetItemFromIndex(index);
		}

		public RecordToken<T> FindFirstWithDisplayString(string displayString, int startIndex, int count)
		{
			int index = FindFirstIndexWithDisplayString(displayString, startIndex, count);
			return GetItemFromIndex(index);
		}

		public int FindFirstIndexWithDisplayString(string displayString)
		{
			return _results.FindIndex(delegate(RecordToken<T> r)
							  {
								  return r.DisplayString == displayString;
							  });
			// todo This could be a binary search. We need our own search so that
			// we find the first element. The .net BinarySearch finds any, not the first.
		}

		public int FindFirstIndexWithDisplayString(string displayString, int startIndex, int count)
		{
			return _results.FindIndex(startIndex,
									  count,
									  delegate(RecordToken<T> r)
									  {
										  return r.DisplayString == displayString;
									  });
		}

		public int FindFirstIndexWithDisplayString(string displayString, int startIndex)
		{
			return _results.FindIndex(startIndex,
									  delegate(RecordToken<T> r)
									  {
										  return r.DisplayString == displayString;
									  });
		}

		#endregion

		#region FindFirst(Index) of RepositoryId

		public RecordToken<T> FindFirst(RepositoryId id)
		{
			int index = FindFirstIndex(id);
			return GetItemFromIndex(index);
		}

		public RecordToken<T> FindFirst(RepositoryId id, int startIndex)
		{
			int index = FindFirstIndex(id, startIndex);
			return GetItemFromIndex(index);
		}

		public RecordToken<T> FindFirst(RepositoryId id, int startIndex, int count)
		{
			int index = FindFirstIndex(id, startIndex, count);
			return GetItemFromIndex(index);
		}

		public int FindFirstIndex(RepositoryId id, int startIndex, int count)
		{
			return _results.FindIndex(startIndex,
									  count,
									  delegate(RecordToken<T> r)
										  {
											  return (r.Id == id);
										  });
		}

		public int FindFirstIndex(RepositoryId id)
		{
			return _results.FindIndex(delegate(RecordToken<T> r)
										  {
											  return (r.Id == id);
										  });
		}

		public int FindFirstIndex(RepositoryId id, int startIndex)
		{
			return _results.FindIndex(startIndex,
									  delegate(RecordToken<T> r)
										  {
											  return (r.Id == id);
										  });
		}
		#endregion

		#region FindFirst(Index) of T
		public RecordToken<T> FindFirst(T item)
		{
			int index = FindFirstIndex(item);
			return GetItemFromIndex(index);
		}

		public RecordToken<T> FindFirst(T item, int startIndex)
		{
			int index = FindFirstIndex(item, startIndex);
			return GetItemFromIndex(index);
		}

		public RecordToken<T> FindFirst(T item, int startIndex, int count)
		{
			int index = FindFirstIndex(item, startIndex, count);
			return GetItemFromIndex(index);
		}

		public int FindFirstIndex(T item)
		{
			RepositoryId id = _repository.GetId(item);
			return FindFirstIndex(id);
		}

		public int FindFirstIndex(T item, int startIndex)
		{
			RepositoryId id = _repository.GetId(item);
			return FindFirstIndex(id, startIndex);
		}

		public int FindFirstIndex(T item, int startIndex, int count)
		{
			RepositoryId id = _repository.GetId(item);
			return FindFirstIndex(id, startIndex, count);
		}

		#endregion

		#region FindFirstIndex of RecordToken<T>

		public int FindFirstIndex(RecordToken<T> token, int startIndex, int count)
		{
			return _results.FindIndex(startIndex,
									  count,
									  delegate(RecordToken<T> r)
									  {
										  return (r == token);
									  });
		}

		public int FindFirstIndex(RecordToken<T> token)
		{
			return _results.FindIndex(delegate(RecordToken<T> r)
										  {
											  return (r == token);
										  });
		}

		public int FindFirstIndex(RecordToken<T> token, int startIndex)
		{
			return _results.FindIndex(startIndex,
									  delegate(RecordToken<T> r)
									  {
										  return (r == token);
									  });
		}

		#endregion

		public static explicit operator BindingList<RecordToken<T>>(ResultSet<T> results)
		{
			return new BindingList<RecordToken<T>>(results._results);
		}

		#region IEnumerable<RecordToken<T>> Members

		IEnumerator<RecordToken<T>> IEnumerable<RecordToken<T>>.GetEnumerator()
		{
			return this._results.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<RecordToken<T>>) this).GetEnumerator();
		}

		#endregion
	}
}
