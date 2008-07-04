using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public abstract class SynchronizableRepository<T> : IRepository<T> where T: class, new()
	{
		internal abstract void AddItem(T item);

		#region IRepository<T> Members

		public abstract DateTime LastModified {get;}

		public abstract bool CanQuery {get;}

		public abstract bool CanPersist {get;}

		public abstract T CreateItem();

		public abstract int CountAllItems();

		public abstract RepositoryId GetId(T item);

		public abstract T GetItem(RepositoryId id);

		public abstract void DeleteItem(T item);

		public abstract void DeleteItem(RepositoryId id);

		public abstract RepositoryId[] GetAllItems();

		public abstract void SaveItem(T item);

		public abstract void SaveItems(IEnumerable<T> items);

		public abstract ResultSet<T> GetItemsMatching(Query query);

		#endregion

		#region IDisposable Members

		public void Dispose() { }

		#endregion
	}

	public class SynchronicRepository<T> : IRepository<T> where T: class, new()
	{
		SynchronizableRepository<T> _primary;
		SynchronizableRepository<T> _secondary;

		public SynchronicRepository(SynchronizableRepository<T> primary, SynchronizableRepository<T> secondary)
		{
			if (primary == null)
			{
				throw new ArgumentNullException("primary");
			}
			if (secondary == null)
			{
				throw new ArgumentNullException("secondary");
			}
			if (primary == secondary)
			{
				throw new ArgumentException("primary and secondary must not be equal");
			}
			_primary = primary;
			_secondary = secondary;
		}

		#region IRepository<T> Members

		public DateTime LastModified
		{
			get { return _primary.LastModified; }
		}

		public bool CanQuery
		{
			get { return _primary.CanQuery; }
		}

		public bool CanPersist
		{
			get { return _primary.CanPersist || _secondary.CanPersist; }
		}

		public T CreateItem()
		{
			T item = _primary.CreateItem();
			_secondary.AddItem(item);
			return item;
		}

		public int CountAllItems()
		{
			return _primary.CountAllItems();
		}

		public RepositoryId GetId(T item)
		{
			return _primary.GetId(item);
		}

		public T GetItem(RepositoryId id)
		{
			return _primary.GetItem(id);
		}

		public void DeleteItem(T item)
		{
			_primary.DeleteItem(item);
		}

		public void DeleteItem(RepositoryId id)
		{
			_primary.DeleteItem(id);
		}

		public RepositoryId[] GetAllItems()
		{
			return _primary.GetAllItems();
		}

		public void SaveItem(T item)
		{
			_primary.SaveItem(item);
		}

		public void SaveItems(IEnumerable<T> items)
		{
			_primary.SaveItems(items);
		}

		public ResultSet<T> GetItemsMatching(Query query)
		{
			return _primary.GetItemsMatching(query);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_primary.Dispose();
			_secondary.Dispose();
		}

		#endregion
	}
}
