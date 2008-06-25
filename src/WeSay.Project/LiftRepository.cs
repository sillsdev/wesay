using System;
using System.Collections.Generic;
using System.Text;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay.Project
{
	public class LiftRepository:IRepository<LexEntry>
	{
		private DateTime lastModified;

		public DateTime LastModified
		{
			get { return lastModified; }
		}

		public LexEntry CreateItem()
		{
			throw new NotImplementedException();
		}

		public int CountAllItems()
		{
			throw new NotImplementedException();
		}

		public RepositoryId GetId(LexEntry item)
		{
			throw new NotImplementedException();
		}

		public LexEntry GetItem(RepositoryId id)
		{
			throw new NotImplementedException();
		}

		public void DeleteItem(LexEntry item)
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

		public void SaveItem(LexEntry item)
		{
			throw new NotImplementedException();
		}

		public bool CanQuery()
		{
			throw new NotImplementedException();
		}

		public bool CanPersist()
		{
			throw new NotImplementedException();
		}

		public RepositoryId[] GetItemsMatchingQuery()
		{
			throw new NotImplementedException();
		}
	}
}
