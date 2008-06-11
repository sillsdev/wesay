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
using WeSay.Language;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel
{

	public class LexEntryRepository: IDisposable
	{
		public LexEntryRepository(string path)
		{
			_recordListManager = new PrivateDb4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), path);
			Db4oLexModelHelper.Initialize(_recordListManager.DataSource.Data);
		}

		private readonly PrivateDb4oRecordListManager _recordListManager;

		//todo make this private and remove it.
		public Db4oDataSource Db4oDataSource
		{
			get { return _recordListManager.DataSource; }
		}

		public RecordToken GetRecordToken(LexEntry item, ISortHelper<LexEntry> sortHelper)
		{
			Db4oRepositoryId id = (Db4oRepositoryId) GetId(item);
			foreach (string displayString in sortHelper.GetDisplayStrings(item))
			{
				return new RecordToken(displayString, id);
			}
			return new RecordToken(string.Empty, id);
		}

		public LexEntry CreateItem()
		{
			LexEntry item = new LexEntry(null, new Guid(), GetNextBirthOrder());
			IRecordList<LexEntry> type = _recordListManager.GetListOfType<LexEntry>();
			type.Add(item);
			return item;
		}

		public LexEntry CreateItem(Extensible eInfo)
		{
			LexEntry item = new LexEntry(eInfo, GetNextBirthOrder());
			IRecordList<LexEntry> type = _recordListManager.GetListOfType<LexEntry>();
			type.Add(item);
			return item;
		}

		private int GetNextBirthOrder()
		{
			IHistoricalEntryCountProvider entryCountProvider = HistoricalEntryCountProviderForDb4o.GetOrMakeFromDatabase(this._recordListManager.DataSource);
			return entryCountProvider.GetNextNumber();
		}

		public RepositoryId GetId(LexEntry item)
		{
			long id = _recordListManager.DataSource.Data.Ext().GetID(item);
			return new Db4oRepositoryId(id);
		}

		public LexEntry GetItem(RepositoryId id)
		{
			return _recordListManager.GetItem<LexEntry>(((Db4oRepositoryId)id).Db4oId);
		}

		public LexEntry GetItem(RecordToken recordToken)
		{
			return GetItem(recordToken.Id);
		}

		public void SaveItems(IEnumerable<LexEntry> items)
		{
			foreach (LexEntry item in items)
			{
				_recordListManager.DataSource.Data.Set(item);
			}
			_recordListManager.DataSource.Data.Commit();
		}

		public void SaveItem(LexEntry item)
		{
			_recordListManager.DataSource.Data.Set(item);
			_recordListManager.DataSource.Data.Commit();
		}

		public void DeleteItem(LexEntry item)
		{
			DeleteItem(GetId(item));
		}

		public void DeleteItem(RepositoryId repositoryId)
		{
			IRecordList<LexEntry> type = _recordListManager.GetListOfType<LexEntry>();
			type.Remove(GetItem(repositoryId));
		}


		public IList<RecordToken> GetEntriesWithSimilarLexicalForm(
			string lexicalForm,
			WritingSystem writingSystem,
			ApproximateMatcherOptions matcherOptions)
		{
			LexEntrySortHelper sortHelper =
					new LexEntrySortHelper(this,
										   writingSystem,
										   true /*IsWritingSystemUsedInLexicalForm*/);
			List<RecordToken> recordTokens = this._recordListManager.GetSortedList(sortHelper);

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

			return  ApproximateMatcher.FindClosestForms<RecordToken>(recordTokens,
													GetFormForMatchingStrategy,
													lexicalForm,
													matcherOptions);
		}

		private static string GetFormForMatchingStrategy(object item)
		{
			return ((RecordToken)item).DisplayString;
		}

		public List<RecordToken> GetEntriesWithMatchingLexicalForm(
			string lexicalForm,
			WritingSystem writingSystem)
		{
			// search dictionary for entry with new lexical form
			LexEntrySortHelper sortHelper = new LexEntrySortHelper(this,
																   writingSystem,
																   true);

			// This should probably be optimized by using a specific query
			List<RecordToken> entriesByLexicalForm = _recordListManager.GetSortedList(sortHelper);
			IComparer<string> stringComparer = sortHelper.KeyComparer;
			List<RecordToken> result = new List<RecordToken>();
			RecordTokenComparer comparer = new RecordTokenComparer(stringComparer);
			comparer.IgnoreId = true;
			RecordToken searchToken = new RecordToken(lexicalForm, RepositoryId.Empty);
			int index = entriesByLexicalForm.BinarySearch(searchToken, comparer);
			while (index >= 0 && index < entriesByLexicalForm.Count &&
				   entriesByLexicalForm[index].DisplayString == lexicalForm)
			{
				result.Add(entriesByLexicalForm[index]);
				++index;
			}
			return result;
		}
		public RecordTokenComparer GetRecordTokenComparerForLexicalForm(WritingSystem writingSystem)
		{
			LexEntrySortHelper sortHelper = new LexEntrySortHelper(this,
																   writingSystem,
																   true);
			return new RecordTokenComparer(sortHelper.KeyComparer);
		}

		public IList<RecordToken> GetAllEntriesSortedByHeadword(WritingSystem headwordWritingSystem)
		{
			HeadwordSortedListHelper sortHelper = new HeadwordSortedListHelper(_recordListManager.DataSource,
																			   headwordWritingSystem);
			return _recordListManager.GetSortedList(sortHelper);
		}

		public LexEntry GetLexEntryWithMatchingId(string id)
		{
			IQuery q = _recordListManager.DataSource.Data.Query();
			q.Constrain(typeof(LexEntry));
			q.Descend("_id").Constrain(id);
			IObjectSet matches = q.Execute();
			if (matches.Count == 0)
			{
				return null;
			}
			if (matches.Count > 1)//review: not sure if we should throw or not
			{
				throw new ApplicationException(String.Format("There were {0} objects found with the id {1}", matches.Count, id));
			}
			System.Diagnostics.Debug.Assert(matches[0].GetType() == typeof(LexEntry));
			return (LexEntry)matches[0];
		}


		public int GetHomographNumber(LexEntry entry, WritingSystem headwordWritingSystem)
		{
			HeadwordSortedListHelper helper = new HeadwordSortedListHelper(_recordListManager.DataSource,
										  headwordWritingSystem);
			IList<RecordToken> recordTokensSortedByHeadWord = _recordListManager.GetSortedList(helper);
			RepositoryId databaseIdOfEntry = GetId(entry);
			// find our position within the sorted list of entries
			int ourIndex = -1;
			for (int i = 0; i != recordTokensSortedByHeadWord.Count; ++i)
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

			for (int searchIndex = ourIndex - 1; searchIndex > -1; --searchIndex)
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
			if (!this._disposed)
			{
				throw new ApplicationException("Disposed not explicitly called on LexEntryRepository.");
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
			if (!this._disposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic
					_recordListManager.Dispose();
				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("LexEntryRepository");
			}
		}
		#endregion

		public List<RecordToken> GetAllEntriesSortedByLexicalForm(WritingSystem writingSystem)
		{
			List<RecordToken> list = KeyToEntryIdInitializer.GetLexicalFormToEntryIdPairs(
										this._recordListManager.DataSource,
										writingSystem.Id);

			list.Sort(new RecordTokenComparer(writingSystem));
			return list;
		}
		internal RepositoryId[] GetAllEntries()
		{
			GenericObjectSetFacade<LexEntry> items = (GenericObjectSetFacade<LexEntry>) this._recordListManager.DataSource.Data.Query<LexEntry>();
			long[] db4oIds = items._delegate.GetIDs();
			return WrapDb4oIdsInRepositoryIds(db4oIds);
		}

		private static RepositoryId[] WrapDb4oIdsInRepositoryIds(long[] db4oIds) {
			RepositoryId[] ids = new RepositoryId[db4oIds.Length];
			for (int i = 0; i != db4oIds.Length;++i)
			{
				ids[i] = new Db4oRepositoryId(db4oIds[i]);
			}
			return ids;
		}

		public List<RecordToken> GetAllEntriesSortedBySemanticDomain(string fieldName)
		{
			SemanticDomainSortHelper sortHelper = new SemanticDomainSortHelper(_recordListManager.DataSource, fieldName);
			return _recordListManager.GetSortedList(sortHelper);
		}
		public RecordTokenComparer GetRecordTokenComparerForSemanticDomain(string fieldName)
		{
			SemanticDomainSortHelper sortHelper = new SemanticDomainSortHelper(_recordListManager.DataSource, fieldName);
			return new RecordTokenComparer(sortHelper.KeyComparer);
		}


		public List<RecordToken> GetEntriesWithMatchingGlossSortedByLexicalForm(LanguageForm glossForm, WritingSystem lexicalUnitWritingSystem)
		{
			LexEntrySortHelper lexEntrySortHelper = new LexEntrySortHelper(lexicalUnitWritingSystem, true);
			List<RecordToken> matches = new List<RecordToken>();
			RepositoryId[] repositoryIds = GetAllEntries();
			foreach (RepositoryId repositoryId in repositoryIds)
			{
				LexEntry entry = GetItem(repositoryId);
				foreach (LexSense sense in entry.Senses)
				{
					if (sense.Gloss[glossForm.WritingSystemId] == glossForm.Form)
					{
						foreach (string displayString in lexEntrySortHelper.GetDisplayStrings(entry))
						{
							matches.Add(new RecordToken(displayString, repositoryId));
						}
					}
				}
			}
			return matches;
		}
		public LexEntry GetLexEntryWithMatchingGuid(Guid guid)
		{
			IQuery q = _recordListManager.DataSource.Data.Query();
			q.Constrain(typeof(LexEntry));
			q.Descend("_guid").Constrain(guid);
			IObjectSet matches = q.Execute();
			if (matches.Count == 0)
			{
				return null;
			}
			if (matches.Count > 1)
			{
				throw new ApplicationException(String.Format("There were {0} objects found with the guid {1}", matches.Count, guid));
			}
			System.Diagnostics.Debug.Assert(matches[0].GetType() == typeof(LexEntry));
			return (LexEntry)matches[0];
		}

		public IList<RecordToken> GetAllEntriesSortedByGloss(WritingSystem writingSystem)
		{
			LexEntrySortHelper sortHelper =
					new LexEntrySortHelper(this,
										   writingSystem,
										   false /*IsWritingSystemUsedInLexicalForm*/);
			return this._recordListManager.GetSortedList(sortHelper);
		}

		public RepositoryId[] GetEntriesUpdatedSince(DateTime last)
		{
			// by moving back 1 milliseconds, we ensure that we
			// will get the correct records with just a > and not >=
			last = last.AddMilliseconds(-1);
			IQuery q = _recordListManager.DataSource.Data.Query();
			q.Constrain(typeof(LexEntry));
			q.Descend("_modificationTime").Constrain(last).Greater();
			IObjectSet objectSet = q.Execute();
			return WrapDb4oIdsInRepositoryIds(objectSet.Ext().GetIDs());
		}

		public int CountAllEntries()
		{
			return GetAllEntries().Length;
		}

		public IRecordList<LexEntry> GetEntriesMatchingFilterSortedByLexicalUnit(IFilter<LexEntry> filter, WritingSystem lexicalUnitWritingSystem)
		{
			LexEntrySortHelper lexEntrySortHelper = new LexEntrySortHelper(lexicalUnitWritingSystem, true);
			return _recordListManager.GetListOfTypeFilteredFurther(filter, lexEntrySortHelper);
		}

	}

	internal class SemanticDomainSortHelper : ISortHelper<LexEntry>
	{
		private readonly Db4oDataSource _db4oData;
		private readonly string _semanticDomainFieldName;

		public SemanticDomainSortHelper(Db4oDataSource db4oData, string semanticDomainFieldName)
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
			_semanticDomainFieldName = semanticDomainFieldName;
		}

		#region IDb4oSortHelper<string,LexEntry> Members

		public IComparer<string> KeyComparer
		{
			get
			{
				return StringComparer.InvariantCulture;
			}
		}

		public List<RecordToken> GetRecordTokensForMatchingRecords()
		{
			return KeyToEntryIdInitializer.GetKeyToEntryIdPairs(_db4oData, GetDisplayStrings);
		}

		public IEnumerable<string> GetDisplayStrings(LexEntry item)
		{
			List<string> keys = new List<string>();
			foreach (LexSense sense in item.Senses)
			{
				OptionRefCollection semanticDomains = sense.GetProperty<OptionRefCollection>(_semanticDomainFieldName);

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
			return keys;
		}

		public string Name
		{
			get
			{
				return "LexEntry sorted by " + _semanticDomainFieldName;
			}
		}

		public override int GetHashCode()
		{
			return _semanticDomainFieldName.GetHashCode();
		}
		#endregion
	}

}
