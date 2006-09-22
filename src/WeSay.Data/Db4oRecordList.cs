using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	public class Db4oRecordList<T> : AbstractRecordList<T> where T : class, new()
	{
		private static int defaultWriteCacheSize = 0;

		private void Initialize(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort, Db4o.Binding.SODAQueryProvider sodaQuery)
		{
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}
			_pdc = TypeDescriptor.GetProperties(typeof(T));

			Db4o.Binding.Db4oList<T> records = new Db4o.Binding.Db4oList<T>((com.db4o.ObjectContainer)dataSource.Data, new List<T>(), filter, sort);
			_records = records;
			records.SortingInDatabase = false;
			records.FilteringInDatabase = false;
			records.ReadCacheSize = 0; // I think this could go back to lower
			records.WriteCacheSize = defaultWriteCacheSize;
			records.PeekPersistedActivationDepth = 99;
			records.ActivationDepth = 99;
			records.RefreshActivationDepth = 99;
			records.SetActivationDepth = 99;
			//            records.RequeryAndRefresh(false);
			records.Storing += new EventHandler<Db4o.Binding.Db4oListEventArgs<T>>(OnRecordStoring);
			try
			{
				if (sodaQuery != null)
				{
					records.SODAQuery = sodaQuery;
				}
				else
				{
					records.Requery(records.FilteringInDatabase && filter != null);
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw (e);
			}
		}

		public Db4oRecordList(Db4oDataSource dataSource)
		{
			Initialize(dataSource, null, null, null);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Db4o.Binding.SODAQueryProvider sodaQuery)
		{
			Initialize(dataSource, null, null, sodaQuery);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Predicate<T> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			Initialize(dataSource, filter, null, null);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (sort == null)
			{
				throw new ArgumentNullException("sort");
			}
			Initialize(dataSource, filter, sort, null);
		}

		public Db4oRecordList(Db4oDataSource dataSource, Comparison<T> sort)
		{
			if (sort == null)
			{
				throw new ArgumentNullException("sort");
			}
			Initialize(dataSource, null, sort, null);
		}

		void OnRecordStoring(object sender, Db4o.Binding.Db4oListEventArgs<T> e)
		{
			if (e.PropertyName == string.Empty)
			{
				OnItemChanged(_records.IndexOf(e.Item));
			}
			else
			{
				OnItemChanged(_records.IndexOf(e.Item), e.PropertyName);
			}
		}

		public override bool Commit()
		{
			VerifyNotDisposed();
			return ((Db4o.Binding.Db4oList<T>)_records).Commit();
		}

		public Db4o.Binding.SODAQueryProvider SODAQuery
		{
			get
			{
				VerifyNotDisposed();
				return ((Db4o.Binding.Db4oList<T>)_records).SODAQuery;
			}
			set
			{
				VerifyNotDisposed();
				((Db4o.Binding.Db4oList<T>)_records).SODAQuery = value;
			}
		}

		public void Add(IEnumerator<T> enumerator)
		{
			VerifyNotDisposed();
			Db4o.Binding.Db4oList<T> records = (Db4o.Binding.Db4oList<T>)_records;
			records.WriteCacheSize = 0;
			while (enumerator.MoveNext())
			{
				Add(enumerator.Current);
			}
			records.Commit();
			records.WriteCacheSize = defaultWriteCacheSize;
		}


		public int WriteCacheSize
		{
			get
			{
				VerifyNotDisposed();
				return ((Db4o.Binding.Db4oList<T>)_records).WriteCacheSize;
			}
			set
			{
				VerifyNotDisposed();
				((Db4o.Binding.Db4oList<T>)_records).WriteCacheSize = value;
			}
		}

		protected override void DoFilter(Predicate<T> itemsToInclude)
		{
			Db4o.Binding.Db4oList<T> records = (Db4o.Binding.Db4oList<T>) _records;
			if (records.FilteringInDatabase)
			{
				records.Commit();
			}
			else
			{
				if (records.IsFiltered)
				{
					RemoveFilter();
				}
			}
			records.Filter = itemsToInclude;
		}
		protected override void DoRemoveFilter()
		{
			Db4o.Binding.Db4oList<T> records = (Db4o.Binding.Db4oList<T>)_records;

			records.Filter = null;
			if (!records.FilteringInDatabase)
			{
				records.Requery(false);
			}
		}

		public override bool IsFiltered
		{
			get
			{
				VerifyNotDisposed();
				Db4o.Binding.Db4oList<T> records = (Db4o.Binding.Db4oList<T>)_records;

				return records.IsFiltered || records.SODAQuery != null;
			}
		}

		protected override void  DoSort(Comparison<T> sort)
		{
			((Db4o.Binding.Db4oList<T>)_records).Sort(sort);
		}

		public override bool IsSorted
		{
			get
			{
				VerifyNotDisposed();
				return ((Db4o.Binding.Db4oList<T>)_records).IsSorted;
			}
		}

		protected override void DoRemoveSort()
		{
			((Db4o.Binding.Db4oList<T>)_records).RemoveSort();
		}

		protected override void  Dispose(bool disposing)
		{
			if (! this.IsDisposed){
				if (disposing)
				{
					((Db4o.Binding.Db4oList<T>)_records).Dispose();
				}
			}
			base.Dispose(disposing);
		}
	}
}
