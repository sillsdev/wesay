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
		abstract protected IRecordList<T> CreateFilteredRecordList<T>(IFilter<T> filter, ISortHelper<T> sortHelper) where T : class, new();
		public abstract T1 GetItem<T1>(long id);

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

		protected virtual IRecordList<T> CreateFilteredRecordListUnlessSlow<T>(IFilter<T> filter, ISortHelper<T> sortHelper) where T : class, new()
		{
			return null;
		}

		protected static string RecordListKey<T>(string filterName, string sorterName) where T : class, new()
		{
			string prefix = "!"; // for normal filtered, sorted lists
			if(string.IsNullOrEmpty(filterName))
			{
				if (string.IsNullOrEmpty(sorterName))
				{
					// this is our master record list
					prefix = "#";
				}
				else
				{
					// this is a master sorted list
					prefix = "@";
				}
			}
			else
			{
				if (string.IsNullOrEmpty(sorterName))
				{
					throw new ArgumentOutOfRangeException("sorterName","cannot be null or empty when filterName is present");
				}
			}

			return prefix + typeof(T).FullName + filterName + sorterName;
		}

		public static bool IsMasterRecordList(DictionaryEntry dictionaryEntry)
		{
			return ((string) dictionaryEntry.Key).StartsWith("#");
		}

		public static bool IsMasterSortedRecordList(DictionaryEntry dictionaryEntry)
		{
			return ((string)dictionaryEntry.Key).StartsWith("@");
		}

		public static bool IsFilteredRecordList(DictionaryEntry dictionaryEntry)
		{
			return ((string)dictionaryEntry.Key).StartsWith("!");
		}

		public void Register<T>(IFilter<T> filter, ISortHelper<T> sortHelper) where T : class, new()
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (sortHelper == null)
			{
				throw new ArgumentNullException("sortHelper");
			}

			string recordListKey = RecordListKey<T>(filter.Key, sortHelper.Name);
			if (!RecordLists.ContainsKey(recordListKey))
			{
				RecordLists.Add(recordListKey, CreateFilteredRecordListUnlessSlow(filter, sortHelper));
			}
		}

		public IRecordList<T> GetListOfType<T>() where T : class, new()
		{
			string recordListKey = RecordListKey<T>(null, null);
			if (!RecordLists.ContainsKey(recordListKey))
			{
				IRecordList<T> MasterRecordList = CreateMasterRecordList<T>();
				MasterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(MasterRecordList_DeletingRecord<T>);
				RecordLists.Add(recordListKey, MasterRecordList);
			}
			return (IRecordList<T>)RecordLists[recordListKey];
		}

		void MasterRecordList_DeletingRecord<T>(object sender, RecordListEventArgs<T> e) where T : class, new()
		{
			DataDeleted.Invoke(this, new DeletedItemEventArgs(e.Item));
		}

		public IRecordList<T> GetListOfTypeFilteredFurther<T>(IFilter<T> filter, ISortHelper<T> sortHelper) where T : class, new()
		{
			if(filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (sortHelper == null)
			{
				throw new ArgumentNullException("sortHelper");
			}
			string recordListKey = RecordListKey<T>(filter.Key, sortHelper.Name);
			if (!RecordLists.ContainsKey(recordListKey))
			{
				throw new InvalidOperationException("Filter must be registered before it can be retrieved with GetListOfType.");
			}
			IRecordList<T> recordList = (IRecordList<T>)RecordLists[recordListKey];
			if (recordList == null)
			{
				recordList = CreateFilteredRecordList(filter, sortHelper);
				RecordLists[recordListKey] = recordList;
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
						if (IsFilteredRecordList(dictionaryEntry))
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
						if (IsMasterSortedRecordList(dictionaryEntry))
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
