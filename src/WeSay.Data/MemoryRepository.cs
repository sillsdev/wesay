using System;
using System.Collections;
using System.Collections.Generic;
using WeSay.Data;

namespace WeSay.Data
{
	class MemoryRepository<T>:IRepository<T> where T : new()
	{
		private readonly Hashtable hashtableIdToObject = new Hashtable();
		private readonly Hashtable hashtableObjectToId = new Hashtable();

		public T CreateItem()
		{
			T t = new T();
			MemoryRepositoryId id = new MemoryRepositoryId();
			hashtableIdToObject.Add(id, t);
			hashtableObjectToId.Add(t, id);
			return t;
		}

		public int CountAllItems()
		{
			return hashtableIdToObject.Count;
		}

		public RepositoryId GetId(T item)
		{
			if (!hashtableObjectToId.ContainsKey(item))
			{
				throw new ArgumentOutOfRangeException("item");
			}
			return (RepositoryId)hashtableObjectToId[item];
		}

		public T GetItem(RepositoryId id)
		{
			if (!hashtableIdToObject.ContainsKey(id))
			{
				throw new ArgumentOutOfRangeException("id");
			}
			return (T)hashtableIdToObject[id];
		}

		public void DeleteItem(T item)
		{
			if(!hashtableObjectToId.ContainsKey(item))
			{
				throw new ArgumentOutOfRangeException("item");
			}
			//hashtableIdToObject.Remove(hashtableObjectToId[item]);
			//hashtableObjectToId.Remove(item);
		}

		public void DeleteItem(RepositoryId id)
		{
			throw new NotImplementedException();
		}

		private class MemoryRepositoryId:RepositoryId
		{
			private static int nextId = 1;
			private readonly int id;

			public MemoryRepositoryId()
			{
				id = nextId;
				++nextId;
			}

			public static bool operator !=(MemoryRepositoryId memoryRepositoryId1, MemoryRepositoryId memoryRepositoryId2)
			{
				return !Equals(memoryRepositoryId1, memoryRepositoryId2);
			}

			public static bool operator ==(MemoryRepositoryId memoryRepositoryId1, MemoryRepositoryId memoryRepositoryId2)
			{
				return Equals(memoryRepositoryId1, memoryRepositoryId2);
			}

			public bool Equals(MemoryRepositoryId memoryRepositoryId)
			{
				if (memoryRepositoryId == null) return false;

				return id == memoryRepositoryId.id;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(this, obj)) return true;
				return Equals(obj as MemoryRepositoryId);
			}

			public override int GetHashCode()
			{
				return id;
			}

			public int CompareTo(MemoryRepositoryId other)
			{
				if (other == null)
				{
					return -1;
				}
				return Comparer<int>.Default.Compare(id, other.id);
			}

			public override int CompareTo(RepositoryId other)
			{
				return CompareTo(other as MemoryRepositoryId);
			}

			public override bool Equals(RepositoryId other)
			{
				return Equals(other as MemoryRepositoryId);
			}
		}
	}
}