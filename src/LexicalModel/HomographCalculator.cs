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
		private readonly IRecordList<LexEntry> _records;
		private IList<RecordToken> _entryIdsSortedByHeadword;
		private Db4oRecordListManager _recordListManager;

		public HomographCalculator(Db4oRecordListManager recordListManager, WritingSystem headwordWritingSystem)
		{
			_headwordWritingSystem = headwordWritingSystem;

			this._recordListManager = recordListManager;
			HeadwordSortedListHelper helper = new HeadwordSortedListHelper(recordListManager,
										  headwordWritingSystem);
			_entryIdsSortedByHeadword = recordListManager.GetSortedList(helper);

			_records = recordListManager.GetListOfType<LexEntry>();
		}

		#region IHomographCalculator Members



		public int GetHomographNumber(LexEntry entry)
		{
			long databaseIdOfEntry = _records.GetId(entry);
			// find our position within the sorted list of entries
			int ourIndex = -1;
			for(int i = 0; i != _entryIdsSortedByHeadword.Count; ++i)
			{
				if(_entryIdsSortedByHeadword[i].Id == databaseIdOfEntry)
				{
					ourIndex = i;
					break;
				}
			}
			string headword = entry.GetHeadWordForm(_headwordWritingSystem.Id);


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
				long searchId = _entryIdsSortedByHeadword[searchIndex].Id;
				LexEntry previousGuy = _recordListManager.GetItem<LexEntry>(searchId);

				if (headword != previousGuy.GetHeadWordForm(_headwordWritingSystem.Id))
				{
					break;
				}
				++found;
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
