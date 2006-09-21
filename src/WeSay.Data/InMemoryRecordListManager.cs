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
		Hashtable _recordProviders;

		public InMemoryRecordListManager(IRecordList<T> sourceRecords)
		{
			_sourceRecords = sourceRecords;
			_recordProviders = new Hashtable();
		}
		#region IRecordListManager Members

		public IRecordList<T> Get()
		{
			if (!_recordProviders.ContainsKey(String.Empty))
			{
				_recordProviders.Add(String.Empty, new InMemoryRecordProvider(_sourceRecords));
			}
			return (IRecordList<T>) _recordProviders[String.Empty];
		}

		public IRecordList<T> Get(IFilter<T> filter)
		{
			if (!_recordProviders.ContainsKey(filter.Key))
			{
				_recordProviders.Add(filter.Key, new InMemoryRecordProvider(_sourceRecords, filter.Inquire));
			}
			return (IRecordList<T>)_recordProviders[filter.Key];
		}
		#endregion
		class InMemoryRecordProvider : InMemoryRecordList<T>
		{
			public InMemoryRecordProvider(IRecordList<T> sourceRecords)
				: base(sourceRecords)
			{
			}

			public InMemoryRecordProvider(IRecordList<T> sourceRecords, Predicate<T> filter)
				: this(sourceRecords)
			{
				if(!this.IsFiltered) {
					this.ApplyFilter(filter);
				}
			}
		}
	}
}
