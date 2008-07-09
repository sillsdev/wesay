using System;
using System.Collections.Generic;
using System.IO;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay.Project
{
	public class LiftRepository:IRepository<LexEntry>
	{
		private DateTime lastModified;

		public LiftRepository(string filePath)
		{
			File.Create(filePath);
		}

		public DateTime LastModified
		{
			get { return lastModified; }
		}

		public LexEntry CreateItem()
		{
			return new LexEntry();
		}

		public int CountAllItems()
		{
			return 0;
		}

		public RepositoryId GetId(LexEntry item)
		{
			throw new ArgumentOutOfRangeException("item");
		}

		public LexEntry GetItem(RepositoryId id)
		{
			throw new ArgumentOutOfRangeException("id");
		}

		public void DeleteItem(LexEntry item)
		{
			throw new ArgumentOutOfRangeException("item");
		}

		public void DeleteItem(RepositoryId id)
		{
			throw new ArgumentOutOfRangeException("id");
		}

		public void DeleteAllItems()
		{
			throw new NotImplementedException();
		}

		public RepositoryId[] GetAllItems()
		{
			throw new NotImplementedException();
		}

		public RepositoryId[] GetItemsModifiedSince(DateTime dateTime)
		{
			throw new NotImplementedException();
		}

		public RepositoryId[] ItemsModifiedSince(DateTime dateTime)
		{
			throw new NotImplementedException();
		}

		public void SaveItem(LexEntry item)
		{
			throw new ArgumentOutOfRangeException("item");
		}

		public bool CanQuery
		{
			get { return false; }
		}

		public bool CanPersist
		{
			get { return true; }
		}

		public void SaveItems(IEnumerable<LexEntry> items)
		{
			throw new NotImplementedException();
		}

		public ResultSet<LexEntry> GetItemsMatching(Query query)
		{
			throw new NotImplementedException();
		}

		public RepositoryId[] GetItemsMatchingQuery()
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
