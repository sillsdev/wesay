using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WeSay.LexicalModel
{
	public class Lexicon
	{
		WeSay.Data.Db4oRecordList<LexEntry> _entries;

		public IBindingList Entries
		{
			get
			{
				return _entries;
			}
		}

		public Lexicon(WeSay.Data.Db4oDataSource dataSource, WeSay.Data.IFilter<LexEntry> filter)
		{
			_entries = new WeSay.Data.Db4oRecordList<LexEntry>(dataSource);//, filter.SODAQuery);
		}



	}
}
