using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;

namespace WeSay.LexicalModel.Db4o_Specific
{
	/// <summary>
	/// Use this class to rapidly get a list of approximate matches of Lexical Forms.
	/// You need a different instance of this for each writing system you want to search on.
	/// </summary>
	public class ApproximateFinder
	{
		private LexEntrySortHelper _sortHelper;
		private ReadOnlyCollection<KeyValuePair<string, long>> _keyIdMap;
		private CachedSortedDb4oList<string, LexEntry> _pairStringLexEntryIdList;

		public ApproximateFinder(Db4oRecordListManager recordListManager, WritingSystem writingSystem)
		{
			_sortHelper = new LexEntrySortHelper(recordListManager.DataSource,
												 writingSystem,
												 true /*IsWritingSystemUsedInLexicalForm*/
				);
			_pairStringLexEntryIdList = recordListManager.GetSortedList(_sortHelper);
			_keyIdMap = _pairStringLexEntryIdList.KeyIdMap;
		}

		private string GetFormForMatching(object item)
		{
			KeyValuePair<string, long> kv = (KeyValuePair<string, long>)item;
			return kv.Key;
		}

		/// <summary>
		/// Returns the Id property of each matching entry.
		/// </summary>
		/// <param name="form"></param>
		/// <param name="matcherOptions"></param>
		/// <returns></returns>
		public List<string> FindIds(string form, ApproximateMatcherOptions matcherOptions)
		{
			List<LexEntry> entries = FindEntries(form,matcherOptions, int.MaxValue);
			return entries.ConvertAll(new Converter<LexEntry, string>(
				delegate(LexEntry e) { return e.GetOrCreateId(true); }));
		}

		public List<LexEntry> FindEntries(string form, ApproximateMatcherOptions matcherOptions, int maximumNumberToRetrieve)
		{

			IList<object> pairs = GetKeyIdPairs(form, matcherOptions);
			List<LexEntry> matches = new List<LexEntry>(pairs.Count);
			foreach (KeyValuePair<string, long> pair in pairs)
			{
				maximumNumberToRetrieve--;
				if (maximumNumberToRetrieve < 0)
				{
					break;
				}
				if (!pair.Key.EndsWith("*"))//be strict about entries added because of other writing-systems/glosses
				{
					LexEntry entry = _pairStringLexEntryIdList.GetValueFromId(pair.Value);
					matches.Add(entry);
				}
			}
			return matches;
		}

		private IList<object> GetKeyIdPairs(string form, ApproximateMatcherOptions matcherOptions)
		{
			return ApproximateMatcher.FindClosestForms<object>(_keyIdMap,
															   GetFormForMatching,
															   form,
															   matcherOptions);
		}

		public List<string> FindForms(string form, ApproximateMatcherOptions matcherOptions)
		{
			IList<object> pairs = GetKeyIdPairs(form, matcherOptions);
			List<string> matches = new List<string>(pairs.Count);

			foreach (KeyValuePair<string, long> pair in pairs)
			{
				matches.Add(pair.Key);
			}
			return matches;
		}
	}
}