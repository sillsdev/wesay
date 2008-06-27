using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using LiftIO.Parsing;
using Palaso.Text;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel.Db4oSpecific;
using Debug=System.Diagnostics.Debug;

namespace WeSay.LexicalModel
{

	public class LexEntryRepository: IRepository<LexEntry>
	{
		private readonly IRepository<LexEntry> _decoratedRepository;
		public LexEntryRepository(string path)
		{
			//use default of Db4oRepository for now
			//todo: eventually use synchronicRepository with Db4o and Lift
			_decoratedRepository = new Db4oRepository<LexEntry>(path);
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
			return _decoratedRepository.GetItemsMatching(query);
		}

		public void SaveItem(LexEntry item)
		{
			_decoratedRepository.SaveItem(item);
		}

		public void DeleteItem(LexEntry item)
		{
			_decoratedRepository.DeleteItem(item);
		}

		public void DeleteItem(RepositoryId repositoryId)
		{
			_decoratedRepository.DeleteItem(repositoryId);
		}

		public IQuery<LexEntry> GetLexEntryQuery(WritingSystem writingSystem,
												 bool isWritingSystemUsedByLexicalForm)
		{
			return new Db4oLexEntryQuery(this, writingSystem, isWritingSystemUsedByLexicalForm);
		}

		public ResultSet<LexEntry> GetEntriesWithSimilarLexicalForm(string lexicalForm,
																	WritingSystem writingSystem,
																	ApproximateMatcherOptions
																			matcherOptions)
		{
			return new ResultSet<LexEntry>(this,
										   ApproximateMatcher.FindClosestForms
													<RecordToken<LexEntry>>(GetAllEntriesSortedByLexicalForm(writingSystem),
																			GetFormForMatchingStrategy,
																			lexicalForm,
																			matcherOptions));
		}

		private static string GetFormForMatchingStrategy(object item)
		{
			return (string)((RecordToken<LexEntry>) item).Results["Form"];
		}

		public ResultSet<LexEntry> GetEntriesWithMatchingLexicalForm(string lexicalForm,
																	 WritingSystem writingSystem)
		{
			ResultSet<LexEntry> resultSet = GetAllEntriesSortedByLexicalForm(writingSystem);
			resultSet.RemoveAll(delegate (RecordToken<LexEntry> token)
								{
									return (string)token.Results["Form"] != lexicalForm;
								});
			return resultSet;
		}

		public ResultSet<LexEntry> GetAllEntriesSortedByHeadword(WritingSystem headwordWritingSystem)
		{
			Query entriesByHeadwordQuery = GetAllLexEntriesQuery().In("VirtualHeadWord")
									.ForEach("Forms").Show("Form").Show("WritingSystemId");
			return GetItemsMatching(entriesByHeadwordQuery, headwordWritingSystem);
		}

		private ResultSet<LexEntry> FilterEntriesToOnlyThoseWithWritingSystemId(ResultSet<LexEntry> entriesWithAllHeadwords, string headwordWritingSystemId)
		{
			entriesWithAllHeadwords.Sort(new SortDefinition("RepositoryId", Comparer<RepositoryId>.Default));

			// remove all entries with writing system != headwordWritingSystem
			//            make sure always have one entry though
			// walk list of entries, removing duplicate entries which have same repository id
			//     if there is no entry for a repository id that has
			//     writingSystemId == headwordWritingSystem then
			//     insert an empty form with writingSystemId to headwordId

			Dictionary<string, object> emptyResults = new Dictionary<string, object>();
			emptyResults.Add("Form",string.Empty);
			emptyResults.Add("WritingSystemId",headwordWritingSystemId);

			List<RecordToken<LexEntry>> entriesWithHeadword = new List<RecordToken<LexEntry>>();
			RepositoryId previousRepositoryId = null;
			bool headWordRepositoryIdFound = false;

			foreach (RecordToken<LexEntry> token in entriesWithAllHeadwords)
			{
				if(token.Id != previousRepositoryId)
				{
					if(!headWordRepositoryIdFound)
					{
						entriesWithHeadword.Add(new RecordToken<LexEntry>(this, emptyResults, previousRepositoryId));
					}
					headWordRepositoryIdFound = false;
					previousRepositoryId = token.Id;
				}

				if ((string)token.Results["WritingSystemId"] == headwordWritingSystemId)
				{
					entriesWithHeadword.Add(token);
					headWordRepositoryIdFound = true;
				}

			}
			if(!headWordRepositoryIdFound)
			{
				entriesWithHeadword.Add(new RecordToken<LexEntry>(this, emptyResults, previousRepositoryId));
			}
			return new ResultSet<LexEntry>(this, entriesWithHeadword);
		}

		public LexEntry GetLexEntryWithMatchingId(string id)
		{
			Query idOfEntries = GetAllLexEntriesQuery().Show("Id");
			ResultSet<LexEntry> items = GetItemsMatching(idOfEntries);
			RecordToken<LexEntry> first = items.FindFirst(delegate(RecordToken<LexEntry> token)
												{
													return (string)token.Results["Id"] == id;
												});
			if(first == null)
			{
				return null;
			}
			return first.RealObject;
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

			IQuery<LexEntry> query =
					new Db4oHeadwordQuery(this, Db4oDataSource, headwordWritingSystem);
			ResultSet<LexEntry> recordTokensSortedByHeadWord = query.RetrieveItems();
			RepositoryId databaseIdOfEntry = GetId(entry);
			// find our position within the sorted list of entries
			int ourIndex = -1;
			for (int i = 0;i != recordTokensSortedByHeadWord.Count;++i)
			{
				if (recordTokensSortedByHeadWord[i].Id == databaseIdOfEntry)
				{
					ourIndex = i;
					break;
				}
			}
			string headword = entry.GetHeadWordForm(headwordWritingSystem.Id);

			//todo: this is bogus; it fullfills our round-tripping requirement, but would
			//give us bogus homograph numbers

			if (entry.OrderForRoundTripping > 0)
			{
				return entry.OrderForRoundTripping;
			}

			//what number are we?
			int found = 0;

			for (int searchIndex = ourIndex - 1;searchIndex > -1;--searchIndex)
			{
				RepositoryId searchId = recordTokensSortedByHeadWord[searchIndex].Id;
				LexEntry previousGuy = GetItem(searchId);

				if (headword != previousGuy.GetHeadWordForm(headwordWritingSystem.Id))
				{
					break;
				}
				++found;
			}

			// if we're the first with this headword
			if (found == 0)
			{
				//and we're the last entry
				if (ourIndex + 1 >= recordTokensSortedByHeadWord.Count)
				{
					return 0; //no homograph number
				}
				RepositoryId nextId = recordTokensSortedByHeadWord[ourIndex + 1].Id;
				LexEntry nextGuy = GetItem(nextId);

				// the next guy doesn't match
				if (headword != nextGuy.GetHeadWordForm(headwordWritingSystem.Id))
				{
					return 0; //no homograph number
				}
				else
				{
					return 1;
				}
			}
			//there were preceding homographs
			return 1 + found;

			//todo: look at @order and the planned-for order-in-lift field on LexEntry
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

		public ResultSet<LexEntry> GetAllEntriesSortedByLexicalForm(WritingSystem writingSystem)
		{
			Query entriesByHeadwordQuery = GetAllLexEntriesQuery().In("LexicalForm")
						.ForEach("Forms").Show("Form").Show("WritingSystemId");

			return GetItemsMatching(entriesByHeadwordQuery, writingSystem);
		}

		private ResultSet<LexEntry> GetItemsMatching(Query entriesByHeadwordQuery, WritingSystem writingSystem) {
			ResultSet<LexEntry> entriesWithAllHeadwords = GetItemsMatching(entriesByHeadwordQuery);
			ResultSet<LexEntry> result = FilterEntriesToOnlyThoseWithWritingSystemId(entriesWithAllHeadwords, writingSystem.Id);

			result.Sort(new SortDefinition("Form", writingSystem));
			return result;
		}

		public RepositoryId[] GetAllItems()
		{
			return _decoratedRepository.GetAllItems();
		}

		public ResultSet<LexEntry> GetAllEntriesSortedBySemanticDomain(string fieldName)
		{
			Query allSenseProperties = GetAllLexEntriesQuery().ForEach("Senses")
						.ForEach("Properties").Show("Key").Show("Value");

			ResultSet<LexEntry> results = GetItemsMatching(allSenseProperties);
			results.RemoveAll(delegate (RecordToken<LexEntry> token)
				{
					return (string)token.Results["Key"] != fieldName;
				});
			return results;
		}

		public ResultSet<LexEntry> GetEntriesWithMatchingGlossSortedByLexicalForm(
				LanguageForm glossForm, WritingSystem lexicalUnitWritingSystem)
		{
			IQuery<LexEntry> query = GetLexEntryQuery(lexicalUnitWritingSystem, true);

			List<RecordToken<LexEntry>> matches = new List<RecordToken<LexEntry>>();
			RepositoryId[] repositoryIds = GetAllItems();
			foreach (RepositoryId repositoryId in repositoryIds)
			{
				int i = 0;
				LexEntry entry = GetItem(repositoryId);
				foreach (LexSense sense in entry.Senses)
				{
					if (sense.Gloss[glossForm.WritingSystemId] == glossForm.Form)
					{
						foreach (string displayString in query.GetDisplayStrings(entry))
						{
							matches.Add(
									new RecordToken<LexEntry>(this,
															  query,
															  i,
															  displayString,
															  repositoryId));
							++i;
						}
					}
				}
			}
			return new ResultSet<LexEntry>(this, matches);
		}

		public LexEntry GetLexEntryWithMatchingGuid(Guid guid)
		{
			IQuery q = _recordListManager.DataSource.Data.Query();
			q.Constrain(typeof (LexEntry));
			q.Descend("_guid").Constrain(guid);
			IObjectSet matches = q.Execute();
			if (matches.Count == 0)
			{
				return null;
			}
			if (matches.Count > 1)
			{
				throw new ApplicationException(
						String.Format("There were {0} objects found with the guid {1}",
									  matches.Count,
									  guid));
			}
			Debug.Assert(matches[0].GetType() == typeof (LexEntry));
			return (LexEntry) matches[0];
		}

		public ResultSet<LexEntry> GetAllEntriesSortedByGloss(WritingSystem writingSystem)
		{
			Query allEntriesByGloss = GetAllLexEntriesQuery();
			allEntriesByGloss.AddResult("Senses/Gloss/Forms/Form");
			IQuery<LexEntry> query = GetLexEntryQuery(writingSystem, false);
			return query.RetrieveItems();
		}
		public static IEnumerable<string> SplitGlossAtSemicolon(MultiText gloss,
														string writingSystemId)
		{
			bool exact = true;
			string glossText = gloss.GetExactAlternative(writingSystemId);
			if (glossText == string.Empty)
			{
				exact = false;
				glossText = gloss.GetBestAlternative(writingSystemId, "*");
				if (glossText == "*")
				{
					glossText = string.Empty;
				}
			}

			List<string> result = new List<string>();
			string[] keylist = glossText.Split(new char[] { ';' });
			for (int i = 0; i < keylist.Length; i++)
			{
				string k = keylist[i].Trim();
				if ( /*keylist.Length > 1 &&*/ k.Length == 0)
				{
					continue;
				}
				if (exact || i == keylist.Length - 1)
				{
					result.Add(k);
				}
				else
				{
					result.Add(k + "*");
				}
			}
			return result;
		}


		private static Query GetAllLexEntriesQuery() {
			return new Query(typeof(LexEntry));
		}

		public RepositoryId[] GetItemsModifiedSince(DateTime last)
		{
			return _decoratedRepository.GetItemsModifiedSince(last);
		}

		public int CountAllItems()
		{
			return _decoratedRepository.CountAllItems();
		}

		public ResultSet<LexEntry> GetEntriesMatchingFilterSortedByLexicalUnit(
				IFilter<LexEntry> filter, WritingSystem lexicalUnitWritingSystem)
		{
			LexEntrySortHelper lexEntrySortHelper =
					new LexEntrySortHelper(this, lexicalUnitWritingSystem, true);
			_recordListManager.Register(filter, lexEntrySortHelper);
			List<RecordToken<LexEntry>> result = new List<RecordToken<LexEntry>>();
			foreach (LexEntry entry in
					_recordListManager.GetListOfTypeFilteredFurther(filter, lexEntrySortHelper))
			{
				RepositoryId id = GetId(entry);
				int i =
						result.FindAll(
								delegate(RecordToken<LexEntry> match) { return match.Id == id; }).
								Count;

				result.Add(
						new RecordToken<LexEntry>(this,
												  lexEntrySortHelper,
												  i,
												  entry.LexicalForm[lexicalUnitWritingSystem.Id],
												  id));
			}
			return new ResultSet<LexEntry>(this, result);
		}
	}
}