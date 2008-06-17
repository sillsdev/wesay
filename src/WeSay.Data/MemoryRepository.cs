using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
	class MemoryRepository<T>:IRepository<T> where T : new()
	{
		private Hashtable hashtableIdToObject = new Hashtable();
		private Hashtable hashtableObjectToId = new Hashtable();

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
				this.id = nextId;
				++nextId;
			}

			public int Id
			{
				get { return id; }
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
				if (!base.Equals(memoryRepositoryId)) return false;
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
		}
	}
}
