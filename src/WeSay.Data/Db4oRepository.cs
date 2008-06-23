using System;

namespace WeSay.Data
{
	public class Db4oRepository<T> : IRepository<T>
	{
		public Db4oRepository() {}

		public DateTime LastModified
		{
			get { throw new NotImplementedException(); }
		}

		public T CreateItem()
		{
			throw new NotImplementedException();
		}

		public int CountAllItems()
		{
			throw new NotImplementedException();
		}

		public RepositoryId GetId(T item)
		{
			throw new NotImplementedException();
		}

		public T GetItem(RepositoryId id)
		{
			throw new NotImplementedException();
		}

		public void DeleteItem(T item)
		{
			throw new NotImplementedException();
		}

		public void DeleteItem(RepositoryId id)
		{
			throw new NotImplementedException();
		}

		public RepositoryId[] GetAllItems()
		{
			throw new NotImplementedException();
		}

		public RepositoryId[] ItemsModifiedSince(DateTime dateTime)
		{
			throw new NotImplementedException();
		}

		public void SaveItem(T item)
		{
			throw new NotImplementedException();
		}
	}
}
