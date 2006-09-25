using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WeSay.Data
{
	public class Db4oRecordList<T> : AbstractRecordList<T> where T : class, new()
	{
		private static int defaultWriteCacheSize = 0;

		private void Initialize(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort, SodaQueryProvider sodaQuery)
		{
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}
			Db4oList<T> records = new Db4oList<T>((com.db4o.ObjectContainer)dataSource.Data, new List<T>(), filter, sort);
			Records = records;
			records.SortingInDatabase = false;
			records.FilteringInDatabase = false;
			records.ReadCacheSize = 0; // I think this could go back to lower
			records.WriteCacheSize = defaultWriteCacheSize;
			records.PeekPersistedActivationDepth = 99;
			records.ActivationDepth = 99;
			records.RefreshActivationDepth = 99;
			records.SetActivationDepth = 99;
			//            records.RequeryAndRefresh(false);
			records.Storing += new EventHandler<Db4oListEventArgs<T>>(OnRecordStoring);
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
			catch
			{
				Dispose();
				throw;
			}
		}

		public Db4oRecordList(Db4oDataSource dataSource):base()
		{
			Initialize(dataSource, null, null, null);
		}

		public Db4oRecordList(Db4oRecordList<T> source ):base()
		{
			this.ListSortDirection = source.ListSortDirection;
			this.SortProperty = source.SortProperty;

			Db4oList<T> sourceRecords = (Db4oList<T>)source.Records;
			Records = new Db4oList<T>(sourceRecords.Database, sourceRecords.ItemIds, sourceRecords.Filter, sourceRecords.Sorter);
		}

		[CLSCompliant(false)]
		public Db4oRecordList(Db4oDataSource dataSource, SodaQueryProvider sodaQuery)
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

		void OnRecordStoring(object sender, Db4oListEventArgs<T> e)
		{
			if (e.PropertyName.Length == 0)
			{
				OnItemChanged(Records.IndexOf(e.Item));
			}
			else
			{
				OnItemChanged(Records.IndexOf(e.Item), e.PropertyName);
			}
		}

		public override bool Commit()
		{
			VerifyNotDisposed();
			return ((Db4oList<T>)Records).Commit();
		}

		[CLSCompliant(false)]
		public SodaQueryProvider SodaQuery
		{
			get {
				VerifyNotDisposed();
				return ((Db4oList<T>)Records).SODAQuery;
			}
			set {
				VerifyNotDisposed();
				((Db4oList<T>)Records).SODAQuery = value;
			}
		}

		public void Add(IEnumerator<T> enumerator)
		{
			VerifyNotDisposed();
			Db4oList<T> records = (Db4oList<T>)Records;
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
			get {
				VerifyNotDisposed();
				return ((Db4oList<T>)Records).WriteCacheSize;
			}
			set {
				VerifyNotDisposed();
				((Db4oList<T>)Records).WriteCacheSize = value;
			}
		}

		protected override void DoFilter(Predicate<T> itemsToInclude)
		{
			Db4oList<T> records = (Db4oList<T>) Records;
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
			Db4oList<T> records = (Db4oList<T>)Records;

			records.Filter = null;
			if (!records.FilteringInDatabase)
			{
				records.Requery(false);
			}
		}

		public override bool IsFiltered
		{
			get {
				VerifyNotDisposed();
				Db4oList<T> records = (Db4oList<T>)Records;

				return records.IsFiltered || records.SODAQuery != null;
			}
		}

		protected override void  DoSort(Comparison<T> sort)
		{
			((Db4oList<T>)Records).Sort(sort);
		}

		public override bool IsSorted
		{
			get {
				VerifyNotDisposed();
				return ((Db4oList<T>)Records).IsSorted;
			}
		}

		protected override void DoRemoveSort()
		{
			((Db4oList<T>)Records).RemoveSort();
		}

		protected override void  Dispose(bool disposing)
		{
			if (! this.IsDisposed)
			{
				if (disposing)
				{
					((Db4oList<T>)Records).Dispose();
				}
			}
			base.Dispose(disposing);
		}
	}
}
