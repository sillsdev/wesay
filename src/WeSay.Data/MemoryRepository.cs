using System;
using System.Collections;
using System.Collections.Generic;

namespace WeSay.Data
{
	class MemoryRepository<T>:IRepository<T> where T : class, new()
	{
		private readonly Hashtable idToObjectHashtable = new Hashtable();
		private readonly Hashtable objectToIdHashtable = new Hashtable();
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

		public bool CanQuery { get { return true; } }

		public bool CanPersist { get { return false; } }

		public T CreateItem()
		{
			T t = new T();
			MemoryRepositoryId id = new MemoryRepositoryId();
			idToObjectHashtable.Add(id, t);
			objectToIdHashtable.Add(t, id);
			LastModified = PreciseDateTime.UtcNow;
			return t;
		}

		public void DeleteItem(T item)
		{
			if( item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (!objectToIdHashtable.ContainsKey(item))
			{
				throw new ArgumentOutOfRangeException("item");
			}
			idToObjectHashtable.Remove(objectToIdHashtable[item]);
			objectToIdHashtable.Remove(item);
			LastModified = PreciseDateTime.UtcNow;
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

		public void DeleteAllItems()
		{
			idToObjectHashtable.Clear();
			objectToIdHashtable.Clear();
			LastModified = PreciseDateTime.UtcNow;
		}

		public RepositoryId[] GetAllItems()
		{
			int numberOfIds = idToObjectHashtable.Keys.Count;
			RepositoryId[] ids = new RepositoryId[numberOfIds];
			idToObjectHashtable.Keys.CopyTo(ids, 0);
			return ids;
		}

		public void SaveItem(T item)
		{
			if(item == null)
			{
				throw new ArgumentNullException("item");
			}
			if(!objectToIdHashtable.ContainsKey(item))
			{
				throw new ArgumentOutOfRangeException("item", "The item must exist in the repository before it can be saved.");
			}
			DateTime timeOfSave = PreciseDateTime.UtcNow;
			LastModified = timeOfSave;
		}

		public void SaveItems(IEnumerable<T> items)
		{
			if(items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (T t in items)
			{
				SaveItem(t);
			}
		}

		public ResultSet<T> GetItemsMatching(Query query)
		{
			List<RecordToken<T>> results = new List<RecordToken<T>>();
			foreach (T item in objectToIdHashtable.Keys)
			{
				bool hasResults = false;
				foreach (Dictionary<string, object> result in query.GetResults(item))
				{
					hasResults = true;
					results.Add(new RecordToken<T>(this, result, GetId(item)));
				}
				if(!hasResults)
				{
					results.Add(new RecordToken<T>(this, GetId(item)));
				}
			}
			return new ResultSet<T>(this, results);
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
