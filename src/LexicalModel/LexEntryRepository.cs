using System;
using System.Collections.Generic;
using System.Diagnostics;
using Palaso.Data;
using Palaso.Code;
using Palaso.DictionaryServices.Lift;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Lift.Options;
using Palaso.UiBindings;
using Palaso.Progress;
using Palaso.Text;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

//#if MONO
using Palaso.Linq;
//#else

//#endif

namespace WeSay.LexicalModel
{
	public class LexEntryRepository : IDataMapper<LexEntry>, ICountGiver
	{
		public class EntryEventArgs : EventArgs
		{
			public readonly string label;

			public EntryEventArgs(LexEntry entry)
			{
				label = entry.LexicalForm.GetFirstAlternative();
			}
			public EntryEventArgs(RepositoryId repositoryId)
			{
				label = "?";//enhance: how do we get a decent label?
			}
		}
		public event EventHandler<EntryEventArgs> AfterEntryModified;
		public event EventHandler<EntryEventArgs> AfterEntryDeleted;
		//public event EventHandler<EntryEventArgs> AfterEntryAdded;  I (JH) don't know how to tell the difference between new and modified


		readonly ResultSetCacheManager<LexEntry> _caches = new ResultSetCacheManager<LexEntry>();

		//hack to prevent sending nested Save calls, which was causing a bug when
		//the exporter caused an item to get a new id, which led eventually to the list thinking it was modified, etc...
		private bool _currentlySaving;

		private readonly IDataMapper<LexEntry> _decoratedDataMapper;

		#if DEBUG
		private readonly StackTrace _constructionStackTrace;
		#endif

		// review: this constructor is only used for tests, and causes grief with
		// the dispose pattern.  Remove and refactor tests to use the other constructor
		// in a using style. cp
		public LexEntryRepository(string path)
		{
			_disposed = true;
			#if DEBUG
			_constructionStackTrace = new StackTrace();
			#endif
			_decoratedDataMapper = new LiftDataMapper(
				path, null, new string[] {}, new ProgressState()
			);
			_disposed = false;
		}

		// review: may want to change LiftDataMapper to IDataMapper<LexEntry> but I (cp) am leaving
		// this for the moment as would also need to change the container builder.Register in WeSayWordsProject
		public LexEntryRepository(LiftDataMapper decoratedDataMapper)
		{
			Guard.AgainstNull(decoratedDataMapper, "decoratedDataMapper");
			#if DEBUG
			_constructionStackTrace = new StackTrace();
			#endif
			_decoratedDataMapper = decoratedDataMapper;
			_disposed = false;
		}


		public DateTime LastModified
		{
			get { return _decoratedDataMapper.LastModified; }
		}

		public LexEntry CreateItem()
		{
			LexEntry item = _decoratedDataMapper.CreateItem();
			_caches.AddItemToCaches(item);
			return item;
		}

		public RepositoryId[] GetAllItems()
		{
			return _decoratedDataMapper.GetAllItems();
		}

		public int CountAllItems()
		{
			return _decoratedDataMapper.CountAllItems();
		}

		public RepositoryId GetId(LexEntry item)
		{
			return _decoratedDataMapper.GetId(item);
		}

		public LexEntry GetItem(RepositoryId id)
		{
			LexEntry item = _decoratedDataMapper.GetItem(id);
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
					_caches.UpdateItemInCaches(item);
				}
			}
			_decoratedDataMapper.SaveItems(dirtyItems);
			foreach (LexEntry item in dirtyItems)
			{
				item.Clean();
			}
		}

		public ResultSet<LexEntry> GetItemsMatching(IQuery<LexEntry> query)
		{
			return _decoratedDataMapper.GetItemsMatching(query);
		}

		public void SaveItem(LexEntry item)
		{
			if (_currentlySaving) //sometimes the process of saving leads modification which leads to a new save
			{
				return;
			}
			_currentlySaving = true;
			try
			{

				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				if (item.IsDirty)
				{
					_decoratedDataMapper.SaveItem(item);
					_caches.UpdateItemInCaches(item);
					item.Clean();

					//review: I (JH) don't know how to tell the difference between new and modified
					if (AfterEntryModified != null)
					{
						AfterEntryModified(this, new EntryEventArgs(item));
					}
				}
			}
			finally
			{
				_currentlySaving = false;
			}
		}

		public bool CanQuery
		{
			get { return _decoratedDataMapper.CanQuery; }
		}

		public bool CanPersist
		{
			get { return _decoratedDataMapper.CanPersist; }
		}

		public void DeleteItem(LexEntry item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			EntryEventArgs args = new EntryEventArgs(item);

			_caches.DeleteItemFromCaches(item);
			_decoratedDataMapper.DeleteItem(item);

			if(AfterEntryDeleted !=null)
			{
				AfterEntryDeleted(this, args);
			}
		}

		public void DeleteItem(RepositoryId repositoryId)
		{
			EntryEventArgs args = new EntryEventArgs(repositoryId);

			_caches.DeleteItemFromCaches(repositoryId);
			_decoratedDataMapper.DeleteItem(repositoryId);

			if(AfterEntryDeleted !=null)
			{
				AfterEntryDeleted(this, args);
			}
		}

		public void DeleteAllItems()
		{
			_decoratedDataMapper.DeleteAllItems();
			_caches.DeleteAllItemsFromCaches();
		}

		public void NotifyThatLexEntryHasBeenUpdated(LexEntry updatedLexEntry)
		{
			if(updatedLexEntry == null)
			{
				throw new ArgumentNullException("updatedLexEntry");
			}
			//This call checks that the Entry is in the repository
			GetId(updatedLexEntry);
			_caches.UpdateItemInCaches(updatedLexEntry);
		}

		public int GetHomographNumber(LexEntry entry, WritingSystemDefinition headwordWritingSystem)
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
		public ResultSet<LexEntry> GetAllEntriesSortedByHeadword(WritingSystemDefinition writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}

			string cacheName = String.Format("sortedByHeadWord_{0}", writingSystem.Id);
			if (_caches[cacheName] == null)
			{
				DelegateQuery<LexEntry> headWordQuery = new DelegateQuery<LexEntry>(
					delegate(LexEntry entryToQuery)
						 {
							 IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
							 string headWord = entryToQuery.VirtualHeadWord[writingSystem.Id];
							 if (String.IsNullOrEmpty(headWord))
							 {
									 headWord = null;
							 }
							 tokenFieldsAndValues.Add("Form",headWord);
							 return new[] { tokenFieldsAndValues };
						 });

				ResultSet<LexEntry> itemsMatching = _decoratedDataMapper.GetItemsMatching(headWordQuery);
				var sortOrder = new SortDefinition[4];
				sortOrder[0] = new SortDefinition("Form", writingSystem.Collator);
				sortOrder[1] = new SortDefinition("OrderForRoundTripping", Comparer<int>.Default);
				sortOrder[2] = new SortDefinition("OrderInFile", Comparer<int>.Default);
				sortOrder[3] = new SortDefinition("CreationTime", Comparer<DateTime>.Default);

				_caches.Add(cacheName, new ResultSetCache<LexEntry>(this, sortOrder, itemsMatching, headWordQuery));
				// _caches.Add(headWordQuery, /* itemsMatching */ results); // review cp Refactor caches to this signature.
			}
			ResultSet<LexEntry> resultsFromCache = _caches[cacheName].GetResultSet();

			string previousHeadWord = null;
			int homographNumber = 1;
			RecordToken<LexEntry> previousToken = null;
			foreach (RecordToken<LexEntry> token in resultsFromCache)
			{
				// A null Form indicates there is no HeadWord in this writing system.
				// However, we need to ensure that we return all entries, so the AtLeastOne in the query
				// above ensures that we keep it in the result set with a null Form and null WritingSystemId.
				string currentHeadWord = (string) token["Form"];
				if (string.IsNullOrEmpty(currentHeadWord))
				{
					token["HasHomograph"] = false;
					token["HomographNumber"] = 0;
					continue;
				}
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
		/// Gets a ResultSet containing all entries sorted by lexical form for a given writing system.
		/// If a lexical form for a given writingsystem does not exist we substitute one from another writingsystem.
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <param name="writingSystem"></param>
		/// <returns></returns>
		public ResultSet<LexEntry> GetAllEntriesSortedByLexicalFormOrAlternative(WritingSystemDefinition writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			string cacheName = String.Format("sortedByLexicalFormOrAlternative_{0}", writingSystem.Id);
			if (_caches[cacheName] == null)
			{
				DelegateQuery<LexEntry> lexicalFormWithAlternativeQuery = new DelegateQuery<LexEntry>(
					delegate(LexEntry entryToQuery)
					{
						IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
						string lexicalform = entryToQuery.LexicalForm[writingSystem.Id];
						string writingSystemOfForm = writingSystem.Id;
						if (lexicalform == "")
						{
							lexicalform = entryToQuery.LexicalForm.GetBestAlternative(writingSystem.Id);
							foreach (LanguageForm form in entryToQuery.LexicalForm.Forms)
							{
								if(form.Form == lexicalform)
								{
									writingSystemOfForm = form.WritingSystemId;
								}
							}
							if (lexicalform == "")
							{
								lexicalform = null;
							}
						}
						tokenFieldsAndValues.Add("Form", lexicalform);
						tokenFieldsAndValues.Add("WritingSystem", writingSystemOfForm);
						return new IDictionary<string, object>[] { tokenFieldsAndValues };
					});
				ResultSet<LexEntry> itemsMatching = _decoratedDataMapper.GetItemsMatching(lexicalFormWithAlternativeQuery);

				SortDefinition[] sortOrder = new SortDefinition[1];
				sortOrder[0] = new SortDefinition("Form", writingSystem.Collator);

				_caches.Add(cacheName, new ResultSetCache<LexEntry>(this, sortOrder, itemsMatching, lexicalFormWithAlternativeQuery));
			}
			ResultSet<LexEntry> resultsFromCache = _caches[cacheName].GetResultSet();

			return resultsFromCache;
		}

		/// <summary>
		/// Gets a ResultSet containing all entries sorted by lexical form for a given writing system.
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <param name="writingSystem"></param>
		/// <returns></returns>
		private ResultSet<LexEntry> GetAllEntriesSortedByLexicalForm(WritingSystemDefinition writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			string cacheName = String.Format("sortedByLexicalForm_{0}", writingSystem.Id);
			if (_caches[cacheName] == null)
			{
				DelegateQuery<LexEntry> lexicalFormQuery = new DelegateQuery<LexEntry>(
					delegate(LexEntry entryToQuery)
					{
						IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
						string headWord = entryToQuery.LexicalForm[writingSystem.Id];
						if (String.IsNullOrEmpty(headWord)){
								headWord = null;
						}
						tokenFieldsAndValues.Add("Form", headWord);
						return new IDictionary<string, object>[] { tokenFieldsAndValues };
					});
				ResultSet<LexEntry> itemsMatching = _decoratedDataMapper.GetItemsMatching(lexicalFormQuery);

				SortDefinition[] sortOrder = new SortDefinition[1];
				sortOrder[0] = new SortDefinition("Form", writingSystem.Collator);

				_caches.Add(cacheName, new ResultSetCache<LexEntry>(this, sortOrder, itemsMatching, lexicalFormQuery));
			}
			ResultSet<LexEntry> resultsFromCache = _caches[cacheName].GetResultSet();

			return resultsFromCache;
		}

		private ResultSet<LexEntry> GetAllEntriesSortedByGuid()
		{
			string cacheName = String.Format("sortedByGuid");
			if (_caches[cacheName] == null)
			{
				DelegateQuery<LexEntry> guidQuery = new DelegateQuery<LexEntry>(
					delegate(LexEntry entryToQuery)
					{
						IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
						tokenFieldsAndValues.Add("Guid", entryToQuery.Guid);
						return new IDictionary<string, object>[] { tokenFieldsAndValues };
					});
				ResultSet<LexEntry> itemsMatching = _decoratedDataMapper.GetItemsMatching(guidQuery);

				SortDefinition[] sortOrder = new SortDefinition[1];
				sortOrder[0] = new SortDefinition("Guid", Comparer<Guid>.Default);

				_caches.Add(cacheName, new ResultSetCache<LexEntry>(this, sortOrder, itemsMatching, guidQuery));
			}
			ResultSet<LexEntry> resultsFromCache = _caches[cacheName].GetResultSet();

			return resultsFromCache;
		}

		private ResultSet<LexEntry> GetAllEntriesSortedById()
		{
			string cacheName = String.Format("sortedById");
			if (_caches[cacheName] == null)
			{
				DelegateQuery<LexEntry> IdQuery = new DelegateQuery<LexEntry>(
					delegate(LexEntry entryToQuery)
					{
						IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
						tokenFieldsAndValues.Add("Id", entryToQuery.Id);
						return new IDictionary<string, object>[] { tokenFieldsAndValues };
					});
				ResultSet<LexEntry> itemsMatching = _decoratedDataMapper.GetItemsMatching(IdQuery);

				SortDefinition[] sortOrder = new SortDefinition[1];
				sortOrder[0] = new SortDefinition("Id", Comparer<string>.Default);

				_caches.Add(cacheName, new ResultSetCache<LexEntry>(this, sortOrder, itemsMatching, IdQuery));
			}
			ResultSet<LexEntry> resultsFromCache = _caches[cacheName].GetResultSet();

			return resultsFromCache;
		}

		/// <summary>
		/// Gets a ResultSet containing all entries sorted by definition and gloss. It will return both the definition
		/// and the gloss if both exist and are different.
		/// Use "Form" to access the Definition/Gloss in RecordToken.
		/// </summary>
		/// <param name="writingSystem"></param>
		/// <returns>Definition and gloss in "Form" field of RecordToken</returns>
		public ResultSet<LexEntry> GetAllEntriesSortedByDefinitionOrGloss(WritingSystemDefinition writingSystem)
		{
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}

			string cacheName = String.Format("SortByDefinition_{0}", writingSystem.Id);
			if (_caches[cacheName] == null)
			{
				DelegateQuery<LexEntry> definitionQuery = new DelegateQuery<LexEntry>(
					delegate(LexEntry entryToQuery)
					{
						List<IDictionary<string, object>> fieldsandValuesForRecordTokens = new List<IDictionary<string, object>>();

						int senseNumber = 0;
						foreach (LexSense sense in entryToQuery.Senses)
						{
							List<string> definitions = new List<string>();
							List<string> glosses = new List<string>();

							string rawDefinition = sense.Definition[writingSystem.Id];
							string rawGloss = sense.Gloss[writingSystem.Id];

							if(writingSystem.IsUnicodeEncoded)
							{
								definitions = GetTrimmedElementsSeperatedBySemiColon(rawDefinition);
								glosses = GetTrimmedElementsSeperatedBySemiColon(rawGloss);
							}
							else
							{
								definitions.Add(rawDefinition);
								glosses.Add(rawGloss);
							}

							List<string> definitionAndGlosses = new List<string>();
							definitionAndGlosses = MergeListsWhileExcludingDoublesAndEmptyStrings(definitions, glosses);

							if(definitionAndGlosses.Count == 0)
							{
								IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
								tokenFieldsAndValues.Add("Form", null);
								tokenFieldsAndValues.Add("Sense", senseNumber);
								fieldsandValuesForRecordTokens.Add(tokenFieldsAndValues);
							}
							else
							{
								foreach (string definition in definitionAndGlosses)
								{
									IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
									tokenFieldsAndValues.Add("Form", definition);
									tokenFieldsAndValues.Add("Sense", senseNumber);
									fieldsandValuesForRecordTokens.Add(tokenFieldsAndValues);
								}
							}

							senseNumber++;
						}
						return fieldsandValuesForRecordTokens;
					});
				ResultSet<LexEntry> itemsMatching = _decoratedDataMapper.GetItemsMatching(definitionQuery);

				SortDefinition[] sortOrder = new SortDefinition[2];
				sortOrder[0] = new SortDefinition("Form", writingSystem.Collator);
				sortOrder[1] = new SortDefinition("Sense", Comparer<int>.Default);
				_caches.Add(cacheName, new ResultSetCache<LexEntry>(this, sortOrder, itemsMatching, definitionQuery));
			}
			return _caches[cacheName].GetResultSet();
		}

		private List<string> MergeListsWhileExcludingDoublesAndEmptyStrings(IEnumerable<string> list1, IEnumerable<string> list2)
		{
			List<string> mergedList = new List<string>();
			foreach (string definitionElement in list1)
			{
				if((!mergedList.Contains(definitionElement)) && (definitionElement != ""))
				{
					mergedList.Add(definitionElement);
				}
			}
			foreach (string glossElement in list2)
			{
				if (!mergedList.Contains(glossElement) && (glossElement != ""))
				{
					mergedList.Add(glossElement);
				}
			}
			return mergedList;
		}

		private List<string> GetTrimmedElementsSeperatedBySemiColon(string text)
		{
			List<string> textElements = new List<string>();
			foreach (string textElement in text.Split(new char[] { ';' }))
			{
				string textElementTrimmed = textElement.Trim();
				textElements.Add(textElementTrimmed);
			}
			return textElements;
		}

		private bool IsNullOrEmptyOrSemiColon(string text)
		{
			return (String.IsNullOrEmpty(text) || text == ";");
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

			string cachename = String.Format("Semanticdomains_{0}", fieldName);

			if (_caches[cachename] == null)
			{
				DelegateQuery<LexEntry> semanticDomainsQuery = new DelegateQuery<LexEntry>(
					delegate(LexEntry entry)
					{
						List<IDictionary<string, object>> fieldsandValuesForRecordTokens = new List<IDictionary<string, object>>();
						foreach (LexSense sense in entry.Senses)
						{
							foreach (var pair in sense.Properties)
							{
								if (pair.Key == fieldName)
								{
									OptionRefCollection semanticDomains = (OptionRefCollection) pair.Value;
									foreach (string semanticDomain in semanticDomains.Keys)
									{
										IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
										string domain = semanticDomain;
										if (String.IsNullOrEmpty(semanticDomain))
										{
											domain = null;
										}
										if (CheckIfTokenHasAlreadyBeenReturnedForThisSemanticDomain(fieldsandValuesForRecordTokens, domain))
										{
											continue; //This is to avoid duplicates
										}
										tokenFieldsAndValues.Add("SemanticDomain", domain);
										fieldsandValuesForRecordTokens.Add(tokenFieldsAndValues);
									}
								}
							}
						}
						return fieldsandValuesForRecordTokens;
					}
					);
				ResultSet<LexEntry> itemsMatchingQuery = GetItemsMatching(semanticDomainsQuery);
				SortDefinition[] sortDefinition = new SortDefinition[2];
				sortDefinition[0] = new SortDefinition("SemanticDomain", StringComparer.InvariantCulture);
				sortDefinition[1] = new SortDefinition("Sense", Comparer<int>.Default);
				ResultSetCache<LexEntry> cache =
					new ResultSetCache<LexEntry>(this, sortDefinition, itemsMatchingQuery, semanticDomainsQuery);
				_caches.Add(cachename, cache);
			}
			return _caches[cachename].GetResultSet();
		}

		private bool CheckIfTokenHasAlreadyBeenReturnedForThisSemanticDomain(List<IDictionary<string, object>> fieldsandValuesForRecordTokens, string domain)
		{
			foreach (var tokenInfo in fieldsandValuesForRecordTokens)
			{
				if((string)tokenInfo["SemanticDomain"] == domain)
				{
					return true;
				}
			}
			return false;
		}


		private ResultSet<LexEntry> GetAllEntriesWithGlossesSortedByLexicalForm(WritingSystemDefinition lexicalUnitWritingSystem)
		{
			if (lexicalUnitWritingSystem == null)
			{
				throw new ArgumentNullException("lexicalUnitWritingSystem");
			}
			string cachename = String.Format("GlossesSortedByLexicalForm_{0}", lexicalUnitWritingSystem);
			if (_caches[cachename] == null)
			{
				DelegateQuery<LexEntry> MatchingGlossQuery = new DelegateQuery<LexEntry>(
					delegate(LexEntry entry)
					{
						List<IDictionary<string, object>> fieldsandValuesForRecordTokens = new List<IDictionary<string, object>>();
						int senseNumber = 0;
						foreach (LexSense sense in entry.Senses)
						{
							foreach (LanguageForm form in sense.Gloss.Forms)
							{
								IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
								string lexicalForm = entry.LexicalForm[lexicalUnitWritingSystem.Id];
								if (String.IsNullOrEmpty(lexicalForm))
								{
									lexicalForm = null;
								}
								tokenFieldsAndValues.Add("Form", lexicalForm);

								string gloss = form.Form;
								if (String.IsNullOrEmpty(gloss))
								{
									gloss = null;
								}
								tokenFieldsAndValues.Add("Gloss", gloss);

								string glossWritingSystem = form.WritingSystemId;
								if (String.IsNullOrEmpty(glossWritingSystem))
								{
									glossWritingSystem = null;
								}
								tokenFieldsAndValues.Add("GlossWritingSystem", glossWritingSystem);
								tokenFieldsAndValues.Add("SenseNumber", senseNumber);
								fieldsandValuesForRecordTokens.Add(tokenFieldsAndValues);
							}
							senseNumber++;
						}
						return fieldsandValuesForRecordTokens;
					}
					);
				ResultSet<LexEntry> itemsMatchingQuery = GetItemsMatching(MatchingGlossQuery);
				SortDefinition[] sortDefinition = new SortDefinition[4];
				sortDefinition[0] = new SortDefinition("Form", lexicalUnitWritingSystem.Collator);
				sortDefinition[1] = new SortDefinition("Gloss", StringComparer.InvariantCulture);
				sortDefinition[2] = new SortDefinition("GlossWritingSystem", StringComparer.InvariantCulture);
				sortDefinition[3] = new SortDefinition("SenseNumber", Comparer<int>.Default);
				ResultSetCache<LexEntry> cache =
					new ResultSetCache<LexEntry>(this, sortDefinition, itemsMatchingQuery, MatchingGlossQuery);
				_caches.Add(cachename, cache);
			}
			return _caches[cachename].GetResultSet();
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
				 LanguageForm glossForm, WritingSystemDefinition lexicalUnitWritingSystem)
		{

			if (null==glossForm || string.IsNullOrEmpty(glossForm.Form))
			{
				throw new ArgumentNullException("glossForm");
			}
			if (lexicalUnitWritingSystem == null)
			{
				throw new ArgumentNullException("lexicalUnitWritingSystem");
			}
			ResultSet<LexEntry> allGlossesResultSet = GetAllEntriesWithGlossesSortedByLexicalForm(lexicalUnitWritingSystem);
			List<RecordToken<LexEntry>> filteredResultSet = new List<RecordToken<LexEntry>>();
			foreach (RecordToken<LexEntry> recordToken in allGlossesResultSet)
			{
				if (((string) recordToken["Gloss"] == glossForm.Form)
					&& ((string) recordToken["GlossWritingSystem"] == glossForm.WritingSystemId))
				{
					filteredResultSet.Add(recordToken);
				}
			}

			return new ResultSet<LexEntry>(this, filteredResultSet);
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
			ResultSet<LexEntry> allGlossesResultSet = GetAllEntriesSortedById();
			List<RecordToken<LexEntry>> filteredResultSet = new List<RecordToken<LexEntry>>();
			foreach (RecordToken<LexEntry> recordToken in allGlossesResultSet)
			{
				if (((string)recordToken["Id"] == id))
				{
					filteredResultSet.Add(recordToken);
				}
			}
			if (filteredResultSet.Count > 1)
			{
				throw new ApplicationException("More than one entry exists with the guid " + id);
			}
			if (filteredResultSet.Count == 0)
			{
				return null;
			}
			return filteredResultSet[0].RealObject;
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
			ResultSet<LexEntry> allGlossesResultSet = GetAllEntriesSortedByGuid();
			List<RecordToken<LexEntry>> filteredResultSet = new List<RecordToken<LexEntry>>();
			foreach (RecordToken<LexEntry> recordToken in allGlossesResultSet)
			{
				if (((Guid) recordToken["Guid"] == guid))
				{
					filteredResultSet.Add(recordToken);
				}
			}
			if(filteredResultSet.Count > 1)
			{
				throw new ApplicationException("More than one entry exists with the guid " + guid);
			}
			if(filteredResultSet.Count == 0)
			{
				return null;
			}
			return filteredResultSet[0].RealObject;
		}

		/// <summary>
		/// Gets a ResultSet containing entries whose lexical form is similar to lexicalForm
		/// sorted by the lexical form in the given writingsystem.
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithSimilarLexicalForm(string lexicalForm,
																	WritingSystemDefinition writingSystem,
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
																	 WritingSystemDefinition writingSystem)
		{
			if (lexicalForm == null)
			{
				throw new ArgumentNullException("lexicalForm");
			}
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}
			ResultSet<LexEntry> allGlossesResultSet = GetAllEntriesSortedByLexicalForm(writingSystem);
			List<RecordToken<LexEntry>> filteredResultSet = new List<RecordToken<LexEntry>>();
			foreach (RecordToken<LexEntry> recordToken in allGlossesResultSet)
			{
				if (((string)recordToken["Form"] == lexicalForm))
				{
					filteredResultSet.Add(recordToken);
				}
			}

			return new ResultSet<LexEntry>(this, filteredResultSet);
		}

		/// <summary>
		/// Gets a ResultSet containing entries that are missing the field matching field
		/// sorted by the lexical form in the given writing system.
		/// Use "Form" to access the lexical form in a RecordToken.
		/// </summary>
		/// <returns></returns>
		public ResultSet<LexEntry> GetEntriesWithMissingFieldSortedByLexicalUnit(Field field, string[] searchWritingSystemIds, WritingSystemDefinition lexicalUnitWritingSystem)
		{
			 var query = new MissingFieldQuery(field, searchWritingSystemIds, null);
			return GetEntriesWithMissingFieldSortedByLexicalUnit(query, field, lexicalUnitWritingSystem);
	  }

		public ResultSet<LexEntry> GetEntriesWithMissingFieldSortedByLexicalUnit(MissingFieldQuery query, Field field, WritingSystemDefinition lexicalUnitWritingSystem)
		{
			Guard.AgainstNull(lexicalUnitWritingSystem, "lexicalUnitWritingSystem");
			Guard.AgainstNull(field, "field");
			Guard.AgainstNull(query, "query");

			string cacheName = String.Format("missingFieldsSortedByLexicalForm_{0}_{1}_{2}", field, lexicalUnitWritingSystem.Id, query.UniqueCacheId);
			//cacheName = MakeSafeForFileName(cacheName);
			if (_caches[cacheName] == null)
			{
				DelegateQuery<LexEntry> lexicalFormQuery = new DelegateQuery<LexEntry>(
					delegate(LexEntry entryToQuery)
					{
						IDictionary<string, object> tokenFieldsAndValues = new Dictionary<string, object>();
						if(query.FilteringPredicate(entryToQuery))
						{
							string lexicalForm = null;
							if (!String.IsNullOrEmpty(entryToQuery.LexicalForm[lexicalUnitWritingSystem.Id]))
							{
								lexicalForm = entryToQuery.LexicalForm[lexicalUnitWritingSystem.Id];
							}
							tokenFieldsAndValues.Add("Form", lexicalForm);
							return new IDictionary<string, object>[] { tokenFieldsAndValues };
						}
						return new IDictionary<string, object>[0];
					});
				ResultSet<LexEntry> itemsMatching = _decoratedDataMapper.GetItemsMatching(lexicalFormQuery);

				SortDefinition[] sortOrder = new SortDefinition[1];
				sortOrder[0] = new SortDefinition("Form", lexicalUnitWritingSystem.Collator);

				_caches.Add(cacheName, new ResultSetCache<LexEntry>(this, sortOrder, itemsMatching, lexicalFormQuery));
			}
			ResultSet<LexEntry> resultsFromCache = _caches[cacheName].GetResultSet();

			return resultsFromCache;
		}


//
//        private string MakeSafeForFileName(string fileName)
//        {
//            foreach (char invalChar in Path.GetInvalidFileNameChars())
//            {
//                fileName = fileName.Replace(invalChar.ToString(), "");
//            }
//            return fileName;
//        }

		#region IDisposable Members

#if DEBUG
		~LexEntryRepository()
		{
			if (!_disposed)
			{
				throw new ApplicationException(
					"Disposed not explicitly called on LexEntryRepository." + "\n" + _constructionStackTrace
				);
			}
		}
#endif

		private bool _disposed = true;

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
					_decoratedDataMapper.Dispose();
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

		public int Count
		{
			get { return CountAllItems(); }
		}
	}
}
