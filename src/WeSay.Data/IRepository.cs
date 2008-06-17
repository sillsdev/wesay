using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
	public interface IRepository<T>
	{
		T CreateItem();
		int CountAllItems();
		RepositoryId GetId(T item);
		T GetItem(RepositoryId id);
		void DeleteItem(T item);
		void DeleteItem(RepositoryId id);
	}
}
