using System;
using System.Collections.Generic;
using System.Text;
using WeSay.Data;

namespace WeSay.Data
{
	public class ResultSetCache<T> where T : class, new()
	{
		private SortedDictionary<RecordToken<T>, object> _sortedTokens = null;
		private Dictionary<T, List<RecordToken<T>>> _itemToTokenMap = new Dictionary<T, List<RecordToken<T>>>();
		private Query _cachedQuery = null;
		private readonly IRepository<T> _repositoryQueried = null;

		public ResultSetCache(IRepository<T> repositoryQueried, ResultSet<T> resultSetToCache, Query query, SortDefinition[] sortDefinitions)
		{
			_repositoryQueried = repositoryQueried;
			_cachedQuery = query;

			RecordTokenComparer<T> comparerForSorting = new RecordTokenComparer<T>(sortDefinitions);
			_sortedTokens = new SortedDictionary<RecordToken<T>, object>(comparerForSorting);

			foreach (RecordToken<T> token in resultSetToCache)
			{
				if(!_itemToTokenMap.ContainsKey(token.RealObject))
				{
					_itemToTokenMap.Add(token.RealObject, new List<RecordToken<T>>());
					_itemToTokenMap[token.RealObject].Add(token);
				}
				else
				{
					_itemToTokenMap[token.RealObject].Add(token);
				}
				_sortedTokens.Add(token, null);
			}
		}

		public ResultSet<T> GetResultSet()
		{
			ResultSet<T> cachedResults = new ResultSet<T>(_repositoryQueried, _sortedTokens.Keys);
			return cachedResults;
		}

		public void UpdateItemInCache(T item)
		{
			if(item == null)
			{
				throw new ArgumentNullException("item");
			}

			RepositoryId itemId = _repositoryQueried.GetId(item);

			bool hasResults = false;
			List<RecordToken<T>> results = new List<RecordToken<T>>();
				foreach (Dictionary<string, object> result in _cachedQuery.GetResults(item))
				{
					hasResults = true;
					results.Add(new RecordToken<T>(_repositoryQueried, result, itemId));
				}
			//!!!I don't know how to test this case. Just copied it from every other place that produces Recordtokens TA 2008-08-13
				if (!hasResults)
				{
					results.Add(new RecordToken<T>(_repositoryQueried, itemId));
				}

			List<RecordToken<T>> oldRecordTokens = _itemToTokenMap[item];
			_itemToTokenMap.Remove(item);
			_itemToTokenMap.Add(item, results);
		}
	}
}
