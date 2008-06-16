using System;
using System.Collections.Generic;

namespace WeSay.Data
{
	public class RecordToken<T>: IEquatable<RecordToken<T>>
	{
		private readonly string _displayString;
		private readonly RepositoryId _id;
		private readonly IRepository<T> _repository;
		private readonly IQuery<T> _query;
		private readonly int _index;
		public RecordToken(IRepository<T> repository, IQuery<T> query, int index)
		{
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}
			if(index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "must be positive");
			}
			_repository = repository;
			_query = query;
			_index = index;
		}

		public RecordToken(IRepository<T> repository, IQuery<T> query, int index, string s, RepositoryId id)
			: this(repository, query, index)
		{
			_displayString = s;
			this._query = query;
			_id = id;
		}

		public string DisplayString
		{
			get { return this._displayString; }
		}

		public RepositoryId Id
		{
			get { return this._id; }
		}
		public override string ToString()
		{
			return DisplayString;
		}

		public bool IsFresh
		{
			get {
				int i = 0;
				foreach (string displayString in this._query.GetDisplayStrings(RealObject))
				{
					if (i == this._index)
					{
						return DisplayString == displayString;
					}
					++i;
				}
				return false;
			}
		}


		// proxy object
		public T RealObject
		{
			get
			{
				return _repository.GetItem(Id);
			}
		}

		public static int FindFirstWithDisplayString(List<RecordToken<T>> recordTokens, string displayString)
		{
			return recordTokens.FindIndex(delegate (RecordToken<T> r)
							  { return r.DisplayString == displayString; });
			// todo This could be a binary search. We need our own search so that
			// we find the first element. The .net BinarySearch finds any, not the first.
		}

		public static bool operator !=(RecordToken<T> recordToken1, RecordToken<T> recordToken2)
		{
			return !Equals(recordToken1, recordToken2);
		}

		public static bool operator ==(RecordToken<T> recordToken1, RecordToken<T> recordToken2)
		{
			return Equals(recordToken1, recordToken2);
		}

		public bool Equals(RecordToken<T> recordToken)
		{
			if (recordToken == null)
			{
				return false;
			}
			return Equals(_displayString, recordToken._displayString) && Equals(_id, recordToken._id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj as RecordToken<T>);
		}

		public override int GetHashCode()
		{
			return _displayString.GetHashCode() + 29 * _id.GetHashCode();
		}

		public static int IndexOf(List<RecordToken<T>> records, T item, int index, int count)
		{
			if(index < 0 || index >= records.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			if(index + count > records.Count)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			RepositoryId id = null;
			if (records.Count > 0)
			{
				id = records[0]._repository.GetId(item);
			}

			for(int i = index; i < index + count; ++i)
			{
				if(records[i].Id == id)
				{
					return i;
				}
			}
			return -1;
		}

		public static int IndexOf(List<RecordToken<T>> records, T item)
		{
			if(records.Count == 0)
			{
				return -1;
			}
			return IndexOf(records, item, 0, records.Count);
		}

		public static int IndexOf(List<RecordToken<T>> records, T item, int index)
		{
			return IndexOf(records, item, index, records.Count-index);
		}

	}
}
