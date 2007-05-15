	using System;
using System.Collections;

namespace WeSay.Data
{
	public class DeletedItemEventArgs:EventArgs
	{
		private readonly object _itemDeleted;
		public DeletedItemEventArgs(object itemDeleted)
		{
			_itemDeleted = itemDeleted;
		}
		public object ItemDeleted
		{
			get { return this._itemDeleted; }
		}
	}
	public abstract class AbstractRecordListManager : IRecordListManager
	{
		private bool _delayWritingCachesUntilDispose=false;
		private Hashtable _filteredRecordLists;
		protected AbstractRecordListManager()
		{
			_filteredRecordLists = new Hashtable();
		}

		abstract protected IRecordList<T> CreateMasterRecordList<T>() where T : class, new();
		abstract protected IRecordList<T> CreateFilteredRecordList<Key, T>(IFilter<T> filter, ISortHelper<Key, T> sortHelper) where T : class, new();

		#region IRecordListManager Members

		/// <summary>
		/// Used when importing, where we want to go fast and don't care to have a good cache if we crash
		/// </summary>
		public bool DelayWritingCachesUntilDispose
		{
			get
			{
				return _delayWritingCachesUntilDispose;
			}
			set
			{
				_delayWritingCachesUntilDispose = value;
				foreach (IControlCachingBehavior list in _filteredRecordLists)
				{
					list.DelayWritingCachesUntilDispose = value;
				}
			}
		}

		protected virtual IRecordList<T> CreateFilteredRecordListUnlessSlow<Key, T>(IFilter<T> filter, ISortHelper<Key, T> sortHelper) where T : class, new()
		{
			return null;
		}

		private static string RecordListKey<T>(string filterName) where T : class, new()
		{
			return ((filterName == string.Empty)?'#':'!') + typeof(T).FullName + filterName;
		}

		public static bool IsMasterRecordList(DictionaryEntry dictionaryEntry)
		{
			return ((string) dictionaryEntry.Key).StartsWith("#");
		}

		public void Register<Key, T>(IFilter<T> filter, ISortHelper<Key, T> sortHelper) where T : class, new()
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (sortHelper == null)
			{
				throw new ArgumentNullException("sortHelper");
			}

			if (!RecordLists.ContainsKey(RecordListKey<T>(filter.Key)))
			{
				RecordLists.Add(RecordListKey<T>(filter.Key), CreateFilteredRecordListUnlessSlow(filter, sortHelper));
			}
		}

		public IRecordList<T> GetListOfType<T>() where T : class, new()
		{
			if (!RecordLists.ContainsKey(RecordListKey<T>(String.Empty)))
			{
				IRecordList<T> MasterRecordList = CreateMasterRecordList<T>();
				MasterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(MasterRecordList_DeletingRecord<T>);
				RecordLists.Add(RecordListKey<T>(String.Empty), MasterRecordList);
			}
			return (IRecordList<T>)RecordLists[RecordListKey<T>(String.Empty)];
		}

		void MasterRecordList_DeletingRecord<T>(object sender, RecordListEventArgs<T> e) where T : class, new()
		{
			DataDeleted.Invoke(this, new DeletedItemEventArgs(e.Item));
		}

		public IRecordList<T> GetListOfTypeFilteredFurther<Key, T>(IFilter<T> filter, ISortHelper<Key, T> sortHelper) where T : class, new()
		{
			if(filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (sortHelper == null)
			{
				throw new ArgumentNullException("sortHelper");
			}
			if (!RecordLists.ContainsKey(RecordListKey<T>(filter.Key)))
			{
				throw new InvalidOperationException("Filter must be registered before it can be retrieved with GetListOfType.");
			}
			IRecordList<T> recordList = (IRecordList<T>)RecordLists[RecordListKey<T>(filter.Key)];
			if (recordList == null)
			{
				recordList = CreateFilteredRecordList(filter, sortHelper);
				RecordLists[RecordListKey<T>(filter.Key)] = recordList;
			}
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

		protected Hashtable RecordLists
		{
			get { return this._filteredRecordLists; }
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic
					// we need to dispose masters last
					foreach (DictionaryEntry dictionaryEntry in RecordLists)
					{
						if (!IsMasterRecordList(dictionaryEntry))
						{
							IDisposable disposable = dictionaryEntry.Value as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
					foreach (DictionaryEntry dictionaryEntry in RecordLists)
					{
						if (IsMasterRecordList(dictionaryEntry))
						{
							IDisposable disposable = dictionaryEntry.Value as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
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
		public event EventHandler DataCommitted = delegate {};
		public event EventHandler<DeletedItemEventArgs> DataDeleted = delegate
		{
		};

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
				DataCommitted.Invoke(this, null);
			}
		}

		abstract protected bool CommitIfNeeded();
	}
}
