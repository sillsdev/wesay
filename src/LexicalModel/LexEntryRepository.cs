using System;
using System.Collections.Generic;
using System.Diagnostics;
using Palaso.Progress;
using Palaso.Text;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;

namespace WeSay.LexicalModel
{
	public class LexEntryRepository: IRepository<LexEntry>
	{
		Dictionary<string, ResultSetCache<LexEntry>> _sortedResultSetCaches = new Dictionary<string, ResultSetCache<LexEntry>>();

		private readonly IRepository<LexEntry> _decoratedRepository;
		public LexEntryRepository(string path):this(path, new ProgressState())
		{}

		public LexEntryRepository(string path, ProgressState progressState)
		{
			//todo: eventually use synchronicRepository with Db4o and Lift
			_decoratedRepository = new LiftRepository(path, progressState);
		}

		public LexEntryRepository(IRepository<LexEntry> decoratedRepository)
		{
			if (decoratedRepository == null)
			{
				throw new ArgumentNullException("decoratedRepository");
			}

			_decoratedRepository = decoratedRepository;
		}

		public DateTime LastModified
		{
			get { return _decoratedRepository.LastModified; }
		}

		public LexEntry CreateItem()
		{
			LexEntry item = _decoratedRepository.CreateItem();
			UpdateCaches(item);
			return item;
		}

		private void UpdateCaches(LexEntry item)
		{
			foreach (KeyValuePair<string, ResultSetCache<LexEntry>> pair in _sortedResultSetCaches)
			{
				pair.Value.UpdateItemInCache(item);
			}
		}

		private void UpdateCaches(object sender, EventArgs e)
		{
			UpdateCaches((LexEntry) sender);
		}

		public RepositoryId[] GetAllItems()
		{
			return _decoratedRepository.GetAllItems();
		}

		public int CountAllItems()
		{
			return _decoratedRepository.CountAllItems();
		}

		public RepositoryId GetId(LexEntry item)
		{
			return _decoratedRepository.GetId(item);
		}

		public LexEntry GetItem(RepositoryId id)
		{
			LexEntry item = _decoratedRepository.GetItem(id);
			return item;
		}

		public void SaveItems(IEnumerable<LexEntry> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			List<LexEntry> dirtyItems = new List<LexEntry>();
			foreach (LexEntry item in items)
			{
				if (item.IsDirty)
				{
					dirtyItems.Add(item);
					UpdateCaches(item);
				}
			}
			_decoratedRepository.SaveItems(dirtyItems);
			foreach (LexEntry item in dirtyItems)
			{
				item.Clean();
			}
		}

		public ResultSet<LexEntry> GetItemsMatching(Query query)
		{
			return _decoratedRepository.GetItemsMatching(query);
		}

		private ResultSet<LexEntry> GetItemsMatchingCore(Query query)
		{
			return _decoratedRepository.GetItemsMatching(query);
		}

		public void SaveItem(LexEntry item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (item.IsDirty)
			{
				_decoratedRepository.SaveItem(item);
				UpdateCaches(item);
				item.Clean();
			}
		}

		public bool CanQuery
		{
			get { return _decoratedRepository.CanQuery; }
		}

		public bool CanPersist
		{
			get { return _decoratedRepository.CanPersist; }
		}

		public void DeleteItem(LexEntry item)
		{
			DeleteFromCaches(item);
			_decoratedRepository.DeleteItem(item);
		}

		private void DeleteFromCaches(LexEntry item)
		{
			foreach (KeyValuePair<string, ResultSetCache<LexEntry>> pair in _sortedResultSetCaches)
			{
				pair.Value.DeleteItemFromCache(item);
			}
		}

		public void DeleteItem(RepositoryId repositoryId)
		{
			LexEntry item = _decoratedRepository.GetItem(repositoryId);
			DeleteFromCaches(item);
			_decoratedRepository.DeleteItem(repositoryId);
		}

		public void DeleteAllItems()
		{
			_decoratedRepository.DeleteAllItems();
			foreach (KeyValuePair<string, ResultSetCache<LexEntry>> pair in _sortedResultSetCaches)
			{
				pair.Value.DeleteAllItemsFromCache();
			}
		}

		public void NotifyThatLexEntryHasBeenUpdated(LexEntry updatedLexEntry)
		{
			if(updatedLexEntry == null)
			{
				throw new ArgumentNullException("updatedLexEntry");
			}
			//This call checks that the Entry is in the repository
			GetId(updatedLexEntry);
			UpdateCaches(updatedLexEntry);
		}

		public int GetHomographNumber(LexEntry entry, WritingSystem headwordWritingSystem)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if (headwordWritingSystem == null)
			{
				throw new ArgumentNullException("headwordWritingSystem");
			}
			ResultSet<LexEntry> resultSet = GetAllEntriesSortedByHeadword(headwordWritingSystem);
			RecordToken<LexEntry> first = resultSet.FindFirst(entry);
			if (first == null)
			{
				throw new ArgumentOutOfRangeException("entry", entry, "Entry not in repository");
			}
			if ((bool) first["HasHomograph"])
			{
				return (int) first["HomographNumber"];
			}
			return 0;
		}


		/// <summary>
		/// Gets a ResultSet containing all entries sorted by citation if one exists and otherwise
		/// by lexical form.
		/// Use "Form" to access the headword in a RecordToken.
		/// </summary>
		/// <param name="writingSystem"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetAllEntriesSortedByHeadword(WritingSystem writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}

			if (!_sortedResultSetCaches.ContainsKey("sortedByHeadWord"))
			{
				Query query = GetAllLexEntriesQuery();
				query.In("VirtualHeadWord").ForEach("Forms").Show("Form").Show("WritingSystemId");
				query.Show("OrderForRoundTripping");
				query.Show("OrderInFile");
				query.Show("CreationTime");

				ResultSet<LexEntry> itemsMatching = GetItemsMatchingCore(query);
				SortDefinition[] sortOrder = new SortDefinition[4];
				sortOrder[0] = new SortDefinition("Form", writingSystem);
				sortOrder[1] = new SortDefinition("OrderForRoundTripping", Comparer<int>.Default);
				sortOrder[2] = new SortDefinition("OrderInFile", Comparer<int>.Default);
				sortOrder[3] = new SortDefinition("CreationTime", Comparer<DateTime>.Default);

				_sortedResultSetCaches.Add("sortedByHeadWord", new ResultSetCache<LexEntry>(this, itemsMatching, query, sortOrder));
			}
			ResultSet<LexEntry> resultsFromCache = _sortedResultSetCaches["sortedByHeadWord"].GetResultSet();
			resultsFromCache = FilterEntriesToOnlyThoseWithWritingSystemId(resultsFromCache, "Form", "WritingSystemId", writingSystem.Id);

			string previousHeadWord = null;
			int homographNumber = 1;
			RecordToken<LexEntry> previousToken = null;
			foreach (RecordToken<LexEntry> token in resultsFromCache)
			{
				string currentHeadWord = (string) token["Form"];
				Debug.Assert(currentHeadWord != null);
				if (currentHeadWord == previousHeadWord)
				{
					homographNumber++;
				}
				else
				{
					previousHeadWord = currentHeadWord;
					homographNumber = 1;
				}
				// only used to get our sort correct --This comment seems nonsensical --TA 2008-08-14!!!
				token["HomographNumber"] = homographNumber;
				switch (homographNumber)
				{
					case 1:
						token["HasHomograph"] = false;
						break;
					case 2:
						Debug.Assert(previousToken != null);
						previousToken["HasHomograph"] = true;
						token["HasHomograph"] = true;
						break;
					default:
						token["HasHomograph"] = true;
						break;
				}
				previousToken = token;
			}

			return resultsFromCache;
		}

		/// <summary>
		/// Gets a ResultSet containing all entries sorted by lexical form
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <param name="writingSystem"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetAllEntriesSortedByLexicalForm(WritingSystem writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			if (!(_sortedResultSetCaches.ContainsKey("sortedByLexicalForm")))
			{
//                Query query =
//                    GetAllLexEntriesQuery().In("LexicalForm").ForEach("Forms").Show("Form").Show(
//                        "WritingSystemId");
				Query query = GetAllLexEntriesQuery();
//                query.In("LexicalForm").ForEach("Forms").Show("Form").Show("WritingSystemId");
				query.In("LexicalForm").ForEach("Forms").AtLeastOne().
					Where("WritingSystemId", delegate(object id)
					{
						return (string)id == writingSystem.Id;
					}
					).Show("Form").Show("WritingSystemId");

				ResultSet<LexEntry> itemsMatching = GetItemsMatchingCore(query);
				SortDefinition[] sortOrder = new SortDefinition[1];
				sortOrder[0] = new SortDefinition("Form", writingSystem);

				_sortedResultSetCaches.Add("sortedByLexicalForm",
										   new ResultSetCache<LexEntry>(this, itemsMatching, query, sortOrder));
			}
			ResultSet<LexEntry> resultsFromCache = _sortedResultSetCaches["sortedByLexicalForm"].GetResultSet();
			resultsFromCache = FilterEntriesToOnlyThoseWithWritingSystemId(resultsFromCache, "Form", "WritingSystemId", writingSystem.Id);
			return resultsFromCache;
		}

		/// <summary>
		/// Gets a ResultSet containing all entries sorted by definition and gloss. It will return both the definition
		/// and the gloss if both exist and are different.
		/// Use "Form" to access the Definition/Gloss in RecordToken.
		/// </summary>
		/// <param name="writingSystem"></param>
		/// <returns>Definition and gloss in "Form" field of RecordToken</returns>
		public ResultSet<LexEntry> GetAllEntriesSortedByDefinition(WritingSystem writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			Query defQuery = GetAllLexEntriesQuery().ForEach("Senses").In("Definition").ForEach("Forms").Show("Form").Show("WritingSystemId");
			Query glossQuery = GetAllLexEntriesQuery().ForEach("Senses").In("Gloss").ForEach("Forms").Show("Form").Show("WritingSystemId");
			//Remove any results that don't match the desired writingsystem (keeping at least one empty that does if nothing matches)
			ResultSet<LexEntry> defResult = GetItemsMatchingQueryFilteredByWritingSystem(defQuery, "Form", "WritingSystemId", writingSystem);
			ResultSet<LexEntry> glossResult = GetItemsMatchingQueryFilteredByWritingSystem(glossQuery, "Form", "WritingSystemId", writingSystem);

			ResultSet<LexEntry> result = new ResultSet<LexEntry>(this, defResult, glossResult);
			result.Coalesce("Form", delegate(object o)
										{
											return string.IsNullOrEmpty((string) o);
										});
			result.Sort(new SortDefinition("Form", writingSystem));
			return result;

		}

		private ResultSet<LexEntry> GetItemsMatchingQueryFilteredByWritingSystemAndSortedByForm(
				Query query,
				string formField,
				string writingSystemIdField,
				WritingSystem writingSystem)
		{
			ResultSet<LexEntry> result = GetItemsMatchingQueryFilteredByWritingSystem(query, formField, writingSystemIdField, writingSystem);
			result.Sort(new SortDefinition(formField, writingSystem));
			return result;
		}

		private ResultSet<LexEntry> GetItemsMatchingQueryFilteredByWritingSystem(Query query, string formField, string writingSystemIdField, WritingSystem writingSystem)
		{
			ResultSet<LexEntry> allEntriesMatchingQuery = GetItemsMatchingCore(query);
			return FilterEntriesToOnlyThoseWithWritingSystemId(
				allEntriesMatchingQuery,
				formField,
				writingSystemIdField,
				writingSystem.Id);
		}

		/// <summary>
		/// Gets a ResultSet containing entries that contain a semantic domain assigned to them
		/// sorted by semantic domain.
		/// Use "SemanticDomain" to access the semantic domain in a RecordToken.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithSemanticDomainSortedBySemanticDomain(
			string fieldName)
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			Query allSensePropertiesQuery =
					GetAllLexEntriesQuery().ForEach("Senses").ForEach("Properties").Show("Key").Show
							("Value", "SemanticDomains");

			ResultSet<LexEntry> allSenseProperties = GetItemsMatchingCore(allSensePropertiesQuery);
			allSenseProperties.RemoveAll(
					delegate(RecordToken<LexEntry> token) { return (string) token["Key"] != fieldName; });

			List<RecordToken<LexEntry>> result = new List<RecordToken<LexEntry>>();
			foreach (RecordToken<LexEntry> token in allSenseProperties)
			{
				OptionRefCollection semanticDomains = (OptionRefCollection) token["SemanticDomains"];
				foreach (string semanticDomain in semanticDomains.Keys)
				{
					RecordToken<LexEntry> newToken = new RecordToken<LexEntry>(this, token.Id);
					newToken["SemanticDomain"] = semanticDomain;
					result.Add(newToken);
				}
			}
			ResultSet<LexEntry> resultSet = new ResultSet<LexEntry>(this, result);
			resultSet.Sort(new SortDefinition("SemanticDomain", StringComparer.InvariantCulture),
							new SortDefinition("RepositoryId", Comparer<RepositoryId>.Default));

			return resultSet;
		}

	   /// <summary>
	   /// Gets a ResultSet containing entries whose gloss match glossForm sorted by the lexical form
	   /// in the given writingsystem.
	   /// Use "Form" to access the lexical form and "Gloss/Form" to access the Gloss in a RecordToken.
	   /// </summary>
	   /// <param name="glossForm"></param>
	   /// <param name="lexicalUnitWritingSystem"></param>
	   /// <returns></returns>
	   public ResultSet<LexEntry> GetEntriesWithMatchingGlossSortedByLexicalForm(
				LanguageForm glossForm, WritingSystem lexicalUnitWritingSystem)
		{
			if (glossForm == null)
			{
				throw new ArgumentNullException("glossForm");
			}
			if (lexicalUnitWritingSystem == null)
			{
				throw new ArgumentNullException("lexicalUnitWritingSystem");
			}
			Query query = GetAllLexEntriesQuery();
			query.In("LexicalForm").ForEach("Forms").Show("Form").Show("WritingSystemId");
			query.ForEach("Senses").In("Gloss").ForEach("Forms").Show("Form", "Gloss/Form").Show(
					"WritingSystemId", "Gloss/WritingSystemId");

			ResultSet<LexEntry> resultSet = GetItemsMatchingQueryFilteredByWritingSystemAndSortedByForm(
													query,
													"Form",
													"WritingSystemId",
													lexicalUnitWritingSystem);
			resultSet.RemoveAll(delegate(RecordToken<LexEntry> token)
								{
									return (string)token["Gloss/WritingSystemId"] != glossForm.WritingSystemId
										|| (string)token["Gloss/Form"] != glossForm.Form;
								});
			return resultSet;
		}

		/// <summary>
		/// Gets the LexEntry whose Id matches id.
		/// </summary>
		/// <returns></returns>
		public LexEntry GetLexEntryWithMatchingId(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id == string.Empty)
			{
				throw new ArgumentOutOfRangeException("id", "The Id should not be empty.");
			}
			Query idOfEntries = GetAllLexEntriesQuery().Show("Id");
			ResultSet<LexEntry> items = GetItemsMatchingCore(idOfEntries);
			int firstOccuranceOfId = items.FindFirstIndex(delegate(RecordToken<LexEntry> token)
												{
													return (string)token["Id"] == id;
												});
			int secondOccuranceOfId = items.FindFirstIndex(firstOccuranceOfId+1, delegate(RecordToken<LexEntry> token)
												{
													return (string)token["Id"] == id;
												});
			if (firstOccuranceOfId < 0)
			{
				return null;
			}
			if (0 <= secondOccuranceOfId)
			{
				throw new ApplicationException("More than one Entry exists with the Id" + id);
			}
			LexEntry lexEntryWithId = items[firstOccuranceOfId].RealObject;
			return lexEntryWithId;
		}

		/// <summary>
		/// Gets the LexEntry whose Guid matches guid.
		/// </summary>
		/// <returns></returns>
		public LexEntry GetLexEntryWithMatchingGuid(Guid guid)
		{
			if(guid == Guid.Empty)
			{
				throw new ArgumentOutOfRangeException("guid", "Guids should not be empty!");
			}
			Query query = GetAllLexEntriesQuery().Show("Guid");
			ResultSet<LexEntry> items = GetItemsMatchingCore(query);
			int index =
					items.FindFirstIndex(
							delegate(RecordToken<LexEntry> token) { return (Guid) token["Guid"] == guid; });
			if (index < 0)
			{
				return null;
			}
			if (index + 1 < items.Count)
			{
				int nextIndex = items.FindFirstIndex(index + 1,
													 delegate(RecordToken<LexEntry> token) { return (Guid) token["Guid"] == guid; });

				if (nextIndex >= 0)
				{
					throw new ApplicationException("More than one entry exists with the guid " +
												   guid);
				}
			}
			RecordToken<LexEntry> first = items[index];
			return first.RealObject;
		}

		/// <summary>
		/// Gets a ResultSet containing entries whose lexical form is similar to lexicalForm
		/// sorted by the lexical form in the given writingsystem.
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithSimilarLexicalForm(string lexicalForm,
																	WritingSystem writingSystem,
																	ApproximateMatcherOptions
																			matcherOptions)
		{
			if (lexicalForm == null)
			{
				throw new ArgumentNullException("lexicalForm");
			}
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			return new ResultSet<LexEntry>(this,
										   ApproximateMatcher.FindClosestForms
												   <RecordToken<LexEntry>>(
												   GetAllEntriesSortedByLexicalForm(writingSystem),
												   GetFormForMatchingStrategy,
												   lexicalForm,
												   matcherOptions));
		}

		private static string GetFormForMatchingStrategy(object item)
		{
			return (string) ((RecordToken<LexEntry>) item)["Form"];
		}

		/// <summary>
		/// Gets a ResultSet containing entries whose lexical form match lexicalForm
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <param name="lexicalForm"></param>
		/// <param name="writingSystem"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithMatchingLexicalForm(string lexicalForm,
																	 WritingSystem writingSystem)
		{
			if (lexicalForm == null)
			{
				throw new ArgumentNullException("lexicalForm");
			}
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			ResultSet<LexEntry> resultSet = GetAllEntriesSortedByLexicalForm(writingSystem);
			resultSet.RemoveAll(
					delegate(RecordToken<LexEntry> token) { return (string) token["Form"] != lexicalForm; });
			return resultSet;
		}

		private static Query GetAllLexEntriesQuery()
		{
			return new Query(typeof (LexEntry));
		}

		private ResultSet<LexEntry> FilterEntriesToOnlyThoseWithWritingSystemId(
				ResultSet<LexEntry> allResults,
				string formField,
				string writingSystemIdField,
				string filterOnWritingSystemId)
		{
			if (allResults.Count == 0)
			{
				return allResults;
			}

			// Initial     -->  Use WS     --> Remove dups --> Coalesce
			// 1 foo   en       1 foo   en     1 foo   en      1 foo   en
			// 1 bar   de       1 ""    en     1 ""    en
			// 1 blah  fr       1 ""    en
			// 2 hello en       2 hello en     2 hello en      2 hello en
			// 3 world de       3 ""    en     3 ""    en      3 ""    en

			// Make each result have the writing system that we want
			// this is in case there are other fields that need to be kept around
			// even though there isn't a field with the given writing system's form
			// dups will be removed
			ResultSet<LexEntry> filteredResults = allResults;
			foreach (RecordToken<LexEntry> token in filteredResults)
			{
				object id;
				if (!token.TryGetValue(writingSystemIdField, out id)
					|| (string) id != filterOnWritingSystemId)
				{
					token[formField] = string.Empty;
					token[writingSystemIdField] = filterOnWritingSystemId;
				}
			}
			//this will remove duplicates
			filteredResults = new ResultSet<LexEntry>(this, filteredResults);

			filteredResults.Coalesce("Form", delegate(object o)
											  {
												  return string.IsNullOrEmpty((string)o);
											  });

			return filteredResults;

		}

		/// <summary>
		/// Gets a ResultSet containing entries that are missing the field matching field
		/// sorted by the lexical form in the given writing system.
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="lexicalUnitWritingSystem"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithMissingFieldSortedByLexicalUnit(Field field,
																				 WritingSystem
																						 lexicalUnitWritingSystem)
		{
			if(lexicalUnitWritingSystem == null)
			{
				throw new ArgumentNullException("lexicalUnitWritingSystem");
			}
			Predicate<LexEntry> filteringPredicate = new MissingFieldQuery(field).FilteringPredicate;
			Query query = new Query.PredicateQuery<LexEntry>(filteringPredicate);
			query.In("LexicalForm").ForEach("Forms").Show("Form").Show("WritingSystemId");


			ResultSet<LexEntry> itemsMatching = GetItemsMatchingQueryFilteredByWritingSystemAndSortedByForm(query, "Form", "WritingSystemId", lexicalUnitWritingSystem);
			//remove items that don't match our filteringPredicate
			itemsMatching.RemoveAll(
					delegate(RecordToken<LexEntry> token) { return !(bool) token["Matches"]; });
			return itemsMatching;
		}

		#region IDisposable Members

#if DEBUG
		~LexEntryRepository()
		{
			if (!_disposed)
			{
				throw new ApplicationException(
						"Disposed not explicitly called on LexEntryRepository.");
			}
		}
#endif

		private bool _disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic
					_decoratedRepository.Dispose();
				}

				// shared (dispose and finalizable) cleanup logic
				_disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("LexEntryRepository");
			}
		}

		#endregion

		public bool BackendConsumePendingLiftUpdates()
		{
			//Do Nothing
			return true;
			//throw new Exception("The method or operation is not implemented.");
		}

		public bool BackendBringCachesUpToDate()
		{
			//Do Nothing
			return true;
			//throw new Exception("The method or operation is not implemented.");
		}

		public void BackendLiftIsFreshNow()
		{
			//Do Nothing
			//throw new Exception("The method or operation is not implemented.");
		}

		public void BackendRecoverUnsavedChangesOutOfCacheIfNeeded()
		{
			//Do Nothing
			//throw new Exception("The method or operation is not implemented.");
		}
	}
}
