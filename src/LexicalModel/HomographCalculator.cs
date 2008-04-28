using System;
using System.Collections.Generic;
using System.Text;
using Palaso.Text;
using WeSay.Data;
using WeSay.Language;

namespace WeSay.LexicalModel
{

	public interface IHomographCalculator
	{
		/// <summary>
		/// returns 0 if no homograph # needed
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		int GetHomographNumber(LexEntry entry);
	}

	public class HomographCalculator : IHomographCalculator
	{
		private readonly WritingSystem _headwordWritingSystem;
		private readonly Db4oRecordList<LexEntry> _records;
		private CachedSortedDb4oList<string, LexEntry> _entryIdsSortedByHeadword;

		public HomographCalculator(Db4oRecordListManager recordListManager, WritingSystem headwordWritingSystem)
		{
			_headwordWritingSystem = headwordWritingSystem;

			HeadwordSortedListHelper helper = new HeadwordSortedListHelper(recordListManager,
										  headwordWritingSystem);
			_entryIdsSortedByHeadword = recordListManager.GetSortedList(helper);

			_records = (Db4oRecordList<LexEntry>) recordListManager.GetListOfType<LexEntry>();
		}

		#region IHomographCalculator Members



		public int GetHomographNumber(LexEntry entry)
		{
			long databaseIdOfEntry = _records.GetId(entry);
			int ourIndex = _entryIdsSortedByHeadword.GetIds().IndexOf(databaseIdOfEntry);
			string headword = entry.GetHeadWordForm(_headwordWritingSystem.Id);


			//todo: this is bogus; it fullfills our round-tripping requirement, but would
			//give us bogus homograph numbers

			if (entry.OrderForRoundTripping > 0)
			{
				return entry.OrderForRoundTripping;
			}

			//what number are we?
			int found = 0;
			int searchIndex = ourIndex - 1;
			while (searchIndex > -1)
			{
				LexEntry previousGuy = _entryIdsSortedByHeadword.GetValue(searchIndex);
				if (headword != previousGuy.GetHeadWordForm(_headwordWritingSystem.Id))
				{
					break;
				}
				++found;
				--searchIndex;
			}

			// if we're the first with this headword
			if (found == 0)
			{
				//and we're the last entry
				if (ourIndex + 1 >= _entryIdsSortedByHeadword.Count)
				{
					return 0; //no homograph number
				}

				LexEntry nextGuy = _entryIdsSortedByHeadword.GetValue(ourIndex +1);
				// the next guy doesn't match
				if (headword != nextGuy.GetHeadWordForm(_headwordWritingSystem.Id))
				{
					return 0; //no homograph number
				}
				else
				{
					return 1;
				}
			}
			//there were preceding homographs
			return 1+found;

			//todo: look at @order and the planned-for order-in-lift field on LexEntry
		}

		#endregion
	}
}
