using System;
using System.Collections.Generic;

namespace WeSay.Data
{
	public sealed class RecordToken<T>: IEquatable<RecordToken<T>> where T:class ,new()
	{
		public delegate IEnumerable<string[]> DisplayStringGenerator(T item);

		private readonly Dictionary<string, object> _queryResults;
		private readonly RepositoryId _id;
		private readonly IRepository<T> _repository;

		public RecordToken(IRepository<T> repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			_repository = repository;
		}

		[Obsolete]
		public RecordToken(IRepository<T> repository,
						   string s,
						   RepositoryId id)
			: this(repository, new Dictionary<string, object>(), id)
		{
			_queryResults.Add("", s);
		}

		public RecordToken(IRepository<T> repository,
				   Dictionary<string, object> queryResults,
				   RepositoryId id)
			: this(repository)
		{
			_queryResults = queryResults;
			_id = id;
		}

		[Obsolete]
		public string DisplayString
		{
			get { return (string)_queryResults[""]; }
		}

		public RepositoryId Id
		{
			get { return _id; }
		}

		// proxy object
		public T RealObject
		{
			get { return _repository.GetItem(Id); }
		}

		public IDictionary<string, object> Results
		{
			get
			{
				return _queryResults;
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
			return Equals(_id, recordToken._id)
				   && new DictionaryEqualityComparer<string, object>()
					  .Equals(_queryResults, recordToken._queryResults);
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
			int queryResultsHash = new DictionaryEqualityComparer<string, object>()
											.GetHashCode(this._queryResults);
			return queryResultsHash + 29 * _id.GetHashCode();
		}
	}
}