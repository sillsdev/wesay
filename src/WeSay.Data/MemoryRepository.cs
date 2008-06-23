using System;
using System.Collections;
using System.Collections.Generic;

namespace WeSay.Data
{
	class MemoryRepository<T>:IRepository<T> where T : class, new()
	{
		private readonly Hashtable idToObjectHashtable = new Hashtable();
		private readonly Hashtable objectToIdHashtable = new Hashtable();
		private readonly List<KeyValuePair<DateTime, RepositoryId>> modifiedToIdList =
			new List<KeyValuePair<DateTime, RepositoryId>>();
		private DateTime lastModified = DateTime.MinValue;

		public DateTime LastModified
		{
			get { return lastModified; }
			private set
			{
				value = value.ToUniversalTime();
				lastModified = value;
			}
		}

		public T CreateItem()
		{
			T t = new T();
			MemoryRepositoryId id = new MemoryRepositoryId();
			idToObjectHashtable.Add(id, t);
			objectToIdHashtable.Add(t, id);
			modifiedToIdList.Add(new KeyValuePair<DateTime, RepositoryId>(DateTime.UtcNow, id));
			LastModified = DateTime.Now;
			return t;
		}

		public void DeleteItem(T item)
		{
			if (!objectToIdHashtable.ContainsKey(item))
			{
				throw new ArgumentOutOfRangeException("item");
			}
			DeleteFromModifiedTimeList(item);
			idToObjectHashtable.Remove(objectToIdHashtable[item]);
			objectToIdHashtable.Remove(item);
			LastModified = DateTime.Now;
		}

		public void DeleteItem(RepositoryId id)
		{
			if (!idToObjectHashtable.ContainsKey(id))
			{
				throw new ArgumentOutOfRangeException("id");
			}
			T item = GetItem(id);
			DeleteItem(item);
		}

		private void DeleteFromModifiedTimeList(T item)
		{
			KeyValuePair<DateTime, RepositoryId> modifiedTimeToRemove =
				modifiedToIdList.Find(delegate(KeyValuePair<DateTime, RepositoryId> keyValuePair)
									  {
										  return keyValuePair.Value == GetId(item);
									  });
			modifiedToIdList.Remove(modifiedTimeToRemove);
		}

		public RepositoryId[] GetAllItems()
		{
			int numberOfIds = idToObjectHashtable.Keys.Count;
			RepositoryId[] ids = new RepositoryId[numberOfIds];
			idToObjectHashtable.Keys.CopyTo(ids, 0);
			return ids;
		}

		public RepositoryId[] GetItemsModifiedSince(DateTime dateTime)
		{
			int numberOfIds;
			List<RepositoryId> modifiedSinceList = new List<RepositoryId>();

			dateTime = dateTime.ToUniversalTime();
			foreach (KeyValuePair<DateTime, RepositoryId> keyValuePair in modifiedToIdList)
			{
				if(keyValuePair.Key >= dateTime)
				{
					modifiedSinceList.Add(keyValuePair.Value);
				}
			}
			numberOfIds = modifiedSinceList.Count;
			RepositoryId[] ids = new RepositoryId[numberOfIds];
			modifiedSinceList.CopyTo(ids, 0);

			return ids;
		}

		public void SaveItem(T item)
		{
			DateTime timeOfSave = DateTime.UtcNow;
			KeyValuePair<DateTime, RepositoryId> modifiedTimeAndIdOfItem =
				modifiedToIdList.Find(delegate(KeyValuePair<DateTime, RepositoryId> keyValuePair)
									  {
										  return keyValuePair.Value == GetId(item);
									  });
			if (modifiedTimeAndIdOfItem.Value == null)
			{
				throw new ArgumentOutOfRangeException("item");
			}
			modifiedToIdList.Remove(modifiedTimeAndIdOfItem);
			modifiedToIdList.Add(new KeyValuePair<DateTime, RepositoryId>(timeOfSave, GetId(item)));
			LastModified = timeOfSave;
		}

		public void SaveItems(IEnumerable<T> items)
		{
			throw new NotImplementedException();
		}

		public ResultSet<T> GetItemsMatching(Query query)
		{
			throw new NotImplementedException();
		}

		public int CountAllItems()
		{
			return idToObjectHashtable.Count;
		}

		public RepositoryId GetId(T item)
		{
			if (!objectToIdHashtable.ContainsKey(item))
			{
				throw new ArgumentOutOfRangeException("item");
			}
			return (RepositoryId)objectToIdHashtable[item];
		}

		public T GetItem(RepositoryId id)
		{
			if (!idToObjectHashtable.ContainsKey(id))
			{
				throw new ArgumentOutOfRangeException("id");
			}
			return (T)idToObjectHashtable[id];
		}

		#region IDisposable Members
#if DEBUG
		~MemoryRepository()
		{
			if (!this._disposed)
			{
				throw new ApplicationException("Disposed not explicitly called on MemoryRepository.");
			}
		}
#endif

		private bool _disposed = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic

				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("MemoryRepository");
			}
		}
		#endregion

		private class MemoryRepositoryId : RepositoryId
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
				if (memoryRepositoryId == null)
					return false;

				return id == memoryRepositoryId.id;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(this, obj))
					return true;
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