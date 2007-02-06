using System;
using System.Collections.Generic;

namespace WeSay.Data
{
	public class Db4oRecordList<T> : AbstractRecordList<T> where T : class, new()
	{

		private static int defaultWriteCacheSize = 1; // 0 means never commit until database closes, 1 means commit after each write

		private void Initialize(Db4oDataSource dataSource, Predicate<T> filter, Comparison<T> sort, SodaQueryProvider sodaQuery)
		{
			if (dataSource == null)
			{
				Dispose();
				throw new ArgumentNullException("dataSource");
			}
			Db4oList<T> records = new Db4oList<T>(dataSource.Data, new List<T>(), filter, sort);
			Records = records;
			InitializeDb4oListBehavior(records);
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

		private void InitializeDb4oListBehavior(Db4oList<T> records) {
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
		}

		public Db4oRecordList(Db4oDataSource dataSource):base()
		{
			Initialize(dataSource, null, null, null);
		}

		public Db4oRecordList(Db4oRecordList<T> source ):base()
		{
			ListSortDirection = source.ListSortDirection;
			SortProperty = source.SortProperty;

			Db4oList<T> sourceRecords = (Db4oList<T>) source.Records;
			Db4oList<T> records = new Db4oList<T>(sourceRecords.Database, sourceRecords.ItemIds, sourceRecords.Filter, sourceRecords.Sorter);
			InitializeDb4oListBehavior(records);
			Records = records;
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
			int index = Records.IndexOf(e.Item);
			if (index != -1)
			{
				OnItemChanged(index);
			}
		}

//        public override bool Commit()
//        {
//            VerifyNotDisposed();
//            if (((Db4oList<T>)Records).Commit())
//            {
//                if (DataCommitted != null)
//                {
//                    DataCommitted.Invoke(this,null);
//                }
//                return true;
//            }
//            return false;
//        }

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

		protected override void DoFilter(Predicate<T> filter)
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

			//here's the big long step
			records.Filter = filter;
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

		public override int GetIndexFromId(long id)
		{
			int index = ((Db4oList<T>) Records).ItemIds.IndexOf(id);
			if(index == -1)
			{
				throw new ArgumentOutOfRangeException("id not valid");
			}
			return index;
		}

		public long GetId(T item)
		{
			return ((Db4oList<T>)Records).ItemIds[IndexOf(item)];
		}

		public DateTime GetDatabaseLastModified()
		{
			IList<DatabaseModified> modifiedList = ((Db4oList<T>)Records).Database.Query<DatabaseModified>();
			if (modifiedList.Count == 1)
			{
				return modifiedList[0].LastModified;
			}
			return DateTime.UtcNow;
		}

		protected override void DoRemoveSort()
		{
			((Db4oList<T>)Records).RemoveSort();
		}

		protected override void  Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					if (Records != null)
					{
						((Db4oList<T>)Records).Dispose();
					}
				}
			}
			base.Dispose(disposing);
		}
	}
}
