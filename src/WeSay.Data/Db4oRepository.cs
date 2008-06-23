using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Inside.Query;
using Db4objects.Db4o.Query;

namespace WeSay.Data
{
	public class Db4oRepository<T> : IRepository<T> where T:class, new()
	{
		private readonly PrivateDb4oRecordListManager _recordListManager;

		//todo make this private and remove it.
		public Db4oDataSource Db4oDataSource
		{
			get
			{
				return _recordListManager.DataSource;
			}
		}

		public Db4oRepository(string path)
		{
			_recordListManager =
					new PrivateDb4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), path);
			Db4oLexModelHelper.Initialize(_recordListManager.DataSource.Data);
		}

		public DateTime LastModified
		{
			get { throw new NotImplementedException(); }
		}

		public T CreateItem()
		{
			T item = new T();
			IRecordList<T> type = _recordListManager.GetListOfType<T>();
			type.Add(item);
			return item;
		}

		public int CountAllItems()
		{
			return GetAllItems().Length;
		}

		public RepositoryId GetId(T item)
		{
			long id = _recordListManager.DataSource.Data.Ext().GetID(item);
			return new Db4oRepositoryId(id);
		}

		public T GetItem(RepositoryId id)
		{
			return _recordListManager.GetItem<T>(((Db4oRepositoryId)id).Db4oId);
		}

		public void DeleteItem(T item)
		{
			DeleteItem(GetId(item));
		}

		public void DeleteItem(RepositoryId id)
		{
			IRecordList<T> type = _recordListManager.GetListOfType<T>();
			type.Remove(GetItem(id));
		}

		public RepositoryId[] GetAllItems()
		{
			GenericObjectSetFacade<T> items =
					(GenericObjectSetFacade<T>)
					_recordListManager.DataSource.Data.Query<T>();
			long[] db4oIds = items._delegate.GetIDs();
			return WrapDb4oIdsInRepositoryIds(db4oIds);
		}

		private static RepositoryId[] WrapDb4oIdsInRepositoryIds(long[] db4oIds)
		{
			RepositoryId[] ids = new RepositoryId[db4oIds.Length];
			for (int i = 0; i != db4oIds.Length; ++i)
			{
				ids[i] = new Db4oRepositoryId(db4oIds[i]);
			}
			return ids;
		}

		//todo: refactor this using map of id to last modified time
		public RepositoryId[] GetItemsModifiedSince(DateTime last)
		{
			// by moving back 1 milliseconds, we ensure that we
			// will get the correct records with just a > and not >=
			last = last.AddMilliseconds(-1);
			IQuery q = _recordListManager.DataSource.Data.Query();
			q.Constrain(typeof(T));
			q.Descend("_modificationTime").Constrain(last).Greater();
			IObjectSet objectSet = q.Execute();
			return WrapDb4oIdsInRepositoryIds(objectSet.Ext().GetIDs());
		}

		public void SaveItems(IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				_recordListManager.DataSource.Data.Set(item);
			}
			_recordListManager.DataSource.Data.Commit();
		}

		public ResultSet<T> GetItemsMatching(Query query)
		{
			throw new NotImplementedException();
		}

		public void SaveItem(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			_recordListManager.DataSource.Data.Set(item);
			_recordListManager.DataSource.Data.Commit();
		}

		#region IDisposable Members
#if DEBUG
		~Db4oRepository()
		{
			if (!this._disposed)
			{
				throw new ApplicationException("Disposed not explicitly called on Db4oRepository.");
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
					_recordListManager.Dispose();
				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("Db4oRepository");
			}
		}
		#endregion
	}
}
