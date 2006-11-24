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

		private static string RecordListKey<T>(string filterName) where T : class, new()
		{
			return typeof(T).FullName + filterName;
		}

		public void Register<T>(IFilter<T> filter) where T : class, new()
		{
			if (!_filteredRecordLists.ContainsKey(RecordListKey<T>(filter.Key)))
			{
				_filteredRecordLists.Add(RecordListKey<T>(filter.Key), CreateFilteredRecordListUnlessSlow<T>(filter));
			}
		}

		public IRecordList<T> GetListOfType<T>() where T : class, new()
		{
			if (!_filteredRecordLists.ContainsKey(RecordListKey<T>(String.Empty)))
			{
				_filteredRecordLists.Add(RecordListKey<T>(String.Empty), CreateMasterRecordList<T>());
			}
			return (IRecordList<T>)_filteredRecordLists[RecordListKey<T>(String.Empty)];
		}

		public IRecordList<T> GetListOfTypeFilteredFurther<T>(IFilter<T> filter) where T : class, new()
		{
			if (!_filteredRecordLists.ContainsKey(RecordListKey<T>(filter.Key)))
			{
				throw new InvalidOperationException("Filter must be registered before it can be retrieved with GetListOfType.");
			}
			IRecordList<T> recordList = (IRecordList<T>)_filteredRecordLists[RecordListKey<T>(filter.Key)];
			if (recordList == null)
			{
				recordList = CreateFilteredRecordList<T>(filter);
			}
			_filteredRecordLists[RecordListKey<T>(filter.Key)] = recordList;
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

		/// <summary>
		/// Called whenever the record list knows some data was committed to the database
		/// </summary>
		public event EventHandler DataCommitted;

//        protected void OnDataCommitted(object sender, EventArgs e)
//        {
//            if (this.DataCommitted != null)
//            {
//                this.DataCommitted.Invoke(this, null);
//            }
//        }

		/// <summary>
		/// Call this, for example, when switching records in the gui. You don't need to know
		/// whether a commit is pending or not.
		/// </summary>
		public void GoodTimeToCommit()
		{
			if (CommitIfNeeded())
			{
				if (DataCommitted != null)
				{
					DataCommitted.Invoke(this, null);
				}
			}
		}

		abstract protected bool CommitIfNeeded();
	}
}
