using System.Collections.Generic;

namespace WeSay.Data
{
	internal sealed class Db4oRepositoryId: RepositoryId
	{
		private readonly long _id;

		public Db4oRepositoryId(long id)
		{
			_id = id;
		}

		public long Db4oId
		{
			get { return _id; }
		}

		public int CompareTo(Db4oRepositoryId other)
		{
			if (other == null)
			{
				return -1;
			}
			return Comparer<long>.Default.Compare(Db4oId, other.Db4oId);
		}

		public override int CompareTo(RepositoryId other)
		{
			return CompareTo(other as Db4oRepositoryId);
		}

		public override bool Equals(RepositoryId other)
		{
			return Equals(other as Db4oRepositoryId);
		}

		public static bool operator !=(
				Db4oRepositoryId db4oRepositoryId1, Db4oRepositoryId db4oRepositoryId2)
		{
			return !Equals(db4oRepositoryId1, db4oRepositoryId2);
		}

		public static bool operator ==(
				Db4oRepositoryId db4oRepositoryId1, Db4oRepositoryId db4oRepositoryId2)
		{
			return Equals(db4oRepositoryId1, db4oRepositoryId2);
		}

		public bool Equals(Db4oRepositoryId db4oRepositoryId)
		{
			if (db4oRepositoryId == null)
			{
				return false;
			}
			return _id == db4oRepositoryId._id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj as Db4oRepositoryId);
		}

		public override int GetHashCode()
		{
			return (int) _id;
		}

		public override string ToString()
		{
			return _id.ToString();
		}
	}
}