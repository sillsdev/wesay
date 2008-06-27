using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Inside.Query;
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
			IQuery<LexEntry> query = GetLexEntryQuery(writingSystem, true);
			List<RecordToken<LexEntry>> recordTokens =
					new List<RecordToken<LexEntry>>(query.RetrieveItems());

			// filter out any entries that were added because of other
			// writing systems (e.g. reversals)
			int index = 0;
			while (index != recordTokens.Count)
			{
				if (recordTokens[index].DisplayString.EndsWith("*"))
				{
					recordTokens.RemoveAt(index);
				}
				else
				{
					++index;
				}
			}

			return
					new ResultSet<LexEntry>(this,
											ApproximateMatcher.FindClosestForms
													<RecordToken<LexEntry>>(recordTokens,
																			GetFormForMatchingStrategy,
																			lexicalForm,
																			matcherOptions));
		}

		private static string GetFormForMatchingStrategy(object item)
		{
			return ((RecordToken<LexEntry>) item).DisplayString;
		}

		public ResultSet<LexEntry> GetEntriesMatching(IQuery<LexEntry> query)
		{
			throw new NotImplementedException("GetEntriesMatching");
			// Run the sorted query
			//ResultSet<LexEntry> recordTokens = query.RetrieveItems();
			// Apply a filter
		}

		public ResultSet<LexEntry> GetEntriesWithMatchingLexicalForm(string lexicalForm,
																	 WritingSystem writingSystem)
		{
			// search dictionary for entry with new lexical form
			IQuery<LexEntry> query = GetLexEntryQuery(writingSystem, true);
			ResultSet<LexEntry> recordTokens = query.RetrieveItems();

			// This should probably be optimized by using a specific query
			List<RecordToken<LexEntry>> result = new List<RecordToken<LexEntry>>();
			int index = recordTokens.FindFirstIndexWithDisplayString(lexicalForm);
			while (index >= 0 && index < recordTokens.Count &&
				   recordTokens[index].DisplayString == lexicalForm)
			{
				result.Add(recordTokens[index]);
				++index;
			}
			return new ResultSet<LexEntry>(this, result);
		}

		public RecordTokenComparer<LexEntry> GetRecordTokenComparerForLexicalForm(
				WritingSystem writingSystem)
		{
			throw new NotImplementedException("GetRecordTokenComparerForLexicalForm");
			//LexEntrySortHelper sortHelper = new LexEntrySortHelper(this,
			//                                                       writingSystem,
			//                                                       true);
			//return new RecordTokenComparer(sortHelper.KeyComparer);
		}

		IEnumerable<string[]> GetCitationForms(LexEntry e)
		{
			string[] result = new string[2];
			foreach (LanguageForm form in e.CitationForm.Forms)
			{
				result[0] = form.WritingSystemId;
				result[1] = form.Form;
				yield return result;
			}
		}

		public ResultSet<LexEntry> GetAllEntriesSortedByHeadword(WritingSystem headwordWritingSystem)
		{

			Query entriesByHeadwordQuery = GetAllLexEntriesQuery().In("VirtualHeadWord")
									.ForEach("Forms").Show("Form").Show("WritingSystemId");

			ResultSet<LexEntry> entriesByHeadword = GetItemsMatching(entriesByHeadwordQuery);
			// sort by repositoryId, writingSystem
			entriesByHeadword.Sort( new SortKeyDefinition("Id", Comparer<RepositoryId>.Default),
									new SortKeyDefinition("WritingSystemId", Comparer<string>.Default));
			// remove all entries with writing system != headwordWritingSystem
			//            make sure always have one entry though
			// sort: by headword
			ResultSet<LexEntry> entriesByLexicalForm = GetItemsMatching(entriesByLexicalFormQuery);
			// sort: by repository id, writingSystemId (headwordWritingSystem to top)
			// walk list of entries, removing duplicate entries which have same repository id
			//                       if first entry for a repository id does not have writing
			//                       system == headwordWritingSystem then replace display string
			//                       to empty string and writingSystemId to headword
			// merge: replace entriesByLexicalForm with entriesByCitationForm when they have same repository id

			for(int i = 0; i != entriesByLexicalForm.)
			foreach (RecordToken<LexEntry> token in entriesByLexicalForm)
			{

			}

			List<RecordToken<LexEntry>> result = new List<RecordToken<LexEntry>>(r.Count);
			foreach (RecordToken<LexEntry> token in r)
			{
				string headword = HeadwordFromCitationFormAndLexicalForm(token.Results[0], token.Results[1]);
				result.Add(new RecordToken<LexEntry>(this, headword, token.Id));
			}
			result.Sort(new RecordTokenComparer<LexEntry>(headwordWritingSystem));
			return new ResultSet<LexEntry>(this, result);

			//IQuery<LexEntry> query =
			//        new Db4oHeadwordQuery(this, Db4oDataSource, headwordWritingSystem);
			//return query.RetrieveItems();
		}

		private static string HeadwordFromCitationFormAndLexicalForm(
			string citation,
			string lexicalForm)
		{
			if (String.IsNullOrEmpty(citation))
			{
				return lexicalForm;
			}
			return citation;
		}

		public LexEntry GetLexEntryWithMatchingId(string id)
		{
			IQuery q = _recordListManager.DataSource.Data.Query();
			q.Constrain(typeof (LexEntry));
			q.Descend("_id").Constrain(id);
			IObjectSet matches = q.Execute();
			if (matches.Count == 0)
			{
				return null;
			}
			if (matches.Count > 1) //review: not sure if we should throw or not
			{
				throw new ApplicationException(
						String.Format("There were {0} objects found with the id {1}",
									  matches.Count,
									  id));
			}
			Debug.Assert(matches[0].GetType() == typeof (LexEntry));
			return (LexEntry) matches[0];
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
			List<RecordToken<LexEntry>> list =
					KeyToEntryIdInitializer.GetLexicalFormToEntryIdPairs(this,
																		 GetLexEntryQuery(
																				 writingSystem, true),
																		 _recordListManager.
																				 DataSource,
																		 writingSystem.Id);

			list.Sort(new RecordTokenComparer<LexEntry>(writingSystem));
			return new ResultSet<LexEntry>(this, list);
		}

		public RepositoryId[] GetAllItems()
		{
			return _decoratedRepository.GetAllItems();
		}

		public ResultSet<LexEntry> GetAllEntriesSortedBySemanticDomain(string fieldName)
		{
			SemanticDomainSortHelper sortHelper =
					new SemanticDomainSortHelper(this, _recordListManager.DataSource, fieldName);
			return
					new ResultSet<LexEntry>(this,
											PrivateDb4oRecordListManager.GetSortedList(sortHelper));
		}

		public RecordTokenComparer<LexEntry> GetRecordTokenComparerForSemanticDomain(
				string fieldName)
		{
			SemanticDomainSortHelper sortHelper =
					new SemanticDomainSortHelper(this, _recordListManager.DataSource, fieldName);
			return new RecordTokenComparer<LexEntry>(sortHelper.KeyComparer);
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
			return new Query("LexEntry");
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

		private class SemanticDomainSortHelper: ISortHelper<LexEntry>
		{
			private readonly Db4oDataSource _db4oData;
			private readonly LexEntryRepository _repository;
			private readonly string _semanticDomainFieldName;

			public SemanticDomainSortHelper(LexEntryRepository repository,
											Db4oDataSource db4oData,
											string semanticDomainFieldName)
			{
				if (db4oData == null)
				{
					throw new ArgumentNullException("db4oData");
				}
				if (semanticDomainFieldName == null)
				{
					throw new ArgumentNullException("semanticDomainFieldName");
				}
				if (semanticDomainFieldName == string.Empty)
				{
					throw new ArgumentOutOfRangeException("semanticDomainFieldName");
				}

				_db4oData = db4oData;
				_repository = repository;
				_semanticDomainFieldName = semanticDomainFieldName;
			}

			#region IDb4oSortHelper<string,LexEntry> Members

			public IComparer<string> KeyComparer
			{
				get { return StringComparer.InvariantCulture; }
			}

			public ResultSet<LexEntry> RetrieveItems()
			{
				return
						new ResultSet<LexEntry>(_repository,
												KeyToEntryIdInitializer.GetKeyToEntryIdPairs(
														_repository,
														this,
														_db4oData,
														GetDisplayStrings));
			}

			public IEnumerable<string[]> GetDisplayStrings(LexEntry item)
			{
				List<string> keys = new List<string>();
				foreach (LexSense sense in item.Senses)
				{
					OptionRefCollection semanticDomains =
							sense.GetProperty<OptionRefCollection>(_semanticDomainFieldName);

					if (semanticDomains != null)
					{
						foreach (string s in semanticDomains.Keys)
						{
							if (!keys.Contains(s))
							{
								keys.Add(s);
							}
						}
					}
				}
				foreach (string key in keys)
				{
					string[] columns = new string[1];
					columns[0] = key;
					yield return columns;
				}
			}

			public string Name
			{
				get { return "LexEntry sorted by " + _semanticDomainFieldName; }
			}

			public override int GetHashCode()
			{
				return _semanticDomainFieldName.GetHashCode();
			}

			#endregion
		}
	}
}