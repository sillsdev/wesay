using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace WeSay.Data
{
	public class InMemoryRecordListManager<T> : IRecordListManager<T> where T : class, new()
	{
		IRecordList<T> _sourceRecords;
		Dictionary<string, IRecordList<T>> _recordLists;

		public InMemoryRecordListManager(IRecordList<T> sourceRecords)
		{
			_sourceRecords = sourceRecords;
			_recordLists = new Dictionary<string, IRecordList<T>>();
		}
		#region IRecordListManager Members

		public IRecordList<T> Get()
		{
			if (!_recordLists.ContainsKey(String.Empty))
			{
				_recordLists.Add(String.Empty, new FilteredInMemoryRecordList(_sourceRecords));
			}
			return (IRecordList<T>) _recordLists[String.Empty];
		}

		public IRecordList<T> Get(IFilter<T> filter)
		{
			if (!_recordLists.ContainsKey(filter.Key))
			{
				_recordLists.Add(filter.Key, new FilteredInMemoryRecordList(_sourceRecords, filter.Inquire));
			}
			return (IRecordList<T>)_recordLists[filter.Key];
		}
		#endregion

		#region IDisposable Members
#if DEBUG
		~InMemoryRecordListManager()
		{
			if (!this._disposed)
			{
				throw new ApplicationException("Disposed not explicitly called on InMemoryRecordListManager.");
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
					foreach (KeyValuePair<string, IRecordList<T>> keyValuePair in _recordLists)
					{
						keyValuePair.Value.Dispose();
					}
					_recordLists = null;
				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("InMemoryRecordListManager");
			}
		}
		#endregion

		class FilteredInMemoryRecordList : InMemoryRecordList<T>
		{
			IRecordList<T> _masterRecordList;

			public FilteredInMemoryRecordList(IRecordList<T> sourceRecords)
				: base(sourceRecords)
			{
				_masterRecordList = sourceRecords;
				_masterRecordList.ListChanged += new ListChangedEventHandler(OnMasterRecordListListChanged);
			}

			public FilteredInMemoryRecordList(IRecordList<T> sourceRecords, Predicate<T> filter)
				: this(sourceRecords)
			{
				if(!this.IsFiltered) {
					this.ApplyFilter(filter);
				}
			}

			void OnMasterRecordListListChanged(object sender, ListChangedEventArgs e)
			{
				VerifyNotDisposed();
			}

			protected override void OnItemAdded(int newIndex)
			{
				base.OnItemAdded(newIndex);
				_masterRecordList.Add(this[newIndex]);
			}

			protected override void OnItemDeleted(int oldIndex)
			{
				base.OnItemDeleted(oldIndex);
				_masterRecordList.Remove(this[oldIndex]);
			}

			#region IDisposable Members

			protected override void Dispose(bool disposing)
			{
				if (!this.IsDisposed)
				{
					if (disposing)
					{
						// dispose-only, i.e. non-finalizable logic
						_masterRecordList.ListChanged -= OnMasterRecordListListChanged;
						_masterRecordList = null;
					}
				}
				base.Dispose(disposing);
			}
			#endregion
		}
	}
}
