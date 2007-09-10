using WeSay.Data;

namespace WeSay.LexicalModel
{

	public class Lexicon
	{
		 private static Db4oRecordListManager _recordListManager;

		public IRecordList<LexEntry> Entries
		{
			get
			{
				return _recordListManager.GetListOfType<LexEntry>();
			}
		}

		static public void Init(Db4oRecordListManager recordListManager)
		{
			_recordListManager = recordListManager;
		}

		public static LexEntry FindFirstLexEntryMatchingId(string id)
		{
			return WeSay.LexicalModel.Db4o_Specific.Db4oLexQueryHelper.FindFirstEntryMatchingId(_recordListManager.DataSource, id);
		}

		public static LexEntry AddNewEntry()
		{
			return  (LexEntry)_recordListManager.GetListOfType<LexEntry>().AddNew();
		}
	}
}
