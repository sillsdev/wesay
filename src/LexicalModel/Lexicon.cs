using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
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

//        public LexEntry FindFirstEntryWithLexemeForm(string id, string writingSystemId)
//        {
//            List<KeyValuePair<string, long>> pairs =Db4o_Specific.KeyToEntryIdInitializer.GetLexicalFormToEntryIdPairs(_recordListManager.DataSource, writingSystemId);
//
//            foreach (KeyValuePair<string, long> pair in pairs)
//            {
//
//            }
//            return null;
//        }

		public static LexEntry FindFirstLexEntryMatchingId(string id)
		{
			return WeSay.LexicalModel.Db4o_Specific.Db4oLexQueryHelper.FindFirstEntryMatchingId(_recordListManager.DataSource, id);

			//TODO: use search
//            foreach (LexEntry entry in Entries)
//            {
//                if (entry.Id == id)
//                {
//                    return entry;
//                }
//            }
//            return null;
		}

		public static LexEntry AddNewEntry()
		{
			return  (LexEntry)_recordListManager.GetListOfType<LexEntry>().AddNew();
		}
	}
}
