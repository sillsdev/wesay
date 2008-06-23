using System;
using System.Collections.Generic;

namespace WeSay.Data
{
	public sealed class RecordToken<T>: IEquatable<RecordToken<T>> where T:class ,new()
	{
		public delegate IEnumerable<string[]> DisplayStringGenerator(T item);

		private readonly string[] _displayStrings;
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

		public RecordToken(IRepository<T> repository,
						   string s,
						   RepositoryId id)
			: this(repository, new string[] { s }, id)
		{
		}

		public RecordToken(IRepository<T> repository,
				   string[] s,
				   RepositoryId id)
			: this(repository)
		{
			_displayStrings = s;
			_id = id;
		}

		public string DisplayString
		{
			get { return _displayStrings[0]; }
		}

		public RepositoryId Id
		{
			get { return _id; }
		}

		public override string ToString()
		{
			return DisplayString;
		}

		private static bool ContainSameResults(string[] results, string[] strings)
		{
			if(results.Length != strings.Length)
				return false;
			for(int i = 0; i != results.Length;++i)
			{
				if(results[i] != strings[i])
					return false;
			}
			return true;
		}

		// proxy object
		public T RealObject
		{
			get { return _repository.GetItem(Id); }
		}

		public string[] Results
		{
			get
			{
				return _displayStrings;
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
			return ContainSameResults(_displayStrings, recordToken._displayStrings)
				   && Equals(_id, recordToken._id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj as RecordToken<T>);
		}

		//todo: fixme to incorporate each of the displaystrings
		public override int GetHashCode()
		{
			return _displayStrings.GetHashCode() + 29 * _id.GetHashCode();
		}
	}
}