using System;

namespace WeSay.Data
{
	public sealed class RecordToken<T>: IEquatable<RecordToken<T>>
	{
		private string _displayString;
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
			get
			{
				return DisplayString == GetRefreshedDisplayStringForObject();
			}
		}

		/// <summary>
		/// Attempts to freshen the display string when activity on the object
		/// may have changed it. This is not guaranteed to make the recordtoken
		/// fresh. (A record may have two recordtokens associated with it but after editing
		/// only have one. The second is stale no matter how much refreshing happens.)
		/// </summary>
		public void Refresh()
		{
			string refreshedDisplayStringForObject = GetRefreshedDisplayStringForObject();
			if (refreshedDisplayStringForObject != null)
			{
				this._displayString = refreshedDisplayStringForObject;
			}
		}

		private string GetRefreshedDisplayStringForObject()
		{
			int i = 0;
			foreach (string displayString in this._query.GetDisplayStrings(RealObject))
			{
				if (i == this._index)
				{
					return displayString;
				}
				++i;
			}
			return null;
		}

		// proxy object
		public T RealObject
		{
			get
			{
				return _repository.GetItem(Id);
			}
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
	}
}
