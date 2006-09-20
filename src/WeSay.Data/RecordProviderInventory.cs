using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace WeSay.Data
{
	public interface IRecordProvider
	{
		IBindingList Records {get;}
	}

	public class RecordProviderInventory
	{
		IBindingList _sourceRecords;
		Hashtable _recordProviders;

		public RecordProviderInventory(IBindingList sourceRecords)
		{
			_sourceRecords = sourceRecords;
			_recordProviders = new Hashtable();
		}

		public IRecordProvider Get<T>() where T : class, new()
		{
			if (!_recordProviders.ContainsKey(String.Empty))
			{
				_recordProviders.Add(String.Empty, new FilteredListCache<T>(_sourceRecords));
			}
			return (IRecordProvider) _recordProviders[String.Empty];
		}

		public IRecordProvider Get<T>(IFilter<T> filter) where T : class, new()
		{
			if (!_recordProviders.ContainsKey(filter.Key))
			{
				_recordProviders.Add(filter.Key, new FilteredListCache<T>(_sourceRecords, filter));
			}
			return (IRecordProvider)_recordProviders[filter.Key];
		}

		class FilteredListCache<T> : WeSay.Data.IRecordProvider where T : class, new()
		{
			IBindingList _sourceRecords;
			IFilter<T> _filter;
			InMemoryRecordList<T> _filteredRecords;

			public FilteredListCache(IBindingList sourceRecords)
			{
				_sourceRecords = sourceRecords;
			}

			public FilteredListCache(IBindingList sourceRecords, IFilter<T> filter) : this(sourceRecords)
			{
				_filter = filter;
			}

			#region IRecordProvider Members

			public IBindingList Records
			{
				get
				{
					if (_filter == null)
					{
						return _sourceRecords;
					}
					if (_filteredRecords == null)
					{
						_filteredRecords = new InMemoryRecordList<T>();
						_filteredRecords.Add(_sourceRecords.GetEnumerator());
						_filteredRecords.ApplyFilter(_filter.Inquire);
					}
					return _filteredRecords;
				}
			}

			#endregion
		}
	}
}
