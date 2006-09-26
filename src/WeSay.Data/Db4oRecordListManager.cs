using System;
using System.ComponentModel;

namespace WeSay.Data
{
	public class Db4oRecordListManager : AbstractRecordListManager
	{
		private Db4oDataSource _dataSource;

		public Db4oRecordListManager(string filePath)
		: base()
		{
			_dataSource = new Db4oDataSource(filePath);
		}

		protected override IRecordList<T> CreateMasterRecordList<T>()
		{
			return new Db4oRecordList<T>(_dataSource);
		}

		protected override IRecordList<T> CreateFilteredRecordList<T>(Predicate<T> filter)
		{
			return new FilteredDb4oRecordList<T>(Get<T>(), filter);
		}
		protected override void Dispose(bool disposing)
		{
			bool canBeDisposed = !IsDisposed;
			base.Dispose(disposing);
			if (canBeDisposed && disposing)
			{
				_dataSource.Dispose();
			}
		}

		class FilteredDb4oRecordList<T> : Db4oRecordList<T> where T : class, new()
		{
			IRecordList<T> _masterRecordList;
			bool _isSourceMasterRecord;
			Predicate<T> IsRelevant;

			public FilteredDb4oRecordList(IRecordList<T> sourceRecords, Predicate<T> filter)
			: base((Db4oRecordList<T>)sourceRecords)
			{
				_masterRecordList = sourceRecords;
				_masterRecordList.ListChanged += new ListChangedEventHandler(OnMasterRecordListListChanged);
				_masterRecordList.DeletingRecord += new EventHandler<RecordListEventArgs<T>>(OnMasterRecordListDeletingRecord);
				IsRelevant = filter;
				ApplyFilter(filter);
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
					if(IsRelevant(item))
					{
						shouldAdd = !Contains(item);
					}
					else
					{
						shouldAdd = false;
						if (!_isSourceMasterRecord && _masterRecordList != null)
						{
							_masterRecordList.Add(item);
						}
					}

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
				if (IsRelevant(item))
				{
					if (!Contains(item))
					{
						Add(item);
					}
				}
				else if (Contains(item))
				{
					Remove(item);
				}
			}

			private void HandleItemDeletedFromMaster(T item)
			{
				if (Contains(item))
				{
					Remove(item);
				}
			}

			private void HandleItemsClearedFromMaster(IRecordList<T> masterRecordList)
			{
				// The reset event can be raised when items are cleared and also when sorting or filtering occurs
				if (masterRecordList.Count == 0)
				{
					Clear();
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
				if (!IsDisposed)
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
