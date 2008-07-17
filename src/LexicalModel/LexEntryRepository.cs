using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiftIO.Parsing;
using Palaso.Text;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel.Db4oSpecific;

namespace WeSay.LexicalModel
{

	public class LexEntryRepository: IRepository<LexEntry>
	{
		private static int _count = 0;
		private readonly IRepository<LexEntry> _decoratedRepository;
		public LexEntryRepository(string path)
		{
			IncCount();

			//use default of Db4oRepository for now
			//todo: eventually use synchronicRepository with Db4o and Lift
			_decoratedRepository = new LiftRepository(path);
		}

		public LexEntryRepository(IRepository<LexEntry> decoratedRepository)
		{
			IncCount();
			if (decoratedRepository == null)
			{
				throw new ArgumentNullException("decoratedRepository");
			}

			_decoratedRepository = decoratedRepository;
		}

		private static void IncCount()
		{
			_count++;
			Debug.WriteLine(String.Format("LER Count {0:d}", _count));
		}

		public DateTime LastModified
		{
			get
			{
				return _decoratedRepository.LastModified;
			}
		}

		public LexEntry CreateItem()
		{
			LexEntry item = this._decoratedRepository.CreateItem();
			return item;
		}

		// todo:remove
		public LexEntry CreateItem(Extensible eInfo)
		{
			LexEntry item = this._decoratedRepository.CreateItem();
			item.Guid = eInfo.Guid;
			item.Id = eInfo.Id;
			item.ModificationTime = eInfo.ModificationTime;
			item.CreationTime = eInfo.CreationTime;
			return item;
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
			return _decoratedRepository.GetItem(id);
		}

		public void SaveItems(IEnumerable<LexEntry> items)
		{
			_decoratedRepository.SaveItems(items);
		}

		public ResultSet<LexEntry> GetItemsMatching(Query query)
		{
			throw new NotSupportedException("Please use more specific methods. For now, we don't support using any general query for optimization reasons.");
			//return _decoratedRepository.GetItemsMatching(query);
		}

		private ResultSet<LexEntry> GetItemsMatchingCore(Query query)
		{
			return _decoratedRepository.GetItemsMatching(query);
		}

		public void SaveItem(LexEntry item)
		{
			_decoratedRepository.SaveItem(item);
		}

		public bool CanQuery
		{
			get { return _decoratedRepository.CanQuery; }
		}

		public bool CanPersist
		{
			get
			{
				return _decoratedRepository.CanPersist;
			}
		}

		public void DeleteItem(LexEntry item)
		{
			_decoratedRepository.DeleteItem(item);
		}

		public void DeleteItem(RepositoryId repositoryId)
		{
			_decoratedRepository.DeleteItem(repositoryId);
		}

		public void DeleteAllItems()
		{
			_decoratedRepository.DeleteAllItems();
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
			if(first == null)
			{
				throw new ArgumentOutOfRangeException("entry", entry, "Entry not in repository");
			}
			if ((bool)first["HasHomograph"])
			{
				return (int) first["HomographNumber"];
			}
			else
			{
				return 0;
			}
		}

		//todo: look at @order and the planned-for order-in-lift field on LexEntry
		public ResultSet<LexEntry> GetAllEntriesSortedByHeadword(WritingSystem writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			Query query = GetAllLexEntriesQuery();
			query.In("VirtualHeadWord").ForEach("Forms").Show("Form").Show("WritingSystemId");
			query.Show("OrderForRoundTripping");
			query.Show("CreationTime");

			ResultSet<LexEntry> itemsMatching = GetItemsMatchingQueryFilteredByWritingSystemAndSortedByHeadword(query, "Form", "WritingSystemId", writingSystem);
			itemsMatching.Sort(new SortDefinition("Form", writingSystem),
				new SortDefinition("OrderForRoundTripping", Comparer<int>.Default),
				new SortDefinition("CreationTime", Comparer<DateTime>.Default));

			string previousHeadWord = null;
			int homographNumber = 1;
			RecordToken<LexEntry> previousToken = null;
			foreach (RecordToken<LexEntry> token in itemsMatching)
			{
				string currentHeadWord = (string)token["Form"];
				if(currentHeadWord == previousHeadWord)
				{
					homographNumber++;
				}
				else
				{
					previousHeadWord = currentHeadWord;
					homographNumber = 1;
				}
				// only used to get our sort correct
				token["HomographNumber"] = homographNumber;
				switch(homographNumber)
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

			return itemsMatching;
		}


		public ResultSet<LexEntry> GetAllEntriesSortedByLexicalForm(WritingSystem writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			Query query = GetAllLexEntriesQuery().In("LexicalForm")
						.ForEach("Forms").Show("Form").Show("WritingSystemId");

			return GetItemsMatchingQueryFilteredByWritingSystemAndSortedByHeadword(query, "Form", "WritingSystemId", writingSystem);
		}

		public ResultSet<LexEntry> GetAllEntriesSortedByGloss(WritingSystem writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			Query query = GetAllLexEntriesQuery().ForEach("Senses");
			query.In("Definition").ForEach("Forms").Show("Form", "Definition/Form").Show("WritingSystemId", "Definition/WritingSystemId");
			query.In("Gloss").ForEach("Forms").Show("Form", "Gloss/Form").Show("WritingSystemId","Gloss/WritingSystemId");

			return GetItemsMatchingQueryFilteredByWritingSystemAndSortedByHeadword(query, "Definition/Form", "Definition/WritingSystemId", writingSystem);
		}

		private ResultSet<LexEntry> GetItemsMatchingQueryFilteredByWritingSystemAndSortedByHeadword(
			Query query,
			string formField,
			string writingSystemIdField,
			WritingSystem writingSystem)
		{
			ResultSet<LexEntry> allEntriesMatchingQuery = GetItemsMatchingCore(query);
			ResultSet<LexEntry> result = FilterEntriesToOnlyThoseWithWritingSystemId(
											allEntriesMatchingQuery,
											formField,
											writingSystemIdField,
											writingSystem.Id);

			result.Sort(new SortDefinition(formField, writingSystem));
			return result;
		}

		public ResultSet<LexEntry> GetAllEntriesSortedBySemanticDomain(
			string fieldName)
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			Query allSensePropertiesQuery = GetAllLexEntriesQuery().ForEach("Senses")
						.ForEach("Properties").Show("Key").Show("Value","SemanticDomains");

			ResultSet<LexEntry> allSenseProperties = GetItemsMatchingCore(allSensePropertiesQuery);
			allSenseProperties.RemoveAll(delegate(RecordToken<LexEntry> token)
				{
					return (string)token["Key"] != fieldName;
				});

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

			return RemoveDuplicateResults(resultSet);
		}

		private ResultSet<LexEntry> RemoveDuplicateResults(
			ResultSet<LexEntry> resultSet)
		{
			List<RecordToken<LexEntry>> result = new List<RecordToken<LexEntry>>();
			RecordToken<LexEntry> previousToken = null;
			foreach (RecordToken<LexEntry> token in resultSet)
			{
				if(token != previousToken)
				{
					result.Add(token);
				}
				previousToken = token;
			}
			return new ResultSet<LexEntry>(this, result);
		}

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
			query.ForEach("Senses").In("Gloss").ForEach("Forms").Show("Form", "Gloss/Form").Show("WritingSystemId", "Gloss/WritingSystemId");

			ResultSet<LexEntry> resultSet = GetItemsMatchingQueryFilteredByWritingSystemAndSortedByHeadword(
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

		public LexEntry GetLexEntryWithMatchingId(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			Query idOfEntries = GetAllLexEntriesQuery().Show("Id");
			ResultSet<LexEntry> items = GetItemsMatchingCore(idOfEntries);
			RecordToken<LexEntry> first = items.FindFirst(delegate(RecordToken<LexEntry> token)
												{
													return (string)token["Id"] == id;
												});
			if (first == null)
			{
				return null;
			}
			return first.RealObject;
		}

		public LexEntry GetLexEntryWithMatchingGuid(Guid guid)
		{
			Query query = GetAllLexEntriesQuery().Show("Guid");
			ResultSet<LexEntry> items = GetItemsMatchingCore(query);
			int index = items.FindFirstIndex(delegate(RecordToken<LexEntry> token)
												{
													return (Guid)token["Guid"] == guid;
												});
			if (index < 0)
			{
				return null;
			}
			if (index + 1 < items.Count)
			{
				int nextIndex = items.FindFirstIndex(index+1,
									delegate(RecordToken<LexEntry> token)
									{
										return (Guid)token["Guid"] == guid;
									});

				if(nextIndex >= 0)
				{
					throw new ApplicationException("More than one entry exists with the guid " +
												   guid);
				}
			}
			RecordToken<LexEntry> first = items[index];
			return first.RealObject;
		}

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
													<RecordToken<LexEntry>>(GetAllEntriesSortedByLexicalForm(writingSystem),
																			GetFormForMatchingStrategy,
																			lexicalForm,
																			matcherOptions));
		}

		private static string GetFormForMatchingStrategy(object item)
		{
			return (string)((RecordToken<LexEntry>)item)["Form"];
		}

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
			resultSet.RemoveAll(delegate(RecordToken<LexEntry> token)
								{
									return (string)token["Form"] != lexicalForm;
								});
			return resultSet;
		}

		private static Query GetAllLexEntriesQuery()
		{
			return new Query(typeof(LexEntry));
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

			allResults.SortByRepositoryId();

			// remove all entries with writing system != headwordWritingSystem
			//     make sure always have one entry though
			// walk list of entries, removing duplicate entries which have same repository id
			//     if there is no entry for a repository id that has
			//     writingSystemId == filterOnWritingSystemId then
			//     insert an empty form with writingSystemId set to filterOnWritingSystemId

			Dictionary<string, object> emptyResults = new Dictionary<string, object>();
			emptyResults.Add(formField, string.Empty);
			emptyResults.Add(writingSystemIdField, filterOnWritingSystemId);

			List<RecordToken<LexEntry>> filteredResults = new List<RecordToken<LexEntry>>();
			bool headWordRepositoryIdFound = false;
			RecordToken<LexEntry> previousToken = null;
			foreach (RecordToken<LexEntry> token in allResults)
			{
				if (previousToken != null && token.Id != previousToken.Id)
				{
					if (!headWordRepositoryIdFound)
					{
						filteredResults.Add(
								new RecordToken<LexEntry>(this,
														  emptyResults,
														  previousToken.Id));
					}
					headWordRepositoryIdFound = false;
				}

				object id;
				if (token.TryGetValue(writingSystemIdField, out id))
				{
					if((string) id == filterOnWritingSystemId)
					{
						filteredResults.Add(token);
						headWordRepositoryIdFound = true;
					}
				}
				else {
					// we have an entry but without a form with the given id.
					// Create an empty one
					token[formField] = string.Empty;
					token[writingSystemIdField] = filterOnWritingSystemId;
					filteredResults.Add(token);
					headWordRepositoryIdFound = true;
				}
				previousToken = token;
			}
			if (!headWordRepositoryIdFound)
			{
				filteredResults.Add(new RecordToken<LexEntry>(this,
															  emptyResults,
															  previousToken.Id));
			}
			return new ResultSet<LexEntry>(this, filteredResults);
		}


		public ResultSet<LexEntry> GetEntriesWithMissingFieldSortedByLexicalUnit(
				Field field, WritingSystem lexicalUnitWritingSystem)
		{
			Predicate<LexEntry> filteringPredicate = new MissingFieldQuery(field).FilteringPredicate;
			Query query = new Query.PredicateQuery<LexEntry>(filteringPredicate);
			query.In("LexicalForm").ForEach("Forms").Show("Form").Show("WritingSystemId");


			ResultSet<LexEntry> itemsMatching = GetItemsMatchingQueryFilteredByWritingSystemAndSortedByHeadword(query, "Form", "WritingSystemId", lexicalUnitWritingSystem);
			//remove items that don't match our filteringPredicate
			itemsMatching.RemoveAll(delegate(RecordToken<LexEntry> token)
								{
									return !(bool)token["Matches"];
								});
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

		private bool _disposed = false;

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

		[Obsolete]
		//todo: make this private and part of synchronic repository
		public IList<RepositoryId> GetItemsModifiedSince(DateTime last)
		{
			Query query = new Query(typeof(LexEntry)).Show("ModificationTime");
			ResultSet<LexEntry> items = GetItemsMatchingCore(query);
			// remove items that were modified before last
			items.RemoveAll(delegate(RecordToken<LexEntry> token)
								{
									return (DateTime)token["ModificationTime"] < last;
								});
			return new List<RepositoryId>(items);
		}

		public void BackendDoLiftUpdateNow(bool p)
		{
			//Do Nothing
			//throw new Exception("The method or operation is not implemented.");
		}

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
