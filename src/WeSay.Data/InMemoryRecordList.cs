using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace WeSay.Data
{
	public class InMemoryRecordList<T> : AbstractRecordList<T> where T : class, new()
	{
		bool _isSorted;
		bool _isFiltered;

		public InMemoryRecordList():base()
		{
			Records = new List<T>();
		}

		public InMemoryRecordList(IRecordList<T> original)
			: this()
		{
			this.AddRange(original);

			_isSorted = original.IsSorted;
			SortProperty = original.SortProperty;
			ListSortDirection = original.SortDirection;

			_isFiltered = original.IsFiltered;
		}

		public override bool Commit()
		{
			return true;
		}

		protected override void DoSort(Comparison<T> sort)
		{
			((List < T >) Records).Sort(sort);
			_isSorted = true;
		}

		public override bool IsSorted
		{
			get
			{
				VerifyNotDisposed();
				return _isSorted;
			}
		}

		protected override void DoRemoveSort()
		{
			_isSorted = false;
		}

		protected override void DoFilter(Predicate<T> itemsToInclude)
		{
			Predicate<T> itemsToExclude = ComparisonHelper<T>.GetInversePredicate(itemsToInclude);
			((List<T>)Records).RemoveAll(itemsToExclude);
			_isFiltered = true;
		}

		public override bool IsFiltered
		{
			get
			{
				VerifyNotDisposed();
				return _isFiltered;
			}
		}

		protected override void DoRemoveFilter()
		{
			throw new NotImplementedException();
			//_records.Filter = null;
		}

		protected override bool ShouldClearRecords()
		{
			if (base.ShouldClearRecords())
			{
				foreach (T item in Records)
				{
					RegisterItemPropertyChangedHandler(item, false);
				}
				return true;
			}
			return false;
		}

		protected override bool ShouldReplaceRecord(int index, T value)
		{
			if (base.ShouldReplaceRecord(index, value))
			{
				RegisterItemPropertyChangedHandler(this[index], false);
				RegisterItemPropertyChangedHandler(value, true);
				return true;
			}
			return false;
		}

		protected override bool ShouldAddRecord(T item)
		{
			if (base.ShouldAddRecord(item))
			{
				RegisterItemPropertyChangedHandler(item, true);
				return true;
			}
			return false;
		}

		protected override bool ShouldDeleteRecord(T item)
		{
			if (base.ShouldDeleteRecord(item))
			{
				RegisterItemPropertyChangedHandler(item, false);
				return true;
			}
			return false;
		}

		public void RegisterItemPropertyChangedHandler(T item, bool register)
		{
			VerifyNotDisposed();
			INotifyPropertyChanged localItem = item as INotifyPropertyChanged;
			if (localItem == null)
			{
				return;
			}
			localItem.PropertyChanged -= Item_PropertyChanged;
			if (register)
			{
				localItem.PropertyChanged += Item_PropertyChanged;
			}
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnItemChanged(Records.IndexOf((T)sender), e.PropertyName);
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.IsDisposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic
					foreach (T item in Records)
					{
						RegisterItemPropertyChangedHandler(item, false);
					}
				}
				base.Dispose(disposing);
			}
		}
	}
}
