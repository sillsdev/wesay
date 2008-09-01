using System;
using System.Collections.Generic;
using System.Text;
using WeSay.Data;

namespace WeSay.Data
{
	public class ResultSetCache<T> where T : class, new()
	{
		private SortedDictionary<RecordToken<T>, object> _sortedTokens = null;
		private List<IQuery<T>> _cachedQueries = new List<IQuery<T>>();
		private readonly IRepository<T> _repositoryQueried = null;

		public ResultSetCache(IRepository<T> repositoryQueried, SortDefinition[] sortDefinitions)
		{
			if (repositoryQueried == null)
			{
				throw new ArgumentNullException("repositoryQueried");
			}
			_repositoryQueried = repositoryQueried;

			if (sortDefinitions == null)
			{
				_sortedTokens = new SortedDictionary<RecordToken<T>, object>(); //sort by RepositoryId
			}
			else
			{
				RecordTokenComparer<T> comparerForSorting = new RecordTokenComparer<T>(sortDefinitions);
				_sortedTokens = new SortedDictionary<RecordToken<T>, object>(comparerForSorting);
			}
		}

		public ResultSetCache(IRepository<T> repositoryQueried,  SortDefinition[] sortDefinitions, ResultSet<T> resultSetToCache, IQuery<T> queryToCache)
			: this(repositoryQueried, sortDefinitions)
		{
			Add(resultSetToCache, queryToCache);
		}

		private void SortNewResultSetIntoCachedResultSet(ResultSet<T> resultSetToCache)
		{
			foreach (RecordToken<T> token in resultSetToCache)
			{
					_sortedTokens.Add(token, null);
			}
		}

		public void Add(ResultSet<T> resultSetToCache, IQuery<T> queryToCache)
		{
			if(resultSetToCache == null)
			{
				throw new ArgumentNullException("resultSetToCache");
			}
			if (queryToCache == null)
			{
				throw new ArgumentNullException("queryToCache");
			}
			_cachedQueries.Add(queryToCache);
			SortNewResultSetIntoCachedResultSet(resultSetToCache);
		}

		public ResultSet<T> GetResultSet()
		{
			ResultSet<T> cachedResults = new ResultSet<T>(_repositoryQueried, _sortedTokens.Keys);
			return cachedResults;
		}

		/// <summary>
		/// Call this method every time a cached item changes. This method verifies that the item you are trying to update exists int he repository.
		/// </summary>
		/// <param name="item"></param>
		public void UpdateItemInCache(T item)
		{
			if(item == null)
			{
				throw new ArgumentNullException("item");
			}

			RepositoryId itemId = _repositoryQueried.GetId(item);

			RemoveOldTokensWithId(itemId);

			ResultSet<T> itemsQueryResults = QueryNewItem(item);

			foreach (RecordToken<T> token in itemsQueryResults)
			{
				if(!_sortedTokens.ContainsKey(token))
				{
					_sortedTokens.Add(token, null);
				}
			}
		}

		private ResultSet<T> QueryNewItem(T item)
		{
			bool hasResults = false;
			List<RecordToken<T>> results = new List<RecordToken<T>>();
			foreach (IQuery<T> query in _cachedQueries)
			{
				foreach (Dictionary<string, object> result in query.GetResults(item))
				{
					results.Add(new RecordToken<T>(_repositoryQueried, result, _repositoryQueried.GetId(item)));
					hasResults = true;
				}
			}

			//!!!I don't know how to test this case. Just copied it from every other place that produces Recordtokens TA 2008-08-13
			if (!hasResults)
			{
			   results.Add(new RecordToken<T>(_repositoryQueried, _repositoryQueried.GetId(item)));
			}
			return new ResultSet<T>(_repositoryQueried, results);
		}

		private void RemoveOldTokensWithId(RepositoryId itemId)
		{
			List<KeyValuePair<RecordToken<T>, object>> oldTokensToDelete = new List<KeyValuePair<RecordToken<T>, object>>();
			foreach (KeyValuePair<RecordToken<T>, object> token in _sortedTokens)
			{
				if (token.Key.Id == itemId)
				{
					oldTokensToDelete.Add(token);
				}
			}
			foreach (KeyValuePair<RecordToken<T>, object> pair in oldTokensToDelete)
			{
				_sortedTokens.Remove(pair.Key);
			}
		}

		public void DeleteItemFromCache(T item)
		{
			RemoveOldTokensWithId(_repositoryQueried.GetId(item));
		}

		public void DeleteItemFromCache(RepositoryId id)
		{
			CheckIfItemIsInRepository(id);
			RemoveOldTokensWithId(id);
		}

		public void DeleteAllItemsFromCache()
		{
			_sortedTokens.Clear();
		}

		private void CheckIfItemIsInRepository(RepositoryId id)
		{
			_repositoryQueried.GetItem(id);
		}

	}
}
