using System;
using System.Collections.Generic;
using System.Text;
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

			HeadwordSortedListHelper helper = new HeadwordSortedListHelper(recordListManager.DataSource,
										  headwordWritingSystem);
			_entryIdsSortedByHeadword = recordListManager.GetSortedList(helper);

			_records = (Db4oRecordList<LexEntry>) recordListManager.GetListOfType<LexEntry>();
		}

		#region IHomographCalculator Members

		public int GetHomographNumber(LexEntry entry)
		{
			long databaseIdOfEntry = _records.GetId(entry);
			int index = _entryIdsSortedByHeadword.GetIds().IndexOf(databaseIdOfEntry);
			int searchIndex = index - 1;
			string headword = entry.GetHeadWord(_headwordWritingSystem.Id);
			while (searchIndex > -1)
			{
				LexEntry neighbor = _entryIdsSortedByHeadword.GetValue(searchIndex);
				string theirHeadword = neighbor.GetHeadWord(_headwordWritingSystem.Id);
			}

			return 0;
		}



		#endregion
	}
}
