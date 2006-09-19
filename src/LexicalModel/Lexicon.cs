using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WeSay.LexicalModel
{
	public class Lexicon
	{
		WeSay.Data.Db4oBindingList<LexEntry> _entries;

		public IBindingList Entries
		{
			get
			{
				return _entries;
			}
		}

		public Lexicon(WeSay.Data.Db4oDataSource dataSource, IFilter filter)
		{
			_entries = new WeSay.Data.Db4oBindingList<LexEntry>(dataSource);//, filter.SODAQuery);
		}



	}
}
