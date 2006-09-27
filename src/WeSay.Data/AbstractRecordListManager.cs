using System;
using System.Collections;

namespace WeSay.Data
{
	public abstract class AbstractRecordListManager : IRecordListManager
	{
		Hashtable _filteredRecordLists;
		protected AbstractRecordListManager()
		{
			_filteredRecordLists = new Hashtable();
		}

		abstract protected IRecordList<T> CreateMasterRecordList<T>() where T : class, new();
		abstract protected IRecordList<T> CreateFilteredRecordList<T>(IFilter<T> filter) where T : class, new();

		#region IRecordListManager Members

		protected virtual IRecordList<T> CreateFilteredRecordListUnlessSlow<T>(IFilter<T> filter) where T: class, new()
		{
			return null;
		}

		public void Register<T>(IFilter<T> filter) where T : class, new()
		{
			if (!_filteredRecordLists.ContainsKey(filter.Key))
			{
				_filteredRecordLists.Add(filter.Key, CreateFilteredRecordListUnlessSlow<T>(filter));
			}
		}

		public IRecordList<T> Get<T>() where T : class, new()
		{
			if (!_filteredRecordLists.ContainsKey(String.Empty))
			{
				_filteredRecordLists.Add(String.Empty, CreateMasterRecordList<T>());
			}
			return (IRecordList<T>)_filteredRecordLists[String.Empty];
		}

		public IRecordList<T> Get<T>(IFilter<T> filter) where T : class, new()
		{
			if (!_filteredRecordLists.ContainsKey(filter.Key))
			{
				throw new InvalidOperationException("Filter must be registered before it can be retrieved with Get.");
			}
			IRecordList<T> recordList = (IRecordList<T>)_filteredRecordLists[filter.Key];
			if (recordList == null)
			{
				recordList = CreateFilteredRecordList<T>(filter);
			}
			_filteredRecordLists[filter.Key] = recordList;
			return recordList;
		}

		#endregion

		#region IDisposable Members
		#if DEBUG
		~AbstractRecordListManager()
		{
			if (!this._disposed)
			{
				throw new InvalidOperationException("Disposed not explicitly called on " + GetType().FullName + ".");
			}
		}
		#endif

		private bool _disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool IsDisposed
		{
			get
			{
				return _disposed;
			}
			private
			set
			{
				_disposed = value;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic
					foreach (DictionaryEntry dictionaryEntry in _filteredRecordLists)
					{
						IDisposable disposable = dictionaryEntry.Value as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					_filteredRecordLists = null;
				}

				// shared (dispose and finalizable) cleanup logic
				IsDisposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}
		#endregion


	}
}
