using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public interface IRepository<T>
	{
		T GetItem(RepositoryId id);
		RepositoryId GetId(T item);
	}
}
