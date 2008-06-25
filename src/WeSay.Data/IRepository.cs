using System;
using WeSay.Data;

namespace WeSay.Data
{
	public interface IRepository<T>
	{
		DateTime LastModified{ get;}

		T CreateItem();
		int CountAllItems();
		RepositoryId GetId(T item);
		T GetItem(RepositoryId id);
		void DeleteItem(T item);
		void DeleteItem(RepositoryId id);
		RepositoryId[] GetAllItems();
		RepositoryId[] ItemsModifiedSince(DateTime dateTime);
		void SaveItem(T item);
		bool CanQuery();
		bool CanPersist();
		RepositoryId[] GetItemsMatchingQuery();
		void Dispose();
	}
}