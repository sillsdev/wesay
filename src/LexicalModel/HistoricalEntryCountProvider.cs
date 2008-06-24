using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using WeSay.Data;

namespace WeSay.LexicalModel
{
	public interface IHistoricalEntryCountProvider
	{
		int GetNextNumber();
	}

	public class HistoricalEntryCountProviderForInMemory: IHistoricalEntryCountProvider
	{
		internal int _nextNumber;

		private HistoricalEntryCountProviderForInMemory()
		{
			_nextNumber = 0;
		}

		public int GetNextNumber()
		{
			return _nextNumber++;
		}
	}

	public class HistoricalEntryCountProviderForDb4o: IHistoricalEntryCountProvider
	{
		private int _nextNumber;

		[NonSerialized]
		internal Db4oDataSource _db4oData;

		public static IHistoricalEntryCountProvider GetOrMakeFromDatabase(Db4oDataSource db4oData)
		{
			HistoricalEntryCountProviderForDb4o counter = null;
			IQuery q = db4oData.Data.Query();
			q.Constrain(typeof (HistoricalEntryCountProviderForDb4o));
			IObjectSet matches = q.Execute();
			if (matches.Count > 0)
			{
				counter = (HistoricalEntryCountProviderForDb4o) matches[0];
				counter._db4oData = db4oData; // it won't have this in the db
			}
			if (counter == null)
			{
				counter = new HistoricalEntryCountProviderForDb4o(db4oData);
				db4oData.Data.Set(counter);
			}
			return counter;
		}

		private HistoricalEntryCountProviderForDb4o(Db4oDataSource db4oData)
		{
			_db4oData = db4oData;
			_nextNumber = 0;
		}

		public int GetNextNumber()
		{
			int v = _nextNumber;
			++_nextNumber;
			_db4oData.Data.Set(this);
			return v;
		}
	}
}