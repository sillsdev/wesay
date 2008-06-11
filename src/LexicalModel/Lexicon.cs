using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel
{

	public class Lexicon
	{
		 private static Db4oRecordListManager _recordListManager;

		public static IRecordList<LexEntry> Entries
		{
			get
			{
				VerifyInitialized();
				return RecordListManager.GetListOfType<LexEntry>();
			}
		}

		public static Db4oRecordListManager RecordListManager
		{
			get { return _recordListManager; }
			set { _recordListManager = value; }
		}

		static public void Init(Db4oRecordListManager recordListManager)
		{
			RecordListManager = recordListManager;
		}


		public static IList<LexEntry> GetEntriesWithSimilarLexicalForms(string wordForm, WritingSystem writingSystem, ApproximateMatcherOptions matcherOptions, int maxEntriesToGet)
		{
			ApproximateFinder findList = new ApproximateFinder(RecordListManager, writingSystem);
			return findList.FindEntries(wordForm, matcherOptions, maxEntriesToGet);
		}

		public static List<LexEntry> GetEntriesHavingLexicalForm(string lexicalForm,  WritingSystem writingSystem)
		{
			VerifyInitialized();
			// search dictionary for entry with new lexical form
			LexEntrySortHelper sortHelper = new LexEntrySortHelper(RecordListManager.DataSource,
																   writingSystem,
																   true);

			// This should probably be optimized by using a specific query
			List<RecordToken> entriesByLexicalForm = RecordListManager.GetSortedList(sortHelper);
			IComparer<string> stringComparer = sortHelper.KeyComparer;
			List<LexEntry> result = new List<LexEntry>();
			RecordTokenComparer comparer = new RecordTokenComparer(stringComparer);
			comparer.IgnoreId = true;
			RecordToken searchToken = new RecordToken(lexicalForm, 0);
			int index = entriesByLexicalForm.BinarySearch(searchToken,comparer);
			while (index >= 0 && index < entriesByLexicalForm.Count &&
				   entriesByLexicalForm[index].DisplayString == lexicalForm)
			{
				long id = entriesByLexicalForm[index].Id;
				result.Add(RecordListManager.GetItem<LexEntry>(id));
				++index;
			}
			return result;
		}

		public static IEnumerable<LexEntry> GetAllEntriesSortedByHeadword(WritingSystem headwordWritingSystem)
		{
			VerifyInitialized();
			HeadwordSortedListHelper sortHelper = new HeadwordSortedListHelper(RecordListManager,
																			   headwordWritingSystem);
			IList<RecordToken> entryPairs = RecordListManager.GetSortedList(sortHelper);
			List<LexEntry> result = new List<LexEntry>();
			foreach (RecordToken recordToken in entryPairs)
			{
				result.Add(RecordListManager.GetItem<LexEntry>(recordToken.Id));
			}
			return result;
		}

		public static LexEntry FindFirstLexEntryMatchingId(string id)
		{
			VerifyInitialized();
			return WeSay.LexicalModel.Db4o_Specific.Db4oLexQueryHelper.FindFirstEntryMatchingId(RecordListManager.DataSource, id);
		}

		public static LexEntry AddNewEntry()
		{
			VerifyInitialized();
			return (LexEntry)Entries.AddNew();
		}



		private static void VerifyInitialized()
		{
			if (RecordListManager == null)
			{
				throw new InvalidOperationException("Must Call Init before calling this method");
			}
		}

		public static void RemoveEntry(LexEntry entry)
		{
			VerifyInitialized();
			Entries.Remove(entry);
		}




		public static void RegisterFieldWithCache(IList<WritingSystem> writingSystems, bool areLexicalFormWs)
		{
				foreach (WritingSystem writingSystem in writingSystems)
				{
					LexEntrySortHelper sortHelper =
							new LexEntrySortHelper(RecordListManager.DataSource,
												   writingSystem,
												   areLexicalFormWs);
					RecordListManager.GetSortedList(sortHelper);
				}
		}

		public static void RegisterHeadwordListWithCache(IList<WritingSystem> headwordWritingSystems)
		{
			foreach (WritingSystem writingSystem in headwordWritingSystems)
			{
				HeadwordSortedListHelper helper = new HeadwordSortedListHelper(RecordListManager,writingSystem);
				RecordListManager.GetSortedList(helper);//installs it
			}
		}

		public static void DeInitialize(bool doDisposeLikeYouOwnEverything)
		{
			if (doDisposeLikeYouOwnEverything)
			{
				Db4oDataSource db = _recordListManager.DataSource;
				if(!_recordListManager.IsDisposed)
				{
					_recordListManager.Dispose();
				}
				if (db != null)
				{
					db.Dispose();
				}

			}
			_recordListManager = null;
		}
	}
}
