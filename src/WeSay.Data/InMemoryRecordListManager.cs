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
			if (sourceRecords == null)
			{
				this._disposed = true;
				throw new ArgumentNullException();
			}
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
			return (IRecordList<T>)_recordLists[String.Empty];
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
					_sourceRecords = null;
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
			bool _isSourceMasterRecord;
			Predicate<T> IsRelevant;

			public FilteredInMemoryRecordList(IRecordList<T> sourceRecords)
				: base(sourceRecords)
			{
				_masterRecordList = sourceRecords;
				_masterRecordList.ListChanged += new ListChangedEventHandler(OnMasterRecordListListChanged);
				_masterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(OnMasterRecordListDeletingRecord);
			}


			public FilteredInMemoryRecordList(IRecordList<T> sourceRecords, Predicate<T> filter)
				: this(sourceRecords)
			{
				IsRelevant = filter;
				this.ApplyFilter(filter);
			}

			void OnMasterRecordListDeletingRecord(object sender, RecordListEventArgs<T> e)
			{
				_isSourceMasterRecord = true;
				HandleItemDeletedFromMaster(e.Item);
				_isSourceMasterRecord = false;
			}

			void OnMasterRecordListListChanged(object sender, ListChangedEventArgs e)
			{
				IRecordList<T> masterRecordList = (IRecordList<T>)sender;
				VerifyNotDisposed();
				_isSourceMasterRecord = true;
				switch (e.ListChangedType)
				{
					case ListChangedType.ItemAdded:
						HandleItemAddedToMaster(masterRecordList[e.NewIndex]);
						break;
					case ListChangedType.ItemChanged:
						HandleItemChangedInMaster(masterRecordList[e.NewIndex]);
						break;
					case ListChangedType.ItemDeleted:
						break;
					case ListChangedType.Reset:
						HandleItemsClearedFromMaster(masterRecordList);
						break;
				}
				_isSourceMasterRecord = false;
			}

			protected override bool ShouldAddRecord(T item)
			{
				bool shouldAdd = base.ShouldAddRecord(item);
				if (shouldAdd)
				{
					shouldAdd = !Contains(item);
				}
				return shouldAdd;
			}
			protected override bool ShouldDeleteRecord(T item)
			{
				if (!_isSourceMasterRecord && _masterRecordList != null)
				{
					_masterRecordList.Remove(item);
				}
				return base.ShouldDeleteRecord(item);
			}

			private void HandleItemAddedToMaster(T item)
			{
				AddIfRelevantElseRemove(item);
			}

			private void HandleItemChangedInMaster(T item)
			{
				AddIfRelevantElseRemove(item);
			}

			private void AddIfRelevantElseRemove(T item)
			{
				if (this.IsRelevant(item))
				{
					if (!Contains(item))
					{
						this.Add(item);
					}
				}
				else if (this.Contains(item))
				{
					this.Remove(item);
				}
			}

			private void HandleItemDeletedFromMaster(T item)
			{
				if (this.Contains(item))
				{
					this.Remove(item);
				}
			}

			private void HandleItemsClearedFromMaster(IRecordList<T> masterRecordList)
			{
				// The reset event can be raised when items are cleared and also when sorting or filtering occurs
				if (masterRecordList.Count == 0)
				{
					this.Clear();
				}
			}

			protected override void OnItemAdded(int newIndex)
			{
				base.OnItemAdded(newIndex);
				if (!_isSourceMasterRecord && _masterRecordList != null)
				{
					_masterRecordList.Add(this[newIndex]);
				}
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
