using System;
using System.Collections.Generic;
using System.Reflection;
using Palaso.Progress;

namespace WeSay.Data
{
	public class SynchronicRepository<T>: IRepository<T> where T : class, new()
    {
		private readonly IRepository<T> _primary;
		private readonly IRepository<T> _secondary;

        public delegate void CopyStrategy(T destination, T source);

        private readonly CopyStrategy _copyStrategy;

        private readonly Dictionary<RepositoryId, RepositoryId> _primarySecondaryMap;

		public SynchronicRepository(IRepository<T> primary,
									IRepository<T> secondary,
									CopyStrategy copyStrategy)
        {
            if (primary == null)
            {
				Dispose();
                throw new ArgumentNullException("primary");
            }
            if (secondary == null)
            {
				Dispose();
                throw new ArgumentNullException("secondary");
            }
            if (copyStrategy == null)
            {
				Dispose();
                throw new ArgumentNullException("copyStrategy");
            }
            if (ReferenceEquals(primary, secondary))
            {
				Dispose();
                throw new ArgumentException("primary and secondary must not be equal");
            }
            _primary = primary;
            _secondary = secondary;
            _copyStrategy = copyStrategy;
            _primarySecondaryMap = new Dictionary<RepositoryId, RepositoryId>();
            SynchronizeRepositories();
        }

        public SynchronicRepository(IRepository<T> primary, IRepository<T> secondary)
				: this(primary, secondary, DefaultCopyStrategy) {}

        private static void DefaultCopyStrategy(T destination, T source)
        {
			Type type = typeof (T);
			FieldInfo[] fields =
					type.GetFields(BindingFlags.Instance | BindingFlags.Public |
								   BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
            }
        }

        private void SynchronizeRepositories()
        {
            // if one persists, but not the other, then the persistable one wins
            // otherwise, go with the repository most recently changed
            IRepository<T> master = _secondary;
            IRepository<T> slave = _primary;
            if ((_primary.CanPersist && !_secondary.CanPersist) ||
				(_primary.LastModified > _secondary.LastModified &&
				 _primary.CanPersist == _secondary.CanPersist))
            {
                master = _primary;
                slave = _secondary;
            }

			RepositoryId[] ids = master.GetAllItems();

			// avoid doing anything which would change LastModifiedTime if both are empty
			if (ids.Length == 0 && slave.CountAllItems() == 0)
            {
				return;
            }

			slave.DeleteAllItems();

            foreach (RepositoryId id in ids)
            {
                T slaveItem = slave.CreateItem();
                _copyStrategy(slaveItem, master.GetItem(id));
            }
        }

        #region IRepository<T> Members

        public DateTime LastModified
        {
			get
			{
				return
						new DateTime(
								Math.Max(_primary.LastModified.Ticks, _secondary.LastModified.Ticks),
								DateTimeKind.Utc);
			}
        }

        public bool CanQuery
        {
            get { return _primary.CanQuery; }
        }

        public bool CanPersist
        {
            get { return _primary.CanPersist || _secondary.CanPersist; }
        }

		public void Startup(ProgressState state) {}

		public T CreateItem()
        {
            T item = _primary.CreateItem();
            T item2 = _secondary.CreateItem();
            _primarySecondaryMap.Add(_primary.GetId(item), _secondary.GetId(item2));
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
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            DeleteItem(_primary.GetId(item));
        }

        public void DeleteItem(RepositoryId id)
        {
            if (!_primarySecondaryMap.ContainsKey(id))
            {
                throw new ArgumentOutOfRangeException("id", "Item does not exist in repository");
            }
            _secondary.DeleteItem(_primarySecondaryMap[id]);
            _primary.DeleteItem(id);
            _primarySecondaryMap.Remove(id);
        }

		public void DeleteAllItems()
		{
			_primary.DeleteAllItems();
			_secondary.DeleteAllItems();
		}

        public RepositoryId[] GetAllItems()
        {
            return _primary.GetAllItems();
        }

        public void SaveItem(T item)
        {
            T secondaryItem = CopyItemToSecondary(item);
            _primary.SaveItem(item);
            _secondary.SaveItem(secondaryItem);
        }

        private T CopyItemToSecondary(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            T item2 = _secondary.GetItem(_primarySecondaryMap[_primary.GetId(item)]);
            _copyStrategy(item2, item);
            return item2;
        }

        public void SaveItems(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            List<T> secondaryItems = new List<T>();
            foreach (T item in items)
            {
                secondaryItems.Add(CopyItemToSecondary(item));
            }
            _primary.SaveItems(items);
            _secondary.SaveItems(secondaryItems);
        }

        public ResultSet<T> GetItemsMatching(Query query)
        {
            return _primary.GetItemsMatching(query);
        }

        #endregion

        #region IDisposable Members

#if DEBUG
		~SynchronicRepository()
		{
			if (!this._disposed)
			{
				throw new ApplicationException(
						"Disposed not explicitly called on SynchronicRepository.");
			}
		}
#endif

		private bool _disposed;

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
					if (this._primary != null)
					{
						this._primary.Dispose();
					}
					if (this._secondary != null)
					{
						this._secondary.Dispose();
					}
				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("SynchronicRepository");
			}
        }

        #endregion
    }
}