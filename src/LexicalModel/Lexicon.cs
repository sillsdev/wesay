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
				return _recordListManager.GetListOfType<LexEntry>();
			}
		}

		static public void Init(Db4oRecordListManager recordListManager)
		{
			_recordListManager = recordListManager;
		}


		public static IList<LexEntry> GetEntriesWithSimilarLexicalForms(string wordForm, WritingSystem writingSystem, ApproximateMatcherOptions matcherOptions, int maxEntriesToGet)
		{
			ApproximateFinder findList = new ApproximateFinder(_recordListManager, writingSystem);
			return findList.FindEntries(wordForm, matcherOptions, maxEntriesToGet);
		}

		public static List<LexEntry> GetEntriesHavingLexicalForm(string lexicalForm,  WritingSystem writingSystem)
		{
			VerifyInitialized();
			List<LexEntry> result = new List<LexEntry>();
			// search dictionary for entry with new lexical form
			LexEntrySortHelper sortHelper = new LexEntrySortHelper(_recordListManager.DataSource,
																   writingSystem,
																   true);
			CachedSortedDb4oList<string, LexEntry> entriesByLexicalForm = _recordListManager.GetSortedList(sortHelper);
			int index = entriesByLexicalForm.BinarySearch(lexicalForm);
			while (index >= 0 && index < entriesByLexicalForm.Count &&
				   entriesByLexicalForm.GetKey(index) == lexicalForm)
			{
				result.Add(entriesByLexicalForm.GetValue(index));
				++index;
			}
			return result;
		}
		public static LexEntry FindFirstLexEntryMatchingId(string id)
		{
			VerifyInitialized();
			return WeSay.LexicalModel.Db4o_Specific.Db4oLexQueryHelper.FindFirstEntryMatchingId(_recordListManager.DataSource, id);
		}

		public static LexEntry AddNewEntry()
		{
			VerifyInitialized();
			return (LexEntry)Entries.AddNew();
		}



		private static void VerifyInitialized()
		{
			if (_recordListManager == null)
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
							new LexEntrySortHelper(_recordListManager.DataSource,
												   writingSystem,
												   areLexicalFormWs);
					_recordListManager.GetSortedList(sortHelper);
				}
		}

	}
}
