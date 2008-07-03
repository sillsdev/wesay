using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public class SynchronicRepository<T> : IRepository<T> where T: class, new()
	{
		IRepository<T> _primary;
		IRepository<T> _secondary;

		SynchronicRepository(IRepository<T> primary, IRepository<T> secondary)
		{
			if (primary == null)
			{
				throw new ArgumentNullException("primary");
			}
			if (secondary == null)
			{
				throw new ArgumentNullException("secondary");
			}
			_primary = primary;
			_secondary = secondary;
		}

		#region IRepository<T> Members

		public DateTime LastModified
		{
			get { throw new Exception("The method or operation is not implemented."); }
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
			return _primary.CreateItem();
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
